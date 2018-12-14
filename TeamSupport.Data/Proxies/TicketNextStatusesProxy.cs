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
  [KnownType(typeof(TicketNextStatusProxy))]
  public class TicketNextStatusProxy
  {
    public TicketNextStatusProxy() {}
    [DataMember] public int TicketNextStatusID { get; set; }
    [DataMember] public int CurrentStatusID { get; set; }
    [DataMember] public int NextStatusID { get; set; }
    [DataMember] public int Position { get; set; }
    //[DataMember] public DateTime DateCreated { get; set; }
    //[DataMember] public DateTime DateModified { get; set; }
    //[DataMember] public int CreatorID { get; set; }
    //[DataMember] public int ModifierID { get; set; }
          
  }
  
  public partial class TicketNextStatus : BaseItem
  {
    public TicketNextStatusProxy GetProxy()
    {
      TicketNextStatusProxy result = new TicketNextStatusProxy();
      //result.ModifierID = this.ModifierID;
      //result.CreatorID = this.CreatorID;
      result.Position = this.Position;
      result.NextStatusID = this.NextStatusID;
      result.CurrentStatusID = this.CurrentStatusID;
      result.TicketNextStatusID = this.TicketNextStatusID;
       
       
       
      return result;
    }	
  }
}
