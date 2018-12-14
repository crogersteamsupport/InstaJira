using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ExceptionLogViewItem : BaseItem
  {
    private ExceptionLogView _exceptionLogView;
    
    public ExceptionLogViewItem(DataRow row, ExceptionLogView exceptionLogView): base(row, exceptionLogView)
    {
      _exceptionLogView = exceptionLogView;
    }
	
    #region Properties
    
    public ExceptionLogView Collection
    {
      get { return _exceptionLogView; }
    }
        
    
    
    

    
    public string URL
    {
      get { return Row["URL"] != DBNull.Value ? (string)Row["URL"] : null; }
      set { Row["URL"] = CheckValue("URL", value); }
    }
    
    public string PageInfo
    {
      get { return Row["PageInfo"] != DBNull.Value ? (string)Row["PageInfo"] : null; }
      set { Row["PageInfo"] = CheckValue("PageInfo", value); }
    }
    
    public string ExceptionName
    {
      get { return Row["ExceptionName"] != DBNull.Value ? (string)Row["ExceptionName"] : null; }
      set { Row["ExceptionName"] = CheckValue("ExceptionName", value); }
    }
    
    public string Message
    {
      get { return Row["Message"] != DBNull.Value ? (string)Row["Message"] : null; }
      set { Row["Message"] = CheckValue("Message", value); }
    }
    
    public string StackTrace
    {
      get { return Row["StackTrace"] != DBNull.Value ? (string)Row["StackTrace"] : null; }
      set { Row["StackTrace"] = CheckValue("StackTrace", value); }
    }
    
    public string FirstName
    {
      get { return Row["FirstName"] != DBNull.Value ? (string)Row["FirstName"] : null; }
      set { Row["FirstName"] = CheckValue("FirstName", value); }
    }
    
    public string LastName
    {
      get { return Row["LastName"] != DBNull.Value ? (string)Row["LastName"] : null; }
      set { Row["LastName"] = CheckValue("LastName", value); }
    }
    
    public string Name
    {
      get { return Row["Name"] != DBNull.Value ? (string)Row["Name"] : null; }
      set { Row["Name"] = CheckValue("Name", value); }
    }
    

    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public int ExceptionLogID
    {
      get { return (int)Row["ExceptionLogID"]; }
      set { Row["ExceptionLogID"] = CheckValue("ExceptionLogID", value); }
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

  public partial class ExceptionLogView : BaseCollection, IEnumerable<ExceptionLogViewItem>
  {
    public ExceptionLogView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ExceptionLogView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ExceptionLogID"; }
    }



    public ExceptionLogViewItem this[int index]
    {
      get { return new ExceptionLogViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ExceptionLogViewItem exceptionLogViewItem);
    partial void AfterRowInsert(ExceptionLogViewItem exceptionLogViewItem);
    partial void BeforeRowEdit(ExceptionLogViewItem exceptionLogViewItem);
    partial void AfterRowEdit(ExceptionLogViewItem exceptionLogViewItem);
    partial void BeforeRowDelete(int exceptionLogID);
    partial void AfterRowDelete(int exceptionLogID);    

    partial void BeforeDBDelete(int exceptionLogID);
    partial void AfterDBDelete(int exceptionLogID);    

    #endregion

    #region Public Methods

    public ExceptionLogViewItemProxy[] GetExceptionLogViewItemProxies()
    {
      List<ExceptionLogViewItemProxy> list = new List<ExceptionLogViewItemProxy>();

      foreach (ExceptionLogViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int exceptionLogID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ExceptionLogView] WHERE ([ExceptionLogID] = @ExceptionLogID);";
        deleteCommand.Parameters.Add("ExceptionLogID", SqlDbType.Int);
        deleteCommand.Parameters["ExceptionLogID"].Value = exceptionLogID;

        BeforeDBDelete(exceptionLogID);
        BeforeRowDelete(exceptionLogID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(exceptionLogID);
        AfterDBDelete(exceptionLogID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ExceptionLogViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ExceptionLogView] SET     [URL] = @URL,    [PageInfo] = @PageInfo,    [ExceptionName] = @ExceptionName,    [Message] = @Message,    [StackTrace] = @StackTrace,    [FirstName] = @FirstName,    [LastName] = @LastName,    [Name] = @Name  WHERE ([ExceptionLogID] = @ExceptionLogID);";

		
		tempParameter = updateCommand.Parameters.Add("ExceptionLogID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("URL", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PageInfo", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ExceptionName", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Message", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("StackTrace", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("FirstName", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastName", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.NVarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ExceptionLogView] (    [ExceptionLogID],    [URL],    [PageInfo],    [ExceptionName],    [Message],    [StackTrace],    [CreatorID],    [DateCreated],    [FirstName],    [LastName],    [Name]) VALUES ( @ExceptionLogID, @URL, @PageInfo, @ExceptionName, @Message, @StackTrace, @CreatorID, @DateCreated, @FirstName, @LastName, @Name); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.NVarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastName", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("FirstName", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
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
		
		tempParameter = insertCommand.Parameters.Add("StackTrace", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Message", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ExceptionName", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PageInfo", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("URL", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ExceptionLogID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ExceptionLogView] WHERE ([ExceptionLogID] = @ExceptionLogID);";
		deleteCommand.Parameters.Add("ExceptionLogID", SqlDbType.Int);

		try
		{
		  foreach (ExceptionLogViewItem exceptionLogViewItem in this)
		  {
			if (exceptionLogViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(exceptionLogViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = exceptionLogViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ExceptionLogID"].AutoIncrement = false;
			  Table.Columns["ExceptionLogID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				exceptionLogViewItem.Row["ExceptionLogID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(exceptionLogViewItem);
			}
			else if (exceptionLogViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(exceptionLogViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = exceptionLogViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(exceptionLogViewItem);
			}
			else if (exceptionLogViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)exceptionLogViewItem.Row["ExceptionLogID", DataRowVersion.Original];
			  deleteCommand.Parameters["ExceptionLogID"].Value = id;
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

      foreach (ExceptionLogViewItem exceptionLogViewItem in this)
      {
        if (exceptionLogViewItem.Row.Table.Columns.Contains("CreatorID") && (int)exceptionLogViewItem.Row["CreatorID"] == 0) exceptionLogViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (exceptionLogViewItem.Row.Table.Columns.Contains("ModifierID")) exceptionLogViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ExceptionLogViewItem FindByExceptionLogID(int exceptionLogID)
    {
      foreach (ExceptionLogViewItem exceptionLogViewItem in this)
      {
        if (exceptionLogViewItem.ExceptionLogID == exceptionLogID)
        {
          return exceptionLogViewItem;
        }
      }
      return null;
    }

    public virtual ExceptionLogViewItem AddNewExceptionLogViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("ExceptionLogView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ExceptionLogViewItem(row, this);
    }
    
    public virtual void LoadByExceptionLogID(int exceptionLogID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ExceptionLogID], [URL], [PageInfo], [ExceptionName], [Message], [StackTrace], [CreatorID], [DateCreated], [FirstName], [LastName], [Name] FROM [dbo].[ExceptionLogView] WHERE ([ExceptionLogID] = @ExceptionLogID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ExceptionLogID", exceptionLogID);
        Fill(command);
      }
    }
    
    public static ExceptionLogViewItem GetExceptionLogViewItem(LoginUser loginUser, int exceptionLogID)
    {
      ExceptionLogView exceptionLogView = new ExceptionLogView(loginUser);
      exceptionLogView.LoadByExceptionLogID(exceptionLogID);
      if (exceptionLogView.IsEmpty)
        return null;
      else
        return exceptionLogView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ExceptionLogViewItem> Members

    public IEnumerator<ExceptionLogViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ExceptionLogViewItem(row, this);
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
