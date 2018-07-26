CREATE VIEW [dbo].[PlayerStatsWeapon] AS
SELECT
	SUM([Count]) [Count]
	,SUM([HeadshotCount]) [HeadshotCount]
	,[TargetType]
	,[WeaponName]
	,[SteamId]
FROM [PlayerStatsFull]
GROUP BY
	[TargetType]
	,[WeaponName]
	,[SteamId]
