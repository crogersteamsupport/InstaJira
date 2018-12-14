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
  [KnownType(typeof(CustomFieldsViewItemProxy))]
  public class CustomFieldsViewItemProxy
  {
    public CustomFieldsViewItemProxy() {}
    [DataMember] public int CustomFieldID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string ApiFieldName { get; set; }
    [DataMember] public int RefType { get; set; }
    [DataMember] public int FieldType { get; set; }
    [DataMember] public int AuxID { get; set; }
    [DataMember] public int Position { get; set; }
    [DataMember] public string ListValues { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public bool? IsVisibleOnPortal { get; set; }
    [DataMember] public bool IsFirstIndexSelect { get; set; }
    [DataMember] public bool IsRequired { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public int? CustomFieldCategoryID { get; set; }
    [DataMember] public bool IsRequiredToClose { get; set; }
    [DataMember] public string Mask { get; set; }
    [DataMember] public int? ParentCustomFieldID { get; set; }
    [DataMember] public string ParentCustomValue { get; set; }
    [DataMember] public int? ParentProductID { get; set; }
    [DataMember] public string ParentFieldName { get; set; }
    [DataMember] public string ParentProductName { get; set; }
          
  }
}
