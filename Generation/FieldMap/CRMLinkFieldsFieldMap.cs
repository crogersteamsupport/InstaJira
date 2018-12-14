using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class CRMLinkFields
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CRMFieldID", "CRMFieldID", false, false, false);
      _fieldMap.AddMap("CRMLinkID", "CRMLinkID", false, false, false);
      _fieldMap.AddMap("CRMObjectName", "CRMObjectName", false, false, false);
      _fieldMap.AddMap("CRMFieldName", "CRMFieldName", false, false, false);
      _fieldMap.AddMap("CustomFieldID", "CustomFieldID", false, false, false);
      _fieldMap.AddMap("TSFieldName", "TSFieldName", false, false, false);
            
    }
  }
  
}
