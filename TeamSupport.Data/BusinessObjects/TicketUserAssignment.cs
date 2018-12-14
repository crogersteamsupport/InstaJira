using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using TeamSupport.Data.Classes;

namespace TeamSupport.Data
{
	public class TicketUserAssignment
	{
		public TicketUserAssignment() { }

		public TicketUserAssignment(LoginUser LoginUser)
		{
			_loginUser = LoginUser;
		}

		private LoginUser _loginUser { get; set; }
		public LoginUser LoginUser
		{
			get
			{
				return _loginUser;
			}
		}

		public List<UserAssignmentHistoryItem> History { get;set; }

		public void LoadByTicketList(int organizationId, string tickets, bool byId)
		{
			string orderBy = "ORDER BY TicketNumber";
			string sql = string.Empty;
			DataTable dtAssignmentHistory = new DataTable();

			using (SqlCommand command = new SqlCommand())
			{
				SqlParameterCollection filterParameters = command.Parameters;
                string whereClause = " WHERE Tickets.OrganizationID = @organizationId AND UserAssignmentHistory.UserID IS NOT NULL ";
				whereClause += (byId ? " AND UserAssignmentHistory.TicketID IN (" + tickets + ")" : " AND Tickets.TicketNumber IN (" + tickets + ")");

				sql = @"SELECT
	UserAssignmentHistory.UserAssignmentHistoryID,
	UserAssignmentHistory.TicketID,
	Tickets.TicketNumber,
	UserAssignmentHistory.UserID,
	Tickets.OrganizationID,
	users.FirstName + ' ' + users.LastName AS UserName,
	UserAssignmentHistory.DateAssigned
FROM
	UserAssignmentHistory WITH(NOLOCK)
	JOIN Tickets WITH(NOLOCK)
		ON UserAssignmentHistory.TicketID = Tickets.TicketID
	LEFT JOIN users WITH(NOLOCK)
		ON UserAssignmentHistory.UserID = users.UserID " + whereClause + " " + orderBy;

				command.CommandText = sql;
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@organizationId", organizationId);

				using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
				{
					connection.Open();
					command.Connection = connection;

					using (SqlDataReader sdr = command.ExecuteReader())
					{
						dtAssignmentHistory.Load(sdr);
					}

					connection.Close();
				}
			}

			DataNamesMapper<UserAssignmentHistoryItem> mapper = new DataNamesMapper<UserAssignmentHistoryItem>();
			History = mapper.Map(dtAssignmentHistory).ToList();
		}

		public TicketUserAssignmentHistory GetTicketUserAssignmentHistory(int pageNumber = 0, int pageSize = 50)
		{
			TicketUserAssignmentHistory assignmentHistory = new TicketUserAssignmentHistory();
			List<TicketAssignmentInfo> assigmentHistoryList = new List<TicketAssignmentInfo>();

			foreach (int ticketId in History.Select(p => p.TicketID).Distinct().Skip(pageNumber * pageSize).Take(pageSize))
			{
				List<TicketAssignmentDetail> assignmentsList = History.Where(p => p.TicketID == ticketId).Select(p => new TicketAssignmentDetail() { UserID = p.UserID, UserName = p.UserName, DateAssigned = p.DateAssigned }).ToList();
				TicketAssignmentInfo ticketAssignments = new TicketAssignmentInfo
				{
					TicketId = ticketId,
					TicketNumber = History.Where(p => p.TicketID == ticketId).Select(p => p.TicketNumber).First(),
					UserAssigmentHistory = new UserAssigmentHistory() { Assigment = assignmentsList }
				};

				assigmentHistoryList.Add(ticketAssignments);
			}

			assignmentHistory = new TicketUserAssignmentHistory
			{
				Tickets = assigmentHistoryList,
				TotalRecords = History.Select(p => p.TicketID).Distinct().Count()
			};

			return assignmentHistory;
		}

		public class UserAssignmentHistoryItem
		{
			[DataNames("UserAssignmentHistoryID")]
			public int UserAssignmentHistoryID { get; set; }

			[DataNames("TicketID")]
			public int TicketID { get; set; }

			[DataNames("TicketNumber")]
			public int TicketNumber { get; set; }

			[DataNames("UserID")]
			public int UserID { get; set; }

			[DataNames("OrganizationID")]
			public int OrganizationID { get; set; }

			[DataNames("UserName")]
			public string UserName { get; set; }

			[DataNames("DateAssigned")]
			public DateTime DateAssigned { get; set; }
		}

		public class TicketUserAssignmentHistory
		{
			public int TotalRecords { get; set; }
			public List<TicketAssignmentInfo> Tickets { get; set; }
		}

		public class TicketAssignmentInfo
		{
			public int TicketId { get; set; }
			public int TicketNumber { get; set; }
			public UserAssigmentHistory UserAssigmentHistory { get; set; }
		}

		public class UserAssigmentHistory
		{
			public List<TicketAssignmentDetail> Assigment { get; set; }
		}

		public class TicketAssignmentDetail
		{
			public int? UserID { get; set; }
			public string UserName { get; set; }
			public DateTime DateAssigned { get; set; }
		}
	}
}
