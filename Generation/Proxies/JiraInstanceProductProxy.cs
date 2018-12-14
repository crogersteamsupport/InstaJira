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
  [KnownType(typeof(JiraInstanceProductItemProxy))]
  public class JiraInstanceProductItemProxy
  {
    public JiraInstanceProductItemProxy() {}
    [DataMember] public int JiraInstanceProductId { get; set; }
    [DataMember] public int CrmLinkId { get; set; }
    [DataMember] public int? ProductId { get; set; }
          
  }
  
  public partial class JiraInstanceProductItem : BaseItem
  {
    public JiraInstanceProductItemProxy GetProxy()
    {
      JiraInstanceProductItemProxy result = new JiraInstanceProductItemProxy();
      result.ProductId = this.ProductId;
      result.CrmLinkId = this.CrmLinkId;
      result.JiraInstanceProductId = this.JiraInstanceProductId;
       
       
       
      return result;
    }	
  }
}
