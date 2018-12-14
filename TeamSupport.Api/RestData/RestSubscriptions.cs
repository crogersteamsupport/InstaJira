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
  public class RestSubscriptions
  {
    public static string SubscribeToTicket(RestCommand command, int ticketIDOrNumber, int userId)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketIDOrNumber);

      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      UsersViewItem subscribingUser = UsersView.GetUsersViewItem(command.LoginUser, userId);
      if (subscribingUser == null || subscribingUser.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      Subscriptions.AddSubscription(command.LoginUser, userId, ReferenceType.Tickets, ticket.TicketID);

      return ticket.GetXml("Ticket", true);
    }

    public static string UnSubscribeFromTicket(RestCommand command, int ticketIDOrNumber, int userId)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketIDOrNumber);

      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      Subscriptions.RemoveSubscription(command.LoginUser, userId, ReferenceType.Tickets, ticket.TicketID);

      return ticket.GetXml("Ticket", true);
    }
  }
}
