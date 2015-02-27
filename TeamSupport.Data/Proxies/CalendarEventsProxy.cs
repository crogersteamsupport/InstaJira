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
          
  }
  
  public partial class CalendarEvent : BaseItem
  {
    public CalendarEventProxy GetProxy()
    {
      CalendarEventProxy result = new CalendarEventProxy();
      result.CreatorID = this.CreatorID;
      result.RepeatFrequency = this.RepeatFrequency;
      result.Repeat = this.Repeat;
      result.Description = this.Description;
      result.Title = this.Title;
      result.OrganizationID = this.OrganizationID;
      result.CalendarID = this.CalendarID;
       
      result.StartDate = DateTime.SpecifyKind(this.StartDateUtc, DateTimeKind.Utc);
      result.LastModified = DateTime.SpecifyKind(this.LastModifiedUtc, DateTimeKind.Utc);
       
      result.EndDate = this.EndDateUtc == null ? this.EndDateUtc : DateTime.SpecifyKind((DateTime)this.EndDateUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
