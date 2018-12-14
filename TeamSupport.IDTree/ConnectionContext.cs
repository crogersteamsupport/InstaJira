using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Linq;
using System.IO;
using System.Diagnostics;
using System.Web.Security;
using System.Data;

namespace TeamSupport.IDTree
{
    /// <summary>
    /// Model for validating OrganizationID, UserID, TicketID, etc
    /// Centralizes queries for Attachments
    /// </summary>
    public class ConnectionContext : IDisposable
    {
        public AuthenticationModel Authentication { get; private set; }
        public SqlConnection _connection;
        SqlTransaction _transaction;
        public DataContext _db { get; private set; }

        // Login Organization
        public OrganizationModel Organization { get; private set; }
        public int OrganizationID { get { return Organization.OrganizationID; } }

        // Login User
        public UserModel User { get; private set; }
        public int UserID { get { return User.UserID; } }

        public ConnectionContext(bool useTransaction = false) : this(new AuthenticationModel(), useTransaction)
        {

        }

        public ConnectionContext(AuthenticationModel authentication, bool useTransaction = false)
        {
            // SqlConnection
            Authentication = authentication;
            _connection = new SqlConnection(Authentication.ConnectionString);  // using
            _connection.Open(); // connection must be open to begin transaction

            // DataContext
            _db = new DataContext(_connection);
            //_db.ObjectTrackingEnabled = false;  // use linq read-only
            //db.DeferredLoadingEnabled = true; // don't use DataLoadOptions
            if (useTransaction)
            {
                _transaction = _connection.BeginTransaction();
                _db.Transaction = _transaction;
            }

            // Create Logical Model! - note that OrganizationID and UserID come from Authentication
            Organization = new OrganizationModel(this);
            User = new UserModel(this);
        }

        public void Commit() { _db.Transaction.Commit(); }
        public void Rollback() { _db.Transaction.Rollback(); }

        public TicketModel Ticket(int ticketID) { return new TicketModel(Organization, ticketID); }

        public bool CanEdit()
        {
            // user in organization or child of organization?
            //Organization organization = (Organization)Organizations.GetOrganization(UserSession.LoginUser, _organizationID);
            //if (organization.OrganizationID != UserSession.LoginUser.OrganizationID && organization.ParentID != UserSession.LoginUser.OrganizationID)

            return Authentication.IsSystemAdmin || User.AllowUserToEditAnyAction();
        }

        public string AttachmentPath(int id)
        {
            return _db.ExecuteQuery<string>($"SELECT Value FROM FilePaths WITH(NOLOCK) WHERE ID = {id}").FirstOrDefault();
        }

        //public DataRowCollection GetRowCollection(string query, params object[] args)
        //{
        //    using (DataTable _table = new DataTable())
        //    using (SqlCommand _command = new SqlCommand())
        //    {
        //        _command.Connection = _connection;
        //        _command.Transaction = _transaction;
        //        _command.CommandText = query;
        //        _command.CommandType = CommandType.Text;

        //        // parameters
        //        for (int i = 0; i < args.Length; ++i)
        //        {
        //            SqlParameter parameter = new SqlParameter($"@t{i}", _typeMap[args[i].GetType()]);
        //            parameter.Value = args[i];
        //            _command.Parameters.Add(parameter);
        //        }

        //        using (SqlDataAdapter _adapter = new SqlDataAdapter(_command))
        //        {
        //            _adapter.FillSchema(_table, SchemaType.Source);
        //            _adapter.Fill(_table);
        //        }
        //        return _table.Rows;
        //    }
        //}

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

        //static Dictionary<Type, DbType> _typeMap;
        //static ConnectionContext()
        //{
        //    _typeMap = new Dictionary<Type, DbType>();
        //    _typeMap[typeof(byte)] = DbType.Byte;
        //    _typeMap[typeof(sbyte)] = DbType.SByte;
        //    _typeMap[typeof(short)] = DbType.Int16;
        //    _typeMap[typeof(ushort)] = DbType.UInt16;
        //    _typeMap[typeof(int)] = DbType.Int32;
        //    _typeMap[typeof(uint)] = DbType.UInt32;
        //    _typeMap[typeof(long)] = DbType.Int64;
        //    _typeMap[typeof(ulong)] = DbType.UInt64;
        //    _typeMap[typeof(float)] = DbType.Single;
        //    _typeMap[typeof(double)] = DbType.Double;
        //    _typeMap[typeof(decimal)] = DbType.Decimal;
        //    _typeMap[typeof(bool)] = DbType.Boolean;
        //    _typeMap[typeof(string)] = DbType.String;
        //    _typeMap[typeof(char)] = DbType.StringFixedLength;
        //    _typeMap[typeof(Guid)] = DbType.Guid;
        //    _typeMap[typeof(DateTime)] = DbType.DateTime;
        //    _typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
        //    _typeMap[typeof(byte[])] = DbType.Binary;
        //    _typeMap[typeof(byte?)] = DbType.Byte;
        //    _typeMap[typeof(sbyte?)] = DbType.SByte;
        //    _typeMap[typeof(short?)] = DbType.Int16;
        //    _typeMap[typeof(ushort?)] = DbType.UInt16;
        //    _typeMap[typeof(int?)] = DbType.Int32;
        //    _typeMap[typeof(uint?)] = DbType.UInt32;
        //    _typeMap[typeof(long?)] = DbType.Int64;
        //    _typeMap[typeof(ulong?)] = DbType.UInt64;
        //    _typeMap[typeof(float?)] = DbType.Single;
        //    _typeMap[typeof(double?)] = DbType.Double;
        //    _typeMap[typeof(decimal?)] = DbType.Decimal;
        //    _typeMap[typeof(bool?)] = DbType.Boolean;
        //    _typeMap[typeof(char?)] = DbType.StringFixedLength;
        //    _typeMap[typeof(Guid?)] = DbType.Guid;
        //    _typeMap[typeof(DateTime?)] = DbType.DateTime;
        //    _typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
        //    _typeMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;
        //}

    }
}
