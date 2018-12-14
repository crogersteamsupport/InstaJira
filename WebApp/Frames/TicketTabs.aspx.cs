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

public partial class Frames_TicketTabs : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!IsPostBack)
    {
      try 
      {	        
        int ticketTypeID = int.Parse(Request["TicketTypeID"]);
        TicketType ticketType = (TicketType)TicketTypes.GetTicketType(UserSession.LoginUser, ticketTypeID);
        if (ticketType.OrganizationID != UserSession.LoginUser.OrganizationID) throw new Exception("Unauthorized ticket type.");
        CreateTabButtons(ticketType);
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
    builder.Append("Tickets.aspx?TicketTypeID=" + ticketType.TicketTypeID);
    builder.Append("&UserID=" + UserSession.LoginUser.UserID);
    builder.Append("&TicketStatusID="+TicketFilters.Values.Opened);
    tsMain.Tabs.Add(CreateTab(ticketType, "My Tickets", builder.ToString()));
    
    builder = new StringBuilder();
    builder.Append("Tickets.aspx?TicketTypeID=" + ticketType.TicketTypeID);
    builder.Append("&TicketStatusID=" + TicketFilters.Values.Opened);
    tsMain.Tabs.Add(CreateTab(ticketType, "Open Tickets", builder.ToString()));

    builder = new StringBuilder();
    builder.Append("Tickets.aspx?TicketTypeID=" + ticketType.TicketTypeID);
    builder.Append("&TicketStatusID=" + TicketFilters.Values.Closed);
    tsMain.Tabs.Add(CreateTab(ticketType, "Closed Tickets", builder.ToString()));


    builder = new StringBuilder();
    builder.Append("Tickets.aspx?TicketTypeID=" + ticketType.TicketTypeID);
    builder.Append("&UserID=" + TicketFilters.Values.Unassigned);
    tsMain.Tabs.Add(CreateTab(ticketType, "Unassigned Tickets", builder.ToString()));
    
    builder = new StringBuilder();
    builder.Append("Tickets.aspx?TicketTypeID=" + ticketType.TicketTypeID);
    tsMain.Tabs.Add(CreateTab(ticketType, "All Tickets", builder.ToString()));
  }

}
