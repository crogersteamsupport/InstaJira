using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class SearchCustomFilter : BaseItem
  {
    private SearchCustomFilters _searchCustomFilters;
    
    public SearchCustomFilter(DataRow row, SearchCustomFilters searchCustomFilters): base(row, searchCustomFilters)
    {
      _searchCustomFilters = searchCustomFilters;
    }
	
    #region Properties
    
    public SearchCustomFilters Collection
    {
      get { return _searchCustomFilters; }
    }
        
    
    
    
    public int CustomFilterID
    {
      get { return (int)Row["CustomFilterID"]; }
    }
    

    

    
    public bool MatchAll
    {
      get { return (bool)Row["MatchAll"]; }
      set { Row["MatchAll"] = CheckValue("MatchAll", value); }
    }
    
    public string TestValue
    {
      get { return (string)Row["TestValue"]; }
      set { Row["TestValue"] = CheckValue("TestValue", value); }
    }
    
    public string Measure
    {
      get { return (string)Row["Measure"]; }
      set { Row["Measure"] = CheckValue("Measure", value); }
    }
    
    public int FieldID
    {
      get { return (int)Row["FieldID"]; }
      set { Row["FieldID"] = CheckValue("FieldID", value); }
    }
    
    public int TableID
    {
      get { return (int)Row["TableID"]; }
      set { Row["TableID"] = CheckValue("TableID", value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class SearchCustomFilters : BaseCollection, IEnumerable<SearchCustomFilter>
  {
    public SearchCustomFilters(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "SearchCustomFilters"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CustomFilterID"; }
    }



    public SearchCustomFilter this[int index]
    {
      get { return new SearchCustomFilter(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(SearchCustomFilter searchCustomFilter);
    partial void AfterRowInsert(SearchCustomFilter searchCustomFilter);
    partial void BeforeRowEdit(SearchCustomFilter searchCustomFilter);
    partial void AfterRowEdit(SearchCustomFilter searchCustomFilter);
    partial void BeforeRowDelete(int customFilterID);
    partial void AfterRowDelete(int customFilterID);    

    partial void BeforeDBDelete(int customFilterID);
    partial void AfterDBDelete(int customFilterID);    

    #endregion

    #region Public Methods

    public SearchCustomFilterProxy[] GetSearchCustomFilterProxies()
    {
      List<SearchCustomFilterProxy> list = new List<SearchCustomFilterProxy>();

      foreach (SearchCustomFilter item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int customFilterID)
    {
      BeforeDBDelete(customFilterID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SearchCustomFilters] WHERE ([CustomFilterID] = @CustomFilterID);";
        deleteCommand.Parameters.Add("CustomFilterID", SqlDbType.Int);
        deleteCommand.Parameters["CustomFilterID"].Value = customFilterID;

        BeforeRowDelete(customFilterID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(customFilterID);
      }
      AfterDBDelete(customFilterID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("SearchCustomFiltersSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[SearchCustomFilters] SET     [UserID] = @UserID,    [TableID] = @TableID,    [FieldID] = @FieldID,    [Measure] = @Measure,    [TestValue] = @TestValue,    [MatchAll] = @MatchAll  WHERE ([CustomFilterID] = @CustomFilterID);";

		
		tempParameter = updateCommand.Parameters.Add("CustomFilterID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TableID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("FieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Measure", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TestValue", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("MatchAll", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[SearchCustomFilters] (    [UserID],    [TableID],    [FieldID],    [Measure],    [TestValue],    [MatchAll]) VALUES ( @UserID, @TableID, @FieldID, @Measure, @TestValue, @MatchAll); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("MatchAll", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TestValue", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Measure", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("FieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TableID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SearchCustomFilters] WHERE ([CustomFilterID] = @CustomFilterID);";
		deleteCommand.Parameters.Add("CustomFilterID", SqlDbType.Int);

		try
		{
		  foreach (SearchCustomFilter searchCustomFilter in this)
		  {
			if (searchCustomFilter.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(searchCustomFilter);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = searchCustomFilter.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CustomFilterID"].AutoIncrement = false;
			  Table.Columns["CustomFilterID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				searchCustomFilter.Row["CustomFilterID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(searchCustomFilter);
			}
			else if (searchCustomFilter.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(searchCustomFilter);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = searchCustomFilter.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(searchCustomFilter);
			}
			else if (searchCustomFilter.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)searchCustomFilter.Row["CustomFilterID", DataRowVersion.Original];
			  deleteCommand.Parameters["CustomFilterID"].Value = id;
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

      foreach (SearchCustomFilter searchCustomFilter in this)
      {
        if (searchCustomFilter.Row.Table.Columns.Contains("CreatorID") && (int)searchCustomFilter.Row["CreatorID"] == 0) searchCustomFilter.Row["CreatorID"] = LoginUser.UserID;
        if (searchCustomFilter.Row.Table.Columns.Contains("ModifierID")) searchCustomFilter.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public SearchCustomFilter FindByCustomFilterID(int customFilterID)
    {
      foreach (SearchCustomFilter searchCustomFilter in this)
      {
        if (searchCustomFilter.CustomFilterID == customFilterID)
        {
          return searchCustomFilter;
        }
      }
      return null;
    }

    public virtual SearchCustomFilter AddNewSearchCustomFilter()
    {
      if (Table.Columns.Count < 1) LoadColumns("SearchCustomFilters");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new SearchCustomFilter(row, this);
    }
    
    public virtual void LoadByCustomFilterID(int customFilterID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CustomFilterID], [UserID], [TableID], [FieldID], [Measure], [TestValue], [MatchAll] FROM [dbo].[SearchCustomFilters] WHERE ([CustomFilterID] = @CustomFilterID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CustomFilterID", customFilterID);
        Fill(command);
      }
    }
    
    public static SearchCustomFilter GetSearchCustomFilter(LoginUser loginUser, int customFilterID)
    {
      SearchCustomFilters searchCustomFilters = new SearchCustomFilters(loginUser);
      searchCustomFilters.LoadByCustomFilterID(customFilterID);
      if (searchCustomFilters.IsEmpty)
        return null;
      else
        return searchCustomFilters[0];
    }
    
    
    

    #endregion

    #region IEnumerable<SearchCustomFilter> Members

    public IEnumerator<SearchCustomFilter> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new SearchCustomFilter(row, this);
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
