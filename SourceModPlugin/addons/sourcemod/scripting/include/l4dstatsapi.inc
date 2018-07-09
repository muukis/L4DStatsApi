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

	public void GetName(char[] buffer, int maxlength)
	{
		this.GetString("Name", buffer, maxlength);
	}
	
	public void SetName(const char[] value)
	{
		this.SetString("Name", value);
	}

	property int Kills
	{
		public get() { return this.GetInt("Kills"); }
		public set(int value) { this.SetInt("Kills", value); }
	}
	
	property int Deaths
	{
		public get() { return this.GetInt("Deaths"); }
		public set(int value) { this.SetInt("Deaths", value); }
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
};
