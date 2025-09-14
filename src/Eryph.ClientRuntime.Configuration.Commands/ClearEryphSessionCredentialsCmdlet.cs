using System.Management.Automation;
using JetBrains.Annotations;

namespace Eryph.ClientRuntime.Configuration
{
    [PublicAPI]
    [Cmdlet(VerbsCommon.Clear, "EryphSessionCredentials")]
    public class ClearEryphSessionCredentialsCmdlet : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            var variable = SessionState.PSVariable.Get("EryphSessionCredentials");
            if (variable != null)
            {
                SessionState.PSVariable.Remove("EryphSessionCredentials");
                WriteVerbose("Session credentials cleared");
            }
            else
            {
                WriteVerbose("No session credentials were set");
            }
        }
    }
}