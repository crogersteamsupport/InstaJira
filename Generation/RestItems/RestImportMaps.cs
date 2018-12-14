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
  
  public class RestImportMaps
  {
    public static string GetImportMap(RestCommand command, int importMapID)
    {
      ImportMap importMap = ImportMaps.GetImportMap(command.LoginUser, importMapID);
      if (importMap.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return importMap.GetXml("ImportMap", true);
    }
    
    public static string GetImportMaps(RestCommand command)
    {
      ImportMaps importMaps = new ImportMaps(command.LoginUser);
      importMaps.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return importMaps.GetXml("ImportMaps", "ImportMap", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
