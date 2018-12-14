using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.IDTree
{
    public class TicketReminderModel : IDNode
    {
        public TicketModel Ticket { get; private set; }
        public UserModel User { get; private set; }
        public int ReminderID { get; private set; }

        /// <summary> top down - existing action </summary>
        public TicketReminderModel(TicketModel ticket, int reminderID) : this(ticket, reminderID, true)
        {
        }

        private TicketReminderModel(TicketModel ticket, int reminderID, bool verify) : base(ticket)
        {
            Ticket = ticket;
            ReminderID = reminderID;
            if (verify)
                Verify();
        }

        public static TicketReminderModel[] GetTicketReminders(TicketModel ticket)
        {
            int[] reminderIDs = IDReader.Read(TicketChild.TicketReminders, ticket);
            TicketReminderModel[] reminderModels = new TicketReminderModel[reminderIDs.Length];
            for (int i = 0; i < reminderIDs.Length; ++i)
                reminderModels[i] = new TicketReminderModel(ticket, reminderIDs[i], false);
            return reminderModels;
        }
        public override void Verify()
        {
            int organizationID = Ticket.Organization.OrganizationID;
            Verify($"SELECT ReminderID FROM Reminders WITH (NOLOCK) WHERE ReminderID={ReminderID} AND OrganizationID={organizationID} AND RefID={Ticket.TicketID} AND RefType=17");
        }
    }
}
