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

public partial class Frames_TicketTabsOrganization : BaseFramePage
{
  private int _organizationID;

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!IsPostBack)
    {
      try 
      {
        _organizationID = int.Parse(Request["OrganizationID"]);
        int ticketTypeID = int.Parse(Request["TicketTypeID"]);
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

    string ticketUrl = string.Format("../vcr/1_9_0/pages/ticketgrid.html?tf_CustomerID={0:0}&tf_TicketTypeID={1:0}&tf_UserID={1:0}&tf_IsClosed=false", _organizationID, ticketType.TicketTypeID, UserSession.LoginUser.UserID);
    tsMain.Tabs.Add(CreateTab(ticketType, "My Tickets", ticketUrl));

    ticketUrl = string.Format("../vcr/1_9_0/pages/ticketgrid.html?tf_CustomerID={0:0}&tf_TicketTypeID={1:0}&tf_IsClosed=false", _organizationID, ticketType.TicketTypeID);
    tsMain.Tabs.Add(CreateTab(ticketType, "Open Tickets", ticketUrl));

    ticketUrl = string.Format("../vcr/1_9_0/pages/ticketgrid.html?tf_CustomerID={0:0}&tf_TicketTypeID={1:0}&tf_IsClosed=true", _organizationID, ticketType.TicketTypeID);
    tsMain.Tabs.Add(CreateTab(ticketType, "Closed Tickets", ticketUrl));


    ticketUrl = string.Format("../vcr/1_9_0/pages/ticketgrid.html?tf_CustomerID={0:0}&tf_TicketTypeID={1:0}&tf_IsClosed=true&tf_UserID=-2", _organizationID, ticketType.TicketTypeID);
    tsMain.Tabs.Add(CreateTab(ticketType, "Unassigned Tickets", ticketUrl));
  }

}
