using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketGridViewItem : BaseItem
  {
    private TicketGridView _ticketGridView;
    
    public TicketGridViewItem(DataRow row, TicketGridView ticketGridView): base(row, ticketGridView)
    {
      _ticketGridView = ticketGridView;
    }
	
    #region Properties
    
    public TicketGridView Collection
    {
      get { return _ticketGridView; }
    }
        
    
    public string UserName
    {
      get { return Row["UserName"] != DBNull.Value ? (string)Row["UserName"] : null; }
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
    
    public int? SlaViolationTime
    {
      get { return Row["SlaViolationTime"] != DBNull.Value ? (int?)Row["SlaViolationTime"] : null; }
    }
    
    public int? SlaWarningTime
    {
      get { return Row["SlaWarningTime"] != DBNull.Value ? (int?)Row["SlaWarningTime"] : null; }
    }
    
    public int? SlaViolationHours
    {
      get { return Row["SlaViolationHours"] != DBNull.Value ? (int?)Row["SlaViolationHours"] : null; }
    }
    
    public int? SlaWarningHours
    {
      get { return Row["SlaWarningHours"] != DBNull.Value ? (int?)Row["SlaWarningHours"] : null; }
    }
    
    
    
    public int DaysOpened
    {
      get { return (int)Row["DaysOpened"]; }
    }
    
    public int DaysClosed
    {
      get { return (int)Row["DaysClosed"]; }
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
    
    public bool IsClosed
    {
      get { return (bool)Row["IsClosed"]; }
      set { Row["IsClosed"] = CheckValue("IsClosed", value); }
    }
    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
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
    
    public DateTime? DueDate
    {
      get { return Row["DueDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["DueDate"]) : null; }
      set { Row["DueDate"] = CheckValue("DueDate", value); }
    }

    public DateTime? DueDateUtc
    {
      get { return Row["DueDate"] != DBNull.Value ? (DateTime?)Row["DueDate"] : null; }
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

  public partial class TicketGridView : BaseCollection, IEnumerable<TicketGridViewItem>
  {
    public TicketGridView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketGridView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketID"; }
    }



    public TicketGridViewItem this[int index]
    {
      get { return new TicketGridViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketGridViewItem ticketGridViewItem);
    partial void AfterRowInsert(TicketGridViewItem ticketGridViewItem);
    partial void BeforeRowEdit(TicketGridViewItem ticketGridViewItem);
    partial void AfterRowEdit(TicketGridViewItem ticketGridViewItem);
    partial void BeforeRowDelete(int ticketID);
    partial void AfterRowDelete(int ticketID);    

    partial void BeforeDBDelete(int ticketID);
    partial void AfterDBDelete(int ticketID);    

    #endregion

    #region Public Methods

    public TicketGridViewItemProxy[] GetTicketGridViewItemProxies()
    {
      List<TicketGridViewItemProxy> list = new List<TicketGridViewItemProxy>();

      foreach (TicketGridViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketGridView] WHERE ([TicketID] = @TicketID);";
        deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);
        deleteCommand.Parameters["TicketID"].Value = ticketID;

        BeforeDBDelete(ticketID);
        BeforeRowDelete(ticketID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(ticketID);
        AfterDBDelete(ticketID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketGridViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketGridView] SET     [ProductName] = @ProductName,    [ReportedVersion] = @ReportedVersion,    [SolvedVersion] = @SolvedVersion,    [GroupName] = @GroupName,    [TicketTypeName] = @TicketTypeName,    [UserName] = @UserName,    [Status] = @Status,    [StatusPosition] = @StatusPosition,    [SeverityPosition] = @SeverityPosition,    [IsClosed] = @IsClosed,    [Severity] = @Severity,    [TicketNumber] = @TicketNumber,    [IsVisibleOnPortal] = @IsVisibleOnPortal,    [IsKnowledgeBase] = @IsKnowledgeBase,    [ReportedVersionID] = @ReportedVersionID,    [SolvedVersionID] = @SolvedVersionID,    [ProductID] = @ProductID,    [GroupID] = @GroupID,    [UserID] = @UserID,    [TicketStatusID] = @TicketStatusID,    [TicketTypeID] = @TicketTypeID,    [TicketSeverityID] = @TicketSeverityID,    [OrganizationID] = @OrganizationID,    [Name] = @Name,    [ParentID] = @ParentID,    [ModifierID] = @ModifierID,    [DateModified] = @DateModified,    [DateClosed] = @DateClosed,    [CloserID] = @CloserID,    [DaysClosed] = @DaysClosed,    [DaysOpened] = @DaysOpened,    [CloserName] = @CloserName,    [CreatorName] = @CreatorName,    [ModifierName] = @ModifierName,    [SlaViolationTime] = @SlaViolationTime,    [SlaWarningTime] = @SlaWarningTime,    [SlaViolationHours] = @SlaViolationHours,    [SlaWarningHours] = @SlaWarningHours,    [DueDate] = @DueDate  WHERE ([TicketID] = @TicketID);";

		
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
		
		tempParameter = updateCommand.Parameters.Add("UserName", SqlDbType.NVarChar, 201);
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
		
		tempParameter = updateCommand.Parameters.Add("DaysOpened", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CloserName", SqlDbType.NVarChar, 201);
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
		
		tempParameter = updateCommand.Parameters.Add("DueDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketGridView] (    [TicketID],    [ProductName],    [ReportedVersion],    [SolvedVersion],    [GroupName],    [TicketTypeName],    [UserName],    [Status],    [StatusPosition],    [SeverityPosition],    [IsClosed],    [Severity],    [TicketNumber],    [IsVisibleOnPortal],    [IsKnowledgeBase],    [ReportedVersionID],    [SolvedVersionID],    [ProductID],    [GroupID],    [UserID],    [TicketStatusID],    [TicketTypeID],    [TicketSeverityID],    [OrganizationID],    [Name],    [ParentID],    [ModifierID],    [CreatorID],    [DateModified],    [DateCreated],    [DateClosed],    [CloserID],    [DaysClosed],    [DaysOpened],    [CloserName],    [CreatorName],    [ModifierName],    [SlaViolationTime],    [SlaWarningTime],    [SlaViolationHours],    [SlaWarningHours],    [DueDate]) VALUES ( @TicketID, @ProductName, @ReportedVersion, @SolvedVersion, @GroupName, @TicketTypeName, @UserName, @Status, @StatusPosition, @SeverityPosition, @IsClosed, @Severity, @TicketNumber, @IsVisibleOnPortal, @IsKnowledgeBase, @ReportedVersionID, @SolvedVersionID, @ProductID, @GroupID, @UserID, @TicketStatusID, @TicketTypeID, @TicketSeverityID, @OrganizationID, @Name, @ParentID, @ModifierID, @CreatorID, @DateModified, @DateCreated, @DateClosed, @CloserID, @DaysClosed, @DaysOpened, @CloserName, @CreatorName, @ModifierName, @SlaViolationTime, @SlaWarningTime, @SlaViolationHours, @SlaWarningHours, @DueDate); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DueDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
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
		
		tempParameter = insertCommand.Parameters.Add("CloserName", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DaysOpened", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("UserName", SqlDbType.NVarChar, 201);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketGridView] WHERE ([TicketID] = @TicketID);";
		deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);

		try
		{
		  foreach (TicketGridViewItem ticketGridViewItem in this)
		  {
			if (ticketGridViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketGridViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketGridViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketID"].AutoIncrement = false;
			  Table.Columns["TicketID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketGridViewItem.Row["TicketID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketGridViewItem);
			}
			else if (ticketGridViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketGridViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketGridViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketGridViewItem);
			}
			else if (ticketGridViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketGridViewItem.Row["TicketID", DataRowVersion.Original];
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

      foreach (TicketGridViewItem ticketGridViewItem in this)
      {
        if (ticketGridViewItem.Row.Table.Columns.Contains("CreatorID") && (int)ticketGridViewItem.Row["CreatorID"] == 0) ticketGridViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (ticketGridViewItem.Row.Table.Columns.Contains("ModifierID")) ticketGridViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketGridViewItem FindByTicketID(int ticketID)
    {
      foreach (TicketGridViewItem ticketGridViewItem in this)
      {
        if (ticketGridViewItem.TicketID == ticketID)
        {
          return ticketGridViewItem;
        }
      }
      return null;
    }

    public virtual TicketGridViewItem AddNewTicketGridViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketGridView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketGridViewItem(row, this);
    }
    
    public virtual void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TicketID], [ProductName], [ReportedVersion], [SolvedVersion], [GroupName], [TicketTypeName], [UserName], [Status], [StatusPosition], [SeverityPosition], [IsClosed], [Severity], [TicketNumber], [IsVisibleOnPortal], [IsKnowledgeBase], [ReportedVersionID], [SolvedVersionID], [ProductID], [GroupID], [UserID], [TicketStatusID], [TicketTypeID], [TicketSeverityID], [OrganizationID], [Name], [ParentID], [ModifierID], [CreatorID], [DateModified], [DateCreated], [DateClosed], [CloserID], [DaysClosed], [DaysOpened], [CloserName], [CreatorName], [ModifierName], [SlaViolationTime], [SlaWarningTime], [SlaViolationHours], [SlaWarningHours], [DueDate] FROM [dbo].[TicketGridView] WHERE ([TicketID] = @TicketID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketID", ticketID);
        Fill(command);
      }
    }
    
    public static TicketGridViewItem GetTicketGridViewItem(LoginUser loginUser, int ticketID)
    {
      TicketGridView ticketGridView = new TicketGridView(loginUser);
      ticketGridView.LoadByTicketID(ticketID);
      if (ticketGridView.IsEmpty)
        return null;
      else
        return ticketGridView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TicketGridViewItem> Members

    public IEnumerator<TicketGridViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketGridViewItem(row, this);
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
