using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Text;
using System.Globalization;

public partial class Tips_Sla : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
      if (Request["TicketID"] == null) EndResponse("Invalid Ticket");
      LoginUser loginUser = TSAuthentication.GetLoginUser();
      int ticketID = int.Parse(Request["TicketID"]);
      Ticket ticket = Tickets.GetTicket(loginUser, ticketID);
      if (ticket == null) EndResponse("Invalid Ticket");

        tipNumber.InnerText = "Ticket #" + ticket.TicketNumber.ToString();
        tipNumber.Attributes.Add("onclick", "top.Ts.MainPage.openTicket(" + ticket.TicketNumber + "); return false;");
        tipName.InnerHtml = ticket.Name;

        bool isStatusPaused = false;
        SlaTicket slaTicket = SlaTickets.GetSlaTicket(loginUser, ticket.TicketID);

        if (slaTicket != null)
        {
            SlaTrigger slaTrigger = SlaTriggers.GetSlaTrigger(loginUser, slaTicket.SlaTriggerId);

            if (slaTrigger != null)
            {
                isStatusPaused = ticket.IsSlaStatusPaused();
                string slaName = new SlaLevels(TSAuthentication.GetLoginUser()).GetSlaveLevelName(slaTicket.SlaTriggerId);
                wslaName.InnerText = slaName;
            }
            else
            {
                ticket.SlaViolationInitialResponse = null;
                ticket.SlaViolationTimeClosed = null;
                ticket.SlaViolationLastAction = null;
                ticket.SlaWarningInitialResponse = null;
                ticket.SlaWarningTimeClosed = null;
                ticket.SlaWarningLastAction = null;
                ticket.Collection.Save();
            }
        }

        if (isStatusPaused)
        {
            wClose.InnerText = "Paused";
            vClose.InnerText = "Paused";
            wLast.InnerText = "Paused";
            vLast.InnerText = "Paused";
            wInit.InnerText = "Paused";
            vInit.InnerText = "Paused";
            wNext.InnerText = "Paused";
            vNext.InnerText = "Paused";
        }
        else
        {
            DateTime? nextViolation = GetUtcDate(ticket, "SlaViolationInitialResponse");
            nextViolation = GetMinDate(loginUser, nextViolation, GetUtcDate(ticket, "SlaViolationLastAction"));
            nextViolation = GetMinDate(loginUser, nextViolation, GetUtcDate(ticket, "SlaViolationTimeClosed"));

            DateTime? nextWarning = GetUtcDate(ticket, "SlaWarningInitialResponse");
            nextWarning = GetMinDate(loginUser, nextWarning, GetUtcDate(ticket, "SlaWarningLastAction"));
            nextWarning = GetMinDate(loginUser, nextWarning, GetUtcDate(ticket, "SlaWarningTimeClosed"));

            if (nextViolation != null && nextViolation < DateTime.UtcNow) tipSla.Attributes.Add("class", "ts-icon ts-icon-sla-bad");
            else if (nextWarning != null && nextWarning < DateTime.UtcNow) tipSla.Attributes.Add("class", "ts-icon ts-icon-sla-warning");
            else tipSla.Attributes.Add("class", "ts-icon ts-icon-sla-good");

            wClose.InnerText = GetDateString(loginUser, GetUtcDate(ticket, "SlaWarningTimeClosed"));
            vClose.InnerText = GetDateString(loginUser, GetUtcDate(ticket, "SlaViolationTimeClosed"));
            wLast.InnerText = GetDateString(loginUser, GetUtcDate(ticket, "SlaWarningLastAction"));
            vLast.InnerText = GetDateString(loginUser, GetUtcDate(ticket, "SlaViolationLastAction"));
            wInit.InnerText = GetDateString(loginUser, GetUtcDate(ticket, "SlaWarningInitialResponse"));
            vInit.InnerText = GetDateString(loginUser, GetUtcDate(ticket, "SlaViolationInitialResponse"));
            wNext.InnerText = GetDateString(loginUser, nextWarning);
            vNext.InnerText = GetDateString(loginUser, nextViolation);
        }
    }

    private DateTime? GetMinDate(LoginUser loginUser, DateTime? d1, DateTime? d2)
    {
      DateTime? result = null;
      if (d1 == null) result = d2;
      else if (d2 == null) result = d1;
      else if (d1 < d2) result = d1; 
      else result = d2;
      return result;
    }

    private DateTime? GetUtcDate(Ticket ticket, string column)
    {
      object o = ticket.Row[column];
      if (o == null || o == DBNull.Value) return null;
      return (DateTime)o;
    }

    private string GetDateString(LoginUser loginUser, DateTime? date)
    {
      if (date == null) return "None";
      CultureInfo us = new CultureInfo(loginUser.CultureInfo.ToString());

      return ((DateTime)DataUtils.DateToLocal(loginUser, date)).ToString("g", us.DateTimeFormat);
    }

   

    private void EndResponse(string message)
    {
      Response.Write(message);
      Response.End();
    }
}