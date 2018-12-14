using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class CustomerHubFeatureSettings
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CustomerHubFeatureSettingID", "CustomerHubFeatureSettingID", false, false, false);
      _fieldMap.AddMap("CustomerHubID", "CustomerHubID", false, false, false);
      _fieldMap.AddMap("ShowKnowledgeBase", "ShowKnowledgeBase", false, false, false);
      _fieldMap.AddMap("ShowProducts", "ShowProducts", false, false, false);
            
    }
  }
  
}
