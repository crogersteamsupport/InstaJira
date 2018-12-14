using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
    [Serializable]
    public class ReportSender : ServiceThreadPoolProcess
    {
        private bool _isDebug = false;
        private static object _staticLock = new object();
        private MailAddressCollection _debugAddresses;
        private ReportSenderPublicLog _publicLog;
        public const string PHANTOMJSCOMMAND = @"phantomjs highcharts-convert.js -infile {0} -outfile {1}";

        public ReportSender()
        {
            _debugAddresses = new MailAddressCollection();
        }

        public override void Run()
        {
            ScheduledReports.UnlockThread(LoginUser, (int)_threadPosition);

            while (!IsStopped)
            {
                Logs.WriteHeader("Starting Run");

                try
                {
                    int waitBeforeLoggingWithoutScheduledReportsDue = 30;
                    DateTime noScheduledReportsDue = DateTime.UtcNow;

                    while (true)
                    {
                        try
                        {
                            if (ServiceStopped)
                            {
                                Logs.WriteHeader("ServiceThread.ServiceStopped");
                                break;
                            }

                            if (IsStopped)
                            {
                                Logs.WriteHeader("IsStopped");
                                break;
                            }

                            ScheduledReport scheduledReport = GetNextScheduledReport(LoginUser.ConnectionString, (int)_threadPosition, Logs);

                            if (scheduledReport != null)
                            {
                                string publicLogPath = TeamSupport.Data.Quarantine.ServiceQ.GetAttachmentPath16(LoginUser, scheduledReport.OrganizationId, scheduledReport.FilePathID);
                                _publicLog = new ReportSenderPublicLog(publicLogPath, scheduledReport.Id, scheduledReport.OrganizationId);
                                Log(string.Format("Date and times used for this log entries are in TimeZone {0}", _publicLog.OrganizationTimeZoneInfo.DisplayName), LogType.Public);
                                QueueEmail(scheduledReport);
                                noScheduledReportsDue = DateTime.UtcNow;
                            }
                            else
                            {
                                if (DateTime.UtcNow.Subtract(noScheduledReportsDue).Minutes >= waitBeforeLoggingWithoutScheduledReportsDue)
                                {
                                    Log(string.Format("No scheduled reports due found in the last {0} minutes", waitBeforeLoggingWithoutScheduledReportsDue));
                                    noScheduledReportsDue = DateTime.UtcNow;
                                }

                                Thread.Sleep(10000);
                                continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log("Error sending report email - Ending Thread");
                            Logs.WriteException(ex);
                            ExceptionLogs.LogException(LoginUser, ex, "ReportSender", "Error sending report email");
                        }
                        finally
                        {
                            UpdateHealth();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logs.WriteException(ex);
                }
                finally
                {
                    Logs.WriteHeader("Exiting.");
                }
            }
        }

        private static ScheduledReport GetNextScheduledReport(string connectionString, int lockID, Logs logs = null)
        {
            ScheduledReport result;
            LoginUser loginUser = new LoginUser(connectionString, -1, -1, null);

            lock (_staticLock)
            {
                result = ScheduledReports.GetNextWaiting(loginUser, lockID.ToString());
            }

            return result;
        }

        public override void ReleaseAllLocks()
        {
            ScheduledReports.UnlockAll(LoginUser);
        }

        public static string[] GetReportColumnNames(LoginUser scheduledReportCreator, int reportID)
        {
            List<string> result = new List<string>();
            ReportColumn[] columns = GetReportColumns(scheduledReportCreator, reportID);
            foreach (ReportColumn column in columns)
            {
                result.Add(column.Name);
            }
            return result.ToArray();
        }

        public static ReportColumn[] GetReportColumns(LoginUser scheduledReportCreator, int reportID)
        {
            Report report = Reports.GetReport(scheduledReportCreator, reportID);
            if (report.ReportDefType == ReportType.Table || report.ReportDefType == ReportType.TicketView) return report.GetTabularColumns();
            return report.GetSqlColumns();
        }

        private string GetReportChartFile(LoginUser scheduledReportCreator, ScheduledReport scheduledReport, Report report)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string chartOptionsFilesDirectory = "ChartOptionsFiles";
            string chartOptionsFilesPath = Path.Combine(path, chartOptionsFilesDirectory);

            if (!Directory.Exists(chartOptionsFilesPath))
            {
                Directory.CreateDirectory(chartOptionsFilesPath);
            }

            DataTable dataTable = GetReportTableAll(scheduledReportCreator, report, "", false, true, false);
            ReportItem reportItem = new ReportItem(report, true);
            string reportData = TeamSupport.Data.BusinessObjects.Reporting.SummaryReportSql.GetHubChartData(scheduledReportCreator, report);
            string optionsFileData = GetChartOptionsAndData(report.ReportDef, reportData);

            string optionsFile = string.Format("{0}_{1}.js", scheduledReport.Id, scheduledReport.ReportId);
            optionsFile = Path.Combine(chartOptionsFilesPath, optionsFile);
            Log("Writting chart options to: " + optionsFile);
            File.WriteAllText(optionsFile, optionsFileData);
            string outputImage = GenerateCleanOutputFileName(_loginUser, scheduledReport, report.Name, "png");
            Log("outputImage: " + outputImage);

            //Create Batch File
            string batchFile = string.Format("thread_{0}.bat", _threadPosition);
            string batchFileCommand = Path.GetPathRoot(System.Reflection.Assembly.GetEntryAssembly().Location) + Environment.NewLine;

            if (!string.IsNullOrEmpty(batchFileCommand))
            {
                batchFileCommand = batchFileCommand.Replace(@"\", "");
            }

            batchFileCommand += "chdir " + Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + Environment.NewLine;
            batchFileCommand += string.Format(PHANTOMJSCOMMAND, optionsFile, outputImage);
            string batchFileFullPath = Path.Combine(path, batchFile);
            Log("batchFile: " + batchFileFullPath);

            File.WriteAllText(batchFileFullPath, batchFileCommand);
            Log("Command written: " + batchFileCommand);

            if (File.Exists(outputImage))
            {
                File.Delete(outputImage);
                Log("Old chart image deleted.");
            }

            bool commandIsSuccessful = ExecuteCommand(batchFileFullPath);

            if (!commandIsSuccessful)
            {
                outputImage = string.Empty;
            }
            else if (!File.Exists(outputImage))
            {
                Log("outputImage not created/found");
                outputImage = string.Empty;
            }

            return outputImage;
        }

        /// <summary>
        /// Unfortunately we need to recreate the UI process that generates the reports including the JS function that append the json needed.
        /// This needs to match (in its C# version) to the addChartData function in ..\WebApp\Resources\Pages\ReportCharts.js !!!!!!!!!!!!! 
        /// </summary>
        /// <param name="reportDef"></param>
        /// <param name="recordsData"></param>
        /// <returns></returns>
        private string GetChartOptionsAndData(string reportDef, string recordsData)
        {
            dynamic reportDefObject = new System.Dynamic.ExpandoObject();
            reportDefObject.Def = JsonConvert.DeserializeObject(reportDef);

            dynamic options = new System.Dynamic.ExpandoObject();
            options = JsonConvert.DeserializeObject(reportDefObject.Def.Chart.ToString());
            dynamic records = new System.Dynamic.ExpandoObject();
            records = JsonConvert.DeserializeObject(recordsData);

            string[] old = { "#3276B1", "#193b58", "#78A300", "#e72b19", "#008080", "#E57B3A", "#bd4cff", "#FFC312", "#BA55D3" };
            string[] berry = { "#8A2BE2", "#BA55D3", "#4169E1", "#C71585", "#0000FF", "#8019E0", "#DA70D6", "#7B68EE", "#C000C0", "#0000CD", "#800080" };
            string[] bright = { "#008000", "#0000FF", "#800080", "#800080", "#FF00FF", "#008080", "#FFFF00", "#808080", "#00FFFF", "#000080", "#800000", "#FF3939", "#7F7F00", "#C0C0C0", "#FF6347", "#FFE4B5" };
            string[] brightPastel = { "#418CF0", "#FCB441", "#DF3A02", "#056492", "#BFBFBF", "#1A3B69", "#FFE382", "#129CDD", "#CA6B4B", "#005CDB", "#F3D288", "#506381", "#F1B9A8", "#E0830A", "#7893BE" };
            string[] chocolate = { "#A0522D", "#D2691E", "#8B0000", "#CD853F", "#A52A2A", "#F4A460", "#8B4513", "#C04000", "#B22222", "#B65C3A" };
            string[] earthTones = { "#33023", "#B8860B", "#C04000", "#6B8E23", "#CD853F", "#C0C000", "#228B22", "#D2691E", "#808000", "#20B2AA", "#F4A460", "#00C000", "#8FBC8B", "#B22222", "#843A05", "#C00000" };
            string[] excel = { "#9999FF", "#993366", "#FFFFCC", "#CCFFFF", "#660066", "#FF8080", "#0063CB", "#CCCCFF", "#000080", "#FF00FF", "#FFFF00", "#00FFFF", "#800080", "#800000", "#007F7F", "#0000FF" };
            string[] fire = { "#FFD700", "#FF0000", "#FF1493", "#DC143C", "#FF8C00", "#FF00FF", "#FFFF00", "#FF4500", "#C71585", "#DDE221" };
            string[] grayScale = { "#C8C8C8", "#BDBDBD", "#B2B2B2", "#A7A7A7", "#9C9C9C", "#919191", "#868686", "#7A7A7A", "#707070", "#656565", "#565656", "#4F4F4F", "#424242", "#393939", "#2E2E2E", "#232323" };
            string[] light = { "#E6E6FA", "#FFF0F5", "#FFDAB9", "#", "#FFFACD", "#", "#FFE4E1", "#F0FFF0", "#F0F8FF", "#F5F5F5", "#FAEBD7", "#E0FFFF" };
            string[] pastel = { "#87CEEB", "#32CD32", "#BA55D3", "#F08080", "#4682B4", "#9ACD32", "#40E0D0", "#FF69B4", "#F0E68C", "#D2B48C", "#8FBC8B", "#6495ED", "#DDA0DD", "#5F9EA0", "#FFDAB9", "#FFA07A" };
            string[] seaGreen = { "#2E8B57", "#66CDAA", "#4682B4", "#008B8B", "#5F9EA0", "#38B16E", "#48D1CC", "#B0C4DE", "#8FBC8B", "#87CEEB" };
            string[] semiTransparent = { "#FF6969", "#69FF69", "#6969FF", "#FFFF5D", "#69FFFF", "#FF69FF", "#CDB075", "#FFAFAF", "#AFFFAF", "#AFAFFF", "#FFFFAF", "#AFFFFF", "#FFAFFF", "#E4D5B5", "#A4B086", "#819EC1" };

            options.colors = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(brightPastel));
            string series = string.Empty;
            string xAxis = string.Empty;

            if (options.ts.chartType == "pie")
            {
                if (records.Count > 2)
                {
                    return "Please do not select a series field to plot a pie chart.";
                }

                int total = 0;

                for (int i = 0; i < records[1].data.Count; i++)
                {
                    total += (int)records[1].data[i];
                }

                string seriesFormat = @"[{{ type: ""pie"", name: """ + options.ts.seriesTitle.ToString() + @""", data: [{0}]}}]";
                List<string> seriesData = new List<string>();

                for (var i = 0; i < records[1].data.Count; i++)
                {
                    float val = (float)(records[1].data[i]) / total * 100;
                    seriesData.Add(@"[""" + FixBlankSeriesName(records[0].data[i].ToString()) + @""", " + val.ToString("#.00") + "]");
                }

                if (seriesData.Any() && seriesData.Count > 0)
                {
                    series = string.Format(seriesFormat, string.Join(",", seriesData.ToArray()));
                }
                else
                {
                    series = string.Format(seriesFormat, "");
                }
            }
            else if ((records[0].fieldType == "datetime" && records[0].format == "date") || (records[1].fieldType == "datetime" && records[1].format == "date"))
            {
                List<SeriesValues> seriesList = new List<SeriesValues>();
                xAxis = @"{ type: ""datetime"" }";

                if (records.Count == 3)
                {
                    for (var i = 0; i < records[0].data.Count; i++)
                    {
                        var val = records[0].data[i];
                        string name = (string)val;

                        if (String.IsNullOrEmpty(name))
                        {
                            name = "Unknown";
                        }

                        if (!seriesList.Where(p => p.Name == name).Any())
                        {
                            seriesList.Add(new SeriesValues()
                            {
                                Name = name,
                                Value = val,
                                Data = new List<string>()
                            });
                        }

                        SeriesValues values = seriesList.Where(p => p.Name == name).FirstOrDefault();
                        values.Data.Add("[" + ((DateTime)records[1].data[i]).ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds + "," + records[2].data[i] + "]");
                    }

                    string seriesFormat = @"{{""name"": ""{0}"",""value"": ""{1}"",""data"": [{2}]}},";

                    foreach (SeriesValues serie in seriesList)
                    {
                        series += "" + string.Format(seriesFormat, serie.Name, serie.Value, (serie.Data.Any() ? string.Join(",", serie.Data.ToArray()) : ""));
                    }

                    if (!string.IsNullOrEmpty(series))
                    {
                        series = "[" + series.Substring(0, series.Length - 1) + "]";
                    }
                    else
                    {
                        Log("No data in series.", LogType.Both);
                    }
                }
                else if (records.Count == 2)
                {
                    Log("//vv Unhandled chart case until know.");
                    //options.series.push("{ name: records[1].name, data: [] }");

                    //for (var i = 0; i < records[0].data.length; i++)
                    //{
                    //    List<string> item = new List<string>();
                    //    //vv item.push(Date.parse(records[0].data[i]));
                    //    //vv item.push(records[1].data[i]);

                    //    if (item[0] != null)
                    //    {
                    //        if (item[1] == null) item[1] = "0";
                    //        options.series[0].data.push(item);
                    //    }
                    //}
                }
            }
            else
            {
                //options.series = new List<string>(); //vv [];

                if (records.Count == 3)
                {
                    List<string> categoriesList = new List<string>();

                    //// build categories
                    for (int i = 0; i < records[1].data.Count; i++)
                    {
                        string name = FixRecordName(records[1], i);

                        if (!categoriesList.Where(p => p == name).Any())
                        {
                            categoriesList.Add(name);
                        }
                    }

                    xAxis = "{ categories: [" + (categoriesList.Any() ? string.Join(",", categoriesList.Select(x => string.Format(@"""{0}""", x)).ToArray()) : "") + "]}";
                    List<SeriesValues> seriesValues = new List<SeriesValues>();

                    for (int i = 0; i < records[0].data.Count; i++)
                    {
                        string val = records[0].data[i];
                        SeriesValues values = new SeriesValues();

                        if (!seriesValues.Where(p => p.Name == val).Any())
                        {
                            values.Name = string.IsNullOrEmpty(val) ? "Unknown" : val + "";
                            values.Value = val;

                            for (int x = 0; x < categoriesList.Count; x++)
                            {
                                values.Data.Add("0");
                            }

                            seriesValues.Add(values);
                        }
                        else
                        {
                            values = seriesValues.Where(p => p.Name == val).FirstOrDefault();
                        }

                        string findCategory = records[1].data[i].ToString();

                        for (int x = 0; x < categoriesList.Count; x++)
                        {
                            if (categoriesList[x] == findCategory)
                            {
                                values.Data[x] = records[2].data[i];
                                break;
                            }
                        }
                    }

                    string seriesFormat = @"{{""name"": ""{0}"",""value"": ""{1}"",""data"": [{2}]}},";

                    foreach (SeriesValues serie in seriesValues)
                    {
                        series += "" + string.Format(seriesFormat, serie.Name, serie.Value, (serie.Data.Any() ? string.Join(",", serie.Data.ToArray()) : ""));
                        //series = "[{ name: '" + FixBlankSeriesName(FixRecordName(records[0], i)) + "', value: '" + list[i] + "', data: " + (list2.Any() ? string.Join(",", list2.Select(x => string.Format("'{0}'", x)).ToArray()) : "") + " }]";
                    }

                    if (!string.IsNullOrEmpty(series))
                    {
                        series = "[" + series.Substring(0, series.Length - 1) + "]";
                    }
                    else
                    {
                        Log("No data in series.", LogType.Both);
                    }
                }
                else if (records.Count == 2)
                {
                    List<string> list = new List<string>();

                    for (int i = 0; i < records[0].data.Count; i++)
                    {
                        list.Add(records[0].data[i].ToString());
                    }

                    xAxis = "{ categories: [" + (list.Any() ? string.Join(",", list.Select(x => string.Format(@"""{0}""", x)).ToArray()) : "") + "]}";

                    string seriesFormat = @"[{{ name: """ + records[1].name.ToString() + @""", data: [{0}]}}]";
                    list = new List<string>();

                    for (int i = 0; i < records[1].data.Count; i++)
                    {
                        list.Add(records[1].data[i].ToString());
                    }

                    series = string.Format(seriesFormat, (list.Any() ? string.Join(",", list.ToArray()) : ""));
                }
            }

            if (!string.IsNullOrEmpty(series) && records[1].data.Count > 1000)
            {
                return "Your series has " + records[1].data.Count + " items. This might cause issues rendering the chart, please try filtering your data to less than 1000.";
            }

            string jsonOptionsAndData = JsonConvert.SerializeObject(options);

            if (!string.IsNullOrEmpty(xAxis))
            {
                jsonOptionsAndData = jsonOptionsAndData.Substring(0, jsonOptionsAndData.Length - 1) + ", xAxis: " + xAxis + "}";
            }

            jsonOptionsAndData = jsonOptionsAndData.Substring(0, jsonOptionsAndData.Length - 1) + ", series: " + series + "}";

            //Add width and height. This might or not be the right decision here.
            if (jsonOptionsAndData.Contains("{\"chart\":{"))
            {
                jsonOptionsAndData = jsonOptionsAndData.Replace("{\"chart\":{", "{\"chart\":{width: 1000, height: 800,");
            }

            return jsonOptionsAndData;
        }

        private static string FixRecordName(dynamic record, int index)
        {
            if (record.fieldType == "bool")
            {
                return record.name + (record.data[index] == true ? " = True" : " = False");
            }

            return record.data[index];
        }

        private static string FixBlankSeriesName(string val)
        {
            return string.IsNullOrEmpty(val) ? "Unknown" : val + "";
        }

        //vv It does not look like this is used. At least not that I found.
        //private static string createDataArray(dynamic options)
        //{
        //    List<int> result = new List<int>();

        //    for (var i = 0; i < options.xAxis.categories.length; i++)
        //    {
        //        result.Add(0);
        //    }

        //    return result.ToString();
        //}

        //private static int IndexOfCategory(dynamic options, string name)
        //{
        //    for (int i = 0; i < options.xAxis.categories.Count; i++)
        //    {
        //        if (options.xAxis.categories[i] == name)
        //        {
        //            return i;
        //        }
        //    }

        //    return -1;
        //}

        private bool ExecuteCommand(string command)
        {
            bool isSuccessful = false;
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            Log("Starting to process the command...");
            process = System.Diagnostics.Process.Start(processInfo);

            List<string> output = new List<string>();
            List<string> error = new List<string>();

            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    output.Add(e.Data);
                }
            };
            process.BeginOutputReadLine();
            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    error.Add(e.Data);
                }
            };

            process.BeginErrorReadLine();
            process.WaitForExit();
            exitCode = process.ExitCode;
            process.Close();

            isSuccessful = exitCode == 0 && !error.Any() && error.Count == 0 && !output.Where(p => p.Contains("Error:")).Any();

            if (!isSuccessful)
            {
                foreach (string outputLine in output.Where(p => !string.IsNullOrEmpty(p)))
                {
                    Log("Output: " + outputLine);
                }

                foreach (string errorLine in error.Where(p => !string.IsNullOrEmpty(p)))
                {
                    Log("Error: " + errorLine);
                }

                Log("ExitCode: " + exitCode);
            }

            return isSuccessful;
        }

        private static string GenerateCleanOutputFileName(LoginUser loginUser, ScheduledReport scheduledReport, string reportName, string extension)
        {
            int organizationId = scheduledReport.OrganizationId;
            string fileName = string.Empty;
            string outputImagePath = TeamSupport.Data.Quarantine.ServiceQ.GetAttachmentPath17(loginUser, organizationId);
            string reportNameForFile = RemoveSpecialCharacters(reportName);

            if (string.IsNullOrEmpty(reportNameForFile))
            {
                reportNameForFile = string.Format("{0}_{1}", scheduledReport.Id, scheduledReport.ReportId);
            }

            fileName = string.Format("{0}\\{1}.{2}", outputImagePath, reportNameForFile, extension);

            return fileName;
        }

        private static string RemoveSpecialCharacters(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == '-')
                {
                    sb.Append(c);
                }
                else if (c == ' ')
                {
                    sb.Append('_');
                }
            }
            return sb.ToString();
        }

        private void QueueEmail(ScheduledReport scheduledReport)
        {
            scheduledReport.IsSuccessful = false;
            Log(string.Format("Scheduled Report Id: {0}, Report Id: {1}, Organization {2}", scheduledReport.Id.ToString(), scheduledReport.ReportId, scheduledReport.OrganizationId));
            Log(string.Format("Set to start on: {0}", scheduledReport.NextRun), LogType.Public);

            try
            {
                LoginUser scheduledReportCreator = new LoginUser(scheduledReport.CreatorId, scheduledReport.OrganizationId);
                Report report = Reports.GetReport(scheduledReportCreator, scheduledReport.ReportId, scheduledReport.CreatorId);
                Log(string.Format("Generating {0} Report", report.ReportDefType.ToString()), LogType.Both);

                string reportAttachmentFile = string.Empty;

                if (report.ReportDefType == ReportType.Chart)
                {
                    reportAttachmentFile = GetReportChartFile(scheduledReportCreator, scheduledReport, report);
                }
                else
                {
                    reportAttachmentFile = GetReportDataToFile(scheduledReportCreator, report, scheduledReport, "", false, true, Logs);
                }

                if (!string.IsNullOrEmpty(reportAttachmentFile))
                {
                    if (report.ReportDefType != ReportType.Chart)
                    {
                        try
                        {
							float fileSizeMB = 0;

							if (File.Exists(reportAttachmentFile))
							{
								long fileSizeBytes = new FileInfo(reportAttachmentFile).Length;
								fileSizeMB = (fileSizeBytes / 1024f) / 1024f;
							}

							//If the report is 2M or less, don't zip it.  If over 2M, zip it.
							if (fileSizeMB > 2)
							{
								string zipFileName = reportAttachmentFile.Replace(".csv", ".zip");

								if (File.Exists(zipFileName))
								{
									try
									{
										File.Delete(zipFileName);
									}
									catch (IOException ioEx)
									{
										DateTime zipFileNamesuffix = scheduledReport.NextRun != null ? (DateTime)scheduledReport.NextRun : DateTime.UtcNow;
										zipFileName = zipFileName.Replace(".zip", string.Format("_{0}.zip", zipFileNamesuffix.ToString("yyyyMMddHHmm")));
										Log("Previous zip file could not be deleted. Naming the new one: ");
										Log(ioEx.Message + Environment.NewLine + ioEx.StackTrace);
									}
								}

								using (ZipArchive zip = ZipFile.Open(zipFileName, ZipArchiveMode.Create))
								{
									zip.CreateEntryFromFile(reportAttachmentFile, Path.GetFileName(reportAttachmentFile));
								}

								if (File.Exists(zipFileName))
								{
									Log("CSV File zipped", LogType.Both);

									try
									{
										File.Delete(reportAttachmentFile);
									}
									catch (IOException ioEx)
									{
										Log("CSV file could not be deleted.");
										Log(ioEx.Message + Environment.NewLine + ioEx.StackTrace);
									}

									reportAttachmentFile = zipFileName;
								}
								else
								{
									Log("CSV zipped file was not found!");
								}
							}
                        }
                        catch (Exception ex)
                        {
                            Log("Error when zipping the CSV file.");
                            Log(ex.Message + Environment.NewLine + ex.StackTrace);
                        }
                    }

                    Log(string.Format("Report generated and file attachment created: {0}", Path.GetFileName(reportAttachmentFile)));

                    Organization organization = Organizations.GetOrganization(scheduledReportCreator, scheduledReportCreator.OrganizationID);
                    MailMessage message = EmailTemplates.GetScheduledReport(LoginUser, scheduledReport);
                    List<string> invalidEmailAddress = new List<string>();
                    scheduledReport.SetRecipientsAndAttachment(message, organization, ref invalidEmailAddress);

                    Log("Email message created", LogType.Public);
                    Log(string.Format("Email Recipients: {0}", string.Join(",", message.To.Select(p => p.Address).ToArray())), LogType.Both);

                    if (invalidEmailAddress.Any())
                    {
                        Log(string.Format("Invalid Email Recipients, they were excluded: {0}", string.Join(",", invalidEmailAddress.ToArray())), LogType.Both);
                    }

                    if (_isDebug == true)
                    {
                        string debugWhiteList = Settings.ReadString("Debug Email White List", "");
                        string debugDomains = Settings.ReadString("Debug Email Domains", "");
                        string debugAddresses = Settings.ReadString("Debug Email Address", "");

                        if (!string.IsNullOrWhiteSpace(debugWhiteList))
                        {
                            Logs.WriteEvent("DEBUG Whitelist: " + debugWhiteList);
                            string[] addresses = debugWhiteList.Replace(';', ',').Split(',');
                            List<MailAddress> mailAddresses = new List<MailAddress>();

                            foreach (MailAddress mailAddress in message.To)
                            {
                                foreach (string address in addresses)
                                {
                                    if (mailAddress.Address.ToLower().IndexOf(address.ToLower()) > -1)
                                    {
                                        mailAddresses.Add(mailAddress);
                                    }
                                }
                            }

                            message.To.Clear();

                            foreach (MailAddress mailAddress in mailAddresses)
                            {
                                message.To.Add(mailAddress);
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(debugDomains))
                        {
                            Logs.WriteEvent("DEBUG Domains: " + debugDomains);
                            string[] domains = debugDomains.Replace(';', ',').Split(',');
                            List<MailAddress> mailAddresses = new List<MailAddress>();

                            foreach (MailAddress mailAddress in message.To)
                            {
                                foreach (string domain in domains)
                                {
                                    if (mailAddress.Address.ToLower().IndexOf(domain.ToLower()) > -1)
                                    {
                                        mailAddresses.Add(mailAddress);
                                    }
                                }
                            }

                            message.To.Clear();

                            foreach (MailAddress mailAddress in mailAddresses)
                            {
                                message.To.Add(mailAddress);
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(debugAddresses))
                        {
                            Logs.WriteEvent("DEBUG Addresses: " + debugAddresses);
                            message.To.Clear();
                            message.To.Add(debugAddresses.Replace(';', ','));
                        }
                        else
                        {
                            Logs.WriteEvent("NO DEBUG FILTERS SET");
                            return;
                        }

                        if (message.To.Count < 1)
                        {
                            Logs.WriteEvent("No Debug Address specified.");
                            return;
                        }

                        message.Subject = string.Format("[{0}] {1}", Settings.ReadString("Debug Email Subject", "TEST MODE"), message.Subject);
                    }

                    Log("Queueing email(s)", LogType.Public);
                    AddMessage(scheduledReport.OrganizationId, string.Format("Scheduled Report Sent [{0}]", scheduledReport.Id), message, null, new string[] { reportAttachmentFile });
                    scheduledReport.IsSuccessful = true;
                }
                else
                {
                    Log("Report could not be generated and emailed, please contact TeamSupport", LogType.Public);
                }

                if ((ScheduledReportFrequency)scheduledReport.RecurrencyId == ScheduledReportFrequency.Once)
                {
                    scheduledReport.NextRun = null;
                }
                else
                {
                    scheduledReport.SetNextRun(true);
                }

                scheduledReport.RunCount = scheduledReport.RunCount != null ? (short)(scheduledReport.RunCount + 1) : (short)1;
                scheduledReport.LastRun = DateTime.UtcNow;
                scheduledReport.LockProcessId = null;
                scheduledReport.Collection.Save();
                Log(string.Format("Set next run to: {0}", scheduledReport.NextRun == null ? "Never" : scheduledReport.NextRun.ToString()), LogType.Both);
            }
            catch (Exception ex)
            {
                Logs.WriteException(ex);
                ExceptionLogs.LogException(LoginUser, ex, _threadPosition.ToString() + " - Report Sender", scheduledReport.Row);
            }
        }

        private void AddMessage(int organizationID, string description, MailMessage message, string replyToAddress = null, string[] attachments = null, DateTime? timeToSend = null, Ticket ticket = null)
        {
            Organization organization = Organizations.GetOrganization(LoginUser, organizationID);
            string replyAddress = organization.GetReplyToAddress(replyToAddress).Trim();
            int i = 0;

            while (i < message.To.Count)
            {
                MailAddress address = message.To[i];
                if (address.Address.ToLower().Trim() == message.From.Address.ToLower().Trim() || address.Address.ToLower().Trim() == replyAddress || address.Address.ToLower().Trim() == organization.SystemEmailID.ToString().Trim().ToLower() + "@teamsupport.com")
                {
                    message.To.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            if (message.To.Count < 1) return;

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(message.Body);
            message.HeadersEncoding = Encoding.UTF8;
            message.Body = builder.ToString();
            List<MailAddress> addresses = message.To.ToList();
            string body = message.Body;
            string subject = message.Subject;
            MailAddress replyMailAddress = null;

            try
            {
                replyMailAddress = GetMailAddress(replyAddress);
            }
            catch (Exception)
            {
                replyMailAddress = GetMailAddress(organization.GetReplyToAddress());
            }

            Logs.WriteEvent(string.Format("ReplyTo Address[{0}]", replyAddress.Replace("<", "").Replace(">", "")));

            foreach (MailAddress address in addresses)
            {
                try
                {
                    message.To.Clear();
                    message.To.Add(GetMailAddress(address.Address, address.DisplayName));
                    Logs.WriteEvent(string.Format("Successfuly added email address [{0}]", address.ToString()));
                    message.HeadersEncoding = Encoding.UTF8;
                    message.Body = body;
                    message.Subject = subject;
                    message.From = replyMailAddress;
                    EmailTemplates.ReplaceMailAddressParameters(message);
                    Emails.AddEmail(LoginUser, organizationID, null, description, message, attachments, timeToSend);

                    if (message.Subject == null) message.Subject = "";
                }
                catch (Exception ex)
                {
                    Log("Error when creating and adding email message.");
                    Log(ex.Message + Environment.NewLine + ex.StackTrace);
                }
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

        private MailAddress GetMailAddress(string address)
        {
            return new MailAddress(FixMailAddress(address));
        }

        private MailAddress GetMailAddress(string address, string displayName, Encoding displayNameEncoding)
        {
            return new MailAddress(FixMailAddress(address), FixMailName(displayName), displayNameEncoding);
        }

        private static string GetReportDataToFile(LoginUser scheduledReportCreator, Report report, ScheduledReport scheduledReport, string sortField, bool isDesc, bool useUserFilter, Logs logs = null)
        {
            if (logs != null)
            {
                logs.WriteEvent("GetReportTableAll");
            }

            DataTable dataTable = GetReportTableAll(scheduledReportCreator, report, sortField, isDesc, useUserFilter, false);
            dataTable = DataUtils.DecodeDataTable(dataTable);
            dataTable = DataUtils.StripHtmlDataTable(dataTable);

            if (logs != null)
            {
                logs.WriteEventFormat("dataTable created with {0} rows and {1} columns", dataTable.Rows.Count, dataTable.Columns.Count);
            }

            try
            {
                dynamic settings = null;
                object o = report.Row["Settings"];

                if (o != null && o != DBNull.Value && !string.IsNullOrEmpty(o.ToString()))
                {
                    settings = JObject.Parse(o.ToString());
                }

                if (settings != null)
                {
                    if (settings["Columns"] != null)
                    {
                        List<string> reportColumnNames = new List<string>();
                        for (int i = 0; i < settings["Columns"].Count; i++)
                        {
                            reportColumnNames.Add(settings["Columns"][i]["id"].ToString());
                        }

                        int index = 0;
                        foreach (string columnName in reportColumnNames)
                        {
                            DataColumn column = dataTable.Columns[columnName];
                            if (column != null)
                            {
                                column.SetOrdinal(index);
                                index++;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logs.WriteEventFormat("Exception when setting the column order by user settings. {0}", ex.Message);
            }

            StringBuilder stringBuilder = new StringBuilder();

            IEnumerable<string> columnNames = dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            stringBuilder.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                stringBuilder.AppendLine(string.Join(",", fields));
            }

            string fileName = GenerateCleanOutputFileName(scheduledReportCreator, scheduledReport, report.Name, "csv");
            File.WriteAllText(fileName, stringBuilder.ToString());

            if (logs != null)
            {
                logs.WriteEvent("File.WriteAllText completed");
            }

            return fileName;
        }

        private static DataTable GetReportTableAll(LoginUser scheduledReportCreator, Report report, string sortField, bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {
            SqlCommand command = new SqlCommand();
            report.GetCommand(command, includeHiddenFields, false, useUserFilter);

            if (string.IsNullOrWhiteSpace(sortField))
            {
                sortField = GetReportColumnNames(scheduledReportCreator, report.ReportID)[0];
                isDesc = false;
            }

            //need to append the extra stuff for custom and summary reports here. See Reports.cs line 2115
            if (report.ReportDefType == ReportType.Custom)
            {
                string query = @"WITH {0}, r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY [{1}] {2}) AS 'RowNum' FROM q) SELECT  *{3} FROM r ";
                command.CommandText = string.Format(query, command.CommandText, sortField, isDesc ? "DESC" : "ASC", "");
            }

            if (command.CommandText.ToLower().IndexOf(" order by ") < 0)
            {
                command.CommandText = command.CommandText + " ORDER BY [" + sortField + (isDesc ? "] DESC" : "] ASC");
            }

            report.LastSqlExecuted = DataUtils.GetCommandTextSql(command);
            report.Collection.Save();

            DataTable table = new DataTable();

            using (SqlConnection connection = new SqlConnection(scheduledReportCreator.ConnectionString))
            {
                connection.Open();
                command.Connection = connection;

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    try
                    {
                        adapter.Fill(table);
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogs.LogException(scheduledReportCreator, ex, "Report Data");
                        throw;
                    }
                }

                connection.Close();
            }

            return table;
        }

        private class SeriesValues
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public List<string> Data { get; set; }

            public SeriesValues()
            {
                Data = new List<string>();
            }
        }

        [Flags]
        private enum LogType
        {
            None = 0,
            Internal = 1,
            Public = 2,
            Both = Internal | Public
        }

        private void Log(string message, LogType logType = LogType.Internal)
        {
            string error = string.Empty;

            switch (logType)
            {
                case LogType.Internal:
                    Logs.WriteEvent(message);
                    break;
                case LogType.Public:
                    _publicLog.Write(message, ref error);
                    break;
                case LogType.Both:
                    Logs.WriteEvent(message);
                    _publicLog.Write(message, ref error);
                    break;
                default:
                    Logs.WriteEvent(message);
                    break;
            }

            if (!string.IsNullOrEmpty(error))
            {
                Logs.WriteEventFormat("Error found when writing to public log: {0}", error);
            }
        }

        public class ReportSenderPublicLog
        {
            private string _logPath;
            private string _fileName;
            private Organization _organization;
            private TimeZoneInfo _timeZoneInfo;
            private DateTime _organizationDateTime = DateTime.Now;
            private int? _timeOffset = null;

            public ReportSenderPublicLog(string path, int scheduledReportId, int organizationId)
            {
                _logPath = path;
                _fileName = scheduledReportId.ToString() + ".txt";
                _organization = Organizations.GetOrganization(LoginUser.Anonymous, organizationId);
                _timeOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;

                if (_organization != null && _organization.TimeZoneID != null && _organization.TimeZoneID != "")
                {
                    _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_organization.TimeZoneID);
                    _timeOffset = null;
                }

                if (!Directory.Exists(_logPath))
                {
                    Directory.CreateDirectory(_logPath);
                }
            }

            public void Write(string text, ref string error)
            {
                try
                {
                    error = string.Empty;

                    if (!File.Exists(_logPath + @"\" + _fileName))
                    {
                        foreach (string oldFileName in Directory.GetFiles(_logPath))
                        {
                            if (File.GetLastWriteTime(oldFileName).AddDays(30) < DateTime.Today)
                            {
                                File.Delete(oldFileName);
                            }
                        }
                    }

                    if (_timeZoneInfo != null)
                    {
                        _organizationDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZoneInfo);
                    }
                    else
                    {
                        _organizationDateTime = DateTime.Now;
                    }

                    File.AppendAllText(_logPath + @"\" + _fileName, string.Format("{0} {1}{2}: {3}{4}", _organizationDateTime.ToShortDateString(),
                                                                                                           _organizationDateTime.ToLongTimeString(),
                                                                                                           (_timeOffset == null ? "" : " (" + _timeOffset.ToString() + ")"),
                                                                                                           text,
                                                                                                           Environment.NewLine));
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }

            public TimeZoneInfo OrganizationTimeZoneInfo
            {
                get
                {
                    return _timeZoneInfo;
                }
            }
        }
    }
}
