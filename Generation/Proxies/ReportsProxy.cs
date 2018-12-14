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
  [KnownType(typeof(ReportProxy))]
  public class ReportProxy
  {
    public ReportProxy() {}
    [DataMember] public int ReportID { get; set; }
    [DataMember] public int? OrganizationID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public string Query { get; set; }
    [DataMember] public string CustomFieldKeyName { get; set; }
    [DataMember] public ReferenceType CustomRefType { get; set; }
    [DataMember] public int? CustomAuxID { get; set; }
    [DataMember] public int? ReportSubcategoryID { get; set; }
    [DataMember] public string QueryObject { get; set; }
    [DataMember] public string ExternalURL { get; set; }
    [DataMember] public string LastSqlExecuted { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public ReportType ReportType { get; set; }
    [DataMember] public string ReportDef { get; set; }
    [DataMember] public ReportType ReportDefType { get; set; }
    [DataMember] public DateTime DateEdited { get; set; }
    [DataMember] public int EditorID { get; set; }
    [DataMember] public int? FolderID { get; set; }
    [DataMember] public bool IsPrivate { get; set; }
          
  }
  
  public partial class Report : BaseItem
  {
    public ReportProxy GetProxy()
    {
      ReportProxy result = new ReportProxy();
      result.IsPrivate = this.IsPrivate;
      result.FolderID = this.FolderID;
      result.EditorID = this.EditorID;
      result.ReportDefType = this.ReportDefType;
      result.ReportDef = this.ReportDef;
      result.ReportType = this.ReportType;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.LastSqlExecuted = this.LastSqlExecuted;
      result.ExternalURL = this.ExternalURL;
      result.QueryObject = this.QueryObject;
      result.ReportSubcategoryID = this.ReportSubcategoryID;
      result.CustomAuxID = this.CustomAuxID;
      result.CustomRefType = this.CustomRefType;
      result.CustomFieldKeyName = this.CustomFieldKeyName;
      result.Query = this.Query;
      result.Description = this.Description;
      result.Name = this.Name;
      result.OrganizationID = this.OrganizationID;
      result.ReportID = this.ReportID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
      result.DateEdited = DateTime.SpecifyKind(this.DateEditedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
