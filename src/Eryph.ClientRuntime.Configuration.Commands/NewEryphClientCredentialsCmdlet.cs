using System;
using System.Management.Automation;
using System.Security;
using Eryph.IdentityModel.Clients;
using JetBrains.Annotations;
using Org.BouncyCastle.Crypto;

namespace Eryph.ClientRuntime.Configuration
{
    [PublicAPI]
    [Cmdlet(VerbsCommon.New, "EryphClientCredentials")]
    [OutputType(typeof(ClientCredentials))]
    public class NewEryphClientCredentialsCmdlet : PSCmdlet
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

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Configuration { get; set; }



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
                    AsymmetricCipherKeyPair keyPair = null;
                    try
                    {
                        keyPair = IdentityModel.Clients.Internal.PrivateKey.ReadString(inputObject.ToString());
                    }catch{ 
                        // ignore
                    }

                    if (keyPair == null)
                    {
                        WriteError(new ErrorRecord(
                            new InvalidOperationException("Invalid InputObject. InputObject has be a SecureString with private key or a PKCS1 private key string."),
                            $"EryphClientCredentials{ErrorCategory.InvalidArgument}", ErrorCategory.InvalidArgument, inputObject));
                        continue;
                    }
                    keyData = IdentityModel.Clients.Internal.PrivateKey.ToSecureString(keyPair);
                }

                WriteObject(new ClientCredentials(Id, keyData, IdentityEndpoint, Configuration));

            }
            

        }
    }
}
