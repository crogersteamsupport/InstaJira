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
  
  public class RestFilePaths
  {
    public static string GetFilePath(RestCommand command, int iD)
    {
      FilePath filePath = FilePaths.GetFilePath(command.LoginUser, iD);
      if (filePath.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return filePath.GetXml("FilePath", true);
    }
    
    public static string GetFilePaths(RestCommand command)
    {
      FilePaths filePaths = new FilePaths(command.LoginUser);
      filePaths.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return filePaths.GetXml("FilePaths", "FilePath", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
