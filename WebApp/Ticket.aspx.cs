using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class TicketFrame : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    try 
	  {
      if (!IsPostBack)
      {
        string url;
        if (Request["TicketNumber"] != null)
        {
          int ticketNumber = int.Parse(Request["TicketNumber"]);
          url = "~/?TicketNumber=" + ticketNumber.ToString();
        }
        else
        {
          int ticketID = int.Parse(Request["TicketID"]);
          url = "~/?TicketID=" + ticketID.ToString();
        }
        Response.Redirect(url);
      }
	  }
	  catch (Exception)
	  {
		  Response.Write("Invalid ticket id.");
      Response.End();
	  }
  }
}
