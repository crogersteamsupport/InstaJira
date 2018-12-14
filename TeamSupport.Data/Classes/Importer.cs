using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TeamSupport.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Threading;
using System.Web.Security;
using System.IO;
using Microsoft.Win32;
using System.Data.OleDb;
using System.Text.RegularExpressions;

namespace TeamSupport.Data
{
    using IdList = SortedList<string, int>;
    using ImportCustomFields = List<ImportCustomField>;

    class ImportCustomField
    {
        public ImportCustomField(string tableName, string fieldName, int tsFieldID, ReferenceType refType, int auxID)
        {
            TableName = tableName;
            FieldName = fieldName;
            TSFieldID = tsFieldID;
            RefType = refType;
            AuxID = auxID;
        }

        public int AuxID { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public int TSFieldID { get; set; }
        public ReferenceType RefType { get; set; }
    }

    public class Importer
    {
        const int BULK_LIMIT = 1000;
        bool _IsBulk = true;

        class ImportCustomInfo
        {
            public ImportCustomInfo(CustomFieldType fieldType, string listValues)
            {
                FieldType = fieldType;
                ListValues = listValues;
            }

            public CustomFieldType FieldType;
            public string ListValues;
        }

        class ImportType
        {
            public ImportType(string typeName, ReferenceType referenceType, int tsTypeID)
            {
                TypeName = typeName;
                ReferenceType = referenceType;
                TSTypeID = tsTypeID;
            }

            public string TypeName;
            public ReferenceType ReferenceType;
            public int TSTypeID;
        }

        private ImportCustomFields _importCustomFields;
        private LoginUser _loginUser;
        private User _creator;
        private DataRow _currentRow;
        private ImportLog _log;
        private int _organizationID;
        private string _fileName;
        private string[] _ticketTypeNames;

        public Importer(LoginUser loginUser, string fileName)
        {
            _creator = Users.GetUser(loginUser, loginUser.UserID);
            _loginUser = new LoginUser(loginUser.ConnectionString, -5, loginUser.OrganizationID, null);
            _fileName = fileName;
        }

        private string GetConnectionString()
        {
            return @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + _fileName + @";Extended Properties=""Excel 12.0;HDR=YES;IMEX=1""";
            //@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + _fileName + @";Extended Properties=""Excel 8.0;HDR=YES;IMEX=1""";
        }

        private DataRow ReadRow(string tableName, string keyColumnName, string value)
        {
            try
            {
                DataTable table = new DataTable();

                using (OleDbConnection connection = new OleDbConnection(GetConnectionString()))
                {

                    string query = string.Format("SELECT * FROM [{0}$] WHERE {1} = '{2}'", tableName, keyColumnName, value);
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                    connection.Open();
                    adapter.Fill(table);
                    connection.Close();
                }

                if (table.Rows.Count < 1) return null;

                return table.Rows[0];
            }
            catch (Exception)
            {
                return null;
            }
        }

        private DataTable ReadTable(string tableName)
        {
            try
            {
                DataTable table = new DataTable();
                using (OleDbConnection connection = new OleDbConnection(GetConnectionString()))
                {

                    string query = "SELECT * FROM [" + tableName + "$]";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                    connection.Open();
                    adapter.Fill(table);
                    connection.Close();
                }

                for (int i = table.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow row = table.Rows[i];
                    if (row[0].ToString().Trim() == "") row.Delete();
                }
                table.AcceptChanges();

                return table;

            }
            catch (Exception ex)
            {
                _log.AppendMessage("Unable to open table '" + tableName + "' - " + ex.Message);
                return null;
            }
        }

        private DataTable ReadDistinctValues(string tableName, string columnName)
        {
            DataTable table = new DataTable();
            using (OleDbConnection connection = new OleDbConnection(GetConnectionString()))
            {

                string query = string.Format("SELECT DISTINCT ({0}) FROM [{1}$]", columnName, tableName);
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                connection.Open();
                adapter.Fill(table);
                connection.Close();
            }

            return table;
        }

        private int ReadRecordCount(string tableName)
        {
            DataTable table = new DataTable();
            using (OleDbConnection connection = new OleDbConnection(GetConnectionString()))
            {
                OleDbCommand command = new OleDbCommand();
                command.CommandText = string.Format("SELECT COUNT(*) FROM [{0}$]", tableName);
                command.CommandType = CommandType.Text;
                command.Connection = connection;
                connection.Open();
                object o = command.ExecuteScalar();
                connection.Close();

                if (o == null || o == DBNull.Value) return 0;
                else return (int)o;
            }
        }

        private DataTable ReadEmptyTable(string tableName)
        {
            DataTable table = new DataTable();
            using (OleDbConnection connection = new OleDbConnection(GetConnectionString()))
            {

                string query = "SELECT * FROM [" + tableName + "$] WHERE 0=1";
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                connection.Open();
                adapter.Fill(table);
                connection.Close();
            }

            for (int i = table.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = table.Rows[i];
                if (row[0].ToString().Trim() == "") row.Delete();
            }
            table.AcceptChanges();

            return table;

        }

        private int CreateOrganization(string name)
        {
            Organization organization = (new Organizations(_loginUser)).AddNewOrganization();
            organization.Name = name;
            organization.Description = "";
            organization.Website = "";
            organization.ParentID = 1;
            organization.PortalSeats = 100;
            organization.IsApiActive = true;
            organization.IsApiEnabled = false;
            organization.IsAdvancedPortal = true;
            organization.HasPortalAccess = true;
            organization.IsBasicPortal = true;
            organization.ShowWiki = true;
            organization.WhereHeard = "";
            organization.TimeZoneID = "Central Standard Time";
            organization.IsActive = true;
            organization.IsCustomerFree = false;
            organization.BusinessDays = 62;
            organization.ExtraStorageUnits = 0;
            organization.UserSeats = 100;
            organization.ChatSeats = 100;
            organization.APIRequestLimit = 5000;
            organization.CultureName = "en-US";
            organization.ProductType = ProductType.Enterprise;
            organization.AdminOnlyCustomers = false;

            organization.Collection.Save();


            User user = (new Users(_loginUser)).AddNewUser();
            user.ActivatedOn = DateTime.UtcNow;
            user.Email = _creator.Email;
            user.FirstName = user.FirstName;
            user.InOffice = true;
            user.IsActive = true;
            user.IsChatUser = true;
            user.IsFinanceAdmin = true;
            user.IsPasswordExpired = false;
            user.IsPortalUser = true;
            user.IsSystemAdmin = true;
            user.LastActivity = DateTime.UtcNow;
            user.LastLogin = DateTime.UtcNow;
            user.LastName = _creator.LastName;
            user.LastPing = DateTime.UtcNow;
            user.MarkDeleted = false;
            user.CryptedPassword = _creator.CryptedPassword;
            user.OrganizationID = organization.OrganizationID;
            user.ReceiveTicketNotifications = true;
            user.SubscribeToNewTickets = false;
            user.Collection.Save();

            return organization.OrganizationID;
        }

        private void CreateTicketTypes()
        {
            TicketTypes ticketTypes = new TicketTypes(_loginUser);
            ticketTypes.LoadAllPositions(_organizationID);

            string[] value = GetSettingString("TicketTypes").Split(',');
            List<string> ticketTypeNames = new List<string>();

            foreach (string s in value)
            {
                string name = s.Trim();
                ticketTypeNames.Add(name);
                if (ticketTypes.FindByName(name) == null)
                {
                    ticketTypes = new TicketTypes(_loginUser);
                    TicketType ticketType = ticketTypes.AddNewTicketType();
                    ticketType.Name = name;
                    ticketType.Description = "";
                    ticketType.OrganizationID = _organizationID;
                    ticketType.Position = ticketTypes.GetMaxPosition(_organizationID) + 1;
                    ticketTypes.Save();
                    ticketTypes.LoadAllPositions(_organizationID);
                }
            }
            _ticketTypeNames = ticketTypeNames.ToArray();
        }

        public string Import(int organizationID, bool importOnlyCustomFields)
        {
            using (_log = new ImportLog(Path.ChangeExtension(_fileName, ".log")))
            {

                _organizationID = organizationID;
                _loginUser = new LoginUser(_loginUser.ConnectionString, -5, organizationID, null);

                _log.AppendMessage("Import Started: " + DateTime.Now.ToString());
                _currentRow = null;

                CreateTicketTypes();

                using (SqlConnection connection = new SqlConnection(_loginUser.ConnectionString))
                {
                    connection.Open();

                    try
                    {
                        if (!importOnlyCustomFields)
                        {
                            GC.WaitForPendingFinalizers();
                            ImportUsers();
                            GC.WaitForPendingFinalizers();
                            ImportGroups();
                            GC.WaitForPendingFinalizers();
                            ImportUserGroups();
                            GC.WaitForPendingFinalizers();
                            ImportCustomers();
                            GC.WaitForPendingFinalizers();
                            ImportContacts();
                            GC.WaitForPendingFinalizers();
                            ImportPrimaryContacts();
                            GC.WaitForPendingFinalizers();
                            ImportProducts();
                            GC.WaitForPendingFinalizers();
                            ImportProductVersions();
                            GC.WaitForPendingFinalizers();
                            ImportAssets();
                            GC.WaitForPendingFinalizers();
                            ImportAssetHistory();
                            GC.WaitForPendingFinalizers();
                            ImportTickets();
                            GC.WaitForPendingFinalizers();
                            ImportActions();
                            GC.WaitForPendingFinalizers();
                            ImportAttachments();
                            GC.WaitForPendingFinalizers();
                            ImportAddresses();
                            GC.WaitForPendingFinalizers();
                            ImportPhoneNumbers();
                            GC.WaitForPendingFinalizers();
                            ImportNotes();
                            GC.WaitForPendingFinalizers();
                            ImportCustomerProducts();
                            GC.WaitForPendingFinalizers();
                            ImportCustomerTickets();
                            GC.WaitForPendingFinalizers();
                            ImportContactTickets();
                            GC.WaitForPendingFinalizers();
                            ImportAssetTickets();
                            GC.WaitForPendingFinalizers();
                            ImportParentTickets();
                            GC.WaitForPendingFinalizers();
                            ImportTicketSubscriptions();
                            GC.WaitForPendingFinalizers();
                            ImportWiki();
                            GC.WaitForPendingFinalizers();

                        }
                        ImportCustomFields();
                        GC.WaitForPendingFinalizers();
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogs.LogException(_loginUser, ex, "Import Module", _currentRow);
                        string log = "Exception: {0}\n\nStack Trace: {1}\n\nData Row {2}\n\nImport TERMINATED!";
                        _log.AppendMessage(string.Format(log, ex.Message, ex.StackTrace, DataUtils.DataRowToString(_currentRow)));
                        return ex.Message;
                    }
                    connection.Close();
                }
                _log.AppendMessage("Import Ended: " + DateTime.Now.ToString());

                EmailPosts.DeleteImportEmails(_loginUser);
            }

            return "";

        }

        public string Import(string organizationName, bool importOnlyCustomFields)
        {
            return Import(CreateOrganization(organizationName), importOnlyCustomFields);
        }

        #region Type Import Methods

        private void CreateAllCustomFields()
        {
            foreach (string ticketTypeName in _ticketTypeNames)
            {
                CreateCustomFields(ticketTypeName, "DateClosed", true);
            }
            CreateCustomFields("Customers", "Website");
            CreateCustomFields("Contacts", "DateCreated");
            CreateCustomFields("Users", "IsActive");
            CreateCustomFields("Groups", "Description");
            CreateCustomFields("Products", "Description");
            CreateCustomFields("Versions", "IsReleased");
            CreateCustomFields("CustomerProducts", "VersionID");
            CreateCustomFields("Assets", "Status");
        }

        private void CreateCustomFields(string tableName, string lastColumnName)
        {
            CreateCustomFields(tableName, lastColumnName, false);
        }

        private void CreateCustomFields(string tableName, string lastColumnName, bool isTicket)
        {
            DataTable table = ReadTable(tableName);
            if (table == null) return;

            TicketTypes ticketTypes = new TicketTypes(_loginUser);
            ticketTypes.LoadAllPositions(_organizationID);

            ReferenceType referenceType = ReferenceType.None;
            CustomFields customFields = new CustomFields(_loginUser);
            int auxID = -1;

            if (isTicket)
            {
                referenceType = ReferenceType.Tickets; auxID = ticketTypes.FindByName(tableName).TicketTypeID;
            }
            else
            {
                switch (tableName)
                {
                    case "Customers": referenceType = ReferenceType.Organizations; auxID = -1; break;
                    case "Contacts": referenceType = ReferenceType.Contacts; auxID = -1; break;
                    case "Users": referenceType = ReferenceType.Users; auxID = -1; break;
                    case "Products": referenceType = ReferenceType.Products; auxID = -1; break;
                    case "Versions": referenceType = ReferenceType.ProductVersions; auxID = -1; break;
                    case "CustomerProducts": referenceType = ReferenceType.OrganizationProducts; auxID = -1; break;
                    case "Assets": referenceType = ReferenceType.Assets; auxID = -1; break;
                    default:
                        break;
                }
            }

            customFields.LoadByReferenceType(_organizationID, referenceType, auxID);


            int index = -1;
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (table.Columns[i].ColumnName == lastColumnName)
                {
                    index = i + 1;
                    break;
                }
            }

            for (int i = index; i < table.Columns.Count; i++)
            {
                string name = table.Columns[i].ColumnName;
                if (name.Trim() == "") continue;
                CustomField field = customFields.FindByName(name);

                if (field == null)
                {
                    string apiFieldName = CustomFields.GenerateApiFieldName(auxID < 0 ? tableName + "_" + name : tableName + "_" + name + "_" + auxID.ToString());
                    int apiCopyInt = 1;
                    while (customFields.FindByApiFieldName(apiFieldName) != null)
                    {
                        apiFieldName = apiFieldName + "_" + apiCopyInt.ToString();
                        apiCopyInt++;
                    }

                    field = (new CustomFields(_loginUser)).AddNewCustomField();
                    field.OrganizationID = _organizationID;
                    field.Name = name;
                    field.ApiFieldName = apiFieldName;
                    field.Position = customFields.GetMaxPosition(_organizationID, referenceType, auxID);
                    field.AuxID = auxID;
                    field.RefType = referenceType;
                    field.Description = "";
                    ImportCustomInfo info = GetCustomInfo(table, table.Columns[i].ColumnName);
                    field.FieldType = info.FieldType;
                    field.ListValues = info.ListValues;
                    field.Collection.Save();
                    customFields.LoadByReferenceType(_organizationID, referenceType, auxID);
                }

                _importCustomFields.Add(new ImportCustomField(tableName, name, field.CustomFieldID, referenceType, auxID));
            }
        }

        private ProductVersionStatus GetVersionStatus(ProductVersionStatuses statuses, string name)
        {
            name = name.Trim();
            ProductVersionStatus result = statuses.FindByName(name);
            if (result == null)
            {
                result = (new ProductVersionStatuses(_loginUser)).AddNewProductVersionStatus();
                result.IsDiscontinued = false;
                result.IsShipping = true;
                result.Name = name;
                result.OrganizationID = _organizationID;
                result.Position = statuses.GetMaxPosition(_organizationID) + 1;
                result.Collection.Save();
            }
            statuses.LoadAllPositions(_organizationID);
            return result;
        }

        private PhoneType GetPhoneType(PhoneTypes phoneTypes, string name)
        {
            name = name.Trim();
            PhoneType result = phoneTypes.FindByName(name);
            if (result == null)
            {
                result = (new PhoneTypes(_loginUser)).AddNewPhoneType();
                result.Name = name;
                result.OrganizationID = _organizationID;
                result.Position = phoneTypes.GetMaxPosition(_organizationID) + 1;
                result.Collection.Save();
                phoneTypes.LoadAllPositions(_organizationID);
            }
            return result;
        }

        private TicketStatus GetTicketStatus(TicketStatuses statuses, string name, TicketType ticketType)
        {
            name = name.Trim();
            if (string.IsNullOrWhiteSpace(name)) name = "None";
            TicketStatus result = null;
            int statusID = -1;
            if (int.TryParse(name, out statusID))
            {
                result = statuses.FindByTicketStatusID(statusID);
            }


            if (result == null)
            {
                result = statuses.FindByName(name, ticketType.TicketTypeID);

                if (result == null)
                {
                    result = (new TicketStatuses(_loginUser)).AddNewTicketStatus();
                    result.Name = name;
                    result.Description = "";
                    result.TicketTypeID = ticketType.TicketTypeID;
                    result.OrganizationID = _organizationID;
                    result.Position = statuses.GetMaxPosition(_organizationID) + 1;
                    result.IsClosed = false;
                    result.Collection.Save();
                }
            }
            statuses.LoadAllPositions(ticketType.TicketTypeID);
            return result;
        }

        private TicketSeverity GetTicketSeverity(TicketSeverities severities, string name)
        {
            name = name.Trim();
            TicketSeverity result = severities.FindByName(name);
            if (result == null)
            {
                result = (new TicketSeverities(_loginUser)).AddNewTicketSeverity();
                result.Name = name;
                result.Description = "";
                result.OrganizationID = _organizationID;
                result.Position = severities.GetMaxPosition(_organizationID) + 1;
                result.Collection.Save();

            }
            severities.LoadAllPositions(_organizationID);
            return result;
        }

        private SystemActionType GetSystemActionTypeID(string name)
        {
            SystemActionType result = SystemActionType.Custom;
            switch (name.ToLower().Trim())
            {
                case "description": result = SystemActionType.Description; break;
                case "resolution": result = SystemActionType.Resolution; break;
                case "updaterequest": result = SystemActionType.UpdateRequest; break;
                case "email": result = SystemActionType.Email; break;
                case "reminder": result = SystemActionType.Reminder; break;
                default:
                    break;
            }
            return result;
        }

        private int? GetActionTypeID(ActionTypes actionTypes, string name)
        {
            name = name.Trim();
            if (GetSystemActionTypeID(name) != SystemActionType.Custom) return null;

            ActionType actionType = actionTypes.FindByName(name);

            if (actionType == null)
            {
                ActionTypes ats = new ActionTypes(_loginUser);
                actionType = ats.AddNewActionType();
                actionType.Name = name;
                actionType.Description = "";
                actionType.IsTimed = true;
                actionType.OrganizationID = _organizationID;
                actionType.Position = actionTypes.GetMaxPosition(_organizationID) + 1;
                ats.Save();
                int? result = actionType.ActionTypeID;
                actionTypes.LoadAllPositions(_organizationID);
                return result;
            }
            else
            {
                return actionType.ActionTypeID;
            }
        }

        #endregion

        #region Utility Methods
        public static void SetTypeGuessRowsForExcelImport()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Jet\4.0\Engines\Excel", true);
            if (key == null)
            {
                key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Jet\4.0\Engines\Excel", true);
            }

            if (key != null)
            {
                key.SetValue("TypeGuessRows", 0);
                key.SetValue("ImportMixedTypes", "Text");
            }

            SetAceProperties("10.0", 0, "Text");
            SetAceProperties("11.0", 0, "Text");
            SetAceProperties("12.0", 0, "Text");
            SetAceProperties("13.0", 0, "Text");
            SetAceProperties("14.0", 0, "Text");
            SetAceProperties("15.0", 0, "Text");
            SetAceProperties("16.0", 0, "Text");
        }

        private static void SetAceProperties(string officeVersion, int typeGuessRows, string importMixedTypes)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Office\" + officeVersion + @"\Access Connectivity Engine\Engines\Excel", true);
            if (key != null)
            {
                key.SetValue("TypeGuessRows", typeGuessRows);
                key.SetValue("ImportMixedTypes", importMixedTypes);
            }

            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Office\" + officeVersion + @"\Access Connectivity Engine\Engines\Excel", true);
            if (key != null)
            {
                key.SetValue("TypeGuessRows", typeGuessRows);
                key.SetValue("ImportMixedTypes", importMixedTypes);
            }
        }

        private string GetListToCommaString(List<string> list)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string s in list)
            {
                if (builder.Length > 0) builder.Append("|");
                builder.Append(s);
            }

            return builder.ToString();
        }

        private ImportCustomInfo GetCustomInfo(DataTable table, string columnName)
        {

            // test for date time
            bool flag = true;
            bool hasData = false;
            int count = 0;
            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                if (count++ > 100) break;
                string value = row[columnName].ToString().Trim();
                if (value == "") continue;
                try
                {
                    hasData = true;
                    DateTime.Parse(value);
                }
                catch (Exception)
                {
                    flag = false;
                    break;
                }
            }


            if (flag && hasData)
            {
                return new ImportCustomInfo(CustomFieldType.DateTime, "");
            }


            // test for number
            flag = true;
            count = 0;
            hasData = false;
            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                if (count++ > 100) break;
                string value = row[columnName].ToString().Trim();
                if (value == "") continue;
                try
                {
                    double.Parse(value);
                    hasData = true;
                }
                catch (Exception)
                {
                    flag = false;
                    break;
                }
            }


            if (flag && hasData)
            {
                return new ImportCustomInfo(CustomFieldType.Number, "");
            }


            List<string> list = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                string value = row[columnName].ToString().Trim();

                if (value != "" && list.IndexOf(value) < 0) list.Add(value);
                if (list.Count > 50)
                {
                    list.Clear();
                    return new ImportCustomInfo(CustomFieldType.Text, "");
                }
            }


            if (list.Count < 2) return new ImportCustomInfo(CustomFieldType.Text, "");

            if (list.Count == 2)
            {
                if ((list[0].ToLower().Trim() == "false" ||
                    list[0].ToLower().Trim() == "true" ||
                    list[0].ToLower().Trim() == "no" ||
                    list[0].ToLower().Trim() == "yes")
                    &&
                    (list[1].ToLower().Trim() == "false" ||
                    list[1].ToLower().Trim() == "true" ||
                    list[1].ToLower().Trim() == "no" ||
                    list[1].ToLower().Trim() == "yes"))
                {
                    return new ImportCustomInfo(CustomFieldType.Boolean, "");
                }
            }

            return new ImportCustomInfo(CustomFieldType.PickList, GetListToCommaString(list));

        }

        private bool GetDBBool(object o, bool defValue = false)
        {
            if (o == null) return defValue;
            try
            {
                return bool.Parse(o.ToString().Trim());
            }
            catch (Exception)
            {

                return defValue;
            }
        }

        private bool GetDBBool(DataRow row, string columnName, bool defValue = false)
        {
            if (!row.Table.Columns.Contains(columnName)) return defValue;

            try
            {
                return bool.Parse(row[columnName].ToString().Trim());
            }
            catch (Exception)
            {
                return defValue;
            }
        }

        private DateTime? GetDBDate(DataRow row, string columnName, bool allowNull)
        {

            DateTime? result = allowNull ? null : (DateTime?)DateTime.UtcNow;

            if (row.Table.Columns.Contains(columnName))
            {
                object o = row[columnName];
                try
                {
                    if (o.ToString().Trim() != "") result = DateTime.Parse(o.ToString().Trim());
                    result = TimeZoneInfo.ConvertTimeToUtc((DateTime)result);
                }
                catch (Exception)
                {
                }
            }

            return result;
        }

        private DateTime? GetDBDate(object o, bool allowNull)
        {
            DateTime? result = allowNull ? null : (DateTime?)DateTime.UtcNow;

            try
            {
                if (o.ToString().Trim() != "") result = DateTime.Parse(o.ToString().Trim());
                result = TimeZoneInfo.ConvertTimeToUtc((DateTime)result);
            }
            catch (Exception)
            {
            }

            return result;
        }

        private int? GetDBInt(object o, bool allowNull)
        {
            int? result = allowNull ? null : (int?)0;

            try
            {
                if (o.ToString().Trim() != "") result = int.Parse(o.ToString().Trim());
            }
            catch (Exception)
            {
            }

            return result;
        }

        private string GetDBString(DataRow row, string columnName, int maxLength = 0, bool allowNull = false)
        {
            if (!row.Table.Columns.Contains(columnName)) return allowNull ? null : "";

            try
            {
                string result = row[columnName].ToString().Trim();
                if (allowNull && result == "") return null;
                if (result.Length > maxLength && maxLength > 0) result = result.Substring(0, maxLength);
                return result;
            }
            catch (Exception)
            {
                return allowNull ? null : "";
            }
        }

        private string GetDBString(object o, int maxLength, bool allowNull)
        {
            string result = o.ToString().Trim();
            if (allowNull && result == "") return null;
            if (result.Length > maxLength && maxLength > 0) result = result.Substring(0, maxLength);
            return result;
        }

        private int? GetUserOrContact(IdList list, string id)
        {
            int value;
            id = id.ToLower().Trim();
            if (list.TryGetValue(id, out value))
            {
                return value;
            }
            else if (list.TryGetValue("[contact]" + id, out value))
            {
                return value;
            }

            return null;
        }

        private int? GetListID(IdList list, string id)
        {
            int value;
            id = id.ToLower().Trim();
            if (list.TryGetValue(id, out value))
            {
                return value;
            }
            return null;
        }


        private int GetUserContactID(string importID, Users users)
        {
            string id = importID;
            if (importID.ToLower().IndexOf("[contact]") > -1)
            {
                importID = importID.Substring(9, importID.Length - 9);

            }

            User user = users.FindByImportID(importID);
            if (user != null)
                return user.UserID;
            else
                return -1;
        }

        private bool GetSettingBool(string key)
        {
            return GetSettingString(key).ToLower() == "true";
        }

        private string GetSettingString(string key)
        {
            DataRow row = ReadRow("Settings", "Key", key);
            //if (row == null) throw new Exception("Missing Setting: " + key);
            _log.AppendMessage("Missing Setting: " + key);
            if (row == null) return ""; ;
            return row[1].ToString().Trim();
        }

        private int GetSettingInt(string key)
        {
            return int.Parse(GetSettingString(key));
        }

        private IdList GetIdList(BaseCollection collection)
        {
            IdList result = new SortedList<string, int>();
            foreach (DataRow row in collection.Table.Rows)
            {
                string importedID = row["ImportID"] != DBNull.Value ? row["ImportID"].ToString().Trim().ToLower() : "";
                if (importedID != "")
                {
                    try
                    {
                        result.Add(importedID, (int)row[collection.PrimaryKeyFieldName]);
                    }
                    catch
                    {
                    }
                }
            }

            return result;
        }

        private string ConvertHtmlLineBreaks(string text, string lineBreak = "<br />")
        {
            return Regex.Replace(text, @"\r\n?|\n", lineBreak);
        }

        #endregion

        #region Import Methods
        private void ImportUsers()
        {
            DataTable table = ReadTable("Users");
            if (table == null) return;
            Users users = new Users(_loginUser);
            Users existing = new Users(_loginUser);
            existing.LoadByOrganizationID(_organizationID, false);
            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                if (existing.FindByImportID(row["UserID"].ToString().Trim()) != null) continue;
                User foundUser = existing.FindByEmail(row["Email"].ToString());
                if (foundUser != null)
                {
                    foundUser.ImportID = row["UserID"].ToString().Trim();
                    foundUser.Collection.Save();
                    continue;
                }
                User user = users.AddNewUser();
                user.ActivatedOn = DateTime.UtcNow;
                user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(row["Password"].ToString().Trim(), "MD5");
                user.DeactivatedOn = null;
                user.Email = row["Email"].ToString().Trim();
                user.FirstName = row["FirstName"].ToString().Trim();
                user.ImportID = row["UserID"].ToString().Trim();
                user.InOffice = false;
                user.InOfficeComment = "";
                user.IsActive = bool.Parse(row["IsActive"].ToString().Trim());
                user.IsFinanceAdmin = bool.Parse(row["IsFinanceAdmin"].ToString().Trim());
                user.IsSystemAdmin = bool.Parse(row["IsSystemAdmin"].ToString().Trim());
                user.IsPasswordExpired = true;
                user.IsPortalUser = true;
                user.LastActivity = DateTime.UtcNow;
                user.LastLogin = DateTime.UtcNow;
                user.LastName = row["LastName"].ToString().Trim();
                user.MiddleName = row["MiddleName"].ToString().Trim();
                user.OrganizationID = _organizationID;
                user.PrimaryGroupID = null;
                user.SubscribeToNewTickets = false;
                user.ReceiveTicketNotifications = true;
                user.Title = row["Title"].ToString().Trim();
                user.EnforceSingleSession = true;
            }
            if (_IsBulk == true) users.BulkSave(); else users.Save();
            _log.AppendMessage(users.Count.ToString() + " Users Imported.");

        }

        private void ImportGroups()
        {
            DataTable table = ReadTable("Groups");
            if (table == null) return;

            Groups groups = new Groups(_loginUser);
            Groups existing = new Groups(_loginUser);
            existing.LoadByOrganizationID(_organizationID);
            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                Group group = existing.FindByImportID(row["GroupID"].ToString().Trim());
                if (group != null) continue;
                group = existing.FindByName(row["Name"].ToString());
                if (group == null)
                {
                    group = groups.AddNewGroup();
                    group.Description = row["Description"].ToString().Trim();
                    group.Name = row["Name"].ToString().Trim();
                    group.OrganizationID = _organizationID;
                    group.ImportID = row["GroupID"].ToString().Trim();
                }
                else
                {
                    group.ImportID = row["GroupID"].ToString().Trim();
                    group.Collection.Save();

                }

            }
            if (_IsBulk == true) groups.BulkSave(); else groups.Save();
            _log.AppendMessage(groups.Count.ToString() + " Groups Imported.");

        }

        private void ImportUserGroups()
        {
            DataTable table = ReadTable("UserGroups");
            if (table == null) return;

            Users users = new Users(_loginUser);
            users.LoadByOrganizationID(_loginUser.OrganizationID, false);
            Groups groups = new Groups(_loginUser);
            groups.LoadByOrganizationID(_loginUser.OrganizationID);

            int count = 0;
            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                Group group = groups.FindByImportID(row["GroupID"].ToString().Trim());
                User user = users.FindByImportID(row["UserID"].ToString().Trim());
                if (user != null && group != null)
                {
                    count++;
                    try
                    {
                        users.AddUserGroup(user.UserID, group.GroupID);
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    _log.AppendError(row, "User Group skipped due to invalid group or user id.");
                }
            }
            _log.AppendMessage(count.ToString() + " User Groups Imported.");

        }

        private void ImportCustomers()
        {
            DataTable table = ReadTable("Customers");
            if (table == null) return;

            Organizations organizations = new Organizations(_loginUser);
            Organizations existing = new Organizations(_loginUser);
            existing.LoadByParentID(_organizationID, false);
            IdList list = GetIdList(existing);
            int count = 0;
            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                //if (list.ContainsKey(row["CustomerID"].ToString().Trim())) continue;
                if (existing.FindByImportID(row["CustomerID"].ToString().Trim()) != null) continue;

                Organization foundOrg = existing.FindByName(row["Name"].ToString());
                if (foundOrg != null)
                {
                    foundOrg.ImportID = row["CustomerID"].ToString().Trim();
                    foundOrg.Collection.Save();
                    continue;
                }

                Organization organization = organizations.AddNewOrganization();
                organization.Description = "";
                organization.ExtraStorageUnits = 0;
                organization.HasPortalAccess = false;
                organization.ImportID = row["CustomerID"].ToString().Trim();
                organization.InActiveReason = "";
                organization.IsActive = bool.Parse(row["IsActive"].ToString().Trim());
                organization.IsCustomerFree = false;
                organization.Name = row["Name"].ToString().Trim();
                organization.ParentID = _organizationID;
                organization.PortalSeats = 0;
                organization.PrimaryUserID = null;
                organization.ProductType = ProductType.Express;
                organization.SystemEmailID = Guid.NewGuid();
                organization.UserSeats = 0;
                organization.NeedsIndexing = true;
                organization.CompanyDomains = row["Domains"].ToString().Trim();
                organization.WebServiceID = Guid.NewGuid();
                organization.Website = row["Website"].ToString().Trim();
                organization.DateCreated = (DateTime)GetDBDate(row["DateCreated"], false);
                organization.SAExpirationDate = GetDBDate(row["ServiceExpiration"], true);
                if (++count % BULK_LIMIT == 0)
                {
                    if (_IsBulk == true) organizations.BulkSave(); else organizations.Save();
                    organizations = new Organizations(_loginUser);
                    GC.WaitForPendingFinalizers();
                }
            }

            if (_IsBulk == true) organizations.BulkSave(); else organizations.Save();
            _log.AppendMessage(count.ToString() + " Customers Imported.");
        }

        private void ImportContacts()
        {
            DataTable table = ReadTable("Contacts");
            if (table == null) return;

            Organization unknown = Organizations.GetUnknownCompany(_loginUser, _organizationID);
            Organizations organizations = new Organizations(_loginUser);
            organizations.LoadByParentID(_organizationID, false);
            IdList idList = GetIdList(organizations);
            Users users = new Users(_loginUser);
            int count = 0;
            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                string customerID = row["CustomerID"].ToString().Trim().ToLower();
                int orgID;
                if (!idList.TryGetValue(customerID, out orgID))
                {
                    orgID = unknown.OrganizationID;
                    _log.AppendError(row, "Company not found: " + customerID);
                }

                /*
                 Users existingUsers = new Users(_loginUser);
                 existingUsers.LoadByEmail(organization.OrganizationID, row["Email"].ToString().Trim());

                 if (existingUsers.Count > 0)
                 {
                   existingUsers[0].ImportID = "[contact]" + row["ContactID"].ToString().Trim();
                   existingUsers.Save();
                   _log.AppendError(row, "Contact skipped due to already exists.");
                   continue;
                 }
                 else
                 {
                   existingUsers = new Users(_loginUser);
                   existingUsers.LoadByName(row["FirstName"].ToString().Trim() + " " + row["LastName"].ToString().Trim(), organization.OrganizationID, false, false, false);
                   if (existingUsers.Count > 0)
                   {
                     existingUsers[0].ImportID = "[contact]" + row["ContactID"].ToString().Trim();
                     existingUsers.Save();
                     _log.AppendError(row, "Contact skipped due to already exists.");
                     continue;
                   }
                 }*/

                User user = users.AddNewUser();
                user.ActivatedOn = DateTime.UtcNow;
                user.CryptedPassword = "";
                user.DeactivatedOn = null;
                user.Email = row["Email"].ToString().Trim();
                user.FirstName = row["FirstName"].ToString().Trim();
                user.ImportID = "[contact]" + row["ContactID"].ToString().Trim();
                user.InOffice = false;
                user.InOfficeComment = "";
                user.IsActive = GetDBBool(row, "IsActive", true);
                user.IsFinanceAdmin = false;
                user.IsPasswordExpired = true;
                user.IsPortalUser = false;
                user.IsSystemAdmin = false;
                user.LastActivity = DateTime.UtcNow;
                user.LastLogin = DateTime.UtcNow;
                user.NeedsIndexing = true;
                user.LastName = row["LastName"].ToString().Trim();
                user.MiddleName = row["MiddleName"].ToString().Trim();
                user.OrganizationID = orgID;
                user.PrimaryGroupID = null;
                user.Title = row["Title"].ToString().Trim();
                user.DateCreated = (DateTime)GetDBDate(row["DateCreated"], false);
                if (++count % BULK_LIMIT == 0)
                {
                    if (_IsBulk == true) users.BulkSave(); else users.Save();
                    users = new Users(_loginUser);
                    GC.WaitForPendingFinalizers();
                }

            }
            _log.AppendMessage(count.ToString() + " Contacts Imported.");
            if (_IsBulk == true) users.BulkSave(); else users.Save();
        }

        private void ImportPrimaryContacts()
        {
            DataTable table = ReadTable("PrimaryContacts");
            if (table == null) return;

            Users users = new Users(_loginUser);
            users.LoadContacts(_loginUser.OrganizationID, false);
            Organizations organizations = new Organizations(_loginUser);
            organizations.LoadByParentID(_loginUser.OrganizationID, false);

            int count = 0;
            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                Organization organization = organizations.FindByImportID(row["CustomerID"].ToString().Trim());
                User user = users.FindByImportID("[contact]" + row["ContactID"].ToString().Trim());
                if (user != null && organization != null)
                {
                    count++;
                    organization.PrimaryUserID = user.UserID;
                }
                else
                {
                    _log.AppendError(row, "Primary contact skipped due to invalid customer or contact id.");
                }
            }
            organizations.Save();
            _log.AppendMessage(count.ToString() + " Primary Contacts Imported.");
        }

        private void ImportProducts()
        {
            DataTable table = ReadTable("Products");
            if (table == null) return;

            Products products = new Products(_loginUser);
            Products existing = new Products(_loginUser);
            existing.LoadByOrganizationID(_organizationID);

            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                Product p = existing.FindByImportID(row["ProductID"].ToString().Trim());
                if (p != null)
                {
                    p.ImportID = row["ProductID"].ToString().Trim();
                    p.Collection.Save();
                }
                else
                {
                    Product product = products.AddNewProduct();
                    product.Description = row["Description"].ToString().Trim();
                    product.ImportID = row["ProductID"].ToString().Trim();
                    product.Name = row["Name"].ToString().Trim();
                    product.OrganizationID = _organizationID;
                }
            }

            if (_IsBulk == true) products.BulkSave(); else products.Save();
            _log.AppendMessage(products.Count.ToString() + " Products Imported.");
        }

        private void ImportProductVersions()
        {
            DataTable table = ReadTable("Versions");
            if (table == null) return;

            Products products = new Products(_loginUser);
            products.LoadByOrganizationID(_organizationID);

            ProductVersionStatuses statuses = new ProductVersionStatuses(_loginUser);
            statuses.LoadAllPositions(_organizationID);

            ProductVersions productVersions = new ProductVersions(_loginUser);
            ProductVersions existing = new ProductVersions(_loginUser);
            existing.LoadByParentOrganizationID(_organizationID);

            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                Product product = products.FindByImportID(row["ProductID"].ToString().Trim());
                if (product == null)
                {
                    _log.AppendError(row, "Version skipped due to missing product.");
                    continue;
                }
                if (existing.FindByImportID(row["VersionID"].ToString().Trim(), product.ProductID) != null) continue;

                ProductVersion productVersion = productVersions.AddNewProductVersion();
                productVersion.Description = row["Description"].ToString().Trim();
                productVersion.ImportID = row["VersionID"].ToString().Trim();
                productVersion.IsReleased = bool.Parse(row["IsReleased"].ToString().Trim());
                productVersion.ProductID = product.ProductID;
                productVersion.ProductVersionStatusID = GetVersionStatus(statuses, row["ProductVersionStatus"].ToString()).ProductVersionStatusID;
                productVersion.ReleaseDate = GetDBDate(row["ReleaseDate"], true);
                productVersion.VersionNumber = row["VersionNumber"].ToString().Trim();
            }

            if (_IsBulk == true) productVersions.BulkSave(); else productVersions.Save();
            _log.AppendMessage(productVersions.Count.ToString() + " Versions Imported.");
        }

        private void ImportAssets()
        {
            DataTable table = ReadTable("Assets");
            if (table == null) return;

            Products products = new Products(_loginUser);
            products.LoadByOrganizationID(_organizationID);
            IdList productIDs = GetIdList(products);

            Assets assets = new Assets(_loginUser);
            int orgCount = 0;
            int prodCount = 0;

            int count = 0;
            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                string organizationID = row["AssignedTo"].ToString().Trim().ToLower();

                Asset asset = assets.AddNewAsset();
                asset.OrganizationID = _organizationID;
                asset.SerialNumber = GetDBString(row["SerialNumber"], 500, true);
                asset.Name = GetDBString(row["Name"], 500, true);
                switch (GetDBString(row["Location"], 0, false).ToLower().Trim())
                {
                    case "assigned": asset.Location = "1"; break;
                    case "warehouse": asset.Location = "2"; break;
                    case "junkyard": asset.Location = "3"; break;
                    default:
                        asset.Location = organizationID != "" ? "1" : "2";
                        break;
                }
                asset.Notes = GetDBString(row["Notes"], 0, true);

                int productID;
                if (productIDs.TryGetValue(row["ProductID"].ToString().Trim().ToLower(), out productID))
                {
                    asset.ProductID = productID;
                }


                asset.WarrantyExpiration = GetDBDate(row["WarrantyExpiration"], true);
                asset.CreatorID = -1;
                asset.ModifierID = -1;
                asset.DateModified = DateTime.UtcNow;
                asset.DateCreated = (DateTime)GetDBDate(row["DateCreated"], false);
                asset.SubPartOf = null;
                asset.Status = GetDBString(row["Status"], 500, true);
                asset.ImportID = row["AssetID"].ToString().Trim();

                if (++count % BULK_LIMIT == 0)
                {
                    if (_IsBulk == true) assets.BulkSave(); else assets.Save();
                    assets = new Assets(_loginUser);
                    GC.WaitForPendingFinalizers();

                }
            }
            if (_IsBulk == true) assets.BulkSave(); else assets.Save();

            _log.AppendMessage(count.ToString() + " Assets Imported.");

            _log.AppendMessage(orgCount.ToString() + " Assets Customers Imported.");
            _log.AppendMessage(prodCount.ToString() + " Assets Products Imported.");

        }

        private void ImportAssetHistory()
        {
            DataTable table = ReadTable("AssetHistory");
            if (table == null) return;

            Users users = new Users(_loginUser);
            users.LoadContactsAndUsers(_organizationID, false);
            IdList userIDs = GetIdList(users);

            Assets assets = new Assets(_loginUser);
            assets.LoadByOrganizationID(_organizationID);
            IdList assetIDs = GetIdList(assets);

            AssetHistory history = new AssetHistory(_loginUser);
            AssetAssignments assignments = new AssetAssignments(_loginUser);

            int count = 0;
            foreach (DataRow row in table.Rows)
            {

                _currentRow = row;
                AssetHistoryItem item = history.AddNewAssetHistoryItem();

                int assetID;
                if (!assetIDs.TryGetValue(row["AssetID"].ToString().ToLower().Trim(), out assetID))
                {
                    _log.AppendMessage("Asset not found: " + row["AssetID"].ToString());

                    continue;
                }

                item.OrganizationID = _organizationID;
                item.AssetID = assetID;
                item.ActionTime = (DateTime?)GetDBDate(row["ActionTime"], true);
                item.DateCreated = item.ActionTime;
                item.ActionDescription = GetDBString(row["Description"], 500, true);

                item.ShippedFrom = _organizationID;
                item.ShippedFromRefType = 9;

                int userID;
                if (userIDs.TryGetValue("[contact]" + row["ShippedTo"].ToString().ToLower().Trim(), out userID))
                {
                    item.ShippedTo = userID;
                }

                item.RefType = 32;
                item.TrackingNumber = GetDBString(row["TrackingNumber"], 200, true);
                item.ShippingMethod = GetDBString(row["ShippingMethod"], 200, true);
                item.ReferenceNum = GetDBString(row["ReferenceNum"], 200, true);
                item.Comments = GetDBString(row["Comments"], 0, true);
                item.DateModified = DateTime.UtcNow;
                if (++count % BULK_LIMIT == 0)
                {
                    if (_IsBulk == true) history.BulkSave(); else history.Save();
                    history = new AssetHistory(_loginUser);
                    GC.WaitForPendingFinalizers();

                }
            }
            if (_IsBulk == true) history.BulkSave(); else history.Save();

            _log.AppendMessage(count.ToString() + " Asset Histories Imported.");

            SqlCommand command = new SqlCommand();
            command.CommandText = @"
INSERT INTO AssetAssignments (HistoryID) 
SELECT ah.HistoryID FROM AssetHistory ah
LEFT JOIN Assets a ON a.AssetID = ah.AssetID
WHERE ah.ShippedTo IS NOT NULL
AND a.ImportID IS NOT NULL
AND ah.DateModified > DATEADD(MINUTE, -120, GETUTCDATE())
AND a.OrganizationID = @OrganizationID
";
            command.Parameters.AddWithValue("OrganizationID", _organizationID);
            SqlExecutor.ExecuteNonQuery(_loginUser, command);


        }

        private void ImportTickets()
        {
            TicketTypes ticketTypes = new TicketTypes(_loginUser);
            ticketTypes.LoadAllPositions(_organizationID);
            foreach (TicketType ticketType in ticketTypes)
            {
                if (_ticketTypeNames.Contains(ticketType.Name)) ImportTicketsByTicketType(ticketType);
            }
        }

        private void ImportTicketsByTicketType(TicketType ticketType)
        {
            if (ticketType == null) return;

            DataTable table = ReadTable(ticketType.Name);
            if (table == null) return;

            Users users = new Users(_loginUser);
            users.LoadContactsAndUsers(_organizationID, false);
            IdList userIDs = GetIdList(users);

            Groups groups = new Groups(_loginUser);
            groups.LoadByOrganizationID(_organizationID);

            Products products = new Products(_loginUser);
            products.LoadByOrganizationID(_organizationID);

            ProductVersions productVersions = new ProductVersions(_loginUser);
            productVersions.LoadByParentOrganizationID(_organizationID);

            TicketStatuses ticketStatuses = new TicketStatuses(_loginUser);
            ticketStatuses.LoadAllPositions(ticketType.TicketTypeID);

            TicketSeverities ticketSeverities = new TicketSeverities(_loginUser);
            ticketSeverities.LoadAllPositions(_organizationID);

            Tickets tickets = new Tickets(_loginUser);

            KnowledgeBaseCategories kbCats = new KnowledgeBaseCategories(_loginUser);
            kbCats.LoadAllCategories(_organizationID);

            int maxTicketNumber = tickets.GetMaxTicketNumber(_organizationID);
            if (maxTicketNumber < 0) maxTicketNumber++;
            int count = 0;

            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                int ticketNumber;

                if (row.Table.Columns.Contains("TicketNumber") && row["TicketNumber"].ToString().Trim() != "")
                {
                    ticketNumber = int.Parse(row["TicketNumber"].ToString());
                    Ticket tempTicket = Tickets.GetTicketByNumber(_loginUser, _organizationID, ticketNumber);
                    if (tempTicket != null)
                    {
                        _log.AppendError(row, "Ticket already exists, skipping.");
                        continue;
                    }
                }
                else
                {
                    ticketNumber = maxTicketNumber + 1;
                }

                maxTicketNumber = Math.Max(ticketNumber, maxTicketNumber);
                int? tempID = null;

                int? userID = GetListID(userIDs, row["UserID"].ToString().Trim());

                Group group = groups.FindByImportID(row["GroupID"].ToString().Trim());
                int? groupID = group == null ? null : (int?)group.GroupID;

                tempID = GetUserOrContact(userIDs, row["CreatorID"].ToString().Trim());
                int creatorID = tempID == null ? -1 : (int)tempID;

                tempID = GetUserOrContact(userIDs, row["ModifierID"].ToString().Trim());
                int modifierID = tempID == null ? -1 : (int)tempID;

                int? closerID = GetUserOrContact(userIDs, row["CloserID"].ToString().Trim());

                Product product = products.FindByImportID(row["ProductID"].ToString().Trim());
                int? productID = product == null ? null : (int?)product.ProductID;

                ProductVersion reported = productVersions.FindByImportID(row["ReportedVersionID"].ToString().Trim());
                int? reportedID = reported == null ? null : (int?)reported.ProductVersionID;

                ProductVersion resolved = productVersions.FindByImportID(row["ResolvedVersionID"].ToString().Trim());
                int? resolvedID = resolved == null ? null : (int?)resolved.ProductVersionID;

                Ticket ticket = tickets.AddNewTicket();
                ticket.CloserID = closerID;
                ticket.CreatorID = creatorID;
                ticket.DateCreated = (DateTime)GetDBDate(row["DateCreated"], false);
                ticket.ModifierID = modifierID;
                ticket.DateModified = (DateTime)GetDBDate(row["DateModified"], false);
                ticket.DateClosed = GetDBDate(row["DateClosed"], true);
                ticket.DueDate = GetDBDate(row, "DueDate", true);
                ticket.TicketSource = GetDBString(row, "Source", 100, true);
                ticket.IsVisibleOnPortal = GetDBBool(row, "VisibleOnPortal", false);
                ticket.GroupID = null;
                ticket.ImportID = row["TicketID"].ToString().Trim();
                ticket.IsKnowledgeBase = row.Table.Columns.Contains("IsKnowledgeBase") ? GetDBBool(row["IsKnowledgeBase"]) : false;
                if (ticket.IsKnowledgeBase)
                {
                    string parentCatName = GetDBString(row["KBParentCatName"], 250, true);
                    string catName = GetDBString(row["KBCatName"], 250, true);
                    KnowledgeBaseCategory cat = null;
                    if (catName != null)
                    {
                        if (parentCatName == null)
                        {
                            cat = kbCats.FindByName(catName, -1);
                            if (cat == null)
                            {
                                // craete cat
                                AddKnowledgeBaseCategory(null, catName);
                                kbCats = new KnowledgeBaseCategories(_loginUser);
                                kbCats.LoadAllCategories(_organizationID);
                                cat = kbCats.FindByName(catName, -1);
                            }
                        }
                        else
                        {
                            KnowledgeBaseCategory parent = kbCats.FindByName(parentCatName, -1);
                            if (parent != null)
                            {
                                cat = kbCats.FindByName(catName, parent.CategoryID);
                            }
                            else
                            {
                                // create parent 
                                KnowledgeBaseCategory parentCat = AddKnowledgeBaseCategory(null, parentCatName);
                                AddKnowledgeBaseCategory(parentCat.CategoryID, catName);
                                kbCats = new KnowledgeBaseCategories(_loginUser);
                                kbCats.LoadAllCategories(_organizationID);
                                cat = kbCats.FindByName(catName, parentCat.CategoryID);
                            }
                        }
                    }

                    if (cat != null)
                    {
                        ticket.KnowledgeBaseCategoryID = cat.CategoryID;
                    }
                }

                ticket.ModifierID = _loginUser.UserID;
                ticket.Name = GetDBString(row["Name"], 250, false);
                ticket.OrganizationID = _organizationID;
                ticket.ParentID = null;
                ticket.ProductID = productID;
                ticket.ReportedVersionID = reportedID;
                ticket.SolvedVersionID = resolvedID;
                ticket.TicketNumber = ticketNumber;
                ticket.TicketSeverityID = GetTicketSeverity(ticketSeverities, row["TicketSeverity"].ToString()).TicketSeverityID;
                ticket.TicketStatusID = GetTicketStatus(ticketStatuses, row["TicketStatus"].ToString(), ticketType).TicketStatusID;

                ticket.TicketTypeID = ticketType.TicketTypeID;
                ticket.UserID = userID;
                ticket.NeedsIndexing = true;
                ticket.GroupID = groupID;

                if (++count % BULK_LIMIT == 0)
                {
                    if (_IsBulk == true) tickets.BulkSave(); else tickets.Save();
                    tickets = new Tickets(_loginUser);
                    GC.WaitForPendingFinalizers();
                    EmailPosts.DeleteImportEmails(_loginUser);

                }
            }
            if (_IsBulk == true) tickets.BulkSave(); else tickets.Save();
            EmailPosts.DeleteImportEmails(_loginUser);

            _log.AppendMessage(count.ToString() + " " + ticketType.Name + " Imported.");

        }

        public KnowledgeBaseCategory AddKnowledgeBaseCategory(int? parentID, string name)
        {
            KnowledgeBaseCategory cat = (new KnowledgeBaseCategories(_loginUser)).AddNewKnowledgeBaseCategory();
            cat.OrganizationID = _organizationID;
            cat.CategoryName = name;
            cat.ParentID = parentID ?? -1;
            cat.Position = GetKnowledgeBaseCategoryMaxPosition(parentID) + 1;
            cat.VisibleOnPortal = true;
            cat.Collection.Save();
            return cat;
        }

        private int GetKnowledgeBaseCategoryMaxPosition(int? parentID)
        {
            parentID = parentID ?? -1;

            KnowledgeBaseCategories cats = new KnowledgeBaseCategories(_loginUser);
            if (parentID < 0) cats.LoadCategories(_organizationID);
            else cats.LoadSubcategories((int)parentID);

            int max = -1;

            foreach (KnowledgeBaseCategory cat in cats)
            {
                if (cat.Position != null && cat.Position > max) max = (int)cat.Position;
            }

            return max;
        }

        private void ImportActions()
        {
            DataTable table = ReadTable("Actions");
            if (table == null) return;

            Users users = new Users(_loginUser);
            users.LoadByOrganizationID(_organizationID, false);

            Users contacts = new Users(_loginUser);
            contacts.LoadByOrganizationID(_organizationID, false);
            IdList contactIDs = GetIdList(contacts);
            contacts = null;
            GC.WaitForPendingFinalizers();

            Tickets tickets = new Tickets(_loginUser);
            tickets.LoadByOrganizationID(_organizationID);

            ActionTypes actionTypes = new ActionTypes(_loginUser);
            actionTypes.LoadAllPositions(_organizationID);


            Actions actions = new Actions(_loginUser);
            int count = 0;
            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                Ticket ticket = tickets.FindByImportID(row["TicketID"].ToString().Trim());
                if (ticket == null)
                {
                    _log.AppendError(row, "Action skipped due to missing ticket");
                    continue;
                }

                string desc = ConvertHtmlLineBreaks(row["Description"].ToString().Trim());
                if (desc == "")
                {
                    _log.AppendError(row, "No Description");
                }


                string creatorString = row["CreatorID"].ToString().Trim();
                int? creatorID = null;
                if (creatorString != "")
                {
                    User creator = users.FindByImportID(creatorString);
                    if (creator == null)
                    {
                        creatorID = GetListID(contactIDs, creatorString);
                    }
                    else
                    {
                        creatorID = creator.UserID;
                    }
                }

                creatorID = creatorID == null ? -1 : creatorID;

                TeamSupport.Data.Action action = actions.AddNewAction();

                action.SystemActionTypeID = GetSystemActionTypeID(row["ActionType"].ToString());
                action.ActionTypeID = GetActionTypeID(actionTypes, row["ActionType"].ToString());
                action.CreatorID = (int)creatorID;
                action.DateCreated = (DateTime)GetDBDate(row["DateCreated"].ToString().Trim(), false);
                action.DateModified = DateTime.UtcNow;
                action.DateStarted = GetDBDate(row["DateStarted"].ToString().Trim(), true);
                action.Description = desc;
                action.ActionSource = "Import";
                action.IsVisibleOnPortal = row["VisibleOnPortal"].ToString().ToLower().IndexOf("t") > -1;
                action.ModifierID = _loginUser.UserID;
                action.Name = "";// GetDBString(row["Name"], 500, false);
                if (action.Name.Length > 499) action.Name = action.Name.Substring(0, 499);
                action.TicketID = ticket.TicketID;
                string importID = GetDBString(row, "ActionID", 250, true);
                action.ImportID = string.IsNullOrWhiteSpace(importID) ? "Unassigned" : importID;
                action.TimeSpent = GetDBInt(row["TimeSpent"], true);
                if (action.IsVisibleOnPortal && !ticket.IsVisibleOnPortal) ticket.IsVisibleOnPortal = true;

                action.Pinned = GetDBBool(row, "IsPinned", false);


                if (++count % BULK_LIMIT == 0)
                {
                    tickets.Save();
                    EmailPosts.DeleteImportEmails(_loginUser);
                    if (_IsBulk == true) actions.BulkSave(); else actions.Save();
                    actions = new Actions(_loginUser);
                    GC.WaitForPendingFinalizers();
                }

            }
            tickets.Save();
            EmailPosts.DeleteImportEmails(_loginUser);
            if (_IsBulk == true) actions.BulkSave(); else actions.Save();
            _log.AppendMessage(count.ToString() + " Actions Imported.");
        }

        private void ImportAttachments()
        {
            DataTable table = ReadTable("Attachments");
            if (table == null) return;

            Tickets tickets = new Tickets(_loginUser);
            tickets.LoadByOrganizationID(_organizationID);
            Actions actions = new Actions(_loginUser);
            actions.LoadByOrganizationID(_organizationID, GetOrganizationActionsFilterByImportIDsList());
            Organizations customers = new Organizations(_loginUser);
            customers.LoadByParentID(_organizationID, false);
            Attachments attachments = new Attachments(_loginUser);

            foreach (DataRow row in table.Rows)
            {
                try
                {

                    string mask = row["FileName"].ToString().Trim();
                    if (mask == "*.*")
                    {
                        string path = Path.Combine(Path.GetDirectoryName(_fileName), row["Folder"].ToString().Trim());
                        if (!Directory.Exists(path))
                        {
                            _log.AppendError(row, "Attachment skipped due to directory does not exist. - " + path);
                            continue;
                        }

                        string[] files = Directory.GetFiles(path);
                        foreach (string file in files)
                        {
                            ImportAttachment(row, file, attachments, actions, tickets, customers);
                        }
                    }
                    else
                    {
                        string folder = row["Folder"].ToString().Trim();
                        string root = Path.GetDirectoryName(_fileName);
                        string sourceFile = root + folder;
                        sourceFile = Path.Combine(sourceFile, mask);
                        ImportAttachment(row, sourceFile, attachments, actions, tickets, customers);
                    }
                }
                catch (Exception ex)
                {
                    _log.AppendError(row, "Attachment Skipped: " + ex.Message + ": " + ex.StackTrace);
                }

            }

            if (_IsBulk == true) attachments.BulkSave(); else attachments.Save();
            _log.AppendMessage(attachments.Count.ToString() + " Attachments Imported.");

        }

        private string GetOrganizationActionsFilterByImportIDsList()
        {
            return System.Configuration.ConfigurationManager.AppSettings["OrganizationActionsFilterByImportIDsList"];
        }

        private void ImportAttachment(DataRow row, string sourceFile, Attachments attachments, Actions actions, Tickets tickets, Organizations customers)
        {
            AttachmentProxy.References refType = AttachmentProxy.References.None;
            int id = -1;
            _currentRow = row;

            if (!File.Exists(sourceFile))
            {
                _log.AppendError(row, "Attachment skipped due to file does not exist.2 - " + sourceFile);
                return;
            }


            string objectID = row["ObjectID"].ToString().Trim();
            if (row["ReferenceObject"].ToString().ToLower().IndexOf("ticket") > -1)
            {
                Ticket ticket = tickets.FindByImportID(objectID);
                if (ticket == null)
                {
                    _log.AppendError(row, "Attachment skipped due to ticket does not exist.");
                    return;
                }

                TeamSupport.Data.Action action = Actions.GetTicketFirstAction(_loginUser, ticket.TicketID);
                if (action == null)
                {
                    _log.AppendError(row, "Attachment skipped due to action description does not exist.");
                    return;
                }
                refType = AttachmentProxy.References.Actions;
                id = action.ActionID;
            }
            else if (row["ReferenceObject"].ToString().ToLower().IndexOf("action") > -1)
            {
                TeamSupport.Data.Action action = actions.FindByImportID(objectID);
                if (action == null)
                {
                    _log.AppendError(row, "Attachment skipped due to action does not exist.");
                    return;
                }
                refType = AttachmentProxy.References.Actions;
                id = action.ActionID;
            }
            else if (row["ReferenceObject"].ToString().ToLower().IndexOf("customer") > -1)
            {
                Organization customer = customers.FindByImportID(objectID);

                if (customer == null)
                {
                    _log.AppendError(row, "Attachment skipped due to customer does not exist.");
                    return;
                }
                refType = AttachmentProxy.References.Organizations;
                id = customer.OrganizationID;
            }
            else
            {
                _log.AppendError(row, "Attachment skipped due to invalid reference object.");
                return;

            }

            string path = Attachments.GetAttachmentPath(_loginUser, refType, id, 3);
            Directory.CreateDirectory(path);
            string ext = Path.GetExtension(sourceFile);
            string fileName = Path.GetFileName(sourceFile);


            try
            {
                FileInfo info = new FileInfo(sourceFile);
                info.CopyTo(Path.Combine(path, fileName));
                Attachment attachment = attachments.AddNewAttachment();
                attachment.FileName = fileName;
                attachment.FileSize = info.Length;
                attachment.FileType = DataUtils.MimeTypeFromFileName(fileName);
                attachment.OrganizationID = _organizationID;
                attachment.Path = Path.Combine(path, fileName);
                attachment.FilePathID = 3;
                attachment.RefID = id;
                attachment.RefType = refType;
            }
            catch (Exception ex)
            {
                _log.AppendError(row, "Attachment skipped due to error: " + ex.Message);

            }

        }

        private void ImportWiki()
        {
            DataTable table = ReadTable("Wiki");
            if (table == null) return;

            Users users = new Users(_loginUser);
            users.LoadContactsAndUsers(_organizationID, false);

            WikiArticles wikis = new WikiArticles(_loginUser);
            Dictionary<string, int> list = new Dictionary<string, int>();
            Dictionary<int, string> parents = new Dictionary<int, string>();

            int count = 0;
            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;

                User creator = users.FindByImportID(row["CreatedBy"].ToString().Trim());
                int? creatorID = creator == null ? null : (int?)creator.UserID;

                User modifier = users.FindByImportID(row["ModifiedBy"].ToString().Trim());
                int? modifierID = modifier == null ? null : (int?)modifier.UserID;

                WikiArticle wiki = wikis.AddNewWikiArticle();
                wiki.ArticleName = GetDBString(row["ArticleName"], 500, true);
                wiki.Body = row["Body"].ToString();
                wiki.CreatedBy = creatorID;
                wiki.CreatedDate = GetDBDate(row["CreatedDate"], true);
                wiki.ModifiedBy = modifierID;
                wiki.ModifiedDate = GetDBDate(row["ModifiedDate"], true);
                wiki.OrganizationID = _organizationID;
                wiki.ParentID = null;
                wiki.PortalEdit = row["PortalEdit"].ToString().ToLower().IndexOf("t") > -1;
                wiki.PortalView = row["PortalView"].ToString().ToLower().IndexOf("t") > -1;
                wiki.Private = row["Private"].ToString().ToLower().IndexOf("t") > -1;
                wiki.PublicEdit = row["PublicEdit"].ToString().ToLower().IndexOf("t") > -1;
                wiki.PublicView = row["PublicView"].ToString().ToLower().IndexOf("t") > -1;
                wiki.NeedsIndexing = true;
                wiki.IsDeleted = row["IsDeleted"].ToString().ToLower().IndexOf("t") > -1;
                wiki.Version = GetDBInt(row["Version"], true) == null ? 1 : GetDBInt(row["Version"], true);
                wiki.Collection.Save();
                list.Add(row["WikiID"].ToString(), wiki.ArticleID);
                parents.Add(wiki.ArticleID, row["ParentID"].ToString());

                count++;
            }

            foreach (WikiArticle wiki in wikis)
            {
                try
                {
                    wiki.ParentID = list[parents[wiki.ArticleID]];
                }
                catch
                {
                }
            }
            wikis.Save();

            _log.AppendMessage(count.ToString() + " Wiki Articles Imported.");


        }

        private void ImportAddresses()
        {
            DataTable table = ReadTable("Addresses");
            if (table == null) return;

            Users users = new Users(_loginUser);
            users.LoadContactsAndUsers(_organizationID, false);
            IdList userList = GetIdList(users);
            users = null;
            GC.WaitForPendingFinalizers();


            Organizations organizations = new Organizations(_loginUser);
            organizations.LoadByParentID(_organizationID, false);
            IdList orgList = GetIdList(organizations);
            organizations = null;
            GC.WaitForPendingFinalizers();


            Addresses addresses = new Addresses(_loginUser);
            int count = 0;
            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                ReferenceType refType;
                string objectID = row["ObjectID"].ToString().Trim().ToLower();
                int refID;

                if (row["ReferenceObject"].ToString().ToLower().IndexOf("user") > -1)
                {
                    refType = ReferenceType.Users;
                    int userID;
                    if (!userList.TryGetValue(objectID, out userID))
                    {
                        _log.AppendError(row, "Address skipped due to missing user.");
                        continue;
                    }
                    refID = userID;
                }
                else if (row["ReferenceObject"].ToString().ToLower().IndexOf("contact") > -1)
                {
                    refType = ReferenceType.Users;
                    int userID;
                    if (!userList.TryGetValue("[contact]" + objectID, out userID))
                    {
                        _log.AppendError(row, "Address skipped due to missing contact.");
                        continue;
                    }
                    refID = userID;
                }
                else
                {
                    refType = ReferenceType.Organizations;
                    int orgID;
                    if (!orgList.TryGetValue(objectID, out orgID))
                    {
                        _log.AppendError(row, "Address skipped due to missing organization.");
                        continue;
                    }

                    refID = orgID;
                }

                Address address = addresses.AddNewAddress();
                address.Addr1 = row["AddressLine1"].ToString().Trim();
                address.Addr2 = row["AddressLine2"].ToString().Trim();
                address.Addr3 = row["AddressLine3"].ToString().Trim();
                address.City = row["City"].ToString().Trim();
                address.Comment = "";
                address.Country = row["Country"].ToString().Trim();
                address.Description = row["Description"].ToString().Trim();
                address.RefID = refID;
                address.RefType = refType;
                address.State = row["State"].ToString().Trim();
                address.Zip = row["Zip"].ToString().Trim();

                if (++count % BULK_LIMIT == 0)
                {
                    if (_IsBulk == true) addresses.BulkSave(); else addresses.Save();
                    addresses = new Addresses(_loginUser);
                    GC.WaitForPendingFinalizers();
                }

            }

            if (_IsBulk == true) addresses.BulkSave(); else addresses.Save();
            _log.AppendMessage(count.ToString() + " Addresses Imported.");

        }

        private void ImportPhoneNumbers()
        {
            DataTable table = ReadTable("PhoneNumbers");
            if (table == null) return;

            Users users = new Users(_loginUser);
            users.LoadContactsAndUsers(_organizationID, false);
            IdList userList = GetIdList(users);
            users = null;
            GC.WaitForPendingFinalizers();

            Organizations organizations = new Organizations(_loginUser);
            organizations.LoadByParentID(_organizationID, false);
            IdList orgList = GetIdList(organizations);
            organizations = null;
            GC.WaitForPendingFinalizers();

            PhoneTypes phoneTypes = new PhoneTypes(_loginUser);
            phoneTypes.LoadAllPositions(_organizationID);


            PhoneNumbers phoneNumbers = new PhoneNumbers(_loginUser);
            int count = 0;
            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                ReferenceType refType;
                string objectID = row["ObjectID"].ToString().Trim().ToLower();
                int refID;

                if (row["ReferenceObject"].ToString().ToLower().IndexOf("user") > -1)
                {
                    refType = ReferenceType.Users;
                    int userID;
                    if (!userList.TryGetValue(objectID, out userID))
                    {
                        _log.AppendError(row, "Phone number skipped due to missing user.");
                        continue;
                    }
                    refID = userID;
                }
                else if (row["ReferenceObject"].ToString().ToLower().IndexOf("contact") > -1)
                {
                    refType = ReferenceType.Users;
                    int userID;
                    if (!userList.TryGetValue("[contact]" + objectID, out userID))
                    {
                        _log.AppendError(row, "Phone number skipped due to missing contact.");
                        continue;
                    }
                    refID = userID;
                }
                else
                {
                    refType = ReferenceType.Organizations;
                    //Organization organization = organizations.FindByImportID(objectID);
                    int orgID;
                    if (!orgList.TryGetValue(objectID, out orgID))
                    {
                        _log.AppendError(row, "Phone number skipped due to missing organization.");
                        continue;
                    }

                    refID = orgID;
                }

                PhoneNumber phoneNumber = phoneNumbers.AddNewPhoneNumber();
                phoneNumber.Extension = row["Extension"].ToString().Trim();
                phoneNumber.OtherTypeName = "";
                phoneNumber.Number = row["PhoneNumber"].ToString().Trim();
                phoneNumber.PhoneTypeID = GetPhoneType(phoneTypes, row["PhoneType"].ToString()).PhoneTypeID;
                phoneNumber.RefID = refID;
                phoneNumber.RefType = refType;

                if (++count % BULK_LIMIT == 0)
                {
                    if (_IsBulk == true) phoneNumbers.BulkSave(); else phoneNumbers.Save();
                    phoneNumbers = new PhoneNumbers(_loginUser);
                    GC.WaitForPendingFinalizers();
                }

            }

            if (_IsBulk == true) phoneNumbers.BulkSave(); else phoneNumbers.Save();
            _log.AppendMessage(count.ToString() + " Phone Numbers Imported.");
        }

        private void ImportNotes()
        {
            DataTable table = ReadTable("CustomerNotes");
            if (table == null) return;

            Users users = new Users(_loginUser);
            users.LoadByOrganizationID(_organizationID, false);

            Organizations organizations = new Organizations(_loginUser);
            organizations.LoadByParentID(_organizationID, false);


            Notes notes = new Notes(_loginUser);

            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                Organization organization = organizations.FindByImportID(row["CustomerID"].ToString().Trim());
                if (organization == null)
                {
                    _log.AppendError(row, "Note skipped due to missing organization.");
                    continue;
                }

                User creator = users.FindByImportID(row["CreatorID"].ToString().Trim());
                int creatorID = creator == null ? _loginUser.UserID : creator.UserID;

                Note note = notes.AddNewNote();
                note.CreatorID = creatorID;
                note.DateCreated = (DateTime)GetDBDate(row["DateCreated"], false);
                note.Description = ConvertHtmlLineBreaks(GetDBString(row["Description"], 0, false));
                note.RefID = organization.OrganizationID;
                note.RefType = ReferenceType.Organizations;
                note.Title = row["Title"].ToString().Trim();
            }

            if (_IsBulk == true) notes.BulkSave(); else notes.Save();
            _log.AppendMessage(notes.Count.ToString() + " Notes Imported.");

        }

        private void ImportCustomerProducts()
        {
            DataTable table = ReadTable("CustomerProducts");
            if (table == null) return;

            Organizations organizations = new Organizations(_loginUser);
            organizations.LoadByParentID(_organizationID, false);

            Products products = new Products(_loginUser);
            products.LoadByOrganizationID(_organizationID);

            ProductVersions productVersions = new ProductVersions(_loginUser);
            productVersions.LoadByParentOrganizationID(_organizationID);

            OrganizationProducts organizationProducts = new OrganizationProducts(_loginUser);
            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                Organization organization = organizations.FindByImportID(row["CustomerID"].ToString().Trim());

                Product product = products.FindByImportID(row["ProductID"].ToString().Trim());

                if (organization == null || product == null)
                {
                    _log.AppendError(row, string.Format("Customer Product skipped due to missing organization or product. [Customer={0}  Product={1}  Version={2}",
                      row["CustomerID"].ToString().Trim(),
                      row["ProductID"].ToString().Trim(),
                      row["VersionID"].ToString().Trim()
                      ));
                    continue;
                }

                ProductVersion version = productVersions.FindByImportID(row["VersionID"].ToString().Trim(), product.ProductID);

                if (version == null && row["VersionID"].ToString().Trim() != "")
                {
                    _log.AppendError(row, string.Format("Customer Product skipped due to missing version. [Customer={0}  Product={1}  Version={2}",
                      row["CustomerID"].ToString().Trim(),
                      row["ProductID"].ToString().Trim(),
                      row["VersionID"].ToString().Trim()
                      ));
                }

                OrganizationProduct organizationProduct = organizationProducts.AddNewOrganizationProduct();
                organizationProduct.ImportID = row["UniqueID"].ToString().Trim();
                organizationProduct.IsVisibleOnPortal = false;
                organizationProduct.OrganizationID = organization.OrganizationID;
                organizationProduct.ProductID = product.ProductID;
                organizationProduct.ProductVersionID = version == null ? null : (int?)version.ProductVersionID;
            }
            if (_IsBulk == true) organizationProducts.BulkSave(); else organizationProducts.Save();

            _log.AppendMessage(organizationProducts.Count.ToString() + " Customer Products Imported.");

        }

        private void ImportCustomerTickets()
        {
            DataTable table = ReadTable("CustomerTickets");
            if (table == null) return;

            Organizations organizations = new Organizations(_loginUser);
            organizations.LoadByParentID(_organizationID, false);
            IdList organizationIDs = GetIdList(organizations);
            organizations = null;
            GC.WaitForPendingFinalizers();

            Tickets tickets = new Tickets(_loginUser);
            tickets.LoadByOrganizationID(_organizationID);
            IdList ticketIDs = GetIdList(tickets);
            tickets = new Tickets(_loginUser);
            GC.WaitForPendingFinalizers();

            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                int ticketID;
                int orgID;

                if (ticketIDs.TryGetValue(row["TicketID"].ToString().Trim().ToLower(), out ticketID))
                {
                    if (organizationIDs.TryGetValue(row["CustomerID"].ToString().Trim().ToLower(), out orgID))
                    {
                        tickets.AddOrganization(orgID, ticketID);
                    }
                    else
                    {
                        _log.AppendError(row, "Customer Ticket skipped due to missing organization.");
                    }
                }
                else
                {
                    _log.AppendError(row, "Customer Ticket skipped due to missing ticket.");
                }
            }
            EmailPosts.DeleteImportEmails(_loginUser);
        }

        private void ImportAssetTickets()
        {
            DataTable table = ReadTable("AssetTickets");
            if (table == null) return;

            Assets assets = new Assets(_loginUser);
            assets.LoadByOrganizationID(_organizationID);
            IdList assetIDs = GetIdList(assets);

            Tickets tickets = new Tickets(_loginUser);
            tickets.LoadByOrganizationID(_organizationID);
            IdList ticketIDs = GetIdList(tickets);

            int count = 0;

            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                int ticketID;
                int assetID;

                if (ticketIDs.TryGetValue(row["TicketID"].ToString().ToLower().Trim(), out ticketID))
                {
                    if (assetIDs.TryGetValue(row["AssetID"].ToString().ToLower().Trim(), out assetID))
                    {
                        tickets.AddAsset(assetID, ticketID);
                        count++;
                    }
                    else
                    {
                        _log.AppendError(row, "Asset Ticket skipped due to missing asset.");
                    }
                }
                else
                {
                    _log.AppendError(row, "Asset Ticket skipped due to missing ticket.");
                }
            }
            EmailPosts.DeleteImportEmails(_loginUser);
            _log.AppendMessage(count.ToString() + " asset and ticket associated.");

        }

        private void ImportParentTickets()
        {
            DataTable table = ReadTable("ParentTickets");
            if (table == null) return;

            Tickets tickets = new Tickets(_loginUser);
            tickets.LoadByOrganizationID(_organizationID);
            IdList ticketIDs = GetIdList(tickets);
            int count = 0;

            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                int parentID;
                if (ticketIDs.TryGetValue(row["ParentTicketID"].ToString().ToLower().Trim(), out parentID))
                {
                    Ticket ticket = tickets.FindByImportID(row["ChildTicketID"].ToString().Trim());
                    if (ticket == null)
                    {
                        _log.AppendError(row, "Parent skipped due to missing child ticket");
                        continue;
                    }

                    ticket.ParentID = parentID;
                    count++;
                }
                else
                {
                    _log.AppendError(row, "Parent Ticket skipped due to missing parent ticket.");
                }

            }
            tickets.Save();
            EmailPosts.DeleteImportEmails(_loginUser);
            _log.AppendMessage(count.ToString() + " child tickets associated.");


        }

        private void ImportTicketSubscriptions()
        {
            DataTable table = ReadTable("TicketSubscriptions");
            if (table == null) return;

            Users users = new Users(_loginUser);
            users.LoadByOrganizationID(_organizationID, false);

            Tickets tickets = new Tickets(_loginUser);
            tickets.LoadByOrganizationID(_organizationID);
            if (table == null) return;

            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                Ticket ticket = tickets.FindByImportID(row["TicketID"].ToString().Trim());
                User user = users.FindByImportID(row["UserID"].ToString().Trim());

                if (user == null || ticket == null)
                {
                    _log.AppendError(row, "Ticket Suscription skipped due to missing user or ticket.");
                    continue;
                }

                tickets.AddSubscription(user.UserID, ticket.TicketID);
                EmailPosts.DeleteImportEmails(_loginUser);

            }
        }

        private void ImportContactTickets()
        {
            DataTable table = ReadTable("ContactTickets");
            if (table == null) return;

            Users users = new Users(_loginUser);
            users.LoadContactsAndUsers(_organizationID, false);
            IdList userIDs = GetIdList(users);
            users = null;
            GC.WaitForPendingFinalizers();

            Tickets tickets = new Tickets(_loginUser);
            tickets.LoadByOrganizationID(_organizationID);
            IdList ticketIDs = GetIdList(tickets);
            tickets = new Tickets(_loginUser);
            GC.WaitForPendingFinalizers();

            foreach (DataRow row in table.Rows)
            {
                int userID;
                int ticketID;
                if (userIDs.TryGetValue("[contact]" + row["ContactID"].ToString().ToLower().Trim(), out userID))
                {
                    if (ticketIDs.TryGetValue(row["TicketID"].ToString().Trim().ToLower(), out ticketID))
                    {
                        tickets.AddContact(userID, ticketID);
                    }
                    else
                    {
                        _log.AppendError(row, "Contact Ticket skipped due to missing ticket.");
                    }
                }
                else
                {
                    _log.AppendError(row, "Contact Ticket skipped due to missing user.");
                }


            }
            EmailPosts.DeleteImportEmails(_loginUser);
        }

        private void ImportCustomFields(ReferenceType refType, List<ImportCustomField> importFields, int auxID)
        {
            CustomValues values = new CustomValues(_loginUser);
            IdList idList = null;
            string fieldName = "";
            string tableName = "";
            string idPrefix = "";

            switch (refType)
            {
                case ReferenceType.OrganizationProducts:
                    OrganizationProducts organizationProducts = new OrganizationProducts(_loginUser);
                    organizationProducts.LoadByParentOrganizationID(_organizationID);
                    idList = GetIdList(organizationProducts);
                    fieldName = "UniqueID";
                    break;
                case ReferenceType.Organizations:
                    Organizations organizations = new Organizations(_loginUser);
                    organizations.LoadByParentID(_organizationID, false);
                    idList = GetIdList(organizations);
                    fieldName = "CustomerID";
                    break;
                case ReferenceType.Products:
                    Products products = new Products(_loginUser);
                    products.LoadByOrganizationID(_organizationID);
                    idList = GetIdList(products);
                    fieldName = "ProductID";
                    break;
                case ReferenceType.ProductVersions:
                    ProductVersions productVersions = new ProductVersions(_loginUser);
                    productVersions.LoadByParentOrganizationID(_organizationID);
                    idList = GetIdList(productVersions);
                    fieldName = "VersionID";
                    break;
                case ReferenceType.Tickets:
                    Tickets tickets = new Tickets(_loginUser);
                    tickets.LoadByOrganizationID(_organizationID);
                    idList = GetIdList(tickets);
                    fieldName = "TicketID";
                    break;
                case ReferenceType.Users:
                    Users users = new Users(_loginUser);
                    users.LoadContactsAndUsers(_organizationID, false);
                    idList = GetIdList(users);
                    fieldName = "UserID";
                    break;
                case ReferenceType.Contacts:
                    Users contacts = new Users(_loginUser);
                    contacts.LoadContactsAndUsers(_organizationID, false);
                    idList = GetIdList(contacts);
                    fieldName = "ContactID";
                    idPrefix = "[contact]";
                    break;
                case ReferenceType.Assets:
                    Assets assets = new Assets(_loginUser);
                    assets.LoadByOrganizationID(_organizationID);
                    idList = GetIdList(assets);
                    fieldName = "AssetID";
                    break;
                default:
                    break;
            }

            foreach (ImportCustomField icf in importFields)
            {
                if (icf.RefType == refType && icf.AuxID == auxID)
                {
                    tableName = icf.TableName;
                    break;
                }
            }

            if (tableName == "" || fieldName == "") return;
            DataTable table = ReadTable(tableName);
            int count = 0;
            foreach (DataRow row in table.Rows)
            {
                _currentRow = row;
                foreach (ImportCustomField importField in importFields)
                {
                    if (importField.TableName == tableName)
                    {
                        try
                        {

                            string itemValue = row[importField.FieldName].ToString().Trim();
                            if (itemValue != "")
                            {
                                int id;
                                if (idList.TryGetValue(idPrefix + row[fieldName].ToString().Trim().ToLower(), out id))
                                {

                                    /*CustomValue value = values.AddNewCustomValue();
                                    value.CustomFieldID = importField.TSFieldID;
                                    value.Value = itemValue;
                                    value.RefID = id;*/
                                    try
                                    {
                                        CustomValues.UpdateValue(_loginUser, importField.TSFieldID, id, itemValue);
                                        //values.Save();
                                    }
                                    catch (Exception ex)
                                    {
                                        _log.AppendError(row, string.Format("CustomField Skippedskipped. [Table={0}  CustomFieldID={1}  RefID={2}  Error={3}  Stack={4}",
                                                    tableName,
                                                    importField.TSFieldID.ToString(),
                                                    id.ToString(),
                                                    ex.Message,
                                                    ex.StackTrace
                                                    ));
                                    }
                                    count++;

                                    /*if (count % BULK_LIMIT == 0)
                                    {
                                      values.BulkSave();
                                      values = new CustomValues(_loginUser);
                                      GC.WaitForPendingFinalizers();
                                    }*/

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.AppendError(row, string.Format("CustomField Skipped skipped. [Table={0}  CustomFieldID={1}  RefID={2}  Error={3}  Stack={4}",
                                        tableName,
                                        importField.TSFieldID.ToString(),
                                        "",
                                        ex.Message,
                                        ex.StackTrace
                                        ));
                        }
                    }
                }
            }

            //values.BulkSave();
            _log.AppendMessage(count.ToString() + " Custom Fields Imported for " + tableName);

        }

        private void ImportCustomFields()
        {

            _importCustomFields = new List<ImportCustomField>();
            CreateAllCustomFields();

            ImportCustomFields(ReferenceType.OrganizationProducts, _importCustomFields, -1);
            ImportCustomFields(ReferenceType.Organizations, _importCustomFields, -1);
            ImportCustomFields(ReferenceType.Products, _importCustomFields, -1);
            ImportCustomFields(ReferenceType.ProductVersions, _importCustomFields, -1);
            ImportCustomFields(ReferenceType.Contacts, _importCustomFields, -1);
            ImportCustomFields(ReferenceType.Users, _importCustomFields, -1);
            ImportCustomFields(ReferenceType.Assets, _importCustomFields, -1);

            TicketTypes ticketTypes = new TicketTypes(_loginUser);
            ticketTypes.LoadAllPositions(_organizationID);
            foreach (TicketType ticketType in ticketTypes)
            {
                ImportCustomFields(ReferenceType.Tickets, _importCustomFields, ticketType.TicketTypeID);
            }


        }

        #endregion

    }
}
