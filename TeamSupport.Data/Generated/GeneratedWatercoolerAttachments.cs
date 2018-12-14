using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class WatercoolerAttachment : BaseItem
  {
    private WatercoolerAttachments _watercoolerAttachments;
    
    public WatercoolerAttachment(DataRow row, WatercoolerAttachments watercoolerAttachments): base(row, watercoolerAttachments)
    {
      _watercoolerAttachments = watercoolerAttachments;
    }
	
    #region Properties
    
    public WatercoolerAttachments Collection
    {
      get { return _watercoolerAttachments; }
    }
        
    
    
    

    

    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
        set { Row["CreatorID"] = CheckValue("CreatorID",value); }
    }

    public WaterCoolerAttachmentType RefType
    {
        get { return (WaterCoolerAttachmentType)Row["RefType"]; }
        set { Row["RefType"] = CheckValue("RefType",value); }
    }
    
    public int AttachmentID
    {
      get { return (int)Row["AttachmentID"]; }
        set { Row["AttachmentID"] = CheckValue("AttachmentID",value); }
    }
    
    public int MessageID
    {
      get { return (int)Row["MessageID"]; }
        set { Row["MessageID"] = CheckValue("MessageID",value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateCreated
    {
      get { return DateToLocal((DateTime)Row["DateCreated"]); }
        set { Row["DateCreated"] = CheckValue("DateCreated",value); }
    }

    public DateTime DateCreatedUtc
    {
      get { return (DateTime)Row["DateCreated"]; }
    }
    

    #endregion
    
    
  }

  public partial class WatercoolerAttachments : BaseCollection, IEnumerable<WatercoolerAttachment>
  {
    public WatercoolerAttachments(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "WatercoolerAttachments"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "MessageID"; }
    }



    public WatercoolerAttachment this[int index]
    {
      get { return new WatercoolerAttachment(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(WatercoolerAttachment watercoolerAttachment);
    partial void AfterRowInsert(WatercoolerAttachment watercoolerAttachment);
    partial void BeforeRowEdit(WatercoolerAttachment watercoolerAttachment);
    partial void AfterRowEdit(WatercoolerAttachment watercoolerAttachment);
    partial void BeforeRowDelete(int messageID);
    partial void AfterRowDelete(int messageID);    

    partial void BeforeDBDelete(int messageID);
    partial void AfterDBDelete(int messageID);    

    #endregion

    #region Public Methods

    public WatercoolerAttachmentProxy[] GetWatercoolerAttachmentProxies()
    {
      List<WatercoolerAttachmentProxy> list = new List<WatercoolerAttachmentProxy>();

      foreach (WatercoolerAttachment item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }

    public WatercoolerAttachmentProxy[] GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType type)
    {
        List<WatercoolerAttachmentProxy> list = new List<WatercoolerAttachmentProxy>();

        foreach (WatercoolerAttachment item in this)
        {
            if (item.RefType == type)
            {
                WatercoolerAttachmentProxy wcAtt = item.GetProxy();
                if (wcAtt != null)
                    list.Add(wcAtt);
            }
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
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[WatercoolerAttachments] WHERE ([MessageID] = @MessageID);";
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
		//SqlTransaction transaction = connection.BeginTransaction("WatercoolerAttachmentsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[WatercoolerAttachments] SET     [AttachmentID] = @AttachmentID,    [RefType] = @RefType  WHERE ([MessageID] = @MessageID);";

		
		tempParameter = updateCommand.Parameters.Add("MessageID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("AttachmentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[WatercoolerAttachments] (    [MessageID],    [AttachmentID],    [RefType],    [CreatorID],    [DateCreated]) VALUES ( @MessageID, @AttachmentID, @RefType, @CreatorID, @DateCreated); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("AttachmentID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[WatercoolerAttachments] WHERE ([MessageID] = @MessageID);";
		deleteCommand.Parameters.Add("MessageID", SqlDbType.Int);

		try
		{
		  foreach (WatercoolerAttachment watercoolerAttachment in this)
		  {
			if (watercoolerAttachment.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(watercoolerAttachment);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = watercoolerAttachment.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["MessageID"].AutoIncrement = false;
			  Table.Columns["MessageID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				watercoolerAttachment.Row["MessageID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(watercoolerAttachment);
			}
			else if (watercoolerAttachment.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(watercoolerAttachment);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = watercoolerAttachment.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(watercoolerAttachment);
			}
			else if (watercoolerAttachment.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)watercoolerAttachment.Row["MessageID", DataRowVersion.Original];
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

      foreach (WatercoolerAttachment watercoolerAttachment in this)
      {
        if (watercoolerAttachment.Row.Table.Columns.Contains("CreatorID") && (int)watercoolerAttachment.Row["CreatorID"] == 0) watercoolerAttachment.Row["CreatorID"] = LoginUser.UserID;
        if (watercoolerAttachment.Row.Table.Columns.Contains("ModifierID")) watercoolerAttachment.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public WatercoolerAttachment FindByMessageID(int messageID)
    {
      foreach (WatercoolerAttachment watercoolerAttachment in this)
      {
        if (watercoolerAttachment.MessageID == messageID)
        {
          return watercoolerAttachment;
        }
      }
      return null;
    }

    public virtual WatercoolerAttachment AddNewWatercoolerAttachment()
    {
      if (Table.Columns.Count < 1) LoadColumns("WatercoolerAttachments");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new WatercoolerAttachment(row, this);
    }
    
    public virtual void LoadByMessageID(int messageID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [MessageID], [AttachmentID], [RefType], [CreatorID], [DateCreated] FROM [dbo].[WatercoolerAttachments] WHERE ([MessageID] = @MessageID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("MessageID", messageID);
        Fill(command);
      }
    }
    
    public static WatercoolerAttachment GetWatercoolerAttachment(LoginUser loginUser, int messageID)
    {
      WatercoolerAttachments watercoolerAttachments = new WatercoolerAttachments(loginUser);
      watercoolerAttachments.LoadByMessageID(messageID);
      if (watercoolerAttachments.IsEmpty)
        return null;
      else
        return watercoolerAttachments[0];
    }
    
    
    

    #endregion

    #region IEnumerable<WatercoolerAttachment> Members

    public IEnumerator<WatercoolerAttachment> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new WatercoolerAttachment(row, this);
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
