using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(SlaTriggerProxy))]
  public class SlaTriggerProxy
  {
    public SlaTriggerProxy() {}
    [DataMember] public int SlaTriggerID { get; set; }
    [DataMember] public int SlaLevelID { get; set; }
    [DataMember] public int TicketTypeID { get; set; }
    [DataMember] public int TicketSeverityID { get; set; }
    [DataMember] public int TimeInitialResponse { get; set; }
    [DataMember] public int TimeLastAction { get; set; }
    [DataMember] public int TimeToClose { get; set; }
    [DataMember] public bool NotifyUserOnWarning { get; set; }
    [DataMember] public bool NotifyGroupOnWarning { get; set; }
    [DataMember] public bool NotifyUserOnViolation { get; set; }
    [DataMember] public bool NotifyGroupOnViolation { get; set; }
    [DataMember] public int WarningTime { get; set; }
    [DataMember] public bool UseBusinessHours { get; set; }
    [DataMember] public bool PauseOnHoliday { get; set; }
    [DataMember] public int Weekdays { get; set; }
    [DataMember] public DateTime? DayStart { get; set; }
    [DataMember] public DateTime? DayEnd { get; set; }
    [DataMember] public string TimeZone { get; set; }
    [DataMember] public bool NoBusinessHours { get; set; }
          
  }
}
