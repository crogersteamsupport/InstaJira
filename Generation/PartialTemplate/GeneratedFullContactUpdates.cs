using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class FullContactUpdatesItem : BaseItem
  {
    private FullContactUpdates _fullContactUpdates;
    
    public FullContactUpdatesItem(DataRow row, FullContactUpdates fullContactUpdates): base(row, fullContactUpdates)
    {
      _fullContactUpdates = fullContactUpdates;
    }
	
    #region Properties
    
    public FullContactUpdates Collection
    {
      get { return _fullContactUpdates; }
    }
        
    
    
    
    public int Id
    {
      get { return (int)Row["Id"]; }
    }
    

    
    public int? UserId
    {
      get { return Row["UserId"] != DBNull.Value ? (int?)Row["UserId"] : null; }
      set { Row["UserId"] = CheckValue("UserId", value); }
    }
    
    public int? OrganizationId
    {
      get { return Row["OrganizationId"] != DBNull.Value ? (int?)Row["OrganizationId"] : null; }
      set { Row["OrganizationId"] = CheckValue("OrganizationId", value); }
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
    

    #endregion
    
    
  }

  public partial class FullContactUpdates : BaseCollection, IEnumerable<FullContactUpdatesItem>
  {
    public FullContactUpdates(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "FullContactUpdates"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "Id"; }
    }



    public FullContactUpdatesItem this[int index]
    {
      get { return new FullContactUpdatesItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(FullContactUpdatesItem fullContactUpdatesItem);
    partial void AfterRowInsert(FullContactUpdatesItem fullContactUpdatesItem);
    partial void BeforeRowEdit(FullContactUpdatesItem fullContactUpdatesItem);
    partial void AfterRowEdit(FullContactUpdatesItem fullContactUpdatesItem);
    partial void BeforeRowDelete(int id);
    partial void AfterRowDelete(int id);    

    partial void BeforeDBDelete(int id);
    partial void AfterDBDelete(int id);    

    #endregion

    #region Public Methods

    public FullContactUpdatesItemProxy[] GetFullContactUpdatesItemProxies()
    {
      List<FullContactUpdatesItemProxy> list = new List<FullContactUpdatesItemProxy>();

      foreach (FullContactUpdatesItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int id)
    {
      BeforeDBDelete(id);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[FullContactUpdates] WHERE ([Id] = @Id);";
        deleteCommand.Parameters.Add("Id", SqlDbType.Int);
        deleteCommand.Parameters["Id"].Value = id;

        BeforeRowDelete(id);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(id);
      }
      AfterDBDelete(id);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("FullContactUpdatesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[FullContactUpdates] SET     [UserId] = @UserId,    [OrganizationId] = @OrganizationId,    [DateModified] = @DateModified  WHERE ([Id] = @Id);";

		
		tempParameter = updateCommand.Parameters.Add("Id", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[FullContactUpdates] (    [UserId],    [OrganizationId],    [DateModified]) VALUES ( @UserId, @OrganizationId, @DateModified); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserId", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[FullContactUpdates] WHERE ([Id] = @Id);";
		deleteCommand.Parameters.Add("Id", SqlDbType.Int);

		try
		{
		  foreach (FullContactUpdatesItem fullContactUpdatesItem in this)
		  {
			if (fullContactUpdatesItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(fullContactUpdatesItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = fullContactUpdatesItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["Id"].AutoIncrement = false;
			  Table.Columns["Id"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				fullContactUpdatesItem.Row["Id"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(fullContactUpdatesItem);
			}
			else if (fullContactUpdatesItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(fullContactUpdatesItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = fullContactUpdatesItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(fullContactUpdatesItem);
			}
			else if (fullContactUpdatesItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)fullContactUpdatesItem.Row["Id", DataRowVersion.Original];
			  deleteCommand.Parameters["Id"].Value = id;
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

      foreach (FullContactUpdatesItem fullContactUpdatesItem in this)
      {
        if (fullContactUpdatesItem.Row.Table.Columns.Contains("CreatorID") && (int)fullContactUpdatesItem.Row["CreatorID"] == 0) fullContactUpdatesItem.Row["CreatorID"] = LoginUser.UserID;
        if (fullContactUpdatesItem.Row.Table.Columns.Contains("ModifierID")) fullContactUpdatesItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public FullContactUpdatesItem FindById(int id)
    {
      foreach (FullContactUpdatesItem fullContactUpdatesItem in this)
      {
        if (fullContactUpdatesItem.Id == id)
        {
          return fullContactUpdatesItem;
        }
      }
      return null;
    }

    public virtual FullContactUpdatesItem AddNewFullContactUpdatesItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("FullContactUpdates");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new FullContactUpdatesItem(row, this);
    }
    
    public virtual void LoadById(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [Id], [UserId], [OrganizationId], [DateModified] FROM [dbo].[FullContactUpdates] WHERE ([Id] = @Id);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("Id", id);
        Fill(command);
      }
    }
    
    public static FullContactUpdatesItem GetFullContactUpdatesItem(LoginUser loginUser, int id)
    {
      FullContactUpdates fullContactUpdates = new FullContactUpdates(loginUser);
      fullContactUpdates.LoadById(id);
      if (fullContactUpdates.IsEmpty)
        return null;
      else
        return fullContactUpdates[0];
    }
    
    
    

    #endregion

    #region IEnumerable<FullContactUpdatesItem> Members

    public IEnumerator<FullContactUpdatesItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new FullContactUpdatesItem(row, this);
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
