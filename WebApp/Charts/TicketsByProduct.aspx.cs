using System;
using System.Collections.Generic;
using System.Linq;
using TeamSupport.WebUtils;
using TeamSupport.Data;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;

public partial class Charts_TicketsByProduct : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)    {
    }

    [Serializable]
    public class TicketCounts
    {
        public string Product { get; set; }
        public int Issues{ get; set; }
        public int Tasks { get; set; }
        public int Bugs { get; set; }
        public int Features { get; set; }
    }

    [WebMethod]
    public static TicketCounts[] GetData()
    {
        List<TicketCounts> result = new List<TicketCounts>();
        
        SqlCommand command = new SqlCommand(@"select p.name as ProductName, 
                 (select count(*) from tickets as t, products as p2, ticketstatuses as ts, tickettypes as tt
                  where t.productid = p2.productid
                  and t.organizationid = @OrganizationID
                  and t.ticketstatusid = ts.ticketstatusid 
                  and ts.isclosed = 0  
                  and t.tickettypeid = tt.tickettypeid
                  and tt.name = 'issues'
                  and p.productid = p2.productid) as NumIssues,
                 (select count(*) from tickets as t, products as p2, ticketstatuses as ts, tickettypes as tt
                  where t.productid = p2.productid
                  and t.organizationid = @OrganizationID
                  and t.ticketstatusid = ts.ticketstatusid 
                  and ts.isclosed = 0  
                  and t.tickettypeid = tt.tickettypeid
                  and tt.name = 'tasks'
                  and p.productid = p2.productid) as NumTasks,
                 (select count(*) from tickets as t, products as p2, ticketstatuses as ts, tickettypes as tt
                  where t.productid = p2.productid
                  and t.organizationid = @OrganizationID
                  and t.ticketstatusid = ts.ticketstatusid 
                  and ts.isclosed = 0  
                  and t.tickettypeid = tt.tickettypeid
                  and tt.name = 'bugs'
                  and p.productid = p2.productid) as NumBugs,
                 (select count(*) from tickets as t, products as p2, ticketstatuses as ts, tickettypes as tt
                  where t.productid = p2.productid
                  and t.organizationid = @OrganizationID
                  and t.ticketstatusid = ts.ticketstatusid 
                  and ts.isclosed = 0  
                  and t.tickettypeid = tt.tickettypeid
                  and tt.name = 'features'
                  and p.productid = p2.productid) as NumFeatures
                From Products as p
                where p.organizationid = @OrganizationID
                order by p.name");
        command.Parameters.AddWithValue("@OrganizationID", UserSession.LoginUser.OrganizationID);

        DataTable productTickets = SqlExecutor.ExecuteQuery(UserSession.LoginUser, command);

        foreach (DataRow thisRow in productTickets.Rows) {
            TicketCounts counts = new TicketCounts();
            counts.Product = (string)thisRow["ProductName"];
            counts.Issues = (int)thisRow["NumIssues"];
            counts.Tasks = (int)thisRow["NumTasks"];
            counts.Bugs = (int)thisRow["NumBugs"];
            counts.Features = (int)thisRow["NumFeatures"];

            result.Add(counts);
        }
        
        return result.ToArray();
    }
}