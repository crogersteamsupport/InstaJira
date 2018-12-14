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
using System.Web.Services;

public partial class Dialogs_OrganizationProduct : BaseDialogPage
{
  private int _organizationProductID = -1;
  private int _productID = -1;
  private int _versionID = -1;
  private int _organizationID = -1;

  private CustomFieldControls _fieldControls;


  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);

    _fieldControls = new CustomFieldControls(ReferenceType.OrganizationProducts, -1, 2, tblCustomControls, false);
    _fieldControls.LoadCustomControls(200, 1, null);

  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    _manager.AjaxSettings.AddAjaxSetting(cmbProducts, cmbVersions);


    _organizationProductID = Request["OrganizationProductID"] != null ? int.Parse(Request["OrganizationProductID"]) : -1;
    _productID = Request["ProductID"] != null ? int.Parse(Request["ProductID"]) : -1;
    _versionID = Request["VersionID"] != null ? int.Parse(Request["VersionID"]) : -1;
    _organizationID = Request["OrganizationID"] != null ? int.Parse(Request["OrganizationID"]) : -1;
    _fieldControls.RefID = _organizationProductID;
    if (!IsPostBack)
    {
      LoadOrganizationProduct(_organizationProductID);
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

  private void LoadOrganizationProduct(int organizationProductID)
  {
    //LoadCustomers();
    LoadProducts();

    RadComboBoxItem item = new RadComboBoxItem();
    item.Selected = true;


    OrganizationProduct organizationProduct = (OrganizationProduct)OrganizationProducts.GetOrganizationProduct(UserSession.LoginUser, organizationProductID);
    if (organizationProduct != null)
    {
      LoadVersions(organizationProduct.ProductID);
      cmbProducts.SelectedValue = organizationProduct.ProductID.ToString();
      //cmbCustomers.SelectedValue = organizationProduct.OrganizationID.ToString();
      dtExpiration.SelectedDate = organizationProduct.SupportExpiration;
      item.Value = organizationProduct.OrganizationID.ToString();
      item.Text = (Organizations.GetOrganization(UserSession.LoginUser, organizationProduct.OrganizationID)).Name;
      if (organizationProduct.ProductVersionID != null)
        cmbVersions.SelectedValue = organizationProduct.ProductVersionID.ToString();
    }
    else
    {
      cmbProducts.SelectedValue = _productID.ToString();
      //cmbCustomers.SelectedValue = _organizationID.ToString();
      item.Value = _organizationID.ToString();
      if (_organizationID > -1) item.Text = (Organizations.GetOrganization(UserSession.LoginUser, _organizationID)).Name;
      LoadVersions(int.Parse(cmbProducts.SelectedValue));
      cmbVersions.SelectedValue = _versionID.ToString();
      dtExpiration.SelectedDate = null;
    }
    cmbCustomers.Items.Add(item);
  }
  
  private void LoadCustomers()
  {
      cmbCustomers.Items.Clear();
      Organizations organizations = new Organizations(UserSession.LoginUser);
      organizations.LoadByParentID(UserSession.LoginUser.OrganizationID, true);
      foreach (Organization organization in organizations)
      {
        cmbCustomers.Items.Add(new RadComboBoxItem(organization.Name, organization.OrganizationID.ToString()));
      }
  }
  
  private void LoadProducts()
  {
    cmbProducts.Items.Clear();
    Products products = new Products(UserSession.LoginUser);
    products.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
    foreach (Product product in products)
    {
      cmbProducts.Items.Add(new RadComboBoxItem(product.Name, product.ProductID.ToString()));
    }
  }
  
  private void LoadVersions(int productID)
  {
    cmbVersions.Items.Clear();
    cmbVersions.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
    ProductVersions productVersions = new ProductVersions(UserSession.LoginUser);
    productVersions.LoadByProductID(productID);
    foreach (ProductVersion version in productVersions)
    {
      cmbVersions.Items.Add(new RadComboBoxItem(version.VersionNumber, version.ProductVersionID.ToString()));
    }
    cmbVersions.SelectedIndex = 0;
  }

  public override bool Save()
  {
    OrganizationProduct organizationProduct = null;
    
    if (_organizationProductID < 0)
    {
      organizationProduct = (new OrganizationProducts(UserSession.LoginUser)).AddNewOrganizationProduct();
    }
    else
    {
      organizationProduct = (OrganizationProduct)OrganizationProducts.GetOrganizationProduct(UserSession.LoginUser, _organizationProductID);
    }
    
    organizationProduct.OrganizationID = int.Parse(cmbCustomers.SelectedValue);
    organizationProduct.ProductID = int.Parse(cmbProducts.SelectedValue);
    if (cmbVersions.SelectedIndex > 0)
      organizationProduct.ProductVersionID =  int.Parse(cmbVersions.SelectedValue);
    else  
      organizationProduct.ProductVersionID = null;
    organizationProduct.SupportExpiration = DataUtils.DateToUtc(UserSession.LoginUser, dtExpiration.SelectedDate);
    organizationProduct.Collection.Save();

    _fieldControls.RefID = organizationProduct.OrganizationProductID;
    _fieldControls.SaveCustomFields();
  
    return true;
  }

  protected void cmbProducts_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
  {
    LoadVersions(int.Parse(cmbProducts.SelectedValue));
  }

  [WebMethod]
  public static RadComboBoxItemData[] GetOrganization(RadComboBoxContext context)
  {
    IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;

    Organizations organizations = new Organizations(UserSession.LoginUser);
    organizations.LoadByLikeOrganizationName(UserSession.LoginUser.OrganizationID, context["FilterString"].ToString(), !UserSession.CurrentUser.IsSystemAdmin, 250);

    List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();

    foreach (Organization organization in organizations)
    {
      RadComboBoxItemData itemData = new RadComboBoxItemData();
      itemData.Text = organization.Name;
      itemData.Value = organization.OrganizationID.ToString();
      list.Add(itemData);
    }

    return list.ToArray();
  }

}
