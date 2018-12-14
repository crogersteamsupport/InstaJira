using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class Emails
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("EmailID", "EmailID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("Description", "Description", false, false, false);
      _fieldMap.AddMap("FromAddress", "FromAddress", false, false, false);
      _fieldMap.AddMap("ToAddress", "ToAddress", false, false, false);
      _fieldMap.AddMap("CCAddress", "CCAddress", false, false, false);
      _fieldMap.AddMap("BCCAddress", "BCCAddress", false, false, false);
      _fieldMap.AddMap("Subject", "Subject", false, false, false);
      _fieldMap.AddMap("Body", "Body", false, false, false);
      _fieldMap.AddMap("Attachments", "Attachments", false, false, false);
      _fieldMap.AddMap("Size", "Size", false, false, false);
      _fieldMap.AddMap("IsSuccess", "IsSuccess", false, false, false);
      _fieldMap.AddMap("IsWaiting", "IsWaiting", false, false, false);
      _fieldMap.AddMap("IsHtml", "IsHtml", false, false, false);
      _fieldMap.AddMap("Attempts", "Attempts", false, false, false);
      _fieldMap.AddMap("NextAttempt", "NextAttempt", false, false, false);
      _fieldMap.AddMap("DateSent", "DateSent", false, false, false);
      _fieldMap.AddMap("LastFailedReason", "LastFailedReason", false, false, false);
      _fieldMap.AddMap("EmailPostID", "EmailPostID", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("LockProcessID", "LockProcessID", false, false, false);
            
    }
  }
  
}
