using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Diagnostics;
using System.Data.SqlClient;

namespace TeamSupport.IDTree
{
    public enum OrganizationChild
    {
        OrganizationTicket,
        Tag,
        Task,
        Ticket,
        User
    }

    public enum TicketChild
    {
        Action,
        AssetTicket,
        Asset,
        Children,
        Contacts,
        Customers,
        TicketReminders,
        Subscriptions,
        TicketTagLinks,
        TaskAssociations,
    }

    public enum TicketAssociation
    {
        QueueUsers,
        Relationships1,
        Relationships2,
    }


    /// <summary>
    /// ultralight read-only interface to database for verifying primary keys
    /// </summary>
    public static class IDReader
    {
        public static int[] Read(TicketChild childID, TicketModel ticket)
        {
            string query = String.Empty;
            switch (childID)
            {
                case TicketChild.Contacts:
                    query = $"SELECT Users.userid FROM Users WITH (NOLOCK)" +
                        $"JOIN UserTickets WITH (NOLOCK) on UserTickets.userid = Users.UserID" +
                        $" WHERE UserTickets.TicketID = {ticket.TicketID} AND (Users.MarkDeleted = 0)";
                    break;
                case TicketChild.Customers:
                    query = $"Select Organizationid From OrganizationTickets WITH (NOLOCK) WHERE TicketId = {ticket.TicketID}";
                    break;
                case TicketChild.Subscriptions:
                    query = $"SELECT Subscriptions.userid FROM Subscriptions WITH (NOLOCK) " +
                        $"JOIN Users WITH (NOLOCK) on users.userid = Subscriptions.userid " +
                        $"WHERE Reftype = 17 and Refid = {ticket.TicketID} and MarkDeleted = 0";
                    break;
                case TicketChild.TicketReminders:
                    query = $"SELECT ReminderID FROM Reminders WITH (NOLOCK) WHERE RefID = {ticket.TicketID} AND Reftype = 17";
                    break;
                case TicketChild.TaskAssociations:
                    query = $"SELECT TaskID FROM TaskAssociations WITH (NOLOCK) WHERE Refid={ticket.TicketID} and RefType = 17";
                    break;
                case TicketChild.Asset:
                    query = $"SELECT AssetID From AssetTickets WITH (NOLOCK) WHERE TicketID = {ticket.TicketID}";
                    break;
                case TicketChild.Children:
                    query = $"SELECT TicketID FROM Tickets WITH(NOLOCK) WHERE ParentID={ticket.TicketID}";
                    break;
                case TicketChild.TicketTagLinks:
                    query = $"SELECT TagLinkID FROM TagLinks WITH(NOLOCK) WHERE Reftype=17 and RefID = {ticket.TicketID}";
                    break;
            }
            return ticket.ExecuteQuery<int>(query).ToArray();
        }

        public static int[] Read(TicketAssociation childID, TicketModel destinationTicket, TicketModel sourceTicket)
        {
            if (destinationTicket.Connection != sourceTicket.Connection)
                throw new Exception("tickets must come from the same connection");

            string query = String.Empty;
            switch (childID)
            {
                case TicketAssociation.QueueUsers:
                    query = $"SELECT TicketQueue.UserID " +
                        $"FROM TicketQueue WITH (NOLOCK) " +
                        $"JOIN Users WITH (NOLOCK) on Users.userid = TicketQueue.userid " +
                        $"LEFT JOIN TicketQueue TicketQueue2 WITH(NOLOCK) on TicketQueue2.userid = TicketQueue.userid and TicketQueue2.ticketid = {destinationTicket.TicketID} " +
                        $"WHERE TicketQueue.ticketid ={sourceTicket.TicketID}  and TicketQueue2.TicketQueueID IS NULL and MarkDeleted =0";
                    break;
                case TicketAssociation.Relationships1:
                    query = $"SELECT TicketRelationshipID FROM TicketRelationships WITH(NOLOCK) WHERE Ticket1ID={sourceTicket.TicketID} AND Ticket2ID <> {destinationTicket.TicketID}";
                    break;
                case TicketAssociation.Relationships2:
                    query = $"SELECT TicketRelationshipID FROM TicketRelationships WITH(NOLOCK) WHERE Ticket2ID={sourceTicket.TicketID} and Ticket1ID={destinationTicket.TicketID}";
                    break;
            }

            return sourceTicket.ExecuteQuery<int>(query).ToArray();
        }

    }


}
