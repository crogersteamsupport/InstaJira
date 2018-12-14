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
  [KnownType(typeof(UserDeviceProxy))]
  public class UserDeviceProxy
  {
    public UserDeviceProxy() {}
    [DataMember] public int UserDeviceID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public string DeviceID { get; set; }
    [DataMember] public DateTime DateActivated { get; set; }
    [DataMember] public bool IsActivated { get; set; }
          
  }
}
