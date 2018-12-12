using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamSupport.IDTree;
using TeamSupport.Data;
using TeamSupport.DataAPI;
using TeamSupport.ModelAPI;
using System.Data.Linq;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using TeamSupport.Data.BusinessObjects.Reporting;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Diagnostics;

namespace TeamSupport.UnitTest
{
    [TestClass]
    public class ReportTest
    {
        [TestMethod]
        public void ReportTicketsViewTest()
        {
            string userData = AttachmentTest._userScot;
            AuthenticationModel authentication = AuthenticationModel.AuthenticationModelTest(userData, AttachmentTest._connectionString);
            using (ConnectionContext connection = new ConnectionContext(authentication))
            {
                // only actually runs for valid reports where optimization applies
                int[] reportIDs = connection._db.ExecuteQuery<int>("SELECT TOP 1000 ReportID FROM Reports ORDER BY LastTimeTaken DESC").ToArray();
                foreach (int reportID in reportIDs)
                    TestReportID(reportID);
            }
        }

        bool TestReportID(int reportID)
        {
            // NOTE: PAGESIZE = 50 thus getData(0, 99, ...)
            // req = getData(fromPage * PAGESIZE, (toPage * PAGESIZE) + PAGESIZE - 1, sortcol, (sortdir < 1)

            // run as author of report
            LoginUser loginUser = new LoginUser(AttachmentTest._connectionString, 4787299, 1078, (IDataCache)null);    // Scot, TeamSupport
            Report report = Reports.GetReport(loginUser, reportID);

            // valid report to run?
            if ((report == null) || (report.ReportDef == null) ||
                (report.ReportDefType < 0) ||
                !report.OrganizationID.HasValue ||
                report.IsDisabled ||
                (report.ReportDefType == ReportType.Custom))
                return false;

            loginUser = new LoginUser(AttachmentTest._connectionString, report.ModifierID, report.OrganizationID.Value, (IDataCache)null);

            // run the query
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //ReportTicketsViewTempTable.Enable = false;
            if (TryGet(loginUser, report, out DataTable original)) // if original query does throw, don't try optimized
            {
                // run it a second time using temp table #ReportTicketsView
                Debug.WriteLine("");
                Debug.Write($"{reportID},{stopwatch.ElapsedMilliseconds / 1000f},");
                //stopwatch.Restart();
                //ReportTicketsViewTempTable.Enable = true;
                //TryGet(loginUser, report, out DataTable optimized);
                //Debug.WriteLine($", {stopwatch.ElapsedMilliseconds / 1000f}");

                //// same results?
                //if (!ObjectCompare.AreEqual(original, optimized))
                //    Debugger.Break();
            }

            return true;
        }

        bool TryGet(LoginUser loginUser, Report report, out DataTable table)
        {
            // parameters passed in from front end
            int from = 0;
            int to = 10000000;  // default to full data set so comparison works
            string sortField = String.Empty;
            bool isDesc = false;
            bool useUserFilter = false;

            try
            {
                GridResult result = report.GetReportData(loginUser, from, to, sortField, isDesc, useUserFilter);
                table = (DataTable)result.Data;
            }
            catch (Exception ex)
            {
                table = null;
                return false;
            }

            return true;
        }

    }
}
