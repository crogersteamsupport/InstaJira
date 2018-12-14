using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using TeamSupport.Data;
using System.Net;

namespace TeamSupport.Api
{
  public class RestProperties
  {

    public static string GetActionTypes(RestCommand command, bool orderByDateCreated = false)
    {
      ActionTypes items = new ActionTypes(command.LoginUser);
      if (orderByDateCreated)
      {
        items.LoadByOrganizationID(command.Organization.OrganizationID, "DateCreated DESC");
      }
      else
      {
        items.LoadByOrganizationID(command.Organization.OrganizationID);
      }
      return items.GetXml("ActionTypes", "ActionType", true, command.Filters);
    }

    public static string GetPhoneTypes(RestCommand command, bool orderByDateCreated = false)
    {
      PhoneTypes items = new PhoneTypes(command.LoginUser);
      if (orderByDateCreated)
      {
        items.LoadByOrganizationID(command.Organization.OrganizationID, "DateCreated DESC");
      }
      else
      {
        items.LoadByOrganizationID(command.Organization.OrganizationID);
      }
      return items.GetXml("PhoneTypes", "PhoneType", true, command.Filters);
    }

    public static string GetTicketStatuses(RestCommand command, bool orderByDateCreated = false)
    {
      TicketStatuses items = new TicketStatuses(command.LoginUser);
      if (orderByDateCreated)
      {
        items.LoadByOrganizationID(command.Organization.OrganizationID, "DateCreated DESC");
      }
      else
      {
        items.LoadByOrganizationID(command.Organization.OrganizationID);
      }
      return items.GetXml("TicketStatuses", "TicketStatus", true, command.Filters);
    }
    public static string GetTicketSeverities(RestCommand command, bool orderByDateCreated = false)
    {
      TicketSeverities items = new TicketSeverities(command.LoginUser);
      if (orderByDateCreated)
      {
        items.LoadByOrganizationID(command.Organization.OrganizationID, "DateCreated DESC");
      }
      else
      {
        items.LoadByOrganizationID(command.Organization.OrganizationID);
      }
      return items.GetXml("TicketSeverities", "TicketSeverity", true, command.Filters);
    }

    public static string GetTicketTypes(RestCommand command, bool orderByDateCreated = false)
    {
      TicketTypes items = new TicketTypes(command.LoginUser);
      if (orderByDateCreated)
      {
        items.LoadByOrganizationID(command.Organization.OrganizationID, "DateCreated DESC");
      }
      else
      {
        items.LoadByOrganizationID(command.Organization.OrganizationID);
      }
      return items.GetXml("TicketTypes", "TicketType", true, command.Filters);
    }

    public static string GetProductVersionStatuses(RestCommand command, bool orderByDateCreated = false)
    {
      ProductVersionStatuses items = new ProductVersionStatuses(command.LoginUser);
      if (orderByDateCreated)
      {
        items.LoadByOrganizationID(command.Organization.OrganizationID, "DateCreated DESC");
      }
      else
      {
        items.LoadByOrganizationID(command.Organization.OrganizationID);
      }
      return items.GetXml("ProductVersionStatuses", "ProductVersionStatus", true, command.Filters);
    }

    public static string GetKnowledgeBaseCategories(RestCommand command, bool orderByDateCreated = false)
    {
      KnowledgeBaseCategories categories = new KnowledgeBaseCategories(command.LoginUser);
      if (orderByDateCreated)
      {
        categories.LoadAllCategories(command.Organization.OrganizationID, "DateCreated DESC");
      }
      else
      {
        categories.LoadAllCategories(command.Organization.OrganizationID);
      }
      return categories.GetXml("KnowledgeBaseCategories", "KnowledgeBaseCategory", false, command.Filters);
    }
  }
}
