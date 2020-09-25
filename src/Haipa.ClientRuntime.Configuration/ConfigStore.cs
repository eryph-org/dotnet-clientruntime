using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Haipa.IdentityModel.Clients;
using Haipa.IdentityModel.Clients.Internal;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Haipa.ClientRuntime.Configuration
{
    [PublicAPI]
    public class ConfigStore
    {
        private readonly string _configName;
        private readonly IEnvironment _environment;

        private ConfigStore(string configName, string basePath, IEnvironment environment)
        {
            _configName = configName;
            _environment = environment;

            StorePath = ".haipa";
            if (basePath != null)
                StorePath = Path.Combine(basePath, StorePath);
        }

        public string StorePath { get; }

        private ClientConfig _config;
        private object _syncRoot = new object();

        public IReadOnlyDictionary<string, Uri> Endpoints => 
            new ReadOnlyDictionary<string, Uri>(GetSettings().Endpoints??new Dictionary<string, Uri>());

        public bool Exists => _environment.FileSystem.DirectoryExists(StorePath) 
                              && _environment.FileSystem.FileExists(Path.Combine(StorePath, $"{_configName}.config"));

        public IEnumerable<ClientData> GetClients()
        {
            return from client in (GetSettings().Clients ?? new ClientData[0]) 
                select new ClientData(client.Id, client.Name);
        }

        public IEnumerable<ClientCredentials> GetClientCredentials(IEnumerable<ClientData> clients, Uri identityProvider)
        {
            return from client in clients
                let keyFileName = Path.Combine(StorePath, "private", $"{client.Id}.key")
                where _environment.FileSystem.FileExists(keyFileName)
                let keyPairData = PrivateKey.ToSecureString(PrivateKey.ReadFile(keyFileName, _environment.FileSystem))
                select new ClientCredentials(client.Id, keyPairData, identityProvider);
        }

        private ClientConfig GetSettings()
        {
            lock (_syncRoot)
            {
                var configFileName = Path.Combine(StorePath, $"{_configName}.config");
                if (_environment.FileSystem.FileExists(configFileName))
                {
                    using (var reader = _environment.FileSystem.OpenText(configFileName))
                    {
                        var configJson = reader.ReadToEnd();
                        _config = JsonConvert.DeserializeObject<ClientConfig>(configJson);
                    }
                }
                else
                {
                    _config = new ClientConfig();
                }

                return _config;
            }
        }

        
        private void SaveSettings(ClientConfig settings)
        {
            lock (_syncRoot)
            {
                if (!_environment.FileSystem.DirectoryExists(StorePath))
                    Directory.CreateDirectory(StorePath);

                var configFileName = Path.Combine(StorePath, $"{_configName}.config");

                var settingsJson = JsonConvert.SerializeObject(settings, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()

                });
                using (var writer = _environment.FileSystem.CreateText(configFileName))
                    writer.Write(settingsJson);
                _config = null;
            }
        }

        internal string GetDefaultClientId()
        {
            var settings = GetSettings();
            return settings.DefaultClientId;
        }

        internal void SetDefaultClientId(string clientId)
        {
            var settings = GetSettings();
            settings.DefaultClientId = clientId;
            SaveSettings(settings);

        }

        public void SetEndpoint(string endpointName, Uri endpoint)
        {
            var settings = GetSettings();

            settings.Endpoints = settings.Endpoints ?? new Dictionary<string, Uri>();

            if (settings.Endpoints.ContainsKey(endpointName))
                settings.Endpoints.Remove(endpointName);
            
            if(endpoint!=null)
                settings.Endpoints.Add(endpointName, endpoint);

            SaveSettings(settings);
        }

        public void RemoveClient(string clientId)
        {
            var settings = GetSettings();
            settings.Clients = settings.Clients ?? new List<ClientData>();
            settings.Clients = new List<ClientData>(settings.Clients.Where(x => x.Id != clientId));

            SaveSettings(settings);


            var privatePath = Path.Combine(StorePath, "private");

            var privateKeyPath = Path.Combine(privatePath, $"{clientId}.key");
            if (_environment.FileSystem.FileExists(privateKeyPath))
                _environment.FileSystem.FileDelete(privateKeyPath);
        }

        public void AddClient([NotNull] ClientData client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            if (client.Id == "system-client")
                throw new InvalidOperationException("The system client cannot be saved to config store.");
            
            var settings = GetSettings();
            settings.Clients = settings.Clients ?? new List<ClientData>();

            var clients = new List<ClientData>(settings.Clients.Where(x => x.Id != client.Id)) {client};

            settings.Clients = clients;
            SaveSettings(settings);
        }

        public void AddClientCredentials([NotNull] ClientCredentials credentials, [NotNull] ConfigStore endpointConfigStore)
        {
            if (credentials == null) throw new ArgumentNullException(nameof(credentials));
            if (endpointConfigStore == null) throw new ArgumentNullException(nameof(endpointConfigStore));

            if (string.IsNullOrWhiteSpace(credentials.Id))
                throw new InvalidOperationException("Id of credentials cannot be empty.");

            if (string.IsNullOrWhiteSpace(credentials.IdentityProvider?.ToString()))
                throw new InvalidOperationException("IdentityProvider of credentials cannot be empty.");

            if (credentials.KeyPairData == null)
                throw new InvalidOperationException("KeyPairData of credentials cannot be empty.");

            if (credentials.Id == "system-client")
                throw new InvalidOperationException("The system client cannot be saved to config store.");
            
            if (endpointConfigStore._configName != ConfigurationNames.Zero 
                && endpointConfigStore._configName != ConfigurationNames.Local)
            {
                endpointConfigStore.Endpoints.TryGetValue(EndpointNames.Identity, out var currentEndpoint);

                if (currentEndpoint == null)
                {
                    currentEndpoint = credentials.IdentityProvider;
                    endpointConfigStore.SetEndpoint(EndpointNames.Identity, currentEndpoint);
                }

                if (credentials.IdentityProvider != currentEndpoint)
                    throw new InvalidOperationException($"This credentials have been issued by identity provider '{credentials.IdentityProvider}', but the configuration '{_configName}' uses the provider '{currentEndpoint}'.");

            }

            var privatePath = Path.Combine(StorePath, "private");

            var privateKeyPath = Path.Combine(privatePath, $"{credentials.Id}.key");
            if (!_environment.FileSystem.DirectoryExists(privatePath))
                _environment.FileSystem.CreateDirectory(privatePath);


            var keyPairPtr = Marshal.SecureStringToGlobalAllocUnicode(credentials.KeyPairData);

            try
            {
                var keyPair = PrivateKey.ReadString(Marshal.PtrToStringUni(keyPairPtr));
                PrivateKey.WriteFile(privateKeyPath, keyPair,
                    _environment.FileSystem);

            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(keyPairPtr);
            }
        }

        public static ConfigStore GetStore(ConfigStoreLocation location, IEnvironment environment, [NotNull] string configName = ConfigurationNames.Default)
        {
            if (configName == null) throw new ArgumentNullException(nameof(configName));

            string basePath;
            switch (location)
            {
                case ConfigStoreLocation.CurrentDirectory:
                    basePath = environment.GetCurrentDirectory();
                    break;
                case ConfigStoreLocation.User:
                    basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    break;
                case ConfigStoreLocation.System:
                    basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(location), location, null);
            }

            return new ConfigStore(configName, basePath, environment);
        }
    }
}

