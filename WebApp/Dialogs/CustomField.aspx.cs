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
using System.Text;

public partial class Dialogs_CustomField : BaseDialogPage
{
  private int _customFieldID = -1;
  private int _auxID = -1;
  private ReferenceType _refType = ReferenceType.None;

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    if (Request["CustomFieldID"] != null)
    {
      _customFieldID = int.Parse(Request["CustomFieldID"]);
    }

      _refType = (ReferenceType)int.Parse(Request["RefType"]);
      if (Request["AuxID"] != null)
      {
        _auxID = int.Parse(Request["AuxID"]);
      }
      
    if (!IsPostBack)
    {
      LoadFieldTypes();
      if (_refType == ReferenceType.Tickets)
      {
        LoadParentFields();
        LoadParentProducts();
      }
      else
      {
        parentFields.Visible = false;
      }

      if (_customFieldID > -1)
      {
        LoadCustomField(_customFieldID);
      }
    }

    _manager.AjaxSettings.AddAjaxSetting(comboFieldType, divMain);
    _manager.AjaxSettings.AddAjaxSetting(comboParentField, comboParentValue);

    CustomFieldType selectedFieldType = GetSelectedFieldType();
    pnlPickList.Visible = selectedFieldType == CustomFieldType.PickList;
    cbIsRequired.Visible = selectedFieldType != CustomFieldType.Boolean;
    if (_refType == ReferenceType.Tickets)
    {
      cbIsRequiredToClose.Visible = selectedFieldType != CustomFieldType.Boolean;
    }
    else
    {
      cbIsRequiredToClose.Visible = false;
    }

    if (_refType == ReferenceType.Assets)
    {
      cbIsVisibleOnPortal.Visible = false;
    }

    maskDiv.Visible = selectedFieldType == CustomFieldType.Text;
  }

  private CustomFieldType GetSelectedFieldType()
  {
    return (CustomFieldType)(int.Parse(comboFieldType.SelectedValue));
  }

  private int? GetSelectedParentCustomFieldID()
  {
      if (String.IsNullOrEmpty(comboParentField.SelectedValue) || comboParentField.SelectedValue == "-1")
      {
          return null;
      }
      else
      {
          return int.Parse(comboParentField.SelectedValue);
      }
  }

  private string GetSelectedParentCustomValue()
  {
      if (String.IsNullOrEmpty(comboParentField.SelectedValue) || comboParentField.SelectedValue == "-1")
      {
          return null;
      }
      else
      {
          CustomFields parentCustomField = new CustomFields(UserSession.LoginUser);
          parentCustomField.LoadByCustomFieldID(int.Parse(comboParentField.SelectedValue));
          if (parentCustomField[0].IsFirstIndexSelect && comboParentValue.SelectedIndex == 0)
          {
              return null;
          }
          else
          {
              return comboParentValue.SelectedItem.Text;
          }
      }
  }

  private int? GetSelectedParentProductID()
  {
      if (String.IsNullOrEmpty(comboParentProduct.SelectedValue) || comboParentProduct.SelectedValue == "-1")
      {
          return null;
      }
      else
      {
          return int.Parse(comboParentProduct.SelectedValue);
      }
  }

  private void LoadFieldTypes()
  {
    comboFieldType.Items.Clear();
    comboFieldType.Items.Add(new RadComboBoxItem(CustomFields.GetCustomFieldTypeName(CustomFieldType.Text), ((int)CustomFieldType.Text).ToString()));
    comboFieldType.Items.Add(new RadComboBoxItem(CustomFields.GetCustomFieldTypeName(CustomFieldType.Number), ((int)CustomFieldType.Number).ToString()));
    comboFieldType.Items.Add(new RadComboBoxItem(CustomFields.GetCustomFieldTypeName(CustomFieldType.PickList), ((int)CustomFieldType.PickList).ToString()));
    comboFieldType.Items.Add(new RadComboBoxItem(CustomFields.GetCustomFieldTypeName(CustomFieldType.Boolean), ((int)CustomFieldType.Boolean).ToString()));
    comboFieldType.Items.Add(new RadComboBoxItem(CustomFields.GetCustomFieldTypeName(CustomFieldType.DateTime), ((int)CustomFieldType.DateTime).ToString()));
    comboFieldType.Items.Add(new RadComboBoxItem(CustomFields.GetCustomFieldTypeName(CustomFieldType.Date), ((int)CustomFieldType.Date).ToString()));
    comboFieldType.Items.Add(new RadComboBoxItem(CustomFields.GetCustomFieldTypeName(CustomFieldType.Time), ((int)CustomFieldType.Time).ToString()));
  }

  private void LoadParentFields()
  {
    int? catID = Request["CatID"] == null ? null : (int?)int.Parse(Request["CatID"]);
    CustomFields pickListCustomFields = new CustomFields(UserSession.LoginUser);
    pickListCustomFields.LoadByOrganizationFieldTypeAndTicketType(UserSession.LoginUser.OrganizationID, CustomFieldType.PickList, _auxID, _customFieldID, catID);

    if (pickListCustomFields.Count > 0 )
    {
      comboParentField.Items.Clear();
      comboParentField.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
      foreach (CustomField field in pickListCustomFields)
      {
        comboParentField.Items.Add(new RadComboBoxItem(field.Name, field.CustomFieldID.ToString()));
      }
    }
    else
    {
      parentPickList.Visible = false;
    }
  }

  private void LoadParentValues(string values)
  {
    if (values != string.Empty)
    {
      parentValue.Visible = true;
      comboParentValue.Items.Clear();
      char[] delimitedChars = {'|'};
      string[] separatedValues = values.Split(delimitedChars);
      for (int i = 0; i < separatedValues.Length; i++) 
      {
        comboParentValue.Items.Add(new RadComboBoxItem(separatedValues[i], separatedValues[i]));
      }
    }
    else
    {
      parentValue.Visible = false;
    }
  }

  private void LoadParentProducts()
  {
    Organizations organization = new Organizations(UserSession.LoginUser);
    organization.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);

    if (organization[0].ProductType == ProductType.Enterprise || organization[0].ProductType == ProductType.BugTracking)
    {
      Products products = new Products(UserSession.LoginUser);
      products.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);

      if (products.Count > 0)
      {
        comboParentProduct.Items.Clear();
        comboParentProduct.Items.Add(new RadComboBoxItem("Unassigned", "-1"));
        foreach (Product product in products)
        {
          comboParentProduct.Items.Add(new RadComboBoxItem(product.Name, product.ProductID.ToString()));
        }
      }
      else
      {
        parentProduct.Visible = false;
      }
    }
    else
    {
      parentProduct.Visible = false;
    }
  }

  private void LoadCustomField(int customFieldID)
  {
    CustomFields fields = new CustomFields(UserSession.LoginUser);
    fields.LoadByCustomFieldID(customFieldID);
    if (fields.IsEmpty) return;

    if (fields[0].OrganizationID != UserSession.LoginUser.OrganizationID)
    {
      Response.Write("Invalid Request");
      Response.End();
      return;
    }
    
    textDescription.Text = fields[0].Description;
    textName.Text = fields[0].Name;
    textApiFieldName.Text = fields[0].ApiFieldName;
    textList.Text = fields[0].ListValues.Replace("|", "\n");
    if (fields[0].ParentCustomFieldID == null)
    {
      comboParentField.SelectedValue = "-1";
    }
    else
    {
      comboParentField.SelectedValue = ((int)fields[0].ParentCustomFieldID).ToString();
      CustomFields parentCustomField = new CustomFields(UserSession.LoginUser);
      parentCustomField.LoadByCustomFieldID((int)fields[0].ParentCustomFieldID);
      LoadParentValues(parentCustomField[0].ListValues);
      if (fields[0].ParentCustomValue != null)
      {
        comboParentValue.SelectedValue = fields[0].ParentCustomValue;
      }
    }
    if (fields[0].ParentProductID == null)
    {
      comboParentProduct.SelectedValue = "-1";
    }
    else
    {
      comboParentProduct.SelectedValue = ((int)fields[0].ParentProductID).ToString();
    }
    comboFieldType.SelectedValue = ((int)fields[0].FieldType).ToString();
    cbIsVisibleOnPortal.Checked = fields[0].IsVisibleOnPortal == null ? false : (bool)fields[0].IsVisibleOnPortal;
    cbIsRequired.Checked = fields[0].IsRequired;
    cbFirstSelect.Checked = fields[0].IsFirstIndexSelect;
    cbIsRequiredToClose.Checked = fields[0].IsRequiredToClose;
    textMask.Text = fields[0].Mask;
    _refType = fields[0].RefType;
  }

  public override bool Save()
  {
    if (textName.Text.Trim() == "")
    {
      _manager.Alert("Please give the custom field a name.");
      return false;
    }

    string apiFieldName = GetApiFieldName();

    if (apiFieldName == "")
    {
      _manager.Alert("Please give the custom field an API field name.");
      return false;
    }

    if (FieldExists(apiFieldName)) return false;


    int? catID = Request["CatID"] == null ? null : (int?)int.Parse(Request["CatID"]);

    CustomFields fields = new CustomFields(UserSession.LoginUser);
    CustomField field;

    if (_customFieldID < 0)
    {
      field = fields.AddNewCustomField();
      field.OrganizationID = UserSession.LoginUser.OrganizationID;
      field.RefType = _refType;
      field.AuxID = _auxID;
      field.CustomFieldCategoryID = catID;
      field.Position = fields.GetMaxPosition(UserSession.LoginUser.OrganizationID, _refType, _auxID) + 1;
    }
    else
    {
      fields.LoadByCustomFieldID(_customFieldID);
      if (fields.IsEmpty) return false;
      field = fields[0];
    }

    field.Description = textDescription.Text;
    field.Name = textName.Text;
    field.ApiFieldName = apiFieldName;
    field.FieldType = GetSelectedFieldType();
    field.ParentCustomFieldID = GetSelectedParentCustomFieldID();
    field.ParentCustomValue = GetSelectedParentCustomValue();
    field.ParentProductID = GetSelectedParentProductID();
    field.IsVisibleOnPortal = cbIsVisibleOnPortal.Checked;
    field.IsFirstIndexSelect = cbFirstSelect.Checked;
    field.IsRequired = cbIsRequired.Checked;
    field.IsRequiredToClose = cbIsRequiredToClose.Checked;
    field.Mask = textMask.Text;

    string list = textList.Text.Replace("\n", "|");
    if (list != "")
    {
      string[] items = list.Split('|');

      CommaDelimitedStringCollection collection = new CommaDelimitedStringCollection();

      StringBuilder builder = new StringBuilder();
      for (int i = 0; i < items.Length; i++)
			{
    		 builder.Append(items[i].Trim());
         if (i < items.Length-1) builder.Append("|");
			}

      field.ListValues = builder.ToString();
    }
    else
    {
      field.ListValues = list;
    }
    field.Collection.Save();

    ActionLogs.AddActionLog(UserSession.LoginUser, ActionLogType.Update, ReferenceType.CustomFields, field.CustomFieldID, string.Format(
      "{0} updated custom field \"{1}\" ({2})",
      Users.GetUserFullName(UserSession.LoginUser, UserSession.LoginUser.UserID), field.Name, field.CustomFieldID));

    return true;
  }

  private string GetApiFieldName()
  {
    string name = textApiFieldName.Text.Trim();
    if (name == "") 
    {
      name = textName.Text;
    }
    return CustomFields.GenerateApiFieldName(name);
  }

  private bool FieldExists(string apiFieldName)
  {
    CustomFields fields = new CustomFields(UserSession.LoginUser);
    fields.LoadByReferenceType(UserSession.LoginUser.OrganizationID, _refType, _auxID);

    foreach (CustomField field in fields)
    {
      if (field.ApiFieldName.Trim().ToLower() == apiFieldName.Trim().ToLower() && _customFieldID != field.CustomFieldID)
      {
        _manager.Alert("Please choose a unique API field name for this custom field.");
        return true;
      }

      if (field.Name.Trim().ToLower() == textName.Text.Trim().ToLower() && _customFieldID != field.CustomFieldID)
      {
        _manager.Alert("Please choose a unique name for this custom field.");
        return true;
      }
    }
    return false;
  }

  protected void comboParentField_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
  {
      CustomFields selectedCustomField = new CustomFields(UserSession.LoginUser);
      selectedCustomField.LoadByCustomFieldID((Convert.ToInt32(comboParentField.SelectedValue)));
      if (selectedCustomField.IsEmpty)
      {
          LoadParentValues(string.Empty);
          return;
      }

      if (selectedCustomField[0].OrganizationID != UserSession.LoginUser.OrganizationID)
      {
          Response.Write("Invalid Request");
          Response.End();
          return;
      }

      LoadParentValues(selectedCustomField[0].ListValues);
  }
}
