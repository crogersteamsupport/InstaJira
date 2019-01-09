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
        JiraContext _context;
        readonly IGenericRepository<TicketLinkToJira> ticketLinkrepository;
        readonly IGenericRepository<ExceptionLogs> exceptionsRepository;
        Expression<Func<TicketTypes, bool>> IsActive = a => a.IsActive == true;
        Expression<Func<TicketLinkToJira, bool>> IsSynced = a => a.SyncWithJira == true;
        public JiraRepository()
        {
            _context = new JiraContext();
            ticketLinkrepository = new GenericRepository<TicketLinkToJira>();
            exceptionsRepository = new GenericRepository<ExceptionLogs>();
        }
        public Users GetUserMetaData(int? assignedUserToTicketId)
        {
            if (assignedUserToTicketId == null)
                return new Users();
            
            var emails = from alias in _context.Users
                        where alias.UserId == assignedUserToTicketId
                        select alias;
            return emails.FirstOrDefault();
        }

        /// <summary>
        /// Business logic: only looks for active tickets to query CRMLinkFields table for IssueType to send to Jira
        /// If there is no CrmFieldName available then the mappings for issue types has NOT been setup!
        /// </summary>
        /// <param name="ticketTypeID"></param>
        /// <param name="cRMLinkID"></param>
        /// <returns></returns>
        public string GetCRMFieldName(int ticketTypeID, int cRMLinkID)        
        {
            var ticketTypes = from alias in _context.TicketTypes.Where(IsActive)
                              where alias.TicketTypeID == ticketTypeID
                              select alias;
            var name = ticketTypes?.FirstOrDefault().Name;
            if (!string.IsNullOrEmpty(name))
            {
                var crmFieldName = from alias in _context.CRMLinkField
                            where alias.CRMLinkID == cRMLinkID
                            && alias.TSFieldName == name
                            select alias;
                if (crmFieldName?.FirstOrDefault() != null)
                {
                    return crmFieldName.FirstOrDefault().CRMFieldName;
                }
            }
            return string.Empty;
        }

        public Tickets GetTicketData(int ticketId)
        {
            var tickets = from alias in _context.Tickets
                          where alias.TicketID == ticketId
                          select alias;
            return tickets.FirstOrDefault();
        }

        public string GetDescriptionFromActionsForNewTicketCreation(int ticketId, int organizationId)
        {
            var query = from tickets in _context.Tickets
                        where tickets.TicketID == ticketId &&
                        tickets.OrganizationID == organizationId
                        join actions in _context.Actions
                        on tickets.TicketID equals actions.TicketID
                        select actions;
            return query.FirstOrDefault().Description;
        }

        public IQueryable<TicketLinkToJira> GetAllJiraTickets()
        {
            return ticketLinkrepository.GetAll();
        }

        public IQueryable<TicketLinkToJira> GetSingleJiraTicket(int id)
        {
            var query = from alias in _context.TicketLinkToJira
                        where alias.id == id
                        select alias;
            return query;
        }

        public async Task<TicketLinkToJira> GetSingleJiraTicketAsync(int id)
        {
            return await ticketLinkrepository.FindByAsync(a => a.id == id).ConfigureAwait(false);
        }

        public void SaveJiraTickets(TicketLinkToJira jiraTickets)
        {
            ticketLinkrepository.Add(jiraTickets);
        }

        public void SaveExceptions(ExceptionLogs exception)
        {
            exceptionsRepository.Add(exception);
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
            return from alias in _context.CrmLinkTables
                   where alias.CRMLinkID == id
                   select alias;
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
            var temp1 = exceptionsRepository as IDisposable;
            if (temp1 != null)
                temp1.Dispose();
            if (_context != null)
                _context.Dispose();//ToDo: update this as garbage collection for entire context won't give much advantage in average case
            GC.SuppressFinalize(this);
        }

    }
}
