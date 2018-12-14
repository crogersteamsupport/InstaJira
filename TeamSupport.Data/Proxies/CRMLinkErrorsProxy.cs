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
  
  public partial class CRMLinkError : BaseItem
  {
    public CRMLinkErrorProxy GetProxy()
    {
      CRMLinkErrorProxy result = new CRMLinkErrorProxy();
      result.IsCleared = this.IsCleared;
      result.ErrorCount = this.ErrorCount;
      result.ErrorMessage = this.ErrorMessage;
      result.OperationType = this.OperationType;
      result.Exception = this.Exception;
      result.ObjectData = this.ObjectData;
      result.ObjectFieldName = this.ObjectFieldName;
      result.ObjectID = this.ObjectID;
      result.ObjectType = this.ObjectType;
      result.Orientation = this.Orientation;
      result.CRMType = this.CRMType;
      result.OrganizationID = this.OrganizationID;
      result.CRMLinkErrorID = this.CRMLinkErrorID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
