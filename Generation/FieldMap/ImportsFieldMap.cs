using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class Imports
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ImportID", "ImportID", false, false, false);
      _fieldMap.AddMap("FileName", "FileName", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("ImportGUID", "ImportGUID", false, false, false);
      _fieldMap.AddMap("RefType", "RefType", false, false, false);
      _fieldMap.AddMap("AuxID", "AuxID", false, false, false);
      _fieldMap.AddMap("IsDone", "IsDone", false, false, false);
      _fieldMap.AddMap("IsRunning", "IsRunning", false, false, false);
      _fieldMap.AddMap("IsDeleted", "IsDeleted", false, false, false);
      _fieldMap.AddMap("NeedsDeleted", "NeedsDeleted", false, false, false);
      _fieldMap.AddMap("TotalRows", "TotalRows", false, false, false);
      _fieldMap.AddMap("CompletedRows", "CompletedRows", false, false, false);
      _fieldMap.AddMap("DateStarted", "DateStarted", false, false, false);
      _fieldMap.AddMap("DateEnded", "DateEnded", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("IsRolledBack", "IsRolledBack", false, false, false);
            
    }
  }
  
}
