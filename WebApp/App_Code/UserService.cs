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
using System.Linq;
using ImageResizer;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class UserService : System.Web.Services.WebService
    {

        public UserService() { }

        [WebMethod]
        public UserProxy UpdateUserStatusComment(string comment)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            User user = TSAuthentication.GetUser(loginUser);
            user.InOfficeComment = comment;
            user.Collection.Save();

            WaterCooler watercooler = new WaterCooler(loginUser);
            WaterCoolerItem item = watercooler.AddNewWaterCoolerItem();
            item.Message = string.Format("<strong>{0} - </strong>{1}", user.FirstLastName, user.InOfficeComment);
            item.OrganizationID = user.OrganizationID;
            item.TimeStamp = DateTime.UtcNow;
            item.UserID = user.UserID;
            watercooler.Save();

			user.CryptedPassword = string.Empty;

            return user.GetProxy();
        }

        [WebMethod]
        public void SetIsClassicView(bool value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            User user = TSAuthentication.GetUser(loginUser);
            user.IsClassicView = value;
            user.Collection.Save();
        }

        [WebMethod]
        public void SetMenuItems(int userID, string values)
        {
          if (!TSAuthentication.IsSystemAdmin) return;
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return;
          user.MenuItems = values;
          user.Collection.Save();
        }

		  public class result
		  {
			  public string location;
		  }

		  [WebMethod]
		  public void SaveTinyMCEPasteImage()
		  {
			  string image = "";
			  try
            {

                String temppath = HttpContext.Current.Request.PhysicalApplicationPath + "images\\";
                string path = TeamSupport.Data.Quarantine.WebAppQ.GetAttachmentPath4(TSAuthentication.OrganizationID, TSAuthentication.GetLoginUser());
                string filename = Guid.NewGuid().ToString();
                HttpFileCollection tinymceImage = HttpContext.Current.Request.Files;
                string domain = SystemSettings.GetAppUrl();

                string filepath = String.Format("{0}/dc/{1}/images/{2}.png", domain, TSAuthentication.OrganizationID, filename);

                tinymceImage[0].SaveAs(path + '\\' + filename + ".png");

                //byte[] imgbytes = Convert.FromBase64String(image);
                //File.WriteAllBytes(temppath + "temp_" + filename + ".png", imgbytes);

                var lst = new List<result>
                          {
                             new result { location =  "dc/" + TSAuthentication.OrganizationID + "/images/" + filename + ".png"}
                          };
                JavaScriptSerializer serialize = new JavaScriptSerializer();
                this.Context.Response.ContentType = "application/json; charset=utf-8";
                this.Context.Response.Write("{\"location\": \"" + filepath + "\"}");

                //if (image != "")
                //{
                //	ImageBuilder.Current.Build(temppath + "temp_" + filename + ".png", path + '\\' + filename + ".png", new ResizeSettings(image));
                //	File.Delete(temppath + "temp_" + filename + ".png");

                //	var lst = new List<result>
                //				{
                //					new result { location = TSAuthentication.OrganizationID + "/images/" + filename + ".png"}
                //				};

                //	return JsonConvert.SerializeObject(lst);
                //}
            }
            catch (Exception ex)
			  {
				  ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "Save Paste Image", image);


			  }


			  //return "";
		  }


        //tells us whether the logged in user can edit the specified user's account
        [WebMethod]
        public bool AllowUserEdit(int userID)
        {
            return userID == TSAuthentication.GetLoginUser().UserID;
        }

        [WebMethod]
        public void HideWelcomePage()
        {
          User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
          user.ShowWelcomePage = false;
          user.Collection.Save();

        }

        [WebMethod]
        public void SetProductFamiliesRights(int userID, int value)
        {
            if (!TSAuthentication.IsSystemAdmin) return;
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return;
            user.ProductFamiliesRights = value;
            user.Collection.Save();
        }

        [WebMethod]
        public void SetTicketRights(int userID, int value)
        {
          if (!TSAuthentication.IsSystemAdmin) return;
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return;
          user.TicketRights = (TicketRightType)value;
          user.Collection.Save();
        }

        [WebMethod]
        public UserProxy GetUser(int userID)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return null;
			user.CryptedPassword = string.Empty;
            return user.GetProxy();
        }

        [WebMethod]
        public string GetUserPhoto(int userID)
        {
            string path;
            if (userID == -99)
                userID = TSAuthentication.GetLoginUser().UserID;

            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);

				path = String.Format("/dc/{0}/UserAvatar/{1}/120/{2}", user.OrganizationID, userID, Guid.NewGuid());
            return path;
        }

        [WebMethod]
        public string GetUserSignature(int userID)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);

            return user.Signature;
        }

        [WebMethod]
        public AddressProxy[] GetUserAddresses(int userID)
        {
            List<AddressProxy> addresses = new List<AddressProxy>();

            Addresses addrs = new Addresses(TSAuthentication.GetLoginUser());
            addrs.LoadByID(userID, ReferenceType.Users);

            foreach (Address address in addrs)
            {
                addresses.Add(address.GetProxy());
            }

            return addresses.ToArray();
        }

        [WebMethod]
        public PhoneNumberProxy[] GetUserPhoneNumbers(int userID)
        {
            List<PhoneNumberProxy> phones = new List<PhoneNumberProxy>();

            PhoneNumbers numbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
            numbers.LoadByID(userID, ReferenceType.Users);

            foreach (PhoneNumber phone in numbers)
            {
                phones.Add(phone.GetProxy());
            }

            return phones.ToArray();
        }

        [WebMethod]
        public string GetUserContactCard(int userID)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user == null) return null;
          Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), user.OrganizationID);

          StringBuilder ContactCardHTML = new StringBuilder();
          ContactCardHTML.Append("<address>");

          if (organization != null)
          {
            ContactCardHTML.AppendFormat("<strong><a href='#' onclick='top.Ts.MainPage.openCustomer({0}); return false;'>{1}</a></strong><br>", organization.OrganizationID, organization.Name);
          }

          if (!string.IsNullOrEmpty(user.Title))
          {
            ContactCardHTML.AppendFormat("{0}<br>", user.Title);
          }

          if (!string.IsNullOrEmpty(user.Email))
          {
            ContactCardHTML.AppendFormat("<a href='mailto:{0}'>{0}</a><br>", user.Email);
          }

          PhoneNumbers numbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
          numbers.LoadByID(userID, ReferenceType.Users);

          foreach (PhoneNumber phone in numbers)
          {
            ContactCardHTML.AppendFormat("<abbr title='{0}'>{1}:</abbr> {2}<br>", phone.PhoneTypeName, phone.PhoneTypeName.Substring(0, 1), phone.FormattedNumber);
          }

          ContactCardHTML.Append("</address>");

          return ContactCardHTML.ToString();
        }

        [WebMethod]
        public GroupProxy[] GetUserGroups(int userID)
        {
            Groups groups = new Groups(TSAuthentication.GetLoginUser());
            groups.LoadByUserID(userID);
            return groups.GetGroupProxies();
        }

        [WebMethod]
        public bool UpdateUserStatus(bool value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            User user = TSAuthentication.GetUser(loginUser);
            user.InOffice = value;
            user.Collection.Save();
            return value;
            /*
            WaterCooler watercooler = new WaterCooler(loginUser);
            WaterCoolerItem item = watercooler.AddNewWaterCoolerItem();
            item.Message = string.Format("<strong>{0}</strong> {1}", user.FirstLastName, user.InOffice ? "is now in the office." : "has left the office.");
            item.OrganizationID = user.OrganizationID;
            item.TimeStamp = DateTime.UtcNow;
            item.UserID = user.UserID;
            watercooler.Save();
            */
        }

        [WebMethod]
        public bool UpdateSpecificUserStatus(int userID, bool value)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            user.InOffice = value;
            user.Collection.Save();
            return value;
        }

        [WebMethod]
        public UserProxy ToggleUserStatus()
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            User user = TSAuthentication.GetUser(loginUser);
            user.InOffice = !user.InOffice;
            user.Collection.Save();

            WaterCooler watercooler = new WaterCooler(loginUser);
            WaterCoolerItem item = watercooler.AddNewWaterCoolerItem();
            item.Message = string.Format("<strong>{0}</strong> {1}", user.FirstLastName, user.InOffice ? "is now in the office." : "has left the office.");
            item.OrganizationID = user.OrganizationID;
            item.TimeStamp = DateTime.UtcNow;
            item.UserID = user.UserID;
			user.CryptedPassword = string.Empty;
            return user.GetProxy();
        }

        [WebMethod]
        public ChatUserSettingProxy ToggleUserChatStatus()
        {
            ChatUserSetting setting = ChatUserSettings.GetChatUserSetting(TSAuthentication.GetLoginUser(), TSAuthentication.UserID);
            setting.IsAvailable = !setting.IsAvailable;
            setting.Collection.Save();
            return setting.GetProxy();
        }

        [WebMethod]
        public ChatUserSettingProxy ToggleSpecifiedUserChatStatus(int userID)
        {
            ChatUserSetting setting = ChatUserSettings.GetChatUserSetting(TSAuthentication.GetLoginUser(), userID);
            setting.IsAvailable = !setting.IsAvailable;
            setting.Collection.Save();
            return setting.GetProxy();
        }

        [WebMethod]
        public BasicUser[] GetUsers()
        {
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByOrganizationID(TSAuthentication.OrganizationID, true);
            List<BasicUser> result = new List<BasicUser>();

            foreach (User user in users)
            {
                BasicUser basic = new BasicUser();
                basic.Name = user.FirstName + " " + user.LastName;
                basic.UserID = user.UserID;
                basic.InOfficeComment = user.InOfficeComment;
                result.Add(basic);
            }
            return result.ToArray();
        }

        [WebMethod]
        public void CreateDefaultUsers()
        {
            Users.CreateDefaultUsers(TSAuthentication.GetLoginUser());
        }

        [WebMethod]
        public GroupProxy[] GetGroups()
        {
            Groups groups = new Groups(TSAuthentication.GetLoginUser());
            groups.LoadByOrganizationID(TSAuthentication.OrganizationID);
            return groups.GetGroupProxies();
        }

        [WebMethod]
        public GroupProxy[] GetTicketGroups()
        {
            Groups groups = new Groups(TSAuthentication.GetLoginUser());
            groups.LoadByOrganizationID(TSAuthentication.OrganizationID);
            List<GroupProxy> proxies = groups.GetGroupProxies().OrderBy(gp => gp.Name).ToList();

            Groups userGroups = new Groups(TSAuthentication.GetLoginUser());
            userGroups.LoadByUserID(TSAuthentication.UserID);

            foreach (Group userGroup in userGroups.OrderBy(ug => ug.Name))
            {
                GroupProxy proxy = proxies.Find(p => p.GroupID == userGroup.GroupID);

                if (proxy != null)
                {
                    proxies.Remove(proxy);
                    proxies.Insert(0, proxy);
                }
            }

            return proxies.ToArray();
        }

        [WebMethod]
        public BasicUser[] GetGroupUsers(int groupID)
        {
          Users users = new Users(TSAuthentication.GetLoginUser());
          users.LoadByGroupID(groupID);
          List<BasicUser> result = new List<BasicUser>();

          foreach (User user in users)
          {
            BasicUser basic = new BasicUser();
            basic.Name = user.FirstName + " " + user.LastName;
            basic.UserID = user.UserID;
            basic.InOfficeComment = user.InOfficeComment;
            result.Add(basic);
          }
          return result.ToArray();
        }

        [WebMethod]
        public AutocompleteItem[] SearchUsers(string searchTerm)
        {
            UsersView users = new UsersView(TSAuthentication.GetLoginUser());
            users.LoadByTerm(TSAuthentication.OrganizationID, searchTerm, 15);

            List<AutocompleteItem> list = new List<AutocompleteItem>();
            foreach (UsersViewItem user in users)
            {
                list.Add(new AutocompleteItem(user.FirstName + " " + user.LastName, user.UserID.ToString()));
            }

            return list.ToArray();
        }

        [WebMethod]
        public bool ShowIntroVideo()
        {
            if (Settings.UserDB.ReadBool("ShowIntroVideo", true) && Organizations.GetUserCount(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID) == 1)
            {
                Settings.UserDB.WriteBool("ShowIntroVideo", false);
                return true;
            }

            return false;

        }

        [WebMethod]
        public string SaveUserName(int userID, string firstName, string middleName, string lastName)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return null;

            user.FirstName = firstName;
            user.MiddleName = middleName;
            user.LastName = lastName;
            user.Collection.Save();
            TeamSupportSync.SyncUser(user.UserID, user.OrganizationID, user.FirstName, user.LastName, user.Email, user.Title);
            return firstName + ' ' + lastName;

        }

        [WebMethod]
        public string SaveUserInfo(int userID, string info)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return null;
            user.UserInformation = info;
            user.Collection.Save();
            return info;
        }

        [WebMethod]
        public string SaveUserSignature(int userID, string signature)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return null;
            user.Signature = signature;
            user.Collection.Save();
            return signature;
        }

		  [WebMethod]
		  public void ClearUserSignature(int userID)
		  {
			  User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
			  if (user.OrganizationID != TSAuthentication.OrganizationID) return;
			  user.Signature = "";
			  user.Collection.Save();
			  return;
		  }

        [WebMethod]
        public string SaveUserTitle(int userID, string title)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return null;
            user.Title = title.Trim();
            user.Collection.Save();
            TeamSupportSync.SyncUser(user.UserID, user.OrganizationID, user.FirstName, user.LastName, user.Email, user.Title);
            return title;
        }

        [WebMethod]
        public string SaveUserEmail(int userID, string email)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return null;

            if (email.Length < 1 || email.IndexOf('@') < 0 || email.IndexOf('.') < 0)
                return "_error";

            TeamSupportSync.SyncUserEmail(user.UserID, user.OrganizationID, email, user.Email);

            user.Email = email;
            user.Collection.Save();

            return user.Email;
        }

        [WebMethod]
        public string SaveUserLinkedin(int userID, string linkedin)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return null;

            user.LinkedIn = linkedin;
            user.Collection.Save();

            if (!linkedin.StartsWith("http://"))
                linkedin = "http://" + linkedin;
            return linkedin;
        }

        [WebMethod]
        public bool SetIsActive(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            if (value == false)
            {
                Organizations orgs = new Organizations(TSAuthentication.GetLoginUser());
                orgs.ResetDefaultSupportUser(TSAuthentication.GetLoginUser(), user.UserID);
            }
            else
            {
                int userCount = Organizations.GetUserCount(TSAuthentication.GetLoginUser(), TSAuthentication.GetLoginUser().OrganizationID);
                Organization organization = (Organization)Organizations.GetOrganization(TSAuthentication.GetLoginUser(), TSAuthentication.GetLoginUser().OrganizationID);

                if (organization.UserSeats <= userCount)
                {
                    return false;
                }
            }

            user.IsActive = value;
            user.Collection.Save();
            user.EmailCountToMuroc(value);
            return user.IsActive;
        }

        [WebMethod]
        public bool SetEmailNotify(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.ReceiveTicketNotifications = value;
            user.Collection.Save();
            return user.ReceiveTicketNotifications;
        }

        [WebMethod]
        public bool SetSubscribeTickets(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.SubscribeToNewTickets = value;
            user.Collection.Save();
            return user.SubscribeToNewTickets;
        }

        [WebMethod]
        public bool SetSubscribeActions(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.SubscribeToNewActions = value;
            user.Collection.Save();
            return user.SubscribeToNewActions;
        }

        [WebMethod]
        public bool SetAutoSubscribe(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.DoNotAutoSubscribe = value;
            user.Collection.Save();
            return user.DoNotAutoSubscribe;
        }

        [WebMethod]
        public bool SetUseClassicTicketPage(int userID, bool value)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

          user.IsClassicView = value;
          user.Collection.Save();
          return user.IsClassicView;
        }

        [WebMethod]
        public bool SetGroupNotify(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.ReceiveAllGroupNotifications = value;
            user.Collection.Save();
            return user.ReceiveAllGroupNotifications;
        }

        [WebMethod]
        public bool SetUnassignedGroupNotify(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.ReceiveUnassignedGroupEmails = value;
            user.Collection.Save();
            return user.ReceiveUnassignedGroupEmails;
        }

        [WebMethod]
        public bool SetBlockEmail(int userID, bool value)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

          user.BlockInboundEmail = value;
          user.Collection.Save();
          return user.BlockInboundEmail;
        }
        [WebMethod]
        public bool SetOnlyEmailAfterHours(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.OnlyEmailAfterHours = value;
            user.Collection.Save();
            return user.OnlyEmailAfterHours;
        }


        [WebMethod]
        public bool SetSysAdmin(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.IsSystemAdmin = value;
            user.Collection.Save();
            return user.IsSystemAdmin;
        }

        [WebMethod]
        public bool SetRestrictUserFromEditingAnyActions(int userID, bool value)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
          if (!TSAuthentication.IsSystemAdmin) return !value;

          user.RestrictUserFromEditingAnyActions = value;
          user.Collection.Save();
          return user.RestrictUserFromEditingAnyActions;
        }

        [WebMethod]
        public bool SetChangeCanBulkMerge(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanBulkMerge = value;
            user.Collection.Save();
            return user.CanBulkMerge;
        }

        [WebMethod]
        public bool SetRestrictUserFromExportingData(int userID, bool value)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
          if (!TSAuthentication.IsSystemAdmin) return !value;

          user.DisableExporting = value;
          user.Collection.Save();
          return user.DisableExporting;
        }

        [WebMethod]
        public bool SetRestrictUserPublicActions(int userID, bool value)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
          if (!TSAuthentication.IsSystemAdmin) return !value;

          user.DisablePublic = value;
          user.Collection.Save();
          return user.DisablePublic;
        }

        [WebMethod]
        public bool SetAllowUserToEditAnyAction(int userID, bool value)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
          if (!TSAuthentication.IsSystemAdmin) return !value;

          user.AllowUserToEditAnyAction = value;
          user.Collection.Save();
          return user.AllowUserToEditAnyAction;
        }

        [WebMethod]
        public bool SetUserCanPinAction(int userID, bool value)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
          if (!TSAuthentication.IsSystemAdmin) return !value;

          user.UserCanPinAction = value;
          user.Collection.Save();
          return user.UserCanPinAction;
        }

        [WebMethod]
        public bool SetChangeTicketVisibility(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.ChangeTicketVisibility = value;
            user.Collection.Save();
            return user.ChangeTicketVisibility;
        }

        [WebMethod]
        public bool SetInactiveFilter(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.FilterInactive = value;
            user.Collection.Save();
            return user.FilterInactive;
        }

        [WebMethod]
        public bool SetChangeCommunityVisibility(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanChangeCommunityVisibility = value;
            user.Collection.Save();
            return user.CanChangeCommunityVisibility;
        }

        [WebMethod]
        public bool SetChangeCanCreateCompany(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanCreateCompany = value;
            user.Collection.Save();
            return user.CanCreateCompany;
        }

        [WebMethod]
        public bool SetChangeCanEditCompany(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanEditCompany = value;
            user.Collection.Save();
            return user.CanEditCompany;
        }

        [WebMethod]
        public bool SetChangeCanCreateContacts(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanCreateContact = value;
            user.Collection.Save();
            return user.CanCreateContact;
        }

        [WebMethod]
        public bool SetChangeCanEditContacts(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanEditContact = value;
            user.Collection.Save();
            return user.CanEditContact;
        }

        [WebMethod]
        public bool SetChangeCanEditAssets(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanEditAsset = value;
            user.Collection.Save();
            return user.CanEditAsset;
        }

        [WebMethod]
        public bool SetChangeCanCreateAssets(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanCreateAsset = value;
            user.Collection.Save();
            return user.CanCreateAsset;
        }

        [WebMethod]
        public bool SetChangeCanCreateProducts(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanCreateProducts = value;
            user.Collection.Save();
            return user.CanCreateProducts;
        }

        [WebMethod]
        public bool SetChangeCanEditProducts(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanEditProducts = value;
            user.Collection.Save();
            return user.CanEditProducts;
        }

        [WebMethod]
        public bool SetChangeCanCreateVersions(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanCreateVersions = value;
            user.Collection.Save();
            return user.CanCreateVersions;
        }

        [WebMethod]
        public bool SetChangeCanEditVersions(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.CanEditVersions = value;
            user.Collection.Save();
            return user.CanEditVersions;
        }

        [WebMethod]
        public bool SetChangeKbVisibility(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.ChangeKBVisibility = value;
            user.Collection.Save();
            return user.ChangeKBVisibility;
        }

        [WebMethod]
        public bool SetAllowAnyTicketCustomer(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;
            if (!TSAuthentication.IsSystemAdmin) return !value;

            user.AllowAnyTicketCustomer = value;
            user.Collection.Save();
            return user.AllowAnyTicketCustomer;
        }


        [WebMethod]
        public string SetChatUser(int userID, bool value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Organization organization = Organizations.GetOrganization(loginUser, loginUser.OrganizationID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value.ToString();
            if (!TSAuthentication.IsSystemAdmin) return value.ToString();

            if (value && Organizations.GetChatCount(loginUser, loginUser.OrganizationID) >= organization.ChatSeats)
                return "error";

            user.IsChatUser = value;
            user.Collection.Save();
            return user.IsChatUser.ToString();

        }

        [WebMethod]
        public string SetTimezone(int userID, string value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            TimeZoneInfo tzinfo;
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.TimeZoneID = value;
            user.Collection.Save();
            try
            {
                tzinfo = TimeZoneInfo.FindSystemTimeZoneById(value);
            }
            catch (Exception)
            {
                tzinfo = null;
            }

            return tzinfo.DisplayName;
        }

        [WebMethod]
        public string[] GetTimezone()
        {
            List<string> list = new List<string>();
            System.Collections.ObjectModel.ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
            foreach (TimeZoneInfo info in timeZones)
            {
                list.Add(info.DisplayName);
                list.Add(info.Id);
            }
            return list.ToArray();
        }

        [WebMethod]
        public string SetCulture(int userID, string value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            string cu = CultureInfo.GetCultureInfo(int.Parse(value)).Name;

            user.CultureName = cu;
            user.Collection.Save();
            return CultureInfo.GetCultureInfo(int.Parse(value)).DisplayName;
        }

        [WebMethod]
        public string[] GetCultures()
        {
            List<string> list = new List<string>();
            foreach (CultureInfo info in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (!info.IsNeutralCulture)
                {
                    list.Add(info.DisplayName + "_" + info.LCID);
                }
            }
            list.Sort();
            return list.ToArray();
        }

        [WebMethod]
        public string SetUserInformation(int userID, string value)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

            user.UserInformation = value;
            user.Collection.Save();
            return user.UserInformation;
        }

        [WebMethod]
        public string ResetEmailPW(int userID)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return ("There was an error resetting the password.");
            try
            {
                if (DataUtils.ResetPassword(TSAuthentication.GetLoginUser(), user, false))
                {
                return ("A new password has been sent to " + user.FirstName + " " + user.LastName);

                }
            }
            catch(Exception e)
            {
                return ("There was an error resetting the password.");
            }

            return ("There was an error resetting the password.");
        }

        [WebMethod]
        public string CreateNewContact(string emailAddress, string firstName, string lastName, string companyName, string phone, bool createNewCompany)
        {
            User user;
            Users users = new Users(TSAuthentication.GetLoginUser());

            OrganizationService orgService = new OrganizationService();

            Organization organization;
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());

            int newOrgID = GetIDByExactName(companyName);
            string errorMsg, resultStr="";
            string email = emailAddress;

            if (createNewCompany == false)
            {
                // Check Process and Flow
                if (firstName.Length < 1 && lastName.Length < 1 && (email.Length < 1 || email.IndexOf('@') < 0 || email.IndexOf('.') < 0) && newOrgID == -1)
                {
                    errorMsg = "The company you have specified is invalid.";
                    return errorMsg;
                }

                if ((firstName.Length < 1 || lastName.Length < 1))
                {
                    errorMsg = "The name you have specified is invalid.  Please enter a valid name.";
                    return errorMsg;
                }

                /*if ((email.Length < 1 || email.IndexOf('@') < 0 || email.IndexOf('.') < 0))
                {
                    errorMsg = "The email you have specified is invalid.  Please choose another email.";
                    return errorMsg;
                }*/

                if (email != "" && !users.IsEmailValid(email, -1, newOrgID))
                {
                    errorMsg = "The email you have specified is already in use.  Please choose another email.";
                    return errorMsg;
                }

                if (companyName.Length > 0 && newOrgID == -1 && createNewCompany == false)
                {
                    errorMsg = "The company you have specified is invalid.";
                    return errorMsg;
                }

                if (firstName.Length < 1 && lastName.Length < 1 && email.Length < 1 && newOrgID != -1)
                {
                    errorMsg = "The company you have specified is already in use.  Please choose another company name.";
                    return errorMsg;
                }
            }
            // Create Org or Use Unknown
            if (companyName.Length < 1)
            {
                newOrgID = orgService.GetIDByName("_Unknown Company");
            }
            else if (createNewCompany == true)
            {

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
                organization.NeedsIndexing = true;

                int? id = -1;
                organization.PrimaryUserID = id < 0 ? null : id;
                id = -1;
                organization.DefaultSupportUserID = id < 0 ? null : id;
                id = -1;
                organization.DefaultPortalGroupID = id < 0 ? null : id;
                id = -1;
                organization.DefaultSupportGroupID = id < 0 ? null : id;

                organization.TimeZoneID = "Dateline Standard Time";

                try
                {
                    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(organization.TimeZoneID);
                }
                catch (Exception)
                {
                }
                organization.Name = companyName.Trim();
                organization.Website = "";
                organization.CompanyDomains = "";
                organization.Description = "";
                organization.SAExpirationDate = DataUtils.DateToUtc(TSAuthentication.GetLoginUser(), null);
                organization.SlaLevelID = null;

                organization.Collection.Save();

                newOrgID = organization.OrganizationID;

                resultStr = "o" + organization.OrganizationID.ToString();
            }

            // Create User
            if (firstName.Length > 0 && lastName.Length > 0)
            {
                user = users.AddNewUser();
                user.OrganizationID = newOrgID;
                user.LastLogin = DateTime.UtcNow;
                user.LastActivity = DateTime.UtcNow.AddHours(-1);
                user.IsPasswordExpired = true;
                user.ReceiveTicketNotifications = true;
                user.Email = emailAddress.Trim();
                user.FirstName = firstName.Trim();
                user.LastName = lastName.Trim();
                user.Title = "";
                user.MiddleName = "";
                user.IsActive = true;
                user.ActivatedOn = DateTime.UtcNow;
                user.IsPortalUser = false;
                user.EnforceSingleSession = true;
                user.NeedsIndexing = true;
                user.Collection.Save();

                PhoneTypes phoneTypes = new PhoneTypes(TSAuthentication.GetLoginUser());
                phoneTypes.LoadByOrganizationID(TSAuthentication.OrganizationID);

					 if (phone != "")
					 {
						 PhoneNumber p = new PhoneNumbers(TSAuthentication.GetLoginUser()).AddNewPhoneNumber();
						 p.PhoneTypeID = phoneTypes[0].PhoneTypeID;
						 p.Number = phone;
						 p.RefType = ReferenceType.Users;
						 p.RefID = user.UserID;

						 p.Collection.Save();
					 }

                string password = DataUtils.GenerateRandomPassword();
                user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
                user.IsPasswordExpired = true;
                user.Collection.Save();

                resultStr = "u" + user.UserID.ToString();
            }

            return resultStr;
        }

        [WebMethod]
        public OrganizationProxy[] GetUserCustomers(int userID)
        {
          Organizations orgs = new Organizations(TSAuthentication.GetLoginUser());
          orgs.LoadByUserRights(userID);
          return orgs.GetOrganizationProxies();
        }

        [WebMethod]
        public OrganizationProxy[] RemoveUserCustomer(int userID, int organizationID)
        {

          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID == TSAuthentication.OrganizationID && TSAuthentication.IsSystemAdmin)
          {
            user.Collection.RemoveUserCustomer(userID, organizationID);
            Organizations orgs = new Organizations(TSAuthentication.GetLoginUser());
            orgs.LoadByUserRights(userID);
            return orgs.GetOrganizationProxies();
          }
          return null;

        }

        [WebMethod]
        public OrganizationProxy[] AddUserCustomer(int userID, int organizationID)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID == TSAuthentication.OrganizationID && TSAuthentication.IsSystemAdmin)
          {
            user.Collection.AddUserCustomer(userID, organizationID);
            Organizations orgs = new Organizations(TSAuthentication.GetLoginUser());
            orgs.LoadByUserRights(userID);
            return orgs.GetOrganizationProxies();
          }
          return null;
        }

        [WebMethod]
        public ProductFamilyProxy[] GetUserProductFamilies(int userID)
        {
            ProductFamilies productFamilies = new ProductFamilies(TSAuthentication.GetLoginUser());
            productFamilies.LoadByUserRights(userID);
            return productFamilies.GetProductFamilyProxies();
        }

        [WebMethod]
        public ProductFamilyProxy[] RemoveUserProductFamily(int userID, int productFamilyID)
        {

            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID == TSAuthentication.OrganizationID && TSAuthentication.IsSystemAdmin)
            {
                user.Collection.RemoveUserProductFamily(userID, productFamilyID);
                ProductFamilies productFamilies = new ProductFamilies(TSAuthentication.GetLoginUser());
                productFamilies.LoadByUserRights(userID);
                return productFamilies.GetProductFamilyProxies();
            }
            return null;

        }

        [WebMethod]
        public ProductFamilyProxy[] AddUserProductFamily(int userID, int productFamilyID)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID == TSAuthentication.OrganizationID && TSAuthentication.IsSystemAdmin)
            {
                user.Collection.AddUserProductFamily(userID, productFamilyID);
                ProductFamilies productFamilies = new ProductFamilies(TSAuthentication.GetLoginUser());
                productFamilies.LoadByUserRights(userID);
                return productFamilies.GetProductFamilyProxies();
            }
            return null;
        }

        [WebMethod]
        public CustomValueProxy[] GetCustomValues(int userID)
        {
            User users = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (users.OrganizationID != TSAuthentication.OrganizationID) return null;
            CustomValues values = new CustomValues(users.Collection.LoginUser);
            values.LoadByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Users, users.UserID);

            return values.GetCustomValueProxies();
        }

        [WebMethod]
        public AutocompleteItem[] AdminQueryUsers(int orgID, string query)
        {
          List<AutocompleteItem> result = new List<AutocompleteItem>();
          if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return result.ToArray();
          Users users = new Users(TSAuthentication.GetLoginUser());
          users.LoadByName(query, orgID, false, false, false);
          foreach (User user in users)
          {
            result.Add(new AutocompleteItem(string.Format("{0} - {1} ({2:D})", user.FirstLastName, user.Email, user.UserID) , user.UserID.ToString()));
          }

          return result.ToArray();
        }

        [WebMethod]
        public UserProxy[] AdminGetUsers(int orgID)
        {
          if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return null;
          Users users = new Users(TSAuthentication.GetLoginUser());
          users.LoadByOrganizationID(orgID, false);
          return users.GetUserProxies();
        }

        [WebMethod]
        public UserProxy AdminGetUser(int userID)
        {
          if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return null;
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
		  user.CryptedPassword = string.Empty;
          return user.GetProxy();
        }

		[WebMethod]
		public string AdminGetUserLogin(int userID)
		{
			if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return null;
			User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
			BackdoorLogins logins = new BackdoorLogins(TSAuthentication.GetLoginUser());
			BackdoorLogin login = logins.AddNewBackdoorLogin();
			login.UserID = TSAuthentication.GetLoginUser().UserID;
			login.ContactID = userID;
			logins.Save();
			return login.Token.ToString();
		}

        [WebMethod]
        public void SetSingleSessionEnforcement(int userID, bool value)
        {
          if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return;
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          user.EnforceSingleSession = value;
          user.Collection.Save();
        }

        [WebMethod]
        public void AdminSetActive(int userID, bool value)
        {
          if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return;
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          LogAdminUserAction(string.Format("set {0} ({1}) IsActive to {3}", user.FirstLastName, user.UserID.ToString(), "IsActive", value.ToString()));
          user.IsActive = value;
          user.Collection.Save();
        }

        public void LogAdminUserAction(string description)
        {
            User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, TSAuthentication.UserID, String.Format("{0} performed admin action: {1}", user.FirstLastName, description));
        }

        [WebMethod]
        public void AdminSetAdmin(int userID, bool value)
        {
            if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return;
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            LogAdminUserAction(string.Format("set {0} ({1}) IsActive to {3}", user.FirstLastName, user.UserID.ToString(), "IsSystemAdmin", value.ToString()));
            user.IsSystemAdmin = value;
            user.Collection.Save();
        }

        [WebMethod]
        public void AdminSetBilling(int userID, bool value)
        {
            if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return;
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            LogAdminUserAction(string.Format("set {0} ({1}) IsActive to {3}", user.FirstLastName, user.UserID.ToString(), "IsFinanceAdmin", value.ToString()));
            user.IsFinanceAdmin = value;
            user.Collection.Save();
        }

        public int GetIDByExactName(string name)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationNameActive(name, TSAuthentication.OrganizationID);
            if (organizations.IsEmpty) return -1;
            return organizations[0].OrganizationID;
        }

        [WebMethod]
        public string SetFontFamily(int userID, string value)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

          user.FontFamily = (FontFamily)Convert.ToInt16(value);
          user.Collection.Save();

          return Enums.GetDescription(user.FontFamily);
        }

        [WebMethod]
        public string SetFontSize(int userID, string value)
        {
          User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
          if (user.OrganizationID != TSAuthentication.OrganizationID) return value;

          user.FontSize = (FontSize)Convert.ToInt16(value);
          user.Collection.Save();

          return Enums.GetDescription(user.FontSize);
        }

        [WebMethod]
        public string GetShortNameFromID(int userID)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);

            if (user.FirstLastName.Length > 10)
                return user.FirstLastName.Substring(0, 10).ToString() + "...";
            else
                return user.FirstLastName.ToString();
        }


        [WebMethod]
        public string GetUsersSearch(int orgID, string query, bool showInactive)
        {
            Users users = new Users(TSAuthentication.GetLoginUser());

            if (query == "")
                users.LoadByOrganizationID(orgID, !showInactive);
            else
                users.LoadByName(query, orgID, !showInactive, false, false);

            StringBuilder html = new StringBuilder();

            foreach (UserProxy u in users.GetUserProxies())
            {
                ChatUserSetting setting = ChatUserSettings.GetChatUserSetting(TSAuthentication.GetLoginUser(), u.UserID);
                string chatsetting = "";
                string officesetting = "";

                Users tempUser = new Users(TSAuthentication.GetLoginUser());
                int total = tempUser.GetUserTicketCount(u.UserID, false);


                if (setting != null)
                {
                    chatsetting = setting.IsAvailable  && u.IsChatUser ? "<i class='user-tooltip fa-comments-o fa color-red' title='Customer Chat Online'></i>" : "";
                }

                if (!u.InOffice)
                    officesetting = string.Format("<span class='ts-icon ts-icon-offline-small fa fa-circle user-tooltip' title='Office Status {0}' userid='{1}'></span>", u.InOfficeComment, u.UserID);
                else
                    officesetting = string.Format("<span class='ts-icon ts-icon-online-small fa fa-circle user-tooltip' title='Office Status {0}' userid='{1}'></span>", u.InOfficeComment, u.UserID);


                html.AppendFormat(@"<li>
                    <div class='row'>
                        <div class='col-xs-2 pl0'>
                            <div class='avatar'>
                                <img id='userPhoto' src='{0}'>
                            </div>
                        </div>
                        <div class='col-xs-10'>
                            <strong><a class='user' uid='{5}'>{1} ({7}){6}</a></strong>
                            <div>{2}<div class='pull-right'>{3}{4}{8}</div></div>
                        </div>
                    </div></li>",
                                    u.Avatar,
                                    u.FirstName + " " + u.LastName,
                                    u.Title,
                                    u.AppChatStatus == true ? chatsetting : "",
                                    (u.AppChatStatus == true && u.UserID != TSAuthentication.GetLoginUser().UserID) ? "<i class='user-tooltip fa-comment fa color-green user-chat' cid='" + u.UserID + "' title='Online to Chat'></i>" : "",
                                    u.UserID,
                                    u.IsActive ? "" : "<i>(Inactive)</i>",
                                    total,
                                    officesetting
                                    );
            }

            return html.ToString();

        }


        [WebMethod]
        public string GetNewUserMessage()
        {
            int userCount = Organizations.GetUserCount(TSAuthentication.GetLoginUser(), TSAuthentication.GetLoginUser().OrganizationID);
            Organization organization = (Organization)Organizations.GetOrganization(TSAuthentication.GetLoginUser(), TSAuthentication.GetLoginUser().OrganizationID);
            string newusermessage="";

            User u = Users.GetUser(TSAuthentication.GetLoginUser(),TSAuthentication.GetLoginUser().UserID);

            if (organization.UserSeats <= userCount)
            {
                if (u.IsFinanceAdmin)
                {
                    newusermessage = "You have exceeded the number of user seat licenses.  If you would like to add additional users to your account, please contact our sales team at 800.596.2820 x806, or send an email to sales@teamsupport.com";
                }
                else
                {
                    Users users = new Users(TSAuthentication.GetLoginUser());
                    users.LoadFinanceAdmins(TSAuthentication.GetLoginUser().OrganizationID);
                    if (users.IsEmpty)
                    {
                        newusermessage = "Please ask your billing administrator to purchase additional user seat licenses.";
                    }
                    else
                    {
                        newusermessage = "Please ask your billing administrator (" + users[0].FirstLastName + ") to purchase additional user seat licenses.";
                    }
                }
            }
            else
            {
                newusermessage = "";
            }

            return newusermessage;
        }

        [WebMethod]
        public CalEvent[] GetCalendarEvents(string startdate, string pageType, string pageID)
        {
            JavaScriptSerializer serial = new JavaScriptSerializer();
            StringBuilder jsonString = new StringBuilder("");
            List<CalEvent> events = new List<CalEvent>();

            //PageType Enum
            // 0 = ticket
            // 1 = Product
            // 2 = Company
            // 4 = group

            var testDate = DateTime.Parse(startdate);
            if (testDate.Day != 1) {
                testDate = testDate.AddMonths(1);
                startdate = testDate.ToString();
            }

            var offset = TSAuthentication.GetLoginUser().TimeZoneInfo.GetUtcOffset(DateTime.UtcNow).Hours;
            Tickets tickets = new Tickets(TSAuthentication.GetLoginUser());
            ////get all due dates for the current month
            if (pageType == "0" || pageType == "-1") {
                tickets.LoadbyUserMonth(DateTime.Parse(startdate), TSAuthentication.GetLoginUser().UserID, TSAuthentication.GetLoginUser().OrganizationID);
            } else if (pageType == "4") {
                tickets.LoadbyGroupMonth(DateTime.Parse(startdate), offset, int.Parse(pageID), TSAuthentication.GetLoginUser().OrganizationID);
            } else if (pageType == "2") {
                tickets.LoadbyCompanyMonth(DateTime.Parse(startdate), int.Parse(pageID), TSAuthentication.GetLoginUser().OrganizationID);
            }

            foreach (Ticket t in tickets)
            {
                CalEvent cal = new CalEvent();
                cal.color = "red";
                DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc((DateTime)t.DueDateUtc, TSAuthentication.GetLoginUser().TimeZoneInfo);
                cal.start = ((DateTime)t.DueDate).ToString("o");
                cal.title = t.Name;
                cal.type = "ticket";
                cal.id = t.TicketNumber;
                cal.description = "";
                cal.end = null;
                cal.displayend = null;
                cal.allday = false;
                cal.creatorID = -1;

                Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
                organizations.LoadBTicketID(t.TicketID);
                User user = Users.GetUser(TSAuthentication.GetLoginUser(), TSAuthentication.UserID);

                if(organizations.Count > 0)
                {
                    List<CalendarRefItemProxy> calendarreferences = new List<CalendarRefItemProxy>();
                    foreach (Organization organization in organizations)
                    {
                        CalendarRefItemProxy prox = new CalendarRefItemProxy();
                        prox.displayName = organization.Name;
                        prox.RefType = 2;
                        prox.RefID = organization.OrganizationID;

                        if (user.TicketRights == TicketRightType.Customers)
                        {
                            if (CheckUserRights(TSAuthentication.UserID, prox.RefID))
                            {
                                prox.displayName = GetDisplayname(prox);
                                if (prox.displayName != "")
                                    calendarreferences.Add(prox);
                            }
                        }
                        else
                            calendarreferences.Add(prox);
                    }
                    cal.references = calendarreferences.OrderBy(a => a.displayName).ToArray();
                }
                else
                    cal.references = null;


                events.Add(cal);
            }


            //get all reminders for this user for the current month
            Reminders reminders = new Reminders(TSAuthentication.GetLoginUser());
            string tempPageType;
            string tempPageID;

            switch (pageType)
            {
                case "0":
                    tempPageType = "17";
                    Ticket ticket = Tickets.GetTicketByNumber(TSAuthentication.GetLoginUser(), int.Parse(pageID));
                    tempPageID = ticket.TicketID.ToString();
                    break;
                case "2":
                    tempPageType = "9";
                    tempPageID = pageID;
                    break;
                default:
                    tempPageType = pageType;
                    tempPageID = pageID;
                    break;
            }

            reminders.LoadByUserMonth(DateTime.Parse(startdate), TSAuthentication.GetLoginUser().UserID, tempPageType, tempPageID);
            foreach (Reminder r in reminders)
            {
                CalEvent cal = new CalEvent();
                cal.color = "blue";

                cal.title = r.Description;
                cal.start = ((DateTime)r.GetProxy().DueDate).ToString("o");
                cal.end = null;
                cal.displayend = null;
                cal.allday = false;
                cal.references = null;
                cal.creatorID = -1;
                switch (r.RefType)
                {
                    case ReferenceType.Tickets:
                        Ticket t = Tickets.GetTicket(TSAuthentication.GetLoginUser(), r.RefID);
                        if (t != null)
                        {
                            cal.type = "reminder-ticket";
                            cal.id = t.TicketNumber;
                            cal.description = t.Name;
                            break;
                        }
                        else
                            continue;

                    case ReferenceType.Organizations:
                        Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), r.RefID);
                        if (o != null)
                        {
                            cal.type = "reminder-org";
                            cal.id = o.OrganizationID;
                            cal.description = o.Name;
                            break;
                        }
                        else
                            continue;

                    case ReferenceType.Contacts:
                        User u = Users.GetUser(TSAuthentication.GetLoginUser(), r.RefID);
                        if (u != null)
                        {
                            cal.id = u.UserID;
                            cal.type = "reminder-user";
                            cal.description = u.FirstLastName;
                            break;
                        }
                        else
                            continue;
                    case ReferenceType.Tasks:
                        Task ta = Tasks.GetTask(TSAuthentication.GetLoginUser(), r.RefID);
                        if (ta != null)
                        {
                            cal.id = ta.TaskID;
                            cal.type = "reminder-task";
                            cal.description = ta.Name;
                            break;
                        }
                        else
                            continue;
                }


                events.Add(cal);
            }
            ////get all custom events for the month
            CalendarEvents customEvents = new CalendarEvents(TSAuthentication.GetLoginUser());
            customEvents.LoadbyMonth(DateTime.Parse(startdate), TSAuthentication.GetLoginUser().OrganizationID, pageType, pageID, TSAuthentication.GetLoginUser().UserID);
            foreach (CalendarEvent c in customEvents)
            {
                CalEvent cal = new CalEvent();
                cal.color = "green";

                //if (c.AllDay)
                //{
                //    if(c.StartDateUTC != null)
                //        cal.start = DateTime.SpecifyKind((DateTime)c.StartDateUTCUtc, DateTimeKind.Utc).ToString("o").Substring(0, 10);
                //    else
                //        cal.start = ((DateTime)c.StartDateUtc).ToString("o").Substring(0, 10);
                //}
                //else
                //{
                    if (c.StartDateUTC != null)
                        cal.start = DateTime.SpecifyKind((DateTime)c.StartDateUTCUtc, DateTimeKind.Utc).ToString("o");
                    else
                        cal.start = ((DateTime)c.StartDateUtc).ToString("o");
                //}


                if (c.EndDateUTC != null)
                    cal.end = DateTime.SpecifyKind((DateTime)c.EndDateUTCUtc, DateTimeKind.Utc).ToString("o");
                else
                    cal.end = c.EndDateUtc == null ? null : ((DateTime)c.EndDateUtc).ToString("o");


                if (c.AllDay)
                {
                    if(c.EndDateUTC != null)
                    {
                        TimeSpan ts = new TimeSpan(23, 59, 0);
                        cal.displayend = (((DateTime)c.EndDateUTC).Date + ts).ToString("o");
                    }
                    else
                        cal.displayend = c.EndDateUtc == null ? null : ((DateTime)c.EndDateUtc).AddDays(1).ToString("o");
                }
                else
                {
                    if (c.EndDateUTC != null)
                        cal.displayend = DateTime.SpecifyKind((DateTime)c.EndDateUTCUtc, DateTimeKind.Utc).ToString("o");
                    else
                        cal.displayend = c.EndDateUtc == null ? null : ((DateTime)c.EndDateUtc).ToString("o");
                }

                cal.title = c.Title;
                cal.type = "cal";
                cal.id = c.CalendarID;
                cal.description = c.Description;
                cal.allday = c.AllDay;
                cal.isHoliday = c.IsHoliday;
                cal.creatorID = c.CreatorID;

                User user = Users.GetUser(TSAuthentication.GetLoginUser(), TSAuthentication.UserID);

                CalendarRef calRef = new CalendarRef(TSAuthentication.GetLoginUser());
                calRef.LoadByCalendarID(c.CalendarID);
                if (calRef.Count > 0)
                {
                    List<CalendarRefItemProxy> calendarreferences = new List<CalendarRefItemProxy>();
                    foreach (CalendarRefItem calitem in calRef)
                    {
                        CalendarRefItemProxy prox = calitem.GetProxy();
                        if (user.TicketRights == TicketRightType.Customers && prox.RefType == 2)
                        {
                            if (CheckUserRights(TSAuthentication.UserID, prox.RefID))
                            {
                                prox.displayName = GetDisplayname(prox);
                                if (prox.displayName != "")
                                    calendarreferences.Add(prox);
                            }
                        }
                        else
                        {
                            prox.displayName = GetDisplayname(prox);
                            if (prox.displayName != "")
                                calendarreferences.Add(prox);
                        }


                    }
                    cal.references = calendarreferences.OrderBy(a => a.displayName).ToArray();
                }
                else
                    cal.references = null;

                events.Add(cal);
            }

            Tasks tasks = new Tasks(TSAuthentication.GetLoginUser());
            ////get all due dates for the current month
            if (pageType == "0" || pageType == "-1")
            {
                tasks.LoadbyUserMonth(DateTime.Parse(startdate), TSAuthentication.GetLoginUser().UserID, TSAuthentication.GetLoginUser().OrganizationID);
            }
            else if (pageType == "4")
                tasks.LoadbyGroupMonth(DateTime.Parse(startdate), int.Parse(pageID), TSAuthentication.GetLoginUser().OrganizationID);
            else if (pageType == "2")
                tasks.LoadbyCompanyMonth(DateTime.Parse(startdate), int.Parse(pageID), TSAuthentication.GetLoginUser().OrganizationID);

            foreach (Task task in tasks)
            {
                CalEvent cal = new CalEvent();
                cal.color = "purple";
                DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc((DateTime)task.DueDateUtc, TSAuthentication.GetLoginUser().TimeZoneInfo);
                cal.start = ((DateTime)task.DueDate).ToString("o");
                cal.title = task.Name;
                cal.description = task.Name;
                cal.type = "task";
                cal.id = task.TaskID;
                cal.end = null;
                cal.displayend = null;
                cal.allday = false;

                cal.creatorID = -1;

                //Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
                //organizations.LoadByTaskID(task.TaskID);
                //User user = Users.GetUser(TSAuthentication.GetLoginUser(), TSAuthentication.UserID);

                //if (organizations.Count > 0)
                //{
                //    List<CalendarRefItemProxy> calendarreferences = new List<CalendarRefItemProxy>();
                //    foreach (Organization organization in organizations)
                //    {
                //        CalendarRefItemProxy prox = new CalendarRefItemProxy();
                //        prox.displayName = organization.Name;
                //        prox.RefType = 2;
                //        prox.RefID = organization.OrganizationID;

                //        if (user.TicketRights == TicketRightType.Customers)
                //        {
                //            if (CheckUserRights(TSAuthentication.UserID, prox.RefID))
                //            {
                //                prox.displayName = GetDisplayname(prox);
                //                if (prox.displayName != "")
                //                    calendarreferences.Add(prox);
                //            }
                //        }
                //        else
                //            calendarreferences.Add(prox);
                //    }
                //    cal.references = calendarreferences.OrderBy(a => a.displayName).ToArray();
                //}
                //else
                //    cal.references = null;


                events.Add(cal);
            }

            return events.ToArray();
        }

        public bool CheckUserRights(int userID, int companyID)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
            SELECT COUNT(*)
            FROM UserRightsOrganizations
            WHERE UserID = @UserID
            AND OrganizationID = @OrganizationID
            ";
            command.Parameters.AddWithValue("UserID", userID);
            command.Parameters.AddWithValue("OrganizationID", companyID);
            return SqlExecutor.ExecuteInt(TSAuthentication.GetLoginUser(), command) > 0;
        }

        public string GetDisplayname(CalendarRefItemProxy calproxy)
        {
            string Displayname = "";

            switch (calproxy.RefType)
            {
                case 0:
                    Ticket t = Tickets.GetTicketByNumber(TSAuthentication.GetLoginUser(), calproxy.RefID);
                    if(t == null)
                        Displayname = "";
                    else
                        Displayname = t.Name;

                    break;
                case 1:
                    Product p = Products.GetProduct(TSAuthentication.GetLoginUser(), calproxy.RefID);
                    if(p == null)
                        Displayname = "";
                    else
                        Displayname = p.Name;
                    break;
                case 2:
                    Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), calproxy.RefID);
                    if (o == null)
                        Displayname = "";
                    else
                        Displayname = o.Name;
                    break;
                case 3:
                    Displayname = Users.GetUserFullName(TSAuthentication.GetLoginUser(), calproxy.RefID);
                    break;
                case 4:
                    Group g = Groups.GetGroup(TSAuthentication.GetLoginUser(), calproxy.RefID);
                    if (g == null)
                        Displayname = "";
                    else
                        Displayname = g.Name;
                    break;
            }

            return Displayname;
        }

        [WebMethod]
        public void DeleteCalEvent(int eventid)
        {
            CalendarEvent c = CalendarEvents.GetCalendarEvent(TSAuthentication.GetLoginUser(), eventid);
            if (c.OrganizationID != TSAuthentication.OrganizationID) return;
            //if (!TSAuthentication.IsSystemAdmin) return;
            c.Delete();
            c.Collection.Save();
        }

        [WebMethod]
        public void AddAttachment(int calendarID, int attachmentID, CalendarAttachmentType attachmentType)
        {
            try
            {
                CalendarRefItem calAttachment = (new CalendarRef(TSAuthentication.GetLoginUser()).AddNewCalendarRefItem());
                calAttachment.CalendarID = calendarID;
                calAttachment.RefID = attachmentID;
                calAttachment.RefType = (int)attachmentType;
                calAttachment.Collection.Save();
            }
            catch (Exception e)
            {

            }
        }

        [WebMethod]
        public bool SaveCalendarEvent(string data)
        {
            CalendarJsonInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<CalendarJsonInfo>(data);
            DateTime dt;
            DateTimeOffset dto;
            if (info.id != -1)
            {
                CalendarEvent cal = CalendarEvents.GetCalendarEvent(TSAuthentication.GetLoginUser(), info.id);
                if (info.start == null)
                    return false;

                var startDate = (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(info.start));
                cal.StartDate = startDate;
                cal.StartDateUTC = startDate;

                if (info.end != null)
                {
                    var endDate = (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(info.end));

                    if (info.allDay)
                    {
                        cal.EndDate = endDate.AddHours(22).AddMinutes(59);
                        cal.EndDateUTC = endDate.AddHours(22).AddMinutes(59);
                    }
                    else
                    {
                        cal.EndDate = endDate;
                        cal.EndDateUTC = endDate;
                    }
                }
                cal.Title = info.title;
                cal.Description = info.description;
                cal.LastModified = DateTime.Now;
                cal.AllDay = info.allDay;
                cal.IsHoliday = info.isHoliday;
                cal.Collection.Save();

                CalendarRef calRef = new CalendarRef(TSAuthentication.GetLoginUser());
                calRef.LoadByCalendarID(cal.CalendarID);

                //Delete ticket associations that no longer exist
                foreach (CalendarRefItem item in calRef.Where(a => a.RefType == (int)CalendarAttachmentType.Ticket))
                {
                    bool delete = true;
                    foreach(int ticketID in info.Tickets)
                    {
                        if(ticketID == item.RefID)
                        {
                            delete = false;
                            break;
                        }
                    }
                    if(delete)
                    {
                        item.Delete();
                        item.Collection.Save();
                    }
                }
                //Delete product associations that no longer exist
                foreach (CalendarRefItem item in calRef.Where(a => a.RefType == (int)CalendarAttachmentType.Product).ToArray())
                {
                    bool delete = true;
                    foreach (int productID in info.Products)
                    {
                        if (productID == item.RefID)
                        {
                            delete = false;
                            break;
                        }
                    }
                    if (delete)
                    {
                        item.Delete();
                        item.Collection.Save();
                    }
                }
                //Delete company associations that no longer exist
                foreach (CalendarRefItem item in calRef.Where(a => a.RefType == (int)CalendarAttachmentType.Company).ToArray())
                {
                    bool delete = true;
                    foreach (int companyID in info.Company)
                    {
                        if (companyID == item.RefID)
                        {
                            delete = false;
                            break;
                        }
                    }
                    if (delete)
                    {
                        item.Delete();
                        item.Collection.Save();
                    }
                }
                //Delete group associations that no longer exist
                foreach (CalendarRefItem item in calRef.Where(a => a.RefType == (int)CalendarAttachmentType.Group).ToArray())
                {
                    bool delete = true;
                    foreach (int groupID in info.Groups)
                    {
                        if (groupID == item.RefID)
                        {
                            delete = false;
                            break;
                        }
                    }
                    if (delete)
                    {
                        item.Delete();
                        item.Collection.Save();
                    }
                }
                //Delete user associations that no longer exist
                foreach (CalendarRefItem item in calRef.Where(a => a.RefType == (int)CalendarAttachmentType.User).ToArray())
                {
                    bool delete = true;
                    foreach (int userID in info.User)
                    {
                        if (userID == item.RefID)
                        {
                            delete = false;
                            break;
                        }
                    }
                    if (delete)
                    {
                        item.Delete();
                        item.Collection.Save();
                    }
                }

                foreach (int ticketID in info.Tickets)
                {

                    AddAttachment(cal.CalendarID, ticketID, CalendarAttachmentType.Ticket);
                }

                if (info.PageType == 1)
                    AddAttachment(cal.CalendarID, info.PageID, CalendarAttachmentType.Product);
                foreach (int productID in info.Products)
                {
                    AddAttachment(cal.CalendarID, productID, CalendarAttachmentType.Product);
                }

                if (info.PageType == 2)
                    AddAttachment(cal.CalendarID, info.PageID, CalendarAttachmentType.Company);
                foreach (int CompanyID in info.Company)
                {
                    AddAttachment(cal.CalendarID, CompanyID, CalendarAttachmentType.Company);
                }

                if (info.PageType == 4)
                    AddAttachment(cal.CalendarID, info.PageID, CalendarAttachmentType.Group);
                foreach (int groupID in info.Groups)
                {
                    AddAttachment(cal.CalendarID, groupID, CalendarAttachmentType.Group);
                }

                foreach (int UserID in info.User)
                {
                    AddAttachment(cal.CalendarID, UserID, CalendarAttachmentType.User);
                }


            }
            else
            {
                if (info.start == null)
                    return false;

                CalendarEvent cal = (new CalendarEvents(TSAuthentication.GetLoginUser()).AddNewCalendarEvent());
                var startDate = (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(info.start));
                cal.StartDate = startDate;
                cal.StartDateUTC = startDate;

                if (info.end != null)
                {
                    var endDate = (new DateTime(1970, 1, 1)).AddMilliseconds(double.Parse(info.end));

                    if (info.allDay)
                    {
                        cal.EndDate = endDate.AddHours(22).AddMinutes(59);
                        cal.EndDateUTC = endDate.AddHours(22).AddMinutes(59);
                    }
                    else
                    {
                        cal.EndDate = endDate;
                        cal.EndDateUTC = endDate;
                    }
                }


                cal.OrganizationID = TSAuthentication.GetLoginUser().OrganizationID;
                cal.Title = info.title;
                cal.Description = info.description;
                cal.LastModified = DateTime.Now;
                cal.CreatorID = TSAuthentication.GetLoginUser().UserID;
                cal.AllDay = info.allDay;
                cal.IsHoliday = info.isHoliday;
                cal.Collection.Save();

                if(info.PageType == 0)
                    AddAttachment(cal.CalendarID, info.PageID, CalendarAttachmentType.Ticket);
                foreach (int ticketID in info.Tickets)
                {
                    AddAttachment(cal.CalendarID, ticketID, CalendarAttachmentType.Ticket);
                }

                if (info.PageType == 1)
                    AddAttachment(cal.CalendarID, info.PageID, CalendarAttachmentType.Product);
                foreach (int productID in info.Products)
                {
                    AddAttachment(cal.CalendarID, productID, CalendarAttachmentType.Product);
                }

                if (info.PageType == 2)
                    AddAttachment(cal.CalendarID, info.PageID, CalendarAttachmentType.Company);
                foreach (int CompanyID in info.Company)
                {
                    AddAttachment(cal.CalendarID, CompanyID, CalendarAttachmentType.Company);
                }

                if (info.PageType == 4)
                    AddAttachment(cal.CalendarID, info.PageID, CalendarAttachmentType.Group);
                foreach (int groupID in info.Groups)
                {
                    AddAttachment(cal.CalendarID, groupID, CalendarAttachmentType.Group);
                }

                foreach (int UserID in info.User)
                {
                    AddAttachment(cal.CalendarID, UserID, CalendarAttachmentType.User);
                }

            }

            return true;


        }

        [WebMethod]
        public void ChangeEventDate(int eventID, DateTime newTime, DateTime endTime, string eventType, bool allDay)
        {
            switch (eventType)
            {
                case "ticket":
                    Tickets ticket = new Tickets(TSAuthentication.GetLoginUser());
                    ticket.LoadByTicketNumber(TSAuthentication.GetLoginUser().OrganizationID, eventID);
                    ticket[0].DueDate = TimeZoneInfo.ConvertTimeToUtc(newTime);
                    ticket[0].Collection.Save();

                    break;
                case "cal":
                    CalendarEvents events = new CalendarEvents(TSAuthentication.GetLoginUser());
                    DateTimeOffset dto;
                        //DateTimeOffset.TryParse(info.start, null as IFormatProvider,
                        //       DateTimeStyles.AdjustToUniversal,
                        //       out dto);
                    events.LoadByCalendarID(eventID);
                    events[0].StartDate = newTime;
                    events[0].StartDateUTC = newTime.ToUniversalTime();
                    if (allDay)
                    {
                        TimeSpan ts = new TimeSpan(23, 59, 0);
                        events[0].EndDate = endTime.Date.AddDays(-1) + ts;
                        events[0].EndDateUTC = (endTime.Date.AddDays(-1) + ts).ToUniversalTime();
                    }
                    else
                    {
                        events[0].EndDate = endTime;
                        events[0].EndDateUTC = endTime.ToUniversalTime();
                    }
                    events[0].Collection.Save();
                    break;
                default:
                    Reminders reminders = new Reminders(TSAuthentication.GetLoginUser());

                    User user = Users.GetUser(TSAuthentication.GetLoginUser(), TSAuthentication.GetLoginUser().UserID);

                    if (eventType == "reminder-ticket")
                    {
                        ticket = new Tickets(TSAuthentication.GetLoginUser());
                        ticket.LoadByTicketNumber(TSAuthentication.GetLoginUser().OrganizationID, eventID);
                        reminders.LoadByItem(ReferenceType.Tickets, ticket[0].TicketID, TSAuthentication.GetLoginUser().UserID);
                    }
                    if (eventType == "reminder-org")
                        reminders.LoadByItem(ReferenceType.Organizations, eventID, TSAuthentication.GetLoginUser().UserID);
                    if (eventType == "reminder-user")
                        reminders.LoadByItem(ReferenceType.Contacts, eventID, TSAuthentication.GetLoginUser().UserID);

                    //reminders[0].DueDateUtc = TimeZoneInfo.ConvertTimeToUtc(newTime);
                    reminders[0].DueDate = TimeZoneInfo.ConvertTimeToUtc(newTime);
                    reminders[0].Collection.Save();
                    break;
            }
            //CalendarEvents cal;
            //cal.LoadByCalendarID(data);
            //cal[0].StartDate = "a";
            //cal[0].Collection.Save();

        }

        [WebMethod]
        public UserProductProxy GetUserProduct(int userProductID)
        {
          UserProduct op = UserProducts.GetUserProduct(TSAuthentication.GetLoginUser(), userProductID);
          return op.GetProxy();
        }

        [DataContract]
        public class CalEvent
        {
            [DataMember]
            public int id { get; set; }
            [DataMember]
            public int creatorID { get; set; }
            [DataMember]
            public string title { get; set; }
            [DataMember]
            public string start { get; set; }
            [DataMember]
            public string end { get; set; }
            [DataMember]
            public string displayend { get; set; }
            [DataMember]
            public string color { get; set; }
            [DataMember]
            public string type { get; set; }
            [DataMember]
            public string description { get; set; }
            [DataMember]
            public bool allday { get; set; }
            [DataMember]
            public bool isHoliday { get; set; }
            [DataMember]
            public CalendarRefItemProxy[] references { get; set; }

        }


      [DataContract]
        public class BasicUser
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public int UserID { get; set; }
            [DataMember]
            public string InOfficeComment { get; set; }
        }

      public class CalendarJsonInfo
      {
          public CalendarJsonInfo() { }
          [DataMember]
          public int id { get; set; }
          [DataMember]
          public string title { get; set; }
          [DataMember]
          public string start { get; set; }
          [DataMember]
          public string end { get; set; }
          [DataMember]
          public bool allDay { get; set; }
          [DataMember]
          public bool isHoliday { get; set; }
          [DataMember]
          public string description { get; set; }
          [DataMember]
          public List<int> Tickets { get; set; }
          [DataMember]
          public List<int> Groups { get; set; }
          [DataMember]
          public List<int> Products { get; set; }
          [DataMember]
          public List<int> Company { get; set; }
          [DataMember]
          public List<int> User { get; set; }
          [DataMember]
          public int PageType { get; set; }
          [DataMember]
          public int PageID { get; set; }
      }

        [WebMethod]
        public string PullSettings(int userID) {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            return TeamSupport.Data.UserSettings.PullSettings(loginUser, userID);
        }

        [WebMethod]
        public string UpdateSetting(string key, string value, string category = "general") {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            return TeamSupport.Data.UserSettings.UpdateSetting(loginUser, key, value, category);
        }

    }
}
