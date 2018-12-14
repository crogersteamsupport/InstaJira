using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.WebUtils;
using TeamSupport.Data;
using Telerik.Web.UI;
using System.Data;
using System.Text;

public partial class Frames_OrganizationInformation : BaseFramePage
{

  private bool _isAdmin = false;
  private int _organizationID = -1;

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    try
    {
      _organizationID = int.Parse(Request["OrganizationID"]);
      Organization organization = (Organization)Organizations.GetOrganization(UserSession.LoginUser, _organizationID);
      if (organization == null || organization.ParentID != UserSession.LoginUser.OrganizationID) throw new Exception();
    }
    catch (Exception)
    {
      Response.Write("Please select a customer.");
      Response.End();
      return;
    }

    _isAdmin = (UserSession.LoginUser.OrganizationID != _organizationID) || UserSession.CurrentUser.IsSystemAdmin;
    _isAdmin = _isAdmin && (UserSession.CurrentUser.IsSystemAdmin || !UserSession.CurrentUser.IsAdminOnlyCustomers);
    LoadData();

    btnNewPhone.Visible = _isAdmin;
    btnNewAddress.Visible = _isAdmin;
    btnEditProperties.Visible = _isAdmin;

    if (_isAdmin)
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


    if (btnNewAddress.Visible) btnNewAddress.OnClientClick = "ShowDialog(top.GetAddressDialog(" + _organizationID.ToString() + ", 9)); top.Ts.System.logAction('Organization Info - Add Address'); return false;";
    if (btnNewPhone.Visible) btnNewPhone.OnClientClick = "ShowDialog(top.GetPhoneDialog(" + _organizationID.ToString() + ", 9)); top.Ts.System.logAction('Organization Info - Add Phone'); return false;";
    if (btnEditProperties.Visible) btnEditProperties.OnClientClick = "ShowDialog(top.GetOrganizationDialog(" + _organizationID.ToString() + ")); top.Ts.System.logAction('Organization Info - Edit Properties'); return false;";

  }


  public void LoadData()
  {
    LoadProperties(_organizationID);
    LoadNumbers(_organizationID);
    LoadAddresses(_organizationID);
    LoadCharts(_organizationID);
  }

  private void LoadCharts(int organizationID)
  {
    TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);
    ticketTypes.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, UserSession.CurrentUser.ProductType);

    DataTable table = new DataTable();
    table.Columns.Add("TicketType");
    table.Columns.Add("Count", Type.GetType("System.Int32"));

    int total = 0;

    foreach (TicketType ticketType in ticketTypes)
    {
      int count = Tickets.GetOrganizationOpenTicketCount(UserSession.LoginUser, organizationID, ticketType.TicketTypeID);
      total += count;
      if (count > 0) table.Rows.Add(new string[] { ticketType.Name, count.ToString() });
    }

    chartOpenTickets.ChartTitle.TextBlock.Text = total.ToString() + " Open Tickets";
    chartOpenTickets.DataSource = table;
    chartOpenTickets.DataBind();
    chartOpenTickets.Series[0].Appearance.LabelAppearance.Distance = 20;
    chartOpenTickets.Series[0].Appearance.TextAppearance.TextProperties.Font = new System.Drawing.Font("Arial", 6);

    table = new DataTable();
    table.Columns.Add("TicketType");
    table.Columns.Add("Count", Type.GetType("System.Int32"));

    total = 0;

    foreach (TicketType ticketType in ticketTypes)
    {
      int count = Tickets.GetOrganizationClosedTicketCount(UserSession.LoginUser, organizationID, ticketType.TicketTypeID);
      total += count;
      if (count > 0) table.Rows.Add(new string[] { ticketType.Name, count.ToString() });
    }

    chartClosedTickets.ChartTitle.TextBlock.Text = total.ToString() + " Closed Tickets";
    chartClosedTickets.DataSource = table;
    chartClosedTickets.DataBind();
    chartClosedTickets.Series[0].Appearance.LabelAppearance.Distance = 20;
    chartClosedTickets.Series[0].Appearance.TextAppearance.TextProperties.Font = new System.Drawing.Font("Arial", 6);

  }

  private void LoadProperties(int organizationID)
  {
    lblProperties.Visible = true;

    Organizations organizations = new Organizations(UserSession.LoginUser);
    organizations.LoadByOrganizationID(organizationID);

    if (organizations.IsEmpty) return;
    Organization organization = organizations[0];


    Users users = new Users(UserSession.LoginUser);
    string primaryUser = "";
    if (organization.PrimaryUserID != null)
    {
      users.LoadByUserID((int)organization.PrimaryUserID);
      primaryUser = users.IsEmpty ? "" : users[0].LastName + ", " + users[0].FirstName;
    }

    lblProperties.Visible = organizations.IsEmpty;

    DataTable table = new DataTable();
    table.Columns.Add("Name");
    table.Columns.Add("Value");

    string website = organization.Website;
    string link = "";
    if (website != null)
    {
      if (website.IndexOf("http") < 0) website = "http://" + website;
      link = @"<a href=""" + website + @""" target=""OrganizationLink"">" + organization.Website + "</a>";
    }

    table.Rows.Add(new string[] { "Name:", organization.Name });
    table.Rows.Add(new string[] { "Website:", link });
    table.Rows.Add(new string[] { "Description:", organization.Description });
    table.Rows.Add(new string[] { "Service Agreement Expiration Date:", organization.SAExpirationDate == null ? "[None]" : ((DateTime)organization.SAExpirationDate).ToLongDateString() });
    if (organization.SlaLevelID == null)
      table.Rows.Add(new string[] { "Service Level Agreement:", "[None]" });
    else
    {
      SlaLevel level = SlaLevels.GetSlaLevel(UserSession.LoginUser, (int)organization.SlaLevelID);
      if (level != null) table.Rows.Add(new string[] { "Service Level Agreement:", level.Name });
    }

    if (organizationID != UserSession.LoginUser.OrganizationID)
    { 
      table.Rows.Add(new string[] { "Active:", organization.IsActive.ToString() });
      if (UserSession.CurrentUser.IsSystemAdmin)
      {
        table.Rows.Add(new string[] { "API Enabled:", (organization.IsApiActive && organization.IsApiEnabled).ToString() });
        table.Rows.Add(new string[] { "API Token:", organization.WebServiceID.ToString() });
        table.Rows.Add(new string[] { "OrganizationID:", organization.OrganizationID.ToString() });
      }
    }
    if (UserSession.CurrentUser.HasPortalRights)
    {
      table.Rows.Add(new string[] { "Portal Access:", organization.HasPortalAccess.ToString() });
    }
    table.Rows.Add(new string[] { "Primary Contact:", primaryUser });

    if (organization.DefaultSupportUserID != null)
    {
      User supportUser = Users.GetUser(UserSession.LoginUser, (int)organization.DefaultSupportUserID);
      table.Rows.Add(new string[] { "Default Support User:", supportUser.FirstLastName });
    }
    else
	  {
      table.Rows.Add(new string[] { "Default Support User:", "[None]" });
    }

    if (organization.DefaultSupportGroupID != null)
    {
      Group supportGroup = (Group)Groups.GetGroup(UserSession.LoginUser, (int)organization.DefaultSupportGroupID);
      if (supportGroup != null) 
        table.Rows.Add(new string[] { "Default Support Group:", supportGroup.Name });
      else
        table.Rows.Add(new string[] { "Default Support Group:", "[None]" });
    }
    else
    {
      table.Rows.Add(new string[] { "Default Support Group:", "[None]" });
    }
    table.Rows.Add(new string[] { "Domains:", organization.CompanyDomains == null ? "[None Assigned]" : organization.CompanyDomains });
      

    if(organization.SupportHoursMonth != 0)
        table.Rows.Add(new string[] { "Support hours per month:", organization.SupportHoursMonth.ToString() });
    else
        table.Rows.Add(new string[] { "Support hours per month:", "0" });


    CustomFields fields = new CustomFields(UserSession.LoginUser);
    fields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, ReferenceType.Organizations);

    StringBuilder valueAsString = null;
    foreach (CustomField field in fields)
    {
      if (field.CustomFieldCategoryID != null) continue;
      CustomValue value = CustomValues.GetValue(UserSession.LoginUser, field.CustomFieldID, organizationID);
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

    CustomFieldCategories cats = new CustomFieldCategories(UserSession.LoginUser);
    cats.LoadByRefType(ReferenceType.Organizations);

    StringBuilder builder = new StringBuilder();
    string prop = "<div style=\"margin: 5px 5px 5px 15px; line-height: 20px;\"><span style=\"font-weight: bold;\">{0}: </span><span> {1}<br /></span></div>";
    foreach (CustomFieldCategory cat in cats)
    {
      bool isExpanded = Settings.UserDB.ReadBool("Custom Cat Expanded (Customer) - " + cat.Category, false);
      string markerClass = isExpanded ? "ui-icon-triangle-1-s" : "ui-icon-triangle-1-e";
      string displayClass = isExpanded ? "" : "ui-helper-hidden";

      builder.Append("<div class=\"customfield-cat\"><span class=\"ui-icon "+markerClass+"\"></span><span class=\"caption\">" + cat.Category);
      builder.Append("</span></div><div class=\"ui-widget-content ts-separator "+displayClass+"\"></div>");
      builder.Append("<div class=\""+displayClass+"\">");
      foreach (CustomField field in fields)
      {
        if (field.CustomFieldCategoryID != null && field.CustomFieldCategoryID == cat.CustomFieldCategoryID)
        { 
         CustomValue value = CustomValues.GetValue(UserSession.LoginUser, field.CustomFieldID, organizationID);
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
              builder.Append(string.Format(prop, field.Name, valueAsString.ToString()));
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
              builder.Append(string.Format(prop, field.Name, valueAsString.ToString()));
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
              builder.Append(string.Format(prop, field.Name, valueAsString.ToString()));
              break;
            default:
         builder.Append(string.Format(prop, field.Name, value.Value));
              break;
          }
        }
      }
      builder.Append("</div>");
    }
    litProperties.Text = builder.ToString();


    rptProperties.DataSource = table;
    rptProperties.DataBind();
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

    if (!addresses.IsEmpty)
    {
      addresses.Table.Columns.Add("MapLink");


      foreach (Address address in addresses)
      {
        string link = "<a href=\"http://maps.google.com/maps?q={0}\" target=\"_blank\">View Map</a>";
        StringBuilder builder = new StringBuilder();
        if (!String.IsNullOrEmpty(address.Addr1))
        {
          builder.Append(address.Addr1.Replace(' ', '+'));
        }

        if (!String.IsNullOrEmpty(address.Addr2))
        {
          if (builder.Length > 0) builder.Append("+");
          builder.Append(address.Addr2.Replace(' ', '+'));
        }

        if (!String.IsNullOrEmpty(address.Addr3))
        {
          if (builder.Length > 0) builder.Append("+");
          builder.Append(address.Addr3.Replace(' ', '+'));
        }

        if (!String.IsNullOrEmpty(address.City))
        {
          if (builder.Length > 0) builder.Append(",");
          builder.Append(address.City.Replace(' ', '+'));
        }

        if (!String.IsNullOrEmpty(address.State))
        {
          if (builder.Length > 0) builder.Append(",");
          builder.Append(address.State.Replace(' ', '+'));
        }

        if (!String.IsNullOrEmpty(address.Country))
        {
          if (builder.Length > 0) builder.Append(",");
          builder.Append(address.Country.Replace(' ', '+'));
        }

        link = String.Format(link, builder.ToString());

        address.Row["MapLink"] = link;
      }
    }

    lblAddresses.Visible = addresses.IsEmpty;

    rptAddresses.DataSource = addresses.Table;
    rptAddresses.DataBind();
  }

  protected void chartTickets_ItemDataBound(object sender, Telerik.Charting.ChartItemDataBoundEventArgs e)
  {
    DataRowView row = e.DataItem as DataRowView;
    e.SeriesItem.Label.TextBlock.Text = string.Format("{0} (#Y)", row["TicketType"]);
  }
}
