using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Text;

public partial class Tips_Customer : System.Web.UI.Page {

    protected void Page_Load(object sender, EventArgs e) {
        string domain = SystemSettings.GetAppUrl();
        if (Request["CustomerID"] == null) {
            EndResponse("Invalid Customer");
        }

        int organizationID = int.Parse(Request["CustomerID"]);
        Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), organizationID);

        if (organization == null) {
            EndResponse("Invalid Customer");
        }

        if (organization.OrganizationID != TSAuthentication.OrganizationID && organization.ParentID != TSAuthentication.OrganizationID) {
            EndResponse("Invalid Customer");
        }

        tipCompany.InnerText = organization.Name;
        tipCompany.Attributes.Add("onclick", "top.Ts.MainPage.openNewCustomer(" + organizationID.ToString() + "); return false;");

        StringBuilder props = new StringBuilder();
        if (!string.IsNullOrEmpty(organization.Website)) {
            string website;
            website = organization.Website;
            if (organization.Website.IndexOf("http://") < 0 && organization.Website.IndexOf("https://") < 0) {
                website = "http://" + organization.Website;
            }
            props.Append(string.Format("<dt>Website</dt><dd><a target=\"_blank\" href=\"{0}\">{0}</a></dd>", website));
        }

        if (organization.SAExpirationDate != null) {
            string css = organization.SAExpirationDate <= DateTime.UtcNow ? "tip-customer-expired" : "";
            props.Append(string.Format("<dt>Service Expiration</dt><dd class=\"{0}\">{1:D}</dd>", css, (DateTime)organization.SAExpirationDate));
        }

        PhoneNumbersView numbers = new PhoneNumbersView(organization.Collection.LoginUser);
        numbers.LoadByID(organization.OrganizationID, ReferenceType.Organizations);

		foreach (PhoneNumbersViewItemProxy number in numbers.GetPhoneNumbersViewItemProxies()) {
            props.Append(string.Format("<dt>{0}</dt><dd><a href=\"tel:{1}\">{1} {2}</a></dd>", number.PhoneType, number.FormattedPhoneNumber, number.Extension));
        }

        tipProps.InnerHtml = props.ToString();

        TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());
        tickets.LoadLatest5Tickets(organizationID);
        StringBuilder recent = new StringBuilder();

        foreach (TicketsViewItem t in tickets) {
	  		if(t.TicketNumber != null && t.Name != null && t.Status != null) {
                recent.Append(string.Format("<div><a href='{0}?TicketNumber={1}' target='_blank' onclick='top.Ts.MainPage.openTicket({2}); return false;'><span class='ticket-tip-number'>{3}</span><span class='ticket-tip-status'>{4}</span><span class='ticket-tip-name'>{5}</span></a></div>", domain, t.TicketNumber, t.TicketNumber, t.TicketNumber, t.Status.Length > 17 ? t.Status.Substring(0, 15) + "..." : t.Status, t.Name.Length > 35 ? t.Name.Substring(0, 33) + "..." : t.Name));
            }
        }

        if (recent.Length == 0) {
            recent.Append("There are no recent tickets for this organization");
        }

        tipRecent.InnerHtml = recent.ToString();

        //Support Hours
        StringBuilder supportHours = new StringBuilder();

        if (organization.SupportHoursMonth > 0) {
            tipTimeSpent.Visible = true;
            double timeSpent = organization.GetTimeSpentMonth(TSAuthentication.GetLoginUser(), organization.OrganizationID) / 60;
            supportHours.AppendFormat("<div class='ui-widget-content ts-separator'></div><div id='tipRecent' runat='server'><dt>Monthly Support Hours</dt><dt>Hours Used</dt><dd>{0}</dd><dt>Hours Remaining</dt>", Math.Round(timeSpent,2));

            if (timeSpent > organization.SupportHoursMonth) {
                supportHours.AppendFormat("<dd class='red'>-{0}</dd>", Math.Round(timeSpent - organization.SupportHoursMonth, 2));
            } else {
                supportHours.AppendFormat("<dd>{0}</dd>", Math.Round(organization.SupportHoursMonth - timeSpent,2));
            }
        }


        tipTimeSpent.InnerHtml = supportHours.ToString();

		// Customer Notes
		StringBuilder notesString = new StringBuilder();
	    NotesView notes = new NotesView(TSAuthentication.GetLoginUser());
		notes.LoadbyCustomerID(organizationID);

		foreach (NotesViewItem t in notes) {
		    notesString.Append(string.Format("<div><a href='#' target='_blank' onclick='top.Ts.MainPage.openNewCustomerNote({0},{1}); return false;'><span class='ticket-tip-name'>{2}</span></a></div>", t.RefID, t.NoteID, t.Title.Length > 65 ? t.Title.Substring(0, 65) + "..." : t.Title));
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
