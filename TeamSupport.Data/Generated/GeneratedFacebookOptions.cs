using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class FacebookOption : BaseItem
  {
    private FacebookOptions _facebookOptions;
    
    public FacebookOption(DataRow row, FacebookOptions facebookOptions): base(row, facebookOptions)
    {
      _facebookOptions = facebookOptions;
    }
	
    #region Properties
    
    public FacebookOptions Collection
    {
      get { return _facebookOptions; }
    }
        
    
    
    

    

    
    public bool DisplayKB
    {
      get { return (bool)Row["DisplayKB"]; }
      set { Row["DisplayKB"] = CheckValue("DisplayKB", value); }
    }
    
    public bool DisplayArticles
    {
      get { return (bool)Row["DisplayArticles"]; }
      set { Row["DisplayArticles"] = CheckValue("DisplayArticles", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class FacebookOptions : BaseCollection, IEnumerable<FacebookOption>
  {
    public FacebookOptions(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "FacebookOptions"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "OrganizationID"; }
    }



    public FacebookOption this[int index]
    {
      get { return new FacebookOption(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(FacebookOption facebookOption);
    partial void AfterRowInsert(FacebookOption facebookOption);
    partial void BeforeRowEdit(FacebookOption facebookOption);
    partial void AfterRowEdit(FacebookOption facebookOption);
    partial void BeforeRowDelete(int organizationID);
    partial void AfterRowDelete(int organizationID);    

    partial void BeforeDBDelete(int organizationID);
    partial void AfterDBDelete(int organizationID);    

    #endregion

    #region Public Methods

    public FacebookOptionProxy[] GetFacebookOptionProxies()
    {
      List<FacebookOptionProxy> list = new List<FacebookOptionProxy>();

      foreach (FacebookOption item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int organizationID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[FacebookOptions] WHERE ([OrganizationID] = @OrganizationID);";
        deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);
        deleteCommand.Parameters["OrganizationID"].Value = organizationID;

        BeforeDBDelete(organizationID);
        BeforeRowDelete(organizationID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(organizationID);
        AfterDBDelete(organizationID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("FacebookOptionsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[FacebookOptions] SET     [DisplayArticles] = @DisplayArticles,    [DisplayKB] = @DisplayKB  WHERE ([OrganizationID] = @OrganizationID);";

		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayArticles", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayKB", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[FacebookOptions] (    [OrganizationID],    [DisplayArticles],    [DisplayKB]) VALUES ( @OrganizationID, @DisplayArticles, @DisplayKB); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DisplayKB", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayArticles", SqlDbType.Bit, 1);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[FacebookOptions] WHERE ([OrganizationID] = @OrganizationID);";
		deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);

		try
		{
		  foreach (FacebookOption facebookOption in this)
		  {
			if (facebookOption.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(facebookOption);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = facebookOption.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["OrganizationID"].AutoIncrement = false;
			  Table.Columns["OrganizationID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				facebookOption.Row["OrganizationID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(facebookOption);
			}
			else if (facebookOption.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(facebookOption);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = facebookOption.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(facebookOption);
			}
			else if (facebookOption.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)facebookOption.Row["OrganizationID", DataRowVersion.Original];
			  deleteCommand.Parameters["OrganizationID"].Value = id;
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

      foreach (FacebookOption facebookOption in this)
      {
        if (facebookOption.Row.Table.Columns.Contains("CreatorID") && (int)facebookOption.Row["CreatorID"] == 0) facebookOption.Row["CreatorID"] = LoginUser.UserID;
        if (facebookOption.Row.Table.Columns.Contains("ModifierID")) facebookOption.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public FacebookOption FindByOrganizationID(int organizationID)
    {
      foreach (FacebookOption facebookOption in this)
      {
        if (facebookOption.OrganizationID == organizationID)
        {
          return facebookOption;
        }
      }
      return null;
    }

    public virtual FacebookOption AddNewFacebookOption()
    {
      if (Table.Columns.Count < 1) LoadColumns("FacebookOptions");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new FacebookOption(row, this);
    }
    
    public virtual void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [OrganizationID], [DisplayArticles], [DisplayKB] FROM [dbo].[FacebookOptions] WHERE ([OrganizationID] = @OrganizationID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }
    
    public static FacebookOption GetFacebookOption(LoginUser loginUser, int organizationID)
    {
      FacebookOptions facebookOptions = new FacebookOptions(loginUser);
      facebookOptions.LoadByOrganizationID(organizationID);
      if (facebookOptions.IsEmpty)
        return null;
      else
        return facebookOptions[0];
    }
    
    
    

    #endregion

    #region IEnumerable<FacebookOption> Members

    public IEnumerator<FacebookOption> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new FacebookOption(row, this);
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
