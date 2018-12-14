using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class Reminder
    {
    }

    public partial class Reminders
    {

        public void LoadByUser(int userID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Reminders WHERE (UserID = @UserID) AND (IsDismissed = 0) AND RefType <> 61 ORDER BY DueDate";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                Fill(command);
            }
        }

        public void LoadByUserMonth(DateTime date, int userID, string Type, string ID)
        {
            string additional = "";
            string userStr = "(UserID = @UserID)";

            if (Type == "9")
            {
                additional = "and (refid in (select TicketID from OrganizationTickets where OrganizationID = @id) and RefType = 17) or (RefType = @type and RefID = @ID)";
                userStr = "1=1";
            }

            if (Type == "4")
            {
                additional = "and (refid in (select TicketID from Tickets where GroupID = @id) and RefType = 17)";
                userStr = "1=1";
            }

            if (Type == "0")
            {
                additional = "AND (Reftype = @type) AND (RefID = @id)";
                userStr = "1=1";
            }


            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = string.Format("SELECT * FROM Reminders WHERE {0} AND (IsDismissed = 0) AND (Month(Duedate) = @month) AND (Year(Duedate) = @year) {1} ORDER BY DueDate", userStr, additional);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@month", date.Month);
                command.Parameters.AddWithValue("@year", date.Year);
                command.Parameters.AddWithValue("@type", Type);
                command.Parameters.AddWithValue("@id", ID);
                Fill(command);
            }
        }

        public void LoadByItem(ReferenceType refType, int refID, int? userID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Reminders WHERE (UserID = @UserID OR @UserID < 0) AND (IsDismissed = 0) AND (RefType = @RefType) AND (RefID = @RefID) ORDER BY DueDate";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID ?? -1);
                command.Parameters.AddWithValue("@RefType", refType);
                command.Parameters.AddWithValue("@RefID", refID);
                Fill(command);
            }
        }

        public void LoadByItemAll(ReferenceType refType, int refID, int? userID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Reminders WHERE (IsDismissed = 0) AND (RefType = @RefType) AND (RefID = @RefID) ORDER BY DueDate";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID ?? -1);
                command.Parameters.AddWithValue("@RefType", refType);
                command.Parameters.AddWithValue("@RefID", refID);
                Fill(command);
            }
        }

        public void LoadForEmail()
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
            SELECT 
                *
            FROM 
                Reminders 
            WHERE 
                HasEmailSent = 0
                AND IsDismissed = 0
                AND UserID IS NOT NULL
                AND DueDate <= GETUTCDATE()
            ORDER BY
                DueDate";
                command.CommandType = CommandType.Text;
                Fill(command);
            }
        }

        public void LoadByTaskID(int taskID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                SELECT
                    *
                FROM
                    Reminders
                WHERE
                    RefType = 61
                    AND RefID = @TaskID";

                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TaskID", taskID);
                Fill(command);
            }
        }

        public static Reminder GetReminderByTaskID(LoginUser loginUser, int taskID)
        {
            Reminders reminders = new Reminders(loginUser);
            reminders.LoadByTaskID(taskID);
            if (reminders.IsEmpty)
                return null;
            else
                return reminders[0];
        }
    }

}
