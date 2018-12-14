using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CompanyParentsViewItem : BaseItem
  {
    private CompanyParentsView _companyParentsView;
    
    public CompanyParentsViewItem(DataRow row, CompanyParentsView companyParentsView): base(row, companyParentsView)
    {
      _companyParentsView = companyParentsView;
    }
	
    #region Properties
    
    public CompanyParentsView Collection
    {
      get { return _companyParentsView; }
    }
        
    
    
    

    

    
    public string ParentName
    {
      get { return (string)Row["ParentName"]; }
      set { Row["ParentName"] = CheckValue("ParentName", value); }
    }
    
    public int ParentID
    {
      get { return (int)Row["ParentID"]; }
      set { Row["ParentID"] = CheckValue("ParentID", value); }
    }
    
    public int ChildID
    {
      get { return (int)Row["ChildID"]; }
      set { Row["ChildID"] = CheckValue("ChildID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class CompanyParentsView : BaseCollection, IEnumerable<CompanyParentsViewItem>
  {
    public CompanyParentsView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CompanyParentsView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ChildID"; }
    }



    public CompanyParentsViewItem this[int index]
    {
      get { return new CompanyParentsViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CompanyParentsViewItem companyParentsViewItem);
    partial void AfterRowInsert(CompanyParentsViewItem companyParentsViewItem);
    partial void BeforeRowEdit(CompanyParentsViewItem companyParentsViewItem);
    partial void AfterRowEdit(CompanyParentsViewItem companyParentsViewItem);
    partial void BeforeRowDelete(int childID);
    partial void AfterRowDelete(int childID);    

    partial void BeforeDBDelete(int childID);
    partial void AfterDBDelete(int childID);    

    #endregion

    #region Public Methods

    public CompanyParentsViewItemProxy[] GetCompanyParentsViewItemProxies()
    {
      List<CompanyParentsViewItemProxy> list = new List<CompanyParentsViewItemProxy>();

      foreach (CompanyParentsViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int childID)
    {
      BeforeDBDelete(childID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CompanyParentsView] WHERE ([ChildID] = @ChildID);";
        deleteCommand.Parameters.Add("ChildID", SqlDbType.Int);
        deleteCommand.Parameters["ChildID"].Value = childID;

        BeforeRowDelete(childID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(childID);
      }
      AfterDBDelete(childID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CompanyParentsViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CompanyParentsView] SET     [ParentID] = @ParentID,    [ParentName] = @ParentName  WHERE ([ChildID] = @ChildID);";

		
		tempParameter = updateCommand.Parameters.Add("ChildID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ParentName", SqlDbType.NVarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CompanyParentsView] (    [ChildID],    [ParentID],    [ParentName]) VALUES ( @ChildID, @ParentID, @ParentName); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ParentName", SqlDbType.NVarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ChildID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CompanyParentsView] WHERE ([ChildID] = @ChildID);";
		deleteCommand.Parameters.Add("ChildID", SqlDbType.Int);

		try
		{
		  foreach (CompanyParentsViewItem companyParentsViewItem in this)
		  {
			if (companyParentsViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(companyParentsViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = companyParentsViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ChildID"].AutoIncrement = false;
			  Table.Columns["ChildID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				companyParentsViewItem.Row["ChildID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(companyParentsViewItem);
			}
			else if (companyParentsViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(companyParentsViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = companyParentsViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(companyParentsViewItem);
			}
			else if (companyParentsViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)companyParentsViewItem.Row["ChildID", DataRowVersion.Original];
			  deleteCommand.Parameters["ChildID"].Value = id;
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

      foreach (CompanyParentsViewItem companyParentsViewItem in this)
      {
        if (companyParentsViewItem.Row.Table.Columns.Contains("CreatorID") && (int)companyParentsViewItem.Row["CreatorID"] == 0) companyParentsViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (companyParentsViewItem.Row.Table.Columns.Contains("ModifierID")) companyParentsViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CompanyParentsViewItem FindByChildID(int childID)
    {
      foreach (CompanyParentsViewItem companyParentsViewItem in this)
      {
        if (companyParentsViewItem.ChildID == childID)
        {
          return companyParentsViewItem;
        }
      }
      return null;
    }

    public virtual CompanyParentsViewItem AddNewCompanyParentsViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("CompanyParentsView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CompanyParentsViewItem(row, this);
    }
    
    public virtual void LoadByChildID(int childID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ChildID], [ParentID], [ParentName] FROM [dbo].[CompanyParentsView] WHERE ([ChildID] = @ChildID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ChildID", childID);
        Fill(command);
      }
    }
    
    public static CompanyParentsViewItem GetCompanyParentsViewItem(LoginUser loginUser, int childID)
    {
      CompanyParentsView companyParentsView = new CompanyParentsView(loginUser);
      companyParentsView.LoadByChildID(childID);
      if (companyParentsView.IsEmpty)
        return null;
      else
        return companyParentsView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CompanyParentsViewItem> Members

    public IEnumerator<CompanyParentsViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CompanyParentsViewItem(row, this);
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
