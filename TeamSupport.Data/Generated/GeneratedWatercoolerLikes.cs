using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class WatercoolerLike : BaseItem
  {
    private WatercoolerLikes _watercoolerLikes;
    
    public WatercoolerLike(DataRow row, WatercoolerLikes watercoolerLikes): base(row, watercoolerLikes)
    {
      _watercoolerLikes = watercoolerLikes;
    }
	
    #region Properties
    
    public WatercoolerLikes Collection
    {
      get { return _watercoolerLikes; }
    }
        
    
    
    

    

    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID",value); }
    }
    
    public int MessageID
    {
      get { return (int)Row["MessageID"]; }
        set { Row["MessageID"] = CheckValue("MessageID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
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

  public partial class WatercoolerLikes : BaseCollection, IEnumerable<WatercoolerLike>
  {
    public WatercoolerLikes(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "WatercoolerLikes"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "MessageID"; }
    }



    public WatercoolerLike this[int index]
    {
      get { return new WatercoolerLike(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(WatercoolerLike watercoolerLik);
    partial void AfterRowInsert(WatercoolerLike watercoolerLik);
    partial void BeforeRowEdit(WatercoolerLike watercoolerLik);
    partial void AfterRowEdit(WatercoolerLike watercoolerLik);
    partial void BeforeRowDelete(int messageID);
    partial void AfterRowDelete(int messageID);    

    partial void BeforeDBDelete(int messageID);
    partial void AfterDBDelete(int messageID);    

    #endregion

    #region Public Methods

    public WatercoolerLikProxy[] GetWatercoolerLikProxies()
    {
      List<WatercoolerLikProxy> list = new List<WatercoolerLikProxy>();

      foreach (WatercoolerLike item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int messageID)
    {
      BeforeDBDelete(messageID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[WatercoolerLikes] WHERE ([MessageID] = @MessageID AND [UserID] = @UserID);";
        deleteCommand.Parameters.Add("MessageID", SqlDbType.Int);
        deleteCommand.Parameters["MessageID"].Value = messageID;

        BeforeRowDelete(messageID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(messageID);
      }
      AfterDBDelete(messageID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("WatercoolerLikesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[WatercoolerLikes] SET   WHERE ([MessageID] = @MessageID AND [UserID] = @UserID);";

		
		tempParameter = updateCommand.Parameters.Add("MessageID", SqlDbType.Int, 4);
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
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[WatercoolerLikes] (    [MessageID],    [UserID],    [DateCreated]) VALUES ( @MessageID, @UserID, @DateCreated); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("MessageID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[WatercoolerLikes] WHERE ([MessageID] = @MessageID AND [UserID] = @UserID);";
		deleteCommand.Parameters.Add("MessageID", SqlDbType.Int);

		try
		{
		  foreach (WatercoolerLike watercoolerLik in this)
		  {
			if (watercoolerLik.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(watercoolerLik);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = watercoolerLik.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["MessageID"].AutoIncrement = false;
			  Table.Columns["MessageID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				watercoolerLik.Row["MessageID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(watercoolerLik);
			}
			else if (watercoolerLik.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(watercoolerLik);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = watercoolerLik.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(watercoolerLik);
			}
			else if (watercoolerLik.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)watercoolerLik.Row["MessageID", DataRowVersion.Original];
			  deleteCommand.Parameters["MessageID"].Value = id;
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

      foreach (WatercoolerLike watercoolerLik in this)
      {
        if (watercoolerLik.Row.Table.Columns.Contains("CreatorID") && (int)watercoolerLik.Row["CreatorID"] == 0) watercoolerLik.Row["CreatorID"] = LoginUser.UserID;
        if (watercoolerLik.Row.Table.Columns.Contains("ModifierID")) watercoolerLik.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public WatercoolerLike FindByMessageID(int messageID)
    {
      foreach (WatercoolerLike watercoolerLik in this)
      {
        if (watercoolerLik.MessageID == messageID)
        {
          return watercoolerLik;
        }
      }
      return null;
    }

    public virtual WatercoolerLike AddNewWatercoolerLik()
    {
      if (Table.Columns.Count < 1) LoadColumns("WatercoolerLikes");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new WatercoolerLike(row, this);
    }
    
    public virtual void LoadByMessageID(int messageID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [MessageID], [UserID], [DateCreated] FROM [dbo].[WatercoolerLikes] WHERE ([MessageID] = @MessageID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("MessageID", messageID);
        Fill(command);
      }
    }
    
    public static WatercoolerLike GetWatercoolerLik(LoginUser loginUser, int messageID)
    {
      WatercoolerLikes watercoolerLikes = new WatercoolerLikes(loginUser);
      watercoolerLikes.LoadByMessageID(messageID);
      if (watercoolerLikes.IsEmpty)
        return null;
      else
        return watercoolerLikes[0];
    }
    
    
    

    #endregion

    #region IEnumerable<WatercoolerLik> Members

    public IEnumerator<WatercoolerLike> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new WatercoolerLike(row, this);
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
