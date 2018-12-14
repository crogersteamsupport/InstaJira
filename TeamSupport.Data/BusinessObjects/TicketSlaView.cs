using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TicketSlaViewItem
  {
  }
  
  public partial class TicketSlaView
  {

    public void LoadAllUnnotifiedAndExpired()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"
SELECT t.*,sn.*
FROM Tickets t
LEFT JOIN SlaNotifications sn ON t.TicketID = sn.TicketID

WHERE 
(DATEDIFF(MINUTE, GETUTCDATE(), ISNULL(t.SlaViolationTimeClosed, '1/1/1980')) BETWEEN -1440 AND -1 AND
 DATEDIFF(minute,  t.SlaViolationTimeClosed, ISNULL(sn.TimeClosedViolationDate, 999999)) > 10)
OR
(DATEDIFF(MINUTE, GETUTCDATE(), ISNULL(t.SlaWarningTimeClosed, '1/1/1980')) BETWEEN -1440 AND -1 AND
 DATEDIFF(minute, t.SlaWarningTimeClosed, ISNULL(sn.TimeClosedWarningDate, 999999)) > 10)
OR 
(DATEDIFF(MINUTE, GETUTCDATE(), ISNULL(t.SlaViolationLastAction, '1/1/1980')) BETWEEN -1440 AND -1 AND
 DATEDIFF(minute, t.SlaViolationLastAction, ISNULL(sn.LastActionViolationDate, 999999)) > 10)
OR
(DATEDIFF(MINUTE, GETUTCDATE(), ISNULL(t.SlaWarningLastAction, '1/1/1980')) BETWEEN -1440 AND -1 AND
 DATEDIFF(minute, t.SlaWarningLastAction, ISNULL(sn.LastActionWarningDate, 999999)) > 10)
OR 
(DATEDIFF(MINUTE, GETUTCDATE(), ISNULL(t.SlaViolationInitialResponse, '1/1/1980')) BETWEEN -1440 AND -1 AND
 DATEDIFF(minute, t.SlaViolationInitialResponse, ISNULL(sn.InitialResponseViolationDate, 999999)) > 10)
OR
(DATEDIFF(MINUTE, GETUTCDATE(), ISNULL(t.SlaWarningInitialResponse, '1/1/1980')) BETWEEN -1440 AND -1 AND
 DATEDIFF(minute, t.SlaWarningInitialResponse, ISNULL(sn.InitialResponseWarningDate, 999999)) > 10)
";

/*
@"
SELECT tsv.*
FROM TicketSlaView tsv
LEFT JOIN SlaNotifications sn ON tsv.TicketID = sn.TicketID

WHERE 
(ISNULL(tsv.ViolationTimeClosed, 10000) BETWEEN -1440 AND -1
 AND
 DATEDIFF(minute, DATEADD(minute, tsv.ViolationTimeClosed, GETUTCDATE()), ISNULL(sn.TimeClosedViolationDate, 999999)) > 10)
OR
(ISNULL(tsv.WarningTimeClosed, 10000) BETWEEN -1440 AND -1
 AND
 DATEDIFF(minute, DATEADD(minute, tsv.WarningTimeClosed, GETUTCDATE()), ISNULL(sn.TimeClosedWarningDate, 999999)) > 10)
OR 
(ISNULL(tsv.ViolationLastAction, 10000) BETWEEN -1440 AND -1
 AND
 DATEDIFF(minute, DATEADD(minute, tsv.ViolationLastAction, GETUTCDATE()), ISNULL(sn.LastActionViolationDate, 999999)) > 10)
OR
(ISNULL(tsv.WarningLastAction, 10000) BETWEEN -1440 AND -1
 AND
 DATEDIFF(minute, DATEADD(minute, tsv.WarningLastAction, GETUTCDATE()), ISNULL(sn.LastActionWarningDate, 999999)) > 10)
OR 
(ISNULL(tsv.ViolationInitialResponse, 10000) BETWEEN -1440 AND -1
 AND
 DATEDIFF(minute, DATEADD(minute, tsv.ViolationInitialResponse, GETUTCDATE()), ISNULL(sn.InitialResponseViolationDate, 999999)) > 10)
OR
(ISNULL(tsv.WarningInitialResponse, 10000) BETWEEN -1440 AND -1
 AND
 DATEDIFF(minute, DATEADD(minute, tsv.WarningInitialResponse, GETUTCDATE()), ISNULL(sn.InitialResponseWarningDate, 999999)) > 10)
";*/


        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }
  }
  
}
