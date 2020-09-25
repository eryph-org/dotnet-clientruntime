namespace Haipa.ClientRuntime.Configuration
{
    public class ConfigStoreSettings
    {
        public ConfigStoreLocation Clients { get; set; } = ConfigStoreLocation.User;
        public ConfigStoreLocation Endpoints { get; set; } = ConfigStoreLocation.User;
        public ConfigStoreLocation Defaults { get; set; } = ConfigStoreLocation.User;

        public string DefaultConfigurationName { get; set; } = ConfigurationNames.Default;
    }
}