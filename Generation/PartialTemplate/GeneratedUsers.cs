using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class User : BaseItem
  {
    private Users _users;

    public User(DataRow row, Users users): base(row, users)
    {
      _users = users;
    }

    #region Properties

    public Users Collection
    {
      get { return _users; }
    }




    public int UserID
    {
      get { return (int)Row["UserID"]; }
    }



    public string MiddleName
    {
      get { return Row["MiddleName"] != DBNull.Value ? (string)Row["MiddleName"] : null; }
      set { Row["MiddleName"] = CheckValue("MiddleName", value); }
    }

    public string Title
    {
      get { return Row["Title"] != DBNull.Value ? (string)Row["Title"] : null; }
      set { Row["Title"] = CheckValue("Title", value); }
    }

    public string TimeZoneID
    {
      get { return Row["TimeZoneID"] != DBNull.Value ? (string)Row["TimeZoneID"] : null; }
      set { Row["TimeZoneID"] = CheckValue("TimeZoneID", value); }
    }

    public string CultureName
    {
      get { return Row["CultureName"] != DBNull.Value ? (string)Row["CultureName"] : null; }
      set { Row["CultureName"] = CheckValue("CultureName", value); }
    }

    public int? PrimaryGroupID
    {
      get { return Row["PrimaryGroupID"] != DBNull.Value ? (int?)Row["PrimaryGroupID"] : null; }
      set { Row["PrimaryGroupID"] = CheckValue("PrimaryGroupID", value); }
    }

    public string LastVersion
    {
      get { return Row["LastVersion"] != DBNull.Value ? (string)Row["LastVersion"] : null; }
      set { Row["LastVersion"] = CheckValue("LastVersion", value); }
    }

    public Guid? SessionID
    {
      get { return Row["SessionID"] != DBNull.Value ? (Guid?)Row["SessionID"] : null; }
      set { Row["SessionID"] = CheckValue("SessionID", value); }
    }

    public string ImportID
    {
      get { return Row["ImportID"] != DBNull.Value ? (string)Row["ImportID"] : null; }
      set { Row["ImportID"] = CheckValue("ImportID", value); }
    }

    public string OrgsUserCanSeeOnPortal
    {
      get { return Row["OrgsUserCanSeeOnPortal"] != DBNull.Value ? (string)Row["OrgsUserCanSeeOnPortal"] : null; }
      set { Row["OrgsUserCanSeeOnPortal"] = CheckValue("OrgsUserCanSeeOnPortal", value); }
    }

    public string AppChatID
    {
      get { return Row["AppChatID"] != DBNull.Value ? (string)Row["AppChatID"] : null; }
      set { Row["AppChatID"] = CheckValue("AppChatID", value); }
    }

    public string MenuItems
    {
      get { return Row["MenuItems"] != DBNull.Value ? (string)Row["MenuItems"] : null; }
      set { Row["MenuItems"] = CheckValue("MenuItems", value); }
    }

    public string SalesForceID
    {
      get { return Row["SalesForceID"] != DBNull.Value ? (string)Row["SalesForceID"] : null; }
      set { Row["SalesForceID"] = CheckValue("SalesForceID", value); }
    }

    public bool? BlockEmailFromCreatingOnly
    {
      get { return Row["BlockEmailFromCreatingOnly"] != DBNull.Value ? (bool?)Row["BlockEmailFromCreatingOnly"] : null; }
      set { Row["BlockEmailFromCreatingOnly"] = CheckValue("BlockEmailFromCreatingOnly", value); }
    }

    public string verificationPhoneNumber
    {
      get { return Row["verificationPhoneNumber"] != DBNull.Value ? (string)Row["verificationPhoneNumber"] : null; }
      set { Row["verificationPhoneNumber"] = CheckValue("verificationPhoneNumber", value); }
    }

    public string verificationCode
    {
      get { return Row["verificationCode"] != DBNull.Value ? (string)Row["verificationCode"] : null; }
      set { Row["verificationCode"] = CheckValue("verificationCode", value); }
    }

    public int? ImportFileID
    {
      get { return Row["ImportFileID"] != DBNull.Value ? (int?)Row["ImportFileID"] : null; }
      set { Row["ImportFileID"] = CheckValue("ImportFileID", value); }
    }



    public bool PortalLimitOrgChildrenTickets
    {
      get { return (bool)Row["PortalLimitOrgChildrenTickets"]; }
      set { Row["PortalLimitOrgChildrenTickets"] = CheckValue("PortalLimitOrgChildrenTickets", value); }
    }

    public bool PortalViewOnly
    {
      get { return (bool)Row["PortalViewOnly"]; }
      set { Row["PortalViewOnly"] = CheckValue("PortalViewOnly", value); }
    }

    public Guid CalGUID
    {
      get { return (Guid)Row["CalGUID"]; }
      set { Row["CalGUID"] = CheckValue("CalGUID", value); }
    }

    public int ProductFamiliesRights
    {
      get { return (int)Row["ProductFamiliesRights"]; }
      set { Row["ProductFamiliesRights"] = CheckValue("ProductFamiliesRights", value); }
    }

    public bool ReceiveUnassignedGroupEmails
    {
      get { return (bool)Row["ReceiveUnassignedGroupEmails"]; }
      set { Row["ReceiveUnassignedGroupEmails"] = CheckValue("ReceiveUnassignedGroupEmails", value); }
    }

    public bool CanEditVersions
    {
      get { return (bool)Row["CanEditVersions"]; }
      set { Row["CanEditVersions"] = CheckValue("CanEditVersions", value); }
    }

    public bool CanCreateVersions
    {
      get { return (bool)Row["CanCreateVersions"]; }
      set { Row["CanCreateVersions"] = CheckValue("CanCreateVersions", value); }
    }

    public bool CanEditProducts
    {
      get { return (bool)Row["CanEditProducts"]; }
      set { Row["CanEditProducts"] = CheckValue("CanEditProducts", value); }
    }

    public bool CanCreateProducts
    {
      get { return (bool)Row["CanCreateProducts"]; }
      set { Row["CanCreateProducts"] = CheckValue("CanCreateProducts", value); }
    }

    public bool DisableExporting
    {
      get { return (bool)Row["DisableExporting"]; }
      set { Row["DisableExporting"] = CheckValue("DisableExporting", value); }
    }

    public bool DisablePublic
    {
      get { return (bool)Row["DisablePublic"]; }
      set { Row["DisablePublic"] = CheckValue("DisablePublic", value); }
    }

    public bool FilterInactive
    {
      get { return (bool)Row["FilterInactive"]; }
      set { Row["FilterInactive"] = CheckValue("FilterInactive", value); }
    }

    public bool CanChangeCommunityVisibility
    {
      get { return (bool)Row["CanChangeCommunityVisibility"]; }
      set { Row["CanChangeCommunityVisibility"] = CheckValue("CanChangeCommunityVisibility", value); }
    }

    public bool CanEditAsset
    {
      get { return (bool)Row["CanEditAsset"]; }
      set { Row["CanEditAsset"] = CheckValue("CanEditAsset", value); }
    }

    public bool CanCreateAsset
    {
      get { return (bool)Row["CanCreateAsset"]; }
      set { Row["CanCreateAsset"] = CheckValue("CanCreateAsset", value); }
    }

    public bool PortalLimitOrgTickets
    {
      get { return (bool)Row["PortalLimitOrgTickets"]; }
      set { Row["PortalLimitOrgTickets"] = CheckValue("PortalLimitOrgTickets", value); }
    }

    public bool UserCanPinAction
    {
      get { return (bool)Row["UserCanPinAction"]; }
      set { Row["UserCanPinAction"] = CheckValue("UserCanPinAction", value); }
    }

    public bool AllowUserToEditAnyAction
    {
      get { return (bool)Row["AllowUserToEditAnyAction"]; }
      set { Row["AllowUserToEditAnyAction"] = CheckValue("AllowUserToEditAnyAction", value); }
    }

    public bool RestrictUserFromEditingAnyActions
    {
      get { return (bool)Row["RestrictUserFromEditingAnyActions"]; }
      set { Row["RestrictUserFromEditingAnyActions"] = CheckValue("RestrictUserFromEditingAnyActions", value); }
    }

    public bool CanEditContact
    {
      get { return (bool)Row["CanEditContact"]; }
      set { Row["CanEditContact"] = CheckValue("CanEditContact", value); }
    }

    public bool CanCreateContact
    {
      get { return (bool)Row["CanCreateContact"]; }
      set { Row["CanCreateContact"] = CheckValue("CanCreateContact", value); }
    }

    public bool CanEditCompany
    {
      get { return (bool)Row["CanEditCompany"]; }
      set { Row["CanEditCompany"] = CheckValue("CanEditCompany", value); }
    }

    public bool CanCreateCompany
    {
      get { return (bool)Row["CanCreateCompany"]; }
      set { Row["CanCreateCompany"] = CheckValue("CanCreateCompany", value); }
    }

    public FontSize FontSize
    {
      get { return (FontSize)Row["FontSize"]; }
      set { Row["FontSize"] = CheckValue("FontSize", value); }
    }

    public FontFamily FontFamily
    {
      get { return (FontFamily)Row["FontFamily"]; }
      set { Row["FontFamily"] = CheckValue("FontFamily", value); }
    }

    public bool AllowAnyTicketCustomer
    {
      get { return (bool)Row["AllowAnyTicketCustomer"]; }
      set { Row["AllowAnyTicketCustomer"] = CheckValue("AllowAnyTicketCustomer", value); }
    }

    public bool NeedsIndexing
    {
      get { return (bool)Row["NeedsIndexing"]; }
      set { Row["NeedsIndexing"] = CheckValue("NeedsIndexing", value); }
    }

    public bool EnforceSingleSession
    {
      get { return (bool)Row["EnforceSingleSession"]; }
      set { Row["EnforceSingleSession"] = CheckValue("EnforceSingleSession", value); }
    }

    public bool ChangeKBVisibility
    {
      get { return (bool)Row["ChangeKBVisibility"]; }
      set { Row["ChangeKBVisibility"] = CheckValue("ChangeKBVisibility", value); }
    }

    public bool ChangeTicketVisibility
    {
      get { return (bool)Row["ChangeTicketVisibility"]; }
      set { Row["ChangeTicketVisibility"] = CheckValue("ChangeTicketVisibility", value); }
    }

    public bool BlockInboundEmail
    {
      get { return (bool)Row["BlockInboundEmail"]; }
      set { Row["BlockInboundEmail"] = CheckValue("BlockInboundEmail", value); }
    }

    public bool OnlyEmailAfterHours
    {
      get { return (bool)Row["OnlyEmailAfterHours"]; }
      set { Row["OnlyEmailAfterHours"] = CheckValue("OnlyEmailAfterHours", value); }
    }

    public string LinkedIn
    {
      get { return (string)Row["LinkedIn"]; }
      set { Row["LinkedIn"] = CheckValue("LinkedIn", value); }
    }

    public string Signature
    {
      get { return (string)Row["Signature"]; }
      set { Row["Signature"] = CheckValue("Signature", value); }
    }

    public TicketRightType TicketRights
    {
      get { return (TicketRightType)Row["TicketRights"]; }
      set { Row["TicketRights"] = CheckValue("TicketRights", value); }
    }

    public bool AppChatStatus
    {
      get { return (bool)Row["AppChatStatus"]; }
      set { Row["AppChatStatus"] = CheckValue("AppChatStatus", value); }
    }

    public bool PortalAutoReg
    {
      get { return (bool)Row["PortalAutoReg"]; }
      set { Row["PortalAutoReg"] = CheckValue("PortalAutoReg", value); }
    }

    public string UserInformation
    {
      get { return (string)Row["UserInformation"]; }
      set { Row["UserInformation"] = CheckValue("UserInformation", value); }
    }

    public bool ShowWelcomePage
    {
      get { return (bool)Row["ShowWelcomePage"]; }
      set { Row["ShowWelcomePage"] = CheckValue("ShowWelcomePage", value); }
    }

    public bool ApprovedTerms
    {
      get { return (bool)Row["ApprovedTerms"]; }
      set { Row["ApprovedTerms"] = CheckValue("ApprovedTerms", value); }
    }

    public bool SubscribeToNewActions
    {
      get { return (bool)Row["SubscribeToNewActions"]; }
      set { Row["SubscribeToNewActions"] = CheckValue("SubscribeToNewActions", value); }
    }

    public bool IsClassicView
    {
      get { return (bool)Row["IsClassicView"]; }
      set { Row["IsClassicView"] = CheckValue("IsClassicView", value); }
    }

    public bool DoNotAutoSubscribe
    {
      get { return (bool)Row["DoNotAutoSubscribe"]; }
      set { Row["DoNotAutoSubscribe"] = CheckValue("DoNotAutoSubscribe", value); }
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

    public bool SubscribeToNewTickets
    {
      get { return (bool)Row["SubscribeToNewTickets"]; }
      set { Row["SubscribeToNewTickets"] = CheckValue("SubscribeToNewTickets", value); }
    }

    public bool ReceiveAllGroupNotifications
    {
      get { return (bool)Row["ReceiveAllGroupNotifications"]; }
      set { Row["ReceiveAllGroupNotifications"] = CheckValue("ReceiveAllGroupNotifications", value); }
    }

    public bool ReceiveTicketNotifications
    {
      get { return (bool)Row["ReceiveTicketNotifications"]; }
      set { Row["ReceiveTicketNotifications"] = CheckValue("ReceiveTicketNotifications", value); }
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

    public bool IsChatUser
    {
      get { return (bool)Row["IsChatUser"]; }
      set { Row["IsChatUser"] = CheckValue("IsChatUser", value); }
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

    public int LastWaterCoolerID
    {
      get { return (int)Row["LastWaterCoolerID"]; }
      set { Row["LastWaterCoolerID"] = CheckValue("LastWaterCoolerID", value); }
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

    public string CryptedPassword
    {
      get { return (string)Row["CryptedPassword"]; }
      set { Row["CryptedPassword"] = CheckValue("CryptedPassword", value); }
    }

    public string LastName
    {
      get { return (string)Row["LastName"]; }
      set { Row["LastName"] = CheckValue("LastName", value); }
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

    public DateTime? verificationCodeExpiration
    {
      get { return Row["verificationCodeExpiration"] != DBNull.Value ? DateToLocal((DateTime?)Row["verificationCodeExpiration"]) : null; }
      set { Row["verificationCodeExpiration"] = CheckValue("verificationCodeExpiration", value); }
    }

    public DateTime? verificationCodeExpirationUtc
    {
      get { return Row["verificationCodeExpiration"] != DBNull.Value ? (DateTime?)Row["verificationCodeExpiration"] : null; }
    }

    public DateTime? PasswordCreatedUtc
    {
      get { return Row["PasswordCreatedUtc"] != DBNull.Value ? DateToLocal((DateTime?)Row["PasswordCreatedUtc"]) : null; }
      set { Row["PasswordCreatedUtc"] = CheckValue("PasswordCreatedUtc", value); }
    }

    public DateTime? PasswordCreatedUtcUtc
    {
      get { return Row["PasswordCreatedUtc"] != DBNull.Value ? (DateTime?)Row["PasswordCreatedUtc"] : null; }
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

  public partial class Users : BaseCollection, IEnumerable<User>
  {
    public Users(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Users"; }
    }

    public override string PrimaryKeyFieldName
    {
      get { return "UserID"; }
    }



    public User this[int index]
    {
      get { return new User(Table.Rows[index], this); }
    }


    #endregion

    #region Protected Members

    partial void BeforeRowInsert(User user);
    partial void AfterRowInsert(User user);
    partial void BeforeRowEdit(User user);
    partial void AfterRowEdit(User user);
    partial void BeforeRowDelete(int userID);
    partial void AfterRowDelete(int userID);

    partial void BeforeDBDelete(int userID);
    partial void AfterDBDelete(int userID);

    #endregion

    #region Public Methods

    public UserProxy[] GetUserProxies()
    {
      List<UserProxy> list = new List<UserProxy>();

      foreach (User item in this)
      {
        list.Add(item.GetProxy());
      }

      return list.ToArray();
    }

    public virtual void DeleteFromDB(int userID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Users] WHERE ([UserID] = @UserID);";
        deleteCommand.Parameters.Add("UserID", SqlDbType.Int);
        deleteCommand.Parameters["UserID"].Value = userID;

        BeforeDBDelete(userID);
        BeforeRowDelete(userID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(userID);
        AfterDBDelete(userID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("UsersSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Users] SET     [Email] = @Email,    [FirstName] = @FirstName,    [MiddleName] = @MiddleName,    [LastName] = @LastName,    [Title] = @Title,    [CryptedPassword] = @CryptedPassword,    [IsActive] = @IsActive,    [MarkDeleted] = @MarkDeleted,    [TimeZoneID] = @TimeZoneID,    [CultureName] = @CultureName,    [LastLogin] = @LastLogin,    [LastActivity] = @LastActivity,    [LastPing] = @LastPing,    [LastWaterCoolerID] = @LastWaterCoolerID,    [IsSystemAdmin] = @IsSystemAdmin,    [IsFinanceAdmin] = @IsFinanceAdmin,    [IsPasswordExpired] = @IsPasswordExpired,    [IsPortalUser] = @IsPortalUser,    [IsChatUser] = @IsChatUser,    [PrimaryGroupID] = @PrimaryGroupID,    [InOffice] = @InOffice,    [InOfficeComment] = @InOfficeComment,    [ReceiveTicketNotifications] = @ReceiveTicketNotifications,    [ReceiveAllGroupNotifications] = @ReceiveAllGroupNotifications,    [SubscribeToNewTickets] = @SubscribeToNewTickets,    [ActivatedOn] = @ActivatedOn,    [DeactivatedOn] = @DeactivatedOn,    [OrganizationID] = @OrganizationID,    [LastVersion] = @LastVersion,    [SessionID] = @SessionID,    [ImportID] = @ImportID,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [OrgsUserCanSeeOnPortal] = @OrgsUserCanSeeOnPortal,    [DoNotAutoSubscribe] = @DoNotAutoSubscribe,    [IsClassicView] = @IsClassicView,    [SubscribeToNewActions] = @SubscribeToNewActions,    [ApprovedTerms] = @ApprovedTerms,    [ShowWelcomePage] = @ShowWelcomePage,    [UserInformation] = @UserInformation,    [PortalAutoReg] = @PortalAutoReg,    [AppChatID] = @AppChatID,    [AppChatStatus] = @AppChatStatus,    [MenuItems] = @MenuItems,    [TicketRights] = @TicketRights,    [Signature] = @Signature,    [LinkedIn] = @LinkedIn,    [OnlyEmailAfterHours] = @OnlyEmailAfterHours,    [BlockInboundEmail] = @BlockInboundEmail,    [SalesForceID] = @SalesForceID,    [ChangeTicketVisibility] = @ChangeTicketVisibility,    [ChangeKBVisibility] = @ChangeKBVisibility,    [EnforceSingleSession] = @EnforceSingleSession,    [NeedsIndexing] = @NeedsIndexing,    [AllowAnyTicketCustomer] = @AllowAnyTicketCustomer,    [FontFamily] = @FontFamily,    [FontSize] = @FontSize,    [CanCreateCompany] = @CanCreateCompany,    [CanEditCompany] = @CanEditCompany,    [CanCreateContact] = @CanCreateContact,    [CanEditContact] = @CanEditContact,    [RestrictUserFromEditingAnyActions] = @RestrictUserFromEditingAnyActions,    [AllowUserToEditAnyAction] = @AllowUserToEditAnyAction,    [UserCanPinAction] = @UserCanPinAction,    [PortalLimitOrgTickets] = @PortalLimitOrgTickets,    [CanCreateAsset] = @CanCreateAsset,    [CanEditAsset] = @CanEditAsset,    [CanChangeCommunityVisibility] = @CanChangeCommunityVisibility,    [FilterInactive] = @FilterInactive,    [DisableExporting] = @DisableExporting, [DisablePublic] = @DisablePublic, [CanCreateProducts] = @CanCreateProducts,    [CanEditProducts] = @CanEditProducts,    [CanCreateVersions] = @CanCreateVersions,    [CanEditVersions] = @CanEditVersions,    [ReceiveUnassignedGroupEmails] = @ReceiveUnassignedGroupEmails,    [ProductFamiliesRights] = @ProductFamiliesRights,    [BlockEmailFromCreatingOnly] = @BlockEmailFromCreatingOnly,    [CalGUID] = @CalGUID,    [PortalViewOnly] = @PortalViewOnly,    [verificationPhoneNumber] = @verificationPhoneNumber,    [verificationCode] = @verificationCode,    [verificationCodeExpiration] = @verificationCodeExpiration,    [PasswordCreatedUtc] = @PasswordCreatedUtc,    [ImportFileID] = @ImportFileID,    [PortalLimitOrgChildrenTickets] = @PortalLimitOrgChildrenTickets  WHERE ([UserID] = @UserID);";


		tempParameter = updateCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

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

		tempParameter = updateCommand.Parameters.Add("CryptedPassword", SqlDbType.VarChar, 255);
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

		tempParameter = updateCommand.Parameters.Add("TimeZoneID", SqlDbType.VarChar, 300);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("CultureName", SqlDbType.VarChar, 50);
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

		tempParameter = updateCommand.Parameters.Add("LastWaterCoolerID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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

		tempParameter = updateCommand.Parameters.Add("IsChatUser", SqlDbType.Bit, 1);
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

		tempParameter = updateCommand.Parameters.Add("ReceiveTicketNotifications", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("ReceiveAllGroupNotifications", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("SubscribeToNewTickets", SqlDbType.Bit, 1);
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

		tempParameter = updateCommand.Parameters.Add("LastVersion", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("SessionID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 500);
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

		tempParameter = updateCommand.Parameters.Add("OrgsUserCanSeeOnPortal", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("DoNotAutoSubscribe", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("IsClassicView", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("SubscribeToNewActions", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("ApprovedTerms", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("ShowWelcomePage", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("UserInformation", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("PortalAutoReg", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("AppChatID", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("AppChatStatus", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("MenuItems", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("TicketRights", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("Signature", SqlDbType.NVarChar, -1);
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

		tempParameter = updateCommand.Parameters.Add("OnlyEmailAfterHours", SqlDbType.Bit, 1);
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

		tempParameter = updateCommand.Parameters.Add("SalesForceID", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("ChangeTicketVisibility", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("ChangeKBVisibility", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("EnforceSingleSession", SqlDbType.Bit, 1);
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

		tempParameter = updateCommand.Parameters.Add("AllowAnyTicketCustomer", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("FontFamily", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("FontSize", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("CanCreateCompany", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("CanEditCompany", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("CanCreateContact", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("CanEditContact", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("RestrictUserFromEditingAnyActions", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("AllowUserToEditAnyAction", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("UserCanPinAction", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("PortalLimitOrgTickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("CanCreateAsset", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("CanEditAsset", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("CanChangeCommunityVisibility", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("FilterInactive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("DisableExporting", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

        tempParameter = updateCommand.Parameters.Add("DisablePublic", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("CanCreateProducts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("CanEditProducts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("CanCreateVersions", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("CanEditVersions", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("ReceiveUnassignedGroupEmails", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("ProductFamiliesRights", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("BlockEmailFromCreatingOnly", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("CalGUID", SqlDbType.UniqueIdentifier, 16);
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

		tempParameter = updateCommand.Parameters.Add("verificationPhoneNumber", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("verificationCode", SqlDbType.VarChar, 9);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("verificationCodeExpiration", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}

		tempParameter = updateCommand.Parameters.Add("PasswordCreatedUtc", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}

		tempParameter = updateCommand.Parameters.Add("ImportFileID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("PortalLimitOrgChildrenTickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}


		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Users] (    [Email],    [FirstName],    [MiddleName],    [LastName],    [Title],    [CryptedPassword],    [IsActive],    [MarkDeleted],    [TimeZoneID],    [CultureName],    [LastLogin],    [LastActivity],    [LastPing],    [LastWaterCoolerID],    [IsSystemAdmin],    [IsFinanceAdmin],    [IsPasswordExpired],    [IsPortalUser],    [IsChatUser],    [PrimaryGroupID],    [InOffice],    [InOfficeComment],    [ReceiveTicketNotifications],    [ReceiveAllGroupNotifications],    [SubscribeToNewTickets],    [ActivatedOn],    [DeactivatedOn],    [OrganizationID],    [LastVersion],    [SessionID],    [ImportID],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [OrgsUserCanSeeOnPortal],    [DoNotAutoSubscribe],    [IsClassicView],    [SubscribeToNewActions],    [ApprovedTerms],    [ShowWelcomePage],    [UserInformation],    [PortalAutoReg],    [AppChatID],    [AppChatStatus],    [MenuItems],    [TicketRights],    [Signature],    [LinkedIn],    [OnlyEmailAfterHours],    [BlockInboundEmail],    [SalesForceID],    [ChangeTicketVisibility],    [ChangeKBVisibility],    [EnforceSingleSession],    [NeedsIndexing],    [AllowAnyTicketCustomer],    [FontFamily],    [FontSize],    [CanCreateCompany],    [CanEditCompany],    [CanCreateContact],    [CanEditContact],    [RestrictUserFromEditingAnyActions],    [AllowUserToEditAnyAction],    [UserCanPinAction],    [PortalLimitOrgTickets],    [CanCreateAsset],    [CanEditAsset],    [CanChangeCommunityVisibility],    [FilterInactive],    [DisableExporting], [DisablePublic], [CanCreateProducts],    [CanEditProducts],    [CanCreateVersions],    [CanEditVersions],    [ReceiveUnassignedGroupEmails],    [ProductFamiliesRights],    [BlockEmailFromCreatingOnly],    [CalGUID],    [PortalViewOnly],    [verificationPhoneNumber],    [verificationCode],    [verificationCodeExpiration],    [PasswordCreatedUtc],    [ImportFileID],    [PortalLimitOrgChildrenTickets]) VALUES ( @Email, @FirstName, @MiddleName, @LastName, @Title, @CryptedPassword, @IsActive, @MarkDeleted, @TimeZoneID, @CultureName, @LastLogin, @LastActivity, @LastPing, @LastWaterCoolerID, @IsSystemAdmin, @IsFinanceAdmin, @IsPasswordExpired, @IsPortalUser, @IsChatUser, @PrimaryGroupID, @InOffice, @InOfficeComment, @ReceiveTicketNotifications, @ReceiveAllGroupNotifications, @SubscribeToNewTickets, @ActivatedOn, @DeactivatedOn, @OrganizationID, @LastVersion, @SessionID, @ImportID, @DateCreated, @DateModified, @CreatorID, @ModifierID, @OrgsUserCanSeeOnPortal, @DoNotAutoSubscribe, @IsClassicView, @SubscribeToNewActions, @ApprovedTerms, @ShowWelcomePage, @UserInformation, @PortalAutoReg, @AppChatID, @AppChatStatus, @MenuItems, @TicketRights, @Signature, @LinkedIn, @OnlyEmailAfterHours, @BlockInboundEmail, @SalesForceID, @ChangeTicketVisibility, @ChangeKBVisibility, @EnforceSingleSession, @NeedsIndexing, @AllowAnyTicketCustomer, @FontFamily, @FontSize, @CanCreateCompany, @CanEditCompany, @CanCreateContact, @CanEditContact, @RestrictUserFromEditingAnyActions, @AllowUserToEditAnyAction, @UserCanPinAction, @PortalLimitOrgTickets, @CanCreateAsset, @CanEditAsset, @CanChangeCommunityVisibility, @FilterInactive, @DisableExporting, @CanCreateProducts, @CanEditProducts, @CanCreateVersions, @CanEditVersions, @ReceiveUnassignedGroupEmails, @ProductFamiliesRights, @BlockEmailFromCreatingOnly, @CalGUID, @PortalViewOnly, @verificationPhoneNumber, @verificationCode, @verificationCodeExpiration, @PasswordCreatedUtc, @ImportFileID, @PortalLimitOrgChildrenTickets); SET @Identity = SCOPE_IDENTITY();";


		tempParameter = insertCommand.Parameters.Add("PortalLimitOrgChildrenTickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("ImportFileID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("PasswordCreatedUtc", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}

		tempParameter = insertCommand.Parameters.Add("verificationCodeExpiration", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}

		tempParameter = insertCommand.Parameters.Add("verificationCode", SqlDbType.VarChar, 9);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("verificationPhoneNumber", SqlDbType.VarChar, 50);
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

		tempParameter = insertCommand.Parameters.Add("CalGUID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("BlockEmailFromCreatingOnly", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("ProductFamiliesRights", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("ReceiveUnassignedGroupEmails", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("CanEditVersions", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("CanCreateVersions", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("CanEditProducts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("CanCreateProducts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("DisableExporting", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

        tempParameter = insertCommand.Parameters.Add("DisablePublic", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("FilterInactive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("CanChangeCommunityVisibility", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("CanEditAsset", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("CanCreateAsset", SqlDbType.Bit, 1);
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

		tempParameter = insertCommand.Parameters.Add("UserCanPinAction", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("AllowUserToEditAnyAction", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("RestrictUserFromEditingAnyActions", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("CanEditContact", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("CanCreateContact", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("CanEditCompany", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("CanCreateCompany", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("FontSize", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("FontFamily", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("AllowAnyTicketCustomer", SqlDbType.Bit, 1);
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

		tempParameter = insertCommand.Parameters.Add("EnforceSingleSession", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("ChangeKBVisibility", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("ChangeTicketVisibility", SqlDbType.Bit, 1);
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

		tempParameter = insertCommand.Parameters.Add("BlockInboundEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("OnlyEmailAfterHours", SqlDbType.Bit, 1);
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

		tempParameter = insertCommand.Parameters.Add("Signature", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("TicketRights", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("MenuItems", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("AppChatStatus", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("AppChatID", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("PortalAutoReg", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("UserInformation", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("ShowWelcomePage", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("ApprovedTerms", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("SubscribeToNewActions", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("IsClassicView", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("DoNotAutoSubscribe", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("OrgsUserCanSeeOnPortal", SqlDbType.VarChar, 200);
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

		tempParameter = insertCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("SessionID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("LastVersion", SqlDbType.VarChar, 50);
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

		tempParameter = insertCommand.Parameters.Add("SubscribeToNewTickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("ReceiveAllGroupNotifications", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("ReceiveTicketNotifications", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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

		tempParameter = insertCommand.Parameters.Add("IsChatUser", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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

		tempParameter = insertCommand.Parameters.Add("LastWaterCoolerID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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

		tempParameter = insertCommand.Parameters.Add("CultureName", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("TimeZoneID", SqlDbType.VarChar, 300);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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

		tempParameter = insertCommand.Parameters.Add("CryptedPassword", SqlDbType.VarChar, 255);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Users] WHERE ([UserID] = @UserID);";
		deleteCommand.Parameters.Add("UserID", SqlDbType.Int);

		try
		{
		  foreach (User user in this)
		  {
			if (user.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(user);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = user.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["UserID"].AutoIncrement = false;
			  Table.Columns["UserID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				user.Row["UserID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(user);
			}
			else if (user.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(user);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = user.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(user);
			}
			else if (user.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)user.Row["UserID", DataRowVersion.Original];
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

      foreach (User user in this)
      {
        if (user.Row.Table.Columns.Contains("CreatorID") && (int)user.Row["CreatorID"] == 0) user.Row["CreatorID"] = LoginUser.UserID;
        if (user.Row.Table.Columns.Contains("ModifierID")) user.Row["ModifierID"] = LoginUser.UserID;
      }

      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();

      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public User FindByUserID(int userID)
    {
      foreach (User user in this)
      {
        if (user.UserID == userID)
        {
          return user;
        }
      }
      return null;
    }

    public virtual User AddNewUser()
    {
      if (Table.Columns.Count < 1) LoadColumns("Users");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new User(row, this);
    }

    public virtual void LoadByUserID(int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [UserID], [Email], [FirstName], [MiddleName], [LastName], [Title], [CryptedPassword], [IsActive], [MarkDeleted], [TimeZoneID], [CultureName], [LastLogin], [LastActivity], [LastPing], [LastWaterCoolerID], [IsSystemAdmin], [IsFinanceAdmin], [IsPasswordExpired], [IsPortalUser], [IsChatUser], [PrimaryGroupID], [InOffice], [InOfficeComment], [ReceiveTicketNotifications], [ReceiveAllGroupNotifications], [SubscribeToNewTickets], [ActivatedOn], [DeactivatedOn], [OrganizationID], [LastVersion], [SessionID], [ImportID], [DateCreated], [DateModified], [CreatorID], [ModifierID], [OrgsUserCanSeeOnPortal], [DoNotAutoSubscribe], [IsClassicView], [SubscribeToNewActions], [ApprovedTerms], [ShowWelcomePage], [UserInformation], [PortalAutoReg], [AppChatID], [AppChatStatus], [MenuItems], [TicketRights], [Signature], [LinkedIn], [OnlyEmailAfterHours], [BlockInboundEmail], [SalesForceID], [ChangeTicketVisibility], [ChangeKBVisibility], [EnforceSingleSession], [NeedsIndexing], [AllowAnyTicketCustomer], [FontFamily], [FontSize], [CanCreateCompany], [CanEditCompany], [CanCreateContact], [CanEditContact], [RestrictUserFromEditingAnyActions], [AllowUserToEditAnyAction], [UserCanPinAction], [PortalLimitOrgTickets], [CanCreateAsset], [CanEditAsset], [CanChangeCommunityVisibility], [FilterInactive], [DisableExporting], [DisablePublic], [CanCreateProducts], [CanEditProducts], [CanCreateVersions], [CanEditVersions], [ReceiveUnassignedGroupEmails], [ProductFamiliesRights], [BlockEmailFromCreatingOnly], [CalGUID], [PortalViewOnly], [verificationPhoneNumber], [verificationCode], [verificationCodeExpiration], [PasswordCreatedUtc], [ImportFileID], [PortalLimitOrgChildrenTickets] FROM [dbo].[Users] WHERE ([UserID] = @UserID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("UserID", userID);
        Fill(command);
      }
    }

    public static User GetUser(LoginUser loginUser, int userID)
    {
      Users users = new Users(loginUser);
      users.LoadByUserID(userID);
      if (users.IsEmpty)
        return null;
      else
        return users[0];
    }




    #endregion

    #region IEnumerable<User> Members

    public IEnumerator<User> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new User(row, this);
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
