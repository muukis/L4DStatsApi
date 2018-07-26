CREATE VIEW [dbo].[PlayerStatsFull] AS
SELECT
	SUM(wt.[Count]) [Count]
	,SUM(wt.[HeadshotCount]) [HeadshotCount]
	,wt.[Type] [TargetType]
	,w.[Name] [WeaponName]
	,mp.[SteamId]
	,m.[Id] [MatchId]
	,gs.[PublicKey] [GameServerPublicKey]
	,gsg.[PublicKey] [GameServerGroupPublicKey]
FROM [GameServerGroup] gsg
INNER JOIN [GameServer] gs
	ON gsg.Id = gs.GroupId
INNER JOIN [Match] m
	ON gs.Id = m.GameServerId
INNER JOIN [MatchPlayer] mp
	ON m.Id = mp.MatchId
INNER JOIN [Weapon] w
	ON mp.Id = w.MatchPlayerId
INNER JOIN [WeaponTarget] wt
	ON w.Id = wt.WeaponId
WHERE gsg.IsValid = 1
	AND gs.IsValid = 1
GROUP BY
	wt.[Type]
	,w.[Name]
	,mp.[SteamId]
	,m.[Id]
	,gs.[PublicKey]
	,gsg.[PublicKey]
