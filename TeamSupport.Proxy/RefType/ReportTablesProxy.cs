using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
    [DataContract(Namespace = "http://teamsupport.com/")]
    [KnownType(typeof(ReportTableProxy))]
    public class ReportTableProxy
    {
        public ReportTableProxy() { }
        [DataMember] public int ReportTableID { get; set; }
        [DataMember] public string TableName { get; set; }
        [DataMember] public string Alias { get; set; }
        [DataMember] public ReferenceType CustomFieldRefType { get; set; }
        [DataMember] public bool IsCategory { get; set; }
        [DataMember] public string OrganizationIDFieldName { get; set; }
        [DataMember] public string LookupKeyFieldName { get; set; }
        [DataMember] public string LookupDisplayClause { get; set; }
        [DataMember] public string LookupOrderBy { get; set; }
        [DataMember] public bool UseTicketRights { get; set; }

    }
}
