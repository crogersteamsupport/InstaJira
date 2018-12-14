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
  [KnownType(typeof(EmailTemplateParameterProxy))]
  public class EmailTemplateParameterProxy
  {
    public EmailTemplateParameterProxy() {}
    [DataMember] public int EmailTemplateParameterID { get; set; }
    [DataMember] public int EmailTemplateID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Description { get; set; }
          
  }
  
  public partial class EmailTemplateParameter : BaseItem
  {
    public EmailTemplateParameterProxy GetProxy()
    {
      EmailTemplateParameterProxy result = new EmailTemplateParameterProxy();
      result.Description = this.Description;
      result.Name = this.Name;
      result.EmailTemplateID = this.EmailTemplateID;
      result.EmailTemplateParameterID = this.EmailTemplateParameterID;
       
       
       
      return result;
    }	
  }
}
