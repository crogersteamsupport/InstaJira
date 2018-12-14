using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class SubscriptionModel : IDNode
    {
        public TicketModel Ticket { get; private set; }
        public UserModel User { get; private set; }


        /// <summary> top down - existing action </summary>
        public SubscriptionModel(TicketModel ticket, UserModel user) : this(ticket, user, true)
        {
        }

        private SubscriptionModel(TicketModel ticket, UserModel user, bool verify) : base(ticket)
        {
            Ticket = ticket;
            User = user;
            if (verify)
                Verify();
        }

        public static SubscriptionModel[] GetSubscriptions(TicketModel ticket)
        {
            //query = $"SELECT Subscriptions.userid FROM Subscriptions WITH (NOLOCK) " +
            //        $"JOIN Users WITH (NOLOCK) on users.userid = Subscriptions.userid " +
            //        $"WHERE Reftype = 17 and Refid = {ticket.TicketID} and MarkDeleted = 0";
            int[] subscriptionUserIDs = IDReader.Read(TicketChild.Subscriptions, ticket);
            SubscriptionModel[] subscriptions = new SubscriptionModel[subscriptionUserIDs.Length];
            for (int i = 0; i < subscriptionUserIDs.Length; ++i)
                subscriptions[i] = new SubscriptionModel(ticket, new UserModel(ticket.Connection, subscriptionUserIDs[i]), false);
            return subscriptions;
        }

        public override void Verify()
        {
            Verify($"SELECT Subscriptions.userid FROM Subscriptions WITH (NOLOCK) " +
                    $"JOIN Users WITH (NOLOCK) ON users.userid = Subscriptions.userid " +
                    $"WHERE Subscriptions.userid = userID AND Reftype = 17 AND Refid = {Ticket.TicketID} AND MarkDeleted = 0");
        }

    }
}
