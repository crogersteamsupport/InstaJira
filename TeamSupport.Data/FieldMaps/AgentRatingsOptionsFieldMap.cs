using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class AgentRatingsOptions
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("PositiveRatingText", "PositiveRatingText", false, false, false);
      _fieldMap.AddMap("NeutralRatingText", "NeutralRatingText", false, false, false);
      _fieldMap.AddMap("NegativeRatingText", "NegativeRatingText", false, false, false);
      _fieldMap.AddMap("PositiveImage", "PositiveImage", false, false, false);
      _fieldMap.AddMap("NeutralImage", "NeutralImage", false, false, false);
      _fieldMap.AddMap("NegativeImage", "NegativeImage", false, false, false);
      _fieldMap.AddMap("RedirectURL", "RedirectURL", false, false, false);
      _fieldMap.AddMap("ExternalPageLink", "ExternalPageLink", false, false, false);
            
    }
  }
  
}
