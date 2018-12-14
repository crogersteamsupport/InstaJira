using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class UserDevice : BaseItem
  {
    private UserDevices _userDevices;
    
    public UserDevice(DataRow row, UserDevices userDevices): base(row, userDevices)
    {
      _userDevices = userDevices;
    }
	
    #region Properties
    
    public UserDevices Collection
    {
      get { return _userDevices; }
    }
        
    
    
    
    public int UserDeviceID
    {
      get { return (int)Row["UserDeviceID"]; }
    }
    

    

    
    public bool IsActivated
    {
      get { return (bool)Row["IsActivated"]; }
      set { Row["IsActivated"] = CheckValue("IsActivated", value); }
    }
    
    public string DeviceID
    {
      get { return (string)Row["DeviceID"]; }
      set { Row["DeviceID"] = CheckValue("DeviceID", value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateActivated
    {
      get { return DateToLocal((DateTime)Row["DateActivated"]); }
      set { Row["DateActivated"] = CheckValue("DateActivated", value); }
    }

    public DateTime DateActivatedUtc
    {
      get { return (DateTime)Row["DateActivated"]; }
    }
    

    #endregion
    
    
  }

  public partial class UserDevices : BaseCollection, IEnumerable<UserDevice>
  {
    public UserDevices(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "UserDevices"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "UserDeviceID"; }
    }



    public UserDevice this[int index]
    {
      get { return new UserDevice(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(UserDevice userDevice);
    partial void AfterRowInsert(UserDevice userDevice);
    partial void BeforeRowEdit(UserDevice userDevice);
    partial void AfterRowEdit(UserDevice userDevice);
    partial void BeforeRowDelete(int userDeviceID);
    partial void AfterRowDelete(int userDeviceID);    

    partial void BeforeDBDelete(int userDeviceID);
    partial void AfterDBDelete(int userDeviceID);    

    #endregion

    #region Public Methods

    public UserDeviceProxy[] GetUserDeviceProxies()
    {
      List<UserDeviceProxy> list = new List<UserDeviceProxy>();

      foreach (UserDevice item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int userDeviceID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[UserDevices] WHERE ([UserDeviceID] = @UserDeviceID);";
        deleteCommand.Parameters.Add("UserDeviceID", SqlDbType.Int);
        deleteCommand.Parameters["UserDeviceID"].Value = userDeviceID;

        BeforeDBDelete(userDeviceID);
        BeforeRowDelete(userDeviceID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(userDeviceID);
        AfterDBDelete(userDeviceID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("UserDevicesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[UserDevices] SET     [UserID] = @UserID,    [DeviceID] = @DeviceID,    [DateActivated] = @DateActivated,    [IsActivated] = @IsActivated  WHERE ([UserDeviceID] = @UserDeviceID);";

		
		tempParameter = updateCommand.Parameters.Add("UserDeviceID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("DeviceID", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateActivated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsActivated", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[UserDevices] (    [UserID],    [DeviceID],    [DateActivated],    [IsActivated]) VALUES ( @UserID, @DeviceID, @DateActivated, @IsActivated); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("IsActivated", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateActivated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DeviceID", SqlDbType.VarChar, 100);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[UserDevices] WHERE ([UserDeviceID] = @UserDeviceID);";
		deleteCommand.Parameters.Add("UserDeviceID", SqlDbType.Int);

		try
		{
		  foreach (UserDevice userDevice in this)
		  {
			if (userDevice.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(userDevice);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = userDevice.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["UserDeviceID"].AutoIncrement = false;
			  Table.Columns["UserDeviceID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				userDevice.Row["UserDeviceID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(userDevice);
			}
			else if (userDevice.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(userDevice);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = userDevice.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(userDevice);
			}
			else if (userDevice.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)userDevice.Row["UserDeviceID", DataRowVersion.Original];
			  deleteCommand.Parameters["UserDeviceID"].Value = id;
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

      foreach (UserDevice userDevice in this)
      {
        if (userDevice.Row.Table.Columns.Contains("CreatorID") && (int)userDevice.Row["CreatorID"] == 0) userDevice.Row["CreatorID"] = LoginUser.UserID;
        if (userDevice.Row.Table.Columns.Contains("ModifierID")) userDevice.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public UserDevice FindByUserDeviceID(int userDeviceID)
    {
      foreach (UserDevice userDevice in this)
      {
        if (userDevice.UserDeviceID == userDeviceID)
        {
          return userDevice;
        }
      }
      return null;
    }

    public virtual UserDevice AddNewUserDevice()
    {
      if (Table.Columns.Count < 1) LoadColumns("UserDevices");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new UserDevice(row, this);
    }
    
    public virtual void LoadByUserDeviceID(int userDeviceID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [UserDeviceID], [UserID], [DeviceID], [DateActivated], [IsActivated] FROM [dbo].[UserDevices] WHERE ([UserDeviceID] = @UserDeviceID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("UserDeviceID", userDeviceID);
        Fill(command);
      }
    }
    
    public static UserDevice GetUserDevice(LoginUser loginUser, int userDeviceID)
    {
      UserDevices userDevices = new UserDevices(loginUser);
      userDevices.LoadByUserDeviceID(userDeviceID);
      if (userDevices.IsEmpty)
        return null;
      else
        return userDevices[0];
    }
    
    
    

    #endregion

    #region IEnumerable<UserDevice> Members

    public IEnumerator<UserDevice> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new UserDevice(row, this);
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
