using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class NotesViewItem : BaseItem
  {
    private NotesView _notesView;
    
    public NotesViewItem(DataRow row, NotesView notesView): base(row, notesView)
    {
      _notesView = notesView;
    }
	
    #region Properties
    
    public NotesView Collection
    {
      get { return _notesView; }
    }
        
    
    public string CreatorName
    {
      get { return Row["CreatorName"] != DBNull.Value ? (string)Row["CreatorName"] : null; }
    }
    
    public string ModifierName
    {
      get { return Row["ModifierName"] != DBNull.Value ? (string)Row["ModifierName"] : null; }
    }
    
    public string ContactName
    {
      get { return Row["ContactName"] != DBNull.Value ? (string)Row["ContactName"] : null; }
    }
    
    
    

    
    public int? ParentOrganizationID
    {
      get { return Row["ParentOrganizationID"] != DBNull.Value ? (int?)Row["ParentOrganizationID"] : null; }
      set { Row["ParentOrganizationID"] = CheckValue("ParentOrganizationID", value); }
    }
    
    public string OrganizationName
    {
      get { return Row["OrganizationName"] != DBNull.Value ? (string)Row["OrganizationName"] : null; }
      set { Row["OrganizationName"] = CheckValue("OrganizationName", value); }
    }
    

    
    public bool NeedsIndexing
    {
      get { return (bool)Row["NeedsIndexing"]; }
      set { Row["NeedsIndexing"] = CheckValue("NeedsIndexing", value); }
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
    
    public string Description
    {
      get { return (string)Row["Description"]; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public string Title
    {
      get { return (string)Row["Title"]; }
      set { Row["Title"] = CheckValue("Title", value); }
    }
    
    public int RefID
    {
      get { return (int)Row["RefID"]; }
      set { Row["RefID"] = CheckValue("RefID", value); }
    }
    
    public int RefType
    {
      get { return (int)Row["RefType"]; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    
    public int NoteID
    {
      get { return (int)Row["NoteID"]; }
      set { Row["NoteID"] = CheckValue("NoteID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateCreated
    {
      get { return DateToLocal((DateTime)Row["DateCreated"]); }
      set { Row["DateCreated"] = CheckValue("DateCreated", value); }
    }

    public DateTime DateCreatedUtc
    {
      get { return (DateTime)Row["DateCreated"]; }
    }
    
    public DateTime DateModified
    {
      get { return DateToLocal((DateTime)Row["DateModified"]); }
      set { Row["DateModified"] = CheckValue("DateModified", value); }
    }

    public DateTime DateModifiedUtc
    {
      get { return (DateTime)Row["DateModified"]; }
    }
    

    #endregion
    
    
  }

  public partial class NotesView : BaseCollection, IEnumerable<NotesViewItem>
  {
    public NotesView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "NotesView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "NoteID"; }
    }



    public NotesViewItem this[int index]
    {
      get { return new NotesViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(NotesViewItem notesViewItem);
    partial void AfterRowInsert(NotesViewItem notesViewItem);
    partial void BeforeRowEdit(NotesViewItem notesViewItem);
    partial void AfterRowEdit(NotesViewItem notesViewItem);
    partial void BeforeRowDelete(int noteID);
    partial void AfterRowDelete(int noteID);    

    partial void BeforeDBDelete(int noteID);
    partial void AfterDBDelete(int noteID);    

    #endregion

    #region Public Methods

    public NotesViewItemProxy[] GetNotesViewItemProxies()
    {
      List<NotesViewItemProxy> list = new List<NotesViewItemProxy>();

      foreach (NotesViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int noteID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[NotesView] WHERE ([NoteID] = @NoteID);";
        deleteCommand.Parameters.Add("NoteID", SqlDbType.Int);
        deleteCommand.Parameters["NoteID"].Value = noteID;

        BeforeDBDelete(noteID);
        BeforeRowDelete(noteID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(noteID);
        AfterDBDelete(noteID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("NotesViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[NotesView] SET     [RefType] = @RefType,    [RefID] = @RefID,    [Title] = @Title,    [Description] = @Description,    [ModifierID] = @ModifierID,    [DateModified] = @DateModified,    [NeedsIndexing] = @NeedsIndexing,    [CreatorName] = @CreatorName,    [ModifierName] = @ModifierName,    [ParentOrganizationID] = @ParentOrganizationID,    [OrganizationName] = @OrganizationName,    [ContactName] = @ContactName  WHERE ([NoteID] = @NoteID);";

		
		tempParameter = updateCommand.Parameters.Add("NoteID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Title", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("NeedsIndexing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CreatorName", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierName", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ParentOrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationName", SqlDbType.NVarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ContactName", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[NotesView] (    [NoteID],    [RefType],    [RefID],    [Title],    [Description],    [CreatorID],    [ModifierID],    [DateModified],    [DateCreated],    [NeedsIndexing],    [CreatorName],    [ModifierName],    [ParentOrganizationID],    [OrganizationName],    [ContactName]) VALUES ( @NoteID, @RefType, @RefID, @Title, @Description, @CreatorID, @ModifierID, @DateModified, @DateCreated, @NeedsIndexing, @CreatorName, @ModifierName, @ParentOrganizationID, @OrganizationName, @ContactName); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ContactName", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationName", SqlDbType.NVarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ParentOrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ModifierName", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorName", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("NeedsIndexing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
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
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Title", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("NoteID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[NotesView] WHERE ([NoteID] = @NoteID);";
		deleteCommand.Parameters.Add("NoteID", SqlDbType.Int);

		try
		{
		  foreach (NotesViewItem notesViewItem in this)
		  {
			if (notesViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(notesViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = notesViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["NoteID"].AutoIncrement = false;
			  Table.Columns["NoteID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				notesViewItem.Row["NoteID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(notesViewItem);
			}
			else if (notesViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(notesViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = notesViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(notesViewItem);
			}
			else if (notesViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)notesViewItem.Row["NoteID", DataRowVersion.Original];
			  deleteCommand.Parameters["NoteID"].Value = id;
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

      foreach (NotesViewItem notesViewItem in this)
      {
        if (notesViewItem.Row.Table.Columns.Contains("CreatorID") && (int)notesViewItem.Row["CreatorID"] == 0) notesViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (notesViewItem.Row.Table.Columns.Contains("ModifierID")) notesViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public NotesViewItem FindByNoteID(int noteID)
    {
      foreach (NotesViewItem notesViewItem in this)
      {
        if (notesViewItem.NoteID == noteID)
        {
          return notesViewItem;
        }
      }
      return null;
    }

    public virtual NotesViewItem AddNewNotesViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("NotesView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new NotesViewItem(row, this);
    }
    
    public virtual void LoadByNoteID(int noteID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [NoteID], [RefType], [RefID], [Title], [Description], [CreatorID], [ModifierID], [DateModified], [DateCreated], [NeedsIndexing], [CreatorName], [ModifierName], [ParentOrganizationID], [OrganizationName], [ContactName] FROM [dbo].[NotesView] WHERE ([NoteID] = @NoteID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("NoteID", noteID);
        Fill(command);
      }
    }
    
    public static NotesViewItem GetNotesViewItem(LoginUser loginUser, int noteID)
    {
      NotesView notesView = new NotesView(loginUser);
      notesView.LoadByNoteID(noteID);
      if (notesView.IsEmpty)
        return null;
      else
        return notesView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<NotesViewItem> Members

    public IEnumerator<NotesViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new NotesViewItem(row, this);
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
