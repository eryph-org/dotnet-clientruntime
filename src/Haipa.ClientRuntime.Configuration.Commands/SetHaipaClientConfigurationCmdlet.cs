using System;
using System.Management.Automation;
using Haipa.IdentityModel.Clients;
using JetBrains.Annotations;

namespace Haipa.ClientRuntime.Configuration
{
    [PublicAPI]
    [Cmdlet(VerbsCommon.Set, nameof(HaipaClientConfiguration), 
        SupportsShouldProcess = true)]
    [OutputType(typeof(HaipaClientConfiguration))]
    [UsedImplicitly]
    public class SetHaipaClientConfigurationCmdlet : ConfigurationCmdlet
    {
        [Parameter(
            Position = 0, 
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string[] Id { get; set; }

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

        /// <summary>
        /// This parameter indicates that the cmdlet should return
        /// an object to the pipeline after the processing has been
        /// completed.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru
        {
            get => _passThru;
            set => _passThru = value;
        }

        private bool _passThru;

        protected override void ProcessRecord()
        {
            var storesReader = GetStoresReader();

            foreach (var id in Id)
            {
                try
                {
                    var currentClientData = storesReader.GetClientById(id);

                    if (currentClientData == null)
                    {
                        WriteError(new ErrorRecord(
                            new InvalidOperationException(
                                $"Client with id '{id}' not found in configuration '{GetConfigurationName()}'."),
                            $"{nameof(HaipaClientConfiguration)}{ErrorCategory.ObjectNotFound}",
                            ErrorCategory.ObjectNotFound, id));
                        continue;
                    }

                    var storesWriter = GetStoresWriter();

                    if (IsDefault.HasValue)
                    {
                        if (storesReader.GetDefaultClient()?.Id == id && !IsDefault.Value)
                        {
                            if (ShouldProcess(id,$"Unset client as default client for configuration '{GetConfigurationName()}'."))
                                storesWriter.SetDefaultClient(null);
                        }
                        else
                        {
                            if (IsDefault.Value)
                                if (ShouldProcess(id, $"Set client as default client for configuration '{GetConfigurationName()}'."))
                                    storesWriter.SetDefaultClient(currentClientData);
                        }
                    }

                    if (!string.IsNullOrEmpty(Name))
                    {
                        var newName = Name ?? currentClientData.Name;

                        if (ShouldProcess(id,$"Set client name to '{newName}'"))
                        {
                            var clientData = new ClientData(id, newName);
                            storesWriter.AddClient(clientData);

                            currentClientData = clientData;
                        }
                    }

                    if (Credentials != null)
                    {
                        if (!ShouldProcess(id, $"Set new credentials."))
                            continue;

                        var newCredentials = new ClientCredentials(id, Credentials.KeyPairData,
                            Credentials.IdentityProvider, GetConfigurationName());
                        storesWriter.AddClientCredentials(newCredentials);
                    }

                    if(!_passThru) continue;

                    WriteObject(ToOutput(currentClientData, GetConfigurationName()));

                }
                catch (InvalidOperationException ex)
                {
                    WriteError(new ErrorRecord(ex,
                        $"{nameof(HaipaClientConfiguration)}{ErrorCategory.InvalidOperation}",
                        ErrorCategory.InvalidOperation, id));
                }
            }

        }
    }
}