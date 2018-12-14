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
  [KnownType(typeof(FilePathProxy))]
  public class FilePathProxy
  {
    public FilePathProxy() {}
    [DataMember] public int ID { get; set; }
    [DataMember] public string Value { get; set; }
          
  }
  
  public partial class FilePath : BaseItem
  {
    public FilePathProxy GetProxy()
    {
      FilePathProxy result = new FilePathProxy();
      result.Value = this.Value;
      result.ID = this.ID;
       
       
       
      return result;
    }	
  }
}
