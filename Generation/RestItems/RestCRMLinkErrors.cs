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
  
  public class RestCRMLinkErrors
  {
    public static string GetCRMLinkError(RestCommand command, int cRMLinkErrorID)
    {
      CRMLinkError cRMLinkError = CRMLinkErrors.GetCRMLinkError(command.LoginUser, cRMLinkErrorID);
      if (cRMLinkError.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return cRMLinkError.GetXml("CRMLinkError", true);
    }
    
    public static string GetCRMLinkErrors(RestCommand command)
    {
      CRMLinkErrors cRMLinkErrors = new CRMLinkErrors(command.LoginUser);
      cRMLinkErrors.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return cRMLinkErrors.GetXml("CRMLinkErrors", "CRMLinkError", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
