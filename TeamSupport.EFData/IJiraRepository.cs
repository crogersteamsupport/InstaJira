using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamSupport.EFData.Models;

namespace TeamSupport.EFData
{
    public interface IJiraRepository
    {
        IQueryable<CrmLinkTable> GetCrmLinkTableById(int? id);
        IQueryable<TicketLinkToJira> GetAllJiraTickets();
        Task<IQueryable<TicketLinkToJira>> GetTicketsToPushAsIssuesAsync();
        Task<IList<TicketLinkToJira>> GetTicketsToPushAsIssuesAsyncByExpression();
        Task<TicketLinkToJira> GetSingleJiraTicketAsync(int id);
        IQueryable<TicketLinkToJira> GetSingleJiraTicket(int id);
        void SaveJiraTickets(TicketLinkToJira jiraTickets);
        void UpdateTicket(TicketLinkToJira jiraTicket);
        void UpdateTicketAsync(TicketLinkToJira jiraTicket);
        TicketLinkToJira SaveJiraTicketsAsync(TicketLinkToJira jiraTickets);
        void DeleteJiraTicket(TicketLinkToJira jiraTickets);
        void DeleteJiraTicketAsync(TicketLinkToJira jiraTicket);
        Tickets GetTicketData(int ticketId);
        string GetDescriptionFromActionsForNewTicketCreation(int ticketId, int organizationId);
        string GetCRMFieldName(int ticketTypeID, int cRMLinkID);
        Users GetUserMetaData(int? assignedUserToTicketId);
        void SaveExceptions(ExceptionLogs entity);
    }
}