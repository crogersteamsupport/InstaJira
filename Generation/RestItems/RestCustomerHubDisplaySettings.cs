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
  
  public class RestCustomerHubDisplaySettings
  {
    public static string GetCustomerHubDisplaySetting(RestCommand command, int customerHubDisplaySettingID)
    {
      CustomerHubDisplaySetting customerHubDisplaySetting = CustomerHubDisplaySettings.GetCustomerHubDisplaySetting(command.LoginUser, customerHubDisplaySettingID);
      if (customerHubDisplaySetting.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return customerHubDisplaySetting.GetXml("CustomerHubDisplaySetting", true);
    }
    
    public static string GetCustomerHubDisplaySettings(RestCommand command)
    {
      CustomerHubDisplaySettings customerHubDisplaySettings = new CustomerHubDisplaySettings(command.LoginUser);
      customerHubDisplaySettings.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return customerHubDisplaySettings.GetXml("CustomerHubDisplaySettings", "CustomerHubDisplaySetting", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
