using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  
  public partial class SimpleImportFieldsViewItem : BaseItem
  {
    public SimpleImportFieldsViewItemProxy GetProxy()
    {
      SimpleImportFieldsViewItemProxy result = new SimpleImportFieldsViewItemProxy();
      result.OrganizationID = this.OrganizationID;
      result.IsCustom = this.IsCustom;
      result.Position = this.Position;
      result.RefType = this.RefType;
      result.Description = (this.Description);
      result.IsRequired = this.IsRequired;
      result.IsVisible = this.IsVisible;
      result.Size = this.Size;
      result.DataType = this.DataType;
      result.Alias = this.Alias;
      result.FieldName = (this.FieldName);
      result.TableName = (this.TableName);
      result.ImportFieldID = this.ImportFieldID;
       
       
       
      return result;
    }	
  }
}
