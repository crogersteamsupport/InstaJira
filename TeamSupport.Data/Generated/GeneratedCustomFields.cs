using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CustomField : BaseItem
  {
    private CustomFields _customFields;
    
    public CustomField(DataRow row, CustomFields customFields): base(row, customFields)
    {
      _customFields = customFields;
    }
	
    #region Properties
    
    public CustomFields Collection
    {
      get { return _customFields; }
    }
        
    
    
    
    public int CustomFieldID
    {
      get { return (int)Row["CustomFieldID"]; }
    }
    

    
    public string ListValues
    {
      get { return Row["ListValues"] != DBNull.Value ? (string)Row["ListValues"] : null; }
      set { Row["ListValues"] = CheckValue("ListValues", value); }
    }
    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public bool? IsVisibleOnPortal
    {
      get { return Row["IsVisibleOnPortal"] != DBNull.Value ? (bool?)Row["IsVisibleOnPortal"] : null; }
      set { Row["IsVisibleOnPortal"] = CheckValue("IsVisibleOnPortal", value); }
    }
    
    public int? CustomFieldCategoryID
    {
      get { return Row["CustomFieldCategoryID"] != DBNull.Value ? (int?)Row["CustomFieldCategoryID"] : null; }
      set { Row["CustomFieldCategoryID"] = CheckValue("CustomFieldCategoryID", value); }
    }
    
    public string Mask
    {
      get { return Row["Mask"] != DBNull.Value ? (string)Row["Mask"] : null; }
      set { Row["Mask"] = CheckValue("Mask", value); }
    }
    
    public int? ParentCustomFieldID
    {
      get { return Row["ParentCustomFieldID"] != DBNull.Value ? (int?)Row["ParentCustomFieldID"] : null; }
      set { Row["ParentCustomFieldID"] = CheckValue("ParentCustomFieldID", value); }
    }
    
    public string ParentCustomValue
    {
      get { return Row["ParentCustomValue"] != DBNull.Value ? (string)Row["ParentCustomValue"] : null; }
      set { Row["ParentCustomValue"] = CheckValue("ParentCustomValue", value); }
    }
    
    public int? ParentProductID
    {
      get { return Row["ParentProductID"] != DBNull.Value ? (int?)Row["ParentProductID"] : null; }
      set { Row["ParentProductID"] = CheckValue("ParentProductID", value); }
    }
    

    
    public bool IsRequiredToClose
    {
      get { return (bool)Row["IsRequiredToClose"]; }
      set { Row["IsRequiredToClose"] = CheckValue("IsRequiredToClose", value); }
    }
    
    public int ModifierID
    {
      get { return (int)Row["ModifierID"]; }
      set { Row["ModifierID"] = CheckValue("ModifierID", value); }
    }
    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public bool IsRequired
    {
      get { return (bool)Row["IsRequired"]; }
      set { Row["IsRequired"] = CheckValue("IsRequired", value); }
    }
    
    public bool IsFirstIndexSelect
    {
      get { return (bool)Row["IsFirstIndexSelect"]; }
      set { Row["IsFirstIndexSelect"] = CheckValue("IsFirstIndexSelect", value); }
    }
    
    public int Position
    {
      get { return (int)Row["Position"]; }
      set { Row["Position"] = CheckValue("Position", value); }
    }
    
    public int AuxID
    {
      get { return (int)Row["AuxID"]; }
      set { Row["AuxID"] = CheckValue("AuxID", value); }
    }
    
    public CustomFieldType FieldType
    {
      get { return (CustomFieldType)Row["FieldType"]; }
      set { Row["FieldType"] = CheckValue("FieldType", value); }
    }
    
    public ReferenceType RefType
    {
      get { return (ReferenceType)Row["RefType"]; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    
    public string ApiFieldName
    {
      get { return (string)Row["ApiFieldName"]; }
      set { Row["ApiFieldName"] = CheckValue("ApiFieldName", value); }
    }
    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckValue("Name", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateModified
    {
      get { return DateToLocal((DateTime)Row["DateModified"]); }
      set { Row["DateModified"] = CheckValue("DateModified", value); }
    }

    public DateTime DateModifiedUtc
    {
      get { return (DateTime)Row["DateModified"]; }
    }
    
    public DateTime DateCreated
    {
      get { return DateToLocal((DateTime)Row["DateCreated"]); }
      set { Row["DateCreated"] = CheckValue("DateCreated", value); }
    }

    public DateTime DateCreatedUtc
    {
      get { return (DateTime)Row["DateCreated"]; }
    }
    

    #endregion
    
    
  }

  public partial class CustomFields : BaseCollection, IEnumerable<CustomField>
  {
    public CustomFields(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CustomFields"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CustomFieldID"; }
    }



    public CustomField this[int index]
    {
      get { return new CustomField(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CustomField customField);
    partial void AfterRowInsert(CustomField customField);
    partial void BeforeRowEdit(CustomField customField);
    partial void AfterRowEdit(CustomField customField);
    partial void BeforeRowDelete(int customFieldID);
    partial void AfterRowDelete(int customFieldID);    

    partial void BeforeDBDelete(int customFieldID);
    partial void AfterDBDelete(int customFieldID);    

    #endregion

    #region Public Methods

    public CustomFieldProxy[] GetCustomFieldProxies()
    {
      List<CustomFieldProxy> list = new List<CustomFieldProxy>();

      foreach (CustomField item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int customFieldID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomFields] WHERE ([CustomFieldID] = @CustomFieldID);";
        deleteCommand.Parameters.Add("CustomFieldID", SqlDbType.Int);
        deleteCommand.Parameters["CustomFieldID"].Value = customFieldID;

        BeforeDBDelete(customFieldID);
        BeforeRowDelete(customFieldID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(customFieldID);
        AfterDBDelete(customFieldID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CustomFieldsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CustomFields] SET     [OrganizationID] = @OrganizationID,    [Name] = @Name,    [ApiFieldName] = @ApiFieldName,    [RefType] = @RefType,    [FieldType] = @FieldType,    [AuxID] = @AuxID,    [Position] = @Position,    [ListValues] = @ListValues,    [Description] = @Description,    [IsVisibleOnPortal] = @IsVisibleOnPortal,    [IsFirstIndexSelect] = @IsFirstIndexSelect,    [IsRequired] = @IsRequired,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [CustomFieldCategoryID] = @CustomFieldCategoryID,    [IsRequiredToClose] = @IsRequiredToClose,    [Mask] = @Mask,    [ParentCustomFieldID] = @ParentCustomFieldID,    [ParentCustomValue] = @ParentCustomValue,    [ParentProductID] = @ParentProductID  WHERE ([CustomFieldID] = @CustomFieldID);";

		
		tempParameter = updateCommand.Parameters.Add("CustomFieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ApiFieldName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("FieldType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("AuxID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ListValues", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsVisibleOnPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsFirstIndexSelect", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsRequired", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CustomFieldCategoryID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsRequiredToClose", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Mask", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ParentCustomFieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ParentCustomValue", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ParentProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CustomFields] (    [OrganizationID],    [Name],    [ApiFieldName],    [RefType],    [FieldType],    [AuxID],    [Position],    [ListValues],    [Description],    [IsVisibleOnPortal],    [IsFirstIndexSelect],    [IsRequired],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [CustomFieldCategoryID],    [IsRequiredToClose],    [Mask],    [ParentCustomFieldID],    [ParentCustomValue],    [ParentProductID]) VALUES ( @OrganizationID, @Name, @ApiFieldName, @RefType, @FieldType, @AuxID, @Position, @ListValues, @Description, @IsVisibleOnPortal, @IsFirstIndexSelect, @IsRequired, @DateCreated, @DateModified, @CreatorID, @ModifierID, @CustomFieldCategoryID, @IsRequiredToClose, @Mask, @ParentCustomFieldID, @ParentCustomValue, @ParentProductID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ParentProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ParentCustomValue", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ParentCustomFieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Mask", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsRequiredToClose", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CustomFieldCategoryID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsRequired", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsFirstIndexSelect", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsVisibleOnPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ListValues", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("AuxID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("FieldType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ApiFieldName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomFields] WHERE ([CustomFieldID] = @CustomFieldID);";
		deleteCommand.Parameters.Add("CustomFieldID", SqlDbType.Int);

		try
		{
		  foreach (CustomField customField in this)
		  {
			if (customField.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(customField);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = customField.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CustomFieldID"].AutoIncrement = false;
			  Table.Columns["CustomFieldID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				customField.Row["CustomFieldID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(customField);
			}
			else if (customField.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(customField);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = customField.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(customField);
			}
			else if (customField.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)customField.Row["CustomFieldID", DataRowVersion.Original];
			  deleteCommand.Parameters["CustomFieldID"].Value = id;
			  BeforeRowDelete(id);
			  deleteCommand.ExecuteNonQuery();
			  AfterRowDelete(id);
			}
		  }
		  //transaction.Commit();
		}
		catch (Exception)
		{
		  //transaction.Rollback();
		  throw;
		}
		Table.AcceptChanges();
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public void BulkSave()
    {

      foreach (CustomField customField in this)
      {
        if (customField.Row.Table.Columns.Contains("CreatorID") && (int)customField.Row["CreatorID"] == 0) customField.Row["CreatorID"] = LoginUser.UserID;
        if (customField.Row.Table.Columns.Contains("ModifierID")) customField.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CustomField FindByCustomFieldID(int customFieldID)
    {
      foreach (CustomField customField in this)
      {
        if (customField.CustomFieldID == customFieldID)
        {
          return customField;
        }
      }
      return null;
    }

    public virtual CustomField AddNewCustomField()
    {
      if (Table.Columns.Count < 1) LoadColumns("CustomFields");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CustomField(row, this);
    }
    
    public virtual void LoadByCustomFieldID(int customFieldID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CustomFieldID], [OrganizationID], [Name], [ApiFieldName], [RefType], [FieldType], [AuxID], [Position], [ListValues], [Description], [IsVisibleOnPortal], [IsFirstIndexSelect], [IsRequired], [DateCreated], [DateModified], [CreatorID], [ModifierID], [CustomFieldCategoryID], [IsRequiredToClose], [Mask], [ParentCustomFieldID], [ParentCustomValue], [ParentProductID] FROM [dbo].[CustomFields] WHERE ([CustomFieldID] = @CustomFieldID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CustomFieldID", customFieldID);
        Fill(command);
      }
    }
    
    public static CustomField GetCustomField(LoginUser loginUser, int customFieldID)
    {
      CustomFields customFields = new CustomFields(loginUser);
      customFields.LoadByCustomFieldID(customFieldID);
      if (customFields.IsEmpty)
        return null;
      else
        return customFields[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CustomField> Members

    public IEnumerator<CustomField> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CustomField(row, this);
      }
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion
  }
}
