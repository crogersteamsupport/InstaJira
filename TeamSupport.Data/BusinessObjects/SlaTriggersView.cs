using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class SlaTriggersViewItem
  {
  }
  
  public partial class SlaTriggersView
  {
    public void LoadByTicketType(int organizationID, int slaLevelID, int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM SlaTriggersView WHERE TicketTypeID = @TicketTypeID AND OrganizationID = @OrganizationID AND SlaLevelID = @SlaLevelID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@SlaLevelID", slaLevelID);
        command.Parameters.AddWithValue("@TicketTypeID", ticketTypeID);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByTicket(int ticketID)
    {
/*
        MIN(DATEDIFF(n, GETUTCDATE(), DATEADD(n, case when st.TimeToClose < 1 then NULL else st.TimeToClose end, t.DateCreated))) AS ViolationTimeClosed,
        MIN(DATEDIFF(n, GETUTCDATE(), DATEADD(n, case when st.TimeLastAction < 1 then NULL else st.TimeLastAction end, a1.DateCreated))) AS ViolationLastAction,
        MIN(DATEDIFF(n, GETUTCDATE(), DATEADD(n, case when st.TimeInitialResponse < 1 OR a2.ActionCount > 0 then NULL else st.TimeInitialResponse end,t.DateCreated))) AS ViolationInitialResponse,
        MIN(DATEDIFF(n, GETUTCDATE(), DATEADD(n, case when st.TimeToClose < 1 then NULL else st.TimeToClose-st.WarningTime end, t.DateCreated))) AS WarningTimeClosed,
        MIN(DATEDIFF(n, GETUTCDATE(), DATEADD(n, case when st.TimeLastAction < 1 then NULL else st.TimeLastAction-st.WarningTime end, a1.DateCreated))) AS WarningLastAction,
        MIN(DATEDIFF(n, GETUTCDATE(), DATEADD(n, case when st.TimeInitialResponse < 1 OR a2.ActionCount > 0 then NULL else st.TimeInitialResponse-st.WarningTime end,t.DateCreated))) AS WarningInitialResponse
*/ 
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
      @"SELECT stv.* FROM
      ( 
        SELECT t.TicketID, st.SlaTriggerID,
        MIN(dbo.GetSlaExpireMinutes(st.UseBusinessHours, o2.BusinessDays, o2.BusinessDayStart, o2.BusinessDayEnd, t.DateCreated, st.TimeToClose)) AS ViolationTimeClosed,
        MIN(dbo.GetSlaExpireMinutes(st.UseBusinessHours, o2.BusinessDays, o2.BusinessDayStart, o2.BusinessDayEnd, t.DateCreated, st.TimeToClose -st.WarningTime)) AS WarningTimeClosed,
        MIN(dbo.GetSlaExpireMinutes(st.UseBusinessHours, o2.BusinessDays, o2.BusinessDayStart, o2.BusinessDayEnd, a1.DateCreated, st.TimeLastAction)) AS ViolationLastAction,
        MIN(dbo.GetSlaExpireMinutes(st.UseBusinessHours, o2.BusinessDays, o2.BusinessDayStart, o2.BusinessDayEnd, a1.DateCreated, st.TimeLastAction -st.WarningTime)) AS WarningLastAction,
        MIN(dbo.GetSlaExpireMinutes(st.UseBusinessHours, o2.BusinessDays, o2.BusinessDayStart, o2.BusinessDayEnd, t.DateCreated, case when st.TimeInitialResponse < 1 OR a2.ActionCount > 0 then NULL else st.TimeInitialResponse end)) AS ViolationInitialResponse,
        MIN(dbo.GetSlaExpireMinutes(st.UseBusinessHours, o2.BusinessDays, o2.BusinessDayStart, o2.BusinessDayEnd, t.DateCreated, case when st.TimeInitialResponse < 1 OR a2.ActionCount > 0 then NULL else st.TimeInitialResponse end-st.WarningTime)) AS WarningInitialResponse
        FROM Tickets t
        LEFT JOIN OrganizationTickets ot ON ot.TicketID = t.TicketID
        LEFT JOIN Organizations o1 ON o1.OrganizationID = ot.OrganizationID
        LEFT JOIN Organizations o2 ON o2.OrganizationID = t.OrganizationID
        LEFT JOIN SlaTriggers st ON ((st.SlaLevelID = o1.SlaLevelID AND ot.OrganizationID IS NOT NULL) OR (o2.InternalSlaLevelID = st.SlaLevelID AND ot.OrganizationID IS NULL))  AND st.TicketTypeID = t.TicketTypeID AND st.TicketSeverityID = t.TicketSeverityID
        LEFT JOIN TicketStatuses ts ON ts.TicketStatusID = t.TicketStatusID
        LEFT JOIN (SELECT TicketID, MAX(DateCreated) AS DateCreated FROM Actions GROUP BY TicketID) a1 ON a1.TicketID = t.TicketID
        LEFT JOIN (SELECT TicketID, COUNT(ActionID) AS ActionCount FROM Actions WHERE SystemActionTypeID <> 1 GROUP BY TicketID) a2 ON a2.TicketID = t.TicketID
        WHERE st.SlaTriggerID IS NOT NULL
        AND t.TicketID = @TicketID
        AND ISNULL(ts.IsClosed, 0) <> 1
        GROUP BY t.TicketID, st.SlaTriggerID
      ) AS x
      LEFT JOIN SlaTriggersView stv ON stv.SlaTriggerID = x.SlaTriggerID";


        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TicketID", ticketID);
        Fill(command);
      }
    }

        public void LoadByTicketId(int id)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT * FROM SlaTriggersView WHERE SlaTriggerID = dbo.GetSlaTrigger(@TicketID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", id);

                Fill(command);
            }
        }

        public void LoadByCustomer(int organizationId, int customerId)
        {
            string sql = @"SELECT SlaTriggersView.SlaLevelID, SlaTriggersView.SlaTriggerID, SlaTriggersView.LevelName, SlaTriggersView.Severity, SlaTriggersView.TicketType, SlaLevelsAssociated.SLAType
FROM
	SlaTriggersView
	JOIN (SELECT SlaLevels.SlaLevelID, 'Customer-Product' AS SLAType, 1 AS [SlaHierarchy]
			FROM
				OrganizationProducts
				JOIN SlaLevels
					ON OrganizationProducts.SlaLevelID = SlaLevels.SlaLevelID
			WHERE OrganizationProducts.OrganizationID = @customerId
            UNION
            SELECT SlaLevels.SlaLevelID, 'Product' AS SLAType, 2 AS [SlaHierarchy]
			FROM
				OrganizationProducts
				JOIN Products
					ON OrganizationProducts.ProductID = Products.ProductID
				JOIN SlaLevels
					ON Products.SlaLevelID = SlaLevels.SlaLevelID				
			WHERE OrganizationProducts.OrganizationID = @customerId
            UNION
            SELECT SlaLevels.SlaLevelID, 'Customer' AS SLAType, 3 AS [SlaHierarchy]
			FROM
				Organizations
				JOIN SlaLevels
					ON Organizations.SlaLevelID = SlaLevels.SlaLevelID
			WHERE Organizations.OrganizationID = @customerId) AS SlaLevelsAssociated
	ON SlaTriggersView.SlaLevelID = SlaLevelsAssociated.SlaLevelID
WHERE OrganizationID = @organizationId
ORDER BY SlaLevelsAssociated.SlaHierarchy";

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@organizationId", organizationId);
                command.Parameters.AddWithValue("@customerId", customerId);
                Fill(command);
            }
        }
  }
  
}
