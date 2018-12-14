using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  
  public partial class Reminder : BaseItem
  {
    public ReminderProxy GetProxy()
    {
      ReminderProxy result = new ReminderProxy();
      result.CreatorID = this.CreatorID;
      result.HasEmailSent = this.HasEmailSent;
      result.IsDismissed = this.IsDismissed;
      result.UserID = this.UserID;
      result.Description = this.Description;
      result.RefID = this.RefID;
      result.RefType = this.RefType;
      result.OrganizationID = this.OrganizationID;
      result.ReminderID = this.ReminderID;
       
      result.DueDate = DateTime.SpecifyKind(this.DueDateUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
