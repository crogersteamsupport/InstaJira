using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public class Subscriptions : BaseCollection
  {

    public Subscriptions(LoginUser loginUser): base (loginUser)
    {
    }

    public override string TableName
    {
      get { return "Subscriptions"; }
    }

    public override string PrimaryKeyFieldName
    {
      get { return ""; }
    }

    public override void Save(SqlConnection connection)
    {
      
    }

    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
    }

    public static bool IsUserSubscribed(LoginUser loginUser, int userID, ReferenceType refType, int refID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT COUNT(*) FROM Subscriptions WHERE (RefType = @RefType) AND (RefID = @RefID) AND (UserID = @UserID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@RefType", (int)refType);
        command.Parameters.AddWithValue("@RefID", refID);

        Subscriptions subscriptions = new Subscriptions(loginUser);
        return (int)subscriptions.ExecuteScalar(command, "Subscriptions,Users") > 0;

      }
    }

    public static bool ToggleSubscription(LoginUser loginUser, int userID, ReferenceType refType, int refID)
    {
      bool isSubscribed = IsUserSubscribed(loginUser, userID, refType, refID);
      if (isSubscribed)
      {
        RemoveSubscription(loginUser, userID, refType, refID);
        return false;
      }
      else
      {
        AddSubscription(loginUser, userID, refType, refID);
        return true;
      }
    
    }

    public static void RemoveSubscription(LoginUser loginUser, int userID, ReferenceType refType, int refID)
    {
      Subscriptions subscriptions = new Subscriptions(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "DELETE FROM Subscriptions WHERE (RefType = @RefType) AND (RefID = @RefID) AND (UserID = @UserID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@RefType", (int)refType);
        command.Parameters.AddWithValue("@RefID", refID);
        subscriptions.ExecuteNonQuery(command, "Subscriptions,Tickets,Users");
      }


      string description = "Unsubscribed '" + Users.GetUserFullName(loginUser, userID) + "' from ";
      if (refType == ReferenceType.Tickets)
      {
        Ticket ticket = (Ticket)Tickets.GetTicket(loginUser, refID);
        description = description + Tickets.GetTicketLink(ticket);
      }
      ActionLogs.AddActionLog(loginUser, ActionLogType.Delete, refType, refID, description);
      ActionLogs.AddActionLog(loginUser, ActionLogType.Delete, ReferenceType.Users, userID, description);
    }

    public static void AddSubscription(LoginUser loginUser, int userID, ReferenceType refType, int refID)
    {
      if (IsUserSubscribed(loginUser, userID, refType, refID)) return;
      Subscriptions subscriptions = new Subscriptions(loginUser);
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "INSERT INTO Subscriptions (RefType, RefID, UserID, DateCreated, DateModified, CreatorID, ModifierID) VALUES (@RefType, @RefID, @UserID, @DateCreated, @DateModified, @CreatorID, @ModifierID)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@RefID", refID);
        command.Parameters.AddWithValue("@RefType", (int)refType);
        command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
        command.Parameters.AddWithValue("@DateModified", DateTime.UtcNow);
        command.Parameters.AddWithValue("@CreatorID", loginUser.UserID);
        command.Parameters.AddWithValue("@ModifierID", loginUser.UserID);
        subscriptions.ExecuteNonQuery(command, "Subscriptions,Tickets,Users");
      }

      string description = "Subscribed '" + Users.GetUserFullName(loginUser, userID) + "' to ";
      if (refType == ReferenceType.Tickets)
      {
        Ticket ticket = (Ticket)Tickets.GetTicket(loginUser, refID);
        description = description + Tickets.GetTicketLink(ticket);
      }

      ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, refType, refID, description);
      ActionLogs.AddActionLog(loginUser, ActionLogType.Insert, ReferenceType.Users, userID, description);
    }

		public static List<int> GetByOrganizationId(LoginUser loginUser, int organizationId)
		{
			List<int> subscriptionIds = new List<int>();

			using (SqlCommand command = new SqlCommand())
			using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
			{
				command.CommandText = "SELECT RefID FROM Subscriptions WHERE RefID = @RefID AND RefType = @RefType";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@RefID", organizationId);
				command.Parameters.AddWithValue("@RefType", (int)ReferenceType.Organizations);

				connection.Open();
				SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
				command.Connection = connection;
				command.Transaction = transaction;
				SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

				while (reader.Read())
				{
					int refId = (int)reader["RefID"];
					subscriptionIds.Add(refId);
				}
			}

			return subscriptionIds;
		}

		public static void SetByOrganizationId(LoginUser loginUser, int organizationId, int loserOrganizationId)
		{
			Subscriptions subscriptions = new Subscriptions(loginUser);
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "UPDATE Subscriptions SET RefID = @RefID WHERE RefType = @RefType AND RefID = @LoserRefID";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@RefID", organizationId);
				command.Parameters.AddWithValue("@RefType", (int)ReferenceType.Organizations);
				command.Parameters.AddWithValue("@LoserRefID", loserOrganizationId);
				subscriptions.ExecuteNonQuery(command, "Subscriptions");
			}
		}
	}
}
