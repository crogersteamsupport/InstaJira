using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CustomerHubDisplaySetting : BaseItem
  {
    private CustomerHubDisplaySettings _customerHubDisplaySettings;
    
    public CustomerHubDisplaySetting(DataRow row, CustomerHubDisplaySettings customerHubDisplaySettings): base(row, customerHubDisplaySettings)
    {
      _customerHubDisplaySettings = customerHubDisplaySettings;
    }
	
    #region Properties
    
    public CustomerHubDisplaySettings Collection
    {
      get { return _customerHubDisplaySettings; }
    }
        
    
    
    
    public int CustomerHubDisplaySettingID
    {
      get { return (int)Row["CustomerHubDisplaySettingID"]; }
    }
    

    
    public string FontFamily
    {
      get { return Row["FontFamily"] != DBNull.Value ? (string)Row["FontFamily"] : null; }
      set { Row["FontFamily"] = CheckValue("FontFamily", value); }
    }
    
    public string FontColor
    {
      get { return Row["FontColor"] != DBNull.Value ? (string)Row["FontColor"] : null; }
      set { Row["FontColor"] = CheckValue("FontColor", value); }
    }
    
    public string Color1
    {
      get { return Row["Color1"] != DBNull.Value ? (string)Row["Color1"] : null; }
      set { Row["Color1"] = CheckValue("Color1", value); }
    }
    
    public string Color2
    {
      get { return Row["Color2"] != DBNull.Value ? (string)Row["Color2"] : null; }
      set { Row["Color2"] = CheckValue("Color2", value); }
    }
    
    public string Color3
    {
      get { return Row["Color3"] != DBNull.Value ? (string)Row["Color3"] : null; }
      set { Row["Color3"] = CheckValue("Color3", value); }
    }
    
    public string Color4
    {
      get { return Row["Color4"] != DBNull.Value ? (string)Row["Color4"] : null; }
      set { Row["Color4"] = CheckValue("Color4", value); }
    }
    
    public string Color5
    {
      get { return Row["Color5"] != DBNull.Value ? (string)Row["Color5"] : null; }
      set { Row["Color5"] = CheckValue("Color5", value); }
    }
    
    public int? ModifierID
    {
      get { return Row["ModifierID"] != DBNull.Value ? (int?)Row["ModifierID"] : null; }
      set { Row["ModifierID"] = CheckValue("ModifierID", value); }
    }
    

    
    public int CustomerHubID
    {
      get { return (int)Row["CustomerHubID"]; }
      set { Row["CustomerHubID"] = CheckValue("CustomerHubID", value); }
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
    

    #endregion
    
    
  }

  public partial class CustomerHubDisplaySettings : BaseCollection, IEnumerable<CustomerHubDisplaySetting>
  {
    public CustomerHubDisplaySettings(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CustomerHubDisplaySettings"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CustomerHubDisplaySettingID"; }
    }



    public CustomerHubDisplaySetting this[int index]
    {
      get { return new CustomerHubDisplaySetting(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CustomerHubDisplaySetting customerHubDisplaySetting);
    partial void AfterRowInsert(CustomerHubDisplaySetting customerHubDisplaySetting);
    partial void BeforeRowEdit(CustomerHubDisplaySetting customerHubDisplaySetting);
    partial void AfterRowEdit(CustomerHubDisplaySetting customerHubDisplaySetting);
    partial void BeforeRowDelete(int customerHubDisplaySettingID);
    partial void AfterRowDelete(int customerHubDisplaySettingID);    

    partial void BeforeDBDelete(int customerHubDisplaySettingID);
    partial void AfterDBDelete(int customerHubDisplaySettingID);    

    #endregion

    #region Public Methods

    public CustomerHubDisplaySettingProxy[] GetCustomerHubDisplaySettingProxies()
    {
      List<CustomerHubDisplaySettingProxy> list = new List<CustomerHubDisplaySettingProxy>();

      foreach (CustomerHubDisplaySetting item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int customerHubDisplaySettingID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomerHubDisplaySettings] WHERE ([CustomerHubDisplaySettingID] = @CustomerHubDisplaySettingID);";
        deleteCommand.Parameters.Add("CustomerHubDisplaySettingID", SqlDbType.Int);
        deleteCommand.Parameters["CustomerHubDisplaySettingID"].Value = customerHubDisplaySettingID;

        BeforeDBDelete(customerHubDisplaySettingID);
        BeforeRowDelete(customerHubDisplaySettingID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(customerHubDisplaySettingID);
        AfterDBDelete(customerHubDisplaySettingID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CustomerHubDisplaySettingsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CustomerHubDisplaySettings] SET     [CustomerHubID] = @CustomerHubID,    [FontFamily] = @FontFamily,    [FontColor] = @FontColor,    [Color1] = @Color1,    [Color2] = @Color2,    [Color3] = @Color3,    [Color4] = @Color4,    [Color5] = @Color5,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID  WHERE ([CustomerHubDisplaySettingID] = @CustomerHubDisplaySettingID);";

		
		tempParameter = updateCommand.Parameters.Add("CustomerHubDisplaySettingID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CustomerHubID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("FontFamily", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("FontColor", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Color1", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Color2", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Color3", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Color4", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Color5", SqlDbType.VarChar, 100);
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CustomerHubDisplaySettings] (    [CustomerHubID],    [FontFamily],    [FontColor],    [Color1],    [Color2],    [Color3],    [Color4],    [Color5],    [DateModified],    [ModifierID]) VALUES ( @CustomerHubID, @FontFamily, @FontColor, @Color1, @Color2, @Color3, @Color4, @Color5, @DateModified, @ModifierID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("Color5", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Color4", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Color3", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Color2", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Color1", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("FontColor", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("FontFamily", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CustomerHubID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomerHubDisplaySettings] WHERE ([CustomerHubDisplaySettingID] = @CustomerHubDisplaySettingID);";
		deleteCommand.Parameters.Add("CustomerHubDisplaySettingID", SqlDbType.Int);

		try
		{
		  foreach (CustomerHubDisplaySetting customerHubDisplaySetting in this)
		  {
			if (customerHubDisplaySetting.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(customerHubDisplaySetting);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = customerHubDisplaySetting.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CustomerHubDisplaySettingID"].AutoIncrement = false;
			  Table.Columns["CustomerHubDisplaySettingID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				customerHubDisplaySetting.Row["CustomerHubDisplaySettingID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(customerHubDisplaySetting);
			}
			else if (customerHubDisplaySetting.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(customerHubDisplaySetting);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = customerHubDisplaySetting.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(customerHubDisplaySetting);
			}
			else if (customerHubDisplaySetting.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)customerHubDisplaySetting.Row["CustomerHubDisplaySettingID", DataRowVersion.Original];
			  deleteCommand.Parameters["CustomerHubDisplaySettingID"].Value = id;
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

      foreach (CustomerHubDisplaySetting customerHubDisplaySetting in this)
      {
        if (customerHubDisplaySetting.Row.Table.Columns.Contains("CreatorID") && (int)customerHubDisplaySetting.Row["CreatorID"] == 0) customerHubDisplaySetting.Row["CreatorID"] = LoginUser.UserID;
        if (customerHubDisplaySetting.Row.Table.Columns.Contains("ModifierID")) customerHubDisplaySetting.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CustomerHubDisplaySetting FindByCustomerHubDisplaySettingID(int customerHubDisplaySettingID)
    {
      foreach (CustomerHubDisplaySetting customerHubDisplaySetting in this)
      {
        if (customerHubDisplaySetting.CustomerHubDisplaySettingID == customerHubDisplaySettingID)
        {
          return customerHubDisplaySetting;
        }
      }
      return null;
    }

    public virtual CustomerHubDisplaySetting AddNewCustomerHubDisplaySetting()
    {
      if (Table.Columns.Count < 1) LoadColumns("CustomerHubDisplaySettings");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CustomerHubDisplaySetting(row, this);
    }
    
    public virtual void LoadByCustomerHubDisplaySettingID(int customerHubDisplaySettingID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CustomerHubDisplaySettingID], [CustomerHubID], [FontFamily], [FontColor], [Color1], [Color2], [Color3], [Color4], [Color5], [DateModified], [ModifierID] FROM [dbo].[CustomerHubDisplaySettings] WHERE ([CustomerHubDisplaySettingID] = @CustomerHubDisplaySettingID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CustomerHubDisplaySettingID", customerHubDisplaySettingID);
        Fill(command);
      }
    }
    
    public static CustomerHubDisplaySetting GetCustomerHubDisplaySetting(LoginUser loginUser, int customerHubDisplaySettingID)
    {
      CustomerHubDisplaySettings customerHubDisplaySettings = new CustomerHubDisplaySettings(loginUser);
      customerHubDisplaySettings.LoadByCustomerHubDisplaySettingID(customerHubDisplaySettingID);
      if (customerHubDisplaySettings.IsEmpty)
        return null;
      else
        return customerHubDisplaySettings[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CustomerHubDisplaySetting> Members

    public IEnumerator<CustomerHubDisplaySetting> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CustomerHubDisplaySetting(row, this);
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
