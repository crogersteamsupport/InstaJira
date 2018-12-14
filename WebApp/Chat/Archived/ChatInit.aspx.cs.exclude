using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;

public partial class Chat_ChatInit : System.Web.UI.Page
{
  private Organization _organization;

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);

    try
    {
      if (Request["uid"] != null)
      {
        Organizations organizations = new Organizations(LoginUser.Anonymous);
        organizations.LoadByChatID(new Guid(Request["uid"]));
        if (organizations.IsEmpty) throw new Exception();
        _organization = organizations[0];
        if (_organization == null) throw new Exception();
      }
      else
      {
        throw new Exception();
      }
    }
    catch (Exception)
    {
      Response.Write("Unable to process your request.");
      Response.End();
    }

    if (!IsPostBack)
    {
      if (!ChatRequests.IsOperatorAvailable(LoginUser.Anonymous, _organization.OrganizationID))
        Response.Redirect("ChatOffline.aspx?" + Request.QueryString);
    }
  }


  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    if (_organization == null) return;

    if (!IsPostBack)
    {
      if (Request.Cookies["TSChat"] != null)
      {
        textEmail.Text = Request.Cookies["TSChat"]["Email"];
        textFirstName.Text = Request.Cookies["TSChat"]["FirstName"];
        textLastName.Text = Request.Cookies["TSChat"]["LastName"];
      }

      if (Request.Params["email"] != null) textEmail.Text = Request.Params["email"];
      if (Request.Params["fname"] != null) textFirstName.Text = Request.Params["fname"];
      if (Request.Params["lname"] != null) textLastName.Text = Request.Params["lname"];
      if (Request.Params["msg"] != null) textMessage.Text = Request.Params["msg"];
    }
  }
  protected void btnSubmit_Click(object sender, EventArgs e)
  {
    try
    {
      Response.Cookies["TSChat"]["Email"] = textEmail.Text;
      Response.Cookies["TSChat"]["FirstName"] = textFirstName.Text;
      Response.Cookies["TSChat"]["LastName"] = textLastName.Text;
      Response.Cookies["TSChat"].Expires = DateTime.UtcNow.AddYears(14);
    }
    catch (Exception)
    {
      
    }

    ChatRequest request = ChatRequests.RequestChat(LoginUser.Anonymous, _organization.OrganizationID, textFirstName.Text, textLastName.Text, textEmail.Text, textMessage.Text, Request.UserHostAddress);
    Response.Redirect(String.Format("Chat.aspx?cid={0}&rid={1}&uid={2}", request.ChatID.ToString(), request.ChatRequestID.ToString(), Request["uid"]), true);
  }
}
