using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class SAMLSetting : BaseItem
  {
    private SAMLSettings _sAMLSettings;
    
    public SAMLSetting(DataRow row, SAMLSettings sAMLSettings): base(row, sAMLSettings)
    {
      _sAMLSettings = sAMLSettings;
    }
	
    #region Properties
    
    public SAMLSettings Collection
    {
      get { return _sAMLSettings; }
    }
        
    
    
    
    public int Id
    {
      get { return (int)Row["Id"]; }
    }
    

    
    public string Certificate
    {
      get { return Row["Certificate"] != DBNull.Value ? (string)Row["Certificate"] : null; }
      set { Row["Certificate"] = CheckValue("Certificate", value); }
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
    
    public string IdPUrl
    {
      get { return (string)Row["IdPUrl"]; }
      set { Row["IdPUrl"] = CheckValue("IdPUrl", value); }
    }
    
    public bool IsEnabled
    {
      get { return (bool)Row["IsEnabled"]; }
      set { Row["IsEnabled"] = CheckValue("IsEnabled", value); }
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

  public partial class SAMLSettings : BaseCollection, IEnumerable<SAMLSetting>
  {
    public SAMLSettings(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "SAMLSettings"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "Id"; }
    }



    public SAMLSetting this[int index]
    {
      get { return new SAMLSetting(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(SAMLSetting sAMLSetting);
    partial void AfterRowInsert(SAMLSetting sAMLSetting);
    partial void BeforeRowEdit(SAMLSetting sAMLSetting);
    partial void AfterRowEdit(SAMLSetting sAMLSetting);
    partial void BeforeRowDelete(int id);
    partial void AfterRowDelete(int id);    

    partial void BeforeDBDelete(int id);
    partial void AfterDBDelete(int id);    

    #endregion

    #region Public Methods

    public SAMLSettingProxy[] GetSAMLSettingProxies()
    {
      List<SAMLSettingProxy> list = new List<SAMLSettingProxy>();

      foreach (SAMLSetting item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int id)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SAMLSettings] WHERE ([Id] = @Id);";
        deleteCommand.Parameters.Add("Id", SqlDbType.Int);
        deleteCommand.Parameters["Id"].Value = id;

        BeforeDBDelete(id);
        BeforeRowDelete(id);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(id);
        AfterDBDelete(id);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("SAMLSettingsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[SAMLSettings] SET     [OrganizationID] = @OrganizationID,    [IsEnabled] = @IsEnabled,    [IdPUrl] = @IdPUrl,    [Certificate] = @Certificate,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID  WHERE ([Id] = @Id);";

		
		tempParameter = updateCommand.Parameters.Add("Id", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("IsEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IdPUrl", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Certificate", SqlDbType.NVarChar, -1);
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
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[SAMLSettings] (    [OrganizationID],    [IsEnabled],    [IdPUrl],    [Certificate],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID]) VALUES ( @OrganizationID, @IsEnabled, @IdPUrl, @Certificate, @DateCreated, @DateModified, @CreatorID, @ModifierID); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("Certificate", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IdPUrl", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsEnabled", SqlDbType.Bit, 1);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SAMLSettings] WHERE ([Id] = @Id);";
		deleteCommand.Parameters.Add("Id", SqlDbType.Int);

		try
		{
		  foreach (SAMLSetting sAMLSetting in this)
		  {
			if (sAMLSetting.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(sAMLSetting);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = sAMLSetting.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["Id"].AutoIncrement = false;
			  Table.Columns["Id"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				sAMLSetting.Row["Id"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(sAMLSetting);
			}
			else if (sAMLSetting.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(sAMLSetting);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = sAMLSetting.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(sAMLSetting);
			}
			else if (sAMLSetting.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)sAMLSetting.Row["Id", DataRowVersion.Original];
			  deleteCommand.Parameters["Id"].Value = id;
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

      foreach (SAMLSetting sAMLSetting in this)
      {
        if (sAMLSetting.Row.Table.Columns.Contains("CreatorID") && (int)sAMLSetting.Row["CreatorID"] == 0) sAMLSetting.Row["CreatorID"] = LoginUser.UserID;
        if (sAMLSetting.Row.Table.Columns.Contains("ModifierID")) sAMLSetting.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public SAMLSetting FindById(int id)
    {
      foreach (SAMLSetting sAMLSetting in this)
      {
        if (sAMLSetting.Id == id)
        {
          return sAMLSetting;
        }
      }
      return null;
    }

    public virtual SAMLSetting AddNewSAMLSetting()
    {
      if (Table.Columns.Count < 1) LoadColumns("SAMLSettings");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new SAMLSetting(row, this);
    }
    
    public virtual void LoadById(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [Id], [OrganizationID], [IsEnabled], [IdPUrl], [Certificate], [DateCreated], [DateModified], [CreatorID], [ModifierID] FROM [dbo].[SAMLSettings] WHERE ([Id] = @Id);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("Id", id);
        Fill(command);
      }
    }
    
    public static SAMLSetting GetSAMLSetting(LoginUser loginUser, int id)
    {
      SAMLSettings sAMLSettings = new SAMLSettings(loginUser);
      sAMLSettings.LoadById(id);
      if (sAMLSettings.IsEmpty)
        return null;
      else
        return sAMLSettings[0];
    }
    
    
    

    #endregion

    #region IEnumerable<SAMLSetting> Members

    public IEnumerator<SAMLSetting> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new SAMLSetting(row, this);
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
