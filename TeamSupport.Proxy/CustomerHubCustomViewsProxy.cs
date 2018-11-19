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
  [KnownType(typeof(CustomerHubCustomViewProxy))]
  public class CustomerHubCustomViewProxy
  {
    public CustomerHubCustomViewProxy() {}
    [DataMember] public int CustomerHubCustomViewID { get; set; }
    [DataMember] public int CustomerHubID { get; set; }
    [DataMember] public int CustomerHubViewID { get; set; }
    [DataMember] public string CustomView { get; set; }
    [DataMember] public bool IsActive { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int? ModifierID { get; set; }
          
  }
}