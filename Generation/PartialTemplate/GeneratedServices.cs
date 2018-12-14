using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Service : BaseItem
  {
    private Services _services;
    
    public Service(DataRow row, Services services): base(row, services)
    {
      _services = services;
    }
	
    #region Properties
    
    public Services Collection
    {
      get { return _services; }
    }
        
    
    
    
    public int ServiceID
    {
      get { return (int)Row["ServiceID"]; }
    }
    

    

    
    public int HealthMaxMinutes
    {
      get { return (int)Row["HealthMaxMinutes"]; }
      set { Row["HealthMaxMinutes"] = CheckValue("HealthMaxMinutes", value); }
    }
    
    public string NameSpace
    {
      get { return (string)Row["NameSpace"]; }
      set { Row["NameSpace"] = CheckValue("NameSpace", value); }
    }
    
    public bool AutoStart
    {
      get { return (bool)Row["AutoStart"]; }
      set { Row["AutoStart"] = CheckValue("AutoStart", value); }
    }
    
    public string AssemblyName
    {
      get { return (string)Row["AssemblyName"]; }
      set { Row["AssemblyName"] = CheckValue("AssemblyName", value); }
    }
    
    public int RunTimeMax
    {
      get { return (int)Row["RunTimeMax"]; }
      set { Row["RunTimeMax"] = CheckValue("RunTimeMax", value); }
    }
    
    public int RunTimeAvg
    {
      get { return (int)Row["RunTimeAvg"]; }
      set { Row["RunTimeAvg"] = CheckValue("RunTimeAvg", value); }
    }
    
    public int RunCount
    {
      get { return (int)Row["RunCount"]; }
      set { Row["RunCount"] = CheckValue("RunCount", value); }
    }
    
    public int ErrorCount
    {
      get { return (int)Row["ErrorCount"]; }
      set { Row["ErrorCount"] = CheckValue("ErrorCount", value); }
    }
    
    public string LastError
    {
      get { return (string)Row["LastError"]; }
      set { Row["LastError"] = CheckValue("LastError", value); }
    }
    
    public string LastResult
    {
      get { return (string)Row["LastResult"]; }
      set { Row["LastResult"] = CheckValue("LastResult", value); }
    }
    
    public int Interval
    {
      get { return (int)Row["Interval"]; }
      set { Row["Interval"] = CheckValue("Interval", value); }
    }
    
    public bool Enabled
    {
      get { return (bool)Row["Enabled"]; }
      set { Row["Enabled"] = CheckValue("Enabled", value); }
    }
    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckValue("Name", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? LastStartTime
    {
      get { return Row["LastStartTime"] != DBNull.Value ? DateToLocal((DateTime?)Row["LastStartTime"]) : null; }
      set { Row["LastStartTime"] = CheckValue("LastStartTime", value); }
    }

    public DateTime? LastStartTimeUtc
    {
      get { return Row["LastStartTime"] != DBNull.Value ? (DateTime?)Row["LastStartTime"] : null; }
    }
    
    public DateTime? LastEndTime
    {
      get { return Row["LastEndTime"] != DBNull.Value ? DateToLocal((DateTime?)Row["LastEndTime"]) : null; }
      set { Row["LastEndTime"] = CheckValue("LastEndTime", value); }
    }

    public DateTime? LastEndTimeUtc
    {
      get { return Row["LastEndTime"] != DBNull.Value ? (DateTime?)Row["LastEndTime"] : null; }
    }
    
    public DateTime? HealthTime
    {
      get { return Row["HealthTime"] != DBNull.Value ? DateToLocal((DateTime?)Row["HealthTime"]) : null; }
      set { Row["HealthTime"] = CheckValue("HealthTime", value); }
    }

    public DateTime? HealthTimeUtc
    {
      get { return Row["HealthTime"] != DBNull.Value ? (DateTime?)Row["HealthTime"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class Services : BaseCollection, IEnumerable<Service>
  {
    public Services(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Services"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ServiceID"; }
    }



    public Service this[int index]
    {
      get { return new Service(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Service service);
    partial void AfterRowInsert(Service service);
    partial void BeforeRowEdit(Service service);
    partial void AfterRowEdit(Service service);
    partial void BeforeRowDelete(int serviceID);
    partial void AfterRowDelete(int serviceID);    

    partial void BeforeDBDelete(int serviceID);
    partial void AfterDBDelete(int serviceID);    

    #endregion

    #region Public Methods

    public ServiceProxy[] GetServiceProxies()
    {
      List<ServiceProxy> list = new List<ServiceProxy>();

      foreach (Service item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int serviceID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Services] WHERE ([ServiceID] = @ServiceID);";
        deleteCommand.Parameters.Add("ServiceID", SqlDbType.Int);
        deleteCommand.Parameters["ServiceID"].Value = serviceID;

        BeforeDBDelete(serviceID);
        BeforeRowDelete(serviceID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(serviceID);
        AfterDBDelete(serviceID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ServicesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Services] SET     [Name] = @Name,    [Enabled] = @Enabled,    [Interval] = @Interval,    [LastStartTime] = @LastStartTime,    [LastEndTime] = @LastEndTime,    [LastResult] = @LastResult,    [LastError] = @LastError,    [ErrorCount] = @ErrorCount,    [RunCount] = @RunCount,    [RunTimeAvg] = @RunTimeAvg,    [RunTimeMax] = @RunTimeMax,    [AssemblyName] = @AssemblyName,    [AutoStart] = @AutoStart,    [HealthTime] = @HealthTime,    [NameSpace] = @NameSpace,    [HealthMaxMinutes] = @HealthMaxMinutes  WHERE ([ServiceID] = @ServiceID);";

		
		tempParameter = updateCommand.Parameters.Add("ServiceID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Enabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Interval", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastStartTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastEndTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastResult", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastError", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ErrorCount", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RunCount", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RunTimeAvg", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RunTimeMax", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("AssemblyName", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("AutoStart", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("HealthTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("NameSpace", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("HealthMaxMinutes", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Services] (    [Name],    [Enabled],    [Interval],    [LastStartTime],    [LastEndTime],    [LastResult],    [LastError],    [ErrorCount],    [RunCount],    [RunTimeAvg],    [RunTimeMax],    [AssemblyName],    [AutoStart],    [HealthTime],    [NameSpace],    [HealthMaxMinutes]) VALUES ( @Name, @Enabled, @Interval, @LastStartTime, @LastEndTime, @LastResult, @LastError, @ErrorCount, @RunCount, @RunTimeAvg, @RunTimeMax, @AssemblyName, @AutoStart, @HealthTime, @NameSpace, @HealthMaxMinutes); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("HealthMaxMinutes", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("NameSpace", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("HealthTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("AutoStart", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("AssemblyName", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RunTimeMax", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("RunTimeAvg", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("RunCount", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ErrorCount", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastError", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastResult", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastEndTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastStartTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Interval", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Enabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 250);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Services] WHERE ([ServiceID] = @ServiceID);";
		deleteCommand.Parameters.Add("ServiceID", SqlDbType.Int);

		try
		{
		  foreach (Service service in this)
		  {
			if (service.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(service);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = service.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ServiceID"].AutoIncrement = false;
			  Table.Columns["ServiceID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				service.Row["ServiceID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(service);
			}
			else if (service.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(service);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = service.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(service);
			}
			else if (service.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)service.Row["ServiceID", DataRowVersion.Original];
			  deleteCommand.Parameters["ServiceID"].Value = id;
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

      foreach (Service service in this)
      {
        if (service.Row.Table.Columns.Contains("CreatorID") && (int)service.Row["CreatorID"] == 0) service.Row["CreatorID"] = LoginUser.UserID;
        if (service.Row.Table.Columns.Contains("ModifierID")) service.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Service FindByServiceID(int serviceID)
    {
      foreach (Service service in this)
      {
        if (service.ServiceID == serviceID)
        {
          return service;
        }
      }
      return null;
    }

    public virtual Service AddNewService()
    {
      if (Table.Columns.Count < 1) LoadColumns("Services");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Service(row, this);
    }
    
    public virtual void LoadByServiceID(int serviceID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ServiceID], [Name], [Enabled], [Interval], [LastStartTime], [LastEndTime], [LastResult], [LastError], [ErrorCount], [RunCount], [RunTimeAvg], [RunTimeMax], [AssemblyName], [AutoStart], [HealthTime], [NameSpace], [HealthMaxMinutes] FROM [dbo].[Services] WHERE ([ServiceID] = @ServiceID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ServiceID", serviceID);
        Fill(command);
      }
    }
    
    public static Service GetService(LoginUser loginUser, int serviceID)
    {
      Services services = new Services(loginUser);
      services.LoadByServiceID(serviceID);
      if (services.IsEmpty)
        return null;
      else
        return services[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Service> Members

    public IEnumerator<Service> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Service(row, this);
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
