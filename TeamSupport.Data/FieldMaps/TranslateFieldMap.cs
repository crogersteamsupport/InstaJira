using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class Translate
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("PhraseID", "PhraseID", false, false, false);
      _fieldMap.AddMap("English", "English", false, false, false);
      _fieldMap.AddMap("French", "French", false, false, false);
      _fieldMap.AddMap("Italian", "Italian", false, false, false);
      _fieldMap.AddMap("German", "German", false, false, false);
      _fieldMap.AddMap("Spanish", "Spanish", false, false, false);
      _fieldMap.AddMap("Portugese", "Portugese", false, false, false);
            
    }
  }
  
}
