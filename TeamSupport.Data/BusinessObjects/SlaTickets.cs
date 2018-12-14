using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace TeamSupport.Data
{
    public partial class SlaTickets
    {
        public void LoadPending()
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT * FROM SlaTickets WHERE IsPending = 1 ORDER BY DateModified";
                command.CommandType = CommandType.Text;
                Fill(command);
            }
        }

        public static DateTime? CalculateSLA(DateTime DateCreated,
                                            BusinessHours businessHours,
                                            SlaTrigger slaTrigger,
                                            int minutes,
                                            TimeSpan pausedTimeSpan,
                                            List<DateTime> daysToPause,
                                            CalendarEvents holidays)
        {
            bool slaUseBusinessHours = slaTrigger.UseBusinessHours;
            DateTime? slaDayStart = businessHours.DayStartUtc;
            DateTime? slaDayEnd = businessHours.DayEndUtc;
            int slaBusinessDays = businessHours.BusinessDays;
            DateTime? ExpireDate = new DateTime();
            int adjustedMinutes = 0;

            if ((slaUseBusinessHours && (slaDayStart == null || slaDayEnd == null || slaBusinessDays < 1))
                || minutes < 1)
            {
                ExpireDate = null;
            }
            else
            {
                if ((slaUseBusinessHours && slaBusinessDays == 0)
                    || (!slaUseBusinessHours && DateTime.Compare(slaDayStart.Value, slaDayEnd.Value) == 0 && slaBusinessDays == 127)
                    || slaTrigger.NoBusinessHours) //127 means all days are selected.
                {
                    ExpireDate = DateCreated.AddMinutes(minutes);
                }
                else
                {
                    int startOfDayMinutes = slaDayStart.Value.Minute + (slaDayStart.Value.Hour * 60);
                    int endOfDayMinutes = slaDayEnd.Value.Minute + (slaDayEnd.Value.Hour * 60);
                    int currentMinuteInTheProcess = DateCreated.Minute + (DateCreated.Hour * 60);
                    int minutesInOneDay = (24 * 60);

                    //When converted the input to UTC the end time might be less than the start time. E.g. central 8 to 22, is stored as utc 14 to 4
                    if (businessHours.DayEndUtc.Hour < businessHours.DayStartUtc.Hour && businessHours.DayEndUtc.Day > businessHours.DayStartUtc.Day)
                    {
                        adjustedMinutes = slaDayStart.Value.Minute + slaDayStart.Value.Hour * 60;
                        slaDayStart = slaDayStart.Value.AddMinutes(-adjustedMinutes);
                        slaDayEnd = slaDayEnd.Value.AddMinutes(-adjustedMinutes);
                        DateCreated = DateCreated.AddMinutes(-adjustedMinutes);
                        startOfDayMinutes = slaDayStart.Value.Minute + (slaDayStart.Value.Hour * 60);
                        endOfDayMinutes = slaDayEnd.Value.Minute + (slaDayEnd.Value.Hour * 60);
                        currentMinuteInTheProcess = (DateCreated.Minute + (DateCreated.Hour * 60));
                    }

                    //Make sure endOfDayMinutes is greater than startOfDayMinutes
                    if (startOfDayMinutes >= endOfDayMinutes)
                    {
                        //Add 1 day worth of minutes
                        endOfDayMinutes = endOfDayMinutes + minutesInOneDay;
                    }

                    int minutesInBusinessDay = endOfDayMinutes - startOfDayMinutes;

                    //Make sure the start time falls within business hours
                    if (currentMinuteInTheProcess > endOfDayMinutes)
                    {
                        //Reset the time to start of bussiness day AND add 1 day
                        DateCreated = DateCreated.Date.AddDays(1).Add(slaDayStart.Value.TimeOfDay);
                    }
                    else if (currentMinuteInTheProcess < startOfDayMinutes)
                    {
                        DateCreated = DateCreated.Date.Add(slaDayStart.Value.TimeOfDay);
                    }

                    //Repeat until we find the first business day, non-pause day, non-holiday
                    while (!IsValidDay(DateCreated, slaBusinessDays, daysToPause, holidays))
                    {
                        DateCreated = DateCreated.AddDays(1);
                        //If this happened then we need to set it with the start of day hour
                        DateCreated = new DateTime(DateCreated.Year, DateCreated.Month, DateCreated.Day, slaDayStart.Value.Hour, slaDayStart.Value.Minute, 0);
                    }

                    //DateCreated now contains a valid date to start SLA with business days
                    currentMinuteInTheProcess = DateCreated.Minute + (DateCreated.Hour * 60);

                    int slaDays = (minutes / 60) / 24;
                    int slaHours = (minutes - (slaDays * 24 * 60)) / 60;
                    int slaMinutes = minutes - (slaDays * 24 * 60) - (slaHours * 60);

                    ExpireDate = DateCreated;
                    //1) process days
                    while (slaDays > 0)
                    {
                        ExpireDate = ExpireDate.Value.AddDays(1);

                        if (IsValidDay(ExpireDate.Value, slaBusinessDays, daysToPause, holidays))
                        {
                            slaDays--;
                        }
                    }

                    //2) process hours
                    while (slaHours > 0)
                    {
                        ExpireDate = ExpireDate.Value.AddHours(1);

                        if (ExpireDate.Value.Hour > slaDayEnd.Value.Hour || (ExpireDate.Value.Hour == slaDayEnd.Value.Hour && ExpireDate.Value.Minute > slaDayEnd.Value.Minute))
                        {
                            ExpireDate = GetNextBusinessDay(ExpireDate.Value, slaBusinessDays);
                            int minuteOffset = ExpireDate.Value.Minute - slaDayEnd.Value.Minute;
                            ExpireDate = new DateTime(ExpireDate.Value.Year, ExpireDate.Value.Month, ExpireDate.Value.Day, slaDayStart.Value.Hour, slaDayStart.Value.Minute, 0);
                            ExpireDate = ExpireDate.Value.AddMinutes(minuteOffset);
                        }

                        if (IsValidDay(ExpireDate.Value, slaBusinessDays, daysToPause, holidays))
                        {
                            slaHours--;
                        }
                        else
                        {
                            ExpireDate = GetNextBusinessDay(ExpireDate.Value, slaBusinessDays);
                            ExpireDate = ExpireDate.Value.AddHours(-1);
                        }
                    }

                    //3) process minutes
                    while (slaMinutes > 0)
                    {
                        ExpireDate = ExpireDate.Value.AddMinutes(1);

                        if (ExpireDate.Value.Hour == slaDayEnd.Value.Hour && ExpireDate.Value.Minute > slaDayEnd.Value.Minute)
                        {
                            ExpireDate = GetNextBusinessDay(ExpireDate.Value, slaBusinessDays);
                            ExpireDate = new DateTime(ExpireDate.Value.Year, ExpireDate.Value.Month, ExpireDate.Value.Day, slaDayStart.Value.Hour, slaDayStart.Value.Minute, 0);
                        }

                        if (IsValidDay(ExpireDate.Value, slaBusinessDays, daysToPause, holidays))
                        {
                            slaMinutes--;
                        }
                        else
                        {
                            ExpireDate = GetNextBusinessDay(ExpireDate.Value, slaBusinessDays);
                            ExpireDate = ExpireDate.Value.AddMinutes(-1);
                        }
                    }
                }

                ExpireDate = AddPausedTime((DateTime)ExpireDate, pausedTimeSpan, slaBusinessDays, slaDayStart, slaDayEnd, slaUseBusinessHours);

                if (businessHours.DayEndUtc.Hour < businessHours.DayStartUtc.Hour && businessHours.DayEndUtc.Day > businessHours.DayStartUtc.Day)
                {
                    ExpireDate = ExpireDate.Value.AddMinutes(adjustedMinutes);
                }

				//last check to make sure it didn't fall into a Holiday, if so move onto the next valid day
				while (!IsValidDay(ExpireDate.Value, slaBusinessDays, daysToPause, holidays))
				{
					ExpireDate = GetNextBusinessDay(ExpireDate.Value, slaBusinessDays);
				}
			}

            return ExpireDate;
        }

        public static TimeSpan CalculatePausedTime(int ticketId,
                                            Organization organization,
                                            BusinessHours businessHours,
                                            SlaTrigger slaTrigger,
                                            List<DateTime> daysToPause,
                                            CalendarEvents holidays,
                                            LoginUser loginUser,
                                            Dictionary<int, double> businessPausedTimes,
                                            Logs logs = null,
                                            bool overwritePrevious = false,
											DateTime? startOn = null)
        {
            TimeSpan totalPausedTime = new TimeSpan();
            SlaPausedTimes slaPausedTimes = new SlaPausedTimes(loginUser);
            slaPausedTimes.LoadByTicketId(ticketId, startOn);

            foreach (SlaPausedTime slaPausedTime in slaPausedTimes)
            {
                bool hasBeenCalculated = slaPausedTime.BusinessPausedTime != null;

                if (!hasBeenCalculated || (hasBeenCalculated && overwritePrevious))
                {
                    TimeSpan rangePausedTime = SlaPausedTimes.CalculatePausedTime(loginUser,
                                                                            organization,
                                                                            slaTrigger,
                                                                            slaPausedTime.PausedOnUtc,
                                                                            (DateTime)slaPausedTime.ResumedOnUtc,
                                                                            businessHours,
                                                                            daysToPause,
                                                                            holidays,
                                                                            logs);
                    totalPausedTime = totalPausedTime.Add(rangePausedTime);
                    
                    //return the results to stored them in the db.
                    businessPausedTimes.Add(slaPausedTime.Id, rangePausedTime.TotalSeconds);
                }
                else if (hasBeenCalculated && !overwritePrevious)
                {
                    TimeSpan previouslyCalculatedPausedTime = TimeSpan.FromSeconds((int)slaPausedTime.BusinessPausedTime);
                    totalPausedTime = totalPausedTime.Add(previouslyCalculatedPausedTime);
                }
            }

            return totalPausedTime;
        }

        public static DateTime? CalculateSLAWarning(DateTime ViolationDate,
                                            BusinessHours businessHours,
                                            bool noBusinessHours,
                                            int minutes,
                                            int slaWarningTime,
                                            List<DateTime> daysToPause,
                                            CalendarEvents holidays)
        {
            DateTime? slaDayStart = businessHours.DayStartUtc;
            DateTime? slaDayEnd = businessHours.DayEndUtc;
            int slaBusinessDays = businessHours.BusinessDays;
            DateTime? ExpireDate = new DateTime();
            int adjustedMinutes = 0;

            if (slaDayStart == null || slaDayEnd == null || slaBusinessDays < 1 || minutes < 1)
            {
                ExpireDate = null;
            }
            else if (noBusinessHours)
            {
                ExpireDate = ViolationDate.AddMinutes(-slaWarningTime);
            }
            else
            {
                //The Violation needs to be calculated first, which means the start date is already valid.
                int startOfDayMinutes = slaDayStart.Value.Minute + (slaDayStart.Value.Hour * 60);

                //When converted the input to UTC the end time might be less than the start time. E.g. central 8 to 22, is stored as utc 14 to 4
                if (businessHours.DayEndUtc.Hour < businessHours.DayStartUtc.Hour && businessHours.DayEndUtc.Day > businessHours.DayStartUtc.Day)
                {
                    adjustedMinutes = slaDayStart.Value.Minute + slaDayStart.Value.Hour * 60;
                    slaDayStart = slaDayStart.Value.AddMinutes(-adjustedMinutes);
                    slaDayEnd = slaDayEnd.Value.AddMinutes(-adjustedMinutes);
                    ViolationDate = ViolationDate.AddMinutes(-adjustedMinutes);
                    startOfDayMinutes = slaDayStart.Value.Minute + (slaDayStart.Value.Hour * 60);
                }

                int slaDays = (slaWarningTime / 60) / 24;
                int slaHours = (slaWarningTime - (slaDays * 24 * 60)) / 60;
                int slaMinutes = slaWarningTime - (slaDays * 24 * 60) - (slaHours * 60);
                ExpireDate = ViolationDate;

                //1) process days
                while (slaDays > 0)
                {
                    ExpireDate = ExpireDate.Value.AddDays(-1);

                    if (IsValidDay(ExpireDate.Value, slaBusinessDays, daysToPause, holidays))
                    {
                        slaDays--;
                    }
                }

                //2) process hours
                while (slaHours > 0)
                {
                    ExpireDate = ExpireDate.Value.AddHours(-1);

                    if (ExpireDate.Value.Hour < slaDayStart.Value.Hour)
                    {
                        ExpireDate = GetPreviousBusinessDay(ExpireDate.Value, slaBusinessDays);
                        TimeSpan difference = (new DateTime(ExpireDate.Value.Year, ExpireDate.Value.Month, ExpireDate.Value.Day, slaDayStart.Value.Hour, slaDayStart.Value.Minute, 0)) - (DateTime)ExpireDate;
                        ExpireDate = new DateTime(ExpireDate.Value.Year, ExpireDate.Value.Month, ExpireDate.Value.Day, slaDayEnd.Value.Hour, slaDayEnd.Value.Minute, 0);
                        ExpireDate = ExpireDate.Value.AddHours(-1 * difference.Hours).AddMinutes(-1 * difference.Minutes).AddSeconds(-1 * difference.Seconds);
                    }

                    if (IsValidDay(ExpireDate.Value, slaBusinessDays, daysToPause, holidays))
                    {
                        slaHours--;
                    }
                }

                //3) process minutes
                while (slaMinutes > 0)
                {
                    DateTime control = ExpireDate.Value;
                    ExpireDate = ExpireDate.Value.AddMinutes(-1);

                    if (!ExpireDate.Value.Date.Equals(control.Date))
                    {
                        ExpireDate = new DateTime(ExpireDate.Value.Year, ExpireDate.Value.Month, ExpireDate.Value.Day, slaDayEnd.Value.Hour, slaDayEnd.Value.Minute, 0);
                        ExpireDate = ExpireDate.Value.AddMinutes(-1);

                        if (!IsValidDay(ExpireDate.Value, slaBusinessDays, daysToPause, holidays))
                        {
                            ExpireDate = GetPreviousBusinessDay(ExpireDate.Value, slaBusinessDays);
                        }
                    }

                    if (ExpireDate.Value.Hour < slaDayStart.Value.Hour)
                    {
                        ExpireDate = GetPreviousBusinessDay(ExpireDate.Value, slaBusinessDays);
                        TimeSpan difference = (new DateTime(ExpireDate.Value.Year, ExpireDate.Value.Month, ExpireDate.Value.Day, slaDayStart.Value.Hour, slaDayStart.Value.Minute, 0)) - (DateTime)ExpireDate;
                        ExpireDate = new DateTime(ExpireDate.Value.Year, ExpireDate.Value.Month, ExpireDate.Value.Day, slaDayEnd.Value.Hour, slaDayEnd.Value.Minute, 0);
                        ExpireDate = ExpireDate.Value.AddMinutes(-1 * difference.Minutes).AddSeconds(-1 * difference.Seconds);
                    }

                    if (IsValidDay(ExpireDate.Value, slaBusinessDays, daysToPause, holidays))
                    {
                        slaMinutes--;
                    }
                }

				//And only if the ExpireDate falls outside business hours, due to start > end business hours because of the UTC
				if (businessHours.DayEndUtc.Hour < businessHours.DayStartUtc.Hour && businessHours.DayEndUtc.Day > businessHours.DayStartUtc.Day
					&& ExpireDate.Value.Hour < businessHours.DayStartUtc.Hour)
				{
					ExpireDate = ExpireDate.Value.AddMinutes(adjustedMinutes);
				}

				//last check to make sure it didn't fall into a Holiday, if so move onto the next valid day
				while (!IsValidDay(ExpireDate.Value, slaBusinessDays, daysToPause, holidays))
				{
					ExpireDate = GetPreviousBusinessDay(ExpireDate.Value, slaBusinessDays);
				}
			}

            return ExpireDate;
        }

        public static DateTime GetNextBusinessDay(DateTime inputDate, int organizationBusinessDays)
        {
            DateTime businessDay = inputDate;

            businessDay = businessDay.AddDays(1);

            while ((organizationBusinessDays & (int)Math.Pow(2, (double)businessDay.DayOfWeek)) != Math.Pow(2, (double)businessDay.DayOfWeek))
            {
                businessDay = businessDay.AddDays(1);
            }

            return businessDay;
        }

        public static DateTime GetPreviousBusinessDay(DateTime inputDate, int organizationBusinessDays)
        {
            DateTime businessDay = inputDate;

            businessDay = businessDay.AddDays(-1);

            while ((organizationBusinessDays & (int)Math.Pow(2, (double)businessDay.DayOfWeek)) != Math.Pow(2, (double)businessDay.DayOfWeek))
            {
                businessDay = businessDay.AddDays(-1);
            }

            return businessDay;
        }

        public static DateTime AddPausedTime(DateTime inputDate,
                                            TimeSpan pausedTimeSpan,
                                            int organizationBusinessDays,
                                            DateTime? organizationBusinessDayStart,
                                            DateTime? organizationBusinessDayEnd,
                                            bool slaUseBusinessHours)
        {
            DateTime slaWithPausedTime = new DateTime();
            int startOfDayMinutes = organizationBusinessDayStart.Value.Minute + (organizationBusinessDayStart.Value.Hour * 60);
            int endOfDayMinutes = organizationBusinessDayEnd.Value.Minute + (organizationBusinessDayEnd.Value.Hour * 60);
            int minutesInOneDay = (24 * 60);

            //Make sure endOfDayMinutes is greater than startOfDayMinutes
            if (startOfDayMinutes >= endOfDayMinutes)
            {
                //Add 1 day worth of minutes
                endOfDayMinutes = endOfDayMinutes + minutesInOneDay;
            }

            int minutesInBusinessDay = endOfDayMinutes - startOfDayMinutes;

            if (slaUseBusinessHours)
            {
                slaWithPausedTime = inputDate;
                //get full days first
                int fullDays = (int)(pausedTimeSpan.TotalMinutes / minutesInBusinessDay);
                pausedTimeSpan = pausedTimeSpan.Subtract(TimeSpan.FromMinutes(fullDays * minutesInBusinessDay));

                while (pausedTimeSpan.TotalMinutes > 0)
                {
                    if ((slaWithPausedTime.Minute + (slaWithPausedTime.Hour * 60)) + pausedTimeSpan.TotalMinutes <= endOfDayMinutes)
                    {
                        slaWithPausedTime = slaWithPausedTime.AddMinutes(pausedTimeSpan.TotalMinutes);
                        pausedTimeSpan = pausedTimeSpan.Subtract(TimeSpan.FromMinutes(pausedTimeSpan.TotalMinutes));
                    }
                    else
                    {
                        int addMinutes = endOfDayMinutes - (slaWithPausedTime.Minute + (slaWithPausedTime.Hour * 60));

                        if (slaWithPausedTime.TimeOfDay == organizationBusinessDayStart.Value.TimeOfDay)
                        {
                            addMinutes = (int)pausedTimeSpan.TotalMinutes;
                        }

                        slaWithPausedTime = slaWithPausedTime.AddMinutes(addMinutes);
                        pausedTimeSpan = pausedTimeSpan.Subtract(TimeSpan.FromMinutes(addMinutes));

                        if (pausedTimeSpan.TotalMinutes > 0)
                        {
                            slaWithPausedTime = GetNextBusinessDay(slaWithPausedTime, organizationBusinessDays);
                            slaWithPausedTime = slaWithPausedTime.Date + organizationBusinessDayStart.Value.TimeOfDay;
                        }
                    }
                }

                for (int i = 0; i < fullDays; i++)
                {
                    do
                    {
                        slaWithPausedTime = slaWithPausedTime.AddDays(1);
                    } while (!IsBusinessDay(slaWithPausedTime, organizationBusinessDays));
                }
            }
            else
            {
                slaWithPausedTime = inputDate.Add(pausedTimeSpan);
            }

            return slaWithPausedTime;
        }

        public static bool IsValidDay(DateTime day, int businessDays, List<DateTime> daysToPause, CalendarEvents holidays)
        {
            bool IsValid = false;

            //Checks:
            /*
			 1) Use business days to check: org business or trigger business
			 2) Skip PausedOn days
			 3) Skip Holidays
			 */

            //1
            IsValid = IsBusinessDay(day, businessDays);

            //2
            if (IsValid && daysToPause != null && daysToPause.Where(p => p.Date.CompareTo(day.Date) == 0).Any())
            {
                IsValid = false;
            }

            //3
            try
            {
				//holidays might only have items if the sla is set to pause on holidays. See Run() 
				if (IsValid && holidays != null && holidays.Where(p => p.StartDateUTCUtc.Value.Date.CompareTo(day.Date) == 0 || p.EndDateUTCUtc.Value.Date.CompareTo(day.Date) == 0).Any())
				{
                    IsValid = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.AddLog(LoginUser.Anonymous, "SlaProcessor", ex.Message, "IsValidDay", ex.StackTrace, "", "");
            }

            return IsValid;
        }

        public static bool IsBusinessDay(DateTime inputDate, int organizationBusinessDays)
        {
            bool isBusinessDay = false;

            isBusinessDay = (organizationBusinessDays & (int)Math.Pow(2, (double)inputDate.DayOfWeek)) == Math.Pow(2, (double)inputDate.DayOfWeek);

            return isBusinessDay;
        }

        public class BusinessHours
        {
            public DateTime DayStartUtc { get; set; }
            public DateTime DayEndUtc { get; set; }
            public int BusinessDays { get; set; }
        }
    }
}
