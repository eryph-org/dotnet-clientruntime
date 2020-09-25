using System;
using System.Management.Automation;
using JetBrains.Annotations;

namespace Haipa.ClientRuntime.Configuration
{
    [Cmdlet(VerbsCommon.Set, "HaipaConfigurationStore", DefaultParameterSetName = "All")]
    [UsedImplicitly]
    public class SetHaipaConfigurationStoreCmdlet : ConfigurationCmdlet
    {
        [Parameter(Position = 0, ParameterSetName = "All")]
        [ValidateSet("CurrentDirectory", "System", "User")]
        [ValidateNotNullOrEmpty]
        public string All { get; set; }
        

        [Parameter(ParameterSetName = "Single")]
        [ValidateSet("CurrentDirectory", "System", "User")]
        [ValidateNotNullOrEmpty]
        public string Clients { get; set; }

        [Parameter(ParameterSetName = "Single")]
        [ValidateSet("CurrentDirectory", "System", "User")]
        [ValidateNotNullOrEmpty]
        public string Endpoints { get; set; }

        [Parameter(ParameterSetName = "Single")]
        [ValidateSet("CurrentDirectory", "System", "User")]
        [ValidateNotNullOrEmpty]
        public string Defaults { get; set; }


        protected override void ProcessRecord()
        {
            var settings = GetStoreSettings();

            if (All != null)
            {
                var location = (ConfigStoreLocation) Enum.Parse(typeof(ConfigStoreLocation), All);
                settings.Clients = location;
                settings.Endpoints = location;
                settings.Defaults = location;
            }

            if (Clients != null)
            {
                var location = (ConfigStoreLocation)Enum.Parse(typeof(ConfigStoreLocation), Clients);
                settings.Clients = location;
            }

            if (Endpoints != null)
            {
                var location = (ConfigStoreLocation)Enum.Parse(typeof(ConfigStoreLocation), Endpoints);
                settings.Endpoints = location;
            }

            if (Defaults != null)
            {
                var location = (ConfigStoreLocation)Enum.Parse(typeof(ConfigStoreLocation), Defaults);
                settings.Defaults = location;
            }

            if (Configuration != null)
                settings.DefaultConfigurationName = Configuration;

            SessionState.PSVariable.Set("HaipaStoreLocationSettings", settings);

        }

    }
}