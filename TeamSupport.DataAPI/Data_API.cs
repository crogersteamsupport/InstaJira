
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.IO;
using System.Web.Security;
using System.Diagnostics;
using TeamSupport.Data;
using System.Web;
using TeamSupport.IDTree;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.DataAPI
{
    /// <summary>
    /// CRUD Interface (Create, Read, Update, Delete) on verified connection context and verified model objects
    /// 
    /// Log all changes to DB here!!  Thanks :)
    /// </summary>
    public static class Data_API
    {
        /// <summary>
        /// CREATE - create proxy child for model parent
        /// </summary>
        public static IDNode Create<TProxy>(IDNode idNode, TProxy tProxy) where TProxy : class
        {
            IDNode result = null;
            if (TryCreateAttachment(idNode as IAttachmentDestination, tProxy as AttachmentProxy, out result))
                return result;

            string modification = $", CreatorID={idNode.Connection.UserID}, DateCreated={UpdateArguments.ToSql(DateTime.UtcNow)}";
            string now = UpdateArguments.ToSql(DateTime.UtcNow);
            int creatorID = idNode.Connection.UserID;
            string command = String.Empty;

            switch (tProxy) // alphabetized list
            {
                case ActionProxy proxy:
                    {
                        TicketModel model = (TicketModel)idNode;
                        proxy.TicketID = model.TicketID;
                        proxy.CreatorID = proxy.ModifierID = model.Connection.UserID;
                        proxy.DateCreated = proxy.DateModified = DateTime.UtcNow;

                        // 1. sql command
                        //command = proxy.InsertCommandText();
                        //SqlCommand sqlCommand = new SqlCommand(command, model.Connection._connection);
                        //int id = Decimal.ToInt32((decimal)sqlCommand.ExecuteScalar());
                        //result = new ActionModel(model, id);    // how to bypass Verify?

                        // 2. linq
                        DataContext db = model.Connection._db;
                        Table<ActionProxy> table = db.GetTable<ActionProxy>();
                        table.InsertOnSubmit(proxy);
                        db.SubmitChanges();

                        // 3. TeamSupport.Data
                        //proxy.ActionID = NewAction(model, proxy);

                        result = new ActionModel(model, proxy.ActionID);    // how to bypass Verify?
                    }
                    break;
                case ContactProxy proxy:
                    {
                        TicketModel model = (TicketModel)idNode;
                        command = $"INSERT INTO UserTickets (TicketID, UserID, DateCreated, CreatorID)" +
                            $"SELECT {model.TicketID}, {proxy.UserID}, '{now}', {creatorID} ";
                    }
                    break;
                case CustomerProxy proxy:
                    {
                        TicketModel model = (TicketModel)idNode;
                        command = $"INSERT INTO OrganizationTickets (TicketID, OrganizationID, DateCreated, CreatorID, DateModified, ModifierID)" +
                            $"SELECT {model.TicketID}, {proxy.OrganizationID}, '{now}', {creatorID}, '{now}', {creatorID}";
                    }
                    break;
                case SubscriptionProxy proxy:
                    {
                        TicketModel model = (TicketModel)idNode;
                        command = $"INSERT INTO Subscriptions (RefType, RefID, UserID, DateCreated, DateModified, CreatorID, ModifierID)" +
                                $"SELECT 17, {model.TicketID}, {proxy.UserID}, '{now}','{now}', {creatorID}, {creatorID} ";
                    }
                    break;
                case TicketProxy proxy:
                    {
                        UserModel model = (UserModel)idNode;
                        proxy.OrganizationID = model.Organization.OrganizationID;
                        proxy.CreatorID = proxy.ModifierID = model.UserID;
                        proxy.UserID = model.Connection.UserID;
                        proxy.DateCreated = proxy.DateModified = DateTime.UtcNow;
                        proxy.TicketNumber = 1 + model.ExecuteQuery<int>($"SELECT MAX(TicketNumber) FROM Tickets WHERE OrganizationID={model.Organization.OrganizationID}").Max();

                        // 1. sql command
                        //command = proxy.InsertCommandText(proxy.TicketNumber);
                        //SqlCommand sqlCommand = new SqlCommand(command, model.Connection._connection);
                        //int id = Decimal.ToInt32((decimal)sqlCommand.ExecuteScalar());
                        //result = new TicketModel(model.Organization, id);    // how to bypass Verify?

                        // 2. linq
                        DataContext db = model.Organization.Connection._db;
                        Table<TicketProxy> table = db.GetTable<TicketProxy>();
                        table.InsertOnSubmit(proxy);
                        db.SubmitChanges();

                        // 3. TeamSupport.Data
                        //proxy.TicketID = NewTicket(model, proxy);

                        result = new TicketModel(model.Organization, proxy.TicketID);    // how to bypass Verify? - move to UserModel?
                    }
                    break;
                case null:
                default:
                    if (Debugger.IsAttached) Debugger.Break();
                    break;
            }

            //if (!String.IsNullOrEmpty(command))
            //    idNode.ExecuteCommand(command);
            // TODO - log
            return result;
        }

        private static bool TryCreateAttachment(IAttachmentDestination attachedTo, AttachmentProxy tProxy, out IDNode result)
        {
            if ((attachedTo == null) || (tProxy == null))
            {
                result = null;
                return false;
            }

            IDNode idNode = attachedTo as IDNode;
            tProxy.DateCreated = tProxy.DateModified = DateTime.UtcNow;
            tProxy.CreatorID = tProxy.ModifierID = idNode.Connection.UserID;
            tProxy.OrganizationID = idNode.Connection.OrganizationID;

            switch (tProxy) // alphabetized list
            {
                case ActionAttachmentProxy proxy:   // create action attachment
                    proxy.RefID = (idNode as ActionModel).ActionID;
                    break;
                case AssetAttachmentProxy proxy:   // create asset attachment
                    proxy.RefID = (idNode as AssetModel).AssetID;
                    break;
                case ChatAttachmentProxy proxy:   // create asset attachment
                    proxy.RefID = (idNode as ChatModel).ChatID;
                    break;
                case CompanyActivityAttachmentProxy proxy:
                    proxy.RefID = (idNode as NoteModel).NoteID;
                    break;
                case ContactActivityAttachmentProxy proxy:
                    proxy.RefID = (idNode as NoteModel).NoteID;
                    break;
                case OrganizationAttachmentProxy proxy:
                    proxy.RefID = (idNode as OrganizationModel).OrganizationID;
                    break;
                case ProductVersionAttachmentProxy proxy:
                    proxy.RefID = (idNode as ProductVersionModel).ProductVersionID;
                    break;
                case TaskAttachmentProxy proxy:
                    proxy.RefID = (idNode as TaskModel).TaskID;
                    break;
                case UserAttachmentProxy proxy:   // create action attachment
                    proxy.RefID = (idNode as UserModel).UserID;
                    break;
                case UserPhotoAttachmentProxy proxy:   // create action attachment
                    proxy.RefID = (idNode as UserModel).UserID;
                    break;
                case WatercoolerMsgAttachmentProxy proxy:   // create action attachment
                    proxy.RefID = (idNode as WatercoolerMsgModel).MessageID;
                    break;
                case null:
                default:
                    if (Debugger.IsAttached) Debugger.Break();
                    break;
            }

            Table<AttachmentProxy> table = idNode.Connection._db.GetTable<AttachmentProxy>();
            table.InsertOnSubmit(tProxy);
            idNode.Connection._db.SubmitChanges();

            result = new AttachmentModel(attachedTo, tProxy.AttachmentID);    // disable Verify?
            return true;
        }


        public static TProxy ReadRefTypeProxyByRefID<TProxy>(ConnectionContext connection, int id) where TProxy : class
        {
            TProxy tProxy = default(TProxy);
            switch (typeof(TProxy).Name) // alphabetized list
            {
                case "UserPhotoAttachmentProxy": // action
                    {
                        Table<AttachmentProxy> table = connection._db.GetTable<AttachmentProxy>();
                        tProxy = table.Where(a => (a.RefID == id) && (a.RefType == AttachmentProxy.References.UserPhoto)).First() as TProxy;
                    }
                    break;
                default:
                    if (Debugger.IsAttached) Debugger.Break();
                    break;
            }
            return tProxy;
        }

        /// <summary>
        /// Read a row from a RefType table and get back the type safe proxy
        /// </summary>
        public static TProxy ReadRefTypeProxy<TProxy>(ConnectionContext connection, int id) where TProxy : class
        {
            TProxy tProxy = default(TProxy);
            switch (typeof(TProxy).Name) // alphabetized list
            {
                case "AttachmentProxy": // action
                    {
                        // exact match for AttachmentID - returns type-safe proxy (ActionAttachmentProxy, AssetAttachmentProxy...)
                        Table<AttachmentProxy> table = connection._db.GetTable<AttachmentProxy>();
                        tProxy = table.Where(a => a.AttachmentID == id).First() as TProxy;
                    }
                    break;
                default:
                    if (Debugger.IsAttached) Debugger.Break();
                    break;
            }
            return tProxy;
        }

        /// <summary> 
        /// READ - read proxy given a model 
        /// </summary>
        public static TProxy Read<TProxy>(IDNode node) where TProxy : class
        {
            TProxy tProxy = default(TProxy);
            switch (typeof(TProxy).Name) // alphabetized list
            {
                case "ActionProxy": // action
                    {
                        ActionModel model = (ActionModel)node;
                        {
                            DataContext db = model.Connection._db;
                            Table<ActionProxy> table = db.GetTable<ActionProxy>();
                            var query = from row in table where row.ActionID == model.ActionID select row;
                            tProxy = query.First() as TProxy;
                        }
                        //{
                        //    string query = $"SELECT ActionID, ActionTypeID, SystemActionTypeID, Name, TimeSpent, DateStarted, IsVisibleOnPortal, IsKnowledgeBase, ImportID, DateCreated, DateModified, CreatorID, ModifierID, TicketID, Description, SalesForceID, DateModifiedBySalesForceSync, Pinned, ImportFileID FROM Actions WHERE ActionID={model.ActionID}";
                        //    DataRow row = model.Connection.GetRowCollection(query)[0];
                        //    tProxy = new ActionProxy(row) as TProxy;
                        //}
                    }
                    break;
                case "ActionProxy[]":   // ticket actions
                    {
                        TicketModel ticket = (TicketModel)node;
                        string query = $"SELECT * FROM Actions WHERE ActionID={ticket.TicketID}";
                        tProxy = ticket.ExecuteQuery<ActionProxy>(query).ToArray() as TProxy;
                    }
                    break;
                case "AttachmentProxy": // read all attachment types
                    tProxy = ReadRefTypeProxy<TProxy>(node.Connection, (node as AttachmentModel).AttachmentID);
                    break;
                case "AttachmentProxy[]":
                    switch(node)
                    {
                        case ActionModel model: // action attachments
                            Table<AttachmentProxy> table = node.Connection._db.GetTable<AttachmentProxy>();
                            tProxy = table.Where(a => a.RefID == model.ActionID && a.RefType == AttachmentProxy.References.Actions).ToArray() as TProxy;
                            break;
                        case null:
                        default:
                            if (Debugger.IsAttached) Debugger.Break();
                            break;
                    }
                    break;
                case "ReminderProxy":
                    {
                        TicketReminderModel reminder = (TicketReminderModel)node;
                        string query = $"SELECT * FROM Reminders WHERE ReminderID = {reminder.ReminderID} AND RefType=17";
                        tProxy = reminder.ExecuteQuery<ReminderProxy>(query).ToArray() as TProxy;
                    }
                    break;
                case "SubscriptionModel[]":
                    {
                        TicketModel ticket = (TicketModel)node;
                        string query = $"SELECT * FROM Subscriptions WHERE Reftype = 17 and RefID = {ticket.TicketID} and MarkDeleted = 0";
                        tProxy = ticket.ExecuteQuery<SubscriptionModel>(query).ToArray() as TProxy;
                    }
                    break;
                case "TaskAssociationProxy":
                    {
                        TaskAssociationModel taskAssociation = (TaskAssociationModel)node;
                        string query = String.Empty;// = $"SELECT TaskID, RefID, RefType,CreatorID, DateCreated FROM TaskAssociations WHERE TaskID = {model.TaskID} AND RefID = {model.Ticket.TicketID} AND RefType = 17";
                        tProxy = taskAssociation.ExecuteQuery<TaskAssociationProxy>(query).First() as TProxy;
                    }
                    break;
                case "TagLinkProxy[]":
                    {
                        TicketModel ticket = (TicketModel)node;
                        //string query = $"SELECT * FROM TagLinks WHERE RefType = 17 AND RefID = {ticket.TicketID}";
                        //tProxy = ticket.ExecuteQuery<TProxy>(query).ToArray() as TProxy;

                        Table<TagLinkProxy> table = ticket.Connection._db.GetTable<TagLinkProxy>();
                        tProxy = table.Where(t => (t.RefType == ReferenceType.Tickets) && (t.RefID == ticket.TicketID)).ToArray() as TProxy;
                    }
                    break;
                case "TicketProxy": // ticket
                    {
                        TicketModel model = (TicketModel)node;
                        DataContext db = model.Connection._db;
                        Table<TicketProxy> table = db.GetTable<TicketProxy>();
                        var query = from row in table where row.TicketID == model.TicketID select row;
                        tProxy = query.First() as TProxy;
                    }
                    break;
                case "TicketRelationshipProxy[]":
                    {
                        TicketModel model = (TicketModel)node;
                        //string query = $"SELECT * FROM TicketRelationships WHERE Ticket1ID={model.TicketID} OR Ticket2ID={model.TicketID}";
                        string query = $"SELECT * FROM TicketRelationships WHERE Ticket1ID={model.TicketID} OR Ticket2ID={model.TicketID}";
                        tProxy = model.ExecuteQuery<TicketRelationshipProxy>(query).ToArray() as TProxy;
                    }
                    break;
                case "UserProxy":
                    {
                        UserModel user = (UserModel)node;
                        string query = $"SELECT * FROM Users WHERE UserID={user.UserID}";
                        tProxy = node.ExecuteQuery<UserProxy>(query).First() as TProxy;
                    }
                    break;
                default:
                    if (Debugger.IsAttached) Debugger.Break();
                    break;
            }
            return tProxy;
        }

        public static void Update(IDNode node, UpdateArguments args)
        {
            // TODO - set DateModified, ModifierID...
            string modification = $", ModifierID={node.Connection.UserID}, DateModified={UpdateArguments.ToSql(DateTime.UtcNow)}";

            string command = String.Empty;
            switch (node) // alphabetized list
            {
                case ActionModel model:
                    command = $"UPDATE Actions WITH(ROWLOCK) SET {args.ToString()} WHERE ActionID={model.ActionID}";
                    break;
                case AttachmentModel model:
                    command = $"UPDATE Attachments WITH(ROWLOCK) SET {args.ToString()} WHERE AttachmentID={model.AttachmentID}";
                    break;
                case TagLinkModel model:
                    command = $"UPDATE TagLinks WITH(ROWLOCK) SET {args.ToString()} WHERE TagLinkID={model.TagLinkID} AND RefType=17";
                    break;
                case TaskAssociationModel model:
                    command = $"UPDATE TaskAssociations SET {args.ToString()} WHERE TaskID={model.Task.TaskID} AND RefType=17";
                    break;
                case TicketModel model:
                    command = $"UPDATE Tickets SET {args.ToString()} WHERE TicketID= {model.TicketID}";
                    break;
                case TicketReminderModel model:
                    command = $" UPDATE Reminders WITH(ROWLOCK) SET {args.ToString()} WHERE ReminderID={model.ReminderID} AND RefType=17";
                    break;
                case null:
                default:
                    if (Debugger.IsAttached) Debugger.Break();
                    break;
            }
            node.ExecuteCommand(command);
            // TODO - Log
        }

        /// <summary> 
        /// DELETE - delete a model </summary>
        public static void Delete(IDNode node)
        {
            string command = String.Empty;
            switch (node) // alphabetized list
            {
                case AttachmentModel model:
                    command = $"DELETE FROM Attachments WHERE AttachmentID = {model.AttachmentID}";
                    break;
                case OrganizationTicketModel model:
                    command = $"DELETE FROM OrganizationTickets WHERE TicketID={model.Ticket.TicketID} AND OrganizationId = {model.Organization.OrganizationID}";
                    break;
                case TagLinkModel model:
                    command = $"DELETE FROM TagLinks WHERE TagLinkID={model.TagLinkID}";
                    break;
                case TicketModel model:
                    command = $"DELETE FROM Tickets WHERE TicketID={model.TicketID}";
                    break;
                case TicketRelationshipModel model:
                    command = $"DELETE FROM TicketRelationships WHERE TicketID={model.TicketRelationshipID}";
                    break;
                case SubscriptionModel model:
                    command = $"DELETE FROM Subscriptions WHERE RefType=17 AND RefID={model.Ticket.TicketID} AND UserID={model.User.UserID}";
                    break;
                case null:
                default:
                    if (Debugger.IsAttached) Debugger.Break();
                    break;
            }
            node.ExecuteCommand(command);
        }


        #region Log
        /// <summary> Log Message </summary>
        public static void LogMessage(ActionLogType logType, ReferenceType refType, int? refID, string message)
        {
            AuthenticationModel authentication = new AuthenticationModel();
            LoginUser user = new LoginUser(authentication.UserID, authentication.OrganizationID);
            ActionLogs.AddActionLog(user, logType, refType, refID.HasValue ? refID.Value : 0, message);  // 0 if no ID?
        }

        public static void LogMessage(ActionLogType logType, ReferenceType refType, int? refID, string message, Exception ex)
        {
            // log to ExceptionLogs or New Relic, or windows event log?

            string fullMessage = message + ex.ToString() + " ----- STACK: " + ex.StackTrace.ToString();
            if (Debugger.IsAttached)
            {
                Debug.WriteLine(fullMessage);   // see the error in the debug output window
                Debugger.Break();   // something is wrong - fix the code!
            }

            LogMessage(logType, refType, refID, fullMessage);
        }
        #endregion

        #region ExceptionLog AsIs
        public static int LogException(AuthenticationModel authentication, Exception ex, string exceptionName)
        {
            string fullMessage = exceptionName + ex.ToString() + " ----- STACK: " + ex.StackTrace.ToString();
            if (Debugger.IsAttached)
            {
                Debug.WriteLine(fullMessage);   // debug output window (very fast)
                Debugger.Break();   // something is wrong - fix the code!
            }

            LoginUser user = new LoginUser(authentication.UserID, authentication.OrganizationID);
            ExceptionLog log = (new ExceptionLogs(user)).AddNewExceptionLog();
            log.ExceptionName = exceptionName;
            log.Message = ex.Message.Replace(Environment.NewLine, "<br />");
            log.StackTrace = ex.StackTrace.Replace(Environment.NewLine, "<br />");
            log.Collection.Save();

            return log.ExceptionLogID;
        }
        /*
        public static int NewTicket(UserModel user, TicketProxy info)
        {
            LoginUser loginUser = new LoginUser(user.Connection.Authentication.ConnectionString, user.UserID, user.Connection.OrganizationID, null);
            Ticket ticket = new Tickets(loginUser).AddNewTicket();
            ticket.OrganizationID = info.OrganizationID;
            //ticket.TicketSource = info.ChatID != null ? "Chat" : "Agent";
            ticket.Name = info.Name;
            ticket.TicketTypeID = info.TicketTypeID;
            ticket.TicketStatusID = info.TicketStatusID;
            ticket.TicketSeverityID = info.TicketSeverityID;
            ticket.UserID = info.UserID < 0 ? null : (int?)info.UserID;
            ticket.GroupID = info.GroupID < 0 ? null : (int?)info.GroupID;
            ticket.ProductID = info.ProductID < 0 ? null : (int?)info.ProductID;
            //ticket.ReportedVersionID = info.ReportedID < 0 ? null : (int?)info.ReportedID;
            //ticket.SolvedVersionID = info.ResolvedID < 0 ? null : (int?)info.ResolvedID;
            ticket.ProductID = info.ProductID < 0 ? null : (int?)info.ProductID;
            //ticket.IsKnowledgeBase = info.IsKnowledgebase;
            ticket.KnowledgeBaseCategoryID = info.KnowledgeBaseCategoryID < 0 ? null : (int?)info.KnowledgeBaseCategoryID;

            //User user = Users.GetUser(TSAuthentication.GetLoginUser(), TSAuthentication.UserID);
            //if (ticket.IsKnowledgeBase && !user.ChangeKBVisibility && !user.IsSystemAdmin)
            //{
            //    ticket.IsVisibleOnPortal = false;
            //}
            //else
            //{
            //    ticket.IsVisibleOnPortal = info.IsVisibleOnPortal;
            //}

            //ticket.ParentID = info.ParentTicketID;
            //ticket.DueDate = info.DueDate;
            ticket.Collection.Save();
            return ticket.TicketID;
        }

        public static int NewAction(TicketModel ticket, ActionProxy info)
        {
            ConnectionContext connection = ticket.Connection;
            LoginUser loginUser = new LoginUser(connection.Authentication.ConnectionString, connection.UserID, connection.OrganizationID, null);
            TeamSupport.Data.Action action = (new Actions(loginUser)).AddNewAction();
            action.ActionTypeID = null;
            action.Name = "Description";
            action.SystemActionTypeID = SystemActionType.Description;
            //action.ActionSource = ticket.TicketSource;
            action.Description = info.Description;

            //if (!string.IsNullOrWhiteSpace(user.Signature) && info.IsVisibleOnPortal)
            //{
            //    action.Description = action.Description + "<br/><br/>" + user.Signature;
            //}

            //action.IsVisibleOnPortal = ticket.IsVisibleOnPortal;
            //action.IsKnowledgeBase = ticket.IsKnowledgeBase;
            action.TicketID = ticket.TicketID;
            action.TimeSpent = info.TimeSpent;
            action.DateStarted = info.DateStarted;
            action.Collection.Save();
            return action.ActionID;
        }
*/
        #endregion

    }
}
