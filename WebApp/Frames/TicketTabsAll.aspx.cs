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

public partial class Frames_TicketTabsAll : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!IsPostBack)
    {
      try 
      {	        
        CreateTabButtons();
        tsMain.SelectedIndex = Settings.UserDB.ReadInt("SelectedTabIndex" + Request.Url, 0);
        ticketsFrame.Attributes["src"] = tsMain.SelectedTab.Value;
      }
      catch (Exception ex)
      {
        Response.Write("Invalid Ticket Type: " + ex.Message);
        Response.End();
      }
    }
  }

  private RadTab CreateTab(string text, string url)
  {
    RadTab tab = new RadTab();
    tab.Text = text;
    tab.Value = url;
    return tab;
  }

  private void CreateTabButtons()
  {
    tsMain.Tabs.Clear();

    
    StringBuilder builder = new StringBuilder();

    if (!Request.QueryString.ToString().Contains("IsKnowledgeBase")) {
      builder.Append("../vcr/1_9_0/pages/ticketgrid.html?");
      if (!string.IsNullOrEmpty(Request.QueryString.ToString())) builder.Append(Request.QueryString + "&");
      builder.Append("tf_UserID=" + UserSession.LoginUser.UserID.ToString());
      tsMain.Tabs.Add(CreateTab("All My Tickets", builder.ToString()));

      builder = new StringBuilder();
      builder.Append("../vcr/1_9_0/pages/ticketgrid.html?");
      if (!string.IsNullOrEmpty(Request.QueryString.ToString())) builder.Append(Request.QueryString + "&");
      builder.Append("tf_IsClosed=false");
      tsMain.Tabs.Add(CreateTab("All Open Tickets", builder.ToString()));

      builder = new StringBuilder();
      builder.Append("../vcr/1_9_0/pages/ticketgrid.html?");
      if (!string.IsNullOrEmpty(Request.QueryString.ToString())) builder.Append(Request.QueryString + "&");
      builder.Append("tf_IsClosed=true");
      tsMain.Tabs.Add(CreateTab("All Closed Tickets", builder.ToString()));


      builder = new StringBuilder();
      builder.Append("../vcr/1_9_0/pages/ticketgrid.html?");
      if (!string.IsNullOrEmpty(Request.QueryString.ToString())) builder.Append(Request.QueryString + "&");
      builder.Append("tf_UserID=-2");
      tsMain.Tabs.Add(CreateTab("All Unassigned Tickets", builder.ToString()));
    }

    builder = new StringBuilder();
    builder.Append("../vcr/1_9_0/pages/ticketgrid.html?");
    if (!string.IsNullOrEmpty(Request.QueryString.ToString())) builder.Append(Request.QueryString);
    tsMain.Tabs.Add(CreateTab("All Tickets", builder.ToString()));

  }

}
