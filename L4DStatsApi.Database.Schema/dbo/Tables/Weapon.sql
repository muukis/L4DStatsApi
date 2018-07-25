CREATE TABLE [dbo].[Weapon]
(
	[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT (newid()), 
    [MatchPlayerId] UNIQUEIDENTIFIER NOT NULL, 
    [Name] NVARCHAR(50) NOT NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_Weapon_ToTable] FOREIGN KEY ([MatchPlayerId]) REFERENCES [MatchPlayer]([Id])
)

GO

CREATE INDEX [IX_Weapon_Type] ON [dbo].[Weapon] ([Name])
