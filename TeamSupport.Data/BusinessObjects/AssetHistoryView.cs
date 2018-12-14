using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class AssetHistoryViewItem
  {
  }
  
  public partial class AssetHistoryView
  {
    public void LoadByAssetIDLimit(int assetID, int start)
    {
      int end = start + 49;
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
          WITH OrderedHistory AS
          (
	          SELECT 
		          HistoryID, 
		          ROW_NUMBER() OVER (ORDER BY HistoryID DESC) AS rownum
	          FROM 
		          AssetHistory 
	          WHERE 
		          AssetID = @AssetID 
          ) 
          SELECT 
	          a.* 
          FROM
	          AssetHistoryView a
	          JOIN OrderedHistory o
		          ON a.HistoryID = o.HistoryID
          WHERE
	          o.rownum BETWEEN @start and @end
          ORDER BY
            a.HistoryID DESC
                                ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@AssetID", assetID);
        command.Parameters.AddWithValue("@start", start);
        command.Parameters.AddWithValue("@end", end);
        Fill(command);
      }
    }

    public void LoadByAssetID(int assetID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
          SELECT
	          *
          FROM
	          AssetHistoryView
          WHERE
	          AssetID = @AssetID
                                ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@AssetID", assetID);
        Fill(command);
      }
    }

    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM AssetHistoryView WHERE OrganizationID = @OrganizationID ORDER BY DateCreated DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }
  }
  
}
