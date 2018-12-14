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
  
  public class RestEmailPostHistory
  {
    public static string GetEmailPostHistoryItem(RestCommand command, int emailPostID)
    {
      EmailPostHistoryItem emailPostHistoryItem = EmailPostHistory.GetEmailPostHistoryItem(command.LoginUser, emailPostID);
      if (emailPostHistoryItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return emailPostHistoryItem.GetXml("EmailPostHistoryItem", true);
    }
    
    public static string GetEmailPostHistory(RestCommand command)
    {
      EmailPostHistory emailPostHistory = new EmailPostHistory(command.LoginUser);
      emailPostHistory.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return emailPostHistory.GetXml("EmailPostHistory", "EmailPostHistoryItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
