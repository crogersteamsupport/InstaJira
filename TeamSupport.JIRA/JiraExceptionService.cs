using System;
using TeamSupport.EFData;
using TeamSupport.EFData.Models;

namespace TeamSupport.JIRA
{
    internal class JiraExceptionService : IJiraExceptionService, IDisposable
    {
        private IJiraRepository jiraRepo;
        public JiraExceptionService(IJiraRepository jiraRepo) => this.jiraRepo = jiraRepo;
        public void Dispose() => GC.SuppressFinalize(this);
    
        public void SaveExceptionLogs(string URL, Type type, JiraServiceException ex, TicketLinkToJira ticketLinkToJira)
        {
            var exceptionLogs = new ExceptionLogs();
            exceptionLogs.URL = URL ?? string.Empty;
            exceptionLogs.ExceptionName = type.FullName.ToString() ?? string.Empty;
            exceptionLogs.Message = ex.Message ?? string.Empty;
            exceptionLogs.StackTrace = ex.StackTrace ?? string.Empty;
            exceptionLogs.CreatorID = ticketLinkToJira.CreatorID ?? 0;
            exceptionLogs.DateCreated = exceptionLogs.DateModified = DateTime.UtcNow;
            exceptionLogs.ModifierID = ticketLinkToJira.CreatorID ?? 0;
            jiraRepo.SaveExceptions(exceptionLogs);
        }

        public void SaveExceptionLogs(string URL, Type type, JiraServiceException ex)
        {
            var exceptionLogs = new ExceptionLogs();
            exceptionLogs.URL = URL ?? string.Empty;
            exceptionLogs.ExceptionName = type.FullName.ToString() ?? string.Empty;
            exceptionLogs.Message = ex.Message ?? string.Empty;
            exceptionLogs.StackTrace = ex.StackTrace ?? string.Empty;
            exceptionLogs.DateCreated = exceptionLogs.DateModified = DateTime.UtcNow;
            jiraRepo.SaveExceptions(exceptionLogs);
        }
    }
}