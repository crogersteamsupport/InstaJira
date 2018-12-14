USE [TeamSupport]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[OrganizationsViewGet]
(
	@OrganizationID int
)
AS
BEGIN
	SET NOCOUNT OFF; 
	SELECT
		[OrganizationID],
		[Name],
		[Description],
		[Website],
		[IsActive],
		[InActiveReason],
		[PrimaryUserID],
		[PrimaryContactEmail],
		[PrimaryContact],
		[ParentID],
		[DateCreated],
		[DateModified],
		[CreatorID],
		[ModifierID],
		[HasPortalAccess],
		[CreatedBy],
		[LastModifiedBy],
		[SAExpirationDate],
		[SlaName],
		[CRMLinkID],
		[PortalGuid],
		[SlaLevelID],
		[DefaultWikiArticleID],
		[DefaultSupportGroupID],
		[DefaultSupportUserID],
		[DefaultSupportUser],
		[DefaultSupportGroup],
		[CompanyDomains],
		[SupportHoursMonth],
		[SupportHoursUsed],
		[SupportHoursRemaining],
		[NeedsIndexing],
		[CustDisIndex],
		[CustDistIndexTrend]
	FROM
		[dbo].[OrganizationsView]
	WHERE
		([OrganizationID] = @OrganizationID);
END
GO