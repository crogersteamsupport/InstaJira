using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.WebUtils;
using TeamSupport.Data;
using System.Web.Services;
using Telerik.Web.UI;

public partial class Frames_AdminTicketTemplates : System.Web.UI.Page
{
  [Serializable]
  public class ComboBoxItem
  {
    public ComboBoxItem(string label, int value)
    {
      Label = label;
      Value = value;
    }

    public string Label { get; set; }
    public int Value { get; set; }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
  }

  [WebMethod]
  public static OrganizationProxy GetOrganization()
  {
    Organization organization = Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
    OrganizationProxy result = organization.GetProxy();
    return result;
  }

  [WebMethod]
  public static ComboBoxItem[] GetTicketTypes(int ticketTemplateID)
  {
    TicketTypes ticketTypes = new TicketTypes(UserSession.LoginUser);
    ticketTypes.LoadByTicketTemplate(UserSession.LoginUser.OrganizationID, ticketTemplateID);
    List<ComboBoxItem> result = new List<ComboBoxItem>();
    foreach (TicketType ticketType in ticketTypes)
    {
      result.Add(new ComboBoxItem(ticketType.Name, ticketType.TicketTypeID));
    }
    return result.ToArray();
  
  }

  [WebMethod]
  public static ComboBoxItem[] GetActionTypes(int ticketTemplateID)
  {
      ActionTypes actionTypes = new ActionTypes(UserSession.LoginUser);
      actionTypes.LoadByOrganizationID(UserSession.LoginUser.OrganizationID);

      List<ComboBoxItem> result = new List<ComboBoxItem>();
      foreach (ActionType action in actionTypes)
      {
          result.Add(new ComboBoxItem(action.Name, action.ActionTypeID));
      }
      return result.ToArray();

  }

  [WebMethod]
  public static ComboBoxItem[] GetTicketTemplates()
  {
    TicketTemplates templates = new TicketTemplates(UserSession.LoginUser);
    templates.LoadByOrganization(UserSession.LoginUser.OrganizationID);

    List<ComboBoxItem> result = new List<ComboBoxItem>();
    foreach (TicketTemplate template in templates)
    {
      switch (template.TemplateType)
      {
        case TicketTemplateType.TicketType:
          result.Add(new ComboBoxItem("Ticket Type - " + template.Row["Name"], template.TicketTemplateID));
        break;
        case TicketTemplateType.PickList:
          result.Add(new ComboBoxItem(template.TriggerText, template.TicketTemplateID));
        break;
        case TicketTemplateType.ActionType:
        result.Add(new ComboBoxItem("Action Type", template.TicketTemplateID));
        break;

        default:
          break;
      }
      
    }
    return result.ToArray();
  }

  [WebMethod]
  public static TicketTemplateProxy GetTicketTemplate(int ticketTemplateID) 
  {
    return TicketTemplates.GetTicketTemplate(UserSession.LoginUser, ticketTemplateID).GetProxy();
  }

  [WebMethod]
  public static int SaveTicketTemplate(int ticketTemplateID, int templateType, bool isEnabled, bool isPortal, int ticketTypeID, string value, string text)
  {
    if (!UserSession.CurrentUser.IsSystemAdmin) return -1;
    TicketTemplate template;

    if (ticketTemplateID < 0)
      template = (new TicketTemplates(UserSession.LoginUser)).AddNewTicketTemplate();
    else
      template = TicketTemplates.GetTicketTemplate(UserSession.LoginUser, ticketTemplateID);

    
    template.OrganizationID = UserSession.LoginUser.OrganizationID;
    template.TemplateType = (TicketTemplateType)templateType;
    template.TicketTypeID = ticketTypeID;
    template.TemplateText = text;
    template.TriggerText = value;
    template.IsEnabled = isEnabled;
    template.IsVisibleOnPortal = isPortal;

    template.Collection.Save();
    return template.TicketTemplateID;
  }

  [WebMethod]
  public static void DeleteTicketTemplate(int ticketTemplateID)
  {
    if (!UserSession.CurrentUser.IsSystemAdmin) return;
    TicketTemplate template = TicketTemplates.GetTicketTemplate(UserSession.LoginUser, ticketTemplateID);
    if (UserSession.LoginUser.OrganizationID == template.OrganizationID)
    {
      template.Delete();
      template.Collection.Save();
    }
    

  }
}
