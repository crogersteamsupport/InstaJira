using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Security;
using System.Net.Mail;

namespace TeamSupport.Data
{
    public partial class Organization
    {
        public OrganizationsViewItem GetOrganizationView()
        {
            return OrganizationsView.GetOrganizationsViewItem(BaseCollection.LoginUser, OrganizationID);
        }

        public string GetReplyToAddress(string replyAddress = null)
        {
            if (!string.IsNullOrWhiteSpace(replyAddress) && ReplyToAlternateEmailAddresses == true)
            {
                return replyAddress;
            }

            if (string.IsNullOrEmpty(OrganizationReplyToAddress))
            {
                return SystemEmailID.ToString() + "@" + SystemSettings.GetEmailDomain();
            }

            return OrganizationReplyToAddress;
        }

        public MailAddress GetReplyToMailAddress(string replyAddress = null)
        {
            string addr = GetReplyToAddress(replyAddress);

            try
            {
                return new MailAddress(addr);
            }
            catch (Exception)
            {
                return new MailAddress(SystemEmailID.ToString() + "@" + SystemSettings.GetEmailDomain());
            }
        }

        public bool IsInBusinessHours(DateTime utcDateTime)
        {

            if (IsDayInBusinessHours(utcDateTime.DayOfWeek))
            {

                if (BusinessDayEndUtc.TimeOfDay == BusinessDayStartUtc.TimeOfDay)
                {
                    return true;
                }
                else if (BusinessDayEndUtc.TimeOfDay < BusinessDayStartUtc.TimeOfDay)
                {
                    return utcDateTime.TimeOfDay <= BusinessDayEndUtc.TimeOfDay ||
                        utcDateTime.TimeOfDay >= BusinessDayStartUtc.TimeOfDay;
                }
                else
                {
                    return utcDateTime.TimeOfDay >= BusinessDayStartUtc.TimeOfDay &&
                        utcDateTime.TimeOfDay <= BusinessDayEndUtc.TimeOfDay;
                }
            }
            return false;
        }

        public bool IsDayInBusinessHours(DayOfWeek dayOfWeek)
        {
            if (BusinessDays == null) return false;
            return ((int)BusinessDays & (int)Math.Pow(2, (int)dayOfWeek)) > 0;
        }

        public string BusinessDaysText
        {
            get
            {
                StringBuilder days = new StringBuilder();
                foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
                {
                    if (IsDayInBusinessHours(dayOfWeek))
                    {
                        if (days.Length > 0) days.Append(", ");
                        days.Append(Enum.GetName(typeof(DayOfWeek), dayOfWeek));
                    }
                }
                return days.ToString();
            }
        }

        public void ClearBusinessDays()
        {
            BusinessDays = 0;
        }

        public void AddBusinessDay(DayOfWeek dayOfWeek)
        {
            BusinessDays = BusinessDays | (int)Math.Pow(2, (int)dayOfWeek);

        }

        public double GetTimeSpentMonth(LoginUser loginUser, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
SELECT ISNULL(SUM(a.TimeSpent), 0)
FROM Actions a
INNER JOIN Tickets t ON a.TicketID = t.TicketID
INNER JOIN OrganizationTickets ot ON a.TicketID = ot.TicketID
WHERE ot.OrganizationID = @OrganizationID
AND YEAR(a.DateModified) = YEAR(GetDate())
AND MONTH(a.DateModified)  = MONTH(GetDate())
";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);

                Organizations organizations = new Organizations(loginUser);

                return Convert.ToDouble(organizations.ExecuteScalar(command, "TicketsView"));
            }
        }

        public void FullReadFromXml(string data, bool isInsert, ref PhoneNumber phoneNumber, ref Address address)
        {
            //Of the 16 writeable fields only 6 are IDs. So we'll do a normal read and then add code for the 6 ID fields.
            this.ReadFromXml(data, isInsert);

            LoginUser user = Collection.LoginUser;
            FieldMap fieldMap = Collection.FieldMap;

            StringReader reader = new StringReader(data);
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(reader);

            //There are no contacts in a new account. Special implementation will be required if this field to be set in create organization.
            //try
            //{
            //  object primaryUserID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "PrimaryUserID", "PrimaryContact", User.GetIDByName, false, null);
            //  if (primaryUserID != null) this.PrimaryUserID = Convert.ToInt32(primaryUserID);
            //}
            //catch
            //{
            //}

            try
            {
                object defaultPortalGroupID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "DefaultPortalGroupID", "DefaultPortalGroup", Group.GetIDByName, false, null);
                if (defaultPortalGroupID != null) this.DefaultPortalGroupID = Convert.ToInt32(defaultPortalGroupID);
            }
            catch
            {
            }

            try
            {
                object defaultSupportGroupID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "DefaultSupportGroupID", "DefaultSupportGroup", Group.GetIDByName, false, null);
                if (defaultSupportGroupID != null) this.DefaultSupportGroupID = Convert.ToInt32(defaultSupportGroupID);
            }
            catch
            {
            }

            try
            {
                object defaultSupportUserID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "DefaultSupportUserID", "DefaultSupportUser", User.GetIDByName, false, user.OrganizationID);
                if (defaultSupportUserID != null) this.DefaultSupportUserID = Convert.ToInt32(defaultSupportUserID);
            }
            catch
            {
            }

            try
            {
                object sLALevelID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "SlaLevelID", "SlaName", SlaLevel.GetIDByName, false, null);
                if (sLALevelID != null) this.SlaLevelID = Convert.ToInt32(sLALevelID);
            }
            catch
            {
            }

            try
            {
                object phoneTypeID = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "PhoneTypeID", "PhoneType", PhoneType.GetIDByName, false, null, true);
                if (phoneTypeID != null) phoneNumber.PhoneTypeID = Convert.ToInt32(phoneTypeID);
            }
            catch
            {
            }

            try
            {
                object phoneNumberObject = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "PhoneNumber", string.Empty, null, false, null, true);
                if (phoneNumberObject != null) phoneNumber.Number = Convert.ToString(phoneNumberObject);
            }
            catch
            {
            }

            try
            {
                object phoneExtension = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "PhoneExtension", string.Empty, null, false, null, true);
                if (phoneExtension != null) phoneNumber.Extension = Convert.ToString(phoneExtension);
            }
            catch
            {
            }

            try
            {
                object addressDescription = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "AddressDescription", string.Empty, null, false, null, true);
                if (addressDescription != null) address.Description = Convert.ToString(addressDescription);
            }
            catch
            {
            }

            try
            {
                object addressLine1 = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "AddressLine1", string.Empty, null, false, null, true);
                if (addressLine1 != null) address.Addr1 = Convert.ToString(addressLine1);
            }
            catch
            {
            }

            try
            {
                object addressLine2 = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "AddressLine2", string.Empty, null, false, null, true);
                if (addressLine2 != null) address.Addr2 = Convert.ToString(addressLine2);
            }
            catch
            {
            }

            try
            {
                object addressLine3 = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "AddressLine3", string.Empty, null, false, null, true);
                if (addressLine3 != null) address.Addr3 = Convert.ToString(addressLine3);
            }
            catch
            {
            }

            try
            {
                object addressCity = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "AddressCity", string.Empty, null, false, null, true);
                if (addressCity != null) address.City = Convert.ToString(addressCity);
            }
            catch
            {
            }

            try
            {
                object addressState = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "AddressState", string.Empty, null, false, null, true);
                if (addressState != null) address.State = Convert.ToString(addressState);
            }
            catch
            {
            }

            try
            {
                object addressZip = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "AddressZip", string.Empty, null, false, null, true);
                if (addressZip != null) address.Zip = Convert.ToString(addressZip);
            }
            catch
            {
            }

            try
            {
                object addressCountry = DataUtils.GetValueFromObject(user, fieldMap, dataSet, "AddressCountry", string.Empty, null, false, null, true);
                if (addressCountry != null) address.Country = Convert.ToString(addressCountry);
            }
            catch
            {
            }
        }

        public string FontSizeDescription
        {
            get { return Enums.GetDescription((FontSize)Row["FontSize"]); }
        }

        public string FontFamilyDescription
        {
            get { return Enums.GetDescription((FontFamily)Row["FontFamily"]); }
        }

        public static int? GetIDByName(LoginUser loginUser, string name, int? parentID)
        {
            Organizations organizations = new Organizations(loginUser);
            organizations.LoadByOrganizationNameActive(name, (int)parentID);
            if (organizations.IsEmpty) return null;
            else return organizations[0].OrganizationID;
        }

        public static Organization GetCompanyByDomain(int parentOrganizationID, string emailDomain, LoginUser loginUser, bool forceUnknown = false)
        {
            Organization result = null;

            Organizations organizations = new Organizations(loginUser);
            organizations.LoadByParentID(parentOrganizationID, true);

            if (organizations.Any() && !forceUnknown)
            {
                List<KeyValuePair<int, string>> organizationSubDomainList = new List<KeyValuePair<int, string>>();

                //Takes all the suborganizations of the parent org and parses out the subdomains into a list for comparison
                foreach (var org in organizations.Distinct())
                {
                    if (!String.IsNullOrEmpty(org.CompanyDomains))
                    {
                        List<string> companydomains = org.CompanyDomains.Split(',').ToList();

                        foreach (var domain in companydomains)
                        {
                            organizationSubDomainList.Add(new KeyValuePair<int, string>(org.OrganizationID, domain));
                        }
                    }
                }

                //assigns subdomain of organization if match is found
                if (organizationSubDomainList.Any())
                {
                    var subOrgList = organizationSubDomainList.Where(m => m.Value.Trim() == emailDomain);

                    if (subOrgList.Any())
                    {
                        result = organizations.Where(m => m.OrganizationID == subOrgList.First().Key).First();
                    }
                }
            }

            //if we still don't have a suborganizationID assigned to result check if there is unknown company, otherwise create one
            if (result == null)
            {
                if (organizations.Any())
                {
                    var unknownCompanies = organizations.Where(m => m.Name.Contains("_Unknown Company"));
                    if (unknownCompanies.Any())
                    {
                        result = unknownCompanies.First();
                    }
                }
            }

            return result;
        }

    }

    public class SignUpParams
    {
        public SignUpParams()
        {
            initialCampaign = "";
            initialContent = "";
            initialMedium = "";
            initialSource = "";
            initialTerm = "";
            utmCampaign = "";
            utmContent = "";
            utmMedium = "";
            utmSource = "";
            utmTerm = "";
            gaCampaign = "";
            gaContent = "";
            gaMedium = "";
            gaSource = "";
            gaTerm = "";
            gaVisits = 0;
            hubspotutk = "";
            promo = "";
            source = "";
        }

        public string initialSource { get; set; }
        public string initialMedium { get; set; }
        public string initialTerm { get; set; }
        public string initialContent { get; set; }
        public string initialCampaign { get; set; }

        public string utmSource { get; set; }
        public string utmMedium { get; set; }
        public string utmTerm { get; set; }
        public string utmContent { get; set; }
        public string utmCampaign { get; set; }

        public string gaSource { get; set; }
        public string gaMedium { get; set; }
        public string gaTerm { get; set; }
        public string gaContent { get; set; }
        public string gaCampaign { get; set; }
        public int gaVisits { get; set; }

        public string hubspotutk { get; set; }
        public string promo { get; set; }
        public string source { get; set; }
    }

    public partial class Organizations
    {
        public static Organization compDst { get; private set; }

        public static Organization GetTemplateOrganization(LoginUser loginUser, ProductType productType)
        {
            Organizations orgTemplate = new Organizations(LoginUser.Anonymous);

            switch (productType)
            {
                case ProductType.Express: orgTemplate.LoadByOrganizationName("Trial Setup: Express", 1); break;
                case ProductType.HelpDesk: orgTemplate.LoadByOrganizationName("Trial Setup: Support", 1); break;
                case ProductType.Enterprise: orgTemplate.LoadByOrganizationName("Trial Setup: Enterprise", 1); break;
                default: break;
            }
            return orgTemplate.IsEmpty ? null : orgTemplate[0];
        }

        public static User SetupNewAccount(string firstName, string lastName, string email, string password, string company, string phone, string evalProcess, string potentialSeats, ProductType productType, SignUpParams signUpParams, string url, string referrer)
        {
            try
            {
                Organization sourceOrg = GetTemplateOrganization(LoginUser.Anonymous, productType);
                sourceOrg = sourceOrg ?? Organizations.GetOrganization(LoginUser.Anonymous, 1088);
                int sourceOrgID = sourceOrg.OrganizationID;


                Organization organization = (new Organizations(LoginUser.Anonymous)).AddNewOrganization();
                organization.Name = company.Trim();
                organization.ParentID = 1;
                organization.ProductType = productType;
                organization.PortalSeats = organization.ProductType == ProductType.Enterprise || organization.ProductType == ProductType.HelpDesk ? 999999 : 0;
                organization.IsApiActive = organization.ProductType != ProductType.Express;
                organization.IsApiEnabled = true;
                organization.IsAdvancedPortal = organization.PortalSeats > 0;
                organization.IsInventoryEnabled = organization.ProductType == ProductType.Enterprise;
                organization.WhereHeard = "Ya heard";
                organization.TimeZoneID = sourceOrg.TimeZoneID;
                organization.IsActive = true;
                organization.IsCustomerFree = false;
                organization.BusinessDays = sourceOrg.BusinessDays;
                organization.BusinessDayStart = sourceOrg.BusinessDayStartUtc;
                organization.BusinessDayEnd = sourceOrg.BusinessDayEndUtc;
                organization.ExtraStorageUnits = 0;
                organization.UserSeats = 100;
                organization.ChatSeats = 999999;
                organization.APIRequestLimit = 5000;
				organization.APIRequestMinuteLimit = 1200;
                organization.CultureName = "en-US";
                organization.PromoCode = "";
                organization.PrimaryInterest = "";
                organization.PotentialSeats = potentialSeats;
                organization.EvalProcess = evalProcess;
                organization.AddEmailViaTS = sourceOrg.AddEmailViaTS;
                organization.RequireKnownUserForNewEmail = sourceOrg.RequireKnownUserForNewEmail;
                organization.RequireNewKeyword = sourceOrg.RequireNewKeyword;
                organization.ChangeStatusIfClosed = sourceOrg.ChangeStatusIfClosed;
                organization.AddAdditionalContacts = sourceOrg.AddAdditionalContacts;
                organization.MatchEmailSubject = sourceOrg.MatchEmailSubject;
                organization.MarkSpam = sourceOrg.MarkSpam;
                organization.IsPublicArticles = sourceOrg.IsPublicArticles;
                organization.UseForums = sourceOrg.UseForums;
                organization.UseEuropeDate = sourceOrg.UseEuropeDate;
                organization.TimedActionsRequired = sourceOrg.TimedActionsRequired;
                organization.ProductVersionRequired = sourceOrg.ProductVersionRequired;
                organization.ProductRequired = sourceOrg.ProductRequired;
                organization.IsBasicPortal = sourceOrg.IsBasicPortal;
                organization.ShowWiki = sourceOrg.ShowWiki;
                organization.HasPortalAccess = sourceOrg.HasPortalAccess;
                organization.IsApiEnabled = sourceOrg.IsApiEnabled;
                organization.SetNewActionsVisibleToCustomers = sourceOrg.SetNewActionsVisibleToCustomers;
                organization.AgentRating = sourceOrg.AgentRating;
                organization.IsValidated = false;
                organization.SignUpToken = Guid.NewGuid().ToString();
                organization.UseProductFamilies = sourceOrg.UseProductFamilies;
                organization.IsCustomerInsightsActive = sourceOrg.IsCustomerInsightsActive;
                organization.AllowUnsecureAttachmentViewing = sourceOrg.AllowUnsecureAttachmentViewing;
                organization.ShowGroupMembersFirstInTicketAssignmentList = sourceOrg.ShowGroupMembersFirstInTicketAssignmentList;
                organization.Collection.Save();

                //374,826,378,703,377

                Users users = new Users(LoginUser.Anonymous);

                User user = users.AddNewUser();
                user.ActivatedOn = DateTime.UtcNow;
                user.Email = email.Trim();
                user.FirstName = firstName.Trim();
                user.InOffice = true;
                user.InOfficeComment = "";
                user.IsActive = true;
                user.IsSystemAdmin = true;
                user.IsFinanceAdmin = true;
                user.IsChatUser = true;
                user.IsPasswordExpired = false;
                user.LastLogin = DateTime.UtcNow;
                user.LastActivity = DateTime.UtcNow;
                user.LastName = lastName.Trim();
                user.MiddleName = "";
                user.EnforceSingleSession = true;
                user.OrganizationID = organization.OrganizationID;
                user.ReceiveTicketNotifications = true;
                user.ShowWelcomePage = true;
                user.CanCreateAsset = true;
                user.CanEditAsset = true;
                user.UserCanPinAction = true;
                user.CanChangeCommunityVisibility = true;
                user.ChangeTicketVisibility = true;
                user.ChangeKBVisibility = true;
                user.CanCreateContact = true;
                user.CanCreateCompany = true;
                user.CanEditCompany = true;
                user.CanEditContact = true;
                user.IsClassicView = true;
                user.CryptedPassword = password == "" ? "UNVALIDATED" : FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
                user.Collection.Save();



                LoginUser loginUser = new LoginUser(user.UserID, user.OrganizationID);

                OrganizationSettings.WriteString(loginUser, "DisableStatusNotification", true.ToString());

                PhoneNumber phoneNumber = (new PhoneNumbers(loginUser)).AddNewPhoneNumber();
                phoneNumber.Number = phone.Trim();
                phoneNumber.Extension = "";
                phoneNumber.RefID = organization.OrganizationID;
                phoneNumber.RefType = ReferenceType.Organizations;
                phoneNumber.Collection.Save();

                WikiArticles articles = new WikiArticles(LoginUser.Anonymous);
                articles.LoadByOrganizationID(sourceOrgID);
                foreach (WikiArticle article in articles)
                {
                    WikiArticle wiki = (new WikiArticles(loginUser)).AddNewWikiArticle();
                    wiki.CopyRowData(article);
                    wiki.OrganizationID = organization.OrganizationID;
                    wiki.Collection.Save();
                    if (sourceOrg.DefaultWikiArticleID != null && article.ArticleID == sourceOrg.DefaultWikiArticleID) organization.DefaultWikiArticleID = wiki.ArticleID;
                }


                organization.PrimaryUserID = user.UserID;
                organization.Collection.Save();

                PortalOptions portalOptions = new PortalOptions(loginUser);
                PortalOption portalOption = portalOptions.AddNewPortalOption();
                portalOption.CopyRowData(PortalOptions.GetPortalOption(loginUser, sourceOrgID));
                portalOption.OrganizationID = organization.OrganizationID;
                portalOption.PortalName = PortalOptions.ValidatePortalNameChars(organization.Name);

                portalOption.PortalHTMLHeader = string.Format(@"<div align=""right""><a href=""#"" onclick=""window.open('{1}/Chat/ChatInit.aspx?uid={0}', 'TSChat', 'toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,copyhistory=no,resizable=no,width=450,height=500'); return false;""><img src=""{1}/dc/{2}/chat/image"" border=""0"" /></a></div>",
                  organization.ChatID.ToString(),
                  SystemSettings.GetAppUrl(),
                  organization.OrganizationID.ToString());

                portalOptions.Save();

                ProductVersionStatuses pvsSources = new ProductVersionStatuses(loginUser);
                pvsSources.LoadAllPositions(sourceOrgID);
                ProductVersionStatuses pvDests = new ProductVersionStatuses(loginUser);
                foreach (ProductVersionStatus pvsSource in pvsSources)
                {
                    ProductVersionStatus pvsDest = pvDests.AddNewProductVersionStatus();
                    pvsDest.CopyRowData(pvsSource);
                    pvsDest.OrganizationID = organization.OrganizationID;
                }
                pvDests.Save();

                ActionTypes atSources = new ActionTypes(loginUser);
                atSources.LoadAllPositions(sourceOrgID);
                ActionTypes atDests = new ActionTypes(loginUser);
                foreach (ActionType atSource in atSources)
                {
                    ActionType atDest = atDests.AddNewActionType();
                    atDest.CopyRowData(atSource);
                    atDest.OrganizationID = organization.OrganizationID;
                }
                atDests.Save();

                PhoneTypes ptSources = new PhoneTypes(loginUser);
                ptSources.LoadAllPositions(sourceOrgID);
                PhoneTypes ptDests = new PhoneTypes(loginUser);
                foreach (PhoneType ptSource in ptSources)
                {
                    PhoneType ptDest = ptDests.AddNewPhoneType();
                    ptDest.CopyRowData(ptSource);
                    ptDest.OrganizationID = organization.OrganizationID;
                }
                ptDests.Save();

                TicketSeverities tsSources = new TicketSeverities(loginUser);
                tsSources.LoadAllPositions(sourceOrgID);
                TicketSeverities tsDests = new TicketSeverities(loginUser);
                foreach (TicketSeverity tsSource in tsSources)
                {
                    TicketSeverity tsDest = tsDests.AddNewTicketSeverity();
                    tsDest.CopyRowData(tsSource);
                    tsDest.OrganizationID = organization.OrganizationID;
                }
                tsDests.Save();


                TicketTypes ttSources = new TicketTypes(loginUser);
                ttSources.LoadAllPositions(sourceOrgID);
                TicketTypes ttDests = new TicketTypes(loginUser);
                foreach (TicketType ttSource in ttSources)
                {
                    TicketType ttDest = ttDests.AddNewTicketType();
                    ttDest.CopyRowData(ttSource);
                    ttDest.OrganizationID = organization.OrganizationID;
                }
                ttDests.Save();

                TicketStatuses tstDests = new TicketStatuses(loginUser);
                foreach (TicketType tt in ttSources)
                {
                    TicketStatuses statuses = new TicketStatuses(loginUser);
                    statuses.LoadAllPositions(tt.TicketTypeID);
                    foreach (TicketStatus tstSource in statuses)
                    {
                        TicketStatus tstDest = tstDests.AddNewTicketStatus();
                        tstDest.CopyRowData(tstSource);
                        tstDest.OrganizationID = organization.OrganizationID;
                        tstDest.TicketTypeID = ttDests.FindByName(tt.Name).TicketTypeID;
                    }
                }
                TicketStatuses tstSources = new TicketStatuses(loginUser);
                tstSources.LoadByOrganizationID(sourceOrgID);

                tstDests.Save();

                Groups grpSources = new Groups(loginUser);
                grpSources.LoadByOrganizationID(sourceOrgID);
                Groups grpDests = new Groups(loginUser);
                foreach (Group grpSource in grpSources)
                {
                    Group grpDest = grpDests.AddNewGroup();
                    grpDest.CopyRowData(grpSource);
                    grpDest.OrganizationID = organization.OrganizationID;
                    grpDests.Save();
                    grpDests.AddGroupUser(user.UserID, grpDest.GroupID);

                    if (sourceOrg.DefaultPortalGroupID != null && sourceOrg.DefaultPortalGroupID == grpSource.GroupID) sourceOrg.DefaultPortalGroupID = grpDest.GroupID;
                    if (sourceOrg.DefaultSupportGroupID != null && sourceOrg.DefaultSupportGroupID == grpSource.GroupID) sourceOrg.DefaultSupportGroupID = grpDest.GroupID;
                }

                Products prdSources = new Products(loginUser);
                prdSources.LoadByOrganizationID(sourceOrgID);
                Products prdDests = new Products(loginUser);
                foreach (Product prdSource in prdSources)
                {
                    Product prdDest = prdDests.AddNewProduct();
                    prdDest.CopyRowData(prdSource);
                    prdDest.OrganizationID = organization.OrganizationID;
                }
                prdDests.Save();

                ProductVersions verSources = new ProductVersions(loginUser);
                verSources.LoadAll(sourceOrgID);
                ProductVersions verDests = new ProductVersions(loginUser);
                foreach (ProductVersion verSource in verSources)
                {
                    ProductVersion verDest = verDests.AddNewProductVersion();
                    verDest.CopyRowData(verSource);
                    verDest.NeedsIndexing = true;
                    verDest.ProductID = prdDests.FindByName(prdSources.FindByProductID(verDest.ProductID).Name).ProductID;
                    verDest.ProductVersionStatusID = pvDests.FindByName(pvsSources.FindByProductVersionStatusID(verDest.ProductVersionStatusID).Name).ProductVersionStatusID;
                }
                verDests.Save();

                KnowledgeBaseCategories kbSources = new KnowledgeBaseCategories(loginUser);
                kbSources.LoadAllCategories(sourceOrgID);
                KnowledgeBaseCategories kbDests = new KnowledgeBaseCategories(loginUser);
                foreach (KnowledgeBaseCategory kbSource in kbSources)
                {
                    if (kbSource.ParentID < 0)
                    {
                        KnowledgeBaseCategory kbDest = kbDests.AddNewKnowledgeBaseCategory();
                        kbDest.CopyRowData(kbSource);
                        kbDest.OrganizationID = organization.OrganizationID;
                    }
                }
                kbDests.Save();
                KnowledgeBaseCategories kbSubs = new KnowledgeBaseCategories(loginUser);

                foreach (KnowledgeBaseCategory kbSource in kbSources)
                {
                    if (kbSource.ParentID > 0)
                    {
                        KnowledgeBaseCategory kbDest = kbSubs.AddNewKnowledgeBaseCategory();
                        kbDest.CopyRowData(kbSource);
                        kbDest.ParentID = kbDests.FindByName(kbSources.FindByCategoryID(kbSource.ParentID).CategoryName, -1).CategoryID;
                        kbDest.OrganizationID = organization.OrganizationID;
                    }
                }
                kbSubs.Save();

                kbDests = new KnowledgeBaseCategories(loginUser);
                kbDests.LoadAllCategories(organization.OrganizationID);
                Random random = new Random();

                // copy companies

                Organizations compSrcs = new Data.Organizations(loginUser);
                compSrcs.LoadByParentID(sourceOrgID, true);
                Organizations compDsts = new Organizations(loginUser);


                foreach (Organization compSrc in compSrcs)
                {
                    Organization compDst = compDsts.AddNewOrganization();
                    compDst.ParentID = organization.OrganizationID;
                    compDst.PrimaryUserID = null;
                    compDst.ExtraStorageUnits = 0;
                    compDst.PortalSeats = 0;
                    compDst.UserSeats = 0;
                    compDst.IsCustomerFree = false;
                    compDst.ProductType = ProductType.Enterprise;
                    compDst.IsBasicPortal = true;
                    compDst.DefaultSupportUserID = null;
                    compDst.DefaultPortalGroupID = null;
                    compDst.DefaultSupportGroupID = null;
                    compDst.TimeZoneID = compSrc.TimeZoneID;
                    compDst.NeedsIndexing = true;
                    compDst.Name = compSrc.Name;
                    compDst.Website = compSrc.Website;
                    compDst.CompanyDomains = "";
                    compDst.Description = "";
                    compDst.SAExpirationDate = null;
                    compDst.SlaLevelID = null;
                    compDst.SupportHoursMonth = compSrc.SupportHoursMonth;
                    compDst.HasPortalAccess = compSrc.HasPortalAccess;
                    compDst.IsActive = compSrc.IsActive;
                    compDst.IsApiActive = compSrc.IsApiActive;
                    compDst.IsApiEnabled = compSrc.IsApiEnabled;
                    compDst.ImportID = compSrc.OrganizationID.ToString();
                    compDst.InActiveReason = "";
                }
                compDsts.Save();

                foreach (Organization compSrc in compSrcs)
                {
                    Users contSrcs = new Users(loginUser);
                    contSrcs.LoadByOrganizationID(compSrc.OrganizationID, true);
                    Users contDsts = new Users(loginUser);

                    foreach (User contSrc in contSrcs)
                    {
                        User contDst = contDsts.AddNewUser();
                        int dstOrgID = -1;
                        foreach (Organization compDst in compDsts)
                        {
                            if (compDst.ImportID == contSrc.OrganizationID.ToString())
                            {
                                dstOrgID = compDst.OrganizationID;
                                break;
                            }
                        }
                        if (dstOrgID > -1)
                        {
                            contDst.OrganizationID = dstOrgID;
                            contDst.ActivatedOn = DateTime.UtcNow;
                            contDst.LastLogin = DateTime.UtcNow;
                            contDst.LastActivity = DateTime.UtcNow.AddHours(-1);
                            contDst.IsPasswordExpired = true;
                            contDst.ReceiveTicketNotifications = true;
                            contDst.NeedsIndexing = true;
                            contDst.Email = contSrc.Email;
                            contDst.FirstName = contSrc.FirstName;
                            contDst.LastName = contSrc.LastName;
                            contDst.Title = contSrc.Title;
                            contDst.MiddleName = contSrc.MiddleName;
                            contDst.BlockInboundEmail = contSrc.BlockInboundEmail;
                            contDst.BlockEmailFromCreatingOnly = contSrc.BlockEmailFromCreatingOnly;
                            contDst.IsActive = contSrc.IsActive;
                            contDst.IsPortalUser = contSrc.IsPortalUser;
                            contDst.IsSystemAdmin = contSrc.IsSystemAdmin;
                            contDst.IsFinanceAdmin = contSrc.IsFinanceAdmin;
                        }

                    }

                    contDsts.Save();
                }


                Tickets tktSources = new Tickets(loginUser);
                tktSources.LoadByOrganizationID(sourceOrgID);
                Tickets tktDests = new Tickets(loginUser);
                for (int j = tktSources.Count - 1; j >= 0; j--)
                {
                    Ticket tktSource = tktSources[j];
                    try
                    {
                        Ticket tktDest = tktDests.AddNewTicket();
                        tktDest.CopyRowData(tktSource);
                        tktDest.OrganizationID = organization.OrganizationID;
                        tktDest.ParentID = null;
                        tktDest.UserID = user.UserID;
                        tktDest.NeedsIndexing = true;

                        tktDest.TicketTypeID = ttDests.FindByName(ttSources.FindByTicketTypeID(tktSource.TicketTypeID).Name).TicketTypeID;
                        tktDest.TicketSeverityID = tsDests.FindByName(tsSources.FindByTicketSeverityID(tktSource.TicketSeverityID).Name).TicketSeverityID;
                        tktDest.TicketStatusID = tstDests.FindByName(tstSources.FindByTicketStatusID(tktSource.TicketStatusID).Name, tktDest.TicketTypeID).TicketStatusID;
                        if (tktSource.GroupID != null)
                            tktDest.GroupID = grpDests.FindByName(grpSources.FindByGroupID((int)tktSource.GroupID).Name).GroupID;
                        if (tktSource.ProductID != null)
                        {
                            tktDest.ProductID = prdDests.FindByName(prdSources.FindByProductID((int)tktSource.ProductID).Name).ProductID;
                            if (tktSource.SolvedVersionID != null)
                                tktDest.SolvedVersionID = verDests.FindByVersionNumber(verSources.FindByProductVersionID((int)tktSource.SolvedVersionID).VersionNumber, (int)tktDest.ProductID).ProductVersionID;
                            if (tktSource.ReportedVersionID != null)
                                tktDest.ReportedVersionID = verDests.FindByVersionNumber(verSources.FindByProductVersionID((int)tktSource.ReportedVersionID).VersionNumber, (int)tktDest.ProductID).ProductVersionID;
                        }
                        if (tktSource.KnowledgeBaseCategoryID != null)
                        {
                            tktDest.KnowledgeBaseCategoryID = kbDests.FindByName(kbSources.FindByCategoryID((int)tktSource.KnowledgeBaseCategoryID).CategoryName).CategoryID;
                        }
                        int hours = random.Next(-168, 0);
                        tktDest.DateCreated = DateTime.UtcNow.AddHours(hours);
                        tktDest.DateModified = DateTime.UtcNow.AddHours(hours);

                        tktDests.Save();

                        Actions actSources = new Actions(loginUser);
                        actSources.LoadByTicketID(tktSource.TicketID);
                        Actions actDests = new Actions(loginUser);

                        foreach (Action actSource in actSources)
                        {
                            Action actDest = actDests.AddNewAction();
                            actDest.CopyRowData(actSource);
                            actDest.TicketID = tktDest.TicketID;
                            actDest.DateCreated = DateTime.UtcNow.AddHours(hours);
                            actDest.DateModified = DateTime.UtcNow.AddHours(hours);

                            if (actSource.ActionTypeID != null)
                                actDest.ActionTypeID = atDests.FindByName(atSources.FindByActionTypeID((int)actSource.ActionTypeID).Name).ActionTypeID;
                        }
                        actDests.Save();




                    }
                    catch (Exception ex)
                    {
                        ExceptionLogs.LogException(loginUser, ex, "sign up");
                    }

                }

                // copy dashboard
                UserSettings.WriteString(loginUser, "Dashboard", UserSettings.ReadString(loginUser, (int)sourceOrg.PrimaryUserID, "Dashboard"));
                sourceOrg.Collection.Save();

				// Copy Tasks. Only for Enterprise
				if (productType == ProductType.Enterprise)
				{
					Tasks tasksSource = new Tasks(loginUser);
					tasksSource.LoadByOrganizationID(sourceOrgID);

					foreach (Task taskSource in tasksSource)
					{
						Task newTask = (new Tasks(loginUser)).AddNewTask();
						newTask.ParentID = taskSource.ParentID;
						newTask.OrganizationID = organization.OrganizationID;
						newTask.Name = taskSource.Name;
						newTask.Description = taskSource.Description;
						newTask.UserID = user.UserID;
						newTask.IsComplete = taskSource.IsComplete;

						if (taskSource.DueDate.HasValue)
						{
							TimeSpan hours = taskSource.DueDate.Value - taskSource.DateCreated;
							newTask.DueDate = DateTime.UtcNow.AddHours(hours.TotalHours);
						}

						newTask.DateCompleted = taskSource.DateCompleted;
						newTask.Collection.Save();

						TaskAssociations taskAssociationsSource = new TaskAssociations(loginUser);
						taskAssociationsSource.LoadByTaskIDOnly(taskSource.TaskID);

						foreach (TaskAssociation taskAssociationSource in taskAssociationsSource)
						{
							int? refId = null;

							switch (taskAssociationSource.RefType)
							{
								case (int)ReferenceType.Tickets:
									//Tickets newOrgTickets = new Tickets(loginUser);
									//newOrgTickets.LoadByOrganizationID(organization.OrganizationID);

									if (tktDests.Any())
									{
										refId = tktDests[0].TicketID;
									}
									break;
								case (int)ReferenceType.Organizations:
									if (compDsts.Any())
									{
										refId = compDsts[0].OrganizationID;
									}
									break;
								default:
									break;
							}

							if (refId != null)
							{
								TaskAssociation newTaskAssociation = (new TaskAssociations(loginUser)).AddNewTaskAssociation();
								newTaskAssociation.TaskID = newTask.TaskID;
								newTaskAssociation.RefID = (int)refId;
								newTaskAssociation.RefType = taskAssociationSource.RefType;
								newTaskAssociation.CreatorID = user.UserID;
								newTaskAssociation.DateCreated = DateTime.Now;
								newTaskAssociation.Collection.Save();
							}
						}

						Reminders remindersSource = new Reminders(loginUser);
						remindersSource.LoadByItemAll(ReferenceType.Tasks, taskSource.TaskID, taskSource.UserID);

						foreach (Reminder reminderSource in remindersSource)
						{
							Reminder newReminder = (new Reminders(loginUser)).AddNewReminder();
							newReminder.OrganizationID = organization.OrganizationID;
							newReminder.RefType = ReferenceType.Tasks;
							newReminder.RefID = newTask.TaskID;
							newReminder.Description = reminderSource.Description;

							TimeSpan hours = reminderSource.DueDate - reminderSource.DateCreated;
							newReminder.DueDate = DateTime.UtcNow.AddHours(hours.TotalHours);

							newReminder.UserID = user.UserID;
							newReminder.IsDismissed = reminderSource.IsDismissed;
							newReminder.HasEmailSent = reminderSource.HasEmailSent;
							newReminder.CreatorID = user.UserID;
							newReminder.Collection.Save();

							newTask.ReminderID = newReminder.ReminderID;
							newTask.Collection.Save();
						}
					}
				}

				EmailPosts.SendWelcomeNewSignup(loginUser, user.UserID, "");
                EmailPosts.SendSignUpNotification(loginUser, user.UserID, url, referrer);
                TeamSupportSync.SyncNewOrg(organization.OrganizationID, organization.Name, user.UserID, user.FirstName, user.LastName, user.Email, phoneNumber.Number, productType, signUpParams != null ? signUpParams.promo : "", potentialSeats, signUpParams != null ? signUpParams.hubspotutk : "", signUpParams != null ? signUpParams.gaSource : "", signUpParams != null ? signUpParams.gaCampaign : "");

                return user;
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "sign up");
            }
            return null;
        }

        public static void CreateStandardData(LoginUser loginUser, Organization organization, bool createTypes, bool createWorkflow)
        {

            Organizations organizations = new Organizations(loginUser);
            organizations.LoadByOrganizationID(loginUser.OrganizationID);
            if (organizations.IsEmpty || (organizations[0].ParentID != 1 && organizations[0].ParentID != null)) return;

            TicketTypes typesTest = new TicketTypes(loginUser);
            typesTest.LoadByOrganizationID(organization.OrganizationID, organization.ProductType);
            if (!typesTest.IsEmpty) return;

            if (createTypes)
            {
                #region Product Version Statuses
                ProductVersionStatuses productVersionStatuses = new ProductVersionStatuses(loginUser);
                ProductVersionStatus productVersionStatus;

                productVersionStatus = productVersionStatuses.AddNewProductVersionStatus();
                productVersionStatus.Name = "Alpha";
                productVersionStatus.Description = "In house release";
                productVersionStatus.IsDiscontinued = false;
                productVersionStatus.IsShipping = false;
                productVersionStatus.OrganizationID = organization.OrganizationID;
                productVersionStatus.Position = 0;

                productVersionStatus = productVersionStatuses.AddNewProductVersionStatus();
                productVersionStatus.Name = "Beta";
                productVersionStatus.Description = "External test release";
                productVersionStatus.IsDiscontinued = false;
                productVersionStatus.IsShipping = false;
                productVersionStatus.OrganizationID = organization.OrganizationID;
                productVersionStatus.Position = 1;

                productVersionStatus = productVersionStatuses.AddNewProductVersionStatus();
                productVersionStatus.Name = "Released";
                productVersionStatus.Description = "Live customer use";
                productVersionStatus.IsDiscontinued = false;
                productVersionStatus.IsShipping = true;
                productVersionStatus.OrganizationID = organization.OrganizationID;
                productVersionStatus.Position = 2;

                productVersionStatus = productVersionStatuses.AddNewProductVersionStatus();
                productVersionStatus.Name = "Discontinued";
                productVersionStatus.Description = "";
                productVersionStatus.IsDiscontinued = true;
                productVersionStatus.IsShipping = false;
                productVersionStatus.OrganizationID = organization.OrganizationID;
                productVersionStatus.Position = 3;
                productVersionStatuses.Save();
                #endregion
            }

            if (createTypes)
            {
                #region Action Types
                ActionTypes actionTypes = new ActionTypes(loginUser);
                ActionType actionType;

                actionType = actionTypes.AddNewActionType();
                actionType.Name = "Comment";
                actionType.Description = "Add comments for a ticket.";
                actionType.OrganizationID = organization.OrganizationID;
                actionType.Position = 0;
                actionType.IsTimed = true;

                actionTypes.Save();

                #endregion
            }

            if (createTypes)
            {
                #region Phone Types
                PhoneTypes phoneTypes = new PhoneTypes(loginUser);
                PhoneType phoneType;

                phoneType = phoneTypes.AddNewPhoneType();
                phoneType.Name = "Work";
                phoneType.Description = "Work";
                phoneType.Position = 0;
                phoneType.OrganizationID = organization.OrganizationID;

                phoneType = phoneTypes.AddNewPhoneType();
                phoneType.Name = "Mobile";
                phoneType.Description = "Mobile";
                phoneType.Position = 1;
                phoneType.OrganizationID = organization.OrganizationID;

                phoneType = phoneTypes.AddNewPhoneType();
                phoneType.Name = "Home";
                phoneType.Description = "Home";
                phoneType.Position = 2;
                phoneType.OrganizationID = organization.OrganizationID;

                phoneType = phoneTypes.AddNewPhoneType();
                phoneType.Name = "Fax";
                phoneType.Description = "Fax";
                phoneType.Position = 3;
                phoneType.OrganizationID = organization.OrganizationID;

                phoneTypes.Save();
                #endregion
            }

            TicketTypes ticketTypes = new TicketTypes(loginUser);
            TicketStatuses ticketStatuses;
            TicketNextStatuses ticketNextStatuses;

            TicketType ticketType;
            TicketStatus ticketStatus;

            #region Support
            ticketStatuses = new TicketStatuses(loginUser);

            ticketType = ticketTypes.AddNewTicketType();
            ticketType.Name = "Support";
            ticketType.Description = "Support";
            ticketType.OrganizationID = organization.OrganizationID;
            ticketType.IconUrl = "Images/TicketTypes/Phone.gif";
            ticketType.Position = 0;
            ticketTypes.Save();

            if (createTypes)
            {

                ticketStatus = ticketStatuses.AddNewTicketStatus();
                ticketStatus.Name = "New";
                ticketStatus.Description = "New";
                ticketStatus.Position = 0;
                ticketStatus.OrganizationID = organization.OrganizationID;
                ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                ticketStatus.IsClosed = false;

                ticketStatus = ticketStatuses.AddNewTicketStatus();
                ticketStatus.Name = "Under Review";
                ticketStatus.Description = "Under Review";
                ticketStatus.Position = 1;
                ticketStatus.OrganizationID = organization.OrganizationID;
                ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                ticketStatus.IsClosed = false;

                ticketStatus = ticketStatuses.AddNewTicketStatus();
                ticketStatus.Name = "Need More Info";
                ticketStatus.Description = "Need More Info";
                ticketStatus.Position = 2;
                ticketStatus.OrganizationID = organization.OrganizationID;
                ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                ticketStatus.IsClosed = false;

                ticketStatus = ticketStatuses.AddNewTicketStatus();
                ticketStatus.Name = "Waiting on Customer";
                ticketStatus.Description = "Waiting on Customer";
                ticketStatus.Position = 3;
                ticketStatus.OrganizationID = organization.OrganizationID;
                ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                ticketStatus.IsClosed = false;

                ticketStatus = ticketStatuses.AddNewTicketStatus();
                ticketStatus.Name = "Customer Responded";
                ticketStatus.Description = "Customer Responded";
                ticketStatus.IsEmailResponse = true;
                ticketStatus.Position = 4;
                ticketStatus.OrganizationID = organization.OrganizationID;
                ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                ticketStatus.IsClosed = false;

                ticketStatus = ticketStatuses.AddNewTicketStatus();
                ticketStatus.Name = "On Hold";
                ticketStatus.Description = "On Hold";
                ticketStatus.Position = 5;
                ticketStatus.OrganizationID = organization.OrganizationID;
                ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                ticketStatus.IsClosed = false;

                ticketStatus = ticketStatuses.AddNewTicketStatus();
                ticketStatus.Name = "Inform Customer";
                ticketStatus.Description = "Inform Customer";
                ticketStatus.Position = 6;
                ticketStatus.OrganizationID = organization.OrganizationID;
                ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                ticketStatus.IsClosed = false;

                ticketStatus = ticketStatuses.AddNewTicketStatus();
                ticketStatus.Name = "Closed";
                ticketStatus.Description = "Closed";
                ticketStatus.Position = 7;
                ticketStatus.OrganizationID = organization.OrganizationID;
                ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                ticketStatus.IsClosed = true;

                ticketStatuses.Save();

                if (createWorkflow)
                {
                    ticketNextStatuses = new TicketNextStatuses(loginUser);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[1], 0);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[2], 1);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[3], 2);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[4], 3);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[5], 4);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[6], 5);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[7], 6);

                    //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[0], 0);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[2], 1);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[3], 2);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[4], 3);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[5], 4);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[6], 5);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[7], 6);

                    //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[0], 0);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[1], 1);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[3], 2);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[4], 3);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[5], 4);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[6], 5);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[7], 6);

                    //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[0], 0);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[1], 1);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[2], 2);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[4], 3);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[5], 4);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[6], 5);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[7], 6);

                    //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[0], 0);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[1], 1);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[2], 2);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[3], 3);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[5], 4);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[6], 5);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[7], 6);


                    //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[0], 0);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[1], 1);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[2], 2);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[3], 3);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[4], 4);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[6], 5);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[7], 6);


                    //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[0], 0);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[1], 1);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[2], 2);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[3], 3);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[4], 4);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[5], 5);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[7], 6);


                    //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[0], 0);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[1], 1);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[2], 2);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[3], 3);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[4], 4);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[5], 5);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[6], 6);

                    //ticketNextStatuses.Save();
                }
            }

            #endregion

            if (organization.ProductType == ProductType.Enterprise)
            {

                #region Defects
                ticketStatuses = new TicketStatuses(loginUser);

                ticketType = ticketTypes.AddNewTicketType();
                ticketType.Name = "Defects";
                ticketType.Description = "Defects";
                ticketType.OrganizationID = organization.OrganizationID;
                ticketType.IconUrl = "Images/TicketTypes/Bugs.png";
                ticketType.Position = 1;
                ticketTypes.Save();

                if (createTypes)
                {

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "New";
                    ticketStatus.Description = "New";
                    ticketStatus.Position = 0;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "Under Review";
                    ticketStatus.Description = "Under Review";
                    ticketStatus.Position = 1;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "Need More Info";
                    ticketStatus.Description = "Need More Info";
                    ticketStatus.Position = 2;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "Waiting on Customer";
                    ticketStatus.Description = "Waiting on Customer";
                    ticketStatus.Position = 3;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "Customer Responded";
                    ticketStatus.Description = "Customer Responded";
                    ticketStatus.IsEmailResponse = true;
                    ticketStatus.Position = 4;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "On Hold";
                    ticketStatus.Description = "On Hold";
                    ticketStatus.Position = 5;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;


                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "In Engineering";
                    ticketStatus.Description = "In Engineering";
                    ticketStatus.Position = 6;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "In QA";
                    ticketStatus.Description = "In QA";
                    ticketStatus.Position = 7;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "Inform Customer";
                    ticketStatus.Description = "Inform Customer";
                    ticketStatus.Position = 8;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "Closed";
                    ticketStatus.Description = "Closed";
                    ticketStatus.Position = 9;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = true;

                    ticketStatuses.Save();

                    if (createWorkflow)
                    {

                        ticketNextStatuses = new TicketNextStatuses(loginUser);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[1], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[2], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[3], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[4], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[5], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[6], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[7], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[8], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[9], 8);

                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[2], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[3], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[4], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[5], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[6], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[7], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[8], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[9], 8);

                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[1], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[3], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[4], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[5], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[6], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[7], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[8], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[9], 8);

                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[1], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[2], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[4], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[5], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[6], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[7], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[8], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[9], 8);

                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[1], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[2], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[3], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[5], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[6], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[7], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[8], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[9], 8);


                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[1], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[2], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[3], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[4], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[6], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[7], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[8], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[9], 8);


                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[1], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[2], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[3], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[4], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[5], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[7], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[8], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[9], 8);


                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[1], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[2], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[3], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[4], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[5], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[6], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[8], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[9], 8);

                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[1], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[2], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[3], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[4], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[5], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[6], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[7], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[9], 8);

                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[1], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[2], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[3], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[4], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[5], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[6], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[7], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[8], 8);

                        //ticketNextStatuses.Save();
                    }
                }

                #endregion

                #region Features
                ticketStatuses = new TicketStatuses(loginUser);

                ticketType = ticketTypes.AddNewTicketType();
                ticketType.Name = "Features";
                ticketType.Description = "Features";
                ticketType.OrganizationID = organization.OrganizationID;
                ticketType.IconUrl = "Images/TicketTypes/Features.png";
                ticketType.Position = 2;
                ticketTypes.Save();

                if (createTypes)
                {
                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "New";
                    ticketStatus.Description = "New";
                    ticketStatus.Position = 0;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "Waiting Approval";
                    ticketStatus.Description = "Waiting Approval";
                    ticketStatus.Position = 1;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "Need More Info";
                    ticketStatus.Description = "Need More Info";
                    ticketStatus.Position = 2;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "Waiting on Customer";
                    ticketStatus.Description = "Waiting on Customer";
                    ticketStatus.Position = 3;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "Customer Responded";
                    ticketStatus.Description = "Customer Responded";
                    ticketStatus.IsEmailResponse = true;
                    ticketStatus.Position = 4;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "On Hold";
                    ticketStatus.Description = "On Hold";
                    ticketStatus.Position = 5;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;


                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "Approved";
                    ticketStatus.Description = "Approved";
                    ticketStatus.Position = 6;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "Rejected";
                    ticketStatus.Description = "Rejected";
                    ticketStatus.Position = 7;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "In Engineering";
                    ticketStatus.Description = "In Engineering";
                    ticketStatus.Position = 8;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "In QA";
                    ticketStatus.Description = "In QA";
                    ticketStatus.Position = 9;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "Inform Customer";
                    ticketStatus.Description = "Inform Customer";
                    ticketStatus.Position = 10;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = false;

                    ticketStatus = ticketStatuses.AddNewTicketStatus();
                    ticketStatus.Name = "Closed";
                    ticketStatus.Description = "Closed";
                    ticketStatus.Position = 11;
                    ticketStatus.OrganizationID = organization.OrganizationID;
                    ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                    ticketStatus.IsClosed = true;


                    ticketStatuses.Save();
                    if (createWorkflow)
                    {
                        ticketNextStatuses = new TicketNextStatuses(loginUser);

                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[1], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[2], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[3], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[4], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[5], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[6], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[7], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[8], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[9], 8);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[10], 9);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[11], 10);

                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[2], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[3], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[4], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[5], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[6], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[7], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[8], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[9], 8);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[10], 9);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[11], 10);

                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[1], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[3], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[4], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[5], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[6], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[7], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[8], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[9], 8);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[10], 9);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[11], 10);

                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[1], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[2], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[4], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[5], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[6], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[7], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[8], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[9], 8);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[10], 9);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[3], ticketStatuses[11], 10);

                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[1], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[2], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[3], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[5], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[6], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[7], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[8], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[9], 8);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[10], 9);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[4], ticketStatuses[11], 10);

                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[1], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[2], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[3], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[4], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[6], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[7], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[8], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[9], 8);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[10], 9);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[5], ticketStatuses[11], 10);


                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[1], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[2], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[3], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[4], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[5], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[7], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[8], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[9], 8);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[10], 9);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[6], ticketStatuses[11], 10);

                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[1], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[2], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[3], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[4], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[5], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[6], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[8], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[9], 8);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[10], 9);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[7], ticketStatuses[11], 10);

                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[1], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[2], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[3], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[4], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[5], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[6], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[7], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[9], 8);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[10], 9);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[8], ticketStatuses[11], 10);

                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[1], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[2], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[3], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[4], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[5], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[6], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[7], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[8], 8);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[10], 9);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[9], ticketStatuses[11], 10);

                        //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[1], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[2], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[3], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[4], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[5], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[6], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[7], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[8], 8);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[9], 9);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[10], ticketStatuses[11], 10);

                        //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[0], 0);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[1], 1);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[2], 2);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[3], 3);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[4], 4);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[5], 5);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[6], 6);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[7], 7);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[8], 8);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[9], 9);
                        //ticketNextStatuses.AddNextStatus(ticketStatuses[11], ticketStatuses[10], 10);
                        //ticketNextStatuses.Save();
                    }
                }
                #endregion

            }
            #region Tasks
            ticketStatuses = new TicketStatuses(loginUser);

            ticketType = ticketTypes.AddNewTicketType();
            ticketType.Name = "Tasks";
            ticketType.Description = "Tasks";
            ticketType.OrganizationID = organization.OrganizationID;
            ticketType.IconUrl = "Images/TicketTypes/Tasks.png";
            ticketType.Position = 3;

            ticketTypes.Save();
            if (createTypes)
            {

                ticketStatus = ticketStatuses.AddNewTicketStatus();
                ticketStatus.Name = "New";
                ticketStatus.Description = "New";
                ticketStatus.Position = 0;
                ticketStatus.OrganizationID = organization.OrganizationID;
                ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                ticketStatus.IsClosed = false;

                ticketStatus = ticketStatuses.AddNewTicketStatus();
                ticketStatus.Name = "In Progress";
                ticketStatus.Description = "In Progress";
                ticketStatus.Position = 1;
                ticketStatus.OrganizationID = organization.OrganizationID;
                ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                ticketStatus.IsClosed = false;

                ticketStatus = ticketStatuses.AddNewTicketStatus();
                ticketStatus.Name = "Closed";
                ticketStatus.Description = "Closed";
                ticketStatus.Position = 2;
                ticketStatus.OrganizationID = organization.OrganizationID;
                ticketStatus.TicketTypeID = ticketType.TicketTypeID;
                ticketStatus.IsClosed = true;

                ticketStatuses.Save();

                if (createWorkflow)
                {
                    ticketNextStatuses = new TicketNextStatuses(loginUser);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[1], 0);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[2], 1);

                    //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[0], 0);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[2], 1);

                    //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[0], 0);
                    //ticketNextStatuses.AddNextStatus(ticketStatuses[2], ticketStatuses[1], 1);

                    //ticketNextStatuses.Save();
                }
            }
            #endregion



            if (createTypes)
            {

                #region Ticket Severities
                TicketSeverities ticketSeverities = new TicketSeverities(loginUser);
                TicketSeverity ticketSeverity;

                ticketSeverity = ticketSeverities.AddNewTicketSeverity();
                ticketSeverity.Name = "Unassigned";
                ticketSeverity.Description = "Unassigned";
                ticketSeverity.Position = 0;
                ticketSeverity.OrganizationID = organization.OrganizationID;

                ticketSeverity = ticketSeverities.AddNewTicketSeverity();
                ticketSeverity.Name = "High";
                ticketSeverity.Description = "High";
                ticketSeverity.Position = 1;
                ticketSeverity.OrganizationID = organization.OrganizationID;

                ticketSeverity = ticketSeverities.AddNewTicketSeverity();
                ticketSeverity.Name = "Medium";
                ticketSeverity.Description = "Medium";
                ticketSeverity.Position = 2;
                ticketSeverity.OrganizationID = organization.OrganizationID;

                ticketSeverity = ticketSeverities.AddNewTicketSeverity();
                ticketSeverity.Name = "Low";
                ticketSeverity.Description = "Low";
                ticketSeverity.Position = 3;
                ticketSeverity.OrganizationID = organization.OrganizationID;

                ticketSeverities.Save();
                #endregion
            }

            Groups groups = new Groups(loginUser);
            Group group = groups.AddNewGroup();
            group.Description = "Default Support Group";
            group.Name = "Support";
            group.OrganizationID = organization.OrganizationID;
            group.Collection.Save();

            groups.AddGroupUser(loginUser.UserID, group.GroupID);
            organization.DefaultPortalGroupID = group.GroupID;
            organization.Collection.Save();

            WaterCoolerItem wc = (new WaterCooler(loginUser)).AddNewWaterCoolerItem();
            wc.UserID = -1;
            wc.OrganizationID = loginUser.OrganizationID;
            wc.TimeStamp = DateTime.UtcNow;
            wc.GroupFor = null;
            wc.ReplyTo = null;
            wc.Message = "Welcome to the Water Cooler!  This is a place to share group updates, interesting articles, team member statuses, and other items of interest to the rest of your team.";
            wc.MessageType = "Comment";
            wc.Collection.Save();



        }

        partial void BeforeRowDelete(int organizationID)
        {
            Organization organization = (Organization)Organizations.GetOrganization(LoginUser, organizationID);
            string description = "Deleted organization '" + organization.Name + "'";
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Delete, ReferenceType.Organizations, organizationID, description);
        }

        partial void BeforeRowEdit(Organization organization)
        {
            string description;

            Organization oldOrganization = (Organization)Organizations.GetOrganization(LoginUser, organization.OrganizationID);

            if (oldOrganization.Description != organization.Description)
            {
                description = "Changed description for organization '" + organization.Name + "'";
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Organizations, organization.OrganizationID, description);
            }

            if (oldOrganization.HasPortalAccess != organization.HasPortalAccess)
            {
                description = "Changed portal access rights for organization '" + organization.Name + "' to '" + organization.HasPortalAccess.ToString() + "'";
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Organizations, organization.OrganizationID, description);
            }

            if (oldOrganization.InActiveReason != organization.InActiveReason)
            {
                description = "Changed inactive reason for organization '" + organization.Name + "'";
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Organizations, organization.OrganizationID, description);
            }

            if (oldOrganization.IsActive != organization.IsActive)
            {
                description = "Changed active status for organization '" + organization.Name + "' to '" + organization.IsActive.ToString() + "'";
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Organizations, organization.OrganizationID, description);
            }

            if (oldOrganization.Name != organization.Name)
            {
                description = "Changed organization name from '" + oldOrganization.Name + "' to '" + organization.Name + "'";
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Organizations, organization.OrganizationID, description);
            }

            if (oldOrganization.PrimaryUserID != organization.PrimaryUserID)
            {
                string name1 = oldOrganization.PrimaryUserID == null ? "Unassigned" : Users.GetUserFullName(LoginUser, (int)oldOrganization.PrimaryUserID);
                string name2 = organization.PrimaryUserID == null ? "Unassigned" : Users.GetUserFullName(LoginUser, (int)organization.PrimaryUserID);

                description = "Changed primary contact for organization '" + organization.Name + "' from '" + name1 + "' to '" + name2 + "'";
                ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Organizations, organization.OrganizationID, description);
            }
        }

        partial void AfterRowInsert(Organization organization)
        {
            string description = "Created organization '" + organization.Name + "'";
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Insert, ReferenceType.Organizations, organization.OrganizationID, description);
        }

        public void LoadByEmail(string email)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT o.*
																FROM Organizations o
																WHERE ((o.ParentID = 1) OR (o.ParentID is null))
																AND o.IsActive = 1
																AND EXISTS(SELECT * FROM Users u WHERE (u.MarkDeleted = 0) AND (u.Email = @Email) AND u.OrganizationID = o.OrganizationID)
																ORDER BY o.Name";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@Email", email);
                Fill(command, "Organizations,Users");
            }
        }

        public void LoadByEmailExcludeInActive(string email)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT o.*
																FROM Organizations o
																WHERE (
																		(o.ParentID = 1)
																		OR (o.ParentID IS NULL)
																		)
																	AND o.IsActive = 1
																	AND EXISTS (
																		SELECT *
																		FROM Users u
																		WHERE (u.MarkDeleted = 0)
																			AND (u.Email = @Email)
																			AND u.OrganizationID = o.OrganizationID
																		)
																ORDER BY o.NAME
																";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@Email", email);
                Fill(command, "Organizations,Users");
            }
        }

        public void LoadTeamSupport()
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Organizations WHERE ParentID is null";
                command.CommandType = CommandType.Text;
                Fill(command);
            }
        }

        public void LoadTeamSupportCustomers()
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Organizations WHERE ParentID = 1";
                command.CommandType = CommandType.Text;
                Fill(command);
            }
        }

        public void LoadBySignUpToken(string token)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Organizations WHERE SignUpToken = @SignUpToken AND ParentID = 1 AND IsValidated = 0";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@SignUpToken", token);
                Fill(command);
            }

        }

        public void LoadByOrganizationName(string name)
        {
            LoadByOrganizationName(name, 1);
        }

        public void LoadByOrganizationName(string name, int parentID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Organizations WHERE Name = @Name AND ParentID = @ParentID ORDER BY Name";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@ParentID", parentID);
                Fill(command);
            }
        }

        public void LoadByOrganizationNameActive(string name, int parentID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Organizations WHERE Name = @Name AND IsActive = 1 AND ParentID = @ParentID ORDER BY Name ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@ParentID", parentID);
                Fill(command);
            }
        }

        public void LoadByLikeOrganizationName(int parentID, string name, bool loadOnlyActive)
        {
            LoadByLikeOrganizationName(parentID, name, loadOnlyActive, int.MaxValue, false);
        }

        public void LoadByLikeOrganizationName(int parentID, string name, bool loadOnlyActive, int maxRows)
        {
            LoadByLikeOrganizationName(parentID, name, loadOnlyActive, maxRows, false);
        }

        public void CustomerLoadByLikeOrganizationName(int parentID, string name, bool loadOnlyActive, int startIndex, bool filterByUserRights)
        {
            User user = Users.GetUser(LoginUser, LoginUser.UserID);
            bool doFilter = filterByUserRights && user.TicketRights == TicketRightType.Customers;

            using (SqlCommand command = new SqlCommand())
            {
                StringBuilder text = new StringBuilder(@"
                WITH orderedrecords as( SELECT *, ROW_NUMBER() OVER (ORDER BY Name) AS 'RowNumber'
                FROM Organizations
                WHERE (ParentID = @ParentID)
                AND (@ActiveOnly = 0 OR IsActive = 1)
                AND (@UseFilter=0 OR (OrganizationID IN (SELECT OrganizationID FROM UserRightsOrganizations WHERE UserID = @UserID)))
                ");
                if (name.Trim() != "")
                {
                    text.Append(" AND ((Name LIKE '%'+@Name+'%') OR (Description LIKE '%'+@Name+'%')) ");
                }

                text.Append(@" ) select * from orderedrecords where RowNumber between @startIndex and @endIndex ORDER BY name");
                command.CommandText = text.ToString();
                command.CommandType = CommandType.Text;

                command.Parameters.AddWithValue("@Name", name.Trim());
                command.Parameters.AddWithValue("@ParentID", parentID);
                command.Parameters.AddWithValue("@startIndex", startIndex + 1);
                command.Parameters.AddWithValue("@endIndex", startIndex + 20);
                command.Parameters.AddWithValue("@ActiveOnly", loadOnlyActive);
                command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
                command.Parameters.AddWithValue("@UseFilter", doFilter);
                Fill(command);
            }
        }

        public void LoadByLikeOrganizationName(int parentID, string name, bool loadOnlyActive, int maxRows, bool filterByUserRights)
        {
            User user = Users.GetUser(LoginUser, LoginUser.UserID);
            bool doFilter = filterByUserRights && user.TicketRights == TicketRightType.Customers;

            using (SqlCommand command = new SqlCommand())
            {
                StringBuilder text = new StringBuilder(@"
SELECT TOP (@MaxRows) *
FROM Organizations
WHERE (ParentID = @ParentID)
AND (@ActiveOnly = 0 OR IsActive = 1)
AND (@UseFilter=0 OR (OrganizationID IN (SELECT OrganizationID FROM UserRightsOrganizations WHERE UserID = @UserID)))
");
                if (name.Trim() != "")
                {
                    text.Append(" AND ((Name LIKE '%'+@Name+'%') OR (Description LIKE '%'+@Name+'%')) ");
                }

                text.Append(" ORDER BY Name ");
                command.CommandText = text.ToString();
                command.CommandType = CommandType.Text;

                command.Parameters.AddWithValue("@Name", name.Trim());
                command.Parameters.AddWithValue("@ParentID", parentID);
                command.Parameters.AddWithValue("@MaxRows", maxRows);
                command.Parameters.AddWithValue("@ActiveOnly", loadOnlyActive);
                command.Parameters.AddWithValue("@UserID", LoginUser.UserID);
                command.Parameters.AddWithValue("@UseFilter", doFilter);
                Fill(command);
            }
        }

        public void LoadByParentID(int parentID, bool loadOnlyActive)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Organizations WHERE (ParentID = @ParentID) AND (@ActiveOnly = 0 OR IsActive = 1) ORDER BY Name ";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ParentID", parentID);
                command.Parameters.AddWithValue("@ActiveOnly", loadOnlyActive);
                Fill(command);
            }
        }

        public void LoadByCRMLinkID(string crmlinkID, int parentID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Organizations WHERE CRMLinkID = @CrmlinkID AND ParentID = @ParentID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@CrmlinkID", crmlinkID);
                command.Parameters.AddWithValue("@ParentID", parentID);
                Fill(command);
            }
        }

        public void LoadNotTicketCustomers(int parentID, int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Organizations o WHERE ((o.ParentID = @ParentID) OR (o.OrganizationID = @ParentID)) AND (o.IsActive = 1) AND NOT EXISTS(SELECT * FROM OrganizationTickets ot WHERE o.OrganizationID = ot.OrganizationID AND ot.TicketID = @TicketID) ORDER BY o.Name";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ParentID", parentID);
                command.Parameters.AddWithValue("@TicketID", ticketID);
                Fill(command);
            }
        }

        public void LoadByMostActive(int parentID, DateTime beginDate, int top)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT TOP " + top.ToString() + @" o.*,
                                  (
                                    SELECT COUNT(*) FROM Tickets t
                                    INNER JOIN OrganizationTickets ot
	                                  ON ot.TicketID = t.TicketID
                                    WHERE (ot.OrganizationID = o.OrganizationID)
	                                  AND (t.DateModified >= @DateModified)
                                  ) AS TicketCount
                                FROM Organizations o
                                WHERE o.ParentID = @ParentID
                                AND o.IsActive = 1
                                ORDER BY TicketCount DESC";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ParentID", parentID);
                command.Parameters.AddWithValue("@DateModified", beginDate);
                Fill(command);
            }
        }

        /// <summary>
        /// Loads ALL the organizations associated with a ticket
        /// </summary>
        /// <param name="ticketID"></param>

        public void LoadByTicketID(int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT o.* FROM Organizations o LEFT JOIN OrganizationTickets ot ON ot.OrganizationID = o.OrganizationID WHERE ot.TicketID = @TicketID AND o.IsActive = 1 ORDER BY o.Name";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                Fill(command, "Organizations,OrganizationTickets");
            }
        }

        public void LoadByTicketIDOrderedByDateCreated(int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT o.* FROM Organizations o LEFT JOIN OrganizationTickets ot ON ot.OrganizationID = o.OrganizationID WHERE ot.TicketID = @TicketID AND o.IsActive = 1 ORDER BY ot.DateCreated";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                Fill(command, "Organizations,OrganizationTickets");
            }
        }

        public void LoadNameAndIdByTicketID(int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT o.OrganizationID, o.Name FROM Organizations o LEFT JOIN OrganizationTickets ot ON ot.OrganizationID = o.OrganizationID WHERE ot.TicketID = @TicketID AND o.IsActive = 1";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                Fill(command, "Organizations,OrganizationTickets");
            }
        }

        public void LoadSentToSalesForce(int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT o.* FROM Organizations o LEFT JOIN OrganizationTickets ot ON ot.OrganizationID = o.OrganizationID WHERE ot.SentToSalesForce = 1 AND ot.TicketID = @TicketID AND o.IsActive = 1 ORDER BY ot.DateCreated";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                Fill(command, "Organizations,OrganizationTickets");
            }
        }

        public void LoadByNeedsIndexing()
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "IndexerQuery";
                command.CommandType = CommandType.StoredProcedure;
                Fill(command);
            }
        }

        public void LoadByNeedsIndexRebuilt(int minutesSinceLastActive, int daysSinceLastRebuild)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText =
        @"  SELECT * FROM Organizations o
    WHERE o.ParentID = 1
    AND o.IsActive = 1
    AND o.IsRebuildingIndex=0
    AND DATEDIFF(day, o.LastIndexRebuilt, GETUTCDATE()) > @DaysOld
    AND ISNULL((SELECT MAX(u.LastActivity) FROM Users u WHERE u.OrganizationID = o.OrganizationID),'1999-01-01 00:00:00.000') < DATEADD(minute, @LastActive, GETUTCDATE())
    ORDER BY o.LastIndexRebuilt ASC";

                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "IndexerQuery_Organizations";

                command.Parameters.AddWithValue("@DaysOld", daysSinceLastRebuild);
                command.Parameters.AddWithValue("@LastActive", (minutesSinceLastActive * -1));
                Fill(command);
            }
        }

        public void LoadBTicketID(int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText =
        @"SELECT o.* FROM Organizations o
LEFT JOIN OrganizationTickets ot ON ot.OrganizationID = o.OrganizationID
WHERE ot.TicketID = @TicketID
ORDER BY o.Name";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                Fill(command, "Organizations,OrganizationTickets");
            }
        }

        /// <summary>
        /// Loads ONLY the organizations associated with a ticket, but not already associated with a specific contact
        /// </summary>
        /// <param name="ticketID"></param>
        public void LoadByNotContactTicketID(int ticketID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText =
        @"SELECT o.* FROM Organizations o
LEFT JOIN OrganizationTickets ot ON ot.OrganizationID = o.OrganizationID
WHERE ot.TicketID = @TicketID
AND ot.OrganizationID NOT IN (SELECT u.OrganizationID FROM UserTickets ut LEFT JOIN Users u ON u.UserID = ut.UserID WHERE TicketID = @TicketID AND u.MarkDeleted=0)
ORDER BY o.Name";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TicketID", ticketID);
                Fill(command, "Organizations,OrganizationTickets");
            }
        }

        public void LoadByWebServiceID(Guid webServiceID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT o.* FROM Organizations o WHERE WebServiceID = @WebServiceID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@WebServiceID", webServiceID);
                Fill(command, "Organizations");
            }

        }

        public void LoadByChatID(Guid chatID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT o.* FROM Organizations o WHERE ChatID = @ChatID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ChatID", chatID);
                Fill(command, "Organizations");
            }

        }

        public void LoadByUserRights(int userID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Organizations o WHERE o.OrganizationID IN (SELECT uro.OrganizationID FROM UserRightsOrganizations uro WHERE uro.UserID=@UserID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                Fill(command, "Organizations");
            }
        }

        public void LoadByUnknownCompany(int parentID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Organizations o WHERE o.ParentID = @ParentID AND o.Name = '_Unknown Company'";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ParentID", parentID);
                Fill(command, "Organizations");
            }

        }

        public void LoadByCustomerInsightsNewOrModifiedByDate(DateTime lastProcessed, int waitBeforeNewUpdate)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT Organizations.*
FROM
	Organizations WITH(NOLOCK)
	JOIN Organizations AS Parent WITH(NOLOCK)
		ON Organizations.parentId = Parent.organizationId
	LEFT JOIN FullContactUpdates WITH(NOLOCK)
		ON Organizations.organizationId = FullContactUpdates.organizationId
WHERE
	Parent.isActive = 1
	AND Parent.IsCustomerInsightsActive = 1
	AND Organizations.parentId != 1
	AND
	(
		--company was recently updated and not recently processed (within @waitBeforeNewUpdate)
		(
			(
				Organizations.dateCreated > @lastProcessed
				OR Organizations.dateModified > @lastProcessed
			)
			AND FullContactUpdates.id IS NULL
		)
		--company was not processed last time it was updated because of previous restriction (see above) but @waitBeforeNewUpdate has passed already
		OR
		(
			(
				Organizations.dateCreated > @lastProcessed
				OR Organizations.dateModified > @lastProcessed
			)
			AND FullContactUpdates.id IS NOT NULL
			AND FullContactUpdates.dateModified <= DATEADD(HOUR, @waitBeforeNewUpdate * -1, @lastProcessed)
		)
		--updated, skipped because at that time was processed recently, but now it has been more than @waitBeforeNewUpdate
		OR
		(
			(
				Organizations.dateCreated > DATEADD(HOUR, @waitBeforeNewUpdate * -1, @lastProcessed)
				OR Organizations.dateModified > DATEADD(HOUR, @waitBeforeNewUpdate * -1, @lastProcessed)
			)
			AND FullContactUpdates.id IS NOT NULL
			AND FullContactUpdates.dateModified <= DATEADD(HOUR, @waitBeforeNewUpdate * -1, @lastProcessed)
		)
	)
ORDER BY Organizations.dateModified";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@lastProcessed", lastProcessed);
                command.Parameters.AddWithValue("@waitBeforeNewUpdate", waitBeforeNewUpdate);

                Fill(command);
            }
        }

        public void LoadByCustomerInsightsByCompanyTotalTickets(int waitBeforeNewUpdate, int? top = null)
        {
            using (SqlCommand command = new SqlCommand())
            {
                string sql = @"SELECT {0} Organizations.*
FROM
	Organizations WITH(NOLOCK)
	JOIN (
		SELECT COUNT(1) AS Total, OrganizationTickets.organizationId
		FROM
			TicketsView WITH(NOLOCK)
			JOIN OrganizationTickets WITH(NOLOCK)
				ON TicketsView.ticketId = OrganizationTickets.ticketId
		WHERE
			TicketsView.isClosed = 0
		GROUP BY
			OrganizationTickets.organizationId
		) AS TicketCount
		ON Organizations.organizationid = TicketCount.organizationId
  LEFT JOIN FullContactUpdates WITH(NOLOCK)
		ON Organizations.organizationId = FullContactUpdates.organizationId
  JOIN Organizations AS Parent WITH(NOLOCK)
		ON Organizations.parentId = Parent.organizationId
WHERE
  Parent.IsCustomerInsightsActive = 1
  AND (FullContactUpdates.id IS NULL
  OR DATEADD(HOUR, @waitBeforeNewUpdate, FullContactUpdates.dateModified) < GETDATE())
ORDER BY
	TicketCount.Total DESC,
	Organizations.name";
                command.CommandText = string.Format(sql, top == null ? "" : "TOP " + top.ToString());
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@waitBeforeNewUpdate", waitBeforeNewUpdate);

                Fill(command);
            }
        }

        public static int GetChatCount(LoginUser loginUser, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM Users u WHERE (u.IsActive = 1) AND (u.MarkDeleted = 0) AND (u.OrganizationID = @OrganizationID) AND (u.IsChatUser = 1)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);

                Organizations organizations = new Organizations(loginUser);
                return (int)organizations.ExecuteScalar(command, "Users");
            }
        }

        public static int GetUserCount(LoginUser loginUser, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM Users u WHERE (u.IsActive = 1) AND (u.MarkDeleted = 0) AND (u.OrganizationID = @OrganizationID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);

                Organizations organizations = new Organizations(loginUser);
                return (int)organizations.ExecuteScalar(command, "Users");
            }
        }

        public static int GetPortalCount(LoginUser loginUser, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"SELECT COUNT(*) FROM Organizations o
                                WHERE (o.IsActive = 1) AND (o.ParentID = @ParentID) AND (HasPortalAccess = 1)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ParentID", organizationID);

                Organizations organizations = new Organizations(loginUser);
                return (int)organizations.ExecuteScalar(command, "Organizations");
            }
        }

        public static int GetStorageUsed(LoginUser loginUser, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT SUM(a.FileSize) FROM Attachments a WHERE (a.OrganizationID = @OrganizationID)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);

                Organizations organizations = new Organizations(loginUser);
                object o = organizations.ExecuteScalar(command, "Attachments");
                if (o == DBNull.Value)
                {
                    return 0;
                }
                else
                {
                    long value = (long)o;
                    return (int)(value / 1024 / 1024);
                }


            }
        }

        public static int GetExtraStorageAllowed(LoginUser loginUser, int organizationID)
        {
            Organization organization = (Organization)Organizations.GetOrganization(loginUser, organizationID);
            if (organization == null) return 0;
            return organization.ExtraStorageUnits * 500;
        }

        public static bool GetIsParent(LoginUser loginUser, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM dbo.GetCompanyFamilyIDs(@OrganizationID, 1)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);

                Organizations organizations = new Organizations(loginUser);
                return (int)organizations.ExecuteScalar(command, "CustomerRelationships") > 1;
            }
        }

        public static int GetBaseStorageAllowed(LoginUser loginUser, int organizationID)
        {

            Organization organization = Organizations.GetOrganization(loginUser, organizationID);
            if (organization == null) return 0;
            int result = 0;


            switch (organization.ProductType)
            {
                case ProductType.Express:
                    result = organization.UserSeats * 75;
                    break;
                case ProductType.HelpDesk:
                case ProductType.BugTracking:
                    result = organization.UserSeats * 150;
                    break;
                case ProductType.Enterprise:
                    result = organization.UserSeats * 250;
                    break;
                default:
                    break;
            }

            return result;
        }

        public static int GetTotalStorageAllowed(LoginUser loginUser, int organizationID)
        {
            return GetExtraStorageAllowed(loginUser, organizationID) + GetBaseStorageAllowed(loginUser, organizationID);
        }

        public static void DeleteOrganizationAndAllReleatedData(LoginUser loginUser, int organizationID)
        {
            Invoices invoices = new Invoices(loginUser);
            invoices.LoadByOrganizationID(organizationID);

            if (invoices.IsEmpty) DeleteOrganizations(loginUser, organizationID);

        }

        private static void DeleteOrganizations(LoginUser loginUser, int organizationID)
        {
            User user = (User)Users.GetUser(loginUser, loginUser.UserID);
            Organizations organizations = new Organizations(loginUser);
            organizations.LoadByParentID(organizationID, false);
            foreach (Organization organization in organizations)
            {
                DeleteOrganizationAndAllReleatedData(loginUser, organization.OrganizationID);
            }
            if (user != null)
                ActionLogs.AddActionLog(loginUser, ActionLogType.Delete, ReferenceType.Organizations, organizationID, "Organization '" + ((Organization)Organizations.GetOrganization(loginUser, organizationID)).Name + "' was deleted by '" + user.FirstLastName + "'");
            DeleteOrganizationItems(loginUser, organizationID);

            Organization org = (Organization)Organizations.GetOrganization(loginUser, organizationID);
            org.Delete();
            org.Collection.Save();
        }

        private static void DeleteOrganizationItems(LoginUser loginUser, int organizationID)
        {
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);

                command.CommandText = "DELETE FROM OrganizationProducts WHERE OrganizationID = @OrganizationID";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM OrganizationTickets WHERE OrganizationID = @OrganizationID";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM Tickets WHERE OrganizationID = @OrganizationID";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM BillingInfo WHERE OrganizationID = @OrganizationID";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM CreditCards WHERE OrganizationID = @OrganizationID";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM RecentlyViewedItems WHERE (refID = @OrganizationID) AND (refType = 1)";
                command.ExecuteNonQuery();

            }

            DeleteAttachments(loginUser, organizationID);
        }

        public void ResetDefaultSupportUser(LoginUser loginUser, int userID)
        {
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);

                command.CommandText = "Update Organizations Set DefaultSupportUserID=null Where DefaultSupportUserID=@UserID";
                command.ExecuteNonQuery();

            }
        }

        private static void DeleteAttachments(LoginUser loginUser, int organizationID)
        {
            FilePaths filePaths = new FilePaths(loginUser);
            filePaths.LoadThemAll();
            foreach (FilePath filePath in filePaths)
            {
                string path = Path.Combine(Path.Combine(filePath.Value, "Organizations"), organizationID.ToString());
                if (Directory.Exists(path))
                {
                    try
                    {
                        Directory.Delete(path, true);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        public static void SetRebuildIndexes(LoginUser loginUser, int organizationID)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "UPDATE Organizations SET LastIndexRebuilt = '01/01/2000' WHERE OrganizationID = @OrganizationID";
            command.Parameters.AddWithValue("OrganizationID", organizationID);
            SqlExecutor.ExecuteNonQuery(loginUser, command);
        }

        public static void SetAllPortalUsers(LoginUser loginUser, int organizationID, bool sendEmails, bool isHub)
        {
            Users users = new Users(loginUser);
            users.LoadContacts(organizationID, true);
            Random random = new Random();
            foreach (User user in users)
            {
                if (!user.IsPortalUser)
                {
                    user.IsPortalUser = true;
                    if (sendEmails)
                    {
                        string password = DataUtils.GenerateRandomPassword(random);
                        user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
                        user.IsPasswordExpired = true;
                        if (isHub) EmailPosts.SendWelcomeCustomerHubUser(loginUser, user.UserID, password);
                        else EmailPosts.SendWelcomePortalUser(loginUser, user.UserID, password);
                    }
                    user.Collection.Save();
                }
            }

            SqlCommand command = new SqlCommand();
            command.CommandText = "UPDATE Organizations SET HasPortalAccess = 1 WHERE ParentID = @OrganizationID";
            command.Parameters.AddWithValue("OrganizationID", organizationID);
            SqlExecutor.ExecuteNonQuery(loginUser, command);
            /*
                  //EmailPosts.SendWelcomePortalUser(TSAuthentication.GetLoginUser(), user.UserID, password);

                  command = new SqlCommand();
                  command.CommandText = "UPDATE Users SET IsPortalUser = @Value WHERE OrganizationID IN (SELECT OrganizationID FROM Organizations WHERE ParentID=@OrganizationID)";
                  command.Parameters.AddWithValue("OrganizationID", organizationID);
                  command.Parameters.AddWithValue("Value", value ? 1 : 0);
                  SqlExecutor.ExecuteNonQuery(loginUser, command);*/
        }

        public Organization FindOnlyByImportID(string importID)
        {
            importID = importID.ToLower().Trim();
            foreach (Organization organization in this)
            {
                if (organization.ImportID != null && organization.ImportID.Trim().ToLower() == importID)
                {
                    return organization;
                }
            }
            return null;
        }

        public Organization FindByImportID(string importID)
        {
            importID = importID.ToLower().Trim();
            foreach (Organization organization in this)
            {
                if ((organization.ImportID != null && organization.ImportID.Trim().ToLower() == importID) || organization.Name.ToLower().Trim() == importID)
                {
                    return organization;
                }
            }
            return null;
        }

        public Organization FindByName(string name)
        {
            foreach (Organization organization in this)
            {
                if (organization.Name.ToLower().Trim() == name.ToLower().Trim())
                {
                    return organization;
                }
            }
            return null;
        }

        public Organization FindByCRMLinkID(string crmLinkID)
        {
            foreach (Organization organization in this)
            {
                if (organization.CRMLinkID.ToLower().Trim() == crmLinkID.ToLower().Trim())
                {
                    return organization;
                }
            }
            return null;
        }

        public static int GetUnknownCompanyID(LoginUser loginUser)
        {
            return GetUnknownCompanyID(loginUser, loginUser.OrganizationID);
        }

        public static int GetUnknownCompanyID(LoginUser loginUser, int organizationID)
        {
            Organizations organizations = new Organizations(loginUser);
            organizations.LoadByUnknownCompany(organizationID);
            if (organizations.IsEmpty)
            {
                organizations = new Organizations(loginUser);
                Organization newUnknownCompany = organizations.AddNewOrganization();
                newUnknownCompany.Name = "_Unknown Company";
                newUnknownCompany.IsActive = true;
                newUnknownCompany.ParentID = organizationID;
                organizations.Save();
            }
            return organizations[0].OrganizationID;
        }

        public static Organization GetUnknownCompany(LoginUser loginUser, int organizationID)
        {
            Organizations organizations = new Organizations(loginUser);
            organizations.LoadByUnknownCompany(organizationID);
            if (organizations.IsEmpty)
            {
                organizations = new Organizations(loginUser);
                Organization newUnknownCompany = organizations.AddNewOrganization();
                newUnknownCompany.Name = "_Unknown Company";
                newUnknownCompany.IsActive = true;
                newUnknownCompany.ParentID = organizationID;
                organizations.Save();
            }
            return organizations[0];
        }


        public void LoadFirstDomainMatch(int parentID, string domain)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT TOP 1 * FROM Organizations o WHERE o.ParentID = @ParentID AND o.CompanyDomains LIKE @Domain ORDER BY LEN(o.CompanyDomains)";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ParentID", parentID);
                command.Parameters.AddWithValue("@Domain", "%" + domain + "%");
                Fill(command, "Organizations");
            }

        }

        public void LoadByImportID(string importID, int parentID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Organizations WHERE ImportID = @ImportID AND ParentID = @ParentID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ImportID", importID);
                command.Parameters.AddWithValue("@ParentID", parentID);
                Fill(command);
            }
        }

        public void LoadByName(string name, int parentID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM Organizations WHERE ParentID = @ParentID AND Name = @Name";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@ParentID", parentID);
                Fill(command);
            }
        }

        public void MergeUpdateContacts(int losingOrganizationID, int winningOrganizationID, string companyName, LoginUser loginUser)
        {
            Users losingOrganizationContacts = new Users(loginUser);
            losingOrganizationContacts.LoadByOrganizationID(losingOrganizationID, true);
            if (losingOrganizationContacts.Count > 0)
            {
                Users winningOrganizationContacts = new Users(loginUser);
                winningOrganizationContacts.LoadByOrganizationID(winningOrganizationID, false);
                foreach (User losingOrganizationContact in losingOrganizationContacts)
                {
                    if (losingOrganizationContact.Email != string.Empty)
                    {
                        User winningOrganizationMatchingContact = winningOrganizationContacts.FindByEmail(losingOrganizationContact.Email);
                        if (winningOrganizationMatchingContact != null)
                        {
                            //As is, if error on merging files on any of the contacts happen.  It is being logged, but process continues
                            string mergeContactErrLocation = winningOrganizationMatchingContact.Collection.MergeContactsFiles(winningOrganizationMatchingContact, losingOrganizationContact, loginUser);
                            try
                            {
                                using (SqlCommand command = new SqlCommand())
                                {
                                    command.CommandText = "Org_MergeContacts_UpdatesSingle";
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.AddWithValue("@WinOrgID", winningOrganizationID);
                                    command.Parameters.AddWithValue("@WinContactId", winningOrganizationMatchingContact.UserID);
                                    command.Parameters.AddWithValue("@LooseContactID", losingOrganizationContact.UserID);
                                    command.Parameters.AddWithValue("@LoginUserID", loginUser.UserID);
                                    ExecuteNonQuery(command);

                                    string description = "Merged '" + companyName + "' contacts.";
                                    ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Organizations, winningOrganizationID, description);
                                    ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Contacts, winningOrganizationID, description);
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionLog log = (new ExceptionLogs(loginUser)).AddNewExceptionLog();
                                log.ExceptionName = "Merge Update Contact Exception " + e.Source;
                                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                                log.Collection.Save();

                                throw;
                            }
                        }
                    }
                }

                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = @"
			         UPDATE	Users SET OrganizationID = @winningOrganizationID, NeedsIndexing = 1 WHERE OrganizationID = @losingOrganizationID";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@winningOrganizationID", winningOrganizationID);
                    command.Parameters.AddWithValue("@losingOrganizationID", losingOrganizationID);
                    ExecuteNonQuery(command, "OrganizationContacts");
                }
            }
        }

        public void MergeUpdateTickets(int losingOrganizationID, int winningOrganizationID, string companyName, LoginUser loginUser)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
			 UPDATE
				l
			 SET
				l.OrganizationID = @winningOrganizationID
			FROM
				OrganizationTickets l
				LEFT JOIN OrganizationTickets w
					ON l.TicketID = w.TicketID
					AND w.OrganizationID = @winningOrganizationID
			WHERE
				l.OrganizationID = @losingOrganizationID
				AND w.TicketID IS NULL";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@winningOrganizationID", winningOrganizationID);
                command.Parameters.AddWithValue("@losingOrganizationID", losingOrganizationID);
                ExecuteNonQuery(command, "OrganizationTickets");
            }
            string description = "Merged '" + companyName + "' tickets.";
            ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Organizations, winningOrganizationID, description);
        }

        public void MergeUpdateNotes(int losingOrganizationID, int winningOrganizationID, string companyName, LoginUser loginUser)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
			 UPDATE
				Notes
			 SET
				RefID = @winningOrganizationID
				, NeedsIndexing = 1
			 WHERE
				RefID = @losingOrganizationID
				AND RefType = 9";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@winningOrganizationID", winningOrganizationID);
                command.Parameters.AddWithValue("@losingOrganizationID", losingOrganizationID);
                ExecuteNonQuery(command, "Notes");
            }
            string description = "Merged '" + companyName + "' Notes.";
            ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Organizations, winningOrganizationID, description);
        }

        public void MergeUpdateFiles(int losingOrganizationID, int winningOrganizationID, string companyName, LoginUser loginUser)
        {
            Attachments attachments = new Attachments(loginUser);
            attachments.LoadByReference(ReferenceType.Organizations, losingOrganizationID);
            if (attachments.Count > 0)
            {
                string pathWithoutFileName = System.IO.Path.GetDirectoryName(attachments[0].Path);
                string losingOrganizationFolderName = @"\" + losingOrganizationID.ToString();
                string winningOrganizationFolderName = @"\" + winningOrganizationID.ToString();
                string newPath = pathWithoutFileName.Replace(losingOrganizationFolderName, winningOrganizationFolderName);

                foreach (Attachment attachment in attachments)
                {
                    if (!Directory.Exists(newPath)) Directory.CreateDirectory(newPath);
                    string newFileName = DataUtils.VerifyUniqueUrlFileName(newPath, attachment.FileName);
                    string newFullPath = Path.Combine(newPath, newFileName);
                    System.IO.File.Copy(attachment.Path, newFullPath, true);
                    System.IO.File.Delete(attachment.Path);

                    attachment.FileName = newFileName;
                    attachment.Path = newFullPath;
                    attachment.RefID = winningOrganizationID;
                }

                System.IO.Directory.Delete(pathWithoutFileName);
                attachments.Save();
                string description = "Merged '" + companyName + "' Files.";
                ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Organizations, winningOrganizationID, description);
            }
        }

        public void MergeUpdateProducts(int losingOrganizationID, int winningOrganizationID, string companyName, LoginUser loginUser)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
			 UPDATE
				l
			 SET
				l.OrganizationID = @winningOrganizationID
			 FROM
				OrganizationProducts l
				LEFT JOIN OrganizationProducts w
					ON l.ProductID = w.ProductID
					AND
					(
						(l.ProductVersionID IS NULL AND w.ProductVersionID IS NULL)
						OR	l.ProductVersionID = w.ProductVersionID
					)
					AND w.OrganizationID = @winningOrganizationID
			 WHERE
				l.OrganizationID = @losingOrganizationID
				AND w.ProductID IS NULL";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@winningOrganizationID", winningOrganizationID);
                command.Parameters.AddWithValue("@losingOrganizationID", losingOrganizationID);
                ExecuteNonQuery(command, "OrganizationProducts");
            }
            string description = "Merged '" + companyName + "' Products.";
            ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Organizations, winningOrganizationID, description);
        }

        public void MergeUpdateAssets(int losingOrganizationID, int winningOrganizationID, string companyName, LoginUser loginUser)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
			 UPDATE
				AssetHistory
			 SET
				ShippedTo = @winningOrganizationID
			 WHERE
				ShippedTo = @losingOrganizationID
				AND RefType = 9
				AND OrganizationID = @parentOrganizationID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@winningOrganizationID", winningOrganizationID);
                command.Parameters.AddWithValue("@losingOrganizationID", losingOrganizationID);
                command.Parameters.AddWithValue("@parentOrganizationID", loginUser.OrganizationID);
                ExecuteNonQuery(command, "AssetHistory");
            }
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
			 UPDATE
				AssetHistory
			 SET
				ShippedFrom = @winningOrganizationID
			 WHERE
				ShippedFrom = @losingOrganizationID
				AND RefType = 9
				AND OrganizationID = @parentOrganizationID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@winningOrganizationID", winningOrganizationID);
                command.Parameters.AddWithValue("@losingOrganizationID", losingOrganizationID);
                command.Parameters.AddWithValue("@parentOrganizationID", loginUser.OrganizationID);
                ExecuteNonQuery(command, "AssetHistory");
            }
            string description = "Merged '" + companyName + "' Assets.";
            ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Organizations, winningOrganizationID, description);
        }

        public void MergeUpdateWaterCoolerMessages(int losingOrganizationID, int winningOrganizationID, string companyName, LoginUser loginUser)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
			 UPDATE
				l
			 SET
				l.AttachmentID = @winningOrganizationID
			 FROM
				WatercoolerAttachments l
				LEFT JOIN WatercoolerAttachments w
					ON l.MessageID = w.MessageID
					AND w.AttachmentID = @winningOrganizationID
			 WHERE
				l.AttachmentID = @losingOrganizationID
				AND l.RefType = 2
				AND w.MessageID IS NULL";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@winningOrganizationID", winningOrganizationID);
                command.Parameters.AddWithValue("@losingOrganizationID", losingOrganizationID);
                ExecuteNonQuery(command, "WatercoolerAttachments");
            }
            string description = "Merged '" + companyName + "' WaterCoolerMessages.";
            ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Organizations, winningOrganizationID, description);
        }

        public void MergeUpdateRatings(int losingOrganizationID, int winningOrganizationID, string companyName, LoginUser loginUser)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
			 UPDATE
				AgentRatings
			 SET
				CompanyID = @winningOrganizationID
			 WHERE
				CompanyID = @losingOrganizationID
				AND OrganizationID = @parentOrganizationID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@winningOrganizationID", winningOrganizationID);
                command.Parameters.AddWithValue("@losingOrganizationID", losingOrganizationID);
                command.Parameters.AddWithValue("@parentOrganizationID", loginUser.OrganizationID);
                ExecuteNonQuery(command, "AgentRatings");
            }
            string description = "Merged '" + companyName + "' AgentRatings.";
            ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Organizations, winningOrganizationID, description);
        }

        public void MergeUpdateCalendar(int losingOrganizationID, int winningOrganizationID, string companyName, LoginUser loginUser)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
			 UPDATE
				l
			 SET
				l.RefID = @winningOrganizationID
			 FROM
				CalendarRef l
				LEFT JOIN CalendarRef w
					ON l.CalendarID = w.CalendarID
					AND w.RefID = @winningOrganizationID
			 WHERE
				l.RefID = @losingOrganizationID
				AND l.RefType = 2
				AND w.CalendarID IS NULL";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@winningOrganizationID", winningOrganizationID);
                command.Parameters.AddWithValue("@losingOrganizationID", losingOrganizationID);
                ExecuteNonQuery(command, "CalendarRef");
            }
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
			 DELETE
				CalendarRef
			 WHERE
				RefID = @losingOrganizationID
				AND RefType = 2";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@winningOrganizationID", winningOrganizationID);
                command.Parameters.AddWithValue("@losingOrganizationID", losingOrganizationID);
                ExecuteNonQuery(command, "CalendarRef");
            }
            string description = "Merged '" + companyName + "' CalendarEvents.";
            //ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Organizations, winningOrganizationID, description);
        }

        public void MergeUpdateCustomValues(int losingOrganizationID, int winningOrganizationID, string companyName, LoginUser loginUser)
        {
            // This was best solution but is failing with Subquery returned more than 1 value. This is not permitted when the subquery follows =, !=, <, <= , >, >= or when the subquery is used as an expression.
            // probably trigger designed with single record update at a time.
            // we'll need to implement solution on .net
            //		 using (SqlCommand command = new SqlCommand())
            //		 {
            //			 command.CommandText = @"
            //				UPDATE
            //					cvl
            //				SET
            //					cvl.RefID = @WinnerCompanyID
            //				FROM
            //					CustomValues cvl
            //					LEFT JOIN CustomValues cvw
            //						ON cvl.CustomFieldID = cvw.CustomFieldID
            //						AND cvw.RefID = @WinnerCompanyID
            //					JOIN CustomFields cf
            //						ON cvl.CustomFieldID = cf.CustomFieldID
            //				WHERE
            //					cf.OrganizationID = @ParentOrganizationID
            //					AND cf.RefType = 9
            //					AND cvl.RefID = @LossingCompanyID
            //					AND cvw.CustomValueID IS NULL";
            //			 command.CommandType = CommandType.Text;
            //			 command.Parameters.AddWithValue("@winningOrganizationID", winningOrganizationID);
            //			 command.Parameters.AddWithValue("@losingOrganizationID", losingOrganizationID);
            //			 command.Parameters.AddWithValue("@ParentOrganizationID", loginUser.OrganizationID);
            //			 ExecuteNonQuery(command, "CustomValues");
            //		 }
            //		 using (SqlCommand command = new SqlCommand())
            //		 {
            //			 command.CommandText = @"
            //				UPDATE
            //					cvw
            //				SET
            //					cvw.CustomValue = cvl.CustomValue
            //				FROM
            //					CustomValues cvl
            //					LEFT JOIN CustomValues cvw
            //						ON cvl.CustomFieldID = cvw.CustomFieldID
            //						AND cvw.RefID = @WinnerCompanyID
            //					JOIN CustomFields cf
            //						ON cvl.CustomFieldID = cf.CustomFieldID
            //				WHERE
            //					cf.OrganizationID = @ParentOrganizationID
            //					AND cf.RefType = 9
            //					AND cvl.RefID = @LossingCompanyID
            //					AND LTRIM(RTRIM(cvw.CustomValue)) = ''";
            //			 command.CommandType = CommandType.Text;
            //			 command.Parameters.AddWithValue("@winningOrganizationID", winningOrganizationID);
            //			 command.Parameters.AddWithValue("@losingOrganizationID", losingOrganizationID);
            //			 command.Parameters.AddWithValue("@ParentOrganizationID", loginUser.OrganizationID);
            //			 ExecuteNonQuery(command, "CustomValues");
            //		 }
            CustomValues loosingCompanyCustomValues = new CustomValues(loginUser);
            loosingCompanyCustomValues.LoadExistingOnlyByReferenceType(loginUser.OrganizationID, ReferenceType.Organizations, losingOrganizationID);
            if (loosingCompanyCustomValues.Count > 0)
            {
                CustomValues winningCompanyCustomValues = new CustomValues(loginUser);
                winningCompanyCustomValues.LoadExistingOnlyByReferenceType(loginUser.OrganizationID, ReferenceType.Organizations, winningOrganizationID);
                bool updateExistingCustomValues = false;
                bool saveNewCustomValues = false;
                CustomValues newCustomValues = new CustomValues(loginUser);
                foreach (CustomValue loosingCompanyCustomValue in loosingCompanyCustomValues)
                {
                    if (loosingCompanyCustomValue.Value.Trim() != string.Empty)
                    {
                        CustomValue winningCompanyCustomValue = winningCompanyCustomValues.FindByCustomFieldID(loosingCompanyCustomValue.CustomFieldID);
                        if (winningCompanyCustomValue != null)
                        {
                            if (winningCompanyCustomValue.Value.Trim() == string.Empty)
                            {
                                winningCompanyCustomValue.Value = loosingCompanyCustomValue.Value;
                                updateExistingCustomValues = true;
                            }
                        }
                        else
                        {
                            CustomValue newCustomValue = newCustomValues.AddNewCustomValue();
                            newCustomValue.CustomFieldID = loosingCompanyCustomValue.CustomFieldID;
                            newCustomValue.RefID = winningOrganizationID;
                            newCustomValue.Value = loosingCompanyCustomValue.Value;
                            newCustomValue.CreatorID = loosingCompanyCustomValue.CreatorID;
                            newCustomValue.ModifierID = loosingCompanyCustomValue.ModifierID;
							newCustomValue.OrganizationID = loginUser.OrganizationID;
                            saveNewCustomValues = true;
                        }
                    }
                }
                if (updateExistingCustomValues)
                {
                    winningCompanyCustomValues.Save();
                }
                if (saveNewCustomValues)
                {
                    newCustomValues.Save();
                }
            }

            string description = "Merged '" + companyName + "' CustomValues.";
            ActionLogs.AddActionLog(loginUser, ActionLogType.Update, ReferenceType.Organizations, winningOrganizationID, description);
        }

        public void DeleteRecentlyViewItems(int losingOrganizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
			 DELETE
				RecentlyViewedItems
			 WHERE
				RefID = @losingOrganizationID
				AND RefType = 1";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@losingOrganizationID", losingOrganizationID);
                ExecuteNonQuery(command, "RecentlyViewedItems");
            }
        }

        public static int GetFamilyAverageCDI(LoginUser loginUser, int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
        SELECT
            AVG(CustDisIndex)
        FROM
            Organizations o
        WHERE
            o.OrganizationID IN
            (
                SELECT
                    @OrganizationID
                UNION
                SELECT
                    CustomerID
                FROM
                    CustomerRelationships
                WHERE
                    RelatedCustomerID = @OrganizationID
            )";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);

                Organizations organizations = new Organizations(loginUser);
                return (int)organizations.ExecuteScalar(command, "Organizations");
            }
        }

        public void MigrateOrgType(LoginUser loginUser, int organizationID, ProductType productType)
        {
            using (SqlCommand command = new SqlCommand())
            {
                if (productType == ProductType.Enterprise)
                {
                    command.CommandText = @"
                        UPDATE
				            OrganizationSettings
                        SET
                            SettingValue = REPLACE(SettingValue, '""CatID"":""Reminders"",""CatName"":""Reminders""', '""CatID"":""Tasks"",""CatName"":""Tasks""')
                        WHERE
				            OrganizationID = @organizationID
                            AND SettingKey = 'TicketFieldsOrder'

                        UPDATE
				            OrganizationSettings
                        SET
                            SettingValue = REPLACE(SettingValue, ',{""CatID"":""Reminders"",""CatName"":""Reminders"",""Disabled"":""false""}', '')
                        WHERE
				            OrganizationID = @organizationID
                            AND SettingKey = 'NewTicketFieldsOrder'

                        INSERT INTO
	                        Tasks
	                        (
		                        OrganizationID,
		                        Name,
		                        Description,
		                        DueDate,
		                        UserID,
		                        IsComplete,
		                        DateCompleted,
		                        CreatorID,
		                        DateCreated,
		                        ReminderID,
		                        ModifierID,
		                        DateModified
	                        )
                        SELECT
	                        r.OrganizationID,
	                        r.Description,
	                        r.Description,
	                        r.DueDate,
	                        r.UserID,
	                        CASE
		                        WHEN r.IsDismissed = 1 THEN 1
		                        WHEN r.HasEmailSent = 1 THEN 1
		                        ELSE 0
	                        END AS BIT,
	                        r.DueDate,
	                        r.CreatorID,
	                        r.DateCreated,
	                        r.ReminderID,
	                        -5,
	                        GETUTCDATE()
                        FROM
	                        Reminders r
	                        JOIN Organizations o
		                        ON r.OrganizationID = o.OrganizationID
	                        LEFT JOIN Tasks t
		                        ON r.ReminderID = t.ReminderID
                        WHERE
                            r.OrganizationID = @organizationID
                            AND r.RefType NOT IN (59,60,61)
	                        AND t.TaskID IS NULL

                        INSERT INTO
	                        TaskAssociations
	                        (
		                        TaskID,
		                        RefID,
		                        RefType,
		                        CreatorID,
		                        DateCreated
	                        )
                        SELECT
	                        t.TaskID,
	                        r.RefID,
	                        r.RefType,
	                        r.CreatorID,
	                        r.DateCreated
                        FROM
	                        Reminders r
	                        JOIN Organizations o
		                        ON r.OrganizationID = o.OrganizationID
	                        JOIN Tasks t
		                        ON r.ReminderID = t.ReminderID
	                        LEFT JOIN TaskAssociations ta
		                        ON t.TaskID = ta.TaskID
                        WHERE
                            r.OrganizationID = @organizationID
                            AND r.RefType NOT IN (59,60,61)
	                        AND ta.TaskID IS NULL

                        UPDATE
	                        u
                        SET
	                        u.MenuItems = u.MenuItems + ',mniTasks'
                        FROM
	                        Users u
                        WHERE
	                        u.OrganizationID = @organizationID
	                        AND u.MenuItems IS NOT NULL
                    ";
                }
                else
                {
                    command.CommandText = @"
                        UPDATE
				            OrganizationSettings
                        SET
                            SettingValue = REPLACE(SettingValue, '""CatID"":""Tasks"",""CatName"":""Tasks""', '""CatID"":""Reminders"",""CatName"":""Reminders""')
                        WHERE
				            OrganizationID = @organizationID
                            AND SettingKey = 'TicketFieldsOrder'

                        UPDATE
				            OrganizationSettings
                        SET
                            SettingValue = REPLACE(SettingValue, ']', ',{""CatID"":""Reminders"",""CatName"":""Reminders"",""Disabled"":""false""}]')
                        WHERE
				            OrganizationID = @organizationID
                            AND SettingKey = 'NewTicketFieldsOrder'

                        UPDATE
	                        u
                        SET
	                        u.MenuItems = REPLACE(u.MenuItems, ',mniTasks', '')
                        FROM
	                        Users u
                        WHERE
	                        u.OrganizationID = @organizationID
	                        AND u.MenuItems IS NOT NULL
                    ";
                }
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@organizationID", organizationID);
                ExecuteNonQuery(command, "OrganizationSettings");
            }
        }


        public void LoadByTaskID(int taskID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = @"
                    SELECT
                        o.*
                    FROM
                        Organizations o
                        JOIN TaskAssociations ta
                            ON ta.RefType = 9
                            AND ta.RefID = o.OrganizationID
                    WHERE
                        ta.TaskID = @TaskID
                    ORDER BY
                        o.Name";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@TaskID", taskID);
                Fill(command, "Organizations,OrganizationTickets");
            }
        }

        public void CompanyMergeSproc(int losingOrganizationID, int winningOrganizationID, string companyName, LoginUser loginUser)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "Org_MergeCompanies_Updates";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@WinOrgID", winningOrganizationID);
                command.Parameters.AddWithValue("@LooseOrgID", losingOrganizationID);
                command.CommandTimeout = 120;
                ExecuteNonQuery(command);
            }
        }
    }
}
