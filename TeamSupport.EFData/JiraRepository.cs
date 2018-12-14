using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TeamSupport.EFData.Models;

namespace TeamSupport.EFData
{
    public class JiraRepository : IJiraRepository, IDisposable
    {
        readonly IGenericRepository<TicketLinkToJira> ticketLinkrepository;
        readonly IGenericRepository<CrmLinkTable> crmRepository;
        readonly IGenericRepository<TicketsView> ticketsViewRepository;
        readonly IGenericRepository<Tickets> ticketsRepository;
        readonly IGenericRepository<Actions> actionsRepository;
        Expression<Func<TicketLinkToJira, bool>> IsSynced = a => a.SyncWithJira == true;
        public JiraRepository(IGenericRepository<TicketLinkToJira> repository)
        {
            this.ticketLinkrepository = repository;
        }

        public JiraRepository(IGenericRepository<CrmLinkTable> crmRepository)
        {
            this.crmRepository = crmRepository;
        }

        public JiraRepository(IGenericRepository<TicketsView> ticketsViewRepository, IGenericRepository<Tickets> ticketsRepository)
        {
            this.ticketsViewRepository = ticketsViewRepository;
            this.ticketsRepository = ticketsRepository;
        }

        public JiraRepository(IGenericRepository<Actions> actionsRepository, IGenericRepository<Tickets> ticketsRepository)
        {
            this.actionsRepository = actionsRepository;
            this.ticketsRepository = ticketsRepository;
        }

        public TicketsView GetTicketsFromView(int ticketId)
        {
            var tickets = ticketsRepository.FindBy(a=>a.TicketID == ticketId).FirstOrDefault();
            var ticketsView = ticketsViewRepository.FindBy(a => a.OrganizationID == tickets.OrganizationID && a.TicketNumber == tickets.TicketNumber).FirstOrDefault();

            return ticketsView;            
        }

        public string GetDescriptionFromActionsForNewTicketCreation(int ticketId, int organizationId)
        {
            var ticket = ticketsRepository.FindBy(a => a.TicketID == ticketId && a.OrganizationID == organizationId).FirstOrDefault();
            var actions = new Actions();
            if (ticket != null)
            {
                actions = actionsRepository.FindBy(a => a.TicketID == ticket.TicketID).FirstOrDefault();
            }
            return actions.Description;
        }

        public IQueryable<TicketLinkToJira> GetAllJiraTickets()
        {
            return ticketLinkrepository.GetAll();
        }

        public IQueryable<TicketLinkToJira> GetSingleJiraTicket(int id)
        {
            return ticketLinkrepository.FindBy(a => a.id == id);
        }

        public async Task<TicketLinkToJira> GetSingleJiraTicketAsync(int id)
        {
            return await ticketLinkrepository.FindByAsync(a => a.id == id).ConfigureAwait(false);
        }

        public void SaveJiraTickets(TicketLinkToJira jiraTickets)
        {
            ticketLinkrepository.Add(jiraTickets);
        }

        /// <summary>
        /// Asynchronous save is done at the GenericRepository level
        /// </summary>
        /// <param name="jiraTickets"></param>
        /// <returns>Single Jira Link Ticket</returns>
        public TicketLinkToJira SaveJiraTicketsAsync(TicketLinkToJira jiraTickets)
        {
            return  ticketLinkrepository.AddAsync(jiraTickets).Result;
        }

        public void DeleteJiraTicket(TicketLinkToJira jiraTickets)
        {
            ticketLinkrepository.Delete(jiraTickets);
        }

        public IQueryable<CrmLinkTable> GetCrmLinkTableById(int? id)
        {
           return crmRepository.FindBy(a => a.CRMLinkID == id);
        }

        public async Task<IQueryable<TicketLinkToJira>> GetTicketsToPushAsIssuesAsync()
        {
            return await ticketLinkrepository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task<IList<TicketLinkToJira>> GetTicketsToPushAsIssuesAsyncByExpression()
        {
            return await ticketLinkrepository.GetAllAsync(IsSynced).ConfigureAwait(false);
        }

        public void DeleteJiraTicketAsync(TicketLinkToJira jiraTicket)
        {
            ticketLinkrepository.DeleteAsync(jiraTicket);
        }

        public void UpdateTicket(TicketLinkToJira jiraTicket)
        {
            ticketLinkrepository.Edit(jiraTicket);
        }

        public void UpdateTicketAsync(TicketLinkToJira jiraTicket)
        {
            ticketLinkrepository.EditAsync(jiraTicket);
        }

        public void Dispose()
        {
            var temp = ticketLinkrepository as IDisposable;
            if (temp != null)
                temp.Dispose();
            var temp1 = crmRepository as IDisposable;
            if (temp1 != null)
                temp1.Dispose();
            var temp2 = ticketsViewRepository as IDisposable;
            if (temp2 != null)
                temp2.Dispose();
            var temp3 = ticketsRepository as IDisposable;
            if (temp3 != null)
                temp3.Dispose();
            var temp4 = actionsRepository as IDisposable;
            if (temp4 != null)
                temp4.Dispose();

            GC.SuppressFinalize(this);
        }


    }
}
