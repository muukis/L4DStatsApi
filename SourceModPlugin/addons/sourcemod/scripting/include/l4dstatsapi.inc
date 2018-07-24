#define MAX_LINE_WIDTH 64

methodmap IdentityRequest < JSONObject
{
	// Constructor
	public IdentityRequest() { return view_as<IdentityRequest>(new JSONObject()); }

	public void GetGameServerGroupPrivateKey(char[] buffer, int maxlength)
	{
		this.GetString("GameServerGroupPrivateKey", buffer, maxlength);
	}
	
	public void SetGameServerGroupPrivateKey(const char[] value)
	{
		this.SetString("GameServerGroupPrivateKey", value);
	}

	public void GetGameServerPrivateKey(char[] buffer, int maxlength)
	{
		this.GetString("GameServerPrivateKey", buffer, maxlength);
	}
	
	public void SetGameServerPrivateKey(const char[] value)
	{
		this.SetString("GameServerPrivateKey", value);
	}
};

methodmap IdentityResponse < JSONObject
{
	// Constructor
	public IdentityResponse() { return view_as<IdentityResponse>(new JSONObject()); }

	public void GetGameServerGroupPublicKey(char[] buffer, int maxlength)
	{
		this.GetString("GameServerGroupPublicKey", buffer, maxlength);
	}

	public void GetGameServerPublicKey(char[] buffer, int maxlength)
	{
		this.GetString("GameServerPublicKey", buffer, maxlength);
	}

	public void GetToken(char[] buffer, int maxlength)
	{
		this.GetString("Token", buffer, maxlength);
	}
};

methodmap MatchStartRequest < JSONObject
{
	// Constructor
	public MatchStartRequest() { return view_as<MatchStartRequest>(new JSONObject()); }

	public void GetGameName(char[] buffer, int maxlength)
	{
		this.GetString("GameName", buffer, maxlength);
	}
	
	public void SetGameName(const char[] value)
	{
		this.SetString("GameName", value);
	}

	public void GetMapName(char[] buffer, int maxlength)
	{
		this.GetString("MapName", buffer, maxlength);
	}
	
	public void SetMapName(const char[] value)
	{
		this.SetString("MapName", value);
	}

	public void GetMatchType(char[] buffer, int maxlength)
	{
		this.GetString("MatchType", buffer, maxlength);
	}
	
	public void SetMatchType(const char[] value)
	{
		this.SetString("MatchType", value);
	}
};

methodmap MatchStartResponse < JSONObject
{
	// Constructor
	public MatchStartResponse() { return view_as<MatchStartResponse>(new JSONObject()); }
	
	public void GetMatchId(char[] buffer, int maxlength)
	{
		this.GetString("MatchId", buffer, maxlength);
	}
};

methodmap MatchEndRequest < JSONObject
{
	// Constructor
	public MatchEndRequest() { return view_as<MatchEndRequest>(new JSONObject()); }
	
	public void GetMatchId(char[] buffer, int maxlength)
	{
		this.GetString("MatchId", buffer, maxlength);
	}
	
	public void SetMatchId(const char[] value)
	{
		this.SetString("MatchId", value);
	}
	
	property int SecondsPlayed
	{
		public get() { return this.GetInt("SecondsPlayed"); }
		public set(int value) { this.SetInt("SecondsPlayed", value); }
	}
};

methodmap WeaponRequest < JSONObject
{
	// Constructor
	public WeaponRequest() { return view_as<WeaponRequest>(new JSONObject()); }

	public void GetName(char[] buffer, int maxlength)
	{
		this.GetString("Name", buffer, maxlength);
	}
	
	public void SetName(const char[] value)
	{
		this.SetString("Name", value);
	}

	property JSONArray KillTargets
	{
		public get() { return view_as<JSONArray>(this.Get("KillTargets")); }
		public set(JSONArray value) { this.Set("KillTargets", value); }
	}

	property JSONArray DeathTargets
	{
		public get() { return view_as<JSONArray>(this.Get("DeathTargets")); }
		public set(JSONArray value) { this.Set("DeathTargets", value); }
	}
};

methodmap WeaponTargetRequest < JSONObject
{
	// Constructor
	public WeaponTargetRequest() { return view_as<WeaponTargetRequest>(new JSONObject()); }

	public void GetSteamId(char[] buffer, int maxlength)
	{
		this.GetString("SteamId", buffer, maxlength);
	}
	
	public void SetSteamId(const char[] value)
	{
		this.SetString("SteamId", value);
	}
	
	property int Counter
	{
		public get() { return this.GetInt("Counter"); }
		public set(int value) { this.SetInt("Counter", value); }
	}
	
	property int HeadshotCounter
	{
		public get() { return this.GetInt("HeadshotCounter"); }
		public set(int value) { this.SetInt("HeadshotCounter", value); }
	}
};

methodmap PlayerStatsRequest < JSONObject
{
	// Constructor
	public PlayerStatsRequest() { return view_as<PlayerStatsRequest>(new JSONObject()); }

	public void GetSteamId(char[] buffer, int maxlength)
	{
		this.GetString("SteamId", buffer, maxlength);
	}
	
	public void SetSteamId(const char[] value)
	{
		this.SetString("SteamId", value);
	}

	public void GetBase64EncodedName(char[] buffer, int maxlength)
	{
		this.GetString("Base64EncodedName", buffer, maxlength);
	}
	
	public void SetBase64EncodedName(const char[] value)
	{
		this.SetString("Base64EncodedName", value);
	}

	property JSONArray Weapons
	{
		public get() { return view_as<JSONArray>(this.Get("Weapons")); }
		public set(JSONArray value) { this.Set("Weapons", value); }
	}
	
	public WeaponRequest GetWeapon(const char[] weaponName)
	{
		if (this.Weapons == null)
		{
			this.Weapons = new JSONArray();
		}

		WeaponRequest weapon;
		char tempWeaponName[MAX_LINE_WIDTH];

		for (int i = 0; i < this.Weapons.Length; i++)
		{
			weapon = view_as<WeaponRequest>(this.Weapons.Get(i));
			weapon.GetName(tempWeaponName, sizeof(tempWeaponName));

			if (StrEqual(weaponName, tempWeaponName, false))
			{
				return weapon;
			}

			delete weapon;
			weapon = null;
		}

		weapon = new WeaponRequest();
		weapon.SetName(weaponName);
		this.Weapons.Push(weapon);

		return weapon;
	}
	
	public void AddWeaponTarget(JSONArray weaponTargetArray, const char[] targetSteamId, bool isHeadshot)
	{
		WeaponTargetRequest weaponTarget;
		char tempTargetSteamId[MAX_LINE_WIDTH];

		for (int i = 0; i < weaponTargetArray.Length; i++)
		{
			weaponTarget = view_as<WeaponTargetRequest>(weaponTargetArray.Get(i));
			weaponTarget.GetSteamId(tempTargetSteamId, sizeof(tempTargetSteamId));

			if (StrEqual(targetSteamId, tempTargetSteamId, false))
			{
				weaponTarget.Counter++;

				if (isHeadshot)
				{
					weaponTarget.HeadshotCounter++;
				}

				delete weaponTarget;
				weaponTarget = null;
				return;
			}

			delete weaponTarget;
			weaponTarget = null;
		}
		
		weaponTarget = new WeaponTargetRequest();
		weaponTarget.SetSteamId(targetSteamId);
		weaponTarget.Counter = 1;

		if (isHeadshot)
		{
			weaponTarget.HeadshotCounter = 1;
		}
		else
		{
			weaponTarget.HeadshotCounter = 0;
		}

		weaponTargetArray.Push(weaponTarget);
		delete weaponTarget;
	}
	
	public void AddWeaponKillTarget(const char[] weaponName, const char[] targetSteamId, bool isHeadshot)
	{
		WeaponRequest weapon = this.GetWeapon(weaponName);

		if (weapon.KillTargets == null)
		{
			weapon.KillTargets = new JSONArray();
		}

		this.AddWeaponTarget(weapon.KillTargets, targetSteamId, isHeadshot);
	}
	
	public void AddWeaponDeathTarget(const char[] weaponName, const char[] targetSteamId, bool isHeadshot)
	{
		WeaponRequest weapon = this.GetWeapon(weaponName);

		if (weapon.DeathTargets == null)
		{
			weapon.DeathTargets = new JSONArray();
		}

		this.AddWeaponTarget(weapon.DeathTargets, targetSteamId, isHeadshot);
	}
};

methodmap MatchStatsRequest < JSONObject
{
	// Constructor
	public MatchStatsRequest() { return view_as<MatchStatsRequest>(new JSONObject()); }
	
	public void GetMatchId(char[] buffer, int maxlength)
	{
		this.GetString("MatchId", buffer, maxlength);
	}
	
	public void SetMatchId(const char[] value)
	{
		this.SetString("MatchId", value);
	}

	property JSONArray Players
	{
		public get() { return view_as<JSONArray>(this.Get("Players")); }
		public set(JSONArray value) { this.Set("Players", value); }
	}

	public int EncodeBase64(char[] base64EncodedValue, int len, const char[] value)
	{
		char BASE64_TABLE[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
		char BASE64_FILL_CHAR = '=';

		int valueLength = strlen(value);
		int resPos = 0;

		for (int pos = 0; pos < valueLength; pos++)
		{
			int code = (value[pos] >> 2) & 0x3f;
			resPos += FormatEx(base64EncodedValue[resPos], len - resPos, "%c", BASE64_TABLE[code]);
			code = (value[pos] << 4) & 0x3f;

			if (++pos < valueLength)
			{
				code |= (value[pos] >> 4) & 0x0f;
			}

			resPos += FormatEx(base64EncodedValue[resPos], len - resPos, "%c", BASE64_TABLE[code]);

			if (pos < valueLength)
			{
				code = (value[pos] << 2) & 0x3f;
	
				if (++pos < valueLength)
				{
					code |= (value[pos] >> 6) & 0x03;
				}

				resPos += FormatEx(base64EncodedValue[resPos], len - resPos, "%c", BASE64_TABLE[code]);
			}
			else
			{
				pos++;
				resPos += FormatEx(base64EncodedValue[resPos], len - resPos, "%c", BASE64_FILL_CHAR);
			}

			if (pos < valueLength)
			{
				code = value[pos] & 0x3f;
				resPos += FormatEx(base64EncodedValue[resPos], len - resPos, "%c", BASE64_TABLE[code]);
			}
			else
			{
				resPos += FormatEx(base64EncodedValue[resPos], len - resPos, "%c", BASE64_FILL_CHAR);
			}
		}
	
		return resPos;
	}

	public PlayerStatsRequest CreatePlayerStats(int client, const char[] steamId)
	{
		char playerName[MAX_LINE_WIDTH];
		GetClientName(client, playerName, sizeof(playerName));

		char encodedPlayerName[2*MAX_LINE_WIDTH];
		this.EncodeBase64(encodedPlayerName, sizeof(encodedPlayerName), playerName);

		PlayerStatsRequest playerStatsRequest = new PlayerStatsRequest();
		playerStatsRequest.SetSteamId(steamId);
		playerStatsRequest.SetBase64EncodedName(encodedPlayerName);

		return playerStatsRequest;
	}

	public PlayerStatsRequest GetPlayerStats(int client, const char[] steamId)
	{
		if (this.Players == null)
		{
			this.Players = new JSONArray();
		}

		PlayerStatsRequest playerStats;
		char tempSteamId[MAX_LINE_WIDTH];

		for (int i = 0; i < this.Players.Length; i++)
		{
			playerStats = view_as<PlayerStatsRequest>(this.Players.Get(i));
			playerStats.GetSteamId(tempSteamId, sizeof(tempSteamId));
	
			if (StrEqual(steamId, tempSteamId, false))
			{
				return playerStats;
			}

			delete playerStats;
		}

		playerStats = this.CreatePlayerStats(client, steamId);
		this.Players.Push(playerStats);

		return playerStats;
	}
};
