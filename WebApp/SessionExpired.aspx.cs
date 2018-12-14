using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SessionExpired : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (Request["msg"] != null)
    {
      try 
	    {	        
        string[] lines = Request["msg"].Split('\n');
        foreach (string line in lines)
      	{
	        message.InnerHtml = message.InnerHtml + line + "<br/>";
        }
	    }
	    catch (Exception)
	    {
	    }
    }
  }
}
