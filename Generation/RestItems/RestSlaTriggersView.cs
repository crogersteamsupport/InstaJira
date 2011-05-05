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
  
  public class RestSlaTriggersView
  {
    public static string GetSlaTriggersViewItem(RestCommand command, int slaTriggerID)
    {
      SlaTriggersViewItem slaTriggersViewItem = SlaTriggersView.GetSlaTriggersViewItem(command.LoginUser, slaTriggerID);
      if (slaTriggersViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return slaTriggersViewItem.GetXml("SlaTriggersViewItem", true);
    }
    
    public static string GetSlaTriggersView(RestCommand command)
    {
      SlaTriggersView slaTriggersView = new SlaTriggersView(command.LoginUser);
      slaTriggersView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return slaTriggersView.GetXml("SlaTriggersView", "SlaTriggersViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
