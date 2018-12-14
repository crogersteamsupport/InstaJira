using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace WatsonToneAnalyzer
{
    class MessageUtterance
    {
        public ActionToAnalyze _action { get; private set; }
        public Utterance _utterance { get; private set; }

        public MessageUtterance(ActionToAnalyze actionToAnalyze, Utterance utterance)
        {
            _action = actionToAnalyze;
            _utterance = utterance;
        }
    }

    class WatsonMessage
    {
        List<MessageUtterance> _messageUtterances;
        List<ActionToAnalyze> _actions;

        public WatsonMessage()
        {
            _messageUtterances = new List<MessageUtterance>();
            _actions = new List<ActionToAnalyze>();
        }

        public bool Empty {  get { return UtteranceCount == 0; } }
        public int ActionCount { get { return _actions.Count; } }
        public int UtteranceCount { get { return _messageUtterances.Count; } }

        public override string ToString()
        {
            return String.Format("Actions {0} Utterances {1}", ActionCount, UtteranceCount);
        }

        static int WatsonUtterancePerAPICall = Int32.Parse(ConfigurationManager.AppSettings.Get("WatsonUtterancePerAPICall"));

        public bool TryAdd(ActionToAnalyze actionToAnalyze, Utterance utterance)
        {
            // Max 50 per call
            if (UtteranceCount + 1 > WatsonUtterancePerAPICall)
                return false;

            _actions.Add(actionToAnalyze);
            _messageUtterances.Add(new MessageUtterance(actionToAnalyze, utterance));
            return true;
        }

        public bool TryAdd(ActionToAnalyze actionToAnalyze, List<Utterance> utterances)
        {
            // Max 50 per call
            if (UtteranceCount + utterances.Count > WatsonUtterancePerAPICall)
                return false;

            _actions.Add(actionToAnalyze);
            foreach(Utterance utterance in utterances)
                _messageUtterances.Add(new MessageUtterance(actionToAnalyze, utterance));
            return true;
        }

        /// <summary>serialize to JSON</summary>
        public string ToJSON()
        {
            WatsonPostContent content = new WatsonPostContent();
            content.utterances = _messageUtterances.Select(u => u._utterance).ToList();
            return content.ToJSON();
        }

        /// <summary> Write the message results back to the database </summary>
        public void PublishWatsonResponse(UtteranceToneList watsonResponse)
        {
            foreach (UtteranceResponse response in watsonResponse.utterances_tone)
            {
                MessageUtterance messageUtterance = _messageUtterances[response.utterance_id];
                Utterance sent = messageUtterance._utterance;
                if (string.CompareOrdinal(sent.text, response.utterance_text) != 0)
                    Debugger.Break();

                ActionToAnalyze action = messageUtterance._action;
                action.AddSentiment(response);
            }

            // update the action with the accumulated results
            foreach(ActionToAnalyze action in _actions)
                PublishActionSentiment(action);
        }

        /// <summary>
        /// Async callback from HTTP_POST to put the watson response into the db
        /// </summary>
        /// <param name="result">Watson results</param>
        /// <param name="actionToAnalyze">ActionToAnalyze record we are processing</param>
        static Mutex _singleThreadedTransactions = new Mutex(false);

        static void PublishActionSentiment(ActionToAnalyze actionToAnalyze)
        {
            // Process The ActionToAnalyze results
            WatsonTransaction transaction = null;  // Transaction that can be rolled back
            try
            {
                // 1. Insert ActionSentiment and ActionSentimentScores
                // 2. run the TicketSentimentStrategy to create TicketSentimentScore
                // 3. delete the ActionToAnalyze
                _singleThreadedTransactions.WaitOne();  // connection does not support parallel transactions
                transaction = new WatsonTransaction();
                transaction.RecordWatsonResults(actionToAnalyze);
                transaction.Commit();
            }
            catch (Exception e2)
            {
                if (transaction != null)
                    transaction.Rollback();
                WatsonEventLog.WriteEntry("Watson analysis failed - system will retry", e2);
                Console.WriteLine(e2.ToString());
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
                _singleThreadedTransactions.ReleaseMutex();
            }
        }

    }
}
