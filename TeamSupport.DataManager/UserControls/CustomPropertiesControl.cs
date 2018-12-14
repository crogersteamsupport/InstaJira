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
  public partial class CustomPropertiesControl : BaseOrganizationUserControl
  {
    public CustomPropertiesControl()
    {
      InitializeComponent();
      gridProperties.MasterGridViewTemplate.AutoGenerateColumns = false;
    }

    private ReferenceType GetSelectedPropertyType()
    {
      return (ReferenceType)((NamObjectItem)cmbPropertyTypes.SelectedItem).Value;
    }

    private int GetSelectedTicketType()
    {
      if (cmbTicketType.SelectedItem == null) return -1;
      return (int)((NamObjectItem)cmbTicketType.SelectedItem).Value;
    }

    private void LoadCustomPropertyTypes()
    {
      cmbPropertyTypes.Items.Clear();

      cmbPropertyTypes.Items.Add(new NamObjectItem("Action Types", ReferenceType.ActionTypes));
      cmbPropertyTypes.Items.Add(new NamObjectItem("Phone Types", ReferenceType.PhoneTypes));
      cmbPropertyTypes.Items.Add(new NamObjectItem("Product Version Statuses", ReferenceType.ProductVersionStatuses));
      cmbPropertyTypes.Items.Add(new NamObjectItem("Ticket Severities", ReferenceType.TicketSeverities));
      cmbPropertyTypes.Items.Add(new NamObjectItem("Ticket Statuses", ReferenceType.TicketStatuses));
      cmbPropertyTypes.SelectedIndex = 0;
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

    protected override void LoadOrganization(TeamSupport.Data.Organization organization)
    {
      base.LoadOrganization(organization);
      LoadTicketTypes(organization);
      LoadCustomPropertyTypes();
      LoadCustomProperties();
    }

    private void LoadCustomProperties()
    {
      int oldID = GetSelectedPropertyID();
      
      ReferenceType refType = GetSelectedPropertyType();

      DataTable table = new DataTable();
      table.Columns.Add("ID", System.Type.GetType("System.Int32"));
      table.Columns.Add("Name");
      table.Columns.Add("Description");
      table.Columns.Add("IsClosed", System.Type.GetType("System.Boolean"));
      table.Columns.Add("IsShipping", System.Type.GetType("System.Boolean"));
      table.Columns.Add("IsDiscontinued", System.Type.GetType("System.Boolean"));

      gridProperties.Columns["IsShipping"].IsVisible = false;
      gridProperties.Columns["IsDiscontinued"].IsVisible = false;
      gridProperties.Columns["IsClosed"].IsVisible = false;

      switch (refType)
      {
        case ReferenceType.ActionTypes:
          ActionTypes actionTypes = new ActionTypes(LoginSession.LoginUser);
          actionTypes.LoadAllPositions(OrganizationID);
          foreach (ActionType actionType in actionTypes)
          {
            DataRow row = table.NewRow();
            row[0] = actionType.ActionTypeID;
            row[1] = actionType.Name;
            row[2] = actionType.Description;
            row[3] = false;
            row[4] = false;
            row[5] = false;
            table.Rows.Add(row);
          }
         
          break;
        case ReferenceType.PhoneTypes:
          PhoneTypes phoneTypes = new PhoneTypes(LoginSession.LoginUser);
          phoneTypes.LoadAllPositions(OrganizationID);
          foreach (PhoneType phoneType in phoneTypes)
          {
            DataRow row = table.NewRow();
            row[0] = phoneType.PhoneTypeID;
            row[1] = phoneType.Name;
            row[2] = phoneType.Description;
            row[3] = false;
            row[4] = false;
            row[5] = false;
            table.Rows.Add(row);
          }
          break;
        case ReferenceType.ProductVersionStatuses:
          ProductVersionStatuses productVersionStatuses = new ProductVersionStatuses(LoginSession.LoginUser);
          productVersionStatuses.LoadAllPositions(OrganizationID);
          foreach (ProductVersionStatus productVersionStatus in productVersionStatuses)
          {
            DataRow row = table.NewRow();
            row[0] = productVersionStatus.ProductVersionStatusID;
            row[1] = productVersionStatus.Name;
            row[2] = productVersionStatus.Description;
            row[3] = false;
            row[4] = productVersionStatus.IsShipping;
            row[5] = productVersionStatus.IsDiscontinued;
            table.Rows.Add(row);
          }
          gridProperties.Columns["IsShipping"].IsVisible = true;
          gridProperties.Columns["IsDiscontinued"].IsVisible = true;
          break;
        case ReferenceType.TicketSeverities:
          TicketSeverities ticketSeverities = new TicketSeverities(LoginSession.LoginUser);
          ticketSeverities.LoadAllPositions(OrganizationID);
          foreach (TicketSeverity ticketSeverity in ticketSeverities)
          {
            DataRow row = table.NewRow();
            row[0] = ticketSeverity.TicketSeverityID;
            row[1] = ticketSeverity.Name;
            row[2] = ticketSeverity.Description;
            row[3] = false;
            row[4] = false;
            row[5] = false;
            table.Rows.Add(row);
          }
          break;
        case ReferenceType.TicketStatuses:
          TicketStatuses ticketStatuses = new TicketStatuses(LoginSession.LoginUser);
          ticketStatuses.LoadAllPositions(GetSelectedTicketType());
          foreach (TicketStatus ticketStatus in ticketStatuses)
          {
            DataRow row = table.NewRow();
            row[0] = ticketStatus.TicketStatusID;
            row[1] = ticketStatus.Name;
            row[2] = ticketStatus.Description;
            row[3] = ticketStatus.IsClosed;
            row[4] = false;
            row[5] = false;
            table.Rows.Add(row);
          }
          gridProperties.Columns["IsClosed"].IsVisible = true;
          break;
        default:
          break;
      }
      
      

      gridProperties.DataSource = table;
      if (gridProperties.Rows.Count > 0) gridProperties.Rows[0].IsCurrent = true;
      SetSelectedPropertyID(oldID);
    }

    private int GetSelectedPropertyID()
    {
      try
      {
        if (gridProperties.CurrentRow == null || gridProperties.CurrentRow.Cells.Count < 1 || gridProperties.CurrentRow.Cells[0].Value == null) return -1;
        return (int)gridProperties.CurrentRow.Cells[0].Value;
      }
      catch (Exception)
      {

        return -1;
      }

    }

    private void SetSelectedPropertyID(int id)
    {
      foreach (GridViewRowInfo info in gridProperties.Rows)
      {
        if ((int)info.Cells[0].Value == id)
        {
          info.IsCurrent = true;
          break;
        }
      }
    }


    private void cmbPropertyTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
      LoadCustomProperties();
      lblTicketType.Visible = cmbTicketType.Visible = GetSelectedPropertyType() == ReferenceType.TicketStatuses;

    }

    private void btnUp_Click(object sender, EventArgs e)
    {
      switch (GetSelectedPropertyType())
      {
        case ReferenceType.ActionTypes:
          (new ActionTypes(LoginSession.LoginUser)).MovePositionUp(GetSelectedPropertyID());        
          break;
        case ReferenceType.PhoneTypes:
          (new PhoneTypes(LoginSession.LoginUser)).MovePositionUp(GetSelectedPropertyID());
          break;
        case ReferenceType.ProductVersionStatuses:
          (new ProductVersionStatuses(LoginSession.LoginUser)).MovePositionUp(GetSelectedPropertyID());
          break;
        case ReferenceType.TicketSeverities:
          (new TicketSeverities(LoginSession.LoginUser)).MovePositionUp(GetSelectedPropertyID());
          break;
        case ReferenceType.TicketStatuses:
          (new TicketStatuses(LoginSession.LoginUser)).MovePositionUp(GetSelectedPropertyID());
          break;
        default:
          break;
      }
      LoadCustomProperties();
      
    }

    private void btnDown_Click(object sender, EventArgs e)
    {
      switch (GetSelectedPropertyType())
      {
        case ReferenceType.ActionTypes:
          (new ActionTypes(LoginSession.LoginUser)).MovePositionDown(GetSelectedPropertyID());
          break;
        case ReferenceType.PhoneTypes:
          (new PhoneTypes(LoginSession.LoginUser)).MovePositionDown(GetSelectedPropertyID());
          break;
        case ReferenceType.ProductVersionStatuses:
          (new ProductVersionStatuses(LoginSession.LoginUser)).MovePositionDown(GetSelectedPropertyID());
          break;
        case ReferenceType.TicketSeverities:
          (new TicketSeverities(LoginSession.LoginUser)).MovePositionDown(GetSelectedPropertyID());
          break;
        case ReferenceType.TicketStatuses:
          (new TicketStatuses(LoginSession.LoginUser)).MovePositionDown(GetSelectedPropertyID());
          break;
        default:
          break;
      }
      LoadCustomProperties();

    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      switch (GetSelectedPropertyType())
      {
        case ReferenceType.ActionTypes:
          (new ActionTypes(LoginSession.LoginUser)).DeleteFromDB(GetSelectedPropertyID());
          break;
        case ReferenceType.PhoneTypes:
          (new PhoneTypes(LoginSession.LoginUser)).DeleteFromDB(GetSelectedPropertyID());
          break;
        case ReferenceType.ProductVersionStatuses:
          (new ProductVersionStatuses(LoginSession.LoginUser)).DeleteFromDB(GetSelectedPropertyID());
          break;
        case ReferenceType.TicketSeverities:
          (new TicketSeverities(LoginSession.LoginUser)).DeleteFromDB(GetSelectedPropertyID());
          break;
        case ReferenceType.TicketStatuses:
          (new TicketStatuses(LoginSession.LoginUser)).DeleteFromDB(GetSelectedPropertyID());
          break;
        default:
          break;
      }
      LoadCustomProperties();

    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      if (GetSelectedPropertyType() == ReferenceType.TicketStatuses)
        CustomPropertyForm.ShowCustomProperty(OrganizationID, GetSelectedTicketType());
      else
        CustomPropertyForm.ShowCustomProperty(OrganizationID, GetSelectedPropertyType());
      LoadCustomProperties();
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
      if (GetSelectedPropertyType() == ReferenceType.TicketStatuses)
        CustomPropertyForm.ShowCustomProperty(OrganizationID, GetSelectedTicketType(), GetSelectedPropertyID());
      else
        CustomPropertyForm.ShowCustomProperty(OrganizationID, GetSelectedPropertyType(), GetSelectedPropertyID());
      LoadCustomProperties();
    }

    private void gridProperties_DoubleClick(object sender, EventArgs e)
    {
      if (GetSelectedPropertyType() == ReferenceType.TicketStatuses)
        CustomPropertyForm.ShowCustomProperty(OrganizationID, GetSelectedTicketType(), GetSelectedPropertyID());
      else
        CustomPropertyForm.ShowCustomProperty(OrganizationID, GetSelectedPropertyType(), GetSelectedPropertyID());
      LoadCustomProperties();
    }



    
  }
}
