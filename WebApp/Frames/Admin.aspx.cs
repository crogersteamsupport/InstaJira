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
using Telerik.Web.UI;
using System.Text;

public partial class Frames_Admin : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!IsPostBack)
    {
			//tsMain.SelectedIndex = Settings.UserDB.ReadInt("SelectedAdminTabIndex", 0);
			//if (tsMain.SelectedTab == null) tsMain.SelectedIndex = 0;
			//frmAdmin.Attributes["src"] = tsMain.SelectedTab.Value;

			//if (UserSession.LoginUser.OrganizationID == 1078
			//  || UserSession.LoginUser.OrganizationID == 13679
			//  || UserSession.LoginUser.OrganizationID == 362372
			//  || UserSession.LoginUser.OrganizationID == 991835
			//  || UserSession.LoginUser.OrganizationID == 967810
			//  || UserSession.LoginUser.OrganizationID == 1088)
			//{
			//  RadTab importTab = tsMain.FindTabByText("Import");
			//  importTab.Visible = true;
			//}
		}
  }
}
