using System.Collections.Generic;
using System.Threading.Tasks;
using Eryph.IdentityModel.Clients;

namespace Eryph.ClientRuntime.Authentication
{
    public class EryphClientAuthenticationProvider : IApplicationAuthenticationProvider
    {
        private readonly ClientCredentials _clientCredentials;

        public EryphClientAuthenticationProvider(ClientCredentials credentials)
        {
            _clientCredentials = credentials;
            
        }

        public Task<AccessTokenResponse> AuthenticateAsync(string audience, IEnumerable<string> scopes)
        {
            return _clientCredentials.GetAccessToken(scopes);
        }
    }
}