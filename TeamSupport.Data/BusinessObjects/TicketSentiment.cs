using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Diagnostics;

namespace TeamSupport.Data.BusinessObjects
{
    // see ts-app\TeamSupport.ServiceLibrary\TicketSentiments.cs
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

        static int OrganizationSentiment(int organizationID)
        {
            double result = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(LoginUser.GetConnectionString(-1)))
                using (DataContext db = new DataContext(connection))
                {
                    Table<TicketSentiment> ticketSentimentTable = db.GetTable<TicketSentiment>();
                    result = (from sentiment in ticketSentimentTable where (sentiment.OrganizationID == organizationID) && !sentiment.IsAgent select sentiment.TicketSentimentScore).Average();
                }
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("Application", "Exception caught at OrganizationSentiment:" + e.Message + " ----- STACK: " + e.StackTrace.ToString());
                Console.WriteLine(e.ToString());
            }
            return (int)Math.Round(result);
        }
    }
}
