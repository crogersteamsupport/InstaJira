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
  
  public class RestCompanyParentsView
  {
    public static string GetCompanyParentsViewItem(RestCommand command, int childID)
    {
      CompanyParentsViewItem companyParentsViewItem = CompanyParentsView.GetCompanyParentsViewItem(command.LoginUser, childID);
      if (companyParentsViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return companyParentsViewItem.GetXml("CompanyParentsViewItem", true);
    }
    
    public static string GetCompanyParentsView(RestCommand command)
    {
      CompanyParentsView companyParentsView = new CompanyParentsView(command.LoginUser);
      companyParentsView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return companyParentsView.GetXml("CompanyParentsView", "CompanyParentsViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
