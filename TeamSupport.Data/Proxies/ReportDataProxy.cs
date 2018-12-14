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
  [KnownType(typeof(ReportDataItemProxy))]
  public class ReportDataItemProxy
  {
    public ReportDataItemProxy() {}
    [DataMember] public int ReportDataID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public int ReportID { get; set; }
    [DataMember] public string ReportData { get; set; }
    [DataMember] public string QueryObject { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public string OrderByClause { get; set; }
          
  }
  
  public partial class ReportDataItem : BaseItem
  {
    public ReportDataItemProxy GetProxy()
    {
      ReportDataItemProxy result = new ReportDataItemProxy();
      result.OrderByClause = this.OrderByClause;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.QueryObject = this.QueryObject;
      result.ReportData = this.ReportData;
      result.ReportID = this.ReportID;
      result.UserID = this.UserID;
      result.ReportDataID = this.ReportDataID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
