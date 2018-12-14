using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using TeamSupport.WebUtils;

public partial class Frames_Frame : System.Web.UI.MasterPage
{

  protected void Page_Load(object sender, EventArgs e)
  {
    /*
    if (!IsPostBack)
    {
      fieldPostToken.Value = UserSession.PostAuthenticationToken;
    }
    else
    {
      if (fieldPostToken == null || fieldPostToken.Value != UserSession.PostAuthenticationToken)
      {
        Response.Write("You are unauthorized.");
        Response.End();
        return;
      }
    }*/
  }


  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender(e);

    if (!(ContentPlaceHolder1.Page as BaseFramePage).CachePage)
    {
      Response.Cache.SetAllowResponseInBrowserHistory(false);
      Response.Cache.SetCacheability(HttpCacheability.NoCache);
      Response.Cache.SetNoStore();
      Response.Expires = 0;
    
    }
  }
}
