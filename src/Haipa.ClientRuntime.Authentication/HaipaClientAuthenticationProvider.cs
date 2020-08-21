using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Haipa.IdentityModel.Clients;

namespace Haipa.ClientRuntime.Authentication
{
    public class HaipaClientAuthenticationProvider : IApplicationAuthenticationProvider
    {
        private readonly ClientData _haipaClient;

        public HaipaClientAuthenticationProvider(ClientData haipaClient)
        {
            _haipaClient = haipaClient;
        }

        public Task<AccessTokenResponse> AuthenticateAsync(string audience, IEnumerable<string> scopes)
        {
            using(var httpClient = new HttpClient())
                return _haipaClient.GetAccessToken(audience, scopes, httpClient);
        }
    }
}