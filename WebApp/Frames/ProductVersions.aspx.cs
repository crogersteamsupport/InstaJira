using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using TeamSupport.WebUtils;
using TeamSupport.Data;
using Telerik.Web.UI;
using System.Globalization;

public partial class Frames_ProductVersions : BaseFramePage
{
  private int _productID;

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    try
    {
      _productID = int.Parse(Request["ProductID"]);
    }
    catch (Exception)
    {
      Response.Write("");
      Response.End();
      return;
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {

  }
  protected void gridVersions_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
  {
    ProductVersions versions = new ProductVersions(UserSession.LoginUser);
    versions.LoadForGrid(_productID);
    /*
    foreach (DataRow row in versions.Table.Rows)
    {
      row["ReleaseDate"] = row["ReleaseDate"] != DBNull.Value ? DataUtils.DateToLocal(UserSession.LoginUser, (DateTime?)row["ReleaseDate"]) : row["ReleaseDate"];
      //ReportResults.aspx.cs(487):
    }*/
    gridVersions.DataSource = versions.Table;
  }


  protected void gridVersions_ItemDataBound(object sender, GridItemEventArgs e)
  {
    if (e.Item is GridDataItem)
    {
      GridDataItem item = e.Item as GridDataItem;

      foreach (GridColumn column in ((GridItem)(item)).OwnerTableView.Columns)
      {
        if (column is GridDateTimeColumn)
        {
          string s = item[column].Text;
          if (s != "")
          {
            try
            {
              DateTime dt = DateTime.SpecifyKind(DateTime.Parse(s, new CultureInfo("en-US")), DateTimeKind.Utc);
              item[column].Text = DataUtils.DateToLocal(UserSession.LoginUser, dt).ToString("g", UserSession.LoginUser.CultureInfo);
            }
            catch (Exception)
            {
            }
          }
        }
      }
    }
  }
}
