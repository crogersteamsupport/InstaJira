using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class JiraInstanceProductItem : BaseItem
  {
    private JiraInstanceProduct _jiraInstanceProduct;
    
    public JiraInstanceProductItem(DataRow row, JiraInstanceProduct jiraInstanceProduct): base(row, jiraInstanceProduct)
    {
      _jiraInstanceProduct = jiraInstanceProduct;
    }
	
    #region Properties
    
    public JiraInstanceProduct Collection
    {
      get { return _jiraInstanceProduct; }
    }
        
    
    
    
    public int JiraInstanceProductId
    {
      get { return (int)Row["JiraInstanceProductId"]; }
    }
    

    
    public int? ProductId
    {
      get { return Row["ProductId"] != DBNull.Value ? (int?)Row["ProductId"] : null; }
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

  public partial class JiraInstanceProduct : BaseCollection, IEnumerable<JiraInstanceProductItem>
  {
    public JiraInstanceProduct(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "JiraInstanceProduct"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "JiraInstanceProductId"; }
    }



    public JiraInstanceProductItem this[int index]
    {
      get { return new JiraInstanceProductItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(JiraInstanceProductItem jiraInstanceProductItem);
    partial void AfterRowInsert(JiraInstanceProductItem jiraInstanceProductItem);
    partial void BeforeRowEdit(JiraInstanceProductItem jiraInstanceProductItem);
    partial void AfterRowEdit(JiraInstanceProductItem jiraInstanceProductItem);
    partial void BeforeRowDelete(int jiraInstanceProductId);
    partial void AfterRowDelete(int jiraInstanceProductId);    

    partial void BeforeDBDelete(int jiraInstanceProductId);
    partial void AfterDBDelete(int jiraInstanceProductId);    

    #endregion

    #region Public Methods

    public JiraInstanceProductItemProxy[] GetJiraInstanceProductItemProxies()
    {
      List<JiraInstanceProductItemProxy> list = new List<JiraInstanceProductItemProxy>();

      foreach (JiraInstanceProductItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int jiraInstanceProductId)
    {
      BeforeDBDelete(jiraInstanceProductId);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[JiraInstanceProduct] WHERE ([JiraInstanceProductId] = @JiraInstanceProductId);";
        deleteCommand.Parameters.Add("JiraInstanceProductId", SqlDbType.Int);
        deleteCommand.Parameters["JiraInstanceProductId"].Value = jiraInstanceProductId;

        BeforeRowDelete(jiraInstanceProductId);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(jiraInstanceProductId);
      }
      AfterDBDelete(jiraInstanceProductId);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("JiraInstanceProductSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[JiraInstanceProduct] SET     [CrmLinkId] = @CrmLinkId,    [ProductId] = @ProductId  WHERE ([JiraInstanceProductId] = @JiraInstanceProductId);";

		
		tempParameter = updateCommand.Parameters.Add("JiraInstanceProductId", SqlDbType.Int, 4);
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[JiraInstanceProduct] (    [CrmLinkId],    [ProductId]) VALUES ( @CrmLinkId, @ProductId); SET @Identity = SCOPE_IDENTITY();";

		
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[JiraInstanceProduct] WHERE ([JiraInstanceProductId] = @JiraInstanceProductId);";
		deleteCommand.Parameters.Add("JiraInstanceProductId", SqlDbType.Int);

		try
		{
		  foreach (JiraInstanceProductItem jiraInstanceProductItem in this)
		  {
			if (jiraInstanceProductItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(jiraInstanceProductItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = jiraInstanceProductItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["JiraInstanceProductId"].AutoIncrement = false;
			  Table.Columns["JiraInstanceProductId"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				jiraInstanceProductItem.Row["JiraInstanceProductId"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(jiraInstanceProductItem);
			}
			else if (jiraInstanceProductItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(jiraInstanceProductItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = jiraInstanceProductItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(jiraInstanceProductItem);
			}
			else if (jiraInstanceProductItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)jiraInstanceProductItem.Row["JiraInstanceProductId", DataRowVersion.Original];
			  deleteCommand.Parameters["JiraInstanceProductId"].Value = id;
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

      foreach (JiraInstanceProductItem jiraInstanceProductItem in this)
      {
        if (jiraInstanceProductItem.Row.Table.Columns.Contains("CreatorID") && (int)jiraInstanceProductItem.Row["CreatorID"] == 0) jiraInstanceProductItem.Row["CreatorID"] = LoginUser.UserID;
        if (jiraInstanceProductItem.Row.Table.Columns.Contains("ModifierID")) jiraInstanceProductItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public JiraInstanceProductItem FindByJiraInstanceProductId(int jiraInstanceProductId)
    {
      foreach (JiraInstanceProductItem jiraInstanceProductItem in this)
      {
        if (jiraInstanceProductItem.JiraInstanceProductId == jiraInstanceProductId)
        {
          return jiraInstanceProductItem;
        }
      }
      return null;
    }

    public virtual JiraInstanceProductItem AddNewJiraInstanceProductItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("JiraInstanceProduct");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new JiraInstanceProductItem(row, this);
    }
    
    public virtual void LoadByJiraInstanceProductId(int jiraInstanceProductId)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [JiraInstanceProductId], [CrmLinkId], [ProductId] FROM [dbo].[JiraInstanceProduct] WHERE ([JiraInstanceProductId] = @JiraInstanceProductId);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("JiraInstanceProductId", jiraInstanceProductId);
        Fill(command);
      }
    }
    
    public static JiraInstanceProductItem GetJiraInstanceProductItem(LoginUser loginUser, int jiraInstanceProductId)
    {
      JiraInstanceProduct jiraInstanceProduct = new JiraInstanceProduct(loginUser);
      jiraInstanceProduct.LoadByJiraInstanceProductId(jiraInstanceProductId);
      if (jiraInstanceProduct.IsEmpty)
        return null;
      else
        return jiraInstanceProduct[0];
    }
    
    
    

    #endregion

    #region IEnumerable<JiraInstanceProductItem> Members

    public IEnumerator<JiraInstanceProductItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new JiraInstanceProductItem(row, this);
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
