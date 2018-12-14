using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class FullContactUpdatesItem
  {
  }

  public partial class FullContactUpdates
  {
    public void LoadByOrganizationId(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
        @"
          SELECT * 
          FROM 
            FullContactUpdates WITH(NOLOCK)
          WHERE
            organizationId = @organizationId
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@organizationId", id);

        Fill(command);
      }
    }

    public void LoadByContactId(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
        @"
          SELECT * 
          FROM 
            FullContactUpdates WITH(NOLOCK)
          WHERE
            userId = @userId
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@userId", id);

        Fill(command);
      }
    }
  }
}
