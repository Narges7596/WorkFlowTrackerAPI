USE WorkFlowTrackerDB
GO

CREATE OR ALTER PROCEDURE [WorkFlow].spRegistrationUpsert
-- EXEC [WorkFlow].spRegistrationUpsert @UserId=1, @PostId=1
    @Email        NVARCHAR(50),
    @PasswordHash VARBINARY(MAX),
    @PasswordSalt VARBINARY(MAX) 
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM [WorkFlow].Auth WHERE Email = @Email)
    BEGIN
        INSERT INTO [WorkFlow].Auth(
            [Email],
            [PasswordHash],
            [PasswordSalt]
        ) VALUES (
            @Email,
            @PasswordHash,
            @PasswordSalt
        )
    END
    ELSE
    BEGIN
        UPDATE [WorkFlow].Auth
        SET
            [PasswordHash] = @PasswordHash,
            [PasswordSalt] = @PasswordSalt
        WHERE Email = @Email
    END
END
GO

CREATE OR ALTER PROCEDURE [WorkFlow].spLoginConfirmation_Get
    @Email NVARCHAR(50)
AS
BEGIN
    SELECT
        [Auth].[PasswordHash],
        [Auth].[PasswordSalt] 
    FROM [WorkFlow].Auth AS Auth
    WHERE Auth.Email = @Email
END
GO