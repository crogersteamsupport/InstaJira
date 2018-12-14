using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics;
using TeamSupport.Data;

namespace TeamSupport.IDTree
{
    /// <summary>
    /// Wrapper for valid TicketID
    /// </summary>
    public class TicketModel : IDNode, ITaskAssociation
    {
        public OrganizationModel Organization { get; private set; }
        public int TicketID { get; private set; }

        int? _ticketNumber;
        public int TicketNumber
        {
            get
            {
                if (!_ticketNumber.HasValue)
                    _ticketNumber = ExecuteQuery<int>($"SELECT TicketNumber FROM Tickets WITH(NOLOCK) WHERE TicketId = {TicketID}").First();
                return _ticketNumber.Value;
            }
        }

        /// <summary> top down - existing ticket </summary>
        public TicketModel(OrganizationModel organization, int ticketID) : base(organization)
        {
            Organization = organization;   // user session on customer
            TicketID = ticketID;
            Verify();
        }

        /// <summary> bottom up - existing action </summary>
        public TicketModel(ConnectionContext connection, int ticketID) : base(connection)
        {
            Organization = connection.Organization;
            TicketID = ticketID;
            Verify();
        }

        public TicketProxy TicketProxy()
        {
            return ExecuteQuery<TicketProxy>($"SELECT * FROM Tickets WHERE TicketID={TicketID}").First();
        }

        public ActionProxy[] ActionProxies()
        {
            return ExecuteQuery<ActionProxy>($"SELECT * FROM Actions WHERE TicketID={TicketID}").ToArray();
        }

        public override void Verify()
        {
            Verify($"SELECT TicketID FROM Tickets WITH (NOLOCK) WHERE TicketID={TicketID} AND OrganizationID={Organization.OrganizationID}");
        }

        /// <summary> Existing Data.Action </summary>
        public ActionModel Action(int actionID)
        {
            return new ActionModel(this, actionID);
        }

        public ActionModel[] Actions() { return ActionModel.GetActions(this); }
        public AssetTicketModel[] AssetTickets() { return AssetTicketModel.GetAssetTickets(this); }
        public UserTicketModel[] UserTickets() { return UserTicketModel.GetUserTickets(this); }
        public OrganizationTicketModel[] OrganizationTickets() { return OrganizationTicketModel.GetOrganizationTickets(this); }
        public TicketReminderModel[] Reminders() { return TicketReminderModel.GetTicketReminders(this); }
        public SubscriptionModel[] Subscriptions() { return SubscriptionModel.GetSubscriptions(this); }
        //public TaskAssociationModel[] TaskAssociations() { return TaskAssociationModel.GetTaskAssociations(this); }
        //public TicketQueueModel[] TicketQueue() { return TicketQueueModel.GetQueuedTicket(this); }


        public TicketModel[] ChildTickets()
        {
            int[] ticketIDs = IDReader.Read(TicketChild.Children, this);
            TicketModel[] childTickets = new TicketModel[ticketIDs.Length];
            for (int i = 0; i < ticketIDs.Length; ++i)
                childTickets[i] = new TicketModel(Organization, ticketIDs[i]);
            return childTickets;
        }

    }
}
