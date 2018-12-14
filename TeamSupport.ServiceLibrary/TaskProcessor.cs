using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSupport.Data;
using System.Globalization;
using System.Net.Mail;

namespace TeamSupport.ServiceLibrary
{
    public class TaskProcessor : ServiceThreadPoolProcess
    {
        //private Logs logs;
        private bool _isDebug = false;
        private bool _logEnabled = false;
        private int _currentTaskEmailPostID;

        private static object _staticLock = new object();

        private static TaskEmailPost GetNextTaskEmailPost(string connectionString, int lockID, bool isDebug)
        {
            TaskEmailPost result;
            LoginUser loginUser = new LoginUser(connectionString, -1, -1, null);
            lock (_staticLock)
            {
                if (isDebug)
                {
                    SqlExecutor.ExecuteNonQuery(loginUser, "UPDATE TaskEmailPosts SET HoldTime=30 WHERE HoldTime > 30");
                }

                result = TaskEmailPosts.GetNextWaiting(loginUser, lockID.ToString());
            }
            return result;
        }

        public void SetTimeZone(TaskEmailPost taskEmailPost)
        {
            LoginUser.TimeZoneInfo = null;
            Organization organization = Users.GetTSOrganization(LoginUser, taskEmailPost.CreatorID);
            if (organization != null)
            {
                try
                {
                    LoginUser.TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(organization.TimeZoneID);
                    Logs.WriteEvent("TimeZoneID: " + organization.TimeZoneID);
                }
                catch (Exception)
                {
                    LoginUser.TimeZoneInfo = null;
                }

                try
                {
                    LoginUser.OrganizationCulture = new CultureInfo(organization.CultureName);
                    Logs.WriteEvent("Culture: " + organization.CultureName);
                }
                catch (Exception)
                {
                    LoginUser.OrganizationCulture = new CultureInfo("en-US");
                }
            }

            if (LoginUser.TimeZoneInfo == null)
            {
                LoginUser.TimeZoneInfo = TimeZoneInfo.Utc;
            }

        }

        public void ProcessEmail(TaskEmailPost taskEmailPost)
        {
            _currentTaskEmailPostID = taskEmailPost.TaskEmailPostID;
            Logs.WriteEvent("TaskEmailPostType: " + taskEmailPost.TaskEmailPostType.ToString());

            switch ((TaskEmailPostType)taskEmailPost.TaskEmailPostType)
            {
                case TaskEmailPostType.Modified:
                    ProcessTaskModified(taskEmailPost.TaskID, taskEmailPost.CreatorID);
                    break;
                case TaskEmailPostType.Assigned:
                    ProcessTaskAssigned(taskEmailPost.TaskID, taskEmailPost.CreatorID);
                    break;
                case TaskEmailPostType.Complete:
                    ProcessTaskComplete(taskEmailPost.TaskID, taskEmailPost.CreatorID);
                    break;
                case TaskEmailPostType.OldUser:
                    ProcessOldUser(taskEmailPost.TaskID, taskEmailPost.CreatorID, (int)taskEmailPost.OldUserID);
                    break;
                default:
                    break;
            }
        }

        public override void Run()
        {
        TaskEmailPosts.UnlockThread(LoginUser, (int)_threadPosition);
            while (!IsStopped)
            {
                TaskEmailPost taskEmailPost = GetNextTaskEmailPost(LoginUser.ConnectionString, (int)_threadPosition, _isDebug);
                if (taskEmailPost == null)
                {
                    System.Threading.Thread.Sleep(10000);
                    continue;
                }

                try
                {
                    try
                    {
                        if (taskEmailPost.CreatorID != -5)
                        {
                            _isDebug = Settings.ReadBool("Debug", false);
                            _logEnabled = Settings.ReadInt("LoggingEnabled", 0) == 1;

                            Logs.WriteLine();
                            Logs.WriteEvent("***********************************************************************************");
                            Logs.WriteEvent("Processing Task Email Post  TaskEmailPostID: " + taskEmailPost.TaskEmailPostID.ToString());
                            Logs.WriteData(taskEmailPost.Row);
                            Logs.WriteEvent("***********************************************************************************");
                            SetTimeZone(taskEmailPost);
                            ProcessEmail(taskEmailPost);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogs.LogException(LoginUser, ex, "Email", taskEmailPost.Row);
                    }
                    finally
                    {
                        Logs.WriteEvent("Deleting from DB");
                        taskEmailPost.Collection.DeleteFromDB(taskEmailPost.TaskEmailPostID);
                    }

                    Logs.WriteEvent("Updating Health");
                    UpdateHealth();
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(LoginUser, ex, "Email", taskEmailPost.Row);
                }
            }
        }

        private void ProcessTaskModified(int taskID, int modifierID)
        {
            TasksViewItem task = TasksView.GetTasksViewItem(LoginUser, taskID);
            try
            {

                User modifier = Users.GetUser(LoginUser, modifierID);
                if (task == null)
                {
                    Logs.WriteEvent("Unable to find Task, TaskID: " + taskID.ToString());
                    return;
                }

                if (modifier == null)
                {
                    Logs.WriteEvent("Unable to find Modifying User, UserID: " + modifierID.ToString());
                    return;
                }

                if (task.UserID == null)
                {
                    Logs.WriteEvent("Assigned Task User is null");
                    return;
                }

                Organization taskOrganization = Organizations.GetOrganization(LoginUser, task.OrganizationID);

                if (taskOrganization == null)
                {
                    Logs.WriteEvent("Task's Organization IS NULL!!!!");
                    return;
                }

                User owner = Users.GetUser(LoginUser, (int)task.UserID);
                if (owner.UserID == modifier.UserID)
                {
                    Logs.WriteEvent("Modifier and Owner are the same person.");
                    return;
                }

                if (Emails.IsEmailDisabled(LoginUser, owner.UserID, "Tasks")) return;

                MailMessage message = EmailTemplates.GetTaskModified(LoginUser, UsersView.GetUsersViewItem(LoginUser, modifierID), UsersView.GetUsersViewItem(LoginUser, (int)task.UserID), task);
                message.To.Add(GetMailAddress(owner.Email, owner.FirstLastName));
                //message.Subject = message.Subject + " [pvt]";
                EmailTemplates.ReplaceMailAddressParameters(message);

                String description = String.Format("Task modified notification sent to {0} for Task {1}", message.To.ToString(), task.Name);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tasks, task.TaskID, description);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Users, (int)task.UserID, description);

                //string emailReplyToAddress = GetEmailReplyToAddress(LoginUser, ticket);
                //AddMessage(taskOrganization, "Ticket Update Request [" + ticket.TicketNumber.ToString() + "]", message, emailReplyToAddress);
                Emails.AddEmail(LoginUser, task.OrganizationID, null, message.Subject, message);
                Logs.WriteEvent("Message queued");
            }
            catch (Exception ex)
            {
                Logs.WriteEvent("Error with ProcessTaskModified");
                Logs.WriteException(ex);
                ExceptionLogs.LogException(LoginUser, ex, "ProcessTaskModified", task.Row);
            }
        }

        private void ProcessTaskAssigned(int taskID, int modifierID)
        {
            TasksViewItem task = TasksView.GetTasksViewItem(LoginUser, taskID);
            try
            {

                User modifier = Users.GetUser(LoginUser, modifierID);
                if (task == null)
                {
                    Logs.WriteEvent("Unable to find Task, TaskID: " + taskID.ToString());
                    return;
                }

                if (modifier == null)
                {
                    Logs.WriteEvent("Unable to find Modifying User, UserID: " + modifierID.ToString());
                    return;
                }

                if (task.UserID == null)
                {
                    Logs.WriteEvent("Assigned Task User is null");
                    return;
                }

                Organization taskOrganization = Organizations.GetOrganization(LoginUser, task.OrganizationID);

                if (taskOrganization == null)
                {
                    Logs.WriteEvent("Task's Organization IS NULL!!!!");
                    return;
                }

                User owner = Users.GetUser(LoginUser, (int)task.UserID);
                if (owner.UserID == modifier.UserID)
                {
                    Logs.WriteEvent("Modifier and Owner are the same person.");
                    return;
                }

                if (Emails.IsEmailDisabled(LoginUser, owner.UserID, "Tasks")) return;

                MailMessage message = EmailTemplates.GetTaskAssigned(LoginUser, UsersView.GetUsersViewItem(LoginUser, modifierID), UsersView.GetUsersViewItem(LoginUser, (int)task.UserID), task);
                message.To.Add(GetMailAddress(owner.Email, owner.FirstLastName));
                //message.Subject = message.Subject + " [pvt]";
                EmailTemplates.ReplaceMailAddressParameters(message);

                String description = String.Format("Task assigned notification sent to {0} for Task {1}", message.To.ToString(), task.Name);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tasks, task.TaskID, description);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Users, (int)task.UserID, description);

                //string emailReplyToAddress = GetEmailReplyToAddress(LoginUser, ticket);
                //AddMessage(taskOrganization, "Ticket Update Request [" + ticket.TicketNumber.ToString() + "]", message, emailReplyToAddress);
                Emails.AddEmail(LoginUser, task.OrganizationID, null, message.Subject, message);
                Logs.WriteEvent("Message queued");
            
            }
            catch (Exception ex)
            {
                Logs.WriteEvent("Error with ProcessTaskAssigned");
                Logs.WriteException(ex);
                ExceptionLogs.LogException(LoginUser, ex, "ProcessTaskModified", task.Row);
            }
        }

        private void ProcessTaskComplete(int taskID, int modifierID)
        {
            TasksViewItem task = TasksView.GetTasksViewItem(LoginUser, taskID);
            try
            {

                User modifier = Users.GetUser(LoginUser, modifierID);
                if (task == null)
                {
                    Logs.WriteEvent("Unable to find Task, TaskID: " + taskID.ToString());
                    return;
                }

                if (modifier == null)
                {
                    Logs.WriteEvent("Unable to find Modifying User, UserID: " + modifierID.ToString());
                    return;
                }

                if (task.UserID == null)
                {
                    Logs.WriteEvent("Assigned Task User is null");
                    return;
                }

                Organization taskOrganization = Organizations.GetOrganization(LoginUser, task.OrganizationID);

                if (taskOrganization == null)
                {
                    Logs.WriteEvent("Task's Organization IS NULL!!!!");
                    return;
                }

                User owner = Users.GetUser(LoginUser, (int)task.UserID);

                MailMessage message = EmailTemplates.GetTaskComplete(LoginUser, UsersView.GetUsersViewItem(LoginUser, modifierID), UsersView.GetUsersViewItem(LoginUser, (int)task.UserID), task);
                if (owner.UserID == modifier.UserID && !Emails.IsEmailDisabled(LoginUser, (int)task.CreatorID, "Tasks"))
                {
                    User creator = Users.GetUser(LoginUser, (int)task.CreatorID);
                    message.To.Add(GetMailAddress(creator.Email, creator.FirstLastName));
                }
                else if (!Emails.IsEmailDisabled(LoginUser, owner.UserID, "Tasks"))
                {
                    message.To.Add(GetMailAddress(owner.Email, owner.FirstLastName));
                }
                if (message.To.Count < 1) return;
                //message.Subject = message.Subject + " [pvt]";
                //EmailTemplates.ReplaceEmailRecipientParameters(LoginUser, message, ticket, owner.UserID, owner.OnlyEmailAfterHours);
                EmailTemplates.ReplaceMailAddressParameters(message);

                String description = String.Format("Task complete notification sent to {0} for Task {1}", message.To.ToString(), task.Name);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tasks, task.TaskID, description);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Users, (int)task.UserID, description);

                //string emailReplyToAddress = GetEmailReplyToAddress(LoginUser, ticket);
                //AddMessage(taskOrganization, "Ticket Update Request [" + ticket.TicketNumber.ToString() + "]", message, emailReplyToAddress);
                Emails.AddEmail(LoginUser, task.OrganizationID, null, message.Subject, message);
                Logs.WriteEvent("Message queued");
            }
            catch (Exception ex)
            {
                Logs.WriteEvent("Error with ProcessTaskComplete");
                Logs.WriteException(ex);
                ExceptionLogs.LogException(LoginUser, ex, "ProcessTaskComplete", task.Row);
            }
        }

        private void ProcessOldUser(int taskID, int modifierID, int oldUserID)
        {
            TasksViewItem task = TasksView.GetTasksViewItem(LoginUser, taskID);
            try
            {

                User modifier = Users.GetUser(LoginUser, modifierID);
                if (task == null)
                {
                    Logs.WriteEvent("Unable to find Task, TaskID: " + taskID.ToString());
                    return;
                }

                if (modifier == null)
                {
                    Logs.WriteEvent("Unable to find Modifying User, UserID: " + modifierID.ToString());
                    return;
                }

                Organization taskOrganization = Organizations.GetOrganization(LoginUser, task.OrganizationID);

                if (taskOrganization == null)
                {
                    Logs.WriteEvent("Task's Organization IS NULL!!!!");
                    return;
                }

                User oldUser = Users.GetUser(LoginUser, oldUserID);

                UsersViewItem owner = null;

                if (task.UserID != null)
                {
                    owner = UsersView.GetUsersViewItem(LoginUser, (int)task.UserID);
                }

                if (Emails.IsEmailDisabled(LoginUser, oldUser.UserID, "Tasks")) return;

                MailMessage message = EmailTemplates.GetTaskOldUser(LoginUser, UsersView.GetUsersViewItem(LoginUser, modifierID), UsersView.GetUsersViewItem(LoginUser, (int)oldUserID), owner, task);
                //if (oldUser.UserID == modifier.UserID)
                //{
                //    User creator = Users.GetUser(LoginUser, (int)task.CreatorID);
                //    message.To.Add(GetMailAddress(creator.Email, creator.FirstLastName));
                //}
                //else
                //{
                message.To.Add(GetMailAddress(oldUser.Email, oldUser.FirstLastName));
                //}
                //message.Subject = message.Subject + " [pvt]";
                //EmailTemplates.ReplaceEmailRecipientParameters(LoginUser, message, ticket, owner.UserID, owner.OnlyEmailAfterHours);
                EmailTemplates.ReplaceMailAddressParameters(message);

                String description = String.Format("Task old user notification sent to {0} for Task {1}", message.To.ToString(), task.Name);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Tasks, task.TaskID, description);
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Users, (int)task.UserID, description);

                //string emailReplyToAddress = GetEmailReplyToAddress(LoginUser, ticket);
                //AddMessage(taskOrganization, "Ticket Update Request [" + ticket.TicketNumber.ToString() + "]", message, emailReplyToAddress);
                Emails.AddEmail(LoginUser, task.OrganizationID, null, message.Subject, message);
                Logs.WriteEvent("Message queued");
            }
            catch (Exception ex)
            {
                Logs.WriteEvent("Error with ProcessTaskComplete");
                Logs.WriteException(ex);
                ExceptionLogs.LogException(LoginUser, ex, "ProcessTaskComplete", task.Row);
            }
        }

        private MailAddress GetMailAddress(string address, string displayName)
        {
            return new MailAddress(FixMailAddress(address), FixMailName(displayName));
        }

        private string FixMailAddress(string address)
        {
            return address.Replace("<", "").Replace(">", "").Replace("|", " ");
        }

        private string FixMailName(string displayName)
        {
            return displayName.Replace("<", "").Replace(">", "").Replace("|", " ");
        }

        public override void ReleaseAllLocks()
        {
            TaskEmailPosts.UnlockAll(LoginUser);
        }
    }
}
