using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using System.IO;
using System.Data.SqlClient;
using System.Net.Mail;

namespace TeamSupport.ServiceLibrary
{
    [Serializable]
    public class ReminderProcessor : ServiceThread
    {
        private Logs logs;

        public override void Run()
        {
            try
            {
                Reminders reminders = new Reminders(LoginUser);
                reminders.LoadForEmail();
                foreach (Reminder reminder in reminders)
                {
                    ProcessReminder(reminder);
                    UpdateHealth();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser, ex, "ReminderProcessor");
            }
        }

        private void ProcessReminder(Reminder reminder)
        {
            Logs.WriteLine();
            Logs.WriteEvent("***********************************************************************************");
            Logs.WriteEvent("Processing Reminder  ReminderID: " + reminder.ReminderID.ToString());
            Logs.WriteData(reminder.Row);
            Logs.WriteLine();
            Logs.WriteEvent("***********************************************************************************");

            MailMessage message;
            UsersViewItem user = UsersView.GetUsersViewItem(LoginUser, (int)reminder.UserID);
            if (user == null) return;
            string description = "";
            switch (reminder.RefType)
            {
                case ReferenceType.Organizations:
                    OrganizationsViewItem org = OrganizationsView.GetOrganizationsViewItem(LoginUser, reminder.RefID);
                    if (org == null) return;
                    message = EmailTemplates.GetReminderCustomerEmail(LoginUser, reminder, user, org);

                    description = String.Format("Reminder sent to {0} for Organization {1}", message.To.ToString(), org.Name);
                    Logs.WriteEvent(description);
                    ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, reminder.RefType, reminder.RefID, description);
                    ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Organizations, org.OrganizationID, description);
                    break;
                case ReferenceType.Tickets:
                    TicketsViewItem ticket = TicketsView.GetTicketsViewItem(LoginUser, reminder.RefID);
                    if (ticket == null) return;
                    message = EmailTemplates.GetReminderTicketEmail(LoginUser, reminder, user, ticket);
                    EmailTemplates.ReplaceEmailRecipientParameters(LoginUser, message, Tickets.GetTicket(LoginUser, ticket.TicketID), reminder.UserID); //vv

                    TeamSupport.Data.Action action = (new Actions(LoginUser)).AddNewAction();
                    action.ActionTypeID = null;
                    action.Name = "Reminder";
                    action.ActionSource = "Reminder";
                    action.SystemActionTypeID = SystemActionType.Reminder;
                    action.Description = String.Format("<p>The following is a reminder for {0} {1}:</p><p>&nbsp;</p><p>{2}</p>", user.FirstName, user.LastName, reminder.Description);
                    action.IsVisibleOnPortal = false;
                    action.IsKnowledgeBase = false;
                    action.TicketID = ticket.TicketID;
                    action.Collection.Save();

                    description = String.Format("Reminder sent to {0} for Ticket {1}", message.To.ToString(), ticket.TicketNumber.ToString());
                    Logs.WriteEvent(description);
                    ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, reminder.RefType, reminder.RefID, description);
                    ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tickets, ticket.TicketID, description);
                    break;
                case ReferenceType.Contacts:
                    ContactsViewItem contact = ContactsView.GetContactsViewItem(LoginUser, reminder.RefID);
                    if (contact == null) return;
                    message = EmailTemplates.GetReminderContactEmail(LoginUser, reminder, user, contact);
                    description = String.Format("Reminder sent to {0} for Contact {1}", message.To.ToString(), contact.FirstName + " " + contact.LastName);
                    Logs.WriteEvent(description);
                    ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, reminder.RefType, reminder.RefID, description);
                    ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Users, contact.UserID, description);
                    break;
                case ReferenceType.Tasks:
                    TasksViewItem task = TasksView.GetTasksViewItem(LoginUser, reminder.RefID);
                    if (task == null || task.IsComplete)
                    {
                        reminder.IsDismissed = true;
                        reminder.Collection.Save();
                        return;
                    }
                    message = EmailTemplates.GetReminderTaskEmail(LoginUser, reminder, user, task);
                    description = String.Format("Reminder sent to {0} for Task {1}", message.To.ToString(), task.Name);
                    Logs.WriteEvent("ver. 05162017: " + description);
                    ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tasks, task.TaskID, description);
                    ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Users, (int)reminder.UserID, description);

                    TaskAssociations taskAssociations = new TaskAssociations(LoginUser);
                    taskAssociations.LoadByTaskIDOnly(task.TaskID);

                    foreach (TaskAssociation taskAssociation in taskAssociations)
                    {
                        if (taskAssociation.RefType == (int)ReferenceType.Tickets)
                        {
                            TeamSupport.Data.Action taskAction = (new Actions(LoginUser)).AddNewAction();
                            taskAction.ActionTypeID = null;
                            taskAction.Name = "Reminder";
                            taskAction.ActionSource = "Reminder";
                            taskAction.SystemActionTypeID = SystemActionType.Reminder;
                            taskAction.Description = String.Format("<p>The following is a reminder for {0} {1}:</p><p>&nbsp;</p><p>{2}</p>", user.FirstName, user.LastName, reminder.Description);
                            taskAction.IsVisibleOnPortal = false;
                            taskAction.IsKnowledgeBase = false;
                            taskAction.TicketID = taskAssociation.RefID;
                            try
                            {
                                taskAction.Collection.Save();
                            }
                            catch (Exception ex)
                            {
                                Logs.WriteEvent("Ex Reminder Action.Save: " + ex.StackTrace);
                            }
                        }
                    }
                    break;
                default:
                    message = null;
                    break;
            }

            if (message == null) return;

            reminder.HasEmailSent = true;
            reminder.Collection.Save();

            if (Emails.IsEmailDisabled(LoginUser, user.UserID, "Reminders"))
            {
                Logs.WriteEvent("Message skipped due to disabled user setting.");

            }
            else
            {
                MailAddress address = new MailAddress(user.Email, user.FirstName + " " + user.LastName);
                Logs.WriteEvent("Mail Address: " + address.ToString());
                message.To.Add(address);
                EmailTemplates.ReplaceMailAddressParameters(message);
                Emails.AddEmail(LoginUser, reminder.OrganizationID, null, message.Subject, message);
                Logs.WriteEvent("Message queued");
            }
        }


    }
}
