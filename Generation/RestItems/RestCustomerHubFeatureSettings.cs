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
  
  public class RestCustomerHubFeatureSettings
  {
    public static string GetCustomerHubFeatureSetting(RestCommand command, int customerHubFeatureSettingID)
    {
      CustomerHubFeatureSetting customerHubFeatureSetting = CustomerHubFeatureSettings.GetCustomerHubFeatureSetting(command.LoginUser, customerHubFeatureSettingID);
      if (customerHubFeatureSetting.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return customerHubFeatureSetting.GetXml("CustomerHubFeatureSetting", true);
    }
    
    public static string GetCustomerHubFeatureSettings(RestCommand command)
    {
      CustomerHubFeatureSettings customerHubFeatureSettings = new CustomerHubFeatureSettings(command.LoginUser);
      customerHubFeatureSettings.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return customerHubFeatureSettings.GetXml("CustomerHubFeatureSettings", "CustomerHubFeatureSetting", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
