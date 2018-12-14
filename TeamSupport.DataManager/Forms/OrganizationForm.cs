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
  public partial class OrganizationForm : Form
  {
    private int _organizationID;
  
    public OrganizationForm()
    {
      InitializeComponent();
    }

    public static int ShowOrganization(int organizationID)
    {
      OrganizationForm form = new OrganizationForm();
      form._organizationID = organizationID;
      
      if (form.ShowDialog() == DialogResult.OK)
      {
        return form._organizationID;
      }
      else
      {
        return -1;
      }

    }

    public static int ShowOrganization()
    {
      return ShowOrganization(-1);
    
    }

    private void OrganizationForm_Load(object sender, EventArgs e)
    {
      cmbProductTypes.SelectedIndex = 0;
      LoadOrganization();
    }
    
    private void LoadOrganization()
    {
      if (_organizationID > -1)
      {
        Organization organization = (Organization)Organizations.GetOrganization(LoginSession.LoginUser, _organizationID);
        if (organization != null)
        {
          textName.Text = organization.Name;
          textDescription.Text = organization.Description;
          textWebServiceID.Text = organization.WebServiceID.ToString();
          textEmailID.Text = organization.SystemEmailID.ToString();
          textInactiveReason.Text = organization.InActiveReason;
          textWebSite.Text = organization.Website;
          cbActive.Checked = organization.IsActive;
          cbFree.Checked = organization.IsCustomerFree;
          cbPortal.Checked = organization.HasPortalAccess;
          numPortalSeats.Value = organization.PortalSeats;
          numUserSeats.Value = organization.UserSeats;
          numStorageUnits.Value = organization.ExtraStorageUnits;
          cmbProductTypes.SelectedIndex = (int)organization.ProductType;
        }
      }
    }
    
    private void SaveOrganization()
    {
      Organization organization = null;
      if (_organizationID > -1)
      {
         organization = (Organization)Organizations.GetOrganization(LoginSession.LoginUser, _organizationID);
      }
      else
      {
        organization = (new Organizations(LoginSession.LoginUser)).AddNewOrganization();
      }
      
      if (organization == null)
      {
        MessageBox.Show("There was an error saving your organzation.");
        return;
      }

      organization.Name = textName.Text;
      organization.Description = textDescription.Text;
      //organization.WebServiceID = (Guid)textWebServiceID.Text;
      //organization.SystemEmailID = (Guid)textEmailID.Text;
      organization.InActiveReason = textInactiveReason.Text;
      organization.Website = textWebSite.Text;
      organization.IsActive = cbActive.Checked;
      organization.IsCustomerFree = cbFree.Checked;
      organization.HasPortalAccess = cbPortal.Checked;
      organization.PortalSeats = (int)numPortalSeats.Value;
      organization.UserSeats = (int)numUserSeats.Value;
      organization.ExtraStorageUnits = (int)numStorageUnits.Value;
      organization.ProductType = (ProductType)cmbProductTypes.SelectedIndex;
      organization.ParentID = LoginSession.LoginUser.OrganizationID;
      organization.Collection.Save();

      if (_organizationID < 0)
      {
        TicketTypes ticketTypes = new TicketTypes(LoginSession.LoginUser);
        TicketType ticketType;

        ticketType = ticketTypes.AddNewTicketType();
        ticketType.Name = "Issues";
        ticketType.Description = "Issues";
        ticketType.OrganizationID = organization.OrganizationID;
        ticketType.IconUrl = "Images/TicketTypes/Issues.png";
        ticketType.Position = 0;

        ticketType = ticketTypes.AddNewTicketType();
        ticketType.Name = "Features";
        ticketType.Description = "Features";
        ticketType.OrganizationID = organization.OrganizationID;
        ticketType.IconUrl = "Images/TicketTypes/Features.png";
        ticketType.Position = 1;


        ticketType = ticketTypes.AddNewTicketType();
        ticketType.Name = "Tasks";
        ticketType.Description = "Tasks";
        ticketType.OrganizationID = organization.OrganizationID;
        ticketType.IconUrl = "Images/TicketTypes/Tasks.png";
        ticketType.Position = 2;


        ticketType = ticketTypes.AddNewTicketType();
        ticketType.Name = "Bugs";
        ticketType.Description = "Bugs";
        ticketType.OrganizationID = organization.OrganizationID;
        ticketType.IconUrl = "Images/TicketTypes/Bugs.png";
        ticketType.Position = 3;
        ticketTypes.Save();

       
        Organizations.CreateStandardData(LoginSession.LoginUser, organization, false, false);
      }
      
      _organizationID = organization.OrganizationID;
    }

    private void OrganizationForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (DialogResult == DialogResult.OK)
      {
        SaveOrganization();
      }
    }
  }
}
