using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class Group
  {
    public static int? GetIDByName(LoginUser loginUser, string name, int? parentID)
    {
      Groups groups = new Groups(loginUser);
      groups.LoadByGroupName(loginUser.OrganizationID, name, 1);
      if (groups.IsEmpty) return null;
      else return groups[0].GroupID;
    }

		public int TicketCount
		{
			get
			{
				if (Row.Table.Columns.Contains("TicketCount"))
				{
					return (int)Row["TicketCount"];
				}
				else return 0;
			}

		}
	}

  public partial class Groups
  {

 
    public void LoadByUserID(int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Groups g LEFT JOIN GroupUsers gu ON g.GroupID = gu.GroupID WHERE gu.UserID = @UserID Order By Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command, "Groups,GroupUsers");
      }
    }

    public Group FindByImportID(string importID)
    {
      importID = importID.Trim().ToLower();
      foreach (Group group in this)
      {
        if ((group.ImportID != null && group.ImportID.Trim().ToLower() == importID) || group.Name.Trim().ToLower() == importID)
        {
          return group;
        }
      }
      return null;
    }

    public Group FindByName(string name)
    {
      foreach (Group group in this)
      {
        if (group.Name.Trim().ToLower() == name.Trim().ToLower())
        {
          return group;
        }
      }
      return null;
    }
    
    public void LoadByNotUserID(int userID, int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Groups g WHERE (g.OrganizationID = @OrganizationID) AND (1 not in (SELECT 1 FROM GroupUsers gu WHERE gu.UserID = @UserID AND gu.GroupID = g.GroupID)) Order by Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command, "Groups,GroupUsers");
      }
    }

    public void LoadByUserIDGrouped(int userID, int organizationID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"SELECT G.*, 1 SORTBY FROM Groups g LEFT JOIN GroupUsers gu ON g.GroupID = gu.GroupID WHERE gu.UserID = 9761 
                                    UNION ALL
                                    SELECT 0, 0, '?????????????', '', NULL, NULL, NULL, 0, 0, 2 SORTBY
                                    UNION ALL
                                    SELECT G.*, 3 SORTBY
                                    FROM Groups g 
                                    WHERE (g.OrganizationID = 1078) 
                                    AND (1 not in (SELECT 1 FROM GroupUsers gu WHERE gu.UserID = 9761 AND gu.GroupID = g.GroupID)) 
                                    ORDER BY SORTBY, NAME";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@UserID", userID);
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            Fill(command, "Groups,GroupUsers");
        }
    }



    public void LoadByOrganizationID(int organizationID, string orderBy = "Name")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Groups g WHERE (g.OrganizationID = @OrganizationID) ORDER BY g." + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public int GetGroupCount(int groupID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            //command.CommandText = @"SELECT COUNT(*) FROM TicketGridView tgv WHERE (tgv.GroupID = @groupID) AND (tgv.IsClosed = 0)";
            //command.CommandType = CommandType.Text;
            command.CommandText = @"GetGroupOpenTicketCount";
            command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@groupID", groupID);
            return (int)ExecuteScalar(command);
        }

    }

		public void LoadByOrganizationIDForGrid(int organizationID, int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
            SELECT
                g.*, 
                (SELECT COUNT(*) FROM TicketGridView tgv WHERE (tgv.GroupID = g.GroupID) AND (tgv.IsClosed = 0)) AS TicketCount
            FROM
                Groups g
                JOIN Organizations o
                    ON g.OrganizationID = o.OrganizationID
            WHERE
                g.OrganizationID = @OrganizationID
                AND (
                    g.GroupID IN (SELECT GroupID FROM GroupUsers gu WHERE gu.UserID=@UserID)
                    OR (EXISTS (SELECT * FROM Users WHERE UserID=@UserID AND (IsSystemAdmin = 1 OR TicketRights < 1)))
                    OR (o.UseProductFamilies = 1 AND EXISTS (SELECT * FROM Users WHERE UserID=@UserID AND ProductFamiliesRights < 1))
                )
            ORDER BY
                Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command);
      }
    }

    partial void BeforeDBDelete(int groupID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE Tickets SET GroupID = NULL WHERE (GroupID = @GroupID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@GroupID", groupID);
        ExecuteNonQuery(command, "Tickets");
      }

      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "UPDATE Organizations SET DefaultPortalGroupID = NULL WHERE (DefaultPortalGroupID = @GroupID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@GroupID", groupID);
        ExecuteNonQuery(command, "Organizations");
      } 
      
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM GroupUsers WHERE (GroupID = @GroupID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@GroupID", groupID);
        ExecuteNonQuery(command, "GroupUsers");
      }

    }

    public void AddGroupUser(int userID, int groupID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "INSERT INTO GroupUsers (GroupID, UserID, DateCreated, DateModified, CreatorID, ModifierID) VALUES (@GroupID, @UserID, @DateCreated, @DateModified, @CreatorID, @ModifierID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@GroupID", groupID);
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
        command.Parameters.AddWithValue("@DateModified", DateTime.UtcNow);
        command.Parameters.AddWithValue("@CreatorID", LoginUser.UserID);
        command.Parameters.AddWithValue("@ModifierID", LoginUser.UserID);
        ExecuteNonQuery(command, "GroupUsers");
      }

      User user = (User)Users.GetUser(LoginUser, userID);
      Group group = (Group)Groups.GetGroup(LoginUser, groupID);
      string description = "Added '" + user.FirstName + " " + user.LastName + "' to group '" + group.Name + "'";
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Groups, groupID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Users, userID, description);


    }

    public void DeleteGroupUser(int groupID, int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM GroupUsers WHERE (UserID = @UserID) AND (GroupID = @GroupID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@GroupID", groupID);
        ExecuteNonQuery(command, "GroupUsers");
      }
      User user = (User)Users.GetUser(LoginUser, userID);
      Group group = (Group)Groups.GetGroup(LoginUser, groupID);
      string description = "Removed '" + user.FirstName + " " + user.LastName + "' from group '" + group.Name + "'";
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Groups, groupID, description);
      ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Users, userID, description);
    }

    public void LoadByGroupName(int parentID, string name, int maxRows)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SELECT TOP (@MaxRows) * FROM Groups WHERE ((Name LIKE '%'+@Name+'%') OR (Description LIKE '%'+@Name+'%')) AND (OrganizationID = @ParentID) ORDER BY Name";
            command.CommandType = CommandType.Text;

            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@ParentID", parentID);
            command.Parameters.AddWithValue("@MaxRows", maxRows);
            Fill(command);
        }
    }

    public void LoadByGroupName(int parentID, string name)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Groups WHERE Name = @Name AND OrganizationID = @ParentID";
        command.CommandType = CommandType.Text;

        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@ParentID", parentID);
        Fill(command);
      }
    }

        public void LoadByCustomerHubID(int customerHubID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                    SELECT
	                    g.*
                    FROM
	                    CustomerHubs h
	                    JOIN Groups g
		                    ON h.OrganizationID = g.OrganizationID
                    WHERE
	                    h.CustomerHubID = @CustomerHubID
	                    AND 
	                    (
		                    h.ProductFamilyID IS NULL
		                    OR g.ProductFamilyID IS NULL
		                    OR h.ProductFamilyID = g.ProductFamilyID
	                    )
                    ORDER BY
	                    g.Name";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@CustomerHubID", customerHubID);
                Fill(command);
            }
        }

    }
}
