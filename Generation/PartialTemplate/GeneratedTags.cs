using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Tag : BaseItem
  {
    private Tags _tags;
    
    public Tag(DataRow row, Tags tags): base(row, tags)
    {
      _tags = tags;
    }
	
    #region Properties
    
    public Tags Collection
    {
      get { return _tags; }
    }
        
    
    
    
    public int TagID
    {
      get { return (int)Row["TagID"]; }
    }
    

    

    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public string Value
    {
      get { return (string)Row["Value"]; }
      set { Row["Value"] = CheckValue("Value", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
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
    

    #endregion
    
    
  }

  public partial class Tags : BaseCollection, IEnumerable<Tag>
  {
    public Tags(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Tags"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TagID"; }
    }



    public Tag this[int index]
    {
      get { return new Tag(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Tag tag);
    partial void AfterRowInsert(Tag tag);
    partial void BeforeRowEdit(Tag tag);
    partial void AfterRowEdit(Tag tag);
    partial void BeforeRowDelete(int tagID);
    partial void AfterRowDelete(int tagID);    

    partial void BeforeDBDelete(int tagID);
    partial void AfterDBDelete(int tagID);    

    #endregion

    #region Public Methods

    public TagProxy[] GetTagProxies()
    {
      List<TagProxy> list = new List<TagProxy>();

      foreach (Tag item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int tagID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Tags] WHERE ([TagID] = @TagID);";
        deleteCommand.Parameters.Add("TagID", SqlDbType.Int);
        deleteCommand.Parameters["TagID"].Value = tagID;

        BeforeDBDelete(tagID);
        BeforeRowDelete(tagID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(tagID);
        AfterDBDelete(tagID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TagsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Tags] SET     [OrganizationID] = @OrganizationID,    [Value] = @Value  WHERE ([TagID] = @TagID);";

		
		tempParameter = updateCommand.Parameters.Add("TagID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Value", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Tags] (    [OrganizationID],    [Value],    [DateCreated],    [CreatorID]) VALUES ( @OrganizationID, @Value, @DateCreated, @CreatorID); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("Value", SqlDbType.VarChar, 200);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Tags] WHERE ([TagID] = @TagID);";
		deleteCommand.Parameters.Add("TagID", SqlDbType.Int);

		try
		{
		  foreach (Tag tag in this)
		  {
			if (tag.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(tag);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = tag.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TagID"].AutoIncrement = false;
			  Table.Columns["TagID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				tag.Row["TagID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(tag);
			}
			else if (tag.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(tag);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = tag.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(tag);
			}
			else if (tag.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)tag.Row["TagID", DataRowVersion.Original];
			  deleteCommand.Parameters["TagID"].Value = id;
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

      foreach (Tag tag in this)
      {
        if (tag.Row.Table.Columns.Contains("CreatorID") && (int)tag.Row["CreatorID"] == 0) tag.Row["CreatorID"] = LoginUser.UserID;
        if (tag.Row.Table.Columns.Contains("ModifierID")) tag.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Tag FindByTagID(int tagID)
    {
      foreach (Tag tag in this)
      {
        if (tag.TagID == tagID)
        {
          return tag;
        }
      }
      return null;
    }

    public virtual Tag AddNewTag()
    {
      if (Table.Columns.Count < 1) LoadColumns("Tags");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Tag(row, this);
    }
    
    public virtual void LoadByTagID(int tagID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TagID], [OrganizationID], [Value], [DateCreated], [CreatorID] FROM [dbo].[Tags] WHERE ([TagID] = @TagID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TagID", tagID);
        Fill(command);
      }
    }
    
    public static Tag GetTag(LoginUser loginUser, int tagID)
    {
      Tags tags = new Tags(loginUser);
      tags.LoadByTagID(tagID);
      if (tags.IsEmpty)
        return null;
      else
        return tags[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Tag> Members

    public IEnumerator<Tag> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Tag(row, this);
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
