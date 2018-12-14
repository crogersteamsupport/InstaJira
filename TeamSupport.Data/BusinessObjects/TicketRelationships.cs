using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TicketRelationship
  {
  }
  
  public partial class TicketRelationships
  {
    public void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketRelationships WHERE Ticket1ID = @TicketID OR Ticket2ID = @TicketID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketID", ticketID);
        Fill(command);
      }
      
    }

    public static TicketRelationship GetTicketRelationship(LoginUser loginUser, int ticket1ID, int ticket2ID)
    {
      TicketRelationships items = new TicketRelationships(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketRelationships WHERE (Ticket1ID = @Ticket1ID AND Ticket2ID = @Ticket2ID) OR (Ticket1ID = @Ticket2ID AND Ticket2ID = @Ticket1ID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("Ticket1ID", ticket1ID);
        command.Parameters.AddWithValue("Ticket2ID", ticket2ID);
        items.Fill(command);
      }
      if (items.IsEmpty) return null;
      return items[0];
    }
  }
  
}
