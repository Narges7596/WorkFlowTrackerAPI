USE WorkFlowTrackerDB
GO

CREATE OR ALTER PROCEDURE [WorkFlow].[spTag_Upsert]
-- EXEC [WorkFlow].spTag_Upsert
    @TagName NVARCHAR(50),
    @TagId   INT = NULL
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM [WorkFlow].[Tag] WHERE TagId = @TagId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM [WorkFlow].[Tag] WHERE [TagName] = @TagName)
        BEGIN
            INSERT INTO [WorkFlow].[Tag] (
                [TagName]
            )VALUES(
                @TagName
            )
        END
        ELSE
        BEGIN
            THROW 50001, 'TagName already exists.', 1;
        END
    END
    ELSE
    BEGIN
        UPDATE [WorkFlow].[Tag] SET
            [TagName] = @TagName
        WHERE TagId = @TagId
    END
END
GO

--------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE [WorkFlow].[spTag_Get]
-- EXEC [WorkFlow].[spTag_Get]
    @TagId INT = NULL
AS
BEGIN
    SELECT 
    [TagId],
    [TagName]
    FROM [WorkFlow].[Tag]
    WHERE [Tag].TagId = ISNULL(@TagId, [Tag].TagId)
END
GO

--------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE [WorkFlow].[spTag_Delete]
-- EXEC [WorkFlow].spTag_Delete
    @TagId INT
AS
BEGIN
    DELETE FROM [WorkFlow].[Tag]
    WHERE TagId = @TagId
END
GO