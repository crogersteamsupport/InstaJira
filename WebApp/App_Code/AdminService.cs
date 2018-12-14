using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using System.Runtime.Serialization;
using dtSearch.Engine;
using System.Net;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class AdminService : System.Web.Services.WebService
    {

        public AdminService()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        [WebMethod]
        public ForumCategoryInfo[] GetForumCategories()
        {
            List<ForumCategoryInfo> result = new List<ForumCategoryInfo>();
            ForumCategories cats = new ForumCategories(TSAuthentication.GetLoginUser());
            cats.LoadCategories(TSAuthentication.OrganizationID);

            foreach (ForumCategory cat in cats)
            {
                ForumCategoryInfo info = new ForumCategoryInfo();
                info.Category = cat.GetProxy();

                ForumCategories subs = new ForumCategories(cats.LoginUser);
                subs.LoadSubcategories(cat.CategoryID);
                info.Subcategories = subs.GetForumCategoryProxies();

                result.Add(info);
            }

            return result.ToArray();
        }

        [WebMethod]
        public ForumCategoryProxy UpdateForumCategory(int categoryID, string name, string description, int? ticketTypeID, int? groupID, int? productID, int? productFamilyID)
        {
            if (!TSAuthentication.IsSystemAdmin) return null;
            ForumCategory cat = ForumCategories.GetForumCategory(TSAuthentication.GetLoginUser(), categoryID);
            if (cat.OrganizationID != TSAuthentication.OrganizationID) return null;
            cat.CategoryName = name;
            cat.CategoryDesc = description;
            cat.TicketType = ticketTypeID;
            cat.GroupID = groupID;
            cat.ProductID = productID;
            cat.ProductFamilyID = productFamilyID;
            cat.Collection.Save();
            return cat.GetProxy();
        }

        [WebMethod]
        public ForumCategoryProxy AddForumCategory(int? parentID)
        {
            if (!TSAuthentication.IsSystemAdmin) return null;


            ForumCategory cat = (new ForumCategories(TSAuthentication.GetLoginUser())).AddNewForumCategory();
            cat.OrganizationID = TSAuthentication.OrganizationID;
            cat.CategoryName = parentID == null ? "Untitled Category" : "Untitled Subcategory";
            cat.ParentID = parentID ?? -1;    
            cat.Position = GetForumCategoryMaxPosition(parentID) + 1;
            cat.Collection.Save();
            return cat.GetProxy();
        }

        private int GetForumCategoryMaxPosition(int? parentID)
        {
            parentID = parentID ?? -1;

            ForumCategories cats = new ForumCategories(TSAuthentication.GetLoginUser());
            if (parentID < 0) cats.LoadCategories(TSAuthentication.OrganizationID);
            else cats.LoadSubcategories((int)parentID);

            int max = -1;

            foreach (ForumCategory cat in cats)
            {
                if (cat.Position != null && cat.Position > max) max = (int)cat.Position;
            }

            return max;
        }

        [WebMethod]
        public bool DeleteForumCategory(int categoryID)
        {
            if (!TSAuthentication.IsSystemAdmin) return false;
            ForumCategory cat = ForumCategories.GetForumCategory(TSAuthentication.GetLoginUser(), categoryID);
            if (cat.OrganizationID != TSAuthentication.OrganizationID) return false;

            if (cat.ParentID < 0)
            {
                ForumCategories cats = new ForumCategories(TSAuthentication.GetLoginUser());
                cats.LoadSubcategories(cat.CategoryID);

                foreach (ForumCategory item in cats)
                {
                    item.Delete();
                }
                cats.Save();
            }

            cat.Delete();
            cat.Collection.Save();
            return true;
        }


        [WebMethod]
        public void UpdateForumCategoryOrder(string data)
        {
            List<ForumCategoryOrder> orders = JsonConvert.DeserializeObject<List<ForumCategoryOrder>>(data);

            if (!TSAuthentication.IsSystemAdmin) return;

            LoginUser loginUser = TSAuthentication.GetLoginUser();
            int catPos = 0;
            foreach (ForumCategoryOrder order in orders)
            {
                ForumCategory cat = ForumCategories.GetForumCategory(loginUser, (int)order.ParentID);
                cat.Position = catPos;
                cat.Collection.Save();

                int subPos = 0;
                foreach (int id in order.CategoryIDs)
                {
                    ForumCategory sub = ForumCategories.GetForumCategory(loginUser, id);
                    sub.Position = subPos;
                    sub.ParentID = (int)order.ParentID;
                    sub.Collection.Save();
                    subPos++;
                }
                catPos++;
            }
        }

        /// <summary>
        /// Knowledge Base Categories
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public KnowledgeBaseCategoryInfo[] GetKnowledgeBaseCategories()
        {
            List<KnowledgeBaseCategoryInfo> result = new List<KnowledgeBaseCategoryInfo>();
            KnowledgeBaseCategories cats = new KnowledgeBaseCategories(TSAuthentication.GetLoginUser());
            cats.LoadCategories(TSAuthentication.OrganizationID);

            foreach (KnowledgeBaseCategory cat in cats)
            {
                KnowledgeBaseCategoryInfo info = new KnowledgeBaseCategoryInfo();
                info.Category = cat.GetProxy();

                KnowledgeBaseCategories subs = new KnowledgeBaseCategories(cats.LoginUser);
                subs.LoadSubcategories(cat.CategoryID);
                info.Subcategories = subs.GetKnowledgeBaseCategoryProxies();

                result.Add(info);
            }

            return result.ToArray();
        }

        [WebMethod]
        public KnowledgeBaseCategoryProxy UpdateKnowledgeBaseCategory(int categoryID, string name, string description, bool visibleOnPortal, int? productFamilyID)
        {
            if (!TSAuthentication.IsSystemAdmin) return null;
            KnowledgeBaseCategory cat = KnowledgeBaseCategories.GetKnowledgeBaseCategory(TSAuthentication.GetLoginUser(), categoryID);
            if (cat.OrganizationID != TSAuthentication.OrganizationID) return null;
            cat.CategoryName = name;
            cat.CategoryDesc = description;
            cat.VisibleOnPortal = visibleOnPortal;
            cat.ProductFamilyID = productFamilyID;
            cat.Collection.Save();
            return cat.GetProxy();
        }

        [WebMethod]
        public KnowledgeBaseCategoryProxy AddKnowledgeBaseCategory(int? parentID)
        {
            if (!TSAuthentication.IsSystemAdmin) return null;


            KnowledgeBaseCategory cat = (new KnowledgeBaseCategories(TSAuthentication.GetLoginUser())).AddNewKnowledgeBaseCategory();
            cat.OrganizationID = TSAuthentication.OrganizationID;
            cat.CategoryName = parentID == null ? "Untitled Category" : "Untitled Subcategory";
            cat.ParentID = parentID ?? -1;
            cat.Position = GetKnowledgeBaseCategoryMaxPosition(parentID) + 1;
            cat.VisibleOnPortal = true;
            cat.Collection.Save();
            return cat.GetProxy();
        }

        private int GetKnowledgeBaseCategoryMaxPosition(int? parentID)
        {
            parentID = parentID ?? -1;

            KnowledgeBaseCategories cats = new KnowledgeBaseCategories(TSAuthentication.GetLoginUser());
            if (parentID < 0) cats.LoadCategories(TSAuthentication.OrganizationID);
            else cats.LoadSubcategories((int)parentID);

            int max = -1;

            foreach (KnowledgeBaseCategory cat in cats)
            {
                if (cat.Position != null && cat.Position > max) max = (int)cat.Position;
            }

            return max;
        }

        [WebMethod]
        public bool DeleteKnowledgeBaseCategory(int categoryID)
        {
            if (!TSAuthentication.IsSystemAdmin) return false;
            KnowledgeBaseCategory cat = KnowledgeBaseCategories.GetKnowledgeBaseCategory(TSAuthentication.GetLoginUser(), categoryID);
            if (cat.OrganizationID != TSAuthentication.OrganizationID) return false;

            if (cat.ParentID < 0)
            {
                KnowledgeBaseCategories cats = new KnowledgeBaseCategories(TSAuthentication.GetLoginUser());
                cats.LoadSubcategories(cat.CategoryID);

                foreach (KnowledgeBaseCategory item in cats)
                {
                    item.Delete();
                }
                cats.Save();
            }

            cat.Delete();
            cat.Collection.Save();
            return true;
        }

        [WebMethod]
        public void UpdateKnowledgeBaseCategoryOrder(string data)
        {
            List<KnowledgeBaseCategoryOrder> orders = JsonConvert.DeserializeObject<List<KnowledgeBaseCategoryOrder>>(data);

            if (!TSAuthentication.IsSystemAdmin) return;

            LoginUser loginUser = TSAuthentication.GetLoginUser();
            int catPos = 0;
            foreach (KnowledgeBaseCategoryOrder order in orders)
            {
                KnowledgeBaseCategory cat = KnowledgeBaseCategories.GetKnowledgeBaseCategory(loginUser, (int)order.ParentID);
                cat.Position = catPos;
                cat.Collection.Save();

                int subPos = 0;
                foreach (int id in order.CategoryIDs)
                {
                    KnowledgeBaseCategory sub = KnowledgeBaseCategories.GetKnowledgeBaseCategory(loginUser, id);
                    sub.Position = subPos;
                    sub.ParentID = (int)order.ParentID;
                    sub.Collection.Save();
                    subPos++;
                }
                catPos++;
            }
        }

        /// <summary>
        /// Checks if the ticket type is allowed to be linked to Jira in the Integration Admin settings. Link should be active in account too.
        /// </summary>
        /// <returns>True or False</returns>
        [WebMethod]
        public bool GetIsJiraLinkActiveForTicket(int ticketId)
        {
            bool result = false;
            result = !string.IsNullOrEmpty(GetJiraInstanceNameForTicket(ticketId));
            return result;
        }

        [WebMethod]
        public string GetJiraInstanceNameForTicket(int ticketId)
        {
            string jiraInstanceName = string.Empty;
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            CRMLinkTable organizationLinks = new CRMLinkTable(loginUser);
            organizationLinks.LoadByOrganizationID(TSAuthentication.OrganizationID);

            List<CRMLinkTableItem> organizationJiraLinks = organizationLinks.Where(p => p.CRMType.ToLower() == Enums.GetDescription(IntegrationType.Jira).ToLower()).ToList();

            if (organizationJiraLinks != null)
            {
                TicketLinkToJira ticketLink = new TicketLinkToJira(loginUser);
                ticketLink.LoadByTicketID(ticketId);

                if (ticketLink != null
                        && ticketLink.Any()
                        && organizationJiraLinks.Where(p => p.CRMLinkID == ticketLink[0].CrmLinkID).Any())
                {
                    CRMLinkTableItem crmJiraInstance = CRMLinkTable.GetCRMLinkTableItem(loginUser, (int)ticketLink[0].CrmLinkID);

                    if (crmJiraInstance != null && !string.IsNullOrEmpty(crmJiraInstance.InstanceName))
                    {
                        jiraInstanceName = crmJiraInstance.InstanceName;
                    }
                }
                else
                {
                    TicketsViewItem ticket = TicketsView.GetTicketsViewItem(loginUser, ticketId);
                    CRMLinkTableItem ticketJiraInstance = organizationJiraLinks.Where(p => p.Active).FirstOrDefault();

                    if (ticket.ProductID != null)
                    {
                        JiraInstanceProducts jiraInstanceProduct = new JiraInstanceProducts(loginUser);
                        jiraInstanceProduct.LoadByProductAndOrganization((int)ticket.ProductID, ticket.OrganizationID, Enums.GetDescription(IntegrationType.Jira));

                        if (jiraInstanceProduct != null && jiraInstanceProduct.Count > 0)
                        {
                            ticketJiraInstance = organizationJiraLinks.Where(p => p.CRMLinkID == jiraInstanceProduct[0].CrmLinkId && p.Active).SingleOrDefault();
                        }
                    }

                    //if ticket does not have Product then use the default instance if it's active
                    if (ticketJiraInstance == null && ticket.ProductID == null)
                    {
                        ticketJiraInstance = organizationJiraLinks.Where(p => p.InstanceName.Trim().ToLower() == "default" && p.Active).SingleOrDefault();
                    }

                    if (ticketJiraInstance != null && ticketJiraInstance.CRMLinkID != 0)
                    {
                        if (string.IsNullOrEmpty(ticketJiraInstance.RestrictedToTicketTypes))
                        {
                            jiraInstanceName = ticketJiraInstance.InstanceName;
                        }
                        else
                        {
                            foreach (string allowedTicketType in ticketJiraInstance.RestrictedToTicketTypes.Split(','))
                            {
                                if (ticket.TicketTypeID.ToString() == allowedTicketType)
                                {
                                    jiraInstanceName = ticketJiraInstance.InstanceName;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return jiraInstanceName;
        }

        /// <summary>
        /// Checks if the Jira Integration is active.
        /// </summary>
        /// <returns>True or False</returns>
        [WebMethod]
        public bool GetIsJiraLinkActiveForOrganization()
        {
            bool result = false;

            CRMLinkTable organizationLinks = new CRMLinkTable(TSAuthentication.GetLoginUser());
            organizationLinks.LoadByOrganizationID(TSAuthentication.OrganizationID);

            foreach (CRMLinkTableItem link in organizationLinks)
            {
                if (link.CRMType.ToLower() == Enums.GetDescription(IntegrationType.Jira).ToLower() && link.Active)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if the TFS Integration is active.
        /// </summary>
        /// <returns>True or False</returns>
        [WebMethod]
        public bool GetIsTFSLinkActiveForOrganization()
        {
            bool result = false;

            CRMLinkTable organizationLinks = new CRMLinkTable(TSAuthentication.GetLoginUser());
            organizationLinks.LoadByOrganizationIDAndCRMType(TSAuthentication.OrganizationID, Enums.GetDescription(IntegrationType.TFS));

            foreach (CRMLinkTableItem link in organizationLinks)
            {
                if (link.Active)
                {
                    result = true;
                }
            }

            return result;
        }

        [WebMethod]
        public string GetTFSCRMLinkTableRecordForTicket(int ticketId)
        {
            string result = string.Empty;
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            CRMLinkTable organizationLinks = new CRMLinkTable(loginUser);
            organizationLinks.LoadByOrganizationID(TSAuthentication.OrganizationID);

            List<CRMLinkTableItem> organizationTFSLinks = organizationLinks.Where(p => p.CRMType.ToLower() == Enums.GetDescription(IntegrationType.TFS).ToLower() && p.Active).ToList();

            if (organizationTFSLinks.Any())
            {
                // For now all we need to know is if there is an active TFS integration record, because this integration does not support multiple instances (yet).
                result = "active";
            }

            return result;
        }

		[WebMethod]
		public string GetSnowCRMLinkTableRecordForTicket(int ticketId)
		{
			string result = string.Empty;
			LoginUser loginUser = TSAuthentication.GetLoginUser();
			CRMLinkTable organizationLinks = new CRMLinkTable(loginUser);
			organizationLinks.LoadByOrganizationID(TSAuthentication.OrganizationID);

			List<CRMLinkTableItem> organizationSnowLinks = organizationLinks.Where(p => p.CRMType.ToLower() == Enums.GetDescription(IntegrationType.ServiceNow).ToLower() && p.Active).ToList();

			if (organizationSnowLinks != null && organizationSnowLinks.Count > 0)
			{
				if (string.IsNullOrEmpty(organizationSnowLinks[0].RestrictedToTicketTypes))
				{
					result = "active";
				}
				else
				{
					List<int> ticketTypesToEnableSync = organizationSnowLinks[0].RestrictedToTicketTypes.Split(',').Select(p => int.Parse(p)).ToList();
					Ticket ticket = Tickets.GetTicket(loginUser, ticketId);

					if (ticketTypesToEnableSync.Contains(ticket.TicketTypeID))
					{
						result = "active";
					}
				}
			}

			return result;
		}

		[WebMethod]
        public void RollbackImport(int importFileID, ReferenceType refType)
        {
            StringBuilder query = new StringBuilder();
            query.Append(@"
			DECLARE @OrganizationID int
	
			SELECT
				@OrganizationID = OrganizationID
			FROM
				Imports
			WHERE
				ImportID = @ImportFileID
		");

            switch (refType)
            {
                case ReferenceType.Actions:
                    query.Append(GetRollbackActionsQuery());
                    break;
                case ReferenceType.Assets:
                    query.Append(GetRollbackAssetTicketsQuery());
                    query.Append(GetRollbackCustomValuesQuery());
                    query.Append(GetRollbackAssetsQuery());
                    break;
                case ReferenceType.Organizations:
                    query.Append(GetRollbackCustomValuesQuery());
                    query.Append(GetRollbackAddressesAndPhoneNumbersQuery());
                    query.Append(GetRollbackCompaniesQuery());
                    break;
                case ReferenceType.CompanyAddresses:
                    query.Append(GetRollbackAddressesAndPhoneNumbersQuery());
                    query.Append(GetRollbackContactsQuery());
                    query.Append(GetRollbackCompaniesQuery());
                    break;
                case ReferenceType.CompanyPhoneNumbers:
                    query.Append(GetRollbackAddressesAndPhoneNumbersQuery());
                    query.Append(GetRollbackContactsQuery());
                    query.Append(GetRollbackCompaniesQuery());
                    break;
                case ReferenceType.Contacts:
                    query.Append(GetRollbackCustomValuesQuery());
                    query.Append(GetRollbackAddressesAndPhoneNumbersQuery());
                    query.Append(GetRollbackContactsQuery());
                    query.Append(GetRollbackCompaniesQuery());
                    break;
                case ReferenceType.ContactAddresses:
                    query.Append(GetRollbackContactsQuery());
                    query.Append(GetRollbackCompaniesQuery());
                    break;
                case ReferenceType.ContactPhoneNumbers:
                    query.Append(GetRollbackContactsQuery());
                    query.Append(GetRollbackCompaniesQuery());
                    break;
                case ReferenceType.Tickets:
                    query.Append(GetRollbackCustomValuesQuery());
                    query.Append(GetRollbackProductsQuery());
                    query.Append(GetRollbackProductVersionsQuery());
                    query.Append(GetRollbackTicketRelationshipsQuery());
                    query.Append(GetRollbackAssetTicketsQuery());
                    query.Append(GetRollbackContactTicketsQuery());
                    query.Append(GetRollbackOrganizationTicketsQuery());
                    query.Append(GetRollbackActionsQuery());
                    query.Append(GetRollbackTicketsQuery());
                    break;
                case ReferenceType.OrganizationTickets:
                    query.Append(GetRollbackOrganizationTicketsQuery());
                    break;
                case ReferenceType.ContactTickets:
                    query.Append(GetRollbackContactTicketsQuery());
                    break;
                case ReferenceType.AssetTickets:
                    query.Append(GetRollbackAssetTicketsQuery());
                    break;
                case ReferenceType.TicketRelationships:
                    query.Append(GetRollbackTicketRelationshipsQuery());
                    break;
                //case ReferenceType.CustomFieldPickList:
                //  ImportCustomFieldPickList(import);
                //  break;
                case ReferenceType.Products:
                    query.Append(GetRollbackCustomValuesQuery());
                    query.Append(GetRollbackProductsQuery());
                    break;
                case ReferenceType.ProductVersions:
                    query.Append(GetRollbackCustomValuesQuery());
                    query.Append(GetRollbackProductVersionsQuery());
                    break;
                case ReferenceType.Users:
                    query.Append(GetRollbackCustomValuesQuery());
                    query.Append(GetRollbackAddressesAndPhoneNumbersQuery());
                    query.Append(GetRollbackUsersQuery());
                    break;
                case ReferenceType.OrganizationProducts:
                    query.Append(GetRollbackCustomValuesQuery());
                    query.Append(GetRollbackOrganizationProductsQuery());
                    break;
                case ReferenceType.Notes:
                    query.Append(GetRollbackOrganizationNotesQuery());
                    break;
            }

            query.Append(@"
			UPDATE 
				Imports
			SET 
				IsRolledBack = 1
			WHERE 
				ImportID = @ImportFileID		
		");

            LoginUser loginUser = TSAuthentication.GetLoginUser();
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = query.ToString();
                    command.Parameters.AddWithValue("@ImportFileID", importFileID);
                    command.ExecuteNonQuery();

                    //_importUser = new Data.LoginUser(LoginUser.ConnectionString, -5, imports[0].OrganizationID, null);
                    Imports import = new Imports(loginUser);
                    import.LoadByImportID(importFileID);
                    string path = TeamSupport.Data.Quarantine.WebAppQ.GetAttachmentPath1(loginUser, import);
                    //string logPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Logs");
                    //logPath = Path.Combine(logPath, imports[0].OrganizationID.ToString());
                    ImportLog _importLog = new ImportLog(path, importFileID);
                    _importLog.Write("Rollback Complete on " + DateTime.Now.ToShortDateString());
                }
                catch (Exception e)
                {
                    ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), e, "AdminService.RollbackImport");
                }
            }
        }

        [WebMethod]
        public List<CustomerHubLinkModel> GetHubURL()
        {
            List<CustomerHubLinkModel> hubList = null;

            CustomerHubs hubs = new CustomerHubs(TSAuthentication.GetLoginUser());
            hubs.LoadByOrganizationID(TSAuthentication.OrganizationID);

            if (hubs.Any())
            {
                hubList = new List<CustomerHubLinkModel>();
                foreach (var hub in hubs)
                {
                    hubList.Add(new CustomerHubLinkModel(hub.CustomerHubID, hub.PortalName, hub.ProductFamilyID, string.Format("{0}.{1}", hub.PortalName, SystemSettings.GetHubURL())));
                }
            }
            else
            {
                hubList = new List<CustomerHubLinkModel>();
                bool success = MigratePortalSettings(TSAuthentication.OrganizationID, TSAuthentication.GetLoginUser());
                if (success)
                {
                    CustomerHubs hubs2 = new CustomerHubs(TSAuthentication.GetLoginUser());
                    hubs2.LoadByOrganizationID(TSAuthentication.OrganizationID);
                    hubList.Add(new CustomerHubLinkModel(hubs[0].CustomerHubID, hubs2[0].PortalName, hubs2[0].ProductFamilyID, string.Format("{0}.{1}", hubs2[0].PortalName, SystemSettings.GetHubURL())));
                }
            }
            return hubList;
        }

        [WebMethod]
        public CustomerHubCreationModel CreateNewHub(string name, int hubToCopyID, int? productFamilyID)
        {
            CustomerHubCreationModel result = null;

            CustomerHubLinkModel newHubModel = new CustomerHubLinkModel(-1, name, productFamilyID != -1 ? productFamilyID : null, null);
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            //get old hub info to replicate
            CustomerHubs customerHubs = new CustomerHubs(loginUser);
            customerHubs.LoadByCustomerHubID(hubToCopyID);

            if (customerHubs.Any())
            {

                CustomerHubAuthentication authenticationSettings = new CustomerHubAuthentication(loginUser);
                authenticationSettings.LoadByCustomerHubID(hubToCopyID);

                CustomerHubDisplaySettings displaySettings = new CustomerHubDisplaySettings(loginUser);
                displaySettings.LoadByCustomerHubID(hubToCopyID);

                CustomerHubFeatureSettings featureSettings = new CustomerHubFeatureSettings(loginUser);
                featureSettings.LoadByCustomerHubID(hubToCopyID);

                result = BuildNewHub(newHubModel, customerHubs[0], authenticationSettings[0], displaySettings[0], featureSettings[0]);

                //Profit.
            }

            return result;
        }

        public CustomerHubCreationModel BuildNewHub(CustomerHubLinkModel newHubModel, CustomerHub srcHub, CustomerHubAuthenticationItem srcAuthenticationItem, CustomerHubDisplaySetting srcDisplaySetting, CustomerHubFeatureSetting srcFeatureSetting)
        {
            CustomerHubCreationModel result = null;

            LoginUser loginUser = TSAuthentication.GetLoginUser();
            CustomerHubs hubHelper = new CustomerHubs(loginUser);

            CustomerHub newHub = null;

            //need validation to check if existing hub names are taken!
            try
            {

                newHub = hubHelper.AddNewCustomerHub();

                newHub.OrganizationID = srcHub.OrganizationID;
                newHub.PortalName = Regex.Replace(newHubModel.Name, "[^0-9a-zA-Z-]", "");
                newHub.IsActive = true;
                newHub.ProductFamilyID = newHubModel.ProductFamilyID;
                newHub.DateCreated = DateTime.UtcNow;
                newHub.DateModified = DateTime.UtcNow;
                newHub.ModifierID = TSAuthentication.GetLoginUser().UserID;
                newHub.EnableMigration = false;

                hubHelper.Save();

            }
            catch (Exception ex)
            {
                string creationMessage = "Error creating your customer hub please try again or contact your administrator";

                if (ex.Message.Contains("UNIQUE"))
                {
                    creationMessage = "This hub name is already taken, please enter a different name and try again.";
                }

                newHub = null;
                result = new CustomerHubCreationModel(null, false, creationMessage);
            }

            if (newHub != null)
            {

                CustomerHubAuthentication authenticationHelper = new CustomerHubAuthentication(loginUser);
                CustomerHubAuthenticationItem authenticationItem = authenticationHelper.AddNewCustomerHubAuthenticationItem();

                //authenticationItem = srcAuthenticationItem;

                authenticationItem.EnableSelfRegister = srcAuthenticationItem.EnableSelfRegister;
                authenticationItem.EnableRequestAccess = srcAuthenticationItem.EnableRequestAccess;
                authenticationItem.EnableSSO = srcAuthenticationItem.EnableSSO;
                authenticationItem.RequestTicketType = srcAuthenticationItem.RequestTicketType;
                authenticationItem.RequestGroupType = srcAuthenticationItem.RequestGroupType;
                authenticationItem.AnonymousHubAccess = srcAuthenticationItem.AnonymousHubAccess;
                authenticationItem.AnonymousWikiAccess = srcAuthenticationItem.AnonymousKBAccess;
                authenticationItem.AnonymousKBAccess = srcAuthenticationItem.AnonymousKBAccess;
                authenticationItem.AnonymousProductAccess = srcAuthenticationItem.AnonymousProductAccess;
                authenticationItem.AnonymousTicketAccess = srcAuthenticationItem.AnonymousTicketAccess;
                authenticationItem.HonorServiceAgreementExpirationDate = srcAuthenticationItem.HonorServiceAgreementExpirationDate;
                authenticationItem.HonorSupportExpiration = srcAuthenticationItem.HonorSupportExpiration;
                authenticationItem.RequireTermsAndConditions = srcAuthenticationItem.RequireTermsAndConditions;
                authenticationItem.AnonymousChatAccess = srcAuthenticationItem.AnonymousChatAccess;

                authenticationItem.DateModified = DateTime.UtcNow;
                authenticationItem.CustomerHubID = newHub.CustomerHubID;
                authenticationItem.ModifierID = loginUser.UserID;

                authenticationHelper.Save();

                CustomerHubDisplaySettings displayHelper = new CustomerHubDisplaySettings(loginUser);
                CustomerHubDisplaySetting displaySetting = displayHelper.AddNewCustomerHubDisplaySetting();

                displaySetting.FontFamily = srcDisplaySetting.FontFamily;
                displaySetting.FontColor = srcDisplaySetting.FontColor;
                displaySetting.Color1 = srcDisplaySetting.Color1;
                displaySetting.Color2 = srcDisplaySetting.Color2;
                displaySetting.Color3 = srcDisplaySetting.Color3;
                displaySetting.Color4 = srcDisplaySetting.Color4;
                displaySetting.Color5 = srcDisplaySetting.Color5;

                displaySetting.CustomerHubID = newHub.CustomerHubID;
                displaySetting.DateModified = DateTime.UtcNow;
                displaySetting.ModifierID = loginUser.UserID;

                displayHelper.Save();

                CustomerHubFeatureSettings featureHelper = new CustomerHubFeatureSettings(loginUser);
                CustomerHubFeatureSetting featureSetting = featureHelper.AddNewCustomerHubFeatureSetting();

                featureSetting.EnableKnowledgeBase = srcFeatureSetting.EnableKnowledgeBase;
                featureSetting.EnableProducts = srcFeatureSetting.EnableProducts;
                featureSetting.EnableTicketCreation = srcFeatureSetting.EnableTicketCreation;
                featureSetting.EnableMyTickets = srcFeatureSetting.EnableMyTickets;
                featureSetting.EnableOrganizationTickets = srcFeatureSetting.EnableOrganizationTickets;
                featureSetting.EnableWiki = srcFeatureSetting.EnableWiki;
                featureSetting.EnableTicketGroupSelection = srcFeatureSetting.EnableTicketGroupSelection;
                featureSetting.EnableTicketProductSelection = srcFeatureSetting.EnableTicketProductSelection;
                featureSetting.EnableTicketProductVersionSelection = srcFeatureSetting.EnableTicketProductVersionSelection;
                featureSetting.DefaultTicketTypeID = srcFeatureSetting.DefaultTicketTypeID;
                featureSetting.DefaultGroupTypeID = srcFeatureSetting.DefaultGroupTypeID;
                featureSetting.EnableCustomerProductAssociation = srcFeatureSetting.EnableCustomerProductAssociation;
                featureSetting.EnableChat = srcFeatureSetting.EnableChat;
                featureSetting.EnableCommunity = srcFeatureSetting.EnableCommunity;
                featureSetting.EnableScreenRecording = srcFeatureSetting.EnableScreenRecording;
                featureSetting.EnableVideoRecording = srcFeatureSetting.EnableVideoRecording;
                featureSetting.EnableTicketSeverity = srcFeatureSetting.EnableTicketSeverity;
                featureSetting.EnableTicketSeverityModification = srcFeatureSetting.EnableTicketSeverityModification;
                featureSetting.RestrictProductVersions = srcFeatureSetting.RestrictProductVersions;
                featureSetting.EnableTicketNameModification = srcFeatureSetting.EnableTicketNameModification;

                featureSetting.CustomerHubID = newHub.CustomerHubID;
                featureSetting.DateModified = DateTime.UtcNow;
                featureSetting.ModifierID = loginUser.UserID;

                featureHelper.Save();

                CustomerHubLinkModel customerHubLinkModel = new CustomerHubLinkModel(newHub.CustomerHubID, newHub.PortalName, newHub.ProductFamilyID, string.Format("{0}.{1}", newHub.PortalName, SystemSettings.GetHubURL()));
                result = new CustomerHubCreationModel(customerHubLinkModel, true);
            }

            return result;
        }

        [WebMethod]
        public void DeleteHub(int hubID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            CustomerHubs hubHelper = new CustomerHubs(loginUser);
            hubHelper.LoadByOrganizationID(loginUser.OrganizationID);

            if (hubHelper.Count() > 1)
            {
                CustomerHubCustomViews.DeleteByCustomerHubID(loginUser, hubID);
                CustomerHubAuthentication.DeleteByCustomerHubID(loginUser, hubID);
                CustomerHubAuthentication.DeleteByCustomerHubID(loginUser, hubID);
                CustomerHubDisplaySettings.DeleteByCustomerHubID(loginUser, hubID);
                CustomerHubFeatureSettings.DeleteByCustomerHubID(loginUser, hubID);
                CustomerHubs.DeleteByCustomerHubID(loginUser, hubID);
            }

        }

        [WebMethod]
        public List<string> GetHubRedirectList()
        {
            CustomerHubs hubs = new CustomerHubs(TSAuthentication.GetLoginUser());
            hubs.LoadByOrganizationID(TSAuthentication.OrganizationID);
            List<string> results = new List<string>();

            if (hubs.Any())
            {
                foreach (var hub in hubs)
                {
                    if (string.IsNullOrWhiteSpace(hub.CNameURL))
                    {
                        results.Add(string.Format("{0}.{1}", hub.PortalName, SystemSettings.GetHubURL()));
                    }
                    else
                    {
                        results.Add(string.Format("{0}", hub.CNameURL));
                    }
                }
            }
            else
            {
                bool success = MigratePortalSettings(TSAuthentication.OrganizationID, TSAuthentication.GetLoginUser());
                if (success)
                {
                    CustomerHubs hubs2 = new CustomerHubs(TSAuthentication.GetLoginUser());
                    hubs2.LoadByOrganizationID(TSAuthentication.OrganizationID);
                    results.Add(string.Format("{0}.{1}", hubs2[0].PortalName, SystemSettings.GetHubURL()));
                }
            }
            return results;
        }


        [WebMethod]
        public string GetHubURLwithCName()
        {
            CustomerHubs hubs = new CustomerHubs(TSAuthentication.GetLoginUser());
            hubs.LoadByOrganizationID(TSAuthentication.OrganizationID);

            if (hubs.Any())
            {
                if (string.IsNullOrWhiteSpace(hubs[0].CNameURL))
                {
                    return string.Format("{0}.{1}", hubs[0].PortalName, SystemSettings.GetHubURL());
                }
                else
                {
                    return string.Format("{0}", hubs[0].CNameURL);
                }
            }
            else
            {
                bool success = MigratePortalSettings(TSAuthentication.OrganizationID, TSAuthentication.GetLoginUser());
                if (success)
                {
                    CustomerHubs hubs2 = new CustomerHubs(TSAuthentication.GetLoginUser());
                    hubs2.LoadByOrganizationID(TSAuthentication.OrganizationID);
                    return string.Format("{0}.{1}", hubs2[0].PortalName, SystemSettings.GetHubURL());
                }
            }
            return null;
        }

        /* SLA Trigger methods */
        [WebMethod]
        public SlaTrigger GetSlaTrigger(int slaTriggerId)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            SlaTrigger slaTrigger = SlaTriggers.GetSlaTrigger(loginUser, slaTriggerId);

            return slaTrigger;
        }

        /// <summary>
        /// Method to migrate the customers portal settings into the customer hub tables
        /// </summary>
        /// <returns>returns true if migration was successfull</returns>
        private bool MigratePortalSettings(int parentOrgID, LoginUser loginUser)
        {
            PortalOptions portals = new PortalOptions(loginUser);
            portals.LoadByOrganizationID(parentOrgID);

            if (portals.Any())
            {
                PortalOption portal = portals[0];
                CustomerHubs hubs = new CustomerHubs(loginUser);
                CustomerHub hub;
                hubs.LoadByOrganizationID(parentOrgID);

                //See if we have a existing hub record.  If so update it. 
                if (hubs.Any())
                {
                    hub = hubs[0];
                    hub.PortalName = Regex.Replace(portal.PortalName, "[^0-9a-zA-Z-]", "");
                    hub.OrganizationID = parentOrgID;
                    hub.IsActive = true;
                    hub.DateModified = DateTime.UtcNow;
                    hub.EnableMigration = false;
                    hubs.Save();
                }
                else
                {
                    //If not lets create a new record. 
                    CustomerHubs newHubs = new CustomerHubs(loginUser);
                    hub = newHubs.AddNewCustomerHub();
                    hub.OrganizationID = parentOrgID;
                    //Leave it inactive since this is a brand new hub.
                    hub.IsActive = true;
                    hub.DateModified = DateTime.UtcNow;

                    Organizations orgs = new Organizations(loginUser);
                    orgs.LoadByOrganizationID(parentOrgID);

                    if (orgs.Any())
                    {
                        //trim out all the white space and potentially offending characters from the org name for a temp portal name to use. 
                        hub.PortalName = Regex.Replace(orgs[0].Name, "[^0-9a-zA-Z-]", "");
                    }

                    newHubs.Save();
                }

                //Migrate portal setting to CustomerHubAuthentication Table
                MigratePortalOptionsToAuthentication(parentOrgID, hub.CustomerHubID, portal, loginUser);
                //Migrate portal setting to CustomerHubFeature Table
                MigratePortalOptionsToFeature(parentOrgID, hub.CustomerHubID, portal, loginUser);
                //Migrate portal setting to CustomerHubDisplay Table
                MigratePortalOptionsToDisplay(parentOrgID, hub.CustomerHubID, portal, loginUser);
            }
            else return false;

            return true;
        }

        private void MigratePortalOptionsToAuthentication(int parentOrgID, int customerHubID, PortalOption portal, LoginUser loginUser)
        {
            CustomerHubAuthentication auths = new CustomerHubAuthentication(loginUser);
            CustomerHubAuthentication newAuths = new CustomerHubAuthentication(loginUser);
            CustomerHubAuthenticationItem auth;
            auths.LoadByCustomerHubID(customerHubID);
            //Check and see if there is a existing record if not add one.
            if (!auths.Any())
            {
                auth = newAuths.AddNewCustomerHubAuthenticationItem();
            }
            else auth = auths[0];
            auth.CustomerHubID = customerHubID;
            auth.AnonymousHubAccess = true;
            auth.AnonymousKBAccess = true;
            auth.AnonymousProductAccess = false;
            auth.AnonymousTicketAccess = false;
            auth.AnonymousWikiAccess = true;
            auth.EnableRequestAccess = portal.RequestAccess;
            auth.EnableSelfRegister = portal.AutoRegister;
            auth.HonorServiceAgreementExpirationDate = portal.EnableSaExpiration;
            auth.HonorSupportExpiration = (portal.HonorSupportExpiration) ?? false;
            auth.EnableSSO = false;
            auth.DateModified = DateTime.UtcNow;

            if (portal.RequestType != null && portal.RequestType > 0)
            {
                auth.RequestTicketType = (int)portal.RequestType;
            }
            else
            {
                //If they never setup a request type then default to the top type
                TicketTypes types = new TicketTypes(loginUser);
                types.LoadByPosition(parentOrgID, 1);
                if (types.Any()) auth.RequestTicketType = types[0].TicketTypeID;
                else auth.RequestTicketType = 1;
            }

            if (portal.RequestGroup != null && portal.RequestGroup > 0)
            {
                auth.RequestGroupType = (int)portal.RequestGroup;
            }
            else
            {
                //If they never setup a request group then default to the top group
                Groups groups = new Groups(loginUser);
                groups.LoadByOrganizationID(parentOrgID);
                if (groups.Any()) auth.RequestGroupType = groups[0].GroupID;
                else auth.RequestGroupType = -1;
            }
            newAuths.Save();
            auths.Save();
        }

        private CustomerHubAuthenticationItemProxy CreateHubAuthentication(int parentOrgID, int customerHubID, LoginUser loginUser)
        {
            CustomerHubAuthentication newAuths = new CustomerHubAuthentication(loginUser);
            CustomerHubAuthenticationItem auth = newAuths.AddNewCustomerHubAuthenticationItem();

            auth.CustomerHubID = customerHubID;
            auth.AnonymousHubAccess = true;
            auth.AnonymousKBAccess = true;
            auth.AnonymousProductAccess = false;
            auth.AnonymousTicketAccess = false;
            auth.AnonymousWikiAccess = true;
            auth.EnableRequestAccess = false;
            auth.EnableSelfRegister = false;
            auth.HonorServiceAgreementExpirationDate = false;
            auth.HonorSupportExpiration = false;
            auth.EnableSSO = false;
            auth.DateModified = DateTime.UtcNow;

            TicketTypes types = new TicketTypes(loginUser);
            types.LoadByPosition(parentOrgID, 1);
            if (types.Any()) auth.RequestTicketType = types[0].TicketTypeID;
            else auth.RequestTicketType = 1;

            Groups groups = new Groups(loginUser);
            groups.LoadByOrganizationID(parentOrgID);
            if (groups.Any()) auth.RequestGroupType = groups[0].GroupID;
            else auth.RequestGroupType = -1;

            newAuths.Save();
            return auth.GetProxy();
        }

        private void MigratePortalOptionsToFeature(int parentOrgID, int customerHubID, PortalOption portal, LoginUser loginUser)
        {
            CustomerHubFeatureSettings features = new CustomerHubFeatureSettings(loginUser);
            CustomerHubFeatureSettings newFeatures = new CustomerHubFeatureSettings(loginUser);
            CustomerHubFeatureSetting feature;
            features.LoadByCustomerHubID(customerHubID);
            //Check and see if there is a existing record if not add one.
            if (!features.Any())
            {
                feature = newFeatures.AddNewCustomerHubFeatureSetting();
            }
            else feature = features[0];
            feature.CustomerHubID = customerHubID;
            feature.EnableKnowledgeBase = portal.DisplayAdvKB;
            feature.EnableWiki = portal.DisplayAdvArticles;
            feature.EnableProducts = portal.DisplayAdvProducts;
            feature.EnableTicketCreation = portal.DisplayAdvArticles;
            feature.EnableTicketGroupSelection = (portal.DisplayGroups) ?? true;
            feature.EnableTicketProductSelection = portal.DisplayAdvProducts;
            feature.EnableTicketProductVersionSelection = portal.DisplayProductVersion;
            feature.EnableMyTickets = true;
            feature.EnableTicketCreation = true;
            feature.EnableOrganizationTickets = false;
            feature.EnableScreenRecording = portal.EnableScreenr;
            feature.EnableVideoRecording = portal.EnableVideoRecording;
            feature.DateModified = DateTime.UtcNow;

            if (portal.RequestType != null && portal.RequestType > 0)
            {
                feature.DefaultTicketTypeID = (int)portal.RequestType;
            }
            else
            {
                //If they never setup a request type then default to the top visible type
                TicketTypes types = new TicketTypes(loginUser);
                types.LoadPortalTypesByOrganizationID(parentOrgID);
                if (types.Any()) feature.DefaultTicketTypeID = types[0].TicketTypeID;
                else feature.DefaultTicketTypeID = -1;
            }

            if (portal.RequestGroup != null && portal.RequestGroup > 0)
            {
                feature.DefaultGroupTypeID = (int)portal.RequestGroup;
            }
            else
            {
                //If they never setup a request group then default to the top group
                Groups groups = new Groups(loginUser);
                groups.LoadByOrganizationID(parentOrgID);
                if (groups.Any()) feature.DefaultGroupTypeID = groups[0].GroupID;
                else feature.DefaultGroupTypeID = -1;
            }

            newFeatures.Save();
            features.Save();
        }

        private CustomerHubFeatureSettingProxy CreateHubFeature(int parentOrgID, int customerHubID, LoginUser loginUser)
        {
            CustomerHubFeatureSettings newFeatures = new CustomerHubFeatureSettings(loginUser);
            CustomerHubFeatureSetting feature = newFeatures.AddNewCustomerHubFeatureSetting();

            feature.CustomerHubID = customerHubID;
            feature.EnableKnowledgeBase = true;
            feature.EnableWiki = true;
            feature.EnableProducts = true;
            feature.EnableTicketCreation = true;
            feature.EnableTicketGroupSelection = false;
            feature.EnableTicketProductSelection = false;
            feature.EnableTicketProductVersionSelection = false;
            feature.EnableMyTickets = true;
            feature.EnableTicketCreation = true;
            feature.EnableOrganizationTickets = false;
            feature.DateModified = DateTime.UtcNow;

            TicketTypes types = new TicketTypes(loginUser);
            types.LoadPortalTypesByOrganizationID(parentOrgID);
            if (types.Any()) feature.DefaultTicketTypeID = types[0].TicketTypeID;
            else feature.DefaultTicketTypeID = -1;

            Groups groups = new Groups(loginUser);
            groups.LoadByOrganizationID(parentOrgID);
            if (groups.Any()) feature.DefaultGroupTypeID = groups[0].GroupID;
            else feature.DefaultGroupTypeID = -1;

            newFeatures.Save();
            return feature.GetProxy();
        }

        private void MigratePortalOptionsToDisplay(int parentOrgID, int customerHubID, PortalOption portal, LoginUser loginUser)
        {
            CustomerHubDisplaySettings settings = new CustomerHubDisplaySettings(loginUser);
            CustomerHubDisplaySettings newSettings = new CustomerHubDisplaySettings(loginUser);
            CustomerHubDisplaySetting setting;
            settings.LoadByCustomerHubID(customerHubID);
            //Check and see if there is a existing record if not add one.
            if (!settings.Any())
            {
                setting = newSettings.AddNewCustomerHubDisplaySetting();
            }
            else setting = settings[0];
            setting.CustomerHubID = customerHubID;
            setting.FontColor = portal.FontColor;
            setting.FontFamily = portal.FontFamily;
            setting.Color1 = "#2A3F54";
            setting.Color2 = "#FFFFFF";
            setting.Color3 = "#7C7C7C";
            setting.Color4 = "#41414D";
            setting.Color5 = "#E2E2E2";
            setting.DateModified = DateTime.UtcNow;

            newSettings.Save();
            settings.Save();
        }

        private CustomerHubDisplaySettingProxy CreateHubDisplay(int parentOrgID, int customerHubID, LoginUser loginUser)
        {
            CustomerHubDisplaySettings settings = new CustomerHubDisplaySettings(loginUser);
            CustomerHubDisplaySettings newSettings = new CustomerHubDisplaySettings(loginUser);
            CustomerHubDisplaySetting setting = newSettings.AddNewCustomerHubDisplaySetting();
            setting.CustomerHubID = customerHubID;
            setting.FontColor = "#131111";
            setting.FontFamily = null;
            setting.Color1 = "#2A3F54";
            setting.Color2 = "#FFFFFF";
            setting.Color3 = "#7C7C7C";
            setting.Color4 = "#41414D";
            setting.Color5 = "#E2E2E2";
            setting.DateModified = DateTime.UtcNow;

            newSettings.Save();
            return setting.GetProxy();
        }

        private string GetRollbackActionsQuery()
        {
            return @"
			DELETE
				a
			FROM 
				Actions a
				JOIN Tickets t
					ON a.TicketID = t.TicketID
			WHERE
				t.OrganizationID = @OrganizationID
				AND a.ImportFileID = @ImportFileID
		";
        }

        private string GetRollbackAssetsQuery()
        {
            return @"
			DELETE
				aa
			FROM
				AssetAssignments aa
				JOIN AssetHistory ah
					ON aa.HistoryID = ah.HistoryID
			WHERE
				ah.OrganizationID = @OrganizationID
				AND aa.ImportFileID = @ImportFileID

			DELETE 
				AssetHistory 
			WHERE 
				OrganizationID = @OrganizationID
				AND ImportFileID = @ImportFileID

			DELETE 
				Assets 
			WHERE 
				OrganizationID = @OrganizationID
				AND ImportFileID = @ImportFileID
		";
        }

        private string GetRollbackAssetTicketsQuery()
        {
            return @"
			DELETE
				at
			FROM
				AssetTickets at
				JOIN Tickets t
					ON at.TicketID = t.TicketID
			WHERE
				t.OrganizationID = @OrganizationID
				AND at.ImportFileID = @ImportFileID
		 ";
        }

        private string GetRollbackCustomValuesQuery()
        {
            return @"
			DELETE
				cv
			FROM
				CustomValues cv
				JOIN CustomFields cf
					ON cv.CustomFieldID = cf.CustomFieldID
			WHERE
				cf.OrganizationID = @OrganizationID
				AND cv.ImportFileID = @ImportFileID
		 ";
        }

        private string GetRollbackAddressesAndPhoneNumbersQuery()
        {
            return @"
			DELETE
				a
			FROM
				Addresses a
				LEFT JOIN Users u
					ON a.RefType = 22
					AND a.RefID = u.UserID
				LEFT JOIN Organizations uo
					ON a.RefType = 22
					AND u.OrganizationID = uo.OrganizationID
				LEFT JOIN Organizations o
					ON a.RefType = 9
					AND a.RefID = o.OrganizationID
			WHERE
				(
					(a.RefType = 9 AND o.ParentID = @OrganizationID)
					OR (a.RefType = 22 AND (uo.ParentID = @OrganizationID OR uo.OrganizationID = @OrganizationID))
				)
				AND a.ImportFileID = @ImportFileID

			DELETE
				pn
			FROM
				PhoneNumbers pn
				LEFT JOIN Users c
					ON pn.RefType = 32
					AND pn.RefID = c.UserID
				LEFT JOIN Organizations co
					ON pn.RefType = 32
					AND c.OrganizationID = co.OrganizationID
				LEFT JOIN Organizations o
					ON pn.RefType = 9
					AND pn.RefID = o.OrganizationID
				LEFT JOIN Users u
					ON pn.RefType = 22
					AND pn.RefID = u.UserID
			WHERE
				(
					(pn.RefType = 9 AND o.ParentID = @OrganizationID)
					OR (pn.RefType = 32 AND co.ParentID = @OrganizationID)
					OR (pn.RefType = 22 AND u.OrganizationID = @OrganizationID)
				)
				AND pn.ImportFileID = @ImportFileID
		 ";
        }

        private string GetRollbackCompaniesQuery()
        {
            return @"
			DELETE 
				Organizations 
			WHERE 
				ParentID = @OrganizationID
				AND ImportFileID = @ImportFileID
		 ";
        }

        private string GetRollbackContactsQuery()
        {
            return @"
			DELETE
				c
			FROM 
				Users c
				JOIN Organizations o
					ON c.OrganizationID = o.OrganizationID
			WHERE
				o.ParentID = @OrganizationID
				AND c.ImportFileID = @ImportFileID
		 ";
        }

        private string GetRollbackProductsQuery()
        {
            return @"
			DELETE 
				Products 
			WHERE 
				OrganizationID = @OrganizationID
				AND ImportFileID = @ImportFileID
		 ";
        }

        private string GetRollbackProductVersionsQuery()
        {
            return @"
			DELETE
				pv
			FROM 
				ProductVersions pv
				JOIN Products p
					ON pv.ProductID = p.ProductID
			WHERE
				p.OrganizationID = @OrganizationID
				AND pv.ImportFileID = @ImportFileID
		 ";
        }

        private string GetRollbackTicketRelationshipsQuery()
        {
            return @"
			DELETE 
				TicketRelationships 
			WHERE 
				OrganizationID = @OrganizationID
				AND ImportFileID = @ImportFileID
		 ";
        }

        private string GetRollbackContactTicketsQuery()
        {
            return @"
			DELETE
				ut
			FROM
				UserTickets ut
				JOIN Tickets t
					ON ut.TicketID = t.TicketID
			WHERE
				t.OrganizationID = @OrganizationID
				AND ut.ImportFileID = @ImportFileID
		 ";
        }

        private string GetRollbackOrganizationTicketsQuery()
        {
            return @"
			DELETE
				ot
			FROM 
				OrganizationTickets ot
				JOIN Organizations o
					ON ot.OrganizationID = o.OrganizationID
			WHERE
				o.ParentID = @OrganizationID
				AND ot.ImportFileID = @ImportFileID
		 ";
        }

        private string GetRollbackTicketsQuery()
        {
            return @"
			DELETE 
				Tickets
			WHERE 
				OrganizationID = @OrganizationID
				AND ImportFileID = @ImportFileID
		 ";
        }

        private string GetRollbackUsersQuery()
        {
            return @"
			DELETE
				u
			FROM
				Users u
			WHERE
				u.OrganizationID = @OrganizationID
				AND u.ImportFileID = @ImportFileID
		 ";
        }

        private string GetRollbackOrganizationProductsQuery()
        {
            return @"
			DELETE
				op
			FROM
				OrganizationProducts op
				JOIN Organizations o
					ON op.OrganizationID = o.OrganizationID
			WHERE
				o.ParentID = @OrganizationID
				AND op.ImportFileID = @ImportFileID
		 ";
        }

        private string GetRollbackOrganizationNotesQuery()
        {
            return @"
			DELETE
				n
			FROM
				Notes n
				JOIN Organizations o
					ON n.RefType = 9
					AND n.RefID = o.OrganizationID
			WHERE
				o.ParentID = @OrganizationID
				AND n.ImportFileID = @ImportFileID
		 ";
        }
    }

    [DataContract(Namespace = "http://teamsupport.com/")]
    public class ForumCategoryInfo
    {
        public ForumCategoryInfo() { }
        [DataMember]
        public ForumCategoryProxy Category { get; set; }
        [DataMember]
        public ForumCategoryProxy[] Subcategories { get; set; }
    }

    [DataContract(Namespace = "http://teamsupport.com/")]
    public class ForumCategoryOrder
    {
        public ForumCategoryOrder() { }
        [DataMember]
        public int? ParentID { get; set; }
        [DataMember]
        public List<int> CategoryIDs { get; set; }
    }

    [DataContract(Namespace = "http://teamsupport.com/")]
    public class KnowledgeBaseCategoryInfo
    {
        public KnowledgeBaseCategoryInfo() { }
        [DataMember]
        public KnowledgeBaseCategoryProxy Category { get; set; }
        [DataMember]
        public KnowledgeBaseCategoryProxy[] Subcategories { get; set; }
    }

    [DataContract(Namespace = "http://teamsupport.com/")]
    public class KnowledgeBaseCategoryOrder
    {
        public KnowledgeBaseCategoryOrder() { }
        [DataMember]
        public int? ParentID { get; set; }
        [DataMember]
        public List<int> CategoryIDs { get; set; }
    }
    [DataContract(Namespace = "http://teamsupport.com/")]
    public class CustomerHubCreationModel
    {
        public CustomerHubCreationModel(CustomerHubLinkModel hubLinkModel, bool isSuccess, string errorMessage = null)
        {
            HubLinkModel = hubLinkModel;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }
        public CustomerHubLinkModel HubLinkModel { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

    }

    [DataContract(Namespace = "http://teamsupport.com/")]
    public class CustomerHubLinkModel
    {
        public CustomerHubLinkModel() { }
        public CustomerHubLinkModel(int hubid, string name, int? productFamilyID, string url)
        {
            HubID = hubid;
            Name = name;
            URL = url;
            ProductFamilyID = productFamilyID;
        }
        [DataMember]
        public int HubID { get; set; }
        public string Name { get; set; }
        public int? ProductFamilyID { get; set; }
        public string URL { get; set; }
    }

    public class ImportLog
    {
        private string _logPath;
        private string _fileName;

        public ImportLog(string path, int importID)
        {
            _logPath = path;
            _fileName = importID.ToString() + ".txt";

            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
        }

        public void Write(string text)
        {
            if (!File.Exists(_logPath + @"\" + _fileName))
            {
                foreach (string oldFileName in Directory.GetFiles(_logPath))
                {
                    if (File.GetLastWriteTime(oldFileName).AddDays(30) < DateTime.Today)
                    {
                        File.Delete(oldFileName);
                    }
                }
            }

            File.AppendAllText(_logPath + @"\" + _fileName, DateTime.Now.ToLongTimeString() + ": " + text + Environment.NewLine);
        }
    }
}