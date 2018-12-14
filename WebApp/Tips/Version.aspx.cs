using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Text;

public partial class Tips_Version : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
      if (Request["VersionID"] == null) EndResponse("Invalid Version");

      int versionID = int.Parse(Request["VersionID"]);
      ProductVersionsViewItem version = ProductVersionsView.GetProductVersionsViewItem(TSAuthentication.GetLoginUser(), versionID);
      if (version == null) EndResponse("Invalid Version");

      if (version.OrganizationID != TSAuthentication.OrganizationID) EndResponse("Invalid Version");
      
      tipProduct.InnerText = version.ProductName;
      tipProduct.Attributes.Add("onclick", "top.Ts.MainPage.openNewProduct(" + version.ProductID.ToString() + "); return false;");
      tipVersion.InnerText = version.VersionNumber;
      tipVersion.Attributes.Add("onclick", "top.Ts.MainPage.openNewProductVersion(" + version.ProductVersionID.ToString() + "); return false;");
      tipStatus.InnerText = version.VersionStatus;
      tipReleased.InnerText = version.IsReleased ? "Yes" : "No";
      tipDate.InnerText = version.ReleaseDate == null ? "None" : ((DateTime)version.ReleaseDate).ToString("d");
      tipDesc.InnerHtml = version.Description;
    }

    private void EndResponse(string message)
    {
      Response.Write(message);
      Response.End();
    }
}