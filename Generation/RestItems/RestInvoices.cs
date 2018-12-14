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
  
  public class RestInvoices
  {
    public static string GetInvoice(RestCommand command, int invoiceID)
    {
      Invoice invoice = Invoices.GetInvoice(command.LoginUser, invoiceID);
      if (invoice.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return invoice.GetXml("Invoice", true);
    }
    
    public static string GetInvoices(RestCommand command)
    {
      Invoices invoices = new Invoices(command.LoginUser);
      invoices.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return invoices.GetXml("Invoices", "Invoice", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
