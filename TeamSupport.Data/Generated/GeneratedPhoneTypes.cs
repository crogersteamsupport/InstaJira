using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class PhoneType : BaseItem
  {
    private PhoneTypes _phoneTypes;
    
    public PhoneType(DataRow row, PhoneTypes phoneTypes): base(row, phoneTypes)
    {
      _phoneTypes = phoneTypes;
    }
	
    #region Properties
    
    public PhoneTypes Collection
    {
      get { return _phoneTypes; }
    }
        
    
    
    
    public int PhoneTypeID
    {
      get { return (int)Row["PhoneTypeID"]; }
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

  public partial class PhoneTypes : BaseCollection, IEnumerable<PhoneType>
  {
    public PhoneTypes(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "PhoneTypes"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "PhoneTypeID"; }
    }



    public PhoneType this[int index]
    {
      get { return new PhoneType(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(PhoneType phoneType);
    partial void AfterRowInsert(PhoneType phoneType);
    partial void BeforeRowEdit(PhoneType phoneType);
    partial void AfterRowEdit(PhoneType phoneType);
    partial void BeforeRowDelete(int phoneTypeID);
    partial void AfterRowDelete(int phoneTypeID);    

    partial void BeforeDBDelete(int phoneTypeID);
    partial void AfterDBDelete(int phoneTypeID);    

    #endregion

    #region Public Methods

    public PhoneTypeProxy[] GetPhoneTypeProxies()
    {
      List<PhoneTypeProxy> list = new List<PhoneTypeProxy>();

      foreach (PhoneType item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int phoneTypeID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[PhoneTypes] WHERE ([PhoneTypeID] = @PhoneTypeID);";
        deleteCommand.Parameters.Add("PhoneTypeID", SqlDbType.Int);
        deleteCommand.Parameters["PhoneTypeID"].Value = phoneTypeID;

        BeforeDBDelete(phoneTypeID);
        BeforeRowDelete(phoneTypeID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(phoneTypeID);
        AfterDBDelete(phoneTypeID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("PhoneTypesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[PhoneTypes] SET     [Name] = @Name,    [Description] = @Description,    [Position] = @Position,    [OrganizationID] = @OrganizationID,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID  WHERE ([PhoneTypeID] = @PhoneTypeID);";

		
		tempParameter = updateCommand.Parameters.Add("PhoneTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.VarChar, 50);
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[PhoneTypes] (    [Name],    [Description],    [Position],    [OrganizationID],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID]) VALUES ( @Name, @Description, @Position, @OrganizationID, @DateCreated, @DateModified, @CreatorID, @ModifierID); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 50);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[PhoneTypes] WHERE ([PhoneTypeID] = @PhoneTypeID);";
		deleteCommand.Parameters.Add("PhoneTypeID", SqlDbType.Int);

		try
		{
		  foreach (PhoneType phoneType in this)
		  {
			if (phoneType.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(phoneType);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = phoneType.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["PhoneTypeID"].AutoIncrement = false;
			  Table.Columns["PhoneTypeID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				phoneType.Row["PhoneTypeID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(phoneType);
			}
			else if (phoneType.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(phoneType);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = phoneType.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(phoneType);
			}
			else if (phoneType.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)phoneType.Row["PhoneTypeID", DataRowVersion.Original];
			  deleteCommand.Parameters["PhoneTypeID"].Value = id;
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

      foreach (PhoneType phoneType in this)
      {
        if (phoneType.Row.Table.Columns.Contains("CreatorID") && (int)phoneType.Row["CreatorID"] == 0) phoneType.Row["CreatorID"] = LoginUser.UserID;
        if (phoneType.Row.Table.Columns.Contains("ModifierID")) phoneType.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public PhoneType FindByPhoneTypeID(int phoneTypeID)
    {
      foreach (PhoneType phoneType in this)
      {
        if (phoneType.PhoneTypeID == phoneTypeID)
        {
          return phoneType;
        }
      }
      return null;
    }

    public virtual PhoneType AddNewPhoneType()
    {
      if (Table.Columns.Count < 1) LoadColumns("PhoneTypes");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new PhoneType(row, this);
    }
    
    public virtual void LoadByPhoneTypeID(int phoneTypeID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [PhoneTypeID], [Name], [Description], [Position], [OrganizationID], [DateCreated], [DateModified], [CreatorID], [ModifierID] FROM [dbo].[PhoneTypes] WHERE ([PhoneTypeID] = @PhoneTypeID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("PhoneTypeID", phoneTypeID);
        Fill(command);
      }
    }
    
    public static PhoneType GetPhoneType(LoginUser loginUser, int phoneTypeID)
    {
      PhoneTypes phoneTypes = new PhoneTypes(loginUser);
      phoneTypes.LoadByPhoneTypeID(phoneTypeID);
      if (phoneTypes.IsEmpty)
        return null;
      else
        return phoneTypes[0];
    }
    
    
     

    public void LoadByPosition(int organizationID, int position)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM PhoneTypes WHERE (OrganizationID = @OrganizationID) AND (Position = @Position)";
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
        command.CommandText = "SELECT * FROM PhoneTypes WHERE (OrganizationID = @OrganizationID) ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void ValidatePositions(int organizationID)
    {
      PhoneTypes phoneTypes = new PhoneTypes(LoginUser);
      phoneTypes.LoadAllPositions(organizationID);
      int i = 0;
      foreach (PhoneType phoneType in phoneTypes)
      {
        phoneType.Position = i;
        i++;
      }
      phoneTypes.Save();
    }    

    public void MovePositionUp(int phoneTypeID)
    {
      PhoneTypes types1 = new PhoneTypes(LoginUser);
      types1.LoadByPhoneTypeID(phoneTypeID);
      if (types1.IsEmpty || types1[0].Position < 1) return;

      PhoneTypes types2 = new PhoneTypes(LoginUser);
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
    
    public void MovePositionDown(int phoneTypeID)
    {
      PhoneTypes types1 = new PhoneTypes(LoginUser);
      types1.LoadByPhoneTypeID(phoneTypeID);
      if (types1.IsEmpty || types1[0].Position >= GetMaxPosition(types1[0].OrganizationID)) return;

      PhoneTypes types2 = new PhoneTypes(LoginUser);
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
        command.CommandText = "SELECT MAX(Position) FROM PhoneTypes WHERE OrganizationID = @OrganizationID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        object o = ExecuteScalar(command);
        if (o == DBNull.Value) return -1;
        position = (int)o;
      }
      return position;
    }
    
    

    #endregion

    #region IEnumerable<PhoneType> Members

    public IEnumerator<PhoneType> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new PhoneType(row, this);
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
