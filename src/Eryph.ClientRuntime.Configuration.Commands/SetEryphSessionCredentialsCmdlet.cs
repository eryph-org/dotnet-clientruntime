using System.Management.Automation;
using Eryph.IdentityModel.Clients;
using JetBrains.Annotations;

namespace Eryph.ClientRuntime.Configuration
{
    [PublicAPI]
    [Cmdlet(VerbsCommon.Set, "EryphSessionCredentials")]
    public class SetEryphSessionCredentialsCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNull]
        public ClientCredentials Credentials { get; set; }

        protected override void ProcessRecord()
        {
            SessionState.PSVariable.Set("EryphSessionCredentials", Credentials);
            WriteVerbose($"Session credentials set for client: {Credentials.Id}");
        }
    }
}