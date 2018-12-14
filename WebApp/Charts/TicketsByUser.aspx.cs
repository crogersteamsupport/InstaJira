using System;
using System.Collections.Generic;
using System.Linq;
using TeamSupport.WebUtils;
using TeamSupport.Data;
using System.Web.Services;

public partial class Charts_TicketsByUser : System.Web.UI.Page
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

        Users users = new Users(UserSession.LoginUser);
        users.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, true);

        foreach (User user in users)
        {
            string value = user.DisplayName;
            int count = 0;

            foreach (TicketType ticketType in ticketTypes)
            {
                count += Tickets.GetUserOpenTicketCount(UserSession.LoginUser, user.UserID, ticketType.TicketTypeID);
            }

            TicketCounts counts = new TicketCounts();
            counts.Name = value;
            counts.Count = count;
            result.Add(counts);
        }


        return result.OrderBy(ticketcount => ticketcount.Name).ToArray();
        
    }
}