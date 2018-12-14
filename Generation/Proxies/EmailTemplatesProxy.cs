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
  [KnownType(typeof(EmailTemplateProxy))]
  public class EmailTemplateProxy
  {
    public EmailTemplateProxy() {}
    [DataMember] public int EmailTemplateID { get; set; }
    [DataMember] public int Position { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public bool IsTSOnly { get; set; }
    [DataMember] public string Subject { get; set; }
    [DataMember] public string Header { get; set; }
    [DataMember] public string Footer { get; set; }
    [DataMember] public bool IsHtml { get; set; }
    [DataMember] public bool IncludeDelimiter { get; set; }
    [DataMember] public bool IsEmail { get; set; }
    [DataMember] public bool UseGlobalTemplate { get; set; }
    [DataMember] public string Body { get; set; }
          
  }
  
  public partial class EmailTemplate : BaseItem
  {
    public EmailTemplateProxy GetProxy()
    {
      EmailTemplateProxy result = new EmailTemplateProxy();
      result.Body = this.Body;
      result.UseGlobalTemplate = this.UseGlobalTemplate;
      result.IsEmail = this.IsEmail;
      result.IncludeDelimiter = this.IncludeDelimiter;
      result.IsHtml = this.IsHtml;
      result.Footer = this.Footer;
      result.Header = this.Header;
      result.Subject = this.Subject;
      result.IsTSOnly = this.IsTSOnly;
      result.Description = this.Description;
      result.Name = this.Name;
      result.Position = this.Position;
      result.EmailTemplateID = this.EmailTemplateID;
       
       
       
      return result;
    }	
  }
}
