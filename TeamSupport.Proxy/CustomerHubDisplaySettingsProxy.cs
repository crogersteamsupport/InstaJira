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
  [KnownType(typeof(CustomerHubDisplaySettingProxy))]
  public class CustomerHubDisplaySettingProxy
  {
    public CustomerHubDisplaySettingProxy() {}
    [DataMember] public int CustomerHubDisplaySettingID { get; set; }
    [DataMember] public int CustomerHubID { get; set; }
    [DataMember] public string FontFamily { get; set; }
    [DataMember] public string FontColor { get; set; }
    [DataMember] public string Color1 { get; set; }
    [DataMember] public string Color2 { get; set; }
    [DataMember] public string Color3 { get; set; }
    [DataMember] public string Color4 { get; set; }
    [DataMember] public string Color5 { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int? ModifierID { get; set; }
          
  }
}
