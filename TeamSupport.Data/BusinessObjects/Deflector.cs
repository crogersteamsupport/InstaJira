using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Data.Linq;
using TeamSupport.Data.BusinessObjects;
using System.Threading.Tasks;

namespace TeamSupport.Data
{
    public partial class Deflector
    {
        public static List<string> GetPodIndeces(LoginUser loginUser)
        {
            List<string> jsonResultList = new List<string>();
            List<int> OrganizationList = GetActiveOrganizationList(loginUser);

            foreach (int organizationID in OrganizationList)
            {
                jsonResultList.Add(GetOrganizationIndeces(loginUser, organizationID));
            };

            return jsonResultList;
        }

        public static List<DeflectorReturn> FetchHubDeflections(int organizationID, string phrase, int? customerHubID = null, int ? productFamilyID = null)
        {
            List<DeflectorReturn> result = new List<DeflectorReturn>();
            DeflectorAPI deflectorAPI = new DeflectorAPI();

            //Get deflections via the deflectorAPI
            List<DeflectorReturn> deflectorMatches = new List<DeflectorReturn>();
            string deflectorResponse = deflectorAPI.FetchDeflections(organizationID, phrase);

            if (!String.IsNullOrEmpty(deflectorResponse))
            {
                deflectorMatches = JsonConvert.DeserializeObject<List<DeflectorReturn>>(deflectorResponse);
            }

            if (deflectorMatches.Any())
            {
                string baseHubURL = SystemSettings.GetHubURL();
                List<DataRow> whiteListTickets = new List<DataRow>();
                int? customerHubIDToProcess = CalculateCustomerHubID(organizationID, customerHubID, productFamilyID);

                if (customerHubIDToProcess != null) {
                    whiteListTickets = GetWhiteListTicketPathsByHubID((int)customerHubIDToProcess);
                }

                //Join against the deflector matches to filter
                result.AddRange(deflectorMatches.Join(whiteListTickets,
                        deflectorMatch => deflectorMatch.TicketID,
                        whiteListItem => (int)whiteListItem["TicketID"],
                        (deflectorMatch, whiteListItem) => new DeflectorReturn
                        {
                            TicketID = deflectorMatch.TicketID,
                            Name = deflectorMatch.Name,
                            ReturnURL = FormatHubKBURL(deflectorMatch.TicketID, whiteListItem["CNameURL"].ToString(), whiteListItem["PortalName"].ToString(), baseHubURL)
                        }).ToList());
            }

            return result;
        }

        private static int? CalculateCustomerHubID(int organizationID, int? customerHubID, int? productFamilyID) {
            int? customerHubIDToProcess = null;

            //If we have a hubID that is the best possible scenario
            if (customerHubID != null)
            {
                customerHubIDToProcess = customerHubID;
            }
            //Try by productFamilyID
            else if (productFamilyID != null && productFamilyID != -1)
            {
                CustomerHubs hubHelper = new CustomerHubs(LoginUser.Anonymous);
                hubHelper.LoadByProductFamilyID((int)productFamilyID);
                
                //because who doesn't love doing it this way!?
                if (hubHelper.Any()) {
                    customerHubIDToProcess = hubHelper[0].CustomerHubID;
                }
            }
            //Try and choose the first hub that is not product line associated if no other options are met
            else
            {
                customerHubIDToProcess = CustomerHubs.LoadFirstNonProductAssociatedHubID(organizationID);
            }

            return customerHubIDToProcess;
        }

        private static List<int> GetActiveOrganizationList(LoginUser loginUser)
        {
            List<int> result = new List<int>();
            try
            {
                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT OrganizationID FROM dbo.Organizations WITH (NOLOCK)";
                        command.CommandText += "WHERE IsActive = 1 and ParentID = 1";
                        connection.Open();

                        using (SqlDataReader dr = command.ExecuteReader())
                        {
                            var orgTable = new DataTable();
                            orgTable.Load(dr);

                            foreach (DataRow row in orgTable.Rows)
                            {
                                result.Add((int)row[0]);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = null;
            }

            return result;
        }

        public static string GetOrganizationIndeces(LoginUser loginUser, int organizationID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT tickets.TicketID, tickets.Name, tickets.OrganizationID, tickets.ProductID, tickets.IsVisibleOnPortal, tags.Value FROM dbo.tickets AS Tickets WITH (NOLOCK)";
                        command.CommandText += "INNER JOIN dbo.TagLinks AS TagLinks WITH (NOLOCK) ON TagLinks.RefType = 17 AND TagLinks.RefID = Tickets.TicketID ";
                        command.CommandText += "INNER JOIN dbo.Tags AS Tags WITH (NOLOCK) ON Tags.TagID = TagLinks.TagID ";
                        command.CommandText += "WHERE tickets.organizationID = @organizationID and tickets.IsKnowledgeBase = 1 and tickets.IsVisibleOnPortal = 1";
                        command.CommandText += "FOR JSON PATH";

                        command.Parameters.AddWithValue("@organizationID", organizationID);
                        connection.Open();

                        var jsonResult = new StringBuilder();
                        SqlDataReader reader = command.ExecuteReader();

                        if (!reader.HasRows)
                        {
                            jsonResult.Append("[]");
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                jsonResult.Append(reader.GetValue(0).ToString());
                            }
                        }

                        reader.Close();
                        return jsonResult.ToString();
                    }
                }
            }
            catch (SqlException e)
            {
                return "error";
                // return e.Message;
            }
            catch (Exception e)
            {
                return "error";
            }

        }

        public static List<DataRow> GetWhiteListTicketPathsByHubID(int customerHubID)
        {
            List<DataRow> result = new List<DataRow>();

            using (SqlConnection connection = new SqlConnection(LoginUser.Anonymous.ConnectionString))
            {
                DataTable dt = new DataTable();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = @"SELECT TicketID, CNameURL, PortalName
                                        FROM [Tickets] AS Tickets
                                        INNER JOIN [CustomerHubs] AS Hubs
	                                        ON Tickets.OrganizationID = Hubs.OrganizationID
                                        INNER JOIN [CustomerHubFeatureSettings] AS HubFeatures
	                                        ON Hubs.CustomerHubID = HubFeatures.CustomerHubID
		                                        AND HubFeatures.EnableKnowledgeBase = 1 
		                                        AND HubFeatures.EnableAnonymousProductAssociation = 0
		                                        AND HubFeatures.EnableCustomerSpecificKB = 0
                                        LEFT JOIN Products AS Products
	                                        ON Products.ProductFamilyID = Hubs.ProductFamilyID
                                        WHERE 
	                                        IsKnowledgeBase = 1 
	                                        AND IsVisibleOnPortal = 1
	                                        AND Hubs.CustomerHubID = @CustomerHubID
	                                        AND (Hubs.ProductFamilyID IS NULL OR Products.ProductFamilyID IS NOT NULL)";
                command.Parameters.AddWithValue("@CustomerHubID", customerHubID);
                connection.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    try
                    {
                        adapter.Fill(dt);
                        result.AddRange(dt.AsEnumerable().Select(x => x).ToList());
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
                        return null;
                    }
                }
            }

            return result;
        }

        private static string FormatHubKBURL(int ticketID, string cnameURL, string portalName, string baseHubURL)
        {
            return "https://" + (!string.IsNullOrEmpty(cnameURL) ? cnameURL : portalName + "." + baseHubURL) + "/knowledgeBase/" + ticketID.ToString();
        }
    }
}
