CREATE TABLE [dbo].[WeaponTarget]
(
	[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT (newid()), 
    [WeaponId] UNIQUEIDENTIFIER NOT NULL, 
    [SteamId] NVARCHAR(50) NOT NULL, 
    [Count] INT NOT NULL, 
    [HeadshotCount] INT NOT NULL, 
    [Type] NVARCHAR(10) NOT NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_WeaponTarget_ToTable] FOREIGN KEY ([WeaponId]) REFERENCES [Weapon]([Id]), 
    CONSTRAINT [CK_WeaponTarget_Type] CHECK ([Type] in ('kill', 'death'))
)

GO

CREATE INDEX [IX_WeaponTarget_SteamId] ON [dbo].[WeaponTarget] ([SteamId])

GO

CREATE INDEX [IX_WeaponTarget_Type] ON [dbo].[WeaponTarget] ([Type])
