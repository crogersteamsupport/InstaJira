using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(WaterCoolerViewItemProxy))]
  public class WaterCoolerViewItemProxy
  {
    public WaterCoolerViewItemProxy() {}
    [DataMember] public int MessageID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string TimeStamp { get; set; }
    [DataMember] public string Message { get; set; }
    [DataMember] public int? MessageParent { get; set; }
    [DataMember] public bool IsDeleted { get; set; }
    [DataMember] public DateTime LastModified { get; set; }
    [DataMember] public string UserName { get; set; }     
    [DataMember] public bool NeedsIndexing { get; set; }
          
  }
}
