using System.Collections.Generic;
using System.Threading.Tasks;
using Eryph.IdentityModel.Clients;

namespace Eryph.ClientRuntime.Authentication
{
    /// <summary>
    /// Abstraction for authentication provider
    /// </summary>
    public interface IApplicationAuthenticationProvider
    {
        /// <summary>
        /// Retrieve ClientCredentials for an application.
        /// </summary>
        /// <param name="audience">The audience to target</param>
        /// <param name="scopes">The requested scopes</param>
        /// <returns>authentication result which can be used for authentication with the given audience.</returns>
        Task<AccessTokenResponse> AuthenticateAsync(string audience, IEnumerable<string> scopes);
    }
}
