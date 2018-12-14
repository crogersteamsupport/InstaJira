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
  [KnownType(typeof(SearchCustomFilterProxy))]
  public class SearchCustomFilterProxy
  {
    public SearchCustomFilterProxy() {}
    [DataMember] public int CustomFilterID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public int TableID { get; set; }
    [DataMember] public int FieldID { get; set; }
    [DataMember] public string Measure { get; set; }
    [DataMember] public string TestValue { get; set; }
    [DataMember] public bool MatchAll { get; set; }
          
  }
  
  public partial class SearchCustomFilter : BaseItem
  {
    public SearchCustomFilterProxy GetProxy()
    {
      SearchCustomFilterProxy result = new SearchCustomFilterProxy();
      result.MatchAll = this.MatchAll;
      result.TestValue = this.TestValue;
      result.Measure = this.Measure;
      result.FieldID = this.FieldID;
      result.TableID = this.TableID;
      result.UserID = this.UserID;
      result.CustomFilterID = this.CustomFilterID;
       
       
       
      return result;
    }	
  }
}
