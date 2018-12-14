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
  [KnownType(typeof(CustomFieldCategoryProxy))]
  public class CustomFieldCategoryProxy
  {
    public CustomFieldCategoryProxy() {}
    [DataMember] public int CustomFieldCategoryID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Category { get; set; }
    [DataMember] public int Position { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
    [DataMember] public int? AuxID { get; set; }
    [DataMember] public int? ProductFamilyID { get; set; }
    [DataMember] public string ProductFamilyName { get; set; }
          
  }
  
}
