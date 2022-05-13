using System;
using System.Management.Automation;
using Eryph.ClientRuntime.Powershell;
using Eryph.IdentityModel.Clients;

namespace Eryph.ClientRuntime.Configuration
{
    [Cmdlet(VerbsCommon.Get, "EryphClientCredentials", DefaultParameterSetName = "Id")]
    [OutputType(typeof(ClientCredentials))]
    public class GetEryphClientCredentialsCmdlet : ConfigurationCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = false,
            ParameterSetName = "Id",
            ValueFromPipelineByPropertyName = true,
            ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public string[] Id { get; set; }

        [Parameter(
            Mandatory = false,
            ParameterSetName = "Name",
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(ParameterSetName = "system-client")]
        public SwitchParameter SystemClient { get; set; }

        protected override void ProcessRecord()
        {
            var lookup = new ClientCredentialsLookup(new PowershellEnvironment(SessionState));

            if (SystemClient.IsPresent)
            {
                var configName = GetConfigurationName();
                if (configName != ConfigurationNames.Local && configName != ConfigurationNames.Zero)
                {
                    throw new InvalidOperationException(
$@"The system client cannot be requested for configuration '{configName}'.
System client is only available for configurations 'zero' and 'local'.
To set the configuration use the parameter Configuration or set a default configuration with cmdlet Set-EryphConfigurationStore.");
                }

                var credentials = lookup.GetSystemClientCredentials(configName);
                if (credentials != null)
                    WriteObject(credentials);

                return;
            }

            if (Id == null)
            {
                if (Name != null)
                {
                    var credentials = lookup.GetCredentialsByClientName(Name, GetConfigurationName());

                    if (credentials != null)
                        WriteObject(credentials);

                }
                else
                {

                    var credentials = Configuration == null // search all config stores if no configuration name is set
                        ? lookup.FindCredentials()
                        : lookup.FindCredentials(Configuration);

                    if (credentials != null)
                        WriteObject(credentials);
                }
            }
            else
            {
                foreach (var singleId in Id)
                {
                    var credentials = lookup.GetCredentialsByClientId(singleId, GetConfigurationName());

                    if (credentials == null)
                        continue;

                    WriteObject(credentials);
                }
            }
        }


    }
}