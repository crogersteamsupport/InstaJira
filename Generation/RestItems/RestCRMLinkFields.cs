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
  
  public class RestCRMLinkFields
  {
    public static string GetCRMLinkField(RestCommand command, int cRMFieldID)
    {
      CRMLinkField cRMLinkField = CRMLinkFields.GetCRMLinkField(command.LoginUser, cRMFieldID);
      if (cRMLinkField.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return cRMLinkField.GetXml("CRMLinkField", true);
    }
    
    public static string GetCRMLinkFields(RestCommand command)
    {
      CRMLinkFields cRMLinkFields = new CRMLinkFields(command.LoginUser);
      cRMLinkFields.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return cRMLinkFields.GetXml("CRMLinkFields", "CRMLinkField", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
