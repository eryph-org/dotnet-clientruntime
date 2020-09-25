using System.Management.Automation;
using JetBrains.Annotations;

namespace Haipa.ClientRuntime.Configuration
{
    [Cmdlet(VerbsCommon.Remove, "HaipaClientConfiguration")]
    [UsedImplicitly]
    public class RemoveHaipaClientConfigurationCmdlet : ConfigurationCmdlet
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

        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper restore UnusedAutoPropertyAccessor.Global


        protected override void ProcessRecord()
        {
            var storesWriter = GetStoresWriter();
            storesWriter.RemoveClient(Id);
            if(storesWriter.GetDefaultClientId()==Id)
                storesWriter.SetDefaultClient(null);

        }
    }
}