using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using TeamSupport.Data;
using TeamSupport.WebUtils;

  public class UnhandledExceptionModule : IHttpModule
  {
    protected virtual void OnError(object sender, EventArgs args)
    {
      HttpApplication application = (HttpApplication)sender;
      AddToLog(application.Server.GetLastError(), application.Context);
    }

    protected virtual void AddToLog(Exception e, HttpContext context)
    {
      Exception exception = e;
      if (exception == null) return;
      if (exception is HttpUnhandledException) exception = e.InnerException;
      try
      {
        LoginUser loginUser = UserSession.LoginUser;
        if (loginUser == null) loginUser = new LoginUser(UserSession.ConnectionString, -1, -1, null);
        ExceptionLogs logs = new ExceptionLogs(loginUser);
        ExceptionLog log = logs.AddNewExceptionLog();
        log.URL = context.Request.Url.AbsolutePath;
        log.StackTrace = exception.StackTrace;
        log.Message = exception.Message;
        log.ExceptionName = exception.GetType().Name;
        logs.Save();
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex);
      }
    }


    #region IHttpModule Members

    void IHttpModule.Dispose()
    {
    }

    void IHttpModule.Init(HttpApplication context)
    {
      context.Error += new EventHandler(OnError);
    }

    #endregion
  }
