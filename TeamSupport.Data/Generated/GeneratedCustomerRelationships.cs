using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CustomerRelationship : BaseItem
  {
    private CustomerRelationships _customerRelationships;
    
    public CustomerRelationship(DataRow row, CustomerRelationships customerRelationships): base(row, customerRelationships)
    {
      _customerRelationships = customerRelationships;
    }
	
    #region Properties
    
    public CustomerRelationships Collection
    {
      get { return _customerRelationships; }
    }
        
    
    
    
    public int CustomerRelationshipID
    {
      get { return (int)Row["CustomerRelationshipID"]; }
    }
    

    

    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public int RelatedCustomerID
    {
      get { return (int)Row["RelatedCustomerID"]; }
      set { Row["RelatedCustomerID"] = CheckValue("RelatedCustomerID", value); }
    }
    
    public int CustomerID
    {
      get { return (int)Row["CustomerID"]; }
      set { Row["CustomerID"] = CheckValue("CustomerID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
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

  public partial class CustomerRelationships : BaseCollection, IEnumerable<CustomerRelationship>
  {
    public CustomerRelationships(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CustomerRelationships"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CustomerRelationshipID"; }
    }



    public CustomerRelationship this[int index]
    {
      get { return new CustomerRelationship(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CustomerRelationship customerRelationship);
    partial void AfterRowInsert(CustomerRelationship customerRelationship);
    partial void BeforeRowEdit(CustomerRelationship customerRelationship);
    partial void AfterRowEdit(CustomerRelationship customerRelationship);
    partial void BeforeRowDelete(int customerRelationshipID);
    partial void AfterRowDelete(int customerRelationshipID);    

    partial void BeforeDBDelete(int customerRelationshipID);
    partial void AfterDBDelete(int customerRelationshipID);    

    #endregion

    #region Public Methods

    public CustomerRelationshipProxy[] GetCustomerRelationshipProxies()
    {
      List<CustomerRelationshipProxy> list = new List<CustomerRelationshipProxy>();

      foreach (CustomerRelationship item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int customerRelationshipID)
    {
      BeforeDBDelete(customerRelationshipID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomerRelationships] WHERE ([CustomerRelationshipID] = @CustomerRelationshipID);";
        deleteCommand.Parameters.Add("CustomerRelationshipID", SqlDbType.Int);
        deleteCommand.Parameters["CustomerRelationshipID"].Value = customerRelationshipID;

        BeforeRowDelete(customerRelationshipID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(customerRelationshipID);
      }
      AfterDBDelete(customerRelationshipID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CustomerRelationshipsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CustomerRelationships] SET     [CustomerID] = @CustomerID,    [RelatedCustomerID] = @RelatedCustomerID  WHERE ([CustomerRelationshipID] = @CustomerRelationshipID);";

		
		tempParameter = updateCommand.Parameters.Add("CustomerRelationshipID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CustomerID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RelatedCustomerID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CustomerRelationships] (    [CustomerID],    [RelatedCustomerID],    [DateCreated],    [CreatorID]) VALUES ( @CustomerID, @RelatedCustomerID, @DateCreated, @CreatorID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("RelatedCustomerID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CustomerID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomerRelationships] WHERE ([CustomerRelationshipID] = @CustomerRelationshipID);";
		deleteCommand.Parameters.Add("CustomerRelationshipID", SqlDbType.Int);

		try
		{
		  foreach (CustomerRelationship customerRelationship in this)
		  {
			if (customerRelationship.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(customerRelationship);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = customerRelationship.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CustomerRelationshipID"].AutoIncrement = false;
			  Table.Columns["CustomerRelationshipID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				customerRelationship.Row["CustomerRelationshipID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(customerRelationship);
			}
			else if (customerRelationship.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(customerRelationship);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = customerRelationship.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(customerRelationship);
			}
			else if (customerRelationship.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)customerRelationship.Row["CustomerRelationshipID", DataRowVersion.Original];
			  deleteCommand.Parameters["CustomerRelationshipID"].Value = id;
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

      foreach (CustomerRelationship customerRelationship in this)
      {
        if (customerRelationship.Row.Table.Columns.Contains("CreatorID") && (int)customerRelationship.Row["CreatorID"] == 0) customerRelationship.Row["CreatorID"] = LoginUser.UserID;
        if (customerRelationship.Row.Table.Columns.Contains("ModifierID")) customerRelationship.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CustomerRelationship FindByCustomerRelationshipID(int customerRelationshipID)
    {
      foreach (CustomerRelationship customerRelationship in this)
      {
        if (customerRelationship.CustomerRelationshipID == customerRelationshipID)
        {
          return customerRelationship;
        }
      }
      return null;
    }

    public virtual CustomerRelationship AddNewCustomerRelationship()
    {
      if (Table.Columns.Count < 1) LoadColumns("CustomerRelationships");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CustomerRelationship(row, this);
    }
    
    public virtual void LoadByCustomerRelationshipID(int customerRelationshipID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CustomerRelationshipID], [CustomerID], [RelatedCustomerID], [DateCreated], [CreatorID] FROM [dbo].[CustomerRelationships] WHERE ([CustomerRelationshipID] = @CustomerRelationshipID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CustomerRelationshipID", customerRelationshipID);
        Fill(command);
      }
    }
    
    public static CustomerRelationship GetCustomerRelationship(LoginUser loginUser, int customerRelationshipID)
    {
      CustomerRelationships customerRelationships = new CustomerRelationships(loginUser);
      customerRelationships.LoadByCustomerRelationshipID(customerRelationshipID);
      if (customerRelationships.IsEmpty)
        return null;
      else
        return customerRelationships[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CustomerRelationship> Members

    public IEnumerator<CustomerRelationship> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CustomerRelationship(row, this);
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
