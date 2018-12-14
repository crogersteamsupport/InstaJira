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
  [KnownType(typeof(ActionLinkToTFSItemProxy))]
  public class ActionLinkToTFSItemProxy
  {
    public ActionLinkToTFSItemProxy() {}
    [DataMember] public int id { get; set; }
    [DataMember] public int ActionID { get; set; }
    [DataMember] public DateTime? DateModifiedByTFSSync { get; set; }
    [DataMember] public int? TFSID { get; set; }
          
  }
  
  public partial class ActionLinkToTFSItem : BaseItem
  {
    public ActionLinkToTFSItemProxy GetProxy()
    {
      ActionLinkToTFSItemProxy result = new ActionLinkToTFSItemProxy();
      result.TFSID = this.TFSID;
      result.ActionID = this.ActionID;
      result.id = this.id;
       
       
      result.DateModifiedByTFSSync = this.DateModifiedByTFSSyncUtc == null ? this.DateModifiedByTFSSyncUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedByTFSSyncUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
