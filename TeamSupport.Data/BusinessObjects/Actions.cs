using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Data.Linq;
using TeamSupport.Data.BusinessObjects;

namespace TeamSupport.Data
{
    public partial class Action
    {
        internal Attachments GetAttachments()
        {
            Attachments attachments = new Attachments(BaseCollection.LoginUser);
            attachments.LoadByActionID(ActionID);
            return attachments;
        }

        public ActionsViewItem GetActionView()
        {
            return ActionsView.GetActionsViewItem(BaseCollection.LoginUser, ActionID);
        }

        public string ActionTypeName
        {
            get
            {
                if (Row.Table.Columns.Contains("ActionTypeName") && Row["ActionTypeName"] != DBNull.Value)
                {
                    return (string)Row["ActionTypeName"];
                }
                else return "";
            }

        }

        public string DisplayName
        {
            get
            {
                string title = "";

                switch (SystemActionTypeID)
                {
                    case SystemActionType.Description: title = "Description"; break;
                    case SystemActionType.Resolution: title = "Resolution"; break;
                    case SystemActionType.Email: title = "Email: " + Name; break;
                    case SystemActionType.UpdateRequest: title = "Update Request"; break;
                    case SystemActionType.Chat: title = "Chat"; break;
                    case SystemActionType.Reminder: title = "Reminder"; break;
                    default:
                        title = ActionTypeName == "" ? "[No Action Type]" : ActionTypeName;
                        if (!string.IsNullOrEmpty(Name)) title += ": " + Name;
                        break;
                }
                return title;
            }

        }

        /// <summary>
        /// This will clone all Action object writable properties using reflection. This way we make sure that if there are more fields added (or deleted) in this table this will still work.
        /// This was the easier way to do this due to the amount of properties to process and the high possibility of this table scheme being updated often.
        /// I wanted to do this with serialization (JsonConvert.Deserialize/Serialize object), but didn't work due to how our Data objects are generated (Collections in them), and the primary key.
        /// </summary>
        /// <param name="clone">The empty initialized Action object to clone to.</param>
        public void ClonePropertiesTo(Action clone)
        {
            foreach (System.Reflection.PropertyInfo sourcePropertyInfo in this.GetType().GetProperties())
            {
                if (sourcePropertyInfo.CanWrite
                    && sourcePropertyInfo.Name.ToLower() != "basecollection"
                    && sourcePropertyInfo.PropertyType != typeof(DateTime)
                    && sourcePropertyInfo.PropertyType != typeof(DateTime?))
                {
                    System.Reflection.PropertyInfo destPropertyInfo = clone.GetType().GetProperty(sourcePropertyInfo.Name);
                    destPropertyInfo.SetValue(
                        clone,
                        sourcePropertyInfo.GetValue(this, null),
                        null);
                }
            }

            //DateTime properties are special it needs to be the UTC value. The DateTime (and DateTime?) properties always have a UTC version
            foreach (System.Reflection.PropertyInfo sourcePropertyInfo in this.GetType().GetProperties())
            {
                if (sourcePropertyInfo.CanRead
                    && sourcePropertyInfo.Name.Substring(sourcePropertyInfo.Name.Length - 3).ToLower() == "utc"
                    && (sourcePropertyInfo.PropertyType == typeof(DateTime)
                        || sourcePropertyInfo.PropertyType == typeof(DateTime?)))
                {
                    System.Reflection.PropertyInfo destPropertyInfo = clone.GetType().GetProperty(sourcePropertyInfo.Name.Substring(0, sourcePropertyInfo.Name.Length - 3));
                    destPropertyInfo.SetValue(
                        clone,
                        sourcePropertyInfo.GetValue(this, null),
                        null);
                }
            }
        }

        // extracted from ts-app\WebApp\App_Code\TicketPageService.cs UpdateAction() AND
        // also extracted from ts-app\webapp\app_code\ticketservice.cs 
        public static void Create(DataContext db, int organizationID, int userID, int ticketID, ref ActionProxy actionProxy)
        {
            Action action = new Actions(new LoginUser(userID, organizationID)).AddNewAction();
            action.TicketID = ticketID;
            action.CreatorID = userID;
            action.Description = actionProxy.Description;
            action.SystemActionTypeID = actionProxy.SystemActionTypeID;
            if (!String.IsNullOrEmpty(actionProxy.Name))
                action.Name = actionProxy.Name;

            // add signature?
            string signature = db.ExecuteQuery<string>($"SELECT [Signature] FROM Users WITH (NOLOCK) WHERE UserID={userID} AND OrganizationID={organizationID}").FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(signature) && actionProxy.IsVisibleOnPortal && !actionProxy.IsKnowledgeBase && !actionProxy.Description.Contains(signature))
                action.Description += "<br/><br/>" + signature;

            action.ActionSource = actionProxy.ActionSource;
            action.ActionTypeID = actionProxy.ActionTypeID;
            action.DateStarted = actionProxy.DateStarted;
            action.TimeSpent = actionProxy.TimeSpent;
            action.IsKnowledgeBase = actionProxy.IsKnowledgeBase;
            action.IsVisibleOnPortal = actionProxy.IsVisibleOnPortal;
            action.Collection.Save();
            actionProxy = action.GetProxy();
        }

    }

    public partial class Actions
    {
        private bool _updateChildTickets = true;

        private string _actionLogInstantMessage = null;
        private bool _isAdminClean = false;
        public string ActionLogInstantMessage
        {
            get
            {
                return _actionLogInstantMessage;
            }
            set
            {
                _actionLogInstantMessage = value;
            }
        }

        public bool isAdminClean
        {
            get
            {
                return _isAdminClean;
            }
            set
            {
                _isAdminClean = value;
            }
        }

        partial void BeforeRowDelete(int actionID)
        {
            Action action = (Action)Actions.GetAction(LoginUser, actionID);
            string description = "Deleted action '" + action.Name + "' from " + Tickets.GetTicketLink(LoginUser, action.TicketID);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Tickets, action.TicketID, description);
        }

        partial void BeforeRowEdit(Action action)
        {
            var oldAction = (Action)Actions.GetActionByID(LoginUser, action.ActionID);

            action.Description = HtmlUtility.FixScreenRFrame((action.Row["Description"] == DBNull.Value) ? string.Empty : action.Description);
            string actionNumber = GetActionNumber(action.TicketID, action.ActionID);
            string description = "Modified action #" + actionNumber + " on " + Tickets.GetTicketLink(LoginUser, action.TicketID) + (oldAction.IsVisibleOnPortal != action.IsVisibleOnPortal ? " - action changed is visible to = " + action.IsVisibleOnPortal : "");
            if (!this.isAdminClean)
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, action.TicketID, description);
        }

        private string GetActionNumber(int ticketID, int actionID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM Actions WHERE TicketID = @TicketID AND ActionID <= @ActionID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                command.Parameters.AddWithValue("@ActionID", actionID);

                return ExecuteScalar(command, "Tickets").ToString();
            }
        }

        partial void BeforeRowInsert(Action action)
        {
            action.Description = HtmlUtility.FixScreenRFrame((action.Row["Description"] == DBNull.Value) ? string.Empty : action.Description);
        }

        partial void AfterRowInsert(Action action)
        {
            string description = string.Format("Added {0} action ",  action.IsVisibleOnPortal ? "public" : "private" );
            if (_actionLogInstantMessage != null)
            {
                description = _actionLogInstantMessage;
            }
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tickets, action.TicketID, description + action.Name + " to " + Tickets.GetTicketLink(LoginUser, action.TicketID));
            if (_updateChildTickets) AddChildActions(action);
        }


        public void AddChildActions(Action action)
        {
            Tickets tickets = new Tickets(LoginUser);
            tickets.LoadChildren(action.TicketID);
            Actions actions = new Actions(LoginUser);
            actions._updateChildTickets = false;
            foreach (Ticket ticket in tickets)
            {
                Action newAction = actions.AddNewAction();
                newAction.ActionTypeID = action.ActionTypeID;
                newAction.SystemActionTypeID = action.SystemActionTypeID;
                newAction.Name = action.Name;
                newAction.ActionSource = action.ActionSource;
                newAction.Description = action.Description;// +"<p>This action was added by the parent ticket.</p>";
                newAction.TimeSpent = action.TimeSpent;
                newAction.Row["DateStarted"] = action.Row["DateStarted"];
                newAction.IsVisibleOnPortal = action.IsVisibleOnPortal;
                newAction.IsKnowledgeBase = action.IsKnowledgeBase;
                newAction.ImportID = action.ImportID;
                newAction.TicketID = ticket.TicketID;
            }
            actions.Save();

        }

        /// <summary>
        /// Loads actions for a ticket and orders them by the latest date created first
        /// </summary>
        /// <param name="ticketID"></param>
        public void LoadByTicketID(int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT a.*, u.FirstName + ' ' + u.LastName AS UserName, at.Name AS ActionTypeName FROM Actions a LEFT JOIN Users u ON a.CreatorID = u.UserID LEFT JOIN ActionTypes at ON a.ActionTypeID = at.ActionTypeID WHERE a.TicketID = @TicketID ORDER BY a.DateCreated DESC";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                Fill(command);
            }
        }

        public void LoadLatestByTicket(int ticketID, bool onlyPublic)
        {
            using (SqlCommand command = new SqlCommand())
            {
                if (onlyPublic)
                    command.CommandText = "SELECT TOP 1 a.* FROM Actions a WHERE a.TicketID = @TicketID AND IsVisibleOnPortal = 1 ORDER BY a.DateCreated DESC";
                else
                    command.CommandText = "SELECT TOP 1 a.* FROM Actions a WHERE a.TicketID = @TicketID ORDER BY a.DateCreated DESC";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                Fill(command);
            }
        }

        public static DateTime? GetLastActionDateCreated(LoginUser loginUser, int ticketId)
        {
            DateTime? result = null;

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT MAX(DateCreated) AS DateCreated FROM Actions GROUP BY TicketID HAVING TicketID = @TicketId";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketId", ticketId);
                Actions actions = new Actions(loginUser);
                result = (DateTime?)actions.ExecuteScalar(command);
            }

            return result;
        }

        public static int TotalActionsForSla(LoginUser loginUser, int ticketId, int organizationId)
        {
            int result = 0;

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT COUNT(1) AS ActionCount
FROM Actions ax
LEFT JOIN Tickets tx ON ax.TicketID = tx.TicketID
LEFT JOIN Organizations ox ON ox.OrganizationID = tx.OrganizationID
JOIN Users ON ax.CreatorID = Users.UserID AND Users.OrganizationID = @OrganizationID
WHERE ax.SystemActionTypeID <> 1
    AND(
    ax.IsVisibleOnPortal = 1
    OR ox.SlaInitRespAnyAction = 1
    )
GROUP BY ax.TicketID
HAVING ax.TicketID = @TicketId";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketId", ticketId);
				command.Parameters.AddWithValue("@OrganizationID", organizationId);
                Actions actions = new Actions(loginUser);
                var resultVar = actions.ExecuteScalar(command);

                if (resultVar != null)
                {
                    result = int.Parse(resultVar.ToString());
                }
            }

            return result;
        }

        public void LoadByActionIDs(int[] actionIDs)
        {
            if (actionIDs.Length < 1) return;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < actionIDs.Length; i++)
            {
                if (i != 0) builder.Append(",");
                builder.Append(actionIDs[i]);
            }
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Actions WHERE ActionID IN (" + builder.ToString() + ") ORDER BY DateCreated DESC";
                command.CommandType = CommandType.Text;
                Fill(command);
            }
        }

        public static Action GetTicketDescription(LoginUser loginUser, int ticketID)
        {
            Actions actions = new Actions(loginUser);

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT a.*, u.FirstName + ' ' + u.LastName AS UserName, at.Name AS ActionTypeName FROM Actions a LEFT JOIN Users u ON a.CreatorID = u.UserID LEFT JOIN ActionTypes at ON a.ActionTypeID = at.ActionTypeID WHERE a.TicketID = @TicketID AND a.SystemActionTypeID = 1 ORDER BY a.DateCreated";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                actions.Fill(command);
            }

            if (actions.IsEmpty)
                return null;
            else
                return actions[0];
        }

        public static Action GetTicketFirstAction(LoginUser loginUser, int ticketID)
        {
            Actions actions = new Actions(loginUser);

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT a.*, u.FirstName + ' ' + u.LastName AS UserName, at.Name AS ActionTypeName FROM Actions a LEFT JOIN Users u ON a.CreatorID = u.UserID LEFT JOIN ActionTypes at ON a.ActionTypeID = at.ActionTypeID WHERE a.TicketID = @TicketID ORDER BY a.DateCreated";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                actions.Fill(command);
            }

            if (actions.IsEmpty)
                return null;
            else
                return actions[0];
        }
        public void LoadAll()
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT a.* FROM Actions a left join tickets t on a.ticketid = t.ticketid where organizationid = 1360";
                command.CommandType = CommandType.Text;
                Fill(command);
            }
        }

        public Action FindByImportID(string importID)
        {
            foreach (Action action in this)
            {
                if (action.ImportID == importID)
                {
                    return action;
                }
            }
            return null;
        }

        public void LoadByOrganizationID(int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT a.* FROM Actions a LEFT JOIN Tickets t on a.TicketID = t.TicketID WHERE OrganizationID = @OrganizationID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public void LoadByOrganizationID(int organizationID, string whereClauseSuffix)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT a.* FROM Actions a LEFT JOIN Tickets t on a.TicketID = t.TicketID WHERE OrganizationID = @OrganizationID " + whereClauseSuffix;
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public void LoadModifiedByCRMLinkItem(CRMLinkTableItem item)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText =
                @"
          SELECT
            a.*
          FROM
            Actions a
            JOIN Tickets t
              ON a.TicketID = t.TicketID
            LEFT JOIN TicketStatuses ts
              ON t.TicketStatusID = ts.TicketStatusID
          WHERE
            a.SystemActionTypeID <> 1
            AND a.DateModified > @DateModified
            AND
            (
              a.DateModifiedBySalesForceSync IS NULL
              OR a.DateModified > DATEADD(s, 2, a.DateModifiedBySalesForceSync)
            )
            AND t.OrganizationID = @OrgID
            AND
            (
              @DateModified > '1753-01-01'
              OR ts.IsClosed = 0
            )
          ORDER BY
            a.DateCreated DESC
        ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrgID", item.OrganizationID);
                command.Parameters.AddWithValue("@DateModified", item.LastLink == null ? new DateTime(1753, 1, 1) : item.LastLinkUtc.Value.AddHours(-1));

                using (SqlConnection connection = new SqlConnection(this.LoginUser.ConnectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);

                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandTimeout = 300;
                    SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                    this.Table.Load(reader);
                }
            }
        }

        public void ReplaceActionType(int oldID, int newID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE Actions SET ActionTypeID = @newID WHERE (ActionTypeID = @oldID)";
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@oldID", oldID);
                command.Parameters.AddWithValue("@newID", newID);
                ExecuteNonQuery(command, "Actions");
            }
        }

        public void UpdateSalesForceSync(string salesForceID, DateTime dateModifiedBySalesForceSync, int actionID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE Actions SET SalesForceID = @salesForceID, DateModifiedBySalesForceSync = @dateModifiedBySalesForceSync WHERE ActionID = @actionID";
                command.CommandType = CommandType.Text;
                command.Parameters.Clear();
                SqlParameter salesForceIDParameter = new SqlParameter("@salesForceID", DBNull.Value);
                if (salesForceID != null)
                {
                    salesForceIDParameter.Value = salesForceID;
                }
                command.Parameters.Add(salesForceIDParameter);
                command.Parameters.AddWithValue("@dateModifiedBySalesForceSync", dateModifiedBySalesForceSync);
                command.Parameters.AddWithValue("@actionID", actionID);
                ExecuteNonQuery(command, "Actions");
            }
        }

        public static Action GetActionByID(LoginUser loginUser, int actionID)
        {
            Actions actions = new Actions(loginUser);
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT a.*, u.FirstName + ' ' + u.LastName AS UserName, at.Name AS ActionTypeName FROM Actions a LEFT JOIN Users u ON a.CreatorID = u.UserID LEFT JOIN ActionTypes at ON a.ActionTypeID = at.ActionTypeID WHERE a.ActionID = @ActionID ORDER BY a.DateCreated DESC";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ActionID", actionID);
                actions.Fill(command);
            }
            if (actions.IsEmpty)
                return null;
            else
                return actions[0];
        }

        public void LoadBySalesForceID(string salesForceID, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                string sql = @"SELECT a.*
FROM Actions a
JOIN
	(SELECT TicketID FROM Tickets WHERE ticketId IN (SELECT TicketID
													FROM Actions
													WHERE SalesForceID = @SalesForceID
													)
										AND OrganizationID = @OrganizationID
	) t ON a.TicketID = t.TicketID
WHERE a.SalesForceID = @SalesForceID";
                command.CommandText = sql;

                //command.CommandText = "SELECT * FROM Actions a JOIN Tickets t ON a.TicketID = t.TicketID WHERE a.SalesForceID = @SalesForceID AND t.OrganizationID = @OrganizationID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@SalesForceID", salesForceID);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        //Changes to this method needs to be applied to ActionLinkToJira.LoadToPushToJira also.
        public void LoadToPushToJira(CRMLinkTableItem item, int ticketID)
        {
            string actionTypeFilter = "1 = 1";

            if (item.ActionTypeIDToPush != null)
            {
                actionTypeFilter = "a.ActionTypeID = @actionTypeID";
            }

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText =
                @"
          SELECT
            a.*
          FROM
            Actions a
            LEFT JOIN ActionLinkToJira j
              ON a.ActionID = j.ActionID
          WHERE
            a.SystemActionTypeID <> 1
            AND a.TicketID = @ticketID
            AND " + actionTypeFilter + @"
            AND
            (
              j.DateModifiedByJiraSync IS NULL
              OR a.DateModified > DATEADD(s, 2, j.DateModifiedByJiraSync)
            )
          ORDER BY
            a.DateCreated ASC
        ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ticketID", ticketID);
                command.Parameters.AddWithValue("@DateModified", item.LastLink == null ? new DateTime(1753, 1, 1) : item.LastLinkUtc.Value.AddHours(-1));
                command.Parameters.AddWithValue("@actionTypeID", item.ActionTypeIDToPush == null ? -1 : item.ActionTypeIDToPush);

                Fill(command, "Actions");
            }
        }

        //Changes to this method needs to be applied to ActionLinkToJira.LoadToPushToJira also.
        public void LoadToPushToTFS(CRMLinkTableItem item, int ticketID)
        {
            string actionTypeFilter = "1 = 1";

            if (item.ActionTypeIDToPush != null)
            {
                actionTypeFilter = "a.ActionTypeID = @actionTypeID";
            }

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText =
                @"
          SELECT
            a.*
          FROM
            Actions a
            LEFT JOIN ActionLinkToTFS tfs
              ON a.ActionID = tfs.ActionID
          WHERE
            a.SystemActionTypeID <> 1
            AND a.TicketID = @ticketID
            AND " + actionTypeFilter + @"
            AND
            (
              tfs.DateModifiedByTFSSync IS NULL
              OR a.DateModified > DATEADD(s, 2, tfs.DateModifiedByTFSSync)
            )
          ORDER BY
            a.DateCreated ASC
        ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ticketID", ticketID);
                command.Parameters.AddWithValue("@DateModified", item.LastLink == null ? new DateTime(1753, 1, 1) : item.LastLinkUtc.Value.AddHours(-1));
                command.Parameters.AddWithValue("@actionTypeID", item.ActionTypeIDToPush == null ? -1 : item.ActionTypeIDToPush);

                Fill(command, "Actions");
            }
        }

        public static int GetActionPosition(LoginUser loginUser, int actionID)
        {
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = @"
          SELECT
	          COUNT(*)
          FROM
	          Actions
          WHERE
	          ActionID <= @ActionID
	          AND TicketID = (SELECT TicketID FROM Actions WHERE ActionID = @ActionID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ActionID", actionID);
                int result = (int)command.ExecuteScalar();
                connection.Close();
                return result;
            }
        }

        public static int GetTicketActionCount(LoginUser loginUser, int ticketID)
        {
            int cnt = 0;
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM ACTIONS WHERE TICKETID = @TicketID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                object o = SqlExecutor.ExecuteScalar(loginUser, command);
                if (o == DBNull.Value) return -1;
                cnt = (int)o;
            }
            return cnt;
        }

        public void LoadByImportID(string importID, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Actions a JOIN Tickets t ON a.TicketID = t.TicketID WHERE a.ImportID = @ImportID AND t.OrganizationID = @OrganizationID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ImportID", importID);
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public static string PullUserList(LoginUser loginUser, int ticketID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT Actions.CreatorID, Users.OrganizationID FROM dbo.Actions ";
                        command.CommandText += "INNER JOIN dbo.Users ON Actions.CreatorID = Users.UserID ";
                        command.CommandText += "WHERE Actions.TicketID = @TicketID ";
                        command.CommandText += "FOR JSON PATH, ROOT('userlist')";
                        command.Parameters.AddWithValue("@TicketID", ticketID);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows && reader.Read())
                        {
                            return reader.GetValue(0).ToString();
                        }
                        else
                        {
                            return "nothing";
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                return "negative";
            }
            catch (Exception e)
            {
                return "negative";
            }
        }

        public static string UpdateReaction(LoginUser loginUser, int receiverID, int ticketID, int actionID, int value)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection   = connection;
                        command.CommandText  = "BEGIN TRAN ";
                        command.CommandText += "IF EXISTS (SELECT * FROM dbo.Reactions WHERE ReferenceID = @ReferenceID AND UserID = @UserID) ";
                        command.CommandText += "BEGIN UPDATE dbo.Reactions SET ReactionValue = @ReactionValue, DateTimeChanged = @DateTime WHERE UserID = @UserID AND ReceiverID = @ReceiverID AND ReferenceID = @ReferenceID END ";
                        command.CommandText += "ELSE BEGIN INSERT dbo.Reactions (UserID,ReceiverID,ReferenceID,ReactionValue,DateTimeCreated) VALUES (@UserID,@ReceiverID,@ReferenceID,@ReactionValue,@DateTime) END ";
                        command.CommandText += "COMMIT TRAN";
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@UserID", loginUser.UserID);
                        command.Parameters.AddWithValue("@ReceiverID", receiverID);
                        command.Parameters.AddWithValue("@ReferenceID", actionID);
                        command.Parameters.AddWithValue("@ReactionValue", value);
                        command.Parameters.AddWithValue("@DateTime", DateTime.UtcNow);
                        connection.Open();
                        Int32 result = command.ExecuteNonQuery();
                        return (result > 0) ? "positive" : "negative";
                    }
                }
            }
            catch (SqlException e)
            {
                return "negative";
            }
            catch (Exception e)
            {
                return "negative";
            }
        }

        public static string ListReactions(LoginUser loginUser, int ticketID, int actionID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT Reactions.*, Users.Firstname, Users.LastName FROM dbo.Reactions ";
                        command.CommandText += "INNER JOIN dbo.Users ON Reactions.UserID = Users.UserID ";
                        command.CommandText += "WHERE Reactions.ReactionValue > 0 AND Reactions.ReferenceID = @ReferenceID ";
                        command.CommandText += "FOR JSON PATH, ROOT('reactions')";
                        command.Parameters.AddWithValue("@ReferenceID", actionID);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows && reader.Read())
                        {
                            return reader.GetValue(0).ToString();
                        }
                        else
                        {
                            return "nothing";
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                return "negative";
            }
            catch (Exception e)
            {
                return "negative";
            }
        }

        public static string CountReactions(LoginUser loginUser, int ticketID, int actionID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT COUNT(*) AS tally FROM dbo.Reactions WHERE ReactionValue > 0 AND ReferenceID = @ReferenceID ";
                        command.CommandText += "FOR JSON PATH, ROOT('reactions')";
                        command.Parameters.AddWithValue("@ReferenceID", actionID);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows && reader.Read())
                        {
                            return reader.GetValue(0).ToString();
                        }
                        else
                        {
                            return "nothing";
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                return "negative";
            }
            catch (Exception e)
            {
                return "negative";
            }
        }

        public static string CheckReaction(LoginUser loginUser, int ticketID, int actionID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT COUNT(*) reckoning FROM dbo.Reactions WHERE UserID = @UserID AND ReferenceID = @ReferenceID AND ReactionValue > 0 ";
                        command.CommandText += "FOR JSON PATH, ROOT('validation')";
                        command.Parameters.AddWithValue("@UserID", loginUser.UserID);
                        command.Parameters.AddWithValue("@ReferenceID", actionID);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows && reader.Read())
                        {
                            return reader.GetValue(0).ToString();
                        }
                        else
                        {
                            return "nothing";
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                return "negative";
            }
            catch (Exception e)
            {
                return "negative";
            }
        }

        public static string getPosition(LoginUser loginUser, int ticketID, int actionID) {
            try {
                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString)) {
                    using (SqlCommand command = new SqlCommand()) {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT TOP 1 COUNT(ActionID) AS position FROM dbo.Actions WHERE TicketID = @TicketID AND ActionID <= @ActionID ";
                        command.CommandText += "FOR JSON PATH, ROOT('position')";
                        command.Parameters.AddWithValue("@TicketID", ticketID);
                        command.Parameters.AddWithValue("@ActionID", actionID);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows && reader.Read()) {
                            return reader.GetValue(0).ToString();
                        } else {
                            return "nothing";
                        }
                    }
                }
            } catch (SqlException e) {
                return "negative";
            } catch (Exception e) {
                return "negative";
            }
        }

    }
}
