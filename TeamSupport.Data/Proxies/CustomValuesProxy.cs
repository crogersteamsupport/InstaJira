using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  
  public partial class CustomValue : BaseItem
  {
    public CustomValueProxy GetProxy()
    {
      CustomValueProxy result = new CustomValueProxy();
      result.ImportFileID = this.ImportFileID;
      result.ModifierID = Row["ModifierID"] == DBNull.Value ? -1 : this.ModifierID;
      result.CreatorID = Row["CreatorID"] == DBNull.Value ? -1 : this.CreatorID;
      result.RefID = Row["RefID"] == DBNull.Value ? null : (int?)this.RefID;
      result.CustomFieldID = this.CustomFieldID;
      result.CustomValueID = Row["CustomValueID"] == DBNull.Value ? null : (int?)this.CustomValueID;
       
      result.DateCreated = DateTime.SpecifyKind(Row["DateCreated"] == DBNull.Value ? DateTime.MinValue : this.DateCreated, DateTimeKind.Local);
      result.DateModified = DateTime.SpecifyKind(Row["DateModified"] == DBNull.Value ? DateTime.MinValue : this.DateModified, DateTimeKind.Local);
       
      result.FieldName = this.FieldName;
      result.ApiFieldName = this.ApiFieldName;
      result.ListValues = this.ListValues;
      result.FieldType = this.FieldType;
      result.Name = this.Name;
      result.Description = this.Description;
      result.RefType = this.RefType;
      result.AuxID = this.AuxID;
      result.Position = this.Position;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.IsFirstIndexSelect = this.IsFirstIndexSelect;
      result.IsRequired = this.IsRequired;
      result.OrganizationID = this.OrganizationID;
      result.IsRequiredToClose = this.IsRequiredToClose;
      result.Mask = this.Mask;
      result.CustomFieldCategoryID = this.CustomFieldCategoryID;
	  switch (this.FieldType)
      {
				case CustomFieldType.DateTime:
				case CustomFieldType.Time:
        result.Value = null;
        if (Row["CustomValue"] != DBNull.Value)
        {
          DateTime date;
          if (DateTime.TryParse(this.Value, out date))
          { 
            date = DateTime.SpecifyKind(DateTime.Parse(this.Value), DateTimeKind.Utc);
            result.Value = date;
          }
        }
					break;
				case CustomFieldType.Date:
					result.Value = null;
					if (Row["CustomValue"] != DBNull.Value)
					{
						DateTime date;
						if (DateTime.TryParse(this.Value, out date))
						{
							result.Value = this.Value;
      }
					}
					break;
				default:
        result.Value = Row["CustomValue"] == DBNull.Value ? "" : this.Value;
					break;
      }

      return result;
    }	
  }
}
