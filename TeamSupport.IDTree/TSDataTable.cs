using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.Linq;

namespace TeamSupport.IDTree
{
    public class TSDataTable : IDisposable
    {
        AuthenticationModel _authentication;
        SqlConnection _connection;
        SqlTransaction _transaction;

        //const string ActionQuery = "SET NOCOUNT OFF; SELECT [ActionID], [ActionTypeID], [SystemActionTypeID], [Name], [TimeSpent], [DateStarted], [IsVisibleOnPortal], [IsKnowledgeBase], [ImportID], [DateCreated], [DateModified], [CreatorID], [ModifierID], [TicketID], [ActionSource], [DateModifiedBySalesForceSync], [SalesForceID], [DateModifiedByJiraSync], [JiraID], [Pinned], [Description], [IsClean], [ImportFileID] FROM [dbo].[Actions] WHERE ([ActionID] = @t0);";
        //const string UserQuery = "SET NOCOUNT OFF; SELECT [UserID], [Email], [FirstName], [MiddleName], [LastName], [Title], [CryptedPassword], [IsActive], [MarkDeleted], [TimeZoneID], [CultureName], [LastLogin], [LastActivity], [LastPing], [LastWaterCoolerID], [IsSystemAdmin], [IsFinanceAdmin], [IsPasswordExpired], [IsPortalUser], [IsChatUser], [PrimaryGroupID], [InOffice], [InOfficeComment], [ReceiveTicketNotifications], [ReceiveAllGroupNotifications], [SubscribeToNewTickets], [ActivatedOn], [DeactivatedOn], [OrganizationID], [LastVersion], [SessionID], [ImportID], [DateCreated], [DateModified], [CreatorID], [ModifierID], [OrgsUserCanSeeOnPortal], [DoNotAutoSubscribe], [IsClassicView], [SubscribeToNewActions], [ApprovedTerms], [ShowWelcomePage], [UserInformation], [PortalAutoReg], [AppChatID], [AppChatStatus], [MenuItems], [TicketRights], [Signature], [LinkedIn], [OnlyEmailAfterHours], [BlockInboundEmail], [SalesForceID], [ChangeTicketVisibility], [ChangeKBVisibility], [EnforceSingleSession], [NeedsIndexing], [AllowAnyTicketCustomer], [FontFamily], [FontSize], [CanCreateCompany], [CanEditCompany], [CanCreateContact], [CanEditContact], [RestrictUserFromEditingAnyActions], [AllowUserToEditAnyAction], [UserCanPinAction], [PortalLimitOrgTickets], [CanCreateAsset], [CanEditAsset], [CanChangeCommunityVisibility], [FilterInactive], [DisableExporting], [DisablePublic], [CanCreateProducts], [CanEditProducts], [CanCreateVersions], [CanEditVersions], [ReceiveUnassignedGroupEmails], [ProductFamiliesRights], [BlockEmailFromCreatingOnly], [CalGUID], [PortalViewOnly], [verificationPhoneNumber], [verificationCode], [verificationCodeExpiration], [PasswordCreatedUtc], [ImportFileID], [PortalLimitOrgChildrenTickets], [CanBulkMerge] FROM [dbo].[Users] WHERE ([UserID] = @t0);";

        public TSDataTable(string query, params object[] args)
        {
            _authentication = new AuthenticationModel();
            _connection = new SqlConnection(_authentication.ConnectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction(IsolationLevel.ReadUncommitted);
        }

        public void Commit() { _transaction.Commit(); }
        public void Rollback() { _transaction.Rollback(); }

        public DataRowCollection Load(string query, params object[] args)
        {
            using (DataTable _table = new DataTable())
            using (SqlCommand _command = new SqlCommand())
            {
                _command.Connection = _connection;
                _command.Transaction = _transaction;
                _command.CommandText = query;
                _command.CommandType = CommandType.Text;

                // parameters
                for (int i = 0; i < args.Length; ++i)
                {
                    SqlParameter parameter = new SqlParameter($"@t{i}", _typeMap[args[i].GetType()]);
                    parameter.Value = args[i];
                    _command.Parameters.Add(parameter);
                }

                using (SqlDataAdapter _adapter = new SqlDataAdapter(_command))
                {
                    _adapter.FillSchema(_table, SchemaType.Source);
                    _adapter.Fill(_table);
                }
                return _table.Rows;
            }
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
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }

        static Dictionary<Type, DbType> _typeMap;

        static TSDataTable()
        {
            _typeMap = new Dictionary<Type, DbType>();
            _typeMap[typeof(byte)] = DbType.Byte;
            _typeMap[typeof(sbyte)] = DbType.SByte;
            _typeMap[typeof(short)] = DbType.Int16;
            _typeMap[typeof(ushort)] = DbType.UInt16;
            _typeMap[typeof(int)] = DbType.Int32;
            _typeMap[typeof(uint)] = DbType.UInt32;
            _typeMap[typeof(long)] = DbType.Int64;
            _typeMap[typeof(ulong)] = DbType.UInt64;
            _typeMap[typeof(float)] = DbType.Single;
            _typeMap[typeof(double)] = DbType.Double;
            _typeMap[typeof(decimal)] = DbType.Decimal;
            _typeMap[typeof(bool)] = DbType.Boolean;
            _typeMap[typeof(string)] = DbType.String;
            _typeMap[typeof(char)] = DbType.StringFixedLength;
            _typeMap[typeof(Guid)] = DbType.Guid;
            _typeMap[typeof(DateTime)] = DbType.DateTime;
            _typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            _typeMap[typeof(byte[])] = DbType.Binary;
            _typeMap[typeof(byte?)] = DbType.Byte;
            _typeMap[typeof(sbyte?)] = DbType.SByte;
            _typeMap[typeof(short?)] = DbType.Int16;
            _typeMap[typeof(ushort?)] = DbType.UInt16;
            _typeMap[typeof(int?)] = DbType.Int32;
            _typeMap[typeof(uint?)] = DbType.UInt32;
            _typeMap[typeof(long?)] = DbType.Int64;
            _typeMap[typeof(ulong?)] = DbType.UInt64;
            _typeMap[typeof(float?)] = DbType.Single;
            _typeMap[typeof(double?)] = DbType.Double;
            _typeMap[typeof(decimal?)] = DbType.Decimal;
            _typeMap[typeof(bool?)] = DbType.Boolean;
            _typeMap[typeof(char?)] = DbType.StringFixedLength;
            _typeMap[typeof(Guid?)] = DbType.Guid;
            _typeMap[typeof(DateTime?)] = DbType.DateTime;
            _typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
            _typeMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;
        }
    }

}
