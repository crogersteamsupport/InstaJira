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
  
  public class RestCRMLinkResults
  {
    public static string GetCRMLinkResult(RestCommand command, int cRMResultsID)
    {
      CRMLinkResult cRMLinkResult = CRMLinkResults.GetCRMLinkResult(command.LoginUser, cRMResultsID);
      if (cRMLinkResult.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return cRMLinkResult.GetXml("CRMLinkResult", true);
    }
    
    public static string GetCRMLinkResults(RestCommand command)
    {
      CRMLinkResults cRMLinkResults = new CRMLinkResults(command.LoginUser);
      cRMLinkResults.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return cRMLinkResults.GetXml("CRMLinkResults", "CRMLinkResult", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
