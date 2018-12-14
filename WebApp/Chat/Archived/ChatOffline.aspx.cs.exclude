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
  private Organization _organization;

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);

    try
    {
      if (Request["uid"] != null)
      {
        Organizations organizations = new Organizations(LoginUser.Anonymous);
        organizations.LoadByChatID(new Guid(Request["uid"]));
        if (organizations.IsEmpty) throw new Exception();
        _organization = organizations[0];
        if (_organization == null) throw new Exception();
      }
      else
      {
        throw new Exception();
      }
    }
    catch (Exception)
    {
      Response.Write("Unable to process your request.");
      Response.End();
    }
  }


  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    if (_organization == null) return;

    if (!IsPostBack)
    {
      
      if (Request.Cookies["TSChat"] != null)
      {
        textEmail.Text = Request.Cookies["TSChat"]["Email"];
        textFirstName.Text = Request.Cookies["TSChat"]["FirstName"];
        textLastName.Text = Request.Cookies["TSChat"]["LastName"];
      }

      if (Request.Params["email"] != null) textEmail.Text = Request.Params["email"];
      if (Request.Params["fname"] != null) textFirstName.Text = Request.Params["fname"];
      if (Request.Params["lname"] != null) textLastName.Text = Request.Params["lname"];
      if (Request.Params["msg"] != null) textMessage.Text = Request.Params["msg"];

      if (_organization.OrganizationID == 566596)
      {
        pnlChatForm.Visible = false;
        pnlCustom.InnerHtml = "<p style=\"font-weight:bold;\">Lo sentimos, por el momento no hay un ejecutivo disponible.</p><p>Por favor de click <a href=\"https://ticket.teamsupport.com/AMCO\" target=\"_blank\">aquí</a> para solicitar asistencia y nos comunicaremos con usted en un máximo de 24 horas.</p>";
        pnlQuestion.Visible = false;
        headTitle.Visible = false;
      }

      if (_organization.OrganizationID == 748444)
      {
        pnlChatForm.Visible = false;
        pnlCustom.InnerHtml = "<p style=\"font-weight:bold;\">Lo sentimos, por el momento no hay un ejecutivo disponible.</p><p>Por favor de click <a href=\"https://ticket.teamsupport.com/iditic\" target=\"_blank\">aquí</a> para solicitar asistencia y nos comunicaremos con usted en un máximo de 24 horas.</p>";
        pnlQuestion.Visible = false;
        headTitle.Visible = false;
      }

    }
  }

  protected void btnSubmit_Click(object sender, EventArgs e)
  {
    try
    {
      Response.Cookies["TSChat"]["Email"] = textEmail.Text;
      Response.Cookies["TSChat"]["FirstName"] = textFirstName.Text;
      Response.Cookies["TSChat"]["LastName"] = textLastName.Text;
      Response.Cookies["TSChat"].Expires = DateTime.UtcNow.AddYears(14);
    }
    catch (Exception)
    {
      
    }


    Ticket ticket = (new Tickets(LoginUser.Anonymous)).AddNewTicket();
    ticket.OrganizationID = _organization.OrganizationID;
    ticket.GroupID = _organization.DefaultPortalGroupID;
    ticket.IsKnowledgeBase = false;
    ticket.IsVisibleOnPortal = true;
    ticket.Name = "Offline Chat Question";
    ticket.TicketSeverityID = TicketSeverities.GetTop(LoginUser.Anonymous, _organization.OrganizationID).TicketSeverityID;
    ticket.TicketTypeID = TicketTypes.GetTop(LoginUser.Anonymous, _organization.OrganizationID).TicketTypeID;
    ticket.TicketStatusID = TicketStatuses.GetTop(LoginUser.Anonymous, ticket.TicketTypeID).TicketStatusID;
    ticket.TicketSource = "ChatOffline";
    ticket.PortalEmail = textEmail.Text;
    ticket.Collection.Save();

    StringBuilder builder = new StringBuilder();
    builder.Append("<h2>Offline Chat Request</h2>");
    builder.Append("<table cellspacing=\"0\" cellpadding=\"5\" border=\"0\">");
    builder.Append("<tr><td><strong>First Name:</strong></td><td>" + textFirstName.Text + "</td></tr>");
    builder.Append("<tr><td><strong>Last Name:</strong></td><td>" + textLastName.Text + "</td></tr>");
    builder.Append("<tr><td><strong>Email:</strong></td><td><a href=\"mailto:"+ textEmail.Text+"\">" + textEmail.Text + "</td></tr>");
    builder.Append("<tr><td colspan=\"2\"><strong>Question:</strong></td></tr>");
    builder.Append("<tr><td colspan=\"2\">"+textMessage.Text+"</td></tr>");
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
    users.LoadByEmailOrderByActive(_organization.OrganizationID, textEmail.Text);
    if (!users.IsEmpty) ticket.Collection.AddContact(users[0].UserID, ticket.TicketID);

    Response.Redirect("OfflineThanks.aspx");



  }
}
