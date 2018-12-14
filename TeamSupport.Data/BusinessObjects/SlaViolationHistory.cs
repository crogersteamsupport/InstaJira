using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class SlaViolationHistoryItem
  {
  }
  
  public partial class SlaViolationHistory
  {
        public void LoadByCustomer(int customerID, string sortColumn = "TicketID", string sortDirection = "ASC")
        {
            using (SqlCommand command = new SqlCommand())
            {
                string sql = @"
SELECT SlaViolationHistoryView.*, OrganizationTickets.*, SlaTriggersView.LevelName, SlaTriggersView.TicketType AS SlaTicketType, SlaTriggersView.Severity AS SlaSeverity
FROM
	SlaViolationHistoryView
	JOIN OrganizationTickets
		ON SlaViolationHistoryView.TicketID = OrganizationTickets.TicketID
	LEFT JOIN SlaTriggersView
		ON SlaTriggersView.SlaTriggerID = SlaViolationHistoryView.SlaTriggerId
WHERE OrganizationTickets.OrganizationID = @customerID";

                sql += string.Format(" ORDER BY {0} {1}", sortColumn, sortDirection);
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@customerID", customerID);
                Fill(command);
            }
        }
    }
}
