using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    /// <summary> Link contacts and tickets </summary>
    public class UserTicketModel : IDNode
    {
        public TicketModel Ticket { get; private set; }
        public UserModel User { get; private set; }

        public UserTicketModel(TicketModel ticket, UserModel contact) : this(ticket, contact, true)
        {
        }

        private UserTicketModel(TicketModel ticket, UserModel contact, bool verify) : base(ticket)
        {
            Ticket = ticket;
            User = contact;
            if (verify)
                Verify();
        }

        public static UserTicketModel[] GetUserTickets(TicketModel ticket)
        {
            //query = $"SELECT Users.userid FROM Users WITH (NOLOCK)" +
            //    $"JOIN UserTickets WITH (NOLOCK) on UserTickets.userid = Users.UserID" +
            //    $" WHERE UserTickets.TicketID = {ticket.TicketID} AND (Users.MarkDeleted = 0)";

            int[] contactIDs = IDReader.Read(TicketChild.Contacts, ticket);
            UserTicketModel[] contacts = new UserTicketModel[contactIDs.Length];
            for (int i = 0; i < contactIDs.Length; ++i)
                contacts[i] = new UserTicketModel(ticket, new UserModel(ticket.Connection, contactIDs[i]), false);
            return contacts;
        }

        public override void Verify()
        {
            
        }
    }
}
