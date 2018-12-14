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

public partial class Frames_Users : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!IsPostBack)
    {
      tsMain.SelectedIndex = Settings.UserDB.ReadInt("SelectedUserTabIndex", 0);
      paneUsersGrid.Width = new Unit(Settings.UserDB.ReadInt("UserGridWidth", 200), UnitType.Pixel);
    }
    fieldOrganizationID.Value = UserSession.LoginUser.OrganizationID.ToString();

    if (!UserSession.CurrentUser.IsSystemAdmin)
    {
      tbUser.Items[0].Visible = false;
      tbUser.Items[1].Visible = false;
      tbUser.Items[2].Visible = false;
    }


    tsMain.Tabs.FindTabByText("History").Visible = TSAuthentication.IsSystemAdmin;

    int userCount = Organizations.GetUserCount(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
    Organization organization = (Organization)Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);

    if (organization.UserSeats <= userCount)
    {
      if (UserSession.CurrentUser.IsFinanceAdmin)
      {
        fieldAllowNewUsers.Value = "You have exceeded the number of user seat licenses.  If you would like to add additional users to your account, please contact our sales team at 800.596.2820 x806, or send an email to sales@teamsupport.com";
      }
      else
      {
        Users users = new Users(UserSession.LoginUser);
        users.LoadFinanceAdmins(UserSession.LoginUser.OrganizationID);
        if (users.IsEmpty)
        {
          fieldAllowNewUsers.Value = "Please ask your billing administrator to purchase additional user seat licenses.";
        }
        else
        {
          fieldAllowNewUsers.Value = "Please ask your billing administrator (" + users[0].FirstLastName + ") to purchase additional user seat licenses.";
        }
      }
    }
    else
    {
      fieldAllowNewUsers.Value = "";
    }
    fieldAllowNewUsers.Value = "";
  }

  private int GetUserID()
  {
    if (gridUsers.SelectedItems.Count < 1) return -1;
    GridItem item = gridUsers.SelectedItems[0];
    return (int)item.OwnerTableView.DataKeyValues[item.ItemIndex]["UserID"]; ;
  }

  private bool SetUserID(int id)
  {
    GridDataItem item = gridUsers.MasterTableView.FindItemByKeyValue("UserID", id);
    if (item != null)
    {
      item.Selected = true;
      return true;
    }
    else
    {
      return false;
    }
  }

  protected void gridUsers_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
  {
    Users users = new Users(UserSession.LoginUser);
    users.LoadByOrganizationIDForGrid(UserSession.LoginUser.OrganizationID, !UserSession.CurrentUser.IsSystemAdmin);
    gridUsers.DataSource = users.Table;
  }

  protected void gridUsers_ItemDataBound(object sender, GridItemEventArgs e)
  {
    if (e.Item is GridDataItem)
    {

      GridDataItem item = e.Item as GridDataItem;
      Label label = item.FindControl("lblName") as Label;

      string name = item["LastName"].Text + ", " + item["FirstName"].Text;

      if (!bool.Parse(item["IsActive"].Text))
      {
        label.Font.Italic = true;
        label.Text = name + " (Inactive)";
      }
      else
      {
        name = name + " (" + item["TicketCount"].Text + ")";

        if (String.IsNullOrEmpty(item["InOfficeComment"].Text.Trim()))
        {
          label.Text = name;
        }
        else
        {
          label.Text = name + " - " + item["InOfficeComment"].Text;
        }

      }

      bool isOnline = int.Parse(item["IsOnline"].Text) == 1;

      if (bool.Parse(item["InOffice"].Text))
      {
        (item["Image"].Controls[0] as ImageButton).ImageUrl = isOnline ? "~/images/icons/Online.png" : "~/images/icons/Offline.png";
      }
      else
      {
        (item["Image"].Controls[0] as ImageButton).ImageUrl = isOnline ? "~/images/icons/Unavailable.png" : "~/images/icons/Unavailable_g.png";
      }





    }
  }

  protected void gridUsers_DataBound(object sender, EventArgs e)
  {
    if (!SetUserID(Settings.UserDB.ReadInt("SelectedUserID")) && gridUsers.Items.Count > 0)
    {
      gridUsers.Items[0].Selected = true;
    }

    int id = GetUserID();
    User user = Users.GetUser(UserSession.LoginUser, id);
    if (user == null) return;


    captionSpan.InnerHtml = user.FirstLastName;
    if (tsMain.SelectedTab == null) tsMain.SelectedIndex = 0;
    userContentFrame.Attributes["src"] = tsMain.SelectedTab.Value + user.UserID.ToString();

  }
}
