using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
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
