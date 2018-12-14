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
  public class RestActions
  {

    public static string GetAction(RestCommand command, int actionID)
    {
      ActionsViewItem action = ActionsView.GetActionsViewItem(command.LoginUser, actionID);
      if (action.OrganizationID != command.Organization.OrganizationID)
      {
        throw new RestException(HttpStatusCode.Unauthorized);
      }

      action.Description = RemoveInvalidXmlChars(action.Description);

      return action.GetXml("Action", true);
    }

    public static string GetActions(RestCommand command, int ticketIDOrNumber, int? limitNumber = null)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketIDOrNumber);
      if (ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      ActionsView actions = new ActionsView(command.LoginUser);
      actions.LoadByTicketID(ticket.TicketID, limitNumber);

      actions.Select(p => { p.Description = RemoveInvalidXmlChars(p.Description); return p; }).ToList();

      return actions.GetXml("Actions", "Action", true, command.Filters);
    }

    public static string CreateAction(RestCommand command, int ticketIDOrNumber)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketIDOrNumber);
      if (ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      Actions actions = new Actions(command.LoginUser);
      TeamSupport.Data.Action action = actions.AddNewAction();
      action.TicketID = ticket.TicketID;
      action.ReadFromXml(command.Data, true);
      action.Collection.Save();
      action.UpdateCustomFieldsFromXml(command.Data);
      return ActionsView.GetActionsViewItem(command.LoginUser, action.ActionID).GetXml("Action", true);
    }

    public static string UpdateAction(RestCommand command, int actionID)
    {
      TeamSupport.Data.Action action = Actions.GetAction(command.LoginUser, actionID);
      if (action == null) throw new RestException(HttpStatusCode.BadRequest);
      Ticket ticket = Tickets.GetTicket(command.LoginUser, action.TicketID);
      if (ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      action.ReadFromXml(command.Data, false);
      action.Collection.Save();
      action.UpdateCustomFieldsFromXml(command.Data);
      return ActionsView.GetActionsViewItem(command.LoginUser, action.ActionID).GetXml("Action", true);
    }

    public static string GetCustomerAction(RestCommand command, int actionID)
    {
      ActionsViewItem action = ActionsView.GetActionsViewItem(command.LoginUser, actionID);
      TicketsViewItem ticket = action.GetTicket();
      if (ticket.OrganizationID != command.Organization.ParentID || !ticket.GetIsCustomer(command.Organization.OrganizationID))
      {
        throw new RestException(HttpStatusCode.Unauthorized);
      }

      return action.GetXml("Action", true);
    }

    public static string GetCustomerActions(RestCommand command, int ticketID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumberForCustomer(command.LoginUser, (int)command.Organization.ParentID, ticketID);
      if (ticket.OrganizationID != command.Organization.ParentID || !ticket.GetIsCustomer(command.Organization.OrganizationID)) throw new RestException(HttpStatusCode.Unauthorized);
      ActionsView actions = new ActionsView(command.LoginUser);
      actions.LoadByTicketID(ticket.TicketID);

      return actions.GetXml("Actions", "Action", true, command.Filters);
    }

    public static string CreateCustomerAction(RestCommand command, int ticketID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(command.LoginUser, ticketID);
      if (ticket.OrganizationID != command.Organization.ParentID || !ticket.GetIsCustomer(command.Organization.OrganizationID)) throw new RestException(HttpStatusCode.Unauthorized);

      Actions actions = new Actions(command.LoginUser);
      TeamSupport.Data.Action action = actions.AddNewAction();
      action.TicketID = ticketID;
      action.ReadFromXml(command.Data, true);
      action.Collection.Save();
      action.UpdateCustomFieldsFromXml(command.Data);
      return ActionsView.GetActionsViewItem(command.LoginUser, action.ActionID).GetXml("Action", true);
    }

    public static string UpdateCustomerAction(RestCommand command, int actionID)
    {
      TeamSupport.Data.Action action = Actions.GetAction(command.LoginUser, actionID);
      if (action == null) throw new RestException(HttpStatusCode.BadRequest);
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(command.LoginUser, action.TicketID);
      if (ticket.OrganizationID != command.Organization.ParentID || !ticket.GetIsCustomer(command.Organization.OrganizationID)) throw new RestException(HttpStatusCode.Unauthorized);

      action.ReadFromXml(command.Data, false);
      action.Collection.Save();
      action.UpdateCustomFieldsFromXml(command.Data);
      return ActionsView.GetActionsViewItem(command.LoginUser, action.ActionID).GetXml("Action", true);
    }

    private static string RemoveInvalidXmlChars(string text)
    {
      var validXmlChars = text.Where(ch => XmlConvert.IsXmlChar(ch)).ToArray();
      return new string(validXmlChars);
    }

    private static bool IsValidXmlString(string text)
    {
      try
      {
        XmlConvert.VerifyXmlChars(text);
        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}
