using System.Runtime.InteropServices;
using Eryph.IdentityModel.Clients;

namespace Eryph.ClientRuntime.Configuration;

public class EryphZeroInfo(IEnvironment environment)
    : LocalIdentityProviderInfo(environment, "zero")
{
    protected override bool GetIsRunning()
    {
        return Environment.IsOsPlatform(OSPlatform.Windows) && base.GetIsRunning();
    }
}
