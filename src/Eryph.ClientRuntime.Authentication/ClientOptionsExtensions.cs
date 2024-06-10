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

            return options.BuildHttpPipeline(tokenCredential, scope);
        }

        public static HttpPipeline BuildHttpPipeline(this ClientOptions options, IEnvironment sysEnv, string scope)
        {
            var tokenCredential = new EryphTokenCredential(
                new ClientCredentialsLookup(sysEnv).GetDefaultCredentials());

            return options.BuildHttpPipeline(tokenCredential, scope);
        }

        private static HttpPipeline BuildHttpPipeline(
            this ClientOptions options,
            TokenCredential tokenCredential,
            string scope)
        {
            var authPolicy = new BearerTokenAuthenticationPolicy(tokenCredential, scope);

            var pipelineOptions = new HttpPipelineOptions(options)
            {
                RequestFailedDetailsParser = new EryphRequestFailedDetailsParser(),
                ResponseClassifier = new ResponseClassifier()
            };
            pipelineOptions.PerRetryPolicies.Add(authPolicy);

            return HttpPipelineBuilder.Build(pipelineOptions);
        }
    }
}
