using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Configuration;
using System.Diagnostics;

namespace WatsonToneAnalyzer
{
    /// <summary>
    /// Transaction to insert watson results into the DB
    /// </summary>
    class WatsonResultsTransaction : IDisposable
    {
        const string EVENT_SOURCE = "Application";

        SqlConnection _connection;
        SqlTransaction _transaction;
        DataContext _db;

        class MaxActionSentimentScore
        {
            public int ActionID;
            public int SentimentID;
            public decimal MaxSentimentScore;
            public decimal SentimentMultiplier;
        }

        static System.Threading.Mutex _singleThreadedTransactions = new System.Threading.Mutex(false);

        /// <summary>
        /// Constructor to wrap the "using" of SqlConnection, SqlTransaction, and DataContext
        /// </summary>
        /// <param name="tones"></param>
        /// <param name="actionToAnalyze"></param>
        public WatsonResultsTransaction(Utterance utterance, ActionToAnalyze actionToAnalyze, Action<WatsonTransactionCallback> callback)
        {
            List<Tones> tones = utterance.tones;
            if (tones == null)
                return; // ?

            // open the connection
            try
            {
                _singleThreadedTransactions.WaitOne();
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                _connection = new SqlConnection(connectionString);  // using
                _connection.Open(); // connection must be open to begin transaction

                // start the transaction
                _transaction = _connection.BeginTransaction();  // using

                // create a data context
                _db = new DataContext(_connection); // using
                _db.Transaction = _transaction;

                // insert ActionSentiment
                ActionSentiment sentiment = InsertActionSentiment(_db, actionToAnalyze);
                _db.SubmitChanges();    // get the DB generated ID
                int actionSentimentID = sentiment.ActionSentimentID;

                // insert child records - ActionSentimentScore(s)
                List<ActionSentimentScore> scores = InsertSentimentScores(tones, _db, actionSentimentID);

                // Delete ActionToAnalyze
                actionToAnalyze.DeleteOnSubmit(_db);
                _db.SubmitChanges();

                if (callback != null)
                {
                    WatsonTransactionCallback transaction = new WatsonTransactionCallback()
                    {
                        _db = _db,
                        _sentiment = sentiment,
                        _scores = scores
                    };
                    callback(transaction);
                }

                // Success!
                _transaction.Commit();
            }
            catch (Exception e)
            {
                if (_transaction != null)
                    _transaction.Rollback();

                EventLog.WriteEntry(EVENT_SOURCE, "********************PublishToTable SubmitTransaction: Exception " + e.Message + " ### " + e.Source + " ### " + e.StackTrace.ToString());
                Console.WriteLine("Exception at insert into Action Sentiment:" + e.Message + "###" + e.Source + " ----- STACK: " + e.StackTrace.ToString());
            }
            finally
            {
                _singleThreadedTransactions.ReleaseMutex();
            }
        }

        //void Stuff(ActionToAnalyze actionToAnalyze)
        //{
        //    string query = @"SELECT m.ActionID, s.SentimentID, m.MaxSentimentScore, t.SentimentMultiplier
        //                                FROM (
        //                                    SELECT a.ActionID, a.ActionSentimentID, MAX(s.SentimentScore) AS MaxSentimentScore
        //                                    FROM ActionSentiments a
        //                                    INNER JOIN ActionSentimentScores s ON a.ActionSentimentID=s.ActionSentimentID
        //                                    WHERE TicketID={0} AND a.IsAgent='{1}'
        //                                    GROUP BY a.ActionID, a.ActionSentimentID
        //                                ) AS m
        //                                INNER JOIN ActionSentimentScores AS s ON m.ActionSentimentID=s.ActionSentimentID AND m.MaxSentimentScore=s.SentimentScore
        //                                INNER JOIN ToneSentiments AS t ON t.SentimentID=s.SentimentID
        //                                GROUP BY m.ActionID, s.SentimentID, m.MaxSentimentScore, t.SentimentMultiplier";
        //    string fullQuery = string.Format(query, actionToAnalyze.TicketID, actionToAnalyze.IsAgent ? "1" : "0");
        //    var result = _db.ExecuteQuery<MaxActionSentimentScore>(fullQuery);

        //    Table<TicketSentimentScoreLinq> ticketScoresTable = _db.GetTable<TicketSentimentScoreLinq>();
        //    TicketSentimentScoreLinq ticketSentimentScore = (from u in ticketScoresTable where u.TicketID == actionToAnalyze.TicketID select u).FirstOrDefault();
        //    if (ticketSentimentScore == null)
        //    {
        //        ticketSentimentScore = new TicketSentimentScoreLinq()
        //        {
        //            TicketID = actionToAnalyze.TicketID,
        //            IsAgent = actionToAnalyze.IsAgent,
        //            TicketSentimentScore = 0,
        //            Sad = false,
        //            Frustrated = false,
        //            Satisfied = false,
        //            Excited = false,
        //            Polite = false,
        //            Impolite = false,
        //            Sympathetic = false
        //        };
        //        ticketScoresTable.InsertOnSubmit(ticketSentimentScore);
        //    }

        //    // calculate a normalized ticket sentiment
        //    double ticketSentiment = 0;
        //    {
        //        int count = 0;
        //        List<int> sentiments = new List<int>();
        //        foreach (MaxActionSentimentScore record in result)
        //        {
        //            ++count;
        //            if (record.SentimentID == 0)    // no sentiment found
        //                continue;

        //            ticketSentiment += Convert.ToDouble(record.SentimentMultiplier) * Convert.ToDouble(record.MaxSentimentScore);
        //            ticketSentimentScore.SetSentimentID(record.SentimentID);
        //        }

        //        if (count != 0)
        //            ticketSentiment /= count;  // normalize to +- 100%
        //        ticketSentiment = 500 * ticketSentiment + 500;  // normalize to [0, 1000]
        //    }

        //    // submit record
        //    ticketSentimentScore.TicketSentimentScore = (int)Math.Round(ticketSentiment);
        //    _db.SubmitChanges();
        //}

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_db != null)
                _db.Dispose();

            if (_transaction != null)
                _transaction.Dispose();

            if (_connection != null)
                _connection.Dispose();
        }

        /// <summary>
        /// Create the ActionSentiment for the ActionID
        /// </summary>
        /// <param name="db">context for insert/submit</param>
        /// <param name="a">action to analyze</param>
        /// <returns></returns>
        static ActionSentiment InsertActionSentiment(DataContext db, ActionToAnalyze a)
        {
            // already exists?
            Table<ActionSentiment> table = db.GetTable<ActionSentiment>();
            if (table.Where(u => u.ActionID == a.ActionID).Any())
                throw new Exception("Error: ActionSentiment already exists?");

            // Insert
            ActionSentiment sentiment = new ActionSentiment
            {
                ActionID = a.ActionID,
                TicketID = a.TicketID,
                UserID = a.UserID,
                OrganizationID = a.OrganizationID,
                IsAgent = a.IsAgent,
                DateCreated = DateTime.Now
            };
            table.InsertOnSubmit(sentiment);
            return sentiment;
        }

        /// <summary>
        /// Insert the detected sentiment tones, or a default if not found
        /// </summary>
        /// <param name="tones">tones from IBM Watson - may be empty</param>
        /// <param name="db">data context</param>
        /// <param name="actionSentimentID">Parent record ID</param>
        private static List<ActionSentimentScore> InsertSentimentScores(List<Tones> tones, DataContext db, int actionSentimentID)
        {
            List<ActionSentimentScore> result = new List<ActionSentimentScore>();

            // tones detected?
            Table<ActionSentimentScore> actionSentimentScoreTable = db.GetTable<ActionSentimentScore>();
            if ((tones == null) || !tones.Any())
            {
                // no tones detected
                ActionSentimentScore score = new ActionSentimentScore
                {
                    ActionSentimentID = actionSentimentID,
                    SentimentID = 0,    // no sentiment detected
                    SentimentScore = 0
                };
                actionSentimentScoreTable.InsertOnSubmit(score);
                result.Add(score);
                return result;
            }

            // insert tone sentiment scores
            foreach (Tones tone in tones)
            {
                ActionSentimentScore score = new ActionSentimentScore
                {
                    ActionSentimentID = actionSentimentID,
                    SentimentID = (int)FindSentimentID(tone.tone_id),
                    SentimentScore = Convert.ToDecimal(tone.score)
                };
                actionSentimentScoreTable.InsertOnSubmit(score);
                result.Add(score);
            }

            return result;
        }

        /// <summary> See dbo.ToneSentiment </summary>
        enum ESentiment
        {
            none = 0,
            sad = 1,
            frustrated = 2,
            satisfied = 3,
            excited = 4,
            polite = 5,
            impolite = 6,
            sympathetic = 7
        }

        /// <summary> Find the sentiment enum from tone id text </summary>
        static ESentiment FindSentimentID(string Sentiment)
        {
            ESentiment result;
            if (!Enum.TryParse<ESentiment>(Sentiment, false, out result))
                return ESentiment.none;
            return result;
        }
    }
}
