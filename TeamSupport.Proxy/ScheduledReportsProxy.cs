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
  [KnownType(typeof(ScheduledReportProxy))]
  public class ScheduledReportProxy
  {
    public ScheduledReportProxy() {}
    [DataMember] public int Id { get; set; }
    [DataMember] public string EmailSubject { get; set; }
    [DataMember] public string EmailBody { get; set; }
    [DataMember] public string EmailRecipients { get; set; }
    [DataMember] public int ReportId { get; set; }
    [DataMember] public int OrganizationId { get; set; }
    [DataMember] public short? RunCount { get; set; }
    [DataMember] public bool IsActive { get; set; }
    [DataMember] public DateTime StartDate { get; set; }
    [DataMember] public byte RecurrencyId { get; set; }
    [DataMember] public byte? Every { get; set; }
    [DataMember] public byte? Weekday { get; set; }
    [DataMember] public byte? Monthday { get; set; }
    [DataMember] public DateTime? LastRun { get; set; }
    [DataMember] public bool? IsSuccessful { get; set; }
    [DataMember] public DateTime? NextRun { get; set; }
    [DataMember] public int CreatorId { get; set; }
    [DataMember] public int? ModifierId { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime? DateModified { get; set; }
    [DataMember] public int? LockProcessId { get; set; }
    [DataMember] public int FilePathID { get; set; }
          
  }
}
