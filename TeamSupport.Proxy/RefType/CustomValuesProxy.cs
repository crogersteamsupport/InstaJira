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
  [KnownType(typeof(CustomValueProxy))]
  public class CustomValueProxy
  {
    public CustomValueProxy() {}
    [DataMember] public int? CustomValueID { get; set; }
    [DataMember] public int CustomFieldID { get; set; }
    [DataMember] public int? RefID { get; set; }
    [DataMember] public object Value { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public int? ImportFileID { get; set; }

    /* Custom Fields Info */
    [DataMember] public string FieldName { get; set; }
    [DataMember] public string ApiFieldName { get; set; }
    [DataMember] public string ListValues { get; set; }
    [DataMember] public CustomFieldType FieldType { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
    [DataMember] public int? AuxID { get; set; }
    [DataMember] public int Position { get; set; }
    [DataMember] public bool IsVisibleOnPortal { get; set; }
    [DataMember] public bool IsFirstIndexSelect { get; set; }
    [DataMember] public bool IsRequired { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public bool IsRequiredToClose { get; set; }
    [DataMember] public string Mask { get; set; }
    [DataMember]
    public int CustomFieldCategoryID { get; set; }
  }
  
}
