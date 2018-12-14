using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ExceptionLogs
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ExceptionLogID", "ExceptionLogID", false, false, false);
      _fieldMap.AddMap("URL", "URL", false, false, false);
      _fieldMap.AddMap("PageInfo", "PageInfo", false, false, false);
      _fieldMap.AddMap("ExceptionName", "ExceptionName", false, false, false);
      _fieldMap.AddMap("Message", "Message", false, false, false);
      _fieldMap.AddMap("StackTrace", "StackTrace", false, false, false);
      _fieldMap.AddMap("Browser", "Browser", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
            
    }
  }
  
}
