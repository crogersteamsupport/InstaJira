using System;
using System.Collections.Generic;
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
using System.Web.Services;

public partial class Dialogs_Contact : BaseDialogPage
{
  private int _userID = -1;
  private int _organizationID = -1;

  private CustomFieldControls _fieldControls;

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);
    _fieldControls = new CustomFieldControls(ReferenceType.Contacts, -1, 2, tblCustomControls, false);
    _fieldControls.LoadCustomControls(200);
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    _manager.AjaxSettings.AddAjaxSetting(cbIsPortalUser, cbEmail);
    _manager.AjaxSettings.AddAjaxSetting(cbIsPortalUser, btnReset);

    if (Request["UserID"] != null)
    {
      _userID = int.Parse(Request["UserID"]);
    }

    if (Request["OrganizationID"] != null)
    {
      _organizationID = int.Parse(Request["OrganizationID"]);
    }

    Organization organization = (Organization)Organizations.GetOrganization(UserSession.LoginUser, _organizationID);
    if (organization == null || organization.ParentID != UserSession.LoginUser.OrganizationID)
    {
      Response.Write("Invalid Request");
      Response.End();
      return;
    }

    if (!IsPostBack)
    {
      cbActive.Checked = true;
      Page.Title = "New Contact"; 
      if (_userID > -1) LoadUser(_userID);
      //cbEmail.Visible = (_userID < 0);
      _fieldControls.RefID = _userID;
      _fieldControls.LoadValues();

      User user = Users.GetUser(UserSession.LoginUser, _userID);
      RadComboBoxItem item = new RadComboBoxItem(organization.Name, organization.OrganizationID.ToString());
      item.Selected = true;
      cmbCustomer.Items.Add(item);
      Page.RegisterStartupScript("AddMasks", @"
        <script type=""text/javascript"">
          $('.masked').each(function (index) {
            $(this).mask($(this).attr('placeholder'));
          });
        </script>
      ");
    }




    trAdmin.Visible = false;
    spanIsPortalUser.Visible = cbIsPortalUser.Visible = UserSession.CurrentUser.HasPortalRights && UserSession.CurrentUser.IsSystemAdmin && organization.HasPortalAccess;
    btnReset.Visible = cbIsPortalUser.Visible && cbIsPortalUser.Checked && (_userID > -1) && _userID != UserSession.LoginUser.UserID;

    if (UserSession.CurrentUser.IsTSUser)
    {
      trAdmin.Visible = true;
      cbActive.Visible = true;
      cbIsFinanceAdmin.Visible = true;
      cbIsPortalUser.Visible = true;
      cbIsSystemAdmin.Visible = true;
      btnReset.Visible = _userID > -1;
      btnWelcome.Visible = _userID > -1;
      cbEmail.Visible = !btnReset.Visible;
      cbEmail.Text = "Send user a password email";
      btnReset.Text = "Reset User Password";
    }

  }

  private void LoadUser(int userID)
  {
    Users users = new Users(UserSession.LoginUser);
    users.LoadByUserID(userID);
    if (!users.IsEmpty)
    {
      Page.Title = "Contact - " + users[0].FirstName + " " + users[0].LastName;

      textEmail.Text = users[0].Email;
      textFirstName.Text = users[0].FirstName;
      textLastName.Text = users[0].LastName;
      textMiddleName.Text = users[0].MiddleName;
      textTitle.Text = users[0].Title;
      cbActive.Checked = users[0].IsActive;
      cbBlockEmail.Checked = users[0].BlockInboundEmail;
      cbIsSystemAdmin.Checked = users[0].IsSystemAdmin;
      cbIsFinanceAdmin.Checked = users[0].IsFinanceAdmin;
      cbIsPortalUser.Checked = users[0].IsPortalUser;
    }
  }

  public override bool Save()
  {
    User user;
    Users users = new Users(UserSession.LoginUser); ;
    int newOrgID = int.Parse(cmbCustomer.SelectedValue);


    string email = textEmail.Text.Trim();
    if ((cbIsPortalUser.Checked) && (email.Length < 1 || email.IndexOf('@') < 0 || email.IndexOf('.') < 0))
    {
      _manager.Alert("The email you have specified is invalid.  Please choose another email.");
      return false;
    }

    if (email != "" && !users.IsEmailValid(email, _userID, newOrgID))
    {
      _manager.Alert("The email you have specified is already in use.  Please choose another email.");
      return false;
    }

    if (textFirstName.Text.Trim().Length < 1 || textLastName.Text.Trim().Length < 1)
    {
      _manager.Alert("The name you have specified is invalid.  Please enter your name.");
      return false;
    }


    if (_userID < 0)
    {
      user = users.AddNewUser();
      user.OrganizationID = newOrgID;
      if (cbActive.Checked) user.ActivatedOn = DateTime.UtcNow;
      user.LastLogin = DateTime.UtcNow;
      user.LastActivity = DateTime.UtcNow.AddHours(-1); 
      user.IsPasswordExpired = true;
      user.ReceiveTicketNotifications = true;
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
      if (user.IsActive && !cbActive.Checked) user.DeactivatedOn = DateTime.UtcNow;
      if (user.OrganizationID != newOrgID)
      {
        user.OrganizationID = newOrgID;
        Tickets tickets = new Tickets(UserSession.LoginUser);
        tickets.LoadByContact(user.UserID);
        foreach (Ticket ticket in tickets)
        {
          tickets.AddOrganization(newOrgID, ticket.TicketID);
        }

      }
    }

    user.Email = textEmail.Text;
    user.FirstName = textFirstName.Text;
    user.LastName = textLastName.Text;
    user.Title = textTitle.Text;
    user.MiddleName = textMiddleName.Text;
    user.BlockInboundEmail = cbBlockEmail.Checked;


    user.IsActive = cbActive.Checked;

    if (UserSession.CurrentUser.HasPortalRights && UserSession.CurrentUser.IsSystemAdmin)
      user.IsPortalUser = cbIsPortalUser.Checked;

    if (UserSession.CurrentUser.IsTSUser && UserSession.CurrentUser.IsSystemAdmin)
    {
      user.IsSystemAdmin = cbIsSystemAdmin.Checked;
      user.IsFinanceAdmin = cbIsFinanceAdmin.Checked;
    }



    user.Collection.Save();

    _fieldControls.RefID = user.UserID;
    _fieldControls.SaveCustomFields();

    if (_userID < 0)
    {
      string password = DataUtils.GenerateRandomPassword();
      user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
      user.IsPasswordExpired = true;
      user.Collection.Save();
      if (UserSession.CurrentUser.IsTSUser && cbEmail.Checked)
      {
        EmailPosts.SendWelcomeTSUser(UserSession.LoginUser, user.UserID, password);
      }
      else if (cbEmail.Checked && cbIsPortalUser.Checked) 
        EmailPosts.SendWelcomePortalUser(UserSession.LoginUser, user.UserID, password);

    }

    return true;
  }

  protected void btnReset_Click(object sender, EventArgs e)
  {
    Users users = new Users(UserSession.LoginUser);
    users.LoadByUserID(_userID);
    if (!users.IsEmpty)
    {
      if (users[0].CryptedPassword == "")
      {
        string password = DataUtils.GenerateRandomPassword();
        users[0].CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
        users[0].IsPasswordExpired = true;
        users[0].Collection.Save();
        if (UserSession.CurrentUser.IsTSUser)
          EmailPosts.SendWelcomeTSUser(UserSession.LoginUser, users[0].UserID, password);
        else
          EmailPosts.SendWelcomePortalUser(UserSession.LoginUser, users[0].UserID, password);

        _manager.Alert("A new password has been sent to " + users[0].FirstName + " " + users[0].LastName);
        return;

      }
      else
      {
        if (DataUtils.ResetPassword(UserSession.LoginUser, users[0], !UserSession.CurrentUser.IsTSUser))
        {
          _manager.Alert("A new password has been sent to " + users[0].FirstName + " " + users[0].LastName);
          return;
        }
      }
    }
    _manager.Alert("There was an error sending the password.");

  }
  protected void cbIsPortalUser_CheckedChanged(object sender, EventArgs e)
  {
    if (UserSession.CurrentUser.IsTSUser) return;
    if (_userID < 0)
      cbEmail.Visible = cbIsPortalUser.Checked;
    else
      btnReset.Visible = cbIsPortalUser.Checked;
  }

  protected void btnWelcome_Click(object sender, EventArgs e)
  {
    Users users = new Users(UserSession.LoginUser);
    users.LoadByUserID(_userID);
    EmailPosts.SendWelcomeNewSignup(UserSession.LoginUser, _userID, "");
    _manager.Alert("A welcome message has been sent to " + users[0].FirstName + " " + users[0].LastName);

  }

  [WebMethod]
  public static RadComboBoxItemData[] GetOrganization(RadComboBoxContext context)
  {
    IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;

    Organizations organizations = new Organizations(UserSession.LoginUser);
    organizations.LoadByLikeOrganizationName(UserSession.LoginUser.OrganizationID, context["FilterString"].ToString(), !UserSession.CurrentUser.IsSystemAdmin, 250);

    List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();

    foreach (Organization organization in organizations)
    {
      RadComboBoxItemData itemData = new RadComboBoxItemData();
      itemData.Text = organization.Name;
      itemData.Value = organization.OrganizationID.ToString();
      list.Add(itemData);
    }

    return list.ToArray();
  }

}


