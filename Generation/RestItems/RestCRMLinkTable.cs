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
  
  public class RestCRMLinkTable
  {
    public static string GetCRMLinkTableItem(RestCommand command, int cRMLinkID)
    {
      CRMLinkTableItem cRMLinkTableItem = CRMLinkTable.GetCRMLinkTableItem(command.LoginUser, cRMLinkID);
      if (cRMLinkTableItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return cRMLinkTableItem.GetXml("CRMLinkTableItem", true);
    }
    
    public static string GetCRMLinkTable(RestCommand command)
    {
      CRMLinkTable cRMLinkTable = new CRMLinkTable(command.LoginUser);
      cRMLinkTable.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return cRMLinkTable.GetXml("CRMLinkTable", "CRMLinkTableItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
