using System;
using System.Management.Automation;
using JetBrains.Annotations;

namespace Eryph.ClientRuntime.Configuration
{
    [PublicAPI]
    [Cmdlet(VerbsCommon.Remove, nameof(EryphClientConfiguration), 
        SupportsShouldProcess = true)]
    [UsedImplicitly]
    [OutputType(typeof(EryphClientConfiguration))]
    public class RemoveEryphClientConfigurationCmdlet : ConfigurationCmdlet
    {
        [Parameter(
            Position = 0, 
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string[] Id { get; set; }

        /// <summary>
        /// This parameter overrides the ShouldContinue call to force
        /// the configuration to be removed. This parameter should always
        /// be used with caution.
        /// </summary>
        [Parameter]
        public SwitchParameter Force { get; set; }

        /// <summary>
        /// This parameter indicates that the cmdlet should return
        /// an object to the pipeline after the processing has been
        /// completed.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru { get; set; }

        private bool _yesToAll, _noToAll;

        protected override void BeginProcessing()
        {
        }

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
                        WriteError(new ErrorRecord(new InvalidOperationException($"Client with id '{id}' not found in configuration '{GetConfigurationName()}'."),
                            $"{nameof(EryphClientConfiguration)}{ErrorCategory.ObjectNotFound}",
                            ErrorCategory.ObjectNotFound, id));
                        continue;
                    }
                    
                    var storesWriter = GetStoresWriter();
                    if (!ShouldProcess(id)) continue;

                    if (!Force && !ShouldContinue($"The client with {id} will be deleted permanently.", "Warning!", ref _yesToAll, ref _noToAll))
                    {
                        continue;
                    }

                    storesWriter.RemoveClient(id);
                    if (storesWriter.GetDefaultClientId() == id)
                        storesWriter.SetDefaultClient(null);

                    if (PassThru)
                        WriteObject(ToOutput(currentClientData, GetConfigurationName()));
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