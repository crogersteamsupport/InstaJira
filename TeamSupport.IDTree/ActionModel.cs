﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO;
using System.Diagnostics;
using System.Web;
using TeamSupport.Data;

namespace TeamSupport.IDTree
{
    /// <summary>
    /// Wrapper for Valid ActionID
    /// </summary>
    public class ActionModel : IDNode, IAttachedTo
    {
        public TicketModel Ticket { get; private set; }
        public int ActionID { get; private set; }

        public static int GetTicketID(DataContext db, int actionID)
        {
            return db.ExecuteQuery<int>($"SELECT TicketID FROM Actions WITH (NOLOCK) WHERE ActionID = {actionID}").Min();
        }

        /// <summary> top down - existing action </summary>
        public ActionModel(TicketModel ticket, int actionID) : this(ticket, actionID, true)
        {
        }

        public ActionModel(TicketModel ticket, int actionID, bool verify) : base(ticket)
        {
            Ticket = ticket;
            ActionID = actionID;
            if(verify)
                Verify();
        }

        /// <summary> bottom up  - existing action </summary>
        public ActionModel(ConnectionContext connection, int actionID) : base(connection)
        {
            ActionID = actionID;
            int ticketID = GetTicketID(Connection._db, actionID);
            Ticket = new TicketModel(Connection, ticketID);
            Verify();
        }

        public override bool Equals(object o)
        {
            ActionModel rhs = o as ActionModel;
            return (Connection == rhs.Connection) && (Ticket == rhs.Ticket) && (ActionID == rhs.ActionID);
        }

        public ActionProxy ActionProxy()
        {
            return ExecuteQuery<ActionProxy>($"SELECT * FROM Actions WHERE ActionID={ActionID}").First();
        }

        /// <summary> existing action attachment </summary>
        public ActionAttachmentModel ActionAttachment(int actionAttachmentID)
        {
            return new ActionAttachmentModel(this, actionAttachmentID);
        }

        public bool CanEdit() { return Connection.CanEdit() || (Connection.User.UserID == CreatorID()); }

        public const int ActionPathIndex = 3;
        public string AttachmentPath
        {
            get
            {
                string path = Ticket.Organization.AttachmentPath(ActionPathIndex);
                path = Path.Combine(path, "Actions");   // see AttachmentPath.GetFolderName(AttachmentPath.Folder.Actions);
                path = Path.Combine(path, ActionID.ToString());
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        public override void Verify()
        {
            //Verify($"SELECT ActionID FROM Actions WITH (NOLOCK) WHERE ActionID={ActionID} AND TicketID={Ticket.TicketID}");
        }

        public int CreatorID()
        {
            return ExecuteQuery<int>($"SELECT CreatorID FROM Actions WITH (NOLOCK) WHERE ActionID={ActionID}").Min();
        }

        public int TicketID()
        {
            return ExecuteQuery<int>($"SELECT TicketID FROM Actions WITH (NOLOCK) WHERE ActionID = {ActionID}").Min();
        }

        public static ActionModel[] GetActions(TicketModel ticket)
        {
            string query = $"SELECT ActionID FROM Actions WITH (NOLOCK) WHERE TicketId={ticket.TicketID}";
            int[] actionIDs = ticket.ExecuteQuery<int>(query).ToArray();
            ActionModel[] actions = new ActionModel[actionIDs.Length];
            for (int i = 0; i < actionIDs.Length; ++i)
                actions[i] = new ActionModel(ticket, actionIDs[i], false);
            return actions;
        }

    }
}
