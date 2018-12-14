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
  
  public class RestCustomFieldsView
  {
    public static string GetCustomFieldsViewItem(RestCommand command, int )
    {
      CustomFieldsViewItem customFieldsViewItem = CustomFieldsView.GetCustomFieldsViewItem(command.LoginUser, );
      if (customFieldsViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return customFieldsViewItem.GetXml("CustomFieldsViewItem", true);
    }
    
    public static string GetCustomFieldsView(RestCommand command)
    {
      CustomFieldsView customFieldsView = new CustomFieldsView(command.LoginUser);
      customFieldsView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return customFieldsView.GetXml("CustomFieldsView", "CustomFieldsViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
