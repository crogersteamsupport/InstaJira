using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class AssetAssignment
  {
  }
  
  public partial class AssetAssignments
  {
    public void LoadByAssetID(int assetID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
          SELECT
            a.*
          FROM
            AssetAssignments a
            JOIN AssetHistory h
              ON a.HistoryID = h.HistoryID
          WHERE 
            h.AssetID = @assetID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@assetID", assetID);
        Fill(command);
      }
    }
  }
  
}
