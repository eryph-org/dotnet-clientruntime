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
            using (var httpClient = new HttpClient {BaseAddress = new Uri(audience)})
            {
                var keyPairPtr = Marshal.SecureStringToGlobalAllocUnicode(_clientCredentials.KeyPairData);
                try
                {
                    var keyPair = IdentityModel.Clients.Internal.PrivateKey.ReadString(Marshal.PtrToStringUni(keyPairPtr));
                    return httpClient.GetClientAccessToken(_clientCredentials.Id,
                        keyPair.ToRSAParameters(), scopes);
                }
                finally
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(keyPairPtr);
                }
            }
        }
    }
}