CREATE VIEW [dbo].[PlayerStatsBasic] AS
SELECT
	SUM([Count]) [Count]
	,SUM([HeadshotCount]) [HeadshotCount]
	,[TargetType]
	,[SteamId]
FROM [PlayerStatsFull]
GROUP BY
	[TargetType]
	,[SteamId]