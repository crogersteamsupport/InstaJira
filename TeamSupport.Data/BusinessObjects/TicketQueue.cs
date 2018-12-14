using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TicketQueueItem
  {
  }

  public partial class TicketQueue
  {

    public static TicketQueueItem Enqueue(LoginUser loginUser, int ticketID, int userID)
    {
      TicketQueueItem item = GetTicketQueueItem(loginUser, ticketID, userID);
      if (item == null)
      {
        item = (new TicketQueue(loginUser)).AddNewTicketQueueItem();
        item.TicketID = ticketID;
        item.UserID = userID;
        item.EstimatedDays = 0;
        item.Position = GetLastPosition(loginUser, userID) + 1;
        item.Collection.Save();
        return item;
      }
      return null;
    }

    public static void RepositionTickets(LoginUser loginUser, int userID, int[] ticketIDs)
    {
      TicketQueue queue = new TicketQueue(loginUser);
      queue.LoadByUser(userID);
      List<int> list = new List<int>(ticketIDs);

      foreach (TicketQueueItem item in queue) { item.Position = item.Position + 10000; }

      foreach (TicketQueueItem item in queue) { 
        item.Position = list.IndexOf(item.TicketQueueID); 
      }

      queue.Save();
    }


    public void LoadByTicket(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketQueue WHERE (TicketID = @TicketID) ORDER BY UserID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

    public void LoadByUser(int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT q.*, t.TicketNumber, ISNULL(t.UserName, 'Unassigned') AS UserName, t.Name, t.Status, t.Severity, t.UserID AS AssignedUserID, t.IsClosed FROM TicketQueue q LEFT JOIN TicketsView t ON t.TicketID = q.TicketID WHERE (q.UserID = @UserID) ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        Fill(command);
      }
    }

    public static int GetLastPosition(LoginUser loginUser, int userID)
    {
      TicketQueue queue = new TicketQueue(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT MAX(Position) FROM TicketQueue WHERE (UserID = @UserID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        object result = queue.ExecuteScalar(command);
        return result == DBNull.Value ? -1 : (int)result;
      }
    }

    public static void Dequeue(LoginUser loginUser, int ticketQueueID)
    {
      TicketQueueItem item = TicketQueue.GetTicketQueueItem(loginUser, ticketQueueID);
      if (item != null)
      {
        item.Delete();
        item.Collection.Save();
      }
    }

    public static void Dequeue(LoginUser loginUser, int ticketID, int userID)
    {
      TicketQueueItem item = TicketQueue.GetTicketQueueItem(loginUser, ticketID, userID);
      if (item != null)
      {
        item.Delete();
        item.Collection.Save();
      }
    }

    public static TicketQueueItem GetTicketQueueItem(LoginUser loginUser, int ticketID, int userID)
    {
      TicketQueue queue = new TicketQueue(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT * FROM TicketQueue WHERE (UserID = @UserID) AND (TicketID = @TicketID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        command.Parameters.AddWithValue("@UserID", userID);
        queue.Fill(command);
      }
      if (queue.IsEmpty) return null;
      return queue[0];
    }
  }

}
