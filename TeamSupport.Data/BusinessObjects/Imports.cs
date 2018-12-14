using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class Import
  {
  }
  
  public partial class Imports
  {

    public void LoadWaiting()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP 1 * FROM Imports WHERE (IsDone = 0) AND (IsRunning = 0) ORDER BY DateCreated";
        //command.CommandText = "SELECT TOP 1 * FROM Imports WHERE (IsDone = 0) ORDER BY DateCreated";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public void LoadByLimit(int start)
    {
      int end = start + 15;
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT
          *
        FROM
        (
          SELECT
            *, 
            ROW_NUMBER() OVER (ORDER BY DateCreated Desc) AS rownum  
          FROM
          (
            SELECT
              *
            FROM
              Imports
            WHERE 
              OrganizationID = @OrganizationID
          ) as temp
        ) as results
        WHERE
          rownum between @start and @end
				ORDER BY
          rownum ASC
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        command.Parameters.AddWithValue("@start", start);
        command.Parameters.AddWithValue("@end", end);
        Fill(command);
      }
    }


  }
  
}
