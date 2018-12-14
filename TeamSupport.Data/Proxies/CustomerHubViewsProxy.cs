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
  [KnownType(typeof(CustomerHubViewProxy))]
  public class CustomerHubViewProxy
  {
    public CustomerHubViewProxy() {}
    [DataMember] public int CustomerHubViewID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Route { get; set; }
    [DataMember] public bool IsActive { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
          
  }
  
  public partial class CustomerHubView : BaseItem
  {
    public CustomerHubViewProxy GetProxy()
    {
      CustomerHubViewProxy result = new CustomerHubViewProxy();
      result.IsActive = this.IsActive;
      result.Route = this.Route;
      result.Name = this.Name;
      result.CustomerHubViewID = this.CustomerHubViewID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
