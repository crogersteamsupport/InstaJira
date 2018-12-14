using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Web;

namespace TeamSupport.Data
{

    public enum TSEventLogEventType
    {
        LoginSuccess = 7001,
        LogoutSuccess,
        FailedLoginAttempt,
        AccountLocked
    }


    //PowerShell:  [system.Diagnostics.EventLog]::CreateEventSource("TeamSupport", "Application") This must be created manually
    public class TSEventLog
    {
        public static void WriteEvent(TSEventLogEventType tsEventLogEventType, HttpRequest httpRequest = null, User user = null, Organization organization = null, string[] extraInfo = null)
        {
            try
            {
                string source = "TeamSupport";
                if (EventLog.SourceExists(source))
                {

                    List<object> prams = extraInfo == null ? new List<object>() : new List<object>(extraInfo);
                    EventLogEntryType eventLogEntryType = EventLogEntryType.Information;
                    switch (tsEventLogEventType)
                    {
                        case TSEventLogEventType.LoginSuccess:
                            prams.Add("User logged in");
                            eventLogEntryType = EventLogEntryType.SuccessAudit;
                            break;
                        case TSEventLogEventType.LogoutSuccess:
                            prams.Add("User logged out");
                            eventLogEntryType = EventLogEntryType.SuccessAudit;
                            break;
                        case TSEventLogEventType.FailedLoginAttempt:
                            prams.Add("Failed log in attempt");
                            eventLogEntryType = EventLogEntryType.FailureAudit;
                            break;
                        case TSEventLogEventType.AccountLocked:
                            prams.Add("Account locked out");
                            eventLogEntryType = EventLogEntryType.Warning;
                            break;
                        default:
                            break;
                    }

                    if (httpRequest != null)
                    {
                        prams.Add(string.Format("IPAddress: {0}", httpRequest.UserHostAddress));
                    }

                    if (organization != null)
                    {
                        prams.Add(string.Format("OrganizationID: {0}", organization.OrganizationID.ToString()));
                        prams.Add(string.Format("Account: {0}", organization.Name));
                    }

                    if (user != null)
                    {
                        prams.Add(string.Format("UserID: {0}", user.UserID.ToString()));
                        prams.Add(string.Format("User: {0}", user.FirstLastName));
                        prams.Add(string.Format("Email: {0}", user.Email));
                    }
                    EventLog.WriteEvent(source, new EventInstance((int)tsEventLogEventType, 0, eventLogEntryType), prams.ToArray());
                    //EventLog.WriteEntry(source, message, eventLogEntryType, (int)tsEventLogEventType, (short)tsEventLogCategoryType);
                }
                else
                {
                  //  ExceptionLogs.AddLog(LoginUser.Anonymous, "TeamSupport has not been setup as a source in the event logs.");
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Event Logs", "Error writing event log");
                
            }
        }


    }
}
