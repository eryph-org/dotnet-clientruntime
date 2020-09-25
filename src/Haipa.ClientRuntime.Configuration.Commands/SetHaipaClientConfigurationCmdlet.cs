using System;
using System.Management.Automation;
using Haipa.IdentityModel.Clients;
using JetBrains.Annotations;

namespace Haipa.ClientRuntime.Configuration
{
    [Cmdlet(VerbsCommon.Set, "HaipaClientConfiguration")]
    [OutputType(typeof(ClientData))]
    [UsedImplicitly]
    public class SetHaipaClientConfigurationCmdlet : ConfigurationCmdlet
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
        public string Name { get; set; }


        [Parameter(
            ValueFromPipelineByPropertyName = true)]
        public bool? IsDefault { get; set; }

        [Parameter]
        [ValidateNotNull]
        public ClientCredentials Credentials { get; set; }

        protected override void ProcessRecord()
        {
            var storesReader = GetStoresReader();

            var currentClientData = storesReader.GetClientById(Id);

            if (currentClientData == null)
            {
                throw new InvalidOperationException($"Client with id '{Id}' not found in configuration '{Configuration}'.");
            }

            var storesWriter = GetStoresWriter();

            if (IsDefault.HasValue)
            {
                if(storesReader.GetDefaultClient()?.Id == Id && !IsDefault.Value)
                    storesWriter.SetDefaultClient(null);
                else
                {
                    if (IsDefault.Value)
                        storesWriter.SetDefaultClient(currentClientData);
                }
            }
            
            if (!string.IsNullOrEmpty(Name))
            {
                var newName = Name ?? currentClientData.Name;

                var clientData = new ClientData(Id, newName);
                storesWriter.AddClient(clientData);
            }

            if (Credentials != null)
            {
                var newCredentials = new ClientCredentials(Id, Credentials.KeyPairData, Credentials.IdentityProvider, GetConfigurationName());
                storesWriter.AddClientCredentials(newCredentials);
            }

        }
    }
}