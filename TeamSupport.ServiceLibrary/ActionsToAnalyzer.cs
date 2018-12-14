using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Xml;
using System.Data;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Specialized;
using System.Configuration.Assemblies;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace WatsonToneAnalyzer
{
    /// <summary>
    /// Global class to search the entire DB for actions which need to be Watson analyzed
    /// </summary>
    public class ActionsToAnalyzer
    {
        class ActionGetForWatson
        {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
            public int ActionID;
            public int TicketID;
            public int UserID;
            public int OrganizationID;
            public int IsAgent;
            public DateTime DateCreated;
            public string ActionDescription;
#pragma warning restore CS0649
        }

        public static void FindActionsToAnalyze()
        {
            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection sqlConnection1 = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(sqlConnection1))
                {
                    var results = db.ExecuteQuery<ActionGetForWatson>("Exec " + "dbo.ActionsGetForWatson");
                    Table<ActionToAnalyze> table = db.GetTable<ActionToAnalyze>();
                    foreach (ActionGetForWatson a in results)
                    {
                        ActionToAnalyze actionToAnalyze = new ActionToAnalyze()
                        {
                            ActionID = a.ActionID,
                            TicketID = a.TicketID,
                            UserID = a.UserID,
                            OrganizationID = a.OrganizationID,
                            IsAgent = a.IsAgent == 1,
                            DateCreated = a.DateCreated,
                            ActionDescription = ActionToAnalyze.CleanString(a.ActionDescription),   // clean the text of HTML and special characters
                        };

                        if (!table.Where(u => u.ActionID == actionToAnalyze.ActionID).Any())
                            table.InsertOnSubmit(actionToAnalyze);
                    }

                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                WatsonEventLog.WriteEntry("Exception while reading from action table:", e);
                Console.WriteLine(e.ToString());
            }
        }

    }

}