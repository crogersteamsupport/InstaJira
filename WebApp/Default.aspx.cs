// File List: *.cs;*.vb;*.htm;*.html;*.css;*.aspx;*.asmx;*.js;*.master;*.config
using System;
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
using TeamSupport.WebUtils;
using TeamSupport.Data;
using Telerik.Web.UI;
using System.IO;
using System.IO.Compression;
using System.Drawing;
using System.Collections.Generic;
using System.Web.Services;
using System.Text;
using System.Net.Mail;
using dtSearch.Engine;

public partial class _Default : BasePage
{
  protected override void InitializeCulture()
  {
        //base.InitializeCulture();
        this.Culture = TSAuthentication.GetLoginUser().CultureInfo.Name;
        //this.Culture = UserSession.LoginUser.CultureInfo.Name;
        
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    //Session.Clear();
    //Session.Abandon();
    Response.Cache.SetAllowResponseInBrowserHistory(false);
    Response.Cache.SetCacheability(HttpCacheability.NoCache);
    Response.Cache.SetNoStore();
    Response.Expires = 0;

    fieldSID.Value = TSAuthentication.SessionID;
        Evergage.Value = SystemSettings.GetEverageDataset();

    ChatUserSetting setting = ChatUserSettings.GetSetting(UserSession.LoginUser, UserSession.LoginUser.UserID);
    setting.CurrentChatID = -1;
    setting.Collection.Save();

  }

  [WebMethod]
  public static RadComboBoxItemData[] GetUsers(RadComboBoxContext context)
  {
    IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;
    List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();
    try
    {
      Users users = new Users(UserSession.LoginUser);
      string[] s = context["FilterString"].ToString().Split(',');
      string filter = s[0];
      string search = s[1];
      switch (filter)
      {
        case "OtherChatUsers": users.LoadByName(search, UserSession.LoginUser.OrganizationID, true, false, true, UserSession.LoginUser.UserID); break;
        case "AllChatUsers": users.LoadByName(search, UserSession.LoginUser.OrganizationID, true, false, true); break;
        case "OtherUsers": users.LoadByName(search, UserSession.LoginUser.OrganizationID, true, false, false, UserSession.LoginUser.UserID); break;
        case "AdminUsers": users.LoadByName(search, UserSession.LoginUser.OrganizationID, true, true, false); break;
        default:
          users.LoadByName(search, UserSession.LoginUser.OrganizationID, true, false, false);
          break;
      }


      foreach (User user in users)
      {
        RadComboBoxItemData itemData = new RadComboBoxItemData();
        itemData.Text = user.FirstLastName;
        itemData.Value = user.UserID.ToString();
        list.Add(itemData);
      }
    }
    catch (Exception ex)
    {
      RadComboBoxItemData noData = new RadComboBoxItemData();
      noData.Text = ex.Message;
      noData.Value = "-1";
      list.Add(noData);
      return list.ToArray();
    }
    if (list.Count < 1)
    {
      RadComboBoxItemData noData = new RadComboBoxItemData();
      noData.Text = "[No users to display.]";
      noData.Value = "-1";
      list.Add(noData);
    }

    return list.ToArray();
  }

}

