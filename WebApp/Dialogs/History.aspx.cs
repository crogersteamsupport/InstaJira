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
using TeamSupport.WebUtils;
using TeamSupport.Data;
using Telerik.Web.UI;
using System.IO;

public partial class Dialogs_History : System.Web.UI.Page
{
  private ReferenceType _referenceType;
  private int _referenceID;

  protected void Page_Load(object sender, EventArgs e)
  {
    Page.Culture = UserSession.LoginUser.CultureInfo.Name;
    try
    {
      _referenceID = int.Parse(Request["RefID"]);
      _referenceType = (ReferenceType)int.Parse(Request["RefType"]);

    }
    catch 
    {
      Response.Write("Invalid parameters.");
      Response.End();
    }
    
    if (!IsPostBack)
    {
      gridActionLogs.Rebind();
    }

  }


  protected void gridActionLogs_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
  {
    ActionLogs actionLogs = new ActionLogs(UserSession.LoginUser);
    if (_referenceType == ReferenceType.None)
    {
      actionLogs.LoadAll();
    }
    else if (_referenceType == ReferenceType.Groups)
    {
      actionLogs.LoadByGroupID(_referenceID);
    }
    else if (_referenceType == ReferenceType.Users)
    {
      actionLogs.LoadByUserID(_referenceID);
    }
    else if (_referenceType == ReferenceType.Organizations)
    {
      actionLogs.LoadByOrganizationID(_referenceID);
    }
    else if (_referenceType == ReferenceType.Products)
    {
      actionLogs.LoadByProductID(_referenceID);
    }
    else if (_referenceType == ReferenceType.ProductVersions)
    {
      actionLogs.LoadByProductVersionID(_referenceID);
    }
    else if (_referenceType == ReferenceType.Tickets)
    {
      actionLogs.LoadByTicketID(_referenceID);
    }

    gridActionLogs.VirtualItemCount = actionLogs.Count;
    gridActionLogs.DataSource = actionLogs;

  }

}
