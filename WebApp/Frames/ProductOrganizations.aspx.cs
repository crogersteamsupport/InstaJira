using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;

public partial class Frames_ProductOrganizations : BaseFramePage
{
  private int _productID;
  private int _versionID;

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);

    try
    {
      _productID = int.Parse(Request["ProductID"]);
      if (Request["VersionID"] == null)
        _versionID = -1;
      else 
        _versionID = int.Parse(Request["VersionID"]);
    }
    catch (Exception)
    {
      Response.Write("");
      Response.End();
      return;
    }

    gridCustomers.MasterTableView.PageSize = Settings.UserDB.ReadInt("ProductOrganizationsGridPageSize", 10);
    gridCustomers.MasterTableView.Columns[1].Visible = UserSession.CurrentUser.IsSystemAdmin;

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
      if (count >= 5) break;
      if (gridCustomers.Columns.FindByUniqueNameSafe("CustomField" + field.CustomFieldID.ToString()) == null)
      {
        GridBoundColumn column = new GridBoundColumn();
        gridCustomers.MasterTableView.Columns.Add(column);
        column.HeaderText = field.Name;
        column.DataField = field.Name;
        column.UniqueName = "CustomField" + field.CustomFieldID.ToString();
        switch (field.FieldType)
        {
          case CustomFieldType.Date:
            column.DataFormatString = "{0:" + UserSession.LoginUser.CultureInfo.DateTimeFormat.ShortDatePattern + "}";
            break;
          case CustomFieldType.Time:
            column.DataFormatString = "{0:" + UserSession.LoginUser.CultureInfo.DateTimeFormat.ShortTimePattern + "}";
            break;
        }
      }
      else
      {
        return;
      }
      count++;
    }

    gridCustomers.Rebind();
  }

  protected void gridCustomers_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
  {
    OrganizationProducts organizationProducts = new OrganizationProducts(UserSession.LoginUser);
    if (_versionID > -1) organizationProducts.LoadForProductCustomerGridByVersion(_versionID);
    else organizationProducts.LoadForProductCustomerGridByProduct(_productID);
    gridCustomers.DataSource = organizationProducts.Table;
  }

  protected void gridCustomers_PageSizeChanged(object source, GridPageSizeChangedEventArgs e)
  {
    Settings.UserDB.WriteInt("ProductOrganizationsGridPageSize", e.NewPageSize);

  }
  protected void gridCustomers_ItemDataBound(object sender, GridItemEventArgs e)
  {
    if (e.Item is GridDataItem)
    {
      GridDataItem item = (GridDataItem)e.Item;
      string key = item.GetDataKeyValue("OrganizationProductID").ToString();

      ImageButton button = (ImageButton)item["ButtonEdit"].Controls[0];
      button.OnClientClick = "EditRow('" + key + "'); return false;";

      button = (ImageButton)item["ButtonDelete"].Controls[0];
      button.OnClientClick = "DeleteRow('" + key + "'); return false;";
    } 

  }
}
