using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Diagnostics;

// Code to get an understanding of how big action text would be if we sent it to watson.
// Used to optimize Watson because the code in this project makes it easy...

namespace TeamSupport.CDI
{
    /// <summary>
    /// Actions table
    /// </summary>
    [Table(Name = "Actions")]
    class Action
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _actionID;
        [Column(Storage = "_actionID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ActionID { get { return _actionID; } }

        [Column]
        public int TicketID;
        [Column]
        public string Description;
#pragma warning restore CS0649
    }

    /// <summary>
    /// ActionSentiments table
    /// </summary>
    [Table(Name = "ActionSentiments")]
    class ActionSentiment
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _actionSentimentID;
        [Column(Storage = "_actionSentimentID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ActionSentimentID { get { return _actionSentimentID; } }

        [Column]
        public int ActionID;
#pragma warning restore CS0649
    }

    /// <summary>
    /// Figure out how big the action text would be if we sent it to watson
    /// </summary>
    public class WatsonTicketSize
    {
        public static string CleanString(string RawHtml)
        {
            String text = Regex.Replace(RawHtml, @"<[^>]*>", String.Empty); //remove html tags
            text = Regex.Replace(text, "&nbsp;", " "); //remove HTML space
            text = Regex.Replace(text, @"[\d-]", " "); //removes all digits [0-9]
            text = Regex.Replace(text, @"[\w\d]+\@[\w\d]+\.com", " "); //removes email adresses
            text = Regex.Replace(text, @"\s+", " ");   // remove whitespace

            return text;
        }

        public void WatsonActionSize()
        {
            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    //db.Log = CDIEventLog.Instance;
                    Table<Action> actionTable = db.GetTable<Action>();
                    Table<ActionSentiment> sentimentTable = db.GetTable<ActionSentiment>();

                    var query = (from st in sentimentTable
                                 join a in actionTable on st.ActionID equals a.ActionID
                                 orderby st.ActionSentimentID descending
                                 select a).Take(10000);

                    Action[] all = query.ToArray();
                    //double[] length = new double[all.Length];
                    int longSentenceCount = 0;
                    int totalSentenceCount = 0;
                    for (int i = 0; i < all.Length; ++i)
                    {
                        all[i].Description = CleanString(all[i].Description);

                        // break into sentences
                        string[] sentences = Regex.Split(all[i].Description, @"(?<=[.?!,;:])");
                        totalSentenceCount += sentences.Length;

                        foreach (string sentence in sentences)
                        {
                            if (sentence.Length <= 500)
                                continue;

                            ++longSentenceCount;
                            Debug.WriteLine(sentence);
                            //string  excellent = Regex.Replace(sentence, @"([-_=.]+){3,}\1", "$1");   // remove duplicates
                        }

                        //length[i] = all[i].Description.Length;
                        //if (length[i] > 10000)
                        //    Debugger.Break();

                    }                    

                    //double avg = length.Average();
                    //double stdev = Statistics.StandardDeviation(length, avg);
                    //foreach (double value in length)
                    //    Debug.WriteLine(value);
                    double longPercent = (double)longSentenceCount / totalSentenceCount;
                }
            }
            catch (Exception e)
            {
                CDIEventLog.Instance.WriteEntry("Ticket Read failed", e);
            }
        }
    }

}
