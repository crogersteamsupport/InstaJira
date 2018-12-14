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
  
  public class RestUserDevices
  {
    public static string GetUserDevice(RestCommand command, int userDeviceID)
    {
      UserDevice userDevice = UserDevices.GetUserDevice(command.LoginUser, userDeviceID);
      if (userDevice.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return userDevice.GetXml("UserDevice", true);
    }
    
    public static string GetUserDevices(RestCommand command)
    {
      UserDevices userDevices = new UserDevices(command.LoginUser);
      userDevices.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return userDevices.GetXml("UserDevices", "UserDevice", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
