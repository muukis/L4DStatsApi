CREATE TABLE [dbo].[GameServer] (
    [Id]      UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [ApiUser] NVARCHAR (50)    NOT NULL,
    [ApiKey]  UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Name]    NVARCHAR (255)   NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

