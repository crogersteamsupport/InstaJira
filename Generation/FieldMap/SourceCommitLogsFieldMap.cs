using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class SourceCommitLogs
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CommitID", "CommitID", false, false, false);
      _fieldMap.AddMap("CommitDateTime", "CommitDateTime", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("ProductID", "ProductID", false, false, false);
      _fieldMap.AddMap("VersionID", "VersionID", false, false, false);
      _fieldMap.AddMap("UserName", "UserName", false, false, false);
      _fieldMap.AddMap("Description", "Description", false, false, false);
      _fieldMap.AddMap("Revision", "Revision", false, false, false);
      _fieldMap.AddMap("Tickets", "Tickets", false, false, false);
      _fieldMap.AddMap("RawCommitText", "RawCommitText", false, false, false);
      _fieldMap.AddMap("Status", "Status", false, false, false);
            
    }
  }
  
}
