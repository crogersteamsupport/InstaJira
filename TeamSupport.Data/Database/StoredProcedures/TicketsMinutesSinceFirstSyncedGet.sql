SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
IF OBJECT_ID('dbo.TicketsMinutesSinceFirstSyncedGet','P') IS NOT NULL
	DROP PROC dbo.TicketsMinutesSinceFirstSyncedGet;
GO
 
CREATE PROC [dbo].[TicketsMinutesSinceFirstSyncedGet]
	@OrganizationID int
AS
BEGIN
	DECLARE @TopID int;
	 
	SELECT TOP 1 @TopID = t.TicketID 
	FROM 
		   dbo.Tickets t
		   INNER JOIN dbo.TicketLinkToJira j ON t.TicketID = j.TicketID
	WHERE 
		   t.OrganizationID = @OrganizationID
	ORDER BY t.DateCreated;
	 
	SELECT * FROM dbo.Tickets WHERE TicketID = @TopID;
END