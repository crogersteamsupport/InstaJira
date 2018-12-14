using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ReportTable 
  {
  }

  public partial class ReportTables 
  {
    public void LoadAll()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ReportTables ORDER BY Alias";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public void LoadCategories()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ReportTables WHERE IsCategory = 1 ORDER BY Alias";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }
  }
}
