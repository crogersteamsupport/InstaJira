using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;
using System.Web.Services;
using System.Text;
using System.Globalization;

public partial class Frames_AdminCompany : BaseFramePage
{
	private int _organizationID;

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		_organizationID = UserSession.LoginUser.OrganizationID;


		Refresh();

		btnEditProperties.Visible = UserSession.CurrentUser.IsSystemAdmin;
		btnChat.Visible = UserSession.CurrentUser.IsSystemAdmin;
		btnNewPhone.Visible = UserSession.CurrentUser.IsSystemAdmin;
		btnNewAddress.Visible = UserSession.CurrentUser.IsSystemAdmin;

		if (UserSession.CurrentUser.IsSystemAdmin)
		{
			pnlProperties.Attributes.Add("class", "");
			pnlPhone.Attributes.Add("class", "");
			pnlAddress.Attributes.Add("class", "");
		}
		else
		{
			pnlProperties.Attributes.Add("class", "adminDiv");
			pnlPhone.Attributes.Add("class", "adminDiv");
			pnlAddress.Attributes.Add("class", "adminDiv");
		}

		divChat.Visible = UserSession.CurrentUser.HasChatRights;

		if (btnNewAddress.Visible) btnNewAddress.OnClientClick = "ShowDialog(top.GetAddressDialog(" + _organizationID.ToString() + ", 9)); top.Ts.System.logAction('Admin Organization - Address Dialog Opened'); return false;";
		if (btnNewPhone.Visible) btnNewPhone.OnClientClick = "ShowDialog(top.GetPhoneDialog(" + _organizationID.ToString() + ", 9)); top.Ts.System.logAction('Admin Organization - Phone Dialog Opened'); return false;";
		if (btnEditProperties.Visible) btnEditProperties.OnClientClick = "ShowDialog(top.GetMyCompanyDialog(" + _organizationID.ToString() + ")); top.Ts.System.logAction('Admin Organization - Properties Dialog Opened'); return false;";
		if (btnChat.Visible) btnChat.OnClientClick = "ShowDialog(top.GetChatPropertiesDialog(" + _organizationID.ToString() + ")); top.Ts.System.logAction('Admin Organization - Chat Dialog Opened'); return false;";

	}


	public void Refresh()
	{
		LoadProperties(_organizationID);
		LoadChat(_organizationID);
		LoadNumbers(_organizationID);
		LoadAddresses(_organizationID);
	}

	private void LoadProperties(int organizationID)
	{
		lblProperties.Visible = true;

		Organizations organizations = new Organizations(UserSession.LoginUser);
		organizations.LoadByOrganizationID(organizationID);

		if (organizations.IsEmpty) return;
		Organization organization = organizations[0];


		Users users = new Users(UserSession.LoginUser);
		string primaryUser = "[Unassigned]";
		if (organization.PrimaryUserID != null)
		{
			users.LoadByUserID((int)organization.PrimaryUserID);
			primaryUser = users.IsEmpty ? "" : users[0].FirstLastName;
		}


		string defaultGroup = "[Unassigned]";
		if (organization.DefaultPortalGroupID != null)
		{
			Group group = (Group)Groups.GetGroup(UserSession.LoginUser, (int)organization.DefaultPortalGroupID);
			if (group != null)
			{
				defaultGroup = group.Name;
			}
		}

		lblProperties.Visible = organizations.IsEmpty;

		DataTable table = new DataTable();
		table.Columns.Add("Name");
		table.Columns.Add("Value");

		table.Rows.Add(new string[] { "Name:", organization.Name });
		table.Rows.Add(new string[] { "Description:", organization.Description });
		//table.Rows.Add(new string[] { "Portal Access:", organization.HasPortalAccess.ToString() });
		/*
		if (UserSession.CurrentUser.HasPortalRights)
		{
		  string portalLink = "http://portal.ts.com?OrganizationID=" + organization.OrganizationID.ToString();
		  portalLink = @"<a href=""" + portalLink + @""" target=""PortalLink"" onclick=""window.open('" + portalLink + @"', 'PortalLink')"">" + portalLink + "</a>";
		  table.Rows.Add(new string[] { "Portal Link:", portalLink });
		  table.Rows.Add(new string[] { "Default Portal Group:", defaultGroup });
		}
		 */
		table.Rows.Add(new string[] { "Organization ID:", organization.OrganizationID.ToString() });
		table.Rows.Add(new string[] { "Primary Contact:", primaryUser });


		//"Central Standard Time"

		TimeZoneInfo timeZoneInfo = null;
		try
		{
			timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(organization.TimeZoneID);
		}
		catch (Exception)
		{
			timeZoneInfo = null;
		}

		table.Rows.Add(new string[] { "Time Zone:", timeZoneInfo == null ? "Central Standard Time" : timeZoneInfo.DisplayName });

		table.Rows.Add(new string[] { "Date Format:", (new CultureInfo(organization.CultureName)).DisplayName });
		table.Rows.Add(new string[] { "Default Font Family:", organization.FontFamilyDescription });
		table.Rows.Add(new string[] { "Default Font Size:", organization.FontSizeDescription });

		table.Rows.Add(new string[] { "Business Days:", organization.BusinessDaysText == "" ? "[None Assigned]" : organization.BusinessDaysText });
		table.Rows.Add(new string[] { "Business Day Start:", organization.BusinessDayStart == null ? "[None Assigned]" : ((DateTime)organization.BusinessDayStart).ToString("t", UserSession.LoginUser.CultureInfo) });
		table.Rows.Add(new string[] { "Business Day End:", organization.BusinessDayEnd == null ? "[None Assigned]" : ((DateTime)organization.BusinessDayEnd).ToString("t", UserSession.LoginUser.CultureInfo) });

		table.Rows.Add(new string[] { "Product Required on Ticket:", organization.ProductRequired.ToString() });
		table.Rows.Add(new string[] { "Product Version Required on ticket:", organization.ProductVersionRequired.ToString() });

		table.Rows.Add(new string[] { "Only show products for the customers of a ticket:", Settings.OrganizationDB.ReadBool("ShowOnlyCustomerProducts", false).ToString() });
		table.Rows.Add(new string[] { "Auto Assign Customer with Asset On Tickets:", organization.AutoAssignCustomerWithAssetOnTickets.ToString() });
		table.Rows.Add(new string[] { "Auto Associate Customer To Ticket based on Asset Assignment:", organization.AutoAssociateCustomerToTicketBasedOnAssetAssignment.ToString() });
		table.Rows.Add(new string[] { "Require customer for new ticket:", Settings.OrganizationDB.ReadBool("RequireNewTicketCustomer", false).ToString() });
		table.Rows.Add(new string[] { "Require time spent on timed actions:", organization.TimedActionsRequired.ToString() });
		table.Rows.Add(new string[] { "Disable ticket status update emails:", Settings.OrganizationDB.ReadBool("DisableStatusNotification", false).ToString() });
		table.Rows.Add(new string[] { "Visible to customers is initially enabled for new actions:", organization.SetNewActionsVisibleToCustomers.ToString() });
		table.Rows.Add(new string[] { "Allow unauthenticated users to view attachments:", organization.AllowUnsecureAttachmentViewing.ToString() });
		table.Rows.Add(new string[] { "Allow private actions to satisfy SLA first reponse:", organization.SlaInitRespAnyAction.ToString() });
		table.Rows.Add(new string[] { "Chat ID:", organization.ChatID.ToString() });


		/*    string email = organization.SystemEmailID + "@teamsupport.com";
			table.Rows.Add(new string[] { "System Email:", "<a href=\"mailto:" + email + "\">" + email + "</a>" });
			table.Rows.Add(new string[] { "Organization Reply To Address:", organization.OrganizationReplyToAddress });
			table.Rows.Add(new string[] { "Require [New] keyword for emails:", organization.RequireNewKeyword.ToString() });
			table.Rows.Add(new string[] { "Require a known email address for emails:", organization.RequireKnownUserForNewEmail.ToString() });
			*/

		//table.Rows.Add(new string[] { "Use Community:", organization.UseForums.ToString() });
		WikiArticle defArticle = organization.DefaultWikiArticleID == null ? null : WikiArticles.GetWikiArticle(UserSession.LoginUser, (int)organization.DefaultWikiArticleID);
		table.Rows.Add(new string[] { "Default Wiki Article:", defArticle == null ? "[None Assigned]" : defArticle.ArticleName });


		//table.Rows.Add(new string[] { "Only Admin Can Modify Customers:", organization.AdminOnlyCustomers.ToString() });
		table.Rows.Add(new string[] { "Only Admin Can View Reports:", organization.AdminOnlyReports.ToString() });

		SlaLevel level = organization.InternalSlaLevelID != null ? SlaLevels.GetSlaLevel(UserSession.LoginUser, (int)organization.InternalSlaLevelID) : null;
		table.Rows.Add(new string[] { "Internal SLA:", level == null ? "[None Assigned]" : level.Name });

		table.Rows.Add(new string[] { "Show Group Members First in Ticket Assignment List:", organization.ShowGroupMembersFirstInTicketAssignmentList.ToString() });
		table.Rows.Add(new string[] { "Require Group Assignment On Tickets:", organization.RequireGroupAssignmentOnTickets.ToString() });
		table.Rows.Add(new string[] { "Update Ticket Children Group With Parent:", organization.UpdateTicketChildrenGroupWithParent.ToString() });
		table.Rows.Add(new string[] { "Hide Alert Dismiss for Non Admins:", organization.HideDismissNonAdmins.ToString() });
		if ((organization.ProductType == ProductType.Enterprise || organization.ProductType == ProductType.BugTracking))
			table.Rows.Add(new string[] { "Use Product Lines:", organization.UseProductFamilies.ToString() });

		table.Rows.Add(new string[] { "Customer Insights:", organization.IsCustomerInsightsActive.ToString() });
		table.Rows.Add(new string[] { "Two Factor Verification:", organization.TwoStepVerificationEnabled.ToString() });
		table.Rows.Add(new string[] { "How many days before user passwords expire:", organization.DaysBeforePasswordExpire.ToString() });
		table.Rows.Add(new string[] { "Do not include attachments on outbound emails:", organization.NoAttachmentsInOutboundEmail.ToString() });

		if (organization.NoAttachmentsInOutboundEmail && !string.IsNullOrEmpty(organization.NoAttachmentsInOutboundExcludeProductLine))
		{
			ProductFamilies productFamilies = new ProductFamilies(UserSession.LoginUser);

			try
			{
				productFamilies.LoadByIds(organization.NoAttachmentsInOutboundExcludeProductLine.Split(',').Select(int.Parse).ToList(), organization.OrganizationID);

				if (productFamilies.Count > 0)
				{
					table.Rows.Add(new string[] { "Except for the following Product Lines (include attachments):", string.Join(",", productFamilies.Select(p => p.Name).ToArray()) });
				}
			}
			catch (Exception ex)
			{
				ExceptionLogs.LogException(UserSession.LoginUser, ex, "AdminCompany.aspx.cs.LoadProperties");
			}
		}

		table.Rows.Add(new string[] { "Warn if contact has no email address:", organization.AlertContactNoEmail.ToString() });
		table.Rows.Add(new string[] { "Allow TeamSupport to log into your account for technical support:", (!organization.DisableSupportLogin).ToString() });
		table.Rows.Add(new string[] { "Use Watson:", organization.UseWatson.ToString() });
		// table.Rows.Add(new string[] { "Require Two Factor Authentication:", organization.RequireTwoFactor.ToString() });

		rptProperties.DataSource = table;
		rptProperties.DataBind();
	}

	private void LoadChat(int organzationID)
	{
		Organization organization = Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);

		ChatSetting setting = ChatSettings.GetSetting(UserSession.LoginUser, organzationID);
		StringBuilder builder = new StringBuilder();
		//builder.Append("<table cellpadding=\"0\" cellspacing=\"5\" border=\"0\">");
		//builder.Append("<tr>");
		//builder.Append("<td>Available Image:</td>");
		builder.Append("<div><strong>Customer Chat Link:</strong></div>");
		builder.Append("<div>");
		string script = string.Format("window.open('{1}/Chat/ChatInit.aspx?uid={0}', 'TSChat', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=450,height=575'); return false;", organization.ChatID.ToString(), SystemSettings.GetAppUrl());

		string link = string.Format("<a href=\"#\" onclick=\"{0}\"><img src=\"{2}/dc/{1}/chat/image\" border=\"0\" /></a>", script, organization.OrganizationID, SystemSettings.GetAppUrl());
		textChatCode.Text = link;
		builder.Append(string.Format("<a href=\"#\" onclick=\"{0}\">Test</a>", script));
		builder.Append("</div>");
		divChatProperties.InnerHtml = builder.ToString();
	}

	private void LoadNumbers(int organizationID)
	{
		PhoneNumbers phoneNumbers = new PhoneNumbers(UserSession.LoginUser);
		phoneNumbers.LoadByID(organizationID, ReferenceType.Organizations);

		lblPhone.Visible = phoneNumbers.IsEmpty;

		DataTable table = new DataTable();
		table.Columns.Add("PhoneID");
		table.Columns.Add("Type");
		table.Columns.Add("Number");
		table.Columns.Add("Ext");

		foreach (PhoneNumber phoneNumber in phoneNumbers)
		{
			table.Rows.Add(new string[] { phoneNumber.PhoneID.ToString(), phoneNumber.PhoneTypeName, phoneNumber.FormattedNumber, phoneNumber.Extension == "" ? "" : " Ext: " + phoneNumber.Extension });
		}

		rptPhone.DataSource = table;
		rptPhone.DataBind();
	}

	private void LoadAddresses(int organizationID)
	{
		Addresses addresses = new Addresses(UserSession.LoginUser);
		addresses.LoadByID(organizationID, ReferenceType.Organizations);

		lblAddresses.Visible = addresses.IsEmpty;

		rptAddresses.DataSource = addresses;
		rptAddresses.DataBind();
	}

	[WebMethod]
	public static void ForceCRMResync()
	{
		CRMLinkTable crmLinkTable = new CRMLinkTable(UserSession.LoginUser);
		crmLinkTable.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
		if (!crmLinkTable.IsEmpty)
		{
			crmLinkTable[0].LastProcessed = new DateTime(1900, 1, 1);
			crmLinkTable[0].LastLink = new DateTime(1980, 1, 1);
			crmLinkTable.Save();
		}

	}

	[WebMethod]
	public static void MarkChatOffline()
	{
		Users.MarkUsersChatUnavailable(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
	}


}
