using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Web.Services;

public partial class Oops : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
      string result = "Please let TeamSupport know about this error.";
      try 
      {
        Exception lastException = Server.GetLastError().GetBaseException();

        if (lastException != null &&  UserSession.LoginUser != null)
        {
          ExceptionLog log = ExceptionLogs.LogException(UserSession.LoginUser, lastException, HttpContext.Current.Request.Url.AbsolutePath, "Oops Page");
          if (log != null)
          {
            result = "Please let TeamSupport know about this error, and that the error reference number is <span class=\"error\">" + log.ExceptionLogID.ToString() + "</span>";
          }
        }
		   
      }
      catch (Exception)
      {
		
      }

      litProblem.Text = result;
    }

    /*
    [WebMethod]
    public static TicketProxy CreateTicket(int exceptionLogID) {
      ExceptionLog log = ExceptionLogs.GetExceptionLog(LoginUser.Anonymous, exceptionLogID);
      if (log == null) return null;

      LoginUser loginUser = new LoginUser(log.or;
      try
      {
        Errorlogs

      }
      catch (Exception ex)
      {
        return null;
      }
    
    }*/

}