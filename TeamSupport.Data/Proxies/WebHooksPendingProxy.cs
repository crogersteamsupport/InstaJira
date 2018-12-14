using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  
  public partial class WebHooksPendingItem : BaseItem
  {
    public WebHooksPendingItemProxy GetProxy()
    {
      WebHooksPendingItemProxy result = new WebHooksPendingItemProxy();
      result.IsProcessing = this.IsProcessing;
      result.Inbound = this.Inbound;
      result.Token = this.Token;
      result.BodyData = this.BodyData;
      result.Url = this.Url;
      result.Type = this.Type;
      result.RefId = this.RefId;
      result.RefType = this.RefType;
      result.OrganizationId = this.OrganizationId;
      result.Id = this.Id;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
