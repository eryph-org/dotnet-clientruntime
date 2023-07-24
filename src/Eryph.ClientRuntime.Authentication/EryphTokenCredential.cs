using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Eryph.IdentityModel.Clients;

namespace Eryph.ClientRuntime.Authentication
{
    internal class EryphTokenCredential : TokenCredential
    {
        private readonly ClientCredentials _clientCredentials;
        private readonly HttpClient _httpClient;

        public EryphTokenCredential(ClientCredentials clientCredentials, HttpClient httpClient = null)
        {
            _clientCredentials = clientCredentials;
            _httpClient = httpClient;
        }

        public override async ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            var response = await _clientCredentials.GetAccessToken(requestContext.Scopes.ToArray(), _httpClient);
            return new AccessToken(response.AccessToken, response.ExpiresOn.GetValueOrDefault());
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return GetTokenAsync(requestContext, cancellationToken).GetAwaiter().GetResult();
        }

    }
}