USE WorkFlowTrackerDB
GO

CREATE OR ALTER PROCEDURE [WorkFlow].[spProjectUpsert]
-- EXEC [WorkFlow].spProjectUpsert
    @ProjectName NVARCHAR(50),
    @ClientId    INT,
    @ProjectId   INT = NULL
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM [WorkFlow].[Project] WHERE ProjectId = @ProjectId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM [WorkFlow].[Project] WHERE [ProjectName] = @ProjectName)
        BEGIN
            INSERT INTO [WorkFlow].[Project] (
                [ProjectName],
                [ClientId]
            )VALUES(
                @ProjectName,
                @ClientId
            )
        END
        ELSE
        BEGIN
            THROW 50001, 'ProjectName already exists.', 1;
        END
    END
    ELSE
    BEGIN
        UPDATE [WorkFlow].[Project] SET
            [ProjectName] = @ProjectName,
            [ClientId] = @ClientId
        WHERE ProjectId = @ProjectId
    END
END
GO

--------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE [WorkFlow].[spProject_Get]
-- EXEC [WorkFlow].[spProject_Get]
    @ProjectId INT = NULL,
    @ClientId  INT = NULL
AS
BEGIN
    SELECT 
    [ProjectId],
    [ProjectName],
    [ClientId]
    FROM [WorkFlow].[Project]
    WHERE [Project].ProjectId = ISNULL(@ProjectId, [Project].ProjectId)
    AND [Project].ClientId = ISNULL(@ClientId, [Project].ClientId)
END
GO

--------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE [WorkFlow].[spProject_Delete]
-- EXEC [WorkFlow].spProject_Delete
    @ProjectId INT
AS
BEGIN
    DELETE FROM [WorkFlow].[Project]
    WHERE ProjectId = @ProjectId
END
GO