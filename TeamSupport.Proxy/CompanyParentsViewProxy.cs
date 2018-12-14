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
  [KnownType(typeof(CompanyParentsViewItemProxy))]
  public class CompanyParentsViewItemProxy
  {
    public CompanyParentsViewItemProxy() {}
    [DataMember] public int ChildID { get; set; }
    [DataMember] public int ParentID { get; set; }
    [DataMember] public string ParentName { get; set; }
          
  }
}
