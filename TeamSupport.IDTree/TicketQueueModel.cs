using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;

namespace TeamSupport.IDTree
{
    public class TicketQueueModel : IDNode
    {
        public TicketModel Ticket { get; private set; }
        public UserModel User { get; private set; }
        public int TicketQueueID { get; private set; }

        public TicketQueueModel(UserModel user, TicketModel ticket, int ticketQueueID) : this(user, ticket, ticketQueueID, true)
        {

        }

        private TicketQueueModel(UserModel user, TicketModel ticket, int ticketQueueID, bool verify) : base(ticket)
        {
            Ticket = ticket;
            User = user;
            TicketQueueID = ticketQueueID;
            if (verify)
                Verify();
        }

        public override void Verify()
        {
            Verify($"SELECT TicketQueueID FROM TicketQueue WITH (NOLOCK) WHERE TicketQueueID={TicketQueueID} AND TicketID={Ticket.TicketID} AND UserID={User.UserID}");
        }

        //public static TicketQueueModel[] GetQueuedTicket(TicketModel ticket)
        //{
        //    Table<Proxy.TicketQueueProxy> table = ticket.Connection._db.GetTable<Proxy.TicketQueueProxy>();
        //    var query = from row in table where row.TicketID == ticket.TicketID select row;
        //    Proxy.TicketQueueProxy[] proxies = query.ToArray();

        //    List<TicketQueueModel> queue = new List<TicketQueueModel>();
        //    foreach(Proxy.TicketQueueProxy proxy in proxies)
        //    {
        //        UserModel user = new UserModel(ticket.Connection, proxy.UserID);
        //        if (user.MarkDeleted())
        //            continue;
        //        queue.Add(new TicketQueueModel(user, ticket, proxy.TicketQueueID, false));
        //    }
        //    return queue.ToArray();
        //}
    }
}
