using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;
using System.Security;
using System.Dynamic;
using Newtonsoft.Json;

namespace TeamSupport.Data
{
    public class BaseItem
    {
        public BaseItem(DataRow row, BaseCollection baseCollection)
        {
            _row = row;
            _baseCollection = baseCollection;
        }

        private BaseCollection _baseCollection;

        public BaseCollection BaseCollection
        {
            get { return _baseCollection; }
            set { _baseCollection = value; }
        }

        protected object CheckValue(string columnName, object value)
        {
            DataColumn column = Row.Table.Columns[columnName];
            if (column.DataType == typeof(String) && value != null)
            {
                if (value.GetType() != typeof(String)) return "";
                string s = (string)value;
                if (s.Length >= column.MaxLength && column.MaxLength > 0)
                {
                    value = s.Substring(0, column.MaxLength - 1);
                }
            }
            return value == null ? DBNull.Value : value;
        }

        public int PrimaryKeyID
        {
            get { return (int)Row[_baseCollection.PrimaryKeyFieldName]; }
        }

        public ReferenceType ItemReferenceType
        {
            get
            {
                ReferenceType result = DataUtils.TableNameToReferenceType(_baseCollection.TableName);
                if (result == ReferenceType.Users)
                {
                    int organizationID = (int)Row["OrganizationID"];
                    if (organizationID != _baseCollection.LoginUser.OrganizationID) result = ReferenceType.Contacts;
                }
                return result;
            }
        }

        public int ItemAuxID
        {
            get
            {
                int result = -1;
                if (ItemReferenceType == ReferenceType.Tickets)
                {
                    result = (int)Row["TicketTypeID"];
                }
                return result;
            }
        }

        public CustomFields CustomFields
        {
            get
            {
                CustomFields result = _baseCollection.CustomFields;
                if (result == null)
                {
                    result = new CustomFields(_baseCollection.LoginUser);
                    Organization organization = Organizations.GetOrganization(_baseCollection.LoginUser, _baseCollection.LoginUser.OrganizationID);
                    int orgID = organization.OrganizationID;
                    if (organization.ParentID != 1)
                    {
                        orgID = (int)organization.ParentID;
                    }
                    result.LoadByReferenceType(orgID, ItemReferenceType, ItemAuxID);
                }
                return result;
            }
        }

        public void Delete()
        {
            _row.Delete();
        }

        public DateTime DateToLocal(DateTime dateTime)
        {
            return DataUtils.DateToLocal(_baseCollection.LoginUser, dateTime);
        }

        public DateTime? DateToLocal(DateTime? dateTime)
        {
            return DataUtils.DateToLocal(_baseCollection.LoginUser, dateTime);
        }

        public DateTime DateToUtc(DateTime dateTime)
        {
            return DataUtils.DateToUtc(_baseCollection.LoginUser, dateTime);
        }

        public DateTime? DateToUtc(DateTime? dateTime)
        {
            return DataUtils.DateToUtc(_baseCollection.LoginUser, dateTime);
        }

        public void WriteToXml(XmlWriter writer, bool includeCustomFields, Tags tags = null)
        {
            foreach (FieldMapItem item in _baseCollection.FieldMap)
            {
				//hack: Adding this hack (second IF condition -the OR-) to allow the CryptedPassword to be displayed ONLY for this org. It is not available for any other. The plan is to have this as an option in the system but for now we need this asap.
				if (item.Select
					|| (!item.Select && _baseCollection.LoginUser.OrganizationID == 931810 && item.PrivateName == "CryptedPassword"))
                {
                    string s = "";
                    DataColumn column = Row.Table.Columns[item.PrivateName];
                    if (Row[item.PrivateName] != DBNull.Value)
                    {
                        if (column.DataType == typeof(System.DateTime))
                        {
                            s = DateToLocal((DateTime)Row[item.PrivateName]).ToString("g", _baseCollection.LoginUser.CultureInfo);
                        }
                        else
                        {
                            s = Row[item.PrivateName].ToString();
                        }
                    }
                    string escape = SecurityElement.Escape(s).Replace(Convert.ToChar(0x0).ToString(), "").Replace(Convert.ToChar(0x1C).ToString(), "");
                    writer.WriteElementString(item.PublicName, escape);
                }
            }

            // Ticket 15640
            // Per a skype conversation with Jesus adding AMCO Sales (797841)
            if ((_baseCollection.LoginUser.OrganizationID == 566596 || _baseCollection.LoginUser.OrganizationID == 797841) && _baseCollection.TableName == "TicketsView")
            {
                Organizations customers = new Organizations(_baseCollection.LoginUser);
                customers.LoadNameAndIdByTicketID((int)Row["TicketID"]);
                string customerID = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {
                    // Per a skype conversation with Jesus their _Unknown Company needs to be excluded. 
                    if (customers[i].OrganizationID != 624447)
                    {
                        customerID = customers[i].OrganizationID.ToString();
                        break;
                    }
                }
                writer.WriteElementString("CustomerID", customerID);
            }
            else if (_baseCollection.TableName == "TicketsView")
            {
                Organizations customers = new Organizations(_baseCollection.LoginUser);
                customers.LoadNameAndIdByTicketID((int)Row["TicketID"]);
                string customerID = string.Empty;

                writer.WriteStartElement("Customers");

                for (int i = 0; i < customers.Count; i++)
                {
                    customerID = customers[i].OrganizationID.ToString();
                    writer.WriteStartElement("Customer");
                    writer.WriteElementString("CustomerID", customerID);
                    writer.WriteElementString("CustomerName", customers[i].Name);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();

                ContactsView contacts = new ContactsView(_baseCollection.LoginUser);
                contacts.LoadNameAndIdByTicketID((int)Row["TicketID"]);
                string contactId = string.Empty;

                writer.WriteStartElement("Contacts");

                for (int i = 0; i < contacts.Count; i++)
                {
                    contactId = contacts[i].UserID.ToString();
                    writer.WriteStartElement("Contact");
                    writer.WriteElementString("ContactID", contactId);
                    writer.WriteElementString("ContactName", contacts[i].Name);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            if (includeCustomFields)
            {
                foreach (CustomField field in CustomFields)
                {
                    string s = "";
                    if (Row.Table.Columns.Contains(field.ApiFieldName))
                    {
                        if (Row[field.ApiFieldName] != DBNull.Value)
                        {
                            DataColumn column = Row.Table.Columns[field.ApiFieldName];
                            if (column.DataType == typeof(System.DateTime) || field.FieldType == CustomFieldType.Date)
                            {
								DateTime fieldDateTime = DateTime.Parse(Row[field.ApiFieldName].ToString());
								s = DateToLocal(fieldDateTime).ToString("g", _baseCollection.LoginUser.CultureInfo);
                            }
							else if (column.DataType == typeof(System.Boolean) || field.FieldType == CustomFieldType.Boolean)
							{
								bool flag = false;
								int flagInt = 0;

								s = flag.ToString();

								if (Boolean.TryParse(Row[field.ApiFieldName].ToString(), out flag))
								{
									s = flag.ToString();
								}
								else if (int.TryParse(Row[field.ApiFieldName].ToString(), out flagInt))
								{
									s = (flagInt > 0).ToString();
								}
							}
							else
                            {
                                s = Row[field.ApiFieldName].ToString();
                            }
                        }
                    }
                    else
                    {
                        object value = field.GetValue(PrimaryKeyID);
                        if (value != null)
                        {
                            s = value.ToString();
                        }
                    }
                    string escape = SecurityElement.Escape(s).Replace(Convert.ToChar(0x0).ToString(), "").Replace(Convert.ToChar(0x1C).ToString(), "");
                    writer.WriteElementString(field.ApiFieldName, escape);
                }
            }

			if (tags != null)
			{
				writer.WriteStartElement("Tags");

				foreach(Tag tag in tags)
				{
					writer.WriteStartElement("Tag");
					writer.WriteElementString("Value", tag.Value);
					writer.WriteEndElement();
				}
				
				writer.WriteEndElement();
			}
        }

        public void UpdateCustomFieldsFromXml(string data)
        {
            StringReader reader = new StringReader(data);
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(reader);
            DataRow row = dataSet.Tables[0].Rows[0];

            foreach (DataColumn column in dataSet.Tables[0].Columns)
            {
                foreach (CustomField field in CustomFields)
                {
                    if (field.ApiFieldName.ToLower() == column.ColumnName.ToLower())
                    {
                        bool isValid = false;

                        try
                        {
                            isValid = IsValid(row[column], field.FieldType, field.IsRequired);
                        }
                        catch (Exception ex)
                        {
                            isValid = true;
                            //if the validation check threw an exception then set the value anyway, basically what was done before, and log the exception
                            string columnName = string.Empty;
                            string value = string.Empty;

                            if (column != null)
                            {
                                columnName = column.ColumnName;

                                if (row != null)
                                {
                                    value = row[column].ToString();
                                }
                            }

                            ExceptionLogs.LogException(_baseCollection.LoginUser,
                                                      ex,
                                                      "BaseCollection.UpdateCustomFieldsFromXml",
                                                      string.Format("Validating {0} with value {1}",
                                                                    string.IsNullOrEmpty(columnName) ? "a field" : columnName,
                                                                    string.IsNullOrEmpty(value) ? "{ex.found}" : value));
                        }

                        if (isValid)
                        {
                            field.SetValue(PrimaryKeyID, row[column]);
                        }

                        break;
                    }
                }
            }
        }

        private bool IsValid(object customFieldValue, CustomFieldType customFieldType, bool isRequired)
        {
            bool isValid = false;

            if (isRequired && string.IsNullOrEmpty(customFieldValue.ToString()))
            {
                isValid = false;
            }
            else
            {
                switch (customFieldType)
                {
                    case CustomFieldType.Number:
                        int intValue = 0;
                        isValid = int.TryParse(customFieldValue.ToString(), out intValue);
                        break;
                    default:
                        isValid = true;
                        break;
                }
            }

            return isValid;
        }

        public void ReadFromXml(string data, bool isInsert, bool allowOrganizationUpdateFromAPI = false)
        {
            StringReader reader = new StringReader(data);
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(reader);
            DataRow row = dataSet.Tables[0].Rows[0];

            foreach (DataColumn column in dataSet.Tables[0].Columns)
            {
                foreach (FieldMapItem item in _baseCollection.FieldMap)
                {
                    if (isInsert)
                    {
                        if (!item.Insert) continue;
                    }
                    else
                    {
                        bool allowOrganizationUpdate = allowOrganizationUpdateFromAPI && item.PublicName == "OrganizationID";

                        if (!item.Update && !allowOrganizationUpdate)
                        {
                            continue;
                        }
                    }

                    string columnName = column.ColumnName.ToLower();

                    if (item.PublicName.ToLower() == columnName || item.PublicName.ToLower() + "_id" == columnName)
                    {
                        if (row[column] == DBNull.Value || row[column].ToString().Trim() == "")
                        {
                            if (Row.Table.Columns[item.PrivateName].DataType == typeof(System.String))
                                Row[item.PrivateName] = "";
                            else
                                Row[item.PrivateName] = DBNull.Value;
                        }
                        else
                        {
                            Type dataType = Row.Table.Columns[item.PrivateName].DataType;
                            string value = row[column].ToString();

                            if (dataType == typeof(System.DateTime)) Row[item.PrivateName] = DateTime.Parse(value, _baseCollection.LoginUser.CultureInfo);
                            else if (dataType == typeof(System.Boolean)) Row[item.PrivateName] = bool.Parse(value);
                            else if (dataType == typeof(System.Int32)) Row[item.PrivateName] = int.Parse(value);
                            else if (dataType == typeof(System.Double)) Row[item.PrivateName] = double.Parse(value);
                            else Row[item.PrivateName] = row[column];
                        }
                        break;
                    }
                }
            }
        }

        public void WriteToJson(bool includeCustomFields)
        {
        }

        public string GetXml(string elementName, bool includeCustomFields, Tags tags = null)
        {
            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, new UTF8Encoding(false));
            writer.Formatting = System.Xml.Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement(elementName);

            WriteToXml(writer, includeCustomFields, tags);

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public string GetJson(bool includeCustomFields)
        {
            return "";
        }

        public ExpandoObject GetExpandoObject()
        {
            return DataUtils.DataRowToExpandoObject(Row);

        }

        public void CopyRowData(BaseItem item)
        {
            CopyRowData(item.Row);
        }

        public void CopyRowData(DataRow source)
        {
            foreach (DataColumn col in Row.Table.Columns)
            {
                switch (col.ColumnName)
                {
                    case "ModifierID": Row[col.ColumnName] = BaseCollection.LoginUser.UserID; break;
                    case "CreatorID": Row[col.ColumnName] = BaseCollection.LoginUser.UserID; break;
                    case "DateModified": Row[col.ColumnName] = DateTime.UtcNow; break;
                    case "DateCreated": Row[col.ColumnName] = DateTime.UtcNow; break;
                    default:
                        if (source.Table.Columns.Contains(col.ColumnName))
                        {
                            Row[col.ColumnName] = source[col.ColumnName];
                        }


                        break;
                }
            }



        }

        #region Properties

        private DataRow _row;
        public DataRow Row
        {
            get { return _row; }
        }

		public int TotalRecords
		{
			get
			{
				string apiTotalRecordsElementName = "TotalRecords";

				if (Row.Table.Columns.Contains(apiTotalRecordsElementName) && Row[apiTotalRecordsElementName] != DBNull.Value)
				{
                    int totalRecords = 0;

                    try
                    {
                        totalRecords = (int)Row[apiTotalRecordsElementName];
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("cast") && ex.Message.Contains("not valid"))
                        {
                            string totalRecordsString = Convert.ToString(Row[apiTotalRecordsElementName]);

                            if (!string.IsNullOrEmpty(totalRecordsString))
                            {
                                if (!int.TryParse(totalRecordsString, out totalRecords))
                                {
                                    totalRecords = 0;
                                }
                            }
                        }
                    }

                    return totalRecords;
				}
				else return 0;
			}
		}

		#endregion
	}

	public abstract class BaseCollection : IDisposable
	{
		public BaseCollection(LoginUser loginUser)
		{
			_loginUser = loginUser;
			_table = new DataTable();
			_deadlockCount = 0;
		}

		#region Private Members

		private int _deadlockCount = 0;

		#endregion

		#region Properties

		private LoginUser _loginUser;
		public LoginUser LoginUser
		{
			get { return _loginUser; }
			set { _loginUser = value; }
		}

		private int _cacheExpirationSeconds = 120;
		public int CacheExpirationSeconds
		{
			get { return _cacheExpirationSeconds; }
			set { _cacheExpirationSeconds = value; }
		}

		private bool _useCache = false;
		public bool UseCache
		{
			get { return _useCache; }
			set { _useCache = value; }
		}

		private CustomFields _customFields = null;
		public CustomFields CustomFields
		{
			get { return _customFields; }
			set { _customFields = value; }
		}

		public IDataCache DataCache
		{
			get { return _loginUser.DataCache; }
		}

		private DataTable _table = null;
		public DataTable Table
		{
			get { return _table; }
			set { _table = value; }
		}

		public int Count
		{
			get { return _table.Rows.Count; }
		}

		public bool IsEmpty
		{
			get { return Count < 1; }
		}

		public abstract string TableName
		{
			get;
		}

		public abstract string PrimaryKeyFieldName
		{
			get;
		}

		public virtual void Save()
		{

			using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
			{
				connection.Open();
				try
				{
					Save(connection);
					_deadlockCount = 0;
				}
				catch (SqlException ex)
				{
					if (ex.Number == 1205 && _deadlockCount < 3)
					{
						_deadlockCount++;
						connection.Close();
						System.Threading.Thread.Sleep(10000);
						Save();
						return;
					}
					else
					{
						throw ex;
					}
				}
				finally
				{
					connection.Close();
				}
			}

		}

		protected bool TryDeleteFromDB(SqlCommand command)
		{
			int deadlockCount = 0;
			while (deadlockCount < 5)
			{
				using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
				{
					connection.Open();
					command.Connection = connection;
					try
					{
						command.ExecuteNonQuery();
						return true;
					}
					catch (SqlException ex)
					{
						if (ex.Number == 1205)
						{
							deadlockCount++;
							System.Threading.Thread.Sleep(5000);
						}
						else
						{
							throw ex;
						}
					}
				}
			}
			return false;
		}

		public abstract void Save(SqlConnection connection);

		protected abstract void BuildFieldMap();


		#endregion

		#region Protected Members

		protected FieldMap _fieldMap;

		protected void LoadColumns(string tableName)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "SELECT * FROM " + tableName + " WHERE 0=1";
				command.CommandType = CommandType.Text;
				Fill(command);
				_table.PrimaryKey = null;

				foreach (DataColumn column in _table.Columns)
				{
					if (!column.AllowDBNull && !column.ReadOnly)
					{
						if (column.DataType == typeof(System.DateTime)) column.DefaultValue = DateTime.UtcNow;
						else if (column.DataType == typeof(System.String)) column.DefaultValue = "";
						else if (column.DataType == typeof(System.Boolean)) column.DefaultValue = false;
						else if (column.DataType == typeof(System.Guid)) column.DefaultValue = Guid.NewGuid();
						else column.DefaultValue = 0;
					}
					if (column.AutoIncrement)
					{
						column.AutoIncrement = false;
						column.ReadOnly = false;
						column.AllowDBNull = true;
						column.Unique = false;
					}
				}
			}
		}

		public virtual void ExecuteNonQuery(SqlCommand command)
		{
			ExecuteNonQuery(command, "");
		}

		public virtual void ExecuteNonQuery(SqlCommand command, string tableName)
		{
			FixCommandParameters(command);
			using (SqlConnection connection = new SqlConnection(_loginUser.ConnectionString))
			{
				//SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Snapshot);
				connection.Open();
				command.Connection = connection;
				//command.Transaction = transaction;
				try
				{
					command.ExecuteNonQuery();
					//transaction.Commit();
				}
				catch (Exception e)
				{
					//transaction.Rollback();
					e.Data["CommandText"] = command.CommandText;
					ExceptionLogs.LogException(LoginUser, e, "BaseCollection.ExecuteNonQuery");
					throw;
				}
				connection.Close();
			}

			if (DataCache != null) DataCache.InvalidateItem(tableName, _loginUser.OrganizationID);
		}

		public virtual object ExecuteScalar(SqlCommand command)
		{
			return ExecuteScalar(command, "");
		}

		public virtual object ExecuteScalar(SqlCommand command, string tableNames)
		{
			FixCommandParameters(command);
			using (SqlConnection connection = new SqlConnection(_loginUser.ConnectionString))
			{
				object o;
				if (_useCache && DataCache != null)
				{
					o = DataCache.GetScalar(command);
					if (o != null) return o;
				}

				connection.Open();
				SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
				command.Connection = connection;
				command.Transaction = transaction;
				try
				{
					o = command.ExecuteScalar();
					transaction.Commit();
				}
				catch (Exception e)
				{
					transaction.Rollback();
					e.Data["CommandText"] = command.CommandText;
					ExceptionLogs.LogException(LoginUser, e, "BaseCollection.ExecuteScalar");
					throw;
				}

				connection.Close();
				if (DataCache != null)
				{
					if (tableNames == "") tableNames = TableName;
					DataCache.AddScalar(command, tableNames, _cacheExpirationSeconds, o, _loginUser.OrganizationID);
				}
				return o;
			}
		}

		public void Fill(SqlCommand command, SqlConnection connection)
		{
			FixCommandParameters(command);

			if (_table != null) _table.Dispose();
			_table = new DataTable();

			command.Connection = connection;
			using (SqlDataAdapter adapter = new SqlDataAdapter(command))
			{
				adapter.FillSchema(_table, SchemaType.Source);
				adapter.Fill(_table);
			}
		}

		public virtual void Fill(SqlCommand command, string tableNames, bool includeSchema = true)
		{
			FixCommandParameters(command);

			if (_table != null) _table.Dispose();
			if (_useCache && DataCache != null)
			{
				_table = DataCache.GetTable(command);
				if (_table != null) return;
			}
			_table = new DataTable();


			using (SqlConnection connection = new SqlConnection(_loginUser.ConnectionString))
			{
				connection.Open();
				SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);

				command.Connection = connection;
				command.Transaction = transaction;
				try
				{
					using (SqlDataAdapter adapter = new SqlDataAdapter(command))
					{
						if (includeSchema) adapter.FillSchema(_table, SchemaType.Source);
						adapter.Fill(_table);
					}
					transaction.Commit();
				}
				catch (Exception e)
				{
					transaction.Rollback();
					e.Data["CommandText"] = command.CommandText;
					ExceptionLogs.LogException(LoginUser, e, "BaseCollection.Fill");
					throw;
				}
				connection.Close();
			}

			if (DataCache != null)
			{
				if (tableNames == "") tableNames = TableName;
				DataCache.AddTable(command, tableNames, _cacheExpirationSeconds, _table, _loginUser.OrganizationID);
			}
		}

		public static void FixCommandParameters(SqlCommand command)
		{
			return;
			foreach (SqlParameter parameter in command.Parameters)
			{
				if (parameter.SqlDbType == SqlDbType.NVarChar)
				{
					parameter.SqlDbType = SqlDbType.VarChar;
				}
			}
		}

		protected virtual void Fill(SqlCommand command)
		{
			Fill(command, "");
		}
		#endregion

		#region Public Members

		public FieldMap FieldMap
		{
			get { if (_fieldMap == null) BuildFieldMap(); return _fieldMap; }
		}

		public string InjectCustomFields(string text, string refIDFieldName, ReferenceType refType, int? auxID)
		{
			int i = text.ToLower().IndexOf(" from");
			if (i > -1)
			{
				return text.Insert(i, GetCustomFieldsSelect(refType, auxID, refIDFieldName));
			}
			return text;
		}

		public string InjectCustomFields(string text, string refIDFieldName, ReferenceType refType)
		{
			return InjectCustomFields(text, refIDFieldName, refType, null);
		}

		public string GetCustomFieldsSelect(ReferenceType refType, string refIDFieldName)
		{
			return GetCustomFieldsSelect(refType, null, refIDFieldName);
		}

		public string GetCustomFieldsSelect(ReferenceType refType, int? auxID, string refIDFieldName)
		{
			CustomFields = new CustomFields(LoginUser);
			int orgID = LoginUser.OrganizationID;
			Organization organization = Organizations.GetOrganization(LoginUser, LoginUser.OrganizationID);
			if (organization != null && organization.ParentID != 1) { orgID = (int)organization.ParentID; }
			_customFields.LoadByReferenceType(orgID, refType, auxID);
			return GetCustomFieldsSelect(CustomFields, refIDFieldName);
		}

		public static string GetCustomFieldsSelect(CustomFields fields, string refIDFieldName)
		{
			StringBuilder builder = new StringBuilder();

			foreach (CustomField customField in fields)
			{
				builder.Append(",");
				builder.Append(GetCustomFieldSelect(customField, refIDFieldName, customField.ApiFieldName));
			}
			return builder.ToString();
		}

		public static string GetCustomFieldSelect(CustomField field, string refIDFieldName, string fieldAlias)
		{
			switch (field.FieldType)
			{
				case CustomFieldType.Date:
				case CustomFieldType.Time:
				case CustomFieldType.DateTime: return GetCustomFieldDateSelect(field, refIDFieldName, fieldAlias);
				case CustomFieldType.Boolean: return GetCustomFieldBooleanSelect(field, refIDFieldName, fieldAlias);
				case CustomFieldType.Number: return GetCustomFieldNumberSelect(field, refIDFieldName, fieldAlias);
				default:
					break;
			}


			StringBuilder builder = new StringBuilder();
			builder.Append("(SELECT NULLIF(RTRIM(CustomValue), '') FROM CustomValues WHERE (CustomFieldID = ");
			builder.Append(field.CustomFieldID.ToString());
			builder.Append(") AND (RefID = ");
			builder.Append(refIDFieldName);
			builder.Append(")) ");


			//"(SELECT CustomValue FROM CustomValues WHERE (CustomFieldID = 654) AND (RefID = TicketsView.TicketID)) AS [Approved By Manager]"
			if (field.FieldType == CustomFieldType.PickList)
			{
				string[] items = field.ListValues.Split('|');
				if (items.Length > 0)
				{
					builder.Insert(0, "ISNULL(");
					builder.Append(", '" + items[0].Replace("'", "''") + "')");
				}
			}

			builder.Append("AS [");
			builder.Append(fieldAlias);
			builder.Append("]");
			return builder.ToString();
		}

		public static string GetCustomFieldDateSelect(CustomField field, string refIDFieldName, string fieldAlias)
		{
			string sql = @"
(
  CASE 
    WHEN ISDATE((SELECT NULLIF(RTRIM(CustomValue), '') FROM CustomValues WHERE (CustomFieldID = {0}) AND (RefID = {1}))) = 1  
    THEN (SELECT NULLIF(RTRIM(CustomValue), '') FROM CustomValues WHERE (CustomFieldID = {0}) AND (RefID = {1}))
    ELSE NULL
  END
) AS [{2}]
";
			return string.Format(sql, field.CustomFieldID.ToString(), refIDFieldName, fieldAlias);
		}

		public static string GetCustomFieldNumberSelect(CustomField field, string refIDFieldName, string fieldAlias)
		{
			string sql = @"
(
  CASE 
    WHEN ISNUMERIC((SELECT NULLIF(RTRIM(CustomValue), '') FROM CustomValues WHERE (CustomFieldID = {0}) AND (RefID = {1}))) = 1  
    THEN (SELECT NULLIF(RTRIM(CustomValue), '') FROM CustomValues WHERE (CustomFieldID = {0}) AND (RefID = {1}))
    ELSE NULL
  END
) AS [{2}]
";
			return string.Format(sql, field.CustomFieldID.ToString(), refIDFieldName, fieldAlias);
		}

		public static string GetCustomFieldBooleanSelect(CustomField field, string refIDFieldName, string fieldAlias)
		{
			string sql = @"
(
  CASE 
    WHEN (SELECT NULLIF(RTRIM(CustomValue), '') FROM CustomValues WHERE (CustomFieldID = {0}) AND (RefID = {1})) = 'true'  THEN '1'
    WHEN (SELECT NULLIF(RTRIM(CustomValue), '') FROM CustomValues WHERE (CustomFieldID = {0}) AND (RefID = {1})) = 'false'  THEN '0'
    ELSE NULL
  END 
) AS [{2}]
";
			return string.Format(sql, field.CustomFieldID.ToString(), refIDFieldName, fieldAlias);
		}

		public void DeleteAll()
		{
			foreach (DataRow row in Table.Rows)
			{
				row.Delete();
			}
		}

		public void Delete(BaseItem baseItem)
		{
			baseItem.Row.Delete();
		}
		/*public string GetXml(string listName, string elementName, bool includeCustomFields)
        {
          return GetXml(listName, elementName, includeCustomFields, null);
        }*/


		public static XmlTextWriter BeginXmlWrite(string listName)
		{
			MemoryStream stream = new MemoryStream();
			XmlTextWriter writer = new XmlTextWriter(stream, new UTF8Encoding(false));
			writer.Formatting = System.Xml.Formatting.Indented;
			writer.WriteStartDocument();
			writer.WriteStartElement(listName);
			return writer;
		}

		public void WriteXml(XmlTextWriter writer, BaseItem item, string elementName, bool includeCustomFields, NameValueCollection filters, Tags tags = null)
		{
			if (IsItemFiltered(item, filters)) return;
			writer.WriteStartElement(elementName);
			item.WriteToXml(writer, includeCustomFields, tags);
			writer.WriteEndElement();
		}

		public void WriteXml(XmlTextWriter writer, DataRow row, string elementName, bool includeCustomFields, NameValueCollection filters, Tags tags = null)
		{
			WriteXml(writer, new BaseItem(row, this), elementName, includeCustomFields, filters, tags);
		}

		public static string EndXmlWrite(XmlTextWriter writer)
        {
            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.BaseStream.Position = 0;
            StreamReader reader = new StreamReader(writer.BaseStream);
            return reader.ReadToEnd();
        }

        public static string GetPageQuery(string IdField, string table, string where, string order, string fields)
        {
            string query = @"
DECLARE @TempItems TABLE( ID int IDENTITY, {0} int )

INSERT INTO @TempItems ({0})
SELECT {0} FROM {1} 
{2}
ORDER BY {3}

SELECT {4}
FROM @TempItems ti INNER JOIN {1} x ON x.{0} = ti.{0}
WHERE ID BETWEEN @startRowIndex AND (@startRowIndex + @maximumRows) - 1
ORDER BY {3}
";
            return string.Format(query, IdField, table, where, order, fields);
        }

        public string GetXml(string listName, string elementName, bool includeCustomFields, NameValueCollection filters, bool isPaging = false)
        {
            XmlTextWriter writer = BeginXmlWrite(listName);

			foreach (DataRow row in Table.Rows)
            {
                WriteXml(writer, row, elementName, includeCustomFields, filters);
            }

			if (Table.Rows.Count > 0 && isPaging)
			{
				string apiTotalRecordsElement = "TotalRecords";
				writer.WriteElementString(apiTotalRecordsElement, Table.Rows[0][apiTotalRecordsElement].ToString());
			}

			return EndXmlWrite(writer);

            /*
            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, new UTF8Encoding(false));
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement(listName);


            foreach (DataRow row in Table.Rows)
            {
              BaseItem item = new BaseItem(row, this);

              if (IsItemFiltered(item, filters)) continue;
              writer.WriteStartElement(elementName);
              item.WriteToXml(writer, includeCustomFields);
              writer.WriteEndElement();
            }

            writer.WriteFullEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();*/
        }

        private bool IsItemFiltered(BaseItem item, NameValueCollection filters)
        {
            if (filters == null) return false;

            for (int i = 0; i < filters.Count; i++)
            {

                string[] values = filters.GetValues(i);
                // SEARCH ON CUSTOM FIELDS SOME TIME
                if (values != null)
                {
                    if (IsItemFiltered(item, filters.GetKey(i), values)) return true;
                }
            }

            return false;
        }

        private bool IsItemFiltered(BaseItem item, string name, string[] values)
        {
            int i = name.IndexOf('[');
            string condition = "";
            if (i > -1)
            {
                int j = name.IndexOf(']');
                condition = name.Substring(i + 1, j - i - 1).ToLower();
                name = name.Substring(0, i);
            }

            string field = FieldMap.GetPrivateField(name);
            if (field != "")
            {
                object o = item.Row[field];
                if (o == null) return false;

                Type dataType = item.Row.Table.Columns[field].DataType;

                foreach (string value in values)
                {
                    if (DoesValueMatch(dataType, o, value, condition)) return false;
                }
                return true;
            }
            else
            {
                CustomField customField = item.CustomFields.FindByApiFieldName(name);

                if (customField != null)
                {
                    object o = customField.GetTypedValue(item.PrimaryKeyID);

                    if (o != null)
                    {
                        foreach (string value in values)
                        {
                            if (DoesValueMatch(o.GetType(), o, value, condition)) return false;
                        }
                    }

                }

                return true;
            }

        }

        private bool DoesValueMatch(Type dataType, object o, string value, string condition)
        {
			condition = condition.ToLower();
            bool isNull = o == null || o == DBNull.Value || o.ToString().Trim() == "";

            if (value.ToLower() == "[null]") return condition == "not" ? !isNull : isNull;

            if (isNull) return false;

            if (dataType == typeof(System.DateTime))
            {
                //0123456789012345      
                //lt19990101152465
                if (value.Length != 14) return false;
                int y = int.Parse(value.Substring(0, 4));
                int m = int.Parse(value.Substring(4, 2));
                int d = int.Parse(value.Substring(6, 2));
                int h = int.Parse(value.Substring(8, 2));
                int n = int.Parse(value.Substring(10, 2));
                int s = int.Parse(value.Substring(12, 2));
                DateTime date2 = new DateTime(y, m, d, h, n, s);
                DateTime date1 = (DateTime)o;
                if (condition == "lt") return date2 > date1;
                if (date2 < date1)
                {
                    return date2 < date1;
                }
            }
            else if (dataType == typeof(System.Boolean))
            {
                bool b = value.ToLower().IndexOf("t") > -1 || value.ToLower().IndexOf("1") > -1 || value.ToLower().IndexOf("y") > -1;
                return bool.Parse(value) == (bool)o;
            }
            else if (dataType == typeof(System.Double))
            {
                double d1 = (double)o;
                double d2 = double.Parse(value);

                switch (condition)
                {
                    case "lt": return d1 < d2;
                    case "lte": return d1 <= d2;
                    case "gt": return d1 > d2;
                    case "gte": return d1 >= d2;
                    case "not": return d1 != d2;
                    default: return d1 == d2;
                }
            }
            else if (dataType == typeof(System.Int32))
            {
                int i1 = (int)o;
                int i2 = int.Parse(value);

                switch (condition)
                {
                    case "lt": return i1 < i2;
                    case "lte": return i1 <= i2;
                    case "gt": return i1 > i2;
                    case "gte": return i1 >= i2;
                    case "not": return i1 != i2;
                    default: return i1 == i2;
                }
            }
            else
            {
                if (condition == "contains")
                {
                    return o.ToString().ToLower().Trim().IndexOf(value.ToLower().Trim()) > -1;
                }
                if (condition == "not")
                {
                    return o.ToString().ToLower().Trim() != value.ToLower().Trim();
                }
				if (condition == "doesnotcontain")
				{
					return o.ToString().ToLower().Trim().IndexOf(value.ToLower().Trim()) < 0;
				}
                else
                {
                    return o.ToString().ToLower().Trim() == value.ToLower().Trim();
                }
            }
            return false;

        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(GetExpandoObject());
        }

        public  ExpandoObject[] GetExpandoObject()
        {
            return DataUtils.DataTableToExpandoObject(Table);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _table.Dispose();
        }

        #endregion


    }
}

