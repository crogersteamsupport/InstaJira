using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ReportFolder : BaseItem
  {
    private ReportFolders _reportFolders;
    
    public ReportFolder(DataRow row, ReportFolders reportFolders): base(row, reportFolders)
    {
      _reportFolders = reportFolders;
    }
	
    #region Properties
    
    public ReportFolders Collection
    {
      get { return _reportFolders; }
    }
        
    
    
    
    public int FolderID
    {
      get { return (int)Row["FolderID"]; }
    }
    

    
    public int? ParentID
    {
      get { return Row["ParentID"] != DBNull.Value ? (int?)Row["ParentID"] : null; }
      set { Row["ParentID"] = CheckValue("ParentID", value); }
    }
    
    public int? CreatorID
    {
      get { return Row["CreatorID"] != DBNull.Value ? (int?)Row["CreatorID"] : null; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    

    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckValue("Name", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? DateCreated
    {
      get { return Row["DateCreated"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateCreated"]) : null; }
      set { Row["DateCreated"] = CheckValue("DateCreated", value); }
    }

    public DateTime? DateCreatedUtc
    {
      get { return Row["DateCreated"] != DBNull.Value ? (DateTime?)Row["DateCreated"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class ReportFolders : BaseCollection, IEnumerable<ReportFolder>
  {
    public ReportFolders(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ReportFolders"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "FolderID"; }
    }



    public ReportFolder this[int index]
    {
      get { return new ReportFolder(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ReportFolder reportFolder);
    partial void AfterRowInsert(ReportFolder reportFolder);
    partial void BeforeRowEdit(ReportFolder reportFolder);
    partial void AfterRowEdit(ReportFolder reportFolder);
    partial void BeforeRowDelete(int folderID);
    partial void AfterRowDelete(int folderID);    

    partial void BeforeDBDelete(int folderID);
    partial void AfterDBDelete(int folderID);    

    #endregion

    #region Public Methods

    public ReportFolderProxy[] GetReportFolderProxies()
    {
      List<ReportFolderProxy> list = new List<ReportFolderProxy>();

      foreach (ReportFolder item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int folderID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ReportFolders] WHERE ([FolderID] = @FolderID);";
        deleteCommand.Parameters.Add("FolderID", SqlDbType.Int);
        deleteCommand.Parameters["FolderID"].Value = folderID;

        BeforeDBDelete(folderID);
        BeforeRowDelete(folderID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(folderID);
        AfterDBDelete(folderID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ReportFoldersSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ReportFolders] SET     [OrganizationID] = @OrganizationID,    [Name] = @Name,    [ParentID] = @ParentID  WHERE ([FolderID] = @FolderID);";

		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("FolderID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ReportFolders] (    [OrganizationID],    [Name],    [ParentID],    [CreatorID],    [DateCreated]) VALUES ( @OrganizationID, @Name, @ParentID, @CreatorID, @DateCreated); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 250);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ReportFolders] WHERE ([FolderID] = @FolderID);";
		deleteCommand.Parameters.Add("FolderID", SqlDbType.Int);

		try
		{
		  foreach (ReportFolder reportFolder in this)
		  {
			if (reportFolder.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(reportFolder);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = reportFolder.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["FolderID"].AutoIncrement = false;
			  Table.Columns["FolderID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				reportFolder.Row["FolderID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(reportFolder);
			}
			else if (reportFolder.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(reportFolder);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = reportFolder.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(reportFolder);
			}
			else if (reportFolder.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)reportFolder.Row["FolderID", DataRowVersion.Original];
			  deleteCommand.Parameters["FolderID"].Value = id;
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

      foreach (ReportFolder reportFolder in this)
      {
        if (reportFolder.Row.Table.Columns.Contains("CreatorID") && (int)reportFolder.Row["CreatorID"] == 0) reportFolder.Row["CreatorID"] = LoginUser.UserID;
        if (reportFolder.Row.Table.Columns.Contains("ModifierID")) reportFolder.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ReportFolder FindByFolderID(int folderID)
    {
      foreach (ReportFolder reportFolder in this)
      {
        if (reportFolder.FolderID == folderID)
        {
          return reportFolder;
        }
      }
      return null;
    }

    public virtual ReportFolder AddNewReportFolder()
    {
      if (Table.Columns.Count < 1) LoadColumns("ReportFolders");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ReportFolder(row, this);
    }
    
    public virtual void LoadByFolderID(int folderID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [OrganizationID], [FolderID], [Name], [ParentID], [CreatorID], [DateCreated] FROM [dbo].[ReportFolders] WHERE ([FolderID] = @FolderID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("FolderID", folderID);
        Fill(command);
      }
    }
    
    public static ReportFolder GetReportFolder(LoginUser loginUser, int folderID)
    {
      ReportFolders reportFolders = new ReportFolders(loginUser);
      reportFolders.LoadByFolderID(folderID);
      if (reportFolders.IsEmpty)
        return null;
      else
        return reportFolders[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ReportFolder> Members

    public IEnumerator<ReportFolder> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ReportFolder(row, this);
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
