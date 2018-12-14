using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class OrganizationSetting : BaseItem
  {
    private OrganizationSettings _organizationSettings;
    
    public OrganizationSetting(DataRow row, OrganizationSettings organizationSettings): base(row, organizationSettings)
    {
      _organizationSettings = organizationSettings;
    }
	
    #region Properties
    
    public OrganizationSettings Collection
    {
      get { return _organizationSettings; }
    }
        
    
    
    
    public int OrganizationSettingID
    {
      get { return (int)Row["OrganizationSettingID"]; }
    }
    

    

    
    public string SettingValue
    {
      get { return (string)Row["SettingValue"]; }
      set { Row["SettingValue"] = CheckValue("SettingValue", value); }
    }
    
    public string SettingKey
    {
      get { return (string)Row["SettingKey"]; }
      set { Row["SettingKey"] = CheckValue("SettingKey", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class OrganizationSettings : BaseCollection, IEnumerable<OrganizationSetting>
  {
    public OrganizationSettings(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "OrganizationSettings"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "OrganizationSettingID"; }
    }



    public OrganizationSetting this[int index]
    {
      get { return new OrganizationSetting(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(OrganizationSetting organizationSetting);
    partial void AfterRowInsert(OrganizationSetting organizationSetting);
    partial void BeforeRowEdit(OrganizationSetting organizationSetting);
    partial void AfterRowEdit(OrganizationSetting organizationSetting);
    partial void BeforeRowDelete(int organizationSettingID);
    partial void AfterRowDelete(int organizationSettingID);    

    partial void BeforeDBDelete(int organizationSettingID);
    partial void AfterDBDelete(int organizationSettingID);    

    #endregion

    #region Public Methods

    public OrganizationSettingProxy[] GetOrganizationSettingProxies()
    {
      List<OrganizationSettingProxy> list = new List<OrganizationSettingProxy>();

      foreach (OrganizationSetting item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int organizationSettingID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[OrganizationSettings] WHERE ([OrganizationSettingID] = @OrganizationSettingID);";
        deleteCommand.Parameters.Add("OrganizationSettingID", SqlDbType.Int);
        deleteCommand.Parameters["OrganizationSettingID"].Value = organizationSettingID;

        BeforeDBDelete(organizationSettingID);
        BeforeRowDelete(organizationSettingID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(organizationSettingID);
        AfterDBDelete(organizationSettingID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("OrganizationSettingsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[OrganizationSettings] SET     [OrganizationID] = @OrganizationID,    [SettingKey] = @SettingKey,    [SettingValue] = @SettingValue  WHERE ([OrganizationSettingID] = @OrganizationSettingID);";

		
		tempParameter = updateCommand.Parameters.Add("OrganizationSettingID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("SettingKey", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SettingValue", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[OrganizationSettings] (    [OrganizationID],    [SettingKey],    [SettingValue]) VALUES ( @OrganizationID, @SettingKey, @SettingValue); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("SettingValue", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SettingKey", SqlDbType.VarChar, 255);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[OrganizationSettings] WHERE ([OrganizationSettingID] = @OrganizationSettingID);";
		deleteCommand.Parameters.Add("OrganizationSettingID", SqlDbType.Int);

		try
		{
		  foreach (OrganizationSetting organizationSetting in this)
		  {
			if (organizationSetting.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(organizationSetting);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = organizationSetting.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["OrganizationSettingID"].AutoIncrement = false;
			  Table.Columns["OrganizationSettingID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				organizationSetting.Row["OrganizationSettingID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(organizationSetting);
			}
			else if (organizationSetting.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(organizationSetting);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = organizationSetting.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(organizationSetting);
			}
			else if (organizationSetting.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)organizationSetting.Row["OrganizationSettingID", DataRowVersion.Original];
			  deleteCommand.Parameters["OrganizationSettingID"].Value = id;
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

      foreach (OrganizationSetting organizationSetting in this)
      {
        if (organizationSetting.Row.Table.Columns.Contains("CreatorID") && (int)organizationSetting.Row["CreatorID"] == 0) organizationSetting.Row["CreatorID"] = LoginUser.UserID;
        if (organizationSetting.Row.Table.Columns.Contains("ModifierID")) organizationSetting.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public OrganizationSetting FindByOrganizationSettingID(int organizationSettingID)
    {
      foreach (OrganizationSetting organizationSetting in this)
      {
        if (organizationSetting.OrganizationSettingID == organizationSettingID)
        {
          return organizationSetting;
        }
      }
      return null;
    }

    public virtual OrganizationSetting AddNewOrganizationSetting()
    {
      if (Table.Columns.Count < 1) LoadColumns("OrganizationSettings");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new OrganizationSetting(row, this);
    }
    
    public virtual void LoadByOrganizationSettingID(int organizationSettingID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [OrganizationSettingID], [OrganizationID], [SettingKey], [SettingValue] FROM [dbo].[OrganizationSettings] WHERE ([OrganizationSettingID] = @OrganizationSettingID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationSettingID", organizationSettingID);
        Fill(command);
      }
    }
    
    public static OrganizationSetting GetOrganizationSetting(LoginUser loginUser, int organizationSettingID)
    {
      OrganizationSettings organizationSettings = new OrganizationSettings(loginUser);
      organizationSettings.LoadByOrganizationSettingID(organizationSettingID);
      if (organizationSettings.IsEmpty)
        return null;
      else
        return organizationSettings[0];
    }
    
    
    

    #endregion

    #region IEnumerable<OrganizationSetting> Members

    public IEnumerator<OrganizationSetting> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new OrganizationSetting(row, this);
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
