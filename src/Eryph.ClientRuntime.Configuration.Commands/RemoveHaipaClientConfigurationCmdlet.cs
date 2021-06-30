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
        /// the cmdlet to stop its operation. This parameter should always
        /// be used with caution.
        /// </summary>
        [Parameter]
        public SwitchParameter Force
        {
            get => _force;
            set => _force = value;
        }

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

        private bool _yesToAll, _noToAll;
        private bool _passThru;
        private bool _force;

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
                            ErrorCategory.ObjectNotFound, Id));
                        continue;
                    }
                    
                    var storesWriter = GetStoresWriter();
                    if (!ShouldProcess(id)) continue;

                    if (!ShouldContinue($"The client with {id} will be deleted permanently.", "Warning!",
                        ref _yesToAll, ref _noToAll))
                    {
                        continue;
                    }

                    storesWriter.RemoveClient(id);
                    if (storesWriter.GetDefaultClientId() == id)
                        storesWriter.SetDefaultClient(null);

                    if(_passThru)
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