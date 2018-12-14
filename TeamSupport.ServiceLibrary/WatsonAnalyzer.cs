using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net.Http.Formatting;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Xml;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Specialized;
using System.Data.Linq;
using System.Threading;

namespace WatsonToneAnalyzer
{
    /// <summary>
    /// Class for the watson service to get the actions to analyze, send
    /// them to Watson, and to insert the results back into the DB
    /// </summary>
    public class WatsonAnalyzer
    {
        static int WatsonUtterancePerAPICall = Int32.Parse(ConfigurationManager.AppSettings.Get("WatsonUtterancePerAPICall"));

        /// <summary>
        /// Get the actions to analyze (dbo.ActionToAnalyze) and post to Watson on the BlueMix account
        /// </summary>
        static public void AnalyzeActions()
        {
            // without this the HTTP message to Watson returns 405 - failure on send
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            // keep a handle to all the async transactions we start
            List<Task> asyncTransactionsInProcess = new List<Task>();

            try
            {
                //opens a sqlconnection at the specified location
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    // initialize static data
                    ToneSentiment.Initialize(db);
                    ActionToAnalyze.Initialize(db);

                    // read ActionToAnalyze
                    Table<ActionSentiment> actionSentimentTable = db.GetTable<ActionSentiment>();
                    Table<ActionToAnalyze> actionToAnalyzeTable = db.GetTable<ActionToAnalyze>();
                    IQueryable<ActionToAnalyze> actionToAnalyzeQuery = from action in actionToAnalyzeTable select action;
                    ActionToAnalyze[] actions = actionToAnalyzeQuery.ToArray();

                    // Pack the actions to analyze into watson messages
                    PackActionsToMessages(asyncTransactionsInProcess, db, actionSentimentTable, actions);

                    // ...the remainder can wait to be packed into a call when we have more
                }
            }
            catch (Exception e2)
            {
                WatsonEventLog.WriteEntry("Exception in AnalyzeActions", e2);
                Console.WriteLine(e2.ToString());
            }
            finally
            {
                // wait until all the tone chat messages have been received and recorded
                Task.WaitAll(asyncTransactionsInProcess.ToArray(), 5 * 60 * 1000);  // 5 minute timeout just in case...
            }
        }

        /// <summary> Pack Actions to Messages </summary>
        private static void PackActionsToMessages(List<Task> asyncTransactionsInProcess, DataContext db, Table<ActionSentiment> actionSentimentTable, ActionToAnalyze[] actions)
        {
            // Pack Actions into Messages
            WatsonMessage message = new WatsonMessage();
            foreach (ActionToAnalyze actionToAnalyze in actions)
            {
                // ActionSentiment already exists or emtpy?
                bool duplicateActionSentiment = actionSentimentTable.Where(u => u.ActionID == actionToAnalyze.ActionID).Any();
                if (duplicateActionSentiment)
                    WatsonEventLog.WriteEntry("duplicate ActionID in ActionSentiment table " + actionToAnalyze.ActionID, EventLogEntryType.Error);
                if (duplicateActionSentiment || (actionToAnalyze.WatsonText().Length == 0))
                {
                    actionToAnalyze.DeleteOnSubmit(db);
                    db.SubmitChanges();
                    continue;
                }

                // add action to message?
                Task task = PackActionToMessage(actionToAnalyze, ref message);
                if (task != null)
                {
                    asyncTransactionsInProcess.Add(task);   // wait for watson results
                    //break;    // uncomment to test with a single HTTP post
                }
            }

            // Send the remainder message if mostly full
            if (message.UtteranceCount > 40)
                SendMessage(ref message);
        }

        public static int MessageCount { get; private set; }
        static Task SendMessage(ref WatsonMessage message)
        {
            ++MessageCount;
            //Debug.WriteLine(String.Format("HTTP_POST {0} {1}", MessageCount, message.ToString()));
            WatsonMessage tmp = message;
            Task result = HTTP_POST(tmp);
            message = new WatsonMessage();
            return result;
        }

        /// <summary> Pack Action to Message </summary>
        private static Task PackActionToMessage(ActionToAnalyze actionToAnalyze, ref WatsonMessage message)
        {
            Task result = null;

            // Action fit into a single utterance?
            Utterance utterance;
            if (actionToAnalyze.TryGetUtterance(out utterance))
            {
                // Utterance fit into message?
                if (!message.TryAdd(actionToAnalyze, utterance))
                {
                    result = SendMessage(ref message);  // send this one
                    if (!message.TryAdd(actionToAnalyze, utterance)) // start a new message
                        Debugger.Break();
                }
            }
            else
            {
                // multiple utterances
                List<Utterance> utterances = actionToAnalyze.PackActionToUtterances();
                if (!message.TryAdd(actionToAnalyze, utterances))
                {
                    result = SendMessage(ref message);  // send this one
                    if(!message.TryAdd(actionToAnalyze, utterances)) // start a new message
                        Debugger.Break();
                }
            }

            return result;
        }

        static int _actionsAnalyzed = 0;
        public static int ActionsAnalyzed { get { return _actionsAnalyzed; } }


        static string WatsonGatewayUrl = ConfigurationManager.AppSettings.Get("WatsonGatewayUrl");
        static string WatsonUsername = ConfigurationManager.AppSettings.Get("WatsonUsername");
        static string WatsonPassword = ConfigurationManager.AppSettings.Get("WatsonPassword");

        /// <summary>
        /// Post to IBM watson with Authorization and JSON formatted utterances to process
        /// </summary>
        static async Task HTTP_POST(WatsonMessage message)
        {
            if (message.Empty)
                return;

            //Create Json Readable String with user input:        
            string result = String.Empty;
            string jsonContent = String.Empty;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // credentials
                    var Auth = WatsonUsername + ":" + WatsonPassword;
                    var byteArray = Encoding.ASCII.GetBytes(Auth);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    // JSON content
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Make Post call and await response
                    jsonContent = message.ToJSON();
                    using (var response = await client.PostAsJsonAsync(WatsonGatewayUrl, JObject.Parse(jsonContent)))
                    {
                        HttpContent content = response.Content;
                        result = await content.ReadAsStringAsync() ?? " ";
                        UtteranceToneList watsonResponse = JsonConvert.DeserializeObject<UtteranceToneList>(result);

                        // publish results to DB
                        message.PublishWatsonResponse(watsonResponse);
                        _actionsAnalyzed += message.ActionCount;
                    }
                }
            }
            catch (Exception ex)
            {
                WatsonEventLog.WriteEntry("********************: Error during watson analysis:", ex);
                if (!String.IsNullOrEmpty(jsonContent))
                    WatsonEventLog.WriteEntry(jsonContent);
                if (!String.IsNullOrEmpty(result))
                    WatsonEventLog.WriteEntry(result);
                Console.WriteLine(ex.ToString());
            }
        }
    }

    //Creates the deserialize object for Json Returning from Watson
    public class UtteranceToneList
    {
        public List<UtteranceResponse> utterances_tone { get; set; }
    }

    public class UtteranceResponse
    {
        public int utterance_id;    // index [0, 49] corresponding to the index of the utterance in the request
        public string utterance_text;   // text that was processed
        public List<Tones> tones { get; set; }  // tones detected in text (possibly 0)
    }

    public class Tones
    {
        public float score { get; set; }    // likelihood of this perception
        public String tone_id { get; set; } // sad, frustrated, satisfied, excited, polite, impolite, sympathetic
    }
}

/*
 *  TODO - Add MQChannel.cs and MQConnection.cs to csproj
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;

    /// <summary>TODO - use MQ instead of dbo.ActionToAnalyze</summary>
    public static void RabbitMQ()
    {
        using (WatsonToneAnalyzer.Connection connection = new WatsonToneAnalyzer.Connection())
        {
            connection.CreateChannel(GetActionMessage);
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine(); // keep the channel open!!!
        }
    }

    public static void GetActionMessage(object source, BasicDeliverEventArgs eventArgs)
    {
        // extract message
        var body = eventArgs.Body;
        var message = Encoding.UTF8.GetString(body);

        // json equivalent to SQLSelectFromActionsToAnalyze
        ActionToAnalyze action = ActionToAnalyze.Factory(message);

        // process message
        Console.WriteLine(" [x] Received {0}", message);

        string username = ConfigurationManager.AppSettings.Get("WatsonUsername");
        string password = ConfigurationManager.AppSettings.Get("WatsonPassword");
        HTTP_POST(action._actionID, action._actionDescription, username, password, (result) => PublishToTable(result, action._ticketID, action._userID, action._organizationID, action._isAgent));

        // acknowledge fully processed so release from MQ
        EventingBasicConsumer consumer = (EventingBasicConsumer)source;
        consumer.Model.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
    }*/
