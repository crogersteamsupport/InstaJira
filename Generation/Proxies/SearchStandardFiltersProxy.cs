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
  [KnownType(typeof(SearchStandardFilterProxy))]
  public class SearchStandardFilterProxy
  {
    public SearchStandardFilterProxy() {}
    [DataMember] public int StandardFilterID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public bool Tickets { get; set; }
    [DataMember] public bool KnowledgeBase { get; set; }
    [DataMember] public bool Wikis { get; set; }
    [DataMember] public bool Notes { get; set; }
    [DataMember] public bool ProductVersions { get; set; }
    [DataMember] public bool WaterCooler { get; set; }
    [DataMember] public bool Tasks { get; set; }
          
  }
  
  public partial class SearchStandardFilter : BaseItem
  {
    public SearchStandardFilterProxy GetProxy()
    {
      SearchStandardFilterProxy result = new SearchStandardFilterProxy();
      result.Tasks = this.Tasks;
      result.WaterCooler = this.WaterCooler;
      result.ProductVersions = this.ProductVersions;
      result.Notes = this.Notes;
      result.Wikis = this.Wikis;
      result.KnowledgeBase = this.KnowledgeBase;
      result.Tickets = this.Tickets;
      result.UserID = this.UserID;
      result.StandardFilterID = this.StandardFilterID;
       
       
       
      return result;
    }	
  }
}
