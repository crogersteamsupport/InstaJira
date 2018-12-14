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


public partial class Frames_AccountInformation : BaseFramePage
{
  private int _organizationID;
  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    if (!UserSession.CurrentUser.IsTSUser)
    {
      Response.Write("");
      Response.End();
    }
    _organizationID = int.Parse(Request["OrganizationID"]);

    btnInfo.OnClientClick = "ShowDialog(top.GetAccountDialog(" + _organizationID.ToString() + ")); return false;";

    Addresses addresses = new Addresses(UserSession.LoginUser);
    addresses.LoadByID(_organizationID, ReferenceType.BillingInfo);

    if (addresses.IsEmpty)
    {
      btnAddress.OnClientClick = "ShowDialog(top.GetAddressDialog(" + _organizationID.ToString() + ", 24)); return false;";
    }
    else
    {
      btnAddress.OnClientClick = "ShowDialog(top.GetAddressDialog(" + addresses[0].AddressID.ToString() + ")); return false;";
    }

    LoadDetails();


  }

  private void LoadDetails()
  {
    VerifyBillingInfo();
    BillingInfoItem item = (BillingInfoItem)BillingInfo.GetBillingInfoItem(UserSession.LoginUser, _organizationID);
    Organization organization = (Organization)Organizations.GetOrganization(UserSession.LoginUser, _organizationID);
    StringBuilder builder = new StringBuilder();

    if (organization != null)
    {
     
      builder.Append("<tr><td style=\"width: 200px;\"><strong>Organization ID: </strong></td><td>");
      builder.Append(organization.OrganizationID.ToString() + "</td></tr>");
      builder.Append("<tr><td><strong>Is Customer Free: </strong></td><td>");
      builder.Append(organization.IsCustomerFree.ToString() + "</td></tr>");
      builder.Append("<tr><td><strong>Heard about us from: </strong></td><td>");
      builder.Append(organization.WhereHeard + "</td></tr>");
      builder.Append("<tr><td><strong>Product Type: </strong></td><td>");
      builder.Append(DataUtils.ProductTypeString(organization.ProductType) + "</td></tr>");
      builder.Append("<tr><td><strong>Is Active: </strong></td><td>");
      builder.Append(organization.IsActive.ToString() + "</td></tr>");
      builder.Append("<tr><td><strong>Inactive Reason: </strong></td><td>");
      builder.Append(organization.InActiveReason + "</td></tr>");
      //builder.Append("<tr><td style=\"width: 200px;\"><strong>Automatic Payment: </strong></td><td>");
      //builder.Append(item.IsAutomatic.ToString() + "</td></tr>");

      builder.Append("<tr><td><strong>User Seats: </strong></td><td>");
      builder.Append(organization.UserSeats.ToString() + "</td></tr>");
      builder.Append("<tr><td><strong>Users Used: </strong></td><td>");
      builder.Append(Organizations.GetUserCount(UserSession.LoginUser, _organizationID).ToString() + "</td></tr>");
      if (item.UserPrice == null)
      {
        builder.Append("<tr><td><strong>Override User Price: </strong></td><td>");
        builder.Append("Default</td></tr>");
      }
      else
      {
        builder.Append("<tr><td><strong>Override User Price: </strong></td><td>");
        builder.Append(item.UserPrice.ToString() + "</td></tr>");
      }

      builder.Append("<tr><td><strong>Portal Seats: </strong></td><td>");
      builder.Append(organization.PortalSeats.ToString() + "</td></tr>");
      builder.Append("<tr><td><strong>Portals Used: </strong></td><td>");
      builder.Append(Organizations.GetPortalCount(UserSession.LoginUser, _organizationID).ToString() + "</td></tr>");
      if (item.PortalPrice == null)
      {
        builder.Append("<tr><td><strong>Override Advanced Portal Price: </strong></td><td>");
        builder.Append("Default</td></tr>");
      }
      else
      {
        builder.Append("<tr><td><strong>Override Advanced Portal Price: </strong></td><td>");
        builder.Append(item.PortalPrice.ToString() + "</td></tr>");
      }

      if (item.BasicPortalPrice == null)
      {
        builder.Append("<tr><td><strong>Override Basic Portal Price: </strong></td><td>");
        builder.Append("Default</td></tr>");
      }
      else
      {
        builder.Append("<tr><td><strong>Override Basic Portal Price: </strong></td><td>");
        builder.Append(item.BasicPortalPrice.ToString() + "</td></tr>");
      }
      builder.Append("<tr><td><strong>Advanced Portal: </strong></td><td>");
      builder.Append(organization.IsAdvancedPortal.ToString() + "</td></tr>");
      builder.Append("<tr><td><strong>Basic Portal: </strong></td><td>");
      builder.Append(organization.IsBasicPortal.ToString() + "</td></tr>");
      
      builder.Append("<tr><td><strong>Extra Storage Units: </strong></td><td>");
      builder.Append(organization.ExtraStorageUnits.ToString() + "</td></tr>");
      builder.Append("<tr><td><strong>Storage Used: </strong></td><td>");
      builder.Append(Organizations.GetStorageUsed(UserSession.LoginUser, _organizationID).ToString() + "MB</td></tr>");
      if (item.StoragePrice == null)
      {
        builder.Append("<tr><td><strong>Override Storage Price: </strong></td><td>");
        builder.Append("Default</td></tr>");
      }
      else
      {
        builder.Append("<tr><td><strong>Override Storage Price: </strong></td><td>");
        builder.Append(item.StoragePrice.ToString() + "</td></tr>");
      }

      builder.Append("<tr><td><strong>Extra Storage Allowed: </strong></td><td>");
      builder.Append(Organizations.GetExtraStorageAllowed(UserSession.LoginUser, _organizationID) + "MB</td></tr>");
      builder.Append("<tr><td><strong>Base Storage Allowed: </strong></td><td>");
      builder.Append(Organizations.GetBaseStorageAllowed(UserSession.LoginUser, _organizationID) + "MB</td></tr>");
      builder.Append("<tr><td><strong>Total Storage Allowed: </strong></td><td>");
      builder.Append(Organizations.GetTotalStorageAllowed(UserSession.LoginUser, _organizationID) + "MB</td></tr>");





    /*  if (item.NextInvoiceDate == null)
      {
        builder.Append("<tr><td><strong>Next Invoice Date: </strong></td><td>");
        builder.Append("None</td></tr>");
      }
      else
      {
        builder.Append("<tr><td><strong>Next Invoice Date: </strong></td><td>");
        builder.Append(item.NextInvoiceDate.ToString() + "</td></tr>");
      }
      */
      
      
      builder.Append("<tr><td><strong>API Security Token: </strong></td><td>");
      builder.Append(organization.WebServiceID + "</td></tr>");
      if (organization.IsApiActive != null && organization.IsApiActive == true)
        builder.Append("<tr><td><strong>API Active: </strong></td><td>True</td></tr>");
      else
        builder.Append("<tr><td><strong>API Active: </strong></td><td>False</td></tr>");

      if (organization.IsApiEnabled)
        builder.Append("<tr><td><strong>API Enabled: </strong></td><td>True</td></tr>");
      else
        builder.Append("<tr><td><strong>API Enabled: </strong></td><td>False</td></tr>");

      string email = organization.SystemEmailID + "@teamsupport.com";
      builder.Append("<tr><td><strong>System Email: </strong></td><td>");
      builder.Append("<a href=\"mailto:" + email + "\">" + email + "</a>" + "</td></tr>");
      string portalLink = SystemSettings.GetPortalUrl()+"?OrganizationID=" + organization.OrganizationID.ToString();
      portalLink = @"<a href=""" + portalLink + @""" target=""PortalLink"" onclick=""window.open('" + portalLink + @"', 'PortalLink')"">" + portalLink + "</a>";
      builder.Append("<tr><td><strong>Portal Link: </strong></td><td>");
      builder.Append(portalLink + "</td></tr>");

      builder.Append("<tr><td><strong>Total Bill: </strong></td><td>");
      double total = 0;
      total = (organization.UserSeats-3) * (item.UserPrice == null ? 25.0 : (double)item.UserPrice);
      if (total < 0) total = 0;
      total = total + (organization.PortalSeats * (item.PortalPrice == null ? 3.0 : (double)item.PortalPrice));
      total = total + (organization.ExtraStorageUnits * (item.StoragePrice == null ? 3.0 : (double)item.StoragePrice));

      builder.Append("$" + total.ToString("#0.00") + "</td></tr>");
    }

    LoadAddress();
    if (item.CreditCardID != null) LoadCredit((int)item.CreditCardID);

    litAccount.Text = builder.ToString();
    
    
    
  }
  private void  LoadAddress()
  {
    Addresses addresses = new Addresses(UserSession.LoginUser);
    addresses.LoadByID(_organizationID, ReferenceType.BillingInfo);

    if (addresses.IsEmpty) return;

    StringBuilder builder = new StringBuilder();

    Address address = addresses[0];
    if (address != null)
    {
      builder.Append(address.Addr1 + "<br />");
      if (address.Addr2.Trim() != "")
      {
        builder.Append(address.Addr2 + "<br />");
        if (address.Addr3.Trim() != "") builder.Append(address.Addr3 + "<br />");
      }
      
      builder.Append(address.City + ", " + address.State + " &nbsp&nbsp " + address.Zip + "<br />");
      builder.Append(address.Country);
    }
    else
    {
      builder.Append("No address on file.");
    }
    
    litAddress.Text = builder.ToString();
  }
  private void LoadCredit(int creditID)
  {
    CreditCard creditCard = (CreditCard) CreditCards.GetCreditCard(UserSession.LoginUser, creditID);
    StringBuilder builder = new StringBuilder();

    builder.Append("<tr><td style=\"width: 200px;\"><strong>Name: </strong></td><td>");
    builder.Append(creditCard.NameOnCard + "</td></tr>");
    builder.Append("<tr><td><strong>Number: </strong></td><td>");
    builder.Append(creditCard.DisplayNumber + "</td></tr>");
    builder.Append("<tr><td><strong>Type: </strong></td><td>");
    builder.Append(DataUtils.CreditCardTypeString(creditCard.CreditCardType) + "</td></tr>");
    builder.Append("<tr><td><strong>Security Code: </strong></td><td>");
    builder.Append(creditCard.SecurityCode + "</td></tr>");
    builder.Append("<tr><td><strong>Expiration Date: </strong></td><td>");
    builder.Append(creditCard.ExpirationDate.ToString() + "</td></tr>");
    
    litCredit.Text = builder.ToString();
  
  }
  private void VerifyBillingInfo()
  {
    BillingInfoItem item = (BillingInfoItem)BillingInfo.GetBillingInfoItem(UserSession.LoginUser, _organizationID);
    if (item == null)
    {
      BillingInfo info = new BillingInfo(UserSession.LoginUser);
      item = info.AddNewBillingInfoItem();
      item.IsAutomatic = true;
      item.NextInvoiceDate = DateTime.UtcNow;
      item.OrganizationID = _organizationID;
      item.Collection.Save();
    }
  }

}
