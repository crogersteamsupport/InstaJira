using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class Tickets
  {
    protected override void BuildFieldMap()
    {
      // In addition to the fieldMap the RestTickets.CreateTicket method needs to get a field explicitly added for it to be included in the ticket.
      // (!) Be careful when changing these values since this map is checked by the CRM service when updating the tickets
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("TicketID", "TicketID", false, false, true);
      _fieldMap.AddMap("ReportedVersionID", "ReportedVersionID", true, true, true);
      _fieldMap.AddMap("SolvedVersionID", "SolvedVersionID", true, true, true);
      _fieldMap.AddMap("ProductID", "ProductID", true, true, true);
      _fieldMap.AddMap("GroupID", "GroupID", true, true, true);
      _fieldMap.AddMap("UserID", "UserID", true, true, true);
      _fieldMap.AddMap("TicketStatusID", "TicketStatusID", true, true, true);
      _fieldMap.AddMap("TicketTypeID", "TicketTypeID", true, true, true);
      _fieldMap.AddMap("TicketSeverityID", "TicketSeverityID", true, true, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("Name", "Name", true, true, true);
      _fieldMap.AddMap("ParentID", "ParentID", true, true, true);
      _fieldMap.AddMap("TicketNumber", "TicketNumber", false, false, true);
      _fieldMap.AddMap("IsVisibleOnPortal", "IsVisibleOnPortal", true, true, true);
      _fieldMap.AddMap("IsKnowledgeBase", "IsKnowledgeBase", true, true, true);
      _fieldMap.AddMap("DateClosed", "DateClosed", true, true, true);
      _fieldMap.AddMap("CloserID", "CloserID", true, true, true);
      _fieldMap.AddMap("ImportID", "ImportID", true, true, true);
      _fieldMap.AddMap("LastViolationTime", "LastViolationTime", false, false, false);
      _fieldMap.AddMap("LastWarningTime", "LastWarningTime", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", true, true, true);
      _fieldMap.AddMap("DateModified", "DateModified", true, true, true);
      _fieldMap.AddMap("TicketSource", "TicketSource", false, false, false);
      _fieldMap.AddMap("PortalEmail", "PortalEmail", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", true, true, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", true, true, true);
      _fieldMap.AddMap("KnowledgeBaseCategoryID", "KnowledgeBaseCategoryID", true, true, true);
      _fieldMap.AddMap("SalesForceID", "SalesForceID", false, false, false);
      _fieldMap.AddMap("DateModifiedBySalesForceSync", "DateModifiedBySalesForceSync", false, false, false);
      _fieldMap.AddMap("DueDate", "DueDate", true, true, true);
      _fieldMap.AddMap("ImportFileID", "ImportFileID", false, false, false);
            
    }
  }
  
}
