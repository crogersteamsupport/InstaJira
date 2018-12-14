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
  [KnownType(typeof(TranslateItemProxy))]
  public class TranslateItemProxy
  {
    public TranslateItemProxy() {}
    [DataMember] public int PhraseID { get; set; }
    [DataMember] public string English { get; set; }
    [DataMember] public string French { get; set; }
    [DataMember] public string Italian { get; set; }
    [DataMember] public string German { get; set; }
    [DataMember] public string Spanish { get; set; }
    [DataMember] public string Portugese { get; set; }
          
  }
  
  public partial class TranslateItem : BaseItem
  {
    public TranslateItemProxy GetProxy()
    {
      TranslateItemProxy result = new TranslateItemProxy();
      result.Portugese = this.Portugese;
      result.Spanish = this.Spanish;
      result.German = this.German;
      result.Italian = this.Italian;
      result.French = this.French;
      result.English = this.English;
      result.PhraseID = this.PhraseID;
       
       
       
      return result;
    }	
  }
}
