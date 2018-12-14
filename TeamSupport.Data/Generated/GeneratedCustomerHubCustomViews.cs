using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CustomerHubCustomView : BaseItem
  {
    private CustomerHubCustomViews _customerHubCustomViews;
    
    public CustomerHubCustomView(DataRow row, CustomerHubCustomViews customerHubCustomViews): base(row, customerHubCustomViews)
    {
      _customerHubCustomViews = customerHubCustomViews;
    }
	
    #region Properties
    
    public CustomerHubCustomViews Collection
    {
      get { return _customerHubCustomViews; }
    }
        
    
    
    
    public int CustomerHubCustomViewID
    {
      get { return (int)Row["CustomerHubCustomViewID"]; }
    }
    

    
    public int? ModifierID
    {
      get { return Row["ModifierID"] != DBNull.Value ? (int?)Row["ModifierID"] : null; }
      set { Row["ModifierID"] = CheckValue("ModifierID", value); }
    }
    

    
    public bool IsActive
    {
      get { return (bool)Row["IsActive"]; }
      set { Row["IsActive"] = CheckValue("IsActive", value); }
    }
    
    public string CustomView
    {
      get { return (string)Row["CustomView"]; }
      set { Row["CustomView"] = CheckValue("CustomView", value); }
    }
    
    public int CustomerHubViewID
    {
      get { return (int)Row["CustomerHubViewID"]; }
      set { Row["CustomerHubViewID"] = CheckValue("CustomerHubViewID", value); }
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

  public partial class CustomerHubCustomViews : BaseCollection, IEnumerable<CustomerHubCustomView>
  {
    public CustomerHubCustomViews(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CustomerHubCustomViews"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CustomerHubCustomViewID"; }
    }



    public CustomerHubCustomView this[int index]
    {
      get { return new CustomerHubCustomView(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CustomerHubCustomView customerHubCustomView);
    partial void AfterRowInsert(CustomerHubCustomView customerHubCustomView);
    partial void BeforeRowEdit(CustomerHubCustomView customerHubCustomView);
    partial void AfterRowEdit(CustomerHubCustomView customerHubCustomView);
    partial void BeforeRowDelete(int customerHubCustomViewID);
    partial void AfterRowDelete(int customerHubCustomViewID);    

    partial void BeforeDBDelete(int customerHubCustomViewID);
    partial void AfterDBDelete(int customerHubCustomViewID);    

    #endregion

    #region Public Methods

    public CustomerHubCustomViewProxy[] GetCustomerHubCustomViewProxies()
    {
      List<CustomerHubCustomViewProxy> list = new List<CustomerHubCustomViewProxy>();

      foreach (CustomerHubCustomView item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int customerHubCustomViewID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomerHubCustomViews] WHERE ([CustomerHubCustomViewID] = @CustomerHubCustomViewID);";
        deleteCommand.Parameters.Add("CustomerHubCustomViewID", SqlDbType.Int);
        deleteCommand.Parameters["CustomerHubCustomViewID"].Value = customerHubCustomViewID;

        BeforeDBDelete(customerHubCustomViewID);
        BeforeRowDelete(customerHubCustomViewID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(customerHubCustomViewID);
        AfterDBDelete(customerHubCustomViewID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CustomerHubCustomViewsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CustomerHubCustomViews] SET     [CustomerHubID] = @CustomerHubID,    [CustomerHubViewID] = @CustomerHubViewID,    [CustomView] = @CustomView,    [IsActive] = @IsActive,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID  WHERE ([CustomerHubCustomViewID] = @CustomerHubCustomViewID);";

		
		tempParameter = updateCommand.Parameters.Add("CustomerHubCustomViewID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("CustomerHubViewID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CustomView", SqlDbType.NVarChar, -1);
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CustomerHubCustomViews] (    [CustomerHubID],    [CustomerHubViewID],    [CustomView],    [IsActive],    [DateCreated],    [DateModified],    [ModifierID]) VALUES ( @CustomerHubID, @CustomerHubViewID, @CustomView, @IsActive, @DateCreated, @DateModified, @ModifierID); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("CustomView", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CustomerHubViewID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomerHubCustomViews] WHERE ([CustomerHubCustomViewID] = @CustomerHubCustomViewID);";
		deleteCommand.Parameters.Add("CustomerHubCustomViewID", SqlDbType.Int);

		try
		{
		  foreach (CustomerHubCustomView customerHubCustomView in this)
		  {
			if (customerHubCustomView.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(customerHubCustomView);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = customerHubCustomView.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CustomerHubCustomViewID"].AutoIncrement = false;
			  Table.Columns["CustomerHubCustomViewID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				customerHubCustomView.Row["CustomerHubCustomViewID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(customerHubCustomView);
			}
			else if (customerHubCustomView.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(customerHubCustomView);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = customerHubCustomView.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(customerHubCustomView);
			}
			else if (customerHubCustomView.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)customerHubCustomView.Row["CustomerHubCustomViewID", DataRowVersion.Original];
			  deleteCommand.Parameters["CustomerHubCustomViewID"].Value = id;
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

      foreach (CustomerHubCustomView customerHubCustomView in this)
      {
        if (customerHubCustomView.Row.Table.Columns.Contains("CreatorID") && (int)customerHubCustomView.Row["CreatorID"] == 0) customerHubCustomView.Row["CreatorID"] = LoginUser.UserID;
        if (customerHubCustomView.Row.Table.Columns.Contains("ModifierID")) customerHubCustomView.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CustomerHubCustomView FindByCustomerHubCustomViewID(int customerHubCustomViewID)
    {
      foreach (CustomerHubCustomView customerHubCustomView in this)
      {
        if (customerHubCustomView.CustomerHubCustomViewID == customerHubCustomViewID)
        {
          return customerHubCustomView;
        }
      }
      return null;
    }

    public virtual CustomerHubCustomView AddNewCustomerHubCustomView()
    {
      if (Table.Columns.Count < 1) LoadColumns("CustomerHubCustomViews");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CustomerHubCustomView(row, this);
    }
    
    public virtual void LoadByCustomerHubCustomViewID(int customerHubCustomViewID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CustomerHubCustomViewID], [CustomerHubID], [CustomerHubViewID], [CustomView], [IsActive], [DateCreated], [DateModified], [ModifierID] FROM [dbo].[CustomerHubCustomViews] WHERE ([CustomerHubCustomViewID] = @CustomerHubCustomViewID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CustomerHubCustomViewID", customerHubCustomViewID);
        Fill(command);
      }
    }
    
    public static CustomerHubCustomView GetCustomerHubCustomView(LoginUser loginUser, int customerHubCustomViewID)
    {
      CustomerHubCustomViews customerHubCustomViews = new CustomerHubCustomViews(loginUser);
      customerHubCustomViews.LoadByCustomerHubCustomViewID(customerHubCustomViewID);
      if (customerHubCustomViews.IsEmpty)
        return null;
      else
        return customerHubCustomViews[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CustomerHubCustomView> Members

    public IEnumerator<CustomerHubCustomView> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CustomerHubCustomView(row, this);
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
