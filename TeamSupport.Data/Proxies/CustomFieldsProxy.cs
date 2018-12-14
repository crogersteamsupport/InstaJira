using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  
  public partial class CustomField : BaseItem
  {
    public CustomFieldProxy GetProxy()
    {
      CustomFieldProxy result = new CustomFieldProxy();
      result.ParentProductID = this.ParentProductID;
      result.ParentCustomValue = this.ParentCustomValue;
      result.ParentCustomFieldID = this.ParentCustomFieldID;
      result.Mask = this.Mask;
      result.IsRequiredToClose = this.IsRequiredToClose;
      result.IsRequired = this.IsRequired;
      result.IsFirstIndexSelect = this.IsFirstIndexSelect;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.Description = this.Description;
      result.ListValues = this.ListValues;
      result.Position = this.Position;
      result.AuxID = this.AuxID;
      result.FieldType = this.FieldType;
      result.RefType = this.RefType;
      result.ApiFieldName = this.ApiFieldName;
      result.Name = this.Name;
      result.OrganizationID = this.OrganizationID;
      result.CustomFieldID = this.CustomFieldID;
      result.CustomFieldCategoryID = this.CustomFieldCategoryID;

      /*
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       */
       
       
      return result;
    }	
  }
}
