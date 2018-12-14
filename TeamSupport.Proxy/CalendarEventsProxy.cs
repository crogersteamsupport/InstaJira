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
  [KnownType(typeof(CalendarEventProxy))]
  public class CalendarEventProxy
  {
    public CalendarEventProxy() {}
    [DataMember] public int CalendarID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public DateTime StartDate { get; set; }
    [DataMember] public DateTime? EndDate { get; set; }
    [DataMember] public string Title { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public bool? Repeat { get; set; }
    [DataMember] public int? RepeatFrequency { get; set; }
    [DataMember] public DateTime LastModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public bool AllDay { get; set; }
    [DataMember] public DateTime? StartDateUTC { get; set; }
    [DataMember] public DateTime? EndDateUTC { get; set; }
    [DataMember] public bool IsHoliday { get; set; }
          
  }
}
