using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class SimpleImportFieldsViewItem : BaseItem
  {
    private SimpleImportFieldsView _simpleImportFieldsView;
    
    public SimpleImportFieldsViewItem(DataRow row, SimpleImportFieldsView simpleImportFieldsView): base(row, simpleImportFieldsView)
    {
      _simpleImportFieldsView = simpleImportFieldsView;
    }
	
    #region Properties
    
    public SimpleImportFieldsView Collection
    {
      get { return _simpleImportFieldsView; }
    }
        
    
    
    

    
    public string FieldName
    {
      get { return Row["FieldName"] != DBNull.Value ? (string)Row["FieldName"] : null; }
      set { Row["FieldName"] = CheckValue("FieldName", value); }
    }
    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public int? OrganizationID
    {
      get { return Row["OrganizationID"] != DBNull.Value ? (int?)Row["OrganizationID"] : null; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    
    public string IsCustom
    {
      get { return (string)Row["IsCustom"]; }
      set { Row["IsCustom"] = CheckValue("IsCustom", value); }
    }
    
    public int Position
    {
      get { return (int)Row["Position"]; }
      set { Row["Position"] = CheckValue("Position", value); }
    }
    
    public int RefType
    {
      get { return (int)Row["RefType"]; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    
    public bool IsRequired
    {
      get { return (bool)Row["IsRequired"]; }
      set { Row["IsRequired"] = CheckValue("IsRequired", value); }
    }
    
    public bool IsVisible
    {
      get { return (bool)Row["IsVisible"]; }
      set { Row["IsVisible"] = CheckValue("IsVisible", value); }
    }
    
    public int Size
    {
      get { return (int)Row["Size"]; }
      set { Row["Size"] = CheckValue("Size", value); }
    }
    
    public string DataType
    {
      get { return (string)Row["DataType"]; }
      set { Row["DataType"] = CheckValue("DataType", value); }
    }
    
    public string Alias
    {
      get { return (string)Row["Alias"]; }
      set { Row["Alias"] = CheckValue("Alias", value); }
    }
    
    public string TableName
    {
      get { return (string)Row["TableName"]; }
      set { Row["TableName"] = CheckValue("TableName", value); }
    }
    
    public int ImportFieldID
    {
      get { return (int)Row["ImportFieldID"]; }
      set { Row["ImportFieldID"] = CheckValue("ImportFieldID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class SimpleImportFieldsView : BaseCollection, IEnumerable<SimpleImportFieldsViewItem>
  {
    public SimpleImportFieldsView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "SimpleImportFieldsView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return ""; }
    }



    public SimpleImportFieldsViewItem this[int index]
    {
      get { return new SimpleImportFieldsViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(SimpleImportFieldsViewItem simpleImportFieldsViewItem);
    partial void AfterRowInsert(SimpleImportFieldsViewItem simpleImportFieldsViewItem);
    partial void BeforeRowEdit(SimpleImportFieldsViewItem simpleImportFieldsViewItem);
    partial void AfterRowEdit(SimpleImportFieldsViewItem simpleImportFieldsViewItem);
    //partial void BeforeRowDelete(int );
    //partial void AfterRowDelete(int );    

    //partial void BeforeDBDelete(int );
    //partial void AfterDBDelete(int );    

    #endregion

    #region Public Methods

    public SimpleImportFieldsViewItemProxy[] GetSimpleImportFieldsViewItemProxies()
    {
      List<SimpleImportFieldsViewItemProxy> list = new List<SimpleImportFieldsViewItemProxy>();

      foreach (SimpleImportFieldsViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    //public virtual void DeleteFromDB(int )
    //{
    //  BeforeDBDelete();
    //  using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
    //  {
    //    connection.Open();

    //    SqlCommand deleteCommand = connection.CreateCommand();

    //    deleteCommand.Connection = connection;
    //    deleteCommand.CommandType = CommandType.Text;
    //    deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SimpleImportFieldsView] WH);";
    //    deleteCommand.Parameters.Add("", SqlDbType.Int);
    //    deleteCommand.Parameters[""].Value = ;

    //    BeforeRowDelete();
    //    deleteCommand.ExecuteNonQuery();
    //connection.Close();
    //    if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    //    AfterRowDelete();
    //  }
    //  AfterDBDelete();
      
    //}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("SimpleImportFieldsViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[SimpleImportFieldsView] SET     [ImportFieldID] = @ImportFieldID,    [TableName] = @TableName,    [FieldName] = @FieldName,    [Alias] = @Alias,    [DataType] = @DataType,    [Size] = @Size,    [IsVisible] = @IsVisible,    [IsRequired] = @IsRequired,    [Description] = @Description,    [RefType] = @RefType,    [Position] = @Position,    [IsCustom] = @IsCustom,    [OrganizationID] = @OrganizationID  WH);";

		
		tempParameter = updateCommand.Parameters.Add("ImportFieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TableName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("FieldName", SqlDbType.VarChar, 561);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Alias", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DataType", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Size", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsVisible", SqlDbType.Bit, 1);
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
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 1000);
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
		
		tempParameter = updateCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsCustom", SqlDbType.VarChar, 5);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[SimpleImportFieldsView] (    [ImportFieldID],    [TableName],    [FieldName],    [Alias],    [DataType],    [Size],    [IsVisible],    [IsRequired],    [Description],    [RefType],    [Position],    [IsCustom],    [OrganizationID]) VALUES ( @ImportFieldID, @TableName, @FieldName, @Alias, @DataType, @Size, @IsVisible, @IsRequired, @Description, @RefType, @Position, @IsCustom, @OrganizationID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsCustom", SqlDbType.VarChar, 5);
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
		
		tempParameter = insertCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsRequired", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsVisible", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Size", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DataType", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Alias", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("FieldName", SqlDbType.VarChar, 561);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TableName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ImportFieldID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SimpleImportFieldsView] WH);";
		deleteCommand.Parameters.Add("", SqlDbType.Int);

		try
		{
		  foreach (SimpleImportFieldsViewItem simpleImportFieldsViewItem in this)
		  {
			if (simpleImportFieldsViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(simpleImportFieldsViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = simpleImportFieldsViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns[""].AutoIncrement = false;
			  Table.Columns[""].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				simpleImportFieldsViewItem.Row[""] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(simpleImportFieldsViewItem);
			}
			else if (simpleImportFieldsViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(simpleImportFieldsViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = simpleImportFieldsViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(simpleImportFieldsViewItem);
			}
			else if (simpleImportFieldsViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)simpleImportFieldsViewItem.Row["", DataRowVersion.Original];
			  deleteCommand.Parameters[""].Value = id;
        //BeforeRowDelete(id);
			  deleteCommand.ExecuteNonQuery();
        //AfterRowDelete(id);
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

      foreach (SimpleImportFieldsViewItem simpleImportFieldsViewItem in this)
      {
        if (simpleImportFieldsViewItem.Row.Table.Columns.Contains("CreatorID") && (int)simpleImportFieldsViewItem.Row["CreatorID"] == 0) simpleImportFieldsViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (simpleImportFieldsViewItem.Row.Table.Columns.Contains("ModifierID")) simpleImportFieldsViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    //public SimpleImportFieldsViewItem FindBy(int )
    //{
    //  foreach (SimpleImportFieldsViewItem simpleImportFieldsViewItem in this)
    //  {
    //    if (simpleImportFieldsViewItem. == )
    //    {
    //      return simpleImportFieldsViewItem;
    //    }
    //  }
    //  return null;
    //}

    public virtual SimpleImportFieldsViewItem AddNewSimpleImportFieldsViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("SimpleImportFieldsView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new SimpleImportFieldsViewItem(row, this);
    }
    
    //public virtual void LoadBy(int )
    //{
    //  using (SqlCommand command = new SqlCommand())
    //  {
    //    command.CommandText = "SET NOCOUNT OFF; SELECT [ImportFieldID], [TableName], [FieldName], [Alias], [DataType], [Size], [IsVisible], [IsRequired], [Description], [RefType] FROM [dbo].[SimpleImportFieldsView] WH);";
    //    command.CommandType = CommandType.Text;
    //    command.Parameters.AddWithValue("", );
    //    Fill(command);
    //  }
    //}
    
    //public static SimpleImportFieldsViewItem GetSimpleImportFieldsViewItem(LoginUser loginUser, int )
    //{
    //  SimpleImportFieldsView simpleImportFieldsView = new SimpleImportFieldsView(loginUser);
    //  simpleImportFieldsView.LoadBy();
    //  if (simpleImportFieldsView.IsEmpty)
    //    return null;
    //  else
    //    return simpleImportFieldsView[0];
    //}
    
    
    

    #endregion

    #region IEnumerable<SimpleImportFieldsViewItem> Members

    public IEnumerator<SimpleImportFieldsViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new SimpleImportFieldsViewItem(row, this);
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
