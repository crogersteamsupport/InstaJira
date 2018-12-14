using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class SearchStandardFilter
  {
  }
  
  public partial class SearchStandardFilters
  {
    public void LoadByUserID(int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM SearchStandardFilters WHERE UserID = @UserID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command, "SearchStandardFilters");
      }
    }

    public string ConvertToWhereClause()
    {
      string result = string.Empty;

      if (this.Count > 0)
      {
        if (this[0].Tickets)
        {
          if (!this[0].KnowledgeBase)
          {
           result = " AND tv.IsKnowledgeBase = 0";
          }
        }
        else
        {
          if (this[0].KnowledgeBase)
          {
           result = " AND tv.IsKnowledgeBase = 1";
          }
        }
      }

      return result;
    }
  }
  
}
