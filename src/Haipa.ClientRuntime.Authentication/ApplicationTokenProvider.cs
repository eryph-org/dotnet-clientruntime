﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Haipa.ClientRuntime.Configuration;
using Haipa.IdentityModel.Clients;
using Microsoft.Rest;

namespace Haipa.ClientRuntime.Authentication
{
    public class ApplicationTokenProvider : ITokenProvider
    {
        private IApplicationAuthenticationProvider _authenticationProvider;
        private string _accessToken;
        private DateTimeOffset _expiration;
        private string _tokenAudience;
        private IEnumerable<string> _scopes;
        private static readonly TimeSpan ExpirationThreshold = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Create an application token provider that can retrieve tokens for the given application using the given audience, scopes
        /// and a Haipa client.
        /// </summary>
        /// <param name="credentials">The Haipa client credentials.</param>
        /// <param name="tokenResponse">The token details provided when authenticating with the client.</param>
        /// <param name="scopes">the requested scopes</param>
        public ApplicationTokenProvider(ClientCredentials credentials, AccessTokenResponse tokenResponse, IEnumerable<string> scopes)
        {
            Initialize(credentials.IdentityProvider.ToString(), scopes, tokenResponse.AccessToken, tokenResponse.ExpiresOn.GetValueOrDefault(),
                new HaipaClientAuthenticationProvider(credentials));
        }

        /// <summary>
        /// Gets an access token from the identity endpoint.
        /// Attempts to refresh the access token if it has expired.
        /// </summary>
        public virtual async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            try
            {
                AccessTokenResponse response;
                if (AccessTokenExpired)
                {
                    response = await _authenticationProvider.AuthenticateAsync( _tokenAudience, _scopes).ConfigureAwait(false);
                    _accessToken = response.AccessToken;
                    _expiration = response.ExpiresOn.GetValueOrDefault();
                }

                return new AuthenticationHeaderValue("Bearer", _accessToken);
            }
            catch (Exception authenticationException)
            {
                throw new AuthenticationException("Could not acquire access token.", authenticationException);
            }
        }

        protected virtual bool AccessTokenExpired => DateTime.UtcNow + ExpirationThreshold >= _expiration;

        private void Initialize(string tokenAudience, IEnumerable<string> scopes, string accessToken, DateTimeOffset tokenExpiration, IApplicationAuthenticationProvider authenticationProvider)
        {

            _tokenAudience = tokenAudience;

            _authenticationProvider = authenticationProvider;
            _scopes = scopes;
            _accessToken = accessToken;
            _expiration = tokenExpiration;
        }

        public static async Task<ServiceClientCredentials> LogonWithHaipaClient(ClientCredentials credentials, IEnumerable<string> scopes)
        {
            var scopesArray = scopes as string[] ?? scopes.ToArray();

            var accessTokenResponse =
                await credentials.GetAccessToken(scopesArray).ConfigureAwait(false);

            return new TokenCredentials(new ApplicationTokenProvider(credentials, accessTokenResponse, scopesArray));
        }

        public static Task<ServiceClientCredentials> LogonWithHaipaClient(IEnumerable<string> scopes)
        {
            var clientLookup = new ClientCredentialsLookup();
            return LogonWithHaipaClient(clientLookup.FindCredentials(), scopes);
        }
    }
}
