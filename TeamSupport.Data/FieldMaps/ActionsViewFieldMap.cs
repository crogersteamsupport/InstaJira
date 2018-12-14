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
      _fieldMap.AddMap("ActionID", "ActionID", true, true, true);
      _fieldMap.AddMap("ActionTypeID", "ActionTypeID", true, true, true);
      _fieldMap.AddMap("SystemActionTypeID", "SystemActionTypeID", true, true, true);
      _fieldMap.AddMap("Name", "Name", true, true, true);
      _fieldMap.AddMap("Description", "Description", true, true, true);
      _fieldMap.AddMap("TimeSpent", "TimeSpent", true, true, true);
      _fieldMap.AddMap("DateStarted", "DateStarted", true, true, true);
      _fieldMap.AddMap("IsVisibleOnPortal", "IsVisibleOnPortal", true, true, true);
      _fieldMap.AddMap("IsKnowledgeBase", "IsKnowledgeBase", true, true, true);
      _fieldMap.AddMap("DateCreated", "DateCreated", true, true, true);
      _fieldMap.AddMap("DateModified", "DateModified", true, true, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", true, true, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", true, true, true);
      _fieldMap.AddMap("TicketID", "TicketID", true, true, true);
      _fieldMap.AddMap("CreatorName", "CreatorName", true, true, true);
      _fieldMap.AddMap("ModifierName", "ModifierName", true, true, true);
      _fieldMap.AddMap("ActionType", "ActionType", true, true, true);
      _fieldMap.AddMap("ProductName", "ProductName", true, true, true);
      _fieldMap.AddMap("ReportedVersion", "ReportedVersion", true, true, true);
      _fieldMap.AddMap("SolvedVersion", "SolvedVersion", true, true, true);
      _fieldMap.AddMap("GroupName", "GroupName", true, true, true);
      _fieldMap.AddMap("TicketType", "TicketType", true, true, true);
      _fieldMap.AddMap("UserName", "UserName", true, true, true);
      _fieldMap.AddMap("Status", "Status", true, true, true);
      _fieldMap.AddMap("StatusPosition", "StatusPosition", true, true, true);
      _fieldMap.AddMap("SeverityPosition", "SeverityPosition", true, true, true);
      _fieldMap.AddMap("IsClosed", "IsClosed", true, true, true);
      _fieldMap.AddMap("Severity", "Severity", true, true, true);
      _fieldMap.AddMap("TicketNumber", "TicketNumber", true, true, true);
      _fieldMap.AddMap("ReportedVersionID", "ReportedVersionID", true, true, true);
      _fieldMap.AddMap("SolvedVersionID", "SolvedVersionID", true, true, true);
      _fieldMap.AddMap("ProductID", "ProductID", true, true, true);
      _fieldMap.AddMap("GroupID", "GroupID", true, true, true);
      _fieldMap.AddMap("UserID", "UserID", true, true, true);
      _fieldMap.AddMap("TicketStatusID", "TicketStatusID", true, true, true);
      _fieldMap.AddMap("TicketTypeID", "TicketTypeID", true, true, true);
      _fieldMap.AddMap("TicketSeverityID", "TicketSeverityID", true, true, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", true, true, true);
      _fieldMap.AddMap("TicketName", "TicketName", true, true, true);
      _fieldMap.AddMap("DateClosed", "DateClosed", true, true, true);
      _fieldMap.AddMap("CloserID", "CloserID", true, true, true);
      _fieldMap.AddMap("DaysClosed", "DaysClosed", true, true, true);
      _fieldMap.AddMap("DaysOpened", "DaysOpened", true, true, true);
      _fieldMap.AddMap("CloserName", "CloserName", true, true, true);
      _fieldMap.AddMap("HoursSpent", "HoursSpent", true, true, true);
            
    }
  }
  
}
