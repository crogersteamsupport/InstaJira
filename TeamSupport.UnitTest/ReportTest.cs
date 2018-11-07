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

namespace TeamSupport.UnitTest
{


    [TestClass]
    public class ReportTest
    {

        [TestMethod]
        void VerifyFieldNamesEnum()
        {
            string userData = AttachmentTest._userScot;
            AuthenticationModel authentication = AuthenticationModel.AuthenticationModelTest(userData, AttachmentTest._connectionString);
            using (ConnectionContext connection = new ConnectionContext(authentication))
            {
                // hard coded fields for ReportTicketsView
                List<string> fieldNames = new List<string>();
                foreach (EField field in Enum.GetValues(typeof(EField)))
                    fieldNames.Add(field.ToString());
                fieldNames.Sort();

                // DB configured fields for ReportTicketsView
                int reportTicketsViewID = connection._db.ExecuteQuery<int>("SELECT ReportTableID FROM ReportTables WHERE TableName='ReportTicketsView").Min();
                List<string> testNames = connection._db.ExecuteQuery<string>($"SELECT FieldName FROM ReportTableFields WHERE ReportTableID={reportTicketsViewID}").ToList();
                testNames.Sort();

                // Are they the same?  Was one updated but not the other?
                CollectionAssert.AreEqual(fieldNames, testNames);
                Assert.AreEqual(JsonConvert.SerializeObject(fieldNames), JsonConvert.SerializeObject(testNames));
            }
        }

        /*
        static string QueryString()
        {
            Query query = new Query();
            foreach (EField field in Enum.GetValues(typeof(EField)))
                query.Add(field);

            return query.AsSql(1078);
        }

        static string QueryStringNonDateConversion()
        {
            Query query = new Query();
            query.Add(EField.DateCreated);  // DaysOpened, MinutesOpened
            query.Add(EField.DateClosed);   // DaysClosed, MinutesClosed
            query.Add(EField.SlaViolationDate); // SlaViolationTime, SlaViolationHours
            query.Add(EField.SlaWarningDate);   // SlaWarningTime, SlaWarningHours

            query.Add(EField.SlaViolationTimeClosed);
            //query.Add(EField.SlaViolationLastAction);
            //query.Add(EField.SlaViolationInitialResponse);
            //query.Add(EField.SlaWarningTimeClosed);
            //query.Add(EField.SlaWarningLastAction);
            //query.Add(EField.SlaWarningInitialResponse);

            return query.AsSql(1078);
        }

        //DECLARE @From Int; SET @From = 1;
        //DECLARE @To Int; SET @To = 10000001;
        //DECLARE @Param00003 NVarChar; SET @Param00003 = '';
        //DECLARE @OrganizationID Int; SET @OrganizationID = 512739;
        //DECLARE @Self NVarChar; SET @Self = 'Justin Potter';
        //DECLARE @SelfID Int; SET @SelfID = 1083447;
        //DECLARE @UserID Int; SET @UserID = 1083447;
        //DECLARE @Offset NVarChar; SET @Offset = '-07:00';
        //WITH q AS( SELECT ReportTicketsView.UserName AS [Assigned To], ReportTicketsView.Customers AS [Customers], CAST(SWITCHOFFSET(TODATETIMEOFFSET(ReportTicketsView.DateCreated, '+00:00'), '-07:00') AS DATETIME) AS[Date Ticket Created], ReportTicketsView.Name AS[Ticket Name], ReportTicketsView.TicketNumber AS[Ticket Number], (SELECT CAST(NULLIF(RTRIM(CustomValue), '') AS varchar(8000)) FROM CustomValues WHERE(CustomFieldID = 9963) AND(RefID = ReportTicketsView.TicketID))  AS[Product Issue(Support)], (SELECT CAST(NULLIF(RTRIM(CustomValue), '') AS varchar(8000)) FROM CustomValues WHERE(CustomFieldID = 9961) AND(RefID = ReportTicketsView.TicketID))  AS[Reseller(Support)], (SELECT CAST(NULLIF(RTRIM(CustomValue), '') AS varchar(8000)) FROM CustomValues WHERE(CustomFieldID = 21881) AND(RefID = ReportTicketsView.TicketID))  AS[Temperature Check(Support)], (SELECT CAST(NULLIF(RTRIM(CustomValue), '') AS varchar(8000)) FROM CustomValues WHERE(CustomFieldID = 9962) AND(RefID = ReportTicketsView.TicketID))  AS[Time to Resolve(Support)], ROW_NUMBER() OVER(ORDER BY CAST(SWITCHOFFSET(TODATETIMEOFFSET(ReportTicketsView.DateCreated, '+00:00'), '-07:00') AS DATETIME) DESC) AS[RowNum] FROM ReportTicketsView WHERE(ReportTicketsView.OrganizationID = @OrganizationID) AND((DATEPART(year, CAST(SWITCHOFFSET(TODATETIMEOFFSET(ReportTicketsView.[DateCreated], '+00:00'), '-07:00') AS DATETIME)) = 2018) AND((SELECT CAST(NULLIF(RTRIM(CustomValue), '') AS varchar(8000)) FROM CustomValues WHERE(CustomFieldID = 9963) AND(RefID = ReportTicketsView.TicketID))  <> @Param00003))) SELECT* FROM q WHERE RowNum BETWEEN @From AND @To ORDER BY RowNum ASC

        static string QueryString49907()
        {
            Query query = new Query();
            query.Add(EField.UserName);  // DaysOpened, MinutesOpened
            query.Add(EField.Customers);   // DaysClosed, MinutesClosed
            query.Add(EField.DateCreated); // SlaViolationTime, SlaViolationHours
            query.Add(EField.Name);   // SlaWarningTime, SlaWarningHours
            query.Add(EField.TicketID);
            query.Add(EField.TicketNumber);

            return query.AsSql(512739);
        }

        static string QueryString57964()
        {
            Query query = new Query();
            query.Add(EField.UserName);
            query.Add(EField.Contacts);
            query.Add(EField.DateClosed);
            query.Add(EField.DateCreated);
            query.Add(EField.GroupName);
            query.Add(EField.IsClosed);
            query.Add(EField.Status);
            query.Add(EField.Name);
            query.Add(EField.TicketNumber);
            query.Add(EField.TicketTypeName);
            query.Add(EField.TicketID);
            return query.AsSql(414323); // EField.OrganizationID
        }

        static string QueryString32147()
        {
            Query query = new Query();
            query.Add(EField.UserName);
            query.Add(EField.TicketNumber);
            query.Add(EField.TicketID);
            query.Add(EField.GroupName);
            query.Add(EField.TicketNumber);
            query.Add(EField.TicketNumber);
            return query.AsSql(1085741); // EField.OrganizationID
        }

        static string QueryString57460()
        {
            Query query = new Query();
            query.Add(EField.Name);
            query.Add(EField.TicketTypeName);
            query.Add(EField.TicketID);

            return query.AsSql(641823); // EField.OrganizationID
        }

        static string QueryString55737()
        {
            Query query = new Query();
            query.Add(EField.Customers);
            query.Add(EField.DaysOpened);
            query.Add(EField.Severity);
            query.Add(EField.TicketID);
            query.Add(EField.ProductName);
            query.Add(EField.TicketTypeName);

            return query.AsSql(1081853); // EField.OrganizationID
        }

        static string QueryString56958()
        {
            Query query = new Query();
            query.Add(EField.UserName);
            query.Add(EField.DateCreated);
            query.Add(EField.Name);
            query.Add(EField.TicketNumber);
            query.Add(EField.TicketID);
            query.Add(EField.IsClosed);

            return query.AsSql(420794); // EField.OrganizationID
        }

        static string QueryString36592()
        {
            Query query = new Query();
            query.Add(EField.CreatorEmail);
            query.Add(EField.DateCreated);
            query.Add(EField.TicketID);
            return query.AsSql(464464); // EField.OrganizationID
        }
        */



        [TestMethod]
        public void ReportTicketsViewTest()
        {
            //string fields = "{ \"Fields\":[{\"FieldID\":562,\"IsCustom\":false},{\"FieldID\":492,\"IsCustom\":false},{\"FieldID\":494,\"IsCustom\":false},{\"FieldID\":524,\"IsCustom\":false},{\"FieldID\":555,\"IsCustom\":false},{\"FieldID\":9963,\"IsCustom\":true},{\"FieldID\":9961,\"IsCustom\":true},{\"FieldID\":21881,\"IsCustom\":true},{\"FieldID\":9962,\"IsCustom\":true}],\"Subcategory\":\"47\",\"Filters\":[{\"Conjunction\":\"AND\",\"Conditions\":[{\"FieldID\":494,\"IsCustom\":false,\"FieldName\":\"Date Ticket Created\",\"DataType\":\"datetime\",\"Comparator\":\"CURRENT YEAR\"},{\"FieldID\":9963,\"IsCustom\":true,\"FieldName\":\"Product Issue\",\"DataType\":\"text\",\"Comparator\":\"IS NOT\",\"Value1\":\"\"}],\"Filters\":[]}]}";

            string userData = AttachmentTest._userScot;
            AuthenticationModel authentication = AuthenticationModel.AuthenticationModelTest(userData, AttachmentTest._connectionString);
            using (ConnectionContext connection = new ConnectionContext(authentication))
            {
                LoginUser loginUser = new LoginUser(AttachmentTest._connectionString, 552821, 464464, (IDataCache)null);    // UserID, OrganizationID
                Report report = Reports.GetReport(loginUser, 36592);    // ReportID 
                ReportTablePage tabular = new ReportTablePage(loginUser, report);

                ReportTicketsViewTempTable.Enable = false;
                DataTable original = tabular.GetReportTablePage(0, 99, "Email", false, true, true);

                ReportTicketsViewTempTable.Enable = true;
                DataTable withTempTable = tabular.GetReportTablePage(0, 99, "Email", false, true, true);

                Assert.AreEqual(original, withTempTable);

                //SqlCommand command = new SqlCommand();
                //tabular.GetCommand(command, true, false, true, null, null, 120);
                //tabular.AddReportTicketsViewTempTable(command);


                //ReportTicketsViewItemProxy[] report = connection._db.ExecuteQuery<ReportTicketsViewItemProxy>(queryString).ToArray();

                Table<ReportTicketsViewItemProxy> table = connection._db.GetTable<ReportTicketsViewItemProxy>();
                //ReportTicketsViewItemProxy[] proxies = table.Where(t => t.TicketID < 100000).OrderBy(t => t.TicketID).ToArray();
                ReportTicketsViewItemProxy[] proxies = (from t in table select t).Where(t => t.SlaViolationTimeClosed.HasValue).Take(1000).ToArray();
                DateTime now = DateTime.UtcNow;
                foreach (ReportTicketsViewItemProxy proxy in proxies)
                {
                    ReportTicketsViewItemProxy clone = JsonConvert.DeserializeObject<ReportTicketsViewItemProxy>(JsonConvert.SerializeObject(proxy));
                    Assert.AreEqual(JsonConvert.SerializeObject(proxy), JsonConvert.SerializeObject(clone));

                    //clone.CalculateDate(ReportTicketsViewItemProxy.EField.DaysClosed, now);
                    //clone.CalculateDate(ReportTicketsViewItemProxy.EField.MinutesClosed, now);
                    //clone.CalculateDate(ReportTicketsViewItemProxy.EField.DaysOpened, now);
                    //clone.CalculateDate(ReportTicketsViewItemProxy.EField.MinutesOpened, now);
                    //clone.CalculateDate(ReportTicketsViewItemProxy.EField.SlaViolationTime, now);
                    //clone.CalculateDate(ReportTicketsViewItemProxy.EField.SlaWarningTime, now);
                    //clone.CalculateDate(ReportTicketsViewItemProxy.EField.SlaViolationHours, now);
                    //clone.CalculateDate(ReportTicketsViewItemProxy.EField.SlaWarningHours, now);

                    Assert.AreEqual(JsonConvert.SerializeObject(proxy), JsonConvert.SerializeObject(clone));
                }

                //Assert.AreEqual(JsonConvert.SerializeObject(proxies), JsonConvert.SerializeObject(report));
            }


        }

        const decimal _tolerance = 2;

        void EpsilonCompare(int? value1, int? value2)
        {
            if (value1.Equals(value2))
                return;
            if (value1.HasValue)
                value1 = (int)Math.Floor(value1.Value / _tolerance);
            if (value2.HasValue)
                value2 = (int)Math.Floor(value2.Value / _tolerance);
            Assert.AreEqual(value1, value2);
        }

        void EpsilonCompare(decimal? value1, decimal? value2)
        {
            if (value1.Equals(value2))
                return;
            if (value1.HasValue)
                value1 = (decimal)Math.Floor(value1.Value / _tolerance);
            if (value2.HasValue)
                value2 = (decimal)Math.Floor(value2.Value / _tolerance);
            Assert.AreEqual(value1, value2);
        }

        public void ReportTicketsViewOrganizationTest()
        {
            string userData = AttachmentTest._userScot;
            AuthenticationModel authentication = AuthenticationModel.AuthenticationModelTest(userData, AttachmentTest._connectionString);
            using (ConnectionContext connection = new ConnectionContext(authentication))
            {

            }

        }

        public void ReportTicketsViewOrganizationFieldTest()
        {

        }
    }
}
