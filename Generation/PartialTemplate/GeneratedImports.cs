using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Import : BaseItem
  {
    private Imports _imports;
    
    public Import(DataRow row, Imports imports): base(row, imports)
    {
      _imports = imports;
    }
	
    #region Properties
    
    public Imports Collection
    {
      get { return _imports; }
    }
        
    
    
    
    public int ImportID
    {
      get { return (int)Row["ImportID"]; }
    }
    

    
    public int? AuxID
    {
      get { return Row["AuxID"] != DBNull.Value ? (int?)Row["AuxID"] : null; }
      set { Row["AuxID"] = CheckValue("AuxID", value); }
    }
    

    
    public bool IsRolledBack
    {
      get { return (bool)Row["IsRolledBack"]; }
      set { Row["IsRolledBack"] = CheckValue("IsRolledBack", value); }
    }
    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public int CompletedRows
    {
      get { return (int)Row["CompletedRows"]; }
      set { Row["CompletedRows"] = CheckValue("CompletedRows", value); }
    }
    
    public int TotalRows
    {
      get { return (int)Row["TotalRows"]; }
      set { Row["TotalRows"] = CheckValue("TotalRows", value); }
    }
    
    public bool NeedsDeleted
    {
      get { return (bool)Row["NeedsDeleted"]; }
      set { Row["NeedsDeleted"] = CheckValue("NeedsDeleted", value); }
    }
    
    public bool IsDeleted
    {
      get { return (bool)Row["IsDeleted"]; }
      set { Row["IsDeleted"] = CheckValue("IsDeleted", value); }
    }
    
    public bool IsRunning
    {
      get { return (bool)Row["IsRunning"]; }
      set { Row["IsRunning"] = CheckValue("IsRunning", value); }
    }
    
    public bool IsDone
    {
      get { return (bool)Row["IsDone"]; }
      set { Row["IsDone"] = CheckValue("IsDone", value); }
    }
    
    public ReferenceType RefType
    {
      get { return (ReferenceType)Row["RefType"]; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    
    public Guid ImportGUID
    {
      get { return (Guid)Row["ImportGUID"]; }
      set { Row["ImportGUID"] = CheckValue("ImportGUID", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public string FileName
    {
      get { return (string)Row["FileName"]; }
      set { Row["FileName"] = CheckValue("FileName", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? DateStarted
    {
      get { return Row["DateStarted"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateStarted"]) : null; }
      set { Row["DateStarted"] = CheckValue("DateStarted", value); }
    }

    public DateTime? DateStartedUtc
    {
      get { return Row["DateStarted"] != DBNull.Value ? (DateTime?)Row["DateStarted"] : null; }
    }
    
    public DateTime? DateEnded
    {
      get { return Row["DateEnded"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateEnded"]) : null; }
      set { Row["DateEnded"] = CheckValue("DateEnded", value); }
    }

    public DateTime? DateEndedUtc
    {
      get { return Row["DateEnded"] != DBNull.Value ? (DateTime?)Row["DateEnded"] : null; }
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

  public partial class Imports : BaseCollection, IEnumerable<Import>
  {
    public Imports(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Imports"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ImportID"; }
    }



    public Import this[int index]
    {
      get { return new Import(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Import import);
    partial void AfterRowInsert(Import import);
    partial void BeforeRowEdit(Import import);
    partial void AfterRowEdit(Import import);
    partial void BeforeRowDelete(int importID);
    partial void AfterRowDelete(int importID);    

    partial void BeforeDBDelete(int importID);
    partial void AfterDBDelete(int importID);    

    #endregion

    #region Public Methods

    public ImportProxy[] GetImportProxies()
    {
      List<ImportProxy> list = new List<ImportProxy>();

      foreach (Import item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int importID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Imports] WHERE ([ImportID] = @ImportID);";
        deleteCommand.Parameters.Add("ImportID", SqlDbType.Int);
        deleteCommand.Parameters["ImportID"].Value = importID;

        BeforeDBDelete(importID);
        BeforeRowDelete(importID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(importID);
        AfterDBDelete(importID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ImportsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Imports] SET     [FileName] = @FileName,    [OrganizationID] = @OrganizationID,    [ImportGUID] = @ImportGUID,    [RefType] = @RefType,    [AuxID] = @AuxID,    [IsDone] = @IsDone,    [IsRunning] = @IsRunning,    [IsDeleted] = @IsDeleted,    [NeedsDeleted] = @NeedsDeleted,    [TotalRows] = @TotalRows,    [CompletedRows] = @CompletedRows,    [DateStarted] = @DateStarted,    [DateEnded] = @DateEnded,    [IsRolledBack] = @IsRolledBack  WHERE ([ImportID] = @ImportID);";

		
		tempParameter = updateCommand.Parameters.Add("ImportID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("FileName", SqlDbType.VarChar, 255);
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
		
		tempParameter = updateCommand.Parameters.Add("ImportGUID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("AuxID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsDone", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsRunning", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsDeleted", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("NeedsDeleted", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TotalRows", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CompletedRows", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateStarted", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateEnded", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsRolledBack", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Imports] (    [FileName],    [OrganizationID],    [ImportGUID],    [RefType],    [AuxID],    [IsDone],    [IsRunning],    [IsDeleted],    [NeedsDeleted],    [TotalRows],    [CompletedRows],    [DateStarted],    [DateEnded],    [DateCreated],    [CreatorID],    [IsRolledBack]) VALUES ( @FileName, @OrganizationID, @ImportGUID, @RefType, @AuxID, @IsDone, @IsRunning, @IsDeleted, @NeedsDeleted, @TotalRows, @CompletedRows, @DateStarted, @DateEnded, @DateCreated, @CreatorID, @IsRolledBack); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("IsRolledBack", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateEnded", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateStarted", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("CompletedRows", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TotalRows", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("NeedsDeleted", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsDeleted", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsRunning", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsDone", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("AuxID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ImportGUID", SqlDbType.UniqueIdentifier, 16);
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
		
		tempParameter = insertCommand.Parameters.Add("FileName", SqlDbType.VarChar, 255);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Imports] WHERE ([ImportID] = @ImportID);";
		deleteCommand.Parameters.Add("ImportID", SqlDbType.Int);

		try
		{
		  foreach (Import import in this)
		  {
			if (import.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(import);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = import.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ImportID"].AutoIncrement = false;
			  Table.Columns["ImportID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				import.Row["ImportID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(import);
			}
			else if (import.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(import);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = import.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(import);
			}
			else if (import.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)import.Row["ImportID", DataRowVersion.Original];
			  deleteCommand.Parameters["ImportID"].Value = id;
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

      foreach (Import import in this)
      {
        if (import.Row.Table.Columns.Contains("CreatorID") && (int)import.Row["CreatorID"] == 0) import.Row["CreatorID"] = LoginUser.UserID;
        if (import.Row.Table.Columns.Contains("ModifierID")) import.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Import FindByImportID(int importID)
    {
      foreach (Import import in this)
      {
        if (import.ImportID == importID)
        {
          return import;
        }
      }
      return null;
    }

    public virtual Import AddNewImport()
    {
      if (Table.Columns.Count < 1) LoadColumns("Imports");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Import(row, this);
    }
    
    public virtual void LoadByImportID(int importID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ImportID], [FileName], [OrganizationID], [ImportGUID], [RefType], [AuxID], [IsDone], [IsRunning], [IsDeleted], [NeedsDeleted], [TotalRows], [CompletedRows], [DateStarted], [DateEnded], [DateCreated], [CreatorID], [IsRolledBack] FROM [dbo].[Imports] WHERE ([ImportID] = @ImportID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ImportID", importID);
        Fill(command);
      }
    }
    
    public static Import GetImport(LoginUser loginUser, int importID)
    {
      Imports imports = new Imports(loginUser);
      imports.LoadByImportID(importID);
      if (imports.IsEmpty)
        return null;
      else
        return imports[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Import> Members

    public IEnumerator<Import> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Import(row, this);
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
