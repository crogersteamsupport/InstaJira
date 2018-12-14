IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomFieldCategory

(
  @CustomFieldCategoryID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CustomFieldCategoryID],
    [OrganizationID],
    [Category],
    [Position],
    [RefType],
    [AuxID],
    [ProductFamilyID]
  FROM [dbo].[CustomFieldCategories]
  WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomFieldCategory

(
  @OrganizationID int,
  @Category varchar(250),
  @Position int,
  @RefType int,
  @AuxID int,
  @ProductFamilyID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomFieldCategories]
  (
    [OrganizationID],
    [Category],
    [Position],
    [RefType],
    [AuxID],
    [ProductFamilyID])
  VALUES (
    @OrganizationID,
    @Category,
    @Position,
    @RefType,
    @AuxID,
    @ProductFamilyID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomFieldCategory

(
  @CustomFieldCategoryID int,
  @OrganizationID int,
  @Category varchar(250),
  @Position int,
  @RefType int,
  @AuxID int,
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomFieldCategories]
  SET
    [OrganizationID] = @OrganizationID,
    [Category] = @Category,
    [Position] = @Position,
    [RefType] = @RefType,
    [AuxID] = @AuxID,
    [ProductFamilyID] = @ProductFamilyID
  WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomFieldCategory

(
  @CustomFieldCategoryID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomFieldCategories]
  WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomFieldCategory

(
  @CustomFieldCategoryID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CustomFieldCategoryID],
    [OrganizationID],
    [Category],
    [Position],
    [RefType],
    [AuxID],
    [ProductFamilyID]
  FROM [dbo].[CustomFieldCategories]
  WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomFieldCategory

(
  @OrganizationID int,
  @Category varchar(250),
  @Position int,
  @RefType int,
  @AuxID int,
  @ProductFamilyID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomFieldCategories]
  (
    [OrganizationID],
    [Category],
    [Position],
    [RefType],
    [AuxID],
    [ProductFamilyID])
  VALUES (
    @OrganizationID,
    @Category,
    @Position,
    @RefType,
    @AuxID,
    @ProductFamilyID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomFieldCategory

(
  @CustomFieldCategoryID int,
  @OrganizationID int,
  @Category varchar(250),
  @Position int,
  @RefType int,
  @AuxID int,
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomFieldCategories]
  SET
    [OrganizationID] = @OrganizationID,
    [Category] = @Category,
    [Position] = @Position,
    [RefType] = @RefType,
    [AuxID] = @AuxID,
    [ProductFamilyID] = @ProductFamilyID
  WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomFieldCategory

(
  @CustomFieldCategoryID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomFieldCategories]
  WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomFieldCategory

(
  @CustomFieldCategoryID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CustomFieldCategoryID],
    [OrganizationID],
    [Category],
    [Position],
    [RefType],
    [AuxID],
    [ProductFamilyID]
  FROM [dbo].[CustomFieldCategories]
  WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomFieldCategory

(
  @OrganizationID int,
  @Category varchar(250),
  @Position int,
  @RefType int,
  @AuxID int,
  @ProductFamilyID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomFieldCategories]
  (
    [OrganizationID],
    [Category],
    [Position],
    [RefType],
    [AuxID],
    [ProductFamilyID])
  VALUES (
    @OrganizationID,
    @Category,
    @Position,
    @RefType,
    @AuxID,
    @ProductFamilyID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomFieldCategory

(
  @CustomFieldCategoryID int,
  @OrganizationID int,
  @Category varchar(250),
  @Position int,
  @RefType int,
  @AuxID int,
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomFieldCategories]
  SET
    [OrganizationID] = @OrganizationID,
    [Category] = @Category,
    [Position] = @Position,
    [RefType] = @RefType,
    [AuxID] = @AuxID,
    [ProductFamilyID] = @ProductFamilyID
  WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomFieldCategory

(
  @CustomFieldCategoryID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomFieldCategories]
  WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomFieldCategory

(
  @CustomFieldCategoryID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CustomFieldCategoryID],
    [OrganizationID],
    [Category],
    [Position],
    [RefType],
    [AuxID],
    [ProductFamilyID]
  FROM [dbo].[CustomFieldCategories]
  WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomFieldCategory

(
  @OrganizationID int,
  @Category varchar(250),
  @Position int,
  @RefType int,
  @AuxID int,
  @ProductFamilyID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomFieldCategories]
  (
    [OrganizationID],
    [Category],
    [Position],
    [RefType],
    [AuxID],
    [ProductFamilyID])
  VALUES (
    @OrganizationID,
    @Category,
    @Position,
    @RefType,
    @AuxID,
    @ProductFamilyID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomFieldCategory

(
  @CustomFieldCategoryID int,
  @OrganizationID int,
  @Category varchar(250),
  @Position int,
  @RefType int,
  @AuxID int,
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomFieldCategories]
  SET
    [OrganizationID] = @OrganizationID,
    [Category] = @Category,
    [Position] = @Position,
    [RefType] = @RefType,
    [AuxID] = @AuxID,
    [ProductFamilyID] = @ProductFamilyID
  WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomFieldCategory

(
  @CustomFieldCategoryID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomFieldCategories]
  WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID)
GO


IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedSelectCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedSelectCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedSelectCustomFieldCategory

(
  @CustomFieldCategoryID int
)
AS
  SET NOCOUNT OFF;
  SELECT
    [CustomFieldCategoryID],
    [OrganizationID],
    [Category],
    [Position],
    [RefType],
    [AuxID],
    [ProductFamilyID]
  FROM [dbo].[CustomFieldCategories]
  WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedInsertCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedInsertCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedInsertCustomFieldCategory

(
  @OrganizationID int,
  @Category varchar(250),
  @Position int,
  @RefType int,
  @AuxID int,
  @ProductFamilyID int,
  @Identity int OUT
)
AS
  SET NOCOUNT OFF;
  INSERT INTO [dbo].[CustomFieldCategories]
  (
    [OrganizationID],
    [Category],
    [Position],
    [RefType],
    [AuxID],
    [ProductFamilyID])
  VALUES (
    @OrganizationID,
    @Category,
    @Position,
    @RefType,
    @AuxID,
    @ProductFamilyID)

SET @Identity = SCOPE_IDENTITY()
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedUpdateCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedUpdateCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedUpdateCustomFieldCategory

(
  @CustomFieldCategoryID int,
  @OrganizationID int,
  @Category varchar(250),
  @Position int,
  @RefType int,
  @AuxID int,
  @ProductFamilyID int
)
AS
  SET NOCOUNT OFF;
  UPDATE [dbo].[CustomFieldCategories]
  SET
    [OrganizationID] = @OrganizationID,
    [Category] = @Category,
    [Position] = @Position,
    [RefType] = @RefType,
    [AuxID] = @AuxID,
    [ProductFamilyID] = @ProductFamilyID
  WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID)
GO

IF EXISTS (SELECT * FROM sysobjects WHERE name = 'uspGeneratedDeleteCustomFieldCategory' AND user_name(uid) = 'dbo')	DROP PROCEDURE dbo.uspGeneratedDeleteCustomFieldCategory
GO

CREATE PROCEDURE dbo.uspGeneratedDeleteCustomFieldCategory

(
  @CustomFieldCategoryID int
)
AS
  SET NOCOUNT OFF;
  DELETE FROM [dbo].[CustomFieldCategories]
  WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID)
GO


