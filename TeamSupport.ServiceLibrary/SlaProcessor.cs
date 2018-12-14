using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
    [Serializable]
    public class SlaCalculator : ServiceThread
    {
        public SlaCalculator()
        {
        }

        public override void Run()
        {
            try
            {
                SlaTickets slaTickets = new SlaTickets(LoginUser);
                slaTickets.LoadPending();

                if (slaTickets != null && slaTickets.Count > 0)
                {
                    Logs.WriteEvent(string.Format("{0} pending tickets to calculate the SLA values for.", slaTickets.Count));
                }

                foreach (SlaTicket slaTicket in slaTickets)
                {
                    if (IsStopped) break;
                    UpdateHealth();

                    try
                    {
                        Ticket ticket = Tickets.GetTicket(LoginUser, slaTicket.TicketId);

                        if (ticket != null)
                        {
                            bool isStatusPaused = ticket.IsSlaStatusPaused();
                            bool isClosed = ticket.DateClosed != null;
                            DateTime? newSlaViolationTimeClosed = null;
                            DateTime? newSlaWarningTimeClosed = null;
                            DateTime? newSlaViolationLastAction = null;
                            DateTime? newSlaWarningLastAction = null;
                            DateTime? newSlaViolationInitialResponse = null;
                            DateTime? newSlaWarningInitialResponse = null;

                            if (!isClosed && !isStatusPaused)
                            {
                                DateTime? lastActionDateCreated = Actions.GetLastActionDateCreated(LoginUser, ticket.TicketID);
                                int totalActions = Actions.TotalActionsForSla(LoginUser, ticket.TicketID, ticket.OrganizationID);

                                Organization organization = Organizations.GetOrganization(LoginUser, ticket.OrganizationID);
                                SlaTrigger slaTrigger = SlaTriggers.GetSlaTrigger(LoginUser, slaTicket.SlaTriggerId);

                                if (slaTrigger == null)
                                {
                                    Logs.WriteEventFormat("Trigger {0} not found.", slaTicket.SlaTriggerId);
                                }

                                SlaTickets.BusinessHours businessHours = new SlaTickets.BusinessHours()
                                {
                                    DayStartUtc = organization.BusinessDayStartUtc,
                                    DayEndUtc = organization.BusinessDayEndUtc,
                                    BusinessDays = organization.BusinessDays
                                };

                                //Check if we should use SLA's business hours instead of Account's
                                if (!slaTrigger.UseBusinessHours
                                    && !slaTrigger.NoBusinessHours
                                    && slaTrigger.DayStartUtc.HasValue
                                    && slaTrigger.DayEndUtc.HasValue)
                                {
                                    businessHours.DayStartUtc = slaTrigger.DayStartUtc.Value;
                                    businessHours.DayEndUtc = slaTrigger.DayEndUtc.Value;
                                    businessHours.BusinessDays = slaTrigger.Weekdays;
                                }
                                else if (!slaTrigger.UseBusinessHours && !slaTrigger.NoBusinessHours)
                                {
                                    Logs.WriteEventFormat("Using Account's business hours {0} to {1} because while the trigger is set to use sla's business hours one of them has no value. Sla DayStartUtc {2}, Sla DayEndUtc {3}",
                                        organization.BusinessDayStartUtc.ToShortTimeString(),
                                        organization.BusinessDayEndUtc.ToShortTimeString(),
                                        slaTrigger.DayStartUtc.HasValue ? slaTrigger.DayStartUtc.Value.ToShortTimeString() : "NULL",
                                        slaTrigger.DayEndUtc.HasValue ? slaTrigger.DayEndUtc.Value.ToShortTimeString() : "NULL");
                                }

                                Logs.WriteEvent(string.Format("Ticket #{0} id {1}. LastAction: {2}, TotalActions: {3}, Org({4}) {5}, SlaTriggerId {6}.", ticket.TicketNumber,
                                                                                                                                                        ticket.TicketID,
                                                                                                                                                        lastActionDateCreated == null ? "none" : lastActionDateCreated.Value.ToString(),
                                                                                                                                                        totalActions,
                                                                                                                                                        organization.OrganizationID,
                                                                                                                                                        organization.Name,
                                                                                                                                                        slaTrigger.SlaTriggerID));
                                List<DateTime> daysToPause = SlaTriggers.GetSpecificDaysToPause(slaTrigger.SlaTriggerID);
                                bool pauseOnHoliday = slaTrigger.PauseOnHoliday;
                                CalendarEvents holidays = new CalendarEvents(LoginUser);

                                if (pauseOnHoliday)
                                {
                                    holidays.LoadHolidays(organization.OrganizationID);
                                }

                                Dictionary<int, double> businessPausedTimes = new Dictionary<int, double>();

								//We need to have two calculations for the paused time, one for the Time To Close and Initial Response which should count the paused time since the Ticket creation
								//and another one for Last Action where it should only count starting from the last action added
                                TimeSpan totalPausedTimeSpan = SlaTickets.CalculatePausedTime(ticket.TicketID, organization, businessHours, slaTrigger, daysToPause, holidays, LoginUser, businessPausedTimes, Logs);
                                Logs.WriteEventFormat("Total Paused Time: {0}", totalPausedTimeSpan.ToString());

                                UpdateBusinessPausedTimes(LoginUser, businessPausedTimes);

                                newSlaViolationTimeClosed = SlaTickets.CalculateSLA(ticket.DateCreatedUtc, businessHours, slaTrigger, slaTrigger.TimeToClose, totalPausedTimeSpan, daysToPause, holidays);

                                if (newSlaViolationTimeClosed != null)
                                {
                                    newSlaWarningTimeClosed = SlaTickets.CalculateSLAWarning(newSlaViolationTimeClosed.Value,
                                                                                    businessHours,
                                                                                    slaTrigger.NoBusinessHours,
                                                                                    slaTrigger.TimeToClose,
                                                                                    slaTrigger.WarningTime,
                                                                                    daysToPause,
                                                                                    holidays);
                                }

								TimeSpan pausedTimeSpanSinceLastAction = SlaTickets.CalculatePausedTime(ticket.TicketID,
																										organization, businessHours,
																										slaTrigger,
																										daysToPause,
																										holidays,
																										LoginUser,
																										businessPausedTimes,
																										Logs,
																										false,
																										lastActionDateCreated.HasValue ? lastActionDateCreated.Value : (DateTime?)null);
								Logs.WriteEventFormat("Total Paused Time: {0}", pausedTimeSpanSinceLastAction.ToString());

								if (lastActionDateCreated == null)
                                {
                                    newSlaViolationLastAction = null;
                                    newSlaWarningLastAction = null;
                                }
                                else
                                {
                                    newSlaViolationLastAction = SlaTickets.CalculateSLA(lastActionDateCreated.Value,
                                                                                businessHours,
                                                                                slaTrigger,
                                                                                slaTrigger.TimeLastAction,
																				pausedTimeSpanSinceLastAction,
                                                                                daysToPause,
                                                                                holidays);

                                    if (newSlaViolationLastAction != null)
                                    {
                                        newSlaWarningLastAction = SlaTickets.CalculateSLAWarning((DateTime)newSlaViolationLastAction.Value,
                                                                                        businessHours,
                                                                                        slaTrigger.NoBusinessHours,
                                                                                        slaTrigger.TimeLastAction,
                                                                                        slaTrigger.WarningTime,
                                                                                        daysToPause,
                                                                                        holidays);
                                    }
                                }

                                if (slaTrigger.TimeInitialResponse < 1 || totalActions > 0)
                                {
                                    newSlaViolationInitialResponse = null;
                                    newSlaWarningInitialResponse = null;
                                }
                                else
                                {
                                    newSlaViolationInitialResponse = SlaTickets.CalculateSLA(ticket.DateCreatedUtc,
                                                                                    businessHours,
                                                                                    slaTrigger,
                                                                                    slaTrigger.TimeInitialResponse,
																					totalPausedTimeSpan,
                                                                                    daysToPause,
                                                                                    holidays);

                                    if (newSlaViolationInitialResponse != null)
                                    {
                                        newSlaWarningInitialResponse = SlaTickets.CalculateSLAWarning((DateTime)newSlaViolationInitialResponse.Value,
                                                                                            businessHours,
                                                                                            slaTrigger.NoBusinessHours,
                                                                                            slaTrigger.TimeInitialResponse,
                                                                                            slaTrigger.WarningTime,
                                                                                            daysToPause,
                                                                                            holidays);
                                    }
                                }
                            }
                            else
                            {
                                Logs.WriteEventFormat("Ticket is {0}, clearing its SLA values.", isClosed ? "Closed" : "Status Paused");
                            }

                            if (HasAnySlaChanges(ticket,
                                                newSlaViolationTimeClosed,
                                                newSlaWarningTimeClosed,
                                                newSlaViolationLastAction,
                                                newSlaWarningLastAction,
                                                newSlaViolationInitialResponse,
                                                newSlaWarningInitialResponse))
                            {
                                ticket.SlaViolationTimeClosed = newSlaViolationTimeClosed;
                                ticket.SlaWarningTimeClosed = newSlaWarningTimeClosed;
                                ticket.SlaViolationLastAction = newSlaViolationLastAction;
                                ticket.SlaWarningLastAction = newSlaWarningLastAction;
                                ticket.SlaViolationInitialResponse = newSlaViolationInitialResponse;
                                ticket.SlaWarningInitialResponse = newSlaWarningInitialResponse;
                                Tickets.UpdateTicketSla(LoginUser,
                                                        ticket.TicketID,
                                                        newSlaViolationInitialResponse,
                                                        newSlaViolationLastAction,
                                                        newSlaViolationTimeClosed,
                                                        newSlaWarningInitialResponse,
                                                        newSlaWarningLastAction,
                                                        newSlaWarningTimeClosed);
                                Logs.WriteEvent("Ticket SLA calculation completed.");

                                string signalRUrl = Settings.ReadString("SignalRUrl");

                                if (!string.IsNullOrEmpty(signalRUrl))
                                {
                                    //Dictionary<string, string> queryStringData = new Dictionary<string, string>();
                                    //queryStringData.Add("userID", "-1");
                                    //queryStringData.Add("organizationID", ticket.OrganizationID.ToString());
                                    //HubConnection connection = new HubConnection(signalRUrl, queryStringData);
                                    //IHubProxy signalRConnection = connection.CreateHubProxy("TicketSocket");

                                    //try
                                    //{
                                    //    connection.Start().Wait();
                                    //    signalRConnection.Invoke("RefreshSLA", ticket.TicketNumber);
                                    //}
                                    //catch (Exception ex)
                                    //{
                                    //    Logs.WriteEvent("Could not send signalR to refresh the SLA. Message: " + ex.Message);
                                    //}
                                }
                            }
                            else
                            {
                                Logs.WriteEvent("Ticket SLA calculation completed. SLA values did not change, ticket was not updated.");
                            }

                            slaTicket.IsPending = false;
                            slaTicket.Collection.Save();
                        }
                        else
                        {
                            SlaPausedTimes slaPausedTimes = new SlaPausedTimes(LoginUser);
                            slaPausedTimes.LoadByTicketId(slaTicket.TicketId);
                            slaPausedTimes.DeleteAll();
                            slaPausedTimes.Save();

                            slaTicket.Delete();
                            slaTicket.Collection.Save();
                            Logs.WriteEventFormat("Ticket id {0} does not exist anymore, deleted from SlaTickets.", slaTicket.TicketId);
                        }

                        System.Threading.Thread.Sleep(100);
                    }
                    catch (System.Data.SqlClient.SqlException sqlEx)
                    {
                        //Handle the deadlock exception, any other bubble up.
                        if (sqlEx.Number == 1205 || sqlEx.Message.Contains("deadlocked"))
                        {
                            ExceptionLogs.LogException(LoginUser, sqlEx, "SLA Calculator", "Sync");
                            Logs.WriteEventFormat("Exception. Message {0}{1}StackTrace {2}", sqlEx.Message, Environment.NewLine, sqlEx.StackTrace);
                            slaTicket.IsPending = true;
                            slaTicket.Collection.Save();
                            Logs.WriteEventFormat("SlaTicket: TicketId {0} TriggerId {1} still pending.", slaTicket.TicketId, slaTicket.SlaTriggerId);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogs.LogException(LoginUser, ex, "SLA Calculator", "Sync");
                        Logs.WriteEventFormat("Exception. Message {0}{1}StackTrace {2}", ex.Message, Environment.NewLine, ex.StackTrace);

                        slaTicket.IsPending = false;
                        slaTicket.Collection.Save();
                        Logs.WriteEventFormat("SlaTicket: TicketId {0} TriggerId {1} set to not pending.", slaTicket.TicketId, slaTicket.SlaTriggerId);
                    }
                }

                if (slaTickets != null && slaTickets.Count > 0)
                {
                    Logs.WriteEvent(string.Format("Completed processing the {0} pending tickets to calculate the SLA values for.", slaTickets.Count));
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser, ex, "SLA Calculator", "Sync");
                Logs.WriteEvent(string.Format("Exception. Message {0}{1}StackTrace {2}", ex.Message, Environment.NewLine, ex.StackTrace));
            }
        }

        private static bool HasAnySlaChanges(Ticket ticket,
                                            DateTime? newSlaViolationTimeClosed,
                                            DateTime? newSlaWarningTimeClosed,
                                            DateTime? newSlaViolationLastAction,
                                            DateTime? newSlaWarningLastAction,
                                            DateTime? newSlaViolationInitialResponse,
                                            DateTime? newSlaWarningInitialResponse)
        {
            bool hasChanges = false;

            if (!hasChanges && DateTime.Compare(ticket.SlaViolationTimeClosed == null ? new DateTime() : ticket.SlaViolationTimeClosed.Value, 
                                                newSlaViolationTimeClosed == null ? new DateTime() : newSlaViolationTimeClosed.Value) != 0)
            {
                hasChanges = true;
            }

            if (!hasChanges && DateTime.Compare(ticket.SlaWarningTimeClosed == null ? new DateTime() : ticket.SlaWarningTimeClosed.Value,
                                                newSlaWarningTimeClosed == null ? new DateTime() : newSlaWarningTimeClosed.Value) != 0)
            {
                hasChanges = true;
            }

            if (!hasChanges && DateTime.Compare(ticket.SlaViolationLastAction == null ? new DateTime() : ticket.SlaViolationLastAction.Value,
                                                newSlaViolationLastAction == null ? new DateTime() : newSlaViolationLastAction.Value) != 0)
            {
                hasChanges = true;
            }

            if (!hasChanges && DateTime.Compare(ticket.SlaWarningLastAction == null ? new DateTime() : ticket.SlaWarningLastAction.Value,
                                                newSlaWarningLastAction == null ? new DateTime() : newSlaWarningLastAction.Value) != 0)
            {
                hasChanges = true;
            }

            if (!hasChanges && DateTime.Compare(ticket.SlaViolationInitialResponse == null ? new DateTime() : ticket.SlaViolationInitialResponse.Value,
                                                newSlaViolationInitialResponse == null ? new DateTime() : newSlaViolationInitialResponse.Value) != 0)
            {
                hasChanges = true;
            }

            if (!hasChanges && DateTime.Compare(ticket.SlaWarningInitialResponse == null ? new DateTime() : ticket.SlaWarningInitialResponse.Value,
                                                newSlaWarningInitialResponse == null ? new DateTime() : newSlaWarningInitialResponse.Value) != 0)
            {
                hasChanges = true;
            }

            return hasChanges;
        }

        private static void UpdateBusinessPausedTimes(LoginUser loginUser, Dictionary<int, double> businessPausedTimes)
        {
            foreach(KeyValuePair<int, double> pair in businessPausedTimes)
            {
                SlaPausedTimes.UpdateBusinessPausedTime(loginUser, pair.Key, pair.Value);
            }
        }
    }

    [Serializable]
  public class SlaProcessor : ServiceThread
  {
    DateTime _lastDLSAdjustment = DateTime.MinValue;

    public SlaProcessor()
    {
    }

    public override void Run()
    {
      try
      {
          if (DateTime.Now.Subtract(_lastDLSAdjustment).TotalMinutes > 60 && DateTime.Now.Minute > 2 && DateTime.Now.Minute < 30)
          {
            Logs.WriteEvent("Update business hours for DSL");
            _lastDLSAdjustment = DateTime.Now;
            UpdateBusinessHoursForDLSFix();
          }
        }
        catch (Exception)
        {
        }

		try
		{
			foreach (TicketSlaInfo ticket in GetAllUnnotifiedAndExpiredSla(LoginUser))
			{
				if (IsStopped) break;
				Logs.WriteEventFormat("Attempting to process: {0}", ticket.TicketId);
				ProcessTicket(ticket);
				System.Threading.Thread.Sleep(0);
			}
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "SLA Processor", "Sync");
      }
    }

        private void ProcessTicket(TicketSlaInfo ticket)
        {
            UpdateHealth();

			bool isPaused = false;
			bool isPending = false;
			int? slaTriggerId = null;
            Logs.WriteEvent("Getting SlaTicket record");
            SlaTicket slaTicket = SlaTickets.GetSlaTicket(LoginUser, ticket.TicketId);
            
            if (slaTicket != null)
            {
                isPaused = ticket.IsSlaPaused(slaTicket.SlaTriggerId, ticket.OrganizationId);
                isPending = slaTicket.IsPending;
				slaTriggerId = slaTicket.SlaTriggerId;
            }

            Logs.WriteEventFormat("IsPaused: {0}; IsPending: {1}", isPaused.ToString(), isPending.ToString());

            if (!isPaused && !isPending)
            {
                SlaTriggersView triggers = new SlaTriggersView(LoginUser);
                triggers.LoadByTicketId(ticket.TicketId);
                bool warnGroup = false;
                bool warnUser = false;
                bool vioGroup = false;
                bool vioUser = false;

                foreach (SlaTriggersViewItem item in triggers)
                {
                    warnGroup = item.NotifyGroupOnWarning || warnGroup;
                    warnUser = item.NotifyUserOnWarning || warnUser;
                    vioGroup = item.NotifyGroupOnViolation || vioGroup;
                    vioUser = item.NotifyUserOnViolation || vioUser;
                }

                SlaNotification notification = SlaNotifications.GetSlaNotification(LoginUser, ticket.TicketId);
                if (notification == null)
                {
                    notification = (new SlaNotifications(LoginUser)).AddNewSlaNotification();
                    notification.TicketID = ticket.TicketId;
                }

                DateTime notifyTime;

                if (ticket.SlaViolationInitialResponse != null && ticket.SlaViolationInitialResponse <= DateTime.UtcNow)
                {
                    notifyTime = (DateTime)ticket.SlaViolationInitialResponse;
                    if (!IsTooOld(notifyTime))
                    {
                        if (notification.InitialResponseViolationDate == null || Math.Abs(((DateTime)notification.InitialResponseViolationDateUtc - notifyTime).TotalMinutes) > 5)
                        {
                            NotifyViolation(ticket.TicketId, vioUser, vioGroup, false, SlaViolationType.InitialResponse, notification, slaTriggerId);
                            notification.InitialResponseViolationDate = notifyTime;
                        }
                    }
                }
                else if (ticket.SlaWarningInitialResponse != null && ticket.SlaWarningInitialResponse <= DateTime.UtcNow)
                {
                    notifyTime = (DateTime)ticket.SlaWarningInitialResponse;

                    if (!IsTooOld(notifyTime))
                    {
                        if (notification.InitialResponseWarningDate == null || Math.Abs(((DateTime)notification.InitialResponseWarningDateUtc - notifyTime).TotalMinutes) > 5)
                        {
                            NotifyViolation(ticket.TicketId, warnUser, warnGroup, true, SlaViolationType.InitialResponse, notification, slaTriggerId);
                            notification.InitialResponseWarningDate = notifyTime;
                        }
                    }
                }


                if (ticket.SlaViolationLastAction != null && ticket.SlaViolationLastAction <= DateTime.UtcNow)
                {
                    notifyTime = (DateTime)ticket.SlaViolationLastAction;

                    if (!IsTooOld(notifyTime))
                    {
                        if (notification.LastActionViolationDate == null || Math.Abs(((DateTime)notification.LastActionViolationDateUtc - notifyTime).TotalMinutes) > 5)
                        {
                            NotifyViolation(ticket.TicketId, vioUser, vioGroup, false, SlaViolationType.LastAction, notification, slaTriggerId);
                            notification.LastActionViolationDate = notifyTime;
                        }
                    }
                }
                else if (ticket.SlaWarningLastAction != null && ticket.SlaWarningLastAction <= DateTime.UtcNow)
                {
                    notifyTime = (DateTime)ticket.SlaWarningLastAction;

                    if (!IsTooOld(notifyTime))
                    {
                        if (notification.LastActionWarningDate == null || Math.Abs(((DateTime)notification.LastActionWarningDateUtc - notifyTime).TotalMinutes) > 5)
                        {
                            NotifyViolation(ticket.TicketId, warnUser, warnGroup, true, SlaViolationType.LastAction, notification, slaTriggerId);
                            notification.LastActionWarningDate = notifyTime;
                        }
                    }
                }


                if (ticket.SlaViolationTimeClosed != null && ticket.SlaViolationTimeClosed <= DateTime.UtcNow)
                {
                    notifyTime = (DateTime)ticket.SlaViolationTimeClosed;

                    if (!IsTooOld(notifyTime))
                    {
                        if (notification.TimeClosedViolationDate == null || Math.Abs(((DateTime)notification.TimeClosedViolationDateUtc - notifyTime).TotalMinutes) > 5)
                        {
                            NotifyViolation(ticket.TicketId, vioUser, vioGroup, false, SlaViolationType.TimeClosed, notification, slaTriggerId);
                            notification.TimeClosedViolationDate = notifyTime;
                        }
                    }
                }
                else if (ticket.SlaWarningTimeClosed != null && ticket.SlaWarningTimeClosed <= DateTime.UtcNow)
                {
                    notifyTime = (DateTime)ticket.SlaWarningTimeClosed;

                    if (!IsTooOld(notifyTime))
                    {
                        if (notification.TimeClosedWarningDate == null || Math.Abs(((DateTime)notification.TimeClosedWarningDateUtc - notifyTime).TotalMinutes) > 5)
                        {
                            NotifyViolation(ticket.TicketId, warnUser, warnGroup, true, SlaViolationType.TimeClosed, notification, slaTriggerId);
                            notification.TimeClosedWarningDate = notifyTime;
                        }
                    }
                }

                notification.Collection.Save();
            }
        }

    private bool IsTooOld(DateTime notifyTime)
    {
      return (DateTime.UtcNow - notifyTime).TotalDays >= 1;
    }

    private void NotifyViolation(int ticketID, bool useUser, bool useGroup, bool isWarning, SlaViolationType slaViolationType, SlaNotification notification, int? triggerId)
    {
      Users users = new Users(LoginUser);
      User user = null;

      Logs.WriteLine();
      Logs.WriteEvent("***** Processing TicketID: " + ticketID.ToString());
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(LoginUser, ticketID);

		if (ticket == null)
		{
			Logs.WriteEvent("Ticket is NULL, exiting");
			return;
		}

		//Since we are already re-loading the Ticket information we will check again if the SLAs still apply at this point
		string violationType = "";
		bool slaNotApplyAnymore = false;

		switch (slaViolationType)
		{
			case SlaViolationType.InitialResponse:
				violationType = "Initial Response";
				slaNotApplyAnymore = ticket.SlaWarningInitialResponse == null;
				break;
			case SlaViolationType.LastAction:
				violationType = "Last Action";
				slaNotApplyAnymore = ticket.SlaViolationLastAction == null;
				break;
			case SlaViolationType.TimeClosed:
				violationType = "Time to Close";
				slaNotApplyAnymore = ticket.SlaViolationTimeClosed == null;
				break;
			default: break;
		}

		if (slaNotApplyAnymore)
		{
			Logs.WriteEvent($"The {((isWarning) ? "warning" : "violation")} for {violationType} does not apply anymore because the Ticket has been updated to satisfy this SLA while it was in process for the notification.");
		}
		else
		{
			if (!isWarning)
			{
				SlaViolationHistoryItem history = (new SlaViolationHistory(LoginUser)).AddNewSlaViolationHistoryItem();
				history.DateViolated = DateTime.UtcNow;
				history.GroupID = ticket.GroupID;
				history.UserID = ticket.UserID;
				history.ViolationType = slaViolationType;
				history.TicketID = ticket.TicketID;
				history.SlaTriggerId = triggerId;
				history.Collection.Save();
			}

			Actions actions = new Actions(LoginUser);
			actions.LoadLatestByTicket(ticket.TicketID, false);

			ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, violationType + " SLA " + (isWarning ? "warning" : "violation") + " occurred");

			MailMessage message = EmailTemplates.GetSlaEmail(LoginUser, ticket, violationType, isWarning);


			if (ticket.GroupID != null && useGroup)
			{
				users.LoadByGroupID((int)ticket.GroupID);
			}

			if (ticket.UserID != null && useUser && users.FindByUserID((int)ticket.UserID) == null)
			{
				user = Users.GetUser(LoginUser, (int)ticket.UserID);
			}

			foreach (User item in users)
			{
				if (Emails.IsEmailDisabled(LoginUser, item.UserID, "SLA")) continue;
				message.To.Add(new MailAddress(item.Email, item.FirstLastName));
			}

			if (user != null)
			{
				if (!Emails.IsEmailDisabled(LoginUser, user.UserID, "SLA"))
				{
					message.To.Add(new MailAddress(user.Email, user.FirstLastName));
					Logs.WriteEvent(string.Format("Adding Main User, Name:{0}  Email:{1}  UserID:{2}", user.FirstLastName, user.Email, user.UserID.ToString()));
				}
			}

			if (message.To.Count > 0)
			{
				Email email = Emails.AddEmail(LoginUser, ticket.OrganizationID, null, "Sla Message", message);
				Logs.WriteEvent("Email Added (EmailID: " + email.EmailID.ToString() + ")", true);
			}

			try
			{


				TimeZoneInfo tz = null;
				Organization org = Organizations.GetOrganization(LoginUser, ticket.OrganizationID);
				if (org.TimeZoneID != null) { tz = System.TimeZoneInfo.FindSystemTimeZoneById(org.TimeZoneID); }
				if (tz == null)
				{
					Logs.WriteEvent("Timezone is null, using system's");
					tz = System.TimeZoneInfo.Local;
				}
				Logs.WriteEvent(tz.DisplayName);
				Logs.WriteEvent("Supports DLS: " + tz.SupportsDaylightSavingTime.ToString());
				Logs.WriteEvent("Is DLS: " + tz.IsDaylightSavingTime(DateTime.UtcNow).ToString());
				Logs.WriteEvent("UTC: " + DateTime.UtcNow.ToString());
				Logs.WriteEvent(string.Format("NOTIFYING TicketID:{0}  TicketNumber:{1}  OrganizationID:{2} ", ticket.TicketID.ToString(), ticket.TicketNumber.ToString(), ticket.OrganizationID.ToString()));
				Logs.WriteEvent(string.Format("User:{1}  Group:{2}  IsWarning:{3}  NoficationType:{4}", ticketID.ToString(), useUser.ToString(), useGroup.ToString(), isWarning.ToString(), slaViolationType));
				Logs.WriteEvent("Ticket Data:");
				Logs.WriteData(ticket.Row);
				Logs.WriteEvent("Notification Data:");
				Logs.WriteData(notification.Row);
				Logs.WriteEvent("Organization Data:");
				Logs.WriteData(org.Row);
				Organizations customers = new Organizations(LoginUser);
				customers.LoadByTicketID(ticket.TicketID);

				foreach (Organization customer in customers)
				{
					Logs.WriteEvent("-- Customer: " + customer.Name);
					if (customer.SlaLevelID != null)
					{
						SlaLevel level = SlaLevels.GetSlaLevel(LoginUser, (int)customer.SlaLevelID);
						Logs.WriteEvent("SLA Level: " + level.Name);
						SlaTriggers triggers = new SlaTriggers(LoginUser);
						triggers.LoadByTicketTypeAndSeverity(level.SlaLevelID, ticket.TicketTypeID, ticket.TicketSeverityID);

						foreach (SlaTrigger trigger in triggers)
						{
							Logs.WriteData(trigger.Row);
						}
					}
					else
					{
						Logs.WriteEvent("No SLA Level Assigned to " + customer.Name);
					}

				}

				if (org.InternalSlaLevelID != null)
				{
					Logs.WriteEvent("Internal SLA:");

					SlaTriggers triggers = new SlaTriggers(LoginUser);
					triggers.LoadByTicketTypeAndSeverity((int)org.InternalSlaLevelID, ticket.TicketTypeID, ticket.TicketSeverityID);
					foreach (SlaTrigger trigger in triggers)
					{
						Logs.WriteData(trigger.Row);
					}
				}
				else
				{
					Logs.WriteEvent("No Internal SLA");
				}
			}
			catch (Exception ex)
			{
				Logs.WriteEvent("Logging Exception:");
				Logs.WriteException(ex);
			}
		}
    }
    
    private void UpdateBusinessHoursForDLSFix()
    {
      Organizations customers = new Organizations(LoginUser);
      customers.LoadTeamSupportCustomers();

      foreach (Organization customer in customers)
      {
        //if (customer.OrganizationID != 13679) continue;
        TimeZoneInfo tz = System.TimeZoneInfo.Local;
        if (!string.IsNullOrWhiteSpace(customer.TimeZoneID))
        {
          tz = System.TimeZoneInfo.FindSystemTimeZoneById(customer.TimeZoneID);
        }

        DateTime today = DateTime.Now;
        DateTime start = TimeZoneInfo.ConvertTimeFromUtc(customer.BusinessDayStartUtc, tz); 
        DateTime end = TimeZoneInfo.ConvertTimeFromUtc(customer.BusinessDayEndUtc, tz);

        customer.BusinessDayStart = TimeZoneInfo.ConvertTimeToUtc(new DateTime(today.Year, today.Month, today.Day, start.Hour, start.Minute, 0), tz);
        customer.BusinessDayEnd = TimeZoneInfo.ConvertTimeToUtc(new DateTime(today.Year, today.Month, today.Day, end.Hour, end.Minute, 0), tz);
      }
      customers.Save();
    }

		public class TicketSlaInfo
		{
			public int TicketId { get; set; }
			public int TicketStatusId { get; set; }
			public int OrganizationId { get; set; }
			public DateTime? SlaViolationTimeClosed { get; set; }
			public DateTime? SlaWarningTimeClosed { get; set; }
			public DateTime? SlaViolationLastAction { get; set; }
			public DateTime? SlaWarningLastAction { get; set; }
			public DateTime? SlaViolationInitialResponse { get; set; }
			public DateTime? SlaWarningInitialResponse { get; set; }
			private LoginUser _loginUser;

			public TicketSlaInfo(LoginUser loginUser)
			{
				_loginUser = loginUser;
			}

			public bool IsSlaPaused(int triggerId, int organizationId)
			{
				bool isPaused = false;
				TicketStatuses statuses = new TicketStatuses(_loginUser);
				statuses.LoadByStatusIDs(organizationId, new int[] { TicketStatusId });

				if (statuses != null && statuses.Any())
				{
					isPaused = statuses[0].PauseSLA;
				}

				//check if "Pause on Specific Dates"
				if (!isPaused)
				{
					List<DateTime> daysToPause = SlaTriggers.GetSpecificDaysToPause(triggerId);
					isPaused = daysToPause.Where(p => DateTime.Compare(p.Date, DateTime.UtcNow.Date) == 0).Any();
				}

				//If Pause on Company Holidays is selected
				SlaTrigger slaTrigger = SlaTriggers.GetSlaTrigger(_loginUser, triggerId);

				if (!isPaused && slaTrigger.PauseOnHoliday)
				{
					isPaused = SlaTriggers.IsOrganizationHoliday(organizationId, DateTime.UtcNow);
				}

				return isPaused;
			}
		}

		public List<TicketSlaInfo> GetAllUnnotifiedAndExpiredSla(LoginUser loginUser)
		{
			List<TicketSlaInfo> unnotifiedAndExpiredSla = new List<TicketSlaInfo>();

			using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
			{
				using (SqlCommand command = new SqlCommand())
				{
					command.Connection = connection;
					command.CommandText =
			@"
SELECT t.TicketId, t.TicketStatusId, OrganizationId, SlaViolationTimeClosed, SlaWarningTimeClosed, SlaViolationLastAction, SlaWarningLastAction, SlaViolationInitialResponse, SlaWarningInitialResponse
FROM Tickets AS t WITH(NOLOCK)
LEFT JOIN SlaNotifications AS sn WITH(NOLOCK)
ON t.TicketID = sn.TicketID
WHERE
t.DateClosed IS NULL
AND
(
  (
    t.SlaViolationTimeClosed IS NOT NULL AND
    t.SlaViolationTimeClosed < DATEADD(DAY, 1, GETUTCDATE()) AND
	t.SlaViolationTimeClosed > DATEADD(DAY, -1, GETUTCDATE()) AND
    t.SlaViolationTimeClosed > DATEADD(MINUTE, 10, ISNULL(sn.TimeClosedViolationDate, '1/1/1980'))
  )
  OR
  (
    t.SlaWarningTimeClosed IS NOT NULL AND
    t.SlaWarningTimeClosed < DATEADD(DAY, 1, GETUTCDATE()) AND
	t.SlaWarningTimeClosed > DATEADD(DAY, -1, GETUTCDATE()) AND
    t.SlaWarningTimeClosed > DATEADD(MINUTE, 10, ISNULL(sn.TimeClosedWarningDate, '1/1/1980'))
  )
  OR
  (
    t.SlaViolationLastAction IS NOT NULL AND
    t.SlaViolationLastAction < DATEADD(DAY, 1, GETUTCDATE()) AND
	t.SlaViolationLastAction > DATEADD(DAY, -1, GETUTCDATE()) AND
    t.SlaViolationLastAction > DATEADD(MINUTE, 10, ISNULL(sn.LastActionViolationDate, '1/1/1980'))
  )
  OR
  (
    t.SlaWarningLastAction IS NOT NULL AND
    t.SlaWarningLastAction < DATEADD(DAY, 1, GETUTCDATE()) AND
	t.SlaWarningLastAction > DATEADD(DAY, -1, GETUTCDATE()) AND
    t.SlaWarningLastAction > DATEADD(MINUTE, 10, ISNULL(sn.LastActionWarningDate, '1/1/1980'))
  )
  OR
  (
    t.SlaViolationInitialResponse IS NOT NULL AND
    t.SlaViolationInitialResponse < DATEADD(DAY, 1, GETUTCDATE()) AND
	t.SlaViolationInitialResponse > DATEADD(DAY, -1, GETUTCDATE()) AND
    t.SlaViolationInitialResponse > DATEADD(MINUTE, 10, ISNULL(sn.InitialResponseViolationDate, '1/1/1980'))
  )
  OR
  (
    t.SlaWarningInitialResponse IS NOT NULL AND
    t.SlaWarningInitialResponse < DATEADD(DAY, 1, GETUTCDATE()) AND
	t.SlaWarningInitialResponse > DATEADD(DAY, -1, GETUTCDATE()) AND
    t.SlaWarningInitialResponse > DATEADD(MINUTE, 10, ISNULL(sn.InitialResponseWarningDate, '1/1/1980'))
  )
)
";
					command.CommandText =
		@"
SELECT t.TicketId, t.TicketStatusId, OrganizationId, SlaViolationTimeClosed, SlaWarningTimeClosed, SlaViolationLastAction, SlaWarningLastAction, SlaViolationInitialResponse, SlaWarningInitialResponse
FROM Tickets t with(nolock)
LEFT JOIN SlaNotifications sn with(nolock) ON t.TicketID = sn.TicketID
WHERE t.ticketID = 3887997";

					command.CommandType = CommandType.Text;
					connection.Open();

					SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

					if (reader.HasRows)
					{
						while (reader.Read())
						{
							var col = reader.GetOrdinal("SlaViolationTimeClosed");

							TicketSlaInfo ticket = new TicketSlaInfo(LoginUser)
							{
								TicketId = (Int32)reader["TicketId"],
								TicketStatusId = (Int32)reader["TicketStatusId"],
								OrganizationId = (Int32)reader["OrganizationId"],
								SlaViolationTimeClosed = reader.IsDBNull(col) ? (DateTime?)null : (DateTime?)reader.GetDateTime(col),
								SlaWarningTimeClosed = reader.GetNullableDateTime("SlaWarningTimeClosed"),
								SlaViolationLastAction = reader.GetNullableDateTime("SlaViolationLastAction"),
								SlaWarningLastAction = reader.GetNullableDateTime("SlaWarningLastAction"),
								SlaViolationInitialResponse = reader.GetNullableDateTime("SlaViolationInitialResponse"),
								SlaWarningInitialResponse = reader.GetNullableDateTime("SlaWarningInitialResponse")
							};

							unnotifiedAndExpiredSla.Add(ticket);
						}
					}
				}
			}

			return unnotifiedAndExpiredSla;
		}
	}

	public static class ReaderExtensions
	{

		public static DateTime? GetNullableDateTime(this SqlDataReader reader, string name)
		{
			var col = reader.GetOrdinal(name);
			return reader.IsDBNull(col) ?
						(DateTime?)null :
						(DateTime?)reader.GetDateTime(col);
		}
	}
}
