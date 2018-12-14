using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BusinessObjectGenerator.Forms
{
  public partial class FormSettings : Form
  {
    public FormSettings()
    {
      InitializeComponent();
    }

    public static void Open()
    {
      AppSettings.Default.Save();
      FormSettings form = new FormSettings();
      form.ShowDialog();

    
    }

    private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (DialogResult == DialogResult.OK)
	    {
        AppSettings.Default.Save();
	    }
      else
      {
        AppSettings.Default.Reload();
      }
      
    }

    private void btnOutputFolder_Click(object sender, EventArgs e)
    {
      FolderBrowserDialog dialog = new FolderBrowserDialog();
      dialog.SelectedPath = textOutputFolder.Text;
      if (dialog.ShowDialog() == DialogResult.OK)
      {
        textOutputFolder.Text = dialog.SelectedPath;
      }

    }

    private void btnDevelopmentPath_Click(object sender, EventArgs e)
    {
      FolderBrowserDialog dialog = new FolderBrowserDialog();
      dialog.SelectedPath = textDevelopmentPath.Text;
      if (dialog.ShowDialog() == DialogResult.OK)
      {
        textDevelopmentPath.Text = dialog.SelectedPath;
      }

    }


  }
}
