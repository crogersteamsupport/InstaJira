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
  
  public class RestSlaViolationHistory
  {
    public static string GetSlaViolationHistoryItem(RestCommand command, int slaViolationHistoryID)
    {
      SlaViolationHistoryItem slaViolationHistoryItem = SlaViolationHistory.GetSlaViolationHistoryItem(command.LoginUser, slaViolationHistoryID);
      if (slaViolationHistoryItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return slaViolationHistoryItem.GetXml("SlaViolationHistoryItem", true);
    }
    
    public static string GetSlaViolationHistory(RestCommand command)
    {
      SlaViolationHistory slaViolationHistory = new SlaViolationHistory(command.LoginUser);
      slaViolationHistory.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return slaViolationHistory.GetXml("SlaViolationHistory", "SlaViolationHistoryItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
