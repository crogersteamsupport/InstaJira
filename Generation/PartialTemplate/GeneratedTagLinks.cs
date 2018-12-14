using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TagLink : BaseItem
  {
    private TagLinks _tagLinks;
    
    public TagLink(DataRow row, TagLinks tagLinks): base(row, tagLinks)
    {
      _tagLinks = tagLinks;
    }
	
    #region Properties
    
    public TagLinks Collection
    {
      get { return _tagLinks; }
    }
        
    
    
    
    public int TagLinkID
    {
      get { return (int)Row["TagLinkID"]; }
    }
    

    

    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public int RefID
    {
      get { return (int)Row["RefID"]; }
      set { Row["RefID"] = CheckValue("RefID", value); }
    }
    
    public ReferenceType RefType
    {
      get { return (ReferenceType)Row["RefType"]; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    
    public int TagID
    {
      get { return (int)Row["TagID"]; }
      set { Row["TagID"] = CheckValue("TagID", value); }
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

  public partial class TagLinks : BaseCollection, IEnumerable<TagLink>
  {
    public TagLinks(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TagLinks"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TagLinkID"; }
    }



    public TagLink this[int index]
    {
      get { return new TagLink(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TagLink tagLink);
    partial void AfterRowInsert(TagLink tagLink);
    partial void BeforeRowEdit(TagLink tagLink);
    partial void AfterRowEdit(TagLink tagLink);
    partial void BeforeRowDelete(int tagLinkID);
    partial void AfterRowDelete(int tagLinkID);    

    partial void BeforeDBDelete(int tagLinkID);
    partial void AfterDBDelete(int tagLinkID);    

    #endregion

    #region Public Methods

    public TagLinkProxy[] GetTagLinkProxies()
    {
      List<TagLinkProxy> list = new List<TagLinkProxy>();

      foreach (TagLink item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int tagLinkID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TagLinks] WHERE ([TagLinkID] = @TagLinkID);";
        deleteCommand.Parameters.Add("TagLinkID", SqlDbType.Int);
        deleteCommand.Parameters["TagLinkID"].Value = tagLinkID;

        BeforeDBDelete(tagLinkID);
        BeforeRowDelete(tagLinkID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(tagLinkID);
        AfterDBDelete(tagLinkID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TagLinksSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TagLinks] SET     [TagID] = @TagID,    [RefType] = @RefType,    [RefID] = @RefID  WHERE ([TagLinkID] = @TagLinkID);";

		
		tempParameter = updateCommand.Parameters.Add("TagLinkID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TagID", SqlDbType.Int, 4);
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
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TagLinks] (    [TagID],    [RefType],    [RefID],    [DateCreated],    [CreatorID]) VALUES ( @TagID, @RefType, @RefID, @DateCreated, @CreatorID); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("TagID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TagLinks] WHERE ([TagLinkID] = @TagLinkID);";
		deleteCommand.Parameters.Add("TagLinkID", SqlDbType.Int);

		try
		{
		  foreach (TagLink tagLink in this)
		  {
			if (tagLink.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(tagLink);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = tagLink.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TagLinkID"].AutoIncrement = false;
			  Table.Columns["TagLinkID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				tagLink.Row["TagLinkID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(tagLink);
			}
			else if (tagLink.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(tagLink);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = tagLink.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(tagLink);
			}
			else if (tagLink.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)tagLink.Row["TagLinkID", DataRowVersion.Original];
			  deleteCommand.Parameters["TagLinkID"].Value = id;
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

      foreach (TagLink tagLink in this)
      {
        if (tagLink.Row.Table.Columns.Contains("CreatorID") && (int)tagLink.Row["CreatorID"] == 0) tagLink.Row["CreatorID"] = LoginUser.UserID;
        if (tagLink.Row.Table.Columns.Contains("ModifierID")) tagLink.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TagLink FindByTagLinkID(int tagLinkID)
    {
      foreach (TagLink tagLink in this)
      {
        if (tagLink.TagLinkID == tagLinkID)
        {
          return tagLink;
        }
      }
      return null;
    }

    public virtual TagLink AddNewTagLink()
    {
      if (Table.Columns.Count < 1) LoadColumns("TagLinks");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TagLink(row, this);
    }
    
    public virtual void LoadByTagLinkID(int tagLinkID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TagLinkID], [TagID], [RefType], [RefID], [DateCreated], [CreatorID] FROM [dbo].[TagLinks] WHERE ([TagLinkID] = @TagLinkID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TagLinkID", tagLinkID);
        Fill(command);
      }
    }
    
    public static TagLink GetTagLink(LoginUser loginUser, int tagLinkID)
    {
      TagLinks tagLinks = new TagLinks(loginUser);
      tagLinks.LoadByTagLinkID(tagLinkID);
      if (tagLinks.IsEmpty)
        return null;
      else
        return tagLinks[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TagLink> Members

    public IEnumerator<TagLink> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TagLink(row, this);
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
