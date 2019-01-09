using System;
using TeamSupport.EFData.Models;

namespace TeamSupport.JIRA
{
    internal interface IJiraExceptionService
    {
        void SaveExceptionLogs(string URL, Type exceptionName, JiraServiceException ex, TicketLinkToJira ticketLinkToJira);
        void SaveExceptionLogs(string v, Type type, JiraServiceException ex);
    }
}