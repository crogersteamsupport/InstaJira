using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  
  public partial class ReportTable : BaseItem
  {
    public ReportTableProxy GetProxy()
    {
      ReportTableProxy result = new ReportTableProxy();
      result.LookupOrderBy = this.LookupOrderBy;
      result.LookupDisplayClause = this.LookupDisplayClause;
      result.LookupKeyFieldName = this.LookupKeyFieldName;
      result.OrganizationIDFieldName = this.OrganizationIDFieldName;
      result.IsCategory = this.IsCategory;
      result.CustomFieldRefType = this.CustomFieldRefType;
      result.Alias = this.Alias;
      result.TableName = this.TableName;
      result.ReportTableID = this.ReportTableID;
      result.UseTicketRights = this.UseTicketRights;
       
       
      return result;
    }	
  }
}
