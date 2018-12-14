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
    [DataMember] public bool IsReadOnly { get; set; }
    [DataMember] public bool IsOpenable { get; set; }
    [DataMember] public bool IsEmail { get; set; }
    [DataMember] public bool IsLink { get; set; }
    [DataMember] public bool IsSortable { get; set; }
          
  }
  
  public partial class ReportTableField : BaseItem
  {
    public ReportTableFieldProxy GetProxy()
    {
      ReportTableFieldProxy result = new ReportTableFieldProxy();
      result.IsSortable = this.IsSortable;
      result.IsLink = this.IsLink;
      result.IsEmail = this.IsEmail;
      result.IsOpenable = this.IsOpenable;
      result.IsReadOnly = this.IsReadOnly;
      result.LookupTableID = this.LookupTableID;
      result.Description = this.Description;
      result.IsVisible = this.IsVisible;
      result.Size = this.Size;
      result.DataType = this.DataType;
      result.Alias = this.Alias;
      result.FieldName = this.FieldName;
      result.ReportTableID = this.ReportTableID;
      result.ReportTableFieldID = this.ReportTableFieldID;
       
       
       
      return result;
    }	
  }
}
