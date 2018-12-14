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
  
  public partial class ImportMap : BaseItem
  {
    public ImportMapProxy GetProxy()
    {
      ImportMapProxy result = new ImportMapProxy();
      result.IsCustom = this.IsCustom;
      result.FieldID = this.FieldID;
      result.SourceName = this.SourceName;
      result.ImportID = this.ImportID;
      result.ImportMapID = this.ImportMapID;
       
       
       
      return result;
    }	
  }
}
