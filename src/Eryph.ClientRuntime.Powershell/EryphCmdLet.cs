using System;
using System.Linq;
using System.Management.Automation;
using Eryph.ClientRuntime.Configuration;
using Eryph.IdentityModel.Clients;
using JetBrains.Annotations;

namespace Eryph.ClientRuntime.Powershell
{
    [PublicAPI]
    public abstract class EryphCmdLet : PSCmdlet
    {
        [Parameter]
        public ClientCredentials Credentials { get; set; }


        protected override void BeginProcessing()
        { 
            
        }

        protected ClientCredentials GetClientCredentials()
        {
            var clientCredentials = Credentials;
            if (clientCredentials != null) return clientCredentials;


            var lookup = new ClientCredentialsLookup(new PowershellEnvironment(SessionState));
            clientCredentials = lookup.FindCredentials();

            if (clientCredentials == null)
            {
                throw new InvalidOperationException(@"Could not find credentials for eryph.
You can use the parameter Credentials to set the eryph credentials. If not set, the credentials will be searched in your local configuration. 
If there is no default eryph client in your configuration the command will try to access the default system-client of a local running eryph zero or identity server.
To access the system-client you will have to run this command as Administrator (Windows) or root (Linux).
");
            }

            return clientCredentials;
        }

    }

}