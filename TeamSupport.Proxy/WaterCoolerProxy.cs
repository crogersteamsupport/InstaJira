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
  [KnownType(typeof(WaterCoolerItemProxy))]
  public class WaterCoolerItemProxy
  {
    public WaterCoolerItemProxy() {}
    [DataMember] public int MessageID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public DateTime TimeStamp { get; set; }
    [DataMember] public int? GroupFor { get; set; }
    [DataMember] public int? ReplyTo { get; set; }
    [DataMember] public string Message { get; set; }
    [DataMember] public string MessageType { get; set; }
          
  }
}
