using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  partial class Attachment : BaseItem
  {
    private Attachments _attachments;
    
    public Attachment(DataRow row, Attachments attachments): base(row, attachments)
    {
      _attachments = attachments;
    }
	
    #region Properties
    
    public Attachments Collection
    {
      get { return _attachments; }
    }
        
    
    
    
    public int AttachmentID
    {
      get { return (int)Row["AttachmentID"]; }
    }
    

    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public int? ProductFamilyID
    {
      get { return Row["ProductFamilyID"] != DBNull.Value ? (int?)Row["ProductFamilyID"] : null; }
      set { Row["ProductFamilyID"] = CheckValue("ProductFamilyID", value); }
    }
    
    public int? FilePathID
    {
      get { return Row["FilePathID"] != DBNull.Value ? (int?)Row["FilePathID"] : null; }
      set { Row["FilePathID"] = CheckValue("FilePathID", value); }
    }
    
    public bool SentToSnow
    {
      get { return (bool)Row["SentToSnow"]; }
      set { Row["SentToSnow"] = CheckValue("SentToSnow", value); }
    }
    
    public bool SentToTFS
    {
      get { return (bool)Row["SentToTFS"]; }
      set { Row["SentToTFS"] = CheckValue("SentToTFS", value); }
    }
    
    public Guid AttachmentGUID
    {
      get { return (Guid)Row["AttachmentGUID"]; }
      set { Row["AttachmentGUID"] = CheckValue("AttachmentGUID", value); }
    }
    
    public bool SentToJira
    {
      get { return (bool)Row["SentToJira"]; }
      set { Row["SentToJira"] = CheckValue("SentToJira", value); }
    }
    
    public int RefID
    {
      get { return (int)Row["RefID"]; }
      set { Row["RefID"] = CheckValue("RefID", value); }
    }
    
    public AttachmentProxy.References RefType
    {
      get { return (AttachmentProxy.References)Row["RefType"]; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    
    public int ModifierID
    {
      get { return (int)Row["ModifierID"]; }
      set { Row["ModifierID"] = CheckValue("ModifierID", value); }
    }
    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public string Path
    {
      get { return (string)Row["Path"]; }
      set { Row["Path"] = CheckValue("Path", value); }
    }
    
    public long FileSize
    {
      get { return (long)Row["FileSize"]; }
      set { Row["FileSize"] = CheckValue("FileSize", value); }
    }
    
    public string FileType
    {
      get { return (string)Row["FileType"]; }
      set { Row["FileType"] = CheckValue("FileType", value); }
    }
    
    public string FileName
    {
      get { return (string)Row["FileName"]; }
      set { Row["FileName"] = CheckValue("FileName", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
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

  internal partial class Attachments : BaseCollection, IEnumerable<Attachment>
  {
    public Attachments(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Attachments"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "AttachmentID"; }
    }



    public Attachment this[int index]
    {
      get { return new Attachment(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Attachment attachment);
    partial void AfterRowInsert(Attachment attachment);
    partial void BeforeRowEdit(Attachment attachment);
    partial void AfterRowEdit(Attachment attachment);
    partial void BeforeRowDelete(int attachmentID);
    partial void AfterRowDelete(int attachmentID);    

    partial void BeforeDBDelete(int attachmentID);
    partial void AfterDBDelete(int attachmentID);    

    #endregion

    #region Public Methods

    public AttachmentProxy[] GetAttachmentProxies()
    {
      List<AttachmentProxy> list = new List<AttachmentProxy>();

      foreach (Attachment item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int attachmentID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Attachments] WHERE ([AttachmentID] = @AttachmentID);";
        deleteCommand.Parameters.Add("AttachmentID", SqlDbType.Int);
        deleteCommand.Parameters["AttachmentID"].Value = attachmentID;

        BeforeDBDelete(attachmentID);
        BeforeRowDelete(attachmentID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(attachmentID);
        AfterDBDelete(attachmentID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("AttachmentsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Attachments] SET     [OrganizationID] = @OrganizationID,    [FileName] = @FileName,    [FileType] = @FileType,    [FileSize] = @FileSize,    [Path] = @Path,    [Description] = @Description,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [RefType] = @RefType,    [RefID] = @RefID,    [SentToJira] = @SentToJira,    [AttachmentGUID] = @AttachmentGUID,    [ProductFamilyID] = @ProductFamilyID,    [SentToTFS] = @SentToTFS,    [SentToSnow] = @SentToSnow,    [FilePathID] = @FilePathID  WHERE ([AttachmentID] = @AttachmentID);";

		
		tempParameter = updateCommand.Parameters.Add("AttachmentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("FileName", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("FileType", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("FileSize", SqlDbType.BigInt, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 19;
		  tempParameter.Scale = 19;
		}
		
		tempParameter = updateCommand.Parameters.Add("Path", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 2000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SentToJira", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("AttachmentGUID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ProductFamilyID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SentToTFS", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SentToSnow", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("FilePathID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Attachments] (    [OrganizationID],    [FileName],    [FileType],    [FileSize],    [Path],    [Description],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [RefType],    [RefID],    [SentToJira],    [AttachmentGUID],    [ProductFamilyID],    [SentToTFS],    [SentToSnow],    [FilePathID]) VALUES ( @OrganizationID, @FileName, @FileType, @FileSize, @Path, @Description, @DateCreated, @DateModified, @CreatorID, @ModifierID, @RefType, @RefID, @SentToJira, @AttachmentGUID, @ProductFamilyID, @SentToTFS, @SentToSnow, @FilePathID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("FilePathID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("SentToSnow", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SentToTFS", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ProductFamilyID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("AttachmentGUID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SentToJira", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 2000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Path", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("FileSize", SqlDbType.BigInt, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 19;
		  tempParameter.Scale = 19;
		}
		
		tempParameter = insertCommand.Parameters.Add("FileType", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("FileName", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Attachments] WHERE ([AttachmentID] = @AttachmentID);";
		deleteCommand.Parameters.Add("AttachmentID", SqlDbType.Int);

		try
		{
		  foreach (Attachment attachment in this)
		  {
			if (attachment.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(attachment);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = attachment.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["AttachmentID"].AutoIncrement = false;
			  Table.Columns["AttachmentID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				attachment.Row["AttachmentID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(attachment);
			}
			else if (attachment.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(attachment);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = attachment.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(attachment);
			}
			else if (attachment.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)attachment.Row["AttachmentID", DataRowVersion.Original];
			  deleteCommand.Parameters["AttachmentID"].Value = id;
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

      foreach (Attachment attachment in this)
      {
        if (attachment.Row.Table.Columns.Contains("CreatorID") && (int)attachment.Row["CreatorID"] == 0) attachment.Row["CreatorID"] = LoginUser.UserID;
        if (attachment.Row.Table.Columns.Contains("ModifierID")) attachment.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Attachment FindByAttachmentID(int attachmentID)
    {
      foreach (Attachment attachment in this)
      {
        if (attachment.AttachmentID == attachmentID)
        {
          return attachment;
        }
      }
      return null;
    }

    internal virtual Attachment AddNewAttachment()
    {
      if (Table.Columns.Count < 1) LoadColumns("Attachments");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Attachment(row, this);
    }
    
    public virtual void LoadByAttachmentID(int attachmentID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [AttachmentID], [OrganizationID], [FileName], [FileType], [FileSize], [Path], [Description], [DateCreated], [DateModified], [CreatorID], [ModifierID], [RefType], [RefID], [SentToJira], [AttachmentGUID], [ProductFamilyID], [SentToTFS], [SentToSnow], [FilePathID] FROM [dbo].[Attachments] WHERE ([AttachmentID] = @AttachmentID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("AttachmentID", attachmentID);
        Fill(command);
      }
    }
    
    public static Attachment GetAttachment(LoginUser loginUser, int attachmentID)
    {
      Attachments attachments = new Attachments(loginUser);
      attachments.LoadByAttachmentID(attachmentID);
      if (attachments.IsEmpty)
        return null;
      else
        return attachments[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Attachment> Members

    public IEnumerator<Attachment> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Attachment(row, this);
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
