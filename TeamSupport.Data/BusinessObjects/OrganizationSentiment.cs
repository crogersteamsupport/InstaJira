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
    [Table(Name = "OrganizationSentiments")]
    public class OrganizationSentiment
    {
        #pragma warning disable CS0649  // Field is never assigned to in code (assigned by linq to sql)
        int _organizationSentimentID;
        [Column(Storage = "_organizationSentimentID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int OrganizationSentimentID { get { return _organizationSentimentID; } }

        [Column]
        public int OrganizationID;
        [Column]
        public bool IsAgent;
        [Column]
        double OrganizationSentimentScore;
        #pragma warning restore CS0649

        public static double? GetOrganizationSentiment(int organizationID, int parentId)
        {
            double? result = null;
            try
            {
				Organization parentOrganization = Organizations.GetOrganization(LoginUser.Anonymous, parentId);

				if (parentOrganization != null && parentOrganization.UseWatson)
				{
                using (SqlConnection connection = new SqlConnection(LoginUser.GetConnectionString(-1)))
                using (DataContext db = new DataContext(connection))
                {
                    Table<OrganizationSentiment> table = db.GetTable<OrganizationSentiment>();
                    double[] rows = (from sentiment in table
                                     where (sentiment.OrganizationID == organizationID) && !sentiment.IsAgent
                                     select sentiment.OrganizationSentimentScore).ToArray();
                    if (rows.Length > 0)
                        result = rows[0];
                }
				}
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("Application", "Exception caught at OrganizationSentiment:" + e.Message + " ----- STACK: " + e.StackTrace.ToString());
                Console.WriteLine(e.ToString());
            }
            return result;
        }
    }
}
