using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using TeamSupport.CDI.linq;
using System.Configuration;

namespace TeamSupport.CDI.linq
{
    [Table(Name = "OrganizationSentiments")]
    public class OrganizationSentiment
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _organizationSentimentID;
        [Column(Storage = "_organizationSentimentID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int OrganizationSentimentID { get { return _organizationSentimentID; } }

        [Column]
        public int OrganizationID;
        [Column]
        public bool IsAgent;
        [Column]
        public double OrganizationSentimentScore;
        [Column]
        public int TicketSentimentCount;
#pragma warning restore CS0649

        public static string RawUpdateQuery(double organizationSentimentScore, int ticketSentimentCount, int organizationID)
        {
            return String.Format(@"UPDATE OrganizationSentiments SET OrganizationSentimentScore = {0}, TicketSentimentCount = {1} WHERE IsAgent=0 AND OrganizationID = {2}",
                organizationSentimentScore,
                ticketSentimentCount,
                organizationID);
        }

        static Dictionary<int, OrganizationSentiment> _organizationSentiments;
        public static void Initialize(DataContext db)
        {
            _organizationSentiments = new Dictionary<int, OrganizationSentiment>();
            Table<OrganizationSentiment> table = db.GetTable<OrganizationSentiment>();
            OrganizationSentiment[] sentiments = table.Where(o => !o.IsAgent).ToArray();
            foreach (OrganizationSentiment sentiment in sentiments)
            {
                if (!sentiment.IsAgent) // just to be safe - note that no ticket sentiments for agents should exist
                    _organizationSentiments[sentiment.OrganizationID] = sentiment;
            }
        }

        public static bool TryPop(int organizationID, out OrganizationSentiment organizationSentiment)
        {
            if(!_organizationSentiments.ContainsKey(organizationID))
            {
                organizationSentiment = null;
                return false;
            }

            organizationSentiment = _organizationSentiments[organizationID];
            _organizationSentiments.Remove(organizationID);
            return true;
        }

        public static void DeleteRemainder(DataContext db)
        {
            Table<OrganizationSentiment> table = db.GetTable<OrganizationSentiment>();
            foreach (KeyValuePair<int, OrganizationSentiment> pair in _organizationSentiments)
                db.ExecuteCommand(String.Format(@"DELETE FROM [OrganizationSentiments] WHERE [OrganizationID] = {0}", pair.Key));    // DELETE

            _organizationSentiments.Clear();
        }

    }
}
