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

public partial class Dialogs_PhoneNumber : BaseDialogPage
{
  private int _phoneID = -1;
  private int _refID = -1;
  private ReferenceType _refType;

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    if (Request["PhoneID"] != null)
    {
      _phoneID = int.Parse(Request["PhoneID"]);
    }
    else
    {
      _refID = int.Parse(Request["RefID"]);
      _refType = (ReferenceType)int.Parse(Request["RefType"]);
    }


    if (!IsPostBack)
    {
      LoadPhoneTypes();
      if (_phoneID > -1) LoadPhone(_phoneID);
    }
  }

  private void LoadPhone(int phoneID)
  {
    PhoneNumber phoneNumber = (PhoneNumber)PhoneNumbers.GetPhoneNumber(UserSession.LoginUser, phoneID);

    if (phoneNumber != null)
    {
      edtPhoneNumber.Text = phoneNumber.Number;
      edtExtension.Text = phoneNumber.Extension;
      cmbTypes.SelectedValue = phoneNumber.PhoneTypeID.ToString();
    }
  }

  public override bool Save()
  {
    if (string.IsNullOrEmpty(edtPhoneNumber.Text.Trim()))
    {
      _manager.Alert("Please enter a phone number.");
      return false;
    }

    PhoneNumbers phoneNumbers = new PhoneNumbers(UserSession.LoginUser);

    if (_phoneID > -1)
    {
      phoneNumbers.LoadByPhoneID(_phoneID);
      if (!phoneNumbers.IsEmpty)
      {
        PhoneNumber phoneNumber = phoneNumbers[0];
        phoneNumber.Extension = edtExtension.Text;
        phoneNumber.Number = edtPhoneNumber.Text;
        phoneNumber.PhoneTypeID = int.Parse(cmbTypes.SelectedValue);
      }
    }
    else
    {
      PhoneNumber phoneNumber = phoneNumbers.AddNewPhoneNumber();
      phoneNumber.RefID = _refID;
      phoneNumber.RefType = _refType;
      phoneNumber.Extension = edtExtension.Text;
      phoneNumber.Number = edtPhoneNumber.Text;
      phoneNumber.PhoneTypeID = int.Parse(cmbTypes.SelectedValue);
    }

    phoneNumbers.Save();
    return true;
  }

  private void LoadPhoneTypes()
  {
    cmbTypes.Items.Clear();
    PhoneTypes phoneTypes = new PhoneTypes(UserSession.LoginUser);
    phoneTypes.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
    foreach (PhoneType phoneType in phoneTypes)
    {
      cmbTypes.Items.Add(new RadComboBoxItem(phoneType.Name, phoneType.PhoneTypeID.ToString()));
    }
  }
}
