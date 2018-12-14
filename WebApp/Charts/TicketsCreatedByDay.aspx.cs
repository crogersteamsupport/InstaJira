using System;
using System.Collections.Generic;
using TeamSupport.WebUtils;
using TeamSupport.Data;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;

public partial class Charts_TicketsCreatedByDay : BaseChartPage
{
    [Serializable]
    public class TicketCounts
    {
        public string TicketDate { get; set; }
        public int CreatedCount { get; set; }
        public int ClosedCount { get; set; }
    }

    [WebMethod]
    public static TicketCounts[] GetData(DateTime start, DateTime end)
    {
        List<TicketCounts> result = new List<TicketCounts>();

        SqlCommand commandCreated = new SqlCommand(@"SELECT CONVERT(VARCHAR,dt.dtime,1) AS 'Date', ISNULL(TicketsClosed,0) AS 'TicketsClosed', ISNULL(TicketsCreated,0) AS 'TicketsCreated'
            FROM dbo.udfDateTimes(@StartDate,@EndDate,1,'day') AS dt
            LEFT JOIN (SELECT CAST(FLOOR(CAST(DateClosed AS FLOAT)) AS DATETIME) AS 'Date', COUNT(*) AS 'TicketsClosed' FROM Tickets WHERE organizationid = @organizationid AND DateClosed BETWEEN DATEADD(DAY,-1,@StartDate) AND DATEADD(DAY,1,@EndDate)
	            GROUP BY CAST(FLOOR(CAST(DateClosed AS FLOAT)) AS DATETIME)) t ON t.Date = CAST(FLOOR(CAST(dt.dtime AS FLOAT)) AS DATETIME)
            LEFT JOIN (SELECT CAST(FLOOR(CAST(DateCreated AS FLOAT)) AS DATETIME) AS 'Date', COUNT(*) AS 'TicketsCreated' FROM Tickets WHERE organizationid = @organizationid AND DateCreated BETWEEN DATEADD(DAY,-1,@StartDate) AND DATEADD(DAY,1,@EndDate)
	            GROUP BY CAST(FLOOR(CAST(DateCreated AS FLOAT)) AS DATETIME)) t2 ON t2.Date = CAST(FLOOR(CAST(dt.dtime AS FLOAT)) AS DATETIME)
            ORDER BY dt.dtime");

        commandCreated.Parameters.AddWithValue("@organizationid", UserSession.LoginUser.OrganizationID);
        commandCreated.Parameters.AddWithValue("@StartDate", start);
        commandCreated.Parameters.AddWithValue("@EndDate", end);

        DataTable createdClosed = SqlExecutor.ExecuteQuery(UserSession.LoginUser, commandCreated);

        foreach (DataRow thisRow in createdClosed.Rows)
        {
            TicketCounts counts = new TicketCounts();
            counts.TicketDate = (string)thisRow["Date"];
            counts.ClosedCount = (int)thisRow["TicketsClosed"];
            counts.CreatedCount = (int)thisRow["TicketsCreated"];
            result.Add(counts);
        }
        
        return result.ToArray();

    }

    protected void Button1_Click(object sender, EventArgs e) {
        StartDate = StartDatePick.SelectedDate;
        EndDate = EndDatePick.SelectedDate;
    }
}