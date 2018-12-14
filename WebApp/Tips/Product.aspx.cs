using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Text;

public partial class Tips_Product : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
      if (Request["ProductID"] == null) EndResponse("Invalid Product");

      int productID = int.Parse(Request["ProductID"]);
      Product product = Products.GetProduct(TSAuthentication.GetLoginUser(), productID);
      if (product == null) EndResponse("Invalid Product");

      if (product.OrganizationID != TSAuthentication.OrganizationID) EndResponse("Invalid Product");

      tipName.InnerText = product.Name;
      tipName.Attributes.Add("onclick", "top.Ts.MainPage.openNewProduct(" + productID.ToString() + "); return false;");

      tipDesc.InnerHtml = product.Description;
    }

    private void EndResponse(string message)
    {
      Response.Write(message);
      Response.End();
    }
}