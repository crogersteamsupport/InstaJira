using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
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

public partial class Frames_ContactInformation : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    CachePage = false;
    int userID;
    User user;
    
    try
    {
      userID = int.Parse(Request["UserID"]);
      user = Users.GetUser(UserSession.LoginUser, userID);
      Organization organization = Organizations.GetOrganization(UserSession.LoginUser, user.OrganizationID);
      if (organization.ParentID != TSAuthentication.OrganizationID) throw new Exception("Invalid user id");
    }
    catch (Exception)
    {
      Response.Write("[No user to display.]");
      Response.End();
      return;
    }

    bool isAdmin = UserSession.CurrentUser.IsSystemAdmin || !UserSession.CurrentUser.IsAdminOnlyCustomers;
    btnNewPhone.Visible = isAdmin;
    btnNewAddress.Visible = isAdmin;
    btnEditProperties.Visible = isAdmin;
    pnlPhone.Attributes.Add("class", isAdmin ? "" : "divAdmin");
    pnlAddress.Attributes.Add("class", isAdmin ? "" : "divAdmin");

    if (btnNewAddress.Visible) btnNewAddress.OnClientClick = "ShowDialog(top.GetAddressDialog(" + userID.ToString() + ", 22)); top.Ts.System.logAction('Contact Info - Add Address'); return false;";
    if (btnNewPhone.Visible) btnNewPhone.OnClientClick = "ShowDialog(top.GetPhoneDialog(" + userID.ToString() + ", 22)); top.Ts.System.logAction('Contact Info - Add Phone'); return false;";
    if (btnEditProperties.Visible) btnEditProperties.OnClientClick = "parent.ShowDialog(top.GetContactDialog(" + user.OrganizationID.ToString() + "," + userID.ToString() + ")); top.Ts.System.logAction('Contact Info - Edit Properties'); return false;";
    LoadDetails(userID);
  }

  private void LoadDetails(int userID)
  {
    LoadProperties(userID);
    LoadNumbers(userID);
    LoadAddresses(userID);
  }

  private void LoadProperties(int userID)
  {
    Users users = new Users(UserSession.LoginUser);

    users.LoadByUserID(userID);
    lblProperties.Visible = users.IsEmpty;

    if (!users.IsEmpty)
    {
      User user = users[0];

      DataTable table = new DataTable();
      table.Columns.Add("Name");
      table.Columns.Add("Value");

      table.Rows.Add(new string[] { "Email:", "<a href=\"mailto:" + user.Email + "\">" + user.Email + "</a>" });
      table.Rows.Add(new string[] { "Title:", user.Title });
      table.Rows.Add(new string[] { "Active:", user.IsActive.ToString() });
      table.Rows.Add(new string[] { "Prevent this email address from creating or updating tickets:", user.BlockInboundEmail.ToString() });

      CustomFields fields = new CustomFields(UserSession.LoginUser);

      if (UserSession.CurrentUser.IsTSUser)
      {
        table.Rows.Add(new string[] { "System Administrator:", user.IsSystemAdmin.ToString() });
        table.Rows.Add(new string[] { "Financial Administrator:", user.IsFinanceAdmin.ToString() });
        table.Rows.Add(new string[] { "Portal User:", user.IsPortalUser.ToString() });
        table.Rows.Add(new string[] { "Last Logged In:", user.LastLogin.ToString("g", UserSession.LoginUser.CultureInfo) });
        table.Rows.Add(new string[] { "In Office:", user.InOffice.ToString() });
        table.Rows.Add(new string[] { "In Office Comment:", user.InOfficeComment });
        if (user.IsActive)
          table.Rows.Add(new string[] { "Activated On:", user.ActivatedOn.ToString("g", UserSession.LoginUser.CultureInfo) });
        else
        {
          if (user.DeactivatedOn != null)
          {
            DateTime dateTime = (DateTime)user.DeactivatedOn;
            table.Rows.Add(new string[] { "Deactivated On:", dateTime.ToString("g", UserSession.LoginUser.CultureInfo) });
          }
          else
          {
            table.Rows.Add(new string[] { "Deactivated On:", "" });

          }
        }
      }
      else
      {
        if (UserSession.CurrentUser.HasPortalRights)
        {
          table.Rows.Add(new string[] { "Portal User:", user.IsPortalUser.ToString() });
          if (user.IsPortalUser) table.Rows.Add(new string[] { "Last Logged In:", user.LastLogin.ToString("g", UserSession.LoginUser.CultureInfo) });
        }
      }
      fields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, ReferenceType.Contacts);




      StringBuilder valueAsString = null;
      foreach (CustomField field in fields)
      {
        CustomValue value = CustomValues.GetValue(UserSession.LoginUser, field.CustomFieldID, userID);
        switch (value.FieldType)
        {
          case CustomFieldType.Date:
            valueAsString = new StringBuilder();
            if (!string.IsNullOrEmpty(value.Value))
            {
              try
              {
                DateTime valueAsDateTime = DataUtils.DateToLocal(UserSession.LoginUser, DateTime.Parse(value.Value));
                valueAsString.Append(valueAsDateTime.ToString("d", UserSession.LoginUser.CultureInfo));
              }
              catch
              {
                valueAsString.Append(value.Value);
              }
            }
            table.Rows.Add(new string[] { field.Name + ":", valueAsString.ToString() });
            break;
          case CustomFieldType.Time:
            valueAsString = new StringBuilder();
            if (!string.IsNullOrEmpty(value.Value))
            {
              try
              {
                DateTime valueAsDateTime = DataUtils.DateToLocal(UserSession.LoginUser, DateTime.Parse(value.Value));
                valueAsString.Append(valueAsDateTime.ToString("t", UserSession.LoginUser.CultureInfo));
              }
              catch
              {
                valueAsString.Append(value.Value);
              }
            }
            table.Rows.Add(new string[] { field.Name + ":", valueAsString.ToString() });
            break;
          case CustomFieldType.DateTime:
            valueAsString = new StringBuilder();
            if (!string.IsNullOrEmpty(value.Value))
            {
              try
              {
                DateTime valueAsDateTime = DataUtils.DateToLocal(UserSession.LoginUser, DateTime.Parse(value.Value));
                valueAsString.Append(valueAsDateTime.ToString("g", UserSession.LoginUser.CultureInfo));
              }
              catch
              {
                valueAsString.Append(value.Value);
              }
            }
            table.Rows.Add(new string[] { field.Name + ":", valueAsString.ToString() });
            break;
          default:
        table.Rows.Add(new string[] { field.Name + ":", value.Value });
            break;
        }
      }

      rptProperties.DataSource = table;
      rptProperties.DataBind();

    }
  }

  private void LoadNumbers(int userID)
  {
    PhoneNumbers phoneNumbers = new PhoneNumbers(UserSession.LoginUser);
    phoneNumbers.LoadByID(userID, ReferenceType.Users);

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


  private void LoadAddresses(int userID)
  {
    Addresses addresses = new Addresses(UserSession.LoginUser);
    addresses.LoadByID(userID, ReferenceType.Users);
    lblAddresses.Visible = addresses.IsEmpty;


    rptAddresses.DataSource = addresses;
    rptAddresses.DataBind();
  }
}
