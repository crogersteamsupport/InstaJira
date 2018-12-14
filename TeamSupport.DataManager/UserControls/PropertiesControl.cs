using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TeamSupport.Data;

namespace TeamSupport.DataManager.UserControls
{
  public partial class PropertiesControl : BaseOrganizationUserControl
  {
    public PropertiesControl()
    {
      InitializeComponent();
      gridProperties.MasterGridViewTemplate.AutoGenerateColumns = false;
    }


    protected override void LoadOrganization(Organization organization)
    {
      base.LoadOrganization(organization);
      if (organization == null) return;
      
      DataTable table = new DataTable();
      table.Columns.Add("Property");
      table.Columns.Add("Value");

      foreach (DataColumn column in organization.Collection.Table.Columns)
      {
        table.Rows.Add(new string[] { column.ColumnName, organization.Row[column].ToString() });
      }
      
      gridProperties.DataSource = table;
      
      
    }
  }
}
