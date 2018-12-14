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

public partial class Dialogs_Account : BaseDialogPage
{
  private int _organizatinID;

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    if (!UserSession.CurrentUser.IsTSUser)
    {
      Response.Write("");
      Response.End();
      return;
    }

    _organizatinID = int.Parse(Request["OrganizationID"]);

    if (!IsPostBack)
    {
      cmbProductType.Items.Add(new RadComboBoxItem(DataUtils.ProductTypeString(ProductType.Express), ((int)ProductType.Express).ToString()));
      cmbProductType.Items.Add(new RadComboBoxItem(DataUtils.ProductTypeString(ProductType.HelpDesk), ((int)ProductType.HelpDesk).ToString()));
      cmbProductType.Items.Add(new RadComboBoxItem(DataUtils.ProductTypeString(ProductType.BugTracking), ((int)ProductType.BugTracking).ToString()));
      cmbProductType.Items.Add(new RadComboBoxItem(DataUtils.ProductTypeString(ProductType.Enterprise), ((int)ProductType.Enterprise).ToString()));
      
      LoadOrganization(_organizatinID);
    }
  }

  private void LoadOrganization(int organizationID)
  {
    Organization organization = (Organization)Organizations.GetOrganization(UserSession.LoginUser, _organizatinID);
    BillingInfoItem item = (BillingInfoItem)BillingInfo.GetBillingInfoItem(UserSession.LoginUser, _organizatinID);
    if (item != null && organization != null)
    {
      cbFree.Checked = organization.IsCustomerFree;
      cmbProductType.SelectedValue = ((int)organization.ProductType).ToString();
      cbActive.Checked = organization.IsActive;
      cbAdvancedPortal.Checked = organization.IsAdvancedPortal;
      cbBasicPortal.Checked = organization.IsBasicPortal;
      cbAuto.Checked = item.IsAutomatic;
      numUserPrice.Value = (double?)item.UserPrice;
      numChatPrice.Value = (double?)item.ChatPrice;
      numPortalPrice.Value = (double?)item.PortalPrice;
      numBasicPortalPrice.Value = (double?)item.BasicPortalPrice;
      numStoragePrice.Value = (double?)item.StoragePrice;
      numUserSeats.Value = organization.UserSeats;
      numChatSeats.Value = organization.ChatSeats;
      numPortalSeats.Value = organization.PortalSeats;
      numStorage.Value = organization.ExtraStorageUnits;
      textInactive.Text = organization.InActiveReason;
      cbApiActive.Checked = organization.IsApiActive;
      cbApiEnabled.Checked = organization.IsApiEnabled;
      cbInventory.Checked = organization.IsInventoryEnabled;
    }
  }

  public override bool Save()
  {
    Organization organization = (Organization)Organizations.GetOrganization(UserSession.LoginUser, _organizatinID);
    BillingInfoItem item = (BillingInfoItem)BillingInfo.GetBillingInfoItem(UserSession.LoginUser, _organizatinID);
    if (item != null && organization != null)
    {
      organization.IsCustomerFree = cbFree.Checked;
      ProductType productType = (ProductType)int.Parse(cmbProductType.SelectedValue);
      if (productType != organization.ProductType) 
        UserSettings.DeleteOrganizationSettings(UserSession.LoginUser, organization.OrganizationID);
      organization.ProductType = productType;
      organization.IsActive = cbActive.Checked;
      organization.IsBasicPortal = cbBasicPortal.Checked;
      organization.IsAdvancedPortal = cbAdvancedPortal.Checked;
      item.IsAutomatic = cbAuto.Checked;
      item.UserPrice = numUserPrice.Value;
      item.ChatPrice = numChatPrice.Value;
      item.PortalPrice = numPortalPrice.Value;
      item.BasicPortalPrice = numBasicPortalPrice.Value;
      item.StoragePrice = numStoragePrice.Value;
      organization.UserSeats = (int)numUserSeats.Value;
      organization.ChatSeats = (int)numChatSeats.Value;
      organization.PortalSeats = (int)numPortalSeats.Value;
      organization.ExtraStorageUnits = (int)numStorage.Value;
      organization.InActiveReason = textInactive.Text;
      organization.IsApiEnabled = cbApiEnabled.Checked;
      organization.IsApiActive = cbApiActive.Checked;
      organization.IsInventoryEnabled = cbInventory.Checked;
      organization.Collection.Save();
      item.Collection.Save();
    }
    else
    {
      _manager.Alert("There was an error saving the account information");
    }

    return true;
  }




  protected void Page_Load(object sender, EventArgs e)
  {

  }
}
