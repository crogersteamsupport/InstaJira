using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Text;

public partial class TicketPrint : System.Web.UI.Page
{
  private int _ticketID;

  protected void Page_Load(object sender, EventArgs e)
  {
    if (Request["TicketNumber"] != null)
    {
      try
      {
        int ticketNumber = int.Parse(Request["TicketNumber"]);
        Tickets tickets = new Tickets(UserSession.LoginUser);
        tickets.LoadByTicketNumber(UserSession.LoginUser.OrganizationID, ticketNumber);
        if (tickets.IsEmpty)
        {
          RedirectBadRequest();
          return;
        }
        _ticketID = tickets[0].TicketID;
      }
      catch (Exception)
      {
        RedirectBadRequest();
        return;
      }
    }
    else
    {
      try
      {
        _ticketID = int.Parse(Request["ticketid"]);
      }
      catch
      {
        RedirectBadRequest();
        return;
      }

    }

    if (!IsPostBack)
    {
      TicketGridViewItem ticket = TicketGridView.GetTicketGridViewItem(UserSession.LoginUser, _ticketID);
      if (ticket == null)
      {
        RedirectBadRequest();
        return;
      }

      WriteProperties(ticket);
      WriteCustomers(ticket);
      WriteActions(ticket);
    }


  }

  private void RedirectBadRequest()
  {
    Response.Write("Invalid ticket.");
    Response.End();
  }

  private void WriteProperties(TicketGridViewItem ticket)
  {
    lblTitle.Text = "Ticket Number: " + ticket.TicketNumber.ToString();
    lblDescription.Text = ticket.Name;
    StringBuilder builder = new StringBuilder();
    
    builder.Append("<tr>");
    builder.Append(GetColumn("Opened By", ticket.CreatorName));
    builder.Append(GetColumn("Opened On", ticket.DateCreated.ToString("g", UserSession.LoginUser.CultureInfo)));
    builder.Append("</tr>");

    builder.Append("<tr>");
    builder.Append(GetColumn("Last Modified By", ticket.ModifierName));
//    builder.Append("<td>Last Modified By:</td><td> <a href=\"../Default.aspx?UserID=" + ticket.ModifierID + "\" target=\"TSMain\">");
    builder.Append(GetColumn("Last Modified On", ticket.DateModified.ToString("g", UserSession.LoginUser.CultureInfo)));

    builder.Append("</tr>");

    builder.Append("<tr>");
    if (ticket.IsClosed) builder.Append(GetColumn("Days Closed", ticket.DaysClosed.ToString()));
    else builder.Append(GetColumn("Days Opened", ticket.DaysOpened.ToString() ));

    builder.Append(GetColumn("Total Time Spent", DataUtils.MinutesToDisplayTime(Tickets.GetTicketActionTime(UserSession.LoginUser, ticket.TicketID))));
    builder.Append("</tr>");


    if (ticket.IsClosed && ticket.DateClosed != null)
    {
      builder.Append("<tr>");
      if (ticket.CloserID != null)
      {
        builder.Append(GetColumn("Closed By", ticket.CloserName));
      }
      builder.Append(GetColumn("Closed On", ((DateTime)ticket.DateClosed).ToString("g", UserSession.LoginUser.CultureInfo)));
      builder.Append("</tr>");
    }


    builder.Append("<tr>");
    builder.Append(GetColumn("Ticket Type", ticket.TicketTypeName));
    builder.Append(GetColumn("Assigned Group", ticket.GroupName));
    builder.Append("</tr>");

    builder.Append("<tr>");
    builder.Append(GetColumn("Status", ticket.Status));
    builder.Append(GetColumn("Product", ticket.ProductName));
    builder.Append("</tr>");

    builder.Append("<tr>");
    builder.Append(GetColumn("Severity", ticket.Severity));
    builder.Append(GetColumn("Reported Version", ticket.ReportedVersion));
    builder.Append("</tr>");

    builder.Append("<tr>");
    builder.Append(GetColumn("Assigned To", ticket.UserName));
    builder.Append(GetColumn("Resolved Version", ticket.SolvedVersion));
    builder.Append("</tr>");

    builder.Append("<tr>");
    builder.Append(GetColumn("Visible On Portal", ticket.IsVisibleOnPortal.ToString()));
    builder.Append(GetColumn("Knowledge Base", ticket.IsKnowledgeBase.ToString()));
    builder.Append("</tr>");

	builder.Append("<tr>");
    builder.Append(GetColumn("Due Date", ticket.DueDate == null ? "" : ((DateTime)ticket.DueDate).ToString("g", UserSession.LoginUser.CultureInfo)));

    CustomFields fields = new CustomFields(UserSession.LoginUser);
    fields.LoadByTicketTypeID(UserSession.LoginUser.OrganizationID, ticket.TicketTypeID);

    bool flag = true;

    foreach (CustomField field in fields)
    {
      flag = !flag;
      if (flag) builder.Append("<tr>");

      CustomValue value;
      value = CustomValues.GetValue(UserSession.LoginUser, field.CustomFieldID, ticket.TicketID, false);
      builder.Append(GetColumn(field.Name + "", value.Value));
      if (!flag) builder.Append("</tr>");

    }

    if (flag) builder.Append("</tr>");

    litProperties.Text = builder.ToString();
  }

  private void WriteCustomers(TicketGridViewItem ticket)
  {
    StringBuilder builder = new StringBuilder();

    Organizations organizations = new Organizations(UserSession.LoginUser);
    organizations.LoadByTicketID(ticket.TicketID);

    int count = 0;
    foreach (Organization organization in organizations)
    {
      //if (count % 4 == 0) builder.Append("<tr>");

      //builder.Append(string.Format("<td>{0}</td>", (count+1).ToString() + ": " +  organization.Name));
      if (count == 0)
        builder.Append(string.Format("{0}", organization.Name));
      else
        builder.Append(string.Format(", &nbsp&nbsp {0}", organization.Name));
      count++;
      //if (count % 4 == 0) builder.Append("</tr>");
    }
    //if (count % 4 != 0) builder.Append("</tr>");
    if (organizations.Count < 1) builder.Append("[No Customers]");


    litCustomers.Text = builder.ToString();
  
  }

  private void WriteActions(TicketGridViewItem ticket)
  {

    ActionsView actions = new ActionsView(UserSession.LoginUser);
    actions.LoadByTicketID(ticket.TicketID);

    StringBuilder builder = new StringBuilder();
    foreach (ActionsViewItem action in actions)
    {
      string actionTitle = action.Name;
      if (action.Name != action.ActionType)
      {
        actionTitle = action.ActionType + ": " + actionTitle;
      }

      builder.Append(string.Format("<div class=\"actionHeaderDiv ui-corner-all\">{0}</div>", actionTitle));
      builder.Append(string.Format("<div class=\"actionBodyDiv\">{0}</div>", action.Description));
      builder.Append(string.Format("<div class=\"actionFooterDiv\">- {0} {1}</div>",
        action.CreatorName, action.DateCreated.ToString("g", UserSession.LoginUser.CultureInfo)));
    }

    litActions.Text = builder.ToString();
  }

  private string GetColumn(string caption, string value)
  {


    return string.Format("<td><strong>{0}</strong>: &nbsp&nbsp {1}</td>", caption, value);
  }


}
