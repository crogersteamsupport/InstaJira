using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Linq;

namespace TeamSupport.CDI.linq
{
    /// <summary> 
    /// [dbo].[Organizations] 
    /// Customers and Clients
    /// </summary>
    [Table(Name = "Organizations")]
    class Organization
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _organizationID;
        [Column(Storage = "_organizationID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true, UpdateCheck = UpdateCheck.Never)]
        public int OrganizationID { get { return _organizationID; } }

        [Column]
        public int? ParentID;
        //[Column]
        //public string Name;   // slows it down
        [Column]
        public bool IsActive;

        // CDI fields
        [Column]
        public int TotalTicketsCreated;
        [Column]
        public int TicketsOpen;
        [Column]
        public int CreatedLast30;
        [Column]
        public int AvgTimeOpen;
        [Column]
        public int AvgTimeToClose;
        [Column]
        public int CustDisIndex;
        [Column]
        public int? CustDistIndexTrend; // Trending upwards (1 bad),  Trending down (-1 good), the same(0)
#pragma warning restore CS0649

        /// <summary>
        /// This is really fast because there is no linq to sql change tracking or 
        /// parameterization which sql-server uses to test for sql-injection attacks
        /// </summary>
        public static string RawUpdateQuery(Metrics metrics, int organizationID)
        {
            return String.Format(@"UPDATE Organizations SET TotalTicketsCreated = {0}, TicketsOpen = {1}, CreatedLast30 = {2}, AvgTimeOpen = {3}, AvgTimeToClose = {4}, CustDisIndex = {5}, CustDistIndexTrend = {6} WHERE OrganizationID = {7}",
                metrics._totalTicketsCreated,  // TotalTicketsCreated
                metrics._openCount,    // TicketsOpen
                metrics._newCount, // CreatedLast30
                (int)Math.Round(metrics._medianDaysOpen),  // AvgTimeOpen
                metrics._medianDaysToClose.HasValue ? (int)Math.Round(metrics._medianDaysToClose.Value) : 0,  // AvgTimeToClose
                metrics.CDI.Value, // CustDisIndex
                0,  // CustDistIndexTrend
                organizationID);    // OrganizationID
        }

        //// for test output...
        //public static bool TryGet(int organizationID, out Organization organization)
        //{
        //    if (_organizations == null)
        //        _organizations = LoadOrganizations();

        //    if (!_organizations.ContainsKey(organizationID))
        //    {
        //        organization = null;
        //        return false;
        //    }
        //    organization = _organizations[organizationID];
        //    return true;
        //}

        //static Dictionary<int, Organization> _organizations;
        //public static Dictionary<int, Organization> LoadOrganizations()
        //{
        //    Dictionary<int, Organization> allOrganizations = null;
        //    try
        //    {
        //        Organization[] organizations = null;
        //        string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        using (DataContext db = new DataContext(connection))
        //        {
        //            db.ObjectTrackingEnabled = false;   // read-only
        //            Table<Organization> OrganizationsTable = db.GetTable<Organization>();
        //            organizations = (from o in OrganizationsTable select o).ToArray();
        //        }

        //        allOrganizations = new Dictionary<int, Organization>();
        //        foreach (Organization og in organizations)
        //            allOrganizations[og.OrganizationID] = og;
        //    }
        //    catch (Exception e)
        //    {
        //        CDIEventLog.Instance.WriteEntry("Organization Read failed", e);
        //    }

        //    return allOrganizations;
        //}

    }
}
