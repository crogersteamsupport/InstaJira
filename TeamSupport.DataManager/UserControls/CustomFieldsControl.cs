using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TeamSupport.Data;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace TeamSupport.DataManager.UserControls
{
  public partial class CustomFieldsControl : BaseOrganizationUserControl
  {
    public CustomFieldsControl()
    {
      InitializeComponent();
      gridFields.MasterGridViewTemplate.AutoGenerateColumns = false;
    }

    private ReferenceType GetSelectedFieldType()
    {
      return (ReferenceType)((NamObjectItem)cmbFieldTypes.SelectedItem).Value;
    }

    private int GetSelectedTicketType()
    {
      if (cmbTicketType.SelectedItem == null) return -1;
      return (int)((NamObjectItem)cmbTicketType.SelectedItem).Value;
    }
    
    protected override void LoadOrganization(TeamSupport.Data.Organization organization)
    {
      base.LoadOrganization(organization);
      LoadTicketTypes(organization);
      LoadCustomFieldTypes();
      LoadCustomFields();
    }
    
    private void LoadCustomFields()
    {
      int oldID = GetSelectedFieldID();
      CustomFields customFields = new CustomFields(LoginSession.LoginUser);
      if (GetSelectedFieldType() == ReferenceType.Tickets)
        customFields.LoadByReferenceType(OrganizationID, GetSelectedFieldType(), GetSelectedTicketType());
      else
        customFields.LoadByReferenceType(OrganizationID, GetSelectedFieldType());
      
      DataTable table = new DataTable();
      table.Columns.Add("CustomFieldID", System.Type.GetType("System.Int32"));
      table.Columns.Add("Name");
      table.Columns.Add("DataType");
      
      foreach (CustomField field in customFields)
	    {
	      
	      DataRow row = table.NewRow();
	      row[0] = field.CustomFieldID;
	      row[1] = field.Name;
	      row[2] = DataUtils.CustomFieldTypeString(field.FieldType);
		     table.Rows.Add(row);
	    }
      
      gridFields.DataSource = table;
      if (gridFields.Rows.Count > 0) gridFields.Rows[0].IsCurrent = true;
      SetSelectedFieldID(oldID);
    }
    
    private int GetSelectedFieldID()
    {
      try
      {
        if (gridFields.CurrentRow == null || gridFields.CurrentRow.Cells.Count < 1 || gridFields.CurrentRow.Cells[0].Value == null) return -1;
        return (int)gridFields.CurrentRow.Cells[0].Value;
      }
      catch (Exception)
      {

        return -1;
      }
    
    }
    
    private void SetSelectedFieldID(int id)
    {
      foreach (GridViewRowInfo info in gridFields.Rows)
      {
        if ((int)info.Cells[0].Value == id)
        {
          info.IsCurrent = true;
          break;
        }
      }
    }
    
    private void LoadCustomFieldTypes()
    {
      cmbFieldTypes.Items.Clear();

      cmbFieldTypes.Items.Add(new NamObjectItem("Tickets", ReferenceType.Tickets));
      cmbFieldTypes.Items.Add(new NamObjectItem("Users", ReferenceType.Users));
      cmbFieldTypes.Items.Add(new NamObjectItem("Products", ReferenceType.Products));
      cmbFieldTypes.Items.Add(new NamObjectItem("Product Versions", ReferenceType.ProductVersions));
      cmbFieldTypes.Items.Add(new NamObjectItem("Customers", ReferenceType.Organizations));
      cmbFieldTypes.Items.Add(new NamObjectItem("Customer Products", ReferenceType.OrganizationProducts));
      cmbFieldTypes.Items.Add(new NamObjectItem("Customer Contacts", ReferenceType.Contacts));
      cmbFieldTypes.SelectedIndex = 0;
    }
    
    private void LoadTicketTypes(Organization organization)
    {
      if (organization == null) return;
      cmbTicketType.Items.Clear();
      
      TicketTypes ticketTypes = new TicketTypes(LoginSession.LoginUser);
      ticketTypes.LoadByOrganizationID(OrganizationID, organization.ProductType);

      foreach (TicketType type in ticketTypes)
      {
        cmbTicketType.Items.Add(new NamObjectItem(type.Name, type.TicketTypeID));
      }
      
      if (cmbTicketType.Items.Count > 0) cmbTicketType.SelectedIndex = 0;
    
    }

    private void cmbTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
      LoadCustomFields();
      cmbTicketType.Visible = lblTicketType.Visible = GetSelectedFieldType() == ReferenceType.Tickets;
      
    }

    private void btnUp_Click(object sender, EventArgs e)
    {
      CustomFields customFields = new CustomFields(LoginSession.LoginUser);
      customFields.MovePositionUp(GetSelectedFieldID());
      LoadCustomFields();
    }

    private void btnDown_Click(object sender, EventArgs e)
    {
      CustomFields customFields = new CustomFields(LoginSession.LoginUser);
      customFields.MovePositionDown(GetSelectedFieldID());
      LoadCustomFields();
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      CustomFieldForm.ShowCustomField(OrganizationID, GetSelectedFieldType(), GetSelectedTicketType());
      LoadCustomFields();
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
      CustomFieldForm.ShowCustomField(OrganizationID, GetSelectedFieldID());
      LoadCustomFields();
    }

    private void gridFields_DoubleClick(object sender, EventArgs e)
    {
      CustomFieldForm.ShowCustomField(OrganizationID, GetSelectedFieldID());
      LoadCustomFields();
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("Are you sure you would like to delete this custom field?", "Delete Custom Field", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
      CustomField customField = (CustomField)CustomFields.GetCustomField(LoginSession.LoginUser, GetSelectedFieldID());
      if (customField != null)
      {
        customField.Delete();
        customField.Collection.Save();
        LoadCustomFields();
      }
    }
    
    
  }
}
