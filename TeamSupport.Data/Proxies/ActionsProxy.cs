using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  
  public partial class Action : BaseItem
  {
    public ActionProxy GetProxy()
    {
      ActionProxy result = new ActionProxy();
      result.ImportFileID = this.ImportFileID;
      result.Pinned = this.Pinned;
      result.SalesForceID = this.SalesForceID;
      result.TicketID = this.TicketID;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.ImportID = this.ImportID;
      result.IsKnowledgeBase = this.IsKnowledgeBase;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.TimeSpent = this.TimeSpent;
      result.Description = this.Description;
      result.Name = this.Name;
      result.SystemActionTypeID = this.SystemActionTypeID;
      result.ActionTypeID = this.ActionTypeID;
      result.ActionID = this.ActionID;
      result.DisplayName = this.DisplayName;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
      result.DateModifiedBySalesForceSync = this.DateModifiedBySalesForceSyncUtc == null ? this.DateModifiedBySalesForceSyncUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedBySalesForceSyncUtc, DateTimeKind.Utc); 
      result.DateStarted = this.DateStartedUtc == null ? this.DateStartedUtc : DateTime.SpecifyKind((DateTime)this.DateStartedUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
