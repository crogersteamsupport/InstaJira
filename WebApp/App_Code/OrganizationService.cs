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
using Newtonsoft.Json;
using LumenWorks.Framework.IO.Csv;
using System.Linq;
using System.Dynamic;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class OrganizationService : System.Web.Services.WebService
    {

        public OrganizationService()
        {

            //Uncomment the following line if using designed components
            //InitializeComponent();
        }

        [WebMethod]
        public int GetIDByName(string name)
        {
            name = name.Replace('+', ' ').Replace('_', ' ');
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByLikeOrganizationName(TSAuthentication.OrganizationID, name, true, 1);
            if (organizations.IsEmpty) return -1;
            return organizations[0].OrganizationID;
        }


        [WebMethod]
        public int GetIDByExactName(string name)
        {
            name = name.Replace('+', ' ').Replace('_', ' ');
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationNameActive(name, TSAuthentication.OrganizationID);
            if (organizations.IsEmpty) return -1;
            return organizations[0].OrganizationID;
        }

        [WebMethod]
        public int GetIDByExactNameNoFilter(string name)
        {
            //name = name.Replace('+', ' ').Replace('_', ' ');
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationNameActive(name, TSAuthentication.OrganizationID);
            if (organizations.IsEmpty) return -1;
            return organizations[0].OrganizationID;
        }

        //[0] = orgid
        //[1] = userid
        [WebMethod]
        public int[] GetIDByPhone(string phone)
        {
            int[] result = new int[2] { -1, -1 };
            phone = phone.Replace(")", "").Replace("(", "").Replace("-", "").Replace(".", "").Replace(" ", "").Replace("+1", "").Replace("+", "").ToLower();

            PhoneNumbers phoneNumbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
            phoneNumbers.LoadByOrganizationID(TSAuthentication.OrganizationID);

            foreach (PhoneNumber phoneNumber in phoneNumbers)
            {
                string number = phoneNumber.Number.Replace(")", "").Replace("(", "").Replace("-", "").Replace(".", "").Replace(" ", "").Replace("+1", "").Replace("+", "").ToLower();
                if (number == phone)
                {
                    if (phoneNumber.RefType == ReferenceType.Organizations)
                    {
                        result[0] = phoneNumber.RefID;
                    }
                    else
                    {
                        User user = Users.GetUser(TSAuthentication.GetLoginUser(), phoneNumber.RefID);
                        if (user != null)
                        {
                            result[0] = user.OrganizationID;
                            result[1] = user.UserID;
                            break;
                        }

                    }
                }
            }
            return result;
        }

        // need to move to users service
        [WebMethod]
        public UserProxy GetUser(int userID)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user == null) return null;

            if (user.OrganizationID != TSAuthentication.OrganizationID)
            {
                Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), user.OrganizationID);
                if (organization.ParentID != TSAuthentication.OrganizationID) return null;
            }

			user.CryptedPassword = string.Empty;

            return user.GetProxy();
        }

        [WebMethod]
        public OrganizationProxy GetOrganization(int organizationID)
        {
            Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), organizationID);
            if (TSAuthentication.OrganizationID != 1088 && TSAuthentication.OrganizationID != 1078 && organization.ParentID != TSAuthentication.OrganizationID && organization.OrganizationID != TSAuthentication.OrganizationID) return null;
            return organization.GetProxy();
        }

        [WebMethod]
        public bool IsProductRequired()
        {
            Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID);

            return organization.ProductRequired;
        }

        [WebMethod]
        public bool IsProductVersionRequired()
        {
            Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID);

            return organization.ProductVersionRequired;
        }

        [WebMethod]
        public PortalOptionProxy GetPortalOption(int organizationID)
        {
            Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), organizationID);
            //if (organization.OrganizationID != TSAuthentication.OrganizationID || !TSAuthentication.IsSystemAdmin) return null;
            PortalOption result = PortalOptions.GetPortalOption(organization.Collection.LoginUser, organizationID);

            if (result == null)
            {
                result = (new PortalOptions(organization.Collection.LoginUser).AddNewPortalOption());
                result.OrganizationID = organizationID;
                result.Collection.Save();
            }


            return result.GetProxy();
        }

        [WebMethod]
        public AgentRatingsOptionProxy GetAgentRatingOptions(int organizationID)
        {

            AgentRatingsOption options = AgentRatingsOptions.GetAgentRatingsOption(TSAuthentication.GetLoginUser(), organizationID);
            if (options == null)
                return null;
            else
                return options.GetProxy();
        }

        [WebMethod]
        public void SaveAgentRatings(AgentRatingsOptionProxy agentproxy)
        {
            AgentRatingsOption agentRatingOption = AgentRatingsOptions.GetAgentRatingsOption(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID);
            if (agentRatingOption == null)
            {
                AgentRatingsOptions aro = new AgentRatingsOptions(TSAuthentication.GetLoginUser());
                aro.AddNewAgentRatingsOption();
                aro[0].OrganizationID = TSAuthentication.OrganizationID;
                aro[0].PositiveRatingText = agentproxy.PositiveRatingText;
                aro[0].NeutralRatingText = agentproxy.NeutralRatingText;
                aro[0].NegativeRatingText = agentproxy.NegativeRatingText;
                aro[0].RedirectURL = agentproxy.RedirectURL;
                aro[0].ExternalPageLink = agentproxy.ExternalPageLink;
                aro[0].Collection.Save();
            }
            else
            {
                agentRatingOption.PositiveRatingText = agentproxy.PositiveRatingText;
                agentRatingOption.NeutralRatingText = agentproxy.NeutralRatingText;
                agentRatingOption.NegativeRatingText = agentproxy.NegativeRatingText;
                agentRatingOption.RedirectURL = agentproxy.RedirectURL;
                agentRatingOption.ExternalPageLink = agentproxy.ExternalPageLink;
                agentRatingOption.Collection.Save();
            }
        }

        [WebMethod]
        public void SaveCDISettings(CDI_SettingProxy cdi)
        {
            CDI_Setting cdiSettings = CDI_Settings.GetCDI_Setting(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID);
            if (cdiSettings == null)
            {
                CDI_Setting cdiSetting = (new CDI_Settings(TSAuthentication.GetLoginUser())).AddNewCDI_Setting();
                cdiSetting.OrganizationID = TSAuthentication.OrganizationID;
                cdiSetting.TotalTicketsWeight = cdi.TotalTicketsWeight;
                cdiSetting.OpenTicketsWeight = cdi.OpenTicketsWeight;
                cdiSetting.Last30Weight = cdi.Last30Weight;
                cdiSetting.AvgDaysOpenWeight = cdi.AvgDaysOpenWeight;
                cdiSetting.AvgDaysToCloseWeight = cdi.AvgDaysToCloseWeight;
                cdiSetting.GreenUpperRange = cdi.GreenUpperRange;
                cdiSetting.YellowUpperRange = cdi.YellowUpperRange;
                cdiSetting.AverageActionCountWeight = cdi.AverageActionCountWeight;
                cdiSetting.AverageSentimentScoreWeight = cdi.AverageSentimentScoreWeight;
                cdiSetting.AverageSeverityWeight = cdi.AverageSeverityWeight;
                cdiSetting.AgentRatingsWeight = cdi.AgentRatingsWeight;
                cdiSetting.Collection.Save();
            }
            else
            {
                cdiSettings.AverageActionCountWeight = cdi.AverageActionCountWeight;
                cdiSettings.AverageSentimentScoreWeight = cdi.AverageSentimentScoreWeight;
                cdiSettings.AverageSeverityWeight = cdi.AverageSeverityWeight;
                cdiSettings.AgentRatingsWeight = cdi.AgentRatingsWeight;
                cdiSettings.TotalTicketsWeight = cdi.TotalTicketsWeight;
                cdiSettings.OpenTicketsWeight = cdi.OpenTicketsWeight;
                cdiSettings.Last30Weight = cdi.Last30Weight;
                cdiSettings.AvgDaysOpenWeight = cdi.AvgDaysOpenWeight;
                cdiSettings.AvgDaysToCloseWeight = cdi.AvgDaysToCloseWeight;
                cdiSettings.GreenUpperRange = cdi.GreenUpperRange;
                cdiSettings.YellowUpperRange = cdi.YellowUpperRange;
                cdiSettings.Collection.Save();
            }
        }


        [WebMethod]
        public string SetPortalOption(PortalOptionProxy proxy, string externalLink, bool isPublicArticles)
        {
            Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), proxy.OrganizationID);
            if (organization.OrganizationID != TSAuthentication.OrganizationID || !TSAuthentication.IsSystemAdmin) return null;

            PortalOption option = PortalOptions.GetPortalOption(organization.Collection.LoginUser, proxy.OrganizationID);

            option.PortalName = PortalOptions.ValidatePortalNameChars(proxy.PortalName);
            PortalOptions options = new PortalOptions(organization.Collection.LoginUser);
            options.LoadByPortalName(option.PortalName);
            if (options.Count > 0)
            {
                if (options.Count > 1 || options[0].OrganizationID != TSAuthentication.OrganizationID)
                {
                    return "That portal name is already taken.  Please choose a different portal name.";
                }
            }

            FacebookOptions fb = new FacebookOptions(TSAuthentication.GetLoginUser());
            fb.LoadByOrganizationID(proxy.OrganizationID);
            if (fb.IsEmpty)
            {
                FacebookOption fbo = (new FacebookOptions(TSAuthentication.GetLoginUser()).AddNewFacebookOption());
                fbo.OrganizationID = proxy.OrganizationID;
                fbo.DisplayArticles = proxy.DisplayFbArticles;
                fbo.DisplayKB = proxy.DisplayFbKB;
                fbo.Collection.Save();
            }
            else
            {
                fb[0].DisplayArticles = proxy.DisplayFbArticles;
                fb[0].DisplayKB = proxy.DisplayFbKB;
                fb[0].Collection.Save();
            }

            option.AllowNameEditing = proxy.AllowNameEditing;
            option.AllowSeverityEditing = proxy.AllowSeverityEditing;
            option.EnableSaExpiration = proxy.EnableSaExpiration;
            option.PublicLandingPageBody = proxy.PublicLandingPageBody;
            option.PublicLandingPageHeader = proxy.PublicLandingPageHeader;
            option.EnableScreenr = proxy.EnableScreenr;
            option.EnableVideoRecording = proxy.EnableVideoRecording;
            option.DisplayLandingPage = proxy.DisplayLandingPage;
            option.LandingPageHtml = proxy.LandingPageHtml;
            option.DisplayProductVersion = proxy.DisplayProductVersion;
            option.RestrictProductVersion = proxy.RestrictProductVersion;
            option.DisplayAdvKB = proxy.DisplayAdvKB;
            option.DisplayAdvProducts = proxy.DisplayAdvProducts;
            option.DisplaySettings = proxy.DisplaySettings;
            option.DisplayLogout = proxy.DisplayLogout;
            option.DisplayPortalPhone = proxy.DisplayPortalPhone;
            option.DisplayFooter = proxy.DisplayFooter;
            option.DisplayForum = proxy.DisplayForum;
            option.DeflectionEnabled = proxy.DeflectionEnabled;
            option.BasicPortalDirections = proxy.BasicPortalDirections;
            option.AdvPortalWidth = proxy.AdvPortalWidth;
            option.Theme = proxy.Theme;
            option.HideCloseButton = proxy.HideCloseButton;
            option.HonorSupportExpiration = proxy.HonorSupportExpiration;
            option.DisplayProducts = proxy.DisplayProducts;
            option.KBAccess = proxy.KBAccess;
            option.DisablePublicMyTickets = proxy.DisablePublicMyTickets;
            //option.PortalName = proxy.PortalName;
            option.DisplayGroups = proxy.DisplayGroups;
            option.BasicPortalColumnWidth = proxy.BasicPortalColumnWidth;
            option.HideGroupAssignedTo = proxy.HideGroupAssignedTo;
            option.HideUserAssignedTo = proxy.HideUserAssignedTo;
            option.CompanyRequiredInBasic = proxy.CompanyRequiredInBasic;
            option.UseCompanyInBasic = proxy.UseCompanyInBasic;
            option.PageBackgroundColor = proxy.PageBackgroundColor;
            //option.FontColor = proxy.FontColor;
            //option.FontFamily = proxy.FontFamily;
            option.UseRecaptcha = proxy.UseRecaptcha;
            option.PortalHTMLFooter = proxy.PortalHTMLFooter;
            option.PortalHTMLHeader = proxy.PortalHTMLHeader;
            option.DisplayAdvArticles = proxy.DisplayAdvArticles;
            //option.OrganizationID = proxy.OrganizationID;
            option.TwoColumnFields = proxy.TwoColumnFields;
            option.DisplayForum = proxy.DisplayForum;
            option.DisplayTandC = proxy.DisplayTandC;
            option.TermsAndConditions = proxy.TermsAndConditions;
            option.AutoRegister = proxy.AutoRegister;
            option.RequestAccess = proxy.RequestAccess;
            option.RequestGroup = proxy.RequestGroup;
            option.RequestType = proxy.RequestType;
            option.PortalForward = proxy.PortalForward;

            option.Collection.Save();

            //organization.AgentRating = agentRatingEnabled;
            organization.IsPublicArticles = isPublicArticles;
            organization.UseForums = proxy.DisplayForum == null ? false : (bool)proxy.DisplayForum;
            organization.Collection.Save();

            externalLink = externalLink.Trim();
            if (externalLink.IndexOf("http") < 0 && externalLink != "") externalLink = "http://" + externalLink;
            Settings.OrganizationDB.WriteString("ExternalPortalLink", externalLink);

            return null;
        }

        [WebMethod]
        public void ResetRatingImage(int ratingID)
        {
            AgentRatingsOptions ratingOptions = new AgentRatingsOptions(TSAuthentication.GetLoginUser());
            ratingOptions.LoadByOrganizationID(TSAuthentication.OrganizationID);


            switch (ratingID)
            {
                case -1:
                    ratingOptions[0].NegativeImage = null;
                    break;
                case 0:
                    ratingOptions[0].NeutralImage = null;
                    break;
                case 1:
                    ratingOptions[0].PositiveImage = null;
                    break;
            }

            ratingOptions[0].Collection.Save();


            return;
        }


        [WebMethod]
        public bool? UpdateUseCommunity(bool value)
        {
            if (!TSAuthentication.IsSystemAdmin) return null;
            Organization org = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID);
            org.UseForums = value;
            org.Collection.Save();
            return value;
        }

        [WebMethod]
        public CRMLinkTableItemProxy[] AdminGetCrmLinks(int organizationID)
        {

            if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return null;

            CRMLinkTable table = new CRMLinkTable(TSAuthentication.GetLoginUser());
            table.LoadByOrganizationID(organizationID);
            return table.GetCRMLinkTableItemProxies().OrderBy(p => p.CRMType).Select(p => p).ToArray();
        }

        [WebMethod]
        public void AdminDeleteOrganization(int organizationID, string password)
        {
            if (TSAuthentication.OrganizationID == 1078)
            {
                string crtyped = FormsAuthentication.HashPasswordForStoringInConfigFile(password.Trim(), "MD5");
                User eric = Users.GetUser(TSAuthentication.GetLoginUser(), 43);
                User kevin = Users.GetUser(TSAuthentication.GetLoginUser(), 34);
                if (crtyped == eric.CryptedPassword || crtyped == kevin.CryptedPassword)
                {
                    Organizations.DeleteOrganizationAndAllReleatedData(TSAuthentication.GetLoginUser(), organizationID);
                    return;
                }
                throw new Exception("Invalid Password");
            }

        }

        [WebMethod]
        public void SetShowWelcomePage(int organizationID)
        {
            if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return;
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByOrganizationID(organizationID, true);
            foreach (User user in users)
            {
                if (user.IsSystemAdmin) user.ShowWelcomePage = true;
            }

            users.Save();
        }


        [WebMethod]
        public void ResetCrmLastLink(int crmLinkID)
        {
            if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return;
            CRMLinkTableItem item = CRMLinkTable.GetCRMLinkTableItem(TSAuthentication.GetLoginUser(), crmLinkID);
            item.LastLink = null;
            item.Collection.Save();
        }

        [WebMethod]
        public CRMLinkTableItemProxy[] GetCrmLinks()
        {
            if (!TSAuthentication.IsSystemAdmin) return null;
            CRMLinkTable table = new CRMLinkTable(TSAuthentication.GetLoginUser());
            table.LoadByOrganizationID(TSAuthentication.OrganizationID);
			CRMLinkTableItemProxy[] crmLinks = table.GetCRMLinkTableItemProxies();

			//Build the whole webhook urls (if applicable)
			foreach (CRMLinkTableItem crmlink in table)
			{
				if (crmlink.WebHookTokenId != null)
				{
					WebHooksTokenItem crmWebHookToken = WebHooksToken.GetWebHooksTokenItem(TSAuthentication.GetLoginUser(), (int)crmlink.WebHookTokenId);
					IntegrationType integrationType = (IntegrationType)Enum.Parse(typeof(IntegrationType), crmlink.CRMType);

					crmlink.WebHookTokenFullUrl = string.Format("{0}/WebHook/Integration/{1}/{2}/{3}", SystemSettings.GetAppUrl(), integrationType, TSAuthentication.OrganizationID, crmWebHookToken.Token);
					if (crmLinks.Where(p => p.WebHookTokenId == crmlink.WebHookTokenId).Any())
					{
						CRMLinkTableItemProxy proxy = crmLinks.Where(p => p.WebHookTokenId == crmlink.WebHookTokenId).FirstOrDefault();
						proxy.WebHookTokenFullUrl = crmlink.WebHookTokenFullUrl;
					}
				}
			}

			return crmLinks;
        }

        [WebMethod]
        public CRMLinkTableItemProxy[] LoadOrgCrmLinks(int organizationID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            CRMLinkTable table = new CRMLinkTable(loginUser);
            table.LoadByOrganizationID(organizationID);
            return table.GetCRMLinkTableItemProxies();
        }

        [WebMethod]
        public SlaLevelProxy[] GetSlaLevels()
        {
            if (!TSAuthentication.IsSystemAdmin) return null;
            SlaLevels table = new SlaLevels(TSAuthentication.GetLoginUser());
            table.LoadByOrganizationID(TSAuthentication.OrganizationID);
            return table.GetSlaLevelProxies();
        }

        [WebMethod]
        public CRMLinkTableItemProxy SaveCrmLink(
            int crmLinkID,
            bool isActive,
            string crmType,
            string password,
            string token,
            string tag,
            string userName,
            bool email,
            bool portal,
            int? defaultSlaLevelID,
            bool pullCasesAsTickets,
            bool pushTicketsAsCases,
            bool pushTicketsAsAccountComments,
            bool pullCustomerProducts,
            int? actionTypeIDToPush,
            string hostName,
            string defaultProject,
            bool? updateStatus,
            bool updateTicketType,
            bool matchAccountsByName,
            bool useSandBoxServer,
            bool alwaysUseDefaultProjectKey,
            bool includeIssueNonRequired,
            string restrictedToTicketTypes,
            string excludedTicketStatuses,
            string jiraInstanceName,
            bool useNetworkCredentials,
			string webhookToken
            )
        {
            if (!TSAuthentication.IsSystemAdmin) return null;
            CRMLinkTableItem item;

            if (crmLinkID < 0)
            {
                item = (new CRMLinkTable(TSAuthentication.GetLoginUser())).AddNewCRMLinkTableItem();
                item.LastProcessed = DateTime.UtcNow;
                item.LastTicketID = -1;
                item.OrganizationID = TSAuthentication.OrganizationID;
                item.SendBackTicketData = true;
            }
            else
            {
                item = CRMLinkTable.GetCRMLinkTableItem(TSAuthentication.GetLoginUser(), crmLinkID);

                if (item.TypeFieldMatch != tag && (crmType == "Batchbook" || crmType == "Highrise" || crmType == "Salesforce" || crmType == "ZohoCrm"))
                {
                    item.LastLink = null;
                }

                if (item.OrganizationID != TSAuthentication.OrganizationID) return null;
            }

            item.InstanceName = jiraInstanceName;
            item.Active = isActive;
            item.AllowPortalAccess = portal;
            item.CRMType = crmType;
            item.SendWelcomeEmail = email;
            item.Password = password;
            item.SecurityToken1 = token;
            item.TypeFieldMatch = tag;
            item.Username = userName;
            item.DefaultSlaLevelID = defaultSlaLevelID;
            item.PullCasesAsTickets = pullCasesAsTickets;
            item.PushTicketsAsCases = pushTicketsAsCases;
            item.SendBackTicketData = pushTicketsAsAccountComments;
            item.PullCustomerProducts = pullCustomerProducts;
            item.ActionTypeIDToPush = actionTypeIDToPush;
            item.HostName = hostName;
            item.DefaultProject = defaultProject;
            item.UpdateStatus = updateStatus;
            item.MatchAccountsByName = matchAccountsByName;
            item.UpdateTicketType = updateTicketType;
            item.UseSandBoxServer = useSandBoxServer;
            item.AlwaysUseDefaultProjectKey = alwaysUseDefaultProjectKey;
            item.IncludeIssueNonRequired = includeIssueNonRequired;
            item.RestrictedToTicketTypes = restrictedToTicketTypes;
            item.ExcludedTicketStatusUpdate = excludedTicketStatuses;
            item.UseNetworkCredentials = useNetworkCredentials;
			item.Collection.Save();

			if (!string.IsNullOrEmpty(webhookToken) && item.CRMLinkID > 0)
			{
				Uri uri = new Uri(webhookToken);

				if (uri.Segments.Count() > 1)
				{
					webhookToken = uri.Segments[uri.Segments.Count() - 1];
					WebHooksTokenItem crmWebHookToken = WebHooksToken.GetWebHooksTokenItem(TSAuthentication.GetLoginUser(), item.WebHookTokenId ?? 0);
					bool updateTokenInCrm = false;

					if (crmWebHookToken == null)
					{
						crmWebHookToken = (new WebHooksToken(TSAuthentication.GetLoginUser())).AddNewWebHooksTokenItem();
						crmWebHookToken.OrganizationId = TSAuthentication.OrganizationID;
						crmWebHookToken.Token = webhookToken;
						crmWebHookToken.IsEnabled = true;
						crmWebHookToken.CreatorId = TSAuthentication.UserID;
						crmWebHookToken.Collection.Save();
						updateTokenInCrm = true;
					}
					else if (crmWebHookToken.Token != webhookToken && crmWebHookToken.IsEnabled)
					{
						crmWebHookToken.Token = webhookToken;
						crmWebHookToken.DateModified = DateTime.UtcNow;
						crmWebHookToken.ModifierId = TSAuthentication.UserID;
						crmWebHookToken.Collection.Save();
						updateTokenInCrm = true;
					}

					if (updateTokenInCrm)
					{
						item.WebHookTokenId = crmWebHookToken.Id;
						item.Collection.Save();
					}
				}
			}

			if (item.CRMType.ToLower() == IntegrationType.TFS.ToString().ToLower() && item.Active == true)
            {
				AddIntegrationInTicketPageOrder(item.OrganizationID, IntegrationType.TFS.ToString());
            } else if (item.CRMType.ToLower() == IntegrationType.ServiceNow.ToString().ToLower() && item.Active == true)
			{
				AddIntegrationInTicketPageOrder(item.OrganizationID, IntegrationType.ServiceNow.ToString());
			}

			return item.GetProxy();
        }

		[WebMethod]
		public string GenerateWebHookToken(string type)
		{
			string token = HttpServerUtility.UrlTokenEncode(Guid.NewGuid().ToByteArray());
			string fullUrl = string.Format("{0}/WebHook/Integration/{1}/{2}/{3}", SystemSettings.GetAppUrl(), type, TSAuthentication.OrganizationID, token);
			return fullUrl;
		}

        private void AddIntegrationInTicketPageOrder(int organizationID, string type)
        {
            string customPageOrder = Settings.OrganizationDB.ReadString("TicketFieldsOrder", string.Empty);
            if (customPageOrder != string.Empty && customPageOrder.IndexOf(type) < 0)
            {
                Settings.OrganizationDB.WriteString("TicketFieldsOrder", customPageOrder.Replace("]", ",{\"CatID\":\"" + type + "\",\"CatName\":\"" + type + "\",\"Disabled\":\"false\"}]"));
            }

        }

        [WebMethod]
        public CRMLinkFieldProxy[] GetCrmLinkFields(int crmLinkID)
        {
            CRMLinkFields fields = new CRMLinkFields(TSAuthentication.GetLoginUser());
            fields.LoadByCrmLinkID(crmLinkID);
            CRMLinkFieldProxy[] crmLinkFieldMappings = fields.GetCRMLinkFieldProxies();
            CustomFields customFields = new CustomFields(TSAuthentication.GetLoginUser());
            customFields.LoadByOrganization(TSAuthentication.OrganizationID);
            List<TicketType> ticketTypeList = new List<TicketType>();
            TicketType ticketType;

            foreach (CRMLinkFieldProxy field in crmLinkFieldMappings)
            {
                int ticketTypeId = field.CustomFieldID != null ? customFields.Where(p => p.CustomFieldID == (int)field.CustomFieldID).Select(t => t.AuxID).SingleOrDefault() : 0;

                if (ticketTypeId > 0 && (ticketTypeList.Count == 0 || !ticketTypeList.Where(p => p.TicketTypeID == ticketTypeId).Any()))
                {
                    ticketType = TicketTypes.GetTicketType(TSAuthentication.GetLoginUser(), ticketTypeId);

                    if (ticketType != null)
                    {
                        ticketTypeList.Add(ticketType);
                    }
                }
                else
                {
                    ticketType = ticketTypeList.Where(p => p.TicketTypeID == ticketTypeId).SingleOrDefault();
                }


                field.TSFieldName = string.Format("{0}{1}", field.TSFieldName,
                                                            ticketType != null && !string.IsNullOrEmpty(ticketType.Name) ? " (" + ticketType.Name + ")" : "");
            }

            return crmLinkFieldMappings;
        }

        [WebMethod]
        public CRMLinkFieldProxy[] SaveCrmLinkField(int crmLinkID, int tsFieldID, bool isCustom, string crmName, ReferenceType refType)
        {
            CRMLinkTableItem link = CRMLinkTable.GetCRMLinkTableItem(TSAuthentication.GetLoginUser(), crmLinkID);
            if (link.OrganizationID != TSAuthentication.OrganizationID) return null;
            CRMLinkField field = (new CRMLinkFields(link.Collection.LoginUser)).AddNewCRMLinkField();

            if (isCustom)
            {
                CustomField custom = CustomFields.GetCustomField(link.Collection.LoginUser, tsFieldID);
                field.TSFieldName = custom.Name;
            }
            else if (refType != ReferenceType.TicketTypes)
            {
                ReportTableField rtf = ReportTableFields.GetReportTableField(link.Collection.LoginUser, tsFieldID);
                field.TSFieldName = rtf.FieldName;
            }
            else
            {
                TicketType ticketType = TicketTypes.GetTicketType(link.Collection.LoginUser, tsFieldID);
                field.TSFieldName = ticketType.Name;
            }

            switch (refType)
            {
                case ReferenceType.Organizations:
                    field.CRMObjectName = "Account";
                    break;
                case ReferenceType.Contacts:
                    field.CRMObjectName = "Contact";
                    break;
                case ReferenceType.Tickets:
                    field.CRMObjectName = "Ticket";
                    break;
                case ReferenceType.TicketTypes:
                    field.CRMObjectName = "TicketType";
                    break;
                default:
                    break;
            }

            field.CRMLinkID = crmLinkID;
            field.CRMFieldName = crmName;
            field.CustomFieldID = isCustom ? (int?)tsFieldID : null;
            field.Collection.Save();
            return GetCrmLinkFields(crmLinkID);
        }

        [WebMethod]
        public void DeleteCrmLinkField(int crmLinkFieldID)
        {
            CRMLinkField field = CRMLinkFields.GetCRMLinkField(TSAuthentication.GetLoginUser(), crmLinkFieldID);
            CRMLinkTableItem link = CRMLinkTable.GetCRMLinkTableItem(TSAuthentication.GetLoginUser(), field.CRMLinkID);
            if (link.OrganizationID != TSAuthentication.OrganizationID) return;
            field.Delete();
            field.Collection.Save();
        }

        // need to move to users service
        [WebMethod]
        public bool IsUserContact(int userID)
        {
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return true;
            return false;
        }

        [WebMethod]
        public OrganizationProductProxy GetOrganizationProduct(int organizationProductID)
        {
            OrganizationProduct op = OrganizationProducts.GetOrganizationProduct(TSAuthentication.GetLoginUser(), organizationProductID);
            return op.GetProxy();
        }

        [WebMethod]
        public AutocompleteItem[] AdminGetOrganizations(string name)
        {
            return AdminGetCustomers(1, name);
        }

        private void VerifyAccountAdmin()
        {
            bool result = (TSAuthentication.OrganizationID == 1078);
            Groups groups = new Groups(TSAuthentication.GetLoginUser());
            groups.LoadByGroupName(TSAuthentication.OrganizationID, "Account Admin");
            result = result && !groups.IsEmpty;
            if (!result) throw new Exception("User is not an account administrator.");
        }
        public void LogAdminUserAction(string description)
        {
            User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, TSAuthentication.UserID, String.Format("{0} performed admin action: {1}", user.FirstLastName, description));
            Emails.AddEmail(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID, "Admin Change", "TeamSupport Admin Change", description, false, "system@teamsupport.com", "kjones@teamsupport.com");
        }

        [WebMethod]
        public void AdminSetAllPortalUsers(int organizationID, bool sendEmails)
        {
            VerifyAccountAdmin();
            LogAdminUserAction(string.Format("set all Portal users set for {0}, send emails={1}", organizationID.ToString(), sendEmails.ToString()));
            Organizations.SetAllPortalUsers(TSAuthentication.GetLoginUser(), organizationID, sendEmails, false);
        }

        [WebMethod]
        public void AdminSetAllHubUsers(int organizationID, bool sendEmails)
        {
            VerifyAccountAdmin();
            LogAdminUserAction(string.Format("set all Hub users set for {0} send emails={1}", organizationID.ToString(), sendEmails.ToString()));
            Organizations.SetAllPortalUsers(TSAuthentication.GetLoginUser(), organizationID, sendEmails, true);
        }

        [WebMethod]
        public void AdminRebuildIndexes(int organizationID)
        {
            if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return;
            LogAdminUserAction(string.Format("rebuilt indexes for {0}", organizationID.ToString()));
            Organizations.SetRebuildIndexes(TSAuthentication.GetLoginUser(), organizationID);
        }



        [WebMethod]
        public void AdminQueryOrganizations(int parentID, string query)
        {
            List<TypeAheadItem> result = new List<TypeAheadItem>();
            if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return;

            int orgID = -1;
            bool flag = true;
            if (int.TryParse(query, out orgID))
            {
                Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
                if (organization != null)
                {
                    result.Add(new TypeAheadItem(organization.Name + " (" + organization.OrganizationID.ToString() + ")", organization.OrganizationID.ToString()));
                    flag = false;
                }

            }

            if (flag)
            {
                Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
                organizations.LoadByOrganizationName(query, parentID);

                if (organizations.Count > 0)
                {
                    foreach (Organization organization in organizations)
                    {
                        result.Add(new TypeAheadItem(organization.Name + " (" + organization.OrganizationID.ToString() + ")", organization.OrganizationID.ToString()));
                    }
                }
                else
                {
                    organizations.LoadByLikeOrganizationName(parentID, query, false, 20);
                    foreach (Organization organization in organizations)
                    {
                        result.Add(new TypeAheadItem(organization.Name + " (" + organization.OrganizationID.ToString() + ")", organization.OrganizationID.ToString()));
                    }
                }
            }

            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.Write(JsonConvert.SerializeObject(result));
            HttpContext.Current.Response.End();
        }

        [WebMethod]
        public AutocompleteItem[] AdminGetCustomers(int parentID, string name)
        {
            List<AutocompleteItem> result = new List<AutocompleteItem>();

            if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return result.ToArray();
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByLikeOrganizationName(parentID, name, false, 20);
            foreach (Organization organization in organizations)
            {
                result.Add(new AutocompleteItem(organization.Name + " (" + organization.OrganizationID.ToString() + ")", organization.OrganizationID.ToString()));
            }

            return result.ToArray();
        }


        [WebMethod]
        public int AdminGetUserCount(int orgID)
        {
            Organization org = GetAdminOrgTarget(orgID);
            return Organizations.GetUserCount(TSAuthentication.GetLoginUser(), org.OrganizationID);
        }

        [WebMethod]
        public int AdminGetStorageUsed(int orgID)
        {
            Organization org = GetAdminOrgTarget(orgID);
            return Organizations.GetStorageUsed(TSAuthentication.GetLoginUser(), org.OrganizationID);
        }

        [WebMethod]
        public void AdminRenameCompany(int orgID, string name)
        {
            VerifyAccountAdmin();
            LogAdminUserAction(string.Format("renamed {0} to {1}", orgID.ToString(), name));

            Organization org = GetAdminOrgTarget(orgID);
            org.Name = name;
            org.Collection.Save();
            TeamSupportSync.SyncOrg(org.OrganizationID, org.Name);
        }

        [WebMethod]
        public void AdminUpdateSeats(int orgID, int count)
        {
            VerifyAccountAdmin();
            LogAdminUserAction(string.Format("set seat count for {0} to {1}", orgID.ToString(), count.ToString()));
            Organization org = GetAdminOrgTarget(orgID);
            org.UserSeats = count;
            org.Collection.Save();
        }

        [WebMethod]
        public void AdminSetEmailTrialMax(int orgID, int max)
        {
            VerifyAccountAdmin();
            LogAdminUserAction(string.Format("set trial email max for {0} to {1}", orgID.ToString(), max.ToString()));
            Emails.SetMaxEmailsForTrial(TSAuthentication.GetLoginUser(), orgID, max);
        }

        [WebMethod]
        public bool AdminToggleEmailTrialLimit(int orgID)
        {
            VerifyAccountAdmin();
            bool value = Emails.GetIsTrialLimited(TSAuthentication.GetLoginUser(), orgID);
            LogAdminUserAction(string.Format("set trial email limit for {0} to {1}", orgID.ToString(), value.ToString()));
            Emails.SetTrialLimited(TSAuthentication.GetLoginUser(), orgID, !value);
            return !value;
        }

        [WebMethod]
        public string AdminGetTrialEmailData(int orgID)
        {
            dynamic result = new ExpandoObject();
            result.max = Emails.GetMaxEmailsForTrial(TSAuthentication.GetLoginUser(), orgID);
            result.isLimited = Emails.GetIsTrialLimited(TSAuthentication.GetLoginUser(), orgID);
            return JsonConvert.SerializeObject(result);
        }

        [WebMethod]
        public void AdminSetNextTicketNumber(int orgID, int number)
        {
            VerifyAccountAdmin();
            LogAdminUserAction(string.Format("set next ticket number for {0} to {1}", orgID.ToString(), number.ToString()));

            Organization org = GetAdminOrgTarget(orgID);
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Ticket ticket = (new Tickets(loginUser)).AddNewTicket();
            ticket.OrganizationID = org.OrganizationID;
            ticket.GroupID = org.DefaultPortalGroupID;
            ticket.IsKnowledgeBase = false;
            ticket.IsVisibleOnPortal = true;
            ticket.Name = "Place holder for next ticket " + number.ToString();
            ticket.TicketSeverityID = TicketSeverities.GetTop(loginUser, org.OrganizationID).TicketSeverityID;
            ticket.TicketTypeID = TicketTypes.GetTop(loginUser, org.OrganizationID).TicketTypeID;
            ticket.TicketStatusID = TicketStatuses.GetTop(loginUser, ticket.TicketTypeID).TicketStatusID;
            ticket.TicketSource = "Staff";
            ticket.PortalEmail = "";
            ticket.TicketNumber = number - 1;
            ticket.Collection.Save();


            TeamSupport.Data.Action action = (new Actions(loginUser)).AddNewAction();
            action.ActionTypeID = null;
            action.SystemActionTypeID = SystemActionType.Description;
            action.Description = "You can delete this once your next ticket has been created.";
            action.IsKnowledgeBase = false;
            action.IsVisibleOnPortal = true;
            action.ActionSource = "Staff";
            action.Name = "Description";
            action.TicketID = ticket.TicketID;
            action.Collection.Save();
        }


        [WebMethod]
        public bool AdminGetEncryptData(int orgID)
        {
            Organization org = GetAdminOrgTarget(orgID);
            string value = OrganizationSettings.ReadString(TSAuthentication.GetLoginUser(), orgID, "UseEncryptedData", "False");
            return bool.Parse(value);
        }


        [WebMethod]
        public bool AdminToggleEncryptData(int orgID)
        {
            VerifyAccountAdmin();
            bool value = !bool.Parse(OrganizationSettings.ReadString(TSAuthentication.GetLoginUser(), orgID, "UseEncryptedData", "False"));
            LogAdminUserAction(string.Format("set encrypt data for {0} to {1}", orgID.ToString(), value.ToString()));
            OrganizationSettings.WriteString(TSAuthentication.GetLoginUser(), orgID, "UseEncryptedData", value.ToString());
            return value;
        }

        [WebMethod]
        public void AdminUpdateProductType(int orgID, int productType)
        {
            VerifyAccountAdmin();
            LogAdminUserAction(string.Format("set product type for {0} to {1}", orgID.ToString(), ((ProductType)productType).ToString()));

            Organization org = GetAdminOrgTarget(orgID);
            org.ProductType = (ProductType)productType;
            org.IsInventoryEnabled = org.ProductType == ProductType.Enterprise;
            org.Collection.Save();

            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Organizations orgHelper = new Organizations(loginUser);
            orgHelper.MigrateOrgType(loginUser, orgID, org.ProductType);

        }

        [WebMethod]
        public void AdminSetInventory(int orgID, bool value)
        {
            Organization org = GetAdminOrgTarget(orgID);
            LogAdminUserAction(string.Format("sent inventory for {0} to {1}", orgID.ToString(), value.ToString()));

            org.IsInventoryEnabled = value;
            org.Collection.Save();
        }

        [WebMethod]
        public void AdminSetApiCount(int orgID, int count)
        {
            VerifyAccountAdmin();
            LogAdminUserAction(string.Format("set api count for {0} to {1}", orgID.ToString(), count.ToString()));

            Organization org = GetAdminOrgTarget(orgID);
            org.APIRequestLimit = count;
            org.Collection.Save();
        }

        [WebMethod]
        public void AdminEnable(int orgID, bool value)
        {
            VerifyAccountAdmin();
            LogAdminUserAction(string.Format("set enabled {0} to {1}", orgID.ToString(), value.ToString()));

            Organization org = GetAdminOrgTarget(orgID);
            org.IsActive = value;
            org.IsAdvancedPortal = value;
            org.IsBasicPortal = value;
            org.IsApiActive = value;
            org.IsApiEnabled = value;
            org.IsInventoryEnabled = value;
            org.Collection.Save();

            if (!value)
            {
                Users users = new Users(org.Collection.LoginUser);
                users.LoadByOrganizationID(org.OrganizationID, false);
                foreach (User user in users)
                {
                    user.IsActive = false;
                }
                users.Save();
            }
        }

		[WebMethod]
		public void AdminSetWatson(int orgID, bool value)
		{
			Organization org = GetAdminOrgTarget(orgID);
            LogAdminUserAction(string.Format("set Watson for {0} to {1}", orgID.ToString(), value.ToString()));

            org.UseWatson = value;
			org.Collection.Save();
		}

        [WebMethod]
		public void AdminSetRequireTwoFactor(int orgID, bool value)
		{
			Organization org = GetAdminOrgTarget(orgID);
            LogAdminUserAction(string.Format("set RequireTwoFactor for {0} to {1}", orgID.ToString(), value.ToString()));

            org.RequireTwoFactor = value;
			org.Collection.Save();
		}

		public Organization GetAdminOrgTarget(int orgID)
        {
            if (TSAuthentication.OrganizationID != 1078 && TSAuthentication.OrganizationID != 1088) return null;
            return Organizations.GetOrganization(TSAuthentication.GetLoginUser(), orgID);
        }

        [WebMethod]
        public string CleanUpOrphanTicketStatuses()
        {
            if (TSAuthentication.UserID != 34) return "No Access";
            int orgID = 305383;
            Tickets tickets = new Tickets(TSAuthentication.GetLoginUser());
            tickets.LoadByOrganizationID(orgID);

            TicketStatuses statuses = new TicketStatuses(tickets.LoginUser);
            statuses.LoadByOrganizationID(orgID);
            int count = 0;
            int total = 0;
            foreach (Ticket ticket in tickets)
            {
                TicketStatus status = statuses.FindByTicketStatusID(ticket.TicketStatusID);
                if (status != null && status.TicketTypeID != ticket.TicketTypeID)
                {
                    total++;
                    TicketStatus newStatus = statuses.FindByName(status.Name, ticket.TicketTypeID);
                    if (newStatus == null)
                    {
                        foreach (TicketStatus item in statuses)
                        {
                            if (item.TicketTypeID == ticket.TicketTypeID && item.IsClosed == status.IsClosed)
                            {
                                newStatus = item;
                                break;
                            }
                        }
                    }
                    if (newStatus != null)
                    {
                        count++;
                        ticket.TicketStatusID = newStatus.TicketStatusID;
                    }
                }
            }
            tickets.Save();
            return "Clean up was successful! (" + count.ToString() + " of " + total.ToString() + ")";
        }

        [WebMethod]
        public string CopyOrganizationSettings(int sourceID, int destID)
        {
            string result = "No Access";
            if (TSAuthentication.UserID != 34) return result;

            /*      Organization source = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), sourceID);
                  Organization dest = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), sourceID);

                  dest.ChangeStatusIfClosed = source.ChangeStatusIfClosed;
                  dest.AddAdditionalContacts = source.AddAdditionalContacts;
                  //dest.ModifierID = source.ModifierID;
                  //dest.CreatorID = source.CreatorID;
                  dest.MatchEmailSubject = source.MatchEmailSubject;
                  dest.TimedActionsRequired = source.TimedActionsRequired;
                  dest.CultureName = source.CultureName;
                  dest.UseEuropeDate = source.UseEuropeDate;
                  dest.BusinessDays = source.BusinessDays;
                  dest.InternalSlaLevelID = source.InternalSlaLevelID;
                  dest.SlaLevelID = source.SlaLevelID;
                  dest.DefaultWikiArticleID = source.DefaultWikiArticleID;
                  dest.ShowWiki = source.ShowWiki;
                  dest.AdminOnlyReports = source.AdminOnlyReports;
                  dest.AdminOnlyCustomers = source.AdminOnlyCustomers;
                  dest.CompanyDomains = source.CompanyDomains;
                  dest.OrganizationReplyToAddress = source.OrganizationReplyToAddress;
                  dest.EmailDelimiter = source.EmailDelimiter;
                  dest.RequireKnownUserForNewEmail = source.RequireKnownUserForNewEmail;
                  dest.RequireNewKeyword = source.RequireNewKeyword;
                  dest.APIRequestLimit = source.APIRequestLimit;
                  dest.CRMLinkID = source.CRMLinkID;
                  dest.PortalGuid = source.PortalGuid;
                  dest.ChatID = source.ChatID;
                  dest.SystemEmailID = source.SystemEmailID;
                  dest.WebServiceID = source.WebServiceID;
                  dest.ParentID = source.ParentID;
                  dest.ProductType = source.ProductType;
                  dest.DefaultSupportUserID = source.DefaultSupportUserID;
                  dest.DefaultSupportGroupID = source.DefaultSupportGroupID;
                  dest.DefaultPortalGroupID = source.DefaultPortalGroupID;
                  dest.PrimaryUserID = source.PrimaryUserID;
                  dest.IsBasicPortal = source.IsBasicPortal;
                  dest.IsAdvancedPortal = source.IsAdvancedPortal;
                  dest.HasPortalAccess = source.HasPortalAccess;
                  dest.InActiveReason = source.InActiveReason;
                  dest.TimeZoneID = source.TimeZoneID;
                  dest.IsInventoryEnabled = source.IsInventoryEnabled;
                  dest.IsApiEnabled = source.IsApiEnabled;
                  dest.IsApiActive = source.IsApiActive;
                  dest.IsActive = source.IsActive;
                  dest.ImportID = source.ImportID;
                  dest.ExtraStorageUnits = source.ExtraStorageUnits;
                  dest.ChatSeats = source.ChatSeats;
                  dest.PortalSeats = source.PortalSeats;
                  dest.UserSeats = source.UserSeats;
                  dest.IsCustomerFree = source.IsCustomerFree;
                  dest.PromoCode = source.PromoCode;
                  dest.EvalProcess = source.EvalProcess;
                  dest.PotentialSeats = source.PotentialSeats;
                  dest.PrimaryInterest = source.PrimaryInterest;
                  dest.WhereHeard = source.WhereHeard;
                  dest.Website = source.Website;
                  dest.Description = source.Description;
                  dest.Name = source.Name;
                  dest.OrganizationID = source.OrganizationID;


                  CustomFields fields = new CustomFields(TSAuthentication.GetLoginUser());
                  fields.LoadByOrganization(dest.OrganizationID);
                  fields.DeleteAll();
                  fields.Save();

                  */
            return result;
        }

        [WebMethod]
        public ApiInfo GenerateNewApiToken()
        {
            if (!TSAuthentication.IsSystemAdmin) return null;
            Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID);
            organization.WebServiceID = Guid.NewGuid();
            organization.Collection.Save();
            return GetApiInfo();
        }

        [WebMethod]
        public ApiInfo SetApiEnabled(bool value)
        {
            if (!TSAuthentication.IsSystemAdmin) return null;
            Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID);
            organization.IsApiEnabled = value;
            organization.Collection.Save();
            return GetApiInfo();
        }

        [WebMethod]
        public ApiInfo GetApiInfo()
        {
            if (!TSAuthentication.IsSystemAdmin) return null;
            Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID);
            ApiInfo info = new ApiInfo();
            info.RequestCount = ApiLogs.GetDailyRequestCount(organization.Collection.LoginUser, TSAuthentication.OrganizationID);
            info.RequestMax = organization.APIRequestLimit;
            info.Token = organization.WebServiceID.ToString();
            info.IsEnabled = organization.IsApiEnabled;
            info.IsActive = organization.IsApiActive;
            return info;
        }

        [WebMethod]
        public AutocompleteItem[] GetUserOrOrganizationForTicket(string searchTerm)
        {
            User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
            return GetUserOrOrganizationFiltered(searchTerm, !user.AllowAnyTicketCustomer);
        }

        [WebMethod]
        public AutocompleteItem[] GetOrganizationForTicket(string searchTerm)
        {
            User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
            return GetOrganizationFiltered(searchTerm, !user.AllowAnyTicketCustomer);
        }

        [WebMethod]
        public AutocompleteItem[] GetUserOrOrganization(string searchTerm)
        {
            return GetUserOrOrganizationFiltered(searchTerm, true);
        }

        [WebMethod]
        public AutocompleteItem[] GetContacts(string searchTerm)
        {
            UsersView users = new UsersView(TSAuthentication.GetLoginUser());
            users.LoadByLikeName(TSAuthentication.OrganizationID, searchTerm, 50, false, true);

            List<AutocompleteItem> list = new List<AutocompleteItem>();
            foreach (UsersViewItem user in users)
            {
                list.Add(new AutocompleteItem(String.Format("{0}, {1} [{2}]", user.LastName, user.FirstName, user.Organization), user.UserID.ToString(), "u"));
            }

            return list.ToArray();
        }

        [WebMethod]
        public AutocompleteItem[] GetContactsExceptGiven(string searchTerm, int givenUserID)
        {
            UsersView users = new UsersView(TSAuthentication.GetLoginUser());
            users.LoadByLikeNameExceptGiven(TSAuthentication.OrganizationID, searchTerm, 50, false, givenUserID, true);

            List<AutocompleteItem> list = new List<AutocompleteItem>();
            foreach (UsersViewItem user in users)
            {
                list.Add(new AutocompleteItem(String.Format("{0}, {1} [{2}]", user.LastName, user.FirstName, user.Organization), user.UserID.ToString(), "u"));
            }

            return list.ToArray();
        }

        [WebMethod]
        public AutocompleteItem[] GetCompanies(string searchTerm)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByLikeOrganizationName(TSAuthentication.OrganizationID, searchTerm, true, 50, false);

            List<AutocompleteItem> list = new List<AutocompleteItem>();
            foreach (Organization organization in organizations)
            {
                list.Add(new AutocompleteItem(organization.Name, organization.OrganizationID.ToString(), "o"));
            }

            return list.ToArray();
        }

        public AutocompleteItem[] GetUserOrOrganizationFiltered(string searchTerm, bool filterByUserRights)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByLikeOrganizationName(TSAuthentication.OrganizationID, searchTerm, true, 50, filterByUserRights);

            UsersView users = new UsersView(organizations.LoginUser);
            users.LoadByLikeName(TSAuthentication.OrganizationID, searchTerm, 50, true, true);

            List<AutocompleteItem> list = new List<AutocompleteItem>();
            foreach (Organization organization in organizations)
            {
                list.Add(new AutocompleteItem(organization.Name, organization.OrganizationID.ToString(), "o"));
            }

            foreach (UsersViewItem user in users)
            {
                list.Add(new AutocompleteItem(String.Format("{0}, {1} [{2}]", user.LastName, user.FirstName, user.Organization), user.UserID.ToString(), "u"));
            }

            return list.ToArray();
        }

        public AutocompleteItem[] GetOrganizationFiltered(string searchTerm, bool filterByUserRights)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByLikeOrganizationName(TSAuthentication.OrganizationID, searchTerm, true, 50, filterByUserRights);

            List<AutocompleteItem> list = new List<AutocompleteItem>();
            foreach (Organization organization in organizations)
            {
                list.Add(new AutocompleteItem(organization.Name, organization.OrganizationID.ToString(), "o"));
            }

            return list.ToArray();
        }

        [WebMethod]
        public AutocompleteItem[] GetWarehouseAssets(string searchTerm)
        {
            User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
            return GetWarehouseAssetsFiltered(searchTerm);
        }

        public AutocompleteItem[] GetWarehouseAssetsFiltered(string searchTerm)
        {
            AssetsView assets = new AssetsView(TSAuthentication.GetLoginUser());
            assets.LoadByLikeAssetDisplayName(TSAuthentication.OrganizationID, searchTerm, 50);

            List<AutocompleteItem> list = new List<AutocompleteItem>();
            foreach (AssetsViewItem asset in assets)
            {
                list.Add(new AutocompleteItem(asset.DisplayName, asset.AssetID.ToString()));
            }

            return list.ToArray();
        }

        [WebMethod]
        public AutocompleteItem[] SearchOrganization(string searchTerm)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByLikeOrganizationName(TSAuthentication.OrganizationID, searchTerm, true);

            List<AutocompleteItem> list = new List<AutocompleteItem>();
            foreach (Organization organization in organizations)
            {
                list.Add(new AutocompleteItem(organization.Name, organization.OrganizationID.ToString()));
            }

            return list.ToArray();
        }

        [WebMethod]
        public AutocompleteItem[] WCSearchOrganization(string searchTerm)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByLikeOrganizationName(TSAuthentication.OrganizationID, searchTerm, true);

            List<AutocompleteItem> list = new List<AutocompleteItem>();
            foreach (Organization organization in organizations)
            {
                list.Add(new AutocompleteItem(organization.Name, organization.OrganizationID.ToString(), organization.OrganizationID.ToString()));
            }

            return list.ToArray();
        }

        [WebMethod]
        public List<string> OrgSearch(string searchTerm)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByLikeOrganizationName(TSAuthentication.OrganizationID, searchTerm, true);

            List<string> list = new List<string>();
            foreach (Organization organization in organizations)
            {
                list.Add(organization.Name);
            }

            return list;
        }

        [WebMethod]
        public GroupProxy GetGroupInfo(string groupID)
        {
            Groups groups = new Groups(TSAuthentication.GetLoginUser());
            if (groupID == null)
            {
                groups.LoadByOrganizationIDForGrid(TSAuthentication.OrganizationID, TSAuthentication.GetLoginUser().UserID);
                return groups[0].GetProxy();
            }
            else
            {
                groups.LoadByGroupID(Convert.ToInt16(groupID));
                return groups[0].GetProxy();
            }

        }

        [WebMethod]
        public GroupsList GetGroups()
        {
            GroupsList result = new GroupsList();
            StringBuilder html = new StringBuilder();
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Groups groups = new Groups(loginUser);
            groups.LoadByOrganizationIDForGrid(TSAuthentication.OrganizationID, loginUser.UserID);

            Organization organization = Organizations.GetOrganization(loginUser, loginUser.OrganizationID);
            if (organization.UseProductFamilies)
            {
                ProductFamilies productFamilies = new ProductFamilies(loginUser);
                productFamilies.LoadByOrganizationID(loginUser.OrganizationID);
                if (productFamilies.Count > 0)
                {
                    result.HasProductFamilies = true;

                    html.Append(@"<li>
                                <div class='row'>
                                <div class='col-xs-12'>
                                    <strong>Without Product Line:</strong>
                                </div>
                                </div>
                                <ul class='customList-group group-container'>");

                    bool hasGroupsWithoutProductFamilies = false;
                    foreach (GroupProxy group in groups.GetGroupProxies())
                    {
                        if (group.ProductFamilyID == null)
                        {
                            hasGroupsWithoutProductFamilies = true;

                            html.AppendFormat(@"<li>
                                                <div class='row'>
                                                <div class='col-xs-12'>
                                                    <strong><a class='group' gid='{0}'>{1} ({3})</a></strong>
                                                    <div>{2}</div>
                                                </div>
                                                </div>
                                            </li>
                                            ", group.GroupID, group.Name, group.Description, group.TicketCount);
                        }
                    }

                    html.Append(@"</ul></li>");

                    if (!hasGroupsWithoutProductFamilies)
                    {
                        html.Clear();
                    }

                    foreach (ProductFamily productFamily in productFamilies)
                    {
                        html.AppendFormat(@"<li>
                            <div class='row'>
                            <div class='col-xs-12'>
                                <strong>{0}:</strong>
                            </div>
                            </div>
                            <ul class='customList-group group-container'>", productFamily.Name);

                        foreach (GroupProxy group in groups.GetGroupProxies())
                        {
                            if (group.ProductFamilyID == productFamily.ProductFamilyID)
                            {
                                hasGroupsWithoutProductFamilies = true;

                                html.AppendFormat(@"<li>
                                            <div class='row'>
                                            <div class='col-xs-12'>
                                                <strong><a class='group' gid='{0}'>{1} ({3})</a></strong>
                                                <div>{2}</div>
                                            </div>
                                            </div>
                                        </li>
                                        ", group.GroupID, group.Name, group.Description, group.TicketCount);
                            }
                        }

                        html.Append(@"</ul></li>");
                    }
                }
            }

            if (!result.HasProductFamilies)
            {
                foreach (GroupProxy group in groups.GetGroupProxies())
                {
                    html.AppendFormat(@"<li>
                                        <div class='row'>
                                        <div class='col-xs-12'>
                                            <strong><a class='group' gid='{0}'>{1} ({3})</a></strong>
                                            <div>{2}</div>
                                        </div>
                                        </div>
                                        </li>
                                        ", group.GroupID, group.Name, group.Description, group.TicketCount);
                }
            }

            result.Html = html.ToString();
            return result;
        }


        [WebMethod]
        public string ReadServiceSettings()
        {
            if (TSAuthentication.UserID != 34) return "";
            string text = "UPDATE Tickets SET NeedsIndexing = 1";
            SqlCommand command = new SqlCommand(text);
            //SqlExecutor.ExecuteNonQuery(TSAuthentication.GetLoginUser(), command);

            text = "SELECT count(*) FROM tickets where needsindexing > 0 ";
            command = new SqlCommand(text);
            return DataTableToHTMLTable(SqlExecutor.ExecuteQuery(TSAuthentication.GetLoginUser(), command));
        }



        public string DataTableToHTMLTable(DataTable inTable)
        {
            StringBuilder dString = new StringBuilder();
            dString.Append("<table>");
            dString.Append(GetHeader(inTable));
            dString.Append(GetBody(inTable));
            dString.Append("</table>");
            return dString.ToString();
        }

        private string GetHeader(DataTable dTable)
        {
            StringBuilder dString = new StringBuilder();

            dString.Append("<thead><tr>");
            foreach (DataColumn dColumn in dTable.Columns)
            {
                dString.AppendFormat("<th>{0}</th>", dColumn.ColumnName);
            }
            dString.Append("</tr></thead>");

            return dString.ToString();
        }

        private string GetBody(DataTable dTable)
        {
            StringBuilder dString = new StringBuilder();

            dString.Append("<tbody>");

            foreach (DataRow dRow in dTable.Rows)
            {
                dString.Append("<tr>");
                for (int dCount = 0; dCount <= dTable.Columns.Count - 1; dCount++)
                {
                    dString.AppendFormat("<td>{0}</td>", dRow[dCount]);
                }
                dString.Append("</tr>");
            }
            dString.Append("</tbody>");

            return dString.ToString();
        }

        [WebMethod]
        public string GetShortNameFromID(int orgID)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByOrganizationID(orgID);

            if (organizations.IsEmpty) return "N/A";

            if (organizations[0].Name.Length > 10)
                return organizations[0].Name.Substring(0, 10).ToString() + "...";
            else
                return organizations[0].Name.ToString();
        }

        [WebMethod]
        public string GetEvergageData(string orgName)
        {
            using (WebClient client = new WebClient())
            {
                //https://teamsupport.evergage.com/api/dataset/MainApp/account/muroc%20systems%2C%20inc.?_at=AD84A537-E376-65FC-3351-767A6DC6EB12
                return client.DownloadString(string.Format("https://teamsupport.evergage.com/api/dataset/MainApp/account/{0}?_at=AD84A537-E376-65FC-3351-767A6DC6EB12", Uri.EscapeDataString(orgName.ToLower())));
            }

        }

        [WebMethod]
        public void ResetCDI()
        {
            //CDI_Setting cdi = (new CDI_Settings(TSAuthentication.GetLoginUser()).AddNewCDI_Setting());
            CDI_Settings cdi = new CDI_Settings(TSAuthentication.GetLoginUser());
            cdi.LoadByOrganizationID(TSAuthentication.OrganizationID);

            if (cdi.Count > 0)
            {
                cdi[0].NeedCompute = true;
                cdi.Save();
            }
            else
            {
                CDI_Setting newCDI = (new CDI_Settings(TSAuthentication.GetLoginUser()).AddNewCDI_Setting());
                newCDI.OrganizationID = TSAuthentication.GetLoginUser().OrganizationID;
                newCDI.NeedCompute = true;
                newCDI.Collection.Save();
            }
        }

        [WebMethod]
        public CDI_SettingProxy LoadCDISettings(int organizationID)
        {
            
            CDI_Settings cdi = new CDI_Settings(TSAuthentication.GetLoginUser());
            cdi.LoadByOrganizationID(organizationID);
            if (cdi.Count > 0)
                return cdi[0].GetProxy();
            else
                return null;
        }

        [WebMethod]
        public CustomPortalColumnProxy[] LoadCustomPortalColumns(int organizationID)
        {
            CustomPortalColumns cpc = new CustomPortalColumns(TSAuthentication.GetLoginUser());
            cpc.LoadByOrganizationID(organizationID);
            bool didDelete = false;
            foreach (CustomPortalColumn c in cpc.ToList())
            {
                if (c.CustomFieldID != null)
                {
                    CustomFields cf = new CustomFields(TSAuthentication.GetLoginUser());
                    cf.LoadByCustomFieldID((int)c.CustomFieldID);
                    if (cf.Count == 0)
                    {
                        didDelete = true;
                        RemoveCustomPortalColumn("c" + c.CustomFieldID.ToString());
                    }
                }
            }

            if (didDelete)
            {
                CustomPortalColumns cpcFiltered = new CustomPortalColumns(TSAuthentication.GetLoginUser());
                cpcFiltered.LoadByOrganizationID(organizationID);
                return cpcFiltered.GetCustomPortalColumnProxies();
            }
            else
            {
                return cpc.GetCustomPortalColumnProxies();
            }



        }

        [WebMethod]
        public void RemoveCustomPortalColumn(string fieldID)
        {
            CustomPortalColumns cpc;
            int id = int.Parse(fieldID.Substring(1));

            if (fieldID.StartsWith("s"))
            {
                //loadstock
                cpc = new CustomPortalColumns(TSAuthentication.GetLoginUser());
                cpc.LoadByStockFieldID(id, TSAuthentication.GetLoginUser().OrganizationID);
            }
            else
            {
                //load custom
                cpc = new CustomPortalColumns(TSAuthentication.GetLoginUser());
                cpc.LoadByCustomFieldID(id, TSAuthentication.GetLoginUser().OrganizationID);
            }



            cpc[0].Delete();
            cpc[0].Collection.Save();
        }

        [WebMethod]
        public void AddCustomPortalColumn(string fieldID, int position)
        {
            CustomPortalColumn cpc;
            int id = int.Parse(fieldID.Substring(1));

            cpc = (new CustomPortalColumns(TSAuthentication.GetLoginUser()).AddNewCustomPortalColumn());
            cpc.OrganizationID = TSAuthentication.GetLoginUser().OrganizationID;
            cpc.Position = position - 1;

            if (fieldID.StartsWith("s"))
                //save stock
                cpc.StockFieldID = id;
            else
                //save custom
                cpc.CustomFieldID = id;

            cpc.Collection.Save();
        }

        [WebMethod]
        public void SavePortalColOrder(string columns)
        {
            List<string> orders = JsonConvert.DeserializeObject<List<string>>(columns);
            CustomPortalColumns cols = new CustomPortalColumns(TSAuthentication.GetLoginUser());
            cols.LoadByOrganizationID(TSAuthentication.GetLoginUser().OrganizationID);

            int pos = 0;
            foreach (string s in orders)
            {
                CustomPortalColumn c;
                int id = int.Parse(s.Substring(1));
                if (s.StartsWith("s"))
                    c = cols.FindByStockFieldID(id);
                else
                    c = cols.FindByCustomFieldID(id);

                c.Position = pos;
                pos++;
            }
            cols.Save();
        }

        [WebMethod]
        public ProductFamilyProxy[] LoadOrgProductFamilies(int organizationID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            ProductFamilies productFamilies = new ProductFamilies(loginUser);
            User user = loginUser.GetUser();
            if (user.ProductFamiliesRights == (int)ProductFamiliesRightType.AllFamilies)
            {
                productFamilies.LoadByOrganizationID(loginUser.OrganizationID);
            }
            else
            {
                productFamilies.LoadByUserRights(loginUser.UserID);
            }
            return productFamilies.GetProductFamilyProxies();
        }

        [WebMethod]
        public ImportProxy[] LoadImports(int start)
        {
            Imports imports = new Imports(TSAuthentication.GetLoginUser());
            imports.LoadByLimit(start);

            return imports.GetImportProxies();
        }

        [WebMethod]
        public SimpleImportFieldsViewItemProxy[] LoadImportFields(int refType)
        {
            SimpleImportFieldsView importFields = new SimpleImportFieldsView(TSAuthentication.GetLoginUser());
            importFields.LoadByRefType(refType);

            return importFields.GetSimpleImportFieldsViewItemProxies();
        }

        [WebMethod]
        public void SaveImportFieldMaps(int importID, string fieldsData)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            ImportMaps importMaps = new ImportMaps(loginUser);
            List<ImportFieldMap> fields = JsonConvert.DeserializeObject<List<ImportFieldMap>>(fieldsData);
            foreach (ImportFieldMap field in fields)
            {
                ImportMap importMap = importMaps.AddNewImportMap();
                importMap.ImportID = importID;
                importMap.FieldID = field.ImportFieldID;
                importMap.SourceName = field.SourceName;
            }
            importMaps.Save();
        }

        [WebMethod]
        public void SaveImport(string fileName, int refType, string fieldsData)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            Imports imports = new Imports(loginUser);
            Import import = imports.AddNewImport();
            import.RefType = (ReferenceType)refType;
            import.FileName = fileName;
            import.OrganizationID = TSAuthentication.OrganizationID;
            import.FilePathID = 3;
            import.Collection.Save();

            List<ImportFieldMap> fields = JsonConvert.DeserializeObject<List<ImportFieldMap>>(fieldsData);
            if (fields.Count > 0)
            {
                SimpleImportFieldsView importFields = new SimpleImportFieldsView(loginUser);
                importFields.LoadByRefType(refType);

                ImportMaps importMaps = new ImportMaps(loginUser);
                foreach (ImportFieldMap field in fields)
                {
                    ImportMap importMap = importMaps.AddNewImportMap();
                    importMap.ImportID = import.ImportID;
                    importMap.SourceName = field.SourceName;
                    foreach (SimpleImportFieldsViewItem importField in importFields)
                    {
                        if (importField.ImportFieldID == field.ImportFieldID)
                        {
                            importMap.FieldID = importField.ImportFieldID;
                            importMap.IsCustom = Convert.ToBoolean(importField.IsCustom);
                            break;
                        }
                    }
                }
                importMaps.Save();
            }
        }

        [WebMethod]
        public string GetImportPanels(string uploadedFileName, int refType)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            ImportFieldsView importFieldsView = new ImportFieldsView(loginUser);
            importFieldsView.LoadByRefType(refType);

            ImportPanels result = new ImportPanels();
            ImportFields importFields = new ImportFields(loginUser);

            SimpleImportFieldsView simpleImportFieldsView = new SimpleImportFieldsView(loginUser);
            simpleImportFieldsView.LoadByRefType(refType);
            result.ImportFields = simpleImportFieldsView.GetSimpleImportFieldsViewItemProxies();
            uploadedFileName = Path.GetFileName(uploadedFileName);

            string csvFile = TeamSupport.Data.Quarantine.WebAppQ.GetCsvFile(uploadedFileName, loginUser);

            try
            {
                using (CsvReader csv = new CsvReader(new StreamReader(csvFile), true))
                {
                    string[] headers = csv.GetFieldHeaders();
                    result.ImportFieldMap = new ImportFieldMap[headers.Length];

                    while (csv.ReadNextRecord())
                    {
                        for (int i = 0; i < headers.Length; i++)
                        {
                            result.ImportFieldMap[i] = new ImportFieldMap();
                            result.ImportFieldMap[i].SourceName = headers[i];
                            result.ImportFieldMap[i].ExampleValue = csv[i];
                            if (!string.IsNullOrEmpty(headers[i].Trim()))
                            {
                                foreach (ImportFieldsViewItem field in importFieldsView)
                                {
                                    if (field.SourceName != null && field.SourceName.Trim().ToLower() == headers[i].Trim().ToLower())
                                    {
                                        result.ImportFieldMap[i].ImportFieldID = field.ImportFieldID;
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
            catch (IOException ioException)
            {
                ExceptionLogs.LogException(loginUser, ioException, "OrganizationService.GetImportPanels", string.Format("OrgId: {0}", loginUser.OrganizationID));
            }

            return JsonConvert.SerializeObject(result);
        }

        [WebMethod]
        public EMailAlternateInboundItemProxy[] LoadEMailAlternateByOrgID(int OrganizationID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            EMailAlternateInbound items = new EMailAlternateInbound(loginUser);
            items.LoadByOrganizationID(OrganizationID);
            return items.GetEMailAlternateInboundItemProxies();
        }

        [WebMethod]
        public List<ActivityTypesDropDown> LoadNoteActivities()
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            var results = new List<ActivityTypesDropDown>();

            ActivityTypes activities = new ActivityTypes(loginUser);
            activities.LoadByOrganizationID(loginUser.OrganizationID);

            //Get default activity types
            foreach (ActivityTypeEnum activity in Enum.GetValues(typeof(ActivityTypeEnum)))
            {
                var value = Enum.Parse(typeof(ActivityTypeEnum), activity.ToString());
                results.Add(new ActivityTypesDropDown() { Name = activity.ToString(), Value = (int)value });
            }

            //Get custom activity types
            var customActivityTypes = activities.GetActivityTypeProxies().ToList();
            foreach (ActivityTypeProxy activityproxy in customActivityTypes)
            {
                results.Add(new ActivityTypesDropDown() { Name = activityproxy.Name, Value = activityproxy.ActivityTypeID });
            }

            return results;
        }
}
    public class ActivityTypesDropDown
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

[DataContract]
    public class ApiInfo
    {
        [DataMember]
        public int RequestCount { get; set; }
        [DataMember]
        public int RequestMax { get; set; }
        [DataMember]
        public string Token { get; set; }
        [DataMember]
        public bool IsEnabled { get; set; }
        [DataMember]
        public bool IsActive { get; set; }

    }

    [DataContract]
    public class ImportFieldMap
    {
        [DataMember]
        public int ImportFieldID { get; set; }
        [DataMember]
        public string SourceName { get; set; }
        [DataMember]
        public string ExampleValue { get; set; }
        [DataMember]
        public string ImportFieldName { get; set; }

    }

    [DataContract]
    public class ImportPanels
    {
        [DataMember]
        public SimpleImportFieldsViewItemProxy[] ImportFields { get; set; }
        [DataMember]
        public ImportFieldMap[] ImportFieldMap { get; set; }
    }

    [DataContract]
    public class GroupsList
    {
        [DataMember]
        public bool HasProductFamilies { get; set; }
        [DataMember]
        public string Html { get; set; }
    }

}
