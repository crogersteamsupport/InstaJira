using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CustomerHubAuthenticationItem : BaseItem
  {
    private CustomerHubAuthentication _customerHubAuthentication;
    
    public CustomerHubAuthenticationItem(DataRow row, CustomerHubAuthentication customerHubAuthentication): base(row, customerHubAuthentication)
    {
      _customerHubAuthentication = customerHubAuthentication;
    }
	
    #region Properties
    
    public CustomerHubAuthentication Collection
    {
      get { return _customerHubAuthentication; }
    }
        
    
    
    
    public int CustomerHubAuthenticationID
    {
      get { return (int)Row["CustomerHubAuthenticationID"]; }
    }
    

    

    
    public bool EnableSSO
    {
      get { return (bool)Row["EnableSSO"]; }
      set { Row["EnableSSO"] = CheckValue("EnableSSO", value); }
    }
    
    public bool EnableRequestAccess
    {
      get { return (bool)Row["EnableRequestAccess"]; }
      set { Row["EnableRequestAccess"] = CheckValue("EnableRequestAccess", value); }
    }
    
    public bool EnableSelfRegister
    {
      get { return (bool)Row["EnableSelfRegister"]; }
      set { Row["EnableSelfRegister"] = CheckValue("EnableSelfRegister", value); }
    }
    
    public int CustomerHubID
    {
      get { return (int)Row["CustomerHubID"]; }
      set { Row["CustomerHubID"] = CheckValue("CustomerHubID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class CustomerHubAuthentication : BaseCollection, IEnumerable<CustomerHubAuthenticationItem>
  {
    public CustomerHubAuthentication(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CustomerHubAuthentication"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CustomerHubAuthenticationID"; }
    }



    public CustomerHubAuthenticationItem this[int index]
    {
      get { return new CustomerHubAuthenticationItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CustomerHubAuthenticationItem customerHubAuthenticationItem);
    partial void AfterRowInsert(CustomerHubAuthenticationItem customerHubAuthenticationItem);
    partial void BeforeRowEdit(CustomerHubAuthenticationItem customerHubAuthenticationItem);
    partial void AfterRowEdit(CustomerHubAuthenticationItem customerHubAuthenticationItem);
    partial void BeforeRowDelete(int customerHubAuthenticationID);
    partial void AfterRowDelete(int customerHubAuthenticationID);    

    partial void BeforeDBDelete(int customerHubAuthenticationID);
    partial void AfterDBDelete(int customerHubAuthenticationID);    

    #endregion

    #region Public Methods

    public CustomerHubAuthenticationItemProxy[] GetCustomerHubAuthenticationItemProxies()
    {
      List<CustomerHubAuthenticationItemProxy> list = new List<CustomerHubAuthenticationItemProxy>();

      foreach (CustomerHubAuthenticationItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int customerHubAuthenticationID)
    {
      BeforeDBDelete(customerHubAuthenticationID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomerHubAuthentication] WHERE ([CustomerHubAuthenticationID] = @CustomerHubAuthenticationID);";
        deleteCommand.Parameters.Add("CustomerHubAuthenticationID", SqlDbType.Int);
        deleteCommand.Parameters["CustomerHubAuthenticationID"].Value = customerHubAuthenticationID;

        BeforeRowDelete(customerHubAuthenticationID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(customerHubAuthenticationID);
      }
      AfterDBDelete(customerHubAuthenticationID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CustomerHubAuthenticationSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CustomerHubAuthentication] SET     [CustomerHubID] = @CustomerHubID,    [EnableSelfRegister] = @EnableSelfRegister,    [EnableRequestAccess] = @EnableRequestAccess,    [EnableSSO] = @EnableSSO  WHERE ([CustomerHubAuthenticationID] = @CustomerHubAuthenticationID);";

		
		tempParameter = updateCommand.Parameters.Add("CustomerHubAuthenticationID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("EnableSelfRegister", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableRequestAccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EnableSSO", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CustomerHubAuthentication] (    [CustomerHubID],    [EnableSelfRegister],    [EnableRequestAccess],    [EnableSSO]) VALUES ( @CustomerHubID, @EnableSelfRegister, @EnableRequestAccess, @EnableSSO); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("EnableSSO", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableRequestAccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EnableSelfRegister", SqlDbType.Bit, 1);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomerHubAuthentication] WHERE ([CustomerHubAuthenticationID] = @CustomerHubAuthenticationID);";
		deleteCommand.Parameters.Add("CustomerHubAuthenticationID", SqlDbType.Int);

		try
		{
		  foreach (CustomerHubAuthenticationItem customerHubAuthenticationItem in this)
		  {
			if (customerHubAuthenticationItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(customerHubAuthenticationItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = customerHubAuthenticationItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CustomerHubAuthenticationID"].AutoIncrement = false;
			  Table.Columns["CustomerHubAuthenticationID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				customerHubAuthenticationItem.Row["CustomerHubAuthenticationID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(customerHubAuthenticationItem);
			}
			else if (customerHubAuthenticationItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(customerHubAuthenticationItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = customerHubAuthenticationItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(customerHubAuthenticationItem);
			}
			else if (customerHubAuthenticationItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)customerHubAuthenticationItem.Row["CustomerHubAuthenticationID", DataRowVersion.Original];
			  deleteCommand.Parameters["CustomerHubAuthenticationID"].Value = id;
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

      foreach (CustomerHubAuthenticationItem customerHubAuthenticationItem in this)
      {
        if (customerHubAuthenticationItem.Row.Table.Columns.Contains("CreatorID") && (int)customerHubAuthenticationItem.Row["CreatorID"] == 0) customerHubAuthenticationItem.Row["CreatorID"] = LoginUser.UserID;
        if (customerHubAuthenticationItem.Row.Table.Columns.Contains("ModifierID")) customerHubAuthenticationItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CustomerHubAuthenticationItem FindByCustomerHubAuthenticationID(int customerHubAuthenticationID)
    {
      foreach (CustomerHubAuthenticationItem customerHubAuthenticationItem in this)
      {
        if (customerHubAuthenticationItem.CustomerHubAuthenticationID == customerHubAuthenticationID)
        {
          return customerHubAuthenticationItem;
        }
      }
      return null;
    }

    public virtual CustomerHubAuthenticationItem AddNewCustomerHubAuthenticationItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("CustomerHubAuthentication");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CustomerHubAuthenticationItem(row, this);
    }
    
    public virtual void LoadByCustomerHubAuthenticationID(int customerHubAuthenticationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CustomerHubAuthenticationID], [CustomerHubID], [EnableSelfRegister], [EnableRequestAccess], [EnableSSO] FROM [dbo].[CustomerHubAuthentication] WHERE ([CustomerHubAuthenticationID] = @CustomerHubAuthenticationID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CustomerHubAuthenticationID", customerHubAuthenticationID);
        Fill(command);
      }
    }
    
    public static CustomerHubAuthenticationItem GetCustomerHubAuthenticationItem(LoginUser loginUser, int customerHubAuthenticationID)
    {
      CustomerHubAuthentication customerHubAuthentication = new CustomerHubAuthentication(loginUser);
      customerHubAuthentication.LoadByCustomerHubAuthenticationID(customerHubAuthenticationID);
      if (customerHubAuthentication.IsEmpty)
        return null;
      else
        return customerHubAuthentication[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CustomerHubAuthenticationItem> Members

    public IEnumerator<CustomerHubAuthenticationItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CustomerHubAuthenticationItem(row, this);
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
