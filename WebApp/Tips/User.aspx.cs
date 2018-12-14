using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Text;

public partial class Tips_User : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
      if (Request["UserID"] == null) EndResponse("Invalid User");
      int? ticketID = null;
      if (Request["TicketID"] != null)
      {
        int id;
        if (int.TryParse(Request["TicketID"], out id))
        {
          ticketID = id;
        }
      }

		string domain = SystemSettings.GetAppUrl();
      int userID = int.Parse(Request["UserID"]);
      User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
      if (user == null) EndResponse("Invalid User");
      Organization organization = Organizations.GetOrganization(user.Collection.LoginUser, user.OrganizationID);
      if (user.OrganizationID != TSAuthentication.OrganizationID && organization.ParentID != TSAuthentication.OrganizationID) EndResponse("Invalid User");
      tipName.InnerText = user.FirstLastName;

      if (user.OrganizationID == TSAuthentication.OrganizationID)
      {
          tipName.Attributes.Add("onclick", "top.Ts.MainPage.openNewContact(" + user.UserID.ToString() + "); return false;");
        tipCompany.Visible = false;
      }
      else
      {
        tipName.Attributes.Add("onclick", "top.Ts.MainPage.openContact(" + user.UserID.ToString() + "," + user.OrganizationID.ToString() +"); return false;");
        tipCompany.Visible = true;
      }

      tipCompany.InnerText = organization.Name;
      tipCompany.Attributes.Add("onclick", "top.Ts.MainPage.openNewCustomer(" + user.OrganizationID.ToString() + "); return false;");
      if (!string.IsNullOrEmpty(user.Title)) tipTitle.InnerHtml = user.Title + ", ";

      StringBuilder props = new StringBuilder();
      if (!string.IsNullOrEmpty(user.Email))
      {
        if (ticketID != null)
        {
          props.Append(string.Format("<dt>Email</dt><dd><a href=\"{0}\" target=\"_blank\">{1}</a></dd>", DataUtils.GetMailLinkHRef(user.Collection.LoginUser, userID, (int)ticketID), user.Email));
        }
        else
	      {
          props.Append(string.Format("<dt>Email</dt><dd><a href=\"mailto:{0}\" target=\"_blank\">{0}</a></dd>", user.Email));
	      }
      }

      PhoneNumbersView numbers = new PhoneNumbersView(user.Collection.LoginUser);
      numbers.LoadByID(user.UserID, ReferenceType.Users);

      foreach (PhoneNumbersViewItemProxy number in numbers.GetPhoneNumbersViewItemProxies())
      {
        props.Append(string.Format("<dt>{0}</dt><dd><a href=\"tel:{1}\">{1} {2}</a></dd>", number.PhoneType, number.FormattedPhoneNumber, number.Extension));
      }

      tipProps.InnerHtml = props.ToString();

		TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());
		tickets.LoadLatest5UserTickets(user.UserID);
		StringBuilder recent = new StringBuilder();

		foreach (TicketsViewItem t in tickets)
		{
			if (t.TicketNumber != null && t.Name != null && t.Status != null)
				recent.Append(string.Format("<div><a href='{0}?TicketNumber={1}' target='_blank' onclick='top.Ts.MainPage.openTicket({2}); return false;'><span class='ticket-tip-number'>{3}</span><span class='ticket-tip-status'>{4}</span><span class='ticket-tip-name'>{5}</span></a></div>", domain, t.TicketNumber, t.TicketNumber, t.TicketNumber, t.Status.Length > 17 ? t.Status.Substring(0, 15) + "..." : t.Status, t.Name.Length > 35 ? t.Name.Substring(0, 33) + "..." : t.Name));
		}

		if (recent.Length == 0)
			recent.Append("There are no recent tickets for this user");

		tipRecent.InnerHtml = recent.ToString();

		// Customer Notes
		StringBuilder notesString = new StringBuilder();
		NotesView notes = new NotesView(TSAuthentication.GetLoginUser());
		notes.LoadbyContactID(user.UserID);

		foreach (NotesViewItem t in notes) {
			notesString.Append(string.Format("<div><a href='#' target='_blank' onclick='top.Ts.MainPage.openNewContactNote({0},{1}); return false;'><span class='ticket-tip-name'>{2}</span></a></div>", t.RefID, t.NoteID, t.Title.Length > 65 ? t.Title.Substring(0, 65) + "..." : t.Title));
		}

		if (notesString.Length == 0) {
			notesString.Append("");
        }

		tipNotes.InnerHtml = notesString.ToString();

    }

    private void EndResponse(string message) {
      Response.Write(message);
      Response.End();
    }
}
