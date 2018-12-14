using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Web.Services;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Telerik.Web.UI;

public partial class Frames_Dashboard : System.Web.UI.Page
{
  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    Page.Culture = UserSession.LoginUser.CultureInfo.Name;
  }

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender(e);
  }

  public static void SavePortlet(Portlet portlet)
  {
    Settings.UserDB.WriteJson<Portlet>("DashboardPortlet-" + portlet.ID, portlet);
  }

  public static Portlet LoadPortlet(string id)
  {
    return Settings.UserDB.ReadJson<Portlet>("DashboardPortlet-" + id);
  }

  public static string GetPortletID(int reportID)
  {
    return "portlet" + reportID;
  }

  private static DataTable GetReportDataTable(Report report)
  {
    using (SqlConnection connection = new SqlConnection(UserSession.LoginUser.ConnectionString))
    {
      // Pull repoprt data in preparation to apply column order and sort.
      ReportData reportData = new ReportData(UserSession.LoginUser);
      reportData.LoadReportData(report.ReportID, UserSession.LoginUser.UserID);
      Pair[] columnOrder = null;

      if (!reportData.IsEmpty)
      {
        LosFormatter formatter = new LosFormatter();
        StringReader reader = new StringReader(reportData[0].ReportData);
        object[] gridSettings = (object[])formatter.Deserialize(reader);
        columnOrder = (Pair[])gridSettings[2];

        if(string.IsNullOrEmpty(reportData[0].OrderByClause))
        {
          // here is where we need to get the order by clause and initialize the report data.
          // this most be in the first version of the code.
          string sortColumn = null;
          string sortDirection = null;
          TSUtils.GetSortColumnAndDirection((ArrayList)gridSettings[1], ref sortColumn, ref sortDirection);
          if (sortColumn != null)
          {
            string sortArgumentSuffix = null;
            if (string.Compare(sortDirection, "Descending", StringComparison.OrdinalIgnoreCase) == 0)
            {
              sortArgumentSuffix = " DESC";
            }
            else
            {
              sortArgumentSuffix = " ASC";
            }

            reportData[0].OrderByClause = sortColumn + sortArgumentSuffix;
            reportData[0].Collection.Save();
          }

        }
      }

      // Get Table
      string query = report.GetSqlWithOrderByClause(false, null);
      query = query.Replace("SELECT ", "SELECT TOP 100 ");
      SqlCommand command = new SqlCommand(query, connection);
      Report.CreateParameters(UserSession.LoginUser, command, UserSession.LoginUser.UserID);
      SqlDataAdapter adapter = new SqlDataAdapter(command);
      DataTable table = new DataTable();
      connection.Open();
      try
      {
        adapter.Fill(table);

      }
      catch (Exception ex)
      {
        try
        {
          adapter.SelectCommand.CommandText = report.GetSqlOld(false);
          adapter.Fill(table);
        }
        catch (Exception ex2)
        {
        ex.Data["Query"] = query;
        ExceptionLogs.LogException(UserSession.LoginUser, ex2, "Dashboard");
        }
      }

      // columnNames used to apply column order.
      string[] columnNames = new string[table.Columns.Count];

      for (int i = 0; i < table.Columns.Count; i++)
      {
        table.Columns[i].ColumnName = table.Columns[i].ColumnName.Replace(' ', '_');
        columnNames[i] = table.Columns[i].ColumnName;
      }

      // Apply column order.
      if (columnOrder != null)
      {
        int standardReportAdjuster = 0;
        int columnOrderAdjuster = 2;

        if (columnOrder.Length > table.Columns.Count)
        {
          standardReportAdjuster = 1;
          columnOrderAdjuster = 3;
        }

        for (int i = 0; i < table.Columns.Count; i++)
        {
          table.Columns[columnNames[i]].SetOrdinal((int)columnOrder[i + standardReportAdjuster].First - columnOrderAdjuster);
        }
      }

      return table;
    }

  }

  private static void FixPortletYs(List<Portlet> portlets, int column)
  {
    int max = GetPortletColumnMaxY(portlets.ToArray(), column) + 1;
    foreach (Portlet portlet in portlets)
    {
      if (portlet.X == column && portlet.Y < 0)
      {
        portlet.Y = max;
        max++;
      }
    }
  }

  private static int GetPortletColumnMaxY(Portlet[] portlets, int column)
  {
    int result = -1;
    foreach (Portlet portlet in portlets)
    {
      if (portlet.X == column && portlet.Y > -1) result = Math.Max(portlet.Y, result);
    }
    return result;
  }

  private static int ComparePortletY(Portlet portlet1, Portlet portlet2)
  {
    return portlet1.Y - portlet2.Y;
  }

  public static string DataTableToHtml(DataTable table)
  {
    StringBuilder builder = new StringBuilder();

    bool hasTicket = table.Columns.Contains("Ticket_Number");
    bool hasCustomer = table.Columns.Contains("Company_Name");

    builder.Append("<table cellspacing=\"0\" cellpadding=\"5\" border=\"0\">");
    builder.Append("<thead><tr>");
    if (hasTicket || hasCustomer) builder.Append("<th class=\"icon\">&nbsp</th>");
    foreach (DataColumn column in table.Columns)
    {
      builder.Append("<th>");
      builder.Append(column.ColumnName.Replace("_", " "));
      builder.Append("</th>");
    }
    builder.Append("</tr></thead>");

    builder.Append("<tbody>");
    foreach (DataRow row in table.Rows)
    {
      builder.Append("<tr>");

      string slaClass = "";
      if (hasTicket)
      {
        if (table.Columns.Contains("SLA_Violation_Hours"))
        {
          try { if (double.Parse(row["SLA_Violation_Hours"].ToString()) < 1) slaClass = "slaViolation"; }
          catch { }
        }

        if (slaClass == "" && table.Columns.Contains("SLA_Warning_Hours"))
        {
          try { if (double.Parse(row["SLA_Warning_Hours"].ToString()) < 1) slaClass = "slaWarning"; }
          catch { }
        }
      }


      string click = "<td class=\"icon {1}\"><img src=\"../images/icons/folder_open.png\" onclick=\"{0}\" /></td>";
      if (hasTicket)
      {
        builder.Append(string.Format(click, "openTicket(" + row["Ticket_Number"].ToString() + "); return false;", slaClass));
      }
      else if (hasCustomer)
      {
        builder.Append(string.Format(click, "openCustomer('" + row["Company_Name"].ToString().Replace("'", " ") + "'); return false;", " "));
      }


      foreach (DataColumn column in table.Columns)
      {
        builder.Append("<td>");
        if (row[column] is DateTime && row[column] != DBNull.Value)
        {
          builder.Append(DataUtils.DateToLocal(UserSession.LoginUser, ((DateTime)row[column])).ToString("g", UserSession.LoginUser.CultureInfo));
        }
        else
        {
          builder.Append(HttpUtility.HtmlEncode(row[column].ToString()));
        }
        builder.Append("</td>");
      }
      builder.Append("</tr>");
    }
    builder.Append("</tbody>");
    builder.Append("</table>");
    return builder.ToString();
  }

  [WebMethod]
  public static void UpdatePortletHeight(string id, int height)
  {
    Portlet portlet = LoadPortlet(id);
    if (portlet != null)
    {
      portlet.Height = height;
      SavePortlet(portlet);
    }

  }

  [WebMethod]
  public static void UpdatePortletPositions(string[][] positions)
  {
    for (int x = 0; x < positions.Length; x++)
    {
      for (int y = 0; y < positions[x].Length; y++)
      {
        Portlet portlet = LoadPortlet(positions[x][y]);
        if (portlet != null)
        {
          portlet.X = x;
          portlet.Y = y;
          SavePortlet(portlet);
        }
      }
    }
  }

  [WebMethod]
  public static void UpdatePortletVisibility(string id, bool isOpen)
  {
    Portlet portlet = LoadPortlet(id);
    if (portlet != null)
    {
      portlet.IsOpen = isOpen;
      SavePortlet(portlet);
    }

  }

  [WebMethod]
  public static int GetTicketID(int number)
  {
    return Tickets.GetTicketByNumber(UserSession.LoginUser, number).TicketID;

  }

  [WebMethod]
  public static int GetColumnCount()
  {
    return Settings.UserDB.ReadInt("DashboardColumns", 2);
  }

  [WebMethod]
  public static int GetCustomerID(string name)
  {
    Organizations organizations = new Organizations(UserSession.LoginUser);
    organizations.LoadByLikeOrganizationName(UserSession.LoginUser.OrganizationID, name, false);
    return organizations[0].OrganizationID;
  }

  public static string GetPortletHtml(Portlet portlet, Report report)
  {
    string caption = portlet.Caption.Length >= 35 ? portlet.Caption.Substring(0, 32) + "..." : portlet.Caption;

    if (string.IsNullOrEmpty(report.ExternalURL))
    {
      DataTable table = GetReportDataTable(report);
      string data = table.Rows.Count < 1 ? "<div class=\"noRecords\">There are no items to display.</div>" : DataTableToHtml(table);
      StringBuilder builder = new StringBuilder();
      builder.Append("<div class=\"portlet\" id=\"{0}\">");
      builder.Append("<div class=\"portlet-header {4}\">{1}<span class=\"portlet-close portlet-icon ts-icon ts-icon-close-small\"></span><span class=\"portlet-state portlet-icon ts-icon ts-icon-triangle-{3}\"></span></div>");
      builder.Append("<div class=\"portlet-body ui-corner-bottom\"><div class=\"portlet-content ui-corner-bottom\">{2}</div><div class=\"viewMore ui-corner-bottom\"><a href=\"#\" class=\"ts-link\">View Report</a></div></div></div>");
      return string.Format(builder.ToString(), portlet.ID, caption, data, portlet.IsOpen ? "s" : "w", portlet.IsOpen ? "ui-corner-top" : "ui-corner-all");
    }
    else
    {
      StringBuilder builder = new StringBuilder();
      builder.Append("<div class=\"portlet\" id=\"{0}\">");
      builder.Append("<div class=\"portlet-header {4}\">{1}<span class=\"portlet-close portlet-icon ts-icon ts-icon-close-small\"></span><span class=\"portlet-state portlet-icon ts-icon ts-icon-triangle-{3}\"></span></div>");
      builder.Append("<div class=\"portlet-body ui-corner-bottom externalReport\"><div class=\"portlet-content ui-corner-bottom\"><iframe height=\"250px\" width=\"100%\" scrolling=\"yes\" frameborder=\"0\" src=\"{2}\"></iframe></div></div></div>");
      return string.Format(builder.ToString(), portlet.ID, caption, report.ExternalURL, portlet.IsOpen ? "s" : "w", portlet.IsOpen ? "ui-corner-top" : "ui-corner-all");
    }
  }

  [WebMethod]
  public static Portlet[] GetPortlets()
  {
    int[] portletIDs = Settings.UserDB.ReadIntArray("DashboardPortlets");
    if (portletIDs.Length < 1)
    {
      portletIDs = Settings.SystemDB.ReadIntArray("DefaultPortletIDs");
      Settings.UserDB.WriteIntArray("DashboardPortlets", portletIDs);
    }
    int maxCol = GetColumnCount() - 1;
    List<Portlet> portlets = new List<Portlet>();
    foreach (int id in portletIDs)
    {
      try
      {
        Portlet portlet = LoadPortlet(GetPortletID(id));
        Report report = Reports.GetReport(UserSession.LoginUser, id);
        if (report != null)
        {
          if (portlet == null)
          {
            portlet = new Portlet();
            portlet.Caption = report.Name;
            portlet.ID = GetPortletID(id);
            portlet.ReportID = id;
            portlet.IsOpen = true;
            portlet.X = 1;
            portlet.Y = -1;
            portlet.Height = 250;

            if (portlet.Caption.IndexOf("My Open Ticket Summary") > -1 || portlet.Caption.IndexOf("Waiting On Customer Tickets") > -1 || portlet.Caption.IndexOf("My Unassigned Group Tickets") > -1) portlet.X = 0;
            SavePortlet(portlet);
          }

          if (portlet.X - maxCol < 0) portlet.X = 0;
          if (maxCol - portlet.X < 0) portlet.X = maxCol;
          portlets.Add(portlet);
          portlet.Html = GetPortletHtml(portlet, report);
        }
      }
      catch (Exception e)
      {

      }
    }
    FixPortletYs(portlets, 0);
    FixPortletYs(portlets, 1);
    FixPortletYs(portlets, 2);
    FixPortletYs(portlets, 3);
    FixPortletYs(portlets, 4);
    portlets.Sort(ComparePortletY);

    return portlets.ToArray();
  }

  [WebMethod]
  public static void DeletePortlet(string portletID)
  {
    int[] portletIDs = Settings.UserDB.ReadIntArray("DashboardPortlets");
    List<int> list = new List<int>();

    foreach (int i in portletIDs)
    {
      if (GetPortletID(i).ToLower() != portletID.ToLower())
      {
        list.Add(i);
      }
    }
    Settings.UserDB.WriteIntArray("DashboardPortlets", list.ToArray());
  }

  [WebMethod]
  public static void AddPortlet(int reportID, int col)
  {
    Portlet[] portlets = GetPortlets();
    Report report = Reports.GetReport(UserSession.LoginUser, reportID);
    if (report.OrganizationID != UserSession.LoginUser.OrganizationID && report.OrganizationID != null) return;

    Portlet portlet = new Portlet();
    portlet.Caption = report.Name;
    portlet.ID = GetPortletID(reportID);
    portlet.ReportID = reportID;
    portlet.IsOpen = true;
    portlet.X = col;
    portlet.Y = GetPortletColumnMaxY(portlets, col) + 1;
    portlet.Height = 250;
    SavePortlet(portlet);

    int[] portletIDs = Settings.UserDB.ReadIntArray("DashboardPortlets");
    List<int> list = new List<int>(portletIDs);
    list.Add(reportID);
    Settings.UserDB.WriteIntArray("DashboardPortlets", list.ToArray());
  }

  [WebMethod]
  public static RadComboBoxItemData[] GetReports(RadComboBoxContext context)
  {
    IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;
    List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();

    Reports reports = new Reports(UserSession.LoginUser);
    reports.LoadAll(UserSession.LoginUser.OrganizationID);

    foreach (Report report in reports)
    {
      RadComboBoxItemData itemData = new RadComboBoxItemData();
      itemData.Text = report.Name;
      itemData.Value = report.ReportID.ToString();
      list.Add(itemData);
    }

    return list.ToArray();
  }


  [Serializable]
  public class Portlet
  {
    public string ID { get; set; }
    public int ReportID { get; set; }
    public string Caption { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Height { get; set; }
    public bool IsOpen { get; set; }
    public string Html { get; set; }

  }
}
