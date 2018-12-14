using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class JiraInstanceProduct : BaseItem
  {
    private JiraInstanceProducts _jiraInstanceProducts;
    
    public JiraInstanceProduct(DataRow row, JiraInstanceProducts jiraInstanceProducts): base(row, jiraInstanceProducts)
    {
      _jiraInstanceProducts = jiraInstanceProducts;
    }
	
    #region Properties
    
    public JiraInstanceProducts Collection
    {
      get { return _jiraInstanceProducts; }
    }
        
    
    
    
    public int JiraInstanceProductsId
    {
      get { return (int)Row["JiraInstanceProductsId"]; }
    }
    

    
    public int ProductId
    {
		get { return (int)Row["ProductId"]; }
      set { Row["ProductId"] = CheckValue("ProductId", value); }
    }
    

    
    public int CrmLinkId
    {
      get { return (int)Row["CrmLinkId"]; }
      set { Row["CrmLinkId"] = CheckValue("CrmLinkId", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class JiraInstanceProducts : BaseCollection, IEnumerable<JiraInstanceProduct>
  {
    public JiraInstanceProducts(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "JiraInstanceProducts"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "JiraInstanceProductsId"; }
    }



    public JiraInstanceProduct this[int index]
    {
      get { return new JiraInstanceProduct(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(JiraInstanceProduct jiraInstanceProduct);
    partial void AfterRowInsert(JiraInstanceProduct jiraInstanceProduct);
    partial void BeforeRowEdit(JiraInstanceProduct jiraInstanceProduct);
    partial void AfterRowEdit(JiraInstanceProduct jiraInstanceProduct);
    partial void BeforeRowDelete(int jiraInstanceProductsId);
    partial void AfterRowDelete(int jiraInstanceProductsId);    

    partial void BeforeDBDelete(int jiraInstanceProductsId);
    partial void AfterDBDelete(int jiraInstanceProductsId);    

    #endregion

    #region Public Methods

    public JiraInstanceProductProxy[] GetJiraInstanceProductProxies()
    {
      List<JiraInstanceProductProxy> list = new List<JiraInstanceProductProxy>();

      foreach (JiraInstanceProduct item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int jiraInstanceProductsId)
    {
      BeforeDBDelete(jiraInstanceProductsId);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[JiraInstanceProducts] WHERE ([JiraInstanceProductsId] = @JiraInstanceProductsId);";
        deleteCommand.Parameters.Add("JiraInstanceProductsId", SqlDbType.Int);
        deleteCommand.Parameters["JiraInstanceProductsId"].Value = jiraInstanceProductsId;

        BeforeRowDelete(jiraInstanceProductsId);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(jiraInstanceProductsId);
      }
      AfterDBDelete(jiraInstanceProductsId);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("JiraInstanceProductsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[JiraInstanceProducts] SET     [CrmLinkId] = @CrmLinkId,    [ProductId] = @ProductId  WHERE ([JiraInstanceProductsId] = @JiraInstanceProductsId);";

		
		tempParameter = updateCommand.Parameters.Add("JiraInstanceProductsId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CrmLinkId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ProductId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[JiraInstanceProducts] (    [CrmLinkId],    [ProductId]) VALUES ( @CrmLinkId, @ProductId); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ProductId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CrmLinkId", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[JiraInstanceProducts] WHERE ([JiraInstanceProductsId] = @JiraInstanceProductsId);";
		deleteCommand.Parameters.Add("JiraInstanceProductsId", SqlDbType.Int);

		try
		{
		  foreach (JiraInstanceProduct jiraInstanceProduct in this)
		  {
			if (jiraInstanceProduct.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(jiraInstanceProduct);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = jiraInstanceProduct.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["JiraInstanceProductsId"].AutoIncrement = false;
			  Table.Columns["JiraInstanceProductsId"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				jiraInstanceProduct.Row["JiraInstanceProductsId"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(jiraInstanceProduct);
			}
			else if (jiraInstanceProduct.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(jiraInstanceProduct);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = jiraInstanceProduct.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(jiraInstanceProduct);
			}
			else if (jiraInstanceProduct.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)jiraInstanceProduct.Row["JiraInstanceProductsId", DataRowVersion.Original];
			  deleteCommand.Parameters["JiraInstanceProductsId"].Value = id;
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

      foreach (JiraInstanceProduct jiraInstanceProduct in this)
      {
        if (jiraInstanceProduct.Row.Table.Columns.Contains("CreatorID") && (int)jiraInstanceProduct.Row["CreatorID"] == 0) jiraInstanceProduct.Row["CreatorID"] = LoginUser.UserID;
        if (jiraInstanceProduct.Row.Table.Columns.Contains("ModifierID")) jiraInstanceProduct.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public JiraInstanceProduct FindByJiraInstanceProductsId(int jiraInstanceProductsId)
    {
      foreach (JiraInstanceProduct jiraInstanceProduct in this)
      {
        if (jiraInstanceProduct.JiraInstanceProductsId == jiraInstanceProductsId)
        {
          return jiraInstanceProduct;
        }
      }
      return null;
    }

    public virtual JiraInstanceProduct AddNewJiraInstanceProduct()
    {
      if (Table.Columns.Count < 1) LoadColumns("JiraInstanceProducts");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new JiraInstanceProduct(row, this);
    }
    
    public virtual void LoadByJiraInstanceProductsId(int jiraInstanceProductsId)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [JiraInstanceProductsId], [CrmLinkId], [ProductId] FROM [dbo].[JiraInstanceProducts] WHERE ([JiraInstanceProductsId] = @JiraInstanceProductsId);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("JiraInstanceProductsId", jiraInstanceProductsId);
        Fill(command);
      }
    }
    
    public static JiraInstanceProduct GetJiraInstanceProduct(LoginUser loginUser, int jiraInstanceProductsId)
    {
      JiraInstanceProducts jiraInstanceProducts = new JiraInstanceProducts(loginUser);
      jiraInstanceProducts.LoadByJiraInstanceProductsId(jiraInstanceProductsId);
      if (jiraInstanceProducts.IsEmpty)
        return null;
      else
        return jiraInstanceProducts[0];
    }
    
    
    

    #endregion

    #region IEnumerable<JiraInstanceProduct> Members

    public IEnumerator<JiraInstanceProduct> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new JiraInstanceProduct(row, this);
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
