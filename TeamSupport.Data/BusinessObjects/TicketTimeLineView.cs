using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class TicketTimeLineViewItem
    {
    }

    public partial class TicketTimeLineView
    {
        public void LoadByRange(int ticketID, int start, int end)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"WITH BaseQuery AS(
                                        SELECT *, ROW_NUMBER() OVER (ORDER BY DateCreated DESC) AS 'RowNum' FROM TicketTimelineView WHERE TicketID = @TicketID)
                                        SELECT * FROM BaseQuery WHERE RowNum BETWEEN @Start AND @End ORDER BY BaseQuery.RowNum";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
                command.Parameters.AddWithValue("@Start", start);
                command.Parameters.AddWithValue("@End", end);
                Fill(command);
            }
        }

        public void LoadByRefIDAndType(int id, bool isWC) {
            using (SqlCommand command = new SqlCommand()) {
                command.CommandText = @"SELECT * FROM TicketTimelineView WHERE RefID = @RefID AND IsWC = @IsWC";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@RefID", id);
                command.Parameters.AddWithValue("@IsWC", isWC);
                Fill(command);
            }
        }

        public void Pinned (int ticketID) {
            using (SqlCommand command = new SqlCommand()) {
                command.CommandText = "SELECT *, (SELECT COUNT(RefID) FROM TicketTimelineView AS temp WHERE temp.DateCreated <= TicketTimelineView.DateCreated AND temp.TicketID = @TicketID) AS position FROM TicketTimelineView WHERE TicketID = @TicketID AND isPinned > 0";
                // command.CommandText = "SELECT * FROM TicketTimelineView WHERE TicketID = @TicketID AND isPinned > 0";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                Fill(command);
            }
        }
    }
}
