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
  [KnownType(typeof(FacebookOptionProxy))]
  public class FacebookOptionProxy
  {
    public FacebookOptionProxy() {}
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public bool DisplayArticles { get; set; }
    [DataMember] public bool DisplayKB { get; set; }
          
  }
}
