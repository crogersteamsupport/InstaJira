using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Data.BusinessObjects.Reporting
{
    public enum EJoin
    {
        Tickets,
        ProductID,
        ReportedVersionID,
        SolvedVersionID,
        Groups,
        UserID,
        CloserID,
        CreatorID,
        ModifierID,
        TicketStatuses,
        TicketSeverities,
        ForumTickets,
        ForumCategories,
        CategoryID,
        ParentID,
        TicketTypes,
        ProductFamilies,
        TimeSpent,
        TicketLinkToJira
    }

    /// <summary>
    /// The tables and joins in dbo.ReportTicketsView
    /// </summary>
    class ReportTicketsViewJoin
    {
        static Dictionary<EJoin, string> _join;

        EJoin _table;

        public ReportTicketsViewJoin(EJoin table)
        {
            _table = table;
        }

        static ReportTicketsViewJoin()
        {
            // WARNING - Update this if ReportTicketsView changes
            _join = new Dictionary<EJoin, string>
            {
                [EJoin.Tickets] = "dbo.Tickets AS t",
                [EJoin.ProductID] = "LEFT OUTER JOIN dbo.Products AS p ON p.ProductID = t.ProductID",
                [EJoin.ReportedVersionID] = "LEFT OUTER JOIN dbo.ProductVersions AS pv1 ON pv1.ProductVersionID = t.ReportedVersionID",
                [EJoin.SolvedVersionID] = "LEFT OUTER JOIN dbo.ProductVersions AS pv2 ON pv2.ProductVersionID = t.SolvedVersionID",
                [EJoin.Groups] = "LEFT OUTER JOIN dbo.Groups AS g ON g.GroupID = t.GroupID",
                [EJoin.UserID] = "LEFT OUTER JOIN dbo.Users AS u ON u.UserID = t.UserID",
                [EJoin.CloserID] = "LEFT OUTER JOIN dbo.Users AS u2 ON u2.UserID = t.CloserID",
                [EJoin.CreatorID] = "LEFT OUTER JOIN dbo.Users AS u3 ON u3.UserID = t.CreatorID",
                [EJoin.ModifierID] = "LEFT OUTER JOIN dbo.Users AS u4 ON u4.UserID = t.ModifierID",
                [EJoin.TicketStatuses] = "LEFT OUTER JOIN dbo.TicketStatuses AS ts ON ts.TicketStatusID = t.TicketStatusID",
                [EJoin.TicketSeverities] = "LEFT OUTER JOIN dbo.TicketSeverities AS tse ON tse.TicketSeverityID = t.TicketSeverityID",
                [EJoin.ForumTickets] = "LEFT OUTER JOIN dbo.ForumTickets AS ft ON ft.TicketID = t.TicketID",
                [EJoin.ForumCategories] = "LEFT OUTER JOIN dbo.ForumCategories AS fc ON fc.CategoryID = ft.ForumCategory",
                [EJoin.CategoryID] = "LEFT OUTER JOIN dbo.KnowledgeBaseCategories AS kbc ON t.KnowledgeBaseCategoryID = kbc.CategoryID",
                [EJoin.ParentID] = "LEFT OUTER JOIN dbo.KnowledgeBaseCategories AS kbcp ON kbcp.CategoryID = kbc.ParentID",
                [EJoin.TicketTypes] = "LEFT OUTER JOIN dbo.TicketTypes AS tt ON tt.TicketTypeID = t.TicketTypeID",
                [EJoin.ProductFamilies] = "LEFT OUTER JOIN dbo.ProductFamilies AS pf ON pf.ProductFamilyID = p.ProductFamilyID",
                [EJoin.TimeSpent] = "LEFT OUTER JOIN (SELECT TicketID ,SUM(TimeSpent) AS TimeSpent FROM dbo.Actions GROUP BY TicketID ) AS tsv ON tsv.TicketID = t.TicketID",
                [EJoin.TicketLinkToJira] = "LEFT JOIN TicketLinkToJira ON t.TicketID = TicketLinkToJira.TicketID"
            };
        }

        public void ToSql(StringBuilder builder)
        {
            builder.Append($"\n {_join[_table]}");
        }
    }
}
