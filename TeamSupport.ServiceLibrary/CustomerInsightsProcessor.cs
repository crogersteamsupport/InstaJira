using System;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  public class CustomerInsightsProcessor : ServiceThread
  {
    const   string _baseURI                       = "https://api.fullcontact.com/v2/";
    const   string _maxContactApiCallsKey         = "CustomerInsights Max Contact Calls";
    const   string _maxCompanyApiCallsKey         = "CustomerInsights Max Company Calls";
    const   string _currentContactApiCallsKey     = "CustomerInsights Current Contact Calls";
    const   string _currentCompanyApiCallsKey     = "CustomerInsights Current Company Calls";
    const   string _intervalKey                   = "CustomerInsights Process Interval";
    const   string _lastProcessedKey              = "CustomerInsights Last Processed";
    const   string _waitBeforeNewUpdateKey        = "CustomerInsights Wait Hours Before New Update";
    const   string _maxToProcessByTicketCountKey  = "CustomerInsights Max To Process By Ticket Count";
    const   string _securityTokenKey              = "CustomerInsights SecurityToken";

    private string    _securityToken              = "563e14813921adae";
    private DateTime  _lastProcessed              = DateTime.MinValue;
    private int       _maxContactApiCalls         = 0;
    private int       _maxCompanyApiCalls         = 0;
    private int       _currentContactApiCalls     = 0;
    private int       _currentCompanyApiCalls     = 0;
    private int       _waitBeforeNewUpdate        = 0;
    private int       _maxToProcessByTicketCount  = 0;
	private bool	_noMoreRemainingContactCalls = false;
	private bool	_noMoreRemainingCompanyCalls = false;

    public CustomerInsightsProcessor()
    {
        if (Environment.UserInteractive)
            Run();
    }

    public override void Run()
    {
      try
      {
        UpdateHealth();
        if (Settings.ReadBool("CustomerInsights Enabled", true))
        {
          InitializeGlobalVariables();
          int customerInsightsInterval = Settings.ReadInt(_intervalKey, 60);

          if (_lastProcessed.AddMinutes(customerInsightsInterval) < DateTime.UtcNow)
          {
			SetValuesFromAccountStats();
            ProcessCustomerInsights();
            ResetCurrentApiCalls();
            Settings.WriteString(_lastProcessedKey, DateTime.UtcNow.ToString());
          }
        }
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "CustomerInsightsProcessor");
      }
    }

    private void ResetCurrentApiCalls()
    {
      // we need to check if month changed, to reset the counter
      DateTime  today               = DateTime.UtcNow;
      int       lastProcessedMonth  = _lastProcessed.Month;
      int       todayMonth          = today.Month;
      bool      monthChange         = lastProcessedMonth != todayMonth;

      if (monthChange)
      {
        _currentCompanyApiCalls = 0;
        _currentContactApiCalls = 0;
        Settings.WriteInt(_currentCompanyApiCallsKey, 1);
        Settings.WriteInt(_currentContactApiCallsKey, 1);
        Logs.WriteEvent("API calls reset due to start of new month.");
      }
    }

    private void InitializeGlobalVariables()
    {
      _securityToken          = Settings.ReadString(_securityTokenKey, _securityToken);
      _maxContactApiCalls     = Settings.ReadInt(_maxContactApiCallsKey, 200000);
      _maxCompanyApiCalls     = Settings.ReadInt(_maxCompanyApiCallsKey, 100000);
      _currentContactApiCalls = Settings.ReadInt(_currentContactApiCallsKey, 1);
      _currentCompanyApiCalls = Settings.ReadInt(_currentCompanyApiCallsKey, 1);
      _waitBeforeNewUpdate    = Settings.ReadInt(_waitBeforeNewUpdateKey, 24);
      _maxToProcessByTicketCount = Settings.ReadInt(_maxToProcessByTicketCountKey, 100);
      Service service = Services.GetService(_loginUser, ServiceName);
      _lastProcessed  = DateTime.Parse(Settings.ReadString(_lastProcessedKey, DateTime.UtcNow.AddDays(-1).ToString()));
    }

	/// <summary>
	/// We are keeping track of current api calls and the max. But we'll check directly from the FullContact Account Stats just to make sure.
	/// </summary>
	private void SetValuesFromAccountStats()
	{
		try
		{
			string responseText = string.Empty;
			System.Net.HttpWebRequest request = System.Net.WebRequest.Create(string.Format("https://api.fullcontact.com/v2/stats.xml?period={0}-{1}", DateTime.Now.Year, DateTime.Now.Month)) as System.Net.HttpWebRequest;
			request.Headers.Add(String.Format("X-FullContact-APIKey:{0}", _securityToken));

			using (System.Net.HttpWebResponse response = request.GetResponse() as System.Net.HttpWebResponse)
			{
				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					using (var reader = new System.IO.StreamReader(response.GetResponseStream(), ASCIIEncoding.ASCII))
					{
						responseText = reader.ReadToEnd();
						XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.XmlResolver = null;
  						xmlDoc.LoadXml(responseText);

						string xpath = "PrettyPrintObject/metrics/metrics";
						var nodes = xmlDoc.SelectNodes(xpath);
						string contactMetricId = "200";
						string companyMetricId = "company_200";
						int intOut = 0;

						foreach (XmlNode childrenNode in nodes)
						{
							if (childrenNode.SelectSingleNode("metricId") != null && childrenNode.SelectSingleNode("metricId").InnerText == contactMetricId)
							{
								if (childrenNode.SelectSingleNode("usage") != null)
								{
									intOut = 0;
									bool isSuccess = int.TryParse(childrenNode.SelectSingleNode("usage").InnerText, out intOut);

									if (isSuccess)
									{
										_currentContactApiCalls = intOut;
									}
								}

								if (childrenNode.SelectSingleNode("planLevel") != null)
								{
									intOut = 0;
									bool isSuccess = int.TryParse(childrenNode.SelectSingleNode("planLevel").InnerText, out intOut);

									if (isSuccess)
									{
										_maxContactApiCalls = intOut;
									}
								}

								if (childrenNode.SelectSingleNode("remaining") != null)
								{
									intOut = 0;
									bool isSuccess = int.TryParse(childrenNode.SelectSingleNode("remaining").InnerText, out intOut);

									if (isSuccess)
									{
										_noMoreRemainingContactCalls = intOut == 0;
									}
								}
							}

							if (childrenNode.SelectSingleNode("metricId") != null && childrenNode.SelectSingleNode("metricId").InnerText == companyMetricId)
							{
								if (childrenNode.SelectSingleNode("usage") != null)
								{
									intOut = 0;
									bool isSuccess = int.TryParse(childrenNode.SelectSingleNode("usage").InnerText, out intOut);

									if (isSuccess)
									{
										_currentCompanyApiCalls = intOut;
									}
								}

								if (childrenNode.SelectSingleNode("planLevel") != null)
								{
									intOut = 0;
									bool isSuccess = int.TryParse(childrenNode.SelectSingleNode("planLevel").InnerText, out intOut);

									if (isSuccess)
									{
										_maxCompanyApiCalls = intOut;
									}
								}

								if (childrenNode.SelectSingleNode("remaining") != null)
								{
									intOut = 0;
									bool isSuccess = int.TryParse(childrenNode.SelectSingleNode("remaining").InnerText, out intOut);

									if (isSuccess)
									{
										_noMoreRemainingCompanyCalls = intOut == 0;
									}
								}
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logs.WriteException(ex);
		}
	}

    private void ProcessCustomerInsights()
    {
      try
      {
        Logs.WriteEvent("********************** Starting Customer Insights Processor ***********************");
		Logs.WriteEvent(string.Format("Current Api Calls: Contact ({0}) Company ({1})", _currentContactApiCalls, _currentCompanyApiCalls));

		bool skipCompanyUpdates = (_currentCompanyApiCalls >= _maxCompanyApiCalls) || _noMoreRemainingCompanyCalls;
        bool skipContactUpdates = (_currentContactApiCalls >= _maxContactApiCalls) || _noMoreRemainingContactCalls;

		Organizations companies = new Organizations(LoginUser);
		companies.LoadByCustomerInsightsNewOrModifiedByDate(_lastProcessed.AddMinutes(-10), _waitBeforeNewUpdate);
		Logs.WriteEvent(string.Format("{0} companies recently updated or added since {1} were found on organizations with CustomerInsights active.", companies.Count, _lastProcessed.ToString()));

		if (!skipCompanyUpdates)
		{
			foreach (Organization company in companies)
			{
			  if (_currentCompanyApiCalls < _maxCompanyApiCalls)
			  {
				SyncOrganizationInformation(company);
			  }
			  else
			  {
				skipCompanyUpdates = true;
				break;
			  }
			}
		}

		ContactsView contacts = new ContactsView(LoginUser);
		contacts.LoadByCustomerInsightsNewOrModifiedByDate(_lastProcessed.AddMinutes(-10), _waitBeforeNewUpdate);
		Logs.WriteEvent(string.Format("{0} contacts recently updated or added since {1} were found on organizations with CustomerInsights active.", contacts.Count, _lastProcessed.ToString()));

		if (!skipContactUpdates)
		{
			foreach (ContactsViewItem contact in contacts)
			{
			  if (_currentContactApiCalls < _maxContactApiCalls)
			  {
				SyncContactInformation(contact);
			  }
			  else
			  {
				skipContactUpdates = true;
				break;
			  }
			}
		}

        if (!skipCompanyUpdates)
        {
          Organizations companiesTotalTickets = new Organizations(LoginUser);
          companiesTotalTickets.LoadByCustomerInsightsByCompanyTotalTickets(_waitBeforeNewUpdate, (int)_maxToProcessByTicketCount);
          Logs.WriteEvent(string.Format("{0} companies on organizations with CustomerInsights active based on ticket count.", companiesTotalTickets.Count));

          foreach (Organization company in companiesTotalTickets)
          {
            if (!companies.Where(p => p.OrganizationID == company.OrganizationID).Any())
            {
              if (_currentCompanyApiCalls < _maxCompanyApiCalls)
              {
                SyncOrganizationInformation(company);
              }
              else
              {
                skipCompanyUpdates = true;
                break;
              }
            }
          }
        }

        if (!skipContactUpdates)
        {
          ContactsView contactsTotalTickets = new ContactsView(LoginUser);
          contactsTotalTickets.LoadByCustomerInsightsByContactTotalTickets(_waitBeforeNewUpdate, (int)_maxToProcessByTicketCount);
          Logs.WriteEvent(string.Format("{0} contacts on organizations with CustomerInsights active based on ticket count.", contactsTotalTickets.Count));

          foreach (ContactsViewItem contact in contactsTotalTickets)
          {
            if (!contacts.Where(p => p.UserID == contact.UserID).Any())
            {
              if (_currentContactApiCalls < _maxContactApiCalls)
              {
                SyncContactInformation(contact);
              }
              else
              {
                skipContactUpdates = true;
                break;
              }
            }
          }
        }

        if (skipCompanyUpdates)
        {
          Logs.WriteEventFormat("Maximum Company api calls {0} have been reached for this month.", _maxCompanyApiCalls);
        }

        if (skipContactUpdates)
        {
          Logs.WriteEventFormat("Maximum Contact api calls {0} have been reached for this month.", _maxContactApiCalls);
        }
      }
      catch (Exception ex)
      {
        Logs.WriteException(ex);
      }

      Logs.WriteEvent("********************** Customer Insights Processor Finished **********************");
    }

    private void SyncOrganizationInformation(Organization company)
    {
      Logs.WriteEvent(string.Format("Processing company {0}({1})", company.Name, company.OrganizationID));

      if (!string.IsNullOrEmpty(company.Website))
      {
        string responseText = string.Empty;
        string requestParameter = string.Format("{0}={1}", CompanyObjects.Lookup.domain.ToString(), company.Website);
        string requestUrl = string.Format("{0}company/lookup.json?{1}", _baseURI, requestParameter);
        responseText = CustomerInsightsUtilities.MakeHttpWebRequest(requestUrl, _securityToken, Logs, Settings, _currentCompanyApiCallsKey, ref _currentCompanyApiCalls);

        try
        {
          if (!string.IsNullOrEmpty(responseText))
          {
            JObject jObject = JObject.Parse(responseText);
            CompanyObjects.RootObject companyInfo = JsonConvert.DeserializeObject<CompanyObjects.RootObject>(jObject.ToString());
            UpdateCompanyInformation(companyInfo, company);
          }
          else
          {
            Logs.WriteEvent("CustomerInsights did not return information.");
          }
        }
        catch (Exception ex)
        {
          Logs.WriteException(ex);
        }
      }
      else
      {
        Logs.WriteEvent("This company does not have a website entered, can't get its CustomerInsights information.");
      }

      UpdateFullContactCompanyModified(company.OrganizationID);
    }

    private void SyncContactInformation(ContactsViewItem contact)
    {
      Logs.WriteEvent(string.Format("Processing contact {0}({1}) of organization {2}({3})", contact.Name, contact.UserID, contact.Organization, contact.OrganizationID));

      if (!string.IsNullOrEmpty(contact.Email))
      {
        string responseText     = string.Empty;
        string requestParameter = string.Format("{0}={1}", ContactObjects.Lookup.email.ToString(), contact.Email);
        string requestUrl       = string.Format("{0}person.json?{1}", _baseURI, requestParameter);
        responseText            = CustomerInsightsUtilities.MakeHttpWebRequest(requestUrl, _securityToken, Logs, Settings, _currentContactApiCallsKey, ref _currentContactApiCalls);

        try
        {
          if (!string.IsNullOrEmpty(responseText))
          {
            JObject jObject = JObject.Parse(responseText);
            ContactObjects.RootObject contactInfo = JsonConvert.DeserializeObject<ContactObjects.RootObject>(jObject.ToString());
            User currentContact = Users.GetUser(LoginUser, contact.UserID);
            UpdateContactInformation(contactInfo, currentContact, (int)contact.OrganizationParentID);
          }
          else
          {
            Logs.WriteEvent("CustomerInsights did not return information.");
          }
        }
        catch (Exception ex)
        {
          Logs.WriteException(ex);
        }
      }
      else
      {
        Logs.WriteEvent("This contact does not have a email entered, can't get its CustomerInsights information.");
      }

      UpdateFullContactContactModified(contact.UserID);
    }

    private bool UpdateCompanyInformation(CompanyObjects.RootObject customerInsightsOrganizationInfo, Organization currentCompanyInfo)
    {
        bool    isUpdated         = false;
        string  useSocialProfile  = CustomerInsightsUtilities.SocialProfiles.LinkedIn.ToString();

        if (customerInsightsOrganizationInfo.socialProfiles != null && customerInsightsOrganizationInfo.socialProfiles.Count > 0)
        {
          //We will use LinkedIn for bio, if not found then use the first one of the others with that information
          if (!customerInsightsOrganizationInfo.socialProfiles.Exists(p => p.typeName.ToLower() == useSocialProfile.ToLower()))
          {
            useSocialProfile = string.Empty;
            useSocialProfile = customerInsightsOrganizationInfo.socialProfiles.Where(p => !string.IsNullOrEmpty(p.bio)).Select(p => p.typeName).FirstOrDefault();
          }

          if (!string.IsNullOrEmpty(useSocialProfile))
          {
            string bio = customerInsightsOrganizationInfo.socialProfiles.Where(p => p.typeName.ToLower() == useSocialProfile.ToLower() && !string.IsNullOrEmpty(p.bio)).Select(p => p.bio).FirstOrDefault();

            if (CanUpdateCompanyBio(currentCompanyInfo, bio))
            {
              currentCompanyInfo.Description = bio;
              isUpdated = true;
            }
          }
          else
          {
            Logs.WriteEvent("No bio found in any of the social profiles");
          }
        }
        else
        {
          Logs.WriteEvent("No social profile found");
        }


        if (!string.IsNullOrEmpty(customerInsightsOrganizationInfo.logo))
            {
                string resultMessage = string.Empty;
                string logoPath = TeamSupport.Data.Quarantine.ServiceQ.GetAttachmentPath10(LoginUser, (int)currentCompanyInfo.ParentID);
                string logoFullPath = string.Format("{0}\\{1}.png", logoPath, currentCompanyInfo.OrganizationID);

                if (CustomerInsightsUtilities.DownloadImage(customerInsightsOrganizationInfo.logo, logoFullPath, currentCompanyInfo.OrganizationID, AttachmentProxy.References.Organizations, LoginUser, out resultMessage))
                {
                    string description = string.Format("TeamSupport System updated Logo for '{0}'", currentCompanyInfo.Name);
                    ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Organizations, currentCompanyInfo.OrganizationID, description);

                    //delete cached image
                    string cachePath = string.Empty;
                    string pattern = string.Empty;
                    try
                    {
                        cachePath = TeamSupport.Data.Quarantine.ServiceQ.GetAttachmentPath11(LoginUser, currentCompanyInfo.ParentID.ToString());

                        if (System.IO.Directory.Exists(cachePath))
                        {
                            pattern = currentCompanyInfo.OrganizationID.ToString() + "-*.*";
                            string[] files = System.IO.Directory.GetFiles(cachePath, pattern, System.IO.SearchOption.TopDirectoryOnly);

                            foreach (String file in files)
                            {
                                System.IO.File.Delete(file);
                                Logs.WriteEvent(string.Format("Cached file {0} deleted.", file));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logs.WriteEvent("Exception deleting cached images for company.");
                        Logs.WriteEventFormat("CachePath: {0}", cachePath.ToString());
                        Logs.WriteEventFormat("Pattern: {0}", pattern.ToString());
                        Logs.WriteEventFormat("Exception Message: {0}", ex.Message.ToString());
                        Logs.WriteEventFormat("Exception StackTrace: {0}", ex.StackTrace.ToString());
                    }
                }

                if (!string.IsNullOrEmpty(resultMessage))
                {
                    Logs.WriteEvent(resultMessage);
                }
            }
            else
        { 
          Logs.WriteEvent("No logo found");
        }

        if (isUpdated)
        {
          currentCompanyInfo.Collection.Save();
          Logs.WriteEvent(string.Format("Bio pulled from {0}", useSocialProfile));
          string description = "TeamSupport System changed description for organization '" + currentCompanyInfo.Name + "'";
          ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Organizations, currentCompanyInfo.OrganizationID, description);
        }

        return isUpdated;
      }

        private bool UpdateContactInformation(ContactObjects.RootObject customerInsightsContactInfo, User currentContactInfo, int organizationParentId)
    {
        bool    isTitleUpdated    = false;
        bool    isLinkedInUpdated = false;
        string  useSocialProfile  = CustomerInsightsUtilities.SocialProfiles.LinkedIn.ToString();

        if (customerInsightsContactInfo.socialProfiles != null && customerInsightsContactInfo.socialProfiles.Count > 0)
        {
          if (!customerInsightsContactInfo.socialProfiles.Exists(p => p.typeName.ToLower() == CustomerInsightsUtilities.SocialProfiles.LinkedIn.ToString().ToLower()))
          {
            Logs.WriteEvent("LinkedIn not found");
          }
          else
          {
            ContactObjects.SocialProfile contactInfo = customerInsightsContactInfo.socialProfiles.Where(p => p.typeName.ToLower() == useSocialProfile.ToLower()).FirstOrDefault();
            if (contactInfo != null && CanUpdateContactLinkedIn(currentContactInfo, contactInfo.url))
            {
              currentContactInfo.LinkedIn = contactInfo.url;
              isLinkedInUpdated = true;
            }
          }
        }
        else
        {
          Logs.WriteEvent("No social profile found");
        }

        if (customerInsightsContactInfo.organizations != null && customerInsightsContactInfo.organizations.Count > 0)
        {
          ContactObjects.Organization organization = customerInsightsContactInfo.organizations.Where(p => p.isPrimary).FirstOrDefault();
          if (organization != null && CanUpdateContactTitle(currentContactInfo, organization.title))
          {
            currentContactInfo.Title = organization.title;
            isTitleUpdated = true;
          }
        }
        else
        {
          Logs.WriteEvent("No organizations found");
        }

        if (isLinkedInUpdated || isTitleUpdated)
        {
          currentContactInfo.Collection.Save();
          string description = string.Empty;

          if (isLinkedInUpdated)
          {
            Logs.WriteEventFormat("LinkedIn updated to {0}", currentContactInfo.LinkedIn);
            description = string.Format("TeamSupport System changed LinkedIn to {0} ", currentContactInfo.LinkedIn);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, currentContactInfo.UserID, description);
          }

          if (isTitleUpdated)
          {
            Logs.WriteEventFormat("Title updated to {0}", currentContactInfo.Title);
            description = string.Format("TeamSupport System set contact title to {0} ", currentContactInfo.Title);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, currentContactInfo.UserID, description);
          }
        }

        string usePhotoFrom = CustomerInsightsUtilities.SocialProfiles.LinkedIn.ToString();
        string photoUrl = string.Empty;

        if (customerInsightsContactInfo.photos != null && customerInsightsContactInfo.photos.Any())
        {
          if (customerInsightsContactInfo.photos.Exists(p => p.typeName.ToLower() == usePhotoFrom.ToLower()))
          {
            photoUrl = customerInsightsContactInfo.photos.Where(p => p.typeName.ToLower() == usePhotoFrom.ToLower() && !string.IsNullOrEmpty(p.url)).Select(p => p.url).FirstOrDefault();
          }

          if (string.IsNullOrEmpty(photoUrl))
          {
            photoUrl = customerInsightsContactInfo.photos.Where(p => !string.IsNullOrEmpty(p.url)).Select(p => p.url).FirstOrDefault();
          }
        }

        if (!string.IsNullOrEmpty(photoUrl))
            {
                string resultMessage = string.Empty;
                string logoPath = TeamSupport.Data.Quarantine.ServiceQ.GetAttachmentPath12(LoginUser, organizationParentId);
                string logoFullPath = string.Format("{0}\\{1}avatar.jpg", logoPath, currentContactInfo.UserID.ToString());

                if (CustomerInsightsUtilities.DownloadImage(photoUrl, logoFullPath, currentContactInfo.OrganizationID, AttachmentProxy.References.Contacts, LoginUser, out resultMessage))
                {
                    string description = "TeamSupport System updated Photo for  '" + currentContactInfo.DisplayName + "'";
                    ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, currentContactInfo.UserID, description);

                    //delete cached image
                    string cachePath = string.Empty;
                    string pattern = string.Empty;
                    try
                    {
                        cachePath = TeamSupport.Data.Quarantine.ServiceQ.GetAttachmentPath13(LoginUser, organizationParentId);
                        pattern = currentContactInfo.UserID.ToString() + "-*.*";
                        string[] files = System.IO.Directory.GetFiles(cachePath, pattern, System.IO.SearchOption.TopDirectoryOnly);

                        foreach (String file in files)
                        {
                            System.IO.File.Delete(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logs.WriteEvent("Exception deleting cached images for contact.");
                        Logs.WriteEventFormat("CachePath: {0}", cachePath.ToString());
                        Logs.WriteEventFormat("Pattern: {0}", pattern.ToString());
                        Logs.WriteEventFormat("Exception Message: {0}", ex.Message.ToString());
                        Logs.WriteEventFormat("Exception StackTrace: {0}", ex.StackTrace.ToString());
                    }
                }

                if (!string.IsNullOrEmpty(resultMessage))
                {
                    Logs.WriteEvent(resultMessage);
                }
            }
            else
        {
          Logs.WriteEvent("No photo url found");
        }

        return isLinkedInUpdated || isTitleUpdated;
      }

        private bool CanUpdateCompanyBio(Organization organization, string bio)
    {
      bool canUpdate = false;
      bool wasUpdatedByUser = false;

      ActionLogs actionLogs = new ActionLogs(LoginUser);
      actionLogs.LoadByOrganizationID(organization.OrganizationID);

      ActionLog lastActionLog = actionLogs.OrderByDescending(p => p.DateCreated)
                                                            .Where(p => p.RefType == ReferenceType.Organizations &&
                                                                        (p.Description.ToLower().Contains("changed description"))
                                                                        || p.Description.ToLower().Contains("set company description"))
                                                            .FirstOrDefault();

      //If the description has been updated before, we need to check if the user did it. If not then we can update.
      if (lastActionLog != null)
      {
        wasUpdatedByUser = lastActionLog.ModifierID > 0;

        if (!wasUpdatedByUser)
        {
          canUpdate = organization.Description != bio;
        }
      }
      else if (string.IsNullOrEmpty(organization.Description))
      {
        //The description has never changed. Update it if empty
        canUpdate = true;
      }

      return canUpdate;
    }

    private bool CanUpdateContactLinkedIn(User contact, string linkedIn)
	  {
      bool canUpdate = false;
      bool wasUpdatedByUser = false;

      ActionLogs actionLogs = new ActionLogs(LoginUser);
      actionLogs.LoadByUserID(contact.UserID);

      ActionLog lastActionLog = actionLogs.OrderByDescending(p => p.DateCreated)
                                                            .Where(p => p.RefType == ReferenceType.Users &&
                                                                        (p.Description.ToLower().Contains("changed linkedin")
                                                                        || p.Description.ToLower().Contains("set contact linkedin")))
                                                            .FirstOrDefault();

      //If the linked has been updated before, we need to check if the user did it. If not then we can update.
      if (lastActionLog != null)
      {
        wasUpdatedByUser = lastActionLog.ModifierID > 0;

        if (!wasUpdatedByUser)
        {
          canUpdate = contact.LinkedIn != linkedIn;
        }
      }
      else if (string.IsNullOrEmpty(contact.LinkedIn))
      {
          //The linkedin has never changed. Update it if empty
          canUpdate = true;  
      }

      return canUpdate;
    }

    private bool CanUpdateContactTitle(User contact, string title)
    {
      bool canUpdate = false;
      bool wasUpdatedByUser = false;

      ActionLogs actionLogs = new ActionLogs(LoginUser);
      actionLogs.LoadByUserID(contact.UserID);

      ActionLog lastActionLog = actionLogs.OrderByDescending(p => p.DateCreated)
                                                            .Where(p => p.RefType == ReferenceType.Users &&
                                                                        p.Description.ToLower().Contains("set contact title"))
                                                            .FirstOrDefault();

      //If the title has been updated before, we need to check if the user did it. If not then we can update.
      if (lastActionLog != null)
      {
        wasUpdatedByUser = lastActionLog.ModifierID > 0;

        if (!wasUpdatedByUser)
        {
          canUpdate = contact.Title != title;
        }
      }
      else if (string.IsNullOrEmpty(contact.Title))
      {
        //The title has never changed. Update it if empty
        canUpdate = true;  
      }

      return canUpdate;
    }

    private void UpdateFullContactCompanyModified(int organizationId)
    {
      UpdateFullContactModified(organizationId: organizationId);
    }

    private void UpdateFullContactContactModified(int userId)
    {
      UpdateFullContactModified(userId: userId);
    }

    private void UpdateFullContactModified(int? organizationId = null, int? userId = null)
    {
      FullContactUpdates fullContactUpdates = new FullContactUpdates(LoginUser);

      if (organizationId != null)
      {
        fullContactUpdates.LoadByOrganizationId((int)organizationId);
      }
      else if (userId != null)
      {
        fullContactUpdates.LoadByContactId((int)userId);
      }

      if (organizationId != null || userId != null)
      {
        if (fullContactUpdates.Count > 0)
        {
          FullContactUpdatesItem fullContactUpdatesItem = fullContactUpdates.First();
          fullContactUpdatesItem.DateModified = DateTime.UtcNow;
          fullContactUpdatesItem.Collection.Save();
        }
        else
        {
          FullContactUpdates newFullContactUpdate = new FullContactUpdates(LoginUser);
          FullContactUpdatesItem fullContactUpdatesItem = newFullContactUpdate.AddNewFullContactUpdatesItem();

          if (organizationId != null)
          {
            fullContactUpdatesItem.OrganizationId = (int)organizationId;
          }
          else
          {
            fullContactUpdatesItem.UserId = (int)userId;
          }

          fullContactUpdatesItem.DateModified = DateTime.UtcNow;
          newFullContactUpdate.Save();
        }
      }
    }
  }
}
