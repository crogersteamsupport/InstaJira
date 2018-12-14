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
  
  public class RestEMailAlternateInbound
  {
    public static string GetEMailAlternateInboundItem(RestCommand command, int systemEMailID)
    {
      EMailAlternateInboundItem eMailAlternateInboundItem = EMailAlternateInbound.GetEMailAlternateInboundItem(command.LoginUser, systemEMailID);
      if (eMailAlternateInboundItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return eMailAlternateInboundItem.GetXml("EMailAlternateInboundItem", true);
    }
    
    public static string GetEMailAlternateInbound(RestCommand command)
    {
      EMailAlternateInbound eMailAlternateInbound = new EMailAlternateInbound(command.LoginUser);
      eMailAlternateInbound.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return eMailAlternateInbound.GetXml("EMailAlternateInbound", "EMailAlternateInboundItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
