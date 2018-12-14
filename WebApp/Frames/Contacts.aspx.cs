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


public partial class Frames_Contacts : BaseFramePage
{

  private int _organizationID;
  private int _userID = -1;

  protected void Page_Load(object sender, EventArgs e)
  {
    CachePage = false;
    try
    {
      _organizationID = int.Parse(Request["OrganizationID"]);
    }
    catch (Exception)
    {
      Response.Write("");
      Response.End();
      return;
    }

    if (Request["UserID"] != null)
    {
      _userID = int.Parse(Request["UserID"]);
      Settings.UserDB.WriteInt("SelectedContactID", _userID);
    }

    if (!IsPostBack)
    {
      paneUsersGrid.Width = new Unit(Settings.UserDB.ReadInt("ContactGridWidth", 200), UnitType.Pixel);
    }

    tbUser.Items[2].Visible = UserSession.CurrentUser.IsSystemAdmin;

    fieldOrganizationID.Value = _organizationID.ToString();
    if (!UserSession.CurrentUser.IsSystemAdmin && UserSession.CurrentUser.IsAdminOnlyCustomers)
    {
      paneToolbar.Visible = false;
    }

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
    users.LoadByOrganizationIDForGrid(_organizationID, !UserSession.CurrentUser.IsSystemAdmin);
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
        label.Text = name;
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
    if (!SetUserID(Settings.UserDB.ReadInt("SelectedContactID")) && gridUsers.Items.Count > 0)
    {
      gridUsers.Items[0].Selected = true;
    }

    int id = GetUserID();
    User user = Users.GetUser(UserSession.LoginUser, id);
    if (user == null) return;

    userContentFrame.Attributes["src"] = "ContactInformation.aspx?UserID=" + user.UserID.ToString();

  }
}
