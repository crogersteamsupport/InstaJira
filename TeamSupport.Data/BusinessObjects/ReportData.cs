using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ReportDataItem 
  {
  }

  public partial class ReportData 
  {

    public void LoadReportData(int reportID, int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ReportData WHERE (UserID = @UserID) AND (ReportID = @ReportID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@ReportID", reportID);
        Fill(command);
      }
    }

    public static void DeleteReportData(LoginUser loginUser, int reportID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM ReportData WHERE (ReportID = @ReportID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ReportID", reportID);

        ReportData reportData = new ReportData(loginUser);
        reportData.ExecuteScalar(command, "ReportData");
      }
    }

    public static void SaveReportData(LoginUser loginUser, int reportID, int userID, string data, string conditions)
    {
      ReportData reportData = new ReportData(loginUser);
      reportData.LoadReportData(reportID, userID);
      ReportDataItem item = null;
      if (!reportData.IsEmpty) 
        item = reportData[0]; 
      else 
        item = (new ReportData(loginUser)).AddNewReportDataItem();

      item.ReportID = reportID;
      item.UserID = userID;
      item.ReportData = data;
      item.QueryObject = conditions;
      item.Collection.Save();
    }
    /*
    public static string RetrieveReportData(LoginUser loginUser, int reportID, int userID)
    {
      ReportData reportData = new ReportData(loginUser);
      reportData.LoadReportData(reportID, userID);
      if (reportData.IsEmpty) return "";
      return reportData[0].ReportData;
    }
*/
  }
}
