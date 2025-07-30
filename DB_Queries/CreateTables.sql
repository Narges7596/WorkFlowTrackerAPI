Use WorkFlowTrackerDB
GO

-- CREATE SCHEMA [WorkFlow]
-- GO

-- Exported from QuickDBD: https://www.quickdatabasediagrams.com/
-- Link to schema: https://app.quickdatabasediagrams.com/#/d/QzIiNQ

CREATE TABLE [WorkFlow].[User] (
    [UserId]    INT IDENTITY (1, 1) NOT NULL,
    [Email]     VARCHAR(50)  NOT NULL,
    [FirstName] NVARCHAR(50) NOT NULL,
    [LastName]  NVARCHAR(50) NOT NULL,
    [Gender]    VARCHAR(50)  NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY ([UserId]),
    CONSTRAINT [UK_User_Email] UNIQUE ([Email])
)
GO

CREATE TABLE [WorkFlow].[Auth] (
    [Email]        VARCHAR (50)   NOT NULL,
    [PasswordHash] VARBINARY(MAX)  NOT NULL,
    [PasswordSalt] VARBINARY(MAX)  NOT NULL,
    CONSTRAINT [UK_Auth_Email] UNIQUE ([Email])
)
GO

CREATE TABLE [WorkFlow].[UserJobInfo] (
    [UserId]     INT         NOT NULL,
    [Department] VARCHAR(50) NULL,
    [Role]       VARCHAR(50) NULL,
    [Team]       VARCHAR(50) NULL,
    [DateJoined] DateTime    NOT NULL,
    CONSTRAINT [PK_UserJobInfo] PRIMARY KEY NONCLUSTERED ([UserId])
)
GO

CREATE TABLE [WorkFlow].[UserSalary] (
    [UserId] INT  NOT NULL,
    [Salary] INT  NOT NULL,
    [DaysPerWeek] INT  NOT NULL,
    [HoursPerDay] INT  NOT NULL,
    CONSTRAINT [PK_UserSalary] PRIMARY KEY NONCLUSTERED ([UserId])
)
GO
-------------------------------------------------------
CREATE TABLE [WorkFlow].[WorkLog] (
    [WorkLogId]   INT  IDENTITY (1, 1) NOT NULL,
    [UserId]      INT  NOT NULL,
    [WorkDay]     DATE NOT NULL,
    [StartTime]   TIME NOT NULL,
    [EndTime]     TIME NOT NULL,
    [ProjectId]   INT  NOT NULL,
    [Description] NVARCHAR(200) NOT NULL,
    CONSTRAINT [PK_WorkLog] PRIMARY KEY NONCLUSTERED ([WorkLogId])
)
GO

CREATE TABLE [WorkFlow].[Project] (
    [ProjectId] INT IDENTITY (1, 1) NOT NULL,
    [ProjectName]      NVARCHAR(100)  NOT NULL,
    [ClientId]  INT  NOT NULL,
    CONSTRAINT [UK_Project_Name] UNIQUE ([ProjectName])
)
GO

CREATE TABLE [WorkFlow].[Client] (
    [ClientId] INT IDENTITY (1, 1) NOT NULL,
    [ClientName]     NVARCHAR(100)  NOT NULL,
    CONSTRAINT [UK_Client_ClientName] UNIQUE ([ClientName])
)
GO


CREATE TABLE [WorkFlow].[Tag] (
    [TagId] INT IDENTITY (1, 1) NOT NULL,
    [TagName]  NVARCHAR(20) NOT NULL,
    CONSTRAINT [UK_Tag_TagName] UNIQUE ([TagName])
)
GO
-- ALTER TABLE [WorkFlow].[Client]
-- ADD CONSTRAINT UK_Client_ClientName UNIQUE ([ClientName]);

CREATE TABLE [WorkFlow].[WorkLogTag] (
    [TagId]     INT NOT NULL,
    [WorkLogId] INT NOT NULL,
    CONSTRAINT [PK_WorkLogTag] PRIMARY KEY NONCLUSTERED ([TagId],[WorkLogId])
)
GO
-------------------------------------------------------
CREATE TABLE [WorkFlow].[AbsentDay] (
    [AbsentDayId] INT IDENTITY (1, 1) NOT NULL,
    [UserId]      INT           NOT NULL,
    [StartDate]   DATETIME      NOT NULL,
    [EndDate]     DATETIME      NOT NULL,
    [Type]        VARCHAR(50)   NOT NULL, -- CHECK (type IN ('vacation', 'sick', 'special', 'unpaid', 'other'))
    [Status]      VARCHAR(20)   NOT NULL, -- CHECK (status IN ('pending', 'approved', 'rejected'))
    [Description] NVARCHAR(200) NULL,
    [RequestedAt] DATETIME      NOT NULL,
    [ApprovedBy]  INT           NULL,
    [ApprovedAt]  DATETIME      NULL,
)
GO