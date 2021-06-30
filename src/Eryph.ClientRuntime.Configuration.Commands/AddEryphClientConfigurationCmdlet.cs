using System;
using System.Management.Automation;
using Eryph.IdentityModel.Clients;
using JetBrains.Annotations;

namespace Eryph.ClientRuntime.Configuration
{
    [PublicAPI]
    [Cmdlet(VerbsCommon.Add, nameof(EryphClientConfiguration), 
        SupportsShouldProcess = true)]
    [OutputType(typeof(EryphClientConfiguration))]
    [UsedImplicitly]
    public class AddEryphClientConfigurationCmdlet : ConfigurationCmdlet
    {
        [Parameter(
            Position = 0, 
            Mandatory = true,
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Id { get; set; }

        [Parameter(
            Position = 1,
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }
        
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true)]
        [ValidateNotNull]
        public ClientCredentials[] Credentials { get; set; }

        [Parameter] 
        public SwitchParameter AsDefault { get; set; }

        protected override void ProcessRecord()
        {
            var storesReader = GetStoresReader();
            foreach (var credentials in Credentials)
            {
                try
                {
                    if (storesReader.GetClientById(Id) != null)
                    {
                        WriteError(new ErrorRecord(
                            new InvalidOperationException(
                                $"Client with id '{Id}' already exists in configuration '{GetConfigurationName()}'."),
                            $"{nameof(EryphClientConfiguration)}{ErrorCategory.ResourceExists}",
                            ErrorCategory.ResourceExists, Id));
                        continue;
                    }

                    var storesWriter = GetStoresWriter();

                    var clientData = new ClientData(Id, Name);
                    storesWriter.AddClient(clientData);
                    storesWriter.AddClientCredentials(new ClientCredentials(Id, credentials.KeyPairData,
                        credentials.IdentityProvider, GetConfigurationName()));

                    if (AsDefault.IsPresent)
                        storesWriter.SetDefaultClient(clientData);

                    WriteObject(ToOutput(clientData, GetConfigurationName()));
                }
                catch (InvalidOperationException ex)
                {
                    WriteError(new ErrorRecord(ex,
                        $"{nameof(EryphClientConfiguration)}{ErrorCategory.InvalidOperation}",
                        ErrorCategory.InvalidOperation, Id));
                }
            }
        }
    }
}