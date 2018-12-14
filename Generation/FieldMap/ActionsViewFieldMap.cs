using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ActionsView
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ActionID", "ActionID", false, false, false);
      _fieldMap.AddMap("ActionTypeID", "ActionTypeID", false, false, false);
      _fieldMap.AddMap("SystemActionTypeID", "SystemActionTypeID", false, false, false);
      _fieldMap.AddMap("Name", "Name", false, false, false);
      _fieldMap.AddMap("Description", "Description", false, false, false);
      _fieldMap.AddMap("TimeSpent", "TimeSpent", false, false, false);
      _fieldMap.AddMap("DateStarted", "DateStarted", false, false, false);
      _fieldMap.AddMap("IsVisibleOnPortal", "IsVisibleOnPortal", false, false, false);
      _fieldMap.AddMap("IsKnowledgeBase", "IsKnowledgeBase", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("TicketID", "TicketID", false, false, false);
      _fieldMap.AddMap("ActionSource", "ActionSource", false, false, false);
      _fieldMap.AddMap("CreatorName", "CreatorName", false, false, false);
      _fieldMap.AddMap("ModifierName", "ModifierName", false, false, false);
      _fieldMap.AddMap("ActionType", "ActionType", false, false, false);
      _fieldMap.AddMap("ProductName", "ProductName", false, false, false);
      _fieldMap.AddMap("ReportedVersion", "ReportedVersion", false, false, false);
      _fieldMap.AddMap("SolvedVersion", "SolvedVersion", false, false, false);
      _fieldMap.AddMap("GroupName", "GroupName", false, false, false);
      _fieldMap.AddMap("TicketType", "TicketType", false, false, false);
      _fieldMap.AddMap("UserName", "UserName", false, false, false);
      _fieldMap.AddMap("Status", "Status", false, false, false);
      _fieldMap.AddMap("StatusPosition", "StatusPosition", false, false, false);
      _fieldMap.AddMap("SeverityPosition", "SeverityPosition", false, false, false);
      _fieldMap.AddMap("IsClosed", "IsClosed", false, false, false);
      _fieldMap.AddMap("Severity", "Severity", false, false, false);
      _fieldMap.AddMap("TicketNumber", "TicketNumber", false, false, false);
      _fieldMap.AddMap("ReportedVersionID", "ReportedVersionID", false, false, false);
      _fieldMap.AddMap("SolvedVersionID", "SolvedVersionID", false, false, false);
      _fieldMap.AddMap("ProductID", "ProductID", false, false, false);
      _fieldMap.AddMap("GroupID", "GroupID", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("TicketStatusID", "TicketStatusID", false, false, false);
      _fieldMap.AddMap("TicketTypeID", "TicketTypeID", false, false, false);
      _fieldMap.AddMap("TicketSeverityID", "TicketSeverityID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("TicketName", "TicketName", false, false, false);
      _fieldMap.AddMap("DateClosed", "DateClosed", false, false, false);
      _fieldMap.AddMap("CloserID", "CloserID", false, false, false);
      _fieldMap.AddMap("DaysClosed", "DaysClosed", false, false, false);
      _fieldMap.AddMap("DaysOpened", "DaysOpened", false, false, false);
      _fieldMap.AddMap("CloserName", "CloserName", false, false, false);
      _fieldMap.AddMap("HoursSpent", "HoursSpent", false, false, false);
            
    }
  }
  
}
