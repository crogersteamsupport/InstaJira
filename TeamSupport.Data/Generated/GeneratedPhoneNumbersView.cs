using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class PhoneNumbersViewItem : BaseItem
  {
    private PhoneNumbersView _phoneNumbersView;
    
    public PhoneNumbersViewItem(DataRow row, PhoneNumbersView phoneNumbersView): base(row, phoneNumbersView)
    {
      _phoneNumbersView = phoneNumbersView;
    }
	
    #region Properties
    
    public PhoneNumbersView Collection
    {
      get { return _phoneNumbersView; }
    }
        
    
    public string CreatorName
    {
      get { return Row["CreatorName"] != DBNull.Value ? (string)Row["CreatorName"] : null; }
    }
    
    public string ModifierName
    {
      get { return Row["ModifierName"] != DBNull.Value ? (string)Row["ModifierName"] : null; }
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
    
    public string PhoneType
    {
      get { return Row["PhoneType"] != DBNull.Value ? (string)Row["PhoneType"] : null; }
      set { Row["PhoneType"] = CheckValue("PhoneType", value); }
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
    
    public string PhoneNumber
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
    
    public int PhoneID
    {
      get { return (int)Row["PhoneID"]; }
      set { Row["PhoneID"] = CheckValue("PhoneID", value); }
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

  public partial class PhoneNumbersView : BaseCollection, IEnumerable<PhoneNumbersViewItem>
  {
    public PhoneNumbersView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "PhoneNumbersView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "PhoneID"; }
    }



    public PhoneNumbersViewItem this[int index]
    {
      get { return new PhoneNumbersViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(PhoneNumbersViewItem phoneNumbersViewItem);
    partial void AfterRowInsert(PhoneNumbersViewItem phoneNumbersViewItem);
    partial void BeforeRowEdit(PhoneNumbersViewItem phoneNumbersViewItem);
    partial void AfterRowEdit(PhoneNumbersViewItem phoneNumbersViewItem);
    partial void BeforeRowDelete(int phoneID);
    partial void AfterRowDelete(int phoneID);    

    partial void BeforeDBDelete(int phoneID);
    partial void AfterDBDelete(int phoneID);    

    #endregion

    #region Public Methods

    public PhoneNumbersViewItemProxy[] GetPhoneNumbersViewItemProxies()
    {
      List<PhoneNumbersViewItemProxy> list = new List<PhoneNumbersViewItemProxy>();

      foreach (PhoneNumbersViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int phoneID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[PhoneNumbersView] WHERE ([PhoneID] = @PhoneID);";
        deleteCommand.Parameters.Add("PhoneID", SqlDbType.Int);
        deleteCommand.Parameters["PhoneID"].Value = phoneID;

        BeforeDBDelete(phoneID);
        BeforeRowDelete(phoneID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(phoneID);
        AfterDBDelete(phoneID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("PhoneNumbersViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[PhoneNumbersView] SET     [PhoneTypeID] = @PhoneTypeID,    [RefID] = @RefID,    [RefType] = @RefType,    [PhoneNumber] = @PhoneNumber,    [Extension] = @Extension,    [OtherTypeName] = @OtherTypeName,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [PhoneType] = @PhoneType,    [CreatorName] = @CreatorName,    [ModifierName] = @ModifierName  WHERE ([PhoneID] = @PhoneID);";

		
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
		
		tempParameter = updateCommand.Parameters.Add("PhoneType", SqlDbType.VarChar, 50);
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
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[PhoneNumbersView] (    [PhoneID],    [PhoneTypeID],    [RefID],    [RefType],    [PhoneNumber],    [Extension],    [OtherTypeName],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [PhoneType],    [CreatorName],    [ModifierName]) VALUES ( @PhoneID, @PhoneTypeID, @RefID, @RefType, @PhoneNumber, @Extension, @OtherTypeName, @DateCreated, @DateModified, @CreatorID, @ModifierID, @PhoneType, @CreatorName, @ModifierName); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("PhoneType", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		
		tempParameter = insertCommand.Parameters.Add("PhoneID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[PhoneNumbersView] WHERE ([PhoneID] = @PhoneID);";
		deleteCommand.Parameters.Add("PhoneID", SqlDbType.Int);

		try
		{
		  foreach (PhoneNumbersViewItem phoneNumbersViewItem in this)
		  {
			if (phoneNumbersViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(phoneNumbersViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = phoneNumbersViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["PhoneID"].AutoIncrement = false;
			  Table.Columns["PhoneID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				phoneNumbersViewItem.Row["PhoneID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(phoneNumbersViewItem);
			}
			else if (phoneNumbersViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(phoneNumbersViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = phoneNumbersViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(phoneNumbersViewItem);
			}
			else if (phoneNumbersViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)phoneNumbersViewItem.Row["PhoneID", DataRowVersion.Original];
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

      foreach (PhoneNumbersViewItem phoneNumbersViewItem in this)
      {
        if (phoneNumbersViewItem.Row.Table.Columns.Contains("CreatorID") && (int)phoneNumbersViewItem.Row["CreatorID"] == 0) phoneNumbersViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (phoneNumbersViewItem.Row.Table.Columns.Contains("ModifierID")) phoneNumbersViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public PhoneNumbersViewItem FindByPhoneID(int phoneID)
    {
      foreach (PhoneNumbersViewItem phoneNumbersViewItem in this)
      {
        if (phoneNumbersViewItem.PhoneID == phoneID)
        {
          return phoneNumbersViewItem;
        }
      }
      return null;
    }

    public virtual PhoneNumbersViewItem AddNewPhoneNumbersViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("PhoneNumbersView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new PhoneNumbersViewItem(row, this);
    }
    
    public virtual void LoadByPhoneID(int phoneID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [PhoneID], [PhoneTypeID], [RefID], [RefType], [PhoneNumber], [Extension], [OtherTypeName], [DateCreated], [DateModified], [CreatorID], [ModifierID], [PhoneType], [CreatorName], [ModifierName] FROM [dbo].[PhoneNumbersView] WHERE ([PhoneID] = @PhoneID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("PhoneID", phoneID);
        Fill(command);
      }
    }
    
    public static PhoneNumbersViewItem GetPhoneNumbersViewItem(LoginUser loginUser, int phoneID)
    {
      PhoneNumbersView phoneNumbersView = new PhoneNumbersView(loginUser);
      phoneNumbersView.LoadByPhoneID(phoneID);
      if (phoneNumbersView.IsEmpty)
        return null;
      else
        return phoneNumbersView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<PhoneNumbersViewItem> Members

    public IEnumerator<PhoneNumbersViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new PhoneNumbersViewItem(row, this);
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
