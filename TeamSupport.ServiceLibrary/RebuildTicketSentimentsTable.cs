using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;

namespace WatsonToneAnalyzer
{

    class RebuildTicketSentimentsTable
    {
        Dictionary<int, TicketSentiment> _ticketSentiments;
        ToneSentiment[] _toneSentiment;

        public RebuildTicketSentimentsTable()
        {
            _ticketSentiments = new Dictionary<int, TicketSentiment>();
        }

        void Initialize(DataContext db)
        {
            db.ExecuteCommand("truncate table TicketSentiments");

            // tone multipliers
            Table<ToneSentiment> tones = db.GetTable<ToneSentiment>();
            _toneSentiment = (from tone in tones select tone).ToArray();
        }

        public void DoRebuild()
        {
            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    db.ObjectTrackingEnabled = false;   // read-only linq to sql

                    Initialize(db);

                    // load the highest confidence sentiment for each client action
                    MaxActionSentiment[] actionSentiments = LoadActionSentiments(db);
                    Array.Sort(actionSentiments, (lhs, rhs) => lhs.TicketID.CompareTo(rhs.TicketID));
                    foreach (MaxActionSentiment actionSentiment in actionSentiments)
                    {
                        // new TicketSentiment?
                        double actionScore = ToTicketScore(actionSentiment.SentimentID, actionSentiment.MaxSentimentScore);
                        if (!_ticketSentiments.ContainsKey(actionSentiment.TicketID))
                        {
                            _ticketSentiments[actionSentiment.TicketID] = CreateTicketSentiment(actionSentiment, db, actionScore);
                            continue;
                        }

                        // update existing TicketSentiment
                        TicketSentiment sentiment = _ticketSentiments[actionSentiment.TicketID];
                        int count = sentiment.ActionSentimentCount;
                        sentiment.AverageActionSentiment = (count * sentiment.AverageActionSentiment + actionScore) / (count + 1);
                        sentiment.ActionSentimentCount = count + 1;
                        sentiment.TicketSentimentScore = (int)Math.Round(sentiment.AverageActionSentiment);
                        sentiment.SetSentimentID(actionSentiment.SentimentID);
                    }

                    int insertCount = 0;
                    foreach (KeyValuePair<int, TicketSentiment> pair in _ticketSentiments)
                    {
                        pair.Value.Insert(db);
                        if (++insertCount >= 1000)
                        {
                            Console.Write('.');
                            insertCount = 0;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                WatsonEventLog.WriteEntry("Exception in RebuildTicketSentimentsTable", e);
                Console.WriteLine(e.ToString());
            }
        }

        double ToTicketScore(int sentimentID, decimal percent)
        {
            // normalize to [0, 1000]
            double actionScore = Convert.ToDouble(_toneSentiment[sentimentID].SentimentMultiplier.Value) * Convert.ToDouble(percent);
            actionScore = (actionScore + 1) * 500;
            return actionScore;
        }

        /// <summary>
        /// first action sentiment creates the ticket sentiment 
        /// </summary>
        TicketSentiment CreateTicketSentiment(MaxActionSentiment actionSentiment, DataContext db, double actionScore)
        {
            TicketSentiment ticketSentiment = new TicketSentiment()
            {
                TicketID = actionSentiment.TicketID,
                OrganizationID = actionSentiment.OrganizationID,
                IsAgent = actionSentiment.IsAgent,
                TicketDateCreated = actionSentiment.DateCreated,
                TicketSentimentScore = (int)Math.Round(actionScore),
                Sad = false,
                Frustrated = false,
                Satisfied = false,
                Excited = false,
                Polite = false,
                Impolite = false,
                Sympathetic = false,
                AverageActionSentiment = actionScore,
                ActionSentimentCount = 1
            };
            ticketSentiment.SetSentimentID(actionSentiment.SentimentID);
            return ticketSentiment;
        }

        static MaxActionSentiment[] LoadActionSentiments(DataContext db)
        {
            // load the highest confidence sentiment for each action
            string query = @"SELECT m.TicketID, m.OrganizationID, m.ActionID, s.SentimentID, m.MaxSentimentScore, m.IsAgent, t.DateCreated
                        FROM (
	                        SELECT a.TicketID, a.OrganizationID, a.ActionID, a.IsAgent, a.ActionSentimentID, MAX(s.SentimentScore) AS MaxSentimentScore
	                        FROM ActionSentiments as a
	                        INNER JOIN ActionSentimentScores s ON a.ActionSentimentID=s.ActionSentimentID
	                        GROUP BY a.TicketID, a.OrganizationID, a.ActionID, a.IsAgent, a.ActionSentimentID
                        ) AS m
                        INNER JOIN ActionSentimentScores AS s ON m.ActionSentimentID=s.ActionSentimentID AND m.MaxSentimentScore=s.SentimentScore
                        JOIN Tickets AS t ON m.TicketID=t.TicketID
                        WHERE YEAR(t.DateCreated)>=2017 and m.IsAgent=0
                        GROUP BY m.TicketID, m.OrganizationID, m.ActionID, s.SentimentID, m.MaxSentimentScore, m.IsAgent, t.DateCreated";
            return db.ExecuteQuery<MaxActionSentiment>(query).ToArray();
        }
    }

    /// <summary>
    /// Record used in rolling up the sentiment scores for the ticket
    /// </summary>
    class MaxActionSentiment
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        public int TicketID;
        public int OrganizationID;
        public int ActionID;
        public int SentimentID;
        public decimal MaxSentimentScore;
        public bool IsAgent;
        public DateTime DateCreated;
#pragma warning restore CS0649
    }
}
