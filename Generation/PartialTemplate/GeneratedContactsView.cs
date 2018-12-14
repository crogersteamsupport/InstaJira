using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ContactsViewItem : BaseItem
  {
    private ContactsView _contactsView;
    
    public ContactsViewItem(DataRow row, ContactsView contactsView): base(row, contactsView)
    {
      _contactsView = contactsView;
    }
	
    #region Properties
    
    public ContactsView Collection
    {
      get { return _contactsView; }
    }
        
    
    
    
    public string Name
    {
      get { return (string)Row["Name"]; }
    }
    

    
    public string Title
    {
      get { return Row["Title"] != DBNull.Value ? (string)Row["Title"] : null; }
      set { Row["Title"] = CheckValue("Title", value); }
    }
    
    public int? PrimaryGroupID
    {
      get { return Row["PrimaryGroupID"] != DBNull.Value ? (int?)Row["PrimaryGroupID"] : null; }
      set { Row["PrimaryGroupID"] = CheckValue("PrimaryGroupID", value); }
    }
    
    public string Organization
    {
      get { return Row["Organization"] != DBNull.Value ? (string)Row["Organization"] : null; }
      set { Row["Organization"] = CheckValue("Organization", value); }
    }
    
    public string LastVersion
    {
      get { return Row["LastVersion"] != DBNull.Value ? (string)Row["LastVersion"] : null; }
      set { Row["LastVersion"] = CheckValue("LastVersion", value); }
    }
    
    public int? OrganizationParentID
    {
      get { return Row["OrganizationParentID"] != DBNull.Value ? (int?)Row["OrganizationParentID"] : null; }
      set { Row["OrganizationParentID"] = CheckValue("OrganizationParentID", value); }
    }
    
    public string SalesForceID
    {
      get { return Row["SalesForceID"] != DBNull.Value ? (string)Row["SalesForceID"] : null; }
      set { Row["SalesForceID"] = CheckValue("SalesForceID", value); }
    }
    
    public bool? OrganizationActive
    {
      get { return Row["OrganizationActive"] != DBNull.Value ? (bool?)Row["OrganizationActive"] : null; }
      set { Row["OrganizationActive"] = CheckValue("OrganizationActive", value); }
    }
    

    
    public bool BlockInboundEmail
    {
      get { return (bool)Row["BlockInboundEmail"]; }
      set { Row["BlockInboundEmail"] = CheckValue("BlockInboundEmail", value); }
    }
    
    public bool PortalViewOnly
    {
      get { return (bool)Row["PortalViewOnly"]; }
      set { Row["PortalViewOnly"] = CheckValue("PortalViewOnly", value); }
    }
    
    public string LinkedIn
    {
      get { return (string)Row["LinkedIn"]; }
      set { Row["LinkedIn"] = CheckValue("LinkedIn", value); }
    }
    
    public bool PortalLimitOrgTickets
    {
      get { return (bool)Row["PortalLimitOrgTickets"]; }
      set { Row["PortalLimitOrgTickets"] = CheckValue("PortalLimitOrgTickets", value); }
    }
    
    public bool NeedsIndexing
    {
      get { return (bool)Row["NeedsIndexing"]; }
      set { Row["NeedsIndexing"] = CheckValue("NeedsIndexing", value); }
    }
    
    public string CryptedPassword
    {
      get { return (string)Row["CryptedPassword"]; }
      set { Row["CryptedPassword"] = CheckValue("CryptedPassword", value); }
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
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public string InOfficeComment
    {
      get { return (string)Row["InOfficeComment"]; }
      set { Row["InOfficeComment"] = CheckValue("InOfficeComment", value); }
    }
    
    public bool InOffice
    {
      get { return (bool)Row["InOffice"]; }
      set { Row["InOffice"] = CheckValue("InOffice", value); }
    }
    
    public bool IsPortalUser
    {
      get { return (bool)Row["IsPortalUser"]; }
      set { Row["IsPortalUser"] = CheckValue("IsPortalUser", value); }
    }
    
    public bool IsPasswordExpired
    {
      get { return (bool)Row["IsPasswordExpired"]; }
      set { Row["IsPasswordExpired"] = CheckValue("IsPasswordExpired", value); }
    }
    
    public bool IsFinanceAdmin
    {
      get { return (bool)Row["IsFinanceAdmin"]; }
      set { Row["IsFinanceAdmin"] = CheckValue("IsFinanceAdmin", value); }
    }
    
    public bool IsSystemAdmin
    {
      get { return (bool)Row["IsSystemAdmin"]; }
      set { Row["IsSystemAdmin"] = CheckValue("IsSystemAdmin", value); }
    }
    
    public bool MarkDeleted
    {
      get { return (bool)Row["MarkDeleted"]; }
      set { Row["MarkDeleted"] = CheckValue("MarkDeleted", value); }
    }
    
    public bool IsActive
    {
      get { return (bool)Row["IsActive"]; }
      set { Row["IsActive"] = CheckValue("IsActive", value); }
    }
    
    public string LastName
    {
      get { return (string)Row["LastName"]; }
      set { Row["LastName"] = CheckValue("LastName", value); }
    }
    
    public string MiddleName
    {
      get { return (string)Row["MiddleName"]; }
      set { Row["MiddleName"] = CheckValue("MiddleName", value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    
    public string FirstName
    {
      get { return (string)Row["FirstName"]; }
      set { Row["FirstName"] = CheckValue("FirstName", value); }
    }
    
    public string Email
    {
      get { return (string)Row["Email"]; }
      set { Row["Email"] = CheckValue("Email", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? LastPing
    {
      get { return Row["LastPing"] != DBNull.Value ? DateToLocal((DateTime?)Row["LastPing"]) : null; }
      set { Row["LastPing"] = CheckValue("LastPing", value); }
    }

    public DateTime? LastPingUtc
    {
      get { return Row["LastPing"] != DBNull.Value ? (DateTime?)Row["LastPing"] : null; }
    }
    
    public DateTime? DeactivatedOn
    {
      get { return Row["DeactivatedOn"] != DBNull.Value ? DateToLocal((DateTime?)Row["DeactivatedOn"]) : null; }
      set { Row["DeactivatedOn"] = CheckValue("DeactivatedOn", value); }
    }

    public DateTime? DeactivatedOnUtc
    {
      get { return Row["DeactivatedOn"] != DBNull.Value ? (DateTime?)Row["DeactivatedOn"] : null; }
    }
    
    public DateTime? OrganizationSAExpirationDate
    {
      get { return Row["OrganizationSAExpirationDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["OrganizationSAExpirationDate"]) : null; }
      set { Row["OrganizationSAExpirationDate"] = CheckValue("OrganizationSAExpirationDate", value); }
    }

    public DateTime? OrganizationSAExpirationDateUtc
    {
      get { return Row["OrganizationSAExpirationDate"] != DBNull.Value ? (DateTime?)Row["OrganizationSAExpirationDate"] : null; }
    }
    

    
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
    
    public DateTime ActivatedOn
    {
      get { return DateToLocal((DateTime)Row["ActivatedOn"]); }
      set { Row["ActivatedOn"] = CheckValue("ActivatedOn", value); }
    }

    public DateTime ActivatedOnUtc
    {
      get { return (DateTime)Row["ActivatedOn"]; }
    }
    
    public DateTime LastActivity
    {
      get { return DateToLocal((DateTime)Row["LastActivity"]); }
      set { Row["LastActivity"] = CheckValue("LastActivity", value); }
    }

    public DateTime LastActivityUtc
    {
      get { return (DateTime)Row["LastActivity"]; }
    }
    
    public DateTime LastLogin
    {
      get { return DateToLocal((DateTime)Row["LastLogin"]); }
      set { Row["LastLogin"] = CheckValue("LastLogin", value); }
    }

    public DateTime LastLoginUtc
    {
      get { return (DateTime)Row["LastLogin"]; }
    }
    

    #endregion
    
    
  }

  public partial class ContactsView : BaseCollection, IEnumerable<ContactsViewItem>
  {
    public ContactsView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ContactsView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "UserID"; }
    }



    public ContactsViewItem this[int index]
    {
      get { return new ContactsViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ContactsViewItem contactsViewItem);
    partial void AfterRowInsert(ContactsViewItem contactsViewItem);
    partial void BeforeRowEdit(ContactsViewItem contactsViewItem);
    partial void AfterRowEdit(ContactsViewItem contactsViewItem);
    partial void BeforeRowDelete(int userID);
    partial void AfterRowDelete(int userID);    

    partial void BeforeDBDelete(int userID);
    partial void AfterDBDelete(int userID);    

    #endregion

    #region Public Methods

    public ContactsViewItemProxy[] GetContactsViewItemProxies()
    {
      List<ContactsViewItemProxy> list = new List<ContactsViewItemProxy>();

      foreach (ContactsViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int userID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ContactsView] WHERE ([UserID] = @UserID);";
        deleteCommand.Parameters.Add("UserID", SqlDbType.Int);
        deleteCommand.Parameters["UserID"].Value = userID;

        BeforeDBDelete(userID);
        BeforeRowDelete(userID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(userID);
        AfterDBDelete(userID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ContactsViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ContactsView] SET     [Email] = @Email,    [FirstName] = @FirstName,    [Name] = @Name,    [MiddleName] = @MiddleName,    [LastName] = @LastName,    [Title] = @Title,    [IsActive] = @IsActive,    [MarkDeleted] = @MarkDeleted,    [LastLogin] = @LastLogin,    [LastActivity] = @LastActivity,    [LastPing] = @LastPing,    [IsSystemAdmin] = @IsSystemAdmin,    [IsFinanceAdmin] = @IsFinanceAdmin,    [IsPasswordExpired] = @IsPasswordExpired,    [IsPortalUser] = @IsPortalUser,    [PrimaryGroupID] = @PrimaryGroupID,    [InOffice] = @InOffice,    [InOfficeComment] = @InOfficeComment,    [ActivatedOn] = @ActivatedOn,    [DeactivatedOn] = @DeactivatedOn,    [OrganizationID] = @OrganizationID,    [Organization] = @Organization,    [LastVersion] = @LastVersion,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [OrganizationParentID] = @OrganizationParentID,    [CryptedPassword] = @CryptedPassword,    [SalesForceID] = @SalesForceID,    [NeedsIndexing] = @NeedsIndexing,    [OrganizationActive] = @OrganizationActive,    [OrganizationSAExpirationDate] = @OrganizationSAExpirationDate,    [PortalLimitOrgTickets] = @PortalLimitOrgTickets,    [LinkedIn] = @LinkedIn,    [PortalViewOnly] = @PortalViewOnly  WHERE ([UserID] = @UserID);";

		
		tempParameter = updateCommand.Parameters.Add("Email", SqlDbType.NVarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("FirstName", SqlDbType.NVarChar, 100);
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
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("MiddleName", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastName", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Title", SqlDbType.NVarChar, 100);
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
		
		tempParameter = updateCommand.Parameters.Add("Organization", SqlDbType.NVarChar, 255);
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
		
		tempParameter = updateCommand.Parameters.Add("OrganizationParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CryptedPassword", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SalesForceID", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("NeedsIndexing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationSAExpirationDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("PortalLimitOrgTickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LinkedIn", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PortalViewOnly", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("BlockInboundEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ContactsView] (    [Email],    [FirstName],    [UserID],    [Name],    [MiddleName],    [LastName],    [Title],    [IsActive],    [MarkDeleted],    [LastLogin],    [LastActivity],    [LastPing],    [IsSystemAdmin],    [IsFinanceAdmin],    [IsPasswordExpired],    [IsPortalUser],    [PrimaryGroupID],    [InOffice],    [InOfficeComment],    [ActivatedOn],    [DeactivatedOn],    [OrganizationID],    [Organization],    [LastVersion],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [OrganizationParentID],    [CryptedPassword],    [SalesForceID],    [NeedsIndexing],    [OrganizationActive],    [OrganizationSAExpirationDate],    [PortalLimitOrgTickets],    [LinkedIn],    [PortalViewOnly],    [BlockInboundEmail]) VALUES ( @Email, @FirstName, @UserID, @Name, @MiddleName, @LastName, @Title, @IsActive, @MarkDeleted, @LastLogin, @LastActivity, @LastPing, @IsSystemAdmin, @IsFinanceAdmin, @IsPasswordExpired, @IsPortalUser, @PrimaryGroupID, @InOffice, @InOfficeComment, @ActivatedOn, @DeactivatedOn, @OrganizationID, @Organization, @LastVersion, @DateCreated, @DateModified, @CreatorID, @ModifierID, @OrganizationParentID, @CryptedPassword, @SalesForceID, @NeedsIndexing, @OrganizationActive, @OrganizationSAExpirationDate, @PortalLimitOrgTickets, @LinkedIn, @PortalViewOnly, @BlockInboundEmail); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("BlockInboundEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PortalViewOnly", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LinkedIn", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PortalLimitOrgTickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationSAExpirationDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("NeedsIndexing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SalesForceID", SqlDbType.VarChar, 8000);
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
		
		tempParameter = insertCommand.Parameters.Add("OrganizationParentID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("LastVersion", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Organization", SqlDbType.NVarChar, 255);
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
		
		tempParameter = insertCommand.Parameters.Add("Title", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastName", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("MiddleName", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.NVarChar, 201);
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
		
		tempParameter = insertCommand.Parameters.Add("FirstName", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Email", SqlDbType.NVarChar, 1024);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ContactsView] WHERE ([UserID] = @UserID);";
		deleteCommand.Parameters.Add("UserID", SqlDbType.Int);

		try
		{
		  foreach (ContactsViewItem contactsViewItem in this)
		  {
			if (contactsViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(contactsViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = contactsViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["UserID"].AutoIncrement = false;
			  Table.Columns["UserID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				contactsViewItem.Row["UserID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(contactsViewItem);
			}
			else if (contactsViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(contactsViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = contactsViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(contactsViewItem);
			}
			else if (contactsViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)contactsViewItem.Row["UserID", DataRowVersion.Original];
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

      foreach (ContactsViewItem contactsViewItem in this)
      {
        if (contactsViewItem.Row.Table.Columns.Contains("CreatorID") && (int)contactsViewItem.Row["CreatorID"] == 0) contactsViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (contactsViewItem.Row.Table.Columns.Contains("ModifierID")) contactsViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ContactsViewItem FindByUserID(int userID)
    {
      foreach (ContactsViewItem contactsViewItem in this)
      {
        if (contactsViewItem.UserID == userID)
        {
          return contactsViewItem;
        }
      }
      return null;
    }

    public virtual ContactsViewItem AddNewContactsViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("ContactsView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ContactsViewItem(row, this);
    }
    
    public virtual void LoadByUserID(int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [Email], [FirstName], [UserID], [Name], [MiddleName], [LastName], [Title], [IsActive], [MarkDeleted], [LastLogin], [LastActivity], [LastPing], [IsSystemAdmin], [IsFinanceAdmin], [IsPasswordExpired], [IsPortalUser], [PrimaryGroupID], [InOffice], [InOfficeComment], [ActivatedOn], [DeactivatedOn], [OrganizationID], [Organization], [LastVersion], [DateCreated], [DateModified], [CreatorID], [ModifierID], [OrganizationParentID], [CryptedPassword], [SalesForceID], [NeedsIndexing], [OrganizationActive], [OrganizationSAExpirationDate], [PortalLimitOrgTickets], [LinkedIn], [PortalViewOnly], [BlockInboundEmail] FROM [dbo].[ContactsView] WHERE ([UserID] = @UserID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("UserID", userID);
        Fill(command);
      }
    }
    
    public static ContactsViewItem GetContactsViewItem(LoginUser loginUser, int userID)
    {
      ContactsView contactsView = new ContactsView(loginUser);
      contactsView.LoadByUserID(userID);
      if (contactsView.IsEmpty)
        return null;
      else
        return contactsView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ContactsViewItem> Members

    public IEnumerator<ContactsViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ContactsViewItem(row, this);
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
