using System;
using System.Collections;
using System.Configuration;
using System.Data;
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
using System.Web.Services;

public partial class Frames_Reports : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    Page.Culture = UserSession.LoginUser.CultureInfo.Name;

    int newReportID = Settings.Session.ReadInt("NewReportID", -1);
    if (newReportID > -1)
    {
      Settings.Session.WriteInt("NewReportID", -1);
      Settings.UserDB.WriteInt("SelectedReportID", newReportID);
      loadReport();
    }

    if (!IsPostBack)
    {
      paneGrid.Width = new Unit(Settings.UserDB.ReadInt("ReportsGridWidth", 200), UnitType.Pixel);
      if (Settings.UserDB.ReadInt("SelectedReportID") > -1)
      {
        loadReport();
      }

      if (UserSession.LoginUser.OrganizationID < 445050)
      {
        tbUser.FindItemByValue("ExportCSV").Text = "Export to CSV";
        tbUser.FindItemByValue("ExportExcel").Visible = true;
      }

    }
  }

  protected void loadReport()
  {
    int id = GetReportID();
    Report report = (Report)Reports.GetReport(UserSession.LoginUser, id);
    if (report != null)
    {
      ReportTypeOld repType = (ReportTypeOld)Enum.Parse(typeof(ReportTypeOld), Settings.UserDB.ReadInt("SelectedReportTypeOldID").ToString());

      if (ExpandNode(repType, id))
      {

        string url = string.IsNullOrEmpty(report.ExternalURL) ? "../Report/ReportResults.aspx" : report.ExternalURL;
        frmReport.Attributes["src"] = url + "?ReportID=" + id.ToString();

        tbUser.Items[1].Enabled = UserSession.CurrentUser.IsSystemAdmin && report.OrganizationID != null && report.OrganizationID == UserSession.LoginUser.OrganizationID;
        tbUser.Items[2].Enabled = tbUser.Items[1].Enabled && repType != ReportTypeOld.Favorite;
        tbUser.Items[3].Enabled = true;
        tbUser.Items[5].Enabled = true;
        tbUser.Items[6].Enabled = true;

        tbUser.Items[6].Text = report.IsFavorite ? "Remove Favorite" : tbUser.Items[6].Text;
      }
    }
  }

  private int GetReportID()
  {
    int reportID = -1;
    if (reportTree.SelectedNodes.Count > 0)
    {
      RadTreeNode item = reportTree.SelectedNode;
      reportID = int.Parse(item.Value) > 2 ? int.Parse(item.Value) : -1;
    }
    if (reportID == -1)
    { //load up last report viewed if nothing else is selected
      reportID = Settings.UserDB.ReadInt("SelectedReportID", -1);
    }

    return reportID;
  }

  private bool ExpandNode(ReportTypeOld type, int valNodeToSelect)
  {
    RadTreeNode typeNode = reportTree.FindNodeByValue(((int)type).ToString());

    if (typeNode == null)
    {
      return false;
    }
    else
    {
      Reports _reports = new Reports(UserSession.LoginUser);
      switch (type)
      {
        case ReportTypeOld.Standard:
          _reports.LoadStandard();
          break;
        case ReportTypeOld.Graphical:
          _reports.LoadGraphical(UserSession.CurrentUser.OrganizationID);
          break;
        case ReportTypeOld.Favorite:
          _reports.LoadFavorites();
          break;
        default:
          _reports.LoadCustom(UserSession.CurrentUser.OrganizationID);
          break;
      }

      foreach (Report rep in _reports)
      {
        RadTreeNode newNode = new RadTreeNode();
        newNode.Text = rep.Name;
        newNode.Value = rep.ReportID.ToString();
        newNode.Attributes.Add("ExternalURL", rep.ExternalURL);

        if (newNode.Value == valNodeToSelect.ToString()) { newNode.Selected = true; }
        typeNode.Nodes.Add(newNode);
      }

      typeNode.Expanded = true;
      typeNode.ExpandMode = TreeNodeExpandMode.WebService;

      return true;
    }
  }

  [WebMethod]
  public static string GetCsvUrl()
  {
    //'../dc/1078/reports/95'
    return string.Format("../dc/{0}/reports/{1}", UserSession.LoginUser.OrganizationID.ToString(), Settings.UserDB.ReadInt("SelectedReportID").ToString());
  }


}
