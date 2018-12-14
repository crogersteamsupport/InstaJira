using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class PhoneNumber : BaseItem
  {
    private PhoneNumbers _phoneNumbers;
    
    public PhoneNumber(DataRow row, PhoneNumbers phoneNumbers): base(row, phoneNumbers)
    {
      _phoneNumbers = phoneNumbers;
    }
	
    #region Properties
    
    public PhoneNumbers Collection
    {
      get { return _phoneNumbers; }
    }
        
    
    
    
    public int PhoneID
    {
      get { return (int)Row["PhoneID"]; }
    }
    

    
    public int? PhoneTypeID
    {
      get { return Row["PhoneTypeID"] != DBNull.Value ? (int?)Row["PhoneTypeID"] : null; }
      set { Row["PhoneTypeID"] = CheckValue("PhoneTypeID", value); }
    }
    
    public string Extension
    {
      get { return Row["Extension"] != DBNull.Value ? (string)Row["Extension"] : null; }
      set { Row["Extension"] = CheckValue("Extension", value); }
    }
    
    public string OtherTypeName
    {
      get { return Row["OtherTypeName"] != DBNull.Value ? (string)Row["OtherTypeName"] : null; }
      set { Row["OtherTypeName"] = CheckValue("OtherTypeName", value); }
    }
    
    public int? ImportFileID
    {
      get { return Row["ImportFileID"] != DBNull.Value ? (int?)Row["ImportFileID"] : null; }
      set { Row["ImportFileID"] = CheckValue("ImportFileID", value); }
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
    
    public string Number
    {
      get { return (string)Row["PhoneNumber"]; }
      set { Row["PhoneNumber"] = CheckValue("PhoneNumber", value); }
    }
    
    public ReferenceType RefType
    {
      get { return (ReferenceType)Row["RefType"]; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    
    public int RefID
    {
      get { return (int)Row["RefID"]; }
      set { Row["RefID"] = CheckValue("RefID", value); }
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

  public partial class PhoneNumbers : BaseCollection, IEnumerable<PhoneNumber>
  {
    public PhoneNumbers(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "PhoneNumbers"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "PhoneID"; }
    }



    public PhoneNumber this[int index]
    {
      get { return new PhoneNumber(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(PhoneNumber phoneNumber);
    partial void AfterRowInsert(PhoneNumber phoneNumber);
    partial void BeforeRowEdit(PhoneNumber phoneNumber);
    partial void AfterRowEdit(PhoneNumber phoneNumber);
    partial void BeforeRowDelete(int phoneID);
    partial void AfterRowDelete(int phoneID);    

    partial void BeforeDBDelete(int phoneID);
    partial void AfterDBDelete(int phoneID);    

    #endregion

    #region Public Methods

    public PhoneNumberProxy[] GetPhoneNumberProxies()
    {
      List<PhoneNumberProxy> list = new List<PhoneNumberProxy>();

      foreach (PhoneNumber item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int phoneID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[PhoneNumbers] WHERE ([PhoneID] = @PhoneID);";
        deleteCommand.Parameters.Add("PhoneID", SqlDbType.Int);
        deleteCommand.Parameters["PhoneID"].Value = phoneID;

        BeforeDBDelete(phoneID);
        BeforeRowDelete(phoneID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(phoneID);
        AfterDBDelete(phoneID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("PhoneNumbersSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[PhoneNumbers] SET     [PhoneTypeID] = @PhoneTypeID,    [RefID] = @RefID,    [RefType] = @RefType,    [PhoneNumber] = @PhoneNumber,    [Extension] = @Extension,    [OtherTypeName] = @OtherTypeName,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [ImportFileID] = @ImportFileID  WHERE ([PhoneID] = @PhoneID);";

		
		tempParameter = updateCommand.Parameters.Add("PhoneID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("PhoneTypeID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("PhoneNumber", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Extension", SqlDbType.VarChar, 10);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("OtherTypeName", SqlDbType.VarChar, 50);
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
		
		tempParameter = updateCommand.Parameters.Add("ImportFileID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[PhoneNumbers] (    [PhoneTypeID],    [RefID],    [RefType],    [PhoneNumber],    [Extension],    [OtherTypeName],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [ImportFileID]) VALUES ( @PhoneTypeID, @RefID, @RefType, @PhoneNumber, @Extension, @OtherTypeName, @DateCreated, @DateModified, @CreatorID, @ModifierID, @ImportFileID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ImportFileID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		
		tempParameter = insertCommand.Parameters.Add("OtherTypeName", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Extension", SqlDbType.VarChar, 10);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PhoneNumber", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("PhoneTypeID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[PhoneNumbers] WHERE ([PhoneID] = @PhoneID);";
		deleteCommand.Parameters.Add("PhoneID", SqlDbType.Int);

		try
		{
		  foreach (PhoneNumber phoneNumber in this)
		  {
			if (phoneNumber.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(phoneNumber);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = phoneNumber.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["PhoneID"].AutoIncrement = false;
			  Table.Columns["PhoneID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				phoneNumber.Row["PhoneID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(phoneNumber);
			}
			else if (phoneNumber.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(phoneNumber);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = phoneNumber.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(phoneNumber);
			}
			else if (phoneNumber.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)phoneNumber.Row["PhoneID", DataRowVersion.Original];
			  deleteCommand.Parameters["PhoneID"].Value = id;
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

      foreach (PhoneNumber phoneNumber in this)
      {
        if (phoneNumber.Row.Table.Columns.Contains("CreatorID") && (int)phoneNumber.Row["CreatorID"] == 0) phoneNumber.Row["CreatorID"] = LoginUser.UserID;
        if (phoneNumber.Row.Table.Columns.Contains("ModifierID")) phoneNumber.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public PhoneNumber FindByPhoneID(int phoneID)
    {
      foreach (PhoneNumber phoneNumber in this)
      {
        if (phoneNumber.PhoneID == phoneID)
        {
          return phoneNumber;
        }
      }
      return null;
    }

    public virtual PhoneNumber AddNewPhoneNumber()
    {
      if (Table.Columns.Count < 1) LoadColumns("PhoneNumbers");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new PhoneNumber(row, this);
    }
    
    public virtual void LoadByPhoneID(int phoneID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [PhoneID], [PhoneTypeID], [RefID], [RefType], [PhoneNumber], [Extension], [OtherTypeName], [DateCreated], [DateModified], [CreatorID], [ModifierID], [ImportFileID] FROM [dbo].[PhoneNumbers] WHERE ([PhoneID] = @PhoneID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("PhoneID", phoneID);
        Fill(command);
      }
    }
    
    public static PhoneNumber GetPhoneNumber(LoginUser loginUser, int phoneID)
    {
      PhoneNumbers phoneNumbers = new PhoneNumbers(loginUser);
      phoneNumbers.LoadByPhoneID(phoneID);
      if (phoneNumbers.IsEmpty)
        return null;
      else
        return phoneNumbers[0];
    }
    
    
    

    #endregion

    #region IEnumerable<PhoneNumber> Members

    public IEnumerator<PhoneNumber> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new PhoneNumber(row, this);
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
