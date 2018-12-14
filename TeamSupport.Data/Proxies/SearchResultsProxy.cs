using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace TeamSupport.Data
{
    [DataContract(Namespace = "http://teamsupport.com/")]
    [KnownType(typeof(ActionLogProxy))]
    public class SearchResultsProxy
    {
        [DataMember]
        public int Count { get; set; }
        [DataMember]
        public SearchResultsItemProxy[] Items { get; set; }

    }


}
