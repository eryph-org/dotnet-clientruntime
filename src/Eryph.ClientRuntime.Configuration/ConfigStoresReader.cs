﻿using System;
using System.Collections.Generic;
using System.Linq;
using Eryph.IdentityModel.Clients;
using JetBrains.Annotations;

namespace Eryph.ClientRuntime.Configuration
{
    [PublicAPI]
    public class ConfigStoresReader
    {
        private readonly string _configName;
        private readonly IEnvironment _environment;
        private readonly IEnumerable<ConfigStore> _stores;

        public ConfigStoresReader(IEnvironment environment, string configName = ConfigurationNames.Default) : this(
            GetStores(environment, configName), environment, configName)
        {

        }

        public ConfigStoresReader(IEnumerable<ConfigStore> stores, IEnvironment environment, string configName = ConfigurationNames.Default)
        {
            _configName = configName;
            _environment = environment;
            _stores = stores.Where(x=>x.Exists).ToArray();
        }

        public ClientCredentials GetClientCredentials(string clientId)
        {
            var endpointLookup = new EndpointLookup(_environment);
            var identityEndpoint = endpointLookup.GetEndpoint(EndpointNames.Identity, _configName);

            if (identityEndpoint != null)
                return _stores.SelectMany(
                        x => x.GetClientCredentials(
                            x.GetClients().Where(c => c.Id == clientId), identityEndpoint))
                    .FirstOrDefault();
            
            switch (_configName)
            {
                case ConfigurationNames.Local: throw new InvalidOperationException(
                    "Could not find running local identity endpoint. Make sure that the eryph identity process is running.");
                case ConfigurationNames.Zero:
                    throw new InvalidOperationException(
                        "Could not find running eryph zero endpoint. Make sure that the eryph zero process is running.");
                default:
                    throw new InvalidOperationException("Could not find identity endpoint in configuration.");
            }
        }

        public ClientData GetDefaultClient()
        {
            var defaultClientId =
                _stores.Select(x => x.GetDefaultClientId()).FirstOrDefault(x => !string.IsNullOrEmpty(x));
            return GetClientById(defaultClientId);
        }

        public IEnumerable<ClientData> GetClients()
        {
            return _stores.Where(x => x.Exists).Select(x => new ClientsByStore
            {
                StoreLocation = x.Location,
                Clients = x.GetClients()
            })
                .OrderBy(x=>x.StoreLocation)
                .SelectMany(x=>x.Clients)
                .Distinct(new ClientsComparer())
                .ToArray();
        }


        public ClientData GetClientById(string clientId)
        {
            return GetClients().FirstOrDefault(x => x.Id == clientId);
        }

        public ClientData GetClientByName(string clientName)
        {
            return GetClients().FirstOrDefault(x => x.Name == clientName);
        }

        public IReadOnlyDictionary<string, Uri> GetEndpoints()
        {
            return _stores.SelectMany(x => x.Endpoints)
                .GroupBy(x => x.Key)
                .ToDictionary(group => group.Key,
                    group => group.First().Value);

        }

        
        private static IEnumerable<ConfigStore> GetStores(IEnvironment environment, [NotNull] string configName = ConfigurationNames.Default)
        {
            if (configName == null) throw new ArgumentNullException(nameof(configName));

            yield return ConfigStore.GetStore(ConfigStoreLocation.CurrentDirectory, environment, configName);
            yield return ConfigStore.GetStore(ConfigStoreLocation.User, environment, configName);
            yield return ConfigStore.GetStore(ConfigStoreLocation.System, environment, configName);

        }

        private class ClientsByStore
        {
            public ConfigStoreLocation StoreLocation { get; set; }
            public IEnumerable<ClientData> Clients { get; set; }

        }

        private class ClientsComparer : IEqualityComparer<ClientData>
        {
            public bool Equals(ClientData x, ClientData y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null) return false;
                if (y is null) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Id == y.Id;
            }

            public int GetHashCode(ClientData obj)
            {
                return (obj.Id != null ? obj.Id.GetHashCode() : 0);
            }
        }
    }


}