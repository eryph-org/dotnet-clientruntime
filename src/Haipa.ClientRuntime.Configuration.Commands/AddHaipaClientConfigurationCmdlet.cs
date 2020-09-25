using System;
using System.Management.Automation;
using Haipa.IdentityModel.Clients;
using JetBrains.Annotations;

namespace Haipa.ClientRuntime.Configuration
{
    [Cmdlet(VerbsCommon.Add, "HaipaClientConfiguration")]
    [OutputType(typeof(ClientData))]
    [UsedImplicitly]
    public class AddHaipaClientConfigurationCmdlet : ConfigurationCmdlet
    {
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global

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
        

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNull]
        public ClientCredentials[] Credentials { get; set; }

        [Parameter]
        public SwitchParameter AsDefault { get; set; }

        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper restore UnusedAutoPropertyAccessor.Global


        protected override void ProcessRecord()
        {
            var storesReader = GetStoresReader();
            if (storesReader.GetClientById(Id) != null) 
            {
                throw new InvalidOperationException($"Client with id '{Id}' already exists in configuration '{GetConfigurationName()}'.");
            }

            var storesWriter = GetStoresWriter();

            foreach (var credential in Credentials)
            {
                var clientData = new ClientData(Id, Name);
                storesWriter.AddClient(clientData);
                storesWriter.AddClientCredentials(new ClientCredentials(Id, credential.KeyPairData, credential.IdentityProvider));

                if (AsDefault.IsPresent)
                    storesWriter.SetDefaultClient(clientData);

            }
        }
    }
}