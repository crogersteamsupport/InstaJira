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
  
  public class RestActionTypes
  {
    public static string GetActionType(RestCommand command, int actionTypeID)
    {
      ActionType actionType = ActionTypes.GetActionType(command.LoginUser, actionTypeID);
      if (actionType.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return actionType.GetXml("ActionType", true);
    }
    
    public static string GetActionTypes(RestCommand command)
    {
      ActionTypes actionTypes = new ActionTypes(command.LoginUser);
      actionTypes.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return actionTypes.GetXml("ActionTypes", "ActionType", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
