using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class Chat : BaseItem
  {
    public ChatProxy GetProxy()
    {
      ChatProxy result = new ChatProxy();
      result.ActionID = this.ActionID;
      result.InitiatorType = this.InitiatorType;
      result.InitiatorID = this.InitiatorID;
      result.OrganizationID = this.OrganizationID;
      result.ChatID = this.ChatID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
