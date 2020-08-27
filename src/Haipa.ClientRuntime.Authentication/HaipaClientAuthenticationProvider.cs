using System;
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
            using (var httpClient = new HttpClient{BaseAddress = new Uri(audience)})
                return httpClient.GetClientAccessToken(_haipaClient.Id, _haipaClient.KeyPair.ToRSAParameters(), scopes);
        }
    }
}