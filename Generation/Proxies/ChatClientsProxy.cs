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
  [KnownType(typeof(ChatClientProxy))]
  public class ChatClientProxy
  {
    public ChatClientProxy() {}
    [DataMember] public int ChatClientID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string FirstName { get; set; }
    [DataMember] public string LastName { get; set; }
    [DataMember] public string Email { get; set; }
    [DataMember] public string CompanyName { get; set; }
    [DataMember] public DateTime LastPing { get; set; }
    [DataMember] public int? LinkedUserID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
          
  }
  
  public partial class ChatClient : BaseItem
  {
    public ChatClientProxy GetProxy()
    {
      ChatClientProxy result = new ChatClientProxy();
      result.LinkedUserID = this.LinkedUserID;
      result.CompanyName = this.CompanyName;
      result.Email = this.Email;
      result.LastName = this.LastName;
      result.FirstName = this.FirstName;
      result.OrganizationID = this.OrganizationID;
      result.ChatClientID = this.ChatClientID;
       
      result.LastPing = DateTime.SpecifyKind(this.LastPingUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
