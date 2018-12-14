using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ActionType : BaseItem
  {
    private ActionTypes _actionTypes;
    
    public ActionType(DataRow row, ActionTypes actionTypes): base(row, actionTypes)
    {
      _actionTypes = actionTypes;
    }
	
    #region Properties
    
    public ActionTypes Collection
    {
      get { return _actionTypes; }
    }
        
    
    
    
    public int ActionTypeID
    {
      get { return (int)Row["ActionTypeID"]; }
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
    
    public int Position
    {
      get { return (int)Row["Position"]; }
      set { Row["Position"] = CheckValue("Position", value); }
    }
    
    public bool IsTimed
    {
      get { return (bool)Row["IsTimed"]; }
      set { Row["IsTimed"] = CheckValue("IsTimed", value); }
    }
    
    public string Description
    {
      get { return (string)Row["Description"]; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckValue("Name", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
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

  public partial class ActionTypes : BaseCollection, IEnumerable<ActionType>
  {
    public ActionTypes(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ActionTypes"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ActionTypeID"; }
    }



    public ActionType this[int index]
    {
      get { return new ActionType(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ActionType actionType);
    partial void AfterRowInsert(ActionType actionType);
    partial void BeforeRowEdit(ActionType actionType);
    partial void AfterRowEdit(ActionType actionType);
    partial void BeforeRowDelete(int actionTypeID);
    partial void AfterRowDelete(int actionTypeID);    

    partial void BeforeDBDelete(int actionTypeID);
    partial void AfterDBDelete(int actionTypeID);    

    #endregion

    #region Public Methods

    public ActionTypeProxy[] GetActionTypeProxies()
    {
      List<ActionTypeProxy> list = new List<ActionTypeProxy>();

      foreach (ActionType item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int actionTypeID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ActionTypes] WHERE ([ActionTypeID] = @ActionTypeID);";
        deleteCommand.Parameters.Add("ActionTypeID", SqlDbType.Int);
        deleteCommand.Parameters["ActionTypeID"].Value = actionTypeID;

        BeforeDBDelete(actionTypeID);
        BeforeRowDelete(actionTypeID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(actionTypeID);
        AfterDBDelete(actionTypeID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ActionTypesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ActionTypes] SET     [OrganizationID] = @OrganizationID,    [Name] = @Name,    [Description] = @Description,    [IsTimed] = @IsTimed,    [Position] = @Position,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID  WHERE ([ActionTypeID] = @ActionTypeID);";

		
		tempParameter = updateCommand.Parameters.Add("ActionTypeID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("IsTimed", SqlDbType.Bit, 1);
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ActionTypes] (    [OrganizationID],    [Name],    [Description],    [IsTimed],    [Position],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID]) VALUES ( @OrganizationID, @Name, @Description, @IsTimed, @Position, @DateCreated, @DateModified, @CreatorID, @ModifierID); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsTimed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ActionTypes] WHERE ([ActionTypeID] = @ActionTypeID);";
		deleteCommand.Parameters.Add("ActionTypeID", SqlDbType.Int);

		try
		{
		  foreach (ActionType actionType in this)
		  {
			if (actionType.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(actionType);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = actionType.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ActionTypeID"].AutoIncrement = false;
			  Table.Columns["ActionTypeID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				actionType.Row["ActionTypeID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(actionType);
			}
			else if (actionType.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(actionType);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = actionType.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(actionType);
			}
			else if (actionType.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)actionType.Row["ActionTypeID", DataRowVersion.Original];
			  deleteCommand.Parameters["ActionTypeID"].Value = id;
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

      foreach (ActionType actionType in this)
      {
        if (actionType.Row.Table.Columns.Contains("CreatorID") && (int)actionType.Row["CreatorID"] == 0) actionType.Row["CreatorID"] = LoginUser.UserID;
        if (actionType.Row.Table.Columns.Contains("ModifierID")) actionType.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ActionType FindByActionTypeID(int actionTypeID)
    {
      foreach (ActionType actionType in this)
      {
        if (actionType.ActionTypeID == actionTypeID)
        {
          return actionType;
        }
      }
      return null;
    }

    public virtual ActionType AddNewActionType()
    {
      if (Table.Columns.Count < 1) LoadColumns("ActionTypes");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ActionType(row, this);
    }
    
    public virtual void LoadByActionTypeID(int actionTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ActionTypeID], [OrganizationID], [Name], [Description], [IsTimed], [Position], [DateCreated], [DateModified], [CreatorID], [ModifierID] FROM [dbo].[ActionTypes] WHERE ([ActionTypeID] = @ActionTypeID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ActionTypeID", actionTypeID);
        Fill(command);
      }
    }
    
    public static ActionType GetActionType(LoginUser loginUser, int actionTypeID)
    {
      ActionTypes actionTypes = new ActionTypes(loginUser);
      actionTypes.LoadByActionTypeID(actionTypeID);
      if (actionTypes.IsEmpty)
        return null;
      else
        return actionTypes[0];
    }
    
    
     

    public void LoadByPosition(int organizationID, int position)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ActionTypes WHERE (OrganizationID = @OrganizationID) AND (Position = @Position)";
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
        command.CommandText = "SELECT * FROM ActionTypes WHERE (OrganizationID = @OrganizationID) ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void ValidatePositions(int organizationID)
    {
      ActionTypes actionTypes = new ActionTypes(LoginUser);
      actionTypes.LoadAllPositions(organizationID);
      int i = 0;
      foreach (ActionType actionType in actionTypes)
      {
        actionType.Position = i;
        i++;
      }
      actionTypes.Save();
    }    

    public void MovePositionUp(int actionTypeID)
    {
      ActionTypes types1 = new ActionTypes(LoginUser);
      types1.LoadByActionTypeID(actionTypeID);
      if (types1.IsEmpty || types1[0].Position < 1) return;

      ActionTypes types2 = new ActionTypes(LoginUser);
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
    
    public void MovePositionDown(int actionTypeID)
    {
      ActionTypes types1 = new ActionTypes(LoginUser);
      types1.LoadByActionTypeID(actionTypeID);
      if (types1.IsEmpty || types1[0].Position >= GetMaxPosition(types1[0].OrganizationID)) return;

      ActionTypes types2 = new ActionTypes(LoginUser);
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
        command.CommandText = "SELECT MAX(Position) FROM ActionTypes WHERE OrganizationID = @OrganizationID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        object o = ExecuteScalar(command);
        if (o == DBNull.Value) return -1;
        position = (int)o;
      }
      return position;
    }
    
    

    #endregion

    #region IEnumerable<ActionType> Members

    public IEnumerator<ActionType> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ActionType(row, this);
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
