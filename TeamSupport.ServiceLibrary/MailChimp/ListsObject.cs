using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TeamSupport.ServiceLibrary
{
    public class ListsObject
    {
        public class List
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        [DataContract]
        public class Lists
        {
            [DataMember(Name = "lists")]
            public List<List> lists { get; set; }
            [DataMember(Name = "total_items")]
            public int TotalItems { get; set; }
        }
    }
}
