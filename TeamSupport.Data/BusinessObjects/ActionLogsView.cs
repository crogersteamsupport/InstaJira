using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class ActionLogsViewItem
  {
  }
  
  public partial class ActionLogsView
  {
    public void LoadByReference(int refID, ReferenceType referenceType, string orderBy = "")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = " SELECT * FROM ActionLogsView WHERE (RefID = @RefID) AND (RefType = @RefType) AND (OrganizationID = @OrganizationID)";
        if (orderBy != string.Empty)
        {
          command.CommandText += " ORDER BY " + orderBy;
        }
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@RefID", refID);
        command.Parameters.AddWithValue("@RefType", referenceType);
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        Fill(command);
      }
    }

    public void LoadByTicketNumber(int ticketNumber, string orderBy = "")
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
          SELECT
            alv.*
          FROM
            ActionLogsView alv
            JOIN Tickets t
              ON alv.RefID = t.TicketID
          WHERE
            t.TicketNumber = @TicketNumber
            AND alv.RefType = @RefType
            AND alv.OrganizationID = @OrganizationID";
        if (orderBy != string.Empty)
        {
          command.CommandText += " ORDER BY " + orderBy;
        }
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketNumber", ticketNumber);
        command.Parameters.AddWithValue("@RefType", (int)ReferenceType.Tickets);
        command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
        Fill(command);
      }
    }
  }
  
}
