using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketSeverity : BaseItem
  {
    private TicketSeverities _ticketSeverities;
    
    public TicketSeverity(DataRow row, TicketSeverities ticketSeverities): base(row, ticketSeverities)
    {
      _ticketSeverities = ticketSeverities;
    }
	
    #region Properties
    
    public TicketSeverities Collection
    {
      get { return _ticketSeverities; }
    }
        
    
    
    
    public int TicketSeverityID
    {
      get { return (int)Row["TicketSeverityID"]; }
    }
    

    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    

    
    public int ModifierID
    {
      get { return (int)Row["ModifierID"]; }
      set { Row["ModifierID"] = CheckValue("ModifierID", value); }
    }
    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public int Position
    {
      get { return (int)Row["Position"]; }
      set { Row["Position"] = CheckValue("Position", value); }
    }
    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckValue("Name", value); }
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

  public partial class TicketSeverities : BaseCollection, IEnumerable<TicketSeverity>
  {
    public TicketSeverities(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketSeverities"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketSeverityID"; }
    }



    public TicketSeverity this[int index]
    {
      get { return new TicketSeverity(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketSeverity ticketSeverity);
    partial void AfterRowInsert(TicketSeverity ticketSeverity);
    partial void BeforeRowEdit(TicketSeverity ticketSeverity);
    partial void AfterRowEdit(TicketSeverity ticketSeverity);
    partial void BeforeRowDelete(int ticketSeverityID);
    partial void AfterRowDelete(int ticketSeverityID);    

    partial void BeforeDBDelete(int ticketSeverityID);
    partial void AfterDBDelete(int ticketSeverityID);    

    #endregion

    #region Public Methods

    public TicketSeverityProxy[] GetTicketSeverityProxies()
    {
      List<TicketSeverityProxy> list = new List<TicketSeverityProxy>();

      foreach (TicketSeverity item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketSeverityID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketSeverities] WHERE ([TicketSeverityID] = @TicketSeverityID);";
        deleteCommand.Parameters.Add("TicketSeverityID", SqlDbType.Int);
        deleteCommand.Parameters["TicketSeverityID"].Value = ticketSeverityID;

        BeforeDBDelete(ticketSeverityID);
        BeforeRowDelete(ticketSeverityID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(ticketSeverityID);
        AfterDBDelete(ticketSeverityID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketSeveritiesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketSeverities] SET     [Name] = @Name,    [Description] = @Description,    [Position] = @Position,    [OrganizationID] = @OrganizationID,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID  WHERE ([TicketSeverityID] = @TicketSeverityID);";

		
		tempParameter = updateCommand.Parameters.Add("TicketSeverityID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Position", SqlDbType.Int, 4);
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketSeverities] (    [Name],    [Description],    [Position],    [OrganizationID],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID]) VALUES ( @Name, @Description, @Position, @OrganizationID, @DateCreated, @DateModified, @CreatorID, @ModifierID); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 255);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketSeverities] WHERE ([TicketSeverityID] = @TicketSeverityID);";
		deleteCommand.Parameters.Add("TicketSeverityID", SqlDbType.Int);

		try
		{
		  foreach (TicketSeverity ticketSeverity in this)
		  {
			if (ticketSeverity.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketSeverity);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketSeverity.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketSeverityID"].AutoIncrement = false;
			  Table.Columns["TicketSeverityID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketSeverity.Row["TicketSeverityID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketSeverity);
			}
			else if (ticketSeverity.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketSeverity);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketSeverity.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketSeverity);
			}
			else if (ticketSeverity.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketSeverity.Row["TicketSeverityID", DataRowVersion.Original];
			  deleteCommand.Parameters["TicketSeverityID"].Value = id;
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

      foreach (TicketSeverity ticketSeverity in this)
      {
        if (ticketSeverity.Row.Table.Columns.Contains("CreatorID") && (int)ticketSeverity.Row["CreatorID"] == 0) ticketSeverity.Row["CreatorID"] = LoginUser.UserID;
        if (ticketSeverity.Row.Table.Columns.Contains("ModifierID")) ticketSeverity.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketSeverity FindByTicketSeverityID(int ticketSeverityID)
    {
      foreach (TicketSeverity ticketSeverity in this)
      {
        if (ticketSeverity.TicketSeverityID == ticketSeverityID)
        {
          return ticketSeverity;
        }
      }
      return null;
    }

    public virtual TicketSeverity AddNewTicketSeverity()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketSeverities");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketSeverity(row, this);
    }
    
    public virtual void LoadByTicketSeverityID(int ticketSeverityID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TicketSeverityID], [Name], [Description], [Position], [OrganizationID], [DateCreated], [DateModified], [CreatorID], [ModifierID] FROM [dbo].[TicketSeverities] WHERE ([TicketSeverityID] = @TicketSeverityID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketSeverityID", ticketSeverityID);
        Fill(command);
      }
    }
    
    public static TicketSeverity GetTicketSeverity(LoginUser loginUser, int ticketSeverityID)
    {
      TicketSeverities ticketSeverities = new TicketSeverities(loginUser);
      ticketSeverities.LoadByTicketSeverityID(ticketSeverityID);
      if (ticketSeverities.IsEmpty)
        return null;
      else
        return ticketSeverities[0];
    }
    
    
     

    public void LoadByPosition(int organizationID, int position)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketSeverities WHERE (OrganizationID = @OrganizationID) AND (Position = @Position)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        command.Parameters.AddWithValue("Position", position);
        Fill(command);
      }
    }
    
    public void LoadAllPositions(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketSeverities WHERE (OrganizationID = @OrganizationID) ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void ValidatePositions(int organizationID)
    {
      TicketSeverities ticketSeverities = new TicketSeverities(LoginUser);
      ticketSeverities.LoadAllPositions(organizationID);
      int i = 0;
      foreach (TicketSeverity ticketSeverity in ticketSeverities)
      {
        ticketSeverity.Position = i;
        i++;
      }
      ticketSeverities.Save();
    }    

    public void MovePositionUp(int ticketSeverityID)
    {
      TicketSeverities types1 = new TicketSeverities(LoginUser);
      types1.LoadByTicketSeverityID(ticketSeverityID);
      if (types1.IsEmpty || types1[0].Position < 1) return;

      TicketSeverities types2 = new TicketSeverities(LoginUser);
      types2.LoadByPosition(types1[0].OrganizationID, types1[0].Position - 1);
      if (!types2.IsEmpty)
      {
        types2[0].Position = types2[0].Position + 1;
        types2.Save();
      }

      types1[0].Position = types1[0].Position - 1;
      types1.Save();
      ValidatePositions(LoginUser.OrganizationID);
    }
    
    public void MovePositionDown(int ticketSeverityID)
    {
      TicketSeverities types1 = new TicketSeverities(LoginUser);
      types1.LoadByTicketSeverityID(ticketSeverityID);
      if (types1.IsEmpty || types1[0].Position >= GetMaxPosition(types1[0].OrganizationID)) return;

      TicketSeverities types2 = new TicketSeverities(LoginUser);
      types2.LoadByPosition(types1[0].OrganizationID, types1[0].Position + 1);
      if (!types2.IsEmpty)
      {
        types2[0].Position = types2[0].Position - 1;
        types2.Save();
      }

      types1[0].Position = types1[0].Position + 1;
      types1.Save();
	  
      ValidatePositions(LoginUser.OrganizationID);
    }


    public virtual int GetMaxPosition(int organizationID)
    {
      int position = -1;
      
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT MAX(Position) FROM TicketSeverities WHERE OrganizationID = @OrganizationID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        object o = ExecuteScalar(command);
        if (o == DBNull.Value) return -1;
        position = (int)o;
      }
      return position;
    }
    
    

    #endregion

    #region IEnumerable<TicketSeverity> Members

    public IEnumerator<TicketSeverity> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketSeverity(row, this);
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
