using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using System.IO;
using System.Text;

public partial class Chat_Chat : System.Web.UI.MasterPage
{
  private Organization _organization = null;
  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);
    if (Request["uid"] != null)
    {
      Organizations organizations = new Organizations(LoginUser.Anonymous);
      organizations.LoadByChatID(new Guid(Request["uid"]));
      if (organizations.IsEmpty) throw new Exception("ChatID not found");
      _organization = organizations[0];
    }

    if (_organization != null)
    {
      string fileName = AttachmentPath.FindImageFileName(LoginUser.Anonymous, _organization.OrganizationID, AttachmentPath.Folder.ChatImages, "chat_logo");
      if (File.Exists(fileName))
      {
        System.Drawing.Image image = System.Drawing.Image.FromFile(fileName);
        string style = "background: #fff url('/dc/{0}/chat/logo') no-repeat; height: {1}px; width: {2}px; border: solid 1px #B8B8B8;";
        StringBuilder builder = new StringBuilder();
        builder.Append("<style type=\"text/css\">.chat-logo { ");
        builder.Append(string.Format(style, _organization.OrganizationID.ToString(), image.Height.ToString(), image.Width.ToString()));
        builder.Append("}</style>");
        litStyle.Text =  builder.ToString();
        image.Dispose();
      }
    
    
    }


    

  }
}

