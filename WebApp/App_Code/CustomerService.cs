﻿using System;
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
            return user.GetProxy();
        }

        [WebMethod]
        public string SetCompanyName(int orgID, string value)
        {
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
            o.Website = value;
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
            o.CompanyDomains = value;
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
                o.SAExpirationDate = DataUtils.DateToUtc(TSAuthentication.GetLoginUser(), Convert.ToDateTime(value));
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
            o.Description = value;
            o.Collection.Save();
            string description = String.Format("{0} set company description to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, orgID, description);
            return value != "" ? value : "Empty";
        }
        [WebMethod]
        public string SetCompanyInactive(int orgID, string value)
        {
            Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
            o.InActiveReason = value;
            o.Collection.Save();
            string description = String.Format("{0} set company inactive to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Organizations, orgID, description);
            return value;
        }

        [WebMethod]
        public string SetContactEmail(int userID, string email)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.Email = email;
            u.Collection.Save();
            string description = String.Format("{0} set contact email to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, email);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, userID, description);
            return email != "" ? email : "Empty";
        }
        [WebMethod]
        public string SetContactName(int userID, string fname, string mname, string lname)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.FirstName = fname;
            u.MiddleName = mname;
            u.LastName = lname;
            u.Collection.Save();
            string description = String.Format("{0} set contact name to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, u.FirstLastName);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, userID, description);
            return u.FirstLastName;
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
        public string SetContactTitle(int userID, string title)
        {
            User u = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            u.Title = title;
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
            u.Collection.Save();
            string description = String.Format("{0} set contact active to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, value);
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
                if(o.HasPortalAccess == false)
                    return 2;
                else
                    return 1;
            }

            return result;
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

            html.AppendLine(CreateFormElement("Active", user.IsActive, "editable"));
            html.AppendLine(CreateFormElement("Portal User", user.IsPortalUser, "editable"));
            html.AppendLine(CreateFormElement("Prevent email from creating tickets", user.BlockInboundEmail, "editable"));

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


            contact[0] = contactInfo.ToString();
            contact[1] = html.ToString();
            
            return contact;

        }

        public string CreateFormElement(string fieldTitle, object fieldValue, string editable="")
        {
            StringBuilder form = new StringBuilder("");

            if (fieldValue is bool)
            {
                if ((bool)fieldValue == true)
                    fieldValue = "Yes";
                else if ((bool)fieldValue == false)
                    fieldValue = "No";

            }

            if(editable == "db")
            form.AppendFormat(@"<div class='form-group'>
                                <label class='col-xs-4 control-label' for='field{0}'>{1}</label>
                                <div class='col-xs-8'>
                                <span class='form-control-static {3}' id='field{0}'>{2}</span>
                                </div>
                                </div>",fieldTitle.Replace(" ",""), fieldTitle, fieldValue != null ? fieldValue : "Empty", editable );
            else
                form.AppendFormat(@"<div class='form-group'>
                                <label class='col-xs-4 control-label' for='field{0}'>{1}</label>
                                <div class='col-xs-8'>
                                <p class='form-control-static {3}' id='field{0}'>{2}</p>
                                </div>
                                </div>", fieldTitle.Replace(" ", ""), fieldTitle, (fieldValue != null && fieldValue !="" ) ? fieldValue : "Empty", editable);

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
            recent.LoadRecent(TSAuthentication.GetLoginUser().UserID);

            builder.Append(@"<ul class=""recent-list"">");
            foreach (RecentlyViewedItem item in recent)
            {
                builder.Append(CreateRecentlyViewed(item));
            }
            builder.Append("</ul>");
            return builder.ToString();
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
            cats.LoadByRefType(ReferenceType.Organizations, -1);
            return cats.GetCustomFieldCategoryProxies();
        }

        [WebMethod]
        public string LoadCustomProperties(int refID, ReferenceType refType){
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            fields.LoadByReferenceType(TSAuthentication.OrganizationID, refType, -1);
            
            CustomFieldCategories cats = new CustomFieldCategories(TSAuthentication.GetLoginUser());
            cats.LoadByRefType(refType, -1);

            StringBuilder htmltest = new StringBuilder("");
            foreach (CustomField field in fields){
                if (field.CustomFieldCategoryID == null){
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

            foreach (CustomFieldCategory cat in cats){

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
        public string LoadCustomControls(ReferenceType refType){
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            fields.LoadByReferenceType(TSAuthentication.OrganizationID, refType, -1);
            int count = 0;
            CustomFieldCategories cats = new CustomFieldCategories(TSAuthentication.GetLoginUser());
            cats.LoadByRefType(refType, -1);

            StringBuilder htmltest = new StringBuilder("");

            htmltest.Append("<div class='form-group'>");

            foreach (CustomField field in fields)
            {
                if (count == 0)
                {
                    htmltest.Append("<div class='row'>");
                    count++;
                }

                if (field.CustomFieldCategoryID == null)
                {
                    htmltest.AppendFormat("<div class='col-xs-4'><label for='{0}' class='col-xs-4 control-label'>{1}</label>", field.CustomFieldID, field.Name);
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
                    count++;
                }

                if (count % 4 == 0)
                {
                    htmltest.Append("</div>"); //end row
                    count = 0;
                }
            }
            if (count != 0)
            {
                count = 0;
                htmltest.Append("</div>"); // end row if not closed
            }
            htmltest.Append("</div>"); //end form-group

            count = 0;

            foreach (CustomFieldCategory cat in cats)
            {

                htmltest.AppendFormat("<h3>{0}</h3>",cat.Category);
                htmltest.Append("<div class='form-group'>");

                foreach (CustomField field in fields)
                {
                    if (count == 0)
                    {
                        htmltest.Append("<div class='row'>");
                        count++;
                    }

                    if (field.CustomFieldCategoryID == cat.CustomFieldCategoryID)
                    {
                        htmltest.AppendFormat("<div class='col-xs-4'><label for='{0}' class='col-xs-4 control-label'>{1}</label>", field.CustomFieldID, field.Name);
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
                        count++;
                    }

                    if (count % 4 == 0)
                    {
                        htmltest.Append("</div>");
                        count = 0;
                    }
                }
                if (count != 0){
                    count = 0;
                    htmltest.Append("</div>");
                }
                htmltest.Append("</div>");
            }

            return htmltest.ToString();
        }

        [WebMethod]
        public string LoadCustomContactControls()
        {
            int count = 0;
            StringBuilder htmltest = new StringBuilder("");
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            fields.LoadByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Contacts, -1);

            if (fields.Count > 0)
                htmltest.Append("<div class='form-group'>");

                foreach (CustomField field in fields)
                {
                    if (count == 0)
                    {
                        htmltest.Append("<div class='row'>");
                        count++;
                    }


                    htmltest.AppendFormat("<div class='col-xs-4'><label for='{0}' class='col-xs-4 control-label'>{1}</label>", field.CustomFieldID, field.Name);
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
                    count++;


                    if (count % 4 == 0)
                    {
                        htmltest.Append("</div>");
                        count = 0;
                    }
                }
                if (count != 0)
                {
                    count = 0;
                    htmltest.Append("</div>");
                }
                htmltest.Append("</div>");

            return htmltest.ToString();
        }

        [WebMethod]
        public string[] LoadcustomProductHeaders(int organizationID)
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
        public NoteProxy[] LoadNotes(int refID, ReferenceType refType)
        {
            Notes notes = new Notes(TSAuthentication.GetLoginUser());
            notes.LoadByReferenceType(refType, refID);

            return notes.GetNoteProxies();
        }

        [WebMethod]
        public NoteProxy LoadNote(int noteID)
        {
            Notes notes = new Notes(TSAuthentication.GetLoginUser());
            notes.LoadByNoteID(noteID);

            return notes[0].GetProxy();
        }

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
                    if(note == null)
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
                u.RefType =  refType;
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
            Attachments attachments = new Attachments(TSAuthentication.GetLoginUser());
            attachments.LoadByReference(refType, refID);

            return attachments.GetAttachmentProxies();
        }

        [WebMethod]
        public OrganizationCustomProduct[] LoadProducts(int organizationID)
        {
            OrganizationProducts organizationProducts = new OrganizationProducts(TSAuthentication.GetLoginUser());
            organizationProducts.LoadForCustomerProductGrid(organizationID);
            List<OrganizationCustomProduct> list = new List<OrganizationCustomProduct>();
            CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
            fields.LoadByReferenceType(TSAuthentication.GetLoginUser().OrganizationID, ReferenceType.OrganizationProducts);


            foreach (DataRow row in organizationProducts.Table.Rows)
            {
                OrganizationCustomProduct test = new OrganizationCustomProduct();
                test.ProductName = row["ProductName"].ToString();
                test.VersionNumber = row["VersionNumber"].ToString();
                test.SupportExpiration = row["SupportExpiration"].ToString() != "" ? ((DateTime)row["SupportExpiration"]).ToString(GetDateFormatNormal()) : "";
                test.VersionStatus = row["VersionStatus"].ToString();
                test.IsReleased = row["IsReleased"].ToString();
                test.ReleaseDate = row["ReleaseDate"].ToString() != "" ? ((DateTime)row["ReleaseDate"]).ToString(GetDateFormatNormal()) : "";
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

            return custProd;
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
            Products products = new Products(TSAuthentication.GetLoginUser());
            products.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

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
        public OrgProp GetProperties(int organizationID)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationID(organizationID);
            Users users = new Users(TSAuthentication.GetLoginUser());

            OrgProp orgProp = new OrgProp();

            if (organizations.IsEmpty) return null;

            orgProp.SAED = organizations[0].SAExpirationDate == null ? "[None]" : ((DateTime)organizations[0].SAExpirationDate).ToShortDateString();
            if (organizations[0].SlaLevelID == null)
                orgProp.SLA="[None]";
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

            return orgProp;

        }



        [WebMethod]
        public string LoadContacts(int organizationID){
            StringBuilder htmlresults = new StringBuilder("");
            StringBuilder phoneResults = new StringBuilder("");
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByOrganizationIDLastName(organizationID, false);

            foreach (User u in users)
            {

                PhoneNumbers phoneNumbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
                phoneNumbers.LoadByID(u.UserID, ReferenceType.Users);

                foreach (PhoneNumber p in phoneNumbers)
                {
                  phoneResults.AppendFormat("<p class='list-group-item-text'><span class=\"text-muted\">{0}</span>: <a href=\"tel:{1}\" target=\"_blank\">{1}</a> {2}</p>", p.PhoneTypeName, p.Number, string.IsNullOrWhiteSpace(p.Extension) ? "" : "Ext:" + p.Extension);
                }

                htmlresults.AppendFormat(@"<div class='list-group-item'>
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
                            </div>"

                    , u.IsActive ? "user-active" : "user-inactive", u.IsActive ? "Active" : "Inactive", u.FirstLastName, u.Email != "" ? "<a href='mailto:" + u.Email + "'>" + u.Email + "</a>" : "Empty", GetContactTickets(u.UserID, 0), GetContactTickets(u.UserID, 1), phoneResults, u.UserID, u.IsPortalUser == true ? "<p class='list-group-item-text'><span class=\"text-muted\">Has Portal Access</span>" : "", u.Title != "" ? u.Title : "");

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

            if(info.Company != null)
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

            user.Email = info.Email;
            user.FirstName = info.FirstName;
            user.LastName = info.LastName;
            user.Title = info.Title;
            user.MiddleName = info.MiddleName;
            user.BlockInboundEmail = info.BlockInboundEmail;
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
            else if (info.EmailPortalPW && info.IsPortalUser)
                EmailPosts.SendWelcomePortalUser(TSAuthentication.GetLoginUser(), user.UserID, password);

            string description = String.Format("{0} created contact {1}", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, user.FirstLastName);
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Insert, ReferenceType.Users, user.UserID, description);

            return user.UserID;
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
        public void SaveNote(string title, string noteText, int noteID, int refID, ReferenceType refType, bool isAlert = false)
        {
            Note note = null;
            bool isNew = false;
            if (noteID > -1)
            {
                note = (Note)Notes.GetNote(TSAuthentication.GetLoginUser(), noteID);
                string description = String.Format("{0} modified note {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, title);
                ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, refType, noteID, description);

                UserNoteSettings uns = new UserNoteSettings(TSAuthentication.GetLoginUser());
                uns.LoadByIDType(noteID, refType);

                foreach (UserNoteSetting u in uns)
                {
                    u.IsDismissed = false;
                    u.IsSnoozed = false;
                    u.Collection.Save();
                }
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
                note.Collection.Save();
                if (isNew)
                {
                    string description = String.Format("{0} added note {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, title);
                    ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Insert, refType, note.NoteID, description);
                }

            }

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
        public int LoadCDI(int organizationID)
        {
            Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), organizationID);

            return organization.CustDisIndex;
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
                if(open)
                     count = Tickets.GetOrganizationOpenTicketCount(TSAuthentication.GetLoginUser(), organizationID, ticketType.TicketTypeID);
                else
                    count = Tickets.GetOrganizationClosedTicketCount(TSAuthentication.GetLoginUser(), organizationID, ticketType.TicketTypeID);
                total += count;

                if (count > 0)
                    chartString.AppendFormat("{0},{1},", ticketType.Name, count.ToString());
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
                    chartString.AppendFormat("{0},{1},", ticketType.Name, count.ToString());
                //chartString.AppendFormat("['{0}',{1}],",ticketType.Name, count.ToString());
            }
            if (chartString.ToString().EndsWith(","))
            {
                chartString.Remove(chartString.Length - 1, 1);
            }

            return chartString.ToString();
        }

        [WebMethod]
        public string GetDateFormat(bool lower=false)
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
            u.UpdateDeletedOrg(organizationID,unknownID);

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
                    if (DataUtils.ResetPassword(TSAuthentication.GetLoginUser(), users[0], !(TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).ParentID == null)))
                    {
                        return("A new password has been sent to " + users[0].FirstName + " " + users[0].LastName);
                    }
                }
            }
            return("There was an error sending the password.");
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
              phone.IsEmpty || phone[0].Number == "" ? "" : "<li><a href=\"tel:"+ phone[0].Number+"\">" + phone[0].Number + "</a></li>",
              org.HasPortalAccess ? "<p class=\"\">Has portal access</p>" : "<p class=\"\">Does not have portal access</p>", 
              string.IsNullOrWhiteSpace(org.Website) ? "" : "<li><a href=\""+org.Website+" \">"+ org.Website+"</a></li>" , 
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
              string.IsNullOrWhiteSpace(user.Title) ? org.Name != "_Unknown Company" ? "<li><a href='#' class='viewOrg' id='" + user.OrganizationID + "'>" + org.Name + "</a></li>" : "" : org.Name != "_Unknown Company" ? "<li>"+ user.Title +" at <a href='#' class='viewOrg' id='" + user.OrganizationID + "'>" + org.Name + "</a></li>" : ""
              );
        }

        public string CreateRecentlyViewed(RecentlyViewedItem recent)
        {
            string recentHTML;
            string phoneStr;
            //user
            if(recent.RefType == 0){
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
            else{
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

                return string.Format(recentHTML, org[0].Name, phoneStr, org[0].OrganizationID);
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
        public CustomRatingClass[] LoadAgentRatings(int organizationID, string filter)
        {
            List<CustomRatingClass> list = new List<CustomRatingClass>();

            AgentRatings ratings = new AgentRatings(TSAuthentication.GetLoginUser());
            ratings.LoadByOrganizationIDFilter(organizationID, filter);

            Users users = new Users(TSAuthentication.GetLoginUser());

            foreach (AgentRating a in ratings)
            {
                CustomRatingClass ratingclass = new CustomRatingClass();
                ratingclass.rating = a.GetProxy();
            
                AgentRatingUsers agents = new AgentRatingUsers(TSAuthentication.GetLoginUser());
                agents.LoadByAgentRatingID(a.AgentRatingID);

                ratingclass.users = new List<UserProxy>();
                foreach (AgentRatingUser u in agents)
                {

                    users.LoadByUserID(u.UserID);

                    ratingclass.users.Add(users[0].GetProxy());
                }

                users.LoadByUserID(a.ContactID);

                ratingclass.reporter = users[0].GetProxy();

                list.Add(ratingclass);
            }


            return list.ToArray();
        }

        [WebMethod]
        public CustomRatingClass[] LoadAgentRatingsUser(int userID, string filter)
        {
            List<CustomRatingClass> list = new List<CustomRatingClass>();

            AgentRatingUsers aru = new AgentRatingUsers(TSAuthentication.GetLoginUser());
            aru.LoadByUserID(userID);

            foreach (AgentRatingUser user in aru)
            {
                AgentRatings ratings = new AgentRatings(TSAuthentication.GetLoginUser());
                ratings.LoadByAgentRatingIDFilter(user.AgentRatingID, filter);

                Users users = new Users(TSAuthentication.GetLoginUser());

                CustomRatingClass ratingclass = new CustomRatingClass();
                ratingclass.rating = ratings[0].GetProxy();

                AgentRatingUsers agents = new AgentRatingUsers(TSAuthentication.GetLoginUser());
                agents.LoadByAgentRatingID(ratings[0].AgentRatingID);

                ratingclass.users = new List<UserProxy>();
                foreach (AgentRatingUser u in agents)
                {

                    users.LoadByUserID(u.UserID);

                    ratingclass.users.Add(users[0].GetProxy());
                }

                users.LoadByUserID(ratings[0].ContactID);

                ratingclass.reporter = users[0].GetProxy();

                list.Add(ratingclass);

            }

            return list.ToArray();
        }


        public string CreateTextControl(CustomField field, bool isEditable = false, int organizationID = -1)
        {
            StringBuilder html = new StringBuilder();

            if (isEditable)
            {
                CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
                html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value  );
            }
            else
            {
                html.AppendFormat("<div class='col-xs-8'><input class='form-control col-xs-10 customField {1}' id='{0}' name='{0}'></div>", field.CustomFieldID, field.IsRequired ? "required":"");
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
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
            }
            else
                html.AppendFormat("<div class='col-xs-8'><input class='form-control col-xs-10 customField number {1}' id='{0}'  name='{0}'></div>", field.CustomFieldID, field.IsRequired ? "required" : "");

            return html.ToString();
        }

        public string CreateDateControl(CustomField field, bool isEditable = false, int organizationID = -1)
        {
            StringBuilder html = new StringBuilder();
            if (isEditable)
            {
                CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
                html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
            }
            else
                html.AppendFormat("<div class='col-xs-8'><input class='form-control datepicker col-xs-10 customField {1}' id='{0}' type='date' name='{0}'></div>", field.CustomFieldID, field.IsRequired ? "required" : "");

            return html.ToString();
        }

        public string CreateTimeControl(CustomField field, bool isEditable = false, int organizationID = -1)
        {
            StringBuilder html = new StringBuilder();
            if (isEditable)
            {
                CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
                html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
            }
            else
                html.AppendFormat("<div class='col-xs-8'><input class='form-control timepicker col-xs-10 customField {1}' id='{0}' type='time'  name='{0}'></div>", field.CustomFieldID, field.IsRequired ? "required" : "");

            return html.ToString();
        }

        public string CreateDateTimeControl(CustomField field, bool isEditable = false, int organizationID = -1)
        {
            StringBuilder html = new StringBuilder();
            if (isEditable)
            {
                CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
                html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='text'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
            }
            else
                html.AppendFormat("<div class='col-xs-8'><input class='form-control datetimepicker col-xs-10 customField {1}' id='{0}' type='datetime'  name='{0}'></div>", field.CustomFieldID, field.IsRequired ? "required" : "");
            
            return html.ToString();
        }

        public string CreateBooleanControl(CustomField field, bool isEditable = false, int organizationID = -1)
        {
            StringBuilder html = new StringBuilder();
            if (isEditable)
            {
                CustomValue value = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, organizationID);
                html.AppendFormat(@"<div class='form-group'> 
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
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
                                        <label for='{0}' class='col-xs-4 control-label'>{1}</label> 
                                        <div class='col-xs-8'> 
                                            <p class='form-control-static'><a class='editable' id='{0}' data-type='select'>{2}</a></p> 
                                        </div> 
                                    </div>", field.CustomFieldID, field.Name, value.Value);
            }
            else
            {
                html.AppendFormat("<div class='col-xs-8'><select class='form-control customField' id='{0}'  name='{0}' type='picklist'>", field.CustomFieldID);
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

        public class NewCustomerSave
        {
            public NewCustomerSave() { }
            [DataMember] public string Name{ get; set; }
            [DataMember] public string Website{ get; set; }
            [DataMember] public string CompanyDomains{ get; set; }
            [DataMember] public int? DefaultSupportUserID{ get; set; }
            [DataMember] public int? DefaultSupportGroupID{ get; set; }
            [DataMember] public string TimeZoneID{ get; set; }
            [DataMember] public DateTime? SAExpirationDate{ get; set; }
            [DataMember] public int? SlaLevelID{ get; set; }
            [DataMember] public int SupportHoursMonth{ get; set; }
            [DataMember] public bool Active{ get; set; }
            [DataMember] public bool PortalAccess{ get; set; }
            [DataMember] public bool APIEnabled{ get; set; }
            [DataMember] public string InactiveReason{ get; set; }
            [DataMember] public string Description{ get; set; }
            [DataMember] public List<CustomFieldSaveInfo> Fields { get; set; }
        }

        public class NewProductSave
        {
            public NewProductSave() { }
            [DataMember] public string OrganizationID { get; set; }
            [DataMember] public string ProductID { get; set; }
            [DataMember] public int Version { get; set; }
            [DataMember] public string SupportExpiration { get; set; }
            [DataMember] public int OrganizationProductID { get; set; }
            [DataMember] public List<CustomFieldSaveInfo> Fields { get; set; }
        }

        public class NewContactSave
        {
            public NewContactSave() { }
            [DataMember] public string Company { get; set; }
            [DataMember] public bool Active { get; set; }
            [DataMember] public string Email { get; set; }
            [DataMember] public string FirstName { get; set; }
            [DataMember] public string LastName { get; set; }
            [DataMember] public string Title { get; set; }
            [DataMember] public string MiddleName { get; set; }
            [DataMember] public bool BlockInboundEmail { get; set; }
            [DataMember] public bool IsPortalUser { get; set; }
            [DataMember] public bool IsSystemAdmin { get; set; }
            [DataMember] public bool IsFinanceAdmin { get; set; }
            [DataMember] public bool SyncAddress { get; set; }
            [DataMember] public bool SyncPhone { get; set; }
            [DataMember] public bool EmailPortalPW { get; set; }
            [DataMember] public List<CustomFieldSaveInfo> Fields { get; set; }
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
            [DataMember] public string Addr1 { get; set; }
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
            [DataMember] public string ProductName { get; set; }
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
            public int ProductID { get; set; }
            [DataMember]
            public int OrganizationProductID { get; set; }
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
        }

        public class CustomRatingClass
        {
            [DataMember]
            public AgentRatingProxy rating;
            [DataMember]
            public List<UserProxy> users;
            [DataMember]
            public UserProxy reporter;
        }

    }

}