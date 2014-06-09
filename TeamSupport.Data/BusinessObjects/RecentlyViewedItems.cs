using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class RecentlyViewedItem
  {
  }
  
  public partial class RecentlyViewedItems
  {
      public void LoadRecentForCustomerPage(int userID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText =
                @"SELECT TOP 5 * FROM RecentlyViewedItems
                WHERE (UserID = @UserID) 
                AND refType IN (0,1)
                ORDER BY DateViewed Desc";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@UserID", userID);
              Fill(command);
          }
      }

      public void LoadRecent(int userID, int refType)
      {
        using (SqlCommand command = new SqlCommand())
        {
          command.CommandText =
            @"SELECT TOP 5 * FROM RecentlyViewedItems
                WHERE (UserID = @UserID) 
                AND refType = @RefType
                ORDER BY DateViewed Desc";
          command.CommandType = CommandType.Text;
          command.Parameters.AddWithValue("@UserID", userID);
          command.Parameters.AddWithValue("@RefType", refType);
          Fill(command);
        }
      }

      public void DeleteRecentOrg(int organizationID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText =
                @"DELETE FROM RecentlyViewedItems WHERE (refID = @orgID) AND (refType = 1)";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@orgID", organizationID);
              command.ExecuteNonQuery();
          }
      }

  }
  
}
