using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using TeamSupport.Data;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using TeamSupport.WebUtils;

namespace TSWebServices
{
  [ScriptService]
  [WebService(Namespace = "http://teamsupport.com/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class AutomationService : System.Web.Services.WebService
  {
    
    public AutomationService()
    {

      //Uncomment the following line if using designed components 
      //InitializeComponent(); 
    }
    
    [WebMethod]
    public AutomationData GetData()
    {
      
      AutomationData result = new AutomationData();
      TicketAutomationPossibleActions actions = new TicketAutomationPossibleActions(UserSession.LoginUser);
      actions.LoadActive();
      result.Actions = actions.GetTicketAutomationPossibleActionProxies();


      List<AutoFieldItem> fieldItems = new List<AutoFieldItem>();
      ReportTableFields fields = new ReportTableFields(TSAuthentication.GetLoginUser());
      fields.LoadByReportTableID(10);

      CustomFields customs = new CustomFields(fields.LoginUser);
      customs.LoadByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Tickets);

      CustomFields orgfields = new CustomFields(fields.LoginUser);
      orgfields.LoadByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Organizations);
      List<string> orgCustomFields = new List<string>();
      foreach (CustomField c in orgfields)
      {
        orgCustomFields.Add(c.Name +":"+c.CustomFieldID);
      }


      TicketTypes ticketTypes = new TicketTypes(fields.LoginUser);
      ticketTypes.LoadAllPositions(TSAuthentication.OrganizationID);

      foreach (ReportTableField field in fields)
      {
        fieldItems.Add(new AutoFieldItem(field));
      }

      List<AutoFieldItem> customFieldsItems = new List<AutoFieldItem>();
      foreach (CustomField custom in customs)
      {
        TicketType ticketType = ticketTypes.FindByTicketTypeID(custom.AuxID);
        if (ticketType == null)
        {
          fieldItems.Add(new AutoFieldItem(custom));
          customFieldsItems.Add(new AutoFieldItem(custom));
        }
        else
        {
          fieldItems.Add(new AutoFieldItem(custom, string.Format("{0} ({1})", custom.Name, ticketType.Name)));
          customFieldsItems.Add(new AutoFieldItem(custom, string.Format("{0} ({1})", custom.Name, ticketType.Name)));
        }
      }
      result.CustomFields = customFieldsItems.ToArray();

      ReportTableField actionsViewDescription = ReportTableFields.GetReportTableField(fields.LoginUser, 6);
      actionsViewDescription.Alias = "Action Text";
      fieldItems.Add(new AutoFieldItem(actionsViewDescription));

      ReportTableField actionsViewName = ReportTableFields.GetReportTableField(fields.LoginUser, 5);
      fieldItems.Add(new AutoFieldItem(actionsViewName));

      ReportTableField actionsViewType = ReportTableFields.GetReportTableField(fields.LoginUser, 18);
      fieldItems.Add(new AutoFieldItem(actionsViewType));

      AutoFieldItem afiDayOfWeekCreated = new AutoFieldItem();
      afiDayOfWeekCreated.Alias = "Day of Week Created";
      afiDayOfWeekCreated.DataType = "list";
      afiDayOfWeekCreated.FieldID = 101001;
      afiDayOfWeekCreated.FieldName = "Day of Week Created";
      afiDayOfWeekCreated.IsCustom = false;
      afiDayOfWeekCreated.IsVisible = true;
      afiDayOfWeekCreated.ListValues = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
      afiDayOfWeekCreated.LookupTableID = null;
      afiDayOfWeekCreated.Size = 0;
      afiDayOfWeekCreated.Description = "";
      afiDayOfWeekCreated.TableID = -2;
      afiDayOfWeekCreated.RefType = ReferenceType.Tickets;
      afiDayOfWeekCreated.AuxID = null;
      afiDayOfWeekCreated.OtherTrigger = "ticketsview.dayofweekcreated";
      fieldItems.Add(afiDayOfWeekCreated);

      AutoFieldItem afiHourOfDayCreated = new AutoFieldItem();
      afiHourOfDayCreated.Alias = "Hour of Day Created";
      afiHourOfDayCreated.DataType = "text";
      afiHourOfDayCreated.FieldID = 101002;
      afiHourOfDayCreated.FieldName = "Hour of Day Created";
      afiHourOfDayCreated.IsCustom = false;
      afiHourOfDayCreated.IsVisible = true;
      afiHourOfDayCreated.ListValues = null;
      afiHourOfDayCreated.LookupTableID = null;
      afiHourOfDayCreated.Size = 0;
      afiHourOfDayCreated.Description = "";
      afiHourOfDayCreated.TableID = -2;
      afiHourOfDayCreated.RefType = ReferenceType.Tickets;
      afiHourOfDayCreated.AuxID = null;
      afiHourOfDayCreated.OtherTrigger = "ticketsview.hourofdaycreated";
      fieldItems.Add(afiHourOfDayCreated);

      AutoFieldItem afiMinSinceLastAction = new AutoFieldItem();
      afiMinSinceLastAction.Alias = "Minutes Since last Action Added";
      afiMinSinceLastAction.DataType = "text";
      afiMinSinceLastAction.FieldID = 101003;
      afiMinSinceLastAction.FieldName = "Minutes Since last Action Added";
      afiMinSinceLastAction.IsCustom = false;
      afiMinSinceLastAction.IsVisible = true;
      afiMinSinceLastAction.ListValues = null;
      afiMinSinceLastAction.LookupTableID = null;
      afiMinSinceLastAction.Size = 0;
      afiMinSinceLastAction.Description = "";
      afiMinSinceLastAction.TableID = -2;
      afiMinSinceLastAction.RefType = ReferenceType.Tickets;
      afiMinSinceLastAction.AuxID = null;
      afiMinSinceLastAction.OtherTrigger = "ticketsview.minutessincelastactionadded";
      fieldItems.Add(afiMinSinceLastAction);

      AutoFieldItem afiHoursSinceAction = new AutoFieldItem();
      afiHoursSinceAction.Alias = "Hours Since Last Action Added";
      afiHoursSinceAction.DataType = "text";
      afiHoursSinceAction.FieldID = 101004;
      afiHoursSinceAction.FieldName = "Hours Since Last Action Added";
      afiHoursSinceAction.IsCustom = false;
      afiHoursSinceAction.IsVisible = true;
      afiHoursSinceAction.ListValues = null;
      afiHoursSinceAction.LookupTableID = null;
      afiHoursSinceAction.Size = 0;
      afiHoursSinceAction.Description = "";
      afiHoursSinceAction.TableID = -2;
      afiHoursSinceAction.RefType = ReferenceType.Tickets;
      afiHoursSinceAction.AuxID = null;
      afiHoursSinceAction.OtherTrigger = "ticketsview.hourssincelastactionadded";
      fieldItems.Add(afiHoursSinceAction);

      AutoFieldItem afiCurrentDayOfWeek = new AutoFieldItem();
      afiCurrentDayOfWeek.Alias = "Current Day of Week";
      afiCurrentDayOfWeek.DataType = "list";
      afiCurrentDayOfWeek.FieldID = 101005;
      afiCurrentDayOfWeek.FieldName = "Current Day of Week";
      afiCurrentDayOfWeek.IsCustom = false;
      afiCurrentDayOfWeek.IsVisible = true;
      afiCurrentDayOfWeek.ListValues = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
      afiCurrentDayOfWeek.LookupTableID = null;
      afiCurrentDayOfWeek.Size = 0;
      afiCurrentDayOfWeek.Description = "";
      afiCurrentDayOfWeek.TableID = -2;
      afiCurrentDayOfWeek.RefType = ReferenceType.Tickets;
      afiCurrentDayOfWeek.AuxID = null;
      afiCurrentDayOfWeek.OtherTrigger = "ticketsview.currentdayofweek";
      fieldItems.Add(afiCurrentDayOfWeek);

      AutoFieldItem afiCurrentHourOfDay = new AutoFieldItem();
      afiCurrentHourOfDay.Alias = "Current Hour of Day";
      afiCurrentHourOfDay.DataType = "text";
      afiCurrentHourOfDay.FieldID = 101006;
      afiCurrentHourOfDay.FieldName = "Current Hour of Day";
      afiCurrentHourOfDay.IsCustom = false;
      afiCurrentHourOfDay.IsVisible = true;
      afiCurrentHourOfDay.ListValues = null;
      afiCurrentHourOfDay.LookupTableID = null;
      afiCurrentHourOfDay.Size = 0;
      afiCurrentHourOfDay.Description = "";
      afiCurrentHourOfDay.TableID = -2;
      afiCurrentHourOfDay.RefType = ReferenceType.Tickets;
      afiCurrentHourOfDay.AuxID = null;
      afiCurrentHourOfDay.OtherTrigger = "ticketsview.currenthourofday";
      fieldItems.Add(afiCurrentHourOfDay);

      AutoFieldItem afiAssignedUserIsAvailable = new AutoFieldItem();
      afiAssignedUserIsAvailable.Alias = "Assigned User Is Available";
      afiAssignedUserIsAvailable.DataType = "bit";
      afiAssignedUserIsAvailable.FieldID = 101007;
      afiAssignedUserIsAvailable.FieldName = "Assigned User Is Available";
      afiAssignedUserIsAvailable.IsCustom = false;
      afiAssignedUserIsAvailable.IsVisible = true;
      afiAssignedUserIsAvailable.ListValues = null;
      afiAssignedUserIsAvailable.LookupTableID = null;
      afiAssignedUserIsAvailable.Size = 0;
      afiAssignedUserIsAvailable.Description = "";
      afiAssignedUserIsAvailable.TableID = -2;
      afiAssignedUserIsAvailable.RefType = ReferenceType.Tickets;
      afiAssignedUserIsAvailable.AuxID = null;
      afiAssignedUserIsAvailable.OtherTrigger = "ticketsview.assigneduseravailable";
      fieldItems.Add(afiAssignedUserIsAvailable);

      AutoFieldItem afiAssignedUserIsBusy = new AutoFieldItem();
      afiAssignedUserIsBusy.Alias = "Assigned User Is Busy";
      afiAssignedUserIsBusy.DataType = "bit";
      afiAssignedUserIsBusy.FieldID = 101008;
      afiAssignedUserIsBusy.FieldName = "Assigned User Is Busy";
      afiAssignedUserIsBusy.IsCustom = false;
      afiAssignedUserIsBusy.IsVisible = true;
      afiAssignedUserIsBusy.ListValues = null;
      afiAssignedUserIsBusy.LookupTableID = null;
      afiAssignedUserIsBusy.Size = 0;
      afiAssignedUserIsBusy.Description = "";
      afiAssignedUserIsBusy.TableID = -2;
      afiAssignedUserIsBusy.RefType = ReferenceType.Tickets;
      afiAssignedUserIsBusy.AuxID = null;
      afiAssignedUserIsBusy.OtherTrigger = "ticketsview.assignedusernotavailable";
      fieldItems.Add(afiAssignedUserIsBusy);

			AutoFieldItem afiAgentRatings = new AutoFieldItem();
			afiAgentRatings.Alias = "Agent Rating";
			afiAgentRatings.DataType = "list";
			afiAgentRatings.FieldID = 101009;
			afiAgentRatings.FieldName = "AgentRating";
			afiAgentRatings.IsCustom = false;
			afiAgentRatings.IsVisible = true;
			afiAgentRatings.ListValues = new string[] { "Positive", "Negative", "Neutral" };
			afiAgentRatings.LookupTableID = null;
			afiAgentRatings.Size = 0;
			afiAgentRatings.Description = "";
			afiAgentRatings.TableID = -2;
			afiAgentRatings.RefType = ReferenceType.Tickets;
			afiAgentRatings.AuxID = null;
			afiAgentRatings.OtherTrigger = "ticketsview.AgentRating";
			fieldItems.Add(afiAgentRatings);

            AutoFieldItem afiCDIValue = new AutoFieldItem();
            afiCDIValue.Alias = "Customer CDI Value";
            afiCDIValue.DataType = "text";
            afiCDIValue.FieldID = 101010;
            afiCDIValue.FieldName = "Customer CDI Value";
            afiCDIValue.IsCustom = false;
            afiCDIValue.IsVisible = true;
            afiCDIValue.ListValues = null;
            afiCDIValue.LookupTableID = null;
            afiCDIValue.Size = 0;
            afiCDIValue.Description = "";
            afiCDIValue.TableID = -2;
            afiCDIValue.RefType = ReferenceType.Tickets;
            afiCDIValue.AuxID = null;
            afiCDIValue.OtherTrigger = "organizations.cdivalue";
            fieldItems.Add(afiCDIValue);

            AutoFieldItem afiCDITrend = new AutoFieldItem();
            afiCDITrend.Alias = "Customer CDI Trend";
            afiCDITrend.DataType = "text";
            afiCDITrend.FieldID = 101011;
            afiCDITrend.FieldName = "Customer CDI Trend";
            afiCDITrend.IsCustom = false;
            afiCDITrend.IsVisible = true;
            afiCDITrend.ListValues = new string[] { "1", "0", "-1" }; ;
            afiCDITrend.LookupTableID = null;
            afiCDITrend.Size = 0;
            afiCDITrend.Description = "";
            afiCDITrend.TableID = -2;
            afiCDITrend.RefType = ReferenceType.Tickets;
            afiCDITrend.AuxID = null;
            afiCDITrend.OtherTrigger = "organizations.cditrend";
            fieldItems.Add(afiCDITrend);

            AutoFieldItem afiServiceLevelName = new AutoFieldItem();
            afiServiceLevelName.Alias = "Service Level Name";
            afiServiceLevelName.DataType = "text";
            afiServiceLevelName.FieldID = 101012;
            afiServiceLevelName.FieldName = "Service Level Name";
            afiServiceLevelName.IsCustom = false;
            afiServiceLevelName.IsVisible = true;
            afiServiceLevelName.ListValues = null;
            afiServiceLevelName.LookupTableID = null;
            afiServiceLevelName.Size = 0;
            afiServiceLevelName.Description = "";
            afiServiceLevelName.TableID = -2;
            afiServiceLevelName.RefType = ReferenceType.Tickets;
            afiServiceLevelName.AuxID = null;
            afiServiceLevelName.OtherTrigger = "organizations.SLAName";
            fieldItems.Add(afiServiceLevelName);

            AutoFieldItem afiServiceExpirationDate = new AutoFieldItem();
            afiServiceExpirationDate.Alias = "Service Expiration Date";
            afiServiceExpirationDate.DataType = "datetime";
            afiServiceExpirationDate.FieldID = 101013;
            afiServiceExpirationDate.FieldName = "Service Expiration Date";
            afiServiceExpirationDate.IsCustom = false;
            afiServiceExpirationDate.IsVisible = true;
            afiServiceExpirationDate.ListValues = null;
            afiServiceExpirationDate.LookupTableID = null;
            afiServiceExpirationDate.Size = 0;
            afiServiceExpirationDate.Description = "";
            afiServiceExpirationDate.TableID = -2;
            afiServiceExpirationDate.RefType = ReferenceType.Tickets;
            afiServiceExpirationDate.AuxID = null;
            afiServiceExpirationDate.OtherTrigger = "organizations.serviceexpirationdate";
            fieldItems.Add(afiServiceExpirationDate);

            AutoFieldItem afiCustomerIsActive = new AutoFieldItem();
            afiCustomerIsActive.Alias = "Customer Is Active";
            afiCustomerIsActive.DataType = "bit";
            afiCustomerIsActive.FieldID = 101014;
            afiCustomerIsActive.FieldName = "Customer Is Active";
            afiCustomerIsActive.IsCustom = false;
            afiCustomerIsActive.IsVisible = true;
            afiCustomerIsActive.ListValues = null;
            afiCustomerIsActive.LookupTableID = null;
            afiCustomerIsActive.Size = 0;
            afiCustomerIsActive.Description = "";
            afiCustomerIsActive.TableID = -2;
            afiCustomerIsActive.RefType = ReferenceType.Tickets;
            afiCustomerIsActive.AuxID = null;
            afiCustomerIsActive.OtherTrigger = "organizations.active";
            fieldItems.Add(afiCustomerIsActive);

            AutoFieldItem afiCustomerCustomValue = new AutoFieldItem();
            afiCustomerCustomValue.Alias = "Customer Custom Value";
            afiCustomerCustomValue.DataType = "text";
            afiCustomerCustomValue.FieldID = -999;
            afiCustomerCustomValue.FieldName = "Customer Custom Value";
            afiCustomerCustomValue.IsCustom = false;
            afiCustomerCustomValue.IsVisible = true;
            afiCustomerCustomValue.ListValues = orgCustomFields.ToArray();
            afiCustomerCustomValue.LookupTableID = null;
            afiCustomerCustomValue.Size = 0;
            afiCustomerCustomValue.Description = "";
            afiCustomerCustomValue.TableID = -2;
            afiCustomerCustomValue.RefType = ReferenceType.Tickets;
            afiCustomerCustomValue.AuxID = null;
            afiCustomerCustomValue.OtherTrigger = "organizations.customvalue";
            fieldItems.Add(afiCustomerCustomValue);

            AutoFieldItem afiCustomerServiceExpired = new AutoFieldItem();
            afiCustomerServiceExpired.Alias = "Customer Service Expired";
            afiCustomerServiceExpired.DataType = "bit";
            afiCustomerServiceExpired.FieldID = 101015;
            afiCustomerServiceExpired.FieldName = "Customer Service Expired";
            afiCustomerServiceExpired.IsCustom = false;
            afiCustomerServiceExpired.IsVisible = true;
            afiCustomerServiceExpired.ListValues = null;
            afiCustomerServiceExpired.LookupTableID = null;
            afiCustomerServiceExpired.Size = 0;
            afiCustomerServiceExpired.Description = "";
            afiCustomerServiceExpired.TableID = -2;
            afiCustomerServiceExpired.RefType = ReferenceType.Tickets;
            afiCustomerServiceExpired.AuxID = null;
            afiCustomerServiceExpired.OtherTrigger = "organizations.serviceexpired";
            fieldItems.Add(afiCustomerServiceExpired);

            result.Fields = fieldItems.ToArray();

      Users users = new Users(UserSession.LoginUser);
      users.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, true);
      List<AutocompleteItem> userItems = new List<AutocompleteItem>();
      foreach (User user in users)
      {
        userItems.Add(new AutocompleteItem(user.DisplayName, user.UserID.ToString()));
      }
      result.Users = userItems.ToArray();

      Groups groups = new Groups(UserSession.LoginUser);
      groups.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
      List<AutocompleteItem> groupItems = new List<AutocompleteItem>();
      foreach (Group group in groups)
      {
        groupItems.Add(new AutocompleteItem(group.Name, group.GroupID.ToString()));
      }
      result.Groups = groupItems.ToArray();

      TicketSeverities severities = new TicketSeverities(UserSession.LoginUser);
      severities.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
      List<AutocompleteItem> severityItems = new List<AutocompleteItem>();
      foreach (TicketSeverity severity in severities)
      {
        severityItems.Add(new AutocompleteItem(severity.Name, severity.TicketSeverityID.ToString()));
      }
      result.Severities = severityItems.ToArray();

      List<AutocompleteItem> statusItems = new List<AutocompleteItem>();

      List<AutocompleteItem> ticketTypeItems = new List<AutocompleteItem>();
      foreach (TicketType ticketType in ticketTypes)
      {
        ticketTypeItems.Add(new AutocompleteItem(ticketType.Name, ticketType.TicketTypeID.ToString()));


        TicketStatuses statuses = new TicketStatuses(UserSession.LoginUser);
        statuses.LoadAllPositions(ticketType.TicketTypeID);

        foreach (TicketStatus status in statuses)
        {
          statusItems.Add(new AutocompleteItem(ticketType.Name + " - " + status.Name, status.TicketStatusID.ToString()));
        }
      }
      result.Statuses = statusItems.ToArray();
      result.TicketTypes = ticketTypeItems.ToArray();
      return result;
    }

    [WebMethod]
    public TicketAutomationTriggerProxy[] GetTriggers()
    {
      TicketAutomationTriggers triggers = new TicketAutomationTriggers(UserSession.LoginUser);
      triggers.LoadByOrganization(UserSession.LoginUser.OrganizationID);
      return triggers.GetTicketAutomationTriggerProxies();
    }

    [WebMethod]
    public int CreateTrigger()
    {
      TicketAutomationTrigger trigger = (new TicketAutomationTriggers(UserSession.LoginUser)).AddNewTicketAutomationTrigger();
      trigger.Name = "New Trigger";
      trigger.Active = true;
      trigger.Position = 0;
      trigger.OrganizationID = UserSession.LoginUser.OrganizationID;
      trigger.UseCustomSQL = false;
      trigger.Collection.Save();
      return trigger.TriggerID;
    }

    [WebMethod]
    public TriggerData GetTrigger(int triggerID)
    {
      TicketAutomationTriggersViewItem trigger = TicketAutomationTriggersView.GetTicketAutomationTriggersViewItem(UserSession.LoginUser, triggerID);
      if (trigger.OrganizationID != UserSession.LoginUser.OrganizationID) return null;

      TicketAutomationActions actions = new TicketAutomationActions(UserSession.LoginUser);
      actions.LoadByTrigger(triggerID);

      TicketAutomationTriggerLogic logic = new TicketAutomationTriggerLogic(UserSession.LoginUser);
      logic.LoadByTrigger(triggerID);

      TriggerData result = new TriggerData();
      result.Trigger = trigger.GetProxy();
      result.LogicItems = logic.GetTicketAutomationTriggerLogicItemProxies();
      result.Actions = actions.GetTicketAutomationActionProxies();
      return result;
    }

    [WebMethod]
    public void DeleteTrigger(int triggerID)
    {
      TicketAutomationTrigger trigger = TicketAutomationTriggers.GetTicketAutomationTrigger(UserSession.LoginUser, triggerID);
      if (trigger.OrganizationID != UserSession.LoginUser.OrganizationID || !UserSession.CurrentUser.IsSystemAdmin) return;

      trigger.Delete();
      trigger.Collection.Save();
    }

    [WebMethod]
    public TicketAutomationTriggerProxy SaveTrigger(string value)
    {
      SaveTriggerData data = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveTriggerData>(value);
      HttpContext.Current.Request.InputStream.Position = 0;

      TicketAutomationTrigger trigger = null;
      if (data.TriggerID > -1)
      {
        trigger = TicketAutomationTriggers.GetTicketAutomationTrigger(UserSession.LoginUser, data.TriggerID);
        if (trigger.OrganizationID != UserSession.LoginUser.OrganizationID || !UserSession.CurrentUser.IsSystemAdmin) return null;
      }
      else
      {
        trigger = (new TicketAutomationTriggers(UserSession.LoginUser)).AddNewTicketAutomationTrigger();
        trigger.OrganizationID = UserSession.LoginUser.OrganizationID;
      }

      trigger.Active = data.IsActive;
      trigger.Name = data.Name;
      trigger.Position = trigger.Collection.GetMaxPosition(UserSession.LoginUser.OrganizationID) + 1;
      trigger.Collection.Save();

      TicketAutomationTriggerLogic logic = new TicketAutomationTriggerLogic(UserSession.LoginUser);
      logic.LoadByTrigger(data.TriggerID);
      foreach (TicketAutomationTriggerLogicItem logicItem in logic)
      {
        logicItem.Delete();
      }
      logic.Save();

      TicketAutomationActions actions = new TicketAutomationActions(UserSession.LoginUser);
      actions.LoadByTrigger(data.TriggerID);
      foreach (TicketAutomationAction action in actions)
      {
        action.Delete();
      }
      actions.Save();

      actions = new TicketAutomationActions(UserSession.LoginUser);
      foreach (TicketAutomationActionProxy actionProxy in data.Actions)
      {
        TicketAutomationAction action = actions.AddNewTicketAutomationAction();
        action.TriggerID = trigger.TriggerID;
        action.ActionValue = actionProxy.ActionValue;
        action.ActionValue2 = actionProxy.ActionValue2;
        action.ActionID = actionProxy.ActionID;
      }
      actions.Save();

      logic = new TicketAutomationTriggerLogic(UserSession.LoginUser);
      foreach (LogicItem logicItemProxy in data.LogicItems)
      {
        TicketAutomationTriggerLogicItem logicItem = logic.AddNewTicketAutomationTriggerLogicItem();
        logicItem.TriggerID = trigger.TriggerID;
        logicItem.TableID = logicItemProxy.IsCustom ? -1 : logicItemProxy.TableID;
        logicItem.FieldID = logicItemProxy.FieldID;
        logicItem.TestValue = logicItemProxy.TestValue;
        logicItem.Measure = logicItemProxy.Measure;
        logicItem.MatchAll = logicItemProxy.MatchAll;
        logicItem.OtherTrigger = logicItemProxy.OtherTrigger;
      }
      logic.Save();

      return trigger.GetProxy();
    }

    [WebMethod]
    public TicketAutomationActionProxy[] GetActionProxies() { return null; }

    [WebMethod]
    public TicketAutomationTriggerLogicItemProxy[] GetLogicProxies() { return null; }
  }

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class TriggerData
  {
    [DataMember]
    public TicketAutomationTriggersViewItemProxy Trigger { get; set; }
    [DataMember]
    public TicketAutomationActionProxy[] Actions { get; set; }
    [DataMember]
    public TicketAutomationTriggerLogicItemProxy[] LogicItems { get; set; }
  }

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class SaveTriggerData
  {
    public SaveTriggerData()
    {
    }
    [DataMember]
    public int TriggerID { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public bool IsActive { get; set; }
    [DataMember]
    public List<TicketAutomationActionProxy> Actions { get; set; }
    [DataMember]
    public List<LogicItem> LogicItems { get; set; }
  }


  [DataContract(Namespace = "http://teamsupport.com/")]
  public class AutomationData
  {
    [DataMember]
    public TicketAutomationPossibleActionProxy[] Actions { get; set; }
    [DataMember]
    public AutocompleteItem[] Statuses { get; set; }
    [DataMember]
    public AutocompleteItem[] Severities { get; set; }
    [DataMember]
    public AutocompleteItem[] TicketTypes { get; set; }
    [DataMember]
    public AutocompleteItem[] Users { get; set; }
    [DataMember]
    public AutocompleteItem[] Groups { get; set; }
    [DataMember]
    public AutoFieldItem[] Fields { get; set; }
    [DataMember]
    public AutoFieldItem[] CustomFields { get; set; }
  }

  [DataContract(Namespace = "http://teamsupport.com/")]
  public class LogicItem
  {
    [DataMember] public int TriggerLogicID { get; set; }
    [DataMember] public int TriggerID { get; set; }
    [DataMember] public int TableID { get; set; }
    [DataMember] public int FieldID { get; set; }
    [DataMember] public string FieldName { get; set; }
    [DataMember] public string Measure { get; set; }
    [DataMember] public string TestValue { get; set; }
    [DataMember] public bool IsCustom { get; set; }
    [DataMember] public bool MatchAll { get; set; }
    [DataMember] public string OtherTrigger { get; set; }
    
  }
  [DataContract(Namespace = "http://teamsupport.com/")]
  [KnownType(typeof(AutoFieldItem))]
  public class AutoFieldItem
  {
    public AutoFieldItem() { }

    public AutoFieldItem(CustomField field) : this(field, field.Name){}

    public AutoFieldItem(CustomField field, string aliasName) 
    {
      this.FieldID = field.CustomFieldID;
      this.RefType = ReferenceType.Tickets;
      this.AuxID = field.AuxID;
      this.IsCustom = true;
      this.FieldName = field.Name;
      this.Alias = aliasName;

      switch (field.FieldType)
      {
        case CustomFieldType.Text:
          this.DataType = "text";
          break;
        case CustomFieldType.Date:
        case CustomFieldType.Time:
        case CustomFieldType.DateTime:
          this.DataType = "datetime";
          break;
        case CustomFieldType.Boolean:
          this.DataType = "bit";
          break;
        case CustomFieldType.Number:
          this.DataType = "text";
          break;
        case CustomFieldType.PickList:
          this.DataType = "list";
          break;
        default:
          this.DataType = "text";
          break;
      }
      this.Size = 0;
      this.IsVisible = true;
      this.Description = field.Description;
      this.TableID = (int)field.RefType;
      this.LookupTableID = null;
      this.ListValues = field.ListValues.Split('|');
    }

    public AutoFieldItem(ReportTableField field) 
    {
      this.FieldID = field.ReportTableFieldID;
      this.RefType = ReferenceType.Tickets;
      this.AuxID = null;
      this.IsCustom = false;
      this.ListValues = null;
      this.FieldName = field.FieldName;
      this.Alias = field.Alias;
      this.DataType = field.DataType;
      this.Size = field.Size;
      this.TableID = field.ReportTableID;
      this.IsVisible = field.IsVisible;
      this.Description = field.Description;
      this.LookupTableID = field.LookupTableID;
    }

    [DataMember] public int FieldID { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
    [DataMember] public int? AuxID { get; set; }
    [DataMember] public bool IsCustom { get; set; }
    [DataMember] public string[] ListValues { get; set; }
    [DataMember] public string FieldName { get; set; }
    [DataMember] public string Alias { get; set; }
    [DataMember] public string DataType { get; set; }
    [DataMember] public int Size { get; set; }
    [DataMember] public int TableID { get; set; }
    [DataMember] public bool IsVisible { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public int? LookupTableID { get; set; }
    [DataMember] public string OtherTrigger { get; set; }

  }

}