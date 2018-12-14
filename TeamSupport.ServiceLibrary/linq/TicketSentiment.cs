using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Threading;

namespace WatsonToneAnalyzer
{
    [Table(Name = "TicketSentiments")]
    class TicketSentiment
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _ticketSentimentID;
        [Column(Storage = "_ticketSentimentID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int TicketSentimentID { get { return _ticketSentimentID; } }

        [Column]
        public int TicketID;
        [Column]
        public int OrganizationID;
        [Column]
        public bool IsAgent;
        [Column]
        public int TicketSentimentScore;
        [Column]
        public bool Sad;
        [Column]
        public bool Frustrated;
        [Column]
        public bool Satisfied;
        [Column]
        public bool Excited;
        [Column]
        public bool Polite;
        [Column]
        public bool Impolite;
        [Column]
        public bool Sympathetic;
        [Column]
        public double AverageActionSentiment;
        [Column]
        public int ActionSentimentCount;
        [Column]
        public DateTime TicketDateCreated;
#pragma warning restore CS0649

        public void SetSentimentID(int sentimentID)
        {
            switch (sentimentID)
            {
                case 1:
                    Sad = true;
                    break;
                case 2:
                    Frustrated = true;
                    break;
                case 3:
                    Satisfied = true;
                    break;
                case 4:
                    Excited = true;
                    break;
                case 5:
                    Polite = true;
                    break;
                case 6:
                    Impolite = true;
                    break;
                case 7:
                    Sympathetic = true;
                    break;
            }
        }

        static Mutex _mutex = new Mutex(false);

        /// <summary>
        /// Callback on insert of Watson results - using the transaction submitted but NOT committed
        /// 
        /// For each action, use the most likely sentiment (highest SentimentScore)
        /// Use Net Promoter score (promoters - detractors), normalized to [0, 1000] where 500 is neutral
        /// </summary>
        /// <param name="transaction">data associated with the watson transaction</param>
        public static void TicketSentimentStrategy(DataContext db, ActionToAnalyze actionToAnalyze, ActionSentimentScore maxScore)
        {
            if (actionToAnalyze.IsAgent)
                return;

            try
            {
                // normalize to [0, 1000]
                double actionScore = Convert.ToDouble(ToneSentiment.ToneSentiments[maxScore.SentimentID].SentimentMultiplier.Value) * Convert.ToDouble(maxScore.SentimentScore);
                actionScore = 500 * actionScore + 500;

                // submit TicketSentiment
                TicketSentiment score = null;
                CreateTicketSentiment(db, actionToAnalyze, actionScore, out score);
                score.TicketSentimentScore = (int)Math.Round(score.AverageActionSentiment);
                score.SetSentimentID(maxScore.SentimentID);
                db.SubmitChanges();
            }
            catch (Exception e2)
            {
                WatsonEventLog.WriteEntry("Exception caught at select from ACtionsToAnalyze or HttpPOST:", e2);
                Console.WriteLine(e2.ToString());
            }
        }

        static bool CreateTicketSentiment(DataContext db, ActionToAnalyze actionToAnalyze, double actionScore, out TicketSentiment score)
        {
            Table<TicketSentiment> table = db.GetTable<TicketSentiment>();
            score = table.Where(t => (t.TicketID == actionToAnalyze.TicketID) && (t.IsAgent == actionToAnalyze.IsAgent)).FirstOrDefault();
            if (score == null)
            {
                score = new TicketSentiment()
                {
                    TicketID = actionToAnalyze.TicketID,
                    OrganizationID = actionToAnalyze.OrganizationID,
                    IsAgent = actionToAnalyze.IsAgent,
                    TicketDateCreated = actionToAnalyze.DateCreated,
                    TicketSentimentScore = 0,
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
                table.InsertOnSubmit(score);
                return true;
            }
            return false;
        }

        int ToInt(bool value) { return value ? 1 : 0; }

        /// <summary>
        /// Performance optimization - linq is slow because
        /// 1. defaults to thread safe (full where clause of original state).
        /// 2. Uses parameterization which checks each parameter for sql injection attacks
        /// 3. Caches execution plan
        /// 
        /// Note that (1.) can be turned off by table column attribute: [Column(UpdateCheck=UpdateCheck.Never)]
        /// Also, if read-only or using these hard coded versions. turn off object tracking:
        ///     db.ObjectTrackingEnabled = false;
        /// </summary>
        /// <param name="db"></param>
        public void Update(DataContext db)
        {
            // very efficient way to update the ticket
            db.ExecuteCommand(String.Format(@"UPDATE [TicketSentiments]
                SET [TicketSentimentScore] = {0}, 
                [Sad] = {1}, [Frustrated] = {2}, [Satisfied] = {3}, [Excited] = {4}, [Polite] = {5}, [Impolite] = {6}, [Sympathetic] = {7}, 
                [AverageActionSentiment] = {8}, [ActionSentimentCount] = {9}
                WHERE [TicketSentimentID] = {10}",
                TicketSentimentScore,
                ToInt(Sad), ToInt(Frustrated), ToInt(Satisfied), ToInt(Excited), ToInt(Polite), ToInt(Impolite), ToInt(Sympathetic),
                AverageActionSentiment, ActionSentimentCount,
                TicketSentimentID
                ));
        }

        public void Insert(DataContext db)
        {
            //db.ExecuteCommand(String.Format(@"INSERT INTO TicketSentiments (TicketID, OrganizationID, IsAgent, TicketSentimentScore, Sad, Frustrated, Satisfied, Excited, Polite, Impolite, Sympathetic, AverageActionSentiment, ActionSentimentCount, TicketDateCreated)
            //    VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13})",

            string insertCmd = String.Format(@"INSERT INTO TicketSentiments
                VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, CAST(N'{13}' AS DateTime))",
                TicketID, OrganizationID, ToInt(IsAgent), TicketSentimentScore,
                ToInt(Sad), ToInt(Frustrated), ToInt(Satisfied), ToInt(Excited), ToInt(Polite), ToInt(Impolite), ToInt(Sympathetic),
                AverageActionSentiment, ActionSentimentCount, TicketDateCreated);
            db.ExecuteCommand(insertCmd);
        }
    }
}
