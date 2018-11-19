using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace TeamSupport.Data
{
  [DataContract(Namespace = "http://teamsupport.com/")]
  [KnownType(typeof(KnowledgeBaseCategoryTicketsProxy))]
  public class KnowledgeBaseCategoryTicketsProxy
  {
    [DataMember]
    public int? CategoryID { get; set; }
    [DataMember]
    public string CategoryName { get; set; }
    [DataMember]
    public string ParentCategoryName { get; set; }
    [DataMember]
    public int Count { get; set; }
    [DataMember]
    public KnowledgeBaseCategoryTicketsItemProxy[] Items { get; set; }

  }

  [DataContract]
  public class KnowledgeBaseCategoryTicketsItemProxy
  {
    public KnowledgeBaseCategoryTicketsItemProxy()
    {
    }
    [DataMember]
    public int ID { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string DateCreated { get; set; }
    [DataMember]
    public string DateModified { get; set; }

  }

}
