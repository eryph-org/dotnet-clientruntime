using System.Linq;
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

        public override bool IsProcessRunning(string processName, int processId)
        {
            var scriptBlock = _sessionState.InvokeCommand.NewScriptBlock(
                $"Get-Process {processName} -ErrorAction SilentlyContinue | where Id -eq {processId} | Select -First 1");
            var isRunning = scriptBlock.Invoke().Any();
            return isRunning;
        }
    }
}