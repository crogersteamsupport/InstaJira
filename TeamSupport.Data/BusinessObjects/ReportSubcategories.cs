using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ReportSubcategory 
  {
  }

  public partial class ReportSubcategories 
  {
    public void LoadByReportTableID(int reportTableID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT rs.*, rt.Alias FROM ReportSubcategories rs LEFT JOIN ReportTables rt ON rs.ReportTableID = rt.ReportTableID WHERE ReportCategoryTableID = @ReportCategoryTableID ORDER BY rt.Alias";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ReportCategoryTableID", reportTableID);
        Fill(command);
      }
    }
  }
}
