using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class EMailAlternateInboundItem
  {
  }
  
  public partial class EMailAlternateInbound
  {

    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"SELECT eai.*, ISNULL(p.Name, 'Unassigned') AS ProductName, ISNULL(g.Name, 'Unassigned') AS GroupName, ISNULL(tt.Name, 'Unassigned') AS TicketTypeName
FROM EMailAlternateInbound eai
LEFT JOIN Products p ON p.ProductID = eai.ProductID
LEFT JOIN Groups g ON g.GroupID = eai.GroupToAssign
LEFT JOIN TicketTypes tt ON tt.TicketTypeID = eai.DefaultTicketType 
WHERE (eai.OrganizationID = @OrganizationID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public static EMailAlternateInboundItem GetItem(LoginUser loginUser, Guid systemID)
    {
      EMailAlternateInbound result = new EMailAlternateInbound(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"SELECT eai.*, ISNULL(p.Name, 'Unassigned') AS ProductName, ISNULL(g.Name, 'Unassigned') AS GroupName, ISNULL(tt.Name, 'Unassigned') AS TicketTypeName
FROM EMailAlternateInbound eai
LEFT JOIN Products p ON p.ProductID = eai.ProductID
LEFT JOIN Groups g ON g.GroupID = eai.GroupToAssign
LEFT JOIN TicketTypes tt ON tt.TicketTypeID = eai.DefaultTicketType 
WHERE (eai.SystemEmailID = @SystemEmailID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@SystemEmailID", systemID);
        result.Fill(command);
      }

      return result.IsEmpty ? null : result[0];
      
    }

    public static void DeleteFromDB(LoginUser loginUser, Guid systemEMailID)
    {
      EMailAlternateInbound result = new EMailAlternateInbound(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM EMailAlternateInbound  WHERE (SystemEmailID = @SystemEmailID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@SystemEmailID", systemEMailID);
        result.ExecuteNonQuery(command, "EMailAlternateInbound");
      }
    }

  }
  
}

