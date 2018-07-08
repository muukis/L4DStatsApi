#include <sourcemod>
#include <ripext>
#include <l4dstatsapi>

HTTPClient httpClient;

public void OnPluginStart()
{
	PrintToServer("Plugin l4dstatsapi OnPluginStart()");
	
    httpClient = new HTTPClient("https://pilssi.dy.fi:44033/l4dstatsapi/api");
	
	IdentityRequest identityRequest = new IdentityRequest();
	identityRequest.SetGameServerGroupPrivateKey("");
	identityRequest.SetGameServerPrivateKey("");
	
    httpClient.Post("Identity", identityRequest, OnIdentityReceived);
	PrintToServer("IdentityRequest posted...");
	
	delete identityRequest;
}

public void OnIdentityReceived(HTTPResponse response, any value)
{
	PrintToServer("Plugin l4dstatsapi OnIdentityReceived()");
	
    if (response.Status != HTTPStatus_OK) {
		PrintToServer("response.Status = %s", response.Status);
        // Failed to retrieve identity
        return;
    }
	
    if (response.Data == null) {
		PrintToServer("response.Data = null");
        // Invalid JSON response
        return;
    }

	new String:token[1024];
	
    // Indicate that the response is a JSON object
	IdentityResponse identityResponse = view_as<IdentityResponse>(response.Data);
	identityResponse.GetToken(token, sizeof(token));
	
	Format(token, sizeof(token), "Bearer %s", token);
    httpClient.SetHeader("Authorization", token);
	
    PrintToServer("Retrieved identity with token '%s'", token);
}