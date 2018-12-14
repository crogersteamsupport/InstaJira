using System;
using System.Collections.Generic;
using System.Linq;
using TeamSupport.EFData;
using TeamSupport.EFData.Models;

namespace TeamSupport.JIRA
{
    public class JiraService 
    {
        /// <summary>
        /// TicketsView is deprecated! This is to be replaced with a call to Tickets, only
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        public Tickets GetTicketData(int ticketId)
        {
            var ticketData = new Tickets();
            using (var jiraTicketsService = new JiraTicketsService(new JiraRepository(new GenericRepository<Tickets>())))
            {
                ticketData = jiraTicketsService.GetTicketData(ticketId);
            }
            return ticketData;
        }

        /// <summary>
        /// TicketsView is deprecated! This is to be replaced with a call to Tickets, only
        /// </summary>
        /// <param name="ticketId"></param>
        /// <returns></returns>
        public string GetTicketDescription(int ticketId, int organizationId)
        {
            var description = string.Empty;
            using (var jiraTicketsService = new JiraTicketsService(new JiraRepository(new GenericRepository<Actions>(), new GenericRepository<Tickets>())))
            {
                description = jiraTicketsService.GetDescriptionFromActionsForNewTicketCreation(ticketId, organizationId);
            }
            return description;
        }

        public CrmLinkTable GetCRMLinkTableData(int? crmLinkId)
        {
            var crmLinkTable = new CrmLinkTable();
            using (var jiraTicketsService = new JiraTicketsService(new JiraRepository(new GenericRepository<CrmLinkTable>())))
            {
                crmLinkTable = jiraTicketsService.GetCrmLinkTableById(crmLinkId).FirstOrDefault();
            }
            return crmLinkTable;
        }

        public string GetCRMFieldName(int ticketTypeID, int cRMLinkID)
        {
            var crmFieldName = string.Empty;
            using (var jiraTicketsService = new JiraTicketsService(new JiraRepository(new GenericRepository<CRMLinkField>(), new GenericRepository<TicketTypes>())))
            {
                crmFieldName = jiraTicketsService.GetCRMFieldName(ticketTypeID, cRMLinkID);
            }
            return crmFieldName;
        }


        //ToDo: Implement strategy pattern for various models to be built based off of a differing issueType
        //For example: issueType of Story requires more than what issueType Task requires
        public Issue CreateNewJiraTicket(string baseUrl, string username, string password, string projectKey, string issueType, IssueFields issueFields)
        {
            issueFields.timetracking = null;//necessary for API rest call
            var response = new Issue();
            var client = new JiraClient(baseUrl, username, password);
        
            if (issueFields != null)
            {
                response = client.CreateIssue(projectKey, issueType, issueFields);
                
                if (issueFields.assignee != null)
                {
                    var issue = new Issue();
                    var user = new JiraUser();
                    user.name = issueFields.assignee.name;
                    issue.fields = new IssueFields();
                    issue.self = response.self;
                    issue.key = response.key;
                    issue.id = response.id;
                    issue.expand = response.expand;
                    issue.fields.assignee = user;
                    client.UpdateIssue(issue);
                }
                if (response == null)
                {
                    return new Issue();
                }
            }

            return response;
        }

        public bool SaveTicketLinkToJira(TicketLinkToJira ticketLinkToJira)
        {
            var response = true;
            try
            {
                using (var jiraTicketsService = new JiraTicketsService(new JiraRepository(new GenericRepository<TicketLinkToJira>())))
                {
                    jiraTicketsService.SaveJiraTickets(ticketLinkToJira);
                }
            }catch(Exception ex)
            {
                response = false;
                //ToDo: add logging here and exception handling
            }
            return response;
        }

        public Users GetUserMetaData(int? assignedUserToTicketId)
        {
            if(assignedUserToTicketId == null)
                return new Users();
            var response = new Users();
            using (var jiraTicketsService = new JiraTicketsService(new JiraRepository(new GenericRepository<Users>())))
            {
                response = jiraTicketsService.GetUserMetaData(assignedUserToTicketId);
            }

            return response;
        }

        public List<JiraUser> GetUsers(string baseUrl, string username, string password)
        {
            var client = new JiraClient(baseUrl, username, password);
            return client.GetJiraUsers();
        }
    }
}
