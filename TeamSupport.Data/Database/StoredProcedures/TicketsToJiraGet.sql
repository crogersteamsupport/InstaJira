USE [TeamSupport]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[TicketsToJiraGet]
(
	@organizationId int,
	@dateModified datetime,
	@crmLinkId int,
	@isTicketLinkToJira bit = 0
)
AS
BEGIN
	DECLARE @sql nvarchar(1000)
	DECLARE @sqlFilter varchar(1000)
	DECLARE @parameters nvarchar(500)

	SET @sql = 'SELECT 
			j.*
		FROM
			TicketLinkToJira j
			JOIN Tickets t
				ON j.TicketID = t.TicketID'

	IF (@isTicketLinkToJira = 0)
		SET @sql = 'SELECT
					t.*
				FROM
					TicketsView t
					JOIN TicketLinkToJira j
						ON t.TicketID = j.TicketID'

	SET @sqlFilter = '
		WHERE
			j.SyncWithJira = 1
			AND t.OrganizationID = @organizationId
			AND j.CrmLinkID = @crmLinkId
			AND
			(
			  j.DateModifiedByJiraSync IS NULL
			  OR
			  (
				t.DateModified > DATEADD(s, 2, j.DateModifiedByJiraSync)
				AND t.DateModified > @dateModified
			  )
			)
		ORDER BY
			t.DateCreated DESC'

	SET @parameters = N'@organizationId int, @crmLinkId int, @dateModified datetime'

	SET @sql = 'SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
		SET NOCOUNT OFF; ' + @sql + @sqlFilter

	EXECUTE sp_executesql @sql,
						@parameters,
						@organizationId = @organizationId,
						@crmLinkId = @crmLinkId,
						@dateModified = @dateModified
END
GO