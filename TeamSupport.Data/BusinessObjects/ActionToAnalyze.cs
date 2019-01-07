using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using TeamSupport.Data;
using System.Data;

namespace TeamSupport.Data.BusinessObjects
{
    /// <summary>
    /// Linq class to integrate with ActionToAnalyze Table
    /// </summary>
    [Table(Name = "ActionToAnalyze")]
    class ActionToAnalyze
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        private Int64 _ActionToAnalyzeID;
        [Column(Storage = "_ActionToAnalyzeID", IsPrimaryKey = true, IsDbGenerated = true)]
        public Int64 ActionToAnalyzeID { get { return _ActionToAnalyzeID; } }

        [Column]
        public int ActionID;
        [Column]
        public int TicketID;
        [Column]
        public int UserID;
        [Column]
        public int OrganizationID;
        [Column]
        public bool IsAgent;
        [Column]
        public DateTime DateCreated;
        [Column]
        public string ActionDescription;
#pragma warning restore CS0649

        static int MaxActionTextLength;

        static ActionToAnalyze()
        {
            try
            {
                MaxActionTextLength = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("MaxActionTextLength"));
            }
            catch(Exception ex)
            {
                MaxActionTextLength = 5000;
            }
        }
        /// <summary>
        /// Watson utterance only allows 500 char
        /// (don't throw sql exception on truncate of dbo.ActionToAnalyze.ActionDescription)
        /// </summary>
        /// <param name="RawHtml">verbose ActionDescription nvarchar(max)</param>
        /// <returns>500 characters to send to Watson</returns>
        public static string CleanString(string RawHtml)
        {
            // remove email addresses first (even if contains spaces)
            string text = Regex.Replace(RawHtml, @"\s+@", "@");
            Regex ItemRegex = new Regex(@"[\w\d._%+-]+@[ \w\d.-]+\.[\w]{2,3}");

            text = Regex.Replace(text, @"<[^>]*>", String.Empty); //remove html tags
            text = Regex.Replace(text, "&nbsp;", " "); //remove HTML space
            text = Regex.Replace(text, @"[\d-]", String.Empty); //removes all digits [0-9]
            text = Regex.Replace(text, @"\s+", " ");   // remove whitespace
            if (text.Length > MaxActionTextLength)
                text = text.Substring(0, MaxActionTextLength);
            return text;
        }


        /// <summary>
        /// The watson service uses Stored Procedure dbo.ActionsGetForWatson to find records for watson ActionToAnalyze
        /// This routine performs the equivalent checks
        /// </summary>
        /// <param name="action"></param>
        public static void QueueForWatsonToneAnalysis(Action action, SqlConnection connection, LoginUser user)
        {
            try
            {
                //--------- Queue for IBM Watson?
                int creatorID = action.CreatorID;   // JOIN dbo.Users creator WITH(NOLOCK) ON a.[creatorid] = creator.[userid]
                if (action.CreatorID == 0)
                    creatorID = user.UserID;   //  ? first ticket on first action has CreatorID == 0 ?
                LoginUser creator = user;

                int ticketID = action.TicketID;  // JOIN dbo.Tickets t WITH (NOLOCK) ON a.[ticketid] = t.[ticketid]
                Ticket t = Tickets.GetTicket(creator, ticketID);

                int accountID = t.OrganizationID;   // JOIN dbo.Organizations account WITH (NOLOCK) ON t.organizationid = account.organizationid
                Organization account = Organizations.GetOrganization(creator, accountID);

                //account.[usewatson] = 1
                //AND a.[isvisibleonportal] = 1
                //AND t.[isvisibleonportal] = 1
                //AND account.producttype = 2
                // AND NOT EXISTS (SELECT NULL FROM ActionSentiments ast WHERE a.actionid = ast.actionid);
                if (!(account.UseWatson && action.IsVisibleOnPortal && t.IsVisibleOnPortal && (account.ProductType == ProductType.Enterprise))) {
                    return;
                }

                //CASE
                //    WHEN creatorCompany.organizationid = account.organizationid THEN 1
                //    ELSE 0
                //END AS[IsAgent]
                int creatorCompanyID = creator.OrganizationID;  // JOIN dbo.Organizations creatorCompany WITH(NOLOCK) ON creatorCompany.[organizationid] = creator.[organizationid]
                Organization creatorCompany = Organizations.GetOrganization(creator, creatorCompanyID);

                // insert a record into the Table dbo.ActionToAnalyze
                ActionToAnalyze actionToAnalyze = new ActionToAnalyze
                {
                    ActionID = action.ActionID,
                    TicketID = action.TicketID,
                    UserID = creatorID,
                    OrganizationID = creator.OrganizationID,
                    IsAgent = creatorCompany.OrganizationID == account.OrganizationID,
                    ActionDescription = CleanString(action.Description),
                    DateCreated = action.DateCreated
                };

                // SUCCESS! create the ActionToAnalyze
                using (DataContext db = new DataContext(connection))
                {
                    Table<ActionToAnalyze> actionToAnalyzeTable = db.GetTable<ActionToAnalyze>();
                    if (!actionToAnalyzeTable.Where(u => u.ActionID == actionToAnalyze.ActionID).Any())
                        actionToAnalyzeTable.InsertOnSubmit(actionToAnalyze);
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.EventLog.WriteEntry("Application", "Unable to queue action for watson" + e.Message + " ----- STACK: " + e.StackTrace.ToString());
                Console.WriteLine(e.ToString());
                return;
            }
        }
    }
}
