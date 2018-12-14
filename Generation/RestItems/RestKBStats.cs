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
  
  public class RestKBStats
  {
    public static string GetKBStat(RestCommand command, int kBViewID)
    {
      KBStat kBStat = KBStats.GetKBStat(command.LoginUser, kBViewID);
      if (kBStat.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return kBStat.GetXml("KBStat", true);
    }
    
    public static string GetKBStats(RestCommand command)
    {
      KBStats kBStats = new KBStats(command.LoginUser);
      kBStats.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return kBStats.GetXml("KBStats", "KBStat", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
