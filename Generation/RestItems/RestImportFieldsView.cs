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
  
  public class RestImportFieldsView
  {
    public static string GetImportFieldsViewItem(RestCommand command, int )
    {
      ImportFieldsViewItem importFieldsViewItem = ImportFieldsView.GetImportFieldsViewItem(command.LoginUser, );
      if (importFieldsViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return importFieldsViewItem.GetXml("ImportFieldsViewItem", true);
    }
    
    public static string GetImportFieldsView(RestCommand command)
    {
      ImportFieldsView importFieldsView = new ImportFieldsView(command.LoginUser);
      importFieldsView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return importFieldsView.GetXml("ImportFieldsView", "ImportFieldsViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
