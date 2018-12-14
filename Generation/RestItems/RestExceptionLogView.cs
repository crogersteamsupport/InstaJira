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
  
  public class RestExceptionLogView
  {
    public static string GetExceptionLogViewItem(RestCommand command, int exceptionLogID)
    {
      ExceptionLogViewItem exceptionLogViewItem = ExceptionLogView.GetExceptionLogViewItem(command.LoginUser, exceptionLogID);
      if (exceptionLogViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return exceptionLogViewItem.GetXml("ExceptionLogViewItem", true);
    }
    
    public static string GetExceptionLogView(RestCommand command)
    {
      ExceptionLogView exceptionLogView = new ExceptionLogView(command.LoginUser);
      exceptionLogView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return exceptionLogView.GetXml("ExceptionLogView", "ExceptionLogViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
