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
  [KnownType(typeof(AssetAssignmentProxy))]
  public class AssetAssignmentProxy
  {
    public AssetAssignmentProxy() {}
    [DataMember] public int AssetAssignmentsID { get; set; }
    [DataMember] public int HistoryID { get; set; }
    [DataMember] public int? ImportFileID { get; set; }
          
  }
  
  public partial class AssetAssignment : BaseItem
  {
    public AssetAssignmentProxy GetProxy()
    {
      AssetAssignmentProxy result = new AssetAssignmentProxy();
      result.ImportFileID = this.ImportFileID;
      result.HistoryID = this.HistoryID;
      result.AssetAssignmentsID = this.AssetAssignmentsID;
       
       
       
      return result;
    }	
  }
}
