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
  
  public class RestReportTicketsView
  {
    public static string GetReportTicketsViewItem(RestCommand command, int ticketID)
    {
      ReportTicketsViewItem reportTicketsViewItem = ReportTicketsView.GetReportTicketsViewItem(command.LoginUser, ticketID);
      if (reportTicketsViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return reportTicketsViewItem.GetXml("ReportTicketsViewItem", true);
    }
    
    public static string GetReportTicketsView(RestCommand command)
    {
      ReportTicketsView reportTicketsView = new ReportTicketsView(command.LoginUser);
      reportTicketsView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return reportTicketsView.GetXml("ReportTicketsView", "ReportTicketsViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }

    public static string GetTickets(RestCommand command)
    {
      string xml = "";

      if (command.Filters["TicketTypeID"] != null)
      {
        try
        {
          ReportTicketsView tickets = new ReportTicketsView(command.LoginUser);
          int ticketTypeID = int.Parse(command.Filters["TicketTypeID"]);
          TicketType ticketType = TicketTypes.GetTicketType(command.LoginUser, ticketTypeID);
          if (ticketType.OrganizationID != command.Organization.OrganizationID) throw new Exception();
          tickets.LoadByTicketTypeID(ticketTypeID);
          xml = tickets.GetXml("Tickets", "Ticket", true, command.Filters);
        }
        catch (Exception ex)
        {
          throw new RestException(HttpStatusCode.NotAcceptable, "Invalid TicketTypeID to filter.", ex);
        }
      }
      else
      {
        TicketTypes ticketTypes = new TicketTypes(command.LoginUser);
        ticketTypes.LoadByOrganizationID(command.Organization.OrganizationID);

        ReportTicketsView tickets = new ReportTicketsView(command.LoginUser);
        XmlTextWriter writer = Tickets.BeginXmlWrite("Tickets");

        foreach (TicketType ticketType in ticketTypes)
        {
          tickets.LoadByTicketTypeID(ticketType.TicketTypeID);
          //writer.WriteStartElement(ticketType.Name);
          foreach (DataRow row in tickets.Table.Rows)
          {
            tickets.WriteXml(writer, row, "Ticket", true, command.Filters);
          }
          //writer.WriteEndElement();
        }

        xml = Tickets.EndXmlWrite(writer);
      }

      //return tickets.GetXml("Tickets", "Ticket", command.Filters["TicketTypeID"] != null, command.Filters);
      return xml;
    }

    public static string GetTicket(RestCommand command, int ticketID)
    {
      ReportTicketsViewItem ticket = ReportTicketsView.GetReportTicketsViewItemByIDOrNumber(command.LoginUser, ticketID);
      if (ticket.OrganizationID != command.Organization.OrganizationID)
      {
        throw new RestException(HttpStatusCode.Unauthorized);
      }

      return ticket.GetXml("Ticket", true);
    }


  }
  
}





  
