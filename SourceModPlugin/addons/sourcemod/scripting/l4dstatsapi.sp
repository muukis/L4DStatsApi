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

new Handle:PlayerStatsRequests[MAXPLAYERS + 1];
new String:CurrentMatchId[MAX_LINE_WIDTH] = "";
new PostAdminCheckRetryCounter[MAXPLAYERS + 1];

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
}

public void OnPluginEnd()
{
	PrintToServer("Plugin l4dstatsapi OnPluginEnd()");
	delete httpClient;
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

	delete httpClient;
	httpClient = new HTTPClient(apiBaseUrl);

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
}

public Action:event_MapTransition(Handle:event, const String:name[], bool:dontBroadcast)
{
	PrintToServer("Plugin l4dstatsapi event_MapTransition()");
	OnMapEnd();
}

public OnMapEnd()
{
	PrintToServer("Plugin l4dstatsapi OnMapEnd()");
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

	new Handle:playerStatsRequest = CreatePlayerStatsRequest(client, SteamID);
	SendSinglePlayerStatsRequest(playerStatsRequest);
}

public void OnMatchStatsReceived(HTTPResponse response, any value)
{
	if (response.Status != HTTPStatus_OK) {
		// Failed to retrieve identity
		return;
	}

	// Todo: Clear player stats
}

public void SendSinglePlayerStatsRequest(Handle:playerStatsRequest)
{
	JSONArray players = new JSONArray();
	players.Push(view_as<PlayerStatsRequest>(playerStatsRequest));

	MatchStatsRequest matchStatsRequest = new MatchStatsRequest();
	matchStatsRequest.SetMatchId(CurrentMatchId);
	matchStatsRequest.Players = players;

	decl String:json[2048];
	matchStatsRequest.ToString(json, sizeof(json));
	PrintToServer("Plugin l4dstatsapi matchStatsRequest=%s", json);

	httpClient.Post("Stats/match", matchStatsRequest, OnMatchStatsReceived);
	
	delete playerStatsRequest;
	delete players;
	delete matchStatsRequest;
}

public Handle:CreatePlayerStatsRequest(client, const String:SteamId[])
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

public int FindPlayerStatsRequestIndexBySteamId(const String:SteamId[])
{
	for (int i = 1; i <= MAXPLAYERS; i++)
	{
		if (PlayerStatsRequests[i] == INVALID_HANDLE)
		{
			continue;
		}
		
		decl String:CheckedSteamId[MAX_LINE_WIDTH];
		PlayerStatsRequest playerStatsRequest = view_as<PlayerStatsRequest>(PlayerStatsRequests[i]);
		playerStatsRequest.GetSteamId(CheckedSteamId, sizeof(CheckedSteamId));
		delete playerStatsRequest;
		
		if (StrEqual(SteamId, CheckedSteamId, false))
		{
			return i;
		}
	}
	
	return -1;
}

public Handle:GetPlayerStatsRequest(client)
{
	decl String:SteamId[MAX_LINE_WIDTH];

	if (IsClientBotWithSteamId(client, SteamId, sizeof(SteamId)))
	{
		return INVALID_HANDLE;
	}
	
	decl String:CheckedSteamId[MAX_LINE_WIDTH];

	if (PlayerStatsRequests[client] != INVALID_HANDLE)
	{
		PlayerStatsRequest playerStatsRequest = view_as<PlayerStatsRequest>(PlayerStatsRequests[client]);
		playerStatsRequest.GetSteamId(CheckedSteamId, sizeof(CheckedSteamId));
		delete playerStatsRequest;

		if (StrEqual(SteamId, CheckedSteamId, false))
		{
			return PlayerStatsRequests[client];
		}
		else
		{
			SendSinglePlayerStatsRequest(PlayerStatsRequests[client]);
			PlayerStatsRequests[client] = CreatePlayerStatsRequest(client, SteamId);
			return PlayerStatsRequests[client];
		}
	}
	
	int playerStatsRequestIndex = FindPlayerStatsRequestIndexBySteamId(SteamId);
	
	if (playerStatsRequestIndex >= 0)
	{
		if (PlayerStatsRequests[client] == INVALID_HANDLE)
		{
			PlayerStatsRequests[client] = PlayerStatsRequests[playerStatsRequestIndex];
			PlayerStatsRequests[playerStatsRequestIndex] = INVALID_HANDLE;
		}
		else
		{
			SendSinglePlayerStatsRequest(PlayerStatsRequests[client]);
			PlayerStatsRequests[client] = PlayerStatsRequests[playerStatsRequestIndex];
			PlayerStatsRequests[playerStatsRequestIndex] = INVALID_HANDLE;
		}
		
		return PlayerStatsRequests[client];
	}

	if (PlayerStatsRequests[client] != INVALID_HANDLE)
	{
		SendSinglePlayerStatsRequest(PlayerStatsRequests[client]);
	}

	PlayerStatsRequests[client] = CreatePlayerStatsRequest(client, SteamId);
	return PlayerStatsRequests[client];
}
