using Azure.Core;
using Azure.Core.Pipeline;
using Eryph.ClientRuntime.Configuration;
using Eryph.IdentityModel.Clients;

namespace Eryph.ClientRuntime.Authentication
{
    public static class ClientOptionsExtensions
    {

        public static HttpPipeline BuildHttpPipeline(this ClientOptions options, ClientCredentials credentials, string scope)
        {
            var tokenCredential = new EryphTokenCredential(credentials);

            var authPolicy = new BearerTokenAuthenticationPolicy(tokenCredential, scope);
            return HttpPipelineBuilder.Build(options, authPolicy);
        }

        public static HttpPipeline BuildHttpPipeline(this ClientOptions options, IEnvironment sysEnv, string scope)
        {
            var tokenCredential = new EryphTokenCredential(
                new ClientCredentialsLookup(sysEnv).GetDefaultCredentials());

            var authPolicy = new BearerTokenAuthenticationPolicy(tokenCredential, scope);
            return HttpPipelineBuilder.Build(options, authPolicy);
        }
    }
}