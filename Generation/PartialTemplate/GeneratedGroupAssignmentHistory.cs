using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class GroupAssignmentHistoryItem : BaseItem
  {
    private GroupAssignmentHistory _groupAssignmentHistory;
    
    public GroupAssignmentHistoryItem(DataRow row, GroupAssignmentHistory groupAssignmentHistory): base(row, groupAssignmentHistory)
    {
      _groupAssignmentHistory = groupAssignmentHistory;
    }
	
    #region Properties
    
    public GroupAssignmentHistory Collection
    {
      get { return _groupAssignmentHistory; }
    }
        
    
    
    
    public int GroupAssignmentHistoryID
    {
      get { return (int)Row["GroupAssignmentHistoryID"]; }
    }
    

    
    public int? GroupID
    {
      get { return Row["GroupID"] != DBNull.Value ? (int?)Row["GroupID"] : null; }
      set { Row["GroupID"] = CheckValue("GroupID", value); }
    }
    

    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateAssigned
    {
      get { return DateToLocal((DateTime)Row["DateAssigned"]); }
      set { Row["DateAssigned"] = CheckValue("DateAssigned", value); }
    }

    public DateTime DateAssignedUtc
    {
      get { return (DateTime)Row["DateAssigned"]; }
    }
    

    #endregion
    
    
  }

  public partial class GroupAssignmentHistory : BaseCollection, IEnumerable<GroupAssignmentHistoryItem>
  {
    public GroupAssignmentHistory(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "GroupAssignmentHistory"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "GroupAssignmentHistoryID"; }
    }



    public GroupAssignmentHistoryItem this[int index]
    {
      get { return new GroupAssignmentHistoryItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(GroupAssignmentHistoryItem groupAssignmentHistoryItem);
    partial void AfterRowInsert(GroupAssignmentHistoryItem groupAssignmentHistoryItem);
    partial void BeforeRowEdit(GroupAssignmentHistoryItem groupAssignmentHistoryItem);
    partial void AfterRowEdit(GroupAssignmentHistoryItem groupAssignmentHistoryItem);
    partial void BeforeRowDelete(int groupAssignmentHistoryID);
    partial void AfterRowDelete(int groupAssignmentHistoryID);    

    partial void BeforeDBDelete(int groupAssignmentHistoryID);
    partial void AfterDBDelete(int groupAssignmentHistoryID);    

    #endregion

    #region Public Methods

    public GroupAssignmentHistoryItemProxy[] GetGroupAssignmentHistoryItemProxies()
    {
      List<GroupAssignmentHistoryItemProxy> list = new List<GroupAssignmentHistoryItemProxy>();

      foreach (GroupAssignmentHistoryItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int groupAssignmentHistoryID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[GroupAssignmentHistory] WHERE ([GroupAssignmentHistoryID] = @GroupAssignmentHistoryID);";
        deleteCommand.Parameters.Add("GroupAssignmentHistoryID", SqlDbType.Int);
        deleteCommand.Parameters["GroupAssignmentHistoryID"].Value = groupAssignmentHistoryID;

        BeforeDBDelete(groupAssignmentHistoryID);
        BeforeRowDelete(groupAssignmentHistoryID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(groupAssignmentHistoryID);
        AfterDBDelete(groupAssignmentHistoryID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("GroupAssignmentHistorySave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[GroupAssignmentHistory] SET     [TicketID] = @TicketID,    [GroupID] = @GroupID,    [DateAssigned] = @DateAssigned  WHERE ([GroupAssignmentHistoryID] = @GroupAssignmentHistoryID);";

		
		tempParameter = updateCommand.Parameters.Add("GroupAssignmentHistoryID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("GroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateAssigned", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[GroupAssignmentHistory] (    [TicketID],    [GroupID],    [DateAssigned]) VALUES ( @TicketID, @GroupID, @DateAssigned); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateAssigned", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("GroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[GroupAssignmentHistory] WHERE ([GroupAssignmentHistoryID] = @GroupAssignmentHistoryID);";
		deleteCommand.Parameters.Add("GroupAssignmentHistoryID", SqlDbType.Int);

		try
		{
		  foreach (GroupAssignmentHistoryItem groupAssignmentHistoryItem in this)
		  {
			if (groupAssignmentHistoryItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(groupAssignmentHistoryItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = groupAssignmentHistoryItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["GroupAssignmentHistoryID"].AutoIncrement = false;
			  Table.Columns["GroupAssignmentHistoryID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				groupAssignmentHistoryItem.Row["GroupAssignmentHistoryID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(groupAssignmentHistoryItem);
			}
			else if (groupAssignmentHistoryItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(groupAssignmentHistoryItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = groupAssignmentHistoryItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(groupAssignmentHistoryItem);
			}
			else if (groupAssignmentHistoryItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)groupAssignmentHistoryItem.Row["GroupAssignmentHistoryID", DataRowVersion.Original];
			  deleteCommand.Parameters["GroupAssignmentHistoryID"].Value = id;
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

      foreach (GroupAssignmentHistoryItem groupAssignmentHistoryItem in this)
      {
        if (groupAssignmentHistoryItem.Row.Table.Columns.Contains("CreatorID") && (int)groupAssignmentHistoryItem.Row["CreatorID"] == 0) groupAssignmentHistoryItem.Row["CreatorID"] = LoginUser.UserID;
        if (groupAssignmentHistoryItem.Row.Table.Columns.Contains("ModifierID")) groupAssignmentHistoryItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public GroupAssignmentHistoryItem FindByGroupAssignmentHistoryID(int groupAssignmentHistoryID)
    {
      foreach (GroupAssignmentHistoryItem groupAssignmentHistoryItem in this)
      {
        if (groupAssignmentHistoryItem.GroupAssignmentHistoryID == groupAssignmentHistoryID)
        {
          return groupAssignmentHistoryItem;
        }
      }
      return null;
    }

    public virtual GroupAssignmentHistoryItem AddNewGroupAssignmentHistoryItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("GroupAssignmentHistory");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new GroupAssignmentHistoryItem(row, this);
    }
    
    public virtual void LoadByGroupAssignmentHistoryID(int groupAssignmentHistoryID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [GroupAssignmentHistoryID], [TicketID], [GroupID], [DateAssigned] FROM [dbo].[GroupAssignmentHistory] WHERE ([GroupAssignmentHistoryID] = @GroupAssignmentHistoryID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("GroupAssignmentHistoryID", groupAssignmentHistoryID);
        Fill(command);
      }
    }
    
    public static GroupAssignmentHistoryItem GetGroupAssignmentHistoryItem(LoginUser loginUser, int groupAssignmentHistoryID)
    {
      GroupAssignmentHistory groupAssignmentHistory = new GroupAssignmentHistory(loginUser);
      groupAssignmentHistory.LoadByGroupAssignmentHistoryID(groupAssignmentHistoryID);
      if (groupAssignmentHistory.IsEmpty)
        return null;
      else
        return groupAssignmentHistory[0];
    }
    
    
    

    #endregion

    #region IEnumerable<GroupAssignmentHistoryItem> Members

    public IEnumerator<GroupAssignmentHistoryItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new GroupAssignmentHistoryItem(row, this);
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
