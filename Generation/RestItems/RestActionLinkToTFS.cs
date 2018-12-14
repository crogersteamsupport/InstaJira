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
  
  public class RestActionLinkToTFS
  {
    public static string GetActionLinkToTFSItem(RestCommand command, int id)
    {
      ActionLinkToTFSItem actionLinkToTFSItem = ActionLinkToTFS.GetActionLinkToTFSItem(command.LoginUser, id);
      if (actionLinkToTFSItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return actionLinkToTFSItem.GetXml("ActionLinkToTFSItem", true);
    }
    
    public static string GetActionLinkToTFS(RestCommand command)
    {
      ActionLinkToTFS actionLinkToTFS = new ActionLinkToTFS(command.LoginUser);
      actionLinkToTFS.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return actionLinkToTFS.GetXml("ActionLinkToTFS", "ActionLinkToTFSItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
