using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;


public partial class ServiceStatus : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        litRows.Text = GetRowText();
    }

    private static void AddQueryRow(StringBuilder builder, string query, string cpu, string reads, string writes, string elapsed, string program, string host, string handle)
    {
        handle = string.Format("<a class=\"handle\" src=\"#\">{0}</a>", handle);
        builder.Append(string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td></tr>", query, cpu,reads,writes,elapsed,program,host,handle));
    }

    private static string GetRowText()
    {
        SqlCommand command = new SqlCommand();
        command.CommandText = @"
SELECT
    --r.session_id, 
    --r.[status],
    --r.last_wait_type,
    --r.command, 
    --DB_NAME(r.database_id) [db_name],    
    t.[text] AS [Query],
    r.cpu_time AS [CPU],
    r.reads AS [Reads],
    r.writes AS [Writes],
    r.total_elapsed_time AS [Time], 
    s.program_name AS [Program],
    s.host_name AS [Host],
    --r.percent_complete,
    r.wait_time AS [Wait]
    --r.blocking_session_id [blocked_by],whe
    --r.open_transaction_count [open_tran]
    ,convert(varchar(max), r.plan_handle, 2) AS [Plan]
    --p.query_plan
FROM 
    sys.dm_exec_requests r
    INNER JOIN sys.dm_exec_sessions s ON r.session_id = s.session_id
    CROSS APPLY sys.dm_exec_sql_text(r.sql_handle) t
    CROSS APPLY sys.dm_exec_query_plan(r.plan_handle) p
WHERE    
    r.session_id != @@SPID
    AND r.session_id IN (SELECT TOP (20) session_id FROM sys.dm_exec_requests 
    WHERE session_id > 50
    ORDER BY cpu_time DESC)

ORDER BY r.cpu_time DESC;

";

        LoginUser loginUser = GetLoginUser();
        DataTable table = SqlExecutor.ExecuteQuery(loginUser, command);
        StringBuilder builder = new StringBuilder();
        builder.AppendLine(GetHeader(table));
        builder.AppendLine("<tbody>");

        foreach (DataRow row in table.Rows)
        {
            string plan = row["Plan"].ToString();
            builder.AppendLine(string.Format("<tr data-plan=\"{0}\">", plan));
            foreach (DataColumn column in table.Columns)
            {
                string data = "";
                if (column.ColumnName != "Plan")
                {
                    data = row[column].ToString();
                    if (data.Length > 500) data = data.Substring(0, 500) + "...";
                }
                else
                {
                    data = "<button type=\"button\" class=\"btn btn-danger clearcache\">Clear</button>";
                }
                builder.AppendLine(string.Format("<td>{0}</td>", data));
            }
            builder.AppendLine("</tr>");
        }

        builder.AppendLine("</tbody>");
        return builder.ToString();
    }

    private static string GetHeader(DataTable table)
    {
        StringBuilder builder = new StringBuilder("<thead><tr>");
        foreach (DataColumn column in table.Columns)
        {
            builder.AppendLine(string.Format("<th>{0}</th>", column.ColumnName));
        }
        builder.AppendLine("</tr></thead>");
        return builder.ToString();
    }

    [WebMethod]
    public static string GetRows()
    {
        if (TSAuthentication.OrganizationID != 1078) return "";
        return GetRowText();
    }

    [WebMethod]
    public static string ClearCache(string plan)
    {
        //DBCC FREEPROCCACHE(0x060005000D25462980D64D4B3400000001000000000000000000000000000000000000000000000000000000)

        if (TSAuthentication.OrganizationID != 1078 || !TSAuthentication.IsSystemAdmin) return "";
        if (string.IsNullOrWhiteSpace(plan)) return "";

        try
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "DBCC FREEPROCCACHE(0x" + plan + ")";
            SqlExecutor.ExecuteNonQuery(GetLoginUser(), command);
            return "Execution plan cleared.";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private static LoginUser GetLoginUser()
    {
        SqlConnectionStringBuilder cs = new SqlConnectionStringBuilder(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString);
        cs.UserID = "NA1-SQL-Admin";
        cs.Password = "F59QsqUD";
        return new LoginUser(cs.ConnectionString, -1, 1078, null);
    }

}
