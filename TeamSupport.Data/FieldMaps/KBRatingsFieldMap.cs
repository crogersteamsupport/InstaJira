using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class KBRatings
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("KBRatingID", "KBRatingID", false, false, false);
      _fieldMap.AddMap("TicketID", "TicketID", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("IP", "IP", false, false, false);
      _fieldMap.AddMap("Rating", "Rating", false, false, false);
      _fieldMap.AddMap("DateUpdated", "DateUpdated", false, false, false);
      _fieldMap.AddMap("Comment", "Comment", false, false, false);
            
    }
  }
  
}
