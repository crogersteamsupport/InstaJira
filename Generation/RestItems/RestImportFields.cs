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
  
  public class RestImportFields
  {
    public static string GetImportField(RestCommand command, int importFieldID)
    {
      ImportField importField = ImportFields.GetImportField(command.LoginUser, importFieldID);
      if (importField.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return importField.GetXml("ImportField", true);
    }
    
    public static string GetImportFields(RestCommand command)
    {
      ImportFields importFields = new ImportFields(command.LoginUser);
      importFields.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return importFields.GetXml("ImportFields", "ImportField", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
