using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class UsersViewItem
  {
  }
  
  public partial class UsersView
  {

    public void LoadByOrganizationID(int organizationID, bool loadOnlyActive, string orderBy = "LastName, FirstName")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM UsersView WHERE OrganizationID = @OrganizationID AND (@ActiveOnly = 0 OR IsActive = 1) AND (MarkDeleted = 0) ORDER BY " + orderBy;
        command.CommandText = InjectCustomFields(command.CommandText, "UserID", ReferenceType.Users);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@ActiveOnly", loadOnlyActive);
        Fill(command);
      }
    }

    public void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT u.* FROM UsersView u LEFT JOIN UserTickets ut ON ut.UserID = u.UserID WHERE ut.TicketID = @TicketID ORDER BY u.LastName, u.FirstName";
        command.CommandText = InjectCustomFields(command.CommandText, "u.UserID", ReferenceType.Users);
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command, "Organizations,OrganizationTickets");
      }
    }

    public void LoadLastSenderByTicketNumber(int organizationID, int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
          SELECT
	          *
          FROM
	          UsersView
          WHERE
	          OrganizationID = @OrganizationID
	          AND MarkDeleted = 0
	          AND UserID =
	          (
		          SELECT 
			          TOP 1
			          ModifierID
		          FROM 
			          ActionLogs 
		          WHERE 
			          OrganizationID = @OrganizationID
			          AND RefType = 17
			          AND RefID = @TicketID
			          AND Description LIKE '%</a>  from user %' 
		          ORDER BY 
			          ActionLogID DESC
	          )
          ORDER BY
	          UserID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command, "Organizations,OrganizationTickets");
      }
    }

    public void LoadByTerm(int parentID, string term, int max)
    {
      if (term.Trim().Length < 2) return;
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"SELECT TOP (@MaxRows) u.* FROM UsersView u
WHERE ((RTRIM(u.FirstName) +' '+ RTRIM(u.LastName) LIKE '%'+@term+'%') 
OR (RTRIM(RTRIM(u.LastName)) +' '+ RTRIM(u.FirstName) LIKE '%'+@term+'%') 
OR (RTRIM(u.LastName) +', '+ RTRIM(u.FirstName) LIKE '%'+@term+'%') 
OR (RTRIM(u.LastName) +','+ RTRIM(u.FirstName) LIKE '%'+@term+'%') 
OR (RTRIM(u.LastName) LIKE '%'+@term+'%') 
OR (RTRIM(u.FirstName) LIKE '%'+@term+'%') 
--OR (u.email LIKE '%'+@term+'%')
)
AND (u.OrganizationID = @OrganizationID)  
AND (u.MarkDeleted = 0)
ORDER BY u.LastName, u.FirstName";
        command.CommandType = CommandType.Text;

        command.Parameters.AddWithValue("@term", term);
        command.Parameters.AddWithValue("@MaxRows", max);
        command.Parameters.AddWithValue("@OrganizationID", parentID);
        Fill(command);
      }
    }

    public void LoadByLikeName(int organizationID, string name)
    {
      LoadByLikeName(organizationID, name, int.MaxValue, false);
    }

    public void LoadByLikeName(int organizationID, string name, int max)
    {
      LoadByLikeName(organizationID, name, max, false);
    }

    public void LoadByLikeName(int organizationID, string name, int max, bool filterByUserRights, bool active = true )
    {
      if (name.Trim().Length < 2) return;
      User user = Users.GetUser(LoginUser, LoginUser.UserID);
      bool doFilter = filterByUserRights && user.TicketRights == TicketRightType.Customers;
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"SELECT TOP (@MaxRows) u.* FROM UsersView u
LEFT JOIN Organizations o1 ON o1.OrganizationID = u.OrganizationID
LEFT JOIN Organizations o2 ON o2.OrganizationID = o1.ParentID
WHERE ((RTRIM(u.FirstName) +' '+ RTRIM(u.LastName) LIKE '%'+@Name+'%') 
OR (RTRIM(RTRIM(u.LastName)) +' '+ RTRIM(u.FirstName) LIKE '%'+@Name+'%') 
OR (RTRIM(u.LastName) +', '+ RTRIM(u.FirstName) LIKE '%'+@Name+'%') 
OR (RTRIM(u.LastName) +','+ RTRIM(u.FirstName) LIKE '%'+@Name+'%') 
OR (RTRIM(u.LastName) LIKE '%'+@Name+'%') 
OR (RTRIM(u.FirstName) LIKE '%'+@Name+'%') 
OR (RTRIM(o1.Name) LIKE '%'+@Name+'%') 
OR (u.email LIKE '%'+@Name+'%')
)
AND (o2.OrganizationID = @OrganizationID)  
AND (u.MarkDeleted = 0)
AND (@ActiveOnly = 0 OR o1.IsActive = 1) 
AND (@ActiveOnly = 0 OR u.IsActive = 1) 
AND (@UseFilter=0 OR (u.OrganizationID IN (SELECT OrganizationID FROM UserRightsOrganizations WHERE UserID = @UserID)))
ORDER BY u.LastName, u.FirstName";
        command.CommandType = CommandType.Text;

        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@MaxRows", max);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
        command.Parameters.AddWithValue("@UseFilter", doFilter);
        command.Parameters.AddWithValue("@ActiveOnly", active);
        Fill(command);
      }
    }

        public void LoadByLikeNameExceptGiven(int organizationID, string name, int max, bool filterByUserRights, int givenUserID, bool active = true)
        {
            if (name.Trim().Length < 2) return;
            User user = Users.GetUser(LoginUser, LoginUser.UserID);
            bool doFilter = filterByUserRights && user.TicketRights == TicketRightType.Customers;
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText =
                    @"SELECT
                        TOP (@MaxRows) u.*
                    FROM 
                        UsersView u
                        LEFT JOIN Organizations o1
                            ON o1.OrganizationID = u.OrganizationID
                        LEFT JOIN Organizations o2 
                            ON o2.OrganizationID = o1.ParentID
                    WHERE
                        (
                            (RTRIM(u.FirstName) +' '+ RTRIM(u.LastName) LIKE '%'+@Name+'%') 
                            OR (RTRIM(RTRIM(u.LastName)) +' '+ RTRIM(u.FirstName) LIKE '%'+@Name+'%') 
                            OR (RTRIM(u.LastName) +', '+ RTRIM(u.FirstName) LIKE '%'+@Name+'%') 
                            OR (RTRIM(u.LastName) +','+ RTRIM(u.FirstName) LIKE '%'+@Name+'%') 
                            OR (RTRIM(u.LastName) LIKE '%'+@Name+'%') 
                            OR (RTRIM(u.FirstName) LIKE '%'+@Name+'%') 
                            OR (RTRIM(o1.Name) LIKE '%'+@Name+'%') 
                            OR (u.email LIKE '%'+@Name+'%')
                        )
                        AND (u.UserID <> @GivenUserID)  
                        AND (o2.OrganizationID = @OrganizationID)  
                        AND (u.MarkDeleted = 0)
                        AND (@ActiveOnly = 0 OR o1.IsActive = 1) 
                        AND (@ActiveOnly = 0 OR u.IsActive = 1) 
                        AND (@UseFilter=0 OR (u.OrganizationID IN (SELECT OrganizationID FROM UserRightsOrganizations WHERE UserID = @UserID)))
                    ORDER BY 
                        u.LastName
                        , u.FirstName";
                command.CommandType = CommandType.Text;

                command.Parameters.AddWithValue("@GivenUserID", givenUserID);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@MaxRows", max);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
                command.Parameters.AddWithValue("@UseFilter", doFilter);
                command.Parameters.AddWithValue("@ActiveOnly", active);
                Fill(command);
            }
        }

        public void CustomerLoadByLikeName(int organizationID, string name, int startIndex, bool filterByUserRights, bool active = true)
    {
        if (name.Trim().Length < 2 && name != "") return;
        User user = Users.GetUser(LoginUser, LoginUser.UserID);
        bool doFilter = filterByUserRights && user.TicketRights == TicketRightType.Customers;
        using (SqlCommand command = new SqlCommand())
        {
            StringBuilder text = new StringBuilder(
                @"WITH orderedrecords as( SELECT u.*, ROW_NUMBER() OVER (ORDER BY u.FirstName, u.LastName) AS 'RowNumber' FROM UsersView u
            LEFT JOIN Organizations o1 ON o1.OrganizationID = u.OrganizationID
            LEFT JOIN Organizations o2 ON o2.OrganizationID = o1.ParentID
            WHERE ");

            if (name.Trim() != "")
            {
                text.Append(
                @"((RTRIM(u.FirstName) +' '+ RTRIM(u.LastName) LIKE '%'+@Name+'%') 
            OR (RTRIM(RTRIM(u.LastName)) +' '+ RTRIM(u.FirstName) LIKE '%'+@Name+'%') 
            OR (RTRIM(u.LastName) +', '+ RTRIM(u.FirstName) LIKE '%'+@Name+'%') 
            OR (RTRIM(u.LastName) +','+ RTRIM(u.FirstName) LIKE '%'+@Name+'%') 
            OR (RTRIM(u.LastName) LIKE '%'+@Name+'%') 
            OR (RTRIM(u.FirstName) LIKE '%'+@Name+'%') 
            --OR (u.email LIKE '%'+@Name+'%')
            ) AND");
            }

            text.Append(@"
            (o2.OrganizationID = @OrganizationID)  
            AND (u.Firstname != '')
            AND (u.MarkDeleted = 0)
            AND (@UseFilter=0 OR (u.OrganizationID IN (SELECT OrganizationID FROM UserRightsOrganizations WHERE UserID = @UserID)))
            )
            select * from orderedrecords where RowNumber between @startIndex and @endIndex
            ORDER BY FirstName, LastName");

            command.CommandText = text.ToString();
            command.CommandType = CommandType.Text;

            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@startIndex", startIndex+1);
            command.Parameters.AddWithValue("@endIndex", startIndex + 20);
            command.Parameters.AddWithValue("@OrganizationID", organizationID);
            command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
            command.Parameters.AddWithValue("@UseFilter", doFilter);
            Fill(command);
        }
    }

    public void LoadBySubscription(int refID, ReferenceType refType)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = 
@"SELECT u.* FROM UsersView u 
LEFT JOIN Subscriptions s ON u.UserID = s.UserID 
WHERE (s.RefID = @TicketID) 
AND (s.RefType = @RefType) 
AND (u.IsActive = 1) 
AND (u.MarkDeleted = 0)
ORDER BY u.FirstName, u.LastName";

        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", refID);
        command.Parameters.AddWithValue("@RefType", (int)refType);
        Fill(command, "Users,Tickets,Subscriptions");
      }
    }
            
        public void LoadByTicketQueue(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"
SELECT u.* 
FROM UsersView u 
WHERE u.UserID IN (SELECT q.UserID FROM TicketQueue q WHERE q.TicketID=@TicketID)
AND (u.IsActive = 1) 
AND (u.MarkDeleted = 0)
ORDER BY u.FirstName, u.LastName
";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }        
    }

  
}
