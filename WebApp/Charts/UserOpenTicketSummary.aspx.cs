using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using TeamSupport.WebUtils;
using TeamSupport.Data;
using System.Data;
using System.Web.Services;

public partial class Charts_UserOpenTicketSummary : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }

  [Serializable]
  public class TicketCounts
  {
    public string Name { get; set; }
    public int Count { get; set; }
  }

  [WebMethod]
  public static TicketCounts[] GetData()
  {
    List<TicketCounts> result = new List<TicketCounts>();
    TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);
    ticketTypes.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, UserSession.CurrentUser.ProductType);

    foreach (TicketType ticketType in ticketTypes)
    {
      int count = Tickets.GetUserOpenTicketCount(UserSession.LoginUser, UserSession.LoginUser.UserID, ticketType.TicketTypeID);
      string value = ticketType.Name;
      if (value.Length > 8) value = value.Substring(0, 8);
      value = value + " (" + count.ToString() + ")";
      TicketCounts counts = new TicketCounts();
      counts.Name = value;
      counts.Count = count;
      result.Add(counts);
    }

    
    return result.ToArray();
  
  }


}
