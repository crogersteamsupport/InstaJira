using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Ticket : BaseItem
  {
    private Tickets _tickets;
    
    public Ticket(DataRow row, Tickets tickets): base(row, tickets)
    {
      _tickets = tickets;
    }
	
    #region Properties
    
    public Tickets Collection
    {
      get { return _tickets; }
    }
        
    
    
    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
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
    
    public string ImportID
    {
      get { return Row["ImportID"] != DBNull.Value ? (string)Row["ImportID"] : null; }
      set { Row["ImportID"] = CheckValue("ImportID", value); }
    }
    
    public string TicketSource
    {
      get { return Row["TicketSource"] != DBNull.Value ? (string)Row["TicketSource"] : null; }
      set { Row["TicketSource"] = CheckValue("TicketSource", value); }
    }
    
    public string PortalEmail
    {
      get { return Row["PortalEmail"] != DBNull.Value ? (string)Row["PortalEmail"] : null; }
      set { Row["PortalEmail"] = CheckValue("PortalEmail", value); }
    }
    
    public int? DocID
    {
      get { return Row["DocID"] != DBNull.Value ? (int?)Row["DocID"] : null; }
      set { Row["DocID"] = CheckValue("DocID", value); }
    }
    
    public int? KnowledgeBaseCategoryID
    {
      get { return Row["KnowledgeBaseCategoryID"] != DBNull.Value ? (int?)Row["KnowledgeBaseCategoryID"] : null; }
      set { Row["KnowledgeBaseCategoryID"] = CheckValue("KnowledgeBaseCategoryID", value); }
    }
    
    public string SalesForceID
    {
      get { return Row["SalesForceID"] != DBNull.Value ? (string)Row["SalesForceID"] : null; }
      set { Row["SalesForceID"] = CheckValue("SalesForceID", value); }
    }
    
    public string JiraStatus
    {
      get { return Row["JiraStatus"] != DBNull.Value ? (string)Row["JiraStatus"] : null; }
      set { Row["JiraStatus"] = CheckValue("JiraStatus", value); }
    }
    
    public bool? SyncWithJira
    {
      get { return Row["SyncWithJira"] != DBNull.Value ? (bool?)Row["SyncWithJira"] : null; }
      set { Row["SyncWithJira"] = CheckValue("SyncWithJira", value); }
    }
    
    public int? JiraID
    {
      get { return Row["JiraID"] != DBNull.Value ? (int?)Row["JiraID"] : null; }
      set { Row["JiraID"] = CheckValue("JiraID", value); }
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
    
    public string EmailReplyToAddress
    {
      get { return Row["EmailReplyToAddress"] != DBNull.Value ? (string)Row["EmailReplyToAddress"] : null; }
      set { Row["EmailReplyToAddress"] = CheckValue("EmailReplyToAddress", value); }
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
    
    public bool NeedsIndexing
    {
      get { return (bool)Row["NeedsIndexing"]; }
      set { Row["NeedsIndexing"] = CheckValue("NeedsIndexing", value); }
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
    

    /* DateTime */
    
    
    

    
    public DateTime? DateClosed
    {
      get { return Row["DateClosed"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateClosed"]) : null; }
      set { Row["DateClosed"] = CheckValue("DateClosed", value); }
    }

    public DateTime? DateClosedUtc
    {
      get { return Row["DateClosed"] != DBNull.Value ? (DateTime?)Row["DateClosed"] : null; }
    }
    
    public DateTime? LastViolationTime
    {
      get { return Row["LastViolationTime"] != DBNull.Value ? DateToLocal((DateTime?)Row["LastViolationTime"]) : null; }
      set { Row["LastViolationTime"] = CheckValue("LastViolationTime", value); }
    }

    public DateTime? LastViolationTimeUtc
    {
      get { return Row["LastViolationTime"] != DBNull.Value ? (DateTime?)Row["LastViolationTime"] : null; }
    }
    
    public DateTime? LastWarningTime
    {
      get { return Row["LastWarningTime"] != DBNull.Value ? DateToLocal((DateTime?)Row["LastWarningTime"]) : null; }
      set { Row["LastWarningTime"] = CheckValue("LastWarningTime", value); }
    }

    public DateTime? LastWarningTimeUtc
    {
      get { return Row["LastWarningTime"] != DBNull.Value ? (DateTime?)Row["LastWarningTime"] : null; }
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

  public partial class Tickets : BaseCollection, IEnumerable<Ticket>
  {
    public Tickets(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Tickets"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketID"; }
    }



    public Ticket this[int index]
    {
      get { return new Ticket(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Ticket ticket);
    partial void AfterRowInsert(Ticket ticket);
    partial void BeforeRowEdit(Ticket ticket);
    partial void AfterRowEdit(Ticket ticket);
    partial void BeforeRowDelete(int ticketID);
    partial void AfterRowDelete(int ticketID);    

    partial void BeforeDBDelete(int ticketID);
    partial void AfterDBDelete(int ticketID);    

    #endregion

    #region Public Methods

    public TicketProxy[] GetTicketProxies()
    {
      List<TicketProxy> list = new List<TicketProxy>();

      foreach (Ticket item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Tickets] WHERE ([TicketID] = @TicketID);";
        deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);
        deleteCommand.Parameters["TicketID"].Value = ticketID;

        BeforeDBDelete(ticketID);
        BeforeRowDelete(ticketID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(ticketID);
        AfterDBDelete(ticketID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Tickets] SET     [ReportedVersionID] = @ReportedVersionID,    [SolvedVersionID] = @SolvedVersionID,    [ProductID] = @ProductID,    [GroupID] = @GroupID,    [UserID] = @UserID,    [TicketStatusID] = @TicketStatusID,    [TicketTypeID] = @TicketTypeID,    [TicketSeverityID] = @TicketSeverityID,    [OrganizationID] = @OrganizationID,    [Name] = @Name,    [ParentID] = @ParentID,    [TicketNumber] = @TicketNumber,    [IsVisibleOnPortal] = @IsVisibleOnPortal,    [IsKnowledgeBase] = @IsKnowledgeBase,    [DateClosed] = @DateClosed,    [CloserID] = @CloserID,    [ImportID] = @ImportID,    [LastViolationTime] = @LastViolationTime,    [LastWarningTime] = @LastWarningTime,    [TicketSource] = @TicketSource,    [PortalEmail] = @PortalEmail,    [SlaViolationTimeClosed] = @SlaViolationTimeClosed,    [SlaViolationLastAction] = @SlaViolationLastAction,    [SlaViolationInitialResponse] = @SlaViolationInitialResponse,    [SlaWarningTimeClosed] = @SlaWarningTimeClosed,    [SlaWarningLastAction] = @SlaWarningLastAction,    [SlaWarningInitialResponse] = @SlaWarningInitialResponse,    [NeedsIndexing] = @NeedsIndexing,    [DocID] = @DocID,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [DueDate] = @DueDate,    [KnowledgeBaseCategoryID] = @KnowledgeBaseCategoryID,    [DateModifiedBySalesForceSync] = @DateModifiedBySalesForceSync,    [SalesForceID] = @SalesForceID,    [JiraStatus] = @JiraStatus,    [DateModifiedByJiraSync] = @DateModifiedByJiraSync,    [SyncWithJira] = @SyncWithJira,    [JiraID] = @JiraID,    [JiraKey] = @JiraKey,    [JiraLinkURL] = @JiraLinkURL,    [EmailReplyToAddress] = @EmailReplyToAddress,    [ImportFileID] = @ImportFileID  WHERE ([TicketID] = @TicketID);";

		
		tempParameter = updateCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.NVarChar, 255);
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
		
		tempParameter = updateCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastViolationTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastWarningTime", SqlDbType.DateTime, 8);
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
		
		tempParameter = updateCommand.Parameters.Add("PortalEmail", SqlDbType.VarChar, 500);
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
		
		tempParameter = updateCommand.Parameters.Add("DocID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DueDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("KnowledgeBaseCategoryID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModifiedBySalesForceSync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SalesForceID", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("JiraStatus", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModifiedByJiraSync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SyncWithJira", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("JiraID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("JiraKey", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("JiraLinkURL", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EmailReplyToAddress", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Tickets] (    [ReportedVersionID],    [SolvedVersionID],    [ProductID],    [GroupID],    [UserID],    [TicketStatusID],    [TicketTypeID],    [TicketSeverityID],    [OrganizationID],    [Name],    [ParentID],    [TicketNumber],    [IsVisibleOnPortal],    [IsKnowledgeBase],    [DateClosed],    [CloserID],    [ImportID],    [LastViolationTime],    [LastWarningTime],    [TicketSource],    [PortalEmail],    [SlaViolationTimeClosed],    [SlaViolationLastAction],    [SlaViolationInitialResponse],    [SlaWarningTimeClosed],    [SlaWarningLastAction],    [SlaWarningInitialResponse],    [NeedsIndexing],    [DocID],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [DueDate],    [KnowledgeBaseCategoryID],    [DateModifiedBySalesForceSync],    [SalesForceID],    [JiraStatus],    [DateModifiedByJiraSync],    [SyncWithJira],    [JiraID],    [JiraKey],    [JiraLinkURL],    [EmailReplyToAddress],    [ImportFileID]) VALUES ( @ReportedVersionID, @SolvedVersionID, @ProductID, @GroupID, @UserID, @TicketStatusID, @TicketTypeID, @TicketSeverityID, @OrganizationID, @Name, @ParentID, @TicketNumber, @IsVisibleOnPortal, @IsKnowledgeBase, @DateClosed, @CloserID, @ImportID, @LastViolationTime, @LastWarningTime, @TicketSource, @PortalEmail, @SlaViolationTimeClosed, @SlaViolationLastAction, @SlaViolationInitialResponse, @SlaWarningTimeClosed, @SlaWarningLastAction, @SlaWarningInitialResponse, @NeedsIndexing, @DocID, @DateCreated, @DateModified, @CreatorID, @ModifierID, @DueDate, @KnowledgeBaseCategoryID, @DateModifiedBySalesForceSync, @SalesForceID, @JiraStatus, @DateModifiedByJiraSync, @SyncWithJira, @JiraID, @JiraKey, @JiraLinkURL, @EmailReplyToAddress, @ImportFileID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ImportFileID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("EmailReplyToAddress", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("JiraLinkURL", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("JiraKey", SqlDbType.VarChar, -1);
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
		
		tempParameter = insertCommand.Parameters.Add("SyncWithJira", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModifiedByJiraSync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("JiraStatus", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SalesForceID", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModifiedBySalesForceSync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("KnowledgeBaseCategoryID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DueDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
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
		
		tempParameter = insertCommand.Parameters.Add("DocID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		
		tempParameter = insertCommand.Parameters.Add("PortalEmail", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketSource", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastWarningTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastViolationTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		
		tempParameter = insertCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.NVarChar, 255);
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
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Tickets] WHERE ([TicketID] = @TicketID);";
		deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);

		try
		{
		  foreach (Ticket ticket in this)
		  {
			if (ticket.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticket);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticket.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketID"].AutoIncrement = false;
			  Table.Columns["TicketID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticket.Row["TicketID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticket);
			}
			else if (ticket.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticket);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticket.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticket);
			}
			else if (ticket.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticket.Row["TicketID", DataRowVersion.Original];
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

      foreach (Ticket ticket in this)
      {
        if (ticket.Row.Table.Columns.Contains("CreatorID") && (int)ticket.Row["CreatorID"] == 0) ticket.Row["CreatorID"] = LoginUser.UserID;
        if (ticket.Row.Table.Columns.Contains("ModifierID")) ticket.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Ticket FindByTicketID(int ticketID)
    {
      foreach (Ticket ticket in this)
      {
        if (ticket.TicketID == ticketID)
        {
          return ticket;
        }
      }
      return null;
    }

    public virtual Ticket AddNewTicket()
    {
      if (Table.Columns.Count < 1) LoadColumns("Tickets");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Ticket(row, this);
    }
    
    public virtual void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TicketID], [ReportedVersionID], [SolvedVersionID], [ProductID], [GroupID], [UserID], [TicketStatusID], [TicketTypeID], [TicketSeverityID], [OrganizationID], [Name], [ParentID], [TicketNumber], [IsVisibleOnPortal], [IsKnowledgeBase], [DateClosed], [CloserID], [ImportID], [LastViolationTime], [LastWarningTime], [TicketSource], [PortalEmail], [SlaViolationTimeClosed], [SlaViolationLastAction], [SlaViolationInitialResponse], [SlaWarningTimeClosed], [SlaWarningLastAction], [SlaWarningInitialResponse], [NeedsIndexing], [DocID], [DateCreated], [DateModified], [CreatorID], [ModifierID], [DueDate], [KnowledgeBaseCategoryID], [DateModifiedBySalesForceSync], [SalesForceID], [JiraStatus], [DateModifiedByJiraSync], [SyncWithJira], [JiraID], [JiraKey], [JiraLinkURL], [EmailReplyToAddress], [ImportFileID] FROM [dbo].[Tickets] WHERE ([TicketID] = @TicketID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketID", ticketID);
        Fill(command);
      }
    }
    
    public static Ticket GetTicket(LoginUser loginUser, int ticketID)
    {
      Tickets tickets = new Tickets(loginUser);
      tickets.LoadByTicketID(ticketID);
      if (tickets.IsEmpty)
        return null;
      else
        return tickets[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Ticket> Members

    public IEnumerator<Ticket> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Ticket(row, this);
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
