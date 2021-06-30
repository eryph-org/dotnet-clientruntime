using System;
using System.Collections.Generic;
using System.Linq;
using Eryph.IdentityModel.Clients;
using JetBrains.Annotations;

namespace Eryph.ClientRuntime.Configuration
{
    [PublicAPI]
    public sealed class EndpointLookup
    {
        private readonly IEnvironment _systemEnvironment;

        public EndpointLookup(IEnvironment systemEnvironment = null)
        {
            _systemEnvironment = systemEnvironment ?? new DefaultEnvironment();
        }

        public Uri GetEndpoint(string endpointName, string configName = ConfigurationNames.Default)
        {
            var endpoint =  new ConfigStoresReader(_systemEnvironment, configName).GetEndpoints()
                .Concat(GetLocalEndpoints(configName))
                .Where(x => x.Key.Equals(endpointName))
                .Select(x=>x.Value).FirstOrDefault();

            return endpoint;
        }

        private IReadOnlyDictionary<string, Uri> GetLocalEndpoints(string configName = ConfigurationNames.Local)
        {
            if (configName != ConfigurationNames.Zero && configName != ConfigurationNames.Local)
                return new Dictionary<string, Uri>();

            var identityInfo = configName == ConfigurationNames.Zero
                ? new EryphZeroInfo(_systemEnvironment)
                : new LocalIdentityProviderInfo(_systemEnvironment);

            return identityInfo.Endpoints;
        }
    }
    

}