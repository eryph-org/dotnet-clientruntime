using System;
using System.Management.Automation;
using System.Security;
using Haipa.IdentityModel.Clients;

namespace Haipa.ClientRuntime.Configuration
{
    [Cmdlet(VerbsCommon.New, "HaipaClientCredentials")]
    [OutputType(typeof(ClientCredentials))]
    public class NewHaipaClientCredentialsCmdlet : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNull]
        public object[] InputObject { get; set; }

        [Parameter(
            Mandatory = true, 
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNull]
        public string Id { get; set; }


        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true)]

        [ValidateNotNull]
        public Uri IdentityEndpoint { get; set; }

        protected override void ProcessRecord()
        {
            foreach (var inputObject in InputObject)
            {
                SecureString keyData;

                if (inputObject is SecureString secureString)
                {
                    keyData = secureString;
                }
                else
                {
                    keyData = IdentityModel.Clients.Internal.PrivateKey.ToSecureString(
                        IdentityModel.Clients.Internal.PrivateKey.ReadString(inputObject.ToString()));
                }

                WriteObject(new ClientCredentials(Id, keyData, IdentityEndpoint));

            }
            

        }
    }
}