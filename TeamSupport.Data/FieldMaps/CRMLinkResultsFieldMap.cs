using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class CRMLinkResults
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CRMResultsID", "CRMResultsID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("AttemptDateTime", "AttemptDateTime", false, false, false);
      _fieldMap.AddMap("AttemptResult", "AttemptResult", false, false, false);
            
    }
  }
  
}
