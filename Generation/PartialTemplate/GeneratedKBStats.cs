using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class KBStat : BaseItem
  {
    private KBStats _kBStats;
    
    public KBStat(DataRow row, KBStats kBStats): base(row, kBStats)
    {
      _kBStats = kBStats;
    }
	
    #region Properties
    
    public KBStats Collection
    {
      get { return _kBStats; }
    }
        
    
    
    
    public int KBViewID
    {
      get { return (int)Row["KBViewID"]; }
    }
    

    
    public int? OrganizationID
    {
      get { return Row["OrganizationID"] != DBNull.Value ? (int?)Row["OrganizationID"] : null; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public int? KBArticleID
    {
      get { return Row["KBArticleID"] != DBNull.Value ? (int?)Row["KBArticleID"] : null; }
      set { Row["KBArticleID"] = CheckValue("KBArticleID", value); }
    }
    
    public string ViewIP
    {
      get { return Row["ViewIP"] != DBNull.Value ? (string)Row["ViewIP"] : null; }
      set { Row["ViewIP"] = CheckValue("ViewIP", value); }
    }
    
    public string SearchTerm
    {
      get { return Row["SearchTerm"] != DBNull.Value ? (string)Row["SearchTerm"] : null; }
      set { Row["SearchTerm"] = CheckValue("SearchTerm", value); }
    }
    

    

    /* DateTime */
    
    
    

    
    public DateTime? ViewDateTime
    {
      get { return Row["ViewDateTime"] != DBNull.Value ? DateToLocal((DateTime?)Row["ViewDateTime"]) : null; }
      set { Row["ViewDateTime"] = CheckValue("ViewDateTime", value); }
    }

    public DateTime? ViewDateTimeUtc
    {
      get { return Row["ViewDateTime"] != DBNull.Value ? (DateTime?)Row["ViewDateTime"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class KBStats : BaseCollection, IEnumerable<KBStat>
  {
    public KBStats(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "KBStats"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "KBViewID"; }
    }



    public KBStat this[int index]
    {
      get { return new KBStat(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(KBStat kBStat);
    partial void AfterRowInsert(KBStat kBStat);
    partial void BeforeRowEdit(KBStat kBStat);
    partial void AfterRowEdit(KBStat kBStat);
    partial void BeforeRowDelete(int kBViewID);
    partial void AfterRowDelete(int kBViewID);    

    partial void BeforeDBDelete(int kBViewID);
    partial void AfterDBDelete(int kBViewID);    

    #endregion

    #region Public Methods

    public KBStatProxy[] GetKBStatProxies()
    {
      List<KBStatProxy> list = new List<KBStatProxy>();

      foreach (KBStat item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int kBViewID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[KBStats] WHERE ([KBViewID] = @KBViewID);";
        deleteCommand.Parameters.Add("KBViewID", SqlDbType.Int);
        deleteCommand.Parameters["KBViewID"].Value = kBViewID;

        BeforeDBDelete(kBViewID);
        BeforeRowDelete(kBViewID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(kBViewID);
        AfterDBDelete(kBViewID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("KBStatsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[KBStats] SET     [OrganizationID] = @OrganizationID,    [KBArticleID] = @KBArticleID,    [ViewDateTime] = @ViewDateTime,    [ViewIP] = @ViewIP,    [SearchTerm] = @SearchTerm  WHERE ([KBViewID] = @KBViewID);";

		
		tempParameter = updateCommand.Parameters.Add("KBViewID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("KBArticleID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ViewDateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ViewIP", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SearchTerm", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[KBStats] (    [OrganizationID],    [KBArticleID],    [ViewDateTime],    [ViewIP],    [SearchTerm]) VALUES ( @OrganizationID, @KBArticleID, @ViewDateTime, @ViewIP, @SearchTerm); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("SearchTerm", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ViewIP", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ViewDateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("KBArticleID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[KBStats] WHERE ([KBViewID] = @KBViewID);";
		deleteCommand.Parameters.Add("KBViewID", SqlDbType.Int);

		try
		{
		  foreach (KBStat kBStat in this)
		  {
			if (kBStat.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(kBStat);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = kBStat.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["KBViewID"].AutoIncrement = false;
			  Table.Columns["KBViewID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				kBStat.Row["KBViewID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(kBStat);
			}
			else if (kBStat.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(kBStat);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = kBStat.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(kBStat);
			}
			else if (kBStat.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)kBStat.Row["KBViewID", DataRowVersion.Original];
			  deleteCommand.Parameters["KBViewID"].Value = id;
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

      foreach (KBStat kBStat in this)
      {
        if (kBStat.Row.Table.Columns.Contains("CreatorID") && (int)kBStat.Row["CreatorID"] == 0) kBStat.Row["CreatorID"] = LoginUser.UserID;
        if (kBStat.Row.Table.Columns.Contains("ModifierID")) kBStat.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public KBStat FindByKBViewID(int kBViewID)
    {
      foreach (KBStat kBStat in this)
      {
        if (kBStat.KBViewID == kBViewID)
        {
          return kBStat;
        }
      }
      return null;
    }

    public virtual KBStat AddNewKBStat()
    {
      if (Table.Columns.Count < 1) LoadColumns("KBStats");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new KBStat(row, this);
    }
    
    public virtual void LoadByKBViewID(int kBViewID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [KBViewID], [OrganizationID], [KBArticleID], [ViewDateTime], [ViewIP], [SearchTerm] FROM [dbo].[KBStats] WHERE ([KBViewID] = @KBViewID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("KBViewID", kBViewID);
        Fill(command);
      }
    }
    
    public static KBStat GetKBStat(LoginUser loginUser, int kBViewID)
    {
      KBStats kBStats = new KBStats(loginUser);
      kBStats.LoadByKBViewID(kBViewID);
      if (kBStats.IsEmpty)
        return null;
      else
        return kBStats[0];
    }
    
    
    

    #endregion

    #region IEnumerable<KBStat> Members

    public IEnumerator<KBStat> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new KBStat(row, this);
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
