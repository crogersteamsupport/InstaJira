using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ReportTableField 
  {
  }

  public partial class ReportTableFields 
  {

    public void LoadAll(bool onlyVisible = false)
    {
      using (SqlCommand command = new SqlCommand())
      {
        if (onlyVisible == true)
          command.CommandText = "SELECT rtf.*, rt.Alias AS TableAlias FROM ReportTableFields rtf LEFT JOIN ReportTables rt ON rt.ReportTableID = rtf.ReportTableID WHERE IsVisible = 1 ORDER BY rtf.Alias";
        else
          command.CommandText = "SELECT rtf.*, rt.Alias AS TableAlias FROM ReportTableFields rtf LEFT JOIN ReportTables rt ON rt.ReportTableID = rtf.ReportTableID ORDER BY rtf.Alias";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public void LoadByReportTableID(int reportTableID, bool isReadOnly)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT rtf.*, rt.Alias AS TableAlias FROM ReportTableFields rtf LEFT JOIN ReportTables rt ON rt.ReportTableID = rtf.ReportTableID WHERE rtf.ReportTableID = @ReportTableID AND IsVisible = 1 AND IsReadOnly = @IsReadOnly ORDER BY rtf.Alias";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ReportTableID", reportTableID);
        command.Parameters.AddWithValue("IsReadOnly", isReadOnly);
        Fill(command);
      }
    }

    public void LoadByReportTableID(int reportTableID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT rtf.*, rt.Alias AS TableAlias FROM ReportTableFields rtf LEFT JOIN ReportTables rt ON rt.ReportTableID = rtf.ReportTableID WHERE rtf.ReportTableID = @ReportTableID AND IsVisible = 1 ORDER BY rtf.Alias";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ReportTableID", reportTableID);
        Fill(command);
      }
    }

    public void LoadByReportSubcategoryID(int reportSubcategoryID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT rtf.*, rt.Alias AS TableAlias
FROM ReportSubcategories rs 
LEFT JOIN ReportTableFields rtf 
ON (rs.ReportCategoryTableID = rtf.ReportTableID OR rs.ReportTableID = rtf.ReportTableID)
LEFT JOIN ReportTables rt 
ON rt.ReportTableID = rtf.@ReportSubcategoryID
WHERE rs.ReportSubcategoryID = 6
AND IsVisible = 1 ORDER BY rt.Alias, Alias";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ReportSubcategoryID", reportSubcategoryID);
        Fill(command);
      }
    }
  }
}
