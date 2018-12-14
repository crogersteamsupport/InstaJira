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
  
  public class RestSourceCommitLogs
  {
    public static string GetSourceCommitLog(RestCommand command, int commitID)
    {
      SourceCommitLog sourceCommitLog = SourceCommitLogs.GetSourceCommitLog(command.LoginUser, commitID);
      if (sourceCommitLog.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return sourceCommitLog.GetXml("SourceCommitLog", true);
    }
    
    public static string GetSourceCommitLogs(RestCommand command)
    {
      SourceCommitLogs sourceCommitLogs = new SourceCommitLogs(command.LoginUser);
      sourceCommitLogs.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return sourceCommitLogs.GetXml("SourceCommitLogs", "SourceCommitLog", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
