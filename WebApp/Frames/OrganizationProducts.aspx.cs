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
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;

public partial class Frames_OrganizationProducts : BaseFramePage
{
  private int _organizationID;

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);

    try
    {
      _organizationID = int.Parse(Request["OrganizationID"]);
    }
    catch (Exception)
    {
      Response.Write("");
      Response.End();
    }
    paneToolbar.Visible = UserSession.CurrentUser.IsSystemAdmin || !UserSession.CurrentUser.IsAdminOnlyCustomers;
    gridProducts.MasterTableView.Columns[0].Visible = UserSession.CurrentUser.IsSystemAdmin || !UserSession.CurrentUser.IsAdminOnlyCustomers;
    gridProducts.MasterTableView.Columns[1].Visible = UserSession.CurrentUser.IsSystemAdmin;
    gridProducts.MasterTableView.PageSize = Settings.UserDB.ReadInt("OrganizationProductsGridPageSize", 10);
    fieldOrganizationID.Value = _organizationID.ToString();
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    CreateCustomColumns();
  }

  private void CreateCustomColumns()
  {
    //if (ViewState[GetSettingKey("CustomColumnsCreated")] != null) return;
    //ViewState[GetSettingKey("CustomColumnsCreated")] = "1";

    CustomFields fields = new CustomFields(UserSession.LoginUser);
    fields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, ReferenceType.OrganizationProducts);
    int count = 0;

    foreach (CustomField field in fields)
    {
      if (count >= 25) break;
      if (gridProducts.Columns.FindByUniqueNameSafe("CustomField" + field.CustomFieldID.ToString()) == null)
      {
        GridBoundColumn column = new GridBoundColumn();
        gridProducts.MasterTableView.Columns.Add(column);
        column.HeaderText = field.Name;
        column.DataField = field.Name;
        column.UniqueName = "CustomField" + field.CustomFieldID.ToString();
        column.HeaderStyle.Width = new Unit(100, UnitType.Pixel);
      }
      else
      {
        return;
      }
      count++;
    }

    gridProducts.Rebind();
  }

  protected void gridProducts_NeedDataSource1(object source, GridNeedDataSourceEventArgs e)
  {
    OrganizationProducts organizationProducts = new OrganizationProducts(UserSession.LoginUser);
    organizationProducts.LoadForCustomerProductGrid(_organizationID);
    foreach(DataRow row in organizationProducts.Table.Rows)
    {
      row["SupportExpiration"]  = row["SupportExpiration"] != DBNull.Value ? DataUtils.DateToLocal(UserSession.LoginUser, (DateTime?)row["SupportExpiration"]) : row["SupportExpiration"];
      row["ReleaseDate"]        = row["ReleaseDate"] != DBNull.Value ? DataUtils.DateToLocal(UserSession.LoginUser, (DateTime?)row["ReleaseDate"]) : row["ReleaseDate"];
    }
    gridProducts.DataSource = organizationProducts.Table;
  }

  protected void gridProducts_ItemCommand1(object source, GridCommandEventArgs e)
  {

    if (e.CommandName == RadGrid.DeleteCommandName)
    {
      int organizationProductID = (int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["OrganizationProductID"];
      Products products = new Products(UserSession.LoginUser);
      products.RemoveCustomer(organizationProductID);
      gridProducts.Rebind();
    }
  }

  protected void gridProducts_PageSizeChanged(object source, GridPageSizeChangedEventArgs e)
  {
    Settings.UserDB.WriteInt("OrganizationProductsGridPageSize", e.NewPageSize);

  }
  protected void gridProducts_ItemDataBound(object sender, GridItemEventArgs e)
  {
    if (e.Item is GridDataItem)
    {
      GridDataItem item = (GridDataItem)e.Item;
      string key = item.GetDataKeyValue("OrganizationProductID").ToString();
      
      ImageButton button = (ImageButton)item["ButtonEdit"].Controls[0];
      button.OnClientClick = "EditRow('" + key + "'); return false;";

      button = (ImageButton)item["ButtonDelete"].Controls[0];
      button.OnClientClick = "DeleteRow('" + key + "'); return false;";

      button = (ImageButton)item["ButtonOpen"].Controls[0];
      button.OnClientClick = "OpenProduct('" + key + "'); return false;";
    } 
  }
}
