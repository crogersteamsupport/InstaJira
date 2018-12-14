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
  
  public class RestCreditCards
  {
    public static string GetCreditCard(RestCommand command, int creditCardID)
    {
      CreditCard creditCard = CreditCards.GetCreditCard(command.LoginUser, creditCardID);
      if (creditCard.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return creditCard.GetXml("CreditCard", true);
    }
    
    public static string GetCreditCards(RestCommand command)
    {
      CreditCards creditCards = new CreditCards(command.LoginUser);
      creditCards.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return creditCards.GetXml("CreditCards", "CreditCard", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
