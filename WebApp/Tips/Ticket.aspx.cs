using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Text;
using System.Globalization;

public partial class Tips_Ticket : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
      if (Request["TicketID"] == null) EndResponse("Invalid Ticket");

      int ticketID = int.Parse(Request["TicketID"]);
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);
		
		CultureInfo us = new CultureInfo(TSAuthentication.GetLoginUser().CultureInfo.ToString());
		
      if (ticket == null) EndResponse("Invalid Ticket");

      if (ticket.SlaViolationTime < 0) tipSla.Attributes.Add("class", "ts-icon ts-icon-sla-bad");
      else if (ticket.SlaWarningTime < 0) tipSla.Attributes.Add("class", "ts-icon ts-icon-sla-warning");
      else tipSla.Attributes.Add("class", "ts-icon ts-icon-sla-good");
      

      tipNumber.InnerText = "Ticket #" + ticket.TicketNumber.ToString();
      tipNumber.Attributes.Add("onclick", "top.Ts.MainPage.openTicket(" + ticket.TicketNumber + "); return false;");
      tipName.InnerHtml = ticket.Name;

		StringBuilder props = new StringBuilder();
      AddStringProperty(props, "Assigned To", ticket.UserName, true, "", "openUser", ticket.UserID);
      AddStringProperty(props, "Group", ticket.GroupName, true, null, null, null);
      AddStringProperty(props, "Type", ticket.TicketTypeName, false, null, null, null);
      AddStringProperty(props, "Status", ticket.Status, false, null, null, null);
      AddStringProperty(props, "Severity", ticket.Severity, false, null, null, null);
      AddStringProperty(props, "Customers", GetCustomerLinks(ticketID), false, null, null, null);
      AddStringProperty(props, "Tags", GetTagLinks(ticketID), false, null, null, null);
		if (ticket.DueDate != null)
		{
			DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc((DateTime)ticket.DueDateUtc, TSAuthentication.GetLoginUser().TimeZoneInfo);
      
			if (ticket.DueDateUtc < DateTime.UtcNow)
				AddStringProperty(props, "Due Date", cstTime.ToString(us.DateTimeFormat.ShortDatePattern + "  HH:mm"), false, null, null, null, true);
			else
				AddStringProperty(props, "Due Date", cstTime.ToString(us.DateTimeFormat.ShortDatePattern + "  HH:mm"), false, null, null, null);
		}
      tipProps.InnerHtml = props.ToString();
    }

    private string GetCustomerLinks(int ticketID)
    {
      /*ContactsView contacts = new ContactsView(TSAuthentication.GetLoginUser());
      contacts.LoadByTicketID(ticketID);
      foreach (ContactsViewItem contact in contacts)
      {
      }*/

      Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
      //organizations.LoadByNotContactTicketID(ticketID);
      organizations.LoadByTicketID(ticketID);

      StringBuilder result = new StringBuilder();
      foreach (Organization organization in organizations)
      {
        if (result.Length > 0) result.Append(", ");
        result.Append(GetValueString(organization.Name, false, null, "openCustomer", organization.OrganizationID));
      }
      return result.ToString();
    }

    private string GetTagLinks(int ticketID)
    {
      Tags tags = new Tags(TSAuthentication.GetLoginUser());
      tags.LoadByReference(ReferenceType.Tickets, ticketID);
      StringBuilder result = new StringBuilder();
      foreach (Tag tag in tags)
      {
        if (result.Length > 0) result.Append(", ");
        result.Append(GetValueString(tag.Value, false, null, "openTag", tag.TagID));
      }
      return result.ToString();
    }

    private void AddStringProperty(StringBuilder props, string name, string value, bool canBeUnassigned, string href, string function, int? id, bool isInvalid = false)
    {
      string s = GetValueString(value, canBeUnassigned, href, function, id);
      if (!string.IsNullOrEmpty(s))
		{
			if (isInvalid)
				props.Append(string.Format("<dt>{0}</dt><dd style='color:red'>{1}</dd>", name, s));
			else
				props.Append(string.Format("<dt>{0}</dt><dd>{1}</dd>", name, s));
		}
    }

    private string GetValueString(string value, bool canBeUnassigned, string href, string function, int? id)
    { 
      if (!string.IsNullOrEmpty(value))
      {
        if (function != null && id != null) return string.Format("<a href=\"#\" onclick=\"top.Ts.MainPage.{0}({1}); return false;\">{2}</a>", function, id.ToString(), value);
        else if (href != null) return string.Format("<a href=\"{0}\">{1}</a>", href, value);
        else return value;
      }
      else if (canBeUnassigned)
      {
        return "Unassigned";
      }
      else
      {
        return null;
      }
    }
   

    private void EndResponse(string message)
    {
      Response.Write(message);
      Response.End();
    }
}