using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ChatClients
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ChatClientID", "ChatClientID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("FirstName", "FirstName", false, false, false);
      _fieldMap.AddMap("LastName", "LastName", false, false, false);
      _fieldMap.AddMap("Email", "Email", false, false, false);
      _fieldMap.AddMap("CompanyName", "CompanyName", false, false, false);
      _fieldMap.AddMap("LastPing", "LastPing", false, false, false);
      _fieldMap.AddMap("LinkedUserID", "LinkedUserID", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
            
    }
  }
  
}
