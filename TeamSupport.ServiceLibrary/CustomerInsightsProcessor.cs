﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public override void Run()
    {
      try
      {
        if (Settings.ReadBool("CustomerInsights Enabled", true))
        {
          InitializeGlobalVariables();
          int customerInsightsInterval = Settings.ReadInt(_intervalKey, 60);

          if (_lastProcessed.AddMinutes(customerInsightsInterval) < DateTime.Now)
          {
            ProcessCustomerInsights();
            Settings.WriteString(_lastProcessedKey, DateTime.Now.ToString());
          }
        }
        UpdateHealth();
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "CustomerInsightsProcessor");
      }
    }

    private void InitializeGlobalVariables()
    {
      _securityToken = Settings.ReadString(_securityTokenKey, _securityToken);
      _maxContactApiCalls = Settings.ReadInt(_maxContactApiCallsKey, 200000);
      _maxCompanyApiCalls = Settings.ReadInt(_maxCompanyApiCallsKey, 100000);
      _currentContactApiCalls = Settings.ReadInt(_currentContactApiCallsKey, 1);
      _currentCompanyApiCalls = Settings.ReadInt(_currentCompanyApiCallsKey, 1);
      _waitBeforeNewUpdate = Settings.ReadInt(_waitBeforeNewUpdateKey, 24);
      _maxToProcessByTicketCount = Settings.ReadInt(_maxToProcessByTicketCountKey, 100);
      Service service = Services.GetService(_loginUser, ServiceName);
      _lastProcessed = DateTime.Parse(Settings.ReadString(_lastProcessedKey, DateTime.Now.AddDays(-1).ToString()));
    }

    private void ProcessCustomerInsights()
    {
      try
      {
        Logs.WriteEvent("***********************************************************************************");
        Logs.WriteEvent("Starting Customer Insights Processor");
        Logs.WriteEvent("***********************************************************************************");

        Organizations companies = new Organizations(LoginUser);
        companies.LoadByCustomerInsightsNewOrModifiedByDate(_lastProcessed.AddMinutes(-10), _waitBeforeNewUpdate);
        Logs.WriteEvent(string.Format("{0} companies recently updated or added since {1} were found on organizations with CustomerInsights active.", companies.Count, _lastProcessed.ToString()));

        bool skipCompanyUpdates = false;
        bool skipContactUpdates = false;

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

        ContactsView contacts = new ContactsView(LoginUser);
        contacts.LoadByCustomerInsightsNewOrModifiedByDate(_lastProcessed.AddMinutes(-10), _waitBeforeNewUpdate);
        Logs.WriteEvent(string.Format("{0} contacts recently updated or added since {1} were found on organizations with CustomerInsights active.", contacts.Count, _lastProcessed.ToString()));

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
          Logs.WriteEventFormat("Maximum Company api calls {0} have been reached.", _maxCompanyApiCalls);
        }

        if (skipContactUpdates)
        {
          Logs.WriteEventFormat("Maximum Contact api calls {0} have been reached.", _maxContactApiCalls);
        }
      }
      catch (Exception ex)
      {
        Logs.WriteException(ex);
      }

      Logs.WriteEvent("Customer Insights Processor Finished");
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
          string resultMessage  = string.Empty;
          string logoPath       = AttachmentPath.GetPath(LoginUser, (int)currentCompanyInfo.ParentID, AttachmentPath.Folder.OrganizationsLogo);
          string logoFullPath   = string.Format("{0}\\{1}.png", logoPath, currentCompanyInfo.OrganizationID);

          if (CustomerInsightsUtilities.DownloadImage(customerInsightsOrganizationInfo.logo, logoFullPath, currentCompanyInfo.OrganizationID, TeamSupport.Data.ReferenceType.Organizations, LoginUser, out resultMessage))
          {
            string description = string.Format("Updated Logo for '{0}'", currentCompanyInfo.Name);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Organizations, currentCompanyInfo.OrganizationID, description);
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
          string description = "Changed description for organization '" + currentCompanyInfo.Name + "'";
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
            description = string.Format("{0} changed LinkedIn to {1} ", "CRM Service", currentContactInfo.LinkedIn);
            ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, currentContactInfo.UserID, description);
          }

          if (isTitleUpdated)
          {
            Logs.WriteEventFormat("Title updated to {0}", currentContactInfo.Title);
            description = string.Format("{0} set contact title to {1} ", "CRM Service", currentContactInfo.Title);
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
            string resultMessage  = string.Empty;
            string logoPath       = AttachmentPath.GetPath(LoginUser, organizationParentId, AttachmentPath.Folder.ContactImages);
            string logoFullPath   = string.Format("{0}\\{1}avatar.jpg", logoPath, currentContactInfo.UserID.ToString());

            if (CustomerInsightsUtilities.DownloadImage(photoUrl, logoFullPath, currentContactInfo.OrganizationID, TeamSupport.Data.ReferenceType.Contacts, LoginUser, out resultMessage))
            {
              string description = "Updated Photo for  '" + currentContactInfo.DisplayName + "'";
              ActionLogs.AddActionLog(LoginUser, ActionLogType.Update, ReferenceType.Users, currentContactInfo.UserID, description);
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

      ActionLogs actionLogs = new ActionLogs(LoginUser);
      actionLogs.LoadByOrganizationID(organization.OrganizationID);

      ActionLog lastActionLog = actionLogs.OrderByDescending(p => p.DateCreated)
                                                            .Where(p => p.RefType == ReferenceType.Organizations &&
                                                                        p.Description.ToLower().Contains("changed description"))
                                                            .FirstOrDefault();
      bool wasUpdatedByUser = lastActionLog != null && lastActionLog.ModifierID > 0;

      if (lastActionLog == null || (!wasUpdatedByUser && (organization.Description != bio || string.IsNullOrEmpty(organization.Description))))
      {
        canUpdate = true;
      }

      return canUpdate;
    }

    private bool CanUpdateContactLinkedIn(User contact, string linkedIn)
	  {
      bool canUpdate = false;

      ActionLogs actionLogs = new ActionLogs(LoginUser);
      actionLogs.LoadByUserID(contact.UserID);

      ActionLog lastActionLog = actionLogs.OrderByDescending(p => p.DateCreated)
                                                            .Where(p => p.RefType == ReferenceType.Users &&
                                                                        p.Description.ToLower().Contains("changed linkedin"))
                                                            .FirstOrDefault();
      bool wasUpdatedByUser = lastActionLog != null && lastActionLog.ModifierID > 0;

      if (lastActionLog == null || (!wasUpdatedByUser && (contact.LinkedIn != linkedIn || string.IsNullOrEmpty(contact.LinkedIn))))
      {
        canUpdate = true;
      }

      return canUpdate;
    }

    private bool CanUpdateContactTitle(User contact, string title)
    {
      bool canUpdate = false;

      ActionLogs actionLogs = new ActionLogs(LoginUser);
      actionLogs.LoadByUserID(contact.UserID);

      ActionLog lastActionLog = actionLogs.OrderByDescending(p => p.DateCreated)
                                                            .Where(p => p.RefType == ReferenceType.Users &&
                                                                        p.Description.ToLower().Contains("set contact title"))
                                                            .FirstOrDefault();
      bool wasUpdatedByUser = lastActionLog != null && lastActionLog.ModifierID > 0;

      if (lastActionLog == null || (!wasUpdatedByUser && (contact.Title != title || string.IsNullOrEmpty(contact.Title))))
      {
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
          fullContactUpdatesItem.DateModified = DateTime.Now;
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

          fullContactUpdatesItem.DateModified = DateTime.Now;
          newFullContactUpdate.Save();
        }
      }
    }
  }
}
