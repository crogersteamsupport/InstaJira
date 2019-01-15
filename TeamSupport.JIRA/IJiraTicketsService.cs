using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSupport.EFData.Models;

namespace TeamSupport.JIRA
{
    public interface IJiraTicketsService
    {
        void DeleteJiraTicket(TicketLinkToJira jiraTickets);
        void DeleteJiraTicketAsync(TicketLinkToJira jiraTickets);
        List<TicketLinkToJira> GetTicketsToPushAsIssues();
        TicketLinkToJira GetTicketsToPushAsIssuesById(int id);
        void SaveJiraTickets(TicketLinkToJira jiraTickets);
        void SaveJiraTicketsAsync(TicketLinkToJira jiraTickets);
        void UpdateTicket(TicketLinkToJira jiraTicket);
        void UpdateTicketAsync(TicketLinkToJira jiraTicket);
        Task<IQueryable<TicketLinkToJira>> GetTicketsToPushAsIssuesAsync();
        Task<IList<TicketLinkToJira>> GetTicketsToPushAsIssuesAsyncByExpression();
        Task<TicketLinkToJira> GetSingleJiraTicketAsync(int id);
        IQueryable<CrmLinkTable> GetCrmLinkTableById(int? id);
        Tickets GetTicketData(int ticketId);
        string GetDescriptionFromActionsForNewTicketCreation(int ticketId, int organizationId);
        string GetCRMFieldName(int ticketTypeID, int cRMLinkID);
        Users GetUserMetaData(int? assignedUserToTicketId);
        IEnumerable<string> GetTicketCommentsToPush(int ticketId, int organizationId);
    }
}
