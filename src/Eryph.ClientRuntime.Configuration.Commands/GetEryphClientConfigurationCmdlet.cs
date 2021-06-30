using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Eryph.IdentityModel.Clients;
using JetBrains.Annotations;

namespace Eryph.ClientRuntime.Configuration
{
    [PublicAPI]
    [Cmdlet(VerbsCommon.Get,"EryphClientConfiguration", DefaultParameterSetName = "id")]
    [OutputType(typeof(ClientData))]
    public class GetEryphClientConfigurationCmdlet : ConfigurationCmdlet
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

                EryphClientConfiguration Output(ClientData d) => ToOutput(d, configurationName);

                if (Default == true)
                {
                    if (defaultClient != null)
                        WriteObject(defaultClient);
                    return;
                }

                if (Id == null)
                {
                    WriteObject(storesReader.GetClients().Select(Output), true);
                }
                else
                {
                    WriteObject(Id.Select(x => storesReader.GetClientById(x)).Where(x => x != null)
                        .Select(Output), true);
                }
            }
        }


    }
}
