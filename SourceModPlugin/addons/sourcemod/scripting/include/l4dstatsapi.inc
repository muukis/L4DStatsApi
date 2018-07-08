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
