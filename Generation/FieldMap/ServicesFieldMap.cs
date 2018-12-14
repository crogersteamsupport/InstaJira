using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class Services
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ServiceID", "ServiceID", false, false, false);
      _fieldMap.AddMap("Name", "Name", false, false, false);
      _fieldMap.AddMap("Enabled", "Enabled", false, false, false);
      _fieldMap.AddMap("Interval", "Interval", false, false, false);
      _fieldMap.AddMap("LastStartTime", "LastStartTime", false, false, false);
      _fieldMap.AddMap("LastEndTime", "LastEndTime", false, false, false);
      _fieldMap.AddMap("LastResult", "LastResult", false, false, false);
      _fieldMap.AddMap("LastError", "LastError", false, false, false);
      _fieldMap.AddMap("ErrorCount", "ErrorCount", false, false, false);
      _fieldMap.AddMap("RunCount", "RunCount", false, false, false);
      _fieldMap.AddMap("RunTimeAvg", "RunTimeAvg", false, false, false);
      _fieldMap.AddMap("RunTimeMax", "RunTimeMax", false, false, false);
      _fieldMap.AddMap("AssemblyName", "AssemblyName", false, false, false);
      _fieldMap.AddMap("AutoStart", "AutoStart", false, false, false);
      _fieldMap.AddMap("HealthTime", "HealthTime", false, false, false);
      _fieldMap.AddMap("NameSpace", "NameSpace", false, false, false);
      _fieldMap.AddMap("HealthMaxMinutes", "HealthMaxMinutes", false, false, false);
            
    }
  }
  
}
