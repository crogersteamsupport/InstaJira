using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;
using System.Web.Services;
using System.Text;

public partial class Frames_AdminSla : BaseFramePage
{
  public int SelectedTicketTypeID
  {
    get { return Settings.Session.ReadInt("SelectedSlaAdminTicketTypeID", -1); }
    set { Settings.Session.WriteInt("SelectedSlaAdminTicketTypeID", value); }
  }

  public int SelectedSlaLevelID
  {
    get { return Settings.Session.ReadInt("SelectedAdminSlaLevelID", -1); }
    set { Settings.Session.WriteInt("SelectedAdminSlaLevelID", value); }
  }

  [WebMethod]
  public static RadComboBoxItemData[] GetComboLevels()
  {
    SlaLevels levels = new SlaLevels(UserSession.LoginUser);
    levels.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);

    List<RadComboBoxItemData> data = new List<RadComboBoxItemData>();
    foreach (SlaLevel level in levels)
    {
      RadComboBoxItemData item = new RadComboBoxItemData();
      item.Text = level.Name;
      item.Value = level.SlaLevelID.ToString();
      data.Add(item);
    }

    return data.ToArray();
  }

  [WebMethod]
  public static void DeleteLevel(int slaLevelID)
  {
    SlaLevel level = SlaLevels.GetSlaLevel(UserSession.LoginUser, slaLevelID);
    if (level != null && level.OrganizationID == UserSession.LoginUser.OrganizationID && UserSession.CurrentUser.IsSystemAdmin)
    {
      level.Delete();
      level.Collection.Save();
    }
  }

  [WebMethod]
  public static void DeleteTrigger(int slaTriggerID)
  {
    SlaTrigger trigger = SlaTriggers.GetSlaTrigger(UserSession.LoginUser, slaTriggerID);
    SlaLevel level = SlaLevels.GetSlaLevel(UserSession.LoginUser, trigger.SlaLevelID);
    if (trigger != null && level.OrganizationID == UserSession.LoginUser.OrganizationID && UserSession.CurrentUser.IsSystemAdmin)
    {
      trigger.Delete();
      trigger.Collection.Save();
    }
  }

  [WebMethod]
  public static string GetTriggersHtml(int slaLevelID, int ticketTypeID)
  {
    return GetTriggerTable(slaLevelID, ticketTypeID);
  }

  public static string GetTriggerTable(int slaLevelID, int ticketTypeID)
  {
    SlaTriggersView triggers = new SlaTriggersView(UserSession.LoginUser);
    triggers.LoadByTicketType(UserSession.LoginUser.OrganizationID, slaLevelID, ticketTypeID);

    StringBuilder builder = new StringBuilder();

    builder.Append("<table width=\"100%\" border=\"0\" cellpadding=\"5\" cellspacing=\"0\"><thead><tr><th /><th /><th>Severity</th><th>Initial Response</th><th>Last Action</th><th>To Closed</th><th>Warning Time</th><th>Pause on Company Holidays</th><th>Business Hours</th><th>Days of Week</th><th>Start Time</th><th>End Time</th><th>Time Zone</th></tr></thead><tbody>");

    foreach (SlaTriggersViewItem item in triggers)
    {
      string s = "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td><td>{9}</td><td>{10}</td><td>{11}</td><td>{12}</td></tr>";
      string edit = "<img src=\"../images/icons/Edit.png\" alt=\"Edit\" onclick=\"EditTrigger(" + item.SlaTriggerID + ");\" />";
      string delete = "<img src=\"../images/icons/Trash.png\" alt=\"Delete\" onclick=\"DeleteTrigger(" + item.SlaTriggerID + ");\" />";
      builder.Append(string.Format(s, 
        UserSession.CurrentUser.IsSystemAdmin ? edit : "",
        UserSession.CurrentUser.IsSystemAdmin ? delete : "",
        item.Severity,
        DataUtils.MinutesToDisplayTime(item.TimeInitialResponse, "0"),
        DataUtils.MinutesToDisplayTime(item.TimeLastAction, "0"),
        DataUtils.MinutesToDisplayTime(item.TimeToClose, "0"),
        DataUtils.MinutesToDisplayTime(item.WarningTime, "0"),
        item.PauseOnHoliday.ToString(),
        (item.NoBusinessHours) ? "No" : (item.UseBusinessHours) ? "Account" : "Custom",
        (!item.NoBusinessHours && !item.UseBusinessHours) ? GetDays(item.Weekdays) : "",
        (!item.NoBusinessHours && !item.UseBusinessHours) ? (item.DayStart != null ? TimeZoneInfo.ConvertTimeFromUtc(item.DayStartUtc.Value, TimeZoneInfo.FindSystemTimeZoneById(item.TimeZone)).ToString("hh:mm tt") : "") : "",
        (!item.NoBusinessHours && !item.UseBusinessHours) ? (item.DayEnd != null ? TimeZoneInfo.ConvertTimeFromUtc(item.DayEndUtc.Value, TimeZoneInfo.FindSystemTimeZoneById(item.TimeZone)).ToString("hh:mm tt") : "") : "",
        (!item.NoBusinessHours && !item.UseBusinessHours) ? item.TimeZone : ""
        ));
    }

    if (triggers.Count < 1)
    {
      builder.Append("<tr><td colspan=\"13\" >There are no triggers to display.</td></tr>");
    
    }

    builder.Append("</tbody></table>");
    return builder.ToString();
  }

    private static string GetDays(int weedays)
    {
        string days = string.Empty;

        if (((int)weedays & (int)Math.Pow(2, (int)DayOfWeek.Sunday)) > 0)
        {
            days += DayOfWeek.Sunday.ToString();
        }

        if (((int)weedays & (int)Math.Pow(2, (int)DayOfWeek.Monday)) > 0)
        {
            days += (days.Length > 0 ? "," : "") + DayOfWeek.Monday.ToString();
        }

        if (((int)weedays & (int)Math.Pow(2, (int)DayOfWeek.Tuesday)) > 0)
        {
            days += (days.Length > 0 ? "," : "") + DayOfWeek.Tuesday.ToString();
        }

        if (((int)weedays & (int)Math.Pow(2, (int)DayOfWeek.Wednesday)) > 0)
        {
            days += (days.Length > 0 ? "," : "") + DayOfWeek.Wednesday.ToString();
        }

        if (((int)weedays & (int)Math.Pow(2, (int)DayOfWeek.Thursday)) > 0)
        {
            days += (days.Length > 0 ? "," : "") + DayOfWeek.Thursday.ToString();
        }

        if (((int)weedays & (int)Math.Pow(2, (int)DayOfWeek.Friday)) > 0)
        {
            days += (days.Length > 0 ? "," : "") + DayOfWeek.Friday.ToString();
        }

        if (((int)weedays & (int)Math.Pow(2, (int)DayOfWeek.Saturday)) > 0)
        {
            days += (days.Length > 0 ? "," : "") + DayOfWeek.Saturday.ToString();
        }

        return days;
    }

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);

    LoadTicketTypes();
    LoadLevels();
    if (SelectedTicketTypeID > -1) cmbTicketTypes.SelectedValue = SelectedTicketTypeID.ToString();
    if (SelectedSlaLevelID > -1) cmbLevels.SelectedValue = SelectedSlaLevelID.ToString();
    try 
    { 
      divTriggers.InnerHtml = GetTriggerTable(int.Parse(cmbLevels.SelectedValue), int.Parse(cmbTicketTypes.SelectedValue)); 
    }
    catch (Exception)
    { 
      divTriggers.InnerHtml = GetTriggerTable(-1, -1); 
    }
    
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    try
    {
      SelectedSlaLevelID = int.Parse(cmbLevels.SelectedValue);
      SelectedTicketTypeID = int.Parse(cmbTicketTypes.SelectedValue);
    }
    catch(Exception){}
    lnkAddTrigger.Visible = UserSession.CurrentUser.IsSystemAdmin;
    tbMain.Visible = UserSession.CurrentUser.IsSystemAdmin;
    if (tbMain.Visible == false) paneToolbar.Height = new Unit(1, UnitType.Pixel);
  }

  private void LoadLevels()
  {
    cmbLevels.Items.Clear();
    SlaLevels levels = new SlaLevels(UserSession.LoginUser);
    levels.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);

    foreach (SlaLevel level in levels)
    {
      cmbLevels.Items.Add(new RadComboBoxItem(level.Name, level.SlaLevelID.ToString()));
    }
    if (cmbLevels.Items.Count > 0) cmbLevels.SelectedIndex = 0;
  }

  private void LoadTicketTypes()
  {
    cmbTicketTypes.Items.Clear();
    TicketTypes types = new TicketTypes(UserSession.LoginUser);
    types.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, UserSession.CurrentUser.ProductType);
    foreach (TicketType type in types)
    {
      cmbTicketTypes.Items.Add(new RadComboBoxItem(type.Name, type.TicketTypeID.ToString()));
    }
    if (cmbTicketTypes.Items.Count > 0) cmbTicketTypes.SelectedIndex = 0;
  }







}
