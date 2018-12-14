using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ActionsViewItem : BaseItem
  {
    private ActionsView _actionsView;
    
    public ActionsViewItem(DataRow row, ActionsView actionsView): base(row, actionsView)
    {
      _actionsView = actionsView;
    }
	
    #region Properties
    
    public ActionsView Collection
    {
      get { return _actionsView; }
    }
        
    
    public string CreatorName
    {
      get { return Row["CreatorName"] != DBNull.Value ? (string)Row["CreatorName"] : null; }
    }
    
    public string ModifierName
    {
      get { return Row["ModifierName"] != DBNull.Value ? (string)Row["ModifierName"] : null; }
    }
    
    public string ActionType
    {
      get { return Row["ActionType"] != DBNull.Value ? (string)Row["ActionType"] : null; }
    }
    
    public string UserName
    {
      get { return Row["UserName"] != DBNull.Value ? (string)Row["UserName"] : null; }
    }
    
    public int? DaysOpened
    {
      get { return Row["DaysOpened"] != DBNull.Value ? (int?)Row["DaysOpened"] : null; }
    }
    
    public string CloserName
    {
      get { return Row["CloserName"] != DBNull.Value ? (string)Row["CloserName"] : null; }
    }
    
    public decimal? HoursSpent
    {
      get { return Row["HoursSpent"] != DBNull.Value ? (decimal?)Row["HoursSpent"] : null; }
    }
    
    
    
    public int DaysClosed
    {
      get { return (int)Row["DaysClosed"]; }
    }
    
    public bool IsClosed
    {
      get { return (bool)Row["IsClosed"]; }
    }
    

    
    public int? ActionTypeID
    {
      get { return Row["ActionTypeID"] != DBNull.Value ? (int?)Row["ActionTypeID"] : null; }
      set { Row["ActionTypeID"] = CheckValue("ActionTypeID", value); }
    }
    
    public int? TimeSpent
    {
      get { return Row["TimeSpent"] != DBNull.Value ? (int?)Row["TimeSpent"] : null; }
      set { Row["TimeSpent"] = CheckValue("TimeSpent", value); }
    }
    
    public string ActionSource
    {
      get { return Row["ActionSource"] != DBNull.Value ? (string)Row["ActionSource"] : null; }
      set { Row["ActionSource"] = CheckValue("ActionSource", value); }
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
    
    public string TicketType
    {
      get { return Row["TicketType"] != DBNull.Value ? (string)Row["TicketType"] : null; }
      set { Row["TicketType"] = CheckValue("TicketType", value); }
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
    
    public int? TicketNumber
    {
      get { return Row["TicketNumber"] != DBNull.Value ? (int?)Row["TicketNumber"] : null; }
      set { Row["TicketNumber"] = CheckValue("TicketNumber", value); }
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
    
    public int? TicketStatusID
    {
      get { return Row["TicketStatusID"] != DBNull.Value ? (int?)Row["TicketStatusID"] : null; }
      set { Row["TicketStatusID"] = CheckValue("TicketStatusID", value); }
    }
    
    public int? TicketTypeID
    {
      get { return Row["TicketTypeID"] != DBNull.Value ? (int?)Row["TicketTypeID"] : null; }
      set { Row["TicketTypeID"] = CheckValue("TicketTypeID", value); }
    }
    
    public int? TicketSeverityID
    {
      get { return Row["TicketSeverityID"] != DBNull.Value ? (int?)Row["TicketSeverityID"] : null; }
      set { Row["TicketSeverityID"] = CheckValue("TicketSeverityID", value); }
    }
    
    public int? OrganizationID
    {
      get { return Row["OrganizationID"] != DBNull.Value ? (int?)Row["OrganizationID"] : null; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public string TicketName
    {
      get { return Row["TicketName"] != DBNull.Value ? (string)Row["TicketName"] : null; }
      set { Row["TicketName"] = CheckValue("TicketName", value); }
    }
    
    public int? CloserID
    {
      get { return Row["CloserID"] != DBNull.Value ? (int?)Row["CloserID"] : null; }
      set { Row["CloserID"] = CheckValue("CloserID", value); }
    }
    

    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
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
    
    public string Description
    {
      get { return (string)Row["Description"]; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckValue("Name", value); }
    }
    
    public SystemActionType SystemActionTypeID
    {
      get { return (SystemActionType)Row["SystemActionTypeID"]; }
      set { Row["SystemActionTypeID"] = CheckValue("SystemActionTypeID", value); }
    }
    
    public int ActionID
    {
      get { return (int)Row["ActionID"]; }
      set { Row["ActionID"] = CheckValue("ActionID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? DateStarted
    {
      get { return Row["DateStarted"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateStarted"]) : null; }
      set { Row["DateStarted"] = CheckValue("DateStarted", value); }
    }

    public DateTime? DateStartedUtc
    {
      get { return Row["DateStarted"] != DBNull.Value ? (DateTime?)Row["DateStarted"] : null; }
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

  public partial class ActionsView : BaseCollection, IEnumerable<ActionsViewItem>
  {
    public ActionsView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ActionsView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ActionID"; }
    }



    public ActionsViewItem this[int index]
    {
      get { return new ActionsViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ActionsViewItem actionsViewItem);
    partial void AfterRowInsert(ActionsViewItem actionsViewItem);
    partial void BeforeRowEdit(ActionsViewItem actionsViewItem);
    partial void AfterRowEdit(ActionsViewItem actionsViewItem);
    partial void BeforeRowDelete(int actionID);
    partial void AfterRowDelete(int actionID);    

    partial void BeforeDBDelete(int actionID);
    partial void AfterDBDelete(int actionID);    

    #endregion

    #region Public Methods

    public ActionsViewItemProxy[] GetActionsViewItemProxies()
    {
      List<ActionsViewItemProxy> list = new List<ActionsViewItemProxy>();

      foreach (ActionsViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int actionID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ActionsView] WHERE ([ActionID] = @ActionID);";
        deleteCommand.Parameters.Add("ActionID", SqlDbType.Int);
        deleteCommand.Parameters["ActionID"].Value = actionID;

        BeforeDBDelete(actionID);
        BeforeRowDelete(actionID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(actionID);
        AfterDBDelete(actionID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ActionsViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ActionsView] SET     [ActionTypeID] = @ActionTypeID,    [SystemActionTypeID] = @SystemActionTypeID,    [Name] = @Name,    [Description] = @Description,    [TimeSpent] = @TimeSpent,    [DateStarted] = @DateStarted,    [IsVisibleOnPortal] = @IsVisibleOnPortal,    [IsKnowledgeBase] = @IsKnowledgeBase,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [TicketID] = @TicketID,    [ActionSource] = @ActionSource,    [CreatorName] = @CreatorName,    [ModifierName] = @ModifierName,    [ActionType] = @ActionType,    [ProductName] = @ProductName,    [ReportedVersion] = @ReportedVersion,    [SolvedVersion] = @SolvedVersion,    [GroupName] = @GroupName,    [TicketType] = @TicketType,    [UserName] = @UserName,    [Status] = @Status,    [StatusPosition] = @StatusPosition,    [SeverityPosition] = @SeverityPosition,    [IsClosed] = @IsClosed,    [Severity] = @Severity,    [TicketNumber] = @TicketNumber,    [ReportedVersionID] = @ReportedVersionID,    [SolvedVersionID] = @SolvedVersionID,    [ProductID] = @ProductID,    [GroupID] = @GroupID,    [UserID] = @UserID,    [TicketStatusID] = @TicketStatusID,    [TicketTypeID] = @TicketTypeID,    [TicketSeverityID] = @TicketSeverityID,    [OrganizationID] = @OrganizationID,    [TicketName] = @TicketName,    [DateClosed] = @DateClosed,    [CloserID] = @CloserID,    [DaysClosed] = @DaysClosed,    [DaysOpened] = @DaysOpened,    [CloserName] = @CloserName,    [HoursSpent] = @HoursSpent  WHERE ([ActionID] = @ActionID);";

		
		tempParameter = updateCommand.Parameters.Add("ActionID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ActionTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SystemActionTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TimeSpent", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateStarted", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
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
		
		tempParameter = updateCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ActionSource", SqlDbType.VarChar, 50);
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
		
		tempParameter = updateCommand.Parameters.Add("ActionType", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		
		tempParameter = updateCommand.Parameters.Add("TicketType", SqlDbType.VarChar, 255);
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
		
		tempParameter = updateCommand.Parameters.Add("TicketName", SqlDbType.NVarChar, 255);
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
		
		tempParameter = updateCommand.Parameters.Add("HoursSpent", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 24;
		  tempParameter.Scale = 24;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ActionsView] (    [ActionID],    [ActionTypeID],    [SystemActionTypeID],    [Name],    [Description],    [TimeSpent],    [DateStarted],    [IsVisibleOnPortal],    [IsKnowledgeBase],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [TicketID],    [ActionSource],    [CreatorName],    [ModifierName],    [ActionType],    [ProductName],    [ReportedVersion],    [SolvedVersion],    [GroupName],    [TicketType],    [UserName],    [Status],    [StatusPosition],    [SeverityPosition],    [IsClosed],    [Severity],    [TicketNumber],    [ReportedVersionID],    [SolvedVersionID],    [ProductID],    [GroupID],    [UserID],    [TicketStatusID],    [TicketTypeID],    [TicketSeverityID],    [OrganizationID],    [TicketName],    [DateClosed],    [CloserID],    [DaysClosed],    [DaysOpened],    [CloserName],    [HoursSpent]) VALUES ( @ActionID, @ActionTypeID, @SystemActionTypeID, @Name, @Description, @TimeSpent, @DateStarted, @IsVisibleOnPortal, @IsKnowledgeBase, @DateCreated, @DateModified, @CreatorID, @ModifierID, @TicketID, @ActionSource, @CreatorName, @ModifierName, @ActionType, @ProductName, @ReportedVersion, @SolvedVersion, @GroupName, @TicketType, @UserName, @Status, @StatusPosition, @SeverityPosition, @IsClosed, @Severity, @TicketNumber, @ReportedVersionID, @SolvedVersionID, @ProductID, @GroupID, @UserID, @TicketStatusID, @TicketTypeID, @TicketSeverityID, @OrganizationID, @TicketName, @DateClosed, @CloserID, @DaysClosed, @DaysOpened, @CloserName, @HoursSpent); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("HoursSpent", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 24;
		  tempParameter.Scale = 24;
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
		
		tempParameter = insertCommand.Parameters.Add("TicketName", SqlDbType.NVarChar, 255);
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
		
		tempParameter = insertCommand.Parameters.Add("TicketType", SqlDbType.VarChar, 255);
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
		
		tempParameter = insertCommand.Parameters.Add("ActionType", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		
		tempParameter = insertCommand.Parameters.Add("ActionSource", SqlDbType.VarChar, 50);
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
		
		tempParameter = insertCommand.Parameters.Add("DateStarted", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("TimeSpent", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SystemActionTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ActionTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ActionID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ActionsView] WHERE ([ActionID] = @ActionID);";
		deleteCommand.Parameters.Add("ActionID", SqlDbType.Int);

		try
		{
		  foreach (ActionsViewItem actionsViewItem in this)
		  {
			if (actionsViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(actionsViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = actionsViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ActionID"].AutoIncrement = false;
			  Table.Columns["ActionID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				actionsViewItem.Row["ActionID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(actionsViewItem);
			}
			else if (actionsViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(actionsViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = actionsViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(actionsViewItem);
			}
			else if (actionsViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)actionsViewItem.Row["ActionID", DataRowVersion.Original];
			  deleteCommand.Parameters["ActionID"].Value = id;
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

      foreach (ActionsViewItem actionsViewItem in this)
      {
        if (actionsViewItem.Row.Table.Columns.Contains("CreatorID") && (int)actionsViewItem.Row["CreatorID"] == 0) actionsViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (actionsViewItem.Row.Table.Columns.Contains("ModifierID")) actionsViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ActionsViewItem FindByActionID(int actionID)
    {
      foreach (ActionsViewItem actionsViewItem in this)
      {
        if (actionsViewItem.ActionID == actionID)
        {
          return actionsViewItem;
        }
      }
      return null;
    }

    public virtual ActionsViewItem AddNewActionsViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("ActionsView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ActionsViewItem(row, this);
    }
    
    public virtual void LoadByActionID(int actionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ActionID], [ActionTypeID], [SystemActionTypeID], [Name], [Description], [TimeSpent], [DateStarted], [IsVisibleOnPortal], [IsKnowledgeBase], [DateCreated], [DateModified], [CreatorID], [ModifierID], [TicketID], [ActionSource], [CreatorName], [ModifierName], [ActionType], [ProductName], [ReportedVersion], [SolvedVersion], [GroupName], [TicketType], [UserName], [Status], [StatusPosition], [SeverityPosition], [IsClosed], [Severity], [TicketNumber], [ReportedVersionID], [SolvedVersionID], [ProductID], [GroupID], [UserID], [TicketStatusID], [TicketTypeID], [TicketSeverityID], [OrganizationID], [TicketName], [DateClosed], [CloserID], [DaysClosed], [DaysOpened], [CloserName], [HoursSpent] FROM [dbo].[ActionsView] WHERE ([ActionID] = @ActionID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ActionID", actionID);
        Fill(command);
      }
    }
    
    public static ActionsViewItem GetActionsViewItem(LoginUser loginUser, int actionID)
    {
      ActionsView actionsView = new ActionsView(loginUser);
      actionsView.LoadByActionID(actionID);
      if (actionsView.IsEmpty)
        return null;
      else
        return actionsView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ActionsViewItem> Members

    public IEnumerator<ActionsViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ActionsViewItem(row, this);
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
