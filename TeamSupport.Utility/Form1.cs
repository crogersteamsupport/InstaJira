using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TeamSupport.Data;
using System.Configuration;

namespace TeamSupport.Utility
{
  public partial class MainForm : Form
  {

    List<string> _pathsToDelete= new List<string>();
    List<int> _orgsToDelete = new List<int>();
    LoginUser _loginUser;
    int _count;
    int _total;
    Logs _logs = new Logs();

    public MainForm()
    {
      InitializeComponent();
      tbAttachmentFolder.Text = "c:\\tsdata";
      _loginUser = GetLoginUser();
    }

    public static LoginUser GetLoginUser()
    {
      string constring = ConfigurationManager.AppSettings["ConnectionString"];
      return new LoginUser(constring, -1, -1, null);
    }

    private void btnAttachmentsClean_Click(object sender, EventArgs e)
    {
      Cursor.Current = Cursors.WaitCursor;
      try
      {
        _count = 0;
        _total = 0;
        _pathsToDelete.Clear();
        _orgsToDelete.Clear();
        tbAttachments.Clear();
        CleanAttachments(Path.Combine(tbAttachmentFolder.Text, "Organizations"));
        CleanAttachments(Path.Combine(tbAttachmentFolder.Text, "WikiDocs"));
        if (_pathsToDelete.Count < 1)
        {
          tbAttachments.AppendText("There is nothing to delete");
          return;
        }
        
        tbAttachments.AppendText(string.Format("{0} of {1} will be deleted. {2}", _count.ToString(), _total.ToString(), Environment.NewLine));
        StringBuilder builder = new StringBuilder();
        foreach (int id in _orgsToDelete)
        {
          if (builder.Length > 0) builder.Append(",");
          builder.Append(id.ToString());
        }

        tbAttachments.AppendText(string.Format("SELECT * FROM Organizations WHERE OrganizationID IN ({0})", builder.ToString()));
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
    }

    private void CleanAttachments(string path)
    {
      string[] dirs = Directory.GetDirectories(path);
      _total += dirs.Length;
      foreach (string dir in dirs)
      {
        CheckOrg(dir);
      }
    }

    private void CheckOrg(string path)
    {
      try
      {
        int orgID;

        if (!int.TryParse((new DirectoryInfo(path)).Name, out orgID)) return;

        Organization org = Organizations.GetOrganization(_loginUser, orgID);
        if (org == null)
        {
          _count++;
          tbAttachments.AppendText(string.Format("OrgID: {0}, Path: {1}{2}", orgID.ToString(), path, Environment.NewLine));
          _pathsToDelete.Add(path);
          _orgsToDelete.Add(orgID);
        }
      }
      catch (Exception ex)
      {
        _logs.WriteEvent("Path: " + path);
        _logs.WriteException(ex);
        MessageBox.Show(ex.Message);
        throw;
      }
    }
    
    private void btnSetAttachmentFolder_Click(object sender, EventArgs e)
    {
      folderBrowserDialog1.SelectedPath = tbAttachmentFolder.Text;
      if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        tbAttachmentFolder.Text = folderBrowserDialog1.SelectedPath;
      }
    }

    private void btnDeleteAttachments_Click(object sender, EventArgs e)
    {
      bool errors = false;
      if (MessageBox.Show("Are you sure you would like to delete all the folders listed?", "WARNING", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
      {
        try
        {
          Cursor.Current = Cursors.WaitCursor;
          foreach (string path in _pathsToDelete)
          {
            try
            {
              _logs.WriteEvent("Deleting Folder: " + path);
              Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
              _logs.WriteException(ex);
              errors = true;
            }
          }
        }
        finally
        {
          Cursor.Current = Cursors.Default;

        }
        if (errors) MessageBox.Show("Completed with ERRORS.  Review Log"); else MessageBox.Show("Deletion Complete");
      
      
      }
    }
  }
}
