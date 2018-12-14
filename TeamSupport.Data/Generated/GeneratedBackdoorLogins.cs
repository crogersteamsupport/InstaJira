using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class BackdoorLogin : BaseItem
  {
    private BackdoorLogins _backdoorLogins;
    
    public BackdoorLogin(DataRow row, BackdoorLogins backdoorLogins): base(row, backdoorLogins)
    {
      _backdoorLogins = backdoorLogins;
    }
	
    #region Properties
    
    public BackdoorLogins Collection
    {
      get { return _backdoorLogins; }
    }
        
    
    
    
    public int BackdoorLoginID
    {
      get { return (int)Row["BackdoorLoginID"]; }
    }
    

    

    
    public int ContactID
    {
      get { return (int)Row["ContactID"]; }
      set { Row["ContactID"] = CheckValue("ContactID", value); }
    }
    
    public Guid Token
    {
      get { return (Guid)Row["Token"]; }
      set { Row["Token"] = CheckValue("Token", value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateIssued
    {
      get { return DateToLocal((DateTime)Row["DateIssued"]); }
      set { Row["DateIssued"] = CheckValue("DateIssued", value); }
    }

    public DateTime DateIssuedUtc
    {
      get { return (DateTime)Row["DateIssued"]; }
    }
    

    #endregion
    
    
  }

  public partial class BackdoorLogins : BaseCollection, IEnumerable<BackdoorLogin>
  {
    public BackdoorLogins(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "BackdoorLogins"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "BackdoorLoginID"; }
    }



    public BackdoorLogin this[int index]
    {
      get { return new BackdoorLogin(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(BackdoorLogin backdoorLogin);
    partial void AfterRowInsert(BackdoorLogin backdoorLogin);
    partial void BeforeRowEdit(BackdoorLogin backdoorLogin);
    partial void AfterRowEdit(BackdoorLogin backdoorLogin);
    partial void BeforeRowDelete(int backdoorLoginID);
    partial void AfterRowDelete(int backdoorLoginID);    

    partial void BeforeDBDelete(int backdoorLoginID);
    partial void AfterDBDelete(int backdoorLoginID);    

    #endregion

    #region Public Methods

    public BackdoorLoginProxy[] GetBackdoorLoginProxies()
    {
      List<BackdoorLoginProxy> list = new List<BackdoorLoginProxy>();

      foreach (BackdoorLogin item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int backdoorLoginID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[BackdoorLogins] WHERE ([BackdoorLoginID] = @BackdoorLoginID);";
        deleteCommand.Parameters.Add("BackdoorLoginID", SqlDbType.Int);
        deleteCommand.Parameters["BackdoorLoginID"].Value = backdoorLoginID;

        BeforeDBDelete(backdoorLoginID);
        BeforeRowDelete(backdoorLoginID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(backdoorLoginID);
        AfterDBDelete(backdoorLoginID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("BackdoorLoginsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[BackdoorLogins] SET     [UserID] = @UserID,    [Token] = @Token,    [DateIssued] = @DateIssued,    [ContactID] = @ContactID  WHERE ([BackdoorLoginID] = @BackdoorLoginID);";

		
		tempParameter = updateCommand.Parameters.Add("BackdoorLoginID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Token", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateIssued", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ContactID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[BackdoorLogins] (    [UserID],    [Token],    [DateIssued],    [ContactID]) VALUES ( @UserID, @Token, @DateIssued, @ContactID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ContactID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateIssued", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Token", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[BackdoorLogins] WHERE ([BackdoorLoginID] = @BackdoorLoginID);";
		deleteCommand.Parameters.Add("BackdoorLoginID", SqlDbType.Int);

		try
		{
		  foreach (BackdoorLogin backdoorLogin in this)
		  {
			if (backdoorLogin.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(backdoorLogin);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = backdoorLogin.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["BackdoorLoginID"].AutoIncrement = false;
			  Table.Columns["BackdoorLoginID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				backdoorLogin.Row["BackdoorLoginID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(backdoorLogin);
			}
			else if (backdoorLogin.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(backdoorLogin);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = backdoorLogin.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(backdoorLogin);
			}
			else if (backdoorLogin.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)backdoorLogin.Row["BackdoorLoginID", DataRowVersion.Original];
			  deleteCommand.Parameters["BackdoorLoginID"].Value = id;
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

      foreach (BackdoorLogin backdoorLogin in this)
      {
        if (backdoorLogin.Row.Table.Columns.Contains("CreatorID") && (int)backdoorLogin.Row["CreatorID"] == 0) backdoorLogin.Row["CreatorID"] = LoginUser.UserID;
        if (backdoorLogin.Row.Table.Columns.Contains("ModifierID")) backdoorLogin.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public BackdoorLogin FindByBackdoorLoginID(int backdoorLoginID)
    {
      foreach (BackdoorLogin backdoorLogin in this)
      {
        if (backdoorLogin.BackdoorLoginID == backdoorLoginID)
        {
          return backdoorLogin;
        }
      }
      return null;
    }

    public virtual BackdoorLogin AddNewBackdoorLogin()
    {
      if (Table.Columns.Count < 1) LoadColumns("BackdoorLogins");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new BackdoorLogin(row, this);
    }
    
    public virtual void LoadByBackdoorLoginID(int backdoorLoginID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [BackdoorLoginID], [UserID], [Token], [DateIssued], [ContactID] FROM [dbo].[BackdoorLogins] WHERE ([BackdoorLoginID] = @BackdoorLoginID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("BackdoorLoginID", backdoorLoginID);
        Fill(command);
      }
    }
    
    public static BackdoorLogin GetBackdoorLogin(LoginUser loginUser, int backdoorLoginID)
    {
      BackdoorLogins backdoorLogins = new BackdoorLogins(loginUser);
      backdoorLogins.LoadByBackdoorLoginID(backdoorLoginID);
      if (backdoorLogins.IsEmpty)
        return null;
      else
        return backdoorLogins[0];
    }
    
    
    

    #endregion

    #region IEnumerable<BackdoorLogin> Members

    public IEnumerator<BackdoorLogin> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new BackdoorLogin(row, this);
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
