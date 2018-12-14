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
using TeamSupport.WebUtils;

public class BaseDialogPage : System.Web.UI.Page
{
  protected RadAjaxManager _manager;

  public string DialogResult { get; set; }

  public BaseDialogPage()
  {
  }

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    _manager = RadAjaxManager.GetCurrent(Page);
  }

  public virtual bool Save() { return false; }
  public virtual bool Close() { return false; }

  //protected override void SavePageStateToPersistenceMedium(object viewState)
  //{
  //  Session[Request.Url.AbsolutePath] = viewState;
  //}

  //protected override object LoadPageStateFromPersistenceMedium()
  //{
  //  object o = Session[Request.Url.AbsolutePath];
  //  if (o == null)
  //  {
  //    throw new Exception("The Viewstate is missing!!!");
  //  }
  //  else
  //  {
  //    return o;
  //  }
  //}

}
