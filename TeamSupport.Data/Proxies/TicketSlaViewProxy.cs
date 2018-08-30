using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TicketSlaViewItem : BaseItem
  {
    public TicketSlaViewItemProxy GetProxy()
    {
      TicketSlaViewItemProxy result = new TicketSlaViewItemProxy();
      result.WarningInitialResponse = this.WarningInitialResponse;
      result.ViolationInitialResponse = this.ViolationInitialResponse;
      result.WarningLastAction = this.WarningLastAction;
      result.ViolationLastAction = this.ViolationLastAction;
      result.WarningTimeClosed = this.WarningTimeClosed;
      result.ViolationTimeClosed = this.ViolationTimeClosed;
      result.TicketID = this.TicketID;
       
       
       
      return result;
    }	
  }
}
