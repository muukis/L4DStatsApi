#pragma semicolon 1

#include <sourcemod>
#include <ripext>
#include <l4dstatsapi>

#define PLUGIN_NAME "Custom Player Stats (API)"
#define PLUGIN_VERSION "0.1"
#define PLUGIN_DESCRIPTION "Global Player Stats and Ranking."

#define GAMEMODE_UNKNOWN -1
#define GAMEMODE_COOP 0
#define GAMEMODE_VERSUS 1
#define GAMEMODE_REALISM 2
#define GAMEMODE_SURVIVAL 3
#define GAMEMODE_SCAVENGE 4
#define GAMEMODE_REALISMVERSUS 5
#define GAMEMODE_OTHERMUTATIONS 6
#define GAMEMODES 7

#define MAX_LINE_WIDTH 64

new Handle:cvar_Gamemode = INVALID_HANDLE;
new Handle:cvar_ApiBaseUrl = INVALID_HANDLE;
new Handle:cvar_GameServerGroupPrivateKey = INVALID_HANDLE;
new Handle:cvar_GameServerPrivateKey = INVALID_HANDLE;
	
HTTPClient httpClient;
JSONArray playerStatsRequests = null;

new String:CurrentMatchId[MAX_LINE_WIDTH] = "";
new PostAdminCheckRetryCounter[MAXPLAYERS + 1];
new String:GameName[64];

// Plugin Info
public Plugin:myinfo =
{
	name = PLUGIN_NAME,
	author = "Mikko Andersson (muukis)",
	description = PLUGIN_DESCRIPTION,
	version = PLUGIN_VERSION,
	url = "http://www.sourcemod.com/"
};

public void OnPluginStart()
{
	PrintToServer("Plugin l4dstatsapi OnPluginStart()");
	// Plugin version public Cvar
	CreateConVar("l4dstatsapi_version", PLUGIN_VERSION, "Custom Player Stats (API) Version", FCVAR_PLUGIN|FCVAR_SPONLY|FCVAR_REPLICATED|FCVAR_NOTIFY|FCVAR_DONTRECORD);

	cvar_Gamemode = FindConVar("mp_gamemode");

	cvar_ApiBaseUrl = CreateConVar("l4dstatsapi_apibaseurl", "https://pilssi.dy.fi:44033/l4dstatsapi/api", "Base URL for the API (example: https://pilssi.dy.fi:44033/l4dstatsapi/api)", FCVAR_PLUGIN);
	cvar_GameServerGroupPrivateKey = CreateConVar("l4dstatsapi_gameservergroupprivatekey", "66edfde5-54d6-4a4d-91b6-40209eb9414c", "Game server group private key (example: 66edfde5-54d6-4a4d-91b6-40209eb9414c)", FCVAR_PLUGIN);
	cvar_GameServerPrivateKey = CreateConVar("l4dstatsapi_gameserverprivatekey", "4b12123c-896c-4e01-b966-a2cf57b63357", "Game server private key (4b12123c-896c-4e01-b966-a2cf57b63357)", FCVAR_PLUGIN);

	HookEvent("map_transition", event_MapTransition);
	HookEvent("player_disconnect", event_PlayerDisconnect, EventHookMode_Pre);
	HookEvent("player_death", event_PlayerDeath);

	GetGameFolderName(GameName, sizeof(GameName));
}

public void OnPluginEnd()
{
	PrintToServer("Plugin l4dstatsapi OnPluginEnd()");

	SendPlayerStatsRequestsAndEndMatchRequest();

	PrintToServer("Plugin l4dstatsapi OnPluginEnd() - delete httpClient");
	delete httpClient;
	delete playerStatsRequests;

	UnhookEvent("map_transition", event_MapTransition);
	UnhookEvent("player_disconnect", event_PlayerDisconnect, EventHookMode_Pre);
	UnhookEvent("player_death", event_PlayerDeath);
}

public OnConfigsExecuted()
{
	PrintToServer("Plugin l4dstatsapi OnConfigsExecuted()");
	new String:apiBaseUrl[1024];
	GetConVarString(cvar_ApiBaseUrl, apiBaseUrl, sizeof(apiBaseUrl));

	new String:gameServerGroupPrivateKey[37];
	GetConVarString(cvar_GameServerGroupPrivateKey, gameServerGroupPrivateKey, sizeof(gameServerGroupPrivateKey));

	new String:gameServerPrivateKey[37];
	GetConVarString(cvar_GameServerPrivateKey, gameServerPrivateKey, sizeof(gameServerPrivateKey));

	PrintToServer("Plugin l4dstatsapi OnConfigsExecuted() - delete httpClient");
	delete httpClient;
	httpClient = new HTTPClient(apiBaseUrl);

	delete playerStatsRequests;

	IdentityRequest identityRequest = new IdentityRequest();
	identityRequest.SetGameServerGroupPrivateKey(gameServerGroupPrivateKey);
	identityRequest.SetGameServerPrivateKey(gameServerPrivateKey);

	decl String:json[2048];
	identityRequest.ToString(json, sizeof(json));
	PrintToServer("Plugin l4dstatsapi identityRequest=%s", json);
	
	httpClient.Post("Identity", identityRequest, OnIdentityReceived);
	
	delete identityRequest;
}

public void OnIdentityReceived(HTTPResponse response, any value)
{
	if (response.Status != HTTPStatus_OK) {
		// Failed to retrieve identity
		return;
	}
	
	if (response.Data == null) {
		// Invalid JSON response
		return;
	}

	PrintToServer("Plugin l4dstatsapi OnIdentityReceived()");
	new String:token[2048];
	
	// Indicate that the response is a JSON object
	IdentityResponse identityResponse = view_as<IdentityResponse>(response.Data);
	identityResponse.GetToken(token, sizeof(token));

	Format(token, sizeof(token), "Bearer %s", token);
	httpClient.SetHeader("Authorization", token);
	PrintToServer("Plugin l4dstatsapi token=%s", token);

	RequestMapStart();
}

public void RequestMapStart()
{
	decl String:CurrentMapName[MAX_LINE_WIDTH];
	GetCurrentMap(CurrentMapName, sizeof(CurrentMapName));
	
	decl String:CurrentGameMode[MAX_LINE_WIDTH];
	GetConVarString(cvar_Gamemode, CurrentGameMode, sizeof(CurrentGameMode));

	MatchStartRequest matchStartRequest = new MatchStartRequest();
	matchStartRequest.SetGameName(GameName);
	matchStartRequest.SetMapName(CurrentMapName);
	matchStartRequest.SetMatchType(CurrentGameMode);

	decl String:json[2048];
	matchStartRequest.ToString(json, sizeof(json));
	PrintToServer("Plugin l4dstatsapi matchStartRequest=%s", json);
	
	CurrentMatchId = "";

	httpClient.Post("Stats/match/start", matchStartRequest, OnMatchStartReceived);
	
	delete matchStartRequest;
}

public void OnMatchStartReceived(HTTPResponse response, any value)
{
	if (response.Status != HTTPStatus_OK) {
		// Failed to retrieve identity
		return;
	}
	
	if (response.Data == null) {
		// Invalid JSON response
		return;
	}

	// Indicate that the response is a JSON object
	MatchStartResponse matchStartResponse = view_as<MatchStartResponse>(response.Data);
	matchStartResponse.GetMatchId(CurrentMatchId, sizeof(CurrentMatchId));
	PrintToServer("Plugin l4dstatsapi CurrentMatchId=%s", CurrentMatchId);

	delete playerStatsRequests;
	playerStatsRequests = new JSONArray();

	PrintToServer("Plugin l4dstatsapi OnMatchStartReceived()");
}

public Action:event_MapTransition(Handle:event, const String:name[], bool:dontBroadcast)
{
	PrintToServer("Plugin l4dstatsapi event_MapTransition()");
	RequestMapEnd();
}

public OnMapEnd()
{
	RequestMapEnd();
}

public void RequestMapEnd()
{
	PrintToServer("Plugin l4dstatsapi RequestMapEnd()");
	if (strlen(CurrentMatchId) == 0)
	{
		return;
	}

	MatchEndRequest matchEndRequest = new MatchEndRequest();
	matchEndRequest.SetMatchId(CurrentMatchId);
	matchEndRequest.SecondsPlayed = 123;

	decl String:json[2048];
	matchEndRequest.ToString(json, sizeof(json));
	PrintToServer("Plugin l4dstatsapi matchEndRequest=%s", json);
	
	httpClient.Post("Stats/match/end", matchEndRequest, OnMatchEndReceived);
	
	delete matchEndRequest;
}

public void OnMatchEndReceived(HTTPResponse response, any value)
{
	if (response.Status != HTTPStatus_OK) {
		// Failed to retrieve identity
		return;
	}
	
	CurrentMatchId = "";
	
	PrintToServer("Plugin l4dstatsapi OnMatchEndReceived() - delete httpClient");
	delete httpClient;
}

public OnClientPostAdminCheck(client)
{
	PostAdminCheckRetryCounter[client] = 0;

	if (IsClientBot(client))
	{
		return;
	}

	CreateTimer(1.0, ClientPostAdminCheck, client);
}

public bool:IsClientBot(client)
{
	decl String:SteamID[MAX_LINE_WIDTH];
	return IsClientBotWithSteamId(client, SteamID, sizeof(SteamID));
}

public bool:IsClientBotWithSteamId(client, String:steamId[], maxlength)
{
	if (client == 0 || !IsClientConnected(client) || IsFakeClient(client))
	{
		return true;
	}

	GetClientSteamId(client, steamId, maxlength);

	if (StrEqual(steamId, "BOT", false))
	{
		return true;
	}

	return false;
}

public void GetClientSteamId(client, String:steamId[], maxlength)
{
	GetClientAuthId(client, AuthId_Steam2, steamId, maxlength);

	if (StrEqual(steamId, "STEAM_ID_LAN", false))
	{
		GetClientIP(client, steamId, maxlength);
	}
}

public Action:ClientPostAdminCheck(Handle:timer, any:client)
{
	PrintToServer("Plugin l4dstatsapi ClientPostAdminCheck()");

	if (!IsClientInGame(client))
	{
		if (PostAdminCheckRetryCounter[client]++ < 10)
		{
			CreateTimer(3.0, ClientPostAdminCheck, client);
		}

		return;
	}

	decl String:SteamID[MAX_LINE_WIDTH];
	GetClientSteamId(client, SteamID, sizeof(SteamID));

	PlayerStatsRequest playerStatsRequest = GetPlayerStatsRequest(client, SteamID);
	delete playerStatsRequest;
}

public void OnMatchStatsReceived(HTTPResponse response, any value)
{
	if (response.Status != HTTPStatus_OK) {
		// Failed to retrieve identity
		return;
	}

	if (value)
	{
		playerStatsRequests.Clear();
		delete playerStatsRequests;
		playerStatsRequests = new JSONArray();

		RequestMapEnd();
	}
}

public void SendSinglePlayerStatsRequest(PlayerStatsRequest:playerStatsRequest)
{
	PrintToServer("Plugin l4dstatsapi SendSinglePlayerStatsRequest()");
	JSONArray players = new JSONArray();
	players.Push(view_as<PlayerStatsRequest>(playerStatsRequest));

	MatchStatsRequest matchStatsRequest = new MatchStatsRequest();
	matchStatsRequest.SetMatchId(CurrentMatchId);
	matchStatsRequest.Players = players;

	decl String:json[2048];
	matchStatsRequest.ToString(json, sizeof(json));
	PrintToServer("Plugin l4dstatsapi matchStatsRequest=%s", json);

	httpClient.Post("Stats/match", matchStatsRequest, OnMatchStatsReceived);
	
	delete players;
	delete matchStatsRequest;
}

public void SendPlayerStatsRequestsAndEndMatchRequest()
{
	if (playerStatsRequests == null || playerStatsRequests.Length == 0)
	{
		return;
	}
	
	MatchStatsRequest matchStatsRequest = new MatchStatsRequest();
	matchStatsRequest.SetMatchId(CurrentMatchId);
	matchStatsRequest.Players = playerStatsRequests;

	decl String:json[2048];
	matchStatsRequest.ToString(json, sizeof(json));
	PrintToServer("Plugin l4dstatsapi matchStatsRequest=%s", json);

	httpClient.Post("Stats/match", matchStatsRequest, OnMatchStatsReceived, true);

	delete matchStatsRequest;

	RequestMapEnd();
}

public PlayerStatsRequest:CreatePlayerStatsRequest(client, const String:SteamId[])
{
	decl String:PlayerName[MAX_LINE_WIDTH];
	GetClientName(client, PlayerName, sizeof(PlayerName));

	PlayerStatsRequest playerStatsRequest = new PlayerStatsRequest();
	playerStatsRequest.SetSteamId(SteamId);
	playerStatsRequest.SetName(PlayerName);
	playerStatsRequest.Kills = 0;
	playerStatsRequest.Deaths = 0;

	return playerStatsRequest;
}

public PlayerStatsRequest:GetPlayerStatsRequest(client, const String:SteamId[])
{
	PrintToServer("Plugin l4dstatsapi GetPlayerStatsRequest()");

	if (playerStatsRequests == null)
	{
		return null;
	}

	PlayerStatsRequest playerStatsRequest;
	decl String:tempSteamId[MAX_LINE_WIDTH];

	for (int i = 0; i < playerStatsRequests.Length; i++)
	{
		playerStatsRequest = view_as<PlayerStatsRequest>(playerStatsRequests.Get(i));
		playerStatsRequest.GetSteamId(tempSteamId, sizeof(tempSteamId));

		if (StrEqual(SteamId, tempSteamId, false))
		{
			return playerStatsRequest;
		}
		
		delete playerStatsRequest;
	}

	playerStatsRequest = CreatePlayerStatsRequest(client, SteamId);
	playerStatsRequests.Push(playerStatsRequest);

	SendSinglePlayerStatsRequest(playerStatsRequest);

	return playerStatsRequest;
}

public Action:event_PlayerDisconnect(Handle:event, const String:event_name[], bool:dontBroadcast)
{
	new client = GetClientOfUserId(GetEventInt(event, "userid"));
	decl String:steamId[MAX_LINE_WIDTH];

	if (IsClientBotWithSteamId(client, steamId, sizeof(steamId)))
	{
		return Plugin_Continue;
	}

	for (int i = 1; i <= MaxClients; i++)
	{
		if (client == i)
		{
			continue;
		}
		
		if (IsClientInGame(i) && !IsFakeClient(i))
		{
			return Plugin_Continue;
		}
	}

	PrintToServer("Plugin l4dstatsapi NO OTHER HUMAN PLAYERS FOUND ON THE SERVER");
	SendPlayerStatsRequestsAndEndMatchRequest();

	return Plugin_Continue;
}

public Action:event_PlayerDeath(Handle:event, const String:name[], bool:dontBroadcast)
{
	if (playerStatsRequests == null)
	{
		return;
	}

	PrintToServer("Plugin l4dstatsapi event_PlayerDeath()");

	bool attackerIsBot = GetEventBool(event, "attackerisbot");
	bool victimIsBot = GetEventBool(event, "victimisbot");

	if (attackerIsBot && victimIsBot)
	{
		return;
	}

	if (!attackerIsBot)
	{
		int attacker = GetClientOfUserId(GetEventInt(event, "attacker"));
		decl String:attackerSteamId[MAX_LINE_WIDTH];
		GetClientSteamId(attacker, attackerSteamId, sizeof(attackerSteamId));
		PlayerStatsRequest attackerStatsRequest = GetPlayerStatsRequest(attacker, attackerSteamId);
		attackerStatsRequest.Kills++;
		delete attackerStatsRequest;
	}

	if (!victimIsBot)
	{
		int victim = GetClientOfUserId(GetEventInt(event, "userid"));
		decl String:victimSteamId[MAX_LINE_WIDTH];
		GetClientSteamId(victim, victimSteamId, sizeof(victimSteamId));
		PlayerStatsRequest victimStatsRequest = GetPlayerStatsRequest(victim, victimSteamId);
		victimStatsRequest.Deaths++;
		delete victimStatsRequest;
	}
}
