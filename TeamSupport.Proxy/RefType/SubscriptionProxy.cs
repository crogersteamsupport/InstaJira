using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Data
{
    public class SubscriptionProxy
    {
        public Data.ReferenceType RefType;
        public int RefID;
        public int UserID;
        public SubscriptionProxy() { }
        public SubscriptionProxy(References refType) { RefType = (ReferenceType)refType; }

        public enum References
        {
            Organizations = ReferenceType.Organizations,
            Tickets = ReferenceType.Tickets
        }

        public static SubscriptionProxy ClassFactory(References refType)
        {
            switch (refType)
            {
                case References.Organizations: return new OrganizationSubscriptionProxy();
                case References.Tickets: return new TicketSubscriptionProxy();
                default:
                    if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
                    throw new Exception($"Invalid AssetAssignmentsViewItem Reference Type  {refType}");
            }
        }

        public class OrganizationSubscriptionProxy : SubscriptionProxy
        {
            public OrganizationSubscriptionProxy() : base(References.Organizations) { }
        }

        public class TicketSubscriptionProxy : SubscriptionProxy
        {
            public TicketSubscriptionProxy() : base(References.Tickets) { }
        }

    }
}
