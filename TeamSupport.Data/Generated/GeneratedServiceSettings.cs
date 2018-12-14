using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ServiceSetting : BaseItem
  {
    private ServiceSettings _serviceSettings;
    
    public ServiceSetting(DataRow row, ServiceSettings serviceSettings): base(row, serviceSettings)
    {
      _serviceSettings = serviceSettings;
    }
	
    #region Properties
    
    public ServiceSettings Collection
    {
      get { return _serviceSettings; }
    }
        
    
    
    
    public int ServiceSettingID
    {
      get { return (int)Row["ServiceSettingID"]; }
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
    
    public int ServiceID
    {
      get { return (int)Row["ServiceID"]; }
      set { Row["ServiceID"] = CheckValue("ServiceID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class ServiceSettings : BaseCollection, IEnumerable<ServiceSetting>
  {
    public ServiceSettings(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ServiceSettings"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ServiceSettingID"; }
    }



    public ServiceSetting this[int index]
    {
      get { return new ServiceSetting(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ServiceSetting serviceSetting);
    partial void AfterRowInsert(ServiceSetting serviceSetting);
    partial void BeforeRowEdit(ServiceSetting serviceSetting);
    partial void AfterRowEdit(ServiceSetting serviceSetting);
    partial void BeforeRowDelete(int serviceSettingID);
    partial void AfterRowDelete(int serviceSettingID);    

    partial void BeforeDBDelete(int serviceSettingID);
    partial void AfterDBDelete(int serviceSettingID);    

    #endregion

    #region Public Methods

    public ServiceSettingProxy[] GetServiceSettingProxies()
    {
      List<ServiceSettingProxy> list = new List<ServiceSettingProxy>();

      foreach (ServiceSetting item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int serviceSettingID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ServiceSettings] WHERE ([ServiceSettingID] = @ServiceSettingID);";
        deleteCommand.Parameters.Add("ServiceSettingID", SqlDbType.Int);
        deleteCommand.Parameters["ServiceSettingID"].Value = serviceSettingID;

        BeforeDBDelete(serviceSettingID);
        BeforeRowDelete(serviceSettingID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(serviceSettingID);
        AfterDBDelete(serviceSettingID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ServiceSettingsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ServiceSettings] SET     [ServiceID] = @ServiceID,    [SettingKey] = @SettingKey,    [SettingValue] = @SettingValue  WHERE ([ServiceSettingID] = @ServiceSettingID);";

		
		tempParameter = updateCommand.Parameters.Add("ServiceSettingID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ServiceID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("SettingValue", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ServiceSettings] (    [ServiceID],    [SettingKey],    [SettingValue]) VALUES ( @ServiceID, @SettingKey, @SettingValue); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("SettingValue", SqlDbType.VarChar, -1);
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
		
		tempParameter = insertCommand.Parameters.Add("ServiceID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ServiceSettings] WHERE ([ServiceSettingID] = @ServiceSettingID);";
		deleteCommand.Parameters.Add("ServiceSettingID", SqlDbType.Int);

		try
		{
		  foreach (ServiceSetting serviceSetting in this)
		  {
			if (serviceSetting.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(serviceSetting);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = serviceSetting.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ServiceSettingID"].AutoIncrement = false;
			  Table.Columns["ServiceSettingID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				serviceSetting.Row["ServiceSettingID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(serviceSetting);
			}
			else if (serviceSetting.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(serviceSetting);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = serviceSetting.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(serviceSetting);
			}
			else if (serviceSetting.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)serviceSetting.Row["ServiceSettingID", DataRowVersion.Original];
			  deleteCommand.Parameters["ServiceSettingID"].Value = id;
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

      foreach (ServiceSetting serviceSetting in this)
      {
        if (serviceSetting.Row.Table.Columns.Contains("CreatorID") && (int)serviceSetting.Row["CreatorID"] == 0) serviceSetting.Row["CreatorID"] = LoginUser.UserID;
        if (serviceSetting.Row.Table.Columns.Contains("ModifierID")) serviceSetting.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ServiceSetting FindByServiceSettingID(int serviceSettingID)
    {
      foreach (ServiceSetting serviceSetting in this)
      {
        if (serviceSetting.ServiceSettingID == serviceSettingID)
        {
          return serviceSetting;
        }
      }
      return null;
    }

    public virtual ServiceSetting AddNewServiceSetting()
    {
      if (Table.Columns.Count < 1) LoadColumns("ServiceSettings");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ServiceSetting(row, this);
    }
    
    public virtual void LoadByServiceSettingID(int serviceSettingID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ServiceSettingID], [ServiceID], [SettingKey], [SettingValue] FROM [dbo].[ServiceSettings] WHERE ([ServiceSettingID] = @ServiceSettingID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ServiceSettingID", serviceSettingID);
        Fill(command);
      }
    }
    
    public static ServiceSetting GetServiceSetting(LoginUser loginUser, int serviceSettingID)
    {
      ServiceSettings serviceSettings = new ServiceSettings(loginUser);
      serviceSettings.LoadByServiceSettingID(serviceSettingID);
      if (serviceSettings.IsEmpty)
        return null;
      else
        return serviceSettings[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ServiceSetting> Members

    public IEnumerator<ServiceSetting> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ServiceSetting(row, this);
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
