using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class CompanyParentsViewItem
  {
  }
  
  public partial class CompanyParentsView
  {
    public virtual void LoadByChildAndParentIDs(int childID, int parentID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SET NOCOUNT OFF; 
        SELECT 
            [ChildID]
            , [ParentID]
            , [ParentName] 
        FROM 
            [dbo].[CompanyParentsView] 
        WHERE 
            [ChildID] = @ChildID
            AND ParentID = @ParentID;";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ChildID", childID);
        command.Parameters.AddWithValue("ParentID", parentID);
        Fill(command);
      }
    }
  }
  
}
