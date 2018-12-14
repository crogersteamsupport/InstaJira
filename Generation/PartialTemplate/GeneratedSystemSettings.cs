using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class SystemSetting : BaseItem
  {
    private SystemSettings _systemSettings;
    
    public SystemSetting(DataRow row, SystemSettings systemSettings): base(row, systemSettings)
    {
      _systemSettings = systemSettings;
    }
	
    #region Properties
    
    public SystemSettings Collection
    {
      get { return _systemSettings; }
    }
        
    
    
    
    public int SystemSettingID
    {
      get { return (int)Row["SystemSettingID"]; }
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
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class SystemSettings : BaseCollection, IEnumerable<SystemSetting>
  {
    public SystemSettings(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "SystemSettings"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "SystemSettingID"; }
    }



    public SystemSetting this[int index]
    {
      get { return new SystemSetting(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(SystemSetting systemSetting);
    partial void AfterRowInsert(SystemSetting systemSetting);
    partial void BeforeRowEdit(SystemSetting systemSetting);
    partial void AfterRowEdit(SystemSetting systemSetting);
    partial void BeforeRowDelete(int systemSettingID);
    partial void AfterRowDelete(int systemSettingID);    

    partial void BeforeDBDelete(int systemSettingID);
    partial void AfterDBDelete(int systemSettingID);    

    #endregion

    #region Public Methods

    public SystemSettingProxy[] GetSystemSettingProxies()
    {
      List<SystemSettingProxy> list = new List<SystemSettingProxy>();

      foreach (SystemSetting item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int systemSettingID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SystemSettings] WHERE ([SystemSettingID] = @SystemSettingID);";
        deleteCommand.Parameters.Add("SystemSettingID", SqlDbType.Int);
        deleteCommand.Parameters["SystemSettingID"].Value = systemSettingID;

        BeforeDBDelete(systemSettingID);
        BeforeRowDelete(systemSettingID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(systemSettingID);
        AfterDBDelete(systemSettingID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("SystemSettingsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[SystemSettings] SET     [SettingKey] = @SettingKey,    [SettingValue] = @SettingValue  WHERE ([SystemSettingID] = @SystemSettingID);";

		
		tempParameter = updateCommand.Parameters.Add("SystemSettingID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SettingKey", SqlDbType.VarChar, 250);
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[SystemSettings] (    [SettingKey],    [SettingValue]) VALUES ( @SettingKey, @SettingValue); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("SettingValue", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SettingKey", SqlDbType.VarChar, 250);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SystemSettings] WHERE ([SystemSettingID] = @SystemSettingID);";
		deleteCommand.Parameters.Add("SystemSettingID", SqlDbType.Int);

		try
		{
		  foreach (SystemSetting systemSetting in this)
		  {
			if (systemSetting.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(systemSetting);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = systemSetting.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["SystemSettingID"].AutoIncrement = false;
			  Table.Columns["SystemSettingID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				systemSetting.Row["SystemSettingID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(systemSetting);
			}
			else if (systemSetting.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(systemSetting);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = systemSetting.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(systemSetting);
			}
			else if (systemSetting.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)systemSetting.Row["SystemSettingID", DataRowVersion.Original];
			  deleteCommand.Parameters["SystemSettingID"].Value = id;
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

      foreach (SystemSetting systemSetting in this)
      {
        if (systemSetting.Row.Table.Columns.Contains("CreatorID") && (int)systemSetting.Row["CreatorID"] == 0) systemSetting.Row["CreatorID"] = LoginUser.UserID;
        if (systemSetting.Row.Table.Columns.Contains("ModifierID")) systemSetting.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public SystemSetting FindBySystemSettingID(int systemSettingID)
    {
      foreach (SystemSetting systemSetting in this)
      {
        if (systemSetting.SystemSettingID == systemSettingID)
        {
          return systemSetting;
        }
      }
      return null;
    }

    public virtual SystemSetting AddNewSystemSetting()
    {
      if (Table.Columns.Count < 1) LoadColumns("SystemSettings");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new SystemSetting(row, this);
    }
    
    public virtual void LoadBySystemSettingID(int systemSettingID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [SystemSettingID], [SettingKey], [SettingValue] FROM [dbo].[SystemSettings] WHERE ([SystemSettingID] = @SystemSettingID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("SystemSettingID", systemSettingID);
        Fill(command);
      }
    }
    
    public static SystemSetting GetSystemSetting(LoginUser loginUser, int systemSettingID)
    {
      SystemSettings systemSettings = new SystemSettings(loginUser);
      systemSettings.LoadBySystemSettingID(systemSettingID);
      if (systemSettings.IsEmpty)
        return null;
      else
        return systemSettings[0];
    }
    
    
    

    #endregion

    #region IEnumerable<SystemSetting> Members

    public IEnumerator<SystemSetting> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new SystemSetting(row, this);
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
