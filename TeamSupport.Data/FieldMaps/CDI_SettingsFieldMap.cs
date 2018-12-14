using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class CDI_Settings
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("TotalTicketsWeight", "TotalTicketsWeight", false, false, false);
      _fieldMap.AddMap("OpenTicketsWeight", "OpenTicketsWeight", false, false, false);
      _fieldMap.AddMap("Last30Weight", "Last30Weight", false, false, false);
      _fieldMap.AddMap("AvgDaysOpenWeight", "AvgDaysOpenWeight", false, false, false);
      _fieldMap.AddMap("AvgDaysToCloseWeight", "AvgDaysToCloseWeight", false, false, false);
      _fieldMap.AddMap("GreenUpperRange", "GreenUpperRange", false, false, false);
      _fieldMap.AddMap("YellowUpperRange", "YellowUpperRange", false, false, false);
      _fieldMap.AddMap("LastCompute", "LastCompute", false, false, false);
      _fieldMap.AddMap("NeedCompute", "NeedCompute", false, false, false);
            
    }
  }
  
}
