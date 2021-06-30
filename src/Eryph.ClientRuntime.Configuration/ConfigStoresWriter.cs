using System;
using Eryph.IdentityModel.Clients;
using JetBrains.Annotations;

namespace Eryph.ClientRuntime.Configuration
{
    [PublicAPI]
    public class ConfigStoresWriter
    {
        private readonly ConfigStoresWriterSettings _settings;


        public ConfigStoresWriter(IEnvironment environment, string configName) : this(ConfigStoresWriterSettings.DefaultSettings(environment, configName))
        {
        }

        public ConfigStoresWriter(ConfigStoresWriterSettings settings)
        {
            _settings = settings;
        }


        public void SetDefaultClient(ClientData client)
        {
            _settings.DefaultsStore.SetDefaultClientId(client?.Id);
        }

        public string GetDefaultClientId()
        {
            return _settings.DefaultsStore.GetDefaultClientId();
        }

        public void AddClient([NotNull] ClientData client)
        {
            _settings.ClientsStore.AddClient(client);
        }

        public void AddClientCredentials(ClientCredentials credentials)
        {
            _settings.ClientsStore.AddClientCredentials(credentials, _settings.EndpointsStore);
        }

        public void RemoveClient([NotNull] string clientId)
        {
            _settings.ClientsStore.RemoveClient(clientId);
        }

        public void AddEndpoint(string endpointName, Uri endpoint)
        {
            _settings.EndpointsStore.SetEndpoint(endpointName, endpoint);
        }


    }
}