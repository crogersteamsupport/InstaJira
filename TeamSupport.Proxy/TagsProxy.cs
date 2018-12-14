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
  [KnownType(typeof(TagProxy))]
  public class TagProxy
  {
    public TagProxy() {}
    [DataMember] public int TagID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Value { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int CreatorID { get; set; }
          
  }
}
