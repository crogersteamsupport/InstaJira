using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  
  public partial class CustomFieldCategory : BaseItem
  {
    public CustomFieldCategoryProxy GetProxy()
    {
      CustomFieldCategoryProxy result = new CustomFieldCategoryProxy();
      result.ProductFamilyID = this.ProductFamilyID;
      result.AuxID = this.AuxID;
      result.RefType = this.RefType;
      result.Position = this.Position;
      result.Category = this.Category;
      result.OrganizationID = this.OrganizationID;
      result.CustomFieldCategoryID = this.CustomFieldCategoryID;
      result.ProductFamilyName = this.ProductFamilyName;
       
      return result;
    }	
  }
}
