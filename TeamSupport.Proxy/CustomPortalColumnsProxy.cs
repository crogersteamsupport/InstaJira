using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(CustomPortalColumnProxy))]
  public class CustomPortalColumnProxy
  {
    public CustomPortalColumnProxy() {}
    [DataMember] public int CustomColumnID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int Position { get; set; }
    [DataMember] public int? StockFieldID { get; set; }
    [DataMember] public int? CustomFieldID { get; set; }
    [DataMember] public string FieldText { get; set; }
          
  }
}
