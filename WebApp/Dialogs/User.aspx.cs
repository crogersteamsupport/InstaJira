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
using TeamSupport.WebUtils;
using TeamSupport.Data;
using Telerik.Web.UI;
using System.Globalization;

public partial class Dialogs_User : BaseDialogPage
{
    private int _userID = -1;
    private int _organizationID = -1;

    private CustomFieldControls _fieldControls;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (Request["UserID"] != null)
        {
            _userID = int.Parse(Request["UserID"]);
        }

        if (Request["OrganizationID"] != null)
        {
            _organizationID = int.Parse(Request["OrganizationID"]);
        }

        if (!UserSession.CurrentUser.IsSystemAdmin && _userID != UserSession.LoginUser.UserID)
        {
            Response.Write("");
            Response.End();
            return;
        }

        _fieldControls = new CustomFieldControls(ReferenceType.Users, -1, 2, tblCustomControls, false);
        _fieldControls.LoadCustomControls(200);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);


        Organization organization = (Organization)Organizations.GetOrganization(UserSession.LoginUser, _organizationID);

        if (organization.OrganizationID != UserSession.LoginUser.OrganizationID && organization.ParentID != UserSession.LoginUser.OrganizationID)
        {
            Response.Write("Invalid Request");
            Response.End();
            return;
        }

        if (!IsPostBack)
        {
            LoadTimeZones();
            LoadDateFormats();
            LoadFontFamilies(organization.FontFamily);
            LoadFontSizes(organization.FontSize);
            textEmail.Text = "";
            textFirstName.Text = "";
            textLastName.Text = "";
            cbActive.Checked = true;
            cbIsSystemAdmin.Checked = false;
            cbChat.Checked = false;
            cbEmailNotify.Checked = true;
            cbSubscribe.Checked = false;
            cbSubscribeAction.Checked = false;
            cbReceiveGroup.Checked = false;
            cbNoAutoSubscribe.Checked = false;
            cbRestrictUserFromEditingAnyActions.Checked = false;
            cbAllowUserToEditAnyAction.Checked = false;
            cbUserCanPinAction.Checked = true;
            Page.Title = "New User";

            if (!string.IsNullOrEmpty(organization.TimeZoneID))
            {
                cmbTimeZones.SelectedValue = organization.TimeZoneID;
            }

            cmbRights.Items.Add(new RadComboBoxItem("All tickets", "0"));
            cmbRights.Items.Add(new RadComboBoxItem("Only assigned tickets", "1"));
            cmbRights.Items.Add(new RadComboBoxItem("Only assigned and user's groups", "2"));
            cmbRights.Items.Add(new RadComboBoxItem("Only assigned and tickets associated with specific customers", "3"));

            if (_userID > -1) LoadUser(_userID);
            cbEmail.Visible = _userID < 0;
            _fieldControls.RefID = _userID;
            _fieldControls.LoadValues();
            Page.RegisterStartupScript("AddMasks", @"
        <script type=""text/javascript"">
          $('.masked').each(function (index) {
            $(this).mask($(this).attr('placeholder'));
          });
        </script>
      ");
        }


        btnReset.Visible = UserSession.CurrentUser.IsSystemAdmin && _userID != UserSession.LoginUser.UserID && (_userID > -1);
        pnlChange.Visible = _userID == UserSession.LoginUser.UserID;
        cbActive.Visible = tdActive.Visible = trAdmin.Visible = UserSession.CurrentUser.IsSystemAdmin;
        tdChatLabel.Visible = UserSession.CurrentUser.HasChatRights;
        cbChat.Visible = tdChatLabel.Visible;
    }

    private void LoadTimeZones()
    {
        cmbTimeZones.Items.Clear();

        System.Collections.ObjectModel.ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
        foreach (TimeZoneInfo info in timeZones)
        {
            cmbTimeZones.Items.Add(new RadComboBoxItem(info.DisplayName, info.Id));
        }
    }

    private void LoadDateFormats()
    {
        cmbDateFormat.Items.Clear();

        foreach (CultureInfo info in CultureInfo.GetCultures(CultureTypes.AllCultures))
        {
            if (!info.IsNeutralCulture) cmbDateFormat.Items.Add(new RadComboBoxItem(info.DisplayName, info.Name));
        }
        cmbDateFormat.Sort = RadComboBoxSort.Ascending;
        cmbDateFormat.SortItems();
        cmbDateFormat.SelectedValue = "en-US";

    }

    private void LoadFontFamilies(FontFamily selectedValue)
    {
        cmbFontFamilies.Items.Clear();

        Array fontFamilyValues = Enum.GetValues(typeof(FontFamily));
        int x = 0;
        foreach (FontFamily value in fontFamilyValues)
        {
            x = (int)value;
            cmbFontFamilies.Items.Add(new RadComboBoxItem(Enums.GetDescription(value), x.ToString()));
        }
        cmbFontFamilies.SelectedValue = Convert.ToInt32(selectedValue).ToString();
    }

    private void LoadFontSizes(TeamSupport.Data.FontSize selectedValue)
    {
        cmbFontSizes.Items.Clear();
        Array fontSizeValues = Enum.GetValues(typeof(TeamSupport.Data.FontSize));
        int x = 0;
        foreach (TeamSupport.Data.FontSize value in fontSizeValues)
        {
            x = (int)value;
            cmbFontSizes.Items.Add(new RadComboBoxItem(Enums.GetDescription(value), x.ToString()));
        }
        cmbFontSizes.SelectedValue = Convert.ToInt32(selectedValue).ToString();
    }


    private void LoadUser(int userID)
    {
        Users users = new Users(UserSession.LoginUser);
        users.LoadByUserID(userID);
        if (!users.IsEmpty)
        {
            Page.Title = "User - " + users[0].FirstName + " " + users[0].LastName;

            textEmail.Text = users[0].Email;
            textFirstName.Text = users[0].FirstName;
            textLastName.Text = users[0].LastName;
            textMiddleName.Text = users[0].MiddleName;
            textTitle.Text = users[0].Title;
            cbActive.Checked = users[0].IsActive;
            cbIsSystemAdmin.Checked = users[0].IsSystemAdmin;
            cbChat.Checked = users[0].IsChatUser;
            cbEmailNotify.Checked = users[0].ReceiveTicketNotifications;
            cbSubscribe.Checked = users[0].SubscribeToNewTickets;
            cbSubscribeAction.Checked = users[0].SubscribeToNewActions;
            cbReceiveGroup.Checked = users[0].ReceiveAllGroupNotifications;
            cbNoAutoSubscribe.Checked = users[0].DoNotAutoSubscribe;
            cbRestrictUserFromEditingAnyActions.Checked = users[0].RestrictUserFromEditingAnyActions;
            cbAllowUserToEditAnyAction.Checked = users[0].AllowUserToEditAnyAction;
            cbUserCanPinAction.Checked = users[0].UserCanPinAction;

            cmbTimeZones.SelectedValue = "Central Standard Time";

            if (string.IsNullOrEmpty(users[0].TimeZoneID))
            {
                Organization organization = Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
                if (!string.IsNullOrEmpty(organization.TimeZoneID))
                {
                    cmbTimeZones.SelectedValue = organization.TimeZoneID;
                }
            }
            else
            {
                cmbTimeZones.SelectedValue = users[0].TimeZoneID;
            }

            if (string.IsNullOrEmpty(users[0].CultureName))
            {
                Organization organization = Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
                if (!string.IsNullOrEmpty(organization.CultureName))
                {
                    cmbDateFormat.SelectedValue = organization.CultureName;
                }
            }
            else
            {
                cmbDateFormat.SelectedValue = users[0].CultureName;
            }

            cmbRights.SelectedValue = ((int)users[0].TicketRights).ToString();
        }
    }

    public override bool Save()
    {
        User user;
        Users users = new Users(UserSession.LoginUser);

        Organization organization = Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);

        string email = textEmail.Text.Trim();
        if (email.Length < 1 || email.IndexOf('@') < 0 || email.IndexOf('.') < 0 || !users.IsEmailValid(email, _userID, _organizationID))
        {
            _manager.Alert("The email you have specified is invalid.  Please choose another email.");
            return false;
        }

        if (textFirstName.Text.Trim().Length < 1 || textLastName.Text.Trim().Length < 1)
        {
            _manager.Alert("The name you have specified is invalid.  Please enter your name.");
            return false;
        }


        if (cbChat.Checked && Organizations.GetChatCount(UserSession.LoginUser, UserSession.LoginUser.OrganizationID) >= organization.ChatSeats)
        {
            _manager.Alert("You have exceeded your chat licenses.  Please purchase more seats to add additional chat users.");
            cbChat.Checked = false;
            return false;
        }

        if (_userID < 0)
        {
            if (Organizations.GetUserCount(UserSession.LoginUser, UserSession.LoginUser.OrganizationID) >= organization.UserSeats)
            {
                _manager.Alert("You have exceeded your user licenses.  Please send an email to sales@teamsupport.com if you would like to add additional users your account.");
                return false;
            }

            user = users.AddNewUser();
            user.OrganizationID = _organizationID;
            if (cbActive.Checked) user.ActivatedOn = DateTime.UtcNow;
            user.LastLogin = DateTime.UtcNow;
            user.LastActivity = DateTime.UtcNow;
            user.IsPasswordExpired = true;
            user.ReceiveTicketNotifications = true;
            user.EnforceSingleSession = true;
            user.IsClassicView = true;
        }
        else
        {
            users.LoadByUserID(_userID);
            if (users.IsEmpty)
            {
                _manager.Alert("There was an error updating the user information");
                return false;
            }

            user = users[0];
            if (user.IsActive && !cbActive.Checked)
            {
                user.DeactivatedOn = DateTime.UtcNow;
                Organizations orgs = new Organizations(TSAuthentication.GetLoginUser());
                orgs.ResetDefaultSupportUser(TSAuthentication.GetLoginUser(), user.UserID);
            }

            if (true)
            {
                if (!user.IsActive && cbActive.Checked && Organizations.GetUserCount(UserSession.LoginUser, UserSession.LoginUser.OrganizationID) >= organization.UserSeats)
                {
                    _manager.Alert("You have exceeded your user licenses.  Please send an email to sales@teamsupport.com if you would like to add additional users your account.");
                    return false;
                }
            }
        }

        user.Email = textEmail.Text.Trim();
        user.FirstName = textFirstName.Text.Trim();
        user.LastName = textLastName.Text.Trim();
        user.MiddleName = textMiddleName.Text.Trim();
        user.Title = textTitle.Text;
        user.IsPortalUser = true;
        user.TimeZoneID = cmbTimeZones.SelectedValue;
        user.CultureName = cmbDateFormat.SelectedValue;
        user.FontFamily = (FontFamily)Convert.ToInt32(cmbFontFamilies.SelectedValue);
        user.FontSize = (TeamSupport.Data.FontSize)Convert.ToInt32(cmbFontSizes.SelectedValue);
        user.ReceiveTicketNotifications = cbEmailNotify.Checked;
        user.SubscribeToNewTickets = cbSubscribe.Checked;
        user.SubscribeToNewActions = cbSubscribeAction.Checked;
        user.ReceiveAllGroupNotifications = cbReceiveGroup.Checked;
        user.DoNotAutoSubscribe = cbNoAutoSubscribe.Checked;
        user.RestrictUserFromEditingAnyActions = cbRestrictUserFromEditingAnyActions.Checked;
        user.AllowUserToEditAnyAction = cbAllowUserToEditAnyAction.Checked;
        user.UserCanPinAction = cbUserCanPinAction.Checked;
        user.TicketRights = (TicketRightType)int.Parse(cmbRights.SelectedValue);
        user.ShowWelcomePage = true;
        UserSession.LoginUser.TimeZoneInfo = null;

        if (UserSession.CurrentUser.IsSystemAdmin)
        {
            user.IsActive = cbActive.Checked;
            user.IsSystemAdmin = cbIsSystemAdmin.Checked;
            user.IsChatUser = cbChat.Checked;
        }


        string checkRequired = _fieldControls.CheckRequiredCustomFields();
        if (checkRequired != "")
        {
            _manager.Alert(checkRequired);
            return false;
        }

        user.Collection.Save();

        if (user.IsActive) user.EmailCountToMuroc(true);

        _fieldControls.RefID = user.UserID;
        _fieldControls.SaveCustomFields();

        if (_userID < 0)
        {
            string password = DataUtils.GenerateRandomPassword();
            user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
            user.IsPasswordExpired = true;
            user.PasswordCreatedUtc = DateTime.UtcNow;
            user.Collection.Save();
            if (cbEmail.Checked) EmailPosts.SendWelcomeTSUser(UserSession.LoginUser, user.UserID, password);

            Organization orgTemplate = Organizations.GetTemplateOrganization(UserSession.LoginUser, organization.ProductType);

            if (orgTemplate != null) UserSettings.WriteString(UserSession.LoginUser, user.UserID, "Dashboard", UserSettings.ReadString(UserSession.LoginUser, (int)orgTemplate.PrimaryUserID, "Dashboard"));
        }

        TeamSupportSync.SyncUser(user.UserID, user.OrganizationID, user.FirstName, user.LastName, user.Email, user.Title);

        if (UserSession.CurrentUser.UserID == _userID)
        {
            UserSession.RefreshCurrentUserInfo();
            UserSession.RefreshLoginUser();
        }


        return true;
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        Users users = new Users(UserSession.LoginUser);
        users.LoadByUserID(_userID);
        if (!users.IsEmpty)
        {
            if (DataUtils.ResetPassword(UserSession.LoginUser, users[0], false))
            {
                _manager.Alert("A new password has been sent to " + users[0].FirstName + " " + users[0].LastName);
                return;
            }
        }
        _manager.Alert("There was an error reseting the password.");
    }
}
