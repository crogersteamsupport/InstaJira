using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ReportField 
  {
  }

  public partial class ReportFields 
  {

    public void LoadByReportID(int reportID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ReportFields WHERE ReportID = @ReportID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ReportID", reportID);
        Fill(command);
      }
    }

    public void ClearReportFields(int reportID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM ReportFields WHERE (ReportID = @ReportID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ReportID", reportID);
        ExecuteNonQuery(command, "ReportFields");
      }    
    }
  }
}
