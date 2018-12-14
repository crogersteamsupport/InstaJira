using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Telerik.Web.UI;
using System.Data.SqlClient;
using System.Text;
using System.Globalization;

public partial class Frames_ReportResults : BaseFramePage
{
  private int _reportID = -1;
  private Report _report;
  private string _query;
  private TimeZoneInfo _timeZoneInfo;
  private bool _isCustom = false;

  private string GetReportConnectionString()
  {
    return System.Web.Configuration.WebConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString;
  }

  protected override void OnInit(EventArgs e)
  {
    CachePage = true;

    base.OnInit(e);


    gridReport.GroupingSettings.CaseSensitive = false;

    _report = null;
    try
    {
      _reportID = int.Parse(Request["ReportID"]);
      if (_reportID < 0) throw new Exception("Invalid ReportID");
      _report = (Report)Reports.GetReport(UserSession.LoginUser, _reportID);
      if (_report == null) throw new Exception("Invalid Report");
      if (_report.OrganizationID != null && UserSession.LoginUser.OrganizationID != 1 && UserSession.LoginUser.OrganizationID != 1078)
      {
        if (_report.OrganizationID != UserSession.LoginUser.OrganizationID)
        {
          _report = null;
          throw new Exception("Invalid Report");
        }
      }
    }
    catch (Exception ex)
    {
      ExceptionLogs.LogException(UserSession.LoginUser, ex, "ReportResults");
      Response.Write("There was an error retrieving your report.");
      Response.End();
      return;
    }
    _isCustom = _report.ReportSubcategoryID != null;

    fieldReportID.Value = _reportID.ToString();

    if (!IsPostBack)
    {
      if (_isCustom)
      {
        filterControl.ReportSubcategoryID = (int)_report.ReportSubcategoryID;
        filterControl.ReportID = _reportID;
        int h = Settings.UserDB.ReadInt("ReportFilterHeight_" + _reportID.ToString(), 175);
        if (h > 300) h = 300;
        paneFilters.Height = new Unit(h, UnitType.Pixel);
      }
      else
      {
        //paneFilters.Visible = false;
        paneFilters.Height = new Unit(1, UnitType.Pixel);
        barFilters.Visible = false;
      }


      try
      {
        CreateColumns(_report);
        LoadReportSettings();
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(UserSession.LoginUser, ex, "Reports");
      }
    }

  }

  private void CreateColumns(Report report)
  {
    DataTable table = new DataTable();

    using (SqlCommand command = new SqlCommand())
    {
      command.CommandText = report.GetSqlOld(true, filterControl.ReportConditions);
      command.CommandType = CommandType.Text;
      Report.CreateParameters(UserSession.LoginUser, command, UserSession.LoginUser.UserID);

      using (SqlConnection connection = new SqlConnection(GetReportConnectionString()))
      {
        connection.Open();
        command.Connection = connection;
        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
        {
          adapter.FillSchema(table, SchemaType.Source);
          adapter.Fill(table);
        }
        connection.Close();
      }
    }

    ReportFields fields = new ReportFields(UserSession.LoginUser);
    fields.LoadByReportID(_reportID);
    foreach (ReportField field in fields)
    {
      if (field.IsCustomField == false && (field.LinkedFieldID == 30 || field.LinkedFieldID == 146))
      {
        GridButtonColumn column = new GridButtonColumn();
        gridReport.MasterTableView.Columns.Add(column);
        column.ButtonType = GridButtonColumnType.ImageButton;
        column.ImageUrl = "../images/icons/open.png";
        column.UniqueName = "ButtonOpen";
        column.CommandName = "OpenItem";
        column.HeaderStyle.Width = new Unit(32, UnitType.Pixel);
        column.Resizable = false;
        break;
      }
    }

    if (fields.IsEmpty)
    {
      if (table.Columns.Contains("Ticket_Number"))
      {
        GridButtonColumn column = new GridButtonColumn();
        gridReport.MasterTableView.Columns.Add(column);
        column.ButtonType = GridButtonColumnType.ImageButton;
        column.ImageUrl = "../images/icons/open.png";
        column.UniqueName = "ButtonOpen";
        column.CommandName = "OpenItem";
        column.HeaderStyle.Width = new Unit(32, UnitType.Pixel);
        column.Resizable = false;
      }
    }



    foreach (DataColumn dataColumn in table.Columns)
    {
      GridBoundColumn gridColumn;
      if (dataColumn.DataType == typeof(System.DateTime))
      {
        gridColumn = new GridDateTimeColumn();
        gridColumn.DataFormatString = "{0:MM/dd/yyyy hh:mm tt}";
      }
      else if (dataColumn.DataType == typeof(System.Decimal) ||
               dataColumn.DataType == typeof(System.Double)
        )
      {
        gridColumn = new GridNumericColumn();
        gridColumn.DataFormatString = "{0:0.00}";
      }
      else if (dataColumn.DataType == typeof(System.Int16) ||
               dataColumn.DataType == typeof(System.Int32) ||
               dataColumn.DataType == typeof(System.Int64))
      {
        gridColumn = new GridNumericColumn();
        gridColumn.DataFormatString = "{0:0}";
      }
      else
      {
        gridColumn = new GridBoundColumn();
      }


      gridReport.MasterTableView.Columns.Add(gridColumn);
      gridColumn.DataType = dataColumn.DataType;
      gridColumn.DataField = dataColumn.ColumnName.Replace(' ', '_');
      gridColumn.UniqueName = gridColumn.DataField;
      gridColumn.HeaderText = dataColumn.ColumnName.Replace('_', ' ');
      gridColumn.HeaderStyle.Width = new Unit(200, UnitType.Pixel);
    }


  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    ajaxManager.AjaxSettings.AddAjaxSetting(gridReport, gridReport);
    ajaxManager.AjaxSettings.AddAjaxSetting(ajaxManager, gridReport);
    ajaxManager.AjaxSettings.AddAjaxSetting(btnRefresh, gridReport);

    if (_isCustom)
    {
      ajaxManager.AjaxSettings.AddAjaxSetting(divFilter, gridReport);
      ajaxManager.AjaxSettings.AddAjaxSetting(ajaxManager, divFilter);
    }

    Organization organization = (Organization)Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
    if (organization != null && organization.TimeZoneID != null && organization.TimeZoneID != "")
    {
      _timeZoneInfo = System.TimeZoneInfo.FindSystemTimeZoneById(organization.TimeZoneID);
    }
    else
    {
      _timeZoneInfo = TimeZoneInfo.Utc;
    }
    Page.Culture = organization.CultureName;


    if (!IsPostBack)
    {


      gridReport.ClientSettings.Selecting.AllowRowSelect = false;
      gridReport.ClientSettings.ClientEvents.OnRowDblClick = "";
      gridReport.MasterTableView.ClientDataKeyNames = null;
      try
      {
        gridReport.Columns.FindByUniqueName("Ticket_Number");
        gridReport.ClientSettings.Selecting.AllowRowSelect = true;
        gridReport.ClientSettings.ClientEvents.OnRowDblClick = "TicketDblClick";
        gridReport.MasterTableView.ClientDataKeyNames = new string[] { "Ticket_Number" };
      }
      catch (Exception)
      {
      }


    }



  }


  protected void gridReport_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
  {
    if (_reportID < 0)
    {
      gridReport.DataSource = null;
    }
    else
    {
      try
      {
        DataTable table = GetTable(_reportID);
        if (table != null) table.CaseSensitive = false;
        gridReport.DataSource = table;

      }
      catch (Exception ex)
      {
        DataUtils.LogException(UserSession.LoginUser, ex);
        Response.Clear();
        Response.Write("There was an error displaying your report.");
        //Response.Write(ex.StackTrace);
        Response.End();
      }
    }

  }

  private DataTable GetTable(int reportID)
  {
    Report report = Reports.GetReport(UserSession.LoginUser, reportID);
    if (report == null) return null;

    using (SqlConnection connection = new SqlConnection(GetReportConnectionString()))
    {
      DateTime start = DateTime.Now;
      ReportViews reportViews = new ReportViews(UserSession.LoginUser);
      ReportView reportView = reportViews.AddNewReportView();
      reportView.UserID = UserSession.LoginUser.UserID;
      reportView.ReportID = report.ReportID;
      reportView.DateViewed = DateTime.UtcNow;

      _query = report.GetSqlOld(false, filterControl.ReportConditions);
      reportView.SQLExecuted = _query;
      report.LastSqlExecuted = _query;
      report.Collection.Save();

      SqlCommand command = new SqlCommand(_query, connection);

      Report.CreateParameters(UserSession.LoginUser, command, UserSession.LoginUser.UserID);
      SqlDataAdapter adapter = new SqlDataAdapter(command);
      DataTable table = new DataTable();
      connection.Open();
      SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
      command.Transaction = transaction;
      try
      {
        adapter.Fill(table);
        transaction.Commit();
      }
      catch (Exception ex)
      {
        transaction.Rollback();
        reportView.ErrorMessage = ex.Message;
        DataUtils.LogException(UserSession.LoginUser, ex);
      }

      foreach (DataColumn column in table.Columns)
      {
        column.ColumnName = column.ColumnName.Replace(' ', '_');
      }
      reportView.DurationToLoad = DateTime.Now.Subtract(start).TotalSeconds;
      reportView.Collection.Save();
      return table;
    }
  }

  private void SaveReportSettings()
  {
    if (_reportID < 0 || !IsPostBack || filterControl.ReportID != _reportID) return;
    try
    {
      string data = TSUtils.ReadGridSettings(gridReport);
      if (_isCustom)
        ReportData.SaveReportData(UserSession.LoginUser, _reportID, UserSession.LoginUser.UserID, data, DataUtils.ObjectToString(filterControl.ReportConditions));
      else
        ReportData.SaveReportData(UserSession.LoginUser, _reportID, UserSession.LoginUser.UserID, data, "");

    }
    catch (Exception ex)
    {

    }
  }

  private void LoadReportSettings()
  {
    if (_reportID < 0) return;
    try
    {
      ReportData reportData = new ReportData(UserSession.LoginUser);
      reportData.LoadReportData(_reportID, UserSession.LoginUser.UserID);
      if (reportData.IsEmpty) return;

      TSUtils.LoadGridSettings(gridReport, reportData[0].ReportData);
      if (_isCustom && reportData[0].QueryObject.Trim() != "")
      {
        filterControl.ReportConditions = (ReportConditions)DataUtils.StringToObject(reportData[0].QueryObject);
        filterControl.ReportConditions.LoginUser = UserSession.LoginUser;
      }

      try
      {
        gridReport.MasterTableView.Columns.FindByUniqueName("ButtonOpen").HeaderStyle.Width = new Unit(32, UnitType.Pixel);
      }
      catch (Exception) { }

    }
    catch (Exception)
    {

    }
  }

  protected override void Render(HtmlTextWriter writer)
  {
    base.Render(writer);
    SaveReportSettings();

  }

  protected void btnRefresh_Click(object sender, EventArgs e)
  {
    gridReport.Rebind();
  }

  protected void gridReport_ColumnCreated(object sender, GridColumnCreatedEventArgs e)
  {
    /*    if (e.Column.HeaderText != "")
        {
          e.Column.HeaderStyle.Width = new Unit(200, UnitType.Pixel);
        }
        if (e.Column is GridDateTimeColumn)
        {
          (e.Column as GridDateTimeColumn).DataFormatString = "{0:MM/dd/yyyy hh:mm tt}";
        }

        e.Column.HeaderText = e.Column.HeaderText.Replace('_', ' ');

        if (e.Column.DataType == System.Type.GetType("System.Decimal"))
        {

          (e.Column as GridBoundColumn).DataFormatString = "{0:0.00}";
        }
        */
  }
  protected void gridReport_PreRender(object sender, EventArgs e)
  {

    /*//gridReport.Width = new Unit(gridReport.MasterTableView.RenderColumns.Length * 500, UnitType.Pixel);
    if (!IsPostBack)
    {
      foreach (GridColumn col in gridReport.MasterTableView.RenderColumns)
      {
        if (col.UniqueName == "Name")
        {
          col.HeaderStyle.Width = Unit.Pixel(500);
          col.ItemStyle.Width = Unit.Pixel(500);
          col.HeaderText = "kevin";
          foreach (GridFilteringItem filter in gridReport.MasterTableView.GetItems(GridItemType.FilteringItem))
          {
            TextBox txtbx = (TextBox)filter["Name"].Controls[0];
            txtbx.Width = Unit.Pixel(500);
          }
        }
      }
    }*/
  }
  protected void gridReport_ItemDataBound(object sender, GridItemEventArgs e)
  {
    if (e.Item is GridDataItem)
    {
      GridDataItem item = e.Item as GridDataItem;

      try
      {
        string key = item.GetDataKeyValue("Ticket_Number").ToString();
        ImageButton button = (ImageButton)item["ButtonOpen"].Controls[0];
        button.OnClientClick = "OpenTicketTab('" + key + "'); return false;";
      }
      catch (Exception)
      { }


      try
      {
        if (item["Ticket_Hours_Spent"] != null)
        {
          double spent = double.Parse(item["Ticket_Hours_Spent"].Text) * 60;
          {
            int hours = (int)(spent / 60);
            int minutes = (int)Math.Round(spent % 60);
            item["Ticket_Hours_Spent"].Text = string.Format("{0:00}:{1:00}", hours, minutes);
          }
        }
      }
      catch (Exception)
      {

      }

      try
      {
        if (item["SLA_Violation_Hours"].Text.Trim() != "" &&
            item["SLA_Violation_Hours"].Text.IndexOf("&nbsp") < 0 &&
            double.Parse(item["SLA_Violation_Hours"].Text) < -0.01)
        {
          item["ButtonOpen"].CssClass = "slaViolation rgRow";
        }
        else if (item["SLA_Warning_Hours"].Text.Trim() != "" &&
                 item["SLA_Warning_Hours"].Text.IndexOf("&nbsp") < 0 &&
                 double.Parse(item["SLA_Warning_Hours"].Text) < -0.01)
        {
          item["ButtonOpen"].CssClass = "slaWarning rgRow";
        }
      }
      catch (Exception) { }

      foreach (GridColumn column in ((GridItem)(item)).OwnerTableView.Columns)
      {
        if (column is GridDateTimeColumn)
        {
          string s = item[column].Text;
          if (s != "")
          {
            try
            {
              DateTime dt = DateTime.SpecifyKind(DateTime.Parse(s, new CultureInfo("en-US")), DateTimeKind.Utc);
              item[column].Text = DataUtils.DateToLocal(UserSession.LoginUser, dt).ToString("g", UserSession.LoginUser.OrganizationCulture);
            }
            catch (Exception)
            {
            }
          }
        }
      }
    }
  }

  protected void gridReport_SortCommand(object sender, GridSortCommandEventArgs e)
  {
    string orderByClause = GetOrderByClause(e.SortExpression, e.NewSortOrder.ToString());

    ReportData reportData = new ReportData(UserSession.LoginUser);
    reportData.LoadReportData(_reportID, UserSession.LoginUser.UserID);
    if (!reportData.IsEmpty)
    {
      using (SqlConnection connection = new SqlConnection(GetReportConnectionString()))
      {
        reportData[0].OrderByClause = orderByClause;
        reportData[0].Collection.Save();
      }
    }
  }

  private string GetOrderByClause(string columnName, string direction)
  {
    string result = null;
    result = "[" + columnName.Replace("_", " ") + "]";
    if (string.Compare(direction, "Descending", StringComparison.OrdinalIgnoreCase) == 0)
    {
      result += " DESC";
    }
    else
    {
      result += " ASC";
    }
    return result;
  }

}
