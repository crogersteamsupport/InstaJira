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
  
  public class RestSlaLevels
  {
    public static string GetSlaLevel(RestCommand command, int slaLevelID)
    {
      SlaLevel slaLevel = SlaLevels.GetSlaLevel(command.LoginUser, slaLevelID);
      if (slaLevel.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return slaLevel.GetXml("SlaLevel", true);
    }
    
    public static string GetSlaLevels(RestCommand command)
    {
      SlaLevels slaLevels = new SlaLevels(command.LoginUser);
      slaLevels.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return slaLevels.GetXml("SlaLevels", "SlaLevel", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
