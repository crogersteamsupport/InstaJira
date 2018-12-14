using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{

    public partial class SearchStandardFilters
    {
        protected override void BuildFieldMap()
        {
            _fieldMap = new FieldMap();
            _fieldMap.AddMap("StandardFilterID", "StandardFilterID", false, false, false);
            _fieldMap.AddMap("UserID", "UserID", false, false, false);
            _fieldMap.AddMap("Tickets", "Tickets", false, false, false);
            _fieldMap.AddMap("KnowledgeBase", "KnowledgeBase", false, false, false);
            _fieldMap.AddMap("Wikis", "Wikis", false, false, false);
            _fieldMap.AddMap("Notes", "Notes", false, false, false);
            _fieldMap.AddMap("ProductVersions", "ProductVersions", false, false, false);
            _fieldMap.AddMap("WaterCooler", "WaterCooler", false, false, false);
            _fieldMap.AddMap("Tasks", "Tasks", false, false, false);

        }
    }

}
