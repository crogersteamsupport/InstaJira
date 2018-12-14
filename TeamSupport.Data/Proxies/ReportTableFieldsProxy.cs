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
  [KnownType(typeof(ReportTableFieldProxy))]
  public class ReportTableFieldProxy
  {
    public ReportTableFieldProxy() {}
    [DataMember] public int ReportTableFieldID { get; set; }
    [DataMember] public int ReportTableID { get; set; }
    [DataMember] public string FieldName { get; set; }
    [DataMember] public string Alias { get; set; }
    [DataMember] public string DataType { get; set; }
    [DataMember] public int Size { get; set; }
    [DataMember] public bool IsVisible { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public int? LookupTableID { get; set; }
          
  }
  
  public partial class ReportTableField : BaseItem
  {
    public ReportTableFieldProxy GetProxy()
    {
      ReportTableFieldProxy result = new ReportTableFieldProxy();
      result.LookupTableID = this.LookupTableID;
      result.Description = (this.Description);
      result.IsVisible = this.IsVisible;
      result.Size = this.Size;
      result.DataType = this.DataType;
      result.Alias = (this.Alias);
      result.FieldName = (this.FieldName);
      result.ReportTableID = this.ReportTableID;
      result.ReportTableFieldID = this.ReportTableFieldID;
       
       
       
      return result;
    }	
  }
}
