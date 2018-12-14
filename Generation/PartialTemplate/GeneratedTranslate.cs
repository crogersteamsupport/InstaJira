using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TranslateItem : BaseItem
  {
    private Translate _translate;
    
    public TranslateItem(DataRow row, Translate translate): base(row, translate)
    {
      _translate = translate;
    }
	
    #region Properties
    
    public Translate Collection
    {
      get { return _translate; }
    }
        
    
    
    
    public int PhraseID
    {
      get { return (int)Row["PhraseID"]; }
    }
    

    
    public string English
    {
      get { return Row["English"] != DBNull.Value ? (string)Row["English"] : null; }
      set { Row["English"] = CheckValue("English", value); }
    }
    
    public string French
    {
      get { return Row["French"] != DBNull.Value ? (string)Row["French"] : null; }
      set { Row["French"] = CheckValue("French", value); }
    }
    
    public string Italian
    {
      get { return Row["Italian"] != DBNull.Value ? (string)Row["Italian"] : null; }
      set { Row["Italian"] = CheckValue("Italian", value); }
    }
    
    public string German
    {
      get { return Row["German"] != DBNull.Value ? (string)Row["German"] : null; }
      set { Row["German"] = CheckValue("German", value); }
    }
    
    public string Spanish
    {
      get { return Row["Spanish"] != DBNull.Value ? (string)Row["Spanish"] : null; }
      set { Row["Spanish"] = CheckValue("Spanish", value); }
    }
    
    public string Portugese
    {
      get { return Row["Portugese"] != DBNull.Value ? (string)Row["Portugese"] : null; }
      set { Row["Portugese"] = CheckValue("Portugese", value); }
    }
    

    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class Translate : BaseCollection, IEnumerable<TranslateItem>
  {
    public Translate(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Translate"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "PhraseID"; }
    }



    public TranslateItem this[int index]
    {
      get { return new TranslateItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TranslateItem translateItem);
    partial void AfterRowInsert(TranslateItem translateItem);
    partial void BeforeRowEdit(TranslateItem translateItem);
    partial void AfterRowEdit(TranslateItem translateItem);
    partial void BeforeRowDelete(int phraseID);
    partial void AfterRowDelete(int phraseID);    

    partial void BeforeDBDelete(int phraseID);
    partial void AfterDBDelete(int phraseID);    

    #endregion

    #region Public Methods

    public TranslateItemProxy[] GetTranslateItemProxies()
    {
      List<TranslateItemProxy> list = new List<TranslateItemProxy>();

      foreach (TranslateItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int phraseID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Translate] WHERE ([PhraseID] = @PhraseID);";
        deleteCommand.Parameters.Add("PhraseID", SqlDbType.Int);
        deleteCommand.Parameters["PhraseID"].Value = phraseID;

        BeforeDBDelete(phraseID);
        BeforeRowDelete(phraseID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(phraseID);
        AfterDBDelete(phraseID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TranslateSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Translate] SET     [English] = @English,    [French] = @French,    [Italian] = @Italian,    [German] = @German,    [Spanish] = @Spanish,    [Portugese] = @Portugese  WHERE ([PhraseID] = @PhraseID);";

		
		tempParameter = updateCommand.Parameters.Add("PhraseID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("English", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("French", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Italian", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("German", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Spanish", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Portugese", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Translate] (    [English],    [French],    [Italian],    [German],    [Spanish],    [Portugese]) VALUES ( @English, @French, @Italian, @German, @Spanish, @Portugese); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("Portugese", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Spanish", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("German", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Italian", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("French", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("English", SqlDbType.VarChar, 500);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Translate] WHERE ([PhraseID] = @PhraseID);";
		deleteCommand.Parameters.Add("PhraseID", SqlDbType.Int);

		try
		{
		  foreach (TranslateItem translateItem in this)
		  {
			if (translateItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(translateItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = translateItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["PhraseID"].AutoIncrement = false;
			  Table.Columns["PhraseID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				translateItem.Row["PhraseID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(translateItem);
			}
			else if (translateItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(translateItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = translateItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(translateItem);
			}
			else if (translateItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)translateItem.Row["PhraseID", DataRowVersion.Original];
			  deleteCommand.Parameters["PhraseID"].Value = id;
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

      foreach (TranslateItem translateItem in this)
      {
        if (translateItem.Row.Table.Columns.Contains("CreatorID") && (int)translateItem.Row["CreatorID"] == 0) translateItem.Row["CreatorID"] = LoginUser.UserID;
        if (translateItem.Row.Table.Columns.Contains("ModifierID")) translateItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TranslateItem FindByPhraseID(int phraseID)
    {
      foreach (TranslateItem translateItem in this)
      {
        if (translateItem.PhraseID == phraseID)
        {
          return translateItem;
        }
      }
      return null;
    }

    public virtual TranslateItem AddNewTranslateItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("Translate");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TranslateItem(row, this);
    }
    
    public virtual void LoadByPhraseID(int phraseID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [PhraseID], [English], [French], [Italian], [German], [Spanish], [Portugese] FROM [dbo].[Translate] WHERE ([PhraseID] = @PhraseID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("PhraseID", phraseID);
        Fill(command);
      }
    }
    
    public static TranslateItem GetTranslateItem(LoginUser loginUser, int phraseID)
    {
      Translate translate = new Translate(loginUser);
      translate.LoadByPhraseID(phraseID);
      if (translate.IsEmpty)
        return null;
      else
        return translate[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TranslateItem> Members

    public IEnumerator<TranslateItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TranslateItem(row, this);
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
