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
using System.Text;

public partial class Dialogs_Dialog : System.Web.UI.MasterPage
{
  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);
  }
  protected void Page_Load(object sender, EventArgs e)
  {
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
    }

    if (Request["IsPrompt"] != null && Request["IsPrompt"] == "0")
    {
      btnCancel.Text = "OK";
      btnSave.Visible = false;
    }

  }
  protected void btnSave_Click(object sender, EventArgs e)
  {
    if (ContentPlaceHolder1.Page is BaseDialogPage)
    {

      bool saved = false;

      try
      {
        saved = (ContentPlaceHolder1.Page as BaseDialogPage).Save();
        pnlButtons.Visible = !saved;
      }
      catch (Exception ex)
      {
        saved = false;
        ajaxManager.Alert("There was an unexpected error saving your request.\n\n" + ex.Message);
      }

      if (saved)
      { 
        string result = (ContentPlaceHolder1.Page as BaseDialogPage).DialogResult;
        if (result == "") CloseWindow(); else CloseWindow(result);
      }
        
    } 
  }

  protected void btnCancel_Click(object sender, EventArgs e)
  {
      if (ContentPlaceHolder1.Page is BaseDialogPage)
      {
          try
          {
              (ContentPlaceHolder1.Page as BaseDialogPage).Close();
              CloseWindow();
          }
          catch (Exception ex)
          {
              CloseWindow();
          }
      }
  }

  private void CloseWindow()
  {
    //Dialogs.CloseDialog(Page, _windowName);
    //lblClose.Text = "<script type='text/javascript'>Close()</" + "script>";
    DynamicScript.ExecuteScript(Page, "CloseDialog", "Close();");
  }

  private void CloseWindow(string argument)
  {
    //Dialogs.CloseDialog(Page, _windowName);
    //lblClose.Text = "<script type='text/javascript'>Close()</" + "script>";
    DynamicScript.ExecuteScript(Page, "CloseDialog", "Close('"+argument+"');");
  }

  public class DynamicScript
  {
    private StringBuilder _builder;

    #region Properties

    private Page _page;
    public Page Page
    {
      get { return _page; }
      set { _page = value; }
    }

    private string _name;
    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    #endregion

    public DynamicScript(Page page, string name)
    {
      _page = page;
      _name = name;
      _builder = new StringBuilder();
    }


    private static string GetFunction(string name, string script)
    {
      return "function " + name + "(){" + script + "Sys.Application.remove_load(" + name + ");}";
    }

    private static string GetLoad(string name)
    {
      return "Sys.Application.add_load(" + name + ");";
    }

    public void Add(string text)
    {
      _builder.Append(text);
    }

    public void Execute()
    {
      ExecuteScript(_page, _name, _builder.ToString());
    }

    public static void ExecuteScript(Page page, string name, string script)
    {
      ScriptManager.RegisterClientScriptBlock(page, page.GetType(), name + "_function", GetFunction(name, script), true);
      ScriptManager.RegisterStartupScript(page, page.GetType(), name, GetLoad(name), true);

    }


  }

}
