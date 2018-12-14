using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CreditCard : BaseItem
  {
    private CreditCards _creditCards;
    
    public CreditCard(DataRow row, CreditCards creditCards): base(row, creditCards)
    {
      _creditCards = creditCards;
    }
	
    #region Properties
    
    public CreditCards Collection
    {
      get { return _creditCards; }
    }
        
    
    
    
    public int CreditCardID
    {
      get { return (int)Row["CreditCardID"]; }
    }
    

    
    public Byte[] CardNumber
    {
      get { return Row["CardNumber"] != DBNull.Value ? (Byte[])Row["CardNumber"] : null; }
      set { Row["CardNumber"] = CheckValue("CardNumber", value); }
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
    
    public string NameOnCard
    {
      get { return (string)Row["NameOnCard"]; }
      set { Row["NameOnCard"] = CheckValue("NameOnCard", value); }
    }
    
    public string SecurityCode
    {
      get { return (string)Row["SecurityCode"]; }
      set { Row["SecurityCode"] = CheckValue("SecurityCode", value); }
    }
    
    public CreditCardType CreditCardType
    {
      get { return (CreditCardType)Row["CreditCardType"]; }
      set { Row["CreditCardType"] = CheckValue("CreditCardType", value); }
    }
    
    public string DisplayNumber
    {
      get { return (string)Row["DisplayNumber"]; }
      set { Row["DisplayNumber"] = CheckValue("DisplayNumber", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateModfied
    {
      get { return DateToLocal((DateTime)Row["DateModfied"]); }
      set { Row["DateModfied"] = CheckValue("DateModfied", value); }
    }

    public DateTime DateModfiedUtc
    {
      get { return (DateTime)Row["DateModfied"]; }
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
    
    public DateTime ExpirationDate
    {
      get { return DateToLocal((DateTime)Row["ExpirationDate"]); }
      set { Row["ExpirationDate"] = CheckValue("ExpirationDate", value); }
    }

    public DateTime ExpirationDateUtc
    {
      get { return (DateTime)Row["ExpirationDate"]; }
    }
    

    #endregion
    
    
  }

  public partial class CreditCards : BaseCollection, IEnumerable<CreditCard>
  {
    public CreditCards(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CreditCards"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CreditCardID"; }
    }



    public CreditCard this[int index]
    {
      get { return new CreditCard(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CreditCard creditCard);
    partial void AfterRowInsert(CreditCard creditCard);
    partial void BeforeRowEdit(CreditCard creditCard);
    partial void AfterRowEdit(CreditCard creditCard);
    partial void BeforeRowDelete(int creditCardID);
    partial void AfterRowDelete(int creditCardID);    

    partial void BeforeDBDelete(int creditCardID);
    partial void AfterDBDelete(int creditCardID);    

    #endregion

    #region Public Methods

    public CreditCardProxy[] GetCreditCardProxies()
    {
      List<CreditCardProxy> list = new List<CreditCardProxy>();

      foreach (CreditCard item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int creditCardID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CreditCards] WHERE ([CreditCardID] = @CreditCardID);";
        deleteCommand.Parameters.Add("CreditCardID", SqlDbType.Int);
        deleteCommand.Parameters["CreditCardID"].Value = creditCardID;

        BeforeDBDelete(creditCardID);
        BeforeRowDelete(creditCardID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(creditCardID);
        AfterDBDelete(creditCardID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CreditCardsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CreditCards] SET     [OrganizationID] = @OrganizationID,    [DisplayNumber] = @DisplayNumber,    [CreditCardType] = @CreditCardType,    [CardNumber] = @CardNumber,    [SecurityCode] = @SecurityCode,    [ExpirationDate] = @ExpirationDate,    [NameOnCard] = @NameOnCard,    [DateModfied] = @DateModfied,    [ModifierID] = @ModifierID  WHERE ([CreditCardID] = @CreditCardID);";

		
		tempParameter = updateCommand.Parameters.Add("CreditCardID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("DisplayNumber", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CreditCardType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CardNumber", SqlDbType.VarBinary, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SecurityCode", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ExpirationDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("NameOnCard", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModfied", SqlDbType.DateTime, 8);
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CreditCards] (    [OrganizationID],    [DisplayNumber],    [CreditCardType],    [CardNumber],    [SecurityCode],    [ExpirationDate],    [NameOnCard],    [DateCreated],    [DateModfied],    [CreatorID],    [ModifierID]) VALUES ( @OrganizationID, @DisplayNumber, @CreditCardType, @CardNumber, @SecurityCode, @ExpirationDate, @NameOnCard, @DateCreated, @DateModfied, @CreatorID, @ModifierID); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("DateModfied", SqlDbType.DateTime, 8);
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
		
		tempParameter = insertCommand.Parameters.Add("NameOnCard", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ExpirationDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("SecurityCode", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CardNumber", SqlDbType.VarBinary, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreditCardType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayNumber", SqlDbType.VarChar, 50);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CreditCards] WHERE ([CreditCardID] = @CreditCardID);";
		deleteCommand.Parameters.Add("CreditCardID", SqlDbType.Int);

		try
		{
		  foreach (CreditCard creditCard in this)
		  {
			if (creditCard.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(creditCard);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = creditCard.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CreditCardID"].AutoIncrement = false;
			  Table.Columns["CreditCardID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				creditCard.Row["CreditCardID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(creditCard);
			}
			else if (creditCard.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(creditCard);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = creditCard.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(creditCard);
			}
			else if (creditCard.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)creditCard.Row["CreditCardID", DataRowVersion.Original];
			  deleteCommand.Parameters["CreditCardID"].Value = id;
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

      foreach (CreditCard creditCard in this)
      {
        if (creditCard.Row.Table.Columns.Contains("CreatorID") && (int)creditCard.Row["CreatorID"] == 0) creditCard.Row["CreatorID"] = LoginUser.UserID;
        if (creditCard.Row.Table.Columns.Contains("ModifierID")) creditCard.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CreditCard FindByCreditCardID(int creditCardID)
    {
      foreach (CreditCard creditCard in this)
      {
        if (creditCard.CreditCardID == creditCardID)
        {
          return creditCard;
        }
      }
      return null;
    }

    public virtual CreditCard AddNewCreditCard()
    {
      if (Table.Columns.Count < 1) LoadColumns("CreditCards");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CreditCard(row, this);
    }
    
    public virtual void LoadByCreditCardID(int creditCardID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CreditCardID], [OrganizationID], [DisplayNumber], [CreditCardType], [CardNumber], [SecurityCode], [ExpirationDate], [NameOnCard], [DateCreated], [DateModfied], [CreatorID], [ModifierID] FROM [dbo].[CreditCards] WHERE ([CreditCardID] = @CreditCardID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CreditCardID", creditCardID);
        Fill(command);
      }
    }
    
    public static CreditCard GetCreditCard(LoginUser loginUser, int creditCardID)
    {
      CreditCards creditCards = new CreditCards(loginUser);
      creditCards.LoadByCreditCardID(creditCardID);
      if (creditCards.IsEmpty)
        return null;
      else
        return creditCards[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CreditCard> Members

    public IEnumerator<CreditCard> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CreditCard(row, this);
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
