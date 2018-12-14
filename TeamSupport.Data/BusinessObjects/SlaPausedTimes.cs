using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class SlaPausedTimes
    {
        public void LoadByTicketId(int ticketId, DateTime? startOn = null)
        {
            using (SqlCommand command = new SqlCommand())
            {
				string sql = @"SELECT * FROM SlaPausedTimes WITH(NOLOCK) WHERE TicketId = @ticketId AND ResumedOn IS NOT NULL";

				if (startOn != null)
				{
					sql += " AND PausedOn > @startOn";
					command.Parameters.AddWithValue("@startOn", startOn);
				}

				command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ticketId", ticketId);
				
				Fill(command);
            }
        }

        public static TimeSpan CalculatePausedTime(LoginUser loginUser,
                                                    Organization organization,
                                                    SlaTrigger slaTrigger,
                                                    DateTime pausedOn,
                                                    DateTime resumedOn,
                                                    SlaTickets.BusinessHours businessHours,
                                                    List<DateTime> daysToPause,
                                                    CalendarEvents holidays,
                                                    Logs logs = null)
        {
            TimeSpan pausedTime = new TimeSpan();
            int adjustedMinutes = 0;
            bool slaUseBusinessHours = slaTrigger.UseBusinessHours;
            int slaBusinessDays = businessHours.BusinessDays;
            DateTime? slaDayStart = businessHours.DayStartUtc;
            DateTime? slaDayEnd = businessHours.DayEndUtc;

            int startOfDayMinutes = slaDayStart.Value.Minute + (slaDayStart.Value.Hour * 60);
            int endOfDayMinutes = slaDayEnd.Value.Minute + (slaDayEnd.Value.Hour * 60);
            int minutesInOneDay = (24 * 60);

            //When converted the input to UTC the end time might be less than the start time. E.g. central 8 to 22, is stored as utc 14 to 4
            if (businessHours.DayEndUtc.Hour < businessHours.DayStartUtc.Hour && businessHours.DayEndUtc.Day > businessHours.DayStartUtc.Day)
            {
                adjustedMinutes     = slaDayStart.Value.Minute + slaDayStart.Value.Hour * 60;
                slaDayStart         = slaDayStart.Value.AddMinutes(-adjustedMinutes);
                slaDayEnd           = slaDayEnd.Value.AddMinutes(-adjustedMinutes);
                pausedOn            = pausedOn.AddMinutes(-adjustedMinutes);
                resumedOn           = resumedOn.AddMinutes(-adjustedMinutes);
                startOfDayMinutes   = slaDayStart.Value.Minute + (slaDayStart.Value.Hour * 60);
                endOfDayMinutes     = slaDayEnd.Value.Minute + (slaDayEnd.Value.Hour * 60);
            }

            if (pausedOn.Date == resumedOn.Date && (slaUseBusinessHours || (!slaUseBusinessHours && !slaTrigger.NoBusinessHours)) //the last condition means it is using sla custom hours
                && (
                    (!SlaTickets.IsValidDay(pausedOn, slaBusinessDays, daysToPause, holidays) && !SlaTickets.IsValidDay(resumedOn, slaBusinessDays, daysToPause, holidays))
                    || (pausedOn.TimeOfDay.TotalSeconds < slaDayStart.Value.TimeOfDay.TotalSeconds && resumedOn.TimeOfDay.TotalSeconds < slaDayStart.Value.TimeOfDay.TotalSeconds)
                    || (pausedOn.TimeOfDay.TotalSeconds > slaDayEnd.Value.TimeOfDay.TotalSeconds && resumedOn.TimeOfDay.TotalSeconds > slaDayEnd.Value.TimeOfDay.TotalSeconds)
                    )
                )
            {
                if (logs != null)
                {
                    logs.WriteEvent("Paused and Resumed on the same non-valid day and time (non-business day, holiday, day to pause, outside business hours), so no time to add.");
                }
            }
            else if (pausedOn.Date == resumedOn.Date && (slaUseBusinessHours || (!slaUseBusinessHours && !slaTrigger.NoBusinessHours)) //the last condition means it is using sla custom hours
                && (
                    (SlaTickets.IsValidDay(pausedOn, slaBusinessDays, daysToPause, holidays) && SlaTickets.IsValidDay(resumedOn, slaBusinessDays, daysToPause, holidays))
                    && (pausedOn.TimeOfDay.TotalSeconds > slaDayStart.Value.TimeOfDay.TotalSeconds && pausedOn.TimeOfDay.TotalSeconds < slaDayEnd.Value.TimeOfDay.TotalSeconds)
                    && (resumedOn.TimeOfDay.TotalSeconds > slaDayStart.Value.TimeOfDay.TotalSeconds && resumedOn.TimeOfDay.TotalSeconds < slaDayEnd.Value.TimeOfDay.TotalSeconds)
                    )
                )
            {
                pausedTime = resumedOn - pausedOn;
                if (logs != null)
                {
                    logs.WriteEvent("Paused and Resumed on the same valid day and time (business day, non-holiday, not day to pause, inside business hours), simple resumedon - pausedon.");
                }
            }
            else
            {
                if ((slaUseBusinessHours && slaBusinessDays == 0)
                    || (!slaUseBusinessHours && DateTime.Compare(slaDayStart.Value, slaDayEnd.Value) == 0 && slaBusinessDays == 127) //127 means all days are selected.
                    || slaTrigger.NoBusinessHours)
                {
                    pausedTime = resumedOn - pausedOn;
                    logs.WriteEvent("24/7, simple resumedon - pausedon.");
                }
                else
                {
                    //Make sure endOfDayMinutes is greater than startOfDayMinutes
                    if (startOfDayMinutes >= endOfDayMinutes)
                    {
                        //Add 1 day worth of minutes
                        endOfDayMinutes = endOfDayMinutes + minutesInOneDay;
                    }

                    int minutesInBusinessDay = endOfDayMinutes - startOfDayMinutes;

                    //Make sure the pausedon and resumedon are business days
                    while (!SlaTickets.IsValidDay(pausedOn, slaBusinessDays, daysToPause, holidays))
                    {
                        pausedOn = SlaTickets.GetNextBusinessDay(pausedOn, slaBusinessDays);
                    }

                    while (!SlaTickets.IsValidDay(resumedOn, slaBusinessDays, daysToPause, holidays))
                    {
                        resumedOn = SlaTickets.GetNextBusinessDay(resumedOn, slaBusinessDays);
                    }

                    //If the pause spans to more than one (and same) days then loop, set a tempResumedOn to end of business days and moving the pausedOn to next business day start of day
                    while (DateTime.Compare(pausedOn, resumedOn) < 0)
                    {
                        DateTime tempResumedOn = new DateTime();

                        if (DateTime.Compare(pausedOn.Date, resumedOn.Date) < 0)
                        {
                            tempResumedOn = new DateTime(pausedOn.Year, pausedOn.Month, pausedOn.Day, slaDayEnd.Value.Hour, slaDayEnd.Value.Minute, 0);
                        }
                        else if (DateTime.Compare(pausedOn.Date, resumedOn.Date) == 0)
                        {
                            tempResumedOn = resumedOn;
                        }

                        //...do the calculation
                        int secondsPaused = 0;

                        while (pausedOn.Date < tempResumedOn.Date)
                        {
                            int pausedOnSecond = (pausedOn.Minute + (pausedOn.Hour * 60)) * 60;

                            if (pausedOnSecond < (startOfDayMinutes * 60) || pausedOnSecond > (endOfDayMinutes * 60))
                            {
                                pausedOnSecond = startOfDayMinutes * 60;
                            }

                            secondsPaused = (endOfDayMinutes * 60) - pausedOnSecond;
                            pausedTime = pausedTime.Add(TimeSpan.FromSeconds(secondsPaused));
                            pausedOn = SlaTickets.GetNextBusinessDay(pausedOn, slaBusinessDays);
                            pausedOn = new DateTime(pausedOn.Year, pausedOn.Month, pausedOn.Day, slaDayStart.Value.Hour, slaDayStart.Value.Minute, 0);
                        }

                        //same day
                        if (pausedOn.Date == tempResumedOn.Date && pausedOn.TimeOfDay < tempResumedOn.TimeOfDay)
                        {
                            int resumedOnMinute = tempResumedOn.Minute + (tempResumedOn.Hour * 60);
                            int resumedOnSecond = resumedOnSecond = (resumedOnMinute * 60) + tempResumedOn.Second;

                            if (resumedOnMinute < startOfDayMinutes)
                            {
                                resumedOnMinute = startOfDayMinutes;
                                resumedOnSecond = resumedOnSecond = (resumedOnMinute * 60) + tempResumedOn.Second;
                            }

                            if (resumedOnMinute > endOfDayMinutes)
                            {
                                resumedOnMinute = endOfDayMinutes;
                                resumedOnSecond = resumedOnSecond = (resumedOnMinute * 60) + tempResumedOn.Second;
                            }

                            if (resumedOnSecond < (startOfDayMinutes * 60))
                            {
                                resumedOnSecond = (startOfDayMinutes * 60);
                            }

                            if (resumedOnSecond > (endOfDayMinutes * 60))
                            {
                                resumedOnSecond = (endOfDayMinutes * 60);
                            }

                            secondsPaused = resumedOnSecond - ((((pausedOn.Hour * 60) + pausedOn.Minute) * 60) + pausedOn.Second);
                            pausedTime = pausedTime.Add(TimeSpan.FromSeconds(secondsPaused));
                        }

                        //get the next valid day to start
                        pausedOn = SlaTickets.GetNextBusinessDay(pausedOn, slaBusinessDays);
                        pausedOn = new DateTime(pausedOn.Year, pausedOn.Month, pausedOn.Day, slaDayStart.Value.Hour, slaDayStart.Value.Minute, 0);

                        while (!SlaTickets.IsValidDay(pausedOn, slaBusinessDays, daysToPause, holidays))
                        {
                            pausedOn = SlaTickets.GetNextBusinessDay(pausedOn, slaBusinessDays);
                            pausedOn = new DateTime(pausedOn.Year, pausedOn.Month, pausedOn.Day, slaDayStart.Value.Hour, slaDayStart.Value.Minute, 0);
                        }
                    }
                }
            }

            return pausedTime;
        }

        public static TimeSpan CalculatePausedTime(LoginUser loginUser, int organizationId, int triggerId, DateTime pausedOn, DateTime resumedOn, Logs logs = null)
        {
            SlaTrigger slaTrigger = SlaTriggers.GetSlaTrigger(loginUser, triggerId);
            Organization organization = Organizations.GetOrganization(loginUser, organizationId);

            SlaTickets.BusinessHours businessHours = new SlaTickets.BusinessHours()
            {
                DayStartUtc = organization.BusinessDayStartUtc,
                DayEndUtc = organization.BusinessDayEndUtc,
                BusinessDays = organization.BusinessDays
            };

            //Check if we should use SLA's business hours instead of Account's
            if (!slaTrigger.UseBusinessHours
                && slaTrigger.DayStartUtc.HasValue
                && slaTrigger.DayEndUtc.HasValue)
            {
                businessHours.DayStartUtc = slaTrigger.DayStartUtc.Value;
                businessHours.DayEndUtc = slaTrigger.DayEndUtc.Value;
                businessHours.BusinessDays = slaTrigger.Weekdays;
            }

            List<DateTime> daysToPause = SlaTriggers.GetSpecificDaysToPause(slaTrigger.SlaTriggerID);
            bool pauseOnHoliday = slaTrigger.PauseOnHoliday;
            CalendarEvents holidays = new CalendarEvents(loginUser);

            if (pauseOnHoliday)
            {
                holidays.LoadHolidays(organization.OrganizationID);
            }

            return CalculatePausedTime(loginUser, organization, slaTrigger, pausedOn, resumedOn, businessHours, daysToPause, holidays, logs);
        }

        public static void UpdateBusinessPausedTime(LoginUser loginUser, int id, double value)
        {
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE [dbo].[SlaPausedTimes] SET BusinessPausedTime = @Value WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Value", value);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}
