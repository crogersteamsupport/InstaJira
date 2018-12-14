using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;
using TeamSupport.WebUtils;
using TeamSupport.Data;

public partial class Charts_ActionsByDay : BaseChartPage
{
    [Serializable]
    public class ActionCount
    {
        public string ActionDate { get; set; }
        public int Count { get; set; }
    }

    [WebMethod]
    public static ActionCount[] GetData(DateTime start, DateTime end)
    {
        List<ActionCount> result = new List<ActionCount>();

        SqlCommand commandCreated = new SqlCommand(@"SELECT CONVERT(VARCHAR,dt.dtime,1) AS 'Date', COUNT(a.DateCreated) AS 'ActionsAdded'
                        FROM dbo.udfDateTimes(@StartDate,@EndDate,1,'day') AS dt
                        LEFT JOIN (SELECT a.DateCreated, u.OrganizationID FROM Actions a INNER JOIN Users u ON a.CreatorID = u.UserID) a ON CAST(FLOOR(CAST(dt.dtime AS FLOAT)) AS DATETIME) = CAST(FLOOR(CAST(a.DateCreated AS FLOAT)) AS DATETIME) AND a.OrganizationID = @organizationID
                        GROUP BY dt.dtime
                        ORDER BY dt.dtime");

        commandCreated.Parameters.AddWithValue("@organizationid", UserSession.LoginUser.OrganizationID);
        commandCreated.Parameters.AddWithValue("@StartDate", start);
        commandCreated.Parameters.AddWithValue("@EndDate", end);

        DataTable actionsAdded = SqlExecutor.ExecuteQuery(UserSession.LoginUser, commandCreated);

        foreach (DataRow thisRow in actionsAdded.Rows)
        {
            ActionCount counts = new ActionCount();
            counts.ActionDate = (string)thisRow["Date"];
            counts.Count = (int)thisRow["ActionsAdded"];
            result.Add(counts);
        }

        return result.ToArray();

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        StartDate = StartDatePick.SelectedDate;
        EndDate = EndDatePick.SelectedDate;
    }
}