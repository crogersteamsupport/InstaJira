using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ReportTicketsViewItem : BaseItem
  {
    private ReportTicketsView _reportTicketsView;
    
    public ReportTicketsViewItem(DataRow row, ReportTicketsView reportTicketsView): base(row, reportTicketsView)
    {
      _reportTicketsView = reportTicketsView;
    }
	
    #region Properties
    
    public ReportTicketsView Collection
    {
      get { return _reportTicketsView; }
    }
        
    
    public string UserName
    {
      get { return Row["UserName"] != DBNull.Value ? (string)Row["UserName"] : null; }
    }
    
    public int? DaysOpened
    {
      get { return Row["DaysOpened"] != DBNull.Value ? (int?)Row["DaysOpened"] : null; }
    }
    
    public int? MinutesOpened
    {
      get { return Row["MinutesOpened"] != DBNull.Value ? (int?)Row["MinutesOpened"] : null; }
    }
    
    public string CloserName
    {
      get { return Row["CloserName"] != DBNull.Value ? (string)Row["CloserName"] : null; }
    }
    
    public string CreatorName
    {
      get { return Row["CreatorName"] != DBNull.Value ? (string)Row["CreatorName"] : null; }
    }
    
    public string ModifierName
    {
      get { return Row["ModifierName"] != DBNull.Value ? (string)Row["ModifierName"] : null; }
    }
    
    public double? HoursSpent
    {
      get { return Row["HoursSpent"] != DBNull.Value ? (double?)Row["HoursSpent"] : null; }
    }
    
    public string Tags
    {
      get { return Row["Tags"] != DBNull.Value ? (string)Row["Tags"] : null; }
    }
    
    public int? SlaViolationTime
    {
      get { return Row["SlaViolationTime"] != DBNull.Value ? (int?)Row["SlaViolationTime"] : null; }
    }
    
    public int? SlaWarningTime
    {
      get { return Row["SlaWarningTime"] != DBNull.Value ? (int?)Row["SlaWarningTime"] : null; }
    }
    
    public double? SlaViolationHours
    {
      get { return Row["SlaViolationHours"] != DBNull.Value ? (double?)Row["SlaViolationHours"] : null; }
    }
    
    public double? SlaWarningHours
    {
      get { return Row["SlaWarningHours"] != DBNull.Value ? (double?)Row["SlaWarningHours"] : null; }
    }
    
    public int? MinsSinceCreated
    {
      get { return Row["MinsSinceCreated"] != DBNull.Value ? (int?)Row["MinsSinceCreated"] : null; }
    }
    
    public int? DaysSinceCreated
    {
      get { return Row["DaysSinceCreated"] != DBNull.Value ? (int?)Row["DaysSinceCreated"] : null; }
    }
    
    public int? MinsSinceModified
    {
      get { return Row["MinsSinceModified"] != DBNull.Value ? (int?)Row["MinsSinceModified"] : null; }
    }
    
    public int? DaysSinceModified
    {
      get { return Row["DaysSinceModified"] != DBNull.Value ? (int?)Row["DaysSinceModified"] : null; }
    }
    
    public string Contacts
    {
      get { return Row["Contacts"] != DBNull.Value ? (string)Row["Contacts"] : null; }
    }
    
    public string Customers
    {
      get { return Row["Customers"] != DBNull.Value ? (string)Row["Customers"] : null; }
    }
    
    public int? ChildTicketCount
    {
      get { return Row["ChildTicketCount"] != DBNull.Value ? (int?)Row["ChildTicketCount"] : null; }
    }
    
    public int? RelatedTicketCount
    {
      get { return Row["RelatedTicketCount"] != DBNull.Value ? (int?)Row["RelatedTicketCount"] : null; }
    }
    
    public int? MinutesToInitialResponse
    {
      get { return Row["MinutesToInitialResponse"] != DBNull.Value ? (int?)Row["MinutesToInitialResponse"] : null; }
    }
    
    public string DayOfWeekCreated
    {
      get { return Row["DayOfWeekCreated"] != DBNull.Value ? (string)Row["DayOfWeekCreated"] : null; }
    }
    
    public int? HourOfDayCreated
    {
      get { return Row["HourOfDayCreated"] != DBNull.Value ? (int?)Row["HourOfDayCreated"] : null; }
    }
    
    
    
    public string TicketSource
    {
      get { return (string)Row["TicketSource"]; }
    }
    
    public int MinutesClosed
    {
      get { return (int)Row["MinutesClosed"]; }
    }
    
    public int DaysClosed
    {
      get { return (int)Row["DaysClosed"]; }
    }
    
    public bool IsClosed
    {
      get { return (bool)Row["IsClosed"]; }
    }
    

    
    public string ProductName
    {
      get { return Row["ProductName"] != DBNull.Value ? (string)Row["ProductName"] : null; }
      set { Row["ProductName"] = CheckValue("ProductName", value); }
    }
    
    public string ReportedVersion
    {
      get { return Row["ReportedVersion"] != DBNull.Value ? (string)Row["ReportedVersion"] : null; }
      set { Row["ReportedVersion"] = CheckValue("ReportedVersion", value); }
    }
    
    public string SolvedVersion
    {
      get { return Row["SolvedVersion"] != DBNull.Value ? (string)Row["SolvedVersion"] : null; }
      set { Row["SolvedVersion"] = CheckValue("SolvedVersion", value); }
    }
    
    public string GroupName
    {
      get { return Row["GroupName"] != DBNull.Value ? (string)Row["GroupName"] : null; }
      set { Row["GroupName"] = CheckValue("GroupName", value); }
    }
    
    public string TicketTypeName
    {
      get { return Row["TicketTypeName"] != DBNull.Value ? (string)Row["TicketTypeName"] : null; }
      set { Row["TicketTypeName"] = CheckValue("TicketTypeName", value); }
    }
    
    public string Status
    {
      get { return Row["Status"] != DBNull.Value ? (string)Row["Status"] : null; }
      set { Row["Status"] = CheckValue("Status", value); }
    }
    
    public int? StatusPosition
    {
      get { return Row["StatusPosition"] != DBNull.Value ? (int?)Row["StatusPosition"] : null; }
      set { Row["StatusPosition"] = CheckValue("StatusPosition", value); }
    }
    
    public int? SeverityPosition
    {
      get { return Row["SeverityPosition"] != DBNull.Value ? (int?)Row["SeverityPosition"] : null; }
      set { Row["SeverityPosition"] = CheckValue("SeverityPosition", value); }
    }
    
    public string Severity
    {
      get { return Row["Severity"] != DBNull.Value ? (string)Row["Severity"] : null; }
      set { Row["Severity"] = CheckValue("Severity", value); }
    }
    
    public int? ReportedVersionID
    {
      get { return Row["ReportedVersionID"] != DBNull.Value ? (int?)Row["ReportedVersionID"] : null; }
      set { Row["ReportedVersionID"] = CheckValue("ReportedVersionID", value); }
    }
    
    public int? SolvedVersionID
    {
      get { return Row["SolvedVersionID"] != DBNull.Value ? (int?)Row["SolvedVersionID"] : null; }
      set { Row["SolvedVersionID"] = CheckValue("SolvedVersionID", value); }
    }
    
    public int? ProductID
    {
      get { return Row["ProductID"] != DBNull.Value ? (int?)Row["ProductID"] : null; }
      set { Row["ProductID"] = CheckValue("ProductID", value); }
    }
    
    public int? GroupID
    {
      get { return Row["GroupID"] != DBNull.Value ? (int?)Row["GroupID"] : null; }
      set { Row["GroupID"] = CheckValue("GroupID", value); }
    }
    
    public int? UserID
    {
      get { return Row["UserID"] != DBNull.Value ? (int?)Row["UserID"] : null; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    
    public int? ParentID
    {
      get { return Row["ParentID"] != DBNull.Value ? (int?)Row["ParentID"] : null; }
      set { Row["ParentID"] = CheckValue("ParentID", value); }
    }
    
    public int? CloserID
    {
      get { return Row["CloserID"] != DBNull.Value ? (int?)Row["CloserID"] : null; }
      set { Row["CloserID"] = CheckValue("CloserID", value); }
    }
    
    public int? ForumCategory
    {
      get { return Row["ForumCategory"] != DBNull.Value ? (int?)Row["ForumCategory"] : null; }
      set { Row["ForumCategory"] = CheckValue("ForumCategory", value); }
    }
    
    public string CategoryName
    {
      get { return Row["CategoryName"] != DBNull.Value ? (string)Row["CategoryName"] : null; }
      set { Row["CategoryName"] = CheckValue("CategoryName", value); }
    }
    
    public string CreatorEmail
    {
      get { return Row["CreatorEmail"] != DBNull.Value ? (string)Row["CreatorEmail"] : null; }
      set { Row["CreatorEmail"] = CheckValue("CreatorEmail", value); }
    }
    
    public string ModifierEmail
    {
      get { return Row["ModifierEmail"] != DBNull.Value ? (string)Row["ModifierEmail"] : null; }
      set { Row["ModifierEmail"] = CheckValue("ModifierEmail", value); }
    }
    
    public int? KnowledgeBaseCategoryID
    {
      get { return Row["KnowledgeBaseCategoryID"] != DBNull.Value ? (int?)Row["KnowledgeBaseCategoryID"] : null; }
      set { Row["KnowledgeBaseCategoryID"] = CheckValue("KnowledgeBaseCategoryID", value); }
    }
    
    public string KnowledgeBaseCategoryName
    {
      get { return Row["KnowledgeBaseCategoryName"] != DBNull.Value ? (string)Row["KnowledgeBaseCategoryName"] : null; }
      set { Row["KnowledgeBaseCategoryName"] = CheckValue("KnowledgeBaseCategoryName", value); }
    }
    
    public string KnowledgeBaseParentCategoryName
    {
      get { return Row["KnowledgeBaseParentCategoryName"] != DBNull.Value ? (string)Row["KnowledgeBaseParentCategoryName"] : null; }
      set { Row["KnowledgeBaseParentCategoryName"] = CheckValue("KnowledgeBaseParentCategoryName", value); }
    }
    
    public string SalesForceID
    {
      get { return Row["SalesForceID"] != DBNull.Value ? (string)Row["SalesForceID"] : null; }
      set { Row["SalesForceID"] = CheckValue("SalesForceID", value); }
    }
    
    public int? JiraID
    {
      get { return Row["JiraID"] != DBNull.Value ? (int?)Row["JiraID"] : null; }
      set { Row["JiraID"] = CheckValue("JiraID", value); }
    }
    
    public bool? SyncWithJira
    {
      get { return Row["SyncWithJira"] != DBNull.Value ? (bool?)Row["SyncWithJira"] : null; }
      set { Row["SyncWithJira"] = CheckValue("SyncWithJira", value); }
    }
    
    public string JiraKey
    {
      get { return Row["JiraKey"] != DBNull.Value ? (string)Row["JiraKey"] : null; }
      set { Row["JiraKey"] = CheckValue("JiraKey", value); }
    }
    
    public string JiraLinkURL
    {
      get { return Row["JiraLinkURL"] != DBNull.Value ? (string)Row["JiraLinkURL"] : null; }
      set { Row["JiraLinkURL"] = CheckValue("JiraLinkURL", value); }
    }
    
    public string JiraStatus
    {
      get { return Row["JiraStatus"] != DBNull.Value ? (string)Row["JiraStatus"] : null; }
      set { Row["JiraStatus"] = CheckValue("JiraStatus", value); }
    }
    

    
    public bool NeedsIndexing
    {
      get { return (bool)Row["NeedsIndexing"]; }
      set { Row["NeedsIndexing"] = CheckValue("NeedsIndexing", value); }
    }
    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public int ModifierID
    {
      get { return (int)Row["ModifierID"]; }
      set { Row["ModifierID"] = CheckValue("ModifierID", value); }
    }
    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckValue("Name", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public int TicketSeverityID
    {
      get { return (int)Row["TicketSeverityID"]; }
      set { Row["TicketSeverityID"] = CheckValue("TicketSeverityID", value); }
    }
    
    public int TicketTypeID
    {
      get { return (int)Row["TicketTypeID"]; }
      set { Row["TicketTypeID"] = CheckValue("TicketTypeID", value); }
    }
    
    public int TicketStatusID
    {
      get { return (int)Row["TicketStatusID"]; }
      set { Row["TicketStatusID"] = CheckValue("TicketStatusID", value); }
    }
    
    public bool IsKnowledgeBase
    {
      get { return (bool)Row["IsKnowledgeBase"]; }
      set { Row["IsKnowledgeBase"] = CheckValue("IsKnowledgeBase", value); }
    }
    
    public bool IsVisibleOnPortal
    {
      get { return (bool)Row["IsVisibleOnPortal"]; }
      set { Row["IsVisibleOnPortal"] = CheckValue("IsVisibleOnPortal", value); }
    }
    
    public int TicketNumber
    {
      get { return (int)Row["TicketNumber"]; }
      set { Row["TicketNumber"] = CheckValue("TicketNumber", value); }
    }
    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    

    /* DateTime */
    
    public DateTime? SlaViolationDate
    {
      get { return Row["SlaViolationDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["SlaViolationDate"]) : null; }
    }

    public DateTime? SlaViolationDateUtc
    {
      get { return Row["SlaViolationDate"] != DBNull.Value ? (DateTime?)Row["SlaViolationDate"] : null; }
    }
    
    public DateTime? SlaWarningDate
    {
      get { return Row["SlaWarningDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["SlaWarningDate"]) : null; }
    }

    public DateTime? SlaWarningDateUtc
    {
      get { return Row["SlaWarningDate"] != DBNull.Value ? (DateTime?)Row["SlaWarningDate"] : null; }
    }
    
    
    

    
    public DateTime? DateClosed
    {
      get { return Row["DateClosed"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateClosed"]) : null; }
      set { Row["DateClosed"] = CheckValue("DateClosed", value); }
    }

    public DateTime? DateClosedUtc
    {
      get { return Row["DateClosed"] != DBNull.Value ? (DateTime?)Row["DateClosed"] : null; }
    }
    
    public DateTime? SlaViolationTimeClosed
    {
      get { return Row["SlaViolationTimeClosed"] != DBNull.Value ? DateToLocal((DateTime?)Row["SlaViolationTimeClosed"]) : null; }
      set { Row["SlaViolationTimeClosed"] = CheckValue("SlaViolationTimeClosed", value); }
    }

    public DateTime? SlaViolationTimeClosedUtc
    {
      get { return Row["SlaViolationTimeClosed"] != DBNull.Value ? (DateTime?)Row["SlaViolationTimeClosed"] : null; }
    }
    
    public DateTime? SlaViolationLastAction
    {
      get { return Row["SlaViolationLastAction"] != DBNull.Value ? DateToLocal((DateTime?)Row["SlaViolationLastAction"]) : null; }
      set { Row["SlaViolationLastAction"] = CheckValue("SlaViolationLastAction", value); }
    }

    public DateTime? SlaViolationLastActionUtc
    {
      get { return Row["SlaViolationLastAction"] != DBNull.Value ? (DateTime?)Row["SlaViolationLastAction"] : null; }
    }
    
    public DateTime? SlaViolationInitialResponse
    {
      get { return Row["SlaViolationInitialResponse"] != DBNull.Value ? DateToLocal((DateTime?)Row["SlaViolationInitialResponse"]) : null; }
      set { Row["SlaViolationInitialResponse"] = CheckValue("SlaViolationInitialResponse", value); }
    }

    public DateTime? SlaViolationInitialResponseUtc
    {
      get { return Row["SlaViolationInitialResponse"] != DBNull.Value ? (DateTime?)Row["SlaViolationInitialResponse"] : null; }
    }
    
    public DateTime? SlaWarningTimeClosed
    {
      get { return Row["SlaWarningTimeClosed"] != DBNull.Value ? DateToLocal((DateTime?)Row["SlaWarningTimeClosed"]) : null; }
      set { Row["SlaWarningTimeClosed"] = CheckValue("SlaWarningTimeClosed", value); }
    }

    public DateTime? SlaWarningTimeClosedUtc
    {
      get { return Row["SlaWarningTimeClosed"] != DBNull.Value ? (DateTime?)Row["SlaWarningTimeClosed"] : null; }
    }
    
    public DateTime? SlaWarningLastAction
    {
      get { return Row["SlaWarningLastAction"] != DBNull.Value ? DateToLocal((DateTime?)Row["SlaWarningLastAction"]) : null; }
      set { Row["SlaWarningLastAction"] = CheckValue("SlaWarningLastAction", value); }
    }

    public DateTime? SlaWarningLastActionUtc
    {
      get { return Row["SlaWarningLastAction"] != DBNull.Value ? (DateTime?)Row["SlaWarningLastAction"] : null; }
    }
    
    public DateTime? SlaWarningInitialResponse
    {
      get { return Row["SlaWarningInitialResponse"] != DBNull.Value ? DateToLocal((DateTime?)Row["SlaWarningInitialResponse"]) : null; }
      set { Row["SlaWarningInitialResponse"] = CheckValue("SlaWarningInitialResponse", value); }
    }

    public DateTime? SlaWarningInitialResponseUtc
    {
      get { return Row["SlaWarningInitialResponse"] != DBNull.Value ? (DateTime?)Row["SlaWarningInitialResponse"] : null; }
    }
    
    public DateTime? DueDate
    {
      get { return Row["DueDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["DueDate"]) : null; }
      set { Row["DueDate"] = CheckValue("DueDate", value); }
    }

    public DateTime? DueDateUtc
    {
      get { return Row["DueDate"] != DBNull.Value ? (DateTime?)Row["DueDate"] : null; }
    }
    
    public DateTime? DateModifiedBySalesForceSync
    {
      get { return Row["DateModifiedBySalesForceSync"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateModifiedBySalesForceSync"]) : null; }
      set { Row["DateModifiedBySalesForceSync"] = CheckValue("DateModifiedBySalesForceSync", value); }
    }

    public DateTime? DateModifiedBySalesForceSyncUtc
    {
      get { return Row["DateModifiedBySalesForceSync"] != DBNull.Value ? (DateTime?)Row["DateModifiedBySalesForceSync"] : null; }
    }
    
    public DateTime? DateModifiedByJiraSync
    {
      get { return Row["DateModifiedByJiraSync"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateModifiedByJiraSync"]) : null; }
      set { Row["DateModifiedByJiraSync"] = CheckValue("DateModifiedByJiraSync", value); }
    }

    public DateTime? DateModifiedByJiraSyncUtc
    {
      get { return Row["DateModifiedByJiraSync"] != DBNull.Value ? (DateTime?)Row["DateModifiedByJiraSync"] : null; }
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
    
    public DateTime DateModified
    {
      get { return DateToLocal((DateTime)Row["DateModified"]); }
      set { Row["DateModified"] = CheckValue("DateModified", value); }
    }

    public DateTime DateModifiedUtc
    {
      get { return (DateTime)Row["DateModified"]; }
    }
    

    #endregion
    
    
  }

  public partial class ReportTicketsView : BaseCollection, IEnumerable<ReportTicketsViewItem>
  {
    public ReportTicketsView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ReportTicketsView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketID"; }
    }



    public ReportTicketsViewItem this[int index]
    {
      get { return new ReportTicketsViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ReportTicketsViewItem reportTicketsViewItem);
    partial void AfterRowInsert(ReportTicketsViewItem reportTicketsViewItem);
    partial void BeforeRowEdit(ReportTicketsViewItem reportTicketsViewItem);
    partial void AfterRowEdit(ReportTicketsViewItem reportTicketsViewItem);
    partial void BeforeRowDelete(int ticketID);
    partial void AfterRowDelete(int ticketID);    

    partial void BeforeDBDelete(int ticketID);
    partial void AfterDBDelete(int ticketID);    

    #endregion

    #region Public Methods

    public ReportTicketsViewItemProxy[] GetReportTicketsViewItemProxies()
    {
      List<ReportTicketsViewItemProxy> list = new List<ReportTicketsViewItemProxy>();

      foreach (ReportTicketsViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketID)
    {
      BeforeDBDelete(ticketID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ReportTicketsView] WHERE ([TicketID] = @TicketID);";
        deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);
        deleteCommand.Parameters["TicketID"].Value = ticketID;

        BeforeRowDelete(ticketID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(ticketID);
      }
      AfterDBDelete(ticketID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ReportTicketsViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ReportTicketsView] SET     [ProductName] = @ProductName,    [ReportedVersion] = @ReportedVersion,    [SolvedVersion] = @SolvedVersion,    [GroupName] = @GroupName,    [TicketTypeName] = @TicketTypeName,    [UserName] = @UserName,    [Status] = @Status,    [StatusPosition] = @StatusPosition,    [SeverityPosition] = @SeverityPosition,    [IsClosed] = @IsClosed,    [Severity] = @Severity,    [TicketNumber] = @TicketNumber,    [IsVisibleOnPortal] = @IsVisibleOnPortal,    [IsKnowledgeBase] = @IsKnowledgeBase,    [ReportedVersionID] = @ReportedVersionID,    [SolvedVersionID] = @SolvedVersionID,    [ProductID] = @ProductID,    [GroupID] = @GroupID,    [UserID] = @UserID,    [TicketStatusID] = @TicketStatusID,    [TicketTypeID] = @TicketTypeID,    [TicketSeverityID] = @TicketSeverityID,    [OrganizationID] = @OrganizationID,    [Name] = @Name,    [ParentID] = @ParentID,    [ModifierID] = @ModifierID,    [DateModified] = @DateModified,    [DateClosed] = @DateClosed,    [CloserID] = @CloserID,    [DaysClosed] = @DaysClosed,    [MinutesClosed] = @MinutesClosed,    [DaysOpened] = @DaysOpened,    [MinutesOpened] = @MinutesOpened,    [CloserName] = @CloserName,    [CreatorName] = @CreatorName,    [ModifierName] = @ModifierName,    [HoursSpent] = @HoursSpent,    [Tags] = @Tags,    [SlaViolationTime] = @SlaViolationTime,    [SlaWarningTime] = @SlaWarningTime,    [SlaViolationHours] = @SlaViolationHours,    [SlaWarningHours] = @SlaWarningHours,    [MinsSinceCreated] = @MinsSinceCreated,    [DaysSinceCreated] = @DaysSinceCreated,    [MinsSinceModified] = @MinsSinceModified,    [DaysSinceModified] = @DaysSinceModified,    [Contacts] = @Contacts,    [Customers] = @Customers,    [SlaViolationTimeClosed] = @SlaViolationTimeClosed,    [SlaViolationLastAction] = @SlaViolationLastAction,    [SlaViolationInitialResponse] = @SlaViolationInitialResponse,    [SlaWarningTimeClosed] = @SlaWarningTimeClosed,    [SlaWarningLastAction] = @SlaWarningLastAction,    [SlaWarningInitialResponse] = @SlaWarningInitialResponse,    [NeedsIndexing] = @NeedsIndexing,    [SlaViolationDate] = @SlaViolationDate,    [SlaWarningDate] = @SlaWarningDate,    [TicketSource] = @TicketSource,    [ForumCategory] = @ForumCategory,    [CategoryName] = @CategoryName,    [CreatorEmail] = @CreatorEmail,    [ModifierEmail] = @ModifierEmail,    [KnowledgeBaseCategoryID] = @KnowledgeBaseCategoryID,    [KnowledgeBaseCategoryName] = @KnowledgeBaseCategoryName,    [KnowledgeBaseParentCategoryName] = @KnowledgeBaseParentCategoryName,    [DueDate] = @DueDate,    [SalesForceID] = @SalesForceID,    [DateModifiedBySalesForceSync] = @DateModifiedBySalesForceSync,    [DateModifiedByJiraSync] = @DateModifiedByJiraSync,    [JiraID] = @JiraID,    [SyncWithJira] = @SyncWithJira,    [JiraKey] = @JiraKey,    [JiraLinkURL] = @JiraLinkURL,    [JiraStatus] = @JiraStatus,    [ChildTicketCount] = @ChildTicketCount,    [RelatedTicketCount] = @RelatedTicketCount,    [MinutesToInitialResponse] = @MinutesToInitialResponse,    [DayOfWeekCreated] = @DayOfWeekCreated,    [HourOfDayCreated] = @HourOfDayCreated  WHERE ([TicketID] = @TicketID);";

		
		tempParameter = updateCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ProductName", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ReportedVersion", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SolvedVersion", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("GroupName", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketTypeName", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserName", SqlDbType.VarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Status", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("StatusPosition", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SeverityPosition", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsClosed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Severity", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketNumber", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsVisibleOnPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsKnowledgeBase", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ReportedVersionID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SolvedVersionID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("GroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketStatusID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketSeverityID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("DateClosed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("CloserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DaysClosed", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("MinutesClosed", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DaysOpened", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("MinutesOpened", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CloserName", SqlDbType.VarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CreatorName", SqlDbType.VarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierName", SqlDbType.VarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("HoursSpent", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 24;
		  tempParameter.Scale = 24;
		}
		
		tempParameter = updateCommand.Parameters.Add("Tags", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaViolationTime", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaWarningTime", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaViolationHours", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 24;
		  tempParameter.Scale = 24;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaWarningHours", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 24;
		  tempParameter.Scale = 24;
		}
		
		tempParameter = updateCommand.Parameters.Add("MinsSinceCreated", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DaysSinceCreated", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("MinsSinceModified", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DaysSinceModified", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Contacts", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Customers", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaViolationTimeClosed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaViolationLastAction", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaViolationInitialResponse", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaWarningTimeClosed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaWarningLastAction", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaWarningInitialResponse", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("NeedsIndexing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaViolationDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaWarningDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketSource", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ForumCategory", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CategoryName", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CreatorEmail", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierEmail", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("KnowledgeBaseCategoryID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("KnowledgeBaseCategoryName", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("KnowledgeBaseParentCategoryName", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DueDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SalesForceID", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModifiedBySalesForceSync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModifiedByJiraSync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("JiraID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SyncWithJira", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("JiraKey", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("JiraLinkURL", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("JiraStatus", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ChildTicketCount", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RelatedTicketCount", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("MinutesToInitialResponse", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DayOfWeekCreated", SqlDbType.NVarChar, 30);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("HourOfDayCreated", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ReportTicketsView] (    [TicketID],    [ProductName],    [ReportedVersion],    [SolvedVersion],    [GroupName],    [TicketTypeName],    [UserName],    [Status],    [StatusPosition],    [SeverityPosition],    [IsClosed],    [Severity],    [TicketNumber],    [IsVisibleOnPortal],    [IsKnowledgeBase],    [ReportedVersionID],    [SolvedVersionID],    [ProductID],    [GroupID],    [UserID],    [TicketStatusID],    [TicketTypeID],    [TicketSeverityID],    [OrganizationID],    [Name],    [ParentID],    [ModifierID],    [CreatorID],    [DateModified],    [DateCreated],    [DateClosed],    [CloserID],    [DaysClosed],    [MinutesClosed],    [DaysOpened],    [MinutesOpened],    [CloserName],    [CreatorName],    [ModifierName],    [HoursSpent],    [Tags],    [SlaViolationTime],    [SlaWarningTime],    [SlaViolationHours],    [SlaWarningHours],    [MinsSinceCreated],    [DaysSinceCreated],    [MinsSinceModified],    [DaysSinceModified],    [Contacts],    [Customers],    [SlaViolationTimeClosed],    [SlaViolationLastAction],    [SlaViolationInitialResponse],    [SlaWarningTimeClosed],    [SlaWarningLastAction],    [SlaWarningInitialResponse],    [NeedsIndexing],    [SlaViolationDate],    [SlaWarningDate],    [TicketSource],    [ForumCategory],    [CategoryName],    [CreatorEmail],    [ModifierEmail],    [KnowledgeBaseCategoryID],    [KnowledgeBaseCategoryName],    [KnowledgeBaseParentCategoryName],    [DueDate],    [SalesForceID],    [DateModifiedBySalesForceSync],    [DateModifiedByJiraSync],    [JiraID],    [SyncWithJira],    [JiraKey],    [JiraLinkURL],    [JiraStatus],    [ChildTicketCount],    [RelatedTicketCount],    [MinutesToInitialResponse],    [DayOfWeekCreated],    [HourOfDayCreated]) VALUES ( @TicketID, @ProductName, @ReportedVersion, @SolvedVersion, @GroupName, @TicketTypeName, @UserName, @Status, @StatusPosition, @SeverityPosition, @IsClosed, @Severity, @TicketNumber, @IsVisibleOnPortal, @IsKnowledgeBase, @ReportedVersionID, @SolvedVersionID, @ProductID, @GroupID, @UserID, @TicketStatusID, @TicketTypeID, @TicketSeverityID, @OrganizationID, @Name, @ParentID, @ModifierID, @CreatorID, @DateModified, @DateCreated, @DateClosed, @CloserID, @DaysClosed, @MinutesClosed, @DaysOpened, @MinutesOpened, @CloserName, @CreatorName, @ModifierName, @HoursSpent, @Tags, @SlaViolationTime, @SlaWarningTime, @SlaViolationHours, @SlaWarningHours, @MinsSinceCreated, @DaysSinceCreated, @MinsSinceModified, @DaysSinceModified, @Contacts, @Customers, @SlaViolationTimeClosed, @SlaViolationLastAction, @SlaViolationInitialResponse, @SlaWarningTimeClosed, @SlaWarningLastAction, @SlaWarningInitialResponse, @NeedsIndexing, @SlaViolationDate, @SlaWarningDate, @TicketSource, @ForumCategory, @CategoryName, @CreatorEmail, @ModifierEmail, @KnowledgeBaseCategoryID, @KnowledgeBaseCategoryName, @KnowledgeBaseParentCategoryName, @DueDate, @SalesForceID, @DateModifiedBySalesForceSync, @DateModifiedByJiraSync, @JiraID, @SyncWithJira, @JiraKey, @JiraLinkURL, @JiraStatus, @ChildTicketCount, @RelatedTicketCount, @MinutesToInitialResponse, @DayOfWeekCreated, @HourOfDayCreated); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("HourOfDayCreated", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DayOfWeekCreated", SqlDbType.NVarChar, 30);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("MinutesToInitialResponse", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("RelatedTicketCount", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ChildTicketCount", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("JiraStatus", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("JiraLinkURL", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("JiraKey", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SyncWithJira", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("JiraID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModifiedByJiraSync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModifiedBySalesForceSync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("SalesForceID", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DueDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("KnowledgeBaseParentCategoryName", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("KnowledgeBaseCategoryName", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("KnowledgeBaseCategoryID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ModifierEmail", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorEmail", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CategoryName", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ForumCategory", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketSource", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaWarningDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaViolationDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("NeedsIndexing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaWarningInitialResponse", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaWarningLastAction", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaWarningTimeClosed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaViolationInitialResponse", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaViolationLastAction", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaViolationTimeClosed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Customers", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Contacts", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DaysSinceModified", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("MinsSinceModified", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DaysSinceCreated", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("MinsSinceCreated", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaWarningHours", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 24;
		  tempParameter.Scale = 24;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaViolationHours", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 24;
		  tempParameter.Scale = 24;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaWarningTime", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaViolationTime", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Tags", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("HoursSpent", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 24;
		  tempParameter.Scale = 24;
		}
		
		tempParameter = insertCommand.Parameters.Add("ModifierName", SqlDbType.VarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorName", SqlDbType.VarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CloserName", SqlDbType.VarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("MinutesOpened", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DaysOpened", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("MinutesClosed", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DaysClosed", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CloserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateClosed", SqlDbType.DateTime, 8);
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
		
		tempParameter = insertCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 255);
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
		
		tempParameter = insertCommand.Parameters.Add("TicketSeverityID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketStatusID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("GroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("SolvedVersionID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ReportedVersionID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsKnowledgeBase", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsVisibleOnPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketNumber", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Severity", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsClosed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SeverityPosition", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("StatusPosition", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Status", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserName", SqlDbType.VarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketTypeName", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("GroupName", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SolvedVersion", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ReportedVersion", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ProductName", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ReportTicketsView] WHERE ([TicketID] = @TicketID);";
		deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);

		try
		{
		  foreach (ReportTicketsViewItem reportTicketsViewItem in this)
		  {
			if (reportTicketsViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(reportTicketsViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = reportTicketsViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketID"].AutoIncrement = false;
			  Table.Columns["TicketID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				reportTicketsViewItem.Row["TicketID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(reportTicketsViewItem);
			}
			else if (reportTicketsViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(reportTicketsViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = reportTicketsViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(reportTicketsViewItem);
			}
			else if (reportTicketsViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)reportTicketsViewItem.Row["TicketID", DataRowVersion.Original];
			  deleteCommand.Parameters["TicketID"].Value = id;
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

      foreach (ReportTicketsViewItem reportTicketsViewItem in this)
      {
        if (reportTicketsViewItem.Row.Table.Columns.Contains("CreatorID") && (int)reportTicketsViewItem.Row["CreatorID"] == 0) reportTicketsViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (reportTicketsViewItem.Row.Table.Columns.Contains("ModifierID")) reportTicketsViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ReportTicketsViewItem FindByTicketID(int ticketID)
    {
      foreach (ReportTicketsViewItem reportTicketsViewItem in this)
      {
        if (reportTicketsViewItem.TicketID == ticketID)
        {
          return reportTicketsViewItem;
        }
      }
      return null;
    }

    public virtual ReportTicketsViewItem AddNewReportTicketsViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("ReportTicketsView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ReportTicketsViewItem(row, this);
    }
    
    public virtual void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TicketID], [ProductName], [ReportedVersion], [SolvedVersion], [GroupName], [TicketTypeName], [UserName], [Status], [StatusPosition], [SeverityPosition], [IsClosed], [Severity], [TicketNumber], [IsVisibleOnPortal], [IsKnowledgeBase], [ReportedVersionID], [SolvedVersionID], [ProductID], [GroupID], [UserID], [TicketStatusID], [TicketTypeID], [TicketSeverityID], [OrganizationID], [Name], [ParentID], [ModifierID], [CreatorID], [DateModified], [DateCreated], [DateClosed], [CloserID], [DaysClosed], [MinutesClosed], [DaysOpened], [MinutesOpened], [CloserName], [CreatorName], [ModifierName], [HoursSpent], [Tags], [SlaViolationTime], [SlaWarningTime], [SlaViolationHours], [SlaWarningHours], [MinsSinceCreated], [DaysSinceCreated], [MinsSinceModified], [DaysSinceModified], [Contacts], [Customers], [SlaViolationTimeClosed], [SlaViolationLastAction], [SlaViolationInitialResponse], [SlaWarningTimeClosed], [SlaWarningLastAction], [SlaWarningInitialResponse], [NeedsIndexing], [SlaViolationDate], [SlaWarningDate], [TicketSource], [ForumCategory], [CategoryName], [CreatorEmail], [ModifierEmail], [KnowledgeBaseCategoryID], [KnowledgeBaseCategoryName], [KnowledgeBaseParentCategoryName], [DueDate], [SalesForceID], [DateModifiedBySalesForceSync], [DateModifiedByJiraSync], [JiraID], [SyncWithJira], [JiraKey], [JiraLinkURL], [JiraStatus], [ChildTicketCount], [RelatedTicketCount], [MinutesToInitialResponse], [DayOfWeekCreated], [HourOfDayCreated] FROM [dbo].[ReportTicketsView] WHERE ([TicketID] = @TicketID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketID", ticketID);
        Fill(command);
      }
    }
    
    public static ReportTicketsViewItem GetReportTicketsViewItem(LoginUser loginUser, int ticketID)
    {
      ReportTicketsView reportTicketsView = new ReportTicketsView(loginUser);
      reportTicketsView.LoadByTicketID(ticketID);
      if (reportTicketsView.IsEmpty)
        return null;
      else
        return reportTicketsView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ReportTicketsViewItem> Members

    public IEnumerator<ReportTicketsViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ReportTicketsViewItem(row, this);
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
