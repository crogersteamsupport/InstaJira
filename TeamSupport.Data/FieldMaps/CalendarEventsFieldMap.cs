using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class CalendarEvents
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CalendarID", "CalendarID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("StartDate", "StartDate", false, false, false);
      _fieldMap.AddMap("EndDate", "EndDate", false, false, false);
      _fieldMap.AddMap("Title", "Title", false, false, false);
      _fieldMap.AddMap("Description", "Description", false, false, false);
      _fieldMap.AddMap("Repeat", "Repeat", false, false, false);
      _fieldMap.AddMap("RepeatFrequency", "RepeatFrequency", false, false, false);
      _fieldMap.AddMap("LastModified", "LastModified", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("AllDay", "AllDay", false, false, false);
      _fieldMap.AddMap("StartDateUTC", "StartDateUTC", false, false, false);
      _fieldMap.AddMap("EndDateUTC", "EndDateUTC", false, false, false);
      _fieldMap.AddMap("IsHoliday", "IsHoliday", false, false, false);
            
    }
  }
  
}
