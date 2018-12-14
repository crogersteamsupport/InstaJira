using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class PhoneOption : BaseItem
  {
    private PhoneOptions _phoneOptions;
    
    public PhoneOption(DataRow row, PhoneOptions phoneOptions): base(row, phoneOptions)
    {
      _phoneOptions = phoneOptions;
    }
	
    #region Properties
    
    public PhoneOptions Collection
    {
      get { return _phoneOptions; }
    }
        
    
    
    
    public int PhoneOptionID
    {
      get { return (int)Row["PhoneOptionID"]; }
    }
    

    
    public int? PhoneActive
    {
      get { return Row["PhoneActive"] != DBNull.Value ? (int?)Row["PhoneActive"] : null; }
      set { Row["PhoneActive"] = CheckValue("PhoneActive", value); }
    }
    
    public string AccountSID
    {
      get { return Row["AccountSID"] != DBNull.Value ? (string)Row["AccountSID"] : null; }
      set { Row["AccountSID"] = CheckValue("AccountSID", value); }
    }
    
    public string AccountToken
    {
      get { return Row["AccountToken"] != DBNull.Value ? (string)Row["AccountToken"] : null; }
      set { Row["AccountToken"] = CheckValue("AccountToken", value); }
    }
    
    public string WelcomeAudioURL
    {
      get { return Row["WelcomeAudioURL"] != DBNull.Value ? (string)Row["WelcomeAudioURL"] : null; }
      set { Row["WelcomeAudioURL"] = CheckValue("WelcomeAudioURL", value); }
    }
    

    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class PhoneOptions : BaseCollection, IEnumerable<PhoneOption>
  {
    public PhoneOptions(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "PhoneOptions"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "PhoneOptionID"; }
    }



    public PhoneOption this[int index]
    {
      get { return new PhoneOption(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(PhoneOption phoneOption);
    partial void AfterRowInsert(PhoneOption phoneOption);
    partial void BeforeRowEdit(PhoneOption phoneOption);
    partial void AfterRowEdit(PhoneOption phoneOption);
    partial void BeforeRowDelete(int phoneOptionID);
    partial void AfterRowDelete(int phoneOptionID);    

    partial void BeforeDBDelete(int phoneOptionID);
    partial void AfterDBDelete(int phoneOptionID);    

    #endregion

    #region Public Methods

    public PhoneOptionProxy[] GetPhoneOptionProxies()
    {
      List<PhoneOptionProxy> list = new List<PhoneOptionProxy>();

      foreach (PhoneOption item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int phoneOptionID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[PhoneOptions] WHERE ([PhoneOptionID] = @PhoneOptionID);";
        deleteCommand.Parameters.Add("PhoneOptionID", SqlDbType.Int);
        deleteCommand.Parameters["PhoneOptionID"].Value = phoneOptionID;

        BeforeDBDelete(phoneOptionID);
        BeforeRowDelete(phoneOptionID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(phoneOptionID);
        AfterDBDelete(phoneOptionID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("PhoneOptionsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[PhoneOptions] SET     [OrganizationID] = @OrganizationID,    [PhoneActive] = @PhoneActive,    [AccountSID] = @AccountSID,    [AccountToken] = @AccountToken,    [WelcomeAudioURL] = @WelcomeAudioURL  WHERE ([PhoneOptionID] = @PhoneOptionID);";

		
		tempParameter = updateCommand.Parameters.Add("PhoneOptionID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("PhoneActive", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("AccountSID", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("AccountToken", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("WelcomeAudioURL", SqlDbType.VarChar, 400);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[PhoneOptions] (    [OrganizationID],    [PhoneActive],    [AccountSID],    [AccountToken],    [WelcomeAudioURL]) VALUES ( @OrganizationID, @PhoneActive, @AccountSID, @AccountToken, @WelcomeAudioURL); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("WelcomeAudioURL", SqlDbType.VarChar, 400);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("AccountToken", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("AccountSID", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PhoneActive", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[PhoneOptions] WHERE ([PhoneOptionID] = @PhoneOptionID);";
		deleteCommand.Parameters.Add("PhoneOptionID", SqlDbType.Int);

		try
		{
		  foreach (PhoneOption phoneOption in this)
		  {
			if (phoneOption.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(phoneOption);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = phoneOption.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["PhoneOptionID"].AutoIncrement = false;
			  Table.Columns["PhoneOptionID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				phoneOption.Row["PhoneOptionID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(phoneOption);
			}
			else if (phoneOption.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(phoneOption);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = phoneOption.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(phoneOption);
			}
			else if (phoneOption.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)phoneOption.Row["PhoneOptionID", DataRowVersion.Original];
			  deleteCommand.Parameters["PhoneOptionID"].Value = id;
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

      foreach (PhoneOption phoneOption in this)
      {
        if (phoneOption.Row.Table.Columns.Contains("CreatorID") && (int)phoneOption.Row["CreatorID"] == 0) phoneOption.Row["CreatorID"] = LoginUser.UserID;
        if (phoneOption.Row.Table.Columns.Contains("ModifierID")) phoneOption.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public PhoneOption FindByPhoneOptionID(int phoneOptionID)
    {
      foreach (PhoneOption phoneOption in this)
      {
        if (phoneOption.PhoneOptionID == phoneOptionID)
        {
          return phoneOption;
        }
      }
      return null;
    }

    public virtual PhoneOption AddNewPhoneOption()
    {
      if (Table.Columns.Count < 1) LoadColumns("PhoneOptions");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new PhoneOption(row, this);
    }
    
    public virtual void LoadByPhoneOptionID(int phoneOptionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [PhoneOptionID], [OrganizationID], [PhoneActive], [AccountSID], [AccountToken], [WelcomeAudioURL] FROM [dbo].[PhoneOptions] WHERE ([PhoneOptionID] = @PhoneOptionID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("PhoneOptionID", phoneOptionID);
        Fill(command);
      }
    }
    
    public static PhoneOption GetPhoneOption(LoginUser loginUser, int phoneOptionID)
    {
      PhoneOptions phoneOptions = new PhoneOptions(loginUser);
      phoneOptions.LoadByPhoneOptionID(phoneOptionID);
      if (phoneOptions.IsEmpty)
        return null;
      else
        return phoneOptions[0];
    }
    
    
    

    #endregion

    #region IEnumerable<PhoneOption> Members

    public IEnumerator<PhoneOption> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new PhoneOption(row, this);
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
