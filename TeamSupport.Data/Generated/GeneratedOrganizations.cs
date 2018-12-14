using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Organization : BaseItem
  {
    private Organizations _organizations;

    public Organization(DataRow row, Organizations organizations): base(row, organizations)
    {
      _organizations = organizations;
    }

    #region Properties

    public Organizations Collection
    {
      get { return _organizations; }
    }




    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
    }



    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
    }

    public string Website
    {
      get { return Row["Website"] != DBNull.Value ? (string)Row["Website"] : null; }
      set { Row["Website"] = CheckValue("Website", value); }
    }

    public string WhereHeard
    {
      get { return Row["WhereHeard"] != DBNull.Value ? (string)Row["WhereHeard"] : null; }
      set { Row["WhereHeard"] = CheckValue("WhereHeard", value); }
    }

    public string PromoCode
    {
      get { return Row["PromoCode"] != DBNull.Value ? (string)Row["PromoCode"] : null; }
      set { Row["PromoCode"] = CheckValue("PromoCode", value); }
    }

    public string ImportID
    {
      get { return Row["ImportID"] != DBNull.Value ? (string)Row["ImportID"] : null; }
      set { Row["ImportID"] = CheckValue("ImportID", value); }
    }

    public string TimeZoneID
    {
      get { return Row["TimeZoneID"] != DBNull.Value ? (string)Row["TimeZoneID"] : null; }
      set { Row["TimeZoneID"] = CheckValue("TimeZoneID", value); }
    }

    public string InActiveReason
    {
      get { return Row["InActiveReason"] != DBNull.Value ? (string)Row["InActiveReason"] : null; }
      set { Row["InActiveReason"] = CheckValue("InActiveReason", value); }
    }

    public int? PrimaryUserID
    {
      get { return Row["PrimaryUserID"] != DBNull.Value ? (int?)Row["PrimaryUserID"] : null; }
      set { Row["PrimaryUserID"] = CheckValue("PrimaryUserID", value); }
    }

    public int? DefaultPortalGroupID
    {
      get { return Row["DefaultPortalGroupID"] != DBNull.Value ? (int?)Row["DefaultPortalGroupID"] : null; }
      set { Row["DefaultPortalGroupID"] = CheckValue("DefaultPortalGroupID", value); }
    }

    public int? DefaultSupportGroupID
    {
      get { return Row["DefaultSupportGroupID"] != DBNull.Value ? (int?)Row["DefaultSupportGroupID"] : null; }
      set { Row["DefaultSupportGroupID"] = CheckValue("DefaultSupportGroupID", value); }
    }

    public int? DefaultSupportUserID
    {
      get { return Row["DefaultSupportUserID"] != DBNull.Value ? (int?)Row["DefaultSupportUserID"] : null; }
      set { Row["DefaultSupportUserID"] = CheckValue("DefaultSupportUserID", value); }
    }

    public int? ParentID
    {
      get { return Row["ParentID"] != DBNull.Value ? (int?)Row["ParentID"] : null; }
      set { Row["ParentID"] = CheckValue("ParentID", value); }
    }

    public string CRMLinkID
    {
      get { return Row["CRMLinkID"] != DBNull.Value ? (string)Row["CRMLinkID"] : null; }
      set { Row["CRMLinkID"] = CheckValue("CRMLinkID", value); }
    }

    public bool? RequireKnownUserForNewEmail
    {
      get { return Row["RequireKnownUserForNewEmail"] != DBNull.Value ? (bool?)Row["RequireKnownUserForNewEmail"] : null; }
      set { Row["RequireKnownUserForNewEmail"] = CheckValue("RequireKnownUserForNewEmail", value); }
    }

    public string EmailDelimiter
    {
      get { return Row["EmailDelimiter"] != DBNull.Value ? (string)Row["EmailDelimiter"] : null; }
      set { Row["EmailDelimiter"] = CheckValue("EmailDelimiter", value); }
    }

    public string OrganizationReplyToAddress
    {
      get { return Row["OrganizationReplyToAddress"] != DBNull.Value ? (string)Row["OrganizationReplyToAddress"] : null; }
      set { Row["OrganizationReplyToAddress"] = CheckValue("OrganizationReplyToAddress", value); }
    }

    public string CompanyDomains
    {
      get { return Row["CompanyDomains"] != DBNull.Value ? (string)Row["CompanyDomains"] : null; }
      set { Row["CompanyDomains"] = CheckValue("CompanyDomains", value); }
    }

    public int? DefaultWikiArticleID
    {
      get { return Row["DefaultWikiArticleID"] != DBNull.Value ? (int?)Row["DefaultWikiArticleID"] : null; }
      set { Row["DefaultWikiArticleID"] = CheckValue("DefaultWikiArticleID", value); }
    }

    public int? SlaLevelID
    {
      get { return Row["SlaLevelID"] != DBNull.Value ? (int?)Row["SlaLevelID"] : null; }
      set { Row["SlaLevelID"] = CheckValue("SlaLevelID", value); }
    }

    public int? InternalSlaLevelID
    {
      get { return Row["InternalSlaLevelID"] != DBNull.Value ? (int?)Row["InternalSlaLevelID"] : null; }
      set { Row["InternalSlaLevelID"] = CheckValue("InternalSlaLevelID", value); }
    }

    public bool? UseEuropeDate
    {
      get { return Row["UseEuropeDate"] != DBNull.Value ? (bool?)Row["UseEuropeDate"] : null; }
      set { Row["UseEuropeDate"] = CheckValue("UseEuropeDate", value); }
    }

    public bool? MatchEmailSubject
    {
      get { return Row["MatchEmailSubject"] != DBNull.Value ? (bool?)Row["MatchEmailSubject"] : null; }
      set { Row["MatchEmailSubject"] = CheckValue("MatchEmailSubject", value); }
    }

    public bool? MarkSpam
    {
      get { return Row["MarkSpam"] != DBNull.Value ? (bool?)Row["MarkSpam"] : null; }
      set { Row["MarkSpam"] = CheckValue("MarkSpam", value); }
    }

    public string PrimaryInterest
    {
      get { return Row["PrimaryInterest"] != DBNull.Value ? (string)Row["PrimaryInterest"] : null; }
      set { Row["PrimaryInterest"] = CheckValue("PrimaryInterest", value); }
    }

    public string PotentialSeats
    {
      get { return Row["PotentialSeats"] != DBNull.Value ? (string)Row["PotentialSeats"] : null; }
      set { Row["PotentialSeats"] = CheckValue("PotentialSeats", value); }
    }

    public string EvalProcess
    {
      get { return Row["EvalProcess"] != DBNull.Value ? (string)Row["EvalProcess"] : null; }
      set { Row["EvalProcess"] = CheckValue("EvalProcess", value); }
    }

    public bool? AddAdditionalContacts
    {
      get { return Row["AddAdditionalContacts"] != DBNull.Value ? (bool?)Row["AddAdditionalContacts"] : null; }
      set { Row["AddAdditionalContacts"] = CheckValue("AddAdditionalContacts", value); }
    }

    public bool? ChangeStatusIfClosed
    {
      get { return Row["ChangeStatusIfClosed"] != DBNull.Value ? (bool?)Row["ChangeStatusIfClosed"] : null; }
      set { Row["ChangeStatusIfClosed"] = CheckValue("ChangeStatusIfClosed", value); }
    }

    public int? UnknownCompanyID
    {
      get { return Row["UnknownCompanyID"] != DBNull.Value ? (int?)Row["UnknownCompanyID"] : null; }
      set { Row["UnknownCompanyID"] = CheckValue("UnknownCompanyID", value); }
    }

    public string SignUpToken
    {
      get { return Row["SignUpToken"] != DBNull.Value ? (string)Row["SignUpToken"] : null; }
      set { Row["SignUpToken"] = CheckValue("SignUpToken", value); }
    }

    public int? CustDistIndexTrend
    {
      get { return Row["CustDistIndexTrend"] != DBNull.Value ? (int?)Row["CustDistIndexTrend"] : null; }
      set { Row["CustDistIndexTrend"] = CheckValue("CustDistIndexTrend", value); }
    }

    public int? ImportFileID
    {
      get { return Row["ImportFileID"] != DBNull.Value ? (int?)Row["ImportFileID"] : null; }
      set { Row["ImportFileID"] = CheckValue("ImportFileID", value); }
    }

    public string NoAttachmentsInOutboundExcludeProductLine
    {
      get { return Row["NoAttachmentsInOutboundExcludeProductLine"] != DBNull.Value ? (string)Row["NoAttachmentsInOutboundExcludeProductLine"] : null; }
      set { Row["NoAttachmentsInOutboundExcludeProductLine"] = CheckValue("NoAttachmentsInOutboundExcludeProductLine", value); }
    }


    public int APIRequestMinuteLimit
    {
      get { return (int)Row["APIRequestMinuteLimit"]; }
      set { Row["APIRequestMinuteLimit"] = CheckValue("APIRequestMinuteLimit", value); }
    }

    public bool UseWatson
    {
      get { return (bool)Row["UseWatson"]; }
      set { Row["UseWatson"] = CheckValue("UseWatson", value); }
    }

    public bool RequireTwoFactor
    {
      get { return (bool)Row["RequireTwoFactor"]; }
      set { Row["RequireTwoFactor"] = CheckValue("RequireTwoFactor", value); }
    }

    public bool DisableSupportLogin
    {
      get { return (bool)Row["DisableSupportLogin"]; }
      set { Row["DisableSupportLogin"] = CheckValue("DisableSupportLogin", value); }
    }

    public bool AlertContactNoEmail
    {
      get { return (bool)Row["AlertContactNoEmail"]; }
      set { Row["AlertContactNoEmail"] = CheckValue("AlertContactNoEmail", value); }
    }

    public bool RequireGroupAssignmentOnTickets
    {
      get { return (bool)Row["RequireGroupAssignmentOnTickets"]; }
      set { Row["RequireGroupAssignmentOnTickets"] = CheckValue("RequireGroupAssignmentOnTickets", value); }
    }

    public bool AutoAssociateCustomerToTicketBasedOnAssetAssignment
    {
      get { return (bool)Row["AutoAssociateCustomerToTicketBasedOnAssetAssignment"]; }
      set { Row["AutoAssociateCustomerToTicketBasedOnAssetAssignment"] = CheckValue("AutoAssociateCustomerToTicketBasedOnAssetAssignment", value); }
    }

    public bool AutoAssignCustomerWithAssetOnTickets
    {
      get { return (bool)Row["AutoAssignCustomerWithAssetOnTickets"]; }
      set { Row["AutoAssignCustomerWithAssetOnTickets"] = CheckValue("AutoAssignCustomerWithAssetOnTickets", value); }
    }

    public bool NoAttachmentsInOutboundEmail
    {
      get { return (bool)Row["NoAttachmentsInOutboundEmail"]; }
      set { Row["NoAttachmentsInOutboundEmail"] = CheckValue("NoAttachmentsInOutboundEmail", value); }
    }

    public int DaysBeforePasswordExpire
    {
      get { return (int)Row["DaysBeforePasswordExpire"]; }
      set { Row["DaysBeforePasswordExpire"] = CheckValue("DaysBeforePasswordExpire", value); }
    }

    public bool TwoStepVerificationEnabled
    {
      get { return (bool)Row["TwoStepVerificationEnabled"]; }
      set { Row["TwoStepVerificationEnabled"] = CheckValue("TwoStepVerificationEnabled", value); }
    }

    public bool IsCustomerInsightsActive
    {
      get { return (bool)Row["IsCustomerInsightsActive"]; }
      set { Row["IsCustomerInsightsActive"] = CheckValue("IsCustomerInsightsActive", value); }
    }

    public bool UseProductFamilies
    {
      get { return (bool)Row["UseProductFamilies"]; }
      set { Row["UseProductFamilies"] = CheckValue("UseProductFamilies", value); }
    }

    public bool HideDismissNonAdmins
    {
      get { return (bool)Row["HideDismissNonAdmins"]; }
      set { Row["HideDismissNonAdmins"] = CheckValue("HideDismissNonAdmins", value); }
    }

    public bool IsValidated
    {
      get { return (bool)Row["IsValidated"]; }
      set { Row["IsValidated"] = CheckValue("IsValidated", value); }
    }

    public bool AddEmailViaTS
    {
      get { return (bool)Row["AddEmailViaTS"]; }
      set { Row["AddEmailViaTS"] = CheckValue("AddEmailViaTS", value); }
    }

    public bool AgentRating
    {
      get { return (bool)Row["AgentRating"]; }
      set { Row["AgentRating"] = CheckValue("AgentRating", value); }
    }

    public bool ForceUseOfReplyTo
    {
      get { return (bool)Row["ForceUseOfReplyTo"]; }
      set { Row["ForceUseOfReplyTo"] = CheckValue("ForceUseOfReplyTo", value); }
    }

    public bool ReplyToAlternateEmailAddresses
    {
      get { return (bool)Row["ReplyToAlternateEmailAddresses"]; }
      set { Row["ReplyToAlternateEmailAddresses"] = CheckValue("ReplyToAlternateEmailAddresses", value); }
    }

    public bool UpdateTicketChildrenGroupWithParent
    {
      get { return (bool)Row["UpdateTicketChildrenGroupWithParent"]; }
      set { Row["UpdateTicketChildrenGroupWithParent"] = CheckValue("UpdateTicketChildrenGroupWithParent", value); }
    }

    public bool ShowGroupMembersFirstInTicketAssignmentList
    {
      get { return (bool)Row["ShowGroupMembersFirstInTicketAssignmentList"]; }
      set { Row["ShowGroupMembersFirstInTicketAssignmentList"] = CheckValue("ShowGroupMembersFirstInTicketAssignmentList", value); }
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

    public bool SlaInitRespAnyAction
    {
      get { return (bool)Row["SlaInitRespAnyAction"]; }
      set { Row["SlaInitRespAnyAction"] = CheckValue("SlaInitRespAnyAction", value); }
    }

    public int CustDisIndex
    {
      get { return (int)Row["CustDisIndex"]; }
      set { Row["CustDisIndex"] = CheckValue("CustDisIndex", value); }
    }

    public int AvgTimeToClose
    {
      get { return (int)Row["AvgTimeToClose"]; }
      set { Row["AvgTimeToClose"] = CheckValue("AvgTimeToClose", value); }
    }

    public int AvgTimeOpen
    {
      get { return (int)Row["AvgTimeOpen"]; }
      set { Row["AvgTimeOpen"] = CheckValue("AvgTimeOpen", value); }
    }

    public int CreatedLast30
    {
      get { return (int)Row["CreatedLast30"]; }
      set { Row["CreatedLast30"] = CheckValue("CreatedLast30", value); }
    }

    public int TicketsOpen
    {
      get { return (int)Row["TicketsOpen"]; }
      set { Row["TicketsOpen"] = CheckValue("TicketsOpen", value); }
    }

    public int TotalTicketsCreated
    {
      get { return (int)Row["TotalTicketsCreated"]; }
      set { Row["TotalTicketsCreated"] = CheckValue("TotalTicketsCreated", value); }
    }

    public bool NeedCustForTicketMatch
    {
      get { return (bool)Row["NeedCustForTicketMatch"]; }
      set { Row["NeedCustForTicketMatch"] = CheckValue("NeedCustForTicketMatch", value); }
    }

    public bool NeedsIndexing
    {
      get { return (bool)Row["NeedsIndexing"]; }
      set { Row["NeedsIndexing"] = CheckValue("NeedsIndexing", value); }
    }

    public bool IsIndexLocked
    {
      get { return (bool)Row["IsIndexLocked"]; }
      set { Row["IsIndexLocked"] = CheckValue("IsIndexLocked", value); }
    }

    public bool IsRebuildingIndex
    {
      get { return (bool)Row["IsRebuildingIndex"]; }
      set { Row["IsRebuildingIndex"] = CheckValue("IsRebuildingIndex", value); }
    }

    public bool ForceBCCEmailsPrivate
    {
      get { return (bool)Row["ForceBCCEmailsPrivate"]; }
      set { Row["ForceBCCEmailsPrivate"] = CheckValue("ForceBCCEmailsPrivate", value); }
    }

    public bool AllowUnsecureAttachmentViewing
    {
      get { return (bool)Row["AllowUnsecureAttachmentViewing"]; }
      set { Row["AllowUnsecureAttachmentViewing"] = CheckValue("AllowUnsecureAttachmentViewing", value); }
    }

    public bool ProductVersionRequired
    {
      get { return (bool)Row["ProductVersionRequired"]; }
      set { Row["ProductVersionRequired"] = CheckValue("ProductVersionRequired", value); }
    }

    public bool ProductRequired
    {
      get { return (bool)Row["ProductRequired"]; }
      set { Row["ProductRequired"] = CheckValue("ProductRequired", value); }
    }

    public int SupportHoursMonth
    {
      get { return (int)Row["SupportHoursMonth"]; }
      set { Row["SupportHoursMonth"] = CheckValue("SupportHoursMonth", value); }
    }

    public bool SetNewActionsVisibleToCustomers
    {
      get { return (bool)Row["SetNewActionsVisibleToCustomers"]; }
      set { Row["SetNewActionsVisibleToCustomers"] = CheckValue("SetNewActionsVisibleToCustomers", value); }
    }

    public bool UseForums
    {
      get { return (bool)Row["UseForums"]; }
      set { Row["UseForums"] = CheckValue("UseForums", value); }
    }

    public bool IsPublicArticles
    {
      get { return (bool)Row["IsPublicArticles"]; }
      set { Row["IsPublicArticles"] = CheckValue("IsPublicArticles", value); }
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

    public bool TimedActionsRequired
    {
      get { return (bool)Row["TimedActionsRequired"]; }
      set { Row["TimedActionsRequired"] = CheckValue("TimedActionsRequired", value); }
    }

    public string CultureName
    {
      get { return (string)Row["CultureName"]; }
      set { Row["CultureName"] = CheckValue("CultureName", value); }
    }

    public int BusinessDays
    {
      get { return (int)Row["BusinessDays"]; }
      set { Row["BusinessDays"] = CheckValue("BusinessDays", value); }
    }

    public bool ShowWiki
    {
      get { return (bool)Row["ShowWiki"]; }
      set { Row["ShowWiki"] = CheckValue("ShowWiki", value); }
    }

    public bool AdminOnlyReports
    {
      get { return (bool)Row["AdminOnlyReports"]; }
      set { Row["AdminOnlyReports"] = CheckValue("AdminOnlyReports", value); }
    }

    public bool AdminOnlyCustomers
    {
      get { return (bool)Row["AdminOnlyCustomers"]; }
      set { Row["AdminOnlyCustomers"] = CheckValue("AdminOnlyCustomers", value); }
    }

    public bool RequireNewKeyword
    {
      get { return (bool)Row["RequireNewKeyword"]; }
      set { Row["RequireNewKeyword"] = CheckValue("RequireNewKeyword", value); }
    }

    public int APIRequestLimit
    {
      get { return (int)Row["APIRequestLimit"]; }
      set { Row["APIRequestLimit"] = CheckValue("APIRequestLimit", value); }
    }

    public Guid PortalGuid
    {
      get { return (Guid)Row["PortalGuid"]; }
      set { Row["PortalGuid"] = CheckValue("PortalGuid", value); }
    }

    public Guid ChatID
    {
      get { return (Guid)Row["ChatID"]; }
      set { Row["ChatID"] = CheckValue("ChatID", value); }
    }

    public Guid SystemEmailID
    {
      get { return (Guid)Row["SystemEmailID"]; }
      set { Row["SystemEmailID"] = CheckValue("SystemEmailID", value); }
    }

    public Guid WebServiceID
    {
      get { return (Guid)Row["WebServiceID"]; }
      set { Row["WebServiceID"] = CheckValue("WebServiceID", value); }
    }

    public ProductType ProductType
    {
      get { return (ProductType)Row["ProductType"]; }
      set { Row["ProductType"] = CheckValue("ProductType", value); }
    }

    public bool IsBasicPortal
    {
      get { return (bool)Row["IsBasicPortal"]; }
      set { Row["IsBasicPortal"] = CheckValue("IsBasicPortal", value); }
    }

    public bool IsAdvancedPortal
    {
      get { return (bool)Row["IsAdvancedPortal"]; }
      set { Row["IsAdvancedPortal"] = CheckValue("IsAdvancedPortal", value); }
    }

    public bool HasPortalAccess
    {
      get { return (bool)Row["HasPortalAccess"]; }
      set { Row["HasPortalAccess"] = CheckValue("HasPortalAccess", value); }
    }

    public bool IsInventoryEnabled
    {
      get { return (bool)Row["IsInventoryEnabled"]; }
      set { Row["IsInventoryEnabled"] = CheckValue("IsInventoryEnabled", value); }
    }

    public bool IsApiEnabled
    {
      get { return (bool)Row["IsApiEnabled"]; }
      set { Row["IsApiEnabled"] = CheckValue("IsApiEnabled", value); }
    }

    public bool IsApiActive
    {
      get { return (bool)Row["IsApiActive"]; }
      set { Row["IsApiActive"] = CheckValue("IsApiActive", value); }
    }

    public bool IsActive
    {
      get { return (bool)Row["IsActive"]; }
      set { Row["IsActive"] = CheckValue("IsActive", value); }
    }

    public int ExtraStorageUnits
    {
      get { return (int)Row["ExtraStorageUnits"]; }
      set { Row["ExtraStorageUnits"] = CheckValue("ExtraStorageUnits", value); }
    }

    public int ChatSeats
    {
      get { return (int)Row["ChatSeats"]; }
      set { Row["ChatSeats"] = CheckValue("ChatSeats", value); }
    }

    public int PortalSeats
    {
      get { return (int)Row["PortalSeats"]; }
      set { Row["PortalSeats"] = CheckValue("PortalSeats", value); }
    }

    public int UserSeats
    {
      get { return (int)Row["UserSeats"]; }
      set { Row["UserSeats"] = CheckValue("UserSeats", value); }
    }

    public bool IsCustomerFree
    {
      get { return (bool)Row["IsCustomerFree"]; }
      set { Row["IsCustomerFree"] = CheckValue("IsCustomerFree", value); }
    }

    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckValue("Name", value); }
    }


    /* DateTime */





    public DateTime? SAExpirationDate
    {
      get { return Row["SAExpirationDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["SAExpirationDate"]) : null; }
      set { Row["SAExpirationDate"] = CheckValue("SAExpirationDate", value); }
    }

    public DateTime? SAExpirationDateUtc
    {
      get { return Row["SAExpirationDate"] != DBNull.Value ? (DateTime?)Row["SAExpirationDate"] : null; }
    }

    public DateTime? DateLastIndexed
    {
      get { return Row["DateLastIndexed"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateLastIndexed"]) : null; }
      set { Row["DateLastIndexed"] = CheckValue("DateLastIndexed", value); }
    }

    public DateTime? DateLastIndexedUtc
    {
      get { return Row["DateLastIndexed"] != DBNull.Value ? (DateTime?)Row["DateLastIndexed"] : null; }
    }



    public DateTime LastIndexRebuilt
    {
      get { return DateToLocal((DateTime)Row["LastIndexRebuilt"]); }
      set { Row["LastIndexRebuilt"] = CheckValue("LastIndexRebuilt", value); }
    }

    public DateTime LastIndexRebuiltUtc
    {
      get { return (DateTime)Row["LastIndexRebuilt"]; }
    }

    public DateTime BusinessDayEnd
    {
      get { return DateToLocal((DateTime)Row["BusinessDayEnd"]); }
      set { Row["BusinessDayEnd"] = CheckValue("BusinessDayEnd", value); }
    }

    public DateTime BusinessDayEndUtc
    {
      get { return (DateTime)Row["BusinessDayEnd"]; }
    }

    public DateTime BusinessDayStart
    {
      get { return DateToLocal((DateTime)Row["BusinessDayStart"]); }
      set { Row["BusinessDayStart"] = CheckValue("BusinessDayStart", value); }
    }

    public DateTime BusinessDayStartUtc
    {
      get { return (DateTime)Row["BusinessDayStart"]; }
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


    #endregion


  }

  public partial class Organizations : BaseCollection, IEnumerable<Organization>
  {
    public Organizations(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Organizations"; }
    }

    public override string PrimaryKeyFieldName
    {
      get { return "OrganizationID"; }
    }



    public Organization this[int index]
    {
      get { return new Organization(Table.Rows[index], this); }
    }


    #endregion

    #region Protected Members

    partial void BeforeRowInsert(Organization organization);
    partial void AfterRowInsert(Organization organization);
    partial void BeforeRowEdit(Organization organization);
    partial void AfterRowEdit(Organization organization);
    partial void BeforeRowDelete(int organizationID);
    partial void AfterRowDelete(int organizationID);

    partial void BeforeDBDelete(int organizationID);
    partial void AfterDBDelete(int organizationID);

    #endregion

    #region Public Methods

    public OrganizationProxy[] GetOrganizationProxies()
    {
      List<OrganizationProxy> list = new List<OrganizationProxy>();

      foreach (Organization item in this)
      {
        list.Add(item.GetProxy());
      }

      return list.ToArray();
    }

    public virtual void DeleteFromDB(int organizationID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Organizations] WHERE ([OrganizationID] = @OrganizationID);";
        deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);
        deleteCommand.Parameters["OrganizationID"].Value = organizationID;

        BeforeDBDelete(organizationID);
        BeforeRowDelete(organizationID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(organizationID);
        AfterDBDelete(organizationID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("OrganizationsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Organizations] SET     [Name] = @Name,    [Description] = @Description,    [Website] = @Website,    [WhereHeard] = @WhereHeard,    [PromoCode] = @PromoCode,    [IsCustomerFree] = @IsCustomerFree,    [UserSeats] = @UserSeats,    [PortalSeats] = @PortalSeats,    [ChatSeats] = @ChatSeats,    [ExtraStorageUnits] = @ExtraStorageUnits,    [ImportID] = @ImportID,    [IsActive] = @IsActive,    [IsApiActive] = @IsApiActive,    [IsApiEnabled] = @IsApiEnabled,    [IsInventoryEnabled] = @IsInventoryEnabled,    [TimeZoneID] = @TimeZoneID,    [InActiveReason] = @InActiveReason,    [HasPortalAccess] = @HasPortalAccess,    [IsAdvancedPortal] = @IsAdvancedPortal,    [IsBasicPortal] = @IsBasicPortal,    [PrimaryUserID] = @PrimaryUserID,    [DefaultPortalGroupID] = @DefaultPortalGroupID,    [DefaultSupportGroupID] = @DefaultSupportGroupID,    [DefaultSupportUserID] = @DefaultSupportUserID,    [ProductType] = @ProductType,    [ParentID] = @ParentID,    [WebServiceID] = @WebServiceID,    [SystemEmailID] = @SystemEmailID,    [ChatID] = @ChatID,    [PortalGuid] = @PortalGuid,    [CRMLinkID] = @CRMLinkID,    [SAExpirationDate] = @SAExpirationDate,    [APIRequestLimit] = @APIRequestLimit,    [DateModified] = @DateModified,    [RequireNewKeyword] = @RequireNewKeyword,    [RequireKnownUserForNewEmail] = @RequireKnownUserForNewEmail,    [EmailDelimiter] = @EmailDelimiter,    [OrganizationReplyToAddress] = @OrganizationReplyToAddress,    [CompanyDomains] = @CompanyDomains,    [AdminOnlyCustomers] = @AdminOnlyCustomers,    [AdminOnlyReports] = @AdminOnlyReports,    [ShowWiki] = @ShowWiki,    [DefaultWikiArticleID] = @DefaultWikiArticleID,    [SlaLevelID] = @SlaLevelID,    [InternalSlaLevelID] = @InternalSlaLevelID,    [BusinessDays] = @BusinessDays,    [BusinessDayStart] = @BusinessDayStart,    [BusinessDayEnd] = @BusinessDayEnd,    [UseEuropeDate] = @UseEuropeDate,    [CultureName] = @CultureName,    [TimedActionsRequired] = @TimedActionsRequired,    [MatchEmailSubject] = @MatchEmailSubject, [MarkSpam] = @MarkSpam, [ModifierID] = @ModifierID,    [PrimaryInterest] = @PrimaryInterest,    [PotentialSeats] = @PotentialSeats,    [EvalProcess] = @EvalProcess,    [AddAdditionalContacts] = @AddAdditionalContacts,    [ChangeStatusIfClosed] = @ChangeStatusIfClosed,    [IsPublicArticles] = @IsPublicArticles,    [UseForums] = @UseForums,    [SetNewActionsVisibleToCustomers] = @SetNewActionsVisibleToCustomers,    [SupportHoursMonth] = @SupportHoursMonth,    [ProductRequired] = @ProductRequired,    [ProductVersionRequired] = @ProductVersionRequired,    [AllowUnsecureAttachmentViewing] = @AllowUnsecureAttachmentViewing,    [ForceBCCEmailsPrivate] = @ForceBCCEmailsPrivate,    [UnknownCompanyID] = @UnknownCompanyID,    [IsRebuildingIndex] = @IsRebuildingIndex,    [LastIndexRebuilt] = @LastIndexRebuilt,    [IsIndexLocked] = @IsIndexLocked,    [NeedsIndexing] = @NeedsIndexing,    [NeedCustForTicketMatch] = @NeedCustForTicketMatch,    [TotalTicketsCreated] = @TotalTicketsCreated,    [TicketsOpen] = @TicketsOpen,    [CreatedLast30] = @CreatedLast30,    [AvgTimeOpen] = @AvgTimeOpen,    [AvgTimeToClose] = @AvgTimeToClose,    [CustDisIndex] = @CustDisIndex,    [SlaInitRespAnyAction] = @SlaInitRespAnyAction,    [FontFamily] = @FontFamily,    [FontSize] = @FontSize,    [ShowGroupMembersFirstInTicketAssignmentList] = @ShowGroupMembersFirstInTicketAssignmentList,    [UpdateTicketChildrenGroupWithParent] = @UpdateTicketChildrenGroupWithParent,    [ReplyToAlternateEmailAddresses] = @ReplyToAlternateEmailAddresses,    [ForceUseOfReplyTo] = @ForceUseOfReplyTo,    [AgentRating] = @AgentRating,    [AddEmailViaTS] = @AddEmailViaTS,    [SignUpToken] = @SignUpToken,    [IsValidated] = @IsValidated,    [DateLastIndexed] = @DateLastIndexed,    [HideDismissNonAdmins] = @HideDismissNonAdmins,    [CustDistIndexTrend] = @CustDistIndexTrend,    [UseProductFamilies] = @UseProductFamilies,    [IsCustomerInsightsActive] = @IsCustomerInsightsActive,    [TwoStepVerificationEnabled] = @TwoStepVerificationEnabled,    [DaysBeforePasswordExpire] = @DaysBeforePasswordExpire,    [ImportFileID] = @ImportFileID,    [NoAttachmentsInOutboundEmail] = @NoAttachmentsInOutboundEmail,    [AutoAssignCustomerWithAssetOnTickets] = @AutoAssignCustomerWithAssetOnTickets,    [AutoAssociateCustomerToTicketBasedOnAssetAssignment] = @AutoAssociateCustomerToTicketBasedOnAssetAssignment,    [RequireGroupAssignmentOnTickets] = @RequireGroupAssignmentOnTickets,    [AlertContactNoEmail] = @AlertContactNoEmail,    [DisableSupportLogin] = @DisableSupportLogin,    [NoAttachmentsInOutboundExcludeProductLine] = @NoAttachmentsInOutboundExcludeProductLine,    [UseWatson] = @UseWatson, [RequireTwoFactor] = @RequireTwoFactor, [APIRequestMinuteLimit] = @APIRequestMinuteLimit  WHERE ([OrganizationID] = @OrganizationID);";


		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.NVarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("Website", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("WhereHeard", SqlDbType.VarChar, 300);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("PromoCode", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("IsCustomerFree", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("UserSeats", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("PortalSeats", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("ChatSeats", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("ExtraStorageUnits", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 255);
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

		tempParameter = updateCommand.Parameters.Add("IsApiActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("IsApiEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("IsInventoryEnabled", SqlDbType.Bit, 1);
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

		tempParameter = updateCommand.Parameters.Add("InActiveReason", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("HasPortalAccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("IsAdvancedPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("IsBasicPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("PrimaryUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("DefaultPortalGroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("DefaultSupportGroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("DefaultSupportUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("ProductType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("WebServiceID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("SystemEmailID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("ChatID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("PortalGuid", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("CRMLinkID", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("SAExpirationDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}

		tempParameter = updateCommand.Parameters.Add("APIRequestLimit", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}

		tempParameter = updateCommand.Parameters.Add("RequireNewKeyword", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("RequireKnownUserForNewEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("EmailDelimiter", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("OrganizationReplyToAddress", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("CompanyDomains", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("AdminOnlyCustomers", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("AdminOnlyReports", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("ShowWiki", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("DefaultWikiArticleID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("SlaLevelID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("InternalSlaLevelID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("BusinessDays", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("BusinessDayStart", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}

		tempParameter = updateCommand.Parameters.Add("BusinessDayEnd", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}

		tempParameter = updateCommand.Parameters.Add("UseEuropeDate", SqlDbType.Bit, 1);
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

		tempParameter = updateCommand.Parameters.Add("TimedActionsRequired", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("MatchEmailSubject", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

        tempParameter = updateCommand.Parameters.Add("MarkSpam", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("PrimaryInterest", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("PotentialSeats", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("EvalProcess", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("AddAdditionalContacts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("ChangeStatusIfClosed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("IsPublicArticles", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("UseForums", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("SetNewActionsVisibleToCustomers", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("SupportHoursMonth", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("ProductRequired", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("ProductVersionRequired", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("AllowUnsecureAttachmentViewing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("ForceBCCEmailsPrivate", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("UnknownCompanyID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("IsRebuildingIndex", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("LastIndexRebuilt", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}

		tempParameter = updateCommand.Parameters.Add("IsIndexLocked", SqlDbType.Bit, 1);
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

		tempParameter = updateCommand.Parameters.Add("NeedCustForTicketMatch", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("TotalTicketsCreated", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("TicketsOpen", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("CreatedLast30", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("AvgTimeOpen", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("AvgTimeToClose", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("CustDisIndex", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("SlaInitRespAnyAction", SqlDbType.Bit, 1);
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

		tempParameter = updateCommand.Parameters.Add("ShowGroupMembersFirstInTicketAssignmentList", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("UpdateTicketChildrenGroupWithParent", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("ReplyToAlternateEmailAddresses", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("ForceUseOfReplyTo", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("AgentRating", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("AddEmailViaTS", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("SignUpToken", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("IsValidated", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("DateLastIndexed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}

		tempParameter = updateCommand.Parameters.Add("HideDismissNonAdmins", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("CustDistIndexTrend", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = updateCommand.Parameters.Add("UseProductFamilies", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("IsCustomerInsightsActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("TwoStepVerificationEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("DaysBeforePasswordExpire", SqlDbType.Int, 4);
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

		tempParameter = updateCommand.Parameters.Add("NoAttachmentsInOutboundEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("AutoAssignCustomerWithAssetOnTickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("AutoAssociateCustomerToTicketBasedOnAssetAssignment", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("RequireGroupAssignmentOnTickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("AlertContactNoEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("DisableSupportLogin", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("NoAttachmentsInOutboundExcludeProductLine", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("UseWatson", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

        tempParameter = updateCommand.Parameters.Add("RequireTwoFactor", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = updateCommand.Parameters.Add("APIRequestMinuteLimit", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Organizations] (    [Name],    [Description],    [Website],    [WhereHeard],    [PromoCode],    [IsCustomerFree],    [UserSeats],    [PortalSeats],    [ChatSeats],    [ExtraStorageUnits],    [ImportID],    [IsActive],    [IsApiActive],    [IsApiEnabled],    [IsInventoryEnabled],    [TimeZoneID],    [InActiveReason],    [HasPortalAccess],    [IsAdvancedPortal],    [IsBasicPortal],    [PrimaryUserID],    [DefaultPortalGroupID],    [DefaultSupportGroupID],    [DefaultSupportUserID],    [ProductType],    [ParentID],    [WebServiceID],    [SystemEmailID],    [ChatID],    [PortalGuid],    [CRMLinkID],    [SAExpirationDate],    [APIRequestLimit],    [DateCreated],    [DateModified],    [RequireNewKeyword],    [RequireKnownUserForNewEmail],    [EmailDelimiter],    [OrganizationReplyToAddress],    [CompanyDomains],    [AdminOnlyCustomers],    [AdminOnlyReports],    [ShowWiki],    [DefaultWikiArticleID],    [SlaLevelID],    [InternalSlaLevelID],    [BusinessDays],    [BusinessDayStart],    [BusinessDayEnd],    [UseEuropeDate],    [CultureName],    [TimedActionsRequired],    [MatchEmailSubject], [MarkSpam], [CreatorID],    [ModifierID],    [PrimaryInterest],    [PotentialSeats],    [EvalProcess],    [AddAdditionalContacts],    [ChangeStatusIfClosed],    [IsPublicArticles],    [UseForums],    [SetNewActionsVisibleToCustomers],    [SupportHoursMonth],    [ProductRequired],    [ProductVersionRequired],    [AllowUnsecureAttachmentViewing],    [ForceBCCEmailsPrivate],    [UnknownCompanyID],    [IsRebuildingIndex],    [LastIndexRebuilt],    [IsIndexLocked],    [NeedsIndexing],    [NeedCustForTicketMatch],    [TotalTicketsCreated],    [TicketsOpen],    [CreatedLast30],    [AvgTimeOpen],    [AvgTimeToClose],    [CustDisIndex],    [SlaInitRespAnyAction],    [FontFamily],    [FontSize],    [ShowGroupMembersFirstInTicketAssignmentList],    [UpdateTicketChildrenGroupWithParent],    [ReplyToAlternateEmailAddresses],    [ForceUseOfReplyTo],    [AgentRating],    [AddEmailViaTS],    [SignUpToken],    [IsValidated],    [DateLastIndexed],    [HideDismissNonAdmins],    [CustDistIndexTrend],    [UseProductFamilies],    [IsCustomerInsightsActive],    [TwoStepVerificationEnabled],    [DaysBeforePasswordExpire],    [ImportFileID],    [NoAttachmentsInOutboundEmail],    [AutoAssignCustomerWithAssetOnTickets],    [AutoAssociateCustomerToTicketBasedOnAssetAssignment],    [RequireGroupAssignmentOnTickets],    [AlertContactNoEmail],    [DisableSupportLogin],    [NoAttachmentsInOutboundExcludeProductLine],    [UseWatson], [RequireTwoFactor], [APIRequestMinuteLimit]) VALUES ( @Name, @Description, @Website, @WhereHeard, @PromoCode, @IsCustomerFree, @UserSeats, @PortalSeats, @ChatSeats, @ExtraStorageUnits, @ImportID, @IsActive, @IsApiActive, @IsApiEnabled, @IsInventoryEnabled, @TimeZoneID, @InActiveReason, @HasPortalAccess, @IsAdvancedPortal, @IsBasicPortal, @PrimaryUserID, @DefaultPortalGroupID, @DefaultSupportGroupID, @DefaultSupportUserID, @ProductType, @ParentID, @WebServiceID, @SystemEmailID, @ChatID, @PortalGuid, @CRMLinkID, @SAExpirationDate, @APIRequestLimit, @DateCreated, @DateModified, @RequireNewKeyword, @RequireKnownUserForNewEmail, @EmailDelimiter, @OrganizationReplyToAddress, @CompanyDomains, @AdminOnlyCustomers, @AdminOnlyReports, @ShowWiki, @DefaultWikiArticleID, @SlaLevelID, @InternalSlaLevelID, @BusinessDays, @BusinessDayStart, @BusinessDayEnd, @UseEuropeDate, @CultureName, @TimedActionsRequired, @MatchEmailSubject, @MarkSpam, @CreatorID, @ModifierID, @PrimaryInterest, @PotentialSeats, @EvalProcess, @AddAdditionalContacts, @ChangeStatusIfClosed, @IsPublicArticles, @UseForums, @SetNewActionsVisibleToCustomers, @SupportHoursMonth, @ProductRequired, @ProductVersionRequired, @AllowUnsecureAttachmentViewing, @ForceBCCEmailsPrivate, @UnknownCompanyID, @IsRebuildingIndex, @LastIndexRebuilt, @IsIndexLocked, @NeedsIndexing, @NeedCustForTicketMatch, @TotalTicketsCreated, @TicketsOpen, @CreatedLast30, @AvgTimeOpen, @AvgTimeToClose, @CustDisIndex, @SlaInitRespAnyAction, @FontFamily, @FontSize, @ShowGroupMembersFirstInTicketAssignmentList, @UpdateTicketChildrenGroupWithParent, @ReplyToAlternateEmailAddresses, @ForceUseOfReplyTo, @AgentRating, @AddEmailViaTS, @SignUpToken, @IsValidated, @DateLastIndexed, @HideDismissNonAdmins, @CustDistIndexTrend, @UseProductFamilies, @IsCustomerInsightsActive, @TwoStepVerificationEnabled, @DaysBeforePasswordExpire, @ImportFileID, @NoAttachmentsInOutboundEmail, @AutoAssignCustomerWithAssetOnTickets, @AutoAssociateCustomerToTicketBasedOnAssetAssignment, @RequireGroupAssignmentOnTickets, @AlertContactNoEmail, @DisableSupportLogin, @NoAttachmentsInOutboundExcludeProductLine, @UseWatson, @RequireTwoFactor, @APIRequestMinuteLimit); SET @Identity = SCOPE_IDENTITY();";

		tempParameter = insertCommand.Parameters.Add("APIRequestMinuteLimit", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("UseWatson", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

        tempParameter = insertCommand.Parameters.Add("RequireTwoFactor", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float) {
		    tempParameter.Precision = 255;
		    tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("NoAttachmentsInOutboundExcludeProductLine", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("DisableSupportLogin", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("AlertContactNoEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("RequireGroupAssignmentOnTickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("AutoAssociateCustomerToTicketBasedOnAssetAssignment", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("AutoAssignCustomerWithAssetOnTickets", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("NoAttachmentsInOutboundEmail", SqlDbType.Bit, 1);
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

		tempParameter = insertCommand.Parameters.Add("DaysBeforePasswordExpire", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("TwoStepVerificationEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("IsCustomerInsightsActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("UseProductFamilies", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("CustDistIndexTrend", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("HideDismissNonAdmins", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("DateLastIndexed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}

		tempParameter = insertCommand.Parameters.Add("IsValidated", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("SignUpToken", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("AddEmailViaTS", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("AgentRating", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("ForceUseOfReplyTo", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("ReplyToAlternateEmailAddresses", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("UpdateTicketChildrenGroupWithParent", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("ShowGroupMembersFirstInTicketAssignmentList", SqlDbType.Bit, 1);
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

		tempParameter = insertCommand.Parameters.Add("SlaInitRespAnyAction", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("CustDisIndex", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("AvgTimeToClose", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("AvgTimeOpen", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("CreatedLast30", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("TicketsOpen", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("TotalTicketsCreated", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("NeedCustForTicketMatch", SqlDbType.Bit, 1);
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

		tempParameter = insertCommand.Parameters.Add("IsIndexLocked", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("LastIndexRebuilt", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}

		tempParameter = insertCommand.Parameters.Add("IsRebuildingIndex", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("UnknownCompanyID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("ForceBCCEmailsPrivate", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("AllowUnsecureAttachmentViewing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("ProductVersionRequired", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("ProductRequired", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("SupportHoursMonth", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("SetNewActionsVisibleToCustomers", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("UseForums", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("IsPublicArticles", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("ChangeStatusIfClosed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("AddAdditionalContacts", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("EvalProcess", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("PotentialSeats", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("PrimaryInterest", SqlDbType.VarChar, 100);
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

		tempParameter = insertCommand.Parameters.Add("MatchEmailSubject", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

        tempParameter = insertCommand.Parameters.Add("MarkSpam", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("TimedActionsRequired", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("CultureName", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("UseEuropeDate", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("BusinessDayEnd", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}

		tempParameter = insertCommand.Parameters.Add("BusinessDayStart", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}

		tempParameter = insertCommand.Parameters.Add("BusinessDays", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("InternalSlaLevelID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("SlaLevelID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("DefaultWikiArticleID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("ShowWiki", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("AdminOnlyReports", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("AdminOnlyCustomers", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("CompanyDomains", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("OrganizationReplyToAddress", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("EmailDelimiter", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("RequireKnownUserForNewEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("RequireNewKeyword", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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

		tempParameter = insertCommand.Parameters.Add("APIRequestLimit", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("SAExpirationDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}

		tempParameter = insertCommand.Parameters.Add("CRMLinkID", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("PortalGuid", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("ChatID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("SystemEmailID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("WebServiceID", SqlDbType.UniqueIdentifier, 16);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("ProductType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("DefaultSupportUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("DefaultSupportGroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("DefaultPortalGroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("PrimaryUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("IsBasicPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("IsAdvancedPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("HasPortalAccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("InActiveReason", SqlDbType.VarChar, 1000);
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

		tempParameter = insertCommand.Parameters.Add("IsInventoryEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("IsApiEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("IsApiActive", SqlDbType.Bit, 1);
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

		tempParameter = insertCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("ExtraStorageUnits", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("ChatSeats", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("PortalSeats", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("UserSeats", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}

		tempParameter = insertCommand.Parameters.Add("IsCustomerFree", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("PromoCode", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("WhereHeard", SqlDbType.VarChar, 300);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}

		tempParameter = insertCommand.Parameters.Add("Website", SqlDbType.VarChar, 1000);
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

		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.NVarChar, 255);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Organizations] WHERE ([OrganizationID] = @OrganizationID);";
		deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);

		try
		{
		  foreach (Organization organization in this)
		  {
			if (organization.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(organization);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = organization.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["OrganizationID"].AutoIncrement = false;
			  Table.Columns["OrganizationID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				organization.Row["OrganizationID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(organization);
			}
			else if (organization.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(organization);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = organization.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(organization);
			}
			else if (organization.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)organization.Row["OrganizationID", DataRowVersion.Original];
			  deleteCommand.Parameters["OrganizationID"].Value = id;
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

      foreach (Organization organization in this)
      {
        if (organization.Row.Table.Columns.Contains("CreatorID") && (int)organization.Row["CreatorID"] == 0) organization.Row["CreatorID"] = LoginUser.UserID;
        if (organization.Row.Table.Columns.Contains("ModifierID")) organization.Row["ModifierID"] = LoginUser.UserID;
      }

      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();

      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Organization FindByOrganizationID(int organizationID)
    {
      foreach (Organization organization in this)
      {
        if (organization.OrganizationID == organizationID)
        {
          return organization;
        }
      }
      return null;
    }

    public virtual Organization AddNewOrganization()
    {
      if (Table.Columns.Count < 1) LoadColumns("Organizations");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Organization(row, this);
    }

    public virtual void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [OrganizationID], [Name], [Description], [Website], [WhereHeard], [PromoCode], [IsCustomerFree], [UserSeats], [PortalSeats], [ChatSeats], [ExtraStorageUnits], [ImportID], [IsActive], [IsApiActive], [IsApiEnabled], [IsInventoryEnabled], [TimeZoneID], [InActiveReason], [HasPortalAccess], [IsAdvancedPortal], [IsBasicPortal], [PrimaryUserID], [DefaultPortalGroupID], [DefaultSupportGroupID], [DefaultSupportUserID], [ProductType], [ParentID], [WebServiceID], [SystemEmailID], [ChatID], [PortalGuid], [CRMLinkID], [SAExpirationDate], [APIRequestLimit], [DateCreated], [DateModified], [RequireNewKeyword], [RequireKnownUserForNewEmail], [EmailDelimiter], [OrganizationReplyToAddress], [CompanyDomains], [AdminOnlyCustomers], [AdminOnlyReports], [ShowWiki], [DefaultWikiArticleID], [SlaLevelID], [InternalSlaLevelID], [BusinessDays], [BusinessDayStart], [BusinessDayEnd], [UseEuropeDate], [CultureName], [TimedActionsRequired], [MatchEmailSubject], [MarkSpam], [CreatorID], [ModifierID], [PrimaryInterest], [PotentialSeats], [EvalProcess], [AddAdditionalContacts], [ChangeStatusIfClosed], [IsPublicArticles], [UseForums], [SetNewActionsVisibleToCustomers], [SupportHoursMonth], [ProductRequired], [ProductVersionRequired], [AllowUnsecureAttachmentViewing], [ForceBCCEmailsPrivate], [UnknownCompanyID], [IsRebuildingIndex], [LastIndexRebuilt], [IsIndexLocked], [NeedsIndexing], [NeedCustForTicketMatch], [TotalTicketsCreated], [TicketsOpen], [CreatedLast30], [AvgTimeOpen], [AvgTimeToClose], [CustDisIndex], [SlaInitRespAnyAction], [FontFamily], [FontSize], [ShowGroupMembersFirstInTicketAssignmentList], [UpdateTicketChildrenGroupWithParent], [ReplyToAlternateEmailAddresses], [ForceUseOfReplyTo], [AgentRating], [AddEmailViaTS], [SignUpToken], [IsValidated], [DateLastIndexed], [HideDismissNonAdmins], [CustDistIndexTrend], [UseProductFamilies], [IsCustomerInsightsActive], [TwoStepVerificationEnabled], [DaysBeforePasswordExpire], [ImportFileID], [NoAttachmentsInOutboundEmail], [AutoAssignCustomerWithAssetOnTickets], [AutoAssociateCustomerToTicketBasedOnAssetAssignment], [RequireGroupAssignmentOnTickets], [AlertContactNoEmail], [DisableSupportLogin], [NoAttachmentsInOutboundExcludeProductLine], [UseWatson], [RequireTwoFactor], [APIRequestMinuteLimit] FROM [dbo].[Organizations] WHERE ([OrganizationID] = @OrganizationID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }

    public static Organization GetOrganization(LoginUser loginUser, int organizationID)
    {
      Organizations organizations = new Organizations(loginUser);
      organizations.LoadByOrganizationID(organizationID);
      if (organizations.IsEmpty)
        return null;
      else
        return organizations[0];
    }




    #endregion

    #region IEnumerable<Organization> Members

    public IEnumerator<Organization> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Organization(row, this);
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
