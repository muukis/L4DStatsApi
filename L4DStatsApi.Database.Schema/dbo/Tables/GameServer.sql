CREATE TABLE [dbo].[GameServer] (
    [Id]      UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Key]      UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [GroupId] UNIQUEIDENTIFIER NOT NULL, 
    [Name]    NVARCHAR (255)   NULL,
    [IsActive] BIT NOT NULL DEFAULT 1, 
    [IsValid] BIT NOT NULL DEFAULT 1, 
    [LastActive] DATETIME NOT NULL DEFAULT (getutcdate()), 
    PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_GameServer_GameServerGroup] FOREIGN KEY ([GroupId]) REFERENCES [GameServerGroup]([Id])
);


GO

CREATE TRIGGER [dbo].[UpdateGameServerLastActive]
    ON [dbo].[GameServer]
    AFTER UPDATE
    AS
    BEGIN
        SET NoCount ON
		UPDATE [dbo].[GameServer]
        SET [LastActive] = GETUTCDATE()
        WHERE [Id] IN (SELECT DISTINCT [Id] FROM Inserted)
    END
