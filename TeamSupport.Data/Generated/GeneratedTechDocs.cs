using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TechDoc : BaseItem
  {
    private TechDocs _techDocs;
    
    public TechDoc(DataRow row, TechDocs techDocs): base(row, techDocs)
    {
      _techDocs = techDocs;
    }
	
    #region Properties
    
    public TechDocs Collection
    {
      get { return _techDocs; }
    }
        
    
    
    
    public int TechDocID
    {
      get { return (int)Row["TechDocID"]; }
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
    
    public int AttachmentID
    {
      get { return (int)Row["AttachmentID"]; }
      set { Row["AttachmentID"] = CheckValue("AttachmentID", value); }
    }
    
    public int ProductID
    {
      get { return (int)Row["ProductID"]; }
      set { Row["ProductID"] = CheckValue("ProductID", value); }
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

  public partial class TechDocs : BaseCollection, IEnumerable<TechDoc>
  {
    public TechDocs(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TechDocs"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TechDocID"; }
    }



    public TechDoc this[int index]
    {
      get { return new TechDoc(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TechDoc techDoc);
    partial void AfterRowInsert(TechDoc techDoc);
    partial void BeforeRowEdit(TechDoc techDoc);
    partial void AfterRowEdit(TechDoc techDoc);
    partial void BeforeRowDelete(int techDocID);
    partial void AfterRowDelete(int techDocID);    

    partial void BeforeDBDelete(int techDocID);
    partial void AfterDBDelete(int techDocID);    

    #endregion

    #region Public Methods

    public TechDocProxy[] GetTechDocProxies()
    {
      List<TechDocProxy> list = new List<TechDocProxy>();

      foreach (TechDoc item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int techDocID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TechDocs] WHERE ([TechDocID] = @TechDocID);";
        deleteCommand.Parameters.Add("TechDocID", SqlDbType.Int);
        deleteCommand.Parameters["TechDocID"].Value = techDocID;

        BeforeDBDelete(techDocID);
        BeforeRowDelete(techDocID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(techDocID);
        AfterDBDelete(techDocID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TechDocsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TechDocs] SET     [OrganizationID] = @OrganizationID,    [ProductID] = @ProductID,    [AttachmentID] = @AttachmentID,    [IsVisibleOnPortal] = @IsVisibleOnPortal,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID  WHERE ([TechDocID] = @TechDocID);";

		
		tempParameter = updateCommand.Parameters.Add("TechDocID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("AttachmentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TechDocs] (    [OrganizationID],    [ProductID],    [AttachmentID],    [IsVisibleOnPortal],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID]) VALUES ( @OrganizationID, @ProductID, @AttachmentID, @IsVisibleOnPortal, @DateCreated, @DateModified, @CreatorID, @ModifierID); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("AttachmentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ProductID", SqlDbType.Int, 4);
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
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TechDocs] WHERE ([TechDocID] = @TechDocID);";
		deleteCommand.Parameters.Add("TechDocID", SqlDbType.Int);

		try
		{
		  foreach (TechDoc techDoc in this)
		  {
			if (techDoc.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(techDoc);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = techDoc.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TechDocID"].AutoIncrement = false;
			  Table.Columns["TechDocID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				techDoc.Row["TechDocID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(techDoc);
			}
			else if (techDoc.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(techDoc);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = techDoc.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(techDoc);
			}
			else if (techDoc.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)techDoc.Row["TechDocID", DataRowVersion.Original];
			  deleteCommand.Parameters["TechDocID"].Value = id;
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

      foreach (TechDoc techDoc in this)
      {
        if (techDoc.Row.Table.Columns.Contains("CreatorID") && (int)techDoc.Row["CreatorID"] == 0) techDoc.Row["CreatorID"] = LoginUser.UserID;
        if (techDoc.Row.Table.Columns.Contains("ModifierID")) techDoc.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TechDoc FindByTechDocID(int techDocID)
    {
      foreach (TechDoc techDoc in this)
      {
        if (techDoc.TechDocID == techDocID)
        {
          return techDoc;
        }
      }
      return null;
    }

    public virtual TechDoc AddNewTechDoc()
    {
      if (Table.Columns.Count < 1) LoadColumns("TechDocs");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TechDoc(row, this);
    }
    
    public virtual void LoadByTechDocID(int techDocID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TechDocID], [OrganizationID], [ProductID], [AttachmentID], [IsVisibleOnPortal], [DateCreated], [DateModified], [CreatorID], [ModifierID] FROM [dbo].[TechDocs] WHERE ([TechDocID] = @TechDocID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TechDocID", techDocID);
        Fill(command);
      }
    }
    
    public static TechDoc GetTechDoc(LoginUser loginUser, int techDocID)
    {
      TechDocs techDocs = new TechDocs(loginUser);
      techDocs.LoadByTechDocID(techDocID);
      if (techDocs.IsEmpty)
        return null;
      else
        return techDocs[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TechDoc> Members

    public IEnumerator<TechDoc> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TechDoc(row, this);
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
