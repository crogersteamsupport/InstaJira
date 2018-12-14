using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CustomerHubView : BaseItem
  {
    private CustomerHubViews _customerHubViews;
    
    public CustomerHubView(DataRow row, CustomerHubViews customerHubViews): base(row, customerHubViews)
    {
      _customerHubViews = customerHubViews;
    }
	
    #region Properties
    
    public CustomerHubViews Collection
    {
      get { return _customerHubViews; }
    }
        
    
    
    
    public int CustomerHubViewID
    {
      get { return (int)Row["CustomerHubViewID"]; }
    }
    

    

    
    public bool IsActive
    {
      get { return (bool)Row["IsActive"]; }
      set { Row["IsActive"] = CheckValue("IsActive", value); }
    }
    
    public string Route
    {
      get { return (string)Row["Route"]; }
      set { Row["Route"] = CheckValue("Route", value); }
    }
    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckValue("Name", value); }
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

  public partial class CustomerHubViews : BaseCollection, IEnumerable<CustomerHubView>
  {
    public CustomerHubViews(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CustomerHubViews"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CustomerHubViewID"; }
    }



    public CustomerHubView this[int index]
    {
      get { return new CustomerHubView(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CustomerHubView customerHubView);
    partial void AfterRowInsert(CustomerHubView customerHubView);
    partial void BeforeRowEdit(CustomerHubView customerHubView);
    partial void AfterRowEdit(CustomerHubView customerHubView);
    partial void BeforeRowDelete(int customerHubViewID);
    partial void AfterRowDelete(int customerHubViewID);    

    partial void BeforeDBDelete(int customerHubViewID);
    partial void AfterDBDelete(int customerHubViewID);    

    #endregion

    #region Public Methods

    public CustomerHubViewProxy[] GetCustomerHubViewProxies()
    {
      List<CustomerHubViewProxy> list = new List<CustomerHubViewProxy>();

      foreach (CustomerHubView item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int customerHubViewID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomerHubViews] WHERE ([CustomerHubViewID] = @CustomerHubViewID);";
        deleteCommand.Parameters.Add("CustomerHubViewID", SqlDbType.Int);
        deleteCommand.Parameters["CustomerHubViewID"].Value = customerHubViewID;

        BeforeDBDelete(customerHubViewID);
        BeforeRowDelete(customerHubViewID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(customerHubViewID);
        AfterDBDelete(customerHubViewID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CustomerHubViewsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CustomerHubViews] SET     [Name] = @Name,    [Route] = @Route,    [IsActive] = @IsActive  WHERE ([CustomerHubViewID] = @CustomerHubViewID);";

		
		tempParameter = updateCommand.Parameters.Add("CustomerHubViewID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Route", SqlDbType.NVarChar, 60);
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
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CustomerHubViews] (    [Name],    [Route],    [IsActive],    [DateCreated]) VALUES ( @Name, @Route, @IsActive, @DateCreated); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Route", SqlDbType.NVarChar, 60);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.NVarChar, -1);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomerHubViews] WHERE ([CustomerHubViewID] = @CustomerHubViewID);";
		deleteCommand.Parameters.Add("CustomerHubViewID", SqlDbType.Int);

		try
		{
		  foreach (CustomerHubView customerHubView in this)
		  {
			if (customerHubView.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(customerHubView);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = customerHubView.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CustomerHubViewID"].AutoIncrement = false;
			  Table.Columns["CustomerHubViewID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				customerHubView.Row["CustomerHubViewID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(customerHubView);
			}
			else if (customerHubView.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(customerHubView);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = customerHubView.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(customerHubView);
			}
			else if (customerHubView.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)customerHubView.Row["CustomerHubViewID", DataRowVersion.Original];
			  deleteCommand.Parameters["CustomerHubViewID"].Value = id;
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

      foreach (CustomerHubView customerHubView in this)
      {
        if (customerHubView.Row.Table.Columns.Contains("CreatorID") && (int)customerHubView.Row["CreatorID"] == 0) customerHubView.Row["CreatorID"] = LoginUser.UserID;
        if (customerHubView.Row.Table.Columns.Contains("ModifierID")) customerHubView.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CustomerHubView FindByCustomerHubViewID(int customerHubViewID)
    {
      foreach (CustomerHubView customerHubView in this)
      {
        if (customerHubView.CustomerHubViewID == customerHubViewID)
        {
          return customerHubView;
        }
      }
      return null;
    }

    public virtual CustomerHubView AddNewCustomerHubView()
    {
      if (Table.Columns.Count < 1) LoadColumns("CustomerHubViews");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CustomerHubView(row, this);
    }
    
    public virtual void LoadByCustomerHubViewID(int customerHubViewID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CustomerHubViewID], [Name], [Route], [IsActive], [DateCreated] FROM [dbo].[CustomerHubViews] WHERE ([CustomerHubViewID] = @CustomerHubViewID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CustomerHubViewID", customerHubViewID);
        Fill(command);
      }
    }
    
    public static CustomerHubView GetCustomerHubView(LoginUser loginUser, int customerHubViewID)
    {
      CustomerHubViews customerHubViews = new CustomerHubViews(loginUser);
      customerHubViews.LoadByCustomerHubViewID(customerHubViewID);
      if (customerHubViews.IsEmpty)
        return null;
      else
        return customerHubViews[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CustomerHubView> Members

    public IEnumerator<CustomerHubView> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CustomerHubView(row, this);
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
