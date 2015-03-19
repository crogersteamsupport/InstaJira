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
          
  }
  
  public partial class ImportField : BaseItem
  {
    public ImportFieldProxy GetProxy()
    {
      ImportFieldProxy result = new ImportFieldProxy();
      result.Description = this.Description;
      result.IsRequired = this.IsRequired;
      result.IsVisible = this.IsVisible;
      result.Size = this.Size;
      result.DataType = this.DataType;
      result.Alias = this.Alias;
      result.FieldName = this.FieldName;
      result.TableName = this.TableName;
      result.ImportFieldID = this.ImportFieldID;
       
       
       
      return result;
    }	
  }
}