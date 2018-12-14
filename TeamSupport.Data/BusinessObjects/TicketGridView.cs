using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TicketGridViewItem 
  {
  }

  public partial class TicketGridView 
  {
    public void LoadByRecentlyModified(int organizationID, int top)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT TOP " + top.ToString() + @" tgv.* FROM TicketGridView tgv 
                                WHERE (tgv.OrganizationID = @OrganizationID)
                                ORDER BY tgv.DateModified DESC";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
        Fill(command, "TicketGridView");
      }
    }

    public void LoadByTicketNumber(int organizationID, int ticketNumber)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketGridView WHERE OrganizationID = @OrganizationID AND TicketNumber = @TicketNumber";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@TicketNumber", ticketNumber);
        Fill(command, "TicketGridView");
      }

    }

  }


}
