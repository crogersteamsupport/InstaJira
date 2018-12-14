using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class TicketRatings
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("TicketID", "TicketID", false, false, false);
      _fieldMap.AddMap("TicketType", "TicketType", false, false, false);
      _fieldMap.AddMap("Votes", "Votes", false, false, false);
      _fieldMap.AddMap("Rating", "Rating", false, false, false);
      _fieldMap.AddMap("Views", "Views", false, false, false);
      _fieldMap.AddMap("ThumbsUp", "ThumbsUp", false, false, false);
      _fieldMap.AddMap("ThumbsDown", "ThumbsDown", false, false, false);
      _fieldMap.AddMap("LastViewed", "LastViewed", false, false, false);
            
    }
  }
  
}
