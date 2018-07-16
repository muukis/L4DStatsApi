CREATE TABLE [dbo].[MatchPlayer]
(
	[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT (newid()), 
    [MatchId] UNIQUEIDENTIFIER NOT NULL, 
    [SteamId] NVARCHAR(50) NOT NULL, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Kills] INT NOT NULL , 
    [Deaths] INT NOT NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_MatchPlayer_ToTable] FOREIGN KEY ([MatchId]) REFERENCES [Match]([Id])
)

GO

CREATE INDEX [IX_MatchPlayer_SteamId] ON [dbo].[MatchPlayer] ([SteamId])

GO

CREATE INDEX [IX_MatchPlayer_Name] ON [dbo].[MatchPlayer] ([Name])
