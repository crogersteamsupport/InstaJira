using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ActionLog
  {
    public string CreatorName
    {
      get
      {
        if (Row.Table.Columns.Contains("CreatorName") && Row["CreatorName"] != DBNull.Value)
        {
          return (string)Row["CreatorName"];
        }
        else return "";
      }
    }
  }

  public partial class ActionLogs : BaseCollection, IEnumerable<ActionLog>
  {

    //public struct UserID
    //{
    //    public int _value;
    //    static public explicit operator UserID(int refID) { return new UserID() { _value = refID }; }
    //}
    //public static void AddActionLog(LoginUser loginUser, ActionLogType type, UserID refID, string description)
    //{
    //    AddActionLog(loginUser, type, ReferenceType.Users, refID._value, description);
    //}

    //public struct TicketID
    //{
    //    public int _value;
    //    static public explicit operator TicketID(int refID) { return new TicketID() { _value = refID }; }
    //}
    //public static void AddActionLog(LoginUser loginUser, ActionLogType type, TicketID refID, string description)
    //{
    //    TicketLog.AddLog(loginUser, type, refID._value, description);
    //}

    public static void AddActionLog(LoginUser loginUser, ActionLogType type, ReferenceType refType, int refID, string description)
    {
      ActionLogs actionLogs = new ActionLogs(loginUser);
      ActionLog actionLog = actionLogs.AddNewActionLog();
      actionLog.Description = description;
      actionLog.OrganizationID = loginUser.OrganizationID < 0 ? null : (int?)loginUser.OrganizationID;
      actionLog.ActionLogType = type;
      actionLog.RefID = refID;
      actionLog.RefType = refType;
      actionLogs.Save();
    }

    public static void AddInternalActionLog(LoginUser loginUser, string description)
    {
      ActionLogs actionLogs = new ActionLogs(loginUser);
      ActionLog actionLog = actionLogs.AddNewActionLog();
      actionLog.Description = description;
      actionLog.OrganizationID = null;
      actionLog.ActionLogType = ActionLogType.Insert;
      actionLog.RefID = -1;
      actionLog.RefType = ReferenceType.SystemSettings;
      actionLogs.Save();
    }

    public void LoadLastByTypeAndID(ReferenceType refType, int refID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
          SELECT
            TOP 1
            *
          FROM
            ActionLogs
          WHERE
            RefType = @RefType
            AND RefID = @RefID
          ORDER BY
            DateCreated DESC
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefType", refType);
        command.Parameters.AddWithValue("@RefID", refID);
        Fill(command);
      }
    }

    public void LoadAll()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                ORDER BY DateCreated DESC
                                ";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public void LoadByGroupID(int groupID)
    {
      using (SqlCommand command = new SqlCommand())
      {
		command.CommandText = "ActionLogDetailsByGroupID";
		command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@GroupID", groupID);
        Fill(command, "", false);
      }
    }

    public void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.RefType = 17) AND (al.RefID = @TicketID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Actions a ON a.ActionID = al.RefID
                                WHERE (al.RefType = 0) AND (a.TicketID = @TicketID)

                                ORDER BY DateCreated DESC
                                ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadByTicketID(int ticketID, DateTime minUtcDate)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.RefType = 17) AND (al.RefID = @TicketID) AND (al.DateCreated >= @MinDate)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Actions a ON a.ActionID = al.RefID
                                WHERE (al.RefType = 0) AND (a.TicketID = @TicketID) AND (al.DateCreated >= @MinDate)

                                ORDER BY DateCreated DESC
                                ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        command.Parameters.AddWithValue("@MinDate", minUtcDate);
        Fill(command);
      }
    }

    public void LoadByProductID(int productID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.RefType = 13) AND (al.RefID = @ProductID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Tickets t ON t.TicketID = al.RefID
                                WHERE (al.RefType = 17) AND (t.ProductID = @ProductID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Actions a ON a.ActionID = al.RefID
                                LEFT JOIN Tickets t ON t.TicketID = a.TicketID
                                WHERE (al.RefType = 0) AND (t.ProductID = @ProductID)

                                ORDER BY DateCreated DESC
                                ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        Fill(command);
      }
    }

    public void LoadByProductVersionID(int productVersionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.RefType = 14) AND (al.RefID = @ProductVersionID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Tickets t ON t.TicketID = al.RefID
                                WHERE (al.RefType = 17) AND ((t.ReportedVersionID = @ProductVersionID) OR (t.SolvedVersionID = @ProductVersionID))
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Actions a ON a.ActionID = al.RefID
                                LEFT JOIN Tickets t ON t.TicketID = a.TicketID
                                WHERE (al.RefType = 0) AND ((t.ReportedVersionID = @ProductVersionID) OR (t.SolvedVersionID = @ProductVersionID))

                                ORDER BY DateCreated DESC
                                ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
        Fill(command);
      }
    }

    public void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
		command.CommandText = "ActionLogDetailsByOrganizationId";
		command.CommandType = CommandType.StoredProcedure;
		command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command, "", false);
      }
    }


    public void LoadByOrganizationIDLimit(int organizationID, int start)
    {
        int end = start + 49;
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"select * from (
                                select *, ROW_NUMBER() OVER (ORDER BY DateModified Desc) AS rownum  from (
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.RefType = 9) AND (al.RefID = @OrganizationID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Tickets t ON t.TicketID = al.RefID
                                LEFT JOIN OrganizationTickets ot ON ot.TicketID = t.TicketID
                                WHERE (al.RefType = 17) AND (ot.OrganizationID = @OrganizationID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Actions a ON a.ActionID = al.RefID
                                LEFT JOIN Tickets t ON t.TicketID = a.TicketID
                                LEFT JOIN OrganizationTickets ot ON ot.TicketID = t.TicketID
                                WHERE (al.RefType = 0) AND (ot.OrganizationID = @OrganizationID)
                                
                                UNION 

                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.OrganizationID = @OrganizationID)) as temp) as results
                                where rownum between @start and @end
								order by rownum asc

                                ";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@start", start);
            command.Parameters.AddWithValue("@end", end);
            Fill(command);
        }
    }

    public void LoadByProductIDLimit(int productID, int start)
    {
      int end = start + 49;
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT
          *
        FROM
        (
          SELECT
            *, 
            ROW_NUMBER() OVER (ORDER BY DateModified Desc) AS rownum  
          FROM
          (
            SELECT
              al.*, 
              u.FirstName + ' ' + u.LastName AS CreatorName
            FROM
              ActionLogs al
              LEFT JOIN Users u 
                ON u.UserID = al.CreatorID
            WHERE 
              al.RefType = 13
              AND al.RefID = @ProductID
                                
            UNION 
                                
            SELECT 
              al.*, 
              u.FirstName + ' ' + u.LastName AS CreatorName
            FROM 
              ActionLogs al
              LEFT JOIN Users u ON u.UserID = al.CreatorID
              LEFT JOIN Tickets t ON t.TicketID = al.RefID
            WHERE
              al.RefType = 17
              AND t.ProductID = @ProductID
                                
            UNION 
                                
            SELECT 
              al.*, 
              u.FirstName + ' ' + u.LastName AS CreatorName
            FROM
              ActionLogs al
              LEFT JOIN Users u ON u.UserID = al.CreatorID
              LEFT JOIN Actions a ON a.ActionID = al.RefID
              LEFT JOIN Tickets t ON t.TicketID = a.TicketID
            WHERE
              al.RefType = 0
              AND t.ProductID = @ProductID
                                
            UNION 

            SELECT 
              al.*, 
              u.FirstName + ' ' + u.LastName AS CreatorName
            FROM 
              ActionLogs al
              LEFT JOIN Users u ON u.UserID = al.CreatorID
              LEFT JOIN ProductVersions pv ON pv.ProductVersionID = al.RefID
            WHERE
              al.RefType = 14
              AND pv.ProductID = @ProductID
          ) as temp
        ) as results
        WHERE
          rownum between @start and @end
				ORDER BY
          rownum ASC
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductID", productID);
        command.Parameters.AddWithValue("@start", start);
        command.Parameters.AddWithValue("@end", end);
        Fill(command);
      }
    }

    public void LoadByProductVersionIDLimit(int productVersionID, int start)
    {
      int end = start + 20;
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT
          *
        FROM
        (
          SELECT
            TOP " + end.ToString() + @"
            *, 
            ROW_NUMBER() OVER (ORDER BY DateModified Desc) AS rownum  
          FROM
          (
            SELECT
              TOP " + end.ToString() + @"
              al.*, 
              u.FirstName + ' ' + u.LastName AS CreatorName
            FROM
              ActionLogs al
              LEFT JOIN Users u 
                ON u.UserID = al.CreatorID
            WHERE 
              al.RefType = 14
              AND al.RefID = @ProductVersionID
                                
            /* Query timing out. 
            UNION 
                                
            SELECT 
              TOP " + end.ToString() + @"
              al.*, 
              u.FirstName + ' ' + u.LastName AS CreatorName
            FROM 
              ActionLogs al
              LEFT JOIN Users u ON u.UserID = al.CreatorID
              LEFT JOIN Tickets t ON t.TicketID = al.RefID
            WHERE
              al.RefType = 17
              AND (t.ReportedVersionID = @ProductVersionID OR t.SolvedVersionID = @ProductVersionID)
                                
            UNION 
                                
            SELECT 
              TOP " + end.ToString() + @"
              al.*, 
              u.FirstName + ' ' + u.LastName AS CreatorName
            FROM
              ActionLogs al
              LEFT JOIN Users u ON u.UserID = al.CreatorID
              LEFT JOIN Actions a ON a.ActionID = al.RefID
              LEFT JOIN Tickets t ON t.TicketID = a.TicketID
            WHERE
              al.RefType = 0
              AND (t.ReportedVersionID = @ProductVersionID OR t.SolvedVersionID = @ProductVersionID)                               
            */                               
          ) as temp
        ) as results
        WHERE
          rownum between @start and @end
				ORDER BY
          rownum ASC
        ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@ProductVersionID", productVersionID);
        command.Parameters.AddWithValue("@start", start);
        command.Parameters.AddWithValue("@end", end);
        Fill(command);
      }
    }

    public void LoadByProductFamilyIDLimit(int productFamilyID, int start)
    {
        int end = start + 49;
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"
        SELECT
          *
        FROM
        (
          SELECT
            *, 
            ROW_NUMBER() OVER (ORDER BY DateModified Desc) AS rownum  
          FROM
          (
            SELECT
              al.*, 
              u.FirstName + ' ' + u.LastName AS CreatorName
            FROM
              ActionLogs al
              JOIN ProductFamilies pf
                ON al.RefType = 44
                AND al.RefID = pf.ProductFamilyID
              LEFT JOIN Users u 
                ON u.UserID = al.CreatorID
            WHERE 
              pf.ProductFamilyID = @ProductFamilyID
            /*                    
            UNION 
                                
            SELECT
              al.*, 
              u.FirstName + ' ' + u.LastName AS CreatorName
            FROM
              ActionLogs al
              JOIN Products p
                ON al.RefType = 13
                AND al.RefID = p.ProductID
              LEFT JOIN Users u 
                ON u.UserID = al.CreatorID
            WHERE 
              p.ProductFamilyID = @ProductFamilyID
                                
            UNION 
                                
            SELECT 
              al.*, 
              u.FirstName + ' ' + u.LastName AS CreatorName
            FROM 
              ActionLogs al
              LEFT JOIN Users u ON u.UserID = al.CreatorID
              JOIN Tickets t 
                ON t.TicketID = al.RefID
                AND al.RefType = 17
              JOIN Products p
                ON t.ProductID = p.ProductID
            WHERE
              p.ProductFamilyID = @ProductFamilyID
                                
            UNION 
                                
            SELECT 
              al.*, 
              u.FirstName + ' ' + u.LastName AS CreatorName
            FROM
              ActionLogs al
              LEFT JOIN Users u ON u.UserID = al.CreatorID
              JOIN Actions a 
                ON a.ActionID = al.RefID
                AND al.RefType = 0
              JOIN Tickets t
                ON t.TicketID = a.TicketID
              JOIN Products p
                ON t.ProductID = p.ProductID
            WHERE
              p.ProductFamilyID = @ProductFamilyID
                                
            UNION 

            SELECT 
              al.*, 
              u.FirstName + ' ' + u.LastName AS CreatorName
            FROM 
              ActionLogs al
              LEFT JOIN Users u ON u.UserID = al.CreatorID
              JOIN ProductVersions pv
                ON pv.ProductVersionID = al.RefID
                AND al.RefType = 14
              JOIN Products p
                ON pv.ProductID = p.ProductID
            WHERE
              p.ProductFamilyID = @ProductFamilyID
            */
          ) as temp
        ) as results
        WHERE
          rownum between @start and @end
				ORDER BY
          rownum ASC
        ";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@ProductFamilyID", productFamilyID);
            command.Parameters.AddWithValue("@start", start);
            command.Parameters.AddWithValue("@end", end);
            Fill(command);
        }
    }

    public void LoadByUserID(int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
		command.CommandText = "ActionLogDetailsByUserId";
		command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command, "", false);
      }
    }


    public void LoadByUserIDLimit(int userID, int start)
    {
        int end = start + 49;
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = @"select *  from  
                                (select *, ROW_NUMBER() OVER (ORDER BY DateModified Desc) AS rownum  from (                                
                                (SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.RefType = 22) AND (al.RefID = @UserID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                WHERE (al.CreatorID = @UserID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Tickets t ON t.TicketID = al.RefID
                                WHERE (al.RefType = 17) AND (t.UserID = @UserID)
                                
                                UNION 
                                
                                SELECT al.*, u.FirstName + ' ' + u.LastName AS CreatorName
                                FROM ActionLogs al
                                LEFT JOIN Users u ON u.UserID = al.CreatorID
                                LEFT JOIN Actions a ON a.ActionID = al.RefID
                                LEFT JOIN Tickets t ON t.TicketID = a.TicketID
                                WHERE (al.RefType = 0) AND (t.UserID = @UserID)
                                )) as temp) as results
                                where rownum between @start and @end
								order by rownum asc
                                ";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@UserID", userID);
            command.Parameters.AddWithValue("@start", start);
            command.Parameters.AddWithValue("@end", end);
            Fill(command);
        }
    }

    public void LoadByAssetIDLimit(int assetID, int start)
    {
      int end = start + 49;
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT
          *
        FROM
          (
            SELECT
              *, 
              ROW_NUMBER() OVER (ORDER BY DateModified Desc) AS rownum  
            FROM
              (
                SELECT 
                  al.*, 
                  u.FirstName + ' ' + u.LastName AS CreatorName
                FROM
                  ActionLogs al
                  LEFT JOIN Users u ON u.UserID = al.CreatorID
                WHERE 
                  al.RefType = 34
                  AND al.RefID = @AssetID
              ) as temp
          ) as results
        WHERE
          rownum BETWEEN @start AND @end
        ORDER BY
          rownum ASC
                                ";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@AssetID", assetID);
        command.Parameters.AddWithValue("@start", start);
        command.Parameters.AddWithValue("@end", end);
        Fill(command);
      }
    }

		public void UpdateCreatorID(int actionLogID, int creatorID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "UPDATE ActionLogs SET CreatorID = @CreatorID WHERE ActionLogID = @ActionLogID";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@ActionLogID", actionLogID);
				command.Parameters.AddWithValue("@CreatorID", creatorID);
				Fill(command);
			}
		}

	}
}
