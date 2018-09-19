﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.Linq;
using TeamSupport.Data;

namespace TeamSupport.IDTree
{
    public abstract class IDNode
    {
        public ConnectionContext Connection { get; private set; }
        
        protected IDNode(ConnectionContext request)
        {
            Connection = request;
        }

        public IDNode AsIDNode { get { return this; } }

        protected IDNode(IDNode node) //: this(node.Request)
        {
            Connection = node.Connection;
        }

        public IEnumerable<TResult> ExecuteQuery<TResult>(string query, params object[] parameters)
        {
            return Connection._db.ExecuteQuery<TResult>(query, parameters);
        }
        public int ExecuteCommand(string command, params object[] parameters)
        {
            return Connection._db.ExecuteCommand(command, parameters);
        }

        public abstract void Verify();

        /// <summary> Verify helper </summary>
        protected void Verify(string query)
        {
            if (ExecuteQuery<int>(query).Any()) // valid ID found?
                return;

            if (Debugger.IsAttached)
            {
                Debug.WriteLine(query);   // see the failed query in the debug output window
                Debugger.Break();   // ID is wrong - fix the code!
            }
            throw new System.Data.ConstraintException(String.Format($"{query} not found")); // error - a join of the records to authentication just doesn't add up
        }

        public static IAttachmentParent GetModel<T>(ConnectionContext connection, T proxy) where T : class
        {
            switch (proxy.GetType().Name)
            {
                case "ActionAttachmentProxy":
                    {
                        ActionAttachmentProxy attachment = proxy as ActionAttachmentProxy;
                        return new ActionModel(connection, attachment.RefID);
                    }
                case "TaskAttachmentProxy":
                    {
                        TaskAttachmentProxy attachment = proxy as TaskAttachmentProxy;
                        return new TaskModel(connection, attachment.RefID);
                    }
            }
            return null;
        }

    }
}
