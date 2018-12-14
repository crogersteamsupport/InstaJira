using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class FilePath : BaseItem
  {
    private FilePaths _filePaths;
    
    public FilePath(DataRow row, FilePaths filePaths): base(row, filePaths)
    {
      _filePaths = filePaths;
    }
	
    #region Properties
    
    public FilePaths Collection
    {
      get { return _filePaths; }
    }
        
    
    
    
    public int ID
    {
      get { return (int)Row["ID"]; }
    }
    

    

    
    public string Value
    {
      get { return (string)Row["Value"]; }
      set { Row["Value"] = CheckValue("Value", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class FilePaths : BaseCollection, IEnumerable<FilePath>
  {
    public FilePaths(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "FilePaths"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ID"; }
    }



    public FilePath this[int index]
    {
      get { return new FilePath(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(FilePath filePath);
    partial void AfterRowInsert(FilePath filePath);
    partial void BeforeRowEdit(FilePath filePath);
    partial void AfterRowEdit(FilePath filePath);
    partial void BeforeRowDelete(int iD);
    partial void AfterRowDelete(int iD);    

    partial void BeforeDBDelete(int iD);
    partial void AfterDBDelete(int iD);    

    #endregion

    #region Public Methods

    public FilePathProxy[] GetFilePathProxies()
    {
      List<FilePathProxy> list = new List<FilePathProxy>();

      foreach (FilePath item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int iD)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[FilePaths] WHERE ([ID] = @ID);";
        deleteCommand.Parameters.Add("ID", SqlDbType.Int);
        deleteCommand.Parameters["ID"].Value = iD;

        BeforeDBDelete(iD);
        BeforeRowDelete(iD);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(iD);
        AfterDBDelete(iD);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("FilePathsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[FilePaths] SET     [Value] = @Value  WHERE ([ID] = @ID);";

		
		tempParameter = updateCommand.Parameters.Add("ID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Value", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[FilePaths] (    [Value]) VALUES ( @Value); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("Value", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[FilePaths] WHERE ([ID] = @ID);";
		deleteCommand.Parameters.Add("ID", SqlDbType.Int);

		try
		{
		  foreach (FilePath filePath in this)
		  {
			if (filePath.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(filePath);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = filePath.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ID"].AutoIncrement = false;
			  Table.Columns["ID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				filePath.Row["ID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(filePath);
			}
			else if (filePath.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(filePath);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = filePath.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(filePath);
			}
			else if (filePath.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)filePath.Row["ID", DataRowVersion.Original];
			  deleteCommand.Parameters["ID"].Value = id;
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

      foreach (FilePath filePath in this)
      {
        if (filePath.Row.Table.Columns.Contains("CreatorID") && (int)filePath.Row["CreatorID"] == 0) filePath.Row["CreatorID"] = LoginUser.UserID;
        if (filePath.Row.Table.Columns.Contains("ModifierID")) filePath.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public FilePath FindByID(int iD)
    {
      foreach (FilePath filePath in this)
      {
        if (filePath.ID == iD)
        {
          return filePath;
        }
      }
      return null;
    }

    public virtual FilePath AddNewFilePath()
    {
      if (Table.Columns.Count < 1) LoadColumns("FilePaths");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new FilePath(row, this);
    }
    
    public virtual void LoadByID(int iD)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ID], [Value] FROM [dbo].[FilePaths] WHERE ([ID] = @ID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ID", iD);
        Fill(command);
      }
    }
    
    public static FilePath GetFilePath(LoginUser loginUser, int iD)
    {
      FilePaths filePaths = new FilePaths(loginUser);
      filePaths.LoadByID(iD);
      if (filePaths.IsEmpty)
        return null;
      else
        return filePaths[0];
    }
    
    
    

    #endregion

    #region IEnumerable<FilePath> Members

    public IEnumerator<FilePath> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new FilePath(row, this);
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
