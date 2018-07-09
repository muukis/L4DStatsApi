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
	// Plugin version public Cvar
	CreateConVar("l4dstatsapi_version", PLUGIN_VERSION, "Custom Player Stats (API) Version", FCVAR_PLUGIN|FCVAR_SPONLY|FCVAR_REPLICATED|FCVAR_NOTIFY|FCVAR_DONTRECORD);

	cvar_Gamemode = FindConVar("mp_gamemode");

	cvar_ApiBaseUrl = CreateConVar("l4dstatsapi_apibaseurl", "https://pilssi.dy.fi:44033/l4dstatsapi/api", "Base URL for the API (example: https://pilssi.dy.fi:44033/l4dstatsapi/api)", FCVAR_PLUGIN);
	cvar_GameServerGroupPrivateKey = CreateConVar("l4dstatsapi_apibaseurl", "66edfde5-54d6-4a4d-91b6-40209eb9414c", "Game server group private key (example: 66edfde5-54d6-4a4d-91b6-40209eb9414c)", FCVAR_PLUGIN);
	cvar_GameServerPrivateKey = CreateConVar("l4dstatsapi_apibaseurl", "4b12123c-896c-4e01-b966-a2cf57b63357", "Game server private key (4b12123c-896c-4e01-b966-a2cf57b63357)", FCVAR_PLUGIN);
}

public void OnPluginEnd()
{
	delete httpClient;
}

public OnMapStart()
{
	PrintToServer("Plugin l4dstatsapi OnMapStart()");
	new String:apiBaseUrl[1024];
	GetConVarString(cvar_ApiBaseUrl, apiBaseUrl, sizeof(apiBaseUrl));

	new String:gameServerGroupPrivateKey[36];
	GetConVarString(cvar_GameServerGroupPrivateKey, gameServerGroupPrivateKey, sizeof(gameServerGroupPrivateKey));

	new String:gameServerPrivateKey[36];
	GetConVarString(cvar_GameServerPrivateKey, gameServerPrivateKey, sizeof(gameServerPrivateKey));

	delete httpClient;
	httpClient = new HTTPClient(apiBaseUrl);
	
	IdentityRequest identityRequest = new IdentityRequest();
	identityRequest.SetGameServerGroupPrivateKey(gameServerGroupPrivateKey);
	identityRequest.SetGameServerPrivateKey(gameServerPrivateKey);
	
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
}

public void OnMapEnd()
{
	PrintToServer("Plugin l4dstatsapi OnMapEnd()");

	if (strlen(CurrentMatchId) == 0)
	{
		return;
	}

	MatchEndRequest matchEndRequest = new MatchEndRequest();
	matchEndRequest.SetMatchId(CurrentMatchId);
	matchEndRequest.SecondsPlayed = 123;
	
	httpClient.Post("Stats/match/end", matchEndRequest, OnMatchEndReceived);
	
	delete matchEndRequest;
}

public void OnMatchEndReceived(HTTPResponse response, any value)
{
	if (response.Status != HTTPStatus_OK) {
		// Failed to retrieve identity
		return;
	}
	
	if (response.Data == null) {
		// Invalid JSON response
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
	if (client == 0 || !IsClientConnected(client) || IsFakeClient(client))
	{
		return true;
	}

	decl String:SteamID[MAX_LINE_WIDTH];
	GetClientSteamId(client, SteamID, sizeof(SteamID));

	if (StrEqual(SteamID, "BOT", false))
	{
		return true;
	}

	return false;
}

public void GetClientSteamId(client, String:steamId[], maxlength)
{
	GetClientAuthId(client, AuthId_Steam2, steamId, maxlength);

	if (!StrEqual(steamId, "BOT", false))
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

	decl String:PlayerName[MAX_LINE_WIDTH];
	GetClientName(client, PlayerName, sizeof(PlayerName));

	JSONArray players = new JSONArray();
	
	PlayerStatsRequest playerStatsRequest = new PlayerStatsRequest();
	playerStatsRequest.SetSteamId(SteamID);
	playerStatsRequest.SetName(PlayerName);
	playerStatsRequest.Kills = 0;
	playerStatsRequest.Deaths = 0;
	
	players.Push(playerStatsRequest);

	MatchStatsRequest matchStatsRequest = new MatchStatsRequest();
	matchStatsRequest.SetMatchId(CurrentMatchId);
	matchStatsRequest.Set("Players", players);
	//matchStatsRequest.Players = players;
	
	httpClient.Post("Stats/match", matchStatsRequest, OnMatchStatsReceived);
	
	delete playerStatsRequest;
	delete matchStatsRequest.Players;
	delete matchStatsRequest;
}

public void OnMatchStatsReceived(HTTPResponse response, any value)
{
	PrintToServer("Plugin l4dstatsapi OnMatchStatsReceived()");
	if (response.Status != HTTPStatus_OK) {
		// Failed to retrieve identity
		return;
	}
	
	if (response.Data == null) {
		// Invalid JSON response
		return;
	}

	// Todo: Clear player stats
}
