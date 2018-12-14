using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TeamSupport.Data;
using System.Data.OleDb;

namespace TeamSupport.DataManager.UserControls
{
  public partial class ImportControl : BaseOrganizationUserControl
  {
    private DataSet _dataSet;
    private Importer _importer;
    
    public ImportControl()
    {
      InitializeComponent();
    }
    
    protected override void LoadOrganization(Organization organization)
    {
    }

    private void btnOpen_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.FileName = Properties.Settings.Default.SpreadSheetSourceFileName;
      dialog.Filter = "Spreadsheet (*.xls, *.xlsx, *.csv)|*.xls;*.xlsx;*.csv|All Files (*.*)|*.*";
      if (dialog.ShowDialog() == DialogResult.OK)
      {
        lblFileName.Text = dialog.FileName;
        Properties.Settings.Default.SpreadSheetSourceFileName = dialog.FileName;
        Properties.Settings.Default.Save();
        btnImport.Enabled = true;

        try
        {
          Cursor.Current = Cursors.WaitCursor;
          _importer = new Importer(new LoginUser(LoginSession.LoginUser.ConnectionString, 34, 1078, null), dialog.FileName);
        }
        catch (Exception ex)
        {
          MessageBox.Show("There was an error opening the file.\n\n" + ex.Message);
          return;
        }
        finally
        {
          Cursor.Current = Cursors.Default;
        }

      }
    }   
    
    private void btnImport_Click(object sender, EventArgs e)
    {
      if (_importer == null) return;
      try
      {
        Cursor.Current = Cursors.WaitCursor;
        
        if (cbNewOrg.Checked)
          MessageBox.Show(_importer.Import(textOrganizationName.Text, cbCustomFieldsOnly.Checked));
        else
          MessageBox.Show(_importer.Import(OrganizationID, cbCustomFieldsOnly.Checked));

      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
      
    }

    private void cbNewOrg_CheckedChanged(object sender, EventArgs e)
    {
      textOrganizationName.Enabled = cbNewOrg.Checked;
    }

    private void btnClone_Click(object sender, EventArgs e)
    {
      AccountClone.Clone(new LoginUser(LoginSession.LoginUser.ConnectionString, -1, -1, null), OrganizationID, textOrganizationName.Text);
    }

    private void btnUnkownDups_Click(object sender, EventArgs e)
    {
      LoginUser loginUser = new LoginUser(LoginSession.LoginUser.ConnectionString, -1, OrganizationID, null);
      Organizations organizations = new Organizations(loginUser);
      organizations.LoadByUnknownCompany(OrganizationID);
      if (organizations.IsEmpty) {
        MessageBox.Show("_Unknown Company was not fount");
        return; 
      }
      int unkID = organizations[0].OrganizationID;

      Users unkUsers = new Users(loginUser);
      unkUsers.LoadByOrganizationID(unkID, false);

      Users users = new Users(loginUser);
      users.LoadContacts(OrganizationID, false);
      int cnt = 0;
      foreach (User unkUser in unkUsers)
      {
        foreach (User user in users)
        {
          if (user.OrganizationID != unkID && user.Email.Trim().ToLower() == unkUser.Email.Trim().ToLower())
          {
            cnt++;
            Tickets tickets = new Tickets(loginUser);
            tickets.LoadByContact(unkUser.UserID);

            foreach (Ticket ticket in tickets)
            {
              try {
                tickets.AddContact(user.UserID, ticket.TicketID);

              } catch (Exception) { }
              try
              {
                tickets.RemoveContact(unkUser.UserID, ticket.TicketID);
              }
              catch (Exception) { }
              try
              {
                tickets.RemoveOrganization(unkID, ticket.TicketID);
              }
              catch (Exception) { }
            }
            unkUser.MarkDeleted = true;
            
          }
        }
      }
      unkUsers.Save();

      MessageBox.Show(cnt.ToString());
    }


  }
}
