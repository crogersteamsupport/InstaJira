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
using System.Web.Services;
using System.Collections.Generic;
using System.IO;


public partial class Frames_AdminCustomProperties : BaseFramePage
{
  public enum SelectedType { ActionTypes = 0, PhoneTypes=1, ProductVersionStatuses=2, TicketSeverities=3, TicketStatuses=4, TicketTypes=5, ActivityTypes = 6 };

  protected override void OnInit(EventArgs e)
  {
    base.OnInit(e);
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);

    if (!IsPostBack)
    { 
      cmbTypes.Items.Add(new RadComboBoxItem("Action Types", ((int)SelectedType.ActionTypes).ToString()));
      cmbTypes.Items.Add(new RadComboBoxItem("Phone Types", ((int)SelectedType.PhoneTypes).ToString()));
      cmbTypes.Items.Add(new RadComboBoxItem("Product Version Statuses", ((int)SelectedType.ProductVersionStatuses).ToString()));
      cmbTypes.Items.Add(new RadComboBoxItem("Ticket Severities", ((int)SelectedType.TicketSeverities).ToString()));
      cmbTypes.Items.Add(new RadComboBoxItem("Ticket Statuses", ((int)SelectedType.TicketStatuses).ToString()));
      cmbTypes.Items.Add(new RadComboBoxItem("Ticket Types", ((int)SelectedType.TicketTypes).ToString()));
      cmbTypes.Items.Add(new RadComboBoxItem("Activity Types", ((int)SelectedType.ActivityTypes).ToString()));

      /*int type = Settings.UserDB.ReadInt("SelectedCustomPropertyValue", ((int) SelectedType.ActionTypes));
      RadComboBoxItem item = cmbTypes.FindItemByValue(type.ToString());
      if (item != null) item.Selected = true;*/

      spanNewType.Visible = UserSession.CurrentUser.IsSystemAdmin;
      Organization organization = Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
    }
  }

  [WebMethod]
  public static ImageComboBoxData[] GetTicketTypeImagesComboData()
    {
        List<ImageComboBoxData> data = new List<ImageComboBoxData>();
        string[] files = TeamSupport.Data.Quarantine.WebAppQ.GetFiles(UserSession.LoginUser);

        foreach (string file in files)
        {
            ImageComboBoxData item = new ImageComboBoxData();
            item.Text = Path.GetFileName(file);
            item.Value = ("dc/" + UserSession.LoginUser.OrganizationID + "/images/tickettypes/" + item.Text).ToLower();
            item.ImageUrl = "../" + item.Value;
            data.Add(item);
        }

        string path = HttpContext.Current.Request.MapPath("../images/TicketTypes/");
        files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
        foreach (string file in files)
        {
            ImageComboBoxData item = new ImageComboBoxData();
            item.Text = Path.GetFileName(file);
            item.Value = ("images/TicketTypes/" + item.Text).ToLower();
            item.ImageUrl = "../" + item.Value;
            data.Add(item);
        }

        return data.ToArray();
    }


    [WebMethod]
  public static ImageComboBoxData[] GetTicketTypeProductFamilyComboData()
  {
      List<ImageComboBoxData> data = new List<ImageComboBoxData>();
      ProductFamilies productFamilies = new ProductFamilies(UserSession.LoginUser);
      productFamilies.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);
      ImageComboBoxData withoutProductFamily = new ImageComboBoxData();
      withoutProductFamily.Text = "Without Product Line";
      withoutProductFamily.Value = "-1";
      data.Add(withoutProductFamily);

      foreach (ProductFamily productFamily in productFamilies)
      {
          ImageComboBoxData item = new ImageComboBoxData();
          item.Text = productFamily.Name;
          item.Value = productFamily.ProductFamilyID.ToString();
          data.Add(item);
      }

      return data.ToArray();
  }

  [WebMethod]
  public static RadComboBoxItemData[] GetTicketTypesComboData()
  {
    TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);
    ticketTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);

    List<RadComboBoxItemData> data = new List<RadComboBoxItemData>();
    foreach (TicketType ticketType in ticketTypes)
    {
      RadComboBoxItemData item = new RadComboBoxItemData();
      item.Text = ticketType.Name;
      item.Value = ticketType.TicketTypeID.ToString();
      data.Add(item);
    }
    return data.ToArray();
  }

  [WebMethod]
  public static RadComboBoxItemData[] GetTypesCombo()
  {
    TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);
    ticketTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);

    List<RadComboBoxItemData> data = new List<RadComboBoxItemData>();
    foreach (TicketType ticketType in ticketTypes)
    {
      RadComboBoxItemData item = new RadComboBoxItemData();
      item.Text = ticketType.Name;
      item.Value = ticketType.TicketTypeID.ToString();
      data.Add(item);
    }
    return data.ToArray();
  }

  [WebMethod]
  public static RadComboBoxItemData[] GetReplaceTypeComboData(int id, SelectedType type, int ticketTypeID)
  {
    //IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;
    List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();
    /*string[] s = context["FilterString"].ToString().Split(',');
    SelectedType type = (SelectedType)int.Parse(s[0]);
    int ticketTypeID = int.Parse(s[1]);
    int id = int.Parse(s[2]);*/

    BaseCollection collection = null;
    string nameColName = "Name";
    string idColName = "ID";
    switch (type)
    {
      case SelectedType.ActionTypes:
        ActionTypes actionTypes = new ActionTypes(UserSession.LoginUser);
        actionTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);
        collection = actionTypes;
        idColName = "ActionTypeID";
        break;
      case SelectedType.PhoneTypes:
        PhoneTypes phoneTypes = new PhoneTypes(UserSession.LoginUser);
        phoneTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);
        collection = phoneTypes;
        idColName = "PhoneTypeID";
        break;
      case SelectedType.ProductVersionStatuses:
        ProductVersionStatuses productVersionStatuses = new ProductVersionStatuses(UserSession.LoginUser);
        productVersionStatuses.LoadAllPositions(UserSession.LoginUser.OrganizationID);
        collection = productVersionStatuses;
        idColName = "ProductVersionStatusID";
        break;
      case SelectedType.TicketSeverities:
        TicketSeverities ticketSeverities = new TicketSeverities(UserSession.LoginUser);
        ticketSeverities.LoadAllPositions(UserSession.LoginUser.OrganizationID);
        collection = ticketSeverities;
        idColName = "TicketSeverityID";
        break;
      case SelectedType.TicketStatuses:
        TicketStatuses ticketStatuses = new TicketStatuses(UserSession.LoginUser);
        TicketType ticketType = TicketTypes.GetTicketType(UserSession.LoginUser, ticketTypeID);
        if (ticketType.OrganizationID == UserSession.LoginUser.OrganizationID)
        {
          ticketStatuses.LoadAllPositions(ticketTypeID);
          collection = ticketStatuses;
          idColName = "TicketStatusID";
        }
        break;
      case SelectedType.TicketTypes:
        TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);
        ticketTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);
        collection = ticketTypes;
        idColName = "TicketTypeID";
        break;
      case SelectedType.ActivityTypes:
        ActivityTypes activityTypes = new ActivityTypes(UserSession.LoginUser);
        activityTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);
        //Load base items
        //Get default activity types
        foreach (ActivityTypeEnum activity in Enum.GetValues(typeof(ActivityTypeEnum)))
        {
            var value = Enum.Parse(typeof(ActivityTypeEnum), activity.ToString());
            //results.Add(new ActivityTypesDropDown() { Name = activity.ToString(), Value = (int)value });
            RadComboBoxItemData itemData = new RadComboBoxItemData();
            itemData.Text = activity.ToString();
            itemData.Value = ((int)value).ToString();
            list.Add(itemData);
        }

        collection = activityTypes;
        idColName = "ActivityTypeID";
        break;
      default:
        break;
    }

    foreach (DataRow row in collection.Table.Rows)
    {
      int i = (int)row[idColName];
      if (id != i) 
      {
        RadComboBoxItemData itemData = new RadComboBoxItemData();
        itemData.Text = row[nameColName].ToString();
        itemData.Value = i.ToString();
        list.Add(itemData);
      }
    }

    if (list.Count < 1)
    {
      RadComboBoxItemData noData = new RadComboBoxItemData();
      noData.Text = "[No types to display.]";
      noData.Value = "-1";
      list.Add(noData);
    }

    return list.ToArray();
  }

    /// <summary>
    /// Generates the HTML Table to be returned client side.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [WebMethod]
    public static TypesHtmlResult GetTypesHtml2(SelectedType type, string arg)
    {
        Organization organization = Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
        DataTable table = new DataTable();
        table.Columns.Add("ID");
        table.Columns.Add("Name");
        table.Columns.Add("Description");

        switch (type)
        {
            case SelectedType.ActionTypes:
                ActionTypes actionTypes = new ActionTypes(UserSession.LoginUser);
                actionTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);
                table.Columns.Add("Is Timed");
                foreach (ActionType actionType in actionTypes)
                {
                    table.Rows.Add(new string[] { actionType.ActionTypeID.ToString(), actionType.Name, actionType.Description, actionType.IsTimed.ToString() });
                }
                break;
            case SelectedType.PhoneTypes:
                PhoneTypes phoneTypes = new PhoneTypes(UserSession.LoginUser);
                phoneTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);
                foreach (PhoneType phoneType in phoneTypes)
                {
                    table.Rows.Add(new string[] { phoneType.PhoneTypeID.ToString(), phoneType.Name, phoneType.Description });
                }
                break;
            case SelectedType.ProductVersionStatuses:
                ProductVersionStatuses productVersionStatuses = new ProductVersionStatuses(UserSession.LoginUser);
                productVersionStatuses.LoadAllPositions(UserSession.LoginUser.OrganizationID);
                table.Columns.Add("Is Shipping");
                table.Columns.Add("Is Discontinued");
                foreach (ProductVersionStatus productVersionStatus in productVersionStatuses)
                {
                    table.Rows.Add(new string[] { productVersionStatus.ProductVersionStatusID.ToString(), productVersionStatus.Name, productVersionStatus.Description, productVersionStatus.IsShipping.ToString(), productVersionStatus.IsDiscontinued.ToString() });
                }
                break;
            case SelectedType.TicketSeverities:
                TicketSeverities ticketSeverities = new TicketSeverities(UserSession.LoginUser);
                ticketSeverities.LoadAllPositions(UserSession.LoginUser.OrganizationID);
                table.Columns.Add("Visible on Portal");
                foreach (TicketSeverity ticketSeverity in ticketSeverities)
                {
                    table.Rows.Add(new string[] { ticketSeverity.TicketSeverityID.ToString(), ticketSeverity.Name, ticketSeverity.Description, ticketSeverity.VisibleOnPortal.ToString() });
                }
                break;
            case SelectedType.TicketStatuses:
                TicketStatuses ticketStatuses = new TicketStatuses(UserSession.LoginUser);
                ticketStatuses.LoadAllPositions(int.Parse(arg));

                table.Columns.Add("Is Closed");
                table.Columns.Add("Closed Email");
                table.Columns.Add("Email Response");
                table.Columns.Add("Pause SLA?");

                foreach (TicketStatus ticketStatus in ticketStatuses)
                {
                    table.Rows.Add(new string[] { ticketStatus.TicketStatusID.ToString(), ticketStatus.Name, ticketStatus.Description, ticketStatus.IsClosed.ToString(), ticketStatus.IsClosedEmail.ToString(), ticketStatus.IsEmailResponse.ToString(), ticketStatus.PauseSLA.ToString() });
                }
                break;
            case SelectedType.TicketTypes:
                table.Columns.Add("Icon");
                table.Columns.Add("Visible on Portal");
                string icon = "<img src=\"../{0}\" />";
                if (organization.UseProductFamilies)
                {
                    TicketTypesView ticketTypes = new TicketTypesView(UserSession.LoginUser);
                    ticketTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);
                    table.Columns.Add("Product Line");
                    table.Columns.Add("Active");
                    table.Columns.Add("Exclude From CDI");
                    foreach (TicketTypesViewItem ticketType in ticketTypes)
                    {
                        table.Rows.Add(new string[] { ticketType.TicketTypeID.ToString(), ticketType.Name, ticketType.Description, string.Format(icon, ticketType.IconUrl), ticketType.IsVisibleOnPortal.ToString(), ticketType.ProductFamilyName, ticketType.IsActive.ToString(), ticketType.ExcludeFromCDI.ToString() });
                    }
                }
                else
                {
                    table.Columns.Add("Active");
                    TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);
                    ticketTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);
                    table.Columns.Add("Exclude From CDI");
                    foreach (TicketType ticketType in ticketTypes)
                    {
                        table.Rows.Add(new string[] { ticketType.TicketTypeID.ToString(), ticketType.Name, ticketType.Description, string.Format(icon, ticketType.IconUrl), ticketType.IsVisibleOnPortal.ToString(), ticketType.IsActive.ToString(), ticketType.ExcludeFromCDI.ToString() });
                    }
                }
                break;
            case SelectedType.ActivityTypes:
                ActivityTypes activityTypes = new ActivityTypes(UserSession.LoginUser);
                activityTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);
                foreach (ActivityType activityType in activityTypes)
                {
                    table.Rows.Add(new string[] { activityType.ActivityTypeID.ToString(), activityType.Name, activityType.Description });
                }
                break;
            default:
                break;
        }

        TypesHtmlResult result = new TypesHtmlResult();
        result.Html = BuildTable(table);
        result.UseProductFamilies = organization.UseProductFamilies;

        return result;

    }

    /// <summary>
    /// Generates the HTML Table to be returned client side.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [WebMethod]
  public static string GetTypesHtml(SelectedType type, string arg)
  {
      Organization organization = Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
    DataTable table = new DataTable();
    table.Columns.Add("ID");
    table.Columns.Add("Name");
    table.Columns.Add("Description");
    string result = "";
    switch (type)
    {
      case SelectedType.ActionTypes:
        ActionTypes actionTypes = new ActionTypes(UserSession.LoginUser);
        actionTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);
        table.Columns.Add("Is Timed");
        foreach (ActionType actionType in actionTypes)
        {
          table.Rows.Add(new string[] {actionType.ActionTypeID.ToString(), actionType.Name, actionType.Description, actionType.IsTimed.ToString() });
        }
        break;
      case SelectedType.PhoneTypes:
        PhoneTypes phoneTypes = new PhoneTypes(UserSession.LoginUser);
        phoneTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);
        foreach (PhoneType phoneType in phoneTypes)
        {
          table.Rows.Add(new string[] { phoneType.PhoneTypeID.ToString(), phoneType.Name, phoneType.Description });
        }
        break;
      case SelectedType.ProductVersionStatuses:
        ProductVersionStatuses productVersionStatuses = new ProductVersionStatuses(UserSession.LoginUser);
        productVersionStatuses.LoadAllPositions(UserSession.LoginUser.OrganizationID);
        table.Columns.Add("Is Shipping");
        table.Columns.Add("Is Discontinued");
        foreach (ProductVersionStatus productVersionStatus in productVersionStatuses)
        {
          table.Rows.Add(new string[] { productVersionStatus.ProductVersionStatusID.ToString(), productVersionStatus.Name, productVersionStatus.Description, productVersionStatus.IsShipping.ToString(), productVersionStatus.IsDiscontinued.ToString() });
        }
        break;
      case SelectedType.TicketSeverities:
        table.Columns.Add("Visible on Portal");
        TicketSeverities ticketSeverities = new TicketSeverities(UserSession.LoginUser);
        ticketSeverities.LoadAllPositions(UserSession.LoginUser.OrganizationID);
        foreach (TicketSeverity ticketSeverity in ticketSeverities)
        {
            table.Rows.Add(new string[] { ticketSeverity.TicketSeverityID.ToString(), ticketSeverity.Name, ticketSeverity.Description, ticketSeverity.VisibleOnPortal.ToString() });
        }
        break;
      case SelectedType.TicketStatuses:
        TicketStatuses ticketStatuses = new TicketStatuses(UserSession.LoginUser);
        ticketStatuses.LoadAllPositions(int.Parse(arg));

        table.Columns.Add("Is Closed");
        table.Columns.Add("Closed Email");
        table.Columns.Add("Email Response");
        table.Columns.Add("Pause SLA");

        foreach (TicketStatus ticketStatus in ticketStatuses)
        {
          table.Rows.Add(new string[] { ticketStatus.TicketStatusID.ToString(), ticketStatus.Name, ticketStatus.Description, ticketStatus.IsClosed.ToString(), ticketStatus.IsClosedEmail.ToString(), ticketStatus.IsEmailResponse.ToString(), ticketStatus.PauseSLA.ToString() });
        }
        break;
      case SelectedType.TicketTypes:
        table.Columns.Add("Icon");
        table.Columns.Add("Visible on Portal");
        string icon = "<img src=\"../{0}\" />";
        if (organization.UseProductFamilies)
        {
            TicketTypesView ticketTypes = new TicketTypesView(UserSession.LoginUser);
            ticketTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);
            table.Columns.Add("Product Line");
            table.Columns.Add("Active");
            foreach (TicketTypesViewItem ticketType in ticketTypes)
            {
                table.Rows.Add(new string[] { ticketType.TicketTypeID.ToString(), ticketType.Name, ticketType.Description, string.Format(icon, ticketType.IconUrl), ticketType.IsVisibleOnPortal.ToString(), ticketType.ProductFamilyName });
            }
        }
        else
        {
            TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);
            ticketTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);
            table.Columns.Add("Active");
            foreach (TicketType ticketType in ticketTypes)
            {
                table.Rows.Add(new string[] { ticketType.TicketTypeID.ToString(), ticketType.Name, ticketType.Description, string.Format(icon, ticketType.IconUrl), ticketType.IsVisibleOnPortal.ToString() });
            }
        }
        break;
        case SelectedType.ActivityTypes:
            ActivityTypes activityTypes = new ActivityTypes(UserSession.LoginUser);
            activityTypes.LoadAllPositions(UserSession.LoginUser.OrganizationID);
            foreach (ActivityType activityType in activityTypes)
            {
              table.Rows.Add(new string[] { activityType.ActivityTypeID.ToString(), activityType.Name, activityType.Description });
            }
        break;
      default:
        break;
    }

    return BuildTable(table);

  }

  /// <summary>
  /// Generates HTML from a DataTable.  The first column must be the ID.
  /// </summary>
  /// <param name="table"></param>
  /// <returns></returns>
  private static string BuildTable(DataTable table)
  { 
    StringBuilder builder = new StringBuilder();
    builder.Append("<table width=\"748px\" border=\"0\" cellpadding=\"5\" cellspacing=\"0\"><thead><tr><th /><th /><th /><th />");
    for (int i = 0; i < table.Columns.Count; i++)
		{
      builder.Append("<th>" + table.Columns[i].ColumnName + "</th>");
		}
    builder.Append("</tr></thead><tbody>");

    string editFormat = "<img src=\"../images/icons/Edit.png\" alt=\"Edit\" onclick=\"editType({0});\" />";
    string deleteFormat = "<img src=\"../images/icons/Trash.png\" alt=\"Delete\" onclick=\"deleteType({0},'{1}');\" />";
    string upFormat = "<img src=\"../images/icons/Arrow_Up.png\" alt=\"Move Up\" onclick=\"moveUp({0});\" />";
    string downFormat = "<img src=\"../images/icons/Arrow_Down.png\" alt=\"Move Down\" onclick=\"moveDown({0});\" />";

    foreach (DataRow row in table.Rows)
	  {
      string edit = UserSession.CurrentUser.IsSystemAdmin ? string.Format(editFormat, row[0].ToString()) : "&nbsp";
      string delete = UserSession.CurrentUser.IsSystemAdmin ? string.Format(deleteFormat, row[0].ToString(), row[1].ToString()) : "&nbsp";
      string up = UserSession.CurrentUser.IsSystemAdmin ? string.Format(upFormat, row[0].ToString()) : "&nbsp";
      string down = UserSession.CurrentUser.IsSystemAdmin ? string.Format(downFormat, row[0].ToString()) : "&nbsp";
      if (table.Rows.Count < 2) delete = up = down = "&nbsp";
      
      builder.Append(string.Format("<tr><td class=\"headImage\">{0}</td><td class=\"headImage\">{1}</td><td class=\"headImage\">{2}</td><td class=\"headImage\">{3}</td>", edit, delete, up, down));
      for (int i = 0; i < table.Columns.Count; i++)
		  {
        var s = row[i].ToString().Trim() == "" ? " &nbsp " : row[i].ToString();
        builder.Append("<td>" + s + "</td>");
		  }
      builder.Append("</tr>");
	  }

    if (table.Rows.Count < 1)
    {
      builder.Append("<tr><td colspan=\"7\" >There are no types to display.</td></tr>");
    }
    builder.Append("</tbody></table>");

    return builder.ToString();
  }

  [WebMethod]
  public static string CanDelete(int id, SelectedType type, string arg)
  {
    string result = null;
    switch (type)
    {
      case SelectedType.ActionTypes:
        break;
      case SelectedType.PhoneTypes:
        break;
      case SelectedType.ProductVersionStatuses:
        break;
      case SelectedType.TicketSeverities:
        break;
      case SelectedType.TicketStatuses:
        break;
      case SelectedType.TicketTypes:
        Tickets tickets = new Tickets(UserSession.LoginUser);
        tickets.LoadByTicketType(id);
        if (tickets.Count > 0) result = string.Format("This ticket type contains {0} tickets.  Please reassign these tickets before deleting this ticket type.  Please note that you can also rename this ticket type as well", tickets.Count);
        break;
      case SelectedType.ActivityTypes:
        break;
      default:
        break;
    }
    return result;
  }

  [WebMethod]
  public static string ReplaceType(SelectedType type, int oldID, int newID, string arg)
  {
    if (!UserSession.CurrentUser.IsSystemAdmin) return "";
    switch (type)
    {
      case SelectedType.ActionTypes:
        if (ActionTypes.GetActionType(UserSession.LoginUser, oldID).OrganizationID != UserSession.LoginUser.OrganizationID) return "";
        if (ActionTypes.GetActionType(UserSession.LoginUser, newID).OrganizationID != UserSession.LoginUser.OrganizationID) return "";
        (new Actions(UserSession.LoginUser)).ReplaceActionType(oldID, newID);
        ActionTypes actionTypes = new ActionTypes(UserSession.LoginUser);
        actionTypes.DeleteFromDB(oldID);
        actionTypes.ValidatePositions(UserSession.LoginUser.OrganizationID);
        break;
      case SelectedType.PhoneTypes:
        if (PhoneTypes.GetPhoneType(UserSession.LoginUser, oldID).OrganizationID != UserSession.LoginUser.OrganizationID) return "";
        if (PhoneTypes.GetPhoneType(UserSession.LoginUser, newID).OrganizationID != UserSession.LoginUser.OrganizationID) return "";
        (new PhoneNumbers(UserSession.LoginUser)).ReplacePhoneType(oldID, newID);
        PhoneTypes phoneTypes = new PhoneTypes(UserSession.LoginUser);
        phoneTypes.DeleteFromDB(oldID);
        phoneTypes.ValidatePositions(UserSession.LoginUser.OrganizationID);
        break;
      case SelectedType.ProductVersionStatuses:
        if (ProductVersionStatuses.GetProductVersionStatus(UserSession.LoginUser, oldID).OrganizationID != UserSession.LoginUser.OrganizationID) return "";
        if (ProductVersionStatuses.GetProductVersionStatus(UserSession.LoginUser, newID).OrganizationID != UserSession.LoginUser.OrganizationID) return "";
        (new ProductVersions(UserSession.LoginUser)).ReplaceProductVersionStatus(oldID, newID);
        ProductVersionStatuses productVersionStatuses = new ProductVersionStatuses(UserSession.LoginUser);
        productVersionStatuses.DeleteFromDB(oldID);
        productVersionStatuses.ValidatePositions(UserSession.LoginUser.OrganizationID);
        break;
      case SelectedType.TicketSeverities:
        if (TicketSeverities.GetTicketSeverity(UserSession.LoginUser, oldID).OrganizationID != UserSession.LoginUser.OrganizationID) return "";
        if (TicketSeverities.GetTicketSeverity(UserSession.LoginUser, newID).OrganizationID != UserSession.LoginUser.OrganizationID) return "";
        (new Tickets(UserSession.LoginUser)).ReplaceTicketSeverity(oldID, newID);
        TicketSeverities ticketSeverities = new TicketSeverities(UserSession.LoginUser);
        ticketSeverities.DeleteFromDB(oldID);
        ticketSeverities.ValidatePositions(UserSession.LoginUser.OrganizationID);
        break;
      case SelectedType.TicketStatuses:
        TicketStatus oldStatus = TicketStatuses.GetTicketStatus(UserSession.LoginUser, oldID);
        if (oldStatus.OrganizationID != UserSession.LoginUser.OrganizationID) return "";
        if (TicketStatuses.GetTicketStatus(UserSession.LoginUser, newID).OrganizationID != UserSession.LoginUser.OrganizationID) return "";
        (new Tickets(UserSession.LoginUser)).ReplaceTicketStatus(oldID, newID);
        TicketStatuses ticketStatuses = new TicketStatuses(UserSession.LoginUser);
        ticketStatuses.DeleteFromDB(oldID);
        ticketStatuses.ValidatePositions(oldStatus.TicketTypeID);
        break;
      case SelectedType.TicketTypes:
        if (TicketTypes.GetTicketType(UserSession.LoginUser, oldID).OrganizationID != UserSession.LoginUser.OrganizationID) return "";
        if (TicketTypes.GetTicketType(UserSession.LoginUser, newID).OrganizationID != UserSession.LoginUser.OrganizationID) return "";
        (new Tickets(UserSession.LoginUser)).ReplaceTicketType(oldID, newID);
        TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);

		CustomFields customFields = new CustomFields(UserSession.LoginUser);
		customFields.LoadByTicketTypeID(UserSession.LoginUser.OrganizationID, oldID);

		ticketTypes.DeleteFromDB(oldID);
        ticketTypes.ValidatePositions(UserSession.LoginUser.OrganizationID);

		int? crmLinkFieldId = null;

		foreach (CustomField customField in customFields)
		{
			try
			{
				crmLinkFieldId = CRMLinkFields.FindIdByCustomFieldId(customField.CustomFieldID, UserSession.LoginUser);
			}
			catch (Exception ex)
			{
				crmLinkFieldId = null;
			}
			
			if (crmLinkFieldId != null && crmLinkFieldId > 0)
			{
				CRMLinkFields crmLinkFieldsDelete = new CRMLinkFields(UserSession.LoginUser);
				crmLinkFieldsDelete.DeleteFromDB((int)crmLinkFieldId);
			}
		}
		
        break;
        case SelectedType.ActivityTypes:
            if (ActivityTypes.GetActivityType(UserSession.LoginUser, oldID).OrganizationID != UserSession.LoginUser.OrganizationID) return "";
            var types = Enum.GetValues(typeof(ActivityTypeEnum));
            if(newID > types.Length)
                if (ActivityTypes.GetActivityType(UserSession.LoginUser, newID).OrganizationID != UserSession.LoginUser.OrganizationID) return "";
            (new Notes(UserSession.LoginUser)).ReplaceActivityType(oldID, newID);
            ActivityTypes activityTypes = new ActivityTypes(UserSession.LoginUser);
            activityTypes.DeleteFromDB(oldID);
            activityTypes.ValidatePositions(UserSession.LoginUser.OrganizationID);
        break;
      default:
        break;
    }
    return GetTypesHtml(type, arg);
  }

  [WebMethod]
  public static string MoveUp(SelectedType type, int id, string arg)
  {
    if (!UserSession.CurrentUser.IsSystemAdmin) return "";
    switch (type)
    {
      case SelectedType.ActionTypes: (new ActionTypes(UserSession.LoginUser)).MovePositionUp(id); break;
      case SelectedType.PhoneTypes: (new PhoneTypes(UserSession.LoginUser)).MovePositionUp(id); break;
      case SelectedType.ProductVersionStatuses: (new ProductVersionStatuses(UserSession.LoginUser)).MovePositionUp(id); break;
      case SelectedType.TicketSeverities: (new TicketSeverities(UserSession.LoginUser)).MovePositionUp(id); break;
      case SelectedType.TicketStatuses: (new TicketStatuses(UserSession.LoginUser)).MovePositionUp(id); break;
      case SelectedType.TicketTypes: (new TicketTypes(UserSession.LoginUser)).MovePositionUp(id); break;
      case SelectedType.ActivityTypes: (new PhoneTypes(UserSession.LoginUser)).MovePositionUp(id); break;
      default: break;
    }
    return GetTypesHtml(type, arg);
  }

  [WebMethod]
  public static string MoveDown(SelectedType type, int id, string arg)
  {
    if (!UserSession.CurrentUser.IsSystemAdmin) return "";
    switch (type)
    {
      case SelectedType.ActionTypes: (new ActionTypes(UserSession.LoginUser)).MovePositionDown(id); break;
      case SelectedType.PhoneTypes: (new PhoneTypes(UserSession.LoginUser)).MovePositionDown(id); break;
      case SelectedType.ProductVersionStatuses: (new ProductVersionStatuses(UserSession.LoginUser)).MovePositionDown(id); break;
      case SelectedType.TicketSeverities: (new TicketSeverities(UserSession.LoginUser)).MovePositionDown(id); break;
      case SelectedType.TicketStatuses: (new TicketStatuses(UserSession.LoginUser)).MovePositionDown(id); break;
      case SelectedType.TicketTypes: (new TicketTypes(UserSession.LoginUser)).MovePositionDown(id); break;
      case SelectedType.ActivityTypes: (new PhoneTypes(UserSession.LoginUser)).MovePositionDown(id); break;
      default: break;
    }
    return GetTypesHtml(type, arg);
  }

  [WebMethod]
  public static TypeObject GetTypeObject(SelectedType type, int id)
  {
    TypeObject result = new TypeObject();
    
    switch (type)
    {
      case SelectedType.ActionTypes:
        ActionType actionType =  ActionTypes.GetActionType(UserSession.LoginUser, id);
        result.ID = actionType.ActionTypeID;
        result.Name = actionType.Name;
        result.Description = actionType.Description;
        result.IsTimed = actionType.IsTimed;
        break;
      case SelectedType.PhoneTypes:
        PhoneType phoneType = PhoneTypes.GetPhoneType(UserSession.LoginUser, id);
        result.ID = phoneType.PhoneTypeID;
        result.Name = phoneType.Name;
        result.Description = phoneType.Description;
        break;
      case SelectedType.ProductVersionStatuses:
        ProductVersionStatus productVersionStatus = ProductVersionStatuses.GetProductVersionStatus(UserSession.LoginUser, id);
        result.ID = productVersionStatus.ProductVersionStatusID;
        result.Name = productVersionStatus.Name;
        result.Description = productVersionStatus.Description;
        result.IsShipping = productVersionStatus.IsShipping;
        result.IsDiscontinued = productVersionStatus.IsDiscontinued;
        break;
      case SelectedType.TicketSeverities:
        TicketSeverity ticketSeverity = TicketSeverities.GetTicketSeverity(UserSession.LoginUser, id);
        result.ID = ticketSeverity.TicketSeverityID;
        result.Name = ticketSeverity.Name;
        result.Description = ticketSeverity.Description;
        result.IsVisibleOnPortal = ticketSeverity.VisibleOnPortal;
        break;
      case SelectedType.TicketStatuses:
        TicketStatus ticketStatus = TicketStatuses.GetTicketStatus(UserSession.LoginUser, id);
        result.ID = ticketStatus.TicketStatusID;
        result.Name = ticketStatus.Name;
        result.Description = ticketStatus.Description;
        result.IsClosed = ticketStatus.IsClosed;
        result.IsClosedEmail = ticketStatus.IsClosedEmail;
        result.IsEmailResponse = ticketStatus.IsEmailResponse;
        result.PauseSla = ticketStatus.PauseSLA;
        break;
      case SelectedType.TicketTypes:
        TicketType ticketType = TicketTypes.GetTicketType(UserSession.LoginUser, id);
        result.ID = ticketType.TicketTypeID;
        result.Name = ticketType.Name;
        result.Description = ticketType.Description;
        result.IsVisibleOnPortal = ticketType.IsVisibleOnPortal;
        result.IconUrl = ticketType.IconUrl;
        result.IsActive = ticketType.IsActive;
        result.ExcludeFromCDI = ticketType.ExcludeFromCDI;

        if (ticketType.ProductFamilyID == null)
        {
            result.ProductFamilyID = -1;
        }
        else
        {
            result.ProductFamilyID = (int)ticketType.ProductFamilyID;
        }

        break;
        case SelectedType.ActivityTypes:
            ActivityType activityType = ActivityTypes.GetActivityType(UserSession.LoginUser, id);
            result.ID = activityType.ActivityTypeID;
            result.Name = activityType.Name;
            result.Description = activityType.Description;
        break;
      default:
        break;
    }

    return result;
  }

  [WebMethod]
  public static string UpdateType(SelectedType type, 
      string arg, 
      int? id, 
      string name, 
      string description, 
      bool isTimed, 
      bool isClosed, 
      bool isClosedEmail, 
      bool isEmailResponse, 
      bool pauseSla,
      bool isShipping, 
      bool isDiscontinued,
      string productFamilyID,
      string iconUrl, 
      bool isVisibleOnPortal,
      bool isActive,
      bool ExcludeFromCDI)
  {
    if (!UserSession.CurrentUser.IsSystemAdmin) return "";
    switch (type)
    {
      case SelectedType.ActionTypes:
        ActionType actionType = id == null ? (new ActionTypes(UserSession.LoginUser)).AddNewActionType() : ActionTypes.GetActionType(UserSession.LoginUser, (int)id);
        actionType.IsTimed = isTimed;
        actionType.Name = name;
        actionType.Description = description;
        if (id == null) actionType.Position = actionType.Collection.GetMaxPosition(UserSession.LoginUser.OrganizationID) + 1;
        if (id == null) actionType.OrganizationID = UserSession.LoginUser.OrganizationID;
        actionType.Collection.Save();
        actionType.Collection.ValidatePositions(UserSession.LoginUser.OrganizationID);
        break;
      case SelectedType.PhoneTypes:
        PhoneType phoneType = id == null ? (new PhoneTypes(UserSession.LoginUser)).AddNewPhoneType() : PhoneTypes.GetPhoneType(UserSession.LoginUser, (int)id);
        phoneType.Name = name;
        phoneType.Description = description;
        if (id == null) phoneType.Position = phoneType.Collection.GetMaxPosition(UserSession.LoginUser.OrganizationID) + 1;
        if (id == null) phoneType.OrganizationID = UserSession.LoginUser.OrganizationID;
        phoneType.Collection.Save();
        phoneType.Collection.ValidatePositions(UserSession.LoginUser.OrganizationID);
        break;
      case SelectedType.ProductVersionStatuses:
        ProductVersionStatus productVersionStatus = id == null ? (new ProductVersionStatuses(UserSession.LoginUser)).AddNewProductVersionStatus() : ProductVersionStatuses.GetProductVersionStatus(UserSession.LoginUser, (int)id);
        productVersionStatus.IsDiscontinued = isDiscontinued;
        productVersionStatus.IsShipping = isShipping;
        productVersionStatus.Name = name;
        productVersionStatus.Description = description;
        if (id == null) productVersionStatus.Position = productVersionStatus.Collection.GetMaxPosition(UserSession.LoginUser.OrganizationID) + 1;
        if (id == null) productVersionStatus.OrganizationID = UserSession.LoginUser.OrganizationID;
        productVersionStatus.Collection.Save();
        productVersionStatus.Collection.ValidatePositions(UserSession.LoginUser.OrganizationID);
        break;
      case SelectedType.TicketSeverities:
        TicketSeverity ticketSeverity = id == null ? (new TicketSeverities(UserSession.LoginUser)).AddNewTicketSeverity() : TicketSeverities.GetTicketSeverity(UserSession.LoginUser, (int)id);
        ticketSeverity.Name = name;
        ticketSeverity.Description = description;
        if (id == null) ticketSeverity.Position = ticketSeverity.Collection.GetMaxPosition(UserSession.LoginUser.OrganizationID) + 1;
        if (id == null) ticketSeverity.OrganizationID = UserSession.LoginUser.OrganizationID;
        ticketSeverity.VisibleOnPortal = isVisibleOnPortal;
        ticketSeverity.Collection.Save();
        ticketSeverity.Collection.ValidatePositions(UserSession.LoginUser.OrganizationID);
        break;
      case SelectedType.TicketStatuses:
        if (isEmailResponse == true)
        {
          TicketStatuses statuses = new TicketStatuses(UserSession.LoginUser);
          statuses.LoadByTicketTypeID(int.Parse(arg));
          foreach (TicketStatus status in statuses)
          {
            status.IsEmailResponse = false;
          }
          statuses.Save();
        }

        TicketStatus ticketStatus = id == null ? (new TicketStatuses(UserSession.LoginUser)).AddNewTicketStatus() : TicketStatuses.GetTicketStatus(UserSession.LoginUser, (int)id);
        ticketStatus.TicketTypeID = int.Parse(arg);
        ticketStatus.IsClosed = isClosed;
        ticketStatus.IsClosedEmail = isClosedEmail;
        ticketStatus.IsEmailResponse = isEmailResponse;
        ticketStatus.PauseSLA = pauseSla;
        ticketStatus.Name = name;
        ticketStatus.Description = description;
        if (id == null) ticketStatus.Position = ticketStatus.Collection.GetMaxPosition(UserSession.LoginUser.OrganizationID) + 1;
        if (id == null) ticketStatus.OrganizationID = UserSession.LoginUser.OrganizationID;
        ticketStatus.Collection.Save();
        ticketStatus.Collection.ValidatePositions(UserSession.LoginUser.OrganizationID);
        break;
      case SelectedType.TicketTypes:
        TicketType ticketType = id == null ? (new TicketTypes(UserSession.LoginUser)).AddNewTicketType() : TicketTypes.GetTicketType(UserSession.LoginUser, (int)id);
        ticketType.Name = name;
        ticketType.Description = description;
        ticketType.IconUrl = iconUrl;
        ticketType.IsVisibleOnPortal = isVisibleOnPortal;
        ticketType.IsActive = isActive;
        ticketType.ExcludeFromCDI = ExcludeFromCDI;
        if (id == null) ticketType.Position = ticketType.Collection.GetMaxPosition(UserSession.LoginUser.OrganizationID) + 1;
        if (id == null) ticketType.OrganizationID = UserSession.LoginUser.OrganizationID;
        if (productFamilyID == "-1")
        {
            ticketType.ProductFamilyID = null;
        }
        else
        {
            ticketType.ProductFamilyID = Convert.ToInt32(productFamilyID);
        }

        ticketType.Collection.Save();
        ticketType.Collection.ValidatePositions(UserSession.LoginUser.OrganizationID);
        if (id == null)
        {
          try
          {
            System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand();
            command.CommandText = "UPDATE Users SET MenuItems = MenuItems + ',mniTicketType_" + ticketType.TicketTypeID.ToString() + "' WHERE UserID IN (SELECT UserID WHERE OrganizationID = @OrganizationID)";
            command.Parameters.AddWithValue("OrganizationID", UserSession.LoginUser.OrganizationID);
            SqlExecutor.ExecuteNonQuery(UserSession.LoginUser, command);

          }
          catch (Exception ex)
          {
            ExceptionLogs.LogException(UserSession.LoginUser, ex, "Ticket Type Creation - menu item");
          }

          TicketStatuses ticketStatuses = new TicketStatuses(UserSession.LoginUser);
          ticketStatus = ticketStatuses.AddNewTicketStatus();
          ticketStatus.Name = "New";
          ticketStatus.Description = "New";
          ticketStatus.Position = 0;
          ticketStatus.OrganizationID = UserSession.LoginUser.OrganizationID;
          ticketStatus.TicketTypeID = ticketType.TicketTypeID;
          ticketStatus.IsClosed = false;
          ticketStatus.IsClosedEmail = false;

          ticketStatus = ticketStatuses.AddNewTicketStatus();
          ticketStatus.Name = "Closed";
          ticketStatus.Description = "Closed";
          ticketStatus.Position = 30;
          ticketStatus.OrganizationID = UserSession.LoginUser.OrganizationID;
          ticketStatus.TicketTypeID = ticketType.TicketTypeID;
          ticketStatus.IsClosed = true;
          ticketStatus.IsClosedEmail = false;
          ticketStatus.Collection.Save();
          ticketStatus.Collection.ValidatePositions(UserSession.LoginUser.OrganizationID);


/*          TicketNextStatuses ticketNextStatuses = new TicketNextStatuses(UserSession.LoginUser);
          ticketNextStatuses.AddNextStatus(ticketStatuses[0], ticketStatuses[1], 0);
          ticketNextStatuses.AddNextStatus(ticketStatuses[1], ticketStatuses[0], 1);
          ticketNextStatuses.Save();*/
        }

        break;
        case SelectedType.ActivityTypes:
            ActivityType activityType = id == null ? (new ActivityTypes(UserSession.LoginUser)).AddNewActivityType() : ActivityTypes.GetActivityType(UserSession.LoginUser, (int)id);
            activityType.Name = name;
            activityType.Description = description;
            if (id == null) activityType.Position = activityType.Collection.GetMaxPosition(UserSession.LoginUser.OrganizationID) + 1;
            if (id == null) activityType.OrganizationID = UserSession.LoginUser.OrganizationID;
            activityType.Collection.Save();
            activityType.Collection.ValidatePositions(UserSession.LoginUser.OrganizationID);
        break;
      default:
        break;
    }


    return GetTypesHtml(type, arg);
  }

  [Serializable]
  public class ImageComboBoxData
  {
    public string Text { get; set; }
    public string Value { get; set; }
    public string ImageUrl { get; set; }
  }

  [Serializable]
  public class TypeObject
  {
    public TypeObject()
    {
      ID = -1;
      Name = "";
      Description = "";
      IsClosed = false;
      IsClosedEmail = false;
      PauseSla = false;
      IsTimed = false;
      IsShipping = false;
      IsDiscontinued = false;
      ProductFamilyID = -1;
    }
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsClosed { get; set; }
    public bool IsClosedEmail { get; set; }
    public bool IsEmailResponse { get; set; }
    public bool PauseSla { get; set; }
    public bool IsTimed { get; set; }
    public bool IsShipping { get; set; }
    public bool IsDiscontinued { get; set; }
    public string IconUrl { get; set; }
    public bool IsVisibleOnPortal { get; set; }
    public int ProductFamilyID { get; set; }
    public bool IsActive { get; set; }
    public bool ExcludeFromCDI { get; set; }
  }

    [Serializable]
    public class TypesHtmlResult
    {
        public string Html { get; set; }
        public bool UseProductFamilies { get; set; }
    }
}
