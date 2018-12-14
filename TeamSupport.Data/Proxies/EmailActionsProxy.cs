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
  [KnownType(typeof(EmailActionProxy))]
  public class EmailActionProxy
  {
    public EmailActionProxy() {}
    [DataMember] public int EMailActionID { get; set; }
    [DataMember] public DateTime? DateTime { get; set; }
    [DataMember] public string EMailFrom { get; set; }
    [DataMember] public string EMailTo { get; set; }
    [DataMember] public string EMailSubject { get; set; }
    [DataMember] public string EMailBody { get; set; }
    [DataMember] public string OrganizationGUID { get; set; }
    [DataMember] public bool? ActionAdded { get; set; }
    [DataMember] public string Status { get; set; }
    [DataMember] public int? TicketID { get; set; }
    [DataMember] public int? OrganizationID { get; set; }
          
  }
  
  public partial class EmailAction : BaseItem
  {
    public EmailActionProxy GetProxy()
    {
      EmailActionProxy result = new EmailActionProxy();
      result.OrganizationID = this.OrganizationID;
      result.TicketID = this.TicketID;
      result.Status = this.Status;
      result.ActionAdded = this.ActionAdded;
      result.OrganizationGUID = this.OrganizationGUID;
      result.EMailBody = this.EMailBody;
      result.EMailSubject = this.EMailSubject;
      result.EMailTo = this.EMailTo;
      result.EMailFrom = this.EMailFrom;
      result.EMailActionID = this.EMailActionID;
       
       
      //result.DateTime = this.DateTimeUtc == null ? this.DateTimeUtc : DateTime.SpecifyKind((DateTime)this.DateTimeUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
