using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace TeamSupport.Data
{
  public partial class WaterCoolerItem
  {
  }
  
  public partial class WaterCooler
  {

    public void LoadLastMessageID(int organizationID, int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
SELECT TOP 1 *  
FROM WaterCooler 
WHERE OrganizationID = @OrganizationID  
AND UserID <> @UserID
AND (ISNULL(GroupFor,0) = 0 
  OR  (ISNULL(GroupFor,0) IN (SELECT GroupID FROM GroupUsers WHERE UserID = @UserID))) 
ORDER BY MessageID DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command);
      }
    }

    public void LoadUnreadMessages(User user, int lastMessageID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
SELECT * FROM WaterCooler 
WHERE OrganizationID = @OrganizationID  
AND UserID <> @UserID
AND MessageID > @MessageID
AND (ISNULL(GroupFor,0) = 0 
  OR  (ISNULL(GroupFor,0) IN (SELECT GroupID FROM GroupUsers WHERE UserID = @UserID))) 
ORDER BY MessageID ASC";
 
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", user.OrganizationID);
        command.Parameters.AddWithValue("@UserID", user.UserID);
        command.Parameters.AddWithValue("@MessageID", lastMessageID);
        Fill(command);
      }
    }

    public static int LoadUnreadMessageCount(LoginUser loginUser, User user, int lastMessageID)
    {
      WaterCooler waterCooler = new WaterCooler(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
SELECT COUNT(*) FROM WaterCooler 
WHERE OrganizationID = @OrganizationID  
AND UserID <> @UserID
AND MessageID > @MessageID
AND (ISNULL(GroupFor,0) = 0 
  OR  (ISNULL(GroupFor,0) IN (SELECT GroupID FROM GroupUsers WHERE UserID = @UserID)))";

        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", user.OrganizationID);
        command.Parameters.AddWithValue("@UserID", user.UserID);
        command.Parameters.AddWithValue("@MessageID", lastMessageID);
        object o = waterCooler.ExecuteScalar(command);
        if (o == null || o == DBNull.Value) return 0;
        return (int)o;
      }
    }

    public static int GetLastMessageID(LoginUser loginUser)
    {
      WaterCooler cooler = new WaterCooler(loginUser);
      cooler.LoadLastMessageID(loginUser.OrganizationID, loginUser.UserID);
      if (cooler.IsEmpty) return -1;
      return cooler[0].MessageID;
    }

  }
  
}
