using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace TeamSupport.Data.BusinessObjects.Reporting
{
    class UserRights
    {
        public static void UseTicketRights(LoginUser loginUser, int subCAtID, ReportTables tables, SqlCommand command, StringBuilder builder)
        {
            ReportSubcategory subCat = ReportSubcategories.GetReportSubcategory(loginUser, subCAtID);

            if (subCat != null)
            {
                ReportTable catTable = tables.FindByReportTableID((int)subCat.ReportCategoryTableID);

                if (catTable.UseTicketRights)
                {
                    GetUserRightsClause(loginUser, command, builder, catTable.TableName);
                    return;
                }
                else if (catTable.ReportTableID == 6)
                {
                    GetCustomerUserRightsClause(loginUser, command, builder, catTable.TableName);
                }

                if (subCat.ReportTableID != null)
                {
                    ReportTable reportTable = tables.FindByReportTableID((int)subCat.ReportTableID);

                    if (reportTable.UseTicketRights)
                    {
                        GetUserRightsClause(loginUser, command, builder, reportTable.TableName);
                        return;
                    }
                    else if (reportTable.ReportTableID == 6)
                    {
                        GetCustomerUserRightsClause(loginUser, command, builder, reportTable.TableName);
                    }
                }
            }
        }

        private static void GetUserRightsClause(LoginUser loginUser, SqlCommand command, StringBuilder builder, string mainTableName)
        {
            string rightsClause = "";

            User user = Users.GetUser(loginUser, loginUser.UserID);
            switch (user.TicketRights)
            {
                case TicketRightType.All:
                    break;
                case TicketRightType.Assigned:
                    builder.Append(string.Format("AND ({0}.TicketID in ( SELECT t.TicketID FROM tickets t WHERE (t.UserID = @UserID OR t.IsKnowledgeBase=1)))", mainTableName));
                    break;
                case TicketRightType.Groups:
                    rightsClause = @"AND ({0}.TicketID in ( 
		                                SELECT t.TicketID 
		                                FROM tickets t          
		                                WHERE ({1} 
                                        (t.UserID = @UserID) 
		                                OR (t.IsKnowledgeBase = 1) 
		                                OR (t.UserID IS NULL AND t.GroupID IS NULL))
	                                ))";

                    Groups groups = new Groups(loginUser);
                    groups.LoadByUserID(loginUser.UserID);

                    string groupString = groups.Count < 1 ? "" : string.Format("(t.GroupID IN ({0})) OR ", DataUtils.IntArrayToCommaString(groups.Select(gr => gr.GroupID).ToArray()));

                    builder.Append(string.Format(rightsClause, mainTableName, groupString));
                    break;
                case TicketRightType.Customers:
                    rightsClause = @" AND ({0}.TicketID in (
                                    SELECT ot.TicketID FROM OrganizationTickets ot
                                    INNER JOIN UserRightsOrganizations uro ON ot.OrganizationID = uro.OrganizationID 
                                    WHERE uro.UserID = @UserID) OR
                                    {0}.UserID = @UserID OR
                                    {0}.IsKnowledgeBase = 1)";
                    builder.Append(string.Format(rightsClause, mainTableName));
                    break;
                default:
                    break;
            }

            Organizations organization = new Organizations(loginUser);
            organization.LoadByOrganizationID(loginUser.OrganizationID);
            if (organization.Count > 0 && organization[0].UseProductFamilies)
            {
                switch ((ProductFamiliesRightType)user.ProductFamiliesRights)
                {
                    case ProductFamiliesRightType.AllFamilies:
                        break;
                    case ProductFamiliesRightType.SomeFamilies:
                        rightsClause = @" AND (
                    {0}.TicketID IN 
                    (
                        SELECT 
                            t.TicketID 
                        FROM 
                            Tickets t WITH (NOLOCK)
                        WHERE 
                            t.ProductID IS NULL and t.Organizationid = {1}                         
                        UNION
                        SELECT 
                            t.TicketID 
                        FROM 
                            Tickets t WITH (NOLOCK)
                            LEFT JOIN Products p WITH (NOLOCK)
                                ON t.ProductID = p.ProductID
                            LEFT JOIN UserRightsProductFamilies urpf
                                ON p.ProductFamilyID = urpf.ProductFamilyID 
                        WHERE                             
                          urpf.UserID = @UserID and t.Organizationid = {1} 

                    ) 
                    OR {0}.UserID = @UserID
                  )";
                        builder.Append(string.Format(rightsClause, mainTableName, loginUser.OrganizationID));
                        break;
                    default:
                        break;
                }
            }
        }

        private static void GetCustomerUserRightsClause(LoginUser loginUser, SqlCommand command, StringBuilder builder, string mainTableName)
        {
            string rightsClause = "";

            User user = Users.GetUser(loginUser, loginUser.UserID);

            if (user.TicketRights == TicketRightType.Customers)
            {
                rightsClause = @"AND (OrganizationsView.OrganizationID in (
                                SELECT uro.OrganizationID 
                                FROM UserRightsOrganizations uro                                       
                                WHERE uro.UserID = @UserID))";
                builder.Append(string.Format(rightsClause, mainTableName));
            }
        }

    }
}
