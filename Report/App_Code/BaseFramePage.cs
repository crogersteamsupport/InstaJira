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
using Telerik.Web.UI;

public class BaseFramePage: System.Web.UI.Page
{
  private bool _cachePage = true;

  public bool CachePage
  {
    get { return _cachePage; }
    set { _cachePage = value; }
  }

  public BaseFramePage()
  {
  }
  
  
  
  
  
  protected override void SavePageStateToPersistenceMedium(object viewState)
  {
    Session[Request.Url.AbsolutePath] = viewState;
  }

  protected override object LoadPageStateFromPersistenceMedium()
  {
    object o = Session[Request.Url.AbsolutePath];
    if (o == null)
    {
      return null;
      //throw new Exception("The Viewstate is missing!!!");
    }
    else
    {
      return o;
    }
  }
}



