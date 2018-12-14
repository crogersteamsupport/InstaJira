using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CustomValue : BaseItem
  {
    private CustomValues _customValues;
    
    public CustomValue(DataRow row, CustomValues customValues): base(row, customValues)
    {
      _customValues = customValues;
    }
	
    #region Properties
    
    public CustomValues Collection
    {
      get { return _customValues; }
    }
        
    
    
    
    public int CustomValueID
    {
      get { return (int)Row["CustomValueID"]; }
    }
    

    
    public int? ImportFileID
    {
      get { return Row["ImportFileID"] != DBNull.Value ? (int?)Row["ImportFileID"] : null; }
      set { Row["ImportFileID"] = CheckValue("ImportFileID", value); }
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
    
    public string Value
    {
      get { return (string)Row["CustomValue"]; }
      set { Row["CustomValue"] = CheckValue("CustomValue", value); }
    }
    
    public int RefID
    {
      get { return (int)Row["RefID"]; }
      set { Row["RefID"] = CheckValue("RefID", value); }
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

  public partial class CustomValues : BaseCollection, IEnumerable<CustomValue>
  {
    public CustomValues(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CustomValues"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CustomValueID"; }
    }



    public CustomValue this[int index]
    {
      get { return new CustomValue(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CustomValue customValue);
    partial void AfterRowInsert(CustomValue customValue);
    partial void BeforeRowEdit(CustomValue customValue);
    partial void AfterRowEdit(CustomValue customValue);
    partial void BeforeRowDelete(int customValueID);
    partial void AfterRowDelete(int customValueID);    

    partial void BeforeDBDelete(int customValueID);
    partial void AfterDBDelete(int customValueID);    

    #endregion

    #region Public Methods

    public CustomValueProxy[] GetCustomValueProxies()
    {
      List<CustomValueProxy> list = new List<CustomValueProxy>();

      foreach (CustomValue item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int customValueID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomValues] WHERE ([CustomValueID] = @CustomValueID);";
        deleteCommand.Parameters.Add("CustomValueID", SqlDbType.Int);
        deleteCommand.Parameters["CustomValueID"].Value = customValueID;

        BeforeDBDelete(customValueID);
        BeforeRowDelete(customValueID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(customValueID);
        AfterDBDelete(customValueID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CustomValuesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CustomValues] SET     [CustomFieldID] = @CustomFieldID,    [RefID] = @RefID,    [CustomValue] = @CustomValue,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [ImportFileID] = @ImportFileID  WHERE ([CustomValueID] = @CustomValueID);";

		
		tempParameter = updateCommand.Parameters.Add("CustomValueID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CustomFieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CustomValue", SqlDbType.VarChar, 1000);
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
		
		tempParameter = updateCommand.Parameters.Add("ImportFileID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CustomValues] (    [CustomFieldID],    [RefID],    [CustomValue],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [ImportFileID]) VALUES ( @CustomFieldID, @RefID, @CustomValue, @DateCreated, @DateModified, @CreatorID, @ModifierID, @ImportFileID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ImportFileID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("CustomValue", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomValues] WHERE ([CustomValueID] = @CustomValueID);";
		deleteCommand.Parameters.Add("CustomValueID", SqlDbType.Int);

		try
		{
		  foreach (CustomValue customValue in this)
		  {
			if (customValue.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(customValue);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = customValue.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CustomValueID"].AutoIncrement = false;
			  Table.Columns["CustomValueID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				customValue.Row["CustomValueID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(customValue);
			}
			else if (customValue.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(customValue);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = customValue.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(customValue);
			}
			else if (customValue.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)customValue.Row["CustomValueID", DataRowVersion.Original];
			  deleteCommand.Parameters["CustomValueID"].Value = id;
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

      foreach (CustomValue customValue in this)
      {
        if (customValue.Row.Table.Columns.Contains("CreatorID") && (int)customValue.Row["CreatorID"] == 0) customValue.Row["CreatorID"] = LoginUser.UserID;
        if (customValue.Row.Table.Columns.Contains("ModifierID")) customValue.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CustomValue FindByCustomValueID(int customValueID)
    {
      foreach (CustomValue customValue in this)
      {
        if (customValue.CustomValueID == customValueID)
        {
          return customValue;
        }
      }
      return null;
    }

    public virtual CustomValue AddNewCustomValue()
    {
      if (Table.Columns.Count < 1) LoadColumns("CustomValues");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CustomValue(row, this);
    }
    
    public virtual void LoadByCustomValueID(int customValueID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CustomValueID], [CustomFieldID], [RefID], [CustomValue], [DateCreated], [DateModified], [CreatorID], [ModifierID], [ImportFileID] FROM [dbo].[CustomValues] WHERE ([CustomValueID] = @CustomValueID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CustomValueID", customValueID);
        Fill(command);
      }
    }
    
    public static CustomValue GetCustomValue(LoginUser loginUser, int customValueID)
    {
      CustomValues customValues = new CustomValues(loginUser);
      customValues.LoadByCustomValueID(customValueID);
      if (customValues.IsEmpty)
        return null;
      else
        return customValues[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CustomValue> Members

    public IEnumerator<CustomValue> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CustomValue(row, this);
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
