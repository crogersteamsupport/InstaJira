using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class AttachmentDownload : BaseItem
  {
    private AttachmentDownloads _attachmentDownloads;
    
    public AttachmentDownload(DataRow row, AttachmentDownloads attachmentDownloads): base(row, attachmentDownloads)
    {
      _attachmentDownloads = attachmentDownloads;
    }
	
    #region Properties
    
    public AttachmentDownloads Collection
    {
      get { return _attachmentDownloads; }
    }
        
    
    
    
    public int AttachmentDownloadID
    {
      get { return (int)Row["AttachmentDownloadID"]; }
    }
    

    
    public int? AttachmentOrganizationID
    {
      get { return Row["AttachmentOrganizationID"] != DBNull.Value ? (int?)Row["AttachmentOrganizationID"] : null; }
      set { Row["AttachmentOrganizationID"] = CheckValue("AttachmentOrganizationID", value); }
    }
    

    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    
    public int AttachmentID
    {
      get { return (int)Row["AttachmentID"]; }
      set { Row["AttachmentID"] = CheckValue("AttachmentID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateDownloaded
    {
      get { return DateToLocal((DateTime)Row["DateDownloaded"]); }
      set { Row["DateDownloaded"] = CheckValue("DateDownloaded", value); }
    }

    public DateTime DateDownloadedUtc
    {
      get { return (DateTime)Row["DateDownloaded"]; }
    }
    

    #endregion
    
    
  }

  public partial class AttachmentDownloads : BaseCollection, IEnumerable<AttachmentDownload>
  {
    public AttachmentDownloads(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "AttachmentDownloads"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "AttachmentDownloadID"; }
    }



    public AttachmentDownload this[int index]
    {
      get { return new AttachmentDownload(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(AttachmentDownload attachmentDownload);
    partial void AfterRowInsert(AttachmentDownload attachmentDownload);
    partial void BeforeRowEdit(AttachmentDownload attachmentDownload);
    partial void AfterRowEdit(AttachmentDownload attachmentDownload);
    partial void BeforeRowDelete(int attachmentDownloadID);
    partial void AfterRowDelete(int attachmentDownloadID);    

    partial void BeforeDBDelete(int attachmentDownloadID);
    partial void AfterDBDelete(int attachmentDownloadID);    

    #endregion

    #region Public Methods

    public AttachmentDownloadProxy[] GetAttachmentDownloadProxies()
    {
      List<AttachmentDownloadProxy> list = new List<AttachmentDownloadProxy>();

      foreach (AttachmentDownload item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int attachmentDownloadID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[AttachmentDownloads] WHERE ([AttachmentDownloadID] = @AttachmentDownloadID);";
        deleteCommand.Parameters.Add("AttachmentDownloadID", SqlDbType.Int);
        deleteCommand.Parameters["AttachmentDownloadID"].Value = attachmentDownloadID;

        BeforeDBDelete(attachmentDownloadID);
        BeforeRowDelete(attachmentDownloadID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(attachmentDownloadID);
        AfterDBDelete(attachmentDownloadID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("AttachmentDownloadsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[AttachmentDownloads] SET     [AttachmentOrganizationID] = @AttachmentOrganizationID,    [AttachmentID] = @AttachmentID,    [UserID] = @UserID,    [DateDownloaded] = @DateDownloaded  WHERE ([AttachmentDownloadID] = @AttachmentDownloadID);";

		
		tempParameter = updateCommand.Parameters.Add("AttachmentDownloadID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("AttachmentOrganizationID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateDownloaded", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[AttachmentDownloads] (    [AttachmentOrganizationID],    [AttachmentID],    [UserID],    [DateDownloaded]) VALUES ( @AttachmentOrganizationID, @AttachmentID, @UserID, @DateDownloaded); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateDownloaded", SqlDbType.DateTime, 8);
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
		
		tempParameter = insertCommand.Parameters.Add("AttachmentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("AttachmentOrganizationID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[AttachmentDownloads] WHERE ([AttachmentDownloadID] = @AttachmentDownloadID);";
		deleteCommand.Parameters.Add("AttachmentDownloadID", SqlDbType.Int);

		try
		{
		  foreach (AttachmentDownload attachmentDownload in this)
		  {
			if (attachmentDownload.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(attachmentDownload);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = attachmentDownload.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["AttachmentDownloadID"].AutoIncrement = false;
			  Table.Columns["AttachmentDownloadID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				attachmentDownload.Row["AttachmentDownloadID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(attachmentDownload);
			}
			else if (attachmentDownload.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(attachmentDownload);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = attachmentDownload.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(attachmentDownload);
			}
			else if (attachmentDownload.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)attachmentDownload.Row["AttachmentDownloadID", DataRowVersion.Original];
			  deleteCommand.Parameters["AttachmentDownloadID"].Value = id;
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

      foreach (AttachmentDownload attachmentDownload in this)
      {
        if (attachmentDownload.Row.Table.Columns.Contains("CreatorID") && (int)attachmentDownload.Row["CreatorID"] == 0) attachmentDownload.Row["CreatorID"] = LoginUser.UserID;
        if (attachmentDownload.Row.Table.Columns.Contains("ModifierID")) attachmentDownload.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public AttachmentDownload FindByAttachmentDownloadID(int attachmentDownloadID)
    {
      foreach (AttachmentDownload attachmentDownload in this)
      {
        if (attachmentDownload.AttachmentDownloadID == attachmentDownloadID)
        {
          return attachmentDownload;
        }
      }
      return null;
    }

    public virtual AttachmentDownload AddNewAttachmentDownload()
    {
      if (Table.Columns.Count < 1) LoadColumns("AttachmentDownloads");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new AttachmentDownload(row, this);
    }
    
    public virtual void LoadByAttachmentDownloadID(int attachmentDownloadID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [AttachmentDownloadID], [AttachmentOrganizationID], [AttachmentID], [UserID], [DateDownloaded] FROM [dbo].[AttachmentDownloads] WHERE ([AttachmentDownloadID] = @AttachmentDownloadID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("AttachmentDownloadID", attachmentDownloadID);
        Fill(command);
      }
    }
    
    public static AttachmentDownload GetAttachmentDownload(LoginUser loginUser, int attachmentDownloadID)
    {
      AttachmentDownloads attachmentDownloads = new AttachmentDownloads(loginUser);
      attachmentDownloads.LoadByAttachmentDownloadID(attachmentDownloadID);
      if (attachmentDownloads.IsEmpty)
        return null;
      else
        return attachmentDownloads[0];
    }
    
    
    

    #endregion

    #region IEnumerable<AttachmentDownload> Members

    public IEnumerator<AttachmentDownload> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new AttachmentDownload(row, this);
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
