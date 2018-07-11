CREATE TABLE [dbo].[Match]
(
	[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT (newid()), 
	[GameServerId] UNIQUEIDENTIFIER NOT NULL, 
    [GameName] NVARCHAR(64) NOT NULL, 
    [MapName] NVARCHAR(100) NOT NULL, 
    [Type] NVARCHAR(50) NOT NULL, 
    [HasEnded] BIT NOT NULL DEFAULT 0, 
    [SecondsPlayed] INT NULL, 
    [StartTime] DATETIME NOT NULL DEFAULT (getutcdate()), 
    [LastActive] DATETIME NOT NULL DEFAULT (getutcdate()), 
    PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_Match_GameServer] FOREIGN KEY ([GameServerId]) REFERENCES [GameServer]([Id])
)

GO

CREATE TRIGGER [dbo].[UpdateMatchLastActive]
    ON [dbo].[Match]
    AFTER UPDATE
    AS
    BEGIN
        SET NoCount ON
		
		DECLARE @lastActive DATETIME = GETUTCDATE()

		UPDATE [dbo].[Match]
        SET [LastActive] = @lastActive
        WHERE [Id] IN (SELECT DISTINCT [Id] FROM Inserted)

		UPDATE [dbo].[GameServer]
        SET [LastActive] = @lastActive
        WHERE [Id] IN (SELECT DISTINCT [GameServerId] FROM Inserted)
    END