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
  
  public class RestWaterCooler
  {
    public static string GetWaterCoolerItem(RestCommand command, int messageID)
    {
      WaterCoolerItem waterCoolerItem = WaterCooler.GetWaterCoolerItem(command.LoginUser, messageID);
      if (waterCoolerItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return waterCoolerItem.GetXml("WaterCoolerItem", true);
    }
    
    public static string GetWaterCooler(RestCommand command)
    {
      WaterCooler waterCooler = new WaterCooler(command.LoginUser);
      waterCooler.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return waterCooler.GetXml("WaterCooler", "WaterCoolerItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
