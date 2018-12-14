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

public partial class Frames_History : BaseFramePage
{
  private ReferenceType _refType;
  private int _refID;

  protected void Page_Load(object sender, EventArgs e)
  {
    try
    {
      _refID = int.Parse(Request["RefID"]);
      _refType = (ReferenceType) int.Parse(Request["RefType"]);
      RadAjaxManager1.AjaxSettings.AddAjaxSetting(gridActionLogs, gridActionLogs);
    }
    catch (Exception)
    {
      Response.Write("Invalid parameters");
      Response.End();
      
    }
    Page.Culture = UserSession.LoginUser.CultureInfo.Name;



  }

  protected void gridActionLogs_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
  {
    ActionLogs actionLogs = new ActionLogs(UserSession.LoginUser);

    switch (_refType)
    {
      case ReferenceType.None:
        actionLogs.LoadAll();
        break;
      case ReferenceType.Actions:
        break;
      case ReferenceType.ActionTypes:
        break;
      case ReferenceType.Addresses:
        break;
      case ReferenceType.Attachments:
        break;
      case ReferenceType.CustomFields:
        break;
      case ReferenceType.CustomValues:
        break;
      case ReferenceType.Groups:
        actionLogs.LoadByGroupID(_refID);
        break;
      case ReferenceType.GroupUsers:
        break;
      case ReferenceType.OrganizationProducts:
        break;
      case ReferenceType.Organizations:
        actionLogs.LoadByOrganizationID(_refID);
        break;
      case ReferenceType.OrganizationTickets:
        break;
      case ReferenceType.PhoneNumbers:
        break;
      case ReferenceType.PhoneTypes:
        break;
      case ReferenceType.Products:
        actionLogs.LoadByProductID(_refID);
        break;
      case ReferenceType.ProductVersions:
        actionLogs.LoadByProductVersionID(_refID);
        break;
      case ReferenceType.ProductVersionStatuses:
        break;
      case ReferenceType.TechDocs:
        break;
      case ReferenceType.Tickets:
        actionLogs.LoadByTicketID(_refID);
        break;
      case ReferenceType.TicketSeverities:
        break;
      case ReferenceType.TicketStatuses:
        break;
      case ReferenceType.Subscriptions:
        break;
      case ReferenceType.TicketTypes:
        break;
      case ReferenceType.Users:
        actionLogs.LoadByUserID(_refID);
        break;
      case ReferenceType.ActionLogs:
        break;
      case ReferenceType.BillingInfo:
        break;
      case ReferenceType.ExceptionLogs:
        break;
      case ReferenceType.Invoices:
        break;
      case ReferenceType.SystemSettings:
        break;
      case ReferenceType.TicketNextStatuses:
        break;
      case ReferenceType.UserSettings:
        break;
      case ReferenceType.TicketQueue:
        break;
      case ReferenceType.CreditCards:
        break;
      case ReferenceType.Contacts:
        break;
      default:
        break;
    }
    gridActionLogs.VirtualItemCount = actionLogs.Count;
    gridActionLogs.DataSource = actionLogs;
  }
}
