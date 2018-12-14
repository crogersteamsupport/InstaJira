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
  [KnownType(typeof(ImportMapProxy))]
  public class ImportMapProxy
  {
    public ImportMapProxy() {}
    [DataMember] public int ImportMapID { get; set; }
    [DataMember] public int ImportID { get; set; }
    [DataMember] public string SourceName { get; set; }
    [DataMember] public int FieldID { get; set; }
    [DataMember] public bool IsCustom { get; set; }
          
  }
}
