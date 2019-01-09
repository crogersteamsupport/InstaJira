using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamSupport.EFData;
using TeamSupport.EFData.Models;

namespace TeamSupport.JIRA
{
    public class JiraTicketsService : IJiraTicketsService, IDisposable
    {
        private IJiraRepository jiraRepo;
        public JiraTicketsService(IJiraRepository jiraRepo) => this.jiraRepo = jiraRepo;
        public List<TicketLinkToJira> GetTicketsToPushAsIssues() => jiraRepo.GetAllJiraTickets().ToList();
        public async Task<IQueryable<TicketLinkToJira>> GetTicketsToPushAsIssuesAsync() => await jiraRepo.GetTicketsToPushAsIssuesAsync();
        public async Task<IList<TicketLinkToJira>> GetTicketsToPushAsIssuesAsyncByExpression() => await jiraRepo.GetTicketsToPushAsIssuesAsyncByExpression();
        public void SaveJiraTickets(TicketLinkToJira jiraTickets) => jiraRepo.SaveJiraTickets(jiraTickets);               
        public void SaveJiraTicketsAsync(TicketLinkToJira jiraTickets) => jiraRepo.SaveJiraTicketsAsync(jiraTickets);
        public void DeleteJiraTicket(TicketLinkToJira jiraTickets) => jiraRepo.DeleteJiraTicket(jiraTickets);
        public void DeleteJiraTicketAsync(TicketLinkToJira jiraTicket) => jiraRepo.DeleteJiraTicketAsync(jiraTicket);
        public void UpdateTicket(TicketLinkToJira jiraTicket) => jiraRepo.UpdateTicket(jiraTicket);
        public void UpdateTicketAsync(TicketLinkToJira jiraTicket) => jiraRepo.UpdateTicketAsync(jiraTicket);
        public async Task<TicketLinkToJira> GetSingleJiraTicketAsync(int id) => await jiraRepo.GetSingleJiraTicketAsync(id);
        public IQueryable<CrmLinkTable> GetCrmLinkTableById(int? id) => jiraRepo.GetCrmLinkTableById(id);
        public Tickets GetTicketData(int ticketId) =>  jiraRepo.GetTicketData(ticketId);
        public string GetDescriptionFromActionsForNewTicketCreation(int ticketId, int organizationId) => jiraRepo.GetDescriptionFromActionsForNewTicketCreation(ticketId, organizationId);
        public string GetCRMFieldName(int ticketTypeID, int cRMLinkID) => jiraRepo.GetCRMFieldName(ticketTypeID, cRMLinkID);
        public Users GetUserMetaData(int? assignedUserToTicketId) => jiraRepo.GetUserMetaData(assignedUserToTicketId);
        public TicketLinkToJira GetTicketsToPushAsIssuesById(int id) => jiraRepo.GetSingleJiraTicket(id).FirstOrDefault();
        public void Dispose() => GC.SuppressFinalize(this);


    }
}
