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
    class WatsonTransaction : IDisposable
    {
        SqlConnection _connection;
        SqlTransaction _transaction;
        DataContext _db;

        public DataContext DataContext { get { return _db; } }

        public WatsonTransaction()
        {
            string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
            _connection = new SqlConnection(connectionString);  // using
            _connection.Open(); // connection must be open to begin transaction

            // start the transaction
            _transaction = _connection.BeginTransaction();  // using

            // create a data context
            _db = new DataContext(_connection); // using
            _db.Transaction = _transaction;
        }

        public void Commit() { _db.Transaction.Commit(); }
        public void Rollback() { _db.Transaction.Rollback(); }

        public void RecordWatsonResults(ActionToAnalyze actionToAnalyze)
        {
            // insert ActionSentiment
            ActionSentiment sentiment = InsertActionSentiment(_db, actionToAnalyze);
            _db.SubmitChanges();    // get the DB generated ID
            int actionSentimentID = sentiment.ActionSentimentID;

            // insert child records - ActionSentimentScore(s)
            List<Tones> tones = actionToAnalyze.GetTones();
            List<ActionSentimentScore> scores = InsertSentimentScores(tones, _db, actionSentimentID);

            // update the corresponding ticket sentiment
            if (!actionToAnalyze.IsAgent)
            {
                ActionSentimentScore maxScore = scores.Where(s => s.SentimentScore == scores.Max(a => a.SentimentScore)).First();
                TicketSentiment.TicketSentimentStrategy(_db, actionToAnalyze, maxScore);
            }

            // Delete ActionToAnalyze
            actionToAnalyze.DeleteOnSubmit(_db);
            _db.SubmitChanges();
        }

        /// <summary>
        /// Create the ActionSentiment for the ActionID
        /// </summary>
        /// <param name="db">context for insert/submit</param>
        /// <param name="actionToAnalyze">action to analyze</param>
        /// <returns></returns>
        static ActionSentiment InsertActionSentiment(DataContext db, ActionToAnalyze actionToAnalyze)
        {
            // already exists?
            Table<ActionSentiment> table = db.GetTable<ActionSentiment>();
            if (table.Where(u => u.ActionID == actionToAnalyze.ActionID).Any())
                WatsonEventLog.WriteEntry("duplicate ActionID in ActionSentiment table " + actionToAnalyze.ActionID, EventLogEntryType.Error);

            // Insert
            ActionSentiment sentiment = new ActionSentiment
            {
                ActionID = actionToAnalyze.ActionID,
                TicketID = actionToAnalyze.TicketID,
                UserID = actionToAnalyze.UserID,
                OrganizationID = actionToAnalyze.OrganizationID,
                IsAgent = actionToAnalyze.IsAgent,
                DateCreated = DateTime.UtcNow
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

            // if no tones detected - insert default score of 0.0
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
    }
}
