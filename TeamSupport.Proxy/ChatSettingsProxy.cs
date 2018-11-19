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
  [KnownType(typeof(ChatSettingProxy))]
  public class ChatSettingProxy
  {
    public ChatSettingProxy() {}
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public bool UseCss { get; set; }
    [DataMember] public string ClientCss { get; set; }
          
  }
}