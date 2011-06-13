using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class UsersViewItem : BaseItem
  {
    private UsersView _usersView;
    
    public UsersViewItem(DataRow row, UsersView usersView): base(row, usersView)
    {
      _usersView = usersView;
    }
	
    #region Properties
    
    public UsersView Collection
    {
      get { return _usersView; }
    }
        
    
    
    
    public bool IsOnline
    {
      get { return (bool)Row["IsOnline"]; }
    }
    

    
    public string Title
    {
      get { return Row["Title"] != DBNull.Value ? (string)Row["Title"] : null; }
      set { Row["Title"] = CheckNull(value); }
    }
    
    public int? PrimaryGroupID
    {
      get { return Row["PrimaryGroupID"] != DBNull.Value ? (int?)Row["PrimaryGroupID"] : null; }
      set { Row["PrimaryGroupID"] = CheckNull(value); }
    }
    
    public string Organization
    {
      get { return Row["Organization"] != DBNull.Value ? (string)Row["Organization"] : null; }
      set { Row["Organization"] = CheckNull(value); }
    }
    
    public string LastVersion
    {
      get { return Row["LastVersion"] != DBNull.Value ? (string)Row["LastVersion"] : null; }
      set { Row["LastVersion"] = CheckNull(value); }
    }
    

    
    public bool IsChatUser
    {
      get { return (bool)Row["IsChatUser"]; }
      set { Row["IsChatUser"] = CheckNull(value); }
    }
    
    public string CryptedPassword
    {
      get { return (string)Row["CryptedPassword"]; }
      set { Row["CryptedPassword"] = CheckNull(value); }
    }
    
    public int ModifierID
    {
      get { return (int)Row["ModifierID"]; }
      set { Row["ModifierID"] = CheckNull(value); }
    }
    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckNull(value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckNull(value); }
    }
    
    public string InOfficeComment
    {
      get { return (string)Row["InOfficeComment"]; }
      set { Row["InOfficeComment"] = CheckNull(value); }
    }
    
    public bool InOffice
    {
      get { return (bool)Row["InOffice"]; }
      set { Row["InOffice"] = CheckNull(value); }
    }
    
    public bool IsPortalUser
    {
      get { return (bool)Row["IsPortalUser"]; }
      set { Row["IsPortalUser"] = CheckNull(value); }
    }
    
    public bool IsPasswordExpired
    {
      get { return (bool)Row["IsPasswordExpired"]; }
      set { Row["IsPasswordExpired"] = CheckNull(value); }
    }
    
    public bool IsFinanceAdmin
    {
      get { return (bool)Row["IsFinanceAdmin"]; }
      set { Row["IsFinanceAdmin"] = CheckNull(value); }
    }
    
    public bool IsSystemAdmin
    {
      get { return (bool)Row["IsSystemAdmin"]; }
      set { Row["IsSystemAdmin"] = CheckNull(value); }
    }
    
    public bool MarkDeleted
    {
      get { return (bool)Row["MarkDeleted"]; }
      set { Row["MarkDeleted"] = CheckNull(value); }
    }
    
    public bool IsActive
    {
      get { return (bool)Row["IsActive"]; }
      set { Row["IsActive"] = CheckNull(value); }
    }
    
    public string LastName
    {
      get { return (string)Row["LastName"]; }
      set { Row["LastName"] = CheckNull(value); }
    }
    
    public string MiddleName
    {
      get { return (string)Row["MiddleName"]; }
      set { Row["MiddleName"] = CheckNull(value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckNull(value); }
    }
    
    public string FirstName
    {
      get { return (string)Row["FirstName"]; }
      set { Row["FirstName"] = CheckNull(value); }
    }
    
    public string Email
    {
      get { return (string)Row["Email"]; }
      set { Row["Email"] = CheckNull(value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? LastPing
    {
      get { return Row["LastPing"] != DBNull.Value ? DateToLocal((DateTime?)Row["LastPing"]) : null; }
      set { Row["LastPing"] = CheckNull(value); }
    }
    
    public DateTime? DeactivatedOn
    {
      get { return Row["DeactivatedOn"] != DBNull.Value ? DateToLocal((DateTime?)Row["DeactivatedOn"]) : null; }
      set { Row["DeactivatedOn"] = CheckNull(value); }
    }
    

    
    public DateTime DateModified
    {
      get { return DateToLocal((DateTime)Row["DateModified"]); }
      set { Row["DateModified"] = CheckNull(value); }
    }
    
    public DateTime DateCreated
    {
      get { return DateToLocal((DateTime)Row["DateCreated"]); }
      set { Row["DateCreated"] = CheckNull(value); }
    }
    
    public DateTime ActivatedOn
    {
      get { return DateToLocal((DateTime)Row["ActivatedOn"]); }
      set { Row["ActivatedOn"] = CheckNull(value); }
    }
    
    public DateTime LastActivity
    {
      get { return DateToLocal((DateTime)Row["LastActivity"]); }
      set { Row["LastActivity"] = CheckNull(value); }
    }
    
    public DateTime LastLogin
    {
      get { return DateToLocal((DateTime)Row["LastLogin"]); }
      set { Row["LastLogin"] = CheckNull(value); }
    }
    

    #endregion
    
    
  }

  public partial class UsersView : BaseCollection, IEnumerable<UsersViewItem>
  {
    public UsersView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "UsersView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "UserID"; }
    }



    public UsersViewItem this[int index]
    {
      get { return new UsersViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(UsersViewItem usersViewItem);
    partial void AfterRowInsert(UsersViewItem usersViewItem);
    partial void BeforeRowEdit(UsersViewItem usersViewItem);
    partial void AfterRowEdit(UsersViewItem usersViewItem);
    partial void BeforeRowDelete(int userID);
    partial void AfterRowDelete(int userID);    

    partial void BeforeDBDelete(int userID);
    partial void AfterDBDelete(int userID);    

    #endregion

    #region Public Methods

    public UsersViewItemProxy[] GetUsersViewItemProxies()
    {
      List<UsersViewItemProxy> list = new List<UsersViewItemProxy>();

      foreach (UsersViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int userID)
    {
      BeforeDBDelete(userID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[UsersView] WHERE ([UserID] = @UserID);";
        deleteCommand.Parameters.Add("UserID", SqlDbType.Int);
        deleteCommand.Parameters["UserID"].Value = userID;

        BeforeRowDelete(userID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(userID);
      }
      AfterDBDelete(userID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("UsersViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[UsersView] SET     [Email] = @Email,    [FirstName] = @FirstName,    [MiddleName] = @MiddleName,    [LastName] = @LastName,    [Title] = @Title,    [IsActive] = @IsActive,    [MarkDeleted] = @MarkDeleted,    [LastLogin] = @LastLogin,    [LastActivity] = @LastActivity,    [LastPing] = @LastPing,    [IsSystemAdmin] = @IsSystemAdmin,    [IsFinanceAdmin] = @IsFinanceAdmin,    [IsPasswordExpired] = @IsPasswordExpired,    [IsPortalUser] = @IsPortalUser,    [PrimaryGroupID] = @PrimaryGroupID,    [InOffice] = @InOffice,    [InOfficeComment] = @InOfficeComment,    [ActivatedOn] = @ActivatedOn,    [DeactivatedOn] = @DeactivatedOn,    [OrganizationID] = @OrganizationID,    [Organization] = @Organization,    [LastVersion] = @LastVersion,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [IsOnline] = @IsOnline,    [CryptedPassword] = @CryptedPassword,    [IsChatUser] = @IsChatUser  WHERE ([UserID] = @UserID);";

		
		tempParameter = updateCommand.Parameters.Add("Email", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("FirstName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("MiddleName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Title", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("MarkDeleted", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastLogin", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastActivity", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastPing", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsSystemAdmin", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsFinanceAdmin", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsPasswordExpired", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsPortalUser", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PrimaryGroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("InOffice", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("InOfficeComment", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ActivatedOn", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("DeactivatedOn", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Organization", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastVersion", SqlDbType.VarChar, 50);
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
		
		tempParameter = updateCommand.Parameters.Add("IsOnline", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CryptedPassword", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsChatUser", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[UsersView] (    [Email],    [FirstName],    [UserID],    [MiddleName],    [LastName],    [Title],    [IsActive],    [MarkDeleted],    [LastLogin],    [LastActivity],    [LastPing],    [IsSystemAdmin],    [IsFinanceAdmin],    [IsPasswordExpired],    [IsPortalUser],    [PrimaryGroupID],    [InOffice],    [InOfficeComment],    [ActivatedOn],    [DeactivatedOn],    [OrganizationID],    [Organization],    [LastVersion],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [IsOnline],    [CryptedPassword],    [IsChatUser]) VALUES ( @Email, @FirstName, @UserID, @MiddleName, @LastName, @Title, @IsActive, @MarkDeleted, @LastLogin, @LastActivity, @LastPing, @IsSystemAdmin, @IsFinanceAdmin, @IsPasswordExpired, @IsPortalUser, @PrimaryGroupID, @InOffice, @InOfficeComment, @ActivatedOn, @DeactivatedOn, @OrganizationID, @Organization, @LastVersion, @DateCreated, @DateModified, @CreatorID, @ModifierID, @IsOnline, @CryptedPassword, @IsChatUser); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("IsChatUser", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CryptedPassword", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsOnline", SqlDbType.Bit, 1);
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
		
		tempParameter = insertCommand.Parameters.Add("LastVersion", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Organization", SqlDbType.VarChar, 255);
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
		
		tempParameter = insertCommand.Parameters.Add("DeactivatedOn", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("ActivatedOn", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("InOfficeComment", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("InOffice", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PrimaryGroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsPortalUser", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsPasswordExpired", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsFinanceAdmin", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsSystemAdmin", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastPing", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastActivity", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastLogin", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("MarkDeleted", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Title", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("MiddleName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("FirstName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Email", SqlDbType.VarChar, 1024);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[UsersView] WHERE ([UserID] = @UserID);";
		deleteCommand.Parameters.Add("UserID", SqlDbType.Int);

		try
		{
		  foreach (UsersViewItem usersViewItem in this)
		  {
			if (usersViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(usersViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = usersViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["UserID"].AutoIncrement = false;
			  Table.Columns["UserID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				usersViewItem.Row["UserID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(usersViewItem);
			}
			else if (usersViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(usersViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = usersViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(usersViewItem);
			}
			else if (usersViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)usersViewItem.Row["UserID", DataRowVersion.Original];
			  deleteCommand.Parameters["UserID"].Value = id;
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

      foreach (UsersViewItem usersViewItem in this)
      {
        if (usersViewItem.Row.Table.Columns.Contains("CreatorID") && (int)usersViewItem.Row["CreatorID"] == 0) usersViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (usersViewItem.Row.Table.Columns.Contains("ModifierID")) usersViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public UsersViewItem FindByUserID(int userID)
    {
      foreach (UsersViewItem usersViewItem in this)
      {
        if (usersViewItem.UserID == userID)
        {
          return usersViewItem;
        }
      }
      return null;
    }

    public virtual UsersViewItem AddNewUsersViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("UsersView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new UsersViewItem(row, this);
    }
    
    public virtual void LoadByUserID(int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [Email], [FirstName], [UserID], [MiddleName], [LastName], [Title], [IsActive], [MarkDeleted], [LastLogin], [LastActivity], [LastPing], [IsSystemAdmin], [IsFinanceAdmin], [IsPasswordExpired], [IsPortalUser], [PrimaryGroupID], [InOffice], [InOfficeComment], [ActivatedOn], [DeactivatedOn], [OrganizationID], [Organization], [LastVersion], [DateCreated], [DateModified], [CreatorID], [ModifierID], [IsOnline], [CryptedPassword], [IsChatUser] FROM [dbo].[UsersView] WHERE ([UserID] = @UserID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("UserID", userID);
        Fill(command);
      }
    }
    
    public static UsersViewItem GetUsersViewItem(LoginUser loginUser, int userID)
    {
      UsersView usersView = new UsersView(loginUser);
      usersView.LoadByUserID(userID);
      if (usersView.IsEmpty)
        return null;
      else
        return usersView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<UsersViewItem> Members

    public IEnumerator<UsersViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new UsersViewItem(row, this);
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
