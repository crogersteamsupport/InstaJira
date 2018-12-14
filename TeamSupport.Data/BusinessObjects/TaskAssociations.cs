using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class TaskAssociation
    {
    }

    public partial class TaskAssociations
    {
        public void DeleteAssociation(int taskID, int refID, ReferenceType refType)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "DELETE FROM TaskAssociations WHERE (TaskID = @TaskID) AND (RefID = @RefID) AND (RefType = @RefType)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TaskID", taskID);
                command.Parameters.AddWithValue("@RefID", refID);
                command.Parameters.AddWithValue("@RefType", (int)refType);
                ExecuteNonQuery(command, "TaskAssociations");
            }
            //Organization org = (Organization)Organizations.GetOrganization(LoginUser, organizationID);
            //Ticket ticket = (Ticket)Tickets.GetTicket(LoginUser, ticketID);
            //string description = "Removed '" + org.Name + "' from the customer list for " + GetTicketLink(ticket);
            //ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Tickets, ticketID, description);
            //ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Organizations, organizationID, description);
        }

        public void DeleteByReminderIDOnly(int taskID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "DELETE TaskAssociations WHERE TaskID = @TaskID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TaskID", taskID);
                ExecuteNonQuery(command, "TaskAssociations");
            }
        }

        public void LoadByTaskIDOnly(int taskID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                SELECT 
                    * 
                FROM 
                    TaskAssociations
                WHERE 
                    TaskID = @TaskID
                ORDER BY RefType";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TaskID", taskID);
                Fill(command);
            }
        }
    }
}
