CREATE VIEW [dbo].[PlayerStatsFull] AS
WITH [Full] AS
(
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
)
,[DistinctMatchId] AS
(
	SELECT DISTINCT [MatchId] FROM [Full]
)
,[PlayerLastName] AS
(
	SELECT
		mp.[Name] [LastName]
		,mp.[SteamId]
		,MAX(m.[LastActive]) AS [LastActive]
	FROM [Match] m
	INNER JOIN [MatchPlayer] mp
		ON m.Id = mp.MatchId
	INNER JOIN [DistinctMatchId] dm
		ON m.Id = dm.MatchId
	GROUP BY
		mp.[Name]
		,mp.[SteamId]
)
,[FullWithLPlayerLastName] AS
(
	SELECT
		f.*
		,pln.[LastName] [PlayerName]
	FROM [Full] f
	INNER JOIN [PlayerLastName] pln
		ON f.SteamId = pln.[SteamId]
)
SELECT *
FROM [FullWithLPlayerLastName]
