using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Haipa.IdentityModel.Clients;

namespace Haipa.ClientRuntime.Authentication
{
    public class HaipaClientAuthenticationProvider : IApplicationAuthenticationProvider
    {
        private readonly ClientCredentials _clientCredentials;

        public HaipaClientAuthenticationProvider(ClientCredentials credentials)
        {
            _clientCredentials = credentials;
            
        }

        public Task<AccessTokenResponse> AuthenticateAsync(string audience, IEnumerable<string> scopes)
        {
            return _clientCredentials.GetAccessToken(scopes);
        }
    }
}