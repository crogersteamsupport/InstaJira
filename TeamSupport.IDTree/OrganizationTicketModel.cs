using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    /// <summary> Link contact organizations and tickets </summary>
    public class OrganizationTicketModel : IDNode
    {
        public TicketModel Ticket { get; private set; }
        public OrganizationModel Organization { get; private set; }

        public OrganizationTicketModel(TicketModel ticket, OrganizationModel organization) : this(ticket, organization, true)
        {
        }

        private OrganizationTicketModel(TicketModel ticket, OrganizationModel organization, bool verify) : base(ticket)
        {
            Ticket = ticket;
            Organization = organization;
            if (verify)
                Verify();
        }

        /// <summary> find all the contact organizations associated with a ticket </summary>
        public static OrganizationTicketModel[] GetOrganizationTickets(TicketModel ticket)
        {
            //$"Select OrganizationID From OrganizationTickets WITH (NOLOCK) WHERE TicketId = {ticket.TicketID}"
            int[] customerIDs = IDReader.Read(TicketChild.Customers, ticket);
            OrganizationTicketModel[] organizationTickets = new OrganizationTicketModel[customerIDs.Length];
            for (int i = 0; i < customerIDs.Length; ++i)
                organizationTickets[i] = new OrganizationTicketModel(ticket, new OrganizationModel(ticket.Connection, customerIDs[i]), false);
            return organizationTickets;
        }

        public override void Verify()
        {
            Verify($"SELECT OrganizationID FROM OrganizationTickets WITH (NOLOCK) WHERE TicketID={Ticket.TicketID} AND OrganizationID={Organization.OrganizationID}");
        }
    }
}
