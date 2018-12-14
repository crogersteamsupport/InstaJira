using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;


namespace TeamSupport.Data
{
  
  public partial class Report : BaseItem
  {
    public ReportProxy GetProxy()
    {
      ReportProxy result = new ReportProxy();

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
      result.CustomRefType = ReferenceType.None;
      result.CustomFieldKeyName = this.CustomFieldKeyName;
      result.Query = this.Query;
      result.Description = (this.Description);
      result.Name = (this.Name);
      result.OrganizationID = this.OrganizationID;
      result.ReportID = this.ReportID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
      result.DateEdited = DateTime.SpecifyKind(this.DateEditedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
