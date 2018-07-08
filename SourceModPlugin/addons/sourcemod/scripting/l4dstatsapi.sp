#include <sourcemod>
#include <ripext>
#include <l4dstatsapi>

HTTPClient httpClient;

public void OnPluginStart()
{
    httpClient = new HTTPClient("https://pilssi.dy.fi:44033/l4dstatsapi/api");
	
	IdentityRequest identityRequest = new IdentityRequest();
	identityRequest.SetGameServerGroupPrivateKey("");
	identityRequest.SetGameServerPrivateKey("");
	
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

	new String:token[1024];
	
    // Indicate that the response is a JSON object
	IdentityResponse identityResponse = view_as<IdentityResponse>(response.Data);
	identityResponse.GetToken(token, sizeof(token));
	
	Format(token, sizeof(token), "Bearer %s", token);
    httpClient.SetHeader("Authorization", token);
	
    PrintToServer("Retrieved identity with token '%s'", token);
}