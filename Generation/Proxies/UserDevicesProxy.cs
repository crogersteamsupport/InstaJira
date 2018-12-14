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
  
  public partial class UserDevice : BaseItem
  {
    public UserDeviceProxy GetProxy()
    {
      UserDeviceProxy result = new UserDeviceProxy();
      result.IsActivated = this.IsActivated;
      result.DeviceID = this.DeviceID;
      result.UserID = this.UserID;
      result.UserDeviceID = this.UserDeviceID;
       
      result.DateActivated = DateTime.SpecifyKind(this.DateActivatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
