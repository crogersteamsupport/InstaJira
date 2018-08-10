﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.IO;
using TeamSupport.Proxy;
using System.Web.Security;
using System.Diagnostics;
using TeamSupport.Data;
using System.Web;
using TeamSupport.Model;

namespace TeamSupport.DataAPI
{
    /// <summary>
    /// CRUD Interface (Create, Read, Update, Delete) on verified connection context and verified model objects
    /// 
    /// Log all changes to DB here!!  Thanks :)
    /// </summary>
    public static class DataAPI
    {
        static string ToSql(DateTime dateTime) { return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"); }
        static char ToSql(bool value) { return value ? '1' : '0'; }

        #region User
        //class FullName
        //{
        //    public string FirstName;
        //    public string LastName;
        //}
        //public static string UserFullName(DataContext db, int organizationID, int userID)
        //{
        //    string query = $"SELECT FirstName + ' ' + LastName FROM Users  WITH (NOLOCK) WHERE UserID={userID} AND OrganizationID={organizationID}";
        //    FullName fullName = db.ExecuteQuery<FullName>(query).First();  // throws if it fails
        //    return $"{fullName.FirstName} {fullName.LastName}";
        //}
        #endregion


        #region Tickets
        /// <summary> Create Ticket </summary>
        public static void Create(ConnectionContext connection, TicketProxy ticketProxy)
        {
            // TODO - create ticket
            LogMessage(connection.Authentication, ActionLogType.Insert, ReferenceType.Tickets, ticketProxy.TicketID, "Created Ticket");
        }

        /// <summary> Read Ticket </summary>
        public static TicketProxy Read(ConnectionContext connection, TicketModel ticketModel)
        {
            Table<TicketProxy> table = connection._db.GetTable<TicketProxy>();
            return table.Where(t => t.TicketID == ticketModel.TicketID).First();
        }

        /// <summary> Update Ticket </summary>
        public static void Update(ConnectionContext connection, TicketModel ticketModel, TicketProxy ticketProxy)
        {
            // TODO - update ticket
            LogMessage(connection.Authentication, ActionLogType.Update, ReferenceType.Tickets, ticketModel.TicketID, "Updated Ticket");
        }

        /// <summary> Delete Ticket</summary>
        public static void Delete(ConnectionContext connection, TicketModel ticketModel)
        {
            // TODO - delete ticket
            LogMessage(connection.Authentication, ActionLogType.Delete, ReferenceType.Tickets, ticketModel.TicketID, "Deleted Ticket");
        }
        #endregion


        #region Actions
        public static int ActionGetTicketID(DataContext db, int actionID) { return db.ExecuteQuery<int>($"SELECT TicketID FROM Actions WITH (NOLOCK) WHERE ActionID = {actionID}").Min(); }

        /// <summary> Create Action </summary>
        public static void Create(ConnectionContext connection, TicketModel ticketModel, ref ActionProxy actionProxy)
        {
            AuthenticationModel authentication = connection.Authentication;
            Data.Action.Create(connection._db, authentication.OrganizationID, authentication.UserID, ticketModel.TicketID, ref actionProxy);
            LogMessage(connection.Authentication, ActionLogType.Insert, ReferenceType.Actions, actionProxy.ActionID, "Created Action");
        }

        /// <summary> Read Action </summary>
        public static ActionProxy Read(ConnectionContext connection, ActionModel actionModel)
        {
            Table<ActionProxy> table = connection._db.GetTable<ActionProxy>();
            return table.Where(t => t.ActionID == actionModel.ActionID).First();
        }

        /// <summary> Read ticket Actions </summary>
        public static void Read(ConnectionContext connection, TicketModel ticketModel, out ActionProxy[] actionProxies)
        {
            Table<ActionProxy> table = connection._db.GetTable<ActionProxy>();
            actionProxies = table.Where(t => t.TicketID == ticketModel.TicketID).ToArray();
        }

        /// <summary> Update Action </summary>
        public static void Update(ConnectionContext connection, ActionModel actionModel, ActionProxy actionProxy)
        {
            // TODO - update action
            LogMessage(connection.Authentication, ActionLogType.Update, ReferenceType.Actions, actionModel.ActionID, "Updated Action");
        }

        /// <summary> Delete Action </summary>
        public static void Delete(ConnectionContext connection, ActionModel actionModel)
        {
            // TODO - delete action
            LogMessage(connection.Authentication, ActionLogType.Delete, ReferenceType.Actions, actionModel.ActionID, "Deleted Action");
        }
        #endregion


        #region ActionAttachments

        public static int ActionAttachmentGetActionID(DataContext db, int attachmentID)
        {
            return db.ExecuteQuery<int>($"SELECT ActionID FROM ActionAttachments WITH(NOLOCK) WHERE ActionAttachmentID = {attachmentID}").Min();
        }

        /// <summary> Create Action Attachment </summary>
        public static void Create(ConnectionContext connection, ActionModel action, AttachmentProxy proxy)
        {
            // hard code all the numbers, parameterize all the strings so they are SQL-Injection checked
            string query = "INSERT INTO ActionAttachments(OrganizationID, FileName, FileType, FileSize, Path, DateCreated, DateModified, CreatorID, ModifierID, ActionID, SentToJira, SentToTFS, SentToSnow, FilePathID) " +
                $"VALUES({connection.Organization.OrganizationID}, {{0}}, {{1}}, {proxy.FileSize}, {{2}}, '{ToSql(proxy.DateCreated)}', '{ToSql(proxy.DateModified)}', {proxy.CreatorID}, {proxy.ModifierID}, {action.ActionID}, {ToSql(proxy.SentToJira)}, {ToSql(proxy.SentToTFS)}, {ToSql(proxy.SentToSnow)}, {proxy.FilePathID})" +
                "SELECT SCOPE_IDENTITY()";
            decimal value = connection._db.ExecuteQuery<decimal>(query, proxy.FileName, proxy.FileType, proxy.Path).Min();
            proxy.AttachmentID = Decimal.ToInt32(value);
        }

        // load action attachments into attachment proxy
        const string SelectActionAttachmentProxy = "SELECT a.*, a.ActionAttachmentID as AttachmentID, a.ActionAttachmentGUID as AttachmentGUID, (u.FirstName + ' ' + u.LastName) AS CreatorName, a.ActionID as RefID " +
            "FROM ActionAttachments a LEFT JOIN Users u ON u.UserID = a.CreatorID ";

        /// <summary> Read Action Attachment </summary>
        public static AttachmentProxy Read(ConnectionContext connection, ActionAttachment actionAttachment)
        {
            string query = SelectActionAttachmentProxy + $"WHERE ActionAttachmentID = {actionAttachment.ActionAttachmentID}";
            return actionAttachment._db.ExecuteQuery<AttachmentProxy>(query).First();
        }

        /// <summary> Read Action Attachments </summary>
        public static void Read(ConnectionContext connection, ActionModel actionModel, out AttachmentProxy[] attachments)
        {
            string query = SelectActionAttachmentProxy + $"WHERE ActionID = {actionModel.ActionID}";
            attachments = actionModel._db.ExecuteQuery<AttachmentProxy>(query).ToArray();
        }

        /// <summary> Read all Action Attachments for this ticket </summary>
        public static void Read(ConnectionContext connection, TicketModel ticketModel, out AttachmentProxy[] attachments)
        {
            string query = SelectActionAttachmentProxy + $"WHERE ActionID IN (SELECT ActionID FROM Actions WHERE TicketID = {ticketModel.TicketID})";
            attachments = ticketModel._db.ExecuteQuery<AttachmentProxy>(query).ToArray();
        }

        /// <summary> Delete Action Attachment </summary>
        public static void Delete(ConnectionContext connection, ActionAttachment actionAttachment)
        {
            string query = $"DELETE FROM ActionAttachments WHERE ActionAttachmentID = {actionAttachment.ActionAttachmentID}";
            actionAttachment._db.ExecuteCommand(query);
            LogMessage(connection.Authentication, ActionLogType.Delete, ReferenceType.Actions, actionAttachment.ActionAttachmentID, "Deleted Action Attachment");
        }
        #endregion


        #region Log
        /// <summary> Log Message </summary>
        public static void LogMessage(AuthenticationModel authentication, ActionLogType logType, ReferenceType refType, int? refID, string message)
        {
            LoginUser user = new LoginUser(authentication.UserID, authentication.OrganizationID);
            ActionLogs.AddActionLog(user, logType, refType, refID.HasValue ? refID.Value : 0, message);  // 0 if no ID?
        }

        public static void LogMessage(AuthenticationModel authentication, ActionLogType logType, ReferenceType refType, int? refID, string message, Exception ex)
        {
            // log to ExceptionLogs or New Relic, or windows event log?

            string fullMessage = message + ex.ToString() + " ----- STACK: " + ex.StackTrace.ToString();
            if (Debugger.IsAttached)
            {
                Debug.WriteLine(fullMessage);   // debug output window (very fast)
                Debugger.Break();   // something is wrong - fix the code!
            }

            LogMessage(authentication, logType, refType, refID, fullMessage);
        }
        #endregion

    }
}
