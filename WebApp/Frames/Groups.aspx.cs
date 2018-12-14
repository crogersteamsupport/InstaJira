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
using System.Text;

public partial class Frames_Groups : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    tbGroup.Items[0].Visible = UserSession.CurrentUser.IsSystemAdmin;
    tbGroup.Items[1].Visible = UserSession.CurrentUser.IsSystemAdmin;
    tbGroup.Items[2].Visible = UserSession.CurrentUser.IsSystemAdmin;

    if (!IsPostBack)
    {
      tsMain.SelectedIndex = Settings.UserDB.ReadInt("SelectedGroupTabIndex", 0);
      pangGroupGrid.Width = new Unit(Settings.UserDB.ReadInt("GroupGridWidth", 250), UnitType.Pixel);

      tsMain.Tabs.Add(new RadTab("Water Cooler", "../vcr/1_9_0/Pages/Watercooler.html?"));
    }

  }

  private int GetGridGroupID()
  {
    if (gridGroups.SelectedItems.Count < 1) return -1;
    GridItem item = gridGroups.SelectedItems[0];
    return (int)item.OwnerTableView.DataKeyValues[item.ItemIndex]["GroupID"]; ;
  }

  private bool SetGridGroupID(int groupID)
  {
    GridDataItem item = gridGroups.MasterTableView.FindItemByKeyValue("GroupID", groupID);
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

  protected void gridGroups_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
  {
    Groups groups = new Groups(UserSession.LoginUser);
    groups.LoadByOrganizationIDForGrid(UserSession.LoginUser.OrganizationID, UserSession.LoginUser.UserID);
    gridGroups.DataSource = groups.Table;
  }

  protected void gridGroups_DataBound(object sender, EventArgs e)
  {
    if (!SetGridGroupID(Settings.UserDB.ReadInt("SelectedGroupID")) && gridGroups.Items.Count > 0)
    {
      gridGroups.Items[0].Selected = true;
    }

    int id = GetGridGroupID();
    Group group = (Group)Groups.GetGroup(UserSession.LoginUser, id);
    if (group == null) return;


    captionSpan.InnerHtml = group.Name;
    groupContentFrame.Attributes["src"] = tsMain.SelectedTab.Value + group.GroupID.ToString();

  }

  protected void gridGroups_ItemDataBound(object sender, GridItemEventArgs e)
  {
    if (e.Item is GridDataItem)
    {
      GridDataItem item = e.Item as GridDataItem;
      item["Name"].Text = item["Name"].Text + " (" + item["TicketCount"].Text + ")";
    }
  }
}
