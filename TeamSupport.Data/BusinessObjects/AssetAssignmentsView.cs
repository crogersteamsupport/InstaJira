using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class AssetAssignmentsViewItem
  {
  }
  
  public partial class AssetAssignmentsView
  {
    public void LoadByAssetID(int assetID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT * FROM AssetAssignmentsView WHERE AssetID = @assetID ORDER BY AssetAssignmentsID DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@assetID", assetID);
        Fill(command);
      }
    }
  }
  
}
