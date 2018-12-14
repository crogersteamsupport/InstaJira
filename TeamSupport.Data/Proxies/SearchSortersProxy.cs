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
  [KnownType(typeof(SearchSorterProxy))]
  public class SearchSorterProxy
  {
    public SearchSorterProxy() {}
    [DataMember] public int SorterID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public int TableID { get; set; }
    [DataMember] public int FieldID { get; set; }
    [DataMember] public bool Descending { get; set; }
          
  }
  
  public partial class SearchSorter : BaseItem
  {
    public SearchSorterProxy GetProxy()
    {
      SearchSorterProxy result = new SearchSorterProxy();
      result.Descending = this.Descending;
      result.FieldID = this.FieldID;
      result.TableID = this.TableID;
      result.UserID = this.UserID;
      result.SorterID = this.SorterID;
       
       
       
      return result;
    }	
  }
}
