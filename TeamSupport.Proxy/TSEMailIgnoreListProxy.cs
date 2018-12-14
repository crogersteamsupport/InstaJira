using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
    [DataContract(Namespace = "http://teamsupport.com/")]
    [KnownType(typeof(TSEMailIgnoreListItemProxy))]
    public class TSEMailIgnoreListItemProxy
    {
        public TSEMailIgnoreListItemProxy() { }
        [DataMember] public int IgnoreID { get; set; }
        [DataMember] public string FromAddress { get; set; }
        [DataMember] public string ToAddress { get; set; }

    }
}
