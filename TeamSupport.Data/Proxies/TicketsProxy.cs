using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  
  public partial class Ticket : BaseItem
  {
    public TicketProxy GetProxy()
    {
      TicketProxy result = new TicketProxy();

      result.ImportFileID = this.ImportFileID;
      result.SalesForceID = this.SalesForceID;
      result.KnowledgeBaseCategoryID = this.KnowledgeBaseCategoryID;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.DocID = this.DocID;
      result.NeedsIndexing = this.NeedsIndexing;
      result.PortalEmail = this.PortalEmail;
      result.TicketSource = this.TicketSource;
      result.ImportID = this.ImportID;
      result.CloserID = this.CloserID;
      result.IsKnowledgeBase = this.IsKnowledgeBase;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.TicketNumber = this.TicketNumber;
      result.ParentID = this.ParentID;
      result.Name = (this.Name);
      result.OrganizationID = this.OrganizationID;
      result.TicketSeverityID = this.TicketSeverityID;
      result.TicketTypeID = this.TicketTypeID;
      result.TicketStatusID = this.TicketStatusID;
      result.UserID = this.UserID;
      result.GroupID = this.GroupID;
      result.ProductID = this.ProductID;
      result.SolvedVersionID = this.SolvedVersionID;
      result.ReportedVersionID = this.ReportedVersionID;
      result.TicketID = this.TicketID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
      result.DateModifiedBySalesForceSync = this.DateModifiedBySalesForceSyncUtc == null ? this.DateModifiedBySalesForceSyncUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedBySalesForceSyncUtc, DateTimeKind.Utc); 
      result.SlaWarningInitialResponse = this.SlaWarningInitialResponseUtc == null ? this.SlaWarningInitialResponseUtc : DateTime.SpecifyKind((DateTime)this.SlaWarningInitialResponseUtc, DateTimeKind.Utc); 
      result.SlaWarningLastAction = this.SlaWarningLastActionUtc == null ? this.SlaWarningLastActionUtc : DateTime.SpecifyKind((DateTime)this.SlaWarningLastActionUtc, DateTimeKind.Utc); 
      result.SlaWarningTimeClosed = this.SlaWarningTimeClosedUtc == null ? this.SlaWarningTimeClosedUtc : DateTime.SpecifyKind((DateTime)this.SlaWarningTimeClosedUtc, DateTimeKind.Utc); 
      result.SlaViolationInitialResponse = this.SlaViolationInitialResponseUtc == null ? this.SlaViolationInitialResponseUtc : DateTime.SpecifyKind((DateTime)this.SlaViolationInitialResponseUtc, DateTimeKind.Utc); 
      result.SlaViolationLastAction = this.SlaViolationLastActionUtc == null ? this.SlaViolationLastActionUtc : DateTime.SpecifyKind((DateTime)this.SlaViolationLastActionUtc, DateTimeKind.Utc); 
      result.SlaViolationTimeClosed = this.SlaViolationTimeClosedUtc == null ? this.SlaViolationTimeClosedUtc : DateTime.SpecifyKind((DateTime)this.SlaViolationTimeClosedUtc, DateTimeKind.Utc); 
      result.LastWarningTime = this.LastWarningTimeUtc == null ? this.LastWarningTimeUtc : DateTime.SpecifyKind((DateTime)this.LastWarningTimeUtc, DateTimeKind.Utc); 
      result.LastViolationTime = this.LastViolationTimeUtc == null ? this.LastViolationTimeUtc : DateTime.SpecifyKind((DateTime)this.LastViolationTimeUtc, DateTimeKind.Utc); 
      result.DateClosed = this.DateClosedUtc == null ? this.DateClosedUtc : DateTime.SpecifyKind((DateTime)this.DateClosedUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
