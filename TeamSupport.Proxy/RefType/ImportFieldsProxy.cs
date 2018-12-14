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
  [KnownType(typeof(ImportFieldProxy))]
  public class ImportFieldProxy
  {
    public ImportFieldProxy() {}
    [DataMember] public int ImportFieldID { get; set; }
    [DataMember] public string TableName { get; set; }
    [DataMember] public string FieldName { get; set; }
    [DataMember] public string Alias { get; set; }
    [DataMember] public string DataType { get; set; }
    [DataMember] public int Size { get; set; }
    [DataMember] public bool IsVisible { get; set; }
    [DataMember] public bool IsRequired { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public int RefType { get; set; }
    [DataMember] public bool Enabled { get; set; }
    [DataMember] public int Position { get; set; }
          
  }
}
