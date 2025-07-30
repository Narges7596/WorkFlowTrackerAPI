USE WorkFlowTrackerDB
GO

CREATE OR ALTER PROCEDURE [WorkFlow].[spAbsentDay_Upsert]
-- EXEC [WorkFlow].spAbsentDay_Upsert
    @AbsentDayId INT = NULL,
    @UserId      INT,
    @StartDate   DATETIME,
    @EndDate     DATETIME,
    @Type        VARCHAR(50),
    @Status      VARCHAR(20),
    @Description NVARCHAR(200) = NULL,
    @RequestedAt DATETIME,
    @ApprovedBy  INT = NULL,
    @ApprovedAt  DATETIME = NULL
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM [WorkFlow].[AbsentDay] WHERE AbsentDayId = @AbsentDayId)
    BEGIN
        INSERT INTO [WorkFlow].[AbsentDay](
            [UserId],
            [StartDate],
            [EndDate],
            [Type],
            [Status],
            [Description],
            [RequestedAt],
            [ApprovedBy],
            [ApprovedAt]
        )VALUES(
            @UserId,
            @StartDate,
            @EndDate,
            @Type,
            @Status,
            @Description,
            @RequestedAt,
            @ApprovedBy,
            @ApprovedAt
        )
    END
    ELSE
    BEGIN
        UPDATE [WorkFlow].[AbsentDay]
        SET
            [UserId] = @UserId,
            [StartDate] = @StartDate,
            [EndDate] = @EndDate,
            [Type] = @Type,
            [Status] = @Status,
            [Description] = @Description,
            [RequestedAt] = @RequestedAt,
            [ApprovedBy] = @ApprovedBy,
            [ApprovedAt] = @ApprovedAt
        WHERE AbsentDayId = @AbsentDayId
    END
END
GO

-----------------------------------------------------------------

CREATE OR ALTER PROCEDURE [WorkFlow].[spAbsentDay_Delete]
-- EXEC [WorkFlow].spAbsentDay_Delete
    @AbsentDayId INT
AS
BEGIN
    DELETE FROM [WorkFlow].[AbsentDay]
    WHERE AbsentDayId = @AbsentDayId
END
GO

-----------------------------------------------------------------

CREATE OR ALTER PROCEDURE [WorkFlow].[spAbsentDay_Get]
-- EXEC WorkFlow.spAbsentDay_Get
    @UserId     INT         = NULL,
    @StartDate  DATETIME    = NULL,
    @EndDate    DATETIME    = NULL,
    @Type       VARCHAR(50) = NULL,
    @Status     VARCHAR(20) = NULL
AS
BEGIN
    SELECT
        [AbsentDayId],
        [UserId],
        [StartDate],
        [EndDate],
        [Type],
        [Status],
        [Description],
        [RequestedAt],
        [ApprovedBy],
        [ApprovedAt]
    FROM [WorkFlow].[AbsentDay] 
    WHERE [UserId] = ISNULL(@UserId, [UserId])
    AND [StartDate] >= ISNULL(@StartDate, [StartDate])
    AND [EndDate] <= ISNULL(@EndDate, [EndDate])
    AND ISNULL([Type], '') = COALESCE(@Type, [Type], '')
    AND ISNULL([Status], '') = COALESCE(@Status, [Status], '')
END
GO