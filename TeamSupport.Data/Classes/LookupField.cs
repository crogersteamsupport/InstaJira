using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Security;


namespace TeamSupport.Data
{
  using LookupFieldValues = Dictionary<int, string>;

  public class LookupField
  {
    public static LookupFieldValues GetValues(LoginUser loginUser, int reportTableFieldID, string term, int maxRows)
    {
      LookupFieldValues result = new LookupFieldValues();
      ReportTableField field = ReportTableFields.GetReportTableField(loginUser, reportTableFieldID);
      if (field == null || field.LookupTableID == null) return null;
      ReportTable table = ReportTables.GetReportTable(loginUser, (int)field.LookupTableID);
      SqlCommand command = new SqlCommand();
      string[] orgs = table.OrganizationIDFieldName.Split(',');
      StringBuilder orgFields = new StringBuilder("(");
      foreach (String s in orgs)
      {
        if (orgFields.Length > 1)
        {
          orgFields.Append(" OR " + s + " = @OrganizationID");
        }
        else
        {
          orgFields.Append(s + " = @OrganizationID");
        }
        
      }
      orgFields.Append(")");

      string text = "SELECT TOP {0} {1} AS Label, {2} AS ID FROM {3} WHERE {4} AND {1} LIKE '%' + @Term + '%' ORDER BY {5}";
      command.CommandText = string.Format(text, 
        maxRows.ToString(),
        table.LookupDisplayClause, 
        table.LookupKeyFieldName, 
        table.TableName, 
        orgFields.ToString(), 
        table.LookupOrderBy);

      command.CommandType = CommandType.Text;
      command.Parameters.AddWithValue("@Term", term);
      command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
      DataTable dataTable = SqlExecutor.ExecuteQuery(loginUser, command);

      foreach (DataRow row in dataTable.Rows)
      {
        result.Add((int)row[1], row[0].ToString()); 
      }

      return result;
    }

  }
}
