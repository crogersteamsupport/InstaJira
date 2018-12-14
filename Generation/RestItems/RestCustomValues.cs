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
  
  public class RestCustomValues
  {
    public static string GetCustomValue(RestCommand command, int customValueID)
    {
      CustomValue customValue = CustomValues.GetCustomValue(command.LoginUser, customValueID);
      if (customValue.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return customValue.GetXml("CustomValue", true);
    }
    
    public static string GetCustomValues(RestCommand command)
    {
      CustomValues customValues = new CustomValues(command.LoginUser);
      customValues.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return customValues.GetXml("CustomValues", "CustomValue", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
