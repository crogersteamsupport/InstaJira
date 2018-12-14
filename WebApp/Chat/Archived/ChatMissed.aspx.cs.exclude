using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Text;

public partial class Chat_ChatOffline : System.Web.UI.Page
{


  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    if (!IsPostBack)
    {
      if (Request["rid"] != null)
      {
        ChatRequest request = ChatRequests.GetChatRequest(LoginUser.Anonymous, int.Parse(Request["rid"]));
        Chat chat = Chats.GetChat(LoginUser.Anonymous, request.ChatID);
        Organization organization = Organizations.GetOrganization(LoginUser.Anonymous, chat.OrganizationID);
        ChatClient client = ChatClients.GetChatClient(LoginUser.Anonymous, request.RequestorID);

        Ticket ticket = (new Tickets(LoginUser.Anonymous)).AddNewTicket();
        ticket.OrganizationID = organization.OrganizationID;
        ticket.GroupID = organization.DefaultPortalGroupID;
        ticket.IsKnowledgeBase = false;
        ticket.IsVisibleOnPortal = true;
        ticket.Name = "Missed Chat";
        ticket.TicketSeverityID = TicketSeverities.GetTop(LoginUser.Anonymous, organization.OrganizationID).TicketSeverityID;
        ticket.TicketTypeID = TicketTypes.GetTop(LoginUser.Anonymous, organization.OrganizationID).TicketTypeID;
        ticket.TicketStatusID = TicketStatuses.GetTop(LoginUser.Anonymous, ticket.TicketTypeID).TicketStatusID;
        ticket.TicketSource = "ChatOffline";
        ticket.PortalEmail = client.Email;
        ticket.Collection.Save();

        StringBuilder builder = new StringBuilder();
        builder.Append("<h2>Missed Chat Request</h2>");
        builder.Append("<table cellspacing=\"0\" cellpadding=\"5\" border=\"0\">");
        builder.Append("<tr><td><strong>First Name:</strong></td><td>" + client.FirstName + "</td></tr>");
        builder.Append("<tr><td><strong>Last Name:</strong></td><td>" + client.LastName + "</td></tr>");
        builder.Append("<tr><td><strong>Email:</strong></td><td><a href=\"mailto:" + client.Email + "\">" + client.Email + "</td></tr>");
        builder.Append("<tr><td colspan=\"2\"><strong>Question:</strong></td></tr>");
        builder.Append("<tr><td colspan=\"2\">" + request.Message + "</td></tr>");
        builder.Append("</table>");


        TeamSupport.Data.Action action = (new Actions(LoginUser.Anonymous)).AddNewAction();
        action.ActionTypeID = null;
        action.SystemActionTypeID = SystemActionType.Description;
        action.Description = builder.ToString();
        action.IsKnowledgeBase = false;
        action.IsVisibleOnPortal = true;
        action.ActionSource = "ChatOffline";
        action.Name = "Description";
        action.TicketID = ticket.TicketID;
        action.Collection.Save();

        Users users = new Users(LoginUser.Anonymous);
        users.LoadByEmailOrderByActive(organization.OrganizationID, client.Email);
        if (!users.IsEmpty) ticket.Collection.AddContact(users[0].UserID, ticket.TicketID);
      }
      else
      {
        throw new Exception();
      }

    }
  }

}
