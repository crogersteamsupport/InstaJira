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
using System.Security.Cryptography;

public class BasePage : System.Web.UI.Page
{
  private const string _XsrfName = "_XSID";


  protected override void OnPreLoad(EventArgs e)
  {
    base.OnPreLoad(e);
    string sessionXsrfId = Session[_XsrfName] as string;
    if (IsPostBack)
    {
      string vwId = ViewState[_XsrfName] as string;
      if (string.IsNullOrEmpty(vwId) || !vwId.Equals(sessionXsrfId))
      {
        Response.Write("Unauthorized Request");
        Response.End();
        return;
      }
    }
    else
    {
      if (string.IsNullOrEmpty(sessionXsrfId))
      {
        sessionXsrfId = GenerateCode();
        Session.Add(_XsrfName, sessionXsrfId);
        ViewState.Add(_XsrfName, sessionXsrfId);
      }
    }
  }

  private static string GenerateCode()
  {
    RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
    byte[] randBytes = new byte[32];
    random.GetNonZeroBytes(randBytes);
    return Convert.ToBase64String(randBytes);
  }

  protected override void SavePageStateToPersistenceMedium(object viewState)
  {
    Session[Request.Url.AbsolutePath] = viewState;
  }

  protected override object LoadPageStateFromPersistenceMedium()
  {
    object o = Session[Request.Url.AbsolutePath];
    return o;
  }

}
