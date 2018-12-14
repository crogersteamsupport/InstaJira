using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using System.Text;
using TeamSupport.Data.WebHooks;
using System.Web.Services;
using TeamSupport.WebUtils;
using Newtonsoft.Json;
using System.Dynamic;

public partial class ServiceStatus : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        List<object> failures = new List<object>();

        StringBuilder rowBuilder = new StringBuilder();

        bool isFailed = false;
        Services services = new Services(LoginUser.Anonymous);
        services.LoadAll();
        isFailed = false;
        foreach (Service service in services)
        {
            bool isGood = true;
            if (service.LastStartTime == null || !service.Enabled || service.HealthMaxMinutes < 1) continue;
            double timeSinceCheck = DateTime.Now.Subtract((DateTime)service.Row["HealthTime"]).TotalMinutes;
            if ( timeSinceCheck > (service.HealthMaxMinutes + 10))
            {
                isFailed = true;
                isGood = false;
            }
            
            AddStatusRow(rowBuilder, service.Row["Name"].ToString(), isGood, (DateTime)service.Row["HealthTime"], service.HealthMaxMinutes);

            if (timeSinceCheck > service.HealthMaxMinutes && service.AutoStart)
            {
                failures.Add(GetServiceObject(service.AssemblyName, service.NameSpace));
            }
        }

        litStatusRows.Text = rowBuilder.ToString();


        rowBuilder.Clear();

        isFailed = CheckCount(isFailed, failures, 100, "TSEmailSender", FindServiceHost(services, "TSEmailSender"), rowBuilder, 500, "Outbound Email Delayed", "There are outgoing email delays of more than 10 minutes, check EmailSender service and Socket Labs", @"
SELECT COUNT(*)
FROM Emails e 
WHERE IsWaiting = 1 
AND e.Attempts < 1
AND DATEDIFF(MINUTE, e.DateCreated, GETUTCDATE()) > 10
");

        isFailed = CheckCount(isFailed, failures, 1000, "TSEmailSender", FindServiceHost(services, "TSEmailSender"), rowBuilder, 5000, "Outbound Email", "There are too many out going emails pending, check EmailSender service", @"
SELECT COUNT(*)
FROM Emails e 
WHERE IsWaiting = 1 
");

        isFailed = CheckCount(isFailed, failures, 1000, "TSEmailProcessor", FindServiceHost(services, "TSEmailProcessor"), rowBuilder, 5000, "Email Processing", "There are too many records in the EmailPosts table, check EmailProcessor service.", @"
SELECT COUNT(*) FROM EmailPosts
WHERE CreatorID <> -5 AND DATEDIFF(SECOND, GETUTCDATE(), DATEADD(SECOND, HoldTime, DateCreated)) < 0 
");

        isFailed = CheckCount(isFailed, failures, 100, "TSIndexer", FindServiceHost(services, "TSIndexer"), rowBuilder, 1000, "Index Processing", "There are too many records waiting to be indexed, check Indexer service on POD-IDX01 service.", @"
SELECT COUNT(*) FROM Organizations o 
WHERE o.IsIndexLocked = 0
AND o.ParentID = 1
AND (IsRebuildingIndex = 0 OR DATEDIFF(SECOND, DateLastIndexed, GETUTCDATE()) > 300)
AND o.IsActive = 1
AND (
  EXISTS (SELECT * FROM Tickets t WHERE t.OrganizationID = o.OrganizationID AND t.NeedsIndexing=1)
 
)
");
        if (Request.QueryString["json"] != null)
        {
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write(JsonConvert.SerializeObject(failures));
            Response.End();
        }

        //result.Append(string.Format("<h2 style=\"color:{1};\">{0} is{2} running.</h2>", name, !isFailed ? "green" : "red", !isFailed ? "" : " NOT"));

        litQueryRows.Text = rowBuilder.ToString();


        litStatus.Text = !isFailed ? "<div class=\"alert alert-success\">All services are running</div>" : "<div class=\"alert alert-danger\">All services are NOT running</div>";

    }

    private static string FindServiceHost(Services services, string serviceName)
    {
        foreach (Service service in services)
        {
            if (!string.IsNullOrWhiteSpace(service.AssemblyName) && service.AssemblyName.ToLower() == serviceName.ToLower())
            {
                return service.NameSpace;
            }

        }
        return "";
    }

    private static object GetServiceObject(string name, string host)
    {
        dynamic json = new ExpandoObject();
        json.serviceName = name;
        json.host = host;
        return json;
    }

    private static void AddStatusRow(StringBuilder builder, string name, bool status, DateTime lastTime, int maxTime)
    {
        string progress = @"
<div class=""progress"">
  <div class=""progress-bar progress-bar-{1}"" role=""progressbar"" aria-valuenow=""{0}"" aria-valuemin=""0"" aria-valuemax=""100"" style=""width: {0}%"">
    <span class=""sr-only"">{0}% Complete(success)</span>
  </div>
</div>";

        int percent = (int)((1 - (DateTime.Now.Subtract(lastTime).TotalMinutes / maxTime)) * 100);
        string progressColor = "success";
        if (percent < 50) progressColor = "warning";
        if (percent < 0)
        {
            progressColor = "danger";
            percent = 100;
        }
        string bar = string.Format(progress, percent.ToString(), progressColor);
        builder.Append(string.Format("<tr class=\"{0}\"><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>", status ? "" : "danger", name, status ? "Good" : "Down", lastTime, bar));

        if (!status)
        {
            SendMessageToSlack(name + " is down.");

        }
    }

    private static void AddQueryRow(StringBuilder builder, string name, bool status, int max, int count, string message)
    {
        builder.Append(string.Format("<tr class=\"{0}\"><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>", status ? "" : "danger", name, status ? "Good" : "Down", max, count, message));
    }

    private static bool CheckCount(bool isFailed, List<object> failures, int restartCount, string serviceName, string host, StringBuilder builder, int maxCount, string name, string failedMessage, string query)
    {
        int result = (int)SqlExecutor.ExecuteScalar(LoginUser.Anonymous, query);
        bool isBad = false;
        if (result > maxCount)
        {
            isBad = true;
            AddQueryRow(builder, name, false, maxCount, result, failedMessage);
            string message = string.Format("Service: {0} ({1}) records - {2}", name, result.ToString(), failedMessage);
            SendMessageToSlack(message);
        }
        else
        {
            string message = failedMessage;
            AddQueryRow(builder, name, true, maxCount, result, "");

        }

        if (result > restartCount)
        {
            failures.Add(GetServiceObject(serviceName, host));
        }


        return isFailed || isBad;
    }

    [WebMethod]
    public static void SendSlackMsg(string message)
    {

        User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
        //SendMessageToSlack(user.FirstLastName + " has acknowledged this issue on pod " + SystemSettings.GetPodName());
        SendMessageToSlack(string.Format("[{0}] {1} {2}", SystemSettings.GetPodName(), user.FirstLastName, message));
    }
    private static void SendMessageToSlack(string messageText)
    {
        messageText = string.Format("{0} - {1}/ServiceStatus.aspx", messageText, SystemSettings.GetAppUrl());
        try
        {
            SlackMessage message = new SlackMessage(LoginUser.Anonymous);
            message.TextPlain = messageText;
            message.TextRich = messageText;
            message.Color = "#D00000";

            // send to channel
            message.Send("#service-warnings");
        }
        catch (Exception ex)
        {

        }
    }

}


/*
 * SQL to update Servcies table with assembly and name space fields (Service name and host)
 * 
declare  @pod varchar(10)
select @pod = 'beta'

update services set NameSpace = @pod + '-svc01'
update services set NameSpace = @pod + '-idx01' where Name like '%index%'
update services set NameSpace = @pod + '-svc01' where Name like '%email%'
update services set NameSpace = '' where Name like '%tok%'
update services set AssemblyName = 'TS'+Name
update services set AssemblyName = 'TSEmailSender' where AssemblyName like '%emailsender%'
update services set AssemblyName = 'TSEmailProcessor' where AssemblyName like '%emailproc%'
update services set AssemblyName = '' where AssemblyName like '%Tok%'
update services set AssemblyName = 'TSIndexer' where AssemblyName like '%indexer%'
update services set AssemblyName = 'TSIndexRebuilder' where AssemblyName like '%indexreb%'
update services set AssemblyName = 'TSCrmPool' where AssemblyName = 'TSCrmProcessor'
update services set AssemblyName = 'TSCustomerInsights' where AssemblyName = 'TSCustomerInsightsProcessor'
update services set AssemblyName = 'TSWebHooksProcessor' where AssemblyName like '%webhooks%'
update services set AssemblyName = 'TSReportSender' where AssemblyName like '%reportsender%'
update services set AssemblyName = 'TSSlaProcessor' where AssemblyName like '%slaproc%'
update services set AssemblyName = 'TSTaskProcessor' where AssemblyName like '%taskproc%'

update Services set AutoStart=1
update Services set AutoStart=0
where name like '%email%' or name like '%index%' or name like '%tok%'

select * from services order by name

 */
