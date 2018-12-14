using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TeamSupport.Data;

namespace TeamSupport.DataManager
{
  public partial class CustomFieldForm : Form
  {
    private int _customFieldID;
    private int _auxID;
    private int _organizationID;
    private ReferenceType _refType;
    
    public CustomFieldForm()
    {
      InitializeComponent();
    }
    
    public static bool ShowCustomField(int organizationID, int customFieldID)
    {
      CustomFieldForm form = new CustomFieldForm();
      form._customFieldID = customFieldID;
      form._organizationID = organizationID;
      form._auxID = -1;
      return form.ShowDialog() == DialogResult.OK;
    }

    public static bool ShowCustomField(int organizationID, ReferenceType refType, int auxID)
    {
      CustomFieldForm form = new CustomFieldForm();
      form._organizationID = organizationID;
      form._customFieldID = -1;
      form._auxID = auxID;
      form._refType = refType;
      return form.ShowDialog() == DialogResult.OK;
    }
    
    private void LoadCustomField()
    {
      if (_customFieldID > -1)
      {
        CustomField customField = (CustomField)CustomFields.GetCustomField(LoginSession.LoginUser, _customFieldID);
        if (customField != null)
        {
          textName.Text = customField.Name;
          textDescription.Text = customField.Description;
          textList.Text = customField.ListValues;
          cmbType.SelectedIndex = (int)customField.FieldType;
        }
      
      }
      
    
    }
    
    private void SaveCustomField()
    {
      CustomFields customFields = new CustomFields(LoginSession.LoginUser);
      
      CustomField customField = null;
      
      if (_customFieldID < 0)
      {
        customField = customFields.AddNewCustomField();
        customField.AuxID = _auxID;
        customField.RefType = _refType;
        customField.OrganizationID = _organizationID;
        customField.Position = customFields.GetMaxPosition(_organizationID, _refType, _auxID)+1;
      }
      else
      {
        customFields.LoadByCustomFieldID(_customFieldID);
        customField = customFields[0];
      }

      customField.Description = textDescription.Text;
      customField.FieldType = (CustomFieldType)cmbType.SelectedIndex;
      customField.ListValues = textList.Text;
      customField.Name = textName.Text;
      customFields.Save();
    }

    private void CustomFieldForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (DialogResult ==  DialogResult.OK) SaveCustomField();
    }

    private void CustomFieldForm_Load(object sender, EventArgs e)
    {
      cmbType.Items.Clear();
      foreach(CustomFieldType type in Enum.GetValues(typeof(CustomFieldType)))
      {
        cmbType.Items.Add(DataUtils.CustomFieldTypeString(type));  
      }
      cmbType.SelectedIndex = 0;

      LoadCustomField();
    }

    private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
    {
      lblList.Visible = textList.Visible = (CustomFieldType)cmbType.SelectedIndex == CustomFieldType.PickList;
    }
    
    
  }
}
