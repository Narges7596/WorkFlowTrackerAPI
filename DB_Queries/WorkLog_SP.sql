USE WorkFlowTrackerDB
GO

CREATE OR ALTER PROCEDURE [WorkFlow].[spWorkLog_Upsert]
-- EXEC [WorkFlow].spWorkLog_Upsert
    @WorkLogId     INT = NULL,
    @UserId        INT,
    @StartDateTime DATETIME,
    @EndDateTime   DATETIME,
    @ProjectId     INT,
    @Description NVARCHAR(200)
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM [WorkFlow].[WorkLog] WHERE WorkLogId = @WorkLogId)
    BEGIN
        INSERT INTO [WorkFlow].[WorkLog](
            [UserId],
            [StartDateTime],
            [EndDateTime],
            [ProjectId],
            [Description]
        )VALUES(
            @UserId,
            @StartDateTime,
            @EndDateTime,
            @ProjectId,
            @Description
        )
    END
    ELSE
    BEGIN
        UPDATE [WorkFlow].[WorkLog]
        SET
            [StartDateTime] = @StartDateTime,
            [EndDateTime] = @EndDateTime,
            [ProjectId] = @ProjectId,
            [Description] = @Description
        WHERE WorkLogId = @WorkLogId AND UserId = @UserId
    END
END
GO

-----------------------------------------------------------------

CREATE OR ALTER PROCEDURE [WorkFlow].[spWorkLog_Delete]
-- EXEC [WorkFlow].spWorkLog_Delete
    @WorkLogId INT,
    @UserId INT
AS
BEGIN
    DELETE FROM [WorkFlow].[WorkLog]
    WHERE WorkLogId = @WorkLogId AND UserId = @UserId
END
GO

-----------------------------------------------------------------

CREATE OR ALTER PROCEDURE [WorkFlow].[spWorkLog_Get]
-- EXEC WorkFlow.spWorkLog_Get
    @UserId        INT  = NULL,
    @StartDateTime DATETIME = NULL,
    @EndDateTime   DATETIME = NULL,
    @ProjectId     INT  = NULL,
    @DescSearch NVARCHAR(50)
AS
BEGIN
    SELECT
        [WorkLog].[WorkLogId],
        [WorkLog].[UserId],
        [WorkLog].[StartDateTime],
        [WorkLog].[EndDateTime],
        [WorkLog].[Description],
        [ProjectClient].[ClientName],
        [ProjectClient].[ProjectName]
    FROM [WorkFlow].[WorkLog] As [WorkLog]
    CROSS APPLY (
        SELECT
            [Client].[ClientName],
            [Project].[ProjectName]
        FROM [WorkFlow].[Project] AS [Project]
            JOIN [WorkFlow].[Client] AS [Client]
            ON [Project].[ClientId] = [Client].[ClientId]
        WHERE [Project].[ProjectId] = [WorkLog].[ProjectId]
        AND [Project].[ClientId] = [Client].[ClientId] 
    ) AS [ProjectClient]
    
    WHERE [WorkLog].[UserId] = ISNULL(@UserId, [WorkLog].[UserId])
    AND [WorkLog].[ProjectId] = ISNULL(@ProjectId, [WorkLog].[ProjectId])
    AND [WorkLog].[StartDateTime] >= ISNULL(@StartDateTime, [WorkLog].[StartDateTime])
    AND [WorkLog].[EndDateTime] <= ISNULL(@EndDateTime, [WorkLog].[EndDateTime])
    AND (@DescSearch IS NULL OR [WorkLog].[Description] LIKE '%' + @DescSearch + '%')
END
GO

-----------------------------------------------------------------

CREATE OR ALTER PROCEDURE [WorkFlow].[spWorkLogTags_Get]
-- EXEC WorkFlow.spWorkLogTags_Get
    @UserId      INT = NULL,
    @WorkLogId   INT = NULL
AS
BEGIN
    SELECT
        [Tag].[TagId],
        [Tag].[TagName]
    FROM [WorkFlow].[WorkLogTag] As [WorkLogTag]
    JOIN [WorkFlow].[WorkLog] AS [WorkLog]
        ON [WorkLogTag].[WorkLogId] = [WorkLog].[WorkLogId]
    JOIN [WorkFlow].[Tag] AS [Tag]
        ON [WorkLogTag].[TagId] = [Tag].[TagId]
    
    WHERE [WorkLog].[UserId] = ISNULL(@UserId, [WorkLog].[UserId])
    AND [WorkLogTag].[WorkLogId] = ISNULL(@WorkLogId, [WorkLogTag].[WorkLogId])
END
GO