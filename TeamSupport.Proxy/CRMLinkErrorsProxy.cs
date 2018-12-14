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
  [KnownType(typeof(CRMLinkErrorProxy))]
  public class CRMLinkErrorProxy
  {
    public CRMLinkErrorProxy() {}
    [DataMember] public int CRMLinkErrorID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string CRMType { get; set; }
    [DataMember] public string Orientation { get; set; }
    [DataMember] public string ObjectType { get; set; }
    [DataMember] public string ObjectID { get; set; }
    [DataMember] public string ObjectFieldName { get; set; }
    [DataMember] public string ObjectData { get; set; }
    [DataMember] public string Exception { get; set; }
    [DataMember] public string OperationType { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public string ErrorMessage { get; set; }
    [DataMember] public int ErrorCount { get; set; }
    [DataMember] public bool IsCleared { get; set; }
          
  }
}
