using System;
using System.Security.Cryptography;

public class BasePage : System.Web.UI.Page
{
  private const string _XsrfName = "_XSID";


  protected override void OnPreLoad(EventArgs e)
  {
    base.OnPreLoad(e);
    if (IsPostBack)
    {
      string vwId = ViewState[_XsrfName] as string;
      if (string.IsNullOrEmpty(vwId))
      {
        Response.Write("Unauthorized Request");
        Response.End();
        return;
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
}
