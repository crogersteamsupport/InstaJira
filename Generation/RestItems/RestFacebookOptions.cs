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
  
  public class RestFacebookOptions
  {
    public static string GetFacebookOption(RestCommand command, int organizationID)
    {
      FacebookOption facebookOption = FacebookOptions.GetFacebookOption(command.LoginUser, organizationID);
      if (facebookOption.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return facebookOption.GetXml("FacebookOption", true);
    }
    
    public static string GetFacebookOptions(RestCommand command)
    {
      FacebookOptions facebookOptions = new FacebookOptions(command.LoginUser);
      facebookOptions.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return facebookOptions.GetXml("FacebookOptions", "FacebookOption", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
