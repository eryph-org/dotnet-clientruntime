using System.Runtime.InteropServices;
using Eryph.IdentityModel.Clients;

namespace Eryph.ClientRuntime.Configuration
{
    public class EryphZeroInfo : LocalIdentityProviderInfo
    {
        public EryphZeroInfo(IEnvironment environment) : base(environment, "zero")
        {
        }

        protected override bool GetIsRunning()
        {
            return Environment.IsOsPlatform(OSPlatform.Windows) && base.GetIsRunning();
        }
    }
}
