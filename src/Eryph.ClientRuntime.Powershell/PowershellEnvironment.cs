using System.Management.Automation;
using Eryph.IdentityModel.Clients;

namespace Eryph.ClientRuntime.Powershell
{
    public class PowershellEnvironment : DefaultEnvironment
    {
        private readonly SessionState _sessionState;

        public PowershellEnvironment(SessionState sessionState)
        {
            _sessionState = sessionState;
        }

        public override string GetCurrentDirectory()
        {
            return _sessionState.Path.CurrentFileSystemLocation.Path;
        }

        //public override bool IsProcessRunning(string processName, int processId)
        //{
        //   return _sessionState.InvokeCommand.InvokeScript(
        //        $"Get-Process {processName} -ErrorAction SilentlyContinue | where Id -eq {processId} | Select -First 1").Any();

        //}

        //public override bool IsWindowsAdminUser
        //{
        //    get
        //    {
        //        var result = _sessionState.InvokeCommand.InvokeScript(
        //            "[bool](([System.Security.Principal.WindowsIdentity]::GetCurrent()).groups -match \"S-1-5-32-544\")").ToArray();

        //        if (result.Length == 1)
        //            return (bool)result[0].BaseObject;

        //        return false;
        //    }
        //}
    }
}