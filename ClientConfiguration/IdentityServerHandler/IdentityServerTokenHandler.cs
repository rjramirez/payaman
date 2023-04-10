using Common.Constants;
using Common.DataTransferObjects.AppSettings;
using Common.DataTransferObjects.Token;
using IdentityModel.Client;
using static IdentityModel.OidcConstants;

namespace ClientConfiguration.IdentityServerHandler
{
    public class IdentityServerTokenHandler : DelegatingHandler
    {
        private readonly IdentityServerClientDefinition _identityServerClientDefinition;
        private readonly IdentityServerTokenDetail _identityServerTokenDetail;
        private readonly HttpClient _httpClient;
        public IdentityServerTokenHandler(
            IdentityServerClientDefinition identityServerClientDefinition,
            IdentityServerTokenDetail identityServerTokenDetail)
        {
            _identityServerClientDefinition = identityServerClientDefinition;
            _identityServerTokenDetail = identityServerTokenDetail;
            _httpClient = new()
            {
                BaseAddress = new Uri(_identityServerClientDefinition.Authority)
            };
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string accessToken = await RequestTokenAsync();

            request.SetBearerToken(accessToken);

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string> RequestTokenAsync()
        {
            if (string.IsNullOrEmpty(_identityServerTokenDetail.AccessToken) || _identityServerTokenDetail.RefreshDate < DateTime.Now)
            {
                await GetTokenFromEndpointAsync();
            }

            if (!string.IsNullOrEmpty(_identityServerTokenDetail.AccessToken))
            {
                return _identityServerTokenDetail.AccessToken;
            }
            else
            {
                throw new HttpRequestException(TokenErrorConstant.TokenRequestError);
            }
        }

        private async Task GetTokenFromEndpointAsync()
        {
            if (_identityServerClientDefinition.GrantType == GrantTypes.ClientCredentials)
            {
                ClientCredentialsTokenRequest clientCredentialsTokenRequest = new()
                {
                    Address = _identityServerClientDefinition.Authority + "connect/token",
                    ClientId = _identityServerClientDefinition.ClientId,
                    ClientSecret = _identityServerClientDefinition.ClientSecret,
                    GrantType = _identityServerClientDefinition.GrantType
                };

                IdentityModel.Client.TokenResponse tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest);
                if (tokenResponse.IsError)
                {
                    throw new HttpRequestException(TokenErrorConstant.TokenRequestError);
                }

                _identityServerTokenDetail.AccessToken = tokenResponse.AccessToken;
                _identityServerTokenDetail.ClientId = _identityServerClientDefinition.ClientId;
                _identityServerTokenDetail.RefreshDate = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn);

                if (tokenResponse.ExpiresIn >= 60)
                {
                    _identityServerTokenDetail.RefreshDate = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn - 60);
                }
            }
            else
            {
                throw new HttpRequestException(TokenErrorConstant.NotSupportedGrantType);
            }
        }
    }
}
