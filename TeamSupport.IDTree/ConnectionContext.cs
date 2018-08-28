﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Linq;
using System.IO;
using System.Diagnostics;
using System.Web.Security;
using TeamSupport.Proxy;

namespace TeamSupport.IDTree
{
    /// <summary>
    /// Model for validating OrganizationID, UserID, TicketID, etc
    /// Centralizes queries for Attachments
    /// </summary>
    public class ConnectionContext : IDisposable
    {
        const bool _isEnabled = false;
        public static bool IsEnabled { get { return _isEnabled; } }

        public AuthenticationModel Authentication { get; private set; }
        SqlConnection _connection;
        SqlTransaction _transaction;
        public DataContext _db { get; private set; }
        public OrganizationNode Organization { get; private set; }
        public UserNode User { get; private set; }
        //public UserSession UserSession { get; private set; }
        public int UserID {  get { return User.UserID; } }

        public ConnectionContext(bool useTransaction = false)
        {
            // SqlConnection
            Authentication = new AuthenticationModel();
            _connection = new SqlConnection(Authentication.ConnectionString);  // using
            _connection.Open(); // connection must be open to begin transaction

            // DataContext
            _db = new DataContext(_connection);
            _db.ObjectTrackingEnabled = false;  // use linq read-only
            if (useTransaction)
            {
                _transaction = _connection.BeginTransaction();
                _db.Transaction = _transaction;
            }

            // Create Logical Model! - note that OrganizationID and UserID come from Authentication
            Organization = new OrganizationNode(this);
            User = new UserNode(Organization, Authentication.UserID);
            //UserSession = new UserSession(Organization);
        }

        public void Commit() { _db.Transaction.Commit(); }
        public void Rollback() { _db.Transaction.Rollback(); }

        public TicketNode Ticket(int ticketID) { return new TicketNode(Organization, ticketID); }

        public bool CanEdit() { return Authentication.IsSystemAdmin || User.AllowUserToEditAnyAction(); }

        public string AttachmentPath(int id)
        {
            return _db.ExecuteQuery<string>($"SELECT Value FROM FilePaths WITH(NOLOCK) WHERE ID = {id}").FirstOrDefault();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_transaction != null)
                _transaction.Dispose();

            if (_db != null)
                _db.Dispose();

            if (_connection != null)
                _connection.Dispose();
        }

    }
}
