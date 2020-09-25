using Haipa.IdentityModel.Clients;
using JetBrains.Annotations;

namespace Haipa.ClientRuntime.Configuration
{
    [PublicAPI]
    public class ConfigStoresWriterSettings
    {
        public ConfigStore DefaultsStore { get; set;  }
        public ConfigStore ClientsStore { get; set; }
        public ConfigStore EndpointsStore { get; set; }
        public IEnvironment Environment { get; set; }
        public string ConfigurationName { get; set; }


        public static ConfigStoresWriterSettings DefaultSettings(IEnvironment environment, string configName)
        {
            var userStore = ConfigStore.GetStore(ConfigStoreLocation.User, environment, configName);
            return new ConfigStoresWriterSettings
            {
                EndpointsStore = userStore,
                ClientsStore = userStore,
                DefaultsStore = userStore,
                Environment = environment, 
                ConfigurationName = configName
            };
        }

    }
}