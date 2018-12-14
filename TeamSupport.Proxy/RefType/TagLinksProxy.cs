using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Data.Linq.Mapping;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(TagLinkProxy))]
  [Table(Name = "TagLinks")]
  public class TagLinkProxy
  {
    public TagLinkProxy() {}
    public TagLinkProxy(References refType) { RefType = (ReferenceType)refType; }
    [DataMember, Column] public int TagLinkID { get; set; }
    [DataMember, Column] public int TagID { get; set; }
    [DataMember, Column] public ReferenceType RefType { get; set; }
    [DataMember, Column] public int RefID { get; set; }
    [DataMember, Column] public DateTime DateCreated { get; set; }
    [DataMember, Column] public int CreatorID { get; set; }
          

        public static TagLinkProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Tickets: return new TicketTagLinkProxy();
                default:
                    if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
                    throw new Exception($"Invalid AssetAssignmentsViewItem Reference Type  {refType}");
  }
  
        }

        public enum References
        {
            Tickets
        }
    }

    public class TicketTagLinkProxy : TagLinkProxy
    {
        public TicketTagLinkProxy() : base(References.Tickets) { }
    }
}
