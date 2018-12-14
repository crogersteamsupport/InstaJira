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
  
  public class RestImports
  {
    public static string GetImport(RestCommand command, int importID)
    {
      Import import = Imports.GetImport(command.LoginUser, importID);
      if (import.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return import.GetXml("Import", true);
    }
    
    public static string GetImports(RestCommand command)
    {
      Imports imports = new Imports(command.LoginUser);
      imports.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return imports.GetXml("Imports", "Import", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
