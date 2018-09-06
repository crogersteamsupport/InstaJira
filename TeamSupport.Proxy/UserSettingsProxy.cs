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
  [KnownType(typeof(UserSettingProxy))]
  public class UserSettingProxy
  {
    public UserSettingProxy() {}
    [DataMember] public int UserSettingID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public string SettingKey { get; set; }
    [DataMember] public string SettingValue { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
          
  }
}
