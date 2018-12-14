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

public partial class Dialogs_Organization : BaseDialogPage
{
  private int _organizatinID = -1;
  private bool _isAdmin = false;
  private CustomFieldControls _customControls;

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);
    if (Request["OrganizationID"] != null) _organizatinID = int.Parse(Request["OrganizationID"]);
    Organization organization = Organizations.GetOrganization(UserSession.LoginUser, _organizatinID);
    if (organization != null && (organization.OrganizationID != UserSession.LoginUser.OrganizationID && organization.ParentID != UserSession.LoginUser.OrganizationID))
    {
      Response.Write("Invalid Request");
      Response.End();
      return;
    }
    _customControls = new CustomFieldControls(ReferenceType.Organizations, -1, 2, tblCustomControls, false);
    if (_organizatinID != UserSession.LoginUser.OrganizationID)  _customControls.LoadCustomControls(200);
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    _customControls.RefID = _organizatinID;

    pnlEdit.Visible = _organizatinID > 0;
    _isAdmin = UserSession.CurrentUser.IsSystemAdmin && (_organizatinID != UserSession.LoginUser.OrganizationID);
    cbActive.Visible = _isAdmin;
    spanActive.Visible = cbActive.Visible;
    tdPortalInput.Visible = _isAdmin && UserSession.CurrentUser.HasPortalRights;
    tdPortalLabel.Visible = tdPortalInput.Visible;
    trApi.Visible = _isAdmin;

    trTimeZone.Visible = UserSession.LoginUser.OrganizationID == _organizatinID;

    trSupportRow.Visible = UserSession.LoginUser.OrganizationID != _organizatinID;
    spanDefaultPortalGroup.Visible = cmbGroups.Visible = UserSession.LoginUser.OrganizationID == _organizatinID;

    dpSAExpiration.Culture = UserSession.LoginUser.CultureInfo;

    if (!IsPostBack)
    {
      dpSAExpiration.MinDate = new DateTime(1800, 1, 1);
      LoadTimeZones();
      LoadSlas();
      LoadUsers(_organizatinID);
      LoadGroups(_organizatinID);
      LoadOrganization(_organizatinID);
      if (_organizatinID != UserSession.LoginUser.OrganizationID) _customControls.LoadValues();
      Page.RegisterStartupScript("AddMasks", @"
        <script type=""text/javascript"">
          $('.masked').each(function (index) {
            $(this).mask($(this).attr('placeholder'));
          });
        </script>
      ");
    }

    pnlActiveReason.Visible = _isAdmin && !cbActive.Checked;
  }

  private void LoadSlas()
  {
    SlaLevels levels = new SlaLevels(UserSession.LoginUser);
    levels.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);

    cmbSlas.Items.Clear();
    cmbSlas.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
    foreach (SlaLevel level in levels)
    {
      cmbSlas.Items.Add(new RadComboBoxItem(level.Name, level.SlaLevelID.ToString()));
    }
    cmbSlas.SelectedIndex = 0;
  }


  private void LoadTimeZones()
  {
    cmbTimeZones.Items.Clear();

    System.Collections.ObjectModel.ReadOnlyCollection<TimeZoneInfo> timeZones =  TimeZoneInfo.GetSystemTimeZones();
    foreach (TimeZoneInfo info in timeZones)
    {
      cmbTimeZones.Items.Add(new RadComboBoxItem(info.DisplayName, info.Id));
    }
  }


  private void LoadGroups(int organizationID)
  {
    cmbGroups.Items.Clear();
    cmbSupportGroups.Items.Clear();

    Groups groups = new Groups(UserSession.LoginUser);
    groups.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
    cmbGroups.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
    cmbSupportGroups.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
    foreach (Group group in groups)
    {
      cmbGroups.Items.Add(new RadComboBoxItem(group.Name, group.GroupID.ToString()));
      cmbSupportGroups.Items.Add(new RadComboBoxItem(group.Name, group.GroupID.ToString()));
    }
    cmbGroups.SelectedIndex = 0;
    cmbSupportGroups.SelectedIndex = 0;

  }

  private void LoadUsers(int organizationID)
  {
    cmbUsers.Items.Clear();

    Users users = new Users(UserSession.LoginUser);
    users.LoadByOrganizationID(organizationID, true);
    cmbUsers.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
    foreach (User user in users)
    {
      cmbUsers.Items.Add(new RadComboBoxItem(user.DisplayName, user.UserID.ToString()));
    }
    cmbUsers.SelectedIndex = 0;

    cmbSupportUsers.Items.Clear();

    users = new Users(UserSession.LoginUser);
    users.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, true);
    cmbSupportUsers.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
    foreach (User user in users)
    {
      cmbSupportUsers.Items.Add(new RadComboBoxItem(user.DisplayName, user.UserID.ToString()));
    }
    cmbSupportUsers.SelectedIndex = 0;
  }

  private void LoadOrganization(int organizationID)
  {
    Organizations organizations = new Organizations(UserSession.LoginUser);
    organizations.LoadByOrganizationID(organizationID);
    if (!organizations.IsEmpty)
    {
      textName.Text = organizations[0].Name;
      textWebSite.Text = organizations[0].Website;
      textSupportHoursMonth.Text = organizations[0].SupportHoursMonth.ToString();
      textDomains.Text = organizations[0].CompanyDomains;
      textDescription.Text = organizations[0].Description;
      cbPortal.Checked = organizations[0].HasPortalAccess;
      cbActive.Checked = organizations[0].IsActive;
      cbApiEnabled.Checked = organizations[0].IsApiActive && organizations[0].IsApiEnabled;
      spanApiToken.InnerHtml = organizations[0].WebServiceID.ToString();
      textActiveReason.Text = organizations[0].InActiveReason;
      cmbUsers.SelectedValue = organizations[0].PrimaryUserID.ToString();
      cmbSupportUsers.SelectedValue = organizations[0].DefaultSupportUserID.ToString();
      cmbGroups.SelectedValue = organizations[0].DefaultPortalGroupID.ToString();
      cmbSupportGroups.SelectedValue = organizations[0].DefaultSupportGroupID.ToString();
      if (string.IsNullOrEmpty(organizations[0].TimeZoneID))
        cmbTimeZones.SelectedValue = "Central Standard Time";
      else
        cmbTimeZones.SelectedValue = organizations[0].TimeZoneID;
      dpSAExpiration.SelectedDate = organizations[0].SAExpirationDate < new DateTime(1901, 1, 1, 0, 0, 0) ? new DateTime(1901, 1, 1, 0, 0, 0) : organizations[0].SAExpirationDate;
      if (organizations[0].SlaLevelID != null) cmbSlas.SelectedValue = organizations[0].SlaLevelID.ToString();
    }
    else
    {
      textName.Text = "";
      textDescription.Text = "";
      textActiveReason.Text = "";
      cbActive.Checked = true;
      cbApiEnabled.Checked = false;
      cbPortal.Checked = false;
    }
  }

  public override bool Save()
  {
    if (string.IsNullOrEmpty(textName.Text.Trim()))
    {
      _manager.Alert("Please choose a name.");
      return false;
    }

    Organization organization;

    Organizations organizations = new Organizations(UserSession.LoginUser);

    if (_organizatinID < 0)
    {
      organization = organizations.AddNewOrganization();
      organization.ParentID = UserSession.LoginUser.OrganizationID;
      organization.PrimaryUserID = null;
      
      organization.ExtraStorageUnits = 0;
      organization.PortalSeats = 0;
      organization.UserSeats = 0;
      organization.IsCustomerFree = false;
      organization.ProductType = ProductType.Express;
      organization.HasPortalAccess = false;
      organization.IsActive = true;
      organization.IsBasicPortal = true;
    }
    else
    {
      organization = Organizations.GetOrganization(UserSession.LoginUser, _organizatinID);
    }

    int? id = int.Parse(cmbUsers.SelectedValue);
    organization.PrimaryUserID = id < 0 ? null : id;
    id = int.Parse(cmbSupportUsers.SelectedValue);
    organization.DefaultSupportUserID = id < 0 ? null : id;
    id = int.Parse(cmbGroups.SelectedValue);
    organization.DefaultPortalGroupID = id < 0 ? null : id;
    id = int.Parse(cmbSupportGroups.SelectedValue);
    organization.DefaultSupportGroupID = id < 0 ? null : id;

    organization.TimeZoneID = cmbTimeZones.SelectedValue;
    UserSession.LoginUser.TimeZoneInfo = null;
    try
    {
      TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(organization.TimeZoneID);
    }
    catch (Exception)
    {
    }

    if (organization.Name != "_Unknown Company")
    { 
      organization.Name = textName.Text; 
    }

    organization.Website = textWebSite.Text;
    organization.CompanyDomains = textDomains.Text;

    organization.Description = textDescription.Text;
    organization.SAExpirationDate = DataUtils.DateToUtc(UserSession.LoginUser, dpSAExpiration.SelectedDate);

    if (cmbSlas.SelectedIndex == 0)
      organization.SlaLevelID = null;
    else
      organization.SlaLevelID = int.Parse(cmbSlas.SelectedValue);

    int shm = 0;
    int.TryParse(textSupportHoursMonth.Text, out shm);

    organization.SupportHoursMonth = shm;

    if (_isAdmin)
    {
      organization.HasPortalAccess = cbPortal.Checked;
      organization.IsActive = cbActive.Checked;
      organization.IsApiActive = cbApiEnabled.Checked;
      organization.IsApiEnabled = cbApiEnabled.Checked;
      organization.InActiveReason = textActiveReason.Text;
    }
    organization.Collection.Save();
    Settings.Session.WriteInt("SelectedOrganizationID", organization.OrganizationID);

    _customControls.RefID = organization.OrganizationID;
    if (_organizatinID != UserSession.LoginUser.OrganizationID) _customControls.SaveCustomFields();


    return true;
  }


    
  
}
