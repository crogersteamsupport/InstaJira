using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class EMailAlternateInboundItem : BaseItem
  {
    private EMailAlternateInbound _eMailAlternateInbound;
    
    public EMailAlternateInboundItem(DataRow row, EMailAlternateInbound eMailAlternateInbound): base(row, eMailAlternateInbound)
    {
      _eMailAlternateInbound = eMailAlternateInbound;
    }
	
    #region Properties
    
    public EMailAlternateInbound Collection
    {
      get { return _eMailAlternateInbound; }
    }
        
    
    
    

    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public int? GroupToAssign
    {
      get { return Row["GroupToAssign"] != DBNull.Value ? (int?)Row["GroupToAssign"] : null; }
      set { Row["GroupToAssign"] = CheckValue("GroupToAssign", value); }
    }
    
    public int? DefaultTicketType
    {
      get { return Row["DefaultTicketType"] != DBNull.Value ? (int?)Row["DefaultTicketType"] : null; }
      set { Row["DefaultTicketType"] = CheckValue("DefaultTicketType", value); }
    }
    
    public int? ProductID
    {
      get { return Row["ProductID"] != DBNull.Value ? (int?)Row["ProductID"] : null; }
      set { Row["ProductID"] = CheckValue("ProductID", value); }
    }
    
    public string SendingEMailAddress
    {
      get { return Row["SendingEMailAddress"] != DBNull.Value ? (string)Row["SendingEMailAddress"] : null; }
      set { Row["SendingEMailAddress"] = CheckValue("SendingEMailAddress", value); }
    }
    

    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public Guid SystemEMailID
    {
      get { return (Guid)Row["SystemEMailID"]; }
      set { Row["SystemEMailID"] = CheckValue("SystemEMailID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class EMailAlternateInbound : BaseCollection, IEnumerable<EMailAlternateInboundItem>
  {
    public EMailAlternateInbound(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "EMailAlternateInbound"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "SystemEMailID"; }
    }



    public EMailAlternateInboundItem this[int index]
    {
      get { return new EMailAlternateInboundItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(EMailAlternateInboundItem eMailAlternateInboundItem);
    partial void AfterRowInsert(EMailAlternateInboundItem eMailAlternateInboundItem);
    partial void BeforeRowEdit(EMailAlternateInboundItem eMailAlternateInboundItem);
    partial void AfterRowEdit(EMailAlternateInboundItem eMailAlternateInboundItem);
    partial void BeforeRowDelete(int systemEMailID);
    partial void AfterRowDelete(int systemEMailID);    

    partial void BeforeDBDelete(int systemEMailID);
    partial void AfterDBDelete(int systemEMailID);    

    #endregion

    #region Public Methods

    public EMailAlternateInboundItemProxy[] GetEMailAlternateInboundItemProxies()
    {
      List<EMailAlternateInboundItemProxy> list = new List<EMailAlternateInboundItemProxy>();

      foreach (EMailAlternateInboundItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int systemEMailID)
    {
      BeforeDBDelete(systemEMailID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[EMailAlternateInbound] WHERE ([SystemEMailID] = @SystemEMailID);";
        deleteCommand.Parameters.Add("SystemEMailID", SqlDbType.Int);
        deleteCommand.Parameters["SystemEMailID"].Value = systemEMailID;

        BeforeRowDelete(systemEMailID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(systemEMailID);
      }
      AfterDBDelete(systemEMailID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("EMailAlternateInboundSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[EMailAlternateInbound] SET     [OrganizationID] = @OrganizationID,    [Description] = @Description,    [GroupToAssign] = @GroupToAssign,    [DefaultTicketType] = @DefaultTicketType,    [ProductID] = @ProductID,    [SendingEMailAddress] = @SendingEMailAddress  WHERE ([SystemEMailID] = @SystemEMailID);";

		
		tempParameter = updateCommand.Parameters.Add("SystemEMailID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("GroupToAssign", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DefaultTicketType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SendingEMailAddress", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[EMailAlternateInbound] (    [SystemEMailID],    [OrganizationID],    [Description],    [GroupToAssign],    [DefaultTicketType],    [ProductID],    [SendingEMailAddress]) VALUES ( @SystemEMailID, @OrganizationID, @Description, @GroupToAssign, @DefaultTicketType, @ProductID, @SendingEMailAddress); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("SendingEMailAddress", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DefaultTicketType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("GroupToAssign", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 500);
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
		
		tempParameter = insertCommand.Parameters.Add("SystemEMailID", SqlDbType.UniqueIdentifier, 16);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[EMailAlternateInbound] WHERE ([SystemEMailID] = @SystemEMailID);";
		deleteCommand.Parameters.Add("SystemEMailID", SqlDbType.Int);

		try
		{
		  foreach (EMailAlternateInboundItem eMailAlternateInboundItem in this)
		  {
			if (eMailAlternateInboundItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(eMailAlternateInboundItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = eMailAlternateInboundItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["SystemEMailID"].AutoIncrement = false;
			  Table.Columns["SystemEMailID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				eMailAlternateInboundItem.Row["SystemEMailID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(eMailAlternateInboundItem);
			}
			else if (eMailAlternateInboundItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(eMailAlternateInboundItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = eMailAlternateInboundItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(eMailAlternateInboundItem);
			}
			else if (eMailAlternateInboundItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)eMailAlternateInboundItem.Row["SystemEMailID", DataRowVersion.Original];
			  deleteCommand.Parameters["SystemEMailID"].Value = id;
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

      foreach (EMailAlternateInboundItem eMailAlternateInboundItem in this)
      {
        if (eMailAlternateInboundItem.Row.Table.Columns.Contains("CreatorID") && (int)eMailAlternateInboundItem.Row["CreatorID"] == 0) eMailAlternateInboundItem.Row["CreatorID"] = LoginUser.UserID;
        if (eMailAlternateInboundItem.Row.Table.Columns.Contains("ModifierID")) eMailAlternateInboundItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }



    public virtual EMailAlternateInboundItem AddNewEMailAlternateInboundItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("EMailAlternateInbound");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new EMailAlternateInboundItem(row, this);
    }
    
    public virtual void LoadBySystemEMailID(int systemEMailID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [SystemEMailID], [OrganizationID], [Description], [GroupToAssign], [DefaultTicketType], [ProductID], [SendingEMailAddress] FROM [dbo].[EMailAlternateInbound] WHERE ([SystemEMailID] = @SystemEMailID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("SystemEMailID", systemEMailID);
        Fill(command);
      }
    }
    
    public static EMailAlternateInboundItem GetEMailAlternateInboundItem(LoginUser loginUser, int systemEMailID)
    {
      EMailAlternateInbound eMailAlternateInbound = new EMailAlternateInbound(loginUser);
      eMailAlternateInbound.LoadBySystemEMailID(systemEMailID);
      if (eMailAlternateInbound.IsEmpty)
        return null;
      else
        return eMailAlternateInbound[0];
    }
    
    
    

    #endregion

    #region IEnumerable<EMailAlternateInboundItem> Members

    public IEnumerator<EMailAlternateInboundItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new EMailAlternateInboundItem(row, this);
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
