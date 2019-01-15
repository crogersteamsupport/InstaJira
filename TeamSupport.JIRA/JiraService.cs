using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using TeamSupport.EFData;
using TeamSupport.EFData.Models;

namespace TeamSupport.JIRA
{
    public class JiraService : IDisposable
    {
        private IJiraRepository _jiraRepo;
        private IJiraTicketsService _jiraTicketsService;
        // private IJiraLoggerService _jiraLoggerService;
      //  private readonly static log4net.ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IJiraExceptionService _jiraExceptionService;
        public JiraService()
        {
            _jiraRepo = new JiraRepository();
            _jiraTicketsService = new JiraTicketsService(_jiraRepo);
           //_logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            // _jiraLoggerService = new JiraLoggerService(log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));
            _jiraExceptionService = new JiraExceptionService(_jiraRepo);
        }
        

        public Tickets GetTicketData(int ticketId)
        {
            return _jiraTicketsService.GetTicketData(ticketId); 
        }

        public string GetTicketDescription(int ticketId, int organizationId)
        {
            return _jiraTicketsService.GetDescriptionFromActionsForNewTicketCreation(ticketId, organizationId);
        }

        public IEnumerable<string> GetTicketComments(int ticketId, int organizationId)
        {
            return _jiraTicketsService.GetTicketCommentsToPush(ticketId, organizationId);
        }

        public CrmLinkTable GetCRMLinkTableData(int? crmLinkId)
        {
            return _jiraTicketsService.GetCrmLinkTableById(crmLinkId).FirstOrDefault();
        }

        public string GetCRMFieldName(int ticketTypeID, int cRMLinkID)
        {
            return _jiraTicketsService.GetCRMFieldName(ticketTypeID, cRMLinkID);
        }


        //ToDo: Implement strategy pattern for various models to be built based off of a differing issueType
        //For example: issueType of Story requires more than what issueType Task requires
        public Issue CreateNewJiraTicket(string baseUrl, string username, string password, string projectKey, string issueType, IssueFields issueFields, IEnumerable<string> comments)
        {
            issueFields.timetracking = null;//necessary for API rest call
            var response = new Issue();
            var client = new JiraClient(baseUrl, username, password);
        
            if (issueFields != null)
            {
                try
                {
                    response = client.CreateIssue(projectKey, issueType, issueFields);
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                if (comments.Any())
                {
                    foreach (var comment in comments.Reverse())
                    {
                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(comment);
                        client.CreateComment(response, htmlDoc.DocumentNode.InnerText);
                    }
                }
                //if (issueFields.assignee != null)
                //{
                //    UpdateAssignee(issueFields, response, client);
                //}
                if (response == null)
                {
                    return new Issue();
                }
            }

            return response;
        }

        private void UpdateAssignee(IssueFields issueFields, Issue response, JiraClient client)
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

            try
            {
                client.UpdateIssue(issue);
            }
            catch(JiraServiceException ex)
            {
                _jiraExceptionService.SaveExceptionLogs("JiraService", typeof(JiraServiceException), ex);
            }
        }

        public bool SaveTicketLinkToJira(TicketLinkToJira ticketLinkToJira)
        {
           
            var response = true;
            try
            {
                _jiraTicketsService.SaveJiraTickets(ticketLinkToJira);
            }catch(JiraServiceException ex)
            {
                response = false;
              //  _logger.Error("Error during SaveTicketLinkToJira--Could not save entity to TicketLinkToJira table: " + ex.Message);
                //Log to [dbo].[ExceptionLogs] table
                //passed the ticketLinkToJira object for IAuditModel purposes
                _jiraExceptionService.SaveExceptionLogs("JiraService", typeof(JiraServiceException), ex , ticketLinkToJira);
            }
            return response;
        }

        public Users GetUserMetaData(int? assignedUserToTicketId)
        {
            if(assignedUserToTicketId == null)
                return new Users();
            return _jiraTicketsService.GetUserMetaData(assignedUserToTicketId);
        }

        public List<JiraUser> GetUsers(string baseUrl, string username, string password)
        {
            return new JiraClient(baseUrl, username, password).GetJiraUsers();
        }

        public void Dispose()
        {
            //var temp = _logger as IDisposable;
            //if (temp != null)
            //    temp.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
