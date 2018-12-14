using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
	public partial class TicketLinkToJiraItem
	{
	}
  
	public partial class TicketLinkToJira
	{
		//Changes to this method needs to be applied to TicketsView.LoadToPushToJira also.
		public void LoadToPushToJira(CRMLinkTableItem item)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText =
				@"SELECT j.* 
				FROM 
					TicketLinkToJira j
					JOIN Tickets t
						ON j.TicketID = t.TicketID
				WHERE 
					j.SyncWithJira = 1
					AND t.OrganizationID = @OrgID 
					AND j.CrmLinkID = @CrmLinkId
					AND 
					(
						j.DateModifiedByJiraSync IS NULL
						OR 
						(
							t.DateModified > DATEADD(s, 2, j.DateModifiedByJiraSync)
							AND t.DateModified > @DateModified
						)
					)
				ORDER BY 
					t.DateCreated DESC";

				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@OrgID", item.OrganizationID);
				command.Parameters.AddWithValue("@DateModified", item.LastLink == null ? new DateTime(1753, 1, 1) : item.LastLinkUtc.Value.AddHours(-1));
				command.Parameters.AddWithValue("@CrmLinkId", item.CRMLinkID);

				//command.CommandText = "TicketLinkToJiraGet";
				//command.CommandType = CommandType.StoredProcedure;
				//command.Parameters.AddWithValue("@organizationId", item.OrganizationID);
				//command.Parameters.AddWithValue("@dateModified", item.LastLink == null ? new DateTime(1753, 1, 1) : item.LastLinkUtc.Value.AddHours(-1));
				//command.Parameters.AddWithValue("@crmLinkId", item.CRMLinkID);
				//command.Parameters.AddWithValue("@isTicketLinkToJira", 1);
				command.CommandTimeout = 90;
				Fill(command);
			}
		}

		public void LoadByTicketID(int ticketID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText =
				@"SELECT * 
				FROM 
					TicketLinkToJira
				WHERE 
					TicketID = @TicketID
				";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@TicketID", ticketID);
				command.CommandTimeout = 60;

				Fill(command);
			}
		}

		public TicketLinkToJiraItem FindByTicketID(int ticketID)
		{
			foreach (TicketLinkToJiraItem item in this)
			{
				if (item.TicketID == ticketID)
				{
					return item;
				}
			}

			return null;
		}

		public void LoadByCrmLinkId(int crmLinkId, bool excludeNulls = false)
		{
			using (SqlCommand command = new SqlCommand())
			{
				string query = @"SELECT * 
				FROM 
					TicketLinkToJira
				WHERE 
					CrmLinkID = @crmLinkId
				";

				if (excludeNulls)
				{
					query += "AND JiraID IS NOT NULL";
				}

				command.CommandText = query;
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@crmLinkId", crmLinkId);

				Fill(command);
			}
		}
	}
}
