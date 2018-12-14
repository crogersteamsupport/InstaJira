using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CustomFieldsViewItem : BaseItem
  {
    private CustomFieldsView _customFieldsView;
    
    public CustomFieldsViewItem(DataRow row, CustomFieldsView customFieldsView): base(row, customFieldsView)
    {
      _customFieldsView = customFieldsView;
    }
	
    #region Properties
    
    public CustomFieldsView Collection
    {
      get { return _customFieldsView; }
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
    
    public string ParentFieldName
    {
      get { return Row["ParentFieldName"] != DBNull.Value ? (string)Row["ParentFieldName"] : null; }
      set { Row["ParentFieldName"] = CheckValue("ParentFieldName", value); }
    }
    
    public string ParentProductName
    {
      get { return Row["ParentProductName"] != DBNull.Value ? (string)Row["ParentProductName"] : null; }
      set { Row["ParentProductName"] = CheckValue("ParentProductName", value); }
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
    
    public int FieldType
    {
      get { return (int)Row["FieldType"]; }
      set { Row["FieldType"] = CheckValue("FieldType", value); }
    }
    
    public int RefType
    {
      get { return (int)Row["RefType"]; }
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
    
    public int CustomFieldID
    {
      get { return (int)Row["CustomFieldID"]; }
      set { Row["CustomFieldID"] = CheckValue("CustomFieldID", value); }
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

  public partial class CustomFieldsView : BaseCollection, IEnumerable<CustomFieldsViewItem>
  {
    public CustomFieldsView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CustomFieldsView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return ""; }
    }



    public CustomFieldsViewItem this[int index]
    {
      get { return new CustomFieldsViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CustomFieldsViewItem customFieldsViewItem);
    partial void AfterRowInsert(CustomFieldsViewItem customFieldsViewItem);
    partial void BeforeRowEdit(CustomFieldsViewItem customFieldsViewItem);
    partial void AfterRowEdit(CustomFieldsViewItem customFieldsViewItem);
    partial void BeforeRowDelete(int customFieldID);
    partial void AfterRowDelete(int customFieldID);    

    partial void BeforeDBDelete(int customFieldID);
    partial void AfterDBDelete(int customFieldID);    

    #endregion

    #region Public Methods

    public CustomFieldsViewItemProxy[] GetCustomFieldsViewItemProxies()
    {
      List<CustomFieldsViewItemProxy> list = new List<CustomFieldsViewItemProxy>();

      foreach (CustomFieldsViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int customFieldID)
    {
      BeforeDBDelete(customFieldID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomFieldsView] WH);";
        deleteCommand.Parameters.Add("CustomFieldID", SqlDbType.Int);
        deleteCommand.Parameters["CustomFieldID"].Value = customFieldID;

        BeforeRowDelete(customFieldID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(customFieldID);
      }
      AfterDBDelete(customFieldID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CustomFieldsViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CustomFieldsView] SET     [CustomFieldID] = @CustomFieldID,    [OrganizationID] = @OrganizationID,    [Name] = @Name,    [ApiFieldName] = @ApiFieldName,    [RefType] = @RefType,    [FieldType] = @FieldType,    [AuxID] = @AuxID,    [Position] = @Position,    [ListValues] = @ListValues,    [Description] = @Description,    [IsVisibleOnPortal] = @IsVisibleOnPortal,    [IsFirstIndexSelect] = @IsFirstIndexSelect,    [IsRequired] = @IsRequired,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [CustomFieldCategoryID] = @CustomFieldCategoryID,    [IsRequiredToClose] = @IsRequiredToClose,    [Mask] = @Mask,    [ParentCustomFieldID] = @ParentCustomFieldID,    [ParentCustomValue] = @ParentCustomValue,    [ParentProductID] = @ParentProductID,    [ParentFieldName] = @ParentFieldName,    [ParentProductName] = @ParentProductName  WH);";

		
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
		
		tempParameter = updateCommand.Parameters.Add("ParentFieldName", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ParentProductName", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CustomFieldsView] (    [CustomFieldID],    [OrganizationID],    [Name],    [ApiFieldName],    [RefType],    [FieldType],    [AuxID],    [Position],    [ListValues],    [Description],    [IsVisibleOnPortal],    [IsFirstIndexSelect],    [IsRequired],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [CustomFieldCategoryID],    [IsRequiredToClose],    [Mask],    [ParentCustomFieldID],    [ParentCustomValue],    [ParentProductID],    [ParentFieldName],    [ParentProductName]) VALUES ( @CustomFieldID, @OrganizationID, @Name, @ApiFieldName, @RefType, @FieldType, @AuxID, @Position, @ListValues, @Description, @IsVisibleOnPortal, @IsFirstIndexSelect, @IsRequired, @DateCreated, @DateModified, @CreatorID, @ModifierID, @CustomFieldCategoryID, @IsRequiredToClose, @Mask, @ParentCustomFieldID, @ParentCustomValue, @ParentProductID, @ParentFieldName, @ParentProductName); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ParentProductName", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ParentFieldName", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
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
		
		tempParameter = insertCommand.Parameters.Add("CustomFieldID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomFieldsView] WH);";
		deleteCommand.Parameters.Add("", SqlDbType.Int);

		try
		{
		  foreach (CustomFieldsViewItem customFieldsViewItem in this)
		  {
			if (customFieldsViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(customFieldsViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = customFieldsViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns[""].AutoIncrement = false;
			  Table.Columns[""].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				customFieldsViewItem.Row[""] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(customFieldsViewItem);
			}
			else if (customFieldsViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(customFieldsViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = customFieldsViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(customFieldsViewItem);
			}
			else if (customFieldsViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)customFieldsViewItem.Row["", DataRowVersion.Original];
			  deleteCommand.Parameters[""].Value = id;
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

      foreach (CustomFieldsViewItem customFieldsViewItem in this)
      {
        if (customFieldsViewItem.Row.Table.Columns.Contains("CreatorID") && (int)customFieldsViewItem.Row["CreatorID"] == 0) customFieldsViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (customFieldsViewItem.Row.Table.Columns.Contains("ModifierID")) customFieldsViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CustomFieldsViewItem FindByCustomFieldID(int customFieldID)
    {
      foreach (CustomFieldsViewItem customFieldsViewItem in this)
      {
        if (customFieldsViewItem.CustomFieldID == customFieldID)
        {
          return customFieldsViewItem;
        }
      }
      return null;
    }

    public virtual CustomFieldsViewItem AddNewCustomFieldsViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("CustomFieldsView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CustomFieldsViewItem(row, this);
    }
    
    public virtual void LoadByCustomFieldID(int customFieldID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CustomFieldID], [OrganizationID], [Name], [ApiFieldName], [RefType], [FieldType], [AuxID], [Position], [ListValues], [Description], [IsVisibleOnPortal], [IsFirstIndexSelect], [IsRequired], [DateCreated], [DateModified], [CreatorID], [ModifierID], [CustomFieldCategoryID], [IsRequiredToClose], [Mask], [ParentCustomFieldID], [ParentCustomValue], [ParentProductID], [ParentFieldName], [ParentProductName] FROM [dbo].[CustomFieldsView] WHERE ([CustomFieldID] = @CustomFieldID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CustomFieldID", customFieldID);
        Fill(command);
      }
    }
    
    public static CustomFieldsViewItem GetCustomFieldsViewItem(LoginUser loginUser, int customFieldID)
    {
      CustomFieldsView customFieldsView = new CustomFieldsView(loginUser);
      customFieldsView.LoadByCustomFieldID(customFieldID);
      if (customFieldsView.IsEmpty)
        return null;
      else
        return customFieldsView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CustomFieldsViewItem> Members

    public IEnumerator<CustomFieldsViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CustomFieldsViewItem(row, this);
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
