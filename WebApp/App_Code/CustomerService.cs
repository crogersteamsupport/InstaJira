using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Runtime.Serialization;
using System.Globalization;
using Ganss.XSS;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Security.Application;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class CustomerService : System.Web.Services.WebService
    {

        public CustomerService() { }

        [WebMethod]
        public UserProxy GetUser(int userID)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
			user.CryptedPassword = string.Empty;
            return user.GetProxy();
        }

        [WebMethod]
        public string SetCompanyName(int orgID, string value)
		{
			value = DataUtils.CleanValueScript(value);
			Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.Name = value;
            o.Collection.Save();
            string description = String.Format("{0} set company name to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, orgID, description);
            return value != "" ? value : "Empty";
        }
        [WebMethod]
        public string SetCompanyTimezone(int orgID, string value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.TimeZoneID = value;
            o.Collection.Save();
            string description = String.Format("{0} set company time zone to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, orgID, description);
            return value;
        }

        [WebMethod]
        public string SetCompanyWeb(int orgID, string value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.Website = DataUtils.CleanValueScript(value);
            o.Collection.Save();
            string description = String.Format("{0} set company website to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, orgID, description);
            return value != "" ? value : "Empty";
        }
        [WebMethod]
        public int SetCompanyPrimaryContact(int orgID, int value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.PrimaryUserID = value;
            o.Collection.Save();
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), value);
            string description = String.Format("{0} set company name to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, u == null ? "Unassigned" : u.FirstLastName);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, orgID, description);
            return value;
        }
        [WebMethod]
        public int SetCompanyDefaultSupportUser(int orgID, int value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.DefaultSupportUserID = value;
            o.Collection.Save();
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), value);
            string description = String.Format("{0} set company default support user to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, u == null ? "Unassigned" : u.FirstLastName);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, orgID, description);
            return value;
        }
        [WebMethod]
        public int SetCompanyDefaultSupportGroup(int orgID, int value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.DefaultSupportGroupID = value;
            o.Collection.Save();
            Group g = Groups.GetGroup(TSAuthentication.GetLoginUser(), value);
            string description = String.Format("{0} set company default support group to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, g == null ? "Unassigned" : g.Name);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, orgID, description);
            return value;
        }
        [WebMethod]
        public int SetCompanyDefaultPortalGroup(int orgID, int value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.DefaultPortalGroupID = value;
            o.Collection.Save();
            Group g = Groups.GetGroup(TSAuthentication.GetLoginUser(), value);
            string description = String.Format("{0} set company default portal group to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, g == null ? "Unassigned" : g.Name);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, orgID, description);
            return value;
        }
        [WebMethod]
        public string SetCompanyDomain(int orgID, string value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.CompanyDomains = DataUtils.CleanValueScript(value);
			o.Collection.Save();
            string description = String.Format("{0} set company domain to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, orgID, description);
            return value != "" ? value : "Empty";
        }
        [WebMethod]
        public bool SetCompanyActive(int orgID, bool value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.IsActive = value;
            o.Collection.Save();
            string description = String.Format("{0} set company active to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, orgID, description);
            return value;
        }
        [WebMethod]
		public void SetCompanyAndChildrenInactive(int organizationId)
		{
			SetCompanyActive(organizationId, false);
			string[] children = LoadChildren(organizationId);

			if (children.Length > 0)
			{
				LoginUser loginUser = TSAuthentication.GetLoginUser();
				string description = String.Format("{0} set company's children active to False", TSAuthentication.GetUser(loginUser).FirstLastName);
				ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Organizations, organizationId, description);
			}

			foreach (string childOrgJson in children)
			{
				OrganizationProxy childOrg = JsonConvert.DeserializeObject<OrganizationProxy>(childOrgJson);
				
				if (childOrg.OrganizationID > 0)
				{
					SetCompanyActive(childOrg.OrganizationID, false);
				}
			}
		}
		[WebMethod]
        public bool SetCompanyPortalAccess(int orgID, bool value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.HasPortalAccess = value;
            o.Collection.Save();
            string description = String.Format("{0} set company portal access to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, orgID, description);
            return value;
        }
        [WebMethod]
        public bool SetCompanyAPIEnabled(int orgID, bool value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.IsApiActive = value;
            o.IsApiEnabled = value;
            o.Collection.Save();
            string description = String.Format("{0} set company api enabled to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, orgID, description);
            return value;
        }
        [WebMethod]
        public string SetCompanySAE(int orgID, string value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);

			if (value != "" && value != null)
			{
				value = DataUtils.CleanValueScript(value);
				o.SAExpirationDate = DataUtils.DateToUtc(TSAuthentication.GetLoginUser(), Convert.ToDateTime(value + " 12:00:00"));
			}
			else
				o.SAExpirationDate = null;
            o.Collection.Save();
            string description = String.Format("{0} set company service expirated date to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, orgID, description);
            return value;
        }
        [WebMethod]
        public int SetCompanySLA(int orgID, int value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.SlaLevelID = value;
            o.Collection.Save();
            SlaLevel s = SlaLevels.GetSlaLevel(TSAuthentication.GetLoginUser(), value);
            string description = String.Format("{0} set company SLA to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, s.Name);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, orgID, description);
            return value;
        }
        [WebMethod]
        public int SetCompanySupportHours(int orgID, int value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.SupportHoursMonth = value;
            o.Collection.Save();
            string description = String.Format("{0} set company support hours to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, orgID, description);
            return value;
        }
        [WebMethod]
        public string SetCompanyDescription(int orgID, string value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.Description = DataUtils.CleanValueScript(value);
            o.Collection.Save();
            string description = String.Format("{0} set company description to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, orgID, description);
            return value != "" ? value : "Empty";
        }
        [WebMethod]
        public string SetCompanyInactive(int orgID, string value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.InActiveReason = DataUtils.CleanValueScript(value);
			o.Collection.Save();
            string description = String.Format("{0} set company inactive to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, orgID, description);
            return value;
        }

        [WebMethod]
        public string SetContactEmail(int userID, string email)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.Email = DataUtils.CleanValueScript(email);
            u.Collection.Save();
            string description = String.Format("{0} set contact email to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, email);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, userID, description);
            return email != "" ? email : "Empty";
        }
        [WebMethod]
        public string SetContactName(int userID, string fname, string mname, string lname)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.FirstName = DataUtils.CleanValueScript(fname);
			u.MiddleName = DataUtils.CleanValueScript(mname);
			u.LastName = DataUtils.CleanValueScript(lname);
			u.Collection.Save();
            string description = String.Format("{0} set contact name to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, u.FirstLastName);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, userID, description);
            return u.FirstLastName;
        }

        [WebMethod]
        public string SetContactLinkedIn(int userID, string linkedin)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.LinkedIn = DataUtils.CleanValueScript(linkedin);
			u.Collection.Save();
            string description = String.Format("{0} set contact linkedin to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, linkedin);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, userID, description);
            return linkedin != "" ? linkedin : "Empty";
        }

        [WebMethod]
        public string SetContactCompany(int userID, int value)
        {
            Tickets t = new Tickets(TSAuthentication.GetLoginUser());
            t.LoadByContact(userID);

            foreach (Ticket tix in t)
            {
                tix.Collection.RemoveContact(userID, tix.TicketID);
            }

            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.PortalAutoReg = false;
            u.OrganizationID = value;
            u.Collection.Save();

            foreach (Ticket tix in t)
            {
                tix.Collection.AddContact(userID, tix.TicketID);

            }

            EmailPosts ep = new EmailPosts(TSAuthentication.GetLoginUser());
            ep.LoadByRecentUserID(userID);
            ep.DeleteAll();
            ep.Save();


            //User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            //u.OrganizationID = value;
            //u.Collection.Save();

            Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), value);
            string description = String.Format("{0} set contact company to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, organization.Name);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, userID, description);
            return organization.Name != "" ? organization.Name : "Empty";
        }

        [WebMethod]
        public string SetContactCompany2(int userID, int value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            User u = Users.GetUser(loginUser, userID);

            Users users = new Users(loginUser);
            users.LoadByOrganizationID(value, false);
            foreach (User user in users)
            {
                if (user.Email.ToLower() == u.Email.ToLower())
                {
                    return "email already exists";
                }
            }

            Tickets t = new Tickets(loginUser);
            t.LoadByContact(userID);

            foreach (Ticket tix in t)
            {
                tix.Collection.RemoveContact(userID, tix.TicketID);
            }

            u.PortalAutoReg = false;
            u.OrganizationID = value;
            u.Collection.Save();

            List<int> existingEmailPostsId = new List<int>();
            EmailPosts existingEmailPosts = new EmailPosts(loginUser);

            foreach (Ticket tix in t)
            {
                existingEmailPosts.LoadByTicketId(tix.TicketID);

                if (existingEmailPosts != null && existingEmailPosts.Any())
                {
                    existingEmailPostsId.AddRange(existingEmailPosts.Select(p => p.EmailPostID).Distinct());
                    existingEmailPosts = new EmailPosts(loginUser);
                }

                tix.Collection.AddContact(userID, tix.TicketID);
            }

            //Delete the EmailPosts just created, but keep the existing ones
            EmailPosts ep = new EmailPosts(loginUser);
            ep.LoadByRecentUserID(userID, existingEmailPostsId);
            ep.DeleteAll();
            ep.Save();


            //User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            //u.OrganizationID = value;
            //u.Collection.Save();

            Organization organization = Organizations.GetOrganization(loginUser, value);
            string description = String.Format("{0} set contact company to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, organization.Name);
            ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Users, userID, description);
            return organization.Name != "" ? organization.Name : "Empty";
        }

        [WebMethod]
        public string SetContactTitle(int userID, string title)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.Title = DataUtils.CleanValueScript(title);
			u.Collection.Save();
            string description = String.Format("{0} set contact title to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, title);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, userID, description);
            return title != "" ? title : "Empty";
        }
        [WebMethod]
        public bool SetContactActive(int userID, bool value)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.IsActive = value;
            if (!value)
            {
                u.AppChatStatus = false;
                u.AppChatID = "";
            }
            u.Collection.Save();
            string description = String.Format("{0} set contact active to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, userID, description);
            return value;
        }
        [WebMethod]
        public bool SetContactPortalLimitOrgTickets(int userID, bool value)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.PortalLimitOrgTickets = value;
            u.Collection.Save();
            string description = String.Format("{0} set contact Portal Limit Org Tickets to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, userID, description);
            return value;
        }
        [WebMethod]
        public bool SetContactPortalLimitOrgChildrenTickets(int userID, bool value)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.PortalLimitOrgChildrenTickets = value;
            u.Collection.Save();
            string description = String.Format("{0} set contact Portal Limit Org Children Tickets to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, userID, description);
            return value;
        }


        [WebMethod]
        public int SetContactPortalUser(int userID, bool value)
        {
            //0 = false
            //1 = true
            //2 = true and ask about org access
            int result;
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.IsPortalUser = value;
            u.Collection.Save();
            string description = String.Format("{0} set contact portal user to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, userID, description);
            result = Convert.ToInt16(value);

            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), u.OrganizationID);
            if (value == true)
            {
                if (o.HasPortalAccess == false)
                    return 2;
                else
                    return 1;
            }

            return result;
        }

        [WebMethod]
        public bool SetContactPortalViewOnly(int userID, bool value)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.PortalViewOnly = value;
            u.Collection.Save();
            string description = String.Format("{0} set contact Portal View Only to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, userID, description);
            return value;
        }

        [WebMethod]
        public void SetCompanyPortalAccessUser(int userID)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);

            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), u.OrganizationID);
            o.HasPortalAccess = true;
            o.Collection.Save();
            string description = String.Format("{0} set company portal access to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, true);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, u.OrganizationID, description);
            return;
        }


        [WebMethod]
        public bool SetContactPreventEmail(int userID, bool value)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.BlockInboundEmail = value;
            u.Collection.Save();
            string description = String.Format("{0} set contact prevent email from creating tickets to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, userID, description);
            return value;
        }
        [WebMethod]
        public bool SetContactPreventEmailCreatingOnly(int userID, bool value)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.BlockEmailFromCreatingOnly = value;
            u.Collection.Save();
            string description = String.Format("{0} set contact prevent email from creating but allow updating tickets to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, userID, description);
            return value;
        }
        [WebMethod]
        public bool SetContactSystemAdmin(int userID, bool value)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.IsSystemAdmin = value;
            u.Collection.Save();
            string description = String.Format("{0} set contact sys admin to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, userID, description);
            return value;
        }
        [WebMethod]
        public bool SetContactFinancialAdmin(int userID, bool value)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.IsFinanceAdmin = value;
            u.Collection.Save();
            string description = String.Format("{0} set contact financial admin to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, userID, description);
            return value;
        }
        [WebMethod]
        public bool IsTSUser()
        {
            bool _isAdmin = false;
            //_isAdmin = (UserSession.LoginUser.OrganizationID != _organizationID) || UserSession.CurrentUser.IsSystemAdmin;
            //_isAdmin = _isAdmin && (UserSession.CurrentUser.IsSystemAdmin || !UserSession.CurrentUser.IsAdminOnlyCustomers);

            return _isAdmin;
        }

        [WebMethod]
        public bool CanEdit()
        {
            return TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).ParentID == null;
        }

        [WebMethod]
        public string[] LoadContactProperties(int userID)
        {
            StringBuilder html = new StringBuilder("");
            StringBuilder contactInfo = new StringBuilder("");
            string[] contact = new string[2];

            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByUserID(userID);
            User user = users[0];

            contactInfo.AppendLine(CreateFormElement("Name", user.FirstLastName, "editable"));
            contactInfo.AppendLine(CreateFormElement("Email", user.Email, "editable"));
            contactInfo.AppendLine(CreateFormElement("Title", user.Title, "editable"));
            Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), user.OrganizationID);
            contactInfo.AppendLine(CreateFormElement("Company", organization.Name, "editable"));
            contactInfo.AppendLine(CreateFormElement("LinkedIn", user.LinkedIn, "editable"));

            html.AppendLine(CreateFormElement("Active", user.IsActive, "editable"));
            html.AppendLine(CreateFormElement("Portal User", user.IsPortalUser, "editable"));
            html.AppendLine(CreateFormElement("Portal View Only", user.PortalViewOnly, "editable"));
            html.AppendLine(CreateFormElement("Prevent email from creating and updating tickets", user.BlockInboundEmail, "editable"));
            html.AppendLine(CreateFormElement("Prevent email from creating but allow updating tickets", user.BlockEmailFromCreatingOnly, "editable"));
            html.AppendLine(CreateFormElement("Disable Organization Tickets View on Portal", user.PortalLimitOrgTickets, "editable"));
            html.AppendLine(CreateFormElement("Disable Organization Children Tickets View on Portal", user.PortalLimitOrgChildrenTickets, "editable"));

            if (TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).ParentID == null)
            {
                html.AppendLine(CreateFormElement("System Administrator:", user.IsSystemAdmin, "editable"));
                html.AppendLine(CreateFormElement("Financial Administrator:", user.IsFinanceAdmin, "editable"));
                //html.AppendLine(CreateFormElement("Portal User:", user.IsPortalUser));
                html.AppendLine(CreateFormElement("Last Logged In", user.LastLogin.ToString("g", TSAuthentication.GetLoginUser().CultureInfo), "db"));
                html.AppendLine(CreateFormElement("In Office", user.InOffice, "db"));
                html.AppendLine(CreateFormElement("In Office Comment", user.InOfficeComment, "db"));
                if (user.IsActive)
                    html.AppendLine(CreateFormElement("Activated On", user.ActivatedOn.ToString("g", TSAuthentication.GetLoginUser().CultureInfo), "db"));
                else
                {
                    if (user.DeactivatedOn != null)
                    {
                        DateTime dateTime = (DateTime)user.DeactivatedOn;
                        html.AppendLine(CreateFormElement("Deactivated On", dateTime.ToString("g", TSAuthentication.GetLoginUser().CultureInfo), "db"));
                    }
                    else
                    {
                        html.AppendLine(CreateFormElement("Deactivated On", ""));
                    }
                }
            }
            else
            {
                if (TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).HasPortalAccess)
                {
                    //html.AppendLine(CreateFormElement("Portal User:", user.IsPortalUser));
                    if (user.IsPortalUser)
                        html.AppendLine(CreateFormElement("Last Logged In", user.LastLogin.ToString("g", TSAuthentication.GetLoginUser().CultureInfo), "db"));
                }
            }

            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedAttributes.Add("id");
            contact[0] = (contactInfo.ToString());
            contact[1] = (html.ToString());

            return contact;

        }

        public string CreateFormElement(string fieldTitle, object fieldValue, string editable = "")
        {
            StringBuilder form = new StringBuilder("");

            if (fieldValue is bool)
            {
                if ((bool)fieldValue == true)
                    fieldValue = "Yes";
                else if ((bool)fieldValue == false)
                    fieldValue = "No";

            }

            if (editable == "db")
                form.AppendFormat(@"<div class='form-group'>
                                <label class='col-xs-4 control-label' for='field{0}'>{1}</label>
                                <div class='col-xs-8'>
                                <span class='form-control-static {3}' id='field{0}'>{2}</span>
                                </div>
                                </div>", fieldTitle.Replace(" ", ""), fieldTitle, fieldValue != null ? fieldValue : "Empty", editable);
            else
                form.AppendFormat(@"<div class='form-group'>
                                <label class='col-xs-4 control-label' for='field{0}'>{1}</label>
                                <div class='col-xs-8'>
                                <p class='form-control-static {3}' id='field{0}'>{2}</p>
                                </div>
                                </div>", fieldTitle.Replace(" ", ""), fieldTitle, (fieldValue != null && fieldValue != "") ? fieldValue : "Empty", editable);

            return form.ToString();
        }
        /*
          [WebMethod]
          public string[] GetSearchResults(string filter, int startIndex = 1)
          {
              StringBuilder builder = new StringBuilder();
              SearchService s = new SearchService();
              if (string.IsNullOrWhiteSpace(filter)) filter = "xfirstword";

              CompaniesAndContactsSearchResults test = s.SearchCompaniesAndContacts(filter, startIndex, 20, true, true);

              if (test.Items.Length > 0)
              {
                  foreach (CompanyOrContact item in test.Items)
                  {
                      if(item.ReferenceType == ReferenceType.Organizations)
                          builder.Append(CreateCompanyBox(item.Id));
                      else if(item.ReferenceType == ReferenceType.Contacts)
                          builder.Append(CreateContactBox(item.Id));
                  }

                  return builder.ToString();
              }
              else { return ""; }

          }

          [WebMethod]
          public string GetCompanies(string filter, int startIndex = 1)
          {
              LoginUser loginUser = TSAuthentication.GetLoginUser();
              StringBuilder builder = new StringBuilder();
              Organizations organizations = new Organizations(loginUser);
              int i = 0;

              //organizations.CustomerLoadByLikeOrganizationName(loginUser.OrganizationID, filter, false, startIndex, true);

              SearchService s = new SearchService();
              if (string.IsNullOrWhiteSpace(filter)) filter = "xfirstword";
              CompaniesAndContactsSearchResults test = s.SearchCompaniesAndContacts(filter, startIndex, 20, true, false);

              if (test.Items.Length > 0)
              {
                  foreach (CompanyOrContact item in test.Items)
                  {
                      builder.Append(CreateCompanyBox(item.Id));
                  }

                  return builder.ToString();
              }
              else { return ""; }
            //if (organizations.Count > 0)
              //{

              //    foreach (Organization item in organizations)
              //    {
              //            builder.Append(CreateCompanyBox(item));
              //    }

              //    return builder.ToString();
              //}
              //else
              //{
              //    return CreateNoResults();
              //}

          }

          [WebMethod]
          public string GetContacts(string filter, int startIndex = 1)
          {
              LoginUser loginUser = TSAuthentication.GetLoginUser();
              StringBuilder builder = new StringBuilder();
              UsersView users = new UsersView(loginUser);

              if (string.IsNullOrWhiteSpace(filter)) filter = "xfirstword";

              SearchService s = new SearchService();
              CompaniesAndContactsSearchResults test = s.SearchCompaniesAndContacts(filter, startIndex, 20, false, true);

              if (test.Items.Length > 0)
              {
                foreach (CompanyOrContact item in test.Items)
                {
                  builder.Append(CreateContactBox(item.Id));
                }

                return builder.ToString();
              }
              else { return ""; }


              //users.CustomerLoadByLikeName(loginUser.OrganizationID, filter, startIndex, true);

              //if (users.Count > 0)
              //{            

              //    foreach (UsersViewItem item in users)
              //    {
              //            builder.Append(CreateContactBox(item));
              //    }

              //    return builder.ToString();
              //}
              //else
              //{
              //    return CreateNoResults();
              //}

          }

          [WebMethod]
          public string GetMoreResults(string filterType, string filter, int startIndex)
          {
              string results = "";

              switch (filterType)
              {
                  case "all":
                      results = GetSearchResults(filter, startIndex);
                      break;
                  case "customers":
                      results = GetCompanies(filter, startIndex);
                      break;
                  case "contacts":
                      results = GetContacts(filter, startIndex);
                      break;

              }

              if (results.Contains("No Search Results Found!"))
                  return "";
              else
                  return results;
          }
         */

        [WebMethod]
        public string UpdateRecentlyViewed(string viewid)
        {
            int refType, refID;

            if (viewid.StartsWith("u"))
                refType = 0;
            else
                refType = 1;

            refID = Convert.ToInt32(viewid.Substring(1));

            RecentlyViewedItem recent = (new RecentlyViewedItems(TSAuthentication.GetLoginUser()).AddNewRecentlyViewedItem());


            recent.RefID = refID;
            recent.RefType = refType;
            recent.DateViewed = DateTime.UtcNow;
            recent.UserID = TSAuthentication.GetLoginUser().UserID;
            recent.BaseCollection.Save();

            return GetRecentlyViewed();
        }

        [WebMethod]
        public string GetRecentlyViewed()
        {
            StringBuilder builder = new StringBuilder();
            RecentlyViewedItems recent = new RecentlyViewedItems(TSAuthentication.GetLoginUser());
            recent.LoadRecentForCustomerPage(TSAuthentication.GetLoginUser().UserID);

            builder.Append(@"<ul class=""recent-list"">");
            foreach (RecentlyViewedItem item in recent)
            {
                builder.Append(CreateRecentlyViewed(item));
            }
            builder.Append("</ul>");
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedAttributes.Add("data-organizationid");

            return (builder.ToString());
        }

        [WebMethod]
        public SlaLevelProxy[] LoadSlas()
        {
            SlaLevels table = new SlaLevels(TSAuthentication.GetLoginUser());
            table.LoadByOrganizationID(TSAuthentication.OrganizationID);
            return table.GetSlaLevelProxies();
        }

        [WebMethod]
        public SlaLevelProxy[] LoadOrgSlas()
        {
            SlaLevels table = new SlaLevels(TSAuthentication.GetLoginUser());
            table.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);
            return table.GetSlaLevelProxies();
        }

        [WebMethod]
        public System.Collections.ObjectModel.ReadOnlyCollection<TimeZoneInfo> LoadTimeZones()
        {
            System.Collections.ObjectModel.ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
            return timeZones;
        }

        [WebMethod]
        public UserProxy[] LoadUsers()
        {
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByOrganizationID(TSAuthentication.OrganizationID, true);

            return users.GetUserProxies();
        }

        [WebMethod]
        public UserProxy[] LoadOrgUsers(int organizationID)
        {
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByOrganizationIDLastName(organizationID, true);
            return users.GetUserProxies();
        }

        [WebMethod]
        public UserProxy[] LoadOrgSupportUsers(int organizationID)
        {
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID, true);
            return users.GetUserProxies();
        }

        [WebMethod]
        public GroupProxy[] LoadOrgGroups(int organizationID)
        {
            Groups groups = new Groups(TSAuthentication.GetLoginUser());
            groups.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);
            return groups.GetGroupProxies();
        }

        [WebMethod]
        public GroupProxy[] LoadGroups()
        {
            Groups groups = new Groups(TSAuthentication.GetLoginUser());
            groups.LoadByOrganizationID(TSAuthentication.OrganizationID);

            return groups.GetGroupProxies();
        }

        [WebMethod]
        public CustomValueProxy[] GetCustomValues(int refID, ReferenceType refType)
        {
            CustomValues values = new CustomValues(TSAuthentication.GetLoginUser());
            values.LoadByReferenceType(TSAuthentication.OrganizationID, refType, refID);
            return values.GetCustomValueProxies();
        }


        [WebMethod]
        public CustomFieldCategoryProxy[] GetCustomFieldCategories()
        {
            CustomFieldCategories cats = new CustomFieldCategories(TSAuthentication.GetLoginUser());
            cats.LoadByRefTypeWithUserRights(ReferenceType.Organizations, -1);
            return cats.GetCustomFieldCategoryProxies();
        }

        [WebMethod]
        public CustomFieldCategoryProxy[] GetCustomFieldContactCategories()
        {
            CustomFieldCategories cats = new CustomFieldCategories(TSAuthentication.GetLoginUser());
            cats.LoadByRefTypeWithUserRights(ReferenceType.Contacts, -1);
            return cats.GetCustomFieldCategoryProxies();
        }

        [WebMethod]
        public string LoadCustomProperties(int refID, ReferenceType refType)
        {
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            fields.LoadByReferenceType(TSAuthentication.OrganizationID, refType, -1);

            CustomFieldCategories cats = new CustomFieldCategories(TSAuthentication.GetLoginUser());
            cats.LoadByRefType(refType, -1);

            StringBuilder htmltest = new StringBuilder("");
            foreach (CustomField field in fields)
            {
                if (field.CustomFieldCategoryID == null)
                {
                    switch (field.FieldType)
                    {
                        case CustomFieldType.Text: htmltest.AppendLine(CreateTextControl(field, true, refID)); break;
                        case CustomFieldType.Number: htmltest.AppendLine(CreateNumberControl(field, true, refID)); break;
                        case CustomFieldType.Date: htmltest.AppendLine(CreateDateControl(field, true, refID)); break;
                        case CustomFieldType.DateTime: htmltest.AppendLine(CreateDateTimeControl(field, true, refID)); break;
                        case CustomFieldType.Boolean: htmltest.AppendLine(CreateBooleanControl(field, true, refID)); break;
                        case CustomFieldType.PickList: htmltest.AppendLine(CreatePickListControl(field, true, refID)); break;
                        default: break;

                    }
                }
            }

            foreach (CustomFieldCategory cat in cats)
            {

                foreach (CustomField field in fields)
                {
                    if (field.CustomFieldCategoryID == cat.CustomFieldCategoryID)
                    {
                        switch (field.FieldType)
                        {
                            case CustomFieldType.Text: htmltest.AppendLine(CreateTextControl(field, true, refID)); break;
                            case CustomFieldType.Number: htmltest.AppendLine(CreateNumberControl(field, true, refID)); break;
                            case CustomFieldType.Date: htmltest.AppendLine(CreateDateControl(field, true, refID)); break;
                            case CustomFieldType.DateTime: htmltest.AppendLine(CreateDateTimeControl(field, true, refID)); break;
                            case CustomFieldType.Boolean: htmltest.AppendLine(CreateBooleanControl(field, true, refID)); break;
                            case CustomFieldType.PickList: htmltest.AppendLine(CreatePickListControl(field, true, refID)); break;
                            default: break;
                        }
                    }
                }
            }


            return htmltest.ToString();
        }

        [WebMethod]
        public string LoadCustomControls(ReferenceType refType)
        {
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            fields.LoadByReferenceType(TSAuthentication.OrganizationID, refType, -1);
            //int count = 0;
            CustomFieldCategories cats = new CustomFieldCategories(TSAuthentication.GetLoginUser());
            cats.LoadByRefType(refType, -1);

            StringBuilder htmltest = new StringBuilder("");

            //htmltest.Append("<div class='form-group'>");

            foreach (CustomField field in fields)
            {
                //if (count == 0)
                //{
                //    htmltest.Append("<div class='row'>");
                //    count++;
                //}

                if (field.CustomFieldCategoryID == null)
                {
                    htmltest.Append("<div class='form-group'>");
                    htmltest.Append("<div class='row'>");
                    htmltest.AppendFormat("<div class='col-xs-12'><label for='{0}' class='col-xs-3 control-label'>{1}</label>", field.CustomFieldID, field.Name);
                    switch (field.FieldType)
                    {
                        case CustomFieldType.Text: htmltest.AppendLine(CreateTextControl(field)); break;
                        case CustomFieldType.Number: htmltest.AppendLine(CreateNumberControl(field)); break;
                        case CustomFieldType.Time: htmltest.AppendLine(CreateTimeControl(field)); break;
                        case CustomFieldType.Date: htmltest.AppendLine(CreateDateControl(field)); break;
                        case CustomFieldType.DateTime: htmltest.AppendLine(CreateDateTimeControl(field)); break;
                        case CustomFieldType.Boolean: htmltest.AppendLine(CreateBooleanControl(field)); break;
                        case CustomFieldType.PickList: htmltest.AppendLine(CreatePickListControl(field)); break;
                        default: break;
                    }
                    htmltest.Append("</div>");
                    //count++;
                    htmltest.Append("</div>"); //end row
                    htmltest.Append("</div>"); //end form-group
                }

                //if (count % 4 == 0)
                //{
                //    htmltest.Append("</div>"); //end row
                //    count = 0;
                //}
            }
            //if (count != 0)
            //{
            //    count = 0;
            //    htmltest.Append("</div>"); // end row if not closed
            //}
            //htmltest.Append("</div>"); //end form-group

            //count = 0;

            foreach (CustomFieldCategory cat in cats)
            {

                htmltest.AppendFormat("<h3>{0}</h3>", cat.Category);
                //htmltest.Append("<div class='form-group'>");

                foreach (CustomField field in fields)
                {
                    //if (count == 0)
                    //{
                    //    htmltest.Append("<div class='row'>");
                    //    count++;
                    //}

                    if (field.CustomFieldCategoryID == cat.CustomFieldCategoryID)
                    {
                        htmltest.Append("<div class='form-group'>");
                        htmltest.Append("<div class='row'>");
                        htmltest.AppendFormat("<div class='col-xs-12'><label for='{0}' class='col-xs-3 control-label'>{1}</label>", field.CustomFieldID, field.Name);
                        switch (field.FieldType)
                        {
                            case CustomFieldType.Text: htmltest.AppendLine(CreateTextControl(field)); break;
                            case CustomFieldType.Number: htmltest.AppendLine(CreateNumberControl(field)); break;
                            case CustomFieldType.Date: htmltest.AppendLine(CreateDateControl(field)); break;
                            case CustomFieldType.DateTime: htmltest.AppendLine(CreateDateTimeControl(field)); break;
                            case CustomFieldType.Boolean: htmltest.AppendLine(CreateBooleanControl(field)); break;
                            case CustomFieldType.PickList: htmltest.AppendLine(CreatePickListControl(field)); break;
                            default: break;
                        }
                        htmltest.Append("</div>");
                        //count++;
                        htmltest.Append("</div>");
                        htmltest.Append("</div>");
                    }

                    //if (count % 4 == 0)
                    //{
                    //    htmltest.Append("</div>");
                    //    count = 0;
                    //}
                }
                //if (count != 0){
                //    count = 0;
                //    htmltest.Append("</div>");
                //}
                htmltest.Append("</div>");
            }

            return htmltest.ToString();
        }

        [WebMethod]
        public string LoadCustomContactControls()
        {
            //int count = 0;
            StringBuilder htmltest = new StringBuilder("");
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            fields.LoadByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Contacts, -1);

            if (fields.Count > 0)
                //htmltest.Append("<div class='form-group'>");

                foreach (CustomField field in fields)
                {
                    //if (count == 0)
                    //{
                    //    htmltest.Append("<div class='row'>");
                    //    count++;
                    //}

                    htmltest.Append("<div class='form-group'>");
                    htmltest.Append("<div class='row'>");
                    htmltest.AppendFormat("<div class='col-xs-12'><label for='{0}' class='col-xs-3 control-label'>{1}</label>", field.CustomFieldID, field.Name);
                    switch (field.FieldType)
                    {
                        case CustomFieldType.Text: htmltest.AppendLine(CreateTextControl(field)); break;
                        case CustomFieldType.Number: htmltest.AppendLine(CreateTextControl(field)); break;
                        case CustomFieldType.Date: htmltest.AppendLine(CreateDateControl(field)); break;
                        case CustomFieldType.Time: htmltest.AppendLine(CreateTimeControl(field)); break;
                        case CustomFieldType.DateTime: htmltest.AppendLine(CreateDateTimeControl(field)); break;
                        case CustomFieldType.Boolean: htmltest.AppendLine(CreateBooleanControl(field)); break;
                        case CustomFieldType.PickList: htmltest.AppendLine(CreatePickListControl(field)); break;
                        default: break;
                    }
                    htmltest.Append("</div>");
                    //count++;


                    //if (count % 4 == 0)
                    //{
                    //    htmltest.Append("</div>");
                    //    count = 0;
                    //}
                    htmltest.Append("</div>");
                    htmltest.Append("</div>");
                }
            //if (count != 0)
            //    {
            //        count = 0;
            //        htmltest.Append("</div>");
            //    }
            htmltest.Append("</div>");

            return htmltest.ToString();
        }

        [WebMethod]
        public string[] LoadcustomProductHeaders()
        {
            List<string> columns = new List<string>();
            string col;
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            fields.LoadByReferenceType(TSAuthentication.GetLoginUser().OrganizationID, ReferenceType.OrganizationProducts);
            int count = 0;


            foreach (CustomField field in fields)
            {
                if (count >= 25) break;
                columns.Add(field.Name);
                count++;
            }


            return columns.ToArray();
        }

        [WebMethod]
        public string[] LoadcustomContactProductHeaders()
        {
            List<string> columns = new List<string>();
            string col;
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            fields.LoadByReferenceType(TSAuthentication.GetLoginUser().OrganizationID, ReferenceType.UserProducts);
            int count = 0;


            foreach (CustomField field in fields)
            {
                if (count >= 25) break;
                columns.Add(field.Name);
                count++;
            }


            return columns.ToArray();
        }

        [WebMethod]
        public ActionLogProxy[] LoadHistory(int organizationID, int start)
        {
            ActionLogs actionLogs = new ActionLogs(TSAuthentication.GetLoginUser());
            actionLogs.LoadByOrganizationIDLimit(organizationID, start);

            return actionLogs.GetActionLogProxies();
        }

        [WebMethod]
        public ActionLogProxy[] LoadContactHistory(int userID, int start)
        {
            ActionLogs actionLogs = new ActionLogs(TSAuthentication.GetLoginUser());
            actionLogs.LoadByUserIDLimit(userID, start);
            return actionLogs.GetActionLogProxies();
        }

        [WebMethod]
        public CustomerHubProxy[] LoadCustomerHubsByContactID(int userID)
        {
            CustomerHubs customerHubsHelper = new CustomerHubs(TSAuthentication.GetLoginUser());
            customerHubsHelper.LoadByContactID(userID);

            return customerHubsHelper.GetCustomerHubProxies();
        }

        [WebMethod]
        public AutocompleteItem[] SearchNotes(string searchString)
        {
            Notes notes = new Notes(TSAuthentication.GetLoginUser());
            notes.SearchNotes(searchString, TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).OrganizationID.ToString());
            var notesArray = notes.GetNoteProxies();

            List<AutocompleteItem> items = new List<AutocompleteItem>();
            foreach (NoteProxy n in notesArray)
            {
                var organizationName = "";
                //org
                switch (n.RefType)
                {
                    case ReferenceType.Organizations:
                        Organizations org = new Organizations(TSAuthentication.GetLoginUser());
                        org.LoadByOrganizationID(n.RefID);
                        organizationName = org.FirstOrDefault().Name;
                        break;
                    case ReferenceType.Users:
                        Users user = new Users(TSAuthentication.GetLoginUser());
                        user.LoadByUserID(n.RefID);
                        Organizations userorg = new Organizations(TSAuthentication.GetLoginUser());
                        userorg.LoadByOrganizationID(user.FirstOrDefault().OrganizationID);
                        organizationName = userorg.FirstOrDefault().Name;
                        break;
                }


                items.Add(new AutocompleteItem(string.Format("{0} [{1}]", n.Title, organizationName), n.NoteID.ToString(), n));
            }

            return items.ToArray();
        }

        [WebMethod]
        public NoteProxy[] LoadNotes(int refID, ReferenceType refType)
        {
            Notes notes = new Notes(TSAuthentication.GetLoginUser());
            notes.LoadByReferenceType(refType, refID);
            return notes.GetNoteProxies();
        }

        [WebMethod]
        public NoteProxy[] LoadNotes2(int refID, ReferenceType refType, bool includeChildren)
        {
            Notes notes = new Notes(TSAuthentication.GetLoginUser());
            if (refType == ReferenceType.Users)
                notes.LoadByReferenceTypeUser(refType, refID, "DateCreated", includeChildren);
            else
                notes.LoadByReferenceType(refType, refID, "DateCreated", includeChildren);

            ActivityTypes activities = new ActivityTypes(TSAuthentication.GetLoginUser());
            activities.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

            var notesProxy = notes.GetNoteProxies();
            foreach (var note in notesProxy)
            {
                if (refType == ReferenceType.Users)
                {
                    note.Owner = Users.GetUserFullName(TSAuthentication.GetLoginUser(), note.RefID);
                }
                note.Attachments = LoadFiles(note.NoteID, refType == ReferenceType.Organizations ? ReferenceType.CompanyActivity : ReferenceType.ContactActivity);
                if (note.ActivityType < 5) //default values
                {
                    note.ActivityTypeString = Enum.GetName(typeof(ActivityTypeEnum), note.ActivityType);
                }
                else //custom values
                {
                    note.ActivityTypeString = activities.First(x => x.ActivityTypeID == note.ActivityType).Name;
                }
            }

            return notesProxy;
        }

        [WebMethod]
        public NoteProxy[] LoadNotesByUserRights(int refID, ReferenceType refType, bool includeChildren, string organizationID = "")
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Notes notes = new Notes(loginUser);
            if (organizationID != null)
                notes.LoadByReferenceTypeByUserRightsUsers(refType, refID, loginUser.UserID, organizationID, "DateCreated", includeChildren);
            else
                notes.LoadByReferenceTypeByUserRights(refType, refID, loginUser.UserID, "DateCreated", includeChildren);

            var notesProxy = notes.GetNoteProxies();

            ActivityTypes activities = new ActivityTypes(TSAuthentication.GetLoginUser());
            activities.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

            foreach (var note in notesProxy)
            {
                note.Attachments = LoadFiles(note.NoteID, refType == ReferenceType.Organizations ? ReferenceType.CompanyActivity : ReferenceType.ContactActivity);
                if(refType == ReferenceType.Users)
                {
                    note.Owner = Users.GetUserFullName(TSAuthentication.GetLoginUser(), note.RefID);
                }
                if (note.ActivityType < 5) //default values
                {
                    note.ActivityTypeString = Enum.GetName(typeof(ActivityTypeEnum), note.ActivityType);
                }
                else //custom values
                {
                    note.ActivityTypeString = activities.First(x => x.ActivityTypeID == note.ActivityType).Name;
                }
            }

            return notesProxy;
        }

        [WebMethod]
        public NoteProxy LoadNote(int noteID)
        {
            Notes notes = new Notes(TSAuthentication.GetLoginUser());
            notes.LoadByNoteID(noteID);

            var notesProxy = notes[0].GetProxy();
            notesProxy.Attachments = LoadFiles(notesProxy.NoteID, notesProxy.RefType == ReferenceType.Organizations ? ReferenceType.CompanyActivity : ReferenceType.ContactActivity);

            return notesProxy;
        }

        [WebMethod]
        public NoteProxy[] LoadAllTicketAlerts(int ticketID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Notes notes = new Notes(loginUser);
            notes.LoadByTicketUserAndActiveAlert(ticketID, loginUser.UserID);
            return notes.GetNoteProxies();
        }

        [System.Obsolete("Replaced with LoadAllTicketAlerts.")]
        [WebMethod]
        public NoteProxy LoadTicketAlerts(int ticketID)
        {
            TicketCustomer[] customers;
            customers = GetTicketCustomers(ticketID);
            NoteProxy note = null;

            foreach (TicketCustomer cust in customers)
            {
                if (cust.UserID.HasValue)
                {
                    note = LoadAlert((int)cust.UserID, ReferenceType.Users);
                    if (note == null)
                        note = LoadAlert(cust.OrganizationID, ReferenceType.Organizations);
                }
                else
                {
                    note = LoadAlert(cust.OrganizationID, ReferenceType.Organizations);
                }

                if (note != null)
                    return note;
            }

            return note;

        }

        private TicketCustomer[] GetTicketCustomers(int ticketID)
        {
            List<TicketCustomer> customers = new List<TicketCustomer>();

            ContactsView contacts = new ContactsView(TSAuthentication.GetLoginUser());
            contacts.LoadByTicketID(ticketID);


            foreach (ContactsViewItem contact in contacts)
            {
                TicketCustomer customer = new TicketCustomer();
                customer.Company = contact.Organization;
                customer.OrganizationID = contact.OrganizationID;
                customer.Contact = contact.FirstName + " " + contact.LastName;
                customer.UserID = contact.UserID;
                customers.Add(customer);
            }

            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByNotContactTicketID(ticketID);
            foreach (Organization organization in organizations)
            {
                TicketCustomer customer = new TicketCustomer();
                customer.Company = organization.Name;
                customer.OrganizationID = organization.OrganizationID;
                customer.UserID = null;
                customers.Add(customer);
            }
            return customers.ToArray();
        }

        [System.Obsolete("Replaced with LoadAlerts.")]
        [WebMethod]
        public NoteProxy LoadUserAlert(int userID)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            return LoadAlert(u.OrganizationID, ReferenceType.Organizations);
        }


        [System.Obsolete("Replaced with LoadAlerts.")]
        [WebMethod]
        public NoteProxy LoadAlert(int refID, ReferenceType refType)
        {
            Notes notes = new Notes(TSAuthentication.GetLoginUser());
            UserNoteSettings us = new UserNoteSettings(TSAuthentication.GetLoginUser());

            notes.LoadbyIsAlert(refType, refID);

            if (notes.IsEmpty)
                return null;
            else
            {
                us.LoadByUserNoteID(TSAuthentication.GetLoginUser().UserID, notes[0].NoteID, refType);

                if (us.IsEmpty)
                    return notes[0].GetProxy();
                else
                {
                    if (us[0].IsDismissed)
                        return null;

                    if (!us[0].IsSnoozed)
                    {
                        return notes[0].GetProxy();
                    }

                    if (us[0].IsSnoozed && (us[0].SnoozeTimeUtc.AddHours(8) < DateTime.Now))
                    {
                        return notes[0].GetProxy();
                    }
                    else
                        return null;


                }


                // search usernotesettings for noteid and userid
                // if found check if isDismissed is set
                //  if so return null
                // if isSnozzed is set
                // check if snooze time is within 8 hours of current time
                // if so return null
                // else return note notes[0].NoteID


            }
        }

        [WebMethod]
        public NoteProxy[] LoadAlerts(int refID, ReferenceType refType)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Notes notes = new Notes(loginUser);
            // Because somewhere the order is being changed we order newer to older so alerts are displayed older to newer.
            notes.LoadByUserAndActiveAlert(refType, refID, loginUser.UserID);
            return notes.GetNoteProxies();
        }

        [WebMethod]
        public void SnoozeAlertByID(int noteID, ReferenceType refType)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            UserNoteSettings us = new UserNoteSettings(loginUser);
            us.LoadByUserNoteID(loginUser.UserID, noteID, refType);

            if (us.IsEmpty)
            {
                UserNoteSetting u = new UserNoteSettings(loginUser).AddNewUserNoteSetting();
                u.RefID = noteID;
                u.RefType = refType;
                u.UserID = loginUser.UserID;
                u.SnoozeTime = DateTime.Now;
                u.IsSnoozed = true;
                u.Collection.Save();
            }
            else
            {
                us[0].IsSnoozed = true;
                us[0].SnoozeTime = DateTime.Now;
                us[0].Collection.Save();
            }
        }

        [WebMethod]
        public void DismissAlertByID(int noteID, ReferenceType refType)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            UserNoteSettings us = new UserNoteSettings(loginUser);
            us.LoadByUserNoteID(loginUser.UserID, noteID, refType);

            if (us.IsEmpty)
            {
                UserNoteSetting u = new UserNoteSettings(loginUser).AddNewUserNoteSetting();
                u.RefID = noteID;
                u.RefType = refType;
                u.UserID = loginUser.UserID;
                u.IsDismissed = true;
                u.SnoozeTime = DateTime.Now;
                u.Collection.Save();
            }
            else
            {
                us[0].IsDismissed = true;
                us[0].Collection.Save();
            }
        }

        [System.Obsolete("Replaced with SnoozeAlertByID.")]
        [WebMethod]
        public void SnoozeAlert(int refID, ReferenceType refType)
        {
            Notes notes = new Notes(TSAuthentication.GetLoginUser());
            notes.LoadbyIsAlert(refType, refID);

            UserNoteSettings us = new UserNoteSettings(TSAuthentication.GetLoginUser());
            us.LoadByUserNoteID(TSAuthentication.GetLoginUser().UserID, notes[0].NoteID, refType);

            if (us.IsEmpty)
            {
                UserNoteSetting u = new UserNoteSettings(TSAuthentication.GetLoginUser()).AddNewUserNoteSetting();
                u.RefID = notes[0].NoteID;
                u.RefType = refType;
                u.UserID = TSAuthentication.GetLoginUser().UserID;
                u.SnoozeTime = DateTime.Now;
                u.IsSnoozed = true;
                u.Collection.Save();
            }
            else
            {
                us[0].IsSnoozed = true;
                us[0].SnoozeTime = DateTime.Now;
                us[0].Collection.Save();
            }
        }

        [System.Obsolete("Replaced with DismissAlertByID.")]
        [WebMethod]
        public void DismissAlert(int refID, ReferenceType refType)
        {
            Notes notes = new Notes(TSAuthentication.GetLoginUser());
            notes.LoadbyIsAlert(refType, refID);

            UserNoteSettings us = new UserNoteSettings(TSAuthentication.GetLoginUser());
            us.LoadByUserNoteID(TSAuthentication.GetLoginUser().UserID, notes[0].NoteID, refType);

            if (us.IsEmpty)
            {
                UserNoteSetting u = new UserNoteSettings(TSAuthentication.GetLoginUser()).AddNewUserNoteSetting();
                u.RefID = notes[0].NoteID;
                u.RefType = refType;
                u.UserID = TSAuthentication.GetLoginUser().UserID;
                u.IsDismissed = true;
                u.SnoozeTime = DateTime.Now;
                u.Collection.Save();
            }
            else
            {
                us[0].IsDismissed = true;
                us[0].Collection.Save();
            }
        }

        [WebMethod]
        public AttachmentProxy[] LoadFiles(int refID, ReferenceType refType)
        {
            return TeamSupport.Data.Quarantine.WebAppQ.GetAttachmentProxies1(refID, refType, TSAuthentication.GetLoginUser());
        }

        [WebMethod]
        public AttachmentProxy[] LoadFiles2(int refID, ReferenceType refType, bool includeChildren)
        {
            return TeamSupport.Data.Quarantine.WebAppQ.GetAttachmentProxies2(refID, refType, includeChildren, TSAuthentication.GetLoginUser());
        }

        [WebMethod]
        public AttachmentProxy[] LoadFilesByUserRights(int refID, ReferenceType refType, bool includeChildren)
        {
            return TeamSupport.Data.Quarantine.WebAppQ.GetAttachmentProxies3(refID, refType, includeChildren, TSAuthentication.GetLoginUser());
        }


        [WebMethod]
        public OrganizationCustomProduct[] LoadProducts(int organizationID, string sortColumn, string sortDirection)
        {
            OrganizationProducts organizationProducts = new OrganizationProducts(TSAuthentication.GetLoginUser());
            organizationProducts.LoadForCustomerProductGridSorting(organizationID, GetSortColumnTableName(sortColumn), sortDirection);
            List<OrganizationCustomProduct> list = new List<OrganizationCustomProduct>();
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            fields.LoadByReferenceType(TSAuthentication.GetLoginUser().OrganizationID, ReferenceType.OrganizationProducts);

            SlaLevels slaLevels = new SlaLevels(TSAuthentication.GetLoginUser());
            slaLevels.LoadByOrganizationID(organizationID);


            foreach (DataRow row in organizationProducts.Table.Rows)
            {
                OrganizationCustomProduct test = new OrganizationCustomProduct();
                test.ProductName = row["Product"].ToString();
                test.VersionNumber = row["VersionNumber"].ToString();
                test.SupportExpiration = row["SupportExpiration"].ToString() != "" ? DataUtils.DateToLocal(TSAuthentication.GetLoginUser(), (((DateTime)row["SupportExpiration"]))).ToString(GetDateFormatNormal()) : "";
                test.VersionStatus = row["VersionStatus"].ToString();
                test.IsReleased = row["IsReleased"].ToString();
                test.ReleaseDate = row["ReleaseDate"].ToString() != "" ? ((DateTime)row["ReleaseDate"]).ToString(GetDateFormatNormal()) : "";
                test.SlaAssigned = row["SlaLevelName"].ToString();
                test.DateCreated = row["DateCreated"].ToString() != "" ? ((DateTime)row["DateCreated"]).ToString(GetDateFormatNormal()) : "";
                test.OrganizationProductID = (int)row["OrganizationProductID"];
                test.CustomFields = new List<string>();
                foreach (CustomField field in fields)
                {
                    CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, test.OrganizationProductID);
                    test.CustomFields.Add(customValue.Value);
                }


                list.Add(test);
            }


            return list.ToArray();
        }

        [WebMethod]
        public OrganizationCustomProduct[] LoadProducts2(int organizationID, string sortColumn, string sortDirection, bool includeChildren)
        {
            OrganizationProducts organizationProducts = new OrganizationProducts(TSAuthentication.GetLoginUser());
            organizationProducts.LoadForCustomerProductGridSorting(organizationID, GetSortColumnTableName(sortColumn), sortDirection, includeChildren);
            List<OrganizationCustomProduct> list = new List<OrganizationCustomProduct>();
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            fields.LoadByReferenceType(TSAuthentication.GetLoginUser().OrganizationID, ReferenceType.OrganizationProducts);

            foreach (DataRow row in organizationProducts.Table.Rows)
            {
                OrganizationCustomProduct test = new OrganizationCustomProduct();
                test.ProductName = row["Product"].ToString();
                test.VersionNumber = row["VersionNumber"].ToString();
                test.SupportExpiration = row["SupportExpiration"].ToString() != "" ? DataUtils.DateToLocal(TSAuthentication.GetLoginUser(), (((DateTime)row["SupportExpiration"]))).ToString(GetDateFormatNormal()) : "";
                test.VersionStatus = row["VersionStatus"].ToString();
                test.IsReleased = row["IsReleased"].ToString();
                test.ReleaseDate = row["ReleaseDate"].ToString() != "" ? ((DateTime)row["ReleaseDate"]).ToString(GetDateFormatNormal()) : "";
                test.SlaAssigned = row["SlaLevelName"].ToString();
                test.DateCreated = row["DateCreated"].ToString() != "" ? ((DateTime)row["DateCreated"]).ToString(GetDateFormatNormal()) : "";
                test.OrganizationProductID = (int)row["OrganizationProductID"];
                test.CustomFields = new List<string>();
                foreach (CustomField field in fields)
                {
                    CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, test.OrganizationProductID);
                    test.CustomFields.Add(customValue.Value);
                }


                list.Add(test);
            }


            return list.ToArray();
        }

        [WebMethod]
        public CustomerSLATrigger[] LoadSlaTriggers(int organizationID, int customerId, string sortColumn, string sortDirection)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            SlaTriggersView slaTriggers = new SlaTriggersView(loginUser);
            slaTriggers.LoadByCustomer(organizationID, customerId);
            List<CustomerSLATrigger> list = new List<CustomerSLATrigger>();

            foreach (DataRow row in slaTriggers.Table.Rows)
            {
                CustomerSLATrigger customerSlaTrigger = new CustomerSLATrigger();
                customerSlaTrigger.SlaLevelId = int.Parse(row["SlaLevelID"].ToString());
                customerSlaTrigger.SlaTriggerId = int.Parse(row["SlaTriggerID"].ToString());
                customerSlaTrigger.LevelName = row["LevelName"].ToString();
                customerSlaTrigger.Severity = row["Severity"].ToString();
                customerSlaTrigger.TicketType = row["TicketType"].ToString();
                customerSlaTrigger.SLAType = row["SLAType"].ToString();

                list.Add(customerSlaTrigger);
            }

            return list.ToArray();
        }

        [WebMethod]
        public CustomerSLAViolation[] LoadSlaViolations(int organizationID, string sortColumn, string sortDirection)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            SlaViolationHistory slaViolationHistory = new SlaViolationHistory(loginUser);
            slaViolationHistory.LoadByCustomer(organizationID, sortColumn, sortDirection);
            List<CustomerSLAViolation> list = new List<CustomerSLAViolation>();

            foreach (DataRow row in slaViolationHistory.Table.Rows)
            {
                CustomerSLAViolation customerSlaViolation = new CustomerSLAViolation();
                customerSlaViolation.TicketId = int.Parse(row["TicketID"].ToString());
                customerSlaViolation.TicketNumber = int.Parse(row["TicketNumber"].ToString());
                customerSlaViolation.Violation = row["Violation"].ToString();
                customerSlaViolation.DateViolated = DateTime.Parse(row["DateViolated"].ToString());
                customerSlaViolation.LevelName = row["LevelName"].ToString();
                customerSlaViolation.Severity = row["SlaSeverity"].ToString();
                customerSlaViolation.TicketType = row["SlaTicketType"].ToString();

                list.Add(customerSlaViolation);
            }

            return list.ToArray();
        }

        [WebMethod]
        public UserCustomProduct[] LoadContactProducts(int contactID, string sortColumn, string sortDirection)
        {
            UserProducts userProducts = new UserProducts(TSAuthentication.GetLoginUser());
            userProducts.LoadForContactProductGridSorting(contactID, GetSortColumnTableName(sortColumn), sortDirection);
            List<UserCustomProduct> list = new List<UserCustomProduct>();
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            fields.LoadByReferenceType(TSAuthentication.GetLoginUser().OrganizationID, ReferenceType.UserProducts);

            foreach (DataRow row in userProducts.Table.Rows)
            {
                UserCustomProduct test = new UserCustomProduct();
                test.ProductName = row["Product"].ToString();
                test.VersionNumber = row["VersionNumber"].ToString();
                test.SupportExpiration = row["SupportExpiration"].ToString() != "" ? DataUtils.DateToLocal(TSAuthentication.GetLoginUser(), (((DateTime)row["SupportExpiration"]))).ToString(GetDateFormatNormal()) : "";
                test.VersionStatus = row["VersionStatus"].ToString();
                test.IsReleased = row["IsReleased"].ToString();
                test.ReleaseDate = row["ReleaseDate"].ToString() != "" ? ((DateTime)row["ReleaseDate"]).ToString(GetDateFormatNormal()) : "";
                test.DateCreated = row["DateCreated"].ToString() != "" ? ((DateTime)row["DateCreated"]).ToString(GetDateFormatNormal()) : "";
                test.UserProductID = (int)row["UserProductID"];
                test.CustomFields = new List<string>();
                foreach (CustomField field in fields)
                {
                    CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, test.UserProductID);
                    test.CustomFields.Add(customValue.Value);
                }


                list.Add(test);
            }


            return list.ToArray();
        }

        private string GetSortColumnTableName(string interfaceName)
        {
            string result = "OrganizationProductID";
            switch (interfaceName.ToLower())
            {
                case "product name":
                    result = "Product";
                    break;
                case "version":
                    result = "VersionNumber";
                    break;
                case "support expiration":
                    result = "SupportExpiration";
                    break;
                case "status":
                    result = "VersionStatus";
                    break;
                case "released":
                    result = "IsReleased";
                    break;
                case "sla assigned":
                    result = "SlaLevelName";
                    break;
                case "released date":
                    result = "ReleaseDate";
                    break;
                case "date created":
                    result = "OrganizationProductID";
                    break;
            }
            return result;
        }

        [WebMethod]
        public OrganizationCustomProduct LoadProduct(int productID)
        {
            OrganizationProduct organizationProduct = (OrganizationProduct)OrganizationProducts.GetOrganizationProduct(TSAuthentication.GetLoginUser(), productID);
            OrganizationCustomProduct custProd = new OrganizationCustomProduct();

            custProd.ProductName = organizationProduct.ProductID.ToString();
            custProd.VersionNumber = organizationProduct.ProductVersionID.HasValue ? organizationProduct.ProductVersionID.ToString() : "-1";

            if (organizationProduct.SupportExpiration.HasValue)
                custProd.SupportExpiration = ((DateTime)organizationProduct.SupportExpiration).ToString(GetDateFormatNormal());
            else
                custProd.SupportExpiration = "";

            custProd.VersionStatus = "";
            custProd.IsReleased = "";
            custProd.ReleaseDate = null;
            custProd.OrganizationProductID = organizationProduct.OrganizationProductID;
            custProd.ProductID = organizationProduct.ProductID;
            custProd.SlaLevelID = organizationProduct.SlaLevelID != null ? (int)organizationProduct.SlaLevelID : -1;

            return custProd;
        }

        [WebMethod]
        public UserCustomProduct LoadContactProduct(int productID)
        {
            UserProduct userProduct = (UserProduct)UserProducts.GetUserProduct(TSAuthentication.GetLoginUser(), productID);
            UserCustomProduct userProd = new UserCustomProduct();

            userProd.ProductName = userProduct.ProductID.ToString();
            userProd.VersionNumber = userProduct.ProductVersionID.HasValue ? userProduct.ProductVersionID.ToString() : "-1";

            if (userProduct.SupportExpiration.HasValue)
                userProd.SupportExpiration = ((DateTime)userProduct.SupportExpiration).ToString(GetDateFormatNormal());
            else
                userProd.SupportExpiration = "";

            userProd.VersionStatus = "";
            userProd.IsReleased = "";
            userProd.ReleaseDate = null;
            userProd.UserProductID = userProduct.UserProductID;
            userProd.ProductID = userProduct.ProductID;

            return userProd;
        }

        [WebMethod]
        public EmailAddressProxy[] LoadEmails(int refID, ReferenceType refType)
        {
            EmailAddresses emails = new EmailAddresses(TSAuthentication.GetLoginUser());
            emails.LoadByRefID(refID, refType);

            return emails.GetEmailAddressProxies();
        }

        [WebMethod]
        public PhoneNumberProxy[] LoadPhoneNumbers(int refID, ReferenceType refType)
        {
            PhoneNumbers phoneNumbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
            phoneNumbers.LoadByID(refID, refType);

            return phoneNumbers.GetPhoneNumberProxies();
        }

        [WebMethod]
        public ProductProxy[] LoadProductTypes()
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Products products = new Products(loginUser);
            Organization organization = Organizations.GetOrganization(loginUser, loginUser.OrganizationID);
            if (organization.UseProductFamilies)
            {
                products.LoadByOrganizationIDAndUserRights(loginUser.OrganizationID, loginUser.UserID);
            }
            else
            {
                products.LoadByOrganizationID(loginUser.OrganizationID);
            }

            return products.GetProductProxies();
        }

        [WebMethod]
        public ProductVersionProxy[] LoadProductVersions(int productID)
        {
            ProductVersions productVersions = new ProductVersions(TSAuthentication.GetLoginUser());
            productVersions.LoadByProductID(productID);

            return productVersions.GetProductVersionProxies();
        }

        [WebMethod]
        public SlaLevelProxy[] LoadSlaLevels()
        {
            SlaLevels slaLevels = new SlaLevels(TSAuthentication.GetLoginUser());
            slaLevels.LoadByOrganizationID(TSAuthentication.OrganizationID);

            return slaLevels.GetSlaLevelProxies();
        }

        [WebMethod]
        public string LoadAssets(int refID, ReferenceType referenceType)
        {
            StringBuilder htmlresults = new StringBuilder("");
            AssetsView assets = new AssetsView(TSAuthentication.GetLoginUser());
            assets.LoadByRefID(refID, referenceType);

            StringBuilder productVersionNumberDisplayName;
            StringBuilder serialNumberDisplayValue;
            StringBuilder warrantyExpirationDisplayValue;

            foreach (AssetsViewItem asset in assets)
            {
                productVersionNumberDisplayName = new StringBuilder();
                serialNumberDisplayValue = new StringBuilder();
                warrantyExpirationDisplayValue = new StringBuilder();

                if (!string.IsNullOrEmpty(asset.ProductVersionNumber))
                {
                    productVersionNumberDisplayName.Append(" - " + asset.ProductVersionNumber);
                }

                if (string.IsNullOrEmpty(asset.SerialNumber))
                {
                    serialNumberDisplayValue.Append("Empty");
                }
                else
                {
                    serialNumberDisplayValue.Append(asset.SerialNumber);
                }

                if (asset.WarrantyExpiration == null)
                {
                    warrantyExpirationDisplayValue.Append("Empty");
                }
                else
                {
                    warrantyExpirationDisplayValue.Append(((DateTime)asset.WarrantyExpiration).ToString(GetDateFormatNormal()));
                }

                htmlresults.AppendFormat(@"<div class='list-group-item'>
                            <a href='#' id='{0}' class='assetLink'><h4 class='list-group-item-heading'>{1}</h4></a>
                            <div class='row'>
                                <div class='col-xs-8'>
                                    <p class='list-group-item-text'>{2}{3}</p>
                                </div>
                            </div>
                            <div class='row'>
                                <div class='col-xs-8'>
                                    <p class='list-group-item-text'>SN: {4} - Warr. Exp.: {5}</p>
                                </div>
                            </div>
                            </div>
                            </div>"

                    , asset.AssetID
                    , asset.DisplayName
                    , asset.ProductName
                    , productVersionNumberDisplayName
                    , serialNumberDisplayValue
                    , warrantyExpirationDisplayValue);
            }

            return htmlresults.ToString();
        }

        [WebMethod]
        public string LoadAssets2(int refID, ReferenceType referenceType, bool includeChildren)
        {
            StringBuilder htmlresults = new StringBuilder("");
            AssetsView assets = new AssetsView(TSAuthentication.GetLoginUser());
            assets.LoadByRefID(refID, referenceType, includeChildren);

            StringBuilder productVersionNumberDisplayName;
            StringBuilder serialNumberDisplayValue;
            StringBuilder warrantyExpirationDisplayValue;

            foreach (AssetsViewItem asset in assets.OrderBy(p => p.DisplayName))
            {
                productVersionNumberDisplayName = new StringBuilder();
                serialNumberDisplayValue = new StringBuilder();
                warrantyExpirationDisplayValue = new StringBuilder();

                if (!string.IsNullOrEmpty(asset.ProductVersionNumber))
                {
                    productVersionNumberDisplayName.Append(" - " + asset.ProductVersionNumber);
                }

                if (string.IsNullOrEmpty(asset.SerialNumber))
                {
                    serialNumberDisplayValue.Append("Empty");
                }
                else
                {
                    serialNumberDisplayValue.Append(asset.SerialNumber);
                }

                if (asset.WarrantyExpiration == null)
                {
                    warrantyExpirationDisplayValue.Append("Empty");
                }
                else
                {
                    warrantyExpirationDisplayValue.Append(((DateTime)asset.WarrantyExpiration).ToString(GetDateFormatNormal()));
                }

                htmlresults.AppendFormat(@"<div class='list-group-item'>
                            <a href='#' id='{0}' class='assetLink'><h4 class='list-group-item-heading'>{1}</h4></a>
                            <div class='row'>
                                <div class='col-xs-8'>
                                    <p class='list-group-item-text'>{2}{3}</p>
                                </div>
                            </div>
                            <div class='row'>
                                <div class='col-xs-8'>
                                    <p class='list-group-item-text'>SN: {4} - Warr. Exp.: {5}</p>
                                </div>
                            </div>
                            </div>
                            </div>"

                    , asset.AssetID
                    , asset.DisplayName
                    , asset.ProductName
                    , productVersionNumberDisplayName
                    , serialNumberDisplayValue
                    , warrantyExpirationDisplayValue);
            }

            return htmlresults.ToString();
        }

        [WebMethod]
        public string LoadContactAssets(int organizationID)
        {
            StringBuilder htmlresults = new StringBuilder("");
            AssetsView assets = new AssetsView(TSAuthentication.GetLoginUser());
            assets.LoadAssignedToContactsByOrganizationID(organizationID);

            StringBuilder productVersionNumberDisplayName;
            StringBuilder serialNumberDisplayValue;
            StringBuilder warrantyExpirationDisplayValue;

            foreach (AssetsViewItem asset in assets)
            {
                productVersionNumberDisplayName = new StringBuilder();
                serialNumberDisplayValue = new StringBuilder();
                warrantyExpirationDisplayValue = new StringBuilder();

                if (!string.IsNullOrEmpty(asset.ProductVersionNumber))
                {
                    productVersionNumberDisplayName.Append(" - " + asset.ProductVersionNumber);
                }

                if (string.IsNullOrEmpty(asset.SerialNumber))
                {
                    serialNumberDisplayValue.Append("Empty");
                }
                else
                {
                    serialNumberDisplayValue.Append(asset.SerialNumber);
                }

                if (asset.WarrantyExpiration == null)
                {
                    warrantyExpirationDisplayValue.Append("Empty");
                }
                else
                {
                    warrantyExpirationDisplayValue.Append(((DateTime)asset.WarrantyExpiration).ToString(GetDateFormatNormal()));
                }

                htmlresults.AppendFormat(@"<div class='list-group-item'>
                            <a href='#' id='{0}' class='assetLink'><h4 class='list-group-item-heading'>{1}</h4></a>
                            <div class='row'>
                                <div class='col-xs-8'>
                                    <p class='list-group-item-text'>{2}{3}</p>
                                </div>
                            </div>
                            <div class='row'>
                                <div class='col-xs-8'>
                                    <p class='list-group-item-text'>SN: {4} - Warr. Exp.: {5}</p>
                                </div>
                            </div>
                            </div>"

                    , asset.AssetID
                    , asset.DisplayName
                    , asset.ProductName
                    , productVersionNumberDisplayName
                    , serialNumberDisplayValue
                    , warrantyExpirationDisplayValue);
            }

            return htmlresults.ToString();
        }

        [WebMethod]
        public List<CustomValueProxy> LoadCustomProductFields(int productID)
        {
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            List<CustomValueProxy> custfield = new List<CustomValueProxy>();
            fields.LoadByReferenceType(TSAuthentication.GetLoginUser().OrganizationID, ReferenceType.OrganizationProducts);
            foreach (CustomField field in fields)
            {
                CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, productID);
                custfield.Add(customValue.GetProxy());
            }

            return custfield;
        }

        [WebMethod]
        public List<CustomValueProxy> LoadCustomContactProductFields(int productID)
        {
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            List<CustomValueProxy> custfield = new List<CustomValueProxy>();
            fields.LoadByReferenceType(TSAuthentication.GetLoginUser().OrganizationID, ReferenceType.UserProducts);
            foreach (CustomField field in fields)
            {
                CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, productID);
                custfield.Add(customValue.GetProxy());
            }

            return custfield;
        }

        [WebMethod]
        public EmailAddressProxy[] LoadEmail(int emailID)
        {
            EmailAddresses emails = new EmailAddresses(TSAuthentication.GetLoginUser());
            emails.LoadById(emailID);

            return emails.GetEmailAddressProxies();
        }

        [WebMethod]
        public PhoneNumberProxy[] LoadPhoneNumber(int phoneNumberID)
        {
            PhoneNumbers phoneNumbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
            phoneNumbers.LoadByPhoneID(phoneNumberID);

            return phoneNumbers.GetPhoneNumberProxies();
        }

        [WebMethod]
        public AddressProxy[] LoadAddresses(int refID, ReferenceType refType)
        {
            Addresses addresses = new Addresses(TSAuthentication.GetLoginUser());
            addresses.LoadByID(refID, refType);

            return addresses.GetAddressProxies();
        }

        [WebMethod]
        public AddressProxy[] LoadAddress(int addressID)
        {
            Addresses addresses = new Addresses(TSAuthentication.GetLoginUser());
            addresses.LoadByAddressID(addressID);

            return addresses.GetAddressProxies();
        }

        [WebMethod]
        public PhoneTypeProxy[] LoadPhoneTypes(int organizationID)
        {
            PhoneTypes phoneTypes = new PhoneTypes(TSAuthentication.GetLoginUser());
            phoneTypes.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

            return phoneTypes.GetPhoneTypeProxies();
        }

        [WebMethod]
        public string GetCustDistIndexTrend(int organizationID)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationID(organizationID);

            if (organizations[0].CustDistIndexTrend == 1)
                return "<i class='fa fa-arrow-up red'></i>";
            else if (organizations[0].CustDistIndexTrend == -1)
                return "<i class='fa fa-arrow-down green'></i>";
            else
                return "";

        }

        [WebMethod]
        public OrgProp GetProperties(int organizationID)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationID(organizationID);
            Users users = new Users(TSAuthentication.GetLoginUser());

            OrgProp orgProp = new OrgProp();

            if (organizations.IsEmpty) return null;

            orgProp.SAED = organizations[0].SAExpirationDate == null ? "[None]" : ((DateTime)organizations[0].SAExpirationDate).ToShortDateString();
            if (organizations[0].SlaLevelID == null)
                orgProp.SLA = "[None]";
            else
            {
                SlaLevel level = SlaLevels.GetSlaLevel(TSAuthentication.GetLoginUser(), (int)organizations[0].SlaLevelID);
                if (level != null)
                    orgProp.SLA = level.Name;
            }
            string primaryUser = "Empty";
            if (organizations[0].PrimaryUserID != null && organizations[0].PrimaryUserID != -1)
            {
                users.LoadByUserID((int)organizations[0].PrimaryUserID);
                primaryUser = users.IsEmpty ? "" : users[0].LastName + ", " + users[0].FirstName;
            }
            orgProp.PrimaryUser = primaryUser;
            orgProp.orgproxy = organizations[0].GetProxy();

            if (organizations[0].DefaultSupportUserID != null && organizations[0].DefaultSupportUserID != -1)
            {
                User supportUser = Users.GetUser(TSAuthentication.GetLoginUser(), (int)organizations[0].DefaultSupportUserID);
                orgProp.DefaultSupportUser = supportUser.FirstLastName;
            }
            else
            {
                orgProp.DefaultSupportUser = "Empty";
            }

            if (organizations[0].DefaultSupportGroupID != null)
            {
                Group supportGroup = (Group)Groups.GetGroup(TSAuthentication.GetLoginUser(), (int)organizations[0].DefaultSupportGroupID);
                if (supportGroup != null)
                    orgProp.SupportGroup = supportGroup.Name;
                else
                    orgProp.SupportGroup = "Empty";
            }
            else
            {
                orgProp.SupportGroup = "Empty";
            }

            orgProp.Parents = GetParents(organizationID);
            orgProp.IsParent = Organizations.GetIsParent(TSAuthentication.GetLoginUser(), organizationID);
            return orgProp;

        }

        private CompanyParentsViewItemProxy[] GetParents(int childID)
        {
            CompanyParentsView result = new CompanyParentsView(TSAuthentication.GetLoginUser());
            result.LoadByChildID(childID);
            return result.GetCompanyParentsViewItemProxies();
        }

        [WebMethod]
        public OrganizationContactCard GetCustomerCard(int organizationID)
        {
            OrganizationContactCard result = new OrganizationContactCard();

            Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), organizationID);
            if (organization == null) return null;
            result.orgproxy = organization.GetProxy();

            PhoneNumbersView numbers = new PhoneNumbersView(organization.Collection.LoginUser);
            numbers.LoadByID(organization.OrganizationID, ReferenceType.Organizations);
            result.phoneNumbers = numbers.GetPhoneNumbersViewItemProxies();

            TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());
            tickets.LoadLatest5Tickets(organizationID);
            result.tickets = tickets.GetTicketsViewItemProxies();

            return result;
        }

        public class OrganizationContactCard
        {
            [DataMember]
            public OrganizationProxy orgproxy { get; set; }
            [DataMember]
            public PhoneNumbersViewItemProxy[] phoneNumbers { get; set; }
            [DataMember]
            public TicketsViewItemProxy[] tickets { get; set; }
            //also need support hours info
        }

        [WebMethod]
        public string[] LoadChildren(int organizationID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            List<string> results = new List<string>();
            SqlCommand command = new SqlCommand();

            string pageQuery = @"
WITH 
q AS ({0}),
r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY 
    CASE 
        WHEN [NAME] IS NULL THEN 1 
        WHEN [NAME] = ''    THEN 2 
        ELSE 3 
    END DESC, 
    [NAME] ASC) AS 'RowNum' FROM q)
SELECT * INTO #X FROM r
--WHERE RowNum BETWEEN @From AND @To

SELECT 
	o.Name AS Organization, 
	o.OrganizationID, 
	o.Website, 
	o.HasPortalAccess, 
	(SELECT COUNT(*) FROM TicketsView t LEFT JOIN OrganizationTickets ot ON ot.TicketID = t.TicketID WHERE ot.OrganizationID = o.OrganizationID AND t.IsClosed = 0) AS OrgOpenTickets
FROM #X AS x
LEFT JOIN Organizations o ON o.OrganizationID = x.OrganizationID";

            string companyQuery = @"
SELECT 
  LTRIM(o.Name) AS Name, 
  o.OrganizationID
  FROM Organizations o
  JOIN CustomerRelationships cr
	ON o.OrganizationID = cr.CustomerID
  WHERE cr.RelatedCustomerID = @OrganizationID
";

            User user = Users.GetUser(loginUser, loginUser.UserID);
            if (user.TicketRights == TicketRightType.Customers)
            {
                companyQuery = companyQuery + " AND o.OrganizationID IN (SELECT OrganizationID FROM UserRightsOrganizations WHERE UserID = " + user.UserID.ToString() + ")";
            }

            //if (active != null)
            //{
            //    companyQuery = companyQuery + " AND o.IsActive = @IsActive";
            //    command.Parameters.AddWithValue("@IsActive", (bool)active);
            //}

            //if (searchContacts && searchCompanies)
            //{
            //    command.CommandText = string.Format(pageQuery, companyQuery + " UNION ALL " + contactQuery);
            //}
            //else if (searchCompanies)
            //{
            command.CommandText = string.Format(pageQuery, companyQuery);
            //}
            //else if (searchContacts)
            //{
            //    command.CommandText = string.Format(pageQuery, contactQuery);
            //}
            //else
            //{
            //    return results.ToArray();
            //}


            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            //command.Parameters.AddWithValue("@From", from + 1);
            //command.Parameters.AddWithValue("@To", from + count);

            DataTable table = SqlExecutor.ExecuteQuery(loginUser, command);

            foreach (DataRow row in table.Rows)
            {
                //if (row["UserID"] == DBNull.Value)
                //{
                CustomerSearchCompany company = new CustomerSearchCompany();
                company.name = (string)row["Organization"];
                company.organizationID = (int)row["OrganizationID"];
                company.isPortal = (bool)row["HasPortalAccess"];
                company.openTicketCount = (int)row["OrgOpenTickets"];
                company.website = SearchService.GetDBString(row["Website"]);

                List<CustomerSearchPhone> phones = new List<CustomerSearchPhone>();
                PhoneNumbers phoneNumbers = new PhoneNumbers(loginUser);
                //phoneNumbers.LoadByID(company.organizationID, ReferenceType.Organizations);
                foreach (PhoneNumber number in phoneNumbers)
                {
                    phones.Add(new CustomerSearchPhone(number));
                }
                company.phones = phones.ToArray();

                results.Add(JsonConvert.SerializeObject(company));
                //}
                //else
                //{
                //CustomerSearchContact contact = new CustomerSearchContact();
                //contact.organizationID = (int)row["OrganizationID"];
                //contact.isPortal = (bool)row["IsPortalUser"];
                //contact.openTicketCount = (int)row["ContactOpenTickets"];

                //contact.userID = (int)row["UserID"];
                //contact.fName = SearchService.GetDBString(row["FirstName"]);
                //contact.lName = SearchService.GetDBString(row["LastName"]);
                //contact.email = SearchService.GetDBString(row["Email"]);
                //contact.title = SearchService.GetDBString(row["Title"]);
                //contact.organization = SearchService.GetDBString(row["Organization"]);

                //List<CustomerSearchPhone> phones = new List<CustomerSearchPhone>();
                //PhoneNumbers phoneNumbers = new PhoneNumbers(loginUser);
                ////phoneNumbers.LoadByID(contact.userID, ReferenceType.Contacts);
                //foreach (PhoneNumber number in phoneNumbers)
                //{
                //    phones.Add(new CustomerSearchPhone(number));
                //}
                //contact.phones = phones.ToArray();
                //results.Add(JsonConvert.SerializeObject(contact));
                //}

            }

            return results.ToArray();
        }

		[WebMethod]
		public bool HasChildren(int organizationId)
		{
			LoginUser loginUser = TSAuthentication.GetLoginUser();
			SqlCommand command = new SqlCommand();
			command.CommandType = CommandType.Text;
			command.CommandText = @"SELECT COUNT(1) FROM CustomerRelationships WITH(NOLOCK) WHERE RelatedCustomerID = @organizationId";
			command.Parameters.AddWithValue("@OrganizationId", organizationId);
			int childrenCount = SqlExecutor.ExecuteInt(loginUser, command);

			return childrenCount > 0;
		}

        [WebMethod]
        public string LoadContacts(int organizationID, bool isActive)
        {
            StringBuilder htmlresults = new StringBuilder("");
            StringBuilder phoneResults = new StringBuilder("");
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByOrganizationIDLastName(organizationID, isActive);

            foreach (User u in users)
            {

                PhoneNumbers phoneNumbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
                phoneNumbers.LoadByID(u.UserID, ReferenceType.Users);

                foreach (PhoneNumber p in phoneNumbers)
                {
                    phoneResults.AppendFormat("<p class='list-group-item-text'><span class=\"text-muted\">{0}</span>: <a href=\"tel:{1}\" target=\"_blank\">{1}</a> {2}</p>", p.PhoneTypeName, p.Number, string.IsNullOrWhiteSpace(p.Extension) ? "" : "Ext:" + p.Extension);
                }

                var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                htmlresults.AppendFormat(@"<div class='list-group-item'>
                            <div class='row'>
                            <div class='col-xs-1'>
                            <img class='user-avatar' src='/dc/{10}/UserAvatar/{7}/40/{11}'>
                            </div>
                            <div class='col-xs-11'>
                            <span class='pull-right {0}'>{1}</span><a href='#' id='{7}' class='contactlink'><h4 class='list-group-item-heading'>{2}</h4></a>
                            <div class='row'>
                                <div class='col-xs-6'>
                                    <p class='list-group-item-text'>{3}</p>
                                    {9}
                                    {6}
                                    {8}
                                </div>
                                <div class='col-xs-6'>
                                    <p class='list-group-item-text'>{4} Open Tickets</p>
                                    <p class='list-group-item-text'>{5} Closed Tickets</p>                            
                                </div>
                            </div>
                            </div>
                            </div>
                            </div>"

                    , u.IsActive ? "user-active" : "user-inactive", u.IsActive ? "Active" : "Inactive", u.FirstLastName, u.Email != "" ? "<a href='mailto:" + u.Email + "'>" + u.Email + "</a>" : "Empty", GetContactTickets(u.UserID, 0), GetContactTickets(u.UserID, 1), phoneResults, u.UserID, u.IsPortalUser == true ? "<p class='list-group-item-text'><span class=\"text-muted\">Has Portal Access</span>" : "", u.Title != "" ? u.Title : "", u.OrganizationID, (Int64)ts.TotalMilliseconds);

                phoneResults.Clear();
            }

            return htmlresults.ToString();
        }

        [WebMethod]
        public string LoadContacts2(int organizationID, bool isActive, bool includeChildren, int start)
        {
            StringBuilder htmlresults = new StringBuilder("");
            StringBuilder phoneResults = new StringBuilder("");
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Users users = new Users(loginUser);
            users.LoadPagedByOrganizationIDLastName(organizationID, isActive, includeChildren, start);

            foreach (User u in users)
            {

                PhoneNumbers phoneNumbers = new PhoneNumbers(loginUser);
                phoneNumbers.LoadByID(u.UserID, ReferenceType.Users);

                foreach (PhoneNumber p in phoneNumbers)
                {
                    phoneResults.AppendFormat("<p class='list-group-item-text'><span class=\"text-muted\">{0}</span>: <a href=\"tel:{1}\" target=\"_blank\">{1}</a> {2}</p>", p.PhoneTypeName, p.Number, string.IsNullOrWhiteSpace(p.Extension) ? "" : "Ext:" + p.Extension);
                }

                var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                string name;
                if (includeChildren && (organizationID != u.OrganizationID))
                {
                    Organization company = Organizations.GetOrganization(loginUser, u.OrganizationID);
                    name = company.Name + " - " + u.FirstLastName;
                }
                else
                {
                    name = u.FirstLastName;
                }

                htmlresults.AppendFormat(@"<div class='list-group-item'>
                            <div class='row'>
                            <div class='col-xs-1'>
                            <img class='user-avatar' src='/dc/{10}/UserAvatar/{7}/40/{11}'>
                            </div>
                            <div class='col-xs-11'>
                            <span class='pull-right {0}'>{1}</span><a href='#' id='{7}' class='contactlink'><h4 class='list-group-item-heading'>{2}</h4></a>
                            <div class='row'>
                                <div class='col-xs-6'>
                                    <p class='list-group-item-text'>{3}</p>
                                    {9}
                                    {6}
                                    {8}
                                </div>
                                <div class='col-xs-6'>
                                    <p class='list-group-item-text'>{4} Open Tickets</p>
                                    <p class='list-group-item-text'>{5} Closed Tickets</p>                            
                                </div>
                            </div>
                            </div>
                            </div>
                            </div>"

                    , u.IsActive ? "user-active" : "user-inactive", u.IsActive ? "Active" : "Inactive", name, u.Email != "" ? "<a href='mailto:" + u.Email + "'>" + u.Email + "</a>" : "Empty", GetContactTickets(u.UserID, 0), GetContactTickets(u.UserID, 1), phoneResults, u.UserID, u.IsPortalUser == true ? "<p class='list-group-item-text'><span class=\"text-muted\">Has Portal Access</span>" : "", u.Title != "" ? u.Title : "", u.OrganizationID, (Int64)ts.TotalMilliseconds);

                phoneResults.Clear();
            }

            return htmlresults.ToString();
        }

        [WebMethod]
        public int SaveCustomer(string data)
        {
            NewCustomerSave info;
            try
            {
                info = Newtonsoft.Json.JsonConvert.DeserializeObject<NewCustomerSave>(data);
            }
            catch (Exception e)
            {
                return -1;
            }

            Organization organization;
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());

            organization = organizations.AddNewOrganization();
            organization.ParentID = TSAuthentication.GetLoginUser().OrganizationID;
            organization.PrimaryUserID = null;

            organization.ExtraStorageUnits = 0;
            organization.PortalSeats = 0;
            organization.UserSeats = 0;
            organization.IsCustomerFree = false;
            organization.ProductType = ProductType.Express;
            organization.HasPortalAccess = false;
            organization.IsActive = true;
            organization.IsBasicPortal = true;

            organization.DefaultSupportUserID = info.DefaultSupportUserID < 0 ? null : info.DefaultSupportUserID;
            organization.DefaultPortalGroupID = null;
            organization.DefaultSupportGroupID = info.DefaultSupportGroupID < 0 ? null : info.DefaultSupportGroupID;
            organization.TimeZoneID = info.TimeZoneID;
            organization.Name = info.Name;
            organization.Website = info.Website;
            organization.CompanyDomains = info.CompanyDomains;

            organization.Description = info.Description;
            organization.SAExpirationDate = DataUtils.DateToUtc(TSAuthentication.GetLoginUser(), info.SAExpirationDate);

            organization.SlaLevelID = info.SlaLevelID < 0 ? null : info.SlaLevelID;

            int shm = 0;
            int.TryParse(info.SupportHoursMonth.ToString(), out shm);

            organization.SupportHoursMonth = shm;

            if (TSAuthentication.IsSystemAdmin)
            {
                organization.HasPortalAccess = info.PortalAccess;
                organization.IsActive = info.Active;
                organization.IsApiActive = info.APIEnabled;
                organization.IsApiEnabled = info.APIEnabled;
                organization.InActiveReason = info.InactiveReason;
            }
            organization.Collection.Save();

            string description = String.Format("{0} created {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, organization.Name);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Insert, ReferenceType.Organizations, organization.OrganizationID, description);

            foreach (CustomFieldSaveInfo field in info.Fields)
            {
                CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organization.OrganizationID);
                if (field.Value == null)
                {
                    customValue.Value = "";
                }
                else
                {
                    if (customValue.FieldType == CustomFieldType.DateTime)
                    {
                        customValue.Value = ((DateTime)field.Value).ToString();
                        //DateTime dt;
                        //if (DateTime.TryParse(((string)field.Value), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
                        //{
                        //    customValue.Value = dt.ToUniversalTime().ToString();
                        //}
                    }
                    else
                    {
                        customValue.Value = field.Value.ToString();
                    }

                }

                customValue.Collection.Save();
            }

            return organization.OrganizationID;

        }

        [WebMethod]
        public int SaveContact(string data)
        {
            User user;
            Users users = new Users(TSAuthentication.GetLoginUser());
            NewContactSave info;
            int newOrgID;

            info = Newtonsoft.Json.JsonConvert.DeserializeObject<NewContactSave>(data);

            if (info.Company != null)
                newOrgID = int.Parse(info.Company);
            else
                newOrgID = Organizations.GetUnknownCompanyID(TSAuthentication.GetLoginUser());

            if (info.Email != "" && !users.IsEmailValid(info.Email, -1, newOrgID))
            {
                return -1;
            }

            user = users.AddNewUser();
            user.OrganizationID = newOrgID;
            if (info.Active) user.ActivatedOn = DateTime.UtcNow;
            user.LastLogin = DateTime.UtcNow;
            user.LastActivity = DateTime.UtcNow.AddHours(-1);
            user.IsPasswordExpired = true;
            user.ReceiveTicketNotifications = true;
            user.NeedsIndexing = true;

            user.Email = info.Email;
            user.FirstName = info.FirstName;
            user.LastName = info.LastName;
            user.Title = info.Title;
            user.MiddleName = info.MiddleName;
            user.BlockInboundEmail = info.BlockInboundEmail;
            user.BlockEmailFromCreatingOnly = info.BlockEmailFromCreatingOnly;
            user.IsActive = info.Active;

            if (TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).HasPortalAccess && TSAuthentication.IsSystemAdmin)
                user.IsPortalUser = info.IsPortalUser;

            if (TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).ParentID == null && TSAuthentication.IsSystemAdmin)
            {
                user.IsSystemAdmin = info.IsSystemAdmin;
                user.IsFinanceAdmin = info.IsFinanceAdmin;
            }
            user.Collection.Save();

            if (info.SyncAddress)
            {
                Addresses orgAddresses = new Addresses(TSAuthentication.GetLoginUser());
                orgAddresses.LoadByID(user.OrganizationID, ReferenceType.Organizations);
                Addresses newAddresses = new Addresses(TSAuthentication.GetLoginUser());


                foreach (Address add in orgAddresses)
                {
                    Address address = newAddresses.AddNewAddress();
                    address.RefID = user.UserID;
                    address.RefType = ReferenceType.Users;
                    address.Addr1 = add.Addr1;
                    address.Addr2 = add.Addr2;
                    address.Addr3 = add.Addr3;
                    address.City = add.City;
                    address.Country = add.Country;
                    address.Description = add.Description;
                    address.State = add.State;
                    address.Zip = add.Zip;
                }
                newAddresses.Save();

            }


            if (info.SyncPhone)
            {
                PhoneNumbers oldPhoneNumbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
                PhoneNumbers newPhoneNumbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
                oldPhoneNumbers.LoadByID(user.OrganizationID, ReferenceType.Organizations);

                foreach (PhoneNumber phone in oldPhoneNumbers)
                {
                    PhoneNumber phoneNumber = newPhoneNumbers.AddNewPhoneNumber();
                    phoneNumber.RefID = user.UserID;
                    phoneNumber.RefType = ReferenceType.Users;
                    phoneNumber.Extension = phone.Extension;
                    phoneNumber.Number = phone.Number;
                    phoneNumber.PhoneTypeID = phone.PhoneTypeID;
                }

                newPhoneNumbers.Save();
            }


            foreach (CustomFieldSaveInfo field in info.Fields)
            {
                CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, user.UserID);
                if (field.Value == null)
                {
                    customValue.Value = "";
                }
                else
                {
                    if (customValue.FieldType == CustomFieldType.DateTime)
                    {
                        customValue.Value = ((DateTime)field.Value).ToString();
                        //DateTime dt;
                        //if (DateTime.TryParse(((string)field.Value), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
                        //{
                        //    customValue.Value = dt.ToUniversalTime().ToString();
                        //}
                    }
                    else
                    {
                        customValue.Value = field.Value.ToString();
                    }

                }

                customValue.Collection.Save();
            }

            string password = DataUtils.GenerateRandomPassword();
            user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
            user.IsPasswordExpired = true;
            user.Collection.Save();
            if (TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).ParentID == null && info.EmailPortalPW)
            {
                EmailPosts.SendWelcomeTSUser(TSAuthentication.GetLoginUser(), user.UserID, password);
            }
            if (info.EmailPortalPW && info.IsPortalUser)
                EmailPosts.SendWelcomePortalUser(TSAuthentication.GetLoginUser(), user.UserID, password);

            if (info.EmailHubPW && info.IsPortalUser)
                EmailPosts.SendWelcomeCustomerHubUser(TSAuthentication.GetLoginUser(), user.UserID, password);


            string description = String.Format("{0} created contact {1}", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, user.FirstLastName);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Insert, ReferenceType.Users, user.UserID, description);

            return user.UserID;
        }

        [WebMethod]
        public void SaveEmail(string data, int refID, ReferenceType refType)
        {
            EmailSave info;
            info = Newtonsoft.Json.JsonConvert.DeserializeObject<EmailSave>(data);
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            User user = TSAuthentication.GetUser(loginUser);
            EmailAddresses emails = new EmailAddresses(loginUser);

            if (info.EmailID != -1)
            {
                emails.LoadById((int)info.EmailID);
                if (!emails.IsEmpty)
                {
                    EmailAddress email = emails[0];
                    string description = String.Format("{0} modified email from {1} to {2} ", user.FirstLastName, email.Email, info.Email);
                    ActionLogs.AddActionLog(loginUser, ActionLogType.Update, refType, refID, description);
                    email.Email = info.Email;
                    email.ModifierID = loginUser.UserID;
                }

            }
            else
            {
                EmailAddress email = emails.AddNewEmailAddress();
                email.RefID = refID;
                email.RefType = (int)refType;
                email.Email = info.Email;
                email.CreatorID = loginUser.UserID;
                email.ModifierID = loginUser.UserID;
                string description = String.Format("{0} added email {1} ", user.FirstLastName, info.Email);
                ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Insert, refType, refID, description);
            }
            emails.Save();
        }

        [WebMethod]
        public void SavePhoneNumber(string data, int refID, ReferenceType refType)
        {
            PhoneSave info;
            info = Newtonsoft.Json.JsonConvert.DeserializeObject<PhoneSave>(data);

            PhoneNumbers phoneNumbers = new PhoneNumbers(TSAuthentication.GetLoginUser());

            if (info.PhoneID != -1)
            {
                phoneNumbers.LoadByPhoneID((int)info.PhoneID);
                if (!phoneNumbers.IsEmpty)
                {
                    PhoneNumber phoneNumber = phoneNumbers[0];
                    string description = String.Format("{0} modified phone number from {1} to {2} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, phoneNumber.Number, info.Number);
                    ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, refType, refID, description);
                    phoneNumber.Extension = info.Extension;
                    phoneNumber.Number = info.Number;
                    phoneNumber.PhoneTypeID = info.PhoneTypeID;
                }

            }
            else
            {
                PhoneNumber phoneNumber = phoneNumbers.AddNewPhoneNumber();
                phoneNumber.RefID = refID;
                phoneNumber.RefType = refType;
                phoneNumber.Extension = info.Extension;
                phoneNumber.Number = info.Number;
                phoneNumber.PhoneTypeID = info.PhoneTypeID;
                string description = String.Format("{0} added phone number {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, info.Number);
                ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Insert, refType, refID, description);
            }
            phoneNumbers.Save();
        }

        [WebMethod]
        public void SaveAddress(string data, int refID, ReferenceType refType)
        {
            AddressSave info;
            info = Newtonsoft.Json.JsonConvert.DeserializeObject<AddressSave>(data);

            Addresses addresses = new Addresses(TSAuthentication.GetLoginUser());

            if (info.addressID != -1)
            {
                addresses.LoadByAddressID(info.addressID);
                if (!addresses.IsEmpty)
                {
                    Address address = addresses[0];
                    address.Addr1 = info.Addr1;
                    address.Addr2 = info.Addr2;
                    address.Addr3 = info.Addr3;
                    address.City = info.City;
                    address.Country = info.Country;
                    address.Description = info.Description;
                    address.State = info.State;
                    address.Zip = info.Zip;
                    string description = String.Format("{0} modified address {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, info.Description);
                    ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, refType, refID, description);
                }
            }
            else
            {
                Address address = addresses.AddNewAddress();
                address.RefID = refID;
                address.RefType = refType;
                address.Addr1 = info.Addr1;
                address.Addr2 = info.Addr2;
                address.Addr3 = info.Addr3;
                address.City = info.City;
                address.Country = info.Country;
                address.Description = info.Description;
                address.State = info.State;
                address.Zip = info.Zip;
                string description = String.Format("{0} added address {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, info.Description);
                ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Insert, refType, refID, description);
            }
            addresses.Save();
        }

        [WebMethod]
        public int SaveNote(string title, string noteText, int noteID, int refID, ReferenceType refType, int ActivityType, string DateOccurred, bool isAlert = false, int productFamilyID = -1)
        {
            Note note = null;
            bool isNew = false;
            if (noteID > -1)
            {
                note = (Note)Notes.GetNote(TSAuthentication.GetLoginUser(), noteID);
                string description = String.Format("{0} modified note {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, title);
                ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, (ReferenceType)refType, noteID, description);

                UserNoteSettings uns = new UserNoteSettings(TSAuthentication.GetLoginUser());
                uns.LoadByIDType(noteID, refType);

                foreach (UserNoteSetting u in uns)
                {
                    u.Delete();
                }
                uns.Save();
            }
            else
            {
                note = (new Notes(TSAuthentication.GetLoginUser())).AddNewNote();
                note.RefID = refID;
                note.RefType = refType;
                isNew = true;



            }

            if (note != null)
            {
                note.Description = noteText;
                note.Title = title;
                note.IsAlert = isAlert;
                note.ActivityType = ActivityType;
                if(!string.IsNullOrEmpty(DateOccurred))
                    note.DateOccurred = DateTime.Parse(DateOccurred);
                if (productFamilyID != -1)
                {
                    note.ProductFamilyID = productFamilyID;
                }
                else
                {
                    note.ProductFamilyID = null;
                }
                note.Collection.Save();
                if (isNew)
                {
                    string description = String.Format("{0} added note {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, title);
                    ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Insert, (ReferenceType)refType, note.NoteID, description);
                }

            }
            return note.NoteID;
        }

        [WebMethod]
        public void SaveProduct(string data)
        {
            NewProductSave info = null;
            OrganizationProduct organizationProduct = null;

            try
            {
                info = Newtonsoft.Json.JsonConvert.DeserializeObject<NewProductSave>(data);
            }
            catch (Exception e)
            {
                string a = e.Message;
            }
            if (info.OrganizationProductID < 0)
            {
                organizationProduct = (new OrganizationProducts(TSAuthentication.GetLoginUser())).AddNewOrganizationProduct();
                Product p = Products.GetProduct(TSAuthentication.GetLoginUser(), int.Parse(info.ProductID));
                string description = String.Format("{0} added product association to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, p.Name);
                ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, int.Parse(info.OrganizationID), description);
            }
            else
            {
                organizationProduct = (OrganizationProduct)OrganizationProducts.GetOrganizationProduct(TSAuthentication.GetLoginUser(), info.OrganizationProductID);
                Product p = Products.GetProduct(TSAuthentication.GetLoginUser(), int.Parse(info.ProductID));
                string description = String.Format("{0} modified product association to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, p.Name);
                ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Insert, ReferenceType.Organizations, int.Parse(info.OrganizationID), description);
            }

            organizationProduct.OrganizationID = int.Parse(info.OrganizationID);
            organizationProduct.ProductID = int.Parse(info.ProductID);
            if (info.Version > 0)
                organizationProduct.ProductVersionID = info.Version;
            else
            {
                organizationProduct.ProductVersionID = null;
            }

            if (info.SupportExpiration != null && info.SupportExpiration != "")
                organizationProduct.SupportExpiration = DataUtils.DateToUtc(TSAuthentication.GetLoginUser(), DateTime.ParseExact(info.SupportExpiration, GetDateFormatNormal(), null));
            else
                organizationProduct.SupportExpiration = null;

            organizationProduct.SlaLevelID = info.SlaLevelID > 0 ? info.SlaLevelID : (int?)null;

            organizationProduct.Collection.Save();

            foreach (CustomFieldSaveInfo field in info.Fields)
            {
                CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationProduct.OrganizationProductID);
                if (field.Value == null)
                {
                    customValue.Value = "";
                }
                else
                {
                    if (customValue.FieldType == CustomFieldType.DateTime)
                    {
                        //customValue.Value = ((DateTime)field.Value).ToString();
                        DateTime dt;
                        if (DateTime.TryParse(((string)field.Value).Replace("UTC", "GMT"), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
                        {
                            customValue.Value = dt.ToUniversalTime().ToString();
                        }
                    }
                    else
                    {
                        customValue.Value = field.Value.ToString();
                    }

                }
                customValue.Collection.Save();

            }


        }

        [WebMethod]
        public void SaveContactProduct(string data)
        {
            NewProductSave info = null;
            UserProduct userProduct = null;

            try
            {
                info = Newtonsoft.Json.JsonConvert.DeserializeObject<NewProductSave>(data);
            }
            catch (Exception e)
            {
                string a = e.Message;
            }

            LoginUser loginUser = TSAuthentication.GetLoginUser();
            if (info.UserProductID < 0)
            {
                userProduct = (new UserProducts(loginUser)).AddNewUserProduct();
                Product p = Products.GetProduct(loginUser, int.Parse(info.ProductID));
                string description = String.Format("{0} added product association to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, p.Name);
                ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Contacts, int.Parse(info.UserID), description);
            }
            else
            {
                userProduct = (UserProduct)UserProducts.GetUserProduct(loginUser, info.UserProductID);
                Product p = Products.GetProduct(loginUser, int.Parse(info.ProductID));
                string description = String.Format("{0} modified product association to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, p.Name);
                ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Contacts, int.Parse(info.UserID), description);
            }

            userProduct.UserID = int.Parse(info.UserID);
            userProduct.ProductID = int.Parse(info.ProductID);
            if (info.Version > 0)
                userProduct.ProductVersionID = info.Version;
            else
            {
                userProduct.ProductVersionID = null;
            }

            if (info.SupportExpiration != null && info.SupportExpiration != "")
                userProduct.SupportExpiration = DataUtils.DateToUtc(loginUser, DateTime.ParseExact(info.SupportExpiration, GetDateFormatNormal(), null));
            else
                userProduct.SupportExpiration = null;

            userProduct.Collection.Save();

            foreach (CustomFieldSaveInfo field in info.Fields)
            {
                CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, userProduct.UserProductID);
                if (field.Value == null)
                {
                    customValue.Value = "";
                }
                else
                {
                    if (customValue.FieldType == CustomFieldType.DateTime)
                    {
                        //customValue.Value = ((DateTime)field.Value).ToString();
                        DateTime dt;
                        if (DateTime.TryParse(((string)field.Value).Replace("UTC", "GMT"), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
                        {
                            customValue.Value = dt.ToUniversalTime().ToString();
                        }
                    }
                    else
                    {
                        customValue.Value = field.Value.ToString();
                    }

                }
                customValue.Collection.Save();

            }


            OrganizationProducts organizationProducts = new OrganizationProducts(loginUser);
            organizationProducts.LoadByContactProductAndVersionID(userProduct.UserID, userProduct.ProductID, userProduct.ProductVersionID);
            if (organizationProducts.Count < 1)
            {
                OrganizationProduct organizationProduct = (new OrganizationProducts(loginUser)).AddNewOrganizationProduct();
                Product p = Products.GetProduct(loginUser, userProduct.ProductID);
                string description = String.Format("{0} added product association by contact to {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, p.Name);
                User user = Users.GetUser(loginUser, userProduct.UserID);
                ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Organizations, user.OrganizationID, description);
                organizationProduct.OrganizationID = user.OrganizationID;
                organizationProduct.ProductID = userProduct.ProductID;
                if (info.Version > 0)
                    organizationProduct.ProductVersionID = info.Version;
                else
                {
                    organizationProduct.ProductVersionID = null;
                }

                if (info.SupportExpiration != null && info.SupportExpiration != "")
                    organizationProduct.SupportExpiration = DataUtils.DateToUtc(loginUser, DateTime.ParseExact(info.SupportExpiration, GetDateFormatNormal(), null));
                else
                    organizationProduct.SupportExpiration = null;

                organizationProduct.Collection.Save();
            }
        }

        [WebMethod]
        public void AssignAllCustomersToVersion(int productVersionID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            ProductVersionsView productVersionsView = new ProductVersionsView(loginUser);
            productVersionsView.LoadByProductVersionID(productVersionID);

            OrganizationProducts existingOrganizationProducts = new OrganizationProducts(loginUser);
            existingOrganizationProducts.LoadByParentOrganizationIDAndVersionID(loginUser.OrganizationID, productVersionID);

            Organizations allCustomers = new Organizations(loginUser);
            allCustomers.LoadByParentID(loginUser.OrganizationID, true);

            foreach (Organization customer in allCustomers)
            {
                OrganizationProduct existingOrganizationProduct = existingOrganizationProducts.FindByOrganizationID(customer.OrganizationID);

                if (existingOrganizationProduct == null)
                {
                    OrganizationProduct organizationProduct = (new OrganizationProducts(loginUser)).AddNewOrganizationProduct();
                    organizationProduct.OrganizationID = customer.OrganizationID;
                    organizationProduct.ProductID = productVersionsView[0].ProductID;
                    organizationProduct.ProductVersionID = productVersionID;
                    organizationProduct.Collection.Save();

                    string description = String.Format("{0} added product version association to version number {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, productVersionsView[0].VersionNumber);
                    ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Organizations, customer.OrganizationID, description);
                }
            }
        }

        [WebMethod]
        public void UnassignAllCustomersFromVersion(int productVersionID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            ProductVersionsView productVersionsView = new ProductVersionsView(loginUser);
            productVersionsView.LoadByProductVersionID(productVersionID);
            if (productVersionsView.Count > 0 && productVersionsView[0].OrganizationID == loginUser.OrganizationID)
            {
                OrganizationProducts existingOrganizationProducts = new OrganizationProducts(loginUser);
                existingOrganizationProducts.LoadByProductVersionID(loginUser.OrganizationID);

                OrganizationProducts.DeleteAllOrganizationsByProductVersionID(loginUser, productVersionID);

                foreach (OrganizationProduct organizationProduct in existingOrganizationProducts)
                {
                    string description = String.Format("{0} deleted product version association to version number {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, productVersionsView[0].VersionNumber);
                    ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Organizations, organizationProduct.OrganizationID, description);
                }
            }
        }

        [WebMethod]
        public void AssignAllCustomersToProduct(int productID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            Products products = new Products(loginUser);
            products.LoadByProductID(productID);

            OrganizationProducts existingOrganizationProducts = new OrganizationProducts(loginUser);
            existingOrganizationProducts.LoadByParentOrganizationIDAndProductID(loginUser.OrganizationID, productID);

            Organizations allCustomers = new Organizations(loginUser);
            allCustomers.LoadByParentID(loginUser.OrganizationID, true);

            foreach (Organization customer in allCustomers)
            {
                OrganizationProduct existingOrganizationProduct = existingOrganizationProducts.FindByOrganizationID(customer.OrganizationID);

                if (existingOrganizationProduct == null)
                {
                    OrganizationProduct organizationProduct = (new OrganizationProducts(loginUser)).AddNewOrganizationProduct();
                    organizationProduct.OrganizationID = customer.OrganizationID;
                    organizationProduct.ProductID = products[0].ProductID;
                    organizationProduct.Collection.Save();

                    string description = String.Format("{0} added customer association to product {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, products[0].Name);
                    ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Organizations, customer.OrganizationID, description);
                }
            }
        }

        [WebMethod]
        public void UnassignAllCustomersFromProduct(int productID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            Products products = new Products(loginUser);
            products.LoadByProductID(productID);
            if (products.Count > 0 && products[0].OrganizationID == loginUser.OrganizationID)
            {
                OrganizationProducts existingOrganizationProducts = new OrganizationProducts(loginUser);
                existingOrganizationProducts.LoadByProductVersionID(loginUser.OrganizationID);

                OrganizationProducts.DeleteAllOrganizationsByProductID(loginUser, productID);

                foreach (OrganizationProduct organizationProduct in existingOrganizationProducts)
                {
                    string description = String.Format("{0} deleted customer association to product {1} ", TSAuthentication.GetUser(loginUser).FirstLastName, products[0].Name);
                    ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Organizations, organizationProduct.OrganizationID, description);
                }
            }
        }

        [WebMethod]
        public int LoadCDI(int organizationID)
        {
            Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), organizationID);

            return organization.CustDisIndex;
        }

        [WebMethod]
        public int LoadCDI2(int organizationID, bool includeChildren)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            if (includeChildren)
            {
                return Organizations.GetFamilyAverageCDI(loginUser, organizationID);
            }
            else
            {
                Organization organization = Organizations.GetOrganization(loginUser, organizationID);
                return organization.CustDisIndex;
            }
        }

        [WebMethod]
        public string LoadChartData(int organizationID, bool open)
        {

            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

            TicketTypes ticketTypes = new TicketTypes(TSAuthentication.GetLoginUser());
            ticketTypes.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID, organizations[0].ProductType);

            int total = 0;
            StringBuilder chartString = new StringBuilder("");

            foreach (TicketType ticketType in ticketTypes)
            {
                int count;
                if (open)
                    count = Tickets.GetOrganizationOpenTicketCount(TSAuthentication.GetLoginUser(), organizationID, ticketType.TicketTypeID);
                else
                    count = Tickets.GetOrganizationClosedTicketCount(TSAuthentication.GetLoginUser(), organizationID, ticketType.TicketTypeID);
                total += count;

                if (count > 0)
                    chartString.AppendFormat("{0},{1},", ticketType.Name.Replace(",", ""), count.ToString().Replace(",", ""));
                //chartString.AppendFormat("['{0}',{1}],",ticketType.Name, count.ToString());
            }
            if (chartString.ToString().EndsWith(","))
            {
                chartString.Remove(chartString.Length - 1, 1);
            }

            return chartString.ToString();
        }

        [WebMethod]
        public string LoadChartData2(int organizationID, bool open, bool includeChildren)
        {

            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

            TicketTypes ticketTypes = new TicketTypes(TSAuthentication.GetLoginUser());
            ticketTypes.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID, organizations[0].ProductType);

            int total = 0;
            StringBuilder chartString = new StringBuilder("");

            foreach (TicketType ticketType in ticketTypes)
            {
                int count;
                if (open)
                    count = Tickets.GetOrganizationOpenTicketCount(TSAuthentication.GetLoginUser(), organizationID, ticketType.TicketTypeID, includeChildren);
                else
                    count = Tickets.GetOrganizationClosedTicketCount(TSAuthentication.GetLoginUser(), organizationID, ticketType.TicketTypeID, includeChildren);
                total += count;

                if (count > 0)
                    chartString.AppendFormat("{0},{1},", ticketType.Name.Replace(",", ""), count.ToString().Replace(",", ""));
                //chartString.AppendFormat("['{0}',{1}],",ticketType.Name, count.ToString());
            }
            if (chartString.ToString().EndsWith(","))
            {
                chartString.Remove(chartString.Length - 1, 1);
            }

            return chartString.ToString();
        }

        [WebMethod]
        public string LoadContactChartData(int userID, bool open)
        {

            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

            TicketTypes ticketTypes = new TicketTypes(TSAuthentication.GetLoginUser());
            ticketTypes.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID, organizations[0].ProductType);

            int total = 0;
            StringBuilder chartString = new StringBuilder("");

            foreach (TicketType ticketType in ticketTypes)
            {
                int count;
                if (open)
                    count = Tickets.GetContactOpenTicketCount(TSAuthentication.GetLoginUser(), userID, ticketType.TicketTypeID);
                else
                    count = Tickets.GetContactClosedTicketCount(TSAuthentication.GetLoginUser(), userID, ticketType.TicketTypeID);
                total += count;

                if (count > 0)
                    chartString.AppendFormat("{0},{1},", ticketType.Name.Replace(",", ""), count.ToString().Replace(",", ""));
                //chartString.AppendFormat("['{0}',{1}],",ticketType.Name, count.ToString());
            }
            if (chartString.ToString().EndsWith(","))
            {
                chartString.Remove(chartString.Length - 1, 1);
            }

            return chartString.ToString();
        }

        [WebMethod]
        public string GetDateFormat(bool lower = false)
        {
            CultureInfo us = new CultureInfo(TSAuthentication.GetLoginUser().CultureInfo.ToString());
            if (lower)
                return us.DateTimeFormat.ShortDatePattern.ToLower();
            else
                return us.DateTimeFormat.ShortDatePattern.ToUpper();
        }

        public string GetDateFormatNormal()
        {
            CultureInfo us = new CultureInfo(TSAuthentication.GetLoginUser().CultureInfo.ToString());
            return us.DateTimeFormat.ShortDatePattern;
        }

        [WebMethod]
        public string SendWelcome(int userID)
        {
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByUserID(userID);
            EmailPosts.SendWelcomeNewSignup(TSAuthentication.GetLoginUser(), userID, "");
            return ("A welcome message has been sent to " + users[0].FirstName + " " + users[0].LastName);
        }

        [WebMethod]
        public void DeleteOrgzanitionLinks(int organizationID)
        {
            //RecentlyViewedItems recent = new RecentlyViewedItems(TSAuthentication.GetLoginUser());
            //recent.DeleteRecentOrg(organizationID);



            int unknownID = Organizations.GetUnknownCompanyID(TSAuthentication.GetLoginUser());
            Users u = new Users(TSAuthentication.GetLoginUser());
            u.UpdateDeletedOrg(organizationID, unknownID);

            return;
        }

        [WebMethod]
        public string PasswordReset(int userID)
        {
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByUserID(userID);
            if (!users.IsEmpty)
            {
                if (users[0].CryptedPassword == "")
                {
                    string password = DataUtils.GenerateRandomPassword();
                    users[0].CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
                    users[0].IsPasswordExpired = true;
                    users[0].Collection.Save();
                    if (TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).ParentID == null)
                        EmailPosts.SendWelcomeTSUser(TSAuthentication.GetLoginUser(), users[0].UserID, password);
                    else
                        EmailPosts.SendWelcomePortalUser(TSAuthentication.GetLoginUser(), users[0].UserID, password);

                    return ("A new password has been sent to " + users[0].FirstName + " " + users[0].LastName);

                }
                else
                {
                    users[0].IsPasswordExpired = true;
                    users[0].Collection.Save();
                    if (DataUtils.ResetPassword(TSAuthentication.GetLoginUser(), users[0], !(TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).ParentID == null)))
                    {
                        return ("A new password has been sent to " + users[0].FirstName + " " + users[0].LastName);
                    }
                }
            }
            return ("There was an error sending the password.");
        }

        [WebMethod]
        public string ChubPasswordReset(int userID, int? productFamilyID = null)
        {
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByUserID(userID);
            if (!users.IsEmpty)
            {
                if (users[0].CryptedPassword == "")
                {
                    string password = DataUtils.GenerateRandomPassword();
                    users[0].CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
                    users[0].IsPasswordExpired = true;
                    users[0].Collection.Save();
                    if (TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).ParentID == null)
                        EmailPosts.SendWelcomeTSUser(TSAuthentication.GetLoginUser(), users[0].UserID, password);
                    else
                    {
                        EmailPosts.SendWelcomeCustomerHubUser(TSAuthentication.GetLoginUser(), users[0].UserID, password, productFamilyID);
                    }
                    return ("A new password has been sent to " + users[0].FirstName + " " + users[0].LastName);

                }
                else
                {
                    users[0].IsPasswordExpired = true;
                    users[0].Collection.Save();
                    if (DataUtils.ResetPassword(TSAuthentication.GetLoginUser(), users[0], !(TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).ParentID == null), true, productFamilyID))
                    {
                        return ("A customer hub password reset email has been sent to " + users[0].FirstName + " " + users[0].LastName);
                    }
                }
            }
            return ("There was an error sending the password.");
        }

        public string CreateCompanyBox(int orgID) //Organization org
        {

            Organization org = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);

            PhoneNumbers phone = new PhoneNumbers(TSAuthentication.GetLoginUser());
            phone.LoadByID(org.OrganizationID, ReferenceType.Organizations);

            string boxhtml = @"<tr>
                                 <td class=""result-icon"">
                                  <span class=""fa-stack fa-2x"">
                                    <i class=""fa fa-circle fa-stack-2x text-primary""></i>
                                    <i class=""fa fa-building-o fa-stack-1x fa-inverse""></i>
                                  </span>
                                  </td>
                                 <td>
                                   <div class=""peopleinfo"">
                                     <div class=""pull-right""><p class="""">{5} Open Tickets</p>{3}</div>
                                     <h4><a class=""companylink"" id=""o{0}"" href="""">{1}</a></h4>
                                     <ul>{2}{4}</ul>
                                   </div>
                                 </td>
                               </tr>";

            return string.Format(
              boxhtml,
              org.OrganizationID,
              (string.IsNullOrWhiteSpace(org.Name) ? "Unnamed" : org.Name),
              phone.IsEmpty || phone[0].Number == "" ? "" : "<li><a href=\"tel:" + phone[0].Number + "\">" + phone[0].Number + "</a></li>",
              org.HasPortalAccess ? "<p class=\"\">Has portal access</p>" : "<p class=\"\">Does not have portal access</p>",
              string.IsNullOrWhiteSpace(org.Website) ? "" : "<li><a href=\"" + org.Website + " \">" + org.Website + "</a></li>",
              GetCustomerOpenTickets(org));
        }

        public string CreateContactBox(int userID)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            Organization org = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), user.OrganizationID);
            PhoneNumbers phone = new PhoneNumbers(TSAuthentication.GetLoginUser());
            phone.LoadByID(user.UserID, ReferenceType.Users);

            string boxhtml = @"<tr>
                                  <td class=""result-icon"">

                                    <span class=""fa-stack fa-2x"">
                                      <i class=""fa fa-circle fa-stack-2x text-primary""></i>
                                      <i class=""fa fa-user fa-stack-1x fa-inverse""></i>
                                    </span>
                                    </td>
                                  <td>
                                      <div class=""peopleinfo"">
                                      <div class=""pull-right""><p class="""">{7} Open Tickets</p>{6}</div>
                                      <h4><a class=""contactlink"" id=""u{0}"" href="""">{1}{2}{3}</a></h4>
                                      <ul>{8}{4}{5}</ul>
                                  </div>
                                  </td>
                               </tr>";


            return string.Format(
              boxhtml,
              user.UserID,
              (string.IsNullOrWhiteSpace(user.FirstName) && string.IsNullOrWhiteSpace(user.LastName) ? "Unnamed" : user.FirstName + " " + user.LastName),
              "",
              "",
              "<li><a href='mailto:" + user.Email + "'>" + user.Email + "</a></li>",
              phone.IsEmpty || phone[0].Number == "" ? "" : "<li><a href=\"tel:" + phone[0].Number + "\">" + phone[0].Number + "</a></li>",
              user.IsPortalUser ? "<p class=\"\">Has portal access</p>" : "<p class=\"\">Does not have portal access</p>",
              GetContactTickets(user.UserID, 0),
              string.IsNullOrWhiteSpace(user.Title) ? org.Name != "_Unknown Company" ? "<li><a href='#' class='viewOrg' id='" + user.OrganizationID + "'>" + org.Name + "</a></li>" : "" : org.Name != "_Unknown Company" ? "<li>" + user.Title + " at <a href='#' class='viewOrg' id='" + user.OrganizationID + "'>" + org.Name + "</a></li>" : ""
              );
        }

        public string CreateRecentlyViewed(RecentlyViewedItem recent)
        {
            string recentHTML;
            string phoneStr;
            var sanitizer = new HtmlSanitizer();
            //user
            if (recent.RefType == 0)
            {
                Users u = new Users(TSAuthentication.GetLoginUser());
                u.LoadByUserID(recent.RefID);
                PhoneNumbers phone = new PhoneNumbers(TSAuthentication.GetLoginUser());
                phone.LoadByID(u[0].UserID, ReferenceType.Users);
                recentHTML = @" 
                <li>
                        <div class=""recent-info"">
                            <h4><a class=""contactlink"" data-userid=""{3}"" href=""""><i class=""fa fa-user color-orange""></i>{0}</a></h4>
                            <ul>
                                <li><a href=""mailto:{1}"" target=""_blank"">{1}</a></li>{2}
                            </ul>
                        </div>
                </li>";
                phoneStr = phone.IsEmpty ? "" : string.Format("<li><a href=\"tel:{0}\" target=\"_blank\">{0}</a></li>", phone[0].Number);
                return string.Format(recentHTML, u[0].FirstLastName, u[0].Email, phoneStr, u[0].UserID);
            }
            else
            {
                Organizations org = new Organizations(TSAuthentication.GetLoginUser());
                org.LoadByOrganizationID(recent.RefID);
                PhoneNumbers phone = new PhoneNumbers(TSAuthentication.GetLoginUser());
                phone.LoadByID(org[0].OrganizationID, ReferenceType.Organizations);

                recentHTML = @" 
                <li>
                        <div class=""recent-info"">
                            <h4><a class=""companylink"" data-organizationid=""{2}"" href=""""><i class=""fa fa-building-o color-green""></i>{0}</a></h4>{1}
                        </div>
                </li>";
                phoneStr = phone.IsEmpty ? "" : string.Format("<ul><li><a href=\"tel:{0}\" target=\"_blank\">{0}</a></li></ul>", phone[0].Number);

                return string.Format(recentHTML, sanitizer.Sanitize(org[0].Name), phoneStr, org[0].OrganizationID);
            }
        }

        public string GetCustomerOpenTickets(Organization org)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

            TicketTypes ticketTypes = new TicketTypes(TSAuthentication.GetLoginUser());
            ticketTypes.LoadByOrganizationID(organizations[0].OrganizationID, organizations[0].ProductType);

            int total = 0;

            foreach (TicketType ticketType in ticketTypes)
            {
                int count = Tickets.GetOrganizationOpenTicketCount(TSAuthentication.GetLoginUser(), org.OrganizationID, ticketType.TicketTypeID);
                total += count;
            }

            return total.ToString();
        }

        [WebMethod]
        public string GetContactTickets(int userID, int closed)
        {
            TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());

            return tickets.GetUserTicketCount(userID, closed).ToString();
        }

        [WebMethod]
        public string GetOrganizationTickets(int organizationID, int closed)
        {
            TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());

            return tickets.GetOrganizationTicketCount(organizationID, closed).ToString();
        }

        [WebMethod]
        public string GetOrganizationTickets2(int organizationID, int closed, bool includeChildren)
        {
            TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());

            return tickets.GetOrganizationTicketCount(organizationID, closed, includeChildren).ToString();
        }

        [WebMethod]
        public string GetOrganizationSentiment(int organizationID)
        {
            double? result = TeamSupport.Data.BusinessObjects.OrganizationSentiment.GetOrganizationSentiment(organizationID, TSAuthentication.OrganizationID);
            return result.HasValue ? ((int)Math.Round(result.Value)).ToString() : string.Empty;
        }

        [WebMethod]
        public int[] LoadRatingPercents(int organizationID, ReferenceType type)
        {
            List<int> results = new List<int>();

            AgentRatings ratings = new AgentRatings(TSAuthentication.GetLoginUser());
            if (type == ReferenceType.Organizations)
                ratings.LoadByOrganizationIDFilter(organizationID, "", -1);
            else if (type == ReferenceType.Users)
                ratings.LoadByContactIDFilter(organizationID, "", -1);

            if (ratings.Count > 0)
            {
                double negativeRating = 0, neutralRating = 0, positiveRating = 0, total = 0;
                foreach (AgentRating rate in ratings)
                {
                    switch (rate.Rating)
                    {
                        case -1:
                            negativeRating++;
                            break;
                        case 0:
                            neutralRating++;
                            break;
                        case 1:
                            positiveRating++;
                            break;
                    }
                }

                total = ratings.Count;

                results.Add((int)Math.Round((negativeRating / total) * 100));
                results.Add((int)Math.Round((neutralRating / total) * 100));
                results.Add((int)Math.Round((positiveRating / total) * 100));
            }
            else
            {
                results.Add(0);
                results.Add(0);
                results.Add(0);
            }
            return results.ToArray();

        }


        [WebMethod]
        public int[] LoadRatingPercents2(int organizationID, ReferenceType type, int productFamilyID)
        {
            List<int> results = new List<int>();

            AgentRatings ratings = new AgentRatings(TSAuthentication.GetLoginUser());

            if (type == ReferenceType.Organizations)
                ratings.LoadByOrganizationIDFilter(organizationID, "", -1, productFamilyID);
            else if (type == ReferenceType.Users)
                ratings.LoadByContactIDFilter(organizationID, "", -1, productFamilyID);

            if (ratings.Count > 0)
            {
                double negativeRating = 0, neutralRating = 0, positiveRating = 0, total = 0;
                foreach (AgentRating rate in ratings)
                {
                    switch (rate.Rating)
                    {
                        case -1:
                            negativeRating++;
                            break;
                        case 0:
                            neutralRating++;
                            break;
                        case 1:
                            positiveRating++;
                            break;
                    }
                }

                total = ratings.Count;

                results.Add((int)Math.Round((negativeRating / total) * 100));
                results.Add((int)Math.Round((neutralRating / total) * 100));
                results.Add((int)Math.Round((positiveRating / total) * 100));
            }
            else
            {
                results.Add(0);
                results.Add(0);
                results.Add(0);
            }
            return results.ToArray();

        }

        [WebMethod]
        public int[] LoadRatingPercentsUser(int userID)
        {
            List<int> results = new List<int>();
            List<int> ratingIDS = new List<int>();

            AgentRatingUsers aru = new AgentRatingUsers(TSAuthentication.GetLoginUser());
            aru.LoadByUserID(userID);

            double negativeRating = 0, neutralRating = 0, positiveRating = 0, total = 0;

            foreach (AgentRatingUser user in aru)
            {
                ratingIDS.Add(user.AgentRatingID);
            }

            if (aru.Count > 0)
            {
                AgentRatings ratings = new AgentRatings(TSAuthentication.GetLoginUser());
                ratings.LoadByAgentRatingIDFilter(ratingIDS.ToArray(), "", -1);

                foreach (AgentRating a in ratings)
                {
                    if (Tickets.GetTicket(TSAuthentication.GetLoginUser(), a.TicketID) != null)
                    {
                        switch (a.Rating)
                        {
                            case -1:
                                negativeRating++;
                                break;
                            case 0:
                                neutralRating++;
                                break;
                            case 1:
                                positiveRating++;
                                break;
                        }
                    }
                }

                total = aru.Count;
                results.Add((int)Math.Round((negativeRating / total) * 100));
                results.Add((int)Math.Round((neutralRating / total) * 100));
                results.Add((int)Math.Round((positiveRating / total) * 100));
            }
            else
            {
                results.Add(0);
                results.Add(0);
                results.Add(0);
            }

            return results.ToArray();

        }

        [WebMethod]
        public int[] LoadRatingPercentsUser2(int userID, int productFamilyID)
        {
            List<int> results = new List<int>();
            List<int> ratingIDS = new List<int>();

            AgentRatingUsers aru = new AgentRatingUsers(TSAuthentication.GetLoginUser());
            aru.LoadByUserID(userID);

            double negativeRating = 0, neutralRating = 0, positiveRating = 0, total = 0;

            foreach (AgentRatingUser user in aru)
            {
                ratingIDS.Add(user.AgentRatingID);
            }

            if (aru.Count > 0)
            {
                AgentRatings ratings = new AgentRatings(TSAuthentication.GetLoginUser());
                ratings.LoadByAgentRatingIDFilter(ratingIDS.ToArray(), "", -1, productFamilyID);

                foreach (AgentRating a in ratings)
                {
                    if (Tickets.GetTicket(TSAuthentication.GetLoginUser(), a.TicketID) != null)
                    {
                        switch (a.Rating)
                        {
                            case -1:
                                negativeRating++;
                                break;
                            case 0:
                                neutralRating++;
                                break;
                            case 1:
                                positiveRating++;
                                break;
                        }
                    }
                }

                total = ratings.Count;
                results.Add((int)Math.Round((negativeRating / total) * 100));
                results.Add((int)Math.Round((neutralRating / total) * 100));
                results.Add((int)Math.Round((positiveRating / total) * 100));
            }
            else
            {
                results.Add(0);
                results.Add(0);
                results.Add(0);
            }

            return results.ToArray();

        }

        [WebMethod]
        public CustomRatingClass[] LoadAgentRatings(int organizationID, string filter, int start, ReferenceType type)
        {
            List<CustomRatingClass> list = new List<CustomRatingClass>();

            AgentRatings ratings = new AgentRatings(TSAuthentication.GetLoginUser());
            if (type == ReferenceType.Organizations)
                ratings.LoadByOrganizationIDFilter(organizationID, filter, start);
            else if (type == ReferenceType.Users)
                ratings.LoadByContactIDFilter(organizationID, filter, start);

            Users users = new Users(TSAuthentication.GetLoginUser());

            foreach (AgentRating a in ratings)
            {
                CustomRatingClass ratingclass = new CustomRatingClass();
                ratingclass.rating = a.GetProxy();

                AgentRatingUsers agents = new AgentRatingUsers(TSAuthentication.GetLoginUser());
                agents.LoadByAgentRatingID(a.AgentRatingID);

                Organizations org = new Organizations(TSAuthentication.GetLoginUser());
                org.LoadByOrganizationID(a.CompanyID);

                ratingclass.users = new List<UserProxy>();
                foreach (AgentRatingUser u in agents)
                {

                    users.LoadByUserID(u.UserID);

                    ratingclass.users.Add(users[0].GetProxy());
                }

                users.LoadByUserID(a.ContactID);
                ratingclass.org = org[0].GetProxy();
                ratingclass.reporter = users[0].GetProxy();



                list.Add(ratingclass);
            }


            return list.ToArray();
        }

        [WebMethod]
        public CustomRatingClass[] LoadAgentRatings2(int organizationID, string filter, int start, ReferenceType type, int productFamilyID)
        {
            List<CustomRatingClass> list = new List<CustomRatingClass>();

            AgentRatings ratings = new AgentRatings(TSAuthentication.GetLoginUser());
            if (type == ReferenceType.Organizations)
                ratings.LoadByOrganizationIDFilter(organizationID, filter, start, productFamilyID);
            else if (type == ReferenceType.Users)
                ratings.LoadByContactIDFilter(organizationID, filter, start, productFamilyID);

            Users users = new Users(TSAuthentication.GetLoginUser());

            foreach (AgentRating a in ratings)
            {
                CustomRatingClass ratingclass = new CustomRatingClass();
                ratingclass.rating = a.GetProxy();

                AgentRatingUsers agents = new AgentRatingUsers(TSAuthentication.GetLoginUser());
                agents.LoadByAgentRatingID(a.AgentRatingID);

                Organizations org = new Organizations(TSAuthentication.GetLoginUser());
                org.LoadByOrganizationID(a.CompanyID);

                ratingclass.users = new List<UserProxy>();
                foreach (AgentRatingUser u in agents)
                {

                    users.LoadByUserID(u.UserID);

                    ratingclass.users.Add(users[0].GetProxy());
                }

                users.LoadByUserID(a.ContactID);
                ratingclass.org = org[0].GetProxy();
                ratingclass.reporter = users[0].GetProxy();



                list.Add(ratingclass);
            }


            return list.ToArray();
        }

        [WebMethod]
        public void DeleteAgentRating(int ratingID)
        {

            AgentRatingUsers users = new AgentRatingUsers(TSAuthentication.GetLoginUser());
            users.LoadByAgentRatingID(ratingID);

            User u = Users.GetUser(TSAuthentication.GetLoginUser(), users[0].UserID);
            string description = String.Format("{0} deleted agent rating for {1}", TSAuthentication.GetLoginUser().GetUserFullName(), u.FirstLastName);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Delete, ReferenceType.Users, users[0].AgentRatingID, description);

            users[0].Delete();
            users[0].Collection.Save();


            AgentRatings ratings = new AgentRatings(TSAuthentication.GetLoginUser());
            ratings.LoadByAgentRatingID(ratingID);
            ratings[0].Delete();
            ratings[0].Collection.Save();



        }

        [WebMethod]
        public CustomRatingClass[] LoadAgentRatingsUser2(int userID, string filter, int start, int productFamilyID)
        {
            List<CustomRatingClass> list = new List<CustomRatingClass>();
            List<int> ratingIDS = new List<int>();
            int count = 0;

            AgentRatingUsers aru = new AgentRatingUsers(TSAuthentication.GetLoginUser());
            aru.LoadByUserID(userID);

            foreach (AgentRatingUser user in aru)
            {
                ratingIDS.Add(user.AgentRatingID);
            }

            if (aru.Count > 0)
            {
                AgentRatings ratings = new AgentRatings(TSAuthentication.GetLoginUser());
                ratings.LoadByAgentRatingIDFilter(ratingIDS.ToArray(), filter, start);

                foreach (AgentRating a in ratings)
                {

                    if (Tickets.GetTicket(TSAuthentication.GetLoginUser(), a.TicketID) != null)
                    {

                        Users users = new Users(TSAuthentication.GetLoginUser());
                        CustomRatingClass ratingclass = new CustomRatingClass();
                        ratingclass.rating = a.GetProxy();
                        AgentRatingUsers agents = new AgentRatingUsers(TSAuthentication.GetLoginUser());
                        agents.LoadByAgentRatingID(a.AgentRatingID);

                        Organizations org = new Organizations(TSAuthentication.GetLoginUser());
                        org.LoadByOrganizationID(a.CompanyID);

                        ratingclass.users = new List<UserProxy>();
                        foreach (AgentRatingUser u in agents)
                        {

                            users.LoadByUserID(u.UserID);

                            ratingclass.users.Add(users[0].GetProxy());
                        }

                        ratingclass.org = org[0].GetProxy();
                        users.LoadByUserID(ratings[count].ContactID);
                        ratingclass.reporter = users[0].GetProxy();

                        list.Add(ratingclass);
                        count++;
                    }
                }
            }

            return list.ToArray();
        }

        [WebMethod]
        public CustomRatingClass[] LoadAgentRatingsUser(int userID, string filter, int start)
        {
            List<CustomRatingClass> list = new List<CustomRatingClass>();
            List<int> ratingIDS = new List<int>();
            int count = 0;

            AgentRatingUsers aru = new AgentRatingUsers(TSAuthentication.GetLoginUser());
            aru.LoadByUserID(userID);

            foreach (AgentRatingUser user in aru)
            {
                ratingIDS.Add(user.AgentRatingID);
            }

            if (aru.Count > 0)
            {
                AgentRatings ratings = new AgentRatings(TSAuthentication.GetLoginUser());
                ratings.LoadByAgentRatingIDFilter(ratingIDS.ToArray(), filter, start);

                foreach (AgentRating a in ratings)
                {

                    if (Tickets.GetTicket(TSAuthentication.GetLoginUser(), a.TicketID) != null)
                    {

                        Users users = new Users(TSAuthentication.GetLoginUser());
                        CustomRatingClass ratingclass = new CustomRatingClass();
                        ratingclass.rating = a.GetProxy();
                        AgentRatingUsers agents = new AgentRatingUsers(TSAuthentication.GetLoginUser());
                        agents.LoadByAgentRatingID(a.AgentRatingID);

                        Organizations org = new Organizations(TSAuthentication.GetLoginUser());
                        org.LoadByOrganizationID(a.CompanyID);

                        ratingclass.users = new List<UserProxy>();
                        foreach (AgentRatingUser u in agents)
                        {

                            users.LoadByUserID(u.UserID);

                            ratingclass.users.Add(users[0].GetProxy());
                        }

                        ratingclass.org = org[0].GetProxy();
                        users.LoadByUserID(ratings[count].ContactID);
                        ratingclass.reporter = users[0].GetProxy();

                        list.Add(ratingclass);
                        count++;
                    }
                }
            }

            return list.ToArray();
        }

        [WebMethod]
        public string MergeCompanies(int winningOrganizationID, int losingOrganizationID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Organization company = Organizations.GetOrganization(loginUser, winningOrganizationID);
            Organization loosingCompany = Organizations.GetOrganization(loginUser, losingOrganizationID);
            string lossingCompanyNameForHistoryEntries = loosingCompany.Name + " (" + loosingCompany.OrganizationID.ToString() + ")";
            String errLocation = "";
         
            try
            {
                //Merge Contacts - runs sp Org_MergeContacts_Updates
                company.Collection.MergeUpdateContacts(losingOrganizationID, winningOrganizationID, lossingCompanyNameForHistoryEntries, loginUser);
            
                company.Collection.MergeUpdateFiles(losingOrganizationID, winningOrganizationID, lossingCompanyNameForHistoryEntries, loginUser);

                //Company Merge - runs sp Org_MergeCompanies_Updates
                company.Collection.CompanyMergeSproc(losingOrganizationID, winningOrganizationID, lossingCompanyNameForHistoryEntries, loginUser);

                string description = "Merged '" + lossingCompanyNameForHistoryEntries + "' tickets, notes, files, products, assets, WaterCoolerMessages, agent ratings, calendar events, custom values, phone numbers, adresses, and subscriptions.";
                ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Organizations, winningOrganizationID, description);

                company.NeedsIndexing = true;
                company.Collection.Save();               

                return errLocation;

            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(loginUser)).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();
                errLocation=string.Format("Error merging companies. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
                return errLocation;                
            }              
        }

        [WebMethod]
        public string MergeContacts(int winningUserID, int losingUserID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            User contact = Users.GetUser(loginUser, winningUserID);
            User loosingContact = Users.GetUser(loginUser, losingUserID);
            string lossingContactNameForHistoryEntries = loosingContact.FirstLastName + " (" + loosingContact.UserID.ToString() + ")";
            String errLocation = "";

            try
            {
                contact.Collection.MergeUpdateActions(losingUserID, winningUserID, lossingContactNameForHistoryEntries, loginUser);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(loginUser)).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();
                errLocation = string.Format("Error merging contact actions. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                contact.Collection.MergeUpdateTickets(losingUserID, winningUserID, lossingContactNameForHistoryEntries, loginUser);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(loginUser)).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();
                errLocation = string.Format("Error merging contact tickets. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                contact.Collection.MergeUpdateNotes(losingUserID, winningUserID, lossingContactNameForHistoryEntries, loginUser);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(loginUser)).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("Error merging contact notes. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                contact.Collection.MergeUpdateFiles(losingUserID, winningUserID, lossingContactNameForHistoryEntries, loginUser);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(TSAuthentication.GetLoginUser())).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("Error merging contact files. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                contact.Collection.MergeUpdateProducts(losingUserID, winningUserID, lossingContactNameForHistoryEntries, loginUser);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(loginUser)).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("Error merging contact products. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                contact.Collection.MergeUpdateAssets(losingUserID, winningUserID, lossingContactNameForHistoryEntries, loginUser);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(TSAuthentication.GetLoginUser())).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("Error merging contact assets. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                contact.Collection.MergeUpdateRatings(losingUserID, winningUserID, lossingContactNameForHistoryEntries, loginUser);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(TSAuthentication.GetLoginUser())).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("Error merging contact ratings. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                contact.Collection.MergeUpdateCustomValues(losingUserID, winningUserID, lossingContactNameForHistoryEntries, loginUser);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(TSAuthentication.GetLoginUser())).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("Error merging contact custom values. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                PhoneNumbers winnerUserPhoneNumbers = new PhoneNumbers(loginUser);
                winnerUserPhoneNumbers.LoadByID(winningUserID, ReferenceType.Users);
                PhoneNumbers loserUserPhoneNumbers = new PhoneNumbers(loginUser);
                loserUserPhoneNumbers.LoadByID(losingUserID, ReferenceType.Users);

                if (winnerUserPhoneNumbers.Count == 0 && loserUserPhoneNumbers.Count > 0)
                {
                    for (int i = 0; i < loserUserPhoneNumbers.Count; i++)
                    {
                        PhoneNumber phoneNumber = loserUserPhoneNumbers[i];
                        phoneNumber.RefID = winningUserID;
                        phoneNumber.RefType = ReferenceType.Users;
                        phoneNumber.Extension = loserUserPhoneNumbers[i].Extension;
                        phoneNumber.Number = loserUserPhoneNumbers[i].Number;
                        phoneNumber.PhoneTypeID = loserUserPhoneNumbers[i].PhoneTypeID;
                        loserUserPhoneNumbers.Save();
                    }

                    string description = "Merged '" + lossingContactNameForHistoryEntries + "' Phone Numbers.";
                    ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Users, winningUserID, description);
                }
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(TSAuthentication.GetLoginUser())).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("Error merging contact phone numbers. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                Addresses winnerUserAddresses = new Addresses(loginUser);
                winnerUserAddresses.LoadByID(winningUserID, ReferenceType.Users);
                Addresses loserUserAddresses = new Addresses(loginUser);
                loserUserAddresses.LoadByID(losingUserID, ReferenceType.Users);

                if (winnerUserAddresses.Count == 0 && loserUserAddresses.Count > 0)
                {
                    for (int i = 0; i < loserUserAddresses.Count; i++)
                    {
                        Address address = loserUserAddresses[i];
                        address.RefID = winningUserID;
                        address.RefType = ReferenceType.Users;
                        address.Addr1 = loserUserAddresses[i].Addr1;
                        address.Addr2 = loserUserAddresses[i].Addr2;
                        address.Addr3 = loserUserAddresses[i].Addr3;
                        address.City = loserUserAddresses[i].City;
                        address.Country = loserUserAddresses[i].Country;
                        address.Description = loserUserAddresses[i].Description;
                        address.State = loserUserAddresses[i].State;
                        address.Zip = loserUserAddresses[i].Zip;
                        loserUserAddresses.Save();
                    }

                    string description = "Merged '" + lossingContactNameForHistoryEntries + "' Addresses.";
                    ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Users, winningUserID, description);
                }
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(TSAuthentication.GetLoginUser())).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("Error merging contact addresses. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                contact.Collection.DeleteRecentlyViewItems(losingUserID);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(TSAuthentication.GetLoginUser())).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("Error merging company ratings. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                contact.Collection.DeleteFromDB(losingUserID);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(TSAuthentication.GetLoginUser())).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("Error deleting losing company from database. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            contact.NeedsIndexing = true;
            contact.Collection.Save();

            return errLocation;
        }

        public string CreateTextControl(CustomField field, bool isEditable = false, int organizationID = -1)
        {
            StringBuilder html = new StringBuilder();

            if (isEditable)
            {
                CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
                html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-3 control-label'>{1}</label> 
                                        <div class='col-xs-9'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
            }
            else
            {
                html.AppendFormat("<div class='col-xs-9'><input class='form-control col-xs-10 customField {1}' id='{0}' name='{0}'></div>", field.CustomFieldID, field.IsRequired ? "required" : "");
            }
            return html.ToString();
        }

        public string CreateNumberControl(CustomField field, bool isEditable = false, int organizationID = -1)
        {
            StringBuilder html = new StringBuilder();
            if (isEditable)
            {
                CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
                html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-3 control-label'>{1}</label> 
                                        <div class='col-xs-9'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
            }
            else
                html.AppendFormat("<div class='col-xs-9'><input class='form-control col-xs-10 customField number {1}' id='{0}'  name='{0}'></div>", field.CustomFieldID, field.IsRequired ? "required" : "");

            return html.ToString();
        }

        public string CreateDateControl(CustomField field, bool isEditable = false, int organizationID = -1)
        {
            StringBuilder html = new StringBuilder();
            if (isEditable)
            {
                CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
                html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-3 control-label'>{1}</label> 
                                        <div class='col-xs-3'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
            }
            else
                html.AppendFormat("<div class='col-xs-3'><input class='form-control datepicker col-xs-10 customField {1}' id='{0}' type='_date' name='{0}'></div>", field.CustomFieldID, field.IsRequired ? "required" : "");

            return html.ToString();
        }

        public string CreateTimeControl(CustomField field, bool isEditable = false, int organizationID = -1)
        {
            StringBuilder html = new StringBuilder();
            if (isEditable)
            {
                CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
                html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-3 control-label'>{1}</label> 
                                        <div class='col-xs-3'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
            }
            else
                html.AppendFormat("<div class='col-xs-3'><input class='form-control timepicker col-xs-10 customField {1}' id='{0}' type='_time'  name='{0}'></div>", field.CustomFieldID, field.IsRequired ? "required" : "");

            return html.ToString();
        }

        public string CreateDateTimeControl(CustomField field, bool isEditable = false, int organizationID = -1)
        {
            StringBuilder html = new StringBuilder();
            if (isEditable)
            {
                CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
                html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-3 control-label'>{1}</label> 
                                        <div class='col-xs-3'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
            }
            else
                html.AppendFormat("<div class='col-xs-3'><input class='form-control datetimepicker col-xs-10 customField {1}' id='{0}' type='_datetime'  name='{0}'></div>", field.CustomFieldID, field.IsRequired ? "required" : "");

            return html.ToString();
        }

        public string CreateBooleanControl(CustomField field, bool isEditable = false, int organizationID = -1)
        {
            StringBuilder html = new StringBuilder();
            if (isEditable)
            {
                CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
                html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-3 control-label'>{1}</label> 
                                        <div class='col-xs-9'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
            }
            else
            {
                html.AppendFormat("<div class='col-xs-1'><label><input class='customField' id='{0}' type='checkbox'></label></div>", field.CustomFieldID);
            }
            return html.ToString();
        }

        public string CreatePickListControl(CustomField field, bool isEditable = false, int organizationID = -1)
        {
            StringBuilder html = new StringBuilder();
            string[] items = field.ListValues.Split('|');
            if (isEditable)
            {
                CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
                html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-3 control-label'>{1}</label> 
                                        <div class='col-xs-9'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='select'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
            }
            else
            {
                html.AppendFormat("<div class='col-xs-9'><select class='form-control customField' id='{0}'  name='{0}' type='picklist'>", field.CustomFieldID);
                foreach (string item in items)
                {
                    html.AppendFormat("<option value='{0}'>{1}</option>", item, item);
                }
                html.Append("</select></div>");
            }
            return html.ToString();
        }

        public string AddControl(CustomField field)
        {
            StringBuilder htmltest = new StringBuilder();
            switch (field.FieldType)
            {
                case CustomFieldType.Text: htmltest.AppendLine(CreateTextControl(field)); break;
                case CustomFieldType.Number: htmltest.AppendLine(CreateTextControl(field)); break;
                case CustomFieldType.Date: htmltest.AppendLine(CreateDateControl(field)); break;
                case CustomFieldType.DateTime: htmltest.AppendLine(CreateDateTimeControl(field)); break;
                case CustomFieldType.Boolean: htmltest.AppendLine(CreateBooleanControl(field)); break;
                case CustomFieldType.PickList: htmltest.AppendLine(CreatePickListControl(field)); break;
                default: break;
            }
            return htmltest.ToString();
        }

        [WebMethod]
        public CompanyParentsViewItemProxy AddParent(int childID, int parentID)
        {
            if (childID != parentID)
            {
                LoginUser loginUser = TSAuthentication.GetLoginUser();
                CompanyParentsView parentTest = new CompanyParentsView(loginUser);
                parentTest.LoadByChildAndParentIDs(childID, parentID);
                if (parentTest.Count == 0)
                {
                    CustomerRelationships customerRelationships = new CustomerRelationships(loginUser);
                    CustomerRelationship customerRelationship = customerRelationships.AddNewCustomerRelationship();
                    customerRelationship.CustomerID = childID;
                    customerRelationship.RelatedCustomerID = parentID;
                    customerRelationship.CreatorID = loginUser.UserID;
                    customerRelationships.Save();

                    CompanyParentsView parent = new CompanyParentsView(loginUser);
                    parent.LoadByChildAndParentIDs(childID, parentID);

                    string description = String.Format("{0} added {1} as parent", TSAuthentication.GetUser(loginUser).FirstLastName, parent[0].ParentName);
                    ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Organizations, childID, description);

                    return parent.GetCompanyParentsViewItemProxies()[0];
                }
            }
            return null;
        }

        [WebMethod]
        public void RemoveParent(int childID, int parentID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            CompanyParentsView parent = new CompanyParentsView(loginUser);
            parent.LoadByChildAndParentIDs(childID, parentID);

            CustomerRelationships customerRelationships = new CustomerRelationships(loginUser);
            customerRelationships.LoadByCustomerAndRelatedCustomerIDs(childID, parentID);
            customerRelationships[0].Delete();
            customerRelationships.Save();

            string description = String.Format("{0} removed {1} as parent", TSAuthentication.GetUser(loginUser).FirstLastName, parent[0].ParentName);
            ActionLogs.AddActionLog(loginUser, ActionLogType.Delete, ReferenceType.Organizations, childID, description);
        }

        [WebMethod]
        public bool CanAccessCustomer(int customerID)
        {
            int userID = TSAuthentication.GetLoginUser().UserID;
            bool result = false;
            Users u = new Users(TSAuthentication.GetLoginUser());
            u.LoadByUserID(userID);

            if (u[0].TicketRights == TicketRightType.Customers)
            {
                using (SqlConnection connection = new SqlConnection(TSAuthentication.GetLoginUser().ConnectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM UserRightsOrganizations WHERE (UserID=@UserID) AND (OrganizationID=@OrganizationID)";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@OrganizationID", customerID);

                    SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow);
                    if (reader.Read())
                    {
                        result = true;
                    }
                    else
                        result = false;

                    reader.Close();
                    connection.Close();
                    return result;
                }
            }
            else
            {
                return true;
            }

            return true;
        }

        public class NewCustomerSave
        {
            public NewCustomerSave() { }
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public string Website { get; set; }
            [DataMember]
            public string CompanyDomains { get; set; }
            [DataMember]
            public int? DefaultSupportUserID { get; set; }
            [DataMember]
            public int? DefaultSupportGroupID { get; set; }
            [DataMember]
            public string TimeZoneID { get; set; }
            [DataMember]
            public DateTime? SAExpirationDate { get; set; }
            [DataMember]
            public int? SlaLevelID { get; set; }
            [DataMember]
            public int SupportHoursMonth { get; set; }
            [DataMember]
            public bool Active { get; set; }
            [DataMember]
            public bool PortalAccess { get; set; }
            [DataMember]
            public bool APIEnabled { get; set; }
            [DataMember]
            public string InactiveReason { get; set; }
            [DataMember]
            public string Description { get; set; }
            [DataMember]
            public List<CustomFieldSaveInfo> Fields { get; set; }
        }

        public class NewProductSave
        {
            public NewProductSave() { }
            [DataMember]
            public string OrganizationID { get; set; }
            [DataMember]
            public string UserID { get; set; }
            [DataMember]
            public string ProductID { get; set; }
            [DataMember]
            public int Version { get; set; }
            [DataMember]
            public string SupportExpiration { get; set; }
            [DataMember]
            public int SlaLevelID { get; set; }
            [DataMember]
            public int OrganizationProductID { get; set; }
            [DataMember]
            public int UserProductID { get; set; }
            [DataMember]
            public List<CustomFieldSaveInfo> Fields { get; set; }
        }

        public class NewContactSave
        {
            public NewContactSave() { }
            [DataMember]
            public string Company { get; set; }
            [DataMember]
            public bool Active { get; set; }
            [DataMember]
            public string Email { get; set; }
            [DataMember]
            public string FirstName { get; set; }
            [DataMember]
            public string LastName { get; set; }
            [DataMember]
            public string Title { get; set; }
            [DataMember]
            public string MiddleName { get; set; }
            [DataMember]
            public bool BlockInboundEmail { get; set; }
            [DataMember]
            public bool BlockEmailFromCreatingOnly { get; set; }
            [DataMember]
            public bool IsPortalUser { get; set; }
            [DataMember]
            public bool IsSystemAdmin { get; set; }
            [DataMember]
            public bool IsFinanceAdmin { get; set; }
            [DataMember]
            public bool SyncAddress { get; set; }
            [DataMember]
            public bool SyncPhone { get; set; }
            [DataMember]
            public bool EmailPortalPW { get; set; }
            [DataMember]
            public List<CustomFieldSaveInfo> Fields { get; set; }
            [DataMember]
            public bool EmailHubPW { get; set; }
        }

        public class EmailSave
        {
            public EmailSave() { }
            [DataMember]
            public string Email { get; set; }
            [DataMember]
            public int? EmailID { get; set; }
        }

        public class PhoneSave
        {
            public PhoneSave() { }
            [DataMember]
            public string Extension { get; set; }
            [DataMember]
            public string Number { get; set; }
            [DataMember]
            public int PhoneTypeID { get; set; }
            [DataMember]
            public int? PhoneID { get; set; }
        }

        public class AddressSave
        {
            public AddressSave() { }
            [DataMember]
            public string Addr1 { get; set; }
            [DataMember]
            public string Addr2 { get; set; }
            [DataMember]
            public string Addr3 { get; set; }
            [DataMember]
            public string City { get; set; }
            [DataMember]
            public string Country { get; set; }
            [DataMember]
            public string Description { get; set; }
            [DataMember]
            public string State { get; set; }
            [DataMember]
            public string Zip { get; set; }
            [DataMember]
            public int addressID { get; set; }
        }

        public class CustComp
        {
            public string Name { get; set; }
            public object userinfo { get; set; }

        }

        public class CustomFieldSaveInfo
        {
            public CustomFieldSaveInfo() { }
            [DataMember]
            public int CustomFieldID { get; set; }
            [DataMember]
            public object Value { get; set; }
        }

        public class OrganizationCustomProduct
        {
            [DataMember]
            public string ProductName { get; set; }
            [DataMember]
            public string VersionNumber { get; set; }
            [DataMember]
            public string SupportExpiration { get; set; }
            [DataMember]
            public string VersionStatus { get; set; }
            [DataMember]
            public string IsReleased { get; set; }
            [DataMember]
            public string ReleaseDate { get; set; }
            [DataMember]
            public string DateCreated { get; set; }
            [DataMember]
            public string SlaAssigned { get; set; }
            [DataMember]
            public int SlaLevelID { get; set; }
            [DataMember]
            public int ProductID { get; set; }
            [DataMember]
            public int OrganizationProductID { get; set; }
            [DataMember]
            public List<string> CustomFields { get; set; }

        }

        public class UserCustomProduct
        {
            [DataMember]
            public string ProductName { get; set; }
            [DataMember]
            public string VersionNumber { get; set; }
            [DataMember]
            public string SupportExpiration { get; set; }
            [DataMember]
            public string VersionStatus { get; set; }
            [DataMember]
            public string IsReleased { get; set; }
            [DataMember]
            public string ReleaseDate { get; set; }
            [DataMember]
            public string DateCreated { get; set; }
            [DataMember]
            public int SlaLevelID { get; set; }
            [DataMember]
            public string SlaAssigned { get; set; }
            [DataMember]
            public int ProductID { get; set; }
            [DataMember]
            public int UserProductID { get; set; }
            [DataMember]
            public List<string> CustomFields { get; set; }

        }

        public class OrgProp
        {
            [DataMember]
            public OrganizationProxy orgproxy { get; set; }
            [DataMember]
            public string SLA { get; set; }
            [DataMember]
            public string PrimaryUser { get; set; }
            [DataMember]
            public string SupportGroup { get; set; }
            [DataMember]
            public string DefaultSupportUser { get; set; }
            [DataMember]
            public string SAED { get; set; }
            [DataMember]
            public CompanyParentsViewItemProxy[] Parents { get; set; }
            [DataMember]
            public bool IsParent { get; set; }

        }

        public class CustomRatingClass
        {
            [DataMember]
            public AgentRatingProxy rating;
            [DataMember]
            public List<UserProxy> users;
            [DataMember]
            public UserProxy reporter;
            [DataMember]
            public OrganizationProxy org;

        }

        public class CustomerSLATrigger
        {
            public int SlaLevelId { get; set; }
            public int SlaTriggerId { get; set; }
            public string LevelName { get; set; }
            public string Severity { get; set; }
            public string TicketType { get; set; }
            public string SLAType { get; set; }
        }

        public class CustomerSLAViolation
        {
            public int TicketId { get; set; }
            public int TicketNumber { get; set; }
            public string Violation { get; set; }
            public DateTime DateViolated { get; set; }
            public string LevelName { get; set; }
            public string Severity { get; set; }
            public string TicketType { get; set; }
        }
    }
}
