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
  [KnownType(typeof(AssetTicketProxy))]
  public class AssetTicketProxy
  {
    public AssetTicketProxy() {}
    [DataMember] public int TicketID { get; set; }
    [DataMember] public int AssetID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int? CreatorID { get; set; }
    [DataMember] public int? ImportFileID { get; set; }
          
  }
  
  public partial class AssetTicket : BaseItem
  {
    public AssetTicketProxy GetProxy()
    {
      AssetTicketProxy result = new AssetTicketProxy();
      result.ImportFileID = this.ImportFileID;
      result.CreatorID = this.CreatorID;
      result.AssetID = this.AssetID;
      result.TicketID = this.TicketID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
