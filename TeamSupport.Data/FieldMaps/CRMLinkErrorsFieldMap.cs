using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class CRMLinkErrors
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CRMLinkErrorID", "CRMLinkErrorID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("CRMType", "CRMType", false, false, false);
      _fieldMap.AddMap("Orientation", "Orientation", false, false, false);
      _fieldMap.AddMap("ObjectType", "ObjectType", false, false, false);
      _fieldMap.AddMap("ObjectID", "ObjectID", false, false, false);
      _fieldMap.AddMap("ObjectFieldName", "ObjectFieldName", false, false, false);
      _fieldMap.AddMap("ObjectData", "ObjectData", false, false, false);
      _fieldMap.AddMap("Exception", "Exception", false, false, false);
      _fieldMap.AddMap("OperationType", "OperationType", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("ErrorMessage", "ErrorMessage", false, false, false);
      _fieldMap.AddMap("ErrorCount", "ErrorCount", false, false, false);
      _fieldMap.AddMap("IsCleared", "IsCleared", false, false, false);
            
    }
  }
  
}
