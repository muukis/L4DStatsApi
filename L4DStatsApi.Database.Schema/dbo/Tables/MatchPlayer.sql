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
