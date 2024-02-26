using Eryph.ClientRuntime.Powershell;
using Eryph.IdentityModel.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security;
using System.Text;

namespace Eryph.ClientRuntime.Configuration
{
    [Cmdlet(VerbsCommon.Get, "EryphAccessToken")]
    [OutputType(typeof(AccessTokenResult))]
    public class GetEryphAccessToken : EryphCmdLet
    {
        [Parameter(HelpMessage = "The OAuth scopes to request for the access token.")]
        public string[] Scopes { get; set; }

        [Parameter(HelpMessage = "Return only the access token as plain string.")]
        public SwitchParameter AsPlainText { get; set; }

        protected override void ProcessRecord()
        {
            var clientCredentials = GetClientCredentials();
            var response = clientCredentials.GetAccessToken(Scopes).GetAwaiter().GetResult();

            if (AsPlainText.ToBool())
            {
                WriteObject(response.AccessToken);
            }
            else
            {
                WriteObject(new AccessTokenResult(
                    response,
                    clientCredentials.IdentityProvider,
                    clientCredentials.Configuration));
            }
        }

        public class AccessTokenResult
        {
            public AccessTokenResult(
                AccessTokenResponse response,
                Uri identityEndpoint,
                string configuration)
            {
                var secureString = new SecureString();
                foreach (var c in response.AccessToken)
                {
                    secureString.AppendChar(c);
                }
                secureString.MakeReadOnly();
                AccessToken = secureString;
                Scopes = response.Scopes.ToArray();
                ExpiresOn = response.ExpiresOn?.LocalDateTime;
                IdentityEndpoint = identityEndpoint;
                Configuration = configuration;
            }

            // Do not remove properties without checking the output in Powershell.
            // With 4 or fewer properties, Powershell switches into table mode which
            // makes the result unreadable.

            public SecureString AccessToken { get; private set; }

            public string[] Scopes { get; private set; }

            public DateTime? ExpiresOn { get; private set; }

            public Uri IdentityEndpoint { get; private set; }

            public string Configuration { get; private set; }
        }
    }
}
