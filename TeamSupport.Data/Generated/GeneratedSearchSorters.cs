using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class SearchSorter : BaseItem
  {
    private SearchSorters _searchSorters;
    
    public SearchSorter(DataRow row, SearchSorters searchSorters): base(row, searchSorters)
    {
      _searchSorters = searchSorters;
    }
	
    #region Properties
    
    public SearchSorters Collection
    {
      get { return _searchSorters; }
    }
        
    
    
    
    public int SorterID
    {
      get { return (int)Row["SorterID"]; }
    }
    

    

    
    public bool Descending
    {
      get { return (bool)Row["Descending"]; }
      set { Row["Descending"] = CheckValue("Descending", value); }
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

  public partial class SearchSorters : BaseCollection, IEnumerable<SearchSorter>
  {
    public SearchSorters(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "SearchSorters"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "SorterID"; }
    }



    public SearchSorter this[int index]
    {
      get { return new SearchSorter(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(SearchSorter searchSorter);
    partial void AfterRowInsert(SearchSorter searchSorter);
    partial void BeforeRowEdit(SearchSorter searchSorter);
    partial void AfterRowEdit(SearchSorter searchSorter);
    partial void BeforeRowDelete(int sorterID);
    partial void AfterRowDelete(int sorterID);    

    partial void BeforeDBDelete(int sorterID);
    partial void AfterDBDelete(int sorterID);    

    #endregion

    #region Public Methods

    public SearchSorterProxy[] GetSearchSorterProxies()
    {
      List<SearchSorterProxy> list = new List<SearchSorterProxy>();

      foreach (SearchSorter item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int sorterID)
    {
      BeforeDBDelete(sorterID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SearchSorters] WHERE ([SorterID] = @SorterID);";
        deleteCommand.Parameters.Add("SorterID", SqlDbType.Int);
        deleteCommand.Parameters["SorterID"].Value = sorterID;

        BeforeRowDelete(sorterID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(sorterID);
      }
      AfterDBDelete(sorterID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("SearchSortersSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[SearchSorters] SET     [UserID] = @UserID,    [TableID] = @TableID,    [FieldID] = @FieldID,    [Descending] = @Descending  WHERE ([SorterID] = @SorterID);";

		
		tempParameter = updateCommand.Parameters.Add("SorterID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Descending", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[SearchSorters] (    [UserID],    [TableID],    [FieldID],    [Descending]) VALUES ( @UserID, @TableID, @FieldID, @Descending); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("Descending", SqlDbType.Bit, 1);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SearchSorters] WHERE ([SorterID] = @SorterID);";
		deleteCommand.Parameters.Add("SorterID", SqlDbType.Int);

		try
		{
		  foreach (SearchSorter searchSorter in this)
		  {
			if (searchSorter.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(searchSorter);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = searchSorter.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["SorterID"].AutoIncrement = false;
			  Table.Columns["SorterID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				searchSorter.Row["SorterID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(searchSorter);
			}
			else if (searchSorter.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(searchSorter);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = searchSorter.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(searchSorter);
			}
			else if (searchSorter.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)searchSorter.Row["SorterID", DataRowVersion.Original];
			  deleteCommand.Parameters["SorterID"].Value = id;
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

      foreach (SearchSorter searchSorter in this)
      {
        if (searchSorter.Row.Table.Columns.Contains("CreatorID") && (int)searchSorter.Row["CreatorID"] == 0) searchSorter.Row["CreatorID"] = LoginUser.UserID;
        if (searchSorter.Row.Table.Columns.Contains("ModifierID")) searchSorter.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public SearchSorter FindBySorterID(int sorterID)
    {
      foreach (SearchSorter searchSorter in this)
      {
        if (searchSorter.SorterID == sorterID)
        {
          return searchSorter;
        }
      }
      return null;
    }

    public virtual SearchSorter AddNewSearchSorter()
    {
      if (Table.Columns.Count < 1) LoadColumns("SearchSorters");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new SearchSorter(row, this);
    }
    
    public virtual void LoadBySorterID(int sorterID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [SorterID], [UserID], [TableID], [FieldID], [Descending] FROM [dbo].[SearchSorters] WHERE ([SorterID] = @SorterID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("SorterID", sorterID);
        Fill(command);
      }
    }
    
    public static SearchSorter GetSearchSorter(LoginUser loginUser, int sorterID)
    {
      SearchSorters searchSorters = new SearchSorters(loginUser);
      searchSorters.LoadBySorterID(sorterID);
      if (searchSorters.IsEmpty)
        return null;
      else
        return searchSorters[0];
    }
    
    
    

    #endregion

    #region IEnumerable<SearchSorter> Members

    public IEnumerator<SearchSorter> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new SearchSorter(row, this);
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
