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
  [KnownType(typeof(CRMLinkSynchedOrganizationProxy))]
  public class CRMLinkSynchedOrganizationProxy
  {
    public CRMLinkSynchedOrganizationProxy() {}
    [DataMember] public int CRMLinkSynchedOrganizationsID { get; set; }
    [DataMember] public int CRMLinkTableID { get; set; }
    [DataMember] public string OrganizationCRMID { get; set; }
          
  }
  
  public partial class CRMLinkSynchedOrganization : BaseItem
  {
    public CRMLinkSynchedOrganizationProxy GetProxy()
    {
      CRMLinkSynchedOrganizationProxy result = new CRMLinkSynchedOrganizationProxy();
      result.OrganizationCRMID = this.OrganizationCRMID;
      result.CRMLinkTableID = this.CRMLinkTableID;
      result.CRMLinkSynchedOrganizationsID = this.CRMLinkSynchedOrganizationsID;
       
       
       
      return result;
    }	
  }
}
