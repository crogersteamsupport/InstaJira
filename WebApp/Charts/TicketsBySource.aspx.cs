using System;
using System.Collections.Generic;
using System.Linq;
using TeamSupport.WebUtils;
using TeamSupport.Data;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;

public partial class Charts_TicketsBySource : BaseChartPage
{
    [Serializable]
    public class SourceCount
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    [WebMethod]
    public static SourceCount[] GetData(DateTime start, DateTime end)
    {
        List<SourceCount> result = new List<SourceCount>();

        SqlCommand command = new SqlCommand(@"SELECT ISNULL(TicketSource,'Agent') AS 'TicketSource', COUNT(*) AS 'Number'
                    FROM Tickets
                    WHERE OrganizationID = @organizationID
	                    AND DateCreated > @StartDate AND DateCreated < DATEADD(DAY,1,@EndDate)
                    GROUP BY ISNULL(TicketSource,'Agent')");

        command.Parameters.AddWithValue("@organizationid", UserSession.LoginUser.OrganizationID);
        command.Parameters.AddWithValue("@StartDate", start);
        command.Parameters.AddWithValue("@EndDate", end);

        DataTable ticketSource = SqlExecutor.ExecuteQuery(UserSession.LoginUser, command);

        foreach (DataRow thisRow in ticketSource.Rows)
        {
            SourceCount counts = new SourceCount();
            counts.Name = (string)thisRow["TicketSource"];
            counts.Count = (int)thisRow["Number"];
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