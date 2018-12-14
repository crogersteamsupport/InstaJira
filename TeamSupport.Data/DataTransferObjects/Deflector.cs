using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Data
{
    [DataContract]
    public class DeflectorIndex
    {
        [DataMember]
        public int TicketID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int OrganizationID { get; set; }
        [DataMember]
        public int? ProductID { get; set; }
        [DataMember]
        public int TagID { get; set; }
        [DataMember]
        public string Value { get; set; }
    }

    [DataContract]
    public class DeflectorReturn
    {
        [DataMember]
        public int TicketID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ReturnURL { get; set; }
    }
}
