using System;
using Telerik.Web.UI;

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

}
