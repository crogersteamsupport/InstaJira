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
using TeamSupport.WebUtils;
using TeamSupport.Data;
using Telerik.Web.UI;


public partial class Dialogs_Product : BaseDialogPage
{

  private int _productID = -1;
  private CustomFieldControls _fieldControls;

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);
    _fieldControls = new CustomFieldControls(ReferenceType.Products, -1, 2, tblCustomControls, false);
    _fieldControls.LoadCustomControls(200);

  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    if (Request["ProductID"] != null) _productID = int.Parse(Request["ProductID"]);

    _fieldControls.RefID = _productID;
    if (!IsPostBack)
    {
      LoadProduct(_productID);
      _fieldControls.LoadValues();
      Page.RegisterStartupScript("AddMasks", @"
        <script type=""text/javascript"">
          $('.masked').each(function (index) {
            $(this).mask($(this).attr('placeholder'));
          });
        </script>
      ");
    }
  }

  private void LoadProduct(int productID)
  {
    Products products = new Products(UserSession.LoginUser);
    products.LoadByProductID(productID);
    if (products.IsEmpty) return;
    
    if (products[0].OrganizationID != UserSession.LoginUser.OrganizationID)
    {
      Response.Write("Invalid Request");
      Response.End();
      return;
    }
    textName.Text = products[0].Name;
    textDescription.Text = products[0].Description;
  }

  public override bool Save()
  {

    if (textName.Text.Trim() == "")
    {
      _manager.Alert("Please enter a name for your product.");
      return false;
    }

    Product product;
    Products products = new Products(UserSession.LoginUser); ;
    if (_productID < 0)
    {
      product = products.AddNewProduct();
      product.OrganizationID = UserSession.LoginUser.OrganizationID;
    }
    else
    {
      products.LoadByProductID(_productID);
      if (products.IsEmpty) return false;
      product = products[0];
    }

    product.Name = textName.Text;
    product.Description = textDescription.Text;
    product.Collection.Save();

    _fieldControls.RefID = product.ProductID;
    _fieldControls.SaveCustomFields();
    
    return true;
  }

  
}
