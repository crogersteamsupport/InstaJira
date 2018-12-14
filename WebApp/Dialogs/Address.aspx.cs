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

public partial class Dialogs_Address : BaseDialogPage
{
  private int _addressID = -1;
  private int _refID = -1;
  private ReferenceType _refType = ReferenceType.None;

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    if (Request["AddressID"] != null)
    {
      _addressID = int.Parse(Request["AddressID"]);
    }
    else
    {
      _refID = int.Parse(Request["RefID"]);
      _refType = (ReferenceType)int.Parse(Request["RefType"]);
    }

    if (!IsPostBack && _addressID > -1)
    {
      LoadAddress(_addressID);
    }
  }

  private void LoadAddress(int addressID)
  {
    Address address = (Address)Addresses.GetAddress(UserSession.LoginUser, addressID);

    if (address != null)
    {
      edtDesicription.Text = address.Description;
      edtLine1.Text = address.Addr1;
      edtLine2.Text = address.Addr2;
      edtLine3.Text = address.Addr3;
      edtCity.Text = address.City;
      edtState.Text = address.State;
      edtZip.Text = address.Zip;
      edtCountry.Text = address.Country;
    }
  }

  public override bool Save()
  {
    if (string.IsNullOrEmpty(edtDesicription.Text.Trim()))
    {
      _manager.Alert("Please enter a description for this address.");
      return false;
    }

    Addresses addresses = new Addresses(UserSession.LoginUser);

    if (_addressID > -1)
    {
      addresses.LoadByAddressID(_addressID);
      if (!addresses.IsEmpty)
      {
        Address address = addresses[0];
        address.Description = edtDesicription.Text;
        address.Addr1 = edtLine1.Text;
        address.Addr2 = edtLine2.Text;
        address.Addr3 = edtLine3.Text;
        address.City = edtCity.Text;
        address.State = edtState.Text;
        address.Zip = edtZip.Text;
        address.Country = edtCountry.Text;
      }
    }
    else
    {
      Address address = addresses.AddNewAddress();
      address.RefID = _refID;
      address.RefType = _refType;
      address.Addr1 = edtLine1.Text;
      address.Addr2 = edtLine2.Text;
      address.Addr3 = edtLine3.Text;
      address.City = edtCity.Text;
      address.Country = edtCountry.Text;
      address.Description = edtDesicription.Text;
      address.State = edtState.Text;
      address.Zip = edtZip.Text;
    }
    addresses.Save();
    return true;

    
  }
}
