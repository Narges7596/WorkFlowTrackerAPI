USE WorkFlowTrackerDB
GO

/*
EXEC [WorkFlow].spUser_Upsert 
    @Email='test@test.com',
    @FirstName='Narges',
    @LastName='Farazan',
    @Gender='female',
    @Salary=5000

SELECT
    [User].[UserId],
    [User].[Email],
    [User].[FirstName],
    [User].[LastName],
    [User].[Gender],
    [UserJobInfo].[Department],
    [UserJobInfo].[Role],
    [UserJobInfo].[Team],
    [UserJobInfo].[DateJoined],
    [UserSalary].[Salary],
    [UserSalary].[DaysPerWeek],
    [UserSalary].[HoursPerDay] 
FROM [WorkFlow].[User] As [User]
    LEFT JOIN [WorkFlow].[UserSalary] AS [UserSalary]
        ON [User].UserId = [UserSalary].UserId
    LEFT JOIN [WorkFlow].[UserJobInfo] AS [UserJobInfo]
        ON [User].UserId = [UserJobInfo].UserId

EXEC WorkFlow.spUser_Upsert 
    @Email = 'test2@test.com',
    @FirstName = 'test',
    @LastName = 'test',
    @Gender = 'female',
    @Department = 'test',
    @Role = 'test',
    @Team = 'test',
    @Salary = 5000
  */

CREATE OR ALTER PROCEDURE [WorkFlow].[spUser_Upsert]
-- EXEC [WorkFlow].spUser_Upsert
    @Email      NVARCHAR(50),
    @FirstName  NVARCHAR(50),
    @LastName   NVARCHAR(50),
    @Gender     NVARCHAR(50),

    @Department VARCHAR(50) = NULL,
    @Role       VARCHAR(50) = NULL,
    @Team       VARCHAR(50) = NULL,
    @DateJoined DATETIME = NULL,

    @Salary     DECIMAL(18, 4) = NULL,
    @DaysPerWeek INT = 5,
    @HoursPerDay INT = 8,

    @UserId     INT = NULL
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM [WorkFlow].[User] WHERE UserId = @UserId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM [WorkFlow].[User] WHERE Email = @Email)
        BEGIN
        -- if the UserId and the Email didn't exist in the table, so we add it
            DECLARE @OutputUserId INT

            INSERT INTO [WorkFlow].[User](
                [Email],
                [FirstName],
                [LastName],
                [Gender] 
            )VALUES(
                @Email,
                @FirstName,
                @LastName,
                @Gender
            )

            SET @OutputUserId = @@IDENTITY

            INSERT INTO [WorkFlow].UserJobInfo (
                [UserId],
                [Department],
                [Role],
                [Team],
                [DateJoined]
            )VALUES(
                @OutputUserId,
                @Department,
                @Role,
                @Team,
                GETDATE()
            )

            INSERT INTO [WorkFlow].UserSalary (
                [UserId],
                [Salary],
                [DaysPerWeek],
                [HoursPerDay]
            )VALUES(
                @OutputUserId,
                @Salary,
                @DaysPerWeek,
                @HoursPerDay
            )
        END
        ELSE
        BEGIN
            THROW 50001, 'Email already exists.', 1;
        END
    END
    ELSE
    BEGIN
        UPDATE [WorkFlow].[User]
        SET
            [Email] = @Email,
            [FirstName] = @FirstName,
            [LastName] = @LastName,
            [Gender] = @Gender
        WHERE UserId = @UserId

        IF @DateJoined IS NULL
        BEGIN
            UPDATE [WorkFlow].[UserJobInfo]
            SET
                [Department] = @Department,
                [Role]       = @Role,
                [Team]       = @Team
            WHERE UserId = @UserId
        END
        ELSE
        BEGIN
            UPDATE [WorkFlow].[UserJobInfo]
            SET
                [Department] = @Department,
                [Role]       = @Role,
                [Team]       = @Team,
                [DateJoined] = @DateJoined
            WHERE UserId = @UserId
        END

        UPDATE [WorkFlow].[UserSalary]
        SET
            [Salary] = @Salary,
            [DaysPerWeek] = @DaysPerWeek,
            [HoursPerDay] = @HoursPerDay
        WHERE UserId = @UserId
    END
END
GO

CREATE OR ALTER PROCEDURE [WorkFlow].[spUser_Delete]
-- EXEC [WorkFlow].spUser_Delete
    @UserId INT
AS
BEGIN
    DELETE FROM [WorkFlow].[User]
    WHERE UserId = @UserId

    DELETE FROM [WorkFlow].[UserJobInfo]
    WHERE UserId = @UserId

    DELETE FROM [WorkFlow].[UserSalary]
    WHERE UserId = @UserId
END
GO

CREATE OR ALTER PROCEDURE [WorkFlow].[spUser_Get]
-- EXEC WorkFlow.spUser_Get
    @UserId     INT = NULL,
    @Department VARCHAR(50) = NULL,
    @Team       VARCHAR(50) = NULL
AS
BEGIN
    SELECT
        [User].[UserId],
        [User].[FirstName],
        [User].[LastName],
        [User].[Email],
        [User].[Gender],
        
        [UserJobInfo].[Department],
        [UserJobInfo].[Role],
        [UserJobInfo].[Team],
        [UserJobInfo].[DateJoined],

        [UserSalary].[Salary],
        [UserSalary].[DaysPerWeek],
        [UserSalary].[HoursPerDay]

    FROM [WorkFlow].[User] As [User]
        LEFT JOIN [WorkFlow].[UserSalary] AS [UserSalary]
            ON [User].UserId = [UserSalary].UserId
        LEFT JOIN [WorkFlow].[UserJobInfo] AS [UserJobInfo]
            ON [User].UserId = [UserJobInfo].UserId
    WHERE [User].UserId = ISNULL(@UserId, [User].UserId)
    --AND [UserJobInfo].Department = ISNULL(@Department, [UserJobInfo].Department)
    --AND [UserJobInfo].Team = ISNULL(@Team, [UserJobInfo].Team)
    AND ISNULL([UserJobInfo].Department, '') = COALESCE(Department, [UserJobInfo].Department, '')
    AND ISNULL([UserJobInfo].Team, '') = COALESCE(@Team, [UserJobInfo].Team, '')
END
GO