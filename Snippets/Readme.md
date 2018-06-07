# Collection of Snippets

## OAuth Token Manager

Caches oauth tokens in a thread safe way

```C#
    class OAuthTokenManager
    {
        HttpClient httpClient = new HttpClient();

        class AdquiredToken
        {
            internal string Token { get; set; }
            internal DateTime ValidUntil { get; set; }

            internal bool IsValid() => ValidUntil > DateTime.UtcNow;
        }

        private OAuthTokenManager()
        {

        }
        static OAuthTokenManager singleton = new OAuthTokenManager();

  
        static internal OAuthTokenManager Instance { get { return singleton; } }


        Dictionary<string, AdquiredToken> adquiredTokens = new Dictionary<string, AdquiredToken>();
        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        internal async Task<string> GetToken(string uri, string grantType, string clientId, string clientSecret, TimeSpan validity)
        {
            if (adquiredTokens.TryGetValue(uri, out var adquiredToken))
            {
                if (adquiredToken.IsValid())
                    return adquiredToken.Token;
            }

            await semaphoreSlim.WaitAsync();
            try
            {
                if (adquiredTokens.TryGetValue(uri, out adquiredToken))
                {
                    if (adquiredToken.IsValid())
                    {
                        return adquiredToken.Token;
                    }
                }

                adquiredToken = await ObtainToken(uri, grantType, clientId, clientSecret, validity);
                if (adquiredToken != null)
                {
                    adquiredTokens[uri] = adquiredToken;
                    return adquiredToken.Token;
                }
                else
                {
                    adquiredTokens.Remove(uri);
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }

            return null;
        }

        private async Task<AdquiredToken> ObtainToken(string uri, string grantType, string clientId, string clientSecret, TimeSpan validity)
        {
            // 1. Get auth token
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", grantType),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),

            });
            var tokenRes = await httpClient.PostAsync(uri, formContent);

            if (tokenRes.IsSuccessStatusCode)
            {
                var authResPayload = JsonConvert.DeserializeObject<JToken>(await tokenRes.Content.ReadAsStringAsync());
                var token = authResPayload["access_token"].ToString();
                return new AdquiredToken
                {
                    Token = token,
                    ValidUntil = DateTime.UtcNow.Add(validity)
                };
            }

            return null;
        }
    }

```