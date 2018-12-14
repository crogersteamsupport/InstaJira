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
  
  public class RestLoginHistory
  {
    public static string GetLoginHistoryItem(RestCommand command, int loginHistoryID)
    {
      LoginHistoryItem loginHistoryItem = LoginHistory.GetLoginHistoryItem(command.LoginUser, loginHistoryID);
      if (loginHistoryItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return loginHistoryItem.GetXml("LoginHistoryItem", true);
    }
    
    public static string GetLoginHistory(RestCommand command)
    {
      LoginHistory loginHistory = new LoginHistory(command.LoginUser);
      loginHistory.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return loginHistory.GetXml("LoginHistory", "LoginHistoryItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
