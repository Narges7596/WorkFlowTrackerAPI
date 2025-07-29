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
    [ProjectID]   INT  NOT NULL,
    [Description] NVARCHAR(200) NOT NULL,
    CONSTRAINT [PK_WorkLog] PRIMARY KEY NONCLUSTERED ([WorkLogId])
)
GO

CREATE TABLE [WorkFlow].[Project] (
    [ProjectID] INT IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR(100)  NOT NULL,
    [Client]    NVARCHAR(100)  NOT NULL 
)
GO

CREATE TABLE [WorkFlow].[Tag] (
    [TagId] INT IDENTITY (1, 1) NOT NULL,
    [Name]  NVARCHAR(20) NOT NULL
)
GO

CREATE TABLE [WorkFlow].[WorkLogTag] (
    [TagId]     INT NOT NULL,
    [WorkLogId] INT NOT NULL,
    CONSTRAINT [PK_WorkLogTag] PRIMARY KEY NONCLUSTERED ([TagId],[WorkLogId])
)
GO
-------------------------------------------------------
CREATE TABLE [WorkFlow].[timeOffRequest] (
    [RequestId]   INT IDENTITY (1, 1) NOT NULL,
    [UserId]      INT           NOT NULL,
    [StartDate]   DATE          NOT NULL,
    [EndDate]     DATE          NOT NULL,
    [Type]        VARCHAR(50)   NOT NULL, -- CHECK (type IN ('vacation', 'sick', 'special', 'unpaid', 'other'))
    [Status]      VARCHAR(20)   NOT NULL, -- CHECK (status IN ('pending', 'approved', 'rejected'))
    [Description] NVARCHAR(200) NULL,
    [RequestedAt] TIMESTAMP     NOT NULL,
    [ApprovedBy]  INT           NULL,
    CONSTRAINT [PK_timeOffRequest] PRIMARY KEY NONCLUSTERED ([RequestId] ASC)
)
GO