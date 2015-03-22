using System;
using ExactDropboxSyncer.Exact;
using ExactOnline.Client.OAuth;

namespace ExactDropboxSyncer.UI
{
	class ExactOAuthTokenProvider : IExactOnlineOAuthIAccessTokenProvider
	{
	    private readonly string website;
	    private readonly string clientId;
	    private readonly string clientSecret;
	    readonly UserAuthorization userAuthorisation = new UserAuthorization();

	    public ExactOAuthTokenProvider(string website, string clientId, string clientSecret)
	    {
	        this.website = website;
	        this.clientId = clientId;
	        this.clientSecret = clientSecret;
	    }

	    public string GetAccessToken()
		{
            UserAuthorizations.Authorize(userAuthorisation, website, clientId, clientSecret, new Uri(@"http://localhost/oauth2callback"));
            return userAuthorisation.AccessToken;
		}
	}
}
