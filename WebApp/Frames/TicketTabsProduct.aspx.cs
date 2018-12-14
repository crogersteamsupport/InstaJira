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
using System.Text;

public partial class Frames_TicketTabsProduct : BaseFramePage
{
  private int _productID;
  private int _resolvedVersionID = -1;
  private int _reportedVersionID = -1;

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!IsPostBack)
    {
      try 
      {
        _productID = int.Parse(Request["tf_ProductID"]);
        if (Request["tf_SolvedVersionID"] != null) _resolvedVersionID = int.Parse(Request["tf_SolvedVersionID"]);
        if (Request["tf_ReportedVersionID"] != null) _reportedVersionID = int.Parse(Request["tf_ReportedVersionID"]);
        int ticketTypeID = int.Parse(Request["tf_TicketTypeID"]);
        TicketType ticketType = (TicketType)TicketTypes.GetTicketType(UserSession.LoginUser, ticketTypeID);
        if (ticketType.OrganizationID != UserSession.LoginUser.OrganizationID) throw new Exception("Unauthorized ticket type.");
        CreateTabButtons(ticketType);
        tsMain.SelectedIndex = Settings.UserDB.ReadInt("SelectedOrganizationTicketTabIndex" + Request.Url, 0);
        ticketsFrame.Attributes["src"] = tsMain.SelectedTab.Value;
      }
      catch (Exception ex)
      {
        Response.Write("Invalid Ticket Type: " + ex.Message);
        Response.End();
      }
    }
  }

  private RadTab CreateTab(TicketType ticketType, string text, string url)
  {
    RadTab tab = new RadTab();
    tab.Text = text.Replace("Tickets", ticketType.Name);
    tab.Value = url;
    return tab;
  }

  private void CreateTabButtons(TicketType ticketType)
  {
    tsMain.Tabs.Clear();

    StringBuilder builder = new StringBuilder();
    builder.Append("../vcr/1_9_0/pages/ticketgrid.html?tf_TicketTypeID=" + ticketType.TicketTypeID);
    builder.Append("&tf_ProductID=" + _productID.ToString());
    if (_reportedVersionID > -1) builder.Append("&tf_ReportedVersionID=" + _reportedVersionID.ToString());
    if (_resolvedVersionID > -1) builder.Append("&tf_SolvedVersionID=" + _resolvedVersionID.ToString());
    builder.Append("&tf_UserID=" + UserSession.LoginUser.UserID);
    builder.Append("&tf_IsClosed=false");
    tsMain.Tabs.Add(CreateTab(ticketType, "My Tickets", builder.ToString()));
    
    builder = new StringBuilder();
    builder.Append("../vcr/1_9_0/pages/ticketgrid.html?tf_TicketTypeID=" + ticketType.TicketTypeID);
    builder.Append("&tf_ProductID=" + _productID.ToString());
    if (_reportedVersionID > -1) builder.Append("&tf_ReportedVersionID=" + _reportedVersionID.ToString());
    if (_resolvedVersionID > -1) builder.Append("&tf_SolvedVersionID=" + _resolvedVersionID.ToString());
    builder.Append("&tf_IsClosed=false");
    tsMain.Tabs.Add(CreateTab(ticketType, "Open Tickets", builder.ToString()));

    builder = new StringBuilder();
    builder.Append("../vcr/1_9_0/pages/ticketgrid.html?tf_TicketTypeID=" + ticketType.TicketTypeID);
    builder.Append("&tf_ProductID=" + _productID.ToString());
    if (_reportedVersionID > -1) builder.Append("&tf_ReportedVersionID=" + _reportedVersionID.ToString());
    if (_resolvedVersionID > -1) builder.Append("&tf_SolvedVersionID=" + _resolvedVersionID.ToString());
    builder.Append("&tf_IsClosed=true");
    tsMain.Tabs.Add(CreateTab(ticketType, "Closed Tickets", builder.ToString()));


    builder = new StringBuilder();
    builder.Append("../vcr/1_9_0/pages/ticketgrid.html?tf_TicketTypeID=" + ticketType.TicketTypeID);
    builder.Append("&tf_ProductID=" + _productID.ToString());
    if (_reportedVersionID > -1) builder.Append("&tf_ReportedVersionID=" + _reportedVersionID.ToString());
    if (_resolvedVersionID > -1) builder.Append("&tf_SolvedVersionID=" + _resolvedVersionID.ToString());
    builder.Append("&tf_UserID=-2");
    tsMain.Tabs.Add(CreateTab(ticketType, "Unassigned Tickets", builder.ToString()));
  }

}
