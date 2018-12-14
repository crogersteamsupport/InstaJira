using System;
using System.Web.Services;
using System.Collections.Generic;
using System.Web.Script.Services;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Linq;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ReportService : System.Web.Services.WebService
    {

        public ReportService() { }

        [WebMethod]
        public ReportColumn[] GetReportColumns(int reportID)
        {
            return Reports.GetReportColumns(TSAuthentication.GetLoginUser(), reportID);
        }

        [WebMethod]
        public GridResult GetReportData(int reportID, int from, int to, string sortField, bool isDesc, bool useUserFilter)
        {
            return Reports.GetReportData(TSAuthentication.GetLoginUser(), reportID, from, to, sortField, isDesc, useUserFilter);
        }

        [WebMethod]
        public string GetChartReportData(int reportID)
        {
            return TeamSupport.Data.BusinessObjects.Reporting.SummaryReportSql.GetChartReportData(TSAuthentication.GetLoginUser(), reportID);
        }

        [WebMethod]
        public string GetChartData(string summaryReportFields)
        {
            return TeamSupport.Data.BusinessObjects.Reporting.SummaryReportSql.GetChartData(TSAuthentication.GetLoginUser(), summaryReportFields);
        }

        [WebMethod]
        public ReportItem[] GetDashboardReports()
        {
            string data = GetDashboard();
            List<ReportItem> result = new List<ReportItem>();
            if (!string.IsNullOrWhiteSpace(data))
            {

                DashboardItem[] items = JsonConvert.DeserializeObject<DashboardItem[]>(GetDashboard());
                List<string> idlist = new List<string>();
                foreach (DashboardItem item in items)
                {
                    idlist.Add(item.ReportID.ToString());
                }
                Reports reports = new Reports(TSAuthentication.GetLoginUser());
                reports.LoadList(TSAuthentication.OrganizationID, TSAuthentication.UserID, idlist.ToArray());
                foreach (Report report in reports)
                {
                    result.Add(new ReportItem(report, true));
                }
            }

            return result.ToArray();
        }

        [WebMethod]
        public string GetDashboard()
        {
            string result = Settings.UserDB.ReadString("Dashboard", "[]");

            if (result == "[]")
            {
                try
                {
                    string[] ids = null;
                    List<DashboardItem> items = new List<DashboardItem>();
                    ids = Settings.UserDB.ReadString("DashboardPortlets").Split(',');
                    foreach (string id in ids)
                    {


                        OldPortlet oldPortlet = Settings.UserDB.ReadJson<OldPortlet>("DashboardPortlet-portlet" + id);
                        int width = 1;
                        if (oldPortlet != null)
                        {
                            try
                            {
                                if (oldPortlet.X > 0) width = 2;
                            }
                            catch (Exception)
                            {
                            }
                        }

                        items.Add(new DashboardItem(int.Parse(id), 1, width));
                    }
                    result = JsonConvert.SerializeObject(items.ToArray());
                    Settings.UserDB.WriteString("Dashboard", result);

                }
                catch (Exception)
                {

                    result = "[]";
                }
            }
            return result;
        }

        [WebMethod]
        public void SaveDashboard(string data)
        {
            Settings.UserDB.WriteString("Dashboard", data);
        }

        [WebMethod]
        public AutocompleteItem[] FindReport(string term)
        {
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            Reports reports = new Reports(TSAuthentication.GetLoginUser());
            reports.Search(TSAuthentication.OrganizationID, TSAuthentication.UserID, term, 10);
            foreach (Report report in reports)
            {
                result.Add(new AutocompleteItem(report.Name, report.ReportID.ToString()));
            }

            return result.ToArray();
        }

        [WebMethod]
        public string GetReports()
        {
            List<ReportItem> result = new List<ReportItem>();
            Reports reports = new Reports(TSAuthentication.GetLoginUser());
            reports.LoadAll(TSAuthentication.OrganizationID, TSAuthentication.UserID);
            foreach (Report report in reports)
            {
                result.Add(new ReportItem(report, false));
            }

            return JsonConvert.SerializeObject(result);
        }

        [WebMethod]
        public string GetStarredReports()
        {
            List<ReportItem> result = new List<ReportItem>();
            Reports reports = new Reports(TSAuthentication.GetLoginUser());
            reports.LoadAll(TSAuthentication.OrganizationID, TSAuthentication.UserID);
            foreach (Report report in reports)
            {
                if (report.Row["IsFavorite"] != DBNull.Value && (bool)report.Row["IsFavorite"] == true) result.Add(new ReportItem(report, false));
            }

            return JsonConvert.SerializeObject(result);
        }

        [WebMethod]
        public string GetNonStockReports()
        {
            List<ReportItem> result = new List<ReportItem>();
            Reports reports = new Reports(TSAuthentication.GetLoginUser());
            reports.LoadAll(TSAuthentication.OrganizationID, TSAuthentication.UserID);
            foreach (Report report in reports)
            {
                if (report.OrganizationID != null) result.Add(new ReportItem(report, false));
            }

            return JsonConvert.SerializeObject(result);
        }

        [WebMethod]
        public string GetAllReports()
        {
            List<ReportItem> result = new List<ReportItem>();
            Reports reports = new Reports(TSAuthentication.GetLoginUser());
            reports.LoadAll(TSAuthentication.OrganizationID, TSAuthentication.UserID);
            foreach (Report report in reports)
            {
                result.Add(new ReportItem(report, false));
            }

            return JsonConvert.SerializeObject(result);
        }

        [WebMethod]
        public string GetStockReports()
        {
            List<ReportItem> result = new List<ReportItem>();
            Reports reports = new Reports(TSAuthentication.GetLoginUser());
            reports.LoadAll(TSAuthentication.OrganizationID, TSAuthentication.UserID);
            foreach (Report report in reports)
            {
                if (report.OrganizationID == null) result.Add(new ReportItem(report, false));
            }

            return JsonConvert.SerializeObject(result);
        }

        [WebMethod]
        public string GetReportsByReportType(ReportType reportType)
        {
            List<ReportItem> result = new List<ReportItem>();
            Reports reports = new Reports(TSAuthentication.GetLoginUser());
            reports.LoadAll(TSAuthentication.OrganizationID, TSAuthentication.UserID);
            foreach (Report report in reports)
            {
                if (report.ReportDefType == reportType) result.Add(new ReportItem(report, false));
            }

            return JsonConvert.SerializeObject(result);
        }

        [WebMethod]
        public string GetTicketViews()
        {
            List<ReportItem> result = new List<ReportItem>();
            Reports reports = new Reports(TSAuthentication.GetLoginUser());
            reports.LoadAllTicketViews(TSAuthentication.OrganizationID, TSAuthentication.UserID);

            foreach (Report report in reports)
            {
                result.Add(new ReportItem(report, false));
            }

            return JsonConvert.SerializeObject(result);
        }

        [WebMethod]
        public string GetReportsByFolder(int folderID)
        {
            List<ReportItem> result = new List<ReportItem>();
            Reports reports = new Reports(TSAuthentication.GetLoginUser());
            reports.LoadAll(TSAuthentication.OrganizationID, TSAuthentication.UserID);
            foreach (Report report in reports)
            {
                if (report.Row["Folder"] != DBNull.Value && (int?)report.Row["Folder"] == folderID) result.Add(new ReportItem(report, false));
            }

            return JsonConvert.SerializeObject(result);
        }

        [WebMethod]
        public ReportItem GetReport(int reportID)
        {
            Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID, TSAuthentication.UserID);
            report.MigrateToNewReport();
            Reports.UpdateReportView(TSAuthentication.GetLoginUser(), reportID);
            return new ReportItem(report, true);
        }

        [WebMethod]
        public ReportProxy[] AdminGetReports()
        {
            if (TSAuthentication.OrganizationID != 1078) return null;
            Reports reports = new Reports(TSAuthentication.GetLoginUser());
            reports.LoadCustomReports();
            List<ReportProxy> result = new List<ReportProxy>();

            foreach (Report report in reports)
            {
                result.Add(report.GetProxy());
            }
            return result.ToArray();
        }

        [WebMethod]
        public ReportProxy AdminGetReport(int reportID)
        {
            if (TSAuthentication.OrganizationID != 1078) return null;
            Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID);
            return report.GetProxy();
        }

        [WebMethod]
        public int? AdminCloneReport(int reportID, string name)
        {
            if (TSAuthentication.UserID != 34 && TSAuthentication.UserID != 47 && TSAuthentication.UserID != 1839999) return null;
            Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID);
            return report.CloneReport(name);
        }

        [WebMethod]
        public void AdminRenameReport(int reportID, string name)
        {
            if (TSAuthentication.UserID != 34 && TSAuthentication.UserID != 47 && TSAuthentication.UserID != 1839999) return;
            Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID);
            report.Name = name;
            report.Collection.Save();
        }

        [WebMethod]
        public void AdminChangeOrg(int reportID, string orgID)
        {
            if (TSAuthentication.UserID != 34 && TSAuthentication.UserID != 47 && TSAuthentication.UserID != 1839999) return;
            Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID);
            report.OrganizationID = orgID.ToUpper() == "ALL" ? null : (int?)int.Parse(orgID);
            report.Collection.Save();
        }

        [WebMethod]
        public void AdminUpdateQuery(int reportID, string query)
        {
            if (TSAuthentication.UserID != 34 && TSAuthentication.UserID != 47 && TSAuthentication.UserID != 4759191) return;
            Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID);
            report.Query = query;
            report.Collection.Save();
        }

        [WebMethod]
        public void AdminTestQuery(int reportID, string query)
        {
            if (TSAuthentication.UserID != 34 && TSAuthentication.UserID != 47 && TSAuthentication.UserID != 4759191) return;
            if (!string.IsNullOrWhiteSpace(query))
            {
                Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID);
                string oldQuery = report.Query;
                try
                {
                    report.Query = query;
                    report.Collection.Save();
                    Reports.GetReportData(TSAuthentication.GetLoginUser(), reportID, 1, 10, "", true, false);
                }
                finally
                {
                    report.Query = oldQuery;
                    report.Collection.Save();
                }
            }


        }

        [WebMethod]
        public int[] DeleteReports(string reportIDs)
        {
            List<int> result = new List<int>();
            int[] ids = JsonConvert.DeserializeObject<int[]>(reportIDs);
            for (int i = 0; i < ids.Length; i++)
            {
                int reportID = ids[i];
                Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID);
                if (report.OrganizationID == null && TSAuthentication.IsSystemAdmin)
                {
                    Reports.HideStockReport(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID, reportID);
                    result.Add(reportID);
                }
                else if (report.OrganizationID == TSAuthentication.OrganizationID && (TSAuthentication.UserID == report.CreatorID || TSAuthentication.IsSystemAdmin))
                {
                    report.Delete();
                    report.Collection.Save();
                    result.Add(reportID);
                }
            }

            return result.ToArray();

        }

        [WebMethod]
        public int[] DeleteScheduledReports(string reportIDs)
        {
            List<int> result = new List<int>();
            int[] ids = JsonConvert.DeserializeObject<int[]>(reportIDs);
            for (int i = 0; i < ids.Length; i++)
            {
                int reportID = ids[i];
                ScheduledReport scheduledReport = ScheduledReports.GetScheduledReport(TSAuthentication.GetLoginUser(), reportID);
                if (scheduledReport != null && scheduledReport.OrganizationId == TSAuthentication.OrganizationID && (TSAuthentication.UserID == scheduledReport.CreatorId || TSAuthentication.IsSystemAdmin))
                {
                    scheduledReport.Delete();
                    scheduledReport.Collection.Save();
                    result.Add(reportID);
                }
            }

            return result.ToArray();

        }

        [WebMethod]
        public ReportItem CloneReport(int reportID)
        {
            Reports reports = new Reports(TSAuthentication.GetLoginUser());
            reports.LoadAll(TSAuthentication.OrganizationID);

            Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID);

            int i = 0;
            string reportName = report.Name + " Clone ({0:D})";

            while (true)
            {
                i++;
                if (reports.FindByName(string.Format(reportName, i)) == null) break;
            }

            int newID = report.CloneReport(string.Format(reportName, i));

            Report result = Reports.GetReport(TSAuthentication.GetLoginUser(), newID, TSAuthentication.UserID);
            return new ReportItem(result, false);
        }

        [WebMethod]
        public void SetFavorite(int reportID, bool value)
        {
            Reports.SetIsFavorite(TSAuthentication.GetLoginUser(), TSAuthentication.UserID, reportID, value);
        }

        [WebMethod]
        public void SetHidden(int reportID, bool value)
        {
            Reports.SetIsHiddenFromUser(TSAuthentication.GetLoginUser(), TSAuthentication.UserID, reportID, value);
        }

        [WebMethod]
        public ReportCategoryItem[] GetCategories()
        {
            List<ReportCategoryItem> result = new List<ReportCategoryItem>();
            ReportTables tables = new ReportTables(TSAuthentication.GetLoginUser());
            tables.LoadCategories();

            foreach (ReportTable table in tables)
            {
                ReportCategoryItem item = new ReportCategoryItem();
                item.Name = table.Alias;
                item.ReportTableID = table.ReportTableID;
                ReportSubcategories cats = new ReportSubcategories(TSAuthentication.GetLoginUser());
                cats.LoadByReportTableID(table.ReportTableID);

                List<ReportSubcateoryItem> subs = new List<ReportSubcateoryItem>();
                foreach (ReportSubcategory cat in cats)
                {
                    ReportSubcateoryItem sub = new ReportSubcateoryItem();
                    sub.SubCatID = cat.ReportSubcategoryID;
                    sub.Name = cat.ReportTableID != null ? cat.Row["Alias"].ToString() : "None";
                    subs.Add(sub);
                }
                item.Subcategories = subs.ToArray();
                result.Add(item);
            }
            return result.ToArray();
        }

        [WebMethod]
        public ReportFieldItem[] GetFields(int reportSubCatID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            List<ReportFieldItem> result = new List<ReportFieldItem>();
            TicketTypes ticketTypes = new TicketTypes(loginUser);
            ticketTypes.LoadAllPositions(loginUser.OrganizationID);

            ReportSubcategory subCat = ReportSubcategories.GetReportSubcategory(loginUser, reportSubCatID);
            ReportTable primaryTable = ReportTables.GetReportTable(loginUser, subCat.ReportCategoryTableID);

            ReportTableFields reportTableFields = new ReportTableFields(loginUser);
            reportTableFields.LoadByReportTableID(subCat.ReportCategoryTableID);

            CRMLinkTable crmLink = new CRMLinkTable(loginUser);
            crmLink.LoadByOrganizationID(loginUser.OrganizationID);
            bool isJiraActive = crmLink.Where(p => p.CRMType.ToLower() == "jira" && p.Active).Any();
            List<string> jiraFields = new List<string>() { "DateModifiedByJiraSync", "JiraID", "SyncWithJira", "JiraKey", "JiraLinkURL", "JiraStatus" };

            foreach (ReportTableField reportTableField in reportTableFields)
            {
                if ((isJiraActive && jiraFields.Where(p => p == reportTableField.FieldName).Any()
                    || !jiraFields.Where(p => p == reportTableField.FieldName).Any()))
                {
                    result.Add(new ReportFieldItem(primaryTable.Alias, true, reportTableField));
                }
            }

            if (primaryTable.CustomFieldRefType != ReferenceType.None)
            {
                CustomFields customFields = new CustomFields(loginUser);
                customFields.LoadByReferenceType(loginUser.OrganizationID, primaryTable.CustomFieldRefType, null, "Name");

                foreach (CustomField customField in customFields)
                {
                    if (customField.RefType == ReferenceType.Tickets || customField.RefType == ReferenceType.Actions)
                    {
                        TicketType ticketType = ticketTypes.FindByTicketTypeID(customField.AuxID);
                        if (ticketType != null) result.Add(new ReportFieldItem(primaryTable.Alias, true, customField, ticketType.Name));
                    }
                    else
                    {
                        result.Add(new ReportFieldItem(primaryTable.Alias, true, customField, ""));
                    }

                }
            }

            if (subCat.ReportTableID != null)
            {
                ReportTable subTable = ReportTables.GetReportTable(loginUser, (int)subCat.ReportTableID);
                reportTableFields = new ReportTableFields(loginUser);
                reportTableFields.LoadByReportTableID((int)subCat.ReportTableID);
                foreach (ReportTableField reportTableField in reportTableFields)
                {
                    result.Add(new ReportFieldItem(subTable.Alias, false, reportTableField));
                }


                if (subTable.CustomFieldRefType != ReferenceType.None)
                {
                    CustomFields customFields = new CustomFields(loginUser);
                    customFields.LoadByReferenceType(loginUser.OrganizationID, subTable.CustomFieldRefType, null, "Name");

                    foreach (CustomField customField in customFields)
                    {
                        if (customField.RefType == ReferenceType.Tickets || customField.RefType == ReferenceType.Actions)
                        {
                            TicketType ticketType = ticketTypes.FindByTicketTypeID(customField.AuxID);
                            if (ticketType != null) result.Add(new ReportFieldItem(subTable.Alias, false, customField, ticketType.Name));
                        }
                        else
                        {
                            result.Add(new ReportFieldItem(subTable.Alias, false, customField, ""));
                        }
                    }
                }
            }
            return result.ToArray();
        }

        [WebMethod]
        public ReportItem SaveReport(int? reportID, string name, int reportType, string data, bool isStock, bool isPrivate = false)
        {
            Report report = null;
            if (reportID == null)
            {
                report = (new Reports(TSAuthentication.GetLoginUser())).AddNewReport();
            }
            else
            {
                report = Reports.GetReport(TSAuthentication.GetLoginUser(), (int)reportID);
                if (!TSAuthentication.IsSystemAdmin && report.CreatorID != TSAuthentication.UserID) return null;
            }

            if (isStock)
            {
                if (TSAuthentication.UserID != 34 && TSAuthentication.UserID != 43 && TSAuthentication.UserID != 47 && TSAuthentication.UserID != 1839999) return null;
                report.OrganizationID = null;
            }
            else
            {
                report.OrganizationID = TSAuthentication.OrganizationID;
            }

            report.Name = name;
            report.ReportDef = data;
            report.EditorID = TSAuthentication.UserID;
            report.DateEdited = DateTime.UtcNow;
            report.IsPrivate = isPrivate;

            switch (reportType)
            {
                case 0: report.ReportDefType = ReportType.Table; break;
                case 1: report.ReportDefType = ReportType.Chart; break;
                case 2: report.ReportDefType = ReportType.External; break;
                case 4: report.ReportDefType = ReportType.Summary; break;
                case 5: report.ReportDefType = ReportType.TicketView; break;
                default:
                    break;
            }

            report.Collection.Save();
            return new ReportItem(report, true);
        }

        [WebMethod]
        public void SaveReportDef(int reportID, string data)
        {
            Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID);
            if (report.OrganizationID == null) return;
            if (!TSAuthentication.IsSystemAdmin && report.CreatorID != TSAuthentication.UserID) return;
            report.EditorID = TSAuthentication.UserID;
            report.DateEdited = DateTime.UtcNow;
            report.ReportDef = data;
            report.Collection.Save();
        }

        [WebMethod]
        public void SaveUserSettings(int reportID, string data)
        {
            Reports.SetUserSettings(TSAuthentication.GetLoginUser(), TSAuthentication.UserID, reportID, data);
        }

        [WebMethod]
        public ReportFolderProxy[] GetFolders()
        {
            ReportFolders folders = new ReportFolders(TSAuthentication.GetLoginUser());
            folders.LoadAll(TSAuthentication.OrganizationID);
            return folders.GetReportFolderProxies();
        }

        [WebMethod]
        public ReportFolderProxy SaveFolder(int? folderID, string name)
        {
            ReportFolder folder = null;
            if (folderID == null)
            {
                folder = (new ReportFolders(TSAuthentication.GetLoginUser())).AddNewReportFolder();
                folder.OrganizationID = TSAuthentication.OrganizationID;
                folder.CreatorID = TSAuthentication.UserID;
            }
            else
            {
                folder = ReportFolders.GetReportFolder(TSAuthentication.GetLoginUser(), (int)folderID);
                if (!TSAuthentication.IsSystemAdmin && folder.CreatorID != TSAuthentication.UserID) return folder.GetProxy();
            }

            folder.Name = name.Trim();
            folder.Collection.Save();
            return folder.GetProxy();
        }

        [WebMethod]
        public bool DeleteFolder(int folderID) {
            ReportFolder folder = ReportFolders.GetReportFolder(TSAuthentication.GetLoginUser(), (int)folderID);
            if (TSAuthentication.IsSystemAdmin || folder.CreatorID == TSAuthentication.UserID) {
                try {
                    Reports.UnassignFolder(TSAuthentication.GetLoginUser(), folderID);
                    folder.Delete();
                    folder.Collection.Save();
                    return true;
                }
                catch (Exception ex) {
                    return false;
                }
            } else {
                return false;
            }
        }

        [WebMethod]
        public void MoveReports(string reportIDs, int? folderID)
        {
            if (folderID != null)
            {
                ReportFolder folder = ReportFolders.GetReportFolder(TSAuthentication.GetLoginUser(), (int)folderID);
                if (folder.OrganizationID != TSAuthentication.OrganizationID) return;
            }
            int[] ids = JsonConvert.DeserializeObject<int[]>(reportIDs);
            for (int i = 0; i < ids.Length; i++)
            {
                Reports.AssignFolder(TSAuthentication.GetLoginUser(), folderID, TSAuthentication.OrganizationID, ids[i]);
            }
        }

        [WebMethod]
        public AutocompleteItem[] GetLookupDisplayNames(int reportTableFieldID, string term)
        {
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            Dictionary<int, string> values = GetLookupValues(TSAuthentication.GetLoginUser(), reportTableFieldID, term, 10);
            if (values != null)
            {
                foreach (KeyValuePair<int, string> pair in values)
                {
                    bool found = false;
                    foreach (AutocompleteItem item in result)
                    {
                        if (item.label.ToLower().Trim() == pair.Value.ToLower().Trim())
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found) result.Add(new AutocompleteItem(pair.Value, pair.Key.ToString()));
                }
            }
            return result.ToArray();
        }

        public static Dictionary<int, string> GetLookupValues(LoginUser loginUser, int reportTableFieldID, string term, int maxRows)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            ReportTableField field = ReportTableFields.GetReportTableField(loginUser, reportTableFieldID);
            if (field == null || field.LookupTableID == null) return null;
            ReportTable table = ReportTables.GetReportTable(loginUser, (int)field.LookupTableID);
            SqlCommand command = new SqlCommand();
            string[] orgs = table.OrganizationIDFieldName.Split(',');
            StringBuilder orgFields = new StringBuilder("(");
            foreach (String s in orgs)
            {
                if (orgFields.Length > 1)
                {
                    orgFields.Append(" OR " + s + " = @OrganizationID");
                }
                else
                {
                    orgFields.Append(s + " = @OrganizationID");
                }

            }
            orgFields.Append(")");

            string text = "SELECT TOP {0} {1} AS Label, {2} AS ID FROM {3} WHERE {4} AND {1} LIKE '%' + @Term + '%' ORDER BY {5}";
            command.CommandText = string.Format(text,
              maxRows.ToString(),
              table.LookupDisplayClause,
              table.LookupKeyFieldName,
              table.TableName,
              orgFields.ToString(),
              table.LookupOrderBy);

            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@Term", term);
            command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
            DataTable dataTable = SqlExecutor.ExecuteQuery(loginUser, command);

            if (field.LookupTableID == 11 || field.LookupTableID == 17)
            {
                result.Add(-2, "The Report Viewer");
            }
            //result.Add(-1, "Unassigned");
            foreach (DataRow row in dataTable.Rows)
            {
                result.Add((int)row[1], row[0].ToString());
            }

            return result;
        }

        //Scheduled Report methods
        [WebMethod]
        public string GetScheduledReports()
        {
            List<ScheduledReportItem> result = new List<ScheduledReportItem>();
            ScheduledReports reports = new ScheduledReports(TSAuthentication.GetLoginUser());

            try
            {
                reports.LoadAll(TSAuthentication.OrganizationID);

                foreach (ScheduledReport report in reports)
                {
                    result.Add(new ScheduledReportItem(report));
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "ReportService.GetScheduledReports");
            }

            return JsonConvert.SerializeObject(result);
        }

        [WebMethod]
        public ScheduledReportProxy GetScheduledReport(int id)
        {
            ScheduledReportProxy proxy = new ScheduledReportProxy();
            try
            {
                ScheduledReport scheduledReport = ScheduledReports.GetScheduledReport(TSAuthentication.GetLoginUser(), id);
                proxy = scheduledReport.GetProxy();
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "ReportService.GetScheduledReport");
            }

            return proxy;
        }

        [WebMethod]
        public int SaveScheduledReport(int scheduledReportId,
                                            int reportId,
                                            string emailSubject,
                                            string emailBody,
                                            string emailAddresses,
                                            Object startOn,
                                            ScheduledReportFrequency frequency,
                                            int every,
                                            int weekday,
                                            int dayOfMonth,
                                            bool isActive)
        {
            try
            {
                LoginUser loginUser = TSAuthentication.GetLoginUser();
                ScheduledReports scheduledReports = new ScheduledReports(loginUser);

                if (scheduledReportId == 0)
                {
                    ScheduledReport scheduledReport = scheduledReports.AddNewScheduledReport();
                    scheduledReport.ReportId = reportId;
                    scheduledReport.EmailSubject = emailSubject;
                    scheduledReport.EmailBody = emailBody;
                    scheduledReport.EmailRecipients = emailAddresses;
                    scheduledReport.OrganizationId = TSAuthentication.OrganizationID;
                    scheduledReport.IsActive = true;
                    scheduledReport.StartDate = (DateTime)startOn;
                    scheduledReport.RecurrencyId = (byte)frequency;
                    scheduledReport.Every = (byte)every;
                    scheduledReport.Weekday = (byte)weekday;
                    scheduledReport.Monthday = (byte)dayOfMonth;
                    scheduledReport.IsActive = isActive;
                    scheduledReport.FilePathID = 3;

                    //This needs to be set before SetNextRun because that one needs it to get the timezone to use.
                    scheduledReport.CreatorId = loginUser.UserID;

                    if (isActive)
                    {
                        scheduledReport.SetNextRun();
                    }
                    else
                    {
                        scheduledReport.NextRun = null;
                    }

                    scheduledReport.Collection.Save();

                    scheduledReportId = scheduledReport.Id;
                }
                else
                {
                    scheduledReports.LoadById(scheduledReportId);

                    if (scheduledReports != null && scheduledReports.Any())
                    {
                        scheduledReports[0].EmailSubject = emailSubject;
                        scheduledReports[0].EmailBody = emailBody;
                        scheduledReports[0].EmailRecipients = emailAddresses;
                        scheduledReports[0].IsActive = true;
                        try
                        { scheduledReports[0].StartDate = (DateTime)startOn; }
                        catch (Exception ex) { }
                        scheduledReports[0].RecurrencyId = (byte)frequency;
                        scheduledReports[0].Every = (byte)every;
                        scheduledReports[0].Weekday = (byte)weekday;
                        scheduledReports[0].Monthday = (byte)dayOfMonth;
                        scheduledReports[0].IsActive = isActive;

                        if (isActive)
                        {
                            scheduledReports[0].SetNextRun();
                        }
                        else
                        {
                            scheduledReports[0].NextRun = null;
                        }

                        scheduledReports[0].ModifierId = loginUser.UserID;
                        scheduledReports.Save();
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "ReportService.SaveScheduledReport");
            }

            return scheduledReportId;
        }

        [WebMethod]
        public bool SetScheduledReportIsActive(int scheduledReportId, bool isActive)
        {
            bool isSuccessful = false;

            try
            {
                LoginUser loginUser = TSAuthentication.GetLoginUser();
                ScheduledReports scheduledReports = new ScheduledReports(loginUser);
                scheduledReports.LoadById(scheduledReportId);

                if (scheduledReports != null && scheduledReports.Any())
                {
                    scheduledReports[0].IsActive = isActive;

                    if (!isActive)
                    {
                        scheduledReports[0].NextRun = null;
                    }
                    else
                    {
                        scheduledReports[0].SetNextRun();
                    }

                    scheduledReports[0].ModifierId = loginUser.UserID;
                    scheduledReports.Save();
                    isSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                isSuccessful = false;
                ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "ReportService.SetScheduledReportIsActive");
            }

            return isSuccessful;
        }

        [DataContract]
        public class ReportFieldItem
        {
            public ReportFieldItem() { }
            public ReportFieldItem(string table, bool isPrimary, ReportTableField field)
            {
                this.IsPrimary = isPrimary;
                this.ID = field.ReportTableFieldID;
                this.Name = field.Alias;
                this.LookupTableID = field.LookupTableID;
                this.IsCustom = false;
                this.Table = table;

                switch (field.DataType)
                {
                    case "datetime": this.DataType = "datetime"; break;
                    case "bit": this.DataType = "bool"; break;
                    case "float": this.DataType = "number"; break;
                    case "int": this.DataType = "number"; break;
                    default: this.DataType = "text"; break;
                }

            }

            public ReportFieldItem(string table, bool isPrimary, CustomField field, string AuxName)
            {
                this.IsPrimary = isPrimary;
                this.ID = field.CustomFieldID;
                this.Name = field.Name;
                this.ListValues = string.IsNullOrWhiteSpace(field.ListValues) ? null : field.ListValues.Split('|');
                this.IsCustom = true;
                this.AuxName = AuxName;
                this.Table = table;
                switch (field.FieldType)
                {
                    case CustomFieldType.Date:
                    case CustomFieldType.Time:
                    case CustomFieldType.DateTime: this.DataType = "datetime"; break;
                    case CustomFieldType.Boolean: this.DataType = "bool"; break;
                    case CustomFieldType.Number: this.DataType = "number"; break;
                    default: this.DataType = "text"; break;
                }

            }

            [DataMember]
            public int ID { get; set; }
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public string Table { get; set; }
            [DataMember]
            public string AuxName { get; set; }
            [DataMember]
            public string DataType { get; set; }
            [DataMember]
            public int? LookupTableID { get; set; }
            [DataMember]
            public string[] ListValues { get; set; }
            [DataMember]
            public bool IsCustom { get; set; }
            [DataMember]
            public bool IsPrimary { get; set; }

        }

        [DataContract]
        public class ReportCategoryItem
        {
            [DataMember]
            public int ReportTableID { get; set; }
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public ReportSubcateoryItem[] Subcategories { get; set; }
        }

        [DataContract]
        public class ReportSubcateoryItem
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public int SubCatID { get; set; }
        }




        [DataContract]
        public class DashboardItem
        {
            public DashboardItem() { }

            public DashboardItem(int reportID, int rows, int columns)
            {
                this.ReportID = reportID;
                this.Rows = rows;
                this.Columns = columns;
            }

            [DataMember]
            public int ReportID { get; set; }
            [DataMember]
            public int Rows { get; set; }
            [DataMember]
            public int Columns { get; set; }
        }

        [Serializable]
        public class OldPortlet
        {
            public string ID { get; set; }
            public int ReportID { get; set; }
            public string Caption { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public int Height { get; set; }
            public bool IsOpen { get; set; }
            public string Html { get; set; }

        }
    }


}
