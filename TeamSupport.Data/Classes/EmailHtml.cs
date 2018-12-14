using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamSupport.Data
{
  public class EmailHtml
  {
    public static string GetEmailHtml(string title, string body)
    {
      string html =
@"
<html>
  <head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=iso-8859-1"">
    <title>TeamSupport | {0} </title>
  </head>
  <body style=""padding:0; margin:0; color:#15428B; font-size:14px;"" bgcolor=""#ffffff"">
  {1}
  </body>
</html>";

      return String.Format(html, title, body);

    }

    public static string GetTicketTitle(int ticketNumber, string ticketName)
    {
      string title = @"
<div style=""padding: 10px 0 10px 0;"">
  <div style=""background-color: #EDF0F5; color: #004394; padding: 0px 0 0px 5px; border: solid 1px #9FB0CF; font-size: 18px; font-weight: bold;"" class=""ui-corner-all"">
    <div><span style=""font-size: 18px;"">Ticket {0}</span></div>
    <div><span style=""font-size: 16px;"">{1}</span></div>
  </div>
</div>";
      return String.Format(title, ticketNumber.ToString(), ticketName);
    }

    public static string GetTicketProperties(LoginUser loginUser, int ticketID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(loginUser, ticketID);
      if (ticket == null) return "";

      StringBuilder builder = new StringBuilder();
      builder.Append(@"<div style=""background-color: #EDF0F5; color: #004394; margin-bottom:10px; padding-left:5px; border: solid 1px #9FB0CF; font-size: 18px; font-weight: bold;"" class=""ui-corner-all"">Ticket Properties</div>");
      builder.Append(@"<table width=""100%"" cellpadding=""0"" cellspacing=""5"" border=""0"">");

      builder.Append("<tr>");
      builder.Append(GetColumn("Opened By", ticket.CreatorName));
      builder.Append(GetColumn("Opened On", ticket.DateCreated.ToString("g", loginUser.CultureInfo)));
      builder.Append("</tr>");

      builder.Append("<tr>");
      builder.Append(GetColumn("Last Modified By", ticket.ModifierName));
      builder.Append(GetColumn("Last Modified On", ticket.DateModified.ToString("g", loginUser.CultureInfo)));

      builder.Append("</tr>");

      builder.Append("<tr>");
      if (ticket.IsClosed) builder.Append(GetColumn("Days Closed", ticket.DaysClosed.ToString()));
      else builder.Append(GetColumn("Days Opened", ticket.DaysOpened.ToString()));

      builder.Append(GetColumn("Total Time Spent", DataUtils.MinutesToDisplayTime(Tickets.GetTicketActionTime(ticket.Collection.LoginUser, ticket.TicketID))));
      builder.Append("</tr>");


      if (ticket.IsClosed && ticket.DateClosed != null)
      {
        builder.Append("<tr>");
        if (ticket.CloserID != null)
        {
          builder.Append(GetColumn("Closed By", ticket.CloserName));
        }
        builder.Append(GetColumn("Closed On", ((DateTime)ticket.DateClosed).ToString("g", loginUser.CultureInfo)));
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

      CustomFields fields = new CustomFields(ticket.Collection.LoginUser);
      fields.LoadByTicketTypeID(ticket.OrganizationID, ticket.TicketTypeID);

      bool flag = false;
      foreach (CustomField field in fields)
      {
        flag = !flag;
        if (flag) builder.Append("<tr>");

        CustomValue value;
        value = CustomValues.GetValue(ticket.Collection.LoginUser, field.CustomFieldID, ticket.TicketID);
        builder.Append(GetColumn(field.Name + "", value.Value));
        if (!flag) builder.Append("</tr>");

      }
      if (flag) builder.Append("</tr>");
      builder.Append("</table>");

      return builder.ToString();
    }

    public static string GetTicketCustomers(LoginUser loginUser, int ticketID)
    {
      Organizations organizations = new Organizations(loginUser);
      organizations.LoadByTicketID(ticketID);

      StringBuilder builder = new StringBuilder();

      builder.Append(@"<div style=""background-color: #EDF0F5; color: #004394; padding: 0px 0 0px 5px; border: solid 1px #9FB0CF; font-size: 18px; font-weight: bold;"" class=""ui-corner-all"">Customers</div>
                      <div style=""padding: 7px 7px"">");

      for (int i = 0; i < organizations.Count; i++)
      {
        if (i == 0) builder.Append(organizations[i].Name); else builder.Append(string.Format(", &nbsp&nbsp {0}", organizations[i].Name));
      }

      if (organizations.Count < 1) builder.Append("[No Customers]");


      builder.Append("</div>");
      return builder.ToString();

    }

    public static string GetTicketHistory(LoginUser loginUser, int ticketID)
    {
      return GetTicketHistory(loginUser, ticketID, null);
    }

    public static string GetTicketHistory(LoginUser loginUser, int ticketID, DateTime? minUtcDate)
    {
      ActionLogs logs = new ActionLogs(loginUser);
      if (minUtcDate == null) logs.LoadByTicketID(ticketID); else logs.LoadByTicketID(ticketID, (DateTime)minUtcDate);

      if (logs.IsEmpty) return "";

      StringBuilder builder = new StringBuilder();
      builder.Append(@"<div style=""background-color: #EDF0F5; color: #004394; margin-bottom:10px; padding-left:5px; border: solid 1px #9FB0CF; font-size: 18px; font-weight: bold;"" class=""ui-corner-all"">Recent History</div>");

      foreach (ActionLog log in logs)
      {
        builder.Append(string.Format("<div style=\" color: #004394; padding-left:10px; \"><strong>{0}</strong>: {1} - <span style=\"font-style: italic;\">{2}</span></div>", log.CreatorName, log.Description, log.DateCreated.ToString("g", loginUser.CultureInfo)));
      }

      return builder.ToString();
    }

    public static string GetTicketActions(LoginUser loginUser, int ticketID, bool onlyVisibleOnPortal)
    {

      ActionsView actions = new ActionsView(loginUser);
      actions.LoadByTicketID(ticketID, onlyVisibleOnPortal);

      StringBuilder builder = new StringBuilder();
      builder.Append(@"<div style=""background-color: #EDF0F5; color: #004394; margin-bottom:10px; padding-left:5px; border: solid 1px #9FB0CF; font-size: 18px; font-weight: bold;"" class=""ui-corner-all"">Actions</div>");
      foreach (ActionsViewItem action in actions)
      {
        string actionTitle = action.Name;
        if (action.Name != action.ActionType)
        {
          actionTitle = action.ActionType + ": " + actionTitle;
        }

        builder.Append(string.Format("<div style=\"color: #004394; font-size: 16px; font-weight:bold; background-color: #F0F4F7; padding: 2px 0 2px 5px;\" class=\"ui-corner-all\">{0}</div>", actionTitle));
        builder.Append(string.Format("<div style=\"padding: 10px 10px 10px 10px; \">{0}</div>", action.Description));
        builder.Append(string.Format("<div style=\"color: #004394; font-style: italic; border-top: dotted 1px #15428B; padding: 10px 3px 20px 3px; \">- {0} {1}</div>",
          action.CreatorName, action.DateCreated.ToString("g", loginUser.CultureInfo)));
      }

      return builder.ToString();
    }

    private static string GetColumn(string caption, string value)
    {
      return string.Format("<td><strong>{0}</strong>: &nbsp&nbsp {1}</td>", caption, value);
    }
  
  }
}
