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
  [KnownType(typeof(OrganizationEmailProxy))]
  public class OrganizationEmailProxy
  {
    public OrganizationEmailProxy() {}
    [DataMember] public int OrganizationEmailID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int EmailTemplateID { get; set; }
    [DataMember] public string Subject { get; set; }
    [DataMember] public string Header { get; set; }
    [DataMember] public string Footer { get; set; }
    [DataMember] public string Body { get; set; }
    [DataMember] public bool IsHtml { get; set; }
    [DataMember] public bool UseGlobalTemplate { get; set; }
    [DataMember] public int? ProductFamilyID { get; set; }
          
  }
}