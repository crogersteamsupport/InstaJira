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
  
  public class RestExceptionLogs
  {
    public static string GetExceptionLog(RestCommand command, int exceptionLogID)
    {
      ExceptionLog exceptionLog = ExceptionLogs.GetExceptionLog(command.LoginUser, exceptionLogID);
      if (exceptionLog.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return exceptionLog.GetXml("ExceptionLog", true);
    }
    
    public static string GetExceptionLogs(RestCommand command)
    {
      ExceptionLogs exceptionLogs = new ExceptionLogs(command.LoginUser);
      exceptionLogs.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return exceptionLogs.GetXml("ExceptionLogs", "ExceptionLog", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
