using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketTemplate : BaseItem
  {
    private TicketTemplates _ticketTemplates;
    
    public TicketTemplate(DataRow row, TicketTemplates ticketTemplates): base(row, ticketTemplates)
    {
      _ticketTemplates = ticketTemplates;
    }
	
    #region Properties
    
    public TicketTemplates Collection
    {
      get { return _ticketTemplates; }
    }
        
    
    
    
    public int TicketTemplateID
    {
      get { return (int)Row["TicketTemplateID"]; }
    }
    

    
    public int? TicketTypeID
    {
      get { return Row["TicketTypeID"] != DBNull.Value ? (int?)Row["TicketTypeID"] : null; }
      set { Row["TicketTypeID"] = CheckNull(value); }
    }
    
    public string TriggerText
    {
      get { return Row["TriggerText"] != DBNull.Value ? (string)Row["TriggerText"] : null; }
      set { Row["TriggerText"] = CheckNull(value); }
    }
    

    
    public int ModifierID
    {
      get { return (int)Row["ModifierID"]; }
      set { Row["ModifierID"] = CheckNull(value); }
    }
    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckNull(value); }
    }
    
    public string TemplateText
    {
      get { return (string)Row["TemplateText"]; }
      set { Row["TemplateText"] = CheckNull(value); }
    }
    
    public bool IsEnabled
    {
      get { return (bool)Row["IsEnabled"]; }
      set { Row["IsEnabled"] = CheckNull(value); }
    }
    
    public TicketTemplateType TemplateType
    {
      get { return (TicketTemplateType)Row["TemplateType"]; }
      set { Row["TemplateType"] = CheckNull(value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckNull(value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateModified
    {
      get { return DateToLocal((DateTime)Row["DateModified"]); }
      set { Row["DateModified"] = CheckNull(value); }
    }
    
    public DateTime DateCreated
    {
      get { return DateToLocal((DateTime)Row["DateCreated"]); }
      set { Row["DateCreated"] = CheckNull(value); }
    }
    

    #endregion
    
    
  }

  public partial class TicketTemplates : BaseCollection, IEnumerable<TicketTemplate>
  {
    public TicketTemplates(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketTemplates"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketTemplateID"; }
    }



    public TicketTemplate this[int index]
    {
      get { return new TicketTemplate(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketTemplate ticketTemplate);
    partial void AfterRowInsert(TicketTemplate ticketTemplate);
    partial void BeforeRowEdit(TicketTemplate ticketTemplate);
    partial void AfterRowEdit(TicketTemplate ticketTemplate);
    partial void BeforeRowDelete(int ticketTemplateID);
    partial void AfterRowDelete(int ticketTemplateID);    

    partial void BeforeDBDelete(int ticketTemplateID);
    partial void AfterDBDelete(int ticketTemplateID);    

    #endregion

    #region Public Methods

    public TicketTemplateProxy[] GetTicketTemplateProxies()
    {
      List<TicketTemplateProxy> list = new List<TicketTemplateProxy>();

      foreach (TicketTemplate item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketTemplateID)
    {
      BeforeDBDelete(ticketTemplateID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.StoredProcedure;
        deleteCommand.CommandText = "uspGeneratedDeleteTicketTemplate";
        deleteCommand.Parameters.Add("TicketTemplateID", SqlDbType.Int);
        deleteCommand.Parameters["TicketTemplateID"].Value = ticketTemplateID;

        BeforeRowDelete(ticketTemplateID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(ticketTemplateID);
      }
      AfterDBDelete(ticketTemplateID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketTemplatesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.StoredProcedure;
		updateCommand.CommandText = "uspGeneratedUpdateTicketTemplate";

		
		tempParameter = updateCommand.Parameters.Add("TicketTemplateID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("TemplateType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TriggerText", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TemplateText", SqlDbType.VarChar, 8000);
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
		insertCommand.CommandType = CommandType.StoredProcedure;
		insertCommand.CommandText = "uspGeneratedInsertTicketTemplate";

		
		tempParameter = insertCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("TemplateText", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TriggerText", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TemplateType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		deleteCommand.CommandType = CommandType.StoredProcedure;
		deleteCommand.CommandText = "uspGeneratedDeleteTicketTemplate";
		deleteCommand.Parameters.Add("TicketTemplateID", SqlDbType.Int);

		try
		{
		  foreach (TicketTemplate ticketTemplate in this)
		  {
			if (ticketTemplate.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketTemplate);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketTemplate.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketTemplateID"].AutoIncrement = false;
			  Table.Columns["TicketTemplateID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketTemplate.Row["TicketTemplateID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketTemplate);
			}
			else if (ticketTemplate.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketTemplate);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketTemplate.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketTemplate);
			}
			else if (ticketTemplate.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketTemplate.Row["TicketTemplateID", DataRowVersion.Original];
			  deleteCommand.Parameters["TicketTemplateID"].Value = id;
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

      foreach (TicketTemplate ticketTemplate in this)
      {
        if (ticketTemplate.Row.Table.Columns.Contains("CreatorID") && (int)ticketTemplate.Row["CreatorID"] == 0) ticketTemplate.Row["CreatorID"] = LoginUser.UserID;
        if (ticketTemplate.Row.Table.Columns.Contains("ModifierID")) ticketTemplate.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketTemplate FindByTicketTemplateID(int ticketTemplateID)
    {
      foreach (TicketTemplate ticketTemplate in this)
      {
        if (ticketTemplate.TicketTemplateID == ticketTemplateID)
        {
          return ticketTemplate;
        }
      }
      return null;
    }

    public virtual TicketTemplate AddNewTicketTemplate()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketTemplates");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketTemplate(row, this);
    }
    
    public virtual void LoadByTicketTemplateID(int ticketTemplateID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "uspGeneratedSelectTicketTemplate";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("TicketTemplateID", ticketTemplateID);
        Fill(command);
      }
    }
    
    public static TicketTemplate GetTicketTemplate(LoginUser loginUser, int ticketTemplateID)
    {
      TicketTemplates ticketTemplates = new TicketTemplates(loginUser);
      ticketTemplates.LoadByTicketTemplateID(ticketTemplateID);
      if (ticketTemplates.IsEmpty)
        return null;
      else
        return ticketTemplates[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TicketTemplate> Members

    public IEnumerator<TicketTemplate> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketTemplate(row, this);
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
