USE WorkFlowTrackerDB
GO

CREATE OR ALTER PROCEDURE [WorkFlow].[spClient_Upsert]
-- EXEC [WorkFlow].spClient_Upsert
    @ClientName NVARCHAR(50),
    @ClientId   INT = NULL
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM [WorkFlow].[Client] WHERE ClientId = @ClientId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM [WorkFlow].[Client] WHERE [ClientName] = @ClientName)
        BEGIN
            INSERT INTO [WorkFlow].[Client] (
                [ClientName]
            )VALUES(
                @ClientName
            )
        END
        ELSE
        BEGIN
            THROW 50001, 'ClientName already exists.', 1;
        END
    END
    ELSE
    BEGIN
        UPDATE [WorkFlow].[Client] SET
            [ClientName] = @ClientName
        WHERE ClientId = @ClientId
    END
END
GO

--------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE [WorkFlow].[spClient_Get]
-- EXEC [WorkFlow].[spClient_Get]
    @ClientId INT = NULL
AS
BEGIN
    SELECT 
    [ClientId],
    [ClientName]
    FROM [WorkFlow].[Client]
    WHERE [Client].ClientId = ISNULL(@ClientId, [Client].ClientId)
END
GO

--------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE [WorkFlow].[spClient_Delete]
-- EXEC [WorkFlow].spClient_Delete
    @ClientId INT
AS
BEGIN
    DELETE FROM [WorkFlow].[Client]
    WHERE ClientId = @ClientId
END
GO