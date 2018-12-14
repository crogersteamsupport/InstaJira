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
  
  public class RestTechDocs
  {
    public static string GetTechDoc(RestCommand command, int techDocID)
    {
      TechDoc techDoc = TechDocs.GetTechDoc(command.LoginUser, techDocID);
      if (techDoc.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return techDoc.GetXml("TechDoc", true);
    }
    
    public static string GetTechDocs(RestCommand command)
    {
      TechDocs techDocs = new TechDocs(command.LoginUser);
      techDocs.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return techDocs.GetXml("TechDocs", "TechDoc", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
