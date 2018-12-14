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
  
  public class RestBillingInfo
  {
    public static string GetBillingInfoItem(RestCommand command, int organizationID)
    {
      BillingInfoItem billingInfoItem = BillingInfo.GetBillingInfoItem(command.LoginUser, organizationID);
      if (billingInfoItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return billingInfoItem.GetXml("BillingInfoItem", true);
    }
    
    public static string GetBillingInfo(RestCommand command)
    {
      BillingInfo billingInfo = new BillingInfo(command.LoginUser);
      billingInfo.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return billingInfo.GetXml("BillingInfo", "BillingInfoItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
