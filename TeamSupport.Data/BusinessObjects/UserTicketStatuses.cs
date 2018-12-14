using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class UserTicketStatus
  {
  }
  
  public partial class UserTicketStatuses
  {
    public static UserTicketStatus GetUserTicketStatus(LoginUser loginUser, int userID, int ticketID)
    {
      UserTicketStatuses uts = new UserTicketStatuses(loginUser);
      uts.LoadByUserTicket(userID, ticketID);

      if (uts.IsEmpty)
      {
        UserTicketStatus status = (new UserTicketStatuses(loginUser)).AddNewUserTicketStatus();
        status.DateRead = new DateTime(2000,1,1);
        status.IsFlagged = false;
        status.UserID = userID;
        status.TicketID = ticketID;
        status.Collection.Save();
        return status;
      }
      else
      {
        return uts[0];
      }
    }

    public void LoadByUserTicket(int userID, int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM UserTicketStatuses WHERE (UserID = @UserID) AND (TicketID = @TicketID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadByTicketIDs(int userID, int[] ticketIDs)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string ids = DataUtils.IntArrayToCommaString(ticketIDs);

        command.CommandText = "SELECT * FROM UserTicketStatuses WHERE (UserID = @UserID) AND TicketID IN (" + ids + ")";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command);
      }
    }

    public void ResetToUnreadOnSalesForceUpdate(int ticketID, List<int> userIds)
    {
      if (userIds != null && userIds.Count > 0)
      {
        string userIdsString = string.Join(",", userIds.Select(n => n.ToString()).ToArray());

        using (SqlCommand command = new SqlCommand())
        {
          command.CommandText = @"UPDATE UserTicketStatuses
SET
	UserTicketStatuses.dateRead = Tickets.dateModified
FROM
	UserTicketStatuses
	JOIN Tickets
		ON UserTicketStatuses.ticketId = Tickets.ticketId
WHERE
	UserTicketStatuses.ticketId = @ticketId
	AND UserTicketStatuses.userId IN ("+ userIdsString +")";
          command.CommandType = CommandType.Text;
          command.Parameters.AddWithValue("@ticketId", ticketID);
          Fill(command);
        }
      }
    }

  }
  
}
