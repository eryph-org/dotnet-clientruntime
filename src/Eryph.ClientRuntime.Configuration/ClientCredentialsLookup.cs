using System;
using System.Runtime.InteropServices;
using Eryph.IdentityModel.Clients;
using JetBrains.Annotations;

namespace Eryph.ClientRuntime.Configuration
{
    [PublicAPI]
    public sealed class ClientCredentialsLookup
    {
        private readonly IEnvironment _systemEnvironment;

        public ClientCredentialsLookup(IEnvironment systemEnvironment = null)
        {
            _systemEnvironment = systemEnvironment ?? new DefaultEnvironment();
        }

        [CanBeNull]
        public ClientCredentials FindCredentials()
        {
            return _systemEnvironment.IsOsPlatform(OSPlatform.Windows) 
                ? FindCredentials(ConfigurationNames.Default, ConfigurationNames.Zero, ConfigurationNames.Local) 
                : FindCredentials(ConfigurationNames.Default, ConfigurationNames.Local);
        }

        [CanBeNull]
        public ClientCredentials FindCredentials([NotNull] params string[] configNames)
        {
            foreach (var configName in configNames)
            {
                ClientCredentials result;
                try
                {
                    result = GetDefaultCredentials(configName);
                    if (result != null) return result;

                }
                catch (InvalidOperationException)
                {

                }

                try
                {
                    result = GetSystemClientCredentials(configName);
                    if (result != null) return result;

                }
                catch (InvalidOperationException)
                {

                }
            }

            return null;
        }

        [CanBeNull]
        public ClientCredentials GetDefaultCredentials([NotNull] string configName = ConfigurationNames.Default)
        {
            if (configName == null) throw new ArgumentNullException(nameof(configName));

            var storesReader = new ConfigStoresReader(_systemEnvironment, configName);
            var defaultClient = storesReader.GetDefaultClient();
            return defaultClient == null ? null : storesReader.GetClientCredentials(defaultClient.Id);
        }

        [CanBeNull]
        public ClientCredentials GetCredentialsByClientName([NotNull] string clientName, [NotNull] string configName = ConfigurationNames.Default)
        {
            if (clientName == null) throw new ArgumentNullException(nameof(clientName));
            if (configName == null) throw new ArgumentNullException(nameof(configName));

            var storesReader = new ConfigStoresReader(_systemEnvironment, configName);
            var clientData = storesReader.GetClientByName(clientName);
            return clientData == null ? null : storesReader.GetClientCredentials(clientData.Id);


        }

        [CanBeNull]
        public ClientCredentials GetCredentialsByClientId([NotNull] string clientId, [NotNull] string configName = ConfigurationNames.Default)
        {
            if (clientId == null) throw new ArgumentNullException(nameof(clientId));
            if (configName == null) throw new ArgumentNullException(nameof(configName));

            var storesReader = new ConfigStoresReader(_systemEnvironment, configName);
            var clientData = storesReader.GetClientById(clientId);
            return clientData == null ? null : storesReader.GetClientCredentials(clientData.Id);
        }

        [CanBeNull]
        public ClientCredentials GetSystemClientCredentials(string configName = ConfigurationNames.Local)
        {
            if(configName != ConfigurationNames.Zero && configName != ConfigurationNames.Local)
                throw new InvalidOperationException($"The system client is not supported for configuration '{configName}.");

            if (!_systemEnvironment.IsOsPlatform(OSPlatform.Windows) &&
                !_systemEnvironment.IsOsPlatform(OSPlatform.Linux))
                throw new InvalidOperationException("The system client exists only on Windows and Linux systems.");

            if (!_systemEnvironment.IsOsPlatform(OSPlatform.Windows) && configName == ConfigurationNames.Zero)
                throw new InvalidOperationException("The system client for eryph zero exists only on Windows.");

            if (_systemEnvironment.IsOsPlatform(OSPlatform.Windows) && !_systemEnvironment.IsWindowsAdminUser)
                throw new InvalidOperationException(
                    "This application has to be started as admin to access the eryph system client. ");

            
            var identityInfo = configName == ConfigurationNames.Zero
                ? new EryphZeroInfo(_systemEnvironment)
                : new LocalIdentityProviderInfo(_systemEnvironment);

            if (!identityInfo.IsRunning) return null;

            var identityEndpoint = !identityInfo.Endpoints.TryGetValue(EndpointNames.Identity, out var endpoint)
                ? throw new InvalidOperationException("could not find identity endpoint for system-client")
                : endpoint;

            return new ClientCredentials("system-client", 
                identityInfo.GetSystemClientPrivateKey(),
                identityEndpoint, configName);

        }

    }
}