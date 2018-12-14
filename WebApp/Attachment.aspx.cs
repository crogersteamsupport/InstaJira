using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.IO;

partial class Attachment : System.Web.UI.Page
{
  private AttachmentProxy _attachment;

  protected void Page_Load(object sender, EventArgs e)
  {
    int attachmentID = Request["AttachmentID"] == null ? -1 : int.Parse(Request["AttachmentID"]);
    _attachment = TeamSupport.ModelAPI.Model_API.Read<AttachmentProxy>(attachmentID);

    if (_attachment == null)
    {
      Response.Redirect("Message.aspx?Message=invalid_request");
      return;
    }

    if (!File.Exists(_attachment.Path))
    {
      Response.Redirect("Message.aspx?Message=invalid_request");
      return;
    }

    if (_attachment.OrganizationID != UserSession.LoginUser.OrganizationID)
    {
      Response.Redirect("Message.aspx?Message=unauthorized");
      return;
    }

    if (!IsPostBack)
    {
      lblFileName.Text = _attachment.FileName;
      lblSize.Text = (_attachment.FileSize / 1024).ToString() + " KB";
      
      System.Web.HttpBrowserCapabilities browser = Request.Browser;
      if (browser.Browser != "IE" || _attachment.FileType.ToLower().IndexOf("image") > -1)
      {
        OpenAttachment();
      }
    }
    else
    {
      OpenAttachment();
    }


  }
  
  protected void Button1_Click(object sender, EventArgs e)
  {
    OpenAttachment();
  }
  
  private void OpenAttachment()
  {
    if (!TeamSupport.Permissions.UserRights.CanOpenAttachment(UserSession.LoginUser, _attachment))
    {
      Response.Redirect("Message.aspx?Message=invalid_request");
      return;
    }

    Response.Clear();
    Response.ClearContent();
    Response.ClearHeaders();

    string openType = "inline";
    string fileType = _attachment.FileType;

    System.Web.HttpBrowserCapabilities browser = Request.Browser;
    if (browser.Browser == "IE")
    {
      if (_attachment.FileType.ToLower().IndexOf("audio") > -1)
      {
        openType = "attachment";
      }
      else if (_attachment.FileType.ToLower().IndexOf("-zip") > -1 ||
               _attachment.FileType.ToLower().IndexOf("/zip") > -1 ||
               _attachment.FileType.ToLower().IndexOf("zip-") > -1)
                
      {
        fileType = "application/octet-stream";
      }
    }

    Response.AddHeader("Content-Disposition", openType + "; filename=" + _attachment.FileName);
    Response.AddHeader("Content-Length", _attachment.FileSize.ToString());
    Response.ContentType = fileType;

    Response.WriteFile(_attachment.Path);
    Response.End();
  }


  protected void tmrDownload_Tick(object sender, EventArgs e)
  {
    OpenAttachment();
  }
}
