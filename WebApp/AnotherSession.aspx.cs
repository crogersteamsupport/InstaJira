using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using TeamSupport.Data;
using TeamSupport.WebUtils;

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
    HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName].Value = null;
    FormsAuthentication.SignOut();

  }
}
