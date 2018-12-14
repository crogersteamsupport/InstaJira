using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ScheduledReports
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("Id", "Id", false, false, false);
      _fieldMap.AddMap("EmailSubject", "EmailSubject", false, false, false);
      _fieldMap.AddMap("EmailBody", "EmailBody", false, false, false);
      _fieldMap.AddMap("EmailRecipients", "EmailRecipients", false, false, false);
      _fieldMap.AddMap("ReportId", "ReportId", false, false, false);
      _fieldMap.AddMap("OrganizationId", "OrganizationId", false, false, false);
      _fieldMap.AddMap("RunCount", "RunCount", false, false, false);
      _fieldMap.AddMap("IsActive", "IsActive", false, false, false);
      _fieldMap.AddMap("StartDate", "StartDate", false, false, false);
      _fieldMap.AddMap("RecurrencyId", "RecurrencyId", false, false, false);
      _fieldMap.AddMap("Every", "Every", false, false, false);
      _fieldMap.AddMap("Weekday", "Weekday", false, false, false);
      _fieldMap.AddMap("Monthday", "Monthday", false, false, false);
      _fieldMap.AddMap("LastRun", "LastRun", false, false, false);
      _fieldMap.AddMap("IsSuccessful", "IsSuccessful", false, false, false);
      _fieldMap.AddMap("NextRun", "NextRun", false, false, false);
      _fieldMap.AddMap("CreatorId", "CreatorId", false, false, false);
      _fieldMap.AddMap("ModifierId", "ModifierId", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("LockProcessId", "LockProcessId", false, false, false);
            
    }
  }
  
}
