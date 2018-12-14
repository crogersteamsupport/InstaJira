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
  [KnownType(typeof(ReportFieldProxy))]
  public class ReportFieldProxy
  {
    public ReportFieldProxy() {}
    [DataMember] public int ReportFieldID { get; set; }
    [DataMember] public int ReportID { get; set; }
    [DataMember] public int LinkedFieldID { get; set; }
    [DataMember] public bool IsCustomField { get; set; }
          
  }
  
  public partial class ReportField : BaseItem
  {
    public ReportFieldProxy GetProxy()
    {
      ReportFieldProxy result = new ReportFieldProxy();
      result.IsCustomField = this.IsCustomField;
      result.LinkedFieldID = this.LinkedFieldID;
      result.ReportID = this.ReportID;
      result.ReportFieldID = this.ReportFieldID;
       
       
       
      return result;
    }	
  }
}
