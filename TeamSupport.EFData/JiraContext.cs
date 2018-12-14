using System.Data.Entity;
using TeamSupport.EFData.Models;

namespace TeamSupport.EFData
{
    public class JiraContext : DbContext
    {
        public JiraContext() : base("name=MainConnection")
        {
            Database.SetInitializer<JiraContext>(null);
        }

        //public JiraContext(): base("Data Source=dev-sql.corp.teamsupport.com; Initial Catalog=TeamSupportNightly;Persist Security Info=True;User ID=Dev-Sql-WebApp;Password=TeamSupportDev;Connect Timeout=500")
        //{

        //}
        public DbSet<TicketLinkToJira> TicketLinkToJira { get; set; }
        public DbSet<CrmLinkTable> CrmLinkTables { get; set; }
        public DbSet<TicketsView> TicketsView { get; set; }
        public DbSet<Tickets> Tickets { get; set; }
        public DbSet<Actions> Actions { get; set; }
    }
}