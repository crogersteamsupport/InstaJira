using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using TeamSupport.Data;
using System.Net;

namespace TeamSupport.Api
{
  
  public class RestCustomFieldCategories
  {
    public static string GetCustomFieldCategory(RestCommand command, int customFieldCategoryID)
    {
      CustomFieldCategory customFieldCategory = CustomFieldCategories.GetCustomFieldCategory(command.LoginUser, customFieldCategoryID);
      if (customFieldCategory.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return customFieldCategory.GetXml("CustomFieldCategory", true);
    }
    
    public static string GetCustomFieldCategories(RestCommand command)
    {
      CustomFieldCategories customFieldCategories = new CustomFieldCategories(command.LoginUser);
      customFieldCategories.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return customFieldCategories.GetXml("CustomFieldCategories", "CustomFieldCategory", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
