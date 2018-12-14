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

public partial class Dialogs_Version : BaseDialogPage
{
  private int _productID = -1;
  private int _versionID = -1;

  private CustomFieldControls _fieldControls;

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);
    _fieldControls = new CustomFieldControls(ReferenceType.ProductVersions, -1, 2, tblCustomControls, false);
    _fieldControls.LoadCustomControls(200);
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    
    _manager.AjaxSettings.AddAjaxSetting(editorDescription, editorDescription);

    if (Request["VersionID"] != null)
    {
      _versionID = int.Parse(Request["VersionID"]);
    }
    else
    {
      _productID = int.Parse(Request["ProductID"]);

    }

    _fieldControls.RefID = _versionID;

    if (!IsPostBack)
    {
      LoadStatuses();
      LoadProducts();
      LoadVersion(_versionID);
      _fieldControls.LoadValues();
      Page.RegisterStartupScript("AddMasks", @"
        <script type=""text/javascript"">
          $('.masked').each(function (index) {
            $(this).mask($(this).attr('placeholder'));
          });
        </script>
      ");
    }
    dpRelease.Culture = UserSession.LoginUser.CultureInfo;

    lblRelease.Text = cbReleased.Checked ? "Released On:" : "Expected Release On:";
  }

  private void LoadStatuses()
  {
    cmbStatus.Items.Clear();
    ProductVersionStatuses statuses = new ProductVersionStatuses(UserSession.LoginUser);
    statuses.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
    foreach (ProductVersionStatus status in statuses)
    {
      cmbStatus.Items.Add(new RadComboBoxItem(status.Name, status.ProductVersionStatusID.ToString()));
    }

    if (cmbStatus.Items.Count > 0) cmbStatus.SelectedIndex = 0;
  }

  private void LoadProducts()
  {
    cmbProduct.Items.Clear();
    Products products = new Products(UserSession.LoginUser);
    products.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
    foreach (Product product in products)
    {
      cmbProduct.Items.Add(new RadComboBoxItem(product.Name, product.ProductID.ToString()));
    }
    cmbProduct.SelectedValue = _productID.ToString();
    if (cmbProduct.Items.Count > 0) cmbProduct.SelectedIndex = 0;
  }

  private void LoadVersion(int versionID)
  {
    ProductVersions versions = new ProductVersions(UserSession.LoginUser);
    versions.LoadByProductVersionID(versionID);
    if (!versions.IsEmpty)
    {
      Product product = Products.GetProduct(UserSession.LoginUser, versions[0].ProductID);

      if (product.OrganizationID != UserSession.LoginUser.OrganizationID)
      {
        Response.Write("Invalid Request");
        Response.End();
        return;
      }

      textVersionNumber.Text = versions[0].VersionNumber;
      cmbProduct.SelectedValue = versions[0].ProductID.ToString();
      cmbStatus.SelectedValue = versions[0].ProductVersionStatusID.ToString();
      editorDescription.Content = versions[0].Description;
      cbReleased.Checked = versions[0].IsReleased;
      dpRelease.SelectedDate = versions[0].ReleaseDate;
    }
    else
    {
      textVersionNumber.Text = "";
      if (_productID < 0) cmbProduct.SelectedIndex = 0;
      else cmbProduct.SelectedValue = _productID.ToString();
      cmbStatus.SelectedIndex = 0;
      cbReleased.Checked = false;
      dpRelease.SelectedDate = DateTime.Today;
    }
  }

  public override bool Save()
  {
    if (textVersionNumber.Text.Trim() == "")
    {
      _manager.Alert("Please enter a version number.");
      return false;
    }

    ProductVersion version;
    ProductVersions versions = new ProductVersions(UserSession.LoginUser); ;
    if (_versionID < 0)
    {
      version = versions.AddNewProductVersion();
    }
    else
    {
      versions.LoadByProductVersionID(_versionID);
      if (versions.IsEmpty) return false;
      version = versions[0];
    }

    version.VersionNumber = textVersionNumber.Text;
    version.ProductVersionStatusID = int.Parse(cmbStatus.SelectedValue);
    version.ProductID = int.Parse(cmbProduct.SelectedValue);
    version.Description = editorDescription.Content;
    version.IsReleased = cbReleased.Checked;
    if (dpRelease.SelectedDate == null)
      version.ReleaseDate = null;
    else
    {
      DateTime date = new DateTime(((DateTime)dpRelease.SelectedDate).Ticks);
      version.ReleaseDate = DataUtils.DateToUtc(UserSession.LoginUser, date);
    }

    version.Collection.Save();
    (new OrganizationProducts(UserSession.LoginUser)).UpdateVersionProduct(version.ProductVersionID, version.ProductID);

    _fieldControls.RefID = version.ProductVersionID;
    _fieldControls.SaveCustomFields();
    
    return true;
  }


}
