using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CustomerHub : BaseItem
  {
    private CustomerHubs _customerHubs;
    
    public CustomerHub(DataRow row, CustomerHubs customerHubs): base(row, customerHubs)
    {
      _customerHubs = customerHubs;
    }
	
    #region Properties
    
    public CustomerHubs Collection
    {
      get { return _customerHubs; }
    }
        
    
    
    
    public int CustomerHubID
    {
      get { return (int)Row["CustomerHubID"]; }
    }
    

    
    public string PortalName
    {
      get { return Row["PortalName"] != DBNull.Value ? (string)Row["PortalName"] : null; }
      set { Row["PortalName"] = CheckValue("PortalName", value); }
    }
    
    public string CNameURL
    {
      get { return Row["CNameURL"] != DBNull.Value ? (string)Row["CNameURL"] : null; }
      set { Row["CNameURL"] = CheckValue("CNameURL", value); }
    }
    
    public int? ProductFamilyID
    {
      get { return Row["ProductFamilyID"] != DBNull.Value ? (int?)Row["ProductFamilyID"] : null; }
      set { Row["ProductFamilyID"] = CheckValue("ProductFamilyID", value); }
    }
    

    
    public bool IsActive
    {
      get { return (bool)Row["IsActive"]; }
      set { Row["IsActive"] = CheckValue("IsActive", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class CustomerHubs : BaseCollection, IEnumerable<CustomerHub>
  {
    public CustomerHubs(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CustomerHubs"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CustomerHubID"; }
    }



    public CustomerHub this[int index]
    {
      get { return new CustomerHub(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CustomerHub customerHub);
    partial void AfterRowInsert(CustomerHub customerHub);
    partial void BeforeRowEdit(CustomerHub customerHub);
    partial void AfterRowEdit(CustomerHub customerHub);
    partial void BeforeRowDelete(int customerHubID);
    partial void AfterRowDelete(int customerHubID);    

    partial void BeforeDBDelete(int customerHubID);
    partial void AfterDBDelete(int customerHubID);    

    #endregion

    #region Public Methods

    public CustomerHubProxy[] GetCustomerHubProxies()
    {
      List<CustomerHubProxy> list = new List<CustomerHubProxy>();

      foreach (CustomerHub item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int customerHubID)
    {
      BeforeDBDelete(customerHubID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomerHubs] WHERE ([CustomerHubID] = @CustomerHubID);";
        deleteCommand.Parameters.Add("CustomerHubID", SqlDbType.Int);
        deleteCommand.Parameters["CustomerHubID"].Value = customerHubID;

        BeforeRowDelete(customerHubID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(customerHubID);
      }
      AfterDBDelete(customerHubID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CustomerHubsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CustomerHubs] SET     [OrganizationID] = @OrganizationID,    [PortalName] = @PortalName,    [CNameURL] = @CNameURL,    [IsActive] = @IsActive,    [ProductFamilyID] = @ProductFamilyID  WHERE ([CustomerHubID] = @CustomerHubID);";

		
		tempParameter = updateCommand.Parameters.Add("CustomerHubID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("PortalName", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CNameURL", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ProductFamilyID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CustomerHubs] (    [OrganizationID],    [PortalName],    [CNameURL],    [IsActive],    [ProductFamilyID]) VALUES ( @OrganizationID, @PortalName, @CNameURL, @IsActive, @ProductFamilyID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ProductFamilyID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CNameURL", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PortalName", SqlDbType.NVarChar, 100);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomerHubs] WHERE ([CustomerHubID] = @CustomerHubID);";
		deleteCommand.Parameters.Add("CustomerHubID", SqlDbType.Int);

		try
		{
		  foreach (CustomerHub customerHub in this)
		  {
			if (customerHub.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(customerHub);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = customerHub.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CustomerHubID"].AutoIncrement = false;
			  Table.Columns["CustomerHubID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				customerHub.Row["CustomerHubID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(customerHub);
			}
			else if (customerHub.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(customerHub);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = customerHub.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(customerHub);
			}
			else if (customerHub.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)customerHub.Row["CustomerHubID", DataRowVersion.Original];
			  deleteCommand.Parameters["CustomerHubID"].Value = id;
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

      foreach (CustomerHub customerHub in this)
      {
        if (customerHub.Row.Table.Columns.Contains("CreatorID") && (int)customerHub.Row["CreatorID"] == 0) customerHub.Row["CreatorID"] = LoginUser.UserID;
        if (customerHub.Row.Table.Columns.Contains("ModifierID")) customerHub.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CustomerHub FindByCustomerHubID(int customerHubID)
    {
      foreach (CustomerHub customerHub in this)
      {
        if (customerHub.CustomerHubID == customerHubID)
        {
          return customerHub;
        }
      }
      return null;
    }

    public virtual CustomerHub AddNewCustomerHub()
    {
      if (Table.Columns.Count < 1) LoadColumns("CustomerHubs");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CustomerHub(row, this);
    }
    
    public virtual void LoadByCustomerHubID(int customerHubID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CustomerHubID], [OrganizationID], [PortalName], [CNameURL], [IsActive], [ProductFamilyID] FROM [dbo].[CustomerHubs] WHERE ([CustomerHubID] = @CustomerHubID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CustomerHubID", customerHubID);
        Fill(command);
      }
    }
    
    public static CustomerHub GetCustomerHub(LoginUser loginUser, int customerHubID)
    {
      CustomerHubs customerHubs = new CustomerHubs(loginUser);
      customerHubs.LoadByCustomerHubID(customerHubID);
      if (customerHubs.IsEmpty)
        return null;
      else
        return customerHubs[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CustomerHub> Members

    public IEnumerator<CustomerHub> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CustomerHub(row, this);
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
