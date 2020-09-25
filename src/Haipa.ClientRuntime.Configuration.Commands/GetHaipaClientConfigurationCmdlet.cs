using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Haipa.IdentityModel.Clients;

namespace Haipa.ClientRuntime.Configuration
{
    [Cmdlet(VerbsCommon.Get,"HaipaClientConfiguration", DefaultParameterSetName = "id")]
    [OutputType(typeof(ClientData))]
    public class GetHaipaClientConfigurationCmdlet : ConfigurationCmdlet
    {
        [Parameter(
            Position = 0,
            ParameterSetName = "id",
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public string[] Id { get; set; }
        
        [Parameter(
            Position = 0,
            Mandatory = true,
            ParameterSetName = "default")]
        public SwitchParameter Default { get; set; }


        protected override void ProcessRecord()
        {
            var configurationNames = new List<string>
            {
                ConfigurationNames.Default, ConfigurationNames.Local, ConfigurationNames.Zero
            };

            if (Configuration != null)
            {
                configurationNames.Clear();
                configurationNames.Add(Configuration);
            }

            foreach (var configurationName in configurationNames)
            {
                var storesReader = new ConfigStoresReader(new PowershellEnvironment(SessionState), configurationName);
                var defaultClient = storesReader.GetDefaultClient();

                HaipaClientConfiguration ToOutput(ClientData clientData)
                {
                    return new HaipaClientConfiguration
                    {
                        Id = clientData.Id,
                        Name = clientData.Name,
                        Configuration = configurationName,
                        IsDefault = clientData.Id == defaultClient?.Id
                    };
                }


                if (Default == true)
                {
                    if (defaultClient != null)
                        WriteObject(defaultClient);
                    return;
                }

                if (Id == null)
                {
                    WriteObject(storesReader.GetClients().Select(ToOutput), true);
                }
                else
                {
                    WriteObject(Id.Select(x => storesReader.GetClientById(x)).Where(x => x != null)
                        .Select(ToOutput), true);
                }
            }
        }


    }
}
