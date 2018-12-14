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
  [KnownType(typeof(EmailProxy))]
  public class EmailProxy
  {
    public EmailProxy() {}
    [DataMember] public int EmailID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public string FromAddress { get; set; }
    [DataMember] public string ToAddress { get; set; }
    [DataMember] public string CCAddress { get; set; }
    [DataMember] public string BCCAddress { get; set; }
    [DataMember] public string Subject { get; set; }
    [DataMember] public string Body { get; set; }
    [DataMember] public string Attachments { get; set; }
    [DataMember] public int Size { get; set; }
    [DataMember] public bool IsSuccess { get; set; }
    [DataMember] public bool IsWaiting { get; set; }
    [DataMember] public bool IsHtml { get; set; }
    [DataMember] public int Attempts { get; set; }
    [DataMember] public DateTime NextAttempt { get; set; }
    [DataMember] public DateTime? DateSent { get; set; }
    [DataMember] public string LastFailedReason { get; set; }
    [DataMember] public int? EmailPostID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public string LockProcessID { get; set; }
          
  }
  
  public partial class Email : BaseItem
  {
    public EmailProxy GetProxy()
    {
      EmailProxy result = new EmailProxy();
      result.LockProcessID = this.LockProcessID;
      result.EmailPostID = this.EmailPostID;
      result.LastFailedReason = this.LastFailedReason;
      result.Attempts = this.Attempts;
      result.IsHtml = this.IsHtml;
      result.IsWaiting = this.IsWaiting;
      result.IsSuccess = this.IsSuccess;
      result.Size = this.Size;
      result.Attachments = this.Attachments;
      result.Body = this.Body;
      result.Subject = this.Subject;
      result.BCCAddress = this.BCCAddress;
      result.CCAddress = this.CCAddress;
      result.ToAddress = this.ToAddress;
      result.FromAddress = this.FromAddress;
      result.Description = this.Description;
      result.OrganizationID = this.OrganizationID;
      result.EmailID = this.EmailID;
       
      result.NextAttempt = DateTime.SpecifyKind(this.NextAttemptUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
      result.DateSent = this.DateSentUtc == null ? this.DateSentUtc : DateTime.SpecifyKind((DateTime)this.DateSentUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
