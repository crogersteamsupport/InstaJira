using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace TeamSupport.CDI.linq
{
    /// <summary> 
    /// [dbo].[CustDistHistory] 
    /// Last 10 days of CDI data for trend detection
    /// </summary>
    [Table(Name = "CustDistHistory")]
    public class CustDistHistory
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _custDistHistoryID;
        [Column(Storage = "_custDistHistoryID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int CustDistHistoryID { get { return _custDistHistoryID; } }

        [Column]
        public int ParentOrganizationID;
        [Column]
        public int OrganizationID;
        [Column]
        public DateTime CDIDate;
        [Column]
        public int? CDIValue;
#pragma warning restore CS0649
    }
}
