using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class SlaViolationHistoryItem : BaseItem
  {
    private SlaViolationHistory _slaViolationHistory;
    
    public SlaViolationHistoryItem(DataRow row, SlaViolationHistory slaViolationHistory): base(row, slaViolationHistory)
    {
      _slaViolationHistory = slaViolationHistory;
    }
	
    #region Properties
    
    public SlaViolationHistory Collection
    {
      get { return _slaViolationHistory; }
    }
        
    
    
    
    public int SlaViolationHistoryID
    {
      get { return (int)Row["SlaViolationHistoryID"]; }
    }
    

    
    public int? UserID
    {
      get { return Row["UserID"] != DBNull.Value ? (int?)Row["UserID"] : null; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    
    public int? GroupID
    {
      get { return Row["GroupID"] != DBNull.Value ? (int?)Row["GroupID"] : null; }
      set { Row["GroupID"] = CheckValue("GroupID", value); }
    }
    
    public int? SlaTriggerId
    {
      get { return Row["SlaTriggerId"] != DBNull.Value ? (int?)Row["SlaTriggerId"] : null; }
      set { Row["SlaTriggerId"] = CheckValue("SlaTriggerId", value); }
    }
    

    
    public SlaViolationType ViolationType
    {
      get { return (SlaViolationType)Row["ViolationType"]; }
      set { Row["ViolationType"] = CheckValue("ViolationType", value); }
    }
    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateViolated
    {
      get { return DateToLocal((DateTime)Row["DateViolated"]); }
      set { Row["DateViolated"] = CheckValue("DateViolated", value); }
    }

    public DateTime DateViolatedUtc
    {
      get { return (DateTime)Row["DateViolated"]; }
    }
    

    #endregion
    
    
  }

  public partial class SlaViolationHistory : BaseCollection, IEnumerable<SlaViolationHistoryItem>
  {
    public SlaViolationHistory(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "SlaViolationHistory"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "SlaViolationHistoryID"; }
    }



    public SlaViolationHistoryItem this[int index]
    {
      get { return new SlaViolationHistoryItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(SlaViolationHistoryItem slaViolationHistoryItem);
    partial void AfterRowInsert(SlaViolationHistoryItem slaViolationHistoryItem);
    partial void BeforeRowEdit(SlaViolationHistoryItem slaViolationHistoryItem);
    partial void AfterRowEdit(SlaViolationHistoryItem slaViolationHistoryItem);
    partial void BeforeRowDelete(int slaViolationHistoryID);
    partial void AfterRowDelete(int slaViolationHistoryID);    

    partial void BeforeDBDelete(int slaViolationHistoryID);
    partial void AfterDBDelete(int slaViolationHistoryID);    

    #endregion

    #region Public Methods

    public SlaViolationHistoryItemProxy[] GetSlaViolationHistoryItemProxies()
    {
      List<SlaViolationHistoryItemProxy> list = new List<SlaViolationHistoryItemProxy>();

      foreach (SlaViolationHistoryItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int slaViolationHistoryID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SlaViolationHistory] WHERE ([SlaViolationHistoryID] = @SlaViolationHistoryID);";
        deleteCommand.Parameters.Add("SlaViolationHistoryID", SqlDbType.Int);
        deleteCommand.Parameters["SlaViolationHistoryID"].Value = slaViolationHistoryID;

        BeforeDBDelete(slaViolationHistoryID);
        BeforeRowDelete(slaViolationHistoryID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(slaViolationHistoryID);
        AfterDBDelete(slaViolationHistoryID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("SlaViolationHistorySave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[SlaViolationHistory] SET     [UserID] = @UserID,    [GroupID] = @GroupID,    [TicketID] = @TicketID,    [ViolationType] = @ViolationType,    [DateViolated] = @DateViolated,    [SlaTriggerId] = @SlaTriggerId  WHERE ([SlaViolationHistoryID] = @SlaViolationHistoryID);";

		
		tempParameter = updateCommand.Parameters.Add("SlaViolationHistoryID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("GroupID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ViolationType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateViolated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaTriggerId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[SlaViolationHistory] (    [UserID],    [GroupID],    [TicketID],    [ViolationType],    [DateViolated],    [SlaTriggerId]) VALUES ( @UserID, @GroupID, @TicketID, @ViolationType, @DateViolated, @SlaTriggerId); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("SlaTriggerId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateViolated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("ViolationType", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("GroupID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SlaViolationHistory] WHERE ([SlaViolationHistoryID] = @SlaViolationHistoryID);";
		deleteCommand.Parameters.Add("SlaViolationHistoryID", SqlDbType.Int);

		try
		{
		  foreach (SlaViolationHistoryItem slaViolationHistoryItem in this)
		  {
			if (slaViolationHistoryItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(slaViolationHistoryItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = slaViolationHistoryItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["SlaViolationHistoryID"].AutoIncrement = false;
			  Table.Columns["SlaViolationHistoryID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				slaViolationHistoryItem.Row["SlaViolationHistoryID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(slaViolationHistoryItem);
			}
			else if (slaViolationHistoryItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(slaViolationHistoryItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = slaViolationHistoryItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(slaViolationHistoryItem);
			}
			else if (slaViolationHistoryItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)slaViolationHistoryItem.Row["SlaViolationHistoryID", DataRowVersion.Original];
			  deleteCommand.Parameters["SlaViolationHistoryID"].Value = id;
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

      foreach (SlaViolationHistoryItem slaViolationHistoryItem in this)
      {
        if (slaViolationHistoryItem.Row.Table.Columns.Contains("CreatorID") && (int)slaViolationHistoryItem.Row["CreatorID"] == 0) slaViolationHistoryItem.Row["CreatorID"] = LoginUser.UserID;
        if (slaViolationHistoryItem.Row.Table.Columns.Contains("ModifierID")) slaViolationHistoryItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public SlaViolationHistoryItem FindBySlaViolationHistoryID(int slaViolationHistoryID)
    {
      foreach (SlaViolationHistoryItem slaViolationHistoryItem in this)
      {
        if (slaViolationHistoryItem.SlaViolationHistoryID == slaViolationHistoryID)
        {
          return slaViolationHistoryItem;
        }
      }
      return null;
    }

    public virtual SlaViolationHistoryItem AddNewSlaViolationHistoryItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("SlaViolationHistory");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new SlaViolationHistoryItem(row, this);
    }
    
    public virtual void LoadBySlaViolationHistoryID(int slaViolationHistoryID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [SlaViolationHistoryID], [UserID], [GroupID], [TicketID], [ViolationType], [DateViolated], [SlaTriggerId] FROM [dbo].[SlaViolationHistory] WHERE ([SlaViolationHistoryID] = @SlaViolationHistoryID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("SlaViolationHistoryID", slaViolationHistoryID);
        Fill(command);
      }
    }
    
    public static SlaViolationHistoryItem GetSlaViolationHistoryItem(LoginUser loginUser, int slaViolationHistoryID)
    {
      SlaViolationHistory slaViolationHistory = new SlaViolationHistory(loginUser);
      slaViolationHistory.LoadBySlaViolationHistoryID(slaViolationHistoryID);
      if (slaViolationHistory.IsEmpty)
        return null;
      else
        return slaViolationHistory[0];
    }
    
    
    

    #endregion

    #region IEnumerable<SlaViolationHistoryItem> Members

    public IEnumerator<SlaViolationHistoryItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new SlaViolationHistoryItem(row, this);
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
