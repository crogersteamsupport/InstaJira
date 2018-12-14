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
  
  public class RestCustomFields
  {
    public static string GetCustomField(RestCommand command, int customFieldID)
    {
      CustomField customField = CustomFields.GetCustomField(command.LoginUser, customFieldID);
      if (customField.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return customField.GetXml("CustomField", true);
    }
    
    public static string GetCustomFields(RestCommand command)
    {
      CustomFields customFields = new CustomFields(command.LoginUser);
      customFields.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return customFields.GetXml("CustomFields", "CustomField", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
