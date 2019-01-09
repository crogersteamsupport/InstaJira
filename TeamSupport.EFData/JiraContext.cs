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
                
        public DbSet<TicketLinkToJira> TicketLinkToJira { get; set; }
        public DbSet<CrmLinkTable> CrmLinkTables { get; set; }
        public DbSet<Tickets> Tickets { get; set; }
        public DbSet<Actions> Actions { get; set; }
        public DbSet<CRMLinkField> CRMLinkField { get; set; }
        public DbSet<TicketTypes> TicketTypes { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<ExceptionLogs> ExceptionLogs { get; set; }
    }
}