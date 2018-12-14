using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Linq;

namespace TeamSupport.CDI.linq
{
    /// <summary> 
    /// [dbo].[TicketSeverities] 
    /// Assign a number to ticket severity (Priority/Severity 1 is the highest)
    /// </summary>
    [Table(Name = "TicketSeverities")]
    class TicketSeverity
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _ticketSeverityID;
        [Column(Storage = "_ticketSeverityID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int TicketSeverityID { get { return _ticketSeverityID; } }

        [Column]
        public string Name;
        [Column]
        public int? Severity;
#pragma warning restore CS0649

        public static void AssignTicketSeverities()
        {
            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    //db.Log = CDIEventLog.Instance;
                    Table<TicketSeverity> severityTable = db.GetTable<TicketSeverity>();

                    var query = (from t in severityTable
                                 where !t.Severity.HasValue
                                 select t);

                    TicketSeverity[] needsValue = query.ToArray();
                    foreach (TicketSeverity severity in needsValue)
                        severity.AssignSeverity();
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                CDIEventLog.Instance.WriteEntry("Ticket Read failed", e);
            }
        }

        // lower case check
        static string[][] Priorities = new string[][] {
            new string[] { "p0", "p1", "priority 1", "severity 1", "red", "urgent", "u r g e n t", "immediate", "highest", "emergency", "critical", "urgente",
                "blocker", "down", "crítica", "very severe", "a.s.a.p.", "level 1", "cat-1", "sev 1", "sev1", "sev-1" },
            new string[] { "p2", "priority 2", "severity 2", "orange", "high", "major", "serious", "upset", "hour", "moderately severe",
                "important", "escalation", "expedite", "as soon as possible", "level 2", "cat-2", "sev 2", "sev2", "sev-2" },
            new string[] { "p3", "priority 3", "severity 3", "normal", "medium", "moderate", "average", "med", "non-urgent", "standard",
                "default", "broken", "day", "general", "significant", "help desk", "level 3", "cat-3", "sev 3", "sev3", "sev-3"},
            new string[] { "p4", "priority 4", "severity 4", "low", "minor", "insignificant", "inquiry", "curious", "query", "menor", "simple",
                "training", "very light", "days", "trivial", "information", "cosmetic", "no hurry", "level 4", "cat-4", "sev 4", "sev4", "sev-4" },
            new string[] { "p5", "priority 5", "priority 6", "green", "development", "enhancement", "future", "no impact", "no action", "cat-5", "sev5", "sev-5"},
            new string[] { "unassigned", "unclassified", "not set", "needs triage", "none", "new", "blank", "needs leibel",
                "select", "no priority", "to be", "n/a" }
        };

        public void AssignSeverity()
        {
            if (Severity.HasValue)
                return;

            for (int i = 0; i < Priorities.Length; ++i)
            {
                foreach (string priorityName in Priorities[i])
                {
                    string lowerCase = Name.ToLower();

                    if (Name.ToLower().Contains(priorityName))  // lower case
                    {
                        Severity = i;
                        return;
                    }
                }
            }
        }
    }
}
