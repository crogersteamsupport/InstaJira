using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class SearchStandardFilter : BaseItem
  {
    private SearchStandardFilters _searchStandardFilters;
    
    public SearchStandardFilter(DataRow row, SearchStandardFilters searchStandardFilters): base(row, searchStandardFilters)
    {
      _searchStandardFilters = searchStandardFilters;
    }
	
    #region Properties
    
    public SearchStandardFilters Collection
    {
      get { return _searchStandardFilters; }
    }
        
    
    
    
    public int StandardFilterID
    {
      get { return (int)Row["StandardFilterID"]; }
    }
    

    

    
    public bool Tasks
    {
      get { return (bool)Row["Tasks"]; }
      set { Row["Tasks"] = CheckValue("Tasks", value); }
    }
    
    public bool WaterCooler
    {
      get { return (bool)Row["WaterCooler"]; }
      set { Row["WaterCooler"] = CheckValue("WaterCooler", value); }
    }
    
    public bool ProductVersions
    {
      get { return (bool)Row["ProductVersions"]; }
      set { Row["ProductVersions"] = CheckValue("ProductVersions", value); }
    }
    
    public bool Notes
    {
      get { return (bool)Row["Notes"]; }
      set { Row["Notes"] = CheckValue("Notes", value); }
    }
    
    public bool Wikis
    {
      get { return (bool)Row["Wikis"]; }
      set { Row["Wikis"] = CheckValue("Wikis", value); }
    }
    
    public bool KnowledgeBase
    {
      get { return (bool)Row["KnowledgeBase"]; }
      set { Row["KnowledgeBase"] = CheckValue("KnowledgeBase", value); }
    }
    
    public bool Tickets
    {
      get { return (bool)Row["Tickets"]; }
      set { Row["Tickets"] = CheckValue("Tickets", value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class SearchStandardFilters : BaseCollection, IEnumerable<SearchStandardFilter>
  {
    public SearchStandardFilters(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "SearchStandardFilters"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "StandardFilterID"; }
    }



    public SearchStandardFilter this[int index]
    {
      get { return new SearchStandardFilter(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(SearchStandardFilter searchStandardFilter);
    partial void AfterRowInsert(SearchStandardFilter searchStandardFilter);
    partial void BeforeRowEdit(SearchStandardFilter searchStandardFilter);
    partial void AfterRowEdit(SearchStandardFilter searchStandardFilter);
    partial void BeforeRowDelete(int standardFilterID);
    partial void AfterRowDelete(int standardFilterID);    

    partial void BeforeDBDelete(int standardFilterID);
    partial void AfterDBDelete(int standardFilterID);    

    #endregion

    #region Public Methods

    public SearchStandardFilterProxy[] GetSearchStandardFilterProxies()
    {
      List<SearchStandardFilterProxy> list = new List<SearchStandardFilterProxy>();

      foreach (SearchStandardFilter item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int standardFilterID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SearchStandardFilters] WHERE ([StandardFilterID] = @StandardFilterID);";
        deleteCommand.Parameters.Add("StandardFilterID", SqlDbType.Int);
        deleteCommand.Parameters["StandardFilterID"].Value = standardFilterID;

        BeforeDBDelete(standardFilterID);
        BeforeRowDelete(standardFilterID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(standardFilterID);
        AfterDBDelete(standardFilterID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("SearchStandardFiltersSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[SearchStandardFilters] SET     [UserID] = @UserID,    [Tickets] = @Tickets,    [KnowledgeBase] = @KnowledgeBase,    [Wikis] = @Wikis,    [Notes] = @Notes,    [ProductVersions] = @ProductVersions,    [WaterCooler] = @WaterCooler,    [Tasks] = @Tasks  WHERE ([StandardFilterID] = @StandardFilterID);";

		
		tempParameter = updateCommand.Parameters.Add("StandardFilterID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Tickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("KnowledgeBase", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Wikis", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Notes", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ProductVersions", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("WaterCooler", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Tasks", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[SearchStandardFilters] (    [UserID],    [Tickets],    [KnowledgeBase],    [Wikis],    [Notes],    [ProductVersions],    [WaterCooler],    [Tasks]) VALUES ( @UserID, @Tickets, @KnowledgeBase, @Wikis, @Notes, @ProductVersions, @WaterCooler, @Tasks); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("Tasks", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("WaterCooler", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ProductVersions", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Notes", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Wikis", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("KnowledgeBase", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Tickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SearchStandardFilters] WHERE ([StandardFilterID] = @StandardFilterID);";
		deleteCommand.Parameters.Add("StandardFilterID", SqlDbType.Int);

		try
		{
		  foreach (SearchStandardFilter searchStandardFilter in this)
		  {
			if (searchStandardFilter.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(searchStandardFilter);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = searchStandardFilter.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["StandardFilterID"].AutoIncrement = false;
			  Table.Columns["StandardFilterID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				searchStandardFilter.Row["StandardFilterID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(searchStandardFilter);
			}
			else if (searchStandardFilter.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(searchStandardFilter);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = searchStandardFilter.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(searchStandardFilter);
			}
			else if (searchStandardFilter.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)searchStandardFilter.Row["StandardFilterID", DataRowVersion.Original];
			  deleteCommand.Parameters["StandardFilterID"].Value = id;
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

      foreach (SearchStandardFilter searchStandardFilter in this)
      {
        if (searchStandardFilter.Row.Table.Columns.Contains("CreatorID") && (int)searchStandardFilter.Row["CreatorID"] == 0) searchStandardFilter.Row["CreatorID"] = LoginUser.UserID;
        if (searchStandardFilter.Row.Table.Columns.Contains("ModifierID")) searchStandardFilter.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public SearchStandardFilter FindByStandardFilterID(int standardFilterID)
    {
      foreach (SearchStandardFilter searchStandardFilter in this)
      {
        if (searchStandardFilter.StandardFilterID == standardFilterID)
        {
          return searchStandardFilter;
        }
      }
      return null;
    }

    public virtual SearchStandardFilter AddNewSearchStandardFilter()
    {
      if (Table.Columns.Count < 1) LoadColumns("SearchStandardFilters");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new SearchStandardFilter(row, this);
    }
    
    public virtual void LoadByStandardFilterID(int standardFilterID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [StandardFilterID], [UserID], [Tickets], [KnowledgeBase], [Wikis], [Notes], [ProductVersions], [WaterCooler], [Tasks] FROM [dbo].[SearchStandardFilters] WHERE ([StandardFilterID] = @StandardFilterID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("StandardFilterID", standardFilterID);
        Fill(command);
      }
    }
    
    public static SearchStandardFilter GetSearchStandardFilter(LoginUser loginUser, int standardFilterID)
    {
      SearchStandardFilters searchStandardFilters = new SearchStandardFilters(loginUser);
      searchStandardFilters.LoadByStandardFilterID(standardFilterID);
      if (searchStandardFilters.IsEmpty)
        return null;
      else
        return searchStandardFilters[0];
    }
    
    
    

    #endregion

    #region IEnumerable<SearchStandardFilter> Members

    public IEnumerator<SearchStandardFilter> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new SearchStandardFilter(row, this);
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
