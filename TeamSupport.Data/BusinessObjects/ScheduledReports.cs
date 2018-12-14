using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
	public partial class ScheduledReport
	{
		public string Creator
		{
			get
			{
				if (Row.Table.Columns.Contains("Creator") && Row["Creator"] != DBNull.Value)
				{
					return (string)Row["Creator"];
				}
				else return "";
			}
		}

		public string Modifier
		{
			get
			{
				if (Row.Table.Columns.Contains("Modifier") && Row["Modifier"] != DBNull.Value)
				{
					return (string)Row["Modifier"];
				}
				else return "";
			}
		}

		public string ReportName
		{
			get
			{
				if (Row.Table.Columns.Contains("ReportName") && Row["ReportName"] != DBNull.Value)
				{
					return (string)Row["ReportName"];
				}
				else return "";
			}
		}

		public void SetNextRun(bool isServiceCall = false)
		{
			DateTime dateOnly = StartDateUtc.Date;
			DateTime timeOnly = default(DateTime).Add(StartDateUtc.TimeOfDay);
			DayOfWeek dayOfWeek = DayOfWeek.Sunday;
            int dayDiff = 0;
            User creator = Users.GetUser(LoginUser.Anonymous, CreatorId);
            TimeZoneInfo tz = TimeZoneInfo.Local;

            if (!string.IsNullOrWhiteSpace(creator.TimeZoneID))
            {
                tz = TimeZoneInfo.FindSystemTimeZoneById(creator.TimeZoneID);
                Debug(string.Format("CreatorId: {0} TimeZoneID: {1} tz.DisplayName: {2}", CreatorId.ToString(), creator.TimeZoneID.ToString(), tz.DisplayName));
            }

            DateTime StartDateToCreatorTimeZone = TimeZoneInfo.ConvertTimeFromUtc(StartDateUtc, tz);
            DateTime nextRunToCreatorTimeZone = NextRunUtc != null ? TimeZoneInfo.ConvertTimeFromUtc(NextRunUtc.Value, tz) : TimeZoneInfo.ConvertTimeFromUtc(StartDateUtc, tz);

            // The initialDayDiff should not be calculated against our local time but the user's local time.
            int initialDayDiff = StartDateUtc.DayOfWeek - StartDateToCreatorTimeZone.DayOfWeek;

            if (LastRunUtc != null)
			{
				DateTime lastRunToCreatorTimeZone = TimeZoneInfo.ConvertTimeFromUtc(LastRunUtc.Value, tz);
				dateOnly = StartDateUtc > LastRunUtc ? StartDateUtc.Date : ((DateTime)LastRunUtc).Date;
                initialDayDiff = ((DateTime)LastRunUtc).DayOfWeek - lastRunToCreatorTimeZone.DayOfWeek;

                //Difference in day (due to the UTC) between Sund-Sat or Sat-Sun will be handled here because the substraction will return 6 or -6, we only need to know if it's 1 or -1 (day ahead or day behind)
                if (initialDayDiff == -6) //Sun (0) back to Sat (6)
                {
                    initialDayDiff = 1;
                }
                else if (initialDayDiff == 6) //Sat (6) onto Sun (0)
                {
                    initialDayDiff = -1;
                }
            }

			switch ((ScheduledReportFrequency)RecurrencyId)
			{
				case ScheduledReportFrequency.Once:
					NextRun = StartDateUtc;
					break;
				case ScheduledReportFrequency.Weekly:
                    //we need: startdate, every, weekday (1:Sun, ..., 7:Sat)
                    //The list in the UI is: 1: Sunday, ..., 7: Saturday. So we need to substract 1 to convert it to DayOfWeek
                    dayOfWeek = (DayOfWeek)(byte)Weekday - 1;

                    if (dateOnly.Add(timeOnly.TimeOfDay) > DateTime.UtcNow)
                    {
                        while (dateOnly.DayOfWeek != dayOfWeek)
                        {
                            dateOnly = dateOnly.AddDays(1);
                        }
                    }
                    else
                    {
                        int totalDaysInAWeek = 7;
                        int totalDays = (byte)Every * totalDaysInAWeek;

                        while (dateOnly < DateTime.UtcNow)
                        {
                            dateOnly = dateOnly.AddDays(totalDays);

                            if (dateOnly.DayOfWeek != dayOfWeek)
                            {
                                dateOnly = dateOnly.AddDays(-totalDaysInAWeek);
                                dateOnly = dateOnly.AddDays(dayOfWeek - dateOnly.DayOfWeek);
                            }
                        }
                    }

                    NextRun = dateOnly.Add(timeOnly.TimeOfDay).AddDays(initialDayDiff);
                    nextRunToCreatorTimeZone = TimeZoneInfo.ConvertTimeFromUtc(NextRunUtc.Value, tz);

                    // The day of the week check must be done on the creator timezone as well.
                    if (nextRunToCreatorTimeZone.DayOfWeek != dayOfWeek)
                    {
                        dayDiff = dayOfWeek - nextRunToCreatorTimeZone.DayOfWeek;
                        NextRun = dateOnly.AddDays(dayDiff).Add(timeOnly.TimeOfDay);
                    }

                    break;
				case ScheduledReportFrequency.Monthly:
                    //we need: startdate, every, weekday (1:Sun, ..., 7:Sat), 
                    //				monthday (if < 5 then weekday can have a value: the 1st monday.. the 3rd wednesday, etc;
                    //						else weekday has to be null: the 5th of the month, the 20th of the month, etc)

                    DateTime startOfTheMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

					while (dateOnly < DateTime.UtcNow)
					{
						if (Monthday < 5 && Weekday != null && Weekday > 0)
						{
							int totalDaysInAWeek = 7;
							dayOfWeek = (DayOfWeek)(byte)Weekday - 1;
							startOfTheMonth = new DateTime(dateOnly.Year, dateOnly.Month, 1);
							startOfTheMonth = startOfTheMonth.AddMonths((byte)Every);

							// set the first ocurrence of the monthday
							int diff = dayOfWeek - startOfTheMonth.DayOfWeek;

							if (diff < 0)
							{
								startOfTheMonth = startOfTheMonth.AddDays(totalDaysInAWeek + diff);
								diff = 0;
							}

                            dateOnly = startOfTheMonth.AddDays(diff + (((byte)Monthday - 1) * totalDaysInAWeek));
						}
						else
						{
							Weekday = null;

                            if (startOfTheMonth.AddDays((byte)Monthday - 1).Add(timeOnly.TimeOfDay) < new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0).Add(timeOnly.TimeOfDay) || isServiceCall)
                            {
                                dateOnly = startOfTheMonth.AddMonths((byte)Every);
							}
                            else
                            {
                                dateOnly = startOfTheMonth;
							}

                            int monthTemp = dateOnly.Month;
                            dateOnly = dateOnly.AddDays((byte)Monthday - 1); //Calculation starts on first of the month, so substract it.

							if (dateOnly.Month > monthTemp)
                            {
                                dateOnly = dateOnly.AddDays(-dateOnly.Day);
							}
                        }
					}

					Debug("Exited while. Monthly Next Run calculation. DateOnly: " + dateOnly.ToString());

					NextRun = dateOnly.Add(timeOnly.TimeOfDay);

                    if (Monthday < 5 && Weekday != null && Weekday > 0 && NextRun.Value.DayOfWeek != dayOfWeek)
                    {
                        dayDiff = dayOfWeek - NextRun.Value.DayOfWeek;
                        NextRun = dateOnly.AddDays(dayDiff).Add(timeOnly.TimeOfDay);
                    }

                    dayDiff = (int)(dateOnly.Date - NextRun.Value.Date).TotalDays;

                    if (dayDiff != 0)
                    {
                        NextRun = dateOnly.AddDays(dayDiff).Add(timeOnly.TimeOfDay);
                    }

                    if (NextRunUtc.Value.DayOfYear < NextRun.Value.DayOfYear)
                    {
                        NextRun = NextRun.Value.AddDays(1);
                    }

                    break;
                case ScheduledReportFrequency.Daily:
                    DateTime now = DateTime.UtcNow;
					Debug(string.Format("now : {0}", now.ToString("MM/dd/yyyy HH:mm")));

					if (dateOnly.Add(timeOnly.TimeOfDay) < now)
                    {
                        dateOnly = now.Date;
						Debug("dateOnly = now.Date : " + dateOnly.ToString("MM/dd/yyyy HH:mm"));
					}

                    if (dateOnly.Add(timeOnly.TimeOfDay) < now || isServiceCall)
                    {
                        NextRun = dateOnly.AddDays(1).Add(timeOnly.TimeOfDay);
						Debug(string.Format("NextRun = dateOnly.AddDays(1).Add(timeOnly.TimeOfDay) : {0}", NextRun.Value.ToString("MM/dd/yyyy HH:mm")));
					}
                    else
                    {
                        NextRun = dateOnly.Add(timeOnly.TimeOfDay);
						Debug(string.Format("NextRun = dateOnly.Add(timeOnly.TimeOfDay) : ", NextRun.Value.ToString("MM/dd/yyyy HH:mm")));
					}
                    
                    break;
				default:
					break;
			}

            //Check for DLS and update if needed
            if (NextRun.HasValue)
            {
                nextRunToCreatorTimeZone = TimeZoneInfo.ConvertTimeFromUtc(NextRunUtc.Value, tz);
                Debug(string.Format("StartDateToCreatorTimeZone = TimeZoneInfo.ConvertTimeFromUtc(StartDateUtc, tz): {0}", StartDateToCreatorTimeZone.ToString("MM/dd/yyyy HH:mm")));
                Debug(string.Format("nextRunToCreatorTimeZone = TimeZoneInfo.ConvertTimeFromUtc(NextRunUtc.Value, tz): {0}", nextRunToCreatorTimeZone.ToString("MM/dd/yyyy HH:mm")));

                if (TimeSpan.Compare(StartDateToCreatorTimeZone.TimeOfDay, nextRunToCreatorTimeZone.TimeOfDay) != 0)
                {
                    DateTime fixedDateForDLS = TimeZoneInfo.ConvertTimeToUtc(new DateTime(NextRun.Value.Year, NextRun.Value.Month, NextRun.Value.Day, StartDateToCreatorTimeZone.Hour, StartDateToCreatorTimeZone.Minute, 0), tz);
					Debug(string.Format("fixedDateForDLS = TimeZoneInfo.ConvertTimeToUtc(new DateTime(NextRun.Value.Year, NextRun.Value.Month, NextRun.Value.Day, StartDateToCreatorTimeZone.Hour, StartDateToCreatorTimeZone.Minute, 0), tz): {0}", fixedDateForDLS.ToString("MM/dd/yyyy HH:mm")));

					//Last check to see if after converting to the right time zone and fixing for DLS the datetime is not ealier than the converted nextrun, if so add 1 day.
					if (DateTime.Compare(fixedDateForDLS, nextRunToCreatorTimeZone) < 0)
					{
						fixedDateForDLS = fixedDateForDLS.AddDays(1);
					}

					NextRun = fixedDateForDLS;
					Debug(string.Format("NextRun: {0}", NextRun.Value.ToString("MM/dd/yyyy HH:mm")));
                }
            }
		}

        public void SetRecipientsAndAttachment(MailMessage message, Organization organization, ref System.Collections.Generic.List<string> invalidEmailAddress)
        {
            message.From = GetEmailAddressFromString(organization.GetReplyToAddress().Trim());
            AddEmailAddressesFromString(message.To, EmailRecipients, ref invalidEmailAddress);
        }

        private MailAddress GetEmailAddressFromString(string text)
        {
            string name = "";
            int start = text.IndexOf('"');
            int end = -1;
            if (start > -1)
            {
                start++;
                end = text.IndexOf('"', start + 1);
                if (end > -1)
                {
                    name = text.Substring(start, end - start).Trim();
                }
            }

            if (name == "") return new MailAddress(text);
            string address = text;

            start = text.IndexOf('<');
            end = -1;
            if (start > -1)
            {
                start++;
                end = text.IndexOf('>', start + 1);
                if (end > -1)
                {
                    address = text.Substring(start, end - start).Trim();
                }
            }

            return new MailAddress(address, name);
        }

        private void AddEmailAddressesFromString(MailAddressCollection collection, string text, ref System.Collections.Generic.List<string> invalidEmailAddress)
        {
            if (string.IsNullOrEmpty(text.Trim())) return;
            string[] list = text.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in list)
            {
                try
                {
                    MailAddress address = GetEmailAddressFromString(s);
                    if (address != null) collection.Add(address);
                }
                catch (Exception ex)
                {
                    invalidEmailAddress.Add(s);
                }
            }
        }

		private void Debug(string message)
		{
			try
			{
				System.Collections.Specialized.NameValueCollection settings = System.Configuration.ConfigurationManager.AppSettings;
				string orgIdToDebug = settings["DebugOrgId"];

				if (OrganizationId.ToString() == orgIdToDebug)
				{
					string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
					string debugFile = System.IO.Path.Combine(path, "Debug.txt");
					System.IO.File.AppendAllText(debugFile, string.Format("{0} (orgId {1}): {2}{3}", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), orgIdToDebug, message, Environment.NewLine));
				}
			}
			catch (Exception ex)
			{
				ExceptionLogs.LogException(LoginUser.Anonymous, ex, "ScheduledReports-Debug", "Tickets.Clone - Actions");
			}
		}
    }

	public partial class ScheduledReports
	{
		public void LoadAll(int organizationID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"SELECT ScheduledReports.*, Creator.FirstName + ' ' + Creator.LastName AS Creator, Modifier.FirstName + ' ' + Modifier.LastName AS Modifier, Reports.Name AS ReportName
										FROM ScheduledReports
											JOIN Reports ON ScheduledReports.ReportId = Reports.ReportID
											LEFT JOIN [Users] AS Creator ON ScheduledReports.creatorId = Creator.UserID
											LEFT JOIN [Users] AS Modifier ON ScheduledReports.modifierId = Modifier.UserID
										WHERE (ScheduledReports.organizationId = @OrganizationID)";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@OrganizationID", organizationID);
				Fill(command);
			}
		}

		public static ScheduledReport GetNextWaiting(LoginUser loginUser, string processID)
		{
			ScheduledReports scheduledReports = new ScheduledReports(loginUser);

			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = @"UPDATE ScheduledReports
SET LockProcessID = @ProcessID 
OUTPUT Inserted.*
WHERE Id IN (
  SELECT TOP 1 Id 
FROM ScheduledReports 
WHERE
	LockProcessID IS NULL 
	AND NextRun IS NOT NULL 
	AND ISNULL(NextRun,0) > ISNULL(LastRun,0)
	AND NextRun < GETUTCDATE()
    ORDER BY NextRun
    )
";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@ProcessID", processID);
                scheduledReports.Fill(command);
			}

			if (scheduledReports.IsEmpty)
				return null;
			else
				return scheduledReports[0];
		}

		public static void UnlockAll(LoginUser loginUser)
		{
			ScheduledReports scheduledReports = new ScheduledReports(loginUser);

			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "UPDATE ScheduledReports SET LockProcessID = NULL";
				command.CommandType = CommandType.Text;
				scheduledReports.ExecuteNonQuery(command);
			}
		}
		public static void UnlockThread(LoginUser loginUser, int thread)
        {
            ScheduledReports scheduledReports = new ScheduledReports(loginUser);

            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "UPDATE ScheduledReports SET LockProcessId = NULL WHERE LockProcessId = @id";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("id", thread);
                scheduledReports.ExecuteNonQuery(command);
            }
        }
    }

	[DataContract]
	public class ScheduledReportItem
	{

		public ScheduledReportItem(ScheduledReport scheduledReport)
		{
			this.Id = scheduledReport.Id;
			this.OrganizationId = scheduledReport.OrganizationId;
			this.EmailSubject = scheduledReport.EmailSubject;
			this.EmailBody = scheduledReport.EmailBody;
			this.EmailRecipients = scheduledReport.EmailRecipients;
			this.ReportId = scheduledReport.ReportId;
			this.ReportName = scheduledReport.ReportName;
			this.RunCount = scheduledReport.RunCount ?? 0;
			this.IsActive = scheduledReport.IsActive;
			this.LastRun = scheduledReport.LastRun;
         this.IsSuccessful = scheduledReport.IsSuccessful;
			this.NextRun = scheduledReport.NextRun;
			this.CreatorId = scheduledReport.CreatorId;
			this.Creator = scheduledReport.Creator ?? "Unknown";
			this.ModifierId = scheduledReport.ModifierId;
			this.Modifier = scheduledReport.Modifier ?? "Unknown";
			this.DateCreated = scheduledReport.DateCreated;
			this.DateModified = scheduledReport.DateModified;
            this.FilePathID = scheduledReport.FilePathID;
		}

		[DataMember]
		public int Id { get; set; }
		[DataMember]
		public int? OrganizationId { get; set; }
		[DataMember]
		public string EmailSubject { get; set; }
		[DataMember]
		public string EmailBody { get; set; }
		[DataMember]
		public string EmailRecipients { get; set; }
		[DataMember]
		public int ReportId { get; set; }
		[DataMember]
		public string ReportName { get; set; }
		[DataMember]
		public short RunCount { get; set; }
		[DataMember]
		public bool IsActive { get; set; }
		[DataMember]
		public DateTime? LastRun { get; set; }
		[DataMember]
		public bool? IsSuccessful { get; set; }
		[DataMember]
		public DateTime? NextRun { get; set; }
		[DataMember]
		public int CreatorId { get; set; }
		[DataMember]
		public int? ModifierId { get; set; }
		[DataMember]
		public string Creator { get; set; }
		[DataMember]
		public string Modifier { get; set; }
		[DataMember]
		public DateTime DateCreated { get; set; }
		[DataMember]
		public DateTime? DateModified { get; set; }
        [DataMember]
        public int FilePathID { get; set; }
        [DataMember]
		public bool HasLogFile
		{
			get
			{
					bool hasLogFile = false;

					if (OrganizationId != null && Id > 0)
					{
						string path = AttachmentPath.GetPath(LoginUser.Anonymous, (int)OrganizationId, AttachmentPath.Folder.ScheduledReportsLogs, FilePathID);
						string fileName = Id.ToString() + ".txt";
						hasLogFile = System.IO.File.Exists(System.IO.Path.Combine(path, fileName));
					}

					return hasLogFile;
			}
		}
	}
}
