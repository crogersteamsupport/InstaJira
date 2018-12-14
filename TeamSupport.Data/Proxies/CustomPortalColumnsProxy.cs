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
  [KnownType(typeof(CustomPortalColumnProxy))]
  public class CustomPortalColumnProxy
  {
    public CustomPortalColumnProxy() {}
    [DataMember] public int CustomColumnID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int Position { get; set; }
    [DataMember] public int? StockFieldID { get; set; }
    [DataMember] public int? CustomFieldID { get; set; }
    [DataMember] public string FieldText { get; set; }
          
  }
  
  public partial class CustomPortalColumn : BaseItem
  {
    public CustomPortalColumnProxy GetProxy()
    {
      CustomPortalColumnProxy result = new CustomPortalColumnProxy();
      result.CustomFieldID = this.CustomFieldID;
      result.StockFieldID = this.StockFieldID;
      result.Position = this.Position;
      result.OrganizationID = this.OrganizationID;
      result.CustomColumnID = this.CustomColumnID;

      if (result.CustomFieldID != null)
      {
        CustomFields cf = new CustomFields(BaseCollection.LoginUser);
        cf.LoadByCustomFieldID((int)result.CustomFieldID);

        TicketTypes ticketTypes = new TicketTypes(BaseCollection.LoginUser);
        ticketTypes.LoadAllPositions(BaseCollection.LoginUser.OrganizationID);

        TicketType ticketType = ticketTypes.FindByTicketTypeID(cf[0].AuxID);
        if (ticketType == null)
        {
            result.FieldText = cf[0].Name;
        }
        else
        {
            result.FieldText = string.Format("{0} ({1})", cf[0].Name, ticketType.Name);
        }
        
      }
      else
      {
          ReportTableFields rt = new ReportTableFields(BaseCollection.LoginUser);
          rt.LoadByReportTableFieldID((int)result.StockFieldID);
          result.FieldText = rt[0].Alias;
      }
       
       
       
      return result;
    }	
  }
}
