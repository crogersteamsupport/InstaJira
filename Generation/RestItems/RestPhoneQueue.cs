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
  
  public class RestPhoneQueue
  {
    public static string GetPhoneQueueItem(RestCommand command, int phoneQueueID)
    {
      PhoneQueueItem phoneQueueItem = PhoneQueue.GetPhoneQueueItem(command.LoginUser, phoneQueueID);
      if (phoneQueueItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return phoneQueueItem.GetXml("PhoneQueueItem", true);
    }
    
    public static string GetPhoneQueue(RestCommand command)
    {
      PhoneQueue phoneQueue = new PhoneQueue(command.LoginUser);
      phoneQueue.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return phoneQueue.GetXml("PhoneQueue", "PhoneQueueItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
