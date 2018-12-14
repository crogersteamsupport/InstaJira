using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class LoginHistoryItem 
  {
  }

  public partial class LoginHistory 
  {
    

    
    public void LoadForGrid()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT *, u.LastName + ', ' + u.FirstName AS UserName, o.Name AS Company
                                FROM LoginHistory lh 
                                LEFT JOIN Users u ON u.UserID = lh.UserID 
                                LEFT JOIN Organizations o ON o.OrganizationID = u.OrganizationID
                                ORDER BY lh.DateCreated DESC";
        command.CommandType = CommandType.Text;
        Fill(command, "LoginHistory, Users, Organizations");
      }
    }
  }
}
