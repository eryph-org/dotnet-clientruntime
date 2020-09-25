using System.Runtime.InteropServices;
using Haipa.IdentityModel.Clients;

namespace Haipa.ClientRuntime.Configuration
{
    public class HaipaZeroInfo : LocalIdentityProviderInfo
    {
        public HaipaZeroInfo(IEnvironment environment) : base(environment, "zero")
        {
        }

        protected override bool GetIsRunning()
        {
            return Environment.IsOsPlatform(OSPlatform.Windows) && base.GetIsRunning();
        }
    }
}
