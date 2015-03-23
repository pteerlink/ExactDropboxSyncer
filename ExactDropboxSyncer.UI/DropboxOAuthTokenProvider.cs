using System;
using DotNetOpenAuth.OAuth2;
using ExactDropboxSyncer.Dropbox;
using ExactOnline.Client.OAuth;

namespace ExactDropboxSyncer.UI
{
	class DropboxOAuthTokenProvider : IDropboxOAuthIAccessTokenProvider
	{
	    private readonly string clientId;
	    private readonly string clientSecret;
	    private readonly AuthorizationServerDescription authorizationServerDescription;

        private IAuthorizationState state;

	    public DropboxOAuthTokenProvider(string clientId, string clientSecret)
	    {
	        this.clientId = clientId;
	        this.clientSecret = clientSecret;
	        authorizationServerDescription = new AuthorizationServerDescription
            {
                AuthorizationEndpoint = new Uri(@"https://www.dropbox.com/1/oauth2/authorize"),
                ProtocolVersion = ProtocolVersion.V20,
                TokenEndpoint = new Uri(@"https://api.dropbox.com/1/oauth2/token")
            };
	    }

	    public string GetAccessToken()
		{
            var oauth = new OAuthClient(authorizationServerDescription, clientId, clientSecret, new Uri(@"http://localhost:12345/oauth2callback"));
            oauth.Authorize(ref state, null);
            return state.AccessToken;
		}
	}
}
