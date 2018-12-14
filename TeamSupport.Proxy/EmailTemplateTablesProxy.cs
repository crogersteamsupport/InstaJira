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
  [KnownType(typeof(EmailTemplateTableProxy))]
  public class EmailTemplateTableProxy
  {
    public EmailTemplateTableProxy() {}
    [DataMember] public int EmailTemplateTableID { get; set; }
    [DataMember] public int EmailTemplateID { get; set; }
    [DataMember] public int ReportTableID { get; set; }
    [DataMember] public string Alias { get; set; }
    [DataMember] public string Description { get; set; }
          
  }
}
