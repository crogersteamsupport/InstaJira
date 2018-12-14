using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  
  public partial class CalendarRefItem : BaseItem
  {
    public CalendarRefItemProxy GetProxy()
    {
      CalendarRefItemProxy result = new CalendarRefItemProxy();
      result.RefType = this.RefType;
      result.RefID = this.RefID;
      result.CalendarID = this.CalendarID;
      result.displayName = "";
       
      return result;
    }	
  }
}
