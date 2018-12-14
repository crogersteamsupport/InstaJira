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
  
  public class RestSimpleImportFieldsView
  {
    public static string GetSimpleImportFieldsViewItem(RestCommand command, int )
    {
      SimpleImportFieldsViewItem simpleImportFieldsViewItem = SimpleImportFieldsView.GetSimpleImportFieldsViewItem(command.LoginUser, );
      if (simpleImportFieldsViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return simpleImportFieldsViewItem.GetXml("SimpleImportFieldsViewItem", true);
    }
    
    public static string GetSimpleImportFieldsView(RestCommand command)
    {
      SimpleImportFieldsView simpleImportFieldsView = new SimpleImportFieldsView(command.LoginUser);
      simpleImportFieldsView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return simpleImportFieldsView.GetXml("SimpleImportFieldsView", "SimpleImportFieldsViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
