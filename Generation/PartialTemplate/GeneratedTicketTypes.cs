using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketType : BaseItem
  {
    private TicketTypes _ticketTypes;
    
    public TicketType(DataRow row, TicketTypes ticketTypes): base(row, ticketTypes)
    {
      _ticketTypes = ticketTypes;
    }
	
    #region Properties
    
    public TicketTypes Collection
    {
      get { return _ticketTypes; }
    }
        
    
    
    
    public int TicketTypeID
    {
      get { return (int)Row["TicketTypeID"]; }
    }
    

    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
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
    
    public bool IsVisibleOnPortal
    {
      get { return (bool)Row["IsVisibleOnPortal"]; }
      set { Row["IsVisibleOnPortal"] = CheckValue("IsVisibleOnPortal", value); }
    }
    
    public string IconUrl
    {
      get { return (string)Row["IconUrl"]; }
      set { Row["IconUrl"] = CheckValue("IconUrl", value); }
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

  public partial class TicketTypes : BaseCollection, IEnumerable<TicketType>
  {
    public TicketTypes(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketTypes"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketTypeID"; }
    }



    public TicketType this[int index]
    {
      get { return new TicketType(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketType ticketType);
    partial void AfterRowInsert(TicketType ticketType);
    partial void BeforeRowEdit(TicketType ticketType);
    partial void AfterRowEdit(TicketType ticketType);
    partial void BeforeRowDelete(int ticketTypeID);
    partial void AfterRowDelete(int ticketTypeID);    

    partial void BeforeDBDelete(int ticketTypeID);
    partial void AfterDBDelete(int ticketTypeID);    

    #endregion

    #region Public Methods

    public TicketTypeProxy[] GetTicketTypeProxies()
    {
      List<TicketTypeProxy> list = new List<TicketTypeProxy>();

      foreach (TicketType item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketTypeID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketTypes] WHERE ([TicketTypeID] = @TicketTypeID);";
        deleteCommand.Parameters.Add("TicketTypeID", SqlDbType.Int);
        deleteCommand.Parameters["TicketTypeID"].Value = ticketTypeID;

        BeforeDBDelete(ticketTypeID);
        BeforeRowDelete(ticketTypeID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(ticketTypeID);
        AfterDBDelete(ticketTypeID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketTypesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketTypes] SET     [Name] = @Name,    [Description] = @Description,    [Position] = @Position,    [OrganizationID] = @OrganizationID,    [IconUrl] = @IconUrl,    [IsVisibleOnPortal] = @IsVisibleOnPortal,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [ProductFamilyID] = @ProductFamilyID,    [IsActive] = @IsActive  WHERE ([TicketTypeID] = @TicketTypeID);";

		
		tempParameter = updateCommand.Parameters.Add("TicketTypeID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("IconUrl", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsVisibleOnPortal", SqlDbType.Bit, 1);
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
		
		tempParameter = updateCommand.Parameters.Add("ProductFamilyID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketTypes] (    [Name],    [Description],    [Position],    [OrganizationID],    [IconUrl],    [IsVisibleOnPortal],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [ProductFamilyID],    [IsActive]) VALUES ( @Name, @Description, @Position, @OrganizationID, @IconUrl, @IsVisibleOnPortal, @DateCreated, @DateModified, @CreatorID, @ModifierID, @ProductFamilyID, @IsActive); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("IsActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ProductFamilyID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
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
		
		tempParameter = insertCommand.Parameters.Add("IsVisibleOnPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IconUrl", SqlDbType.VarChar, 255);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketTypes] WHERE ([TicketTypeID] = @TicketTypeID);";
		deleteCommand.Parameters.Add("TicketTypeID", SqlDbType.Int);

		try
		{
		  foreach (TicketType ticketType in this)
		  {
			if (ticketType.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketType);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketType.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketTypeID"].AutoIncrement = false;
			  Table.Columns["TicketTypeID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketType.Row["TicketTypeID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketType);
			}
			else if (ticketType.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketType);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketType.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketType);
			}
			else if (ticketType.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketType.Row["TicketTypeID", DataRowVersion.Original];
			  deleteCommand.Parameters["TicketTypeID"].Value = id;
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

      foreach (TicketType ticketType in this)
      {
        if (ticketType.Row.Table.Columns.Contains("CreatorID") && (int)ticketType.Row["CreatorID"] == 0) ticketType.Row["CreatorID"] = LoginUser.UserID;
        if (ticketType.Row.Table.Columns.Contains("ModifierID")) ticketType.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketType FindByTicketTypeID(int ticketTypeID)
    {
      foreach (TicketType ticketType in this)
      {
        if (ticketType.TicketTypeID == ticketTypeID)
        {
          return ticketType;
        }
      }
      return null;
    }

    public virtual TicketType AddNewTicketType()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketTypes");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketType(row, this);
    }
    
    public virtual void LoadByTicketTypeID(int ticketTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TicketTypeID], [Name], [Description], [Position], [OrganizationID], [IconUrl], [IsVisibleOnPortal], [DateCreated], [DateModified], [CreatorID], [ModifierID], [ProductFamilyID], [IsActive] FROM [dbo].[TicketTypes] WHERE ([TicketTypeID] = @TicketTypeID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketTypeID", ticketTypeID);
        Fill(command);
      }
    }
    
    public static TicketType GetTicketType(LoginUser loginUser, int ticketTypeID)
    {
      TicketTypes ticketTypes = new TicketTypes(loginUser);
      ticketTypes.LoadByTicketTypeID(ticketTypeID);
      if (ticketTypes.IsEmpty)
        return null;
      else
        return ticketTypes[0];
    }
    
    
     

    public void LoadByPosition(int organizationID, int position)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM TicketTypes WHERE (OrganizationID = @OrganizationID) AND (Position = @Position)";
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
        command.CommandText = "SELECT * FROM TicketTypes WHERE (OrganizationID = @OrganizationID) ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void ValidatePositions(int organizationID)
    {
      TicketTypes ticketTypes = new TicketTypes(LoginUser);
      ticketTypes.LoadAllPositions(organizationID);
      int i = 0;
      foreach (TicketType ticketType in ticketTypes)
      {
        ticketType.Position = i;
        i++;
      }
      ticketTypes.Save();
    }    

    public void MovePositionUp(int ticketTypeID)
    {
      TicketTypes types1 = new TicketTypes(LoginUser);
      types1.LoadByTicketTypeID(ticketTypeID);
      if (types1.IsEmpty || types1[0].Position < 1) return;

      TicketTypes types2 = new TicketTypes(LoginUser);
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
    
    public void MovePositionDown(int ticketTypeID)
    {
      TicketTypes types1 = new TicketTypes(LoginUser);
      types1.LoadByTicketTypeID(ticketTypeID);
      if (types1.IsEmpty || types1[0].Position >= GetMaxPosition(types1[0].OrganizationID)) return;

      TicketTypes types2 = new TicketTypes(LoginUser);
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
        command.CommandText = "SELECT MAX(Position) FROM TicketTypes WHERE OrganizationID = @OrganizationID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        object o = ExecuteScalar(command);
        if (o == DBNull.Value) return -1;
        position = (int)o;
      }
      return position;
    }
    
    

    #endregion

    #region IEnumerable<TicketType> Members

    public IEnumerator<TicketType> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketType(row, this);
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
