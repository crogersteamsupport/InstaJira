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
  //public enum RestTicketType {All, Bug, Feature, Task, Issue}

  public class RestTicketsViewItem: IRestData
  {
    private int _ticketID;
    private RestCommand _command;

    public RestTicketsViewItem(RestCommand command, string ticketID)
    {
      _ticketID = int.Parse(ticketID);
      _command = command;
    }

    public static void WriteTicketsViewItemXml(RestCommand command, XmlWriter writer, TicketsViewItem ticket, CustomFields customFields)
    {
      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) 
        throw new RestException(HttpStatusCode.BadRequest, "Invalid TicketID");
      
      writer.WriteElementString("TicketID", ticket.TicketID.ToString());
      writer.WriteElementString("TicketNumber", ticket.TicketNumber.ToString());
      writer.WriteElementString("TicketType", ticket.TicketTypeName.ToString());
      writer.WriteElementString("Name", ticket.Name.ToString());
      writer.WriteElementString("IsClosed", ticket.IsClosed.ToString());

      if (customFields != null)
      {
        foreach (CustomField field in customFields)
        {
          CustomValue value = CustomValues.GetValue(command.LoginUser, field.CustomFieldID, ticket.TicketID);
          writer.WriteElementString(field.ApiFieldName, value.Value);
        }
      }
    }

    public static void WriteTicketsViewItemXml(RestCommand command, XmlWriter writer, int ticketID, CustomFields customFields)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(command.LoginUser, ticketID);
      WriteTicketsViewItemXml(command, writer, ticket, customFields);
    }

    public static void WriteTicketsViewItemXml(RestCommand command, XmlWriter writer, string ticketID, CustomFields customFields)
    {
      int id = int.Parse(ticketID);
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(command.LoginUser, id);
      WriteTicketsViewItemXml(command, writer, ticket, customFields);
    }

    #region IRestData Members

    public string GetData()
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItem(_command.LoginUser, _ticketID);
      CustomFields customFields = new CustomFields(_command.LoginUser);
      customFields.LoadByTicketTypeID(_command.Organization.OrganizationID, ticket.TicketTypeID);

      RestXmlWriter writer = new RestXmlWriter("Ticket");
      WriteTicketsViewItemXml(_command, writer.XmlWriter, ticket, customFields);
      return writer.GetXml();
    }

    #endregion
  }

  public class RestTicketsView : IRestData
  {
    private RestTicketType _restTicketType;
    private RestCommand _command;

    public RestTicketsView(RestCommand command, RestTicketType restTicketType)
    {
      _restTicketType = restTicketType;
      _command = command;
    }

    #region IRestData Members

    public string GetData()
    {
      TicketsView tickets = new TicketsView(_command.LoginUser);
      TicketTypes ticketTypes = new TicketTypes(_command.LoginUser);
      ticketTypes.LoadAllPositions(_command.Organization.OrganizationID);
      TicketType ticketType = null;
      CustomFields customFields;
      string elementName = "Ticket";
      switch (_restTicketType)
      {
        case RestTicketType.Bug:
        elementName = "Bug";
        ticketType = ticketTypes.FindByName("Bugs");
        break;
        case RestTicketType.Feature:
        elementName = "Feature";
        ticketType = ticketTypes.FindByName("Features");
        break;
        case RestTicketType.Task:
        elementName = "Task";
        ticketType = ticketTypes.FindByName("Tasks");
        break;
        case RestTicketType.Issue:
        elementName = "Issue";
        ticketType = ticketTypes.FindByName("Issues");
        break;
        default:
          break;
      }

      if (ticketType == null)
      {
        tickets.LoadByOrganizationID(_command.Organization.OrganizationID);
        customFields = null;
      }
      else
      {
        tickets.LoadByTicketTypeID(ticketType.TicketTypeID);
        customFields = new CustomFields(_command.LoginUser);
        customFields.LoadByTicketTypeID(_command.Organization.OrganizationID, ticketType.TicketTypeID);
      }
      
      RestXmlWriter writer = new RestXmlWriter(elementName + "s");

      foreach (TicketsViewItem ticket in tickets)
      {
        writer.XmlWriter.WriteStartElement(elementName);
        RestTicketsViewItem.WriteTicketsViewItemXml(_command, writer.XmlWriter, ticket, customFields);
        writer.XmlWriter.WriteEndElement();
      }
      return writer.GetXml();
    }

    #endregion
  }
}
