IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAttachment

(
  @AttachmentID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AttachmentID],
    [OrganizationID],
    [FileName],
    [FileType],
    [FileSize],
    [Path],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [RefType],
    [RefID],
    [SentToJira],
    [AttachmentGUID],
    [ProductFamilyID],
    [SentToTFS],
    [SentToSnow]
  FROM [dbo].[Attachments]
  WHERE ([AttachmentID] = @AttachmentID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAttachment

(
  @OrganizationID int,
  @FileName nvarchar(1000),
  @FileType varchar(255),
  @FileSize bigint,
  @Path nvarchar(1000),
  @Description varchar(2000),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @RefType int,
  @RefID int,
  @SentToJira bit,
  @AttachmentGUID uniqueidentifier,
  @ProductFamilyID int,
  @SentToTFS bit,
  @SentToSnow bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Attachments]
  (
    [OrganizationID],
    [FileName],
    [FileType],
    [FileSize],
    [Path],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [RefType],
    [RefID],
    [SentToJira],
    [AttachmentGUID],
    [ProductFamilyID],
    [SentToTFS],
    [SentToSnow])
  VALUES (
    @OrganizationID,
    @FileName,
    @FileType,
    @FileSize,
    @Path,
    @Description,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @RefType,
    @RefID,
    @SentToJira,
    @AttachmentGUID,
    @ProductFamilyID,
    @SentToTFS,
    @SentToSnow)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAttachment

(
  @AttachmentID int,
  @OrganizationID int,
  @FileName nvarchar(1000),
  @FileType varchar(255),
  @FileSize bigint,
  @Path nvarchar(1000),
  @Description varchar(2000),
  @DateModified datetime,
  @ModifierID int,
  @RefType int,
  @RefID int,
  @SentToJira bit,
  @AttachmentGUID uniqueidentifier,
  @ProductFamilyID int,
  @SentToTFS bit,
  @SentToSnow bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Attachments]
  SET
    [OrganizationID] = @OrganizationID,
    [FileName] = @FileName,
    [FileType] = @FileType,
    [FileSize] = @FileSize,
    [Path] = @Path,
    [Description] = @Description,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [SentToJira] = @SentToJira,
    [AttachmentGUID] = @AttachmentGUID,
    [ProductFamilyID] = @ProductFamilyID,
    [SentToTFS] = @SentToTFS,
    [SentToSnow] = @SentToSnow
  WHERE ([AttachmentID] = @AttachmentID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAttachment

(
  @AttachmentID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Attachments]
  WHERE ([AttachmentID] = @AttachmentID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAttachment

(
  @AttachmentID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AttachmentID],
    [OrganizationID],
    [FileName],
    [FileType],
    [FileSize],
    [Path],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [RefType],
    [RefID],
    [SentToJira],
    [AttachmentGUID],
    [ProductFamilyID],
    [SentToTFS],
    [SentToSnow]
  FROM [dbo].[Attachments]
  WHERE ([AttachmentID] = @AttachmentID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAttachment

(
  @OrganizationID int,
  @FileName nvarchar(1000),
  @FileType varchar(255),
  @FileSize bigint,
  @Path nvarchar(1000),
  @Description varchar(2000),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @RefType int,
  @RefID int,
  @SentToJira bit,
  @AttachmentGUID uniqueidentifier,
  @ProductFamilyID int,
  @SentToTFS bit,
  @SentToSnow bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Attachments]
  (
    [OrganizationID],
    [FileName],
    [FileType],
    [FileSize],
    [Path],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [RefType],
    [RefID],
    [SentToJira],
    [AttachmentGUID],
    [ProductFamilyID],
    [SentToTFS],
    [SentToSnow])
  VALUES (
    @OrganizationID,
    @FileName,
    @FileType,
    @FileSize,
    @Path,
    @Description,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @RefType,
    @RefID,
    @SentToJira,
    @AttachmentGUID,
    @ProductFamilyID,
    @SentToTFS,
    @SentToSnow)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAttachment

(
  @AttachmentID int,
  @OrganizationID int,
  @FileName nvarchar(1000),
  @FileType varchar(255),
  @FileSize bigint,
  @Path nvarchar(1000),
  @Description varchar(2000),
  @DateModified datetime,
  @ModifierID int,
  @RefType int,
  @RefID int,
  @SentToJira bit,
  @AttachmentGUID uniqueidentifier,
  @ProductFamilyID int,
  @SentToTFS bit,
  @SentToSnow bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Attachments]
  SET
    [OrganizationID] = @OrganizationID,
    [FileName] = @FileName,
    [FileType] = @FileType,
    [FileSize] = @FileSize,
    [Path] = @Path,
    [Description] = @Description,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [SentToJira] = @SentToJira,
    [AttachmentGUID] = @AttachmentGUID,
    [ProductFamilyID] = @ProductFamilyID,
    [SentToTFS] = @SentToTFS,
    [SentToSnow] = @SentToSnow
  WHERE ([AttachmentID] = @AttachmentID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAttachment

(
  @AttachmentID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Attachments]
  WHERE ([AttachmentID] = @AttachmentID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedSelectAttachment

(
  @AttachmentID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [AttachmentID],
    [OrganizationID],
    [FileName],
    [FileType],
    [FileSize],
    [Path],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [RefType],
    [RefID],
    [SentToJira],
    [AttachmentGUID],
    [ProductFamilyID],
    [SentToTFS],
    [SentToSnow]
  FROM [dbo].[Attachments]
  WHERE ([AttachmentID] = @AttachmentID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedInsertAttachment

(
  @OrganizationID int,
  @FileName nvarchar(1000),
  @FileType varchar(255),
  @FileSize bigint,
  @Path nvarchar(1000),
  @Description varchar(2000),
  @DateCreated datetime,
  @DateModified datetime,
  @CreatorID int,
  @ModifierID int,
  @RefType int,
  @RefID int,
  @SentToJira bit,
  @AttachmentGUID uniqueidentifier,
  @ProductFamilyID int,
  @SentToTFS bit,
  @SentToSnow bit,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[Attachments]
  (
    [OrganizationID],
    [FileName],
    [FileType],
    [FileSize],
    [Path],
    [Description],
    [DateCreated],
    [DateModified],
    [CreatorID],
    [ModifierID],
    [RefType],
    [RefID],
    [SentToJira],
    [AttachmentGUID],
    [ProductFamilyID],
    [SentToTFS],
    [SentToSnow])
  VALUES (
    @OrganizationID,
    @FileName,
    @FileType,
    @FileSize,
    @Path,
    @Description,
    @DateCreated,
    @DateModified,
    @CreatorID,
    @ModifierID,
    @RefType,
    @RefID,
    @SentToJira,
    @AttachmentGUID,
    @ProductFamilyID,
    @SentToTFS,
    @SentToSnow)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateAttachment

(
  @AttachmentID int,
  @OrganizationID int,
  @FileName nvarchar(1000),
  @FileType varchar(255),
  @FileSize bigint,
  @Path nvarchar(1000),
  @Description varchar(2000),
  @DateModified datetime,
  @ModifierID int,
  @RefType int,
  @RefID int,
  @SentToJira bit,
  @AttachmentGUID uniqueidentifier,
  @ProductFamilyID int,
  @SentToTFS bit,
  @SentToSnow bit
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[Attachments]
  SET
    [OrganizationID] = @OrganizationID,
    [FileName] = @FileName,
    [FileType] = @FileType,
    [FileSize] = @FileSize,
    [Path] = @Path,
    [Description] = @Description,
    [DateModified] = @DateModified,
    [ModifierID] = @ModifierID,
    [RefType] = @RefType,
    [RefID] = @RefID,
    [SentToJira] = @SentToJira,
    [AttachmentGUID] = @AttachmentGUID,
    [ProductFamilyID] = @ProductFamilyID,
    [SentToTFS] = @SentToTFS,
    [SentToSnow] = @SentToSnow
  WHERE ([AttachmentID] = @AttachmentID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteAttachment' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteAttachment
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteAttachment

(
  @AttachmentID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[Attachments]
  WHERE ([AttachmentID] = @AttachmentID)
GO


