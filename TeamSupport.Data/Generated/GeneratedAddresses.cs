using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Address : BaseItem
  {
    private Addresses _addresses;
    
    public Address(DataRow row, Addresses addresses): base(row, addresses)
    {
      _addresses = addresses;
    }
	
    #region Properties
    
    public Addresses Collection
    {
      get { return _addresses; }
    }
        
    
    
    
    public int AddressID
    {
      get { return (int)Row["AddressID"]; }
    }
    

    
    public string Addr1
    {
      get { return Row["Addr1"] != DBNull.Value ? (string)Row["Addr1"] : null; }
      set { Row["Addr1"] = CheckValue("Addr1", value); }
    }
    
    public string Addr2
    {
      get { return Row["Addr2"] != DBNull.Value ? (string)Row["Addr2"] : null; }
      set { Row["Addr2"] = CheckValue("Addr2", value); }
    }
    
    public string Addr3
    {
      get { return Row["Addr3"] != DBNull.Value ? (string)Row["Addr3"] : null; }
      set { Row["Addr3"] = CheckValue("Addr3", value); }
    }
    
    public string City
    {
      get { return Row["City"] != DBNull.Value ? (string)Row["City"] : null; }
      set { Row["City"] = CheckValue("City", value); }
    }
    
    public string State
    {
      get { return Row["State"] != DBNull.Value ? (string)Row["State"] : null; }
      set { Row["State"] = CheckValue("State", value); }
    }
    
    public string Zip
    {
      get { return Row["Zip"] != DBNull.Value ? (string)Row["Zip"] : null; }
      set { Row["Zip"] = CheckValue("Zip", value); }
    }
    
    public string Country
    {
      get { return Row["Country"] != DBNull.Value ? (string)Row["Country"] : null; }
      set { Row["Country"] = CheckValue("Country", value); }
    }
    
    public string Comment
    {
      get { return Row["Comment"] != DBNull.Value ? (string)Row["Comment"] : null; }
      set { Row["Comment"] = CheckValue("Comment", value); }
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
    
    public string Description
    {
      get { return (string)Row["Description"]; }
      set { Row["Description"] = CheckValue("Description", value); }
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

  public partial class Addresses : BaseCollection, IEnumerable<Address>
  {
    public Addresses(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Addresses"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "AddressID"; }
    }



    public Address this[int index]
    {
      get { return new Address(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Address address);
    partial void AfterRowInsert(Address address);
    partial void BeforeRowEdit(Address address);
    partial void AfterRowEdit(Address address);
    partial void BeforeRowDelete(int addressID);
    partial void AfterRowDelete(int addressID);    

    partial void BeforeDBDelete(int addressID);
    partial void AfterDBDelete(int addressID);    

    #endregion

    #region Public Methods

    public AddressProxy[] GetAddressProxies()
    {
      List<AddressProxy> list = new List<AddressProxy>();

      foreach (Address item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int addressID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Addresses] WHERE ([AddressID] = @AddressID);";
        deleteCommand.Parameters.Add("AddressID", SqlDbType.Int);
        deleteCommand.Parameters["AddressID"].Value = addressID;

        BeforeDBDelete(addressID);
        BeforeRowDelete(addressID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(addressID);
        AfterDBDelete(addressID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("AddressesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Addresses] SET     [RefID] = @RefID,    [RefType] = @RefType,    [Description] = @Description,    [Addr1] = @Addr1,    [Addr2] = @Addr2,    [Addr3] = @Addr3,    [City] = @City,    [State] = @State,    [Zip] = @Zip,    [Country] = @Country,    [Comment] = @Comment,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [ImportFileID] = @ImportFileID  WHERE ([AddressID] = @AddressID);";

		
		tempParameter = updateCommand.Parameters.Add("AddressID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Addr1", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Addr2", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Addr3", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("City", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("State", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Zip", SqlDbType.VarChar, 30);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Country", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Comment", SqlDbType.VarChar, 1024);
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Addresses] (    [RefID],    [RefType],    [Description],    [Addr1],    [Addr2],    [Addr3],    [City],    [State],    [Zip],    [Country],    [Comment],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [ImportFileID]) VALUES ( @RefID, @RefType, @Description, @Addr1, @Addr2, @Addr3, @City, @State, @Zip, @Country, @Comment, @DateCreated, @DateModified, @CreatorID, @ModifierID, @ImportFileID); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("Comment", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Country", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Zip", SqlDbType.VarChar, 30);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("State", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("City", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Addr3", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Addr2", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Addr1", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 1024);
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
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Addresses] WHERE ([AddressID] = @AddressID);";
		deleteCommand.Parameters.Add("AddressID", SqlDbType.Int);

		try
		{
		  foreach (Address address in this)
		  {
			if (address.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(address);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = address.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["AddressID"].AutoIncrement = false;
			  Table.Columns["AddressID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				address.Row["AddressID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(address);
			}
			else if (address.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(address);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = address.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(address);
			}
			else if (address.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)address.Row["AddressID", DataRowVersion.Original];
			  deleteCommand.Parameters["AddressID"].Value = id;
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

      foreach (Address address in this)
      {
        if (address.Row.Table.Columns.Contains("CreatorID") && (int)address.Row["CreatorID"] == 0) address.Row["CreatorID"] = LoginUser.UserID;
        if (address.Row.Table.Columns.Contains("ModifierID")) address.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Address FindByAddressID(int addressID)
    {
      foreach (Address address in this)
      {
        if (address.AddressID == addressID)
        {
          return address;
        }
      }
      return null;
    }

    public virtual Address AddNewAddress()
    {
      if (Table.Columns.Count < 1) LoadColumns("Addresses");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Address(row, this);
    }
    
    public virtual void LoadByAddressID(int addressID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [AddressID], [RefID], [RefType], [Description], [Addr1], [Addr2], [Addr3], [City], [State], [Zip], [Country], [Comment], [DateCreated], [DateModified], [CreatorID], [ModifierID], [ImportFileID] FROM [dbo].[Addresses] WHERE ([AddressID] = @AddressID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("AddressID", addressID);
        Fill(command);
      }
    }
    
    public static Address GetAddress(LoginUser loginUser, int addressID)
    {
      Addresses addresses = new Addresses(loginUser);
      addresses.LoadByAddressID(addressID);
      if (addresses.IsEmpty)
        return null;
      else
        return addresses[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Address> Members

    public IEnumerator<Address> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Address(row, this);
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
