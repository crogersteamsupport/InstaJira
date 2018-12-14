using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.WebUtils;
using TeamSupport.Data;
using Telerik.Web.UI;

public partial class Dialogs_SlaTrigger : BaseDialogPage
{
  enum TimeUnit { Minutes, Hours, Days }

  private int _slaTriggerID = -1;
  private int _slaLevelID = -1;
  private int _ticketTypeID = -1;
  private SlaTrigger _trigger = null;

  protected override void OnLoad(EventArgs e)
  {
    if (!UserSession.CurrentUser.IsSystemAdmin)
    {
      Response.Write("Invalid Request");
      Response.End();
      return;
    }


    base.OnLoad(e);

    if (Request["SlaTriggerID"] != null) _slaTriggerID = int.Parse(Request["SlaTriggerID"]);
    if (Request["SlaLevelID"] != null) _slaLevelID = int.Parse(Request["SlaLevelID"]);
    if (Request["TicketTypeID"] != null) _ticketTypeID = int.Parse(Request["TicketTypeID"]);

    if (!IsPostBack)
    {
      LoadTimeZones();
      SetTimes(Settings.UserDB.ReadInt("SlaTriggerWarningTime", 1440),
         Settings.UserDB.ReadInt("SlaTriggerInitialResponseTime", 60),
         Settings.UserDB.ReadInt("SlaTriggerLastActionTime", 60),
         Settings.UserDB.ReadInt("SlaTriggerClosedTime", 60));

      cbGroupViolations.Checked = Settings.UserDB.ReadBool("SlaTriggerGroupViolations", true);
      cbGroupWarnings.Checked = Settings.UserDB.ReadBool("SlaTriggerGroupWarnings", true);
      cbUserViolations.Checked = Settings.UserDB.ReadBool("SlaTriggerUserViolations", true);
      cbUserWarnings.Checked = Settings.UserDB.ReadBool("SlaTriggerUserWarnings", true);
      rbBusinessHours.Checked = Settings.UserDB.ReadBool("SlaTriggerUseBusinessHours", true);
      rbCustomBusinessHours.Checked = !rbBusinessHours.Checked;
      cbPauseOnOrganizationHolidays.Checked = Settings.UserDB.ReadBool("SlaTriggerPauseOnOrganizationHolidays", true);
      daysToPauseList.Attributes.Add("onkeydown", "DeleteSelectedItems(event);");
    }

    if (_slaTriggerID > -1)
    {
      _trigger = SlaTriggers.GetSlaTrigger(UserSession.LoginUser, _slaTriggerID);
      if (_trigger == null || SlaLevels.GetSlaLevel(UserSession.LoginUser, _trigger.SlaLevelID).OrganizationID != UserSession.LoginUser.OrganizationID)
      {
        Response.Write("Invalid Request");
        Response.End();
        return;
      }
      _slaLevelID = _trigger.SlaLevelID;
      _ticketTypeID = _trigger.TicketTypeID;

      if (!IsPostBack) LoadTrigger(_trigger);
    }
    else
    {
        DisableEnableCustomBusinessHours(rbBusinessHours.Checked, rbNoBusinessHours.Checked);
    }

    SlaLevel level =SlaLevels.GetSlaLevel(UserSession.LoginUser, _slaLevelID);
    TicketType type = TicketTypes.GetTicketType(UserSession.LoginUser, _ticketTypeID);
    if (level == null || type == null || level.OrganizationID != UserSession.LoginUser.OrganizationID || type.OrganizationID != UserSession.LoginUser.OrganizationID)
    {
      Response.Write("Invalid Request");
      Response.End();
      return;
    }

    lblSla.Text = level.Name;
    lblTicketType.Text = type.Name;

    if (!IsPostBack) LoadSeverities();
  }

  private void LoadSeverities()
  {
    TicketSeverities severities = new TicketSeverities(UserSession.LoginUser);
    severities.LoadNotSlaTriggers(UserSession.LoginUser.OrganizationID, _slaLevelID, _ticketTypeID);
    cmbSeverities.Items.Clear();
    if (_trigger != null) 
    {
      TicketSeverity selected = TicketSeverities.GetTicketSeverity(UserSession.LoginUser, _trigger.TicketSeverityID);
      cmbSeverities.Items.Add(new RadComboBoxItem(selected.Name, selected.TicketSeverityID.ToString()));
    }

    foreach (TicketSeverity severity in severities)
    {
      cmbSeverities.Items.Add(new RadComboBoxItem(severity.Name, severity.TicketSeverityID.ToString()));
    }

    if (cmbSeverities.Items.Count > 0) cmbSeverities.SelectedIndex = 0;

  }
  
  private void LoadTrigger(SlaTrigger trigger)
  {
    SetTimes(trigger.WarningTime, trigger.TimeInitialResponse, trigger.TimeLastAction, trigger.TimeToClose);
    cbGroupViolations.Checked = trigger.NotifyGroupOnViolation;
    cbGroupWarnings.Checked = trigger.NotifyGroupOnWarning;
    cbUserViolations.Checked = trigger.NotifyUserOnViolation;
    cbUserWarnings.Checked = trigger.NotifyUserOnWarning;
    rbNoBusinessHours.Checked = trigger.NoBusinessHours;
    rbBusinessHours.Checked = trigger.UseBusinessHours;
    timeSLAStart.SelectedDate = (trigger.DayStartUtc != null ? TimeZoneInfo.ConvertTimeFromUtc(trigger.DayStartUtc.Value, TimeZoneInfo.FindSystemTimeZoneById(trigger.TimeZone)) : new DateTime?());
    timeSLAEnd.SelectedDate = (trigger.DayEndUtc != null ? TimeZoneInfo.ConvertTimeFromUtc(trigger.DayEndUtc.Value, TimeZoneInfo.FindSystemTimeZoneById(trigger.TimeZone)) : new DateTime?());
    cbTimeZones.SelectedValue = trigger.TimeZone;
    cbSLASunday.Checked = ((int)trigger.Weekdays & (int)Math.Pow(2, (int)DayOfWeek.Sunday)) > 0;
    cbSLAMonday.Checked = ((int)trigger.Weekdays & (int)Math.Pow(2, (int)DayOfWeek.Monday)) > 0;
    cbSLATuesday.Checked = ((int)trigger.Weekdays & (int)Math.Pow(2, (int)DayOfWeek.Tuesday)) > 0;
    cbSLAWednesday.Checked = ((int)trigger.Weekdays & (int)Math.Pow(2, (int)DayOfWeek.Wednesday)) > 0;
    cbSLAThursday.Checked = ((int)trigger.Weekdays & (int)Math.Pow(2, (int)DayOfWeek.Thursday)) > 0;
    cbSLAFriday.Checked = ((int)trigger.Weekdays & (int)Math.Pow(2, (int)DayOfWeek.Friday)) > 0;
    cbSLASaturday.Checked = ((int)trigger.Weekdays & (int)Math.Pow(2, (int)DayOfWeek.Saturday)) > 0;
    DisableEnableCustomBusinessHours(trigger.UseBusinessHours, trigger.NoBusinessHours);

    cbPauseOnOrganizationHolidays.Checked = trigger.PauseOnHoliday;
    List<DateTime> daysToPause = SlaTriggers.GetSpecificDaysToPause(trigger.SlaTriggerID);
    DaysToPauseHidden.Value = string.Join(",", daysToPause.Select(p => DataUtils.DateToLocal(UserSession.LoginUser, p).ToString("d")));
    LoginUser loginUser = TSAuthentication.GetLoginUser();

    foreach(DateTime dayToPause in daysToPause.OrderBy(p => p))
    {
        daysToPauseList.Items.Add(new ListItem {
                                                Value = DataUtils.DateToLocal(UserSession.LoginUser, dayToPause).ToString("d"),
                                                Text = DataUtils.DateToLocal(UserSession.LoginUser, dayToPause).ToString("d", loginUser.CultureInfo)
                                               });
    }
  }

  private void SetTimes(int warning, int initial, int action, int closed)
  {
    TimeUnit timeUnit;
    timeUnit = GetTimeUnit(warning);
    numWarning.Value = GetTimeNumber(warning, timeUnit);
    rbWarningMin.Checked = timeUnit == TimeUnit.Minutes;
    rbWarningHour.Checked = timeUnit == TimeUnit.Hours;
    rbWarningDay.Checked = timeUnit == TimeUnit.Days;

    timeUnit = GetTimeUnit(initial);
    numInitial.Value = GetTimeNumber(initial, timeUnit);
    rbInitialMin.Checked = timeUnit == TimeUnit.Minutes;
    rbInitialHour.Checked = timeUnit == TimeUnit.Hours;
    rbInitialDay.Checked = timeUnit == TimeUnit.Days;

    timeUnit = GetTimeUnit(action);
    numAction.Value = GetTimeNumber(action, timeUnit);
    rbActionMin.Checked = timeUnit == TimeUnit.Minutes;
    rbActionHour.Checked = timeUnit == TimeUnit.Hours;
    rbActionDay.Checked = timeUnit == TimeUnit.Days;

    timeUnit = GetTimeUnit(closed);
    numClosed.Value = GetTimeNumber(closed, timeUnit);
    rbClosedMin.Checked = timeUnit == TimeUnit.Minutes;
    rbClosedHour.Checked = timeUnit == TimeUnit.Hours;
    rbClosedDay.Checked = timeUnit == TimeUnit.Days;
  
  }

  private TimeUnit GetTimeUnit(int minutes)
  {
    if (minutes < 60) return TimeUnit.Minutes;
    if (minutes < 24 * 60) return TimeUnit.Hours;
    return TimeUnit.Days;
  }

  private int GetTimeNumber(int minutes)
  {
    return GetTimeNumber(minutes, GetTimeUnit(minutes)); 
  }

  private int GetTimeNumber(int minutes, TimeUnit unit)
  {
    switch (unit)
    {
      case TimeUnit.Minutes: return minutes;
      case TimeUnit.Hours: return minutes / 60;
      case TimeUnit.Days: return minutes / (60 * 24);
      default: break;
    }
    return 0;
  }

  public override bool Save()
  {
    if (rbCustomBusinessHours.Checked && (timeSLAStart.SelectedDate == null || timeSLAEnd.SelectedDate == null))
    {
        string script = "alert('Please select both the SLA Day Start and End.');";
        string name = "ValidationAlert";
        ScriptManager.RegisterClientScriptBlock(this, typeof(Page), name + "_function", "function " + name + "(){" + script + "Sys.Application.remove_load(" + name + ");}", true);
        ScriptManager.RegisterStartupScript(this, typeof(Page), name, "Sys.Application.add_load(" + name + ");", true);
        DialogResult = "";
        return false;
    }

    SlaTrigger trigger;
    SlaTriggers triggers = new SlaTriggers(UserSession.LoginUser);

    if (_slaTriggerID < 0)
    {
      trigger = triggers.AddNewSlaTrigger();
      trigger.SlaLevelID = _slaLevelID;
      trigger.TicketTypeID = _ticketTypeID;
    }
    else
    {
      trigger = SlaTriggers.GetSlaTrigger(UserSession.LoginUser, _slaTriggerID);
      if (trigger == null) return false;
    }

    trigger.TicketSeverityID = int.Parse(cmbSeverities.SelectedValue);
    trigger.NotifyGroupOnViolation = cbGroupViolations.Checked;
    trigger.NotifyGroupOnWarning = cbGroupWarnings.Checked;
    trigger.NotifyUserOnViolation = cbUserViolations.Checked;
    trigger.NotifyUserOnWarning = cbUserWarnings.Checked;
    trigger.NoBusinessHours = rbNoBusinessHours.Checked;
    trigger.UseBusinessHours = rbBusinessHours.Checked;
    trigger.PauseOnHoliday = cbPauseOnOrganizationHolidays.Checked;
    trigger.DayStart = null;
	trigger.DayEnd = null;
    trigger.TimeZone = cbTimeZones.SelectedValue;
    UserSession.LoginUser.TimeZoneInfo = null;

    try
    {
        TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(trigger.TimeZone);

        if (timeSLAStart.SelectedDate != null)
        {
            trigger.DayStart = TimeZoneInfo.ConvertTimeToUtc((DateTime)timeSLAStart.SelectedDate, timeZoneInfo);
        }

        if (timeSLAEnd.SelectedDate != null)
        {
            trigger.DayEnd = TimeZoneInfo.ConvertTimeToUtc((DateTime)timeSLAEnd.SelectedDate, timeZoneInfo);
        }
    }
    catch (Exception ex)
    {
    }

    int weekdays = 0;
    AddSLADay(ref weekdays, DayOfWeek.Sunday, cbSLASunday.Checked);
    AddSLADay(ref weekdays, DayOfWeek.Monday, cbSLAMonday.Checked);
    AddSLADay(ref weekdays, DayOfWeek.Tuesday, cbSLATuesday.Checked);
    AddSLADay(ref weekdays, DayOfWeek.Wednesday, cbSLAWednesday.Checked);
    AddSLADay(ref weekdays, DayOfWeek.Thursday, cbSLAThursday.Checked);
    AddSLADay(ref weekdays, DayOfWeek.Friday, cbSLAFriday.Checked);
    AddSLADay(ref weekdays, DayOfWeek.Saturday, cbSLASaturday.Checked);
    trigger.Weekdays = weekdays;

    List<string> daysToPause = new List<string>();

    if (!String.IsNullOrEmpty(DaysToPauseHidden.Value))
    {
        daysToPause = DaysToPauseHidden.Value.Split(',').Distinct().Where(p => !string.IsNullOrEmpty(p)).ToList();
    }

    SlaPausedDays slaPausedDays = new SlaPausedDays(UserSession.LoginUser);

    if (_slaTriggerID > 0)
    {
        slaPausedDays.LoadByTriggerID(trigger.SlaTriggerID);

        if (slaPausedDays != null && slaPausedDays.Any())
        {
            slaPausedDays.DeleteAll();
            slaPausedDays.Save();
        }

        slaPausedDays = new SlaPausedDays(UserSession.LoginUser);
    }

    int value = numWarning.Value == null ? 0 : (int)numWarning.Value;

    if (rbWarningDay.Checked) trigger.WarningTime = value * 24 * 60;
    else if (rbWarningHour.Checked) trigger.WarningTime = value * 60;
    else trigger.WarningTime = value;

    value = numInitial.Value == null ? 0 : (int)numInitial.Value;
    if (rbInitialDay.Checked) trigger.TimeInitialResponse = value * 24 * 60;
    else if (rbInitialHour.Checked) trigger.TimeInitialResponse = value * 60;
    else trigger.TimeInitialResponse = value;

    value = numAction.Value == null ? 0 : (int)numAction.Value;
    if (rbActionDay.Checked) trigger.TimeLastAction = value * 24 * 60;
    else if (rbActionHour.Checked) trigger.TimeLastAction = value * 60;
    else trigger.TimeLastAction = value;

    value = numClosed.Value == null ? 0 : (int)numClosed.Value;
    if (rbClosedDay.Checked) trigger.TimeToClose = value * 24 * 60;
    else if (rbClosedHour.Checked) trigger.TimeToClose = value * 60;
    else trigger.TimeToClose = value;

    try
    {
        trigger.Collection.Save();

        foreach (string day in daysToPause)
        {
            string decodedDay = System.Net.WebUtility.HtmlDecode(day);
            decodedDay = System.Text.RegularExpressions.Regex.Replace(decodedDay, @"[^\u0000-\u007F]+", string.Empty);
            decodedDay = System.Text.RegularExpressions.Regex.Replace(decodedDay, "[^.0-9/\\s]", "");

            DateTime dayToPause = new DateTime();
            Settings.UserDB.WriteString("SlaTriggerDayToPauseDebug", day + " => " + decodedDay);

            if (DateTime.TryParse(decodedDay, new CultureInfo("en-US"), DateTimeStyles.None, out dayToPause))
            {
                SlaPausedDay slaPausedDay = slaPausedDays.AddNewSlaPausedDay();
                slaPausedDay.SlaTriggerId = trigger.SlaTriggerID;
                slaPausedDay.DateToPause = dayToPause.ToUniversalTime();
                slaPausedDay.Collection.Save();
                Settings.UserDB.WriteString("SlaTriggerDayToPauseParseDebug", "us-US: " + dayToPause.ToShortDateString());
            }
            else if (DateTime.TryParse(decodedDay, TSAuthentication.GetLoginUser().CultureInfo, DateTimeStyles.None, out dayToPause))
            {
                SlaPausedDay slaPausedDay = slaPausedDays.AddNewSlaPausedDay();
                slaPausedDay.SlaTriggerId = trigger.SlaTriggerID;
                slaPausedDay.DateToPause = dayToPause.ToUniversalTime();
                slaPausedDay.Collection.Save();
                Settings.UserDB.WriteString("SlaTriggerDayToPauseParseDebug", TSAuthentication.GetLoginUser().CultureInfo + ": " + dayToPause.ToShortDateString());
            }
            else
            {
                Settings.UserDB.WriteString("SlaTriggerDayToPauseParseDebug", "none");
            }
        }

        Settings.UserDB.WriteInt("SlaTriggerWarningTime", trigger.WarningTime);
        Settings.UserDB.WriteInt("SlaTriggerInitialResponseTime", trigger.TimeInitialResponse);
        Settings.UserDB.WriteInt("SlaTriggerLastActionTime", trigger.TimeLastAction);
        Settings.UserDB.WriteInt("SlaTriggerClosedTime", trigger.TimeToClose);
        Settings.UserDB.WriteBool("SlaTriggerGroupViolations", cbGroupViolations.Checked);
        Settings.UserDB.WriteBool("SlaTriggerGroupWarnings", cbGroupWarnings.Checked);
        Settings.UserDB.WriteBool("SlaTriggerUserViolations", cbUserViolations.Checked);
        Settings.UserDB.WriteBool("SlaTriggerUserWarnings", cbUserWarnings.Checked);
        Settings.UserDB.WriteBool("SlaTriggerNoBusinessHours", rbNoBusinessHours.Checked);
        Settings.UserDB.WriteBool("SlaTriggerUseBusinessHours", rbBusinessHours.Checked);
        Settings.UserDB.WriteBool("SlaTriggerPauseOnOrganizationHolidays", cbPauseOnOrganizationHolidays.Checked);

        DialogResult = trigger.SlaTriggerID.ToString();
    }
    catch (System.Data.SqlClient.SqlException sqlEx)
    {
        if (sqlEx.Message.Contains("duplicate key") && sqlEx.Message.Contains("SlaTriggers"))
        {
            ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), sqlEx, "SlaTrigger.Save");
        }
        else
        {
            throw sqlEx;
        }
    }
    
    return true;
  }

    private static void AddSLADay(ref int slaDays, DayOfWeek dayOfWeek, bool add)
    {
        if (add)
        {
            slaDays = slaDays | (int)Math.Pow(2, (int)dayOfWeek);
        }
    }

    private void LoadTimeZones()
    {
        cbTimeZones.Items.Clear();

        System.Collections.ObjectModel.ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
        foreach (TimeZoneInfo info in timeZones)
        {
            cbTimeZones.Items.Add(new RadComboBoxItem(info.DisplayName, info.Id));
        }
    }

    private void DisableEnableCustomBusinessHours(bool useAccountBusinessHours, bool noBusinessHours)
    {
        if (noBusinessHours)
        {
            rbBusinessHours.Checked = false;
            rbCustomBusinessHours.Checked = false;
            cbTimeZones.Enabled = false;
            timeSLAStart.Enabled = false;
            timeSLAEnd.Enabled = false;
            cbSLASunday.Enabled = false;
            cbSLAMonday.Enabled = false;
            cbSLATuesday.Enabled = false;
            cbSLAWednesday.Enabled = false;
            cbSLAThursday.Enabled = false;
            cbSLAFriday.Enabled = false;
            cbSLASaturday.Enabled = false;
        }
        else
        {
            rbNoBusinessHours.Checked = false;
            rbCustomBusinessHours.Checked = !useAccountBusinessHours;
            cbTimeZones.Enabled = !useAccountBusinessHours;
            timeSLAStart.Enabled = !useAccountBusinessHours;
            timeSLAEnd.Enabled = !useAccountBusinessHours;
            cbSLASunday.Enabled = !useAccountBusinessHours;
            cbSLAMonday.Enabled = !useAccountBusinessHours;
            cbSLATuesday.Enabled = !useAccountBusinessHours;
            cbSLAWednesday.Enabled = !useAccountBusinessHours;
            cbSLAThursday.Enabled = !useAccountBusinessHours;
            cbSLAFriday.Enabled = !useAccountBusinessHours;
            cbSLASaturday.Enabled = !useAccountBusinessHours;
        }
    }
}

