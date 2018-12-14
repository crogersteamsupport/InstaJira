using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class SlaLevel : BaseItem
  {
    private SlaLevels _slaLevels;
    
    public SlaLevel(DataRow row, SlaLevels slaLevels): base(row, slaLevels)
    {
      _slaLevels = slaLevels;
    }
	
    #region Properties
    
    public SlaLevels Collection
    {
      get { return _slaLevels; }
    }
        
    
    
    
    public int SlaLevelID
    {
      get { return (int)Row["SlaLevelID"]; }
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
    
    
    

    

    

    #endregion
    
    
  }

  public partial class SlaLevels : BaseCollection, IEnumerable<SlaLevel>
  {
    public SlaLevels(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "SlaLevels"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "SlaLevelID"; }
    }



    public SlaLevel this[int index]
    {
      get { return new SlaLevel(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(SlaLevel slaLevel);
    partial void AfterRowInsert(SlaLevel slaLevel);
    partial void BeforeRowEdit(SlaLevel slaLevel);
    partial void AfterRowEdit(SlaLevel slaLevel);
    partial void BeforeRowDelete(int slaLevelID);
    partial void AfterRowDelete(int slaLevelID);    

    partial void BeforeDBDelete(int slaLevelID);
    partial void AfterDBDelete(int slaLevelID);    

    #endregion

    #region Public Methods

    public SlaLevelProxy[] GetSlaLevelProxies()
    {
      List<SlaLevelProxy> list = new List<SlaLevelProxy>();

      foreach (SlaLevel item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int slaLevelID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SlaLevels] WHERE ([SlaLevelID] = @SlaLevelID);";
        deleteCommand.Parameters.Add("SlaLevelID", SqlDbType.Int);
        deleteCommand.Parameters["SlaLevelID"].Value = slaLevelID;

        BeforeDBDelete(slaLevelID);
        BeforeRowDelete(slaLevelID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(slaLevelID);
        AfterDBDelete(slaLevelID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("SlaLevelsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[SlaLevels] SET     [OrganizationID] = @OrganizationID,    [Name] = @Name  WHERE ([SlaLevelID] = @SlaLevelID);";

		
		tempParameter = updateCommand.Parameters.Add("SlaLevelID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[SlaLevels] (    [OrganizationID],    [Name]) VALUES ( @OrganizationID, @Name); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 150);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SlaLevels] WHERE ([SlaLevelID] = @SlaLevelID);";
		deleteCommand.Parameters.Add("SlaLevelID", SqlDbType.Int);

		try
		{
		  foreach (SlaLevel slaLevel in this)
		  {
			if (slaLevel.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(slaLevel);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = slaLevel.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["SlaLevelID"].AutoIncrement = false;
			  Table.Columns["SlaLevelID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				slaLevel.Row["SlaLevelID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(slaLevel);
			}
			else if (slaLevel.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(slaLevel);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = slaLevel.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(slaLevel);
			}
			else if (slaLevel.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)slaLevel.Row["SlaLevelID", DataRowVersion.Original];
			  deleteCommand.Parameters["SlaLevelID"].Value = id;
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

      foreach (SlaLevel slaLevel in this)
      {
        if (slaLevel.Row.Table.Columns.Contains("CreatorID") && (int)slaLevel.Row["CreatorID"] == 0) slaLevel.Row["CreatorID"] = LoginUser.UserID;
        if (slaLevel.Row.Table.Columns.Contains("ModifierID")) slaLevel.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public SlaLevel FindBySlaLevelID(int slaLevelID)
    {
      foreach (SlaLevel slaLevel in this)
      {
        if (slaLevel.SlaLevelID == slaLevelID)
        {
          return slaLevel;
        }
      }
      return null;
    }

    public virtual SlaLevel AddNewSlaLevel()
    {
      if (Table.Columns.Count < 1) LoadColumns("SlaLevels");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new SlaLevel(row, this);
    }
    
    public virtual void LoadBySlaLevelID(int slaLevelID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [SlaLevelID], [OrganizationID], [Name] FROM [dbo].[SlaLevels] WHERE ([SlaLevelID] = @SlaLevelID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("SlaLevelID", slaLevelID);
        Fill(command);
      }
    }
    
    public static SlaLevel GetSlaLevel(LoginUser loginUser, int slaLevelID)
    {
      SlaLevels slaLevels = new SlaLevels(loginUser);
      slaLevels.LoadBySlaLevelID(slaLevelID);
      if (slaLevels.IsEmpty)
        return null;
      else
        return slaLevels[0];
    }
    
    
    

    #endregion

    #region IEnumerable<SlaLevel> Members

    public IEnumerator<SlaLevel> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new SlaLevel(row, this);
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
