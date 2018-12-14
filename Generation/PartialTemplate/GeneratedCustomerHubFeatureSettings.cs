using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CustomerHubFeatureSetting : BaseItem
  {
    private CustomerHubFeatureSettings _customerHubFeatureSettings;
    
    public CustomerHubFeatureSetting(DataRow row, CustomerHubFeatureSettings customerHubFeatureSettings): base(row, customerHubFeatureSettings)
    {
      _customerHubFeatureSettings = customerHubFeatureSettings;
    }
	
    #region Properties
    
    public CustomerHubFeatureSettings Collection
    {
      get { return _customerHubFeatureSettings; }
    }
        
    
    
    
    public int CustomerHubFeatureSettingID
    {
      get { return (int)Row["CustomerHubFeatureSettingID"]; }
    }
    

    

    
    public bool ShowProducts
    {
      get { return (bool)Row["ShowProducts"]; }
      set { Row["ShowProducts"] = CheckValue("ShowProducts", value); }
    }
    
    public bool ShowKnowledgeBase
    {
      get { return (bool)Row["ShowKnowledgeBase"]; }
      set { Row["ShowKnowledgeBase"] = CheckValue("ShowKnowledgeBase", value); }
    }
    
    public int CustomerHubID
    {
      get { return (int)Row["CustomerHubID"]; }
      set { Row["CustomerHubID"] = CheckValue("CustomerHubID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class CustomerHubFeatureSettings : BaseCollection, IEnumerable<CustomerHubFeatureSetting>
  {
    public CustomerHubFeatureSettings(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CustomerHubFeatureSettings"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CustomerHubFeatureSettingID"; }
    }



    public CustomerHubFeatureSetting this[int index]
    {
      get { return new CustomerHubFeatureSetting(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CustomerHubFeatureSetting customerHubFeatureSetting);
    partial void AfterRowInsert(CustomerHubFeatureSetting customerHubFeatureSetting);
    partial void BeforeRowEdit(CustomerHubFeatureSetting customerHubFeatureSetting);
    partial void AfterRowEdit(CustomerHubFeatureSetting customerHubFeatureSetting);
    partial void BeforeRowDelete(int customerHubFeatureSettingID);
    partial void AfterRowDelete(int customerHubFeatureSettingID);    

    partial void BeforeDBDelete(int customerHubFeatureSettingID);
    partial void AfterDBDelete(int customerHubFeatureSettingID);    

    #endregion

    #region Public Methods

    public CustomerHubFeatureSettingProxy[] GetCustomerHubFeatureSettingProxies()
    {
      List<CustomerHubFeatureSettingProxy> list = new List<CustomerHubFeatureSettingProxy>();

      foreach (CustomerHubFeatureSetting item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int customerHubFeatureSettingID)
    {
      BeforeDBDelete(customerHubFeatureSettingID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomerHubFeatureSettings] WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID);";
        deleteCommand.Parameters.Add("CustomerHubFeatureSettingID", SqlDbType.Int);
        deleteCommand.Parameters["CustomerHubFeatureSettingID"].Value = customerHubFeatureSettingID;

        BeforeRowDelete(customerHubFeatureSettingID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(customerHubFeatureSettingID);
      }
      AfterDBDelete(customerHubFeatureSettingID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CustomerHubFeatureSettingsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CustomerHubFeatureSettings] SET     [CustomerHubID] = @CustomerHubID,    [ShowKnowledgeBase] = @ShowKnowledgeBase,    [ShowProducts] = @ShowProducts  WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID);";

		
		tempParameter = updateCommand.Parameters.Add("CustomerHubFeatureSettingID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ShowKnowledgeBase", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ShowProducts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CustomerHubFeatureSettings] (    [CustomerHubID],    [ShowKnowledgeBase],    [ShowProducts]) VALUES ( @CustomerHubID, @ShowKnowledgeBase, @ShowProducts); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ShowProducts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ShowKnowledgeBase", SqlDbType.Bit, 1);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomerHubFeatureSettings] WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID);";
		deleteCommand.Parameters.Add("CustomerHubFeatureSettingID", SqlDbType.Int);

		try
		{
		  foreach (CustomerHubFeatureSetting customerHubFeatureSetting in this)
		  {
			if (customerHubFeatureSetting.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(customerHubFeatureSetting);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = customerHubFeatureSetting.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CustomerHubFeatureSettingID"].AutoIncrement = false;
			  Table.Columns["CustomerHubFeatureSettingID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				customerHubFeatureSetting.Row["CustomerHubFeatureSettingID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(customerHubFeatureSetting);
			}
			else if (customerHubFeatureSetting.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(customerHubFeatureSetting);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = customerHubFeatureSetting.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(customerHubFeatureSetting);
			}
			else if (customerHubFeatureSetting.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)customerHubFeatureSetting.Row["CustomerHubFeatureSettingID", DataRowVersion.Original];
			  deleteCommand.Parameters["CustomerHubFeatureSettingID"].Value = id;
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

      foreach (CustomerHubFeatureSetting customerHubFeatureSetting in this)
      {
        if (customerHubFeatureSetting.Row.Table.Columns.Contains("CreatorID") && (int)customerHubFeatureSetting.Row["CreatorID"] == 0) customerHubFeatureSetting.Row["CreatorID"] = LoginUser.UserID;
        if (customerHubFeatureSetting.Row.Table.Columns.Contains("ModifierID")) customerHubFeatureSetting.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CustomerHubFeatureSetting FindByCustomerHubFeatureSettingID(int customerHubFeatureSettingID)
    {
      foreach (CustomerHubFeatureSetting customerHubFeatureSetting in this)
      {
        if (customerHubFeatureSetting.CustomerHubFeatureSettingID == customerHubFeatureSettingID)
        {
          return customerHubFeatureSetting;
        }
      }
      return null;
    }

    public virtual CustomerHubFeatureSetting AddNewCustomerHubFeatureSetting()
    {
      if (Table.Columns.Count < 1) LoadColumns("CustomerHubFeatureSettings");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CustomerHubFeatureSetting(row, this);
    }
    
    public virtual void LoadByCustomerHubFeatureSettingID(int customerHubFeatureSettingID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CustomerHubFeatureSettingID], [CustomerHubID], [ShowKnowledgeBase], [ShowProducts] FROM [dbo].[CustomerHubFeatureSettings] WHERE ([CustomerHubFeatureSettingID] = @CustomerHubFeatureSettingID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CustomerHubFeatureSettingID", customerHubFeatureSettingID);
        Fill(command);
      }
    }
    
    public static CustomerHubFeatureSetting GetCustomerHubFeatureSetting(LoginUser loginUser, int customerHubFeatureSettingID)
    {
      CustomerHubFeatureSettings customerHubFeatureSettings = new CustomerHubFeatureSettings(loginUser);
      customerHubFeatureSettings.LoadByCustomerHubFeatureSettingID(customerHubFeatureSettingID);
      if (customerHubFeatureSettings.IsEmpty)
        return null;
      else
        return customerHubFeatureSettings[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CustomerHubFeatureSetting> Members

    public IEnumerator<CustomerHubFeatureSetting> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CustomerHubFeatureSetting(row, this);
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
