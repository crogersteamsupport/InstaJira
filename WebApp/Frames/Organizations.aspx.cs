using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;
using System.Text;

public partial class Frames_Organizations : BaseFramePage
{
  public class CustomersGridItem
  {
    public CustomersGridItem(string name, string id, int organizationID, int userID, bool isActive, bool isUser)
    {
      _name = name;
      _id = id;
      _organizationID = organizationID;
      _userID = userID;
      _isActive = isActive;
      _isUser = isUser;
    }

    private string _name;
    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    private int _organizationID;
    public int OrganizationID
    {
      get { return _organizationID; }
      set { _organizationID = value; }
    }

    private int _userID;
    public int UserID
    {
      get { return _userID; }
      set { _userID = value; }
    }

    private string _id;
    public string ID
    {
      get { return _id; }
      set { _id = value; }
    }
    private bool _isActive;
    public bool IsActive
    {
      get { return _isActive; }
      set { _isActive = value; }
    }

    private bool _isUser;
    public bool IsUser
    {
      get { return _isUser; }
      set { _isUser = value; }
    }
  }

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);
    CreateNavButtons();

    if (!IsPostBack)
    {
      tsMain.SelectedIndex = Settings.UserDB.ReadInt("SelectedOrganizationTabIndex", 0);
      paneGrid.Width = new Unit(Settings.UserDB.ReadInt("OrganizationsGridWidth", 200), UnitType.Pixel);
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    tbMain.Items[2].Visible = UserSession.CurrentUser.IsSystemAdmin;

    if (!UserSession.CurrentUser.IsSystemAdmin && UserSession.CurrentUser.IsAdminOnlyCustomers)
    {
      paneToolBar.Visible = false;
    }

    if (!IsPostBack)
    {
      if (tsMain.SelectedTab == null) tsMain.SelectedIndex = 0;
    }
  }

  private void CreateNavButtons()
  {
    tsMain.Tabs.Clear();

    tsMain.Tabs.Add(new RadTab("Details", "OrganizationInformation.aspx?OrganizationID="));
    tsMain.Tabs.Add(new RadTab("Contacts", "Contacts.aspx?OrganizationID="));
    if (!UserSession.CurrentUser.IsTSUser)
    {
      if (UserSession.CurrentUser.ProductType == ProductType.Enterprise)
        tsMain.Tabs.Add(new RadTab("Products", "OrganizationProducts.aspx?OrganizationID="));
      tsMain.Tabs.Add(new RadTab("Attachments", "Attachments.aspx?RefType=9&RefID="));
      tsMain.Tabs.Add(new RadTab("Notes", "Notes.aspx?RefType=9&RefID="));
    }
    else
    {
      tsMain.Tabs.Add(new RadTab("Account Info", "AccountInformation.aspx?OrganizationID="));
    }

    RadTab tab = new RadTab("History", "History.aspx?RefType=9&RefID=");
    tsMain.Tabs.Add(tab);

    if (UserSession.CurrentUser.IsInventoryEnabled)
      tsMain.Tabs.Add(new RadTab("Inventory", "../Inventory/CustomerInventory.aspx?CustID="));

    tsMain.Tabs.Add(new RadTab("Water Cooler", "../vcr/1_9_0/Pages/Watercooler.html?"));

    if (!UserSession.CurrentUser.IsTSUser)
    {
      tsMain.Tabs.Add(new RadTab("All Tickets", "TicketTabsAll.aspx?tf_CustomerID="));

      //tsMain.Tabs.Add(new RadTab("All Tickets", "Tickets.aspx?CustomerID="));
      TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);
      ticketTypes.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, UserSession.CurrentUser.ProductType);
      foreach (TicketType ticketType in ticketTypes)
      {
        tsMain.Tabs.Add(new RadTab(ticketType.Name, "TicketTabsOrganization.aspx?TicketTypeID=" + ticketType.TicketTypeID.ToString() + "&OrganizationID="));
      }
    }
    else
    {
      tsMain.Tabs.Add(new RadTab("All Tickets (READONLY - DO NOT MODIFY!!!)", "Tickets.aspx?CustomerID="));
    }


  }

  [WebMethod]
  public static string GetResults(string filter)
  {
    filter = filter.Trim();
    if (filter.Length > 0 && filter.Length < 2) return "";

    UsersView users = new UsersView(UserSession.LoginUser);
    if (filter.Length > 0) users.LoadByLikeName(UserSession.LoginUser.OrganizationID, filter, 100, true);

    Organizations organizations = new Organizations(UserSession.LoginUser);
    organizations.LoadByLikeOrganizationName(UserSession.LoginUser.OrganizationID, filter, false, 100, true);

    StringBuilder builder = new StringBuilder();
    string data = @"{{""OrganizationID"":""{0}"",""UserID"":""{1}""}}";

    int count = 0;

    foreach (Organization item in organizations)
    {
      if (++count > 200) break;
      builder.Append(GetItemHtml("o" + item.OrganizationID, HttpUtility.HtmlEncode(item.Name), string.Format(data, item.OrganizationID.ToString(), "-1")));
    }

    foreach (UsersViewItem item in users)
    {
      if (++count > 200) break;
      builder.Append(GetItemHtml("u" + item.UserID, item.LastName + ", " + item.FirstName + " [" + HttpUtility.HtmlEncode(item.Organization) + "]", string.Format(data, item.OrganizationID.ToString(), item.UserID.ToString())));
    }

    return builder.ToString();
  }

  [WebMethod]
  public static string GetContact(int id)
  {
    UsersView users = new UsersView(UserSession.LoginUser);
    users.LoadByUserID(id);

    StringBuilder builder = new StringBuilder();
    string data = @"{{""OrganizationID"":""{0}"",""UserID"":""{1}""}}";


    foreach (UsersViewItem item in users)
    {
      builder.Append(GetItemHtml("u" + item.UserID, item.LastName + ", " + item.FirstName + " [" + item.Organization + "]", string.Format(data, item.OrganizationID.ToString(), item.UserID.ToString())));
    }

    return builder.ToString();
  }


  [WebMethod]
  public static string GetCustomer(int id)
  {
    Organizations organizations = new Organizations(UserSession.LoginUser);
    organizations.LoadByOrganizationID(id);

    StringBuilder builder = new StringBuilder();
    string data = @"{{""OrganizationID"":""{0}"",""UserID"":""{1}""}}";


    foreach (Organization item in organizations)
    {
      builder.Append(GetItemHtml("o" + item.OrganizationID, HttpUtility.HtmlEncode(item.Name), string.Format(data, item.OrganizationID.ToString(), "-1")));
    }

    return builder.ToString();
  }

  public static string GetItemHtml(string id, string caption, string data)
  {
    string s = @"<div id=""{0}"" class=""ui-menutree-item ui-menutree-state-default ui-corner-all"">
  <div class=""ui-menutree-text"">{1}</div>
  <span class=""ui-menutree-data"">{2}</span>
  </div>";
    return string.Format(s, id, caption, data);
  
  }

}
