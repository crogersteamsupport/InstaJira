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
  
  public class RestTSEMailIgnoreList
  {
    public static string GetTSEMailIgnoreListItem(RestCommand command, int ignoreID)
    {
      TSEMailIgnoreListItem tSEMailIgnoreListItem = TSEMailIgnoreList.GetTSEMailIgnoreListItem(command.LoginUser, ignoreID);
      if (tSEMailIgnoreListItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return tSEMailIgnoreListItem.GetXml("TSEMailIgnoreListItem", true);
    }
    
    public static string GetTSEMailIgnoreList(RestCommand command)
    {
      TSEMailIgnoreList tSEMailIgnoreList = new TSEMailIgnoreList(command.LoginUser);
      tSEMailIgnoreList.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return tSEMailIgnoreList.GetXml("TSEMailIgnoreList", "TSEMailIgnoreListItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
