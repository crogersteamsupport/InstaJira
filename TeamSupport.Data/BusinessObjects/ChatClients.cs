using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ChatClient
  {
  }
  
  public partial class ChatClients
  {
    
    public void LoadByEmail(int organizationID, string email)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ChatClients WHERE (Email = @Email) AND (OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@Email", email);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public static void UpdateClientPing(LoginUser loginUser, int chatClientID)
    {
      ChatClients clients = new ChatClients(loginUser);

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"UPDATE ChatClients SET LastPing = GETUTCDATE() WHERE ChatClientID = @ChatClientID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ChatClientID", chatClientID);
        clients.ExecuteNonQuery(command, "ChatClients");
      }
    }

  }
  
}
