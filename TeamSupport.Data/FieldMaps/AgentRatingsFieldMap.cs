using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class AgentRatings
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("AgentRatingID", "AgentRatingID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("CompanyID", "CompanyID", false, false, false);
      _fieldMap.AddMap("ContactID", "ContactID", false, false, false);
      _fieldMap.AddMap("Rating", "Rating", false, false, false);
      _fieldMap.AddMap("Comment", "Comment", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("TicketID", "TicketID", false, false, false);
            
    }
  }
  
}
