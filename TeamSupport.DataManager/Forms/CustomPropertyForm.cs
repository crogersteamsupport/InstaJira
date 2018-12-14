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
  public partial class CustomPropertyForm : Form
  {
    private int _id;
    private ReferenceType _refType;
    private int _ticketTypeID;
    private int _organizationID;
  
    public CustomPropertyForm()
    {
      InitializeComponent();
    }
    
    public static bool ShowCustomProperty(int organizationID, ReferenceType refType, int id)
    {
      return ShowForm(organizationID, refType, -1, id);
    }

    public static bool ShowCustomProperty(int organizationID, ReferenceType refType)
    {
      return ShowForm(organizationID, refType, -1, -1);
    }

    public static bool ShowCustomProperty(int organizationID, int ticketTypeID, int id)
    {
      return ShowForm(organizationID, ReferenceType.TicketStatuses, ticketTypeID, id);
    }

    public static bool ShowCustomProperty(int organizationID, int ticketTypeID)
    {
      return ShowForm(organizationID, ReferenceType.TicketStatuses, ticketTypeID, -1);
    }

    private static bool ShowForm(int organizationID, ReferenceType refType, int ticketTypeID, int id)
    {
      CustomPropertyForm form = new CustomPropertyForm();
      form._id = id;
      form._refType = refType;
      form._ticketTypeID = ticketTypeID;
      form._organizationID = organizationID;
      form.cbClosed.Visible = refType == ReferenceType.TicketStatuses;
      form.cbDiscontinued.Visible = form.cbShipping.Visible = refType == ReferenceType.ProductVersionStatuses;
      return form.ShowDialog() == DialogResult.OK;
    }

    private void CustomPropertyForm_Load(object sender, EventArgs e)
    {
      if (_id > -1) LoadCustomProperty();
    }

    private void CustomPropertyForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (DialogResult == DialogResult.OK) SaveCustomProperty();
    }
    
    private void SaveCustomProperty()
    {
      switch (_refType)
      {
        case ReferenceType.ActionTypes:
          ActionType actionType;
          ActionTypes actionTypes = new ActionTypes(LoginSession.LoginUser);
          
          if (_id < 0)
          {
            actionType = actionTypes.AddNewActionType();
            actionType.OrganizationID = _organizationID;
            actionType.Position = actionTypes.GetMaxPosition(_organizationID) + 1;
          }
          else
          {
            actionTypes.LoadByActionTypeID(_id);
            actionType = actionTypes[0];
          }
          actionType.Description = textDescription.Text;
          actionType.Name = textName.Text;
          actionTypes.Save();
          break;
        case ReferenceType.PhoneTypes:
          PhoneType phoneType;
          PhoneTypes phoneTypes = new PhoneTypes(LoginSession.LoginUser);

          if (_id < 0)
          {
            phoneType = phoneTypes.AddNewPhoneType();
            phoneType.OrganizationID = _organizationID;
            phoneType.Position = phoneTypes.GetMaxPosition(_organizationID) + 1;
          }
          else
          {
            phoneTypes.LoadByPhoneTypeID(_id);
            phoneType = phoneTypes[0];
          }
          phoneType.Description = textDescription.Text;
          phoneType.Name = textName.Text;
          phoneTypes.Save();
          break;
        case ReferenceType.ProductVersionStatuses:
          ProductVersionStatus productVersionStatus;
          ProductVersionStatuses productVersionStatuses = new ProductVersionStatuses(LoginSession.LoginUser);

          if (_id < 0)
          {
            productVersionStatus = productVersionStatuses.AddNewProductVersionStatus();
            productVersionStatus.OrganizationID = _organizationID;
            productVersionStatus.Position = productVersionStatuses.GetMaxPosition(_organizationID) + 1;
          }
          else
          {
            productVersionStatuses.LoadByProductVersionStatusID(_id);
            productVersionStatus = productVersionStatuses[0];
          }
          productVersionStatus.Description = textDescription.Text;
          productVersionStatus.Name = textName.Text;
          productVersionStatus.IsDiscontinued = cbDiscontinued.Checked;
          productVersionStatus.IsShipping = cbShipping.Checked;
          productVersionStatuses.Save();
          break;
        case ReferenceType.TicketSeverities:
          TicketSeverity ticketSeverity;
          TicketSeverities ticketSeverities = new TicketSeverities(LoginSession.LoginUser);

          if (_id < 0)
          {
            ticketSeverity = ticketSeverities.AddNewTicketSeverity();
            ticketSeverity.OrganizationID = _organizationID;
            ticketSeverity.Position = ticketSeverities.GetMaxPosition(_organizationID) + 1;
          }
          else
          {
            ticketSeverities.LoadByTicketSeverityID(_id);
            ticketSeverity = ticketSeverities[0];
          }
          ticketSeverity.Description = textDescription.Text;
          ticketSeverity.Name = textName.Text;
          ticketSeverities.Save();
        
          break;
        case ReferenceType.TicketStatuses:
          TicketStatus ticketStatus;
          TicketStatuses ticketStatuses = new TicketStatuses(LoginSession.LoginUser);

          if (_id < 0)
          {
            ticketStatus = ticketStatuses.AddNewTicketStatus();
            ticketStatus.OrganizationID = _organizationID;
            ticketStatus.TicketTypeID = _ticketTypeID;
            ticketStatus.Position = ticketStatuses.GetMaxPosition(_ticketTypeID) + 1;
          }
          else
          {
            ticketStatuses.LoadByTicketStatusID(_id);
            ticketStatus = ticketStatuses[0];
          }
          ticketStatus.Description = textDescription.Text;
          ticketStatus.Name = textName.Text;
          ticketStatus.IsClosed = cbClosed.Checked;
          ticketStatuses.Save();
          break;
        default:
          break;
      }
    }
    
    private void LoadCustomProperty()
    {
      switch (_refType)
      {
        case ReferenceType.ActionTypes:
          ActionType actionType = (ActionType)ActionTypes.GetActionType(LoginSession.LoginUser, _id);
          textDescription.Text = actionType.Description;
          textName.Text = actionType.Name;
          break;
        case ReferenceType.PhoneTypes:
          PhoneType phoneType = (PhoneType)PhoneTypes.GetPhoneType(LoginSession.LoginUser, _id);
          textDescription.Text = phoneType.Description;
          textName.Text = phoneType.Name;
          break;
        case ReferenceType.ProductVersionStatuses:
          ProductVersionStatus productVersionStatus = (ProductVersionStatus)ProductVersionStatuses.GetProductVersionStatus(LoginSession.LoginUser, _id);
          textDescription.Text = productVersionStatus.Description;
          textName.Text = productVersionStatus.Name;
          cbDiscontinued.Checked = productVersionStatus.IsDiscontinued;
          cbShipping.Checked = productVersionStatus.IsShipping;
          break;
        case ReferenceType.TicketSeverities:
          TicketSeverity ticketSeverity = (TicketSeverity)TicketSeverities.GetTicketSeverity(LoginSession.LoginUser, _id);
          textDescription.Text = ticketSeverity.Description;
          textName.Text = ticketSeverity.Name;
          break;
        case ReferenceType.TicketStatuses:
          TicketStatus ticketStatus = (TicketStatus)TicketStatuses.GetTicketStatus(LoginSession.LoginUser, _id);
          textDescription.Text = ticketStatus.Description;
          textName.Text = ticketStatus.Name;
          cbClosed.Checked = ticketStatus.IsClosed;
          break;
        default:
          break;
      }
    }
  }
}
