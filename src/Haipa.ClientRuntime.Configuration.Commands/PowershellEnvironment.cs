using System.Management.Automation;
using Haipa.IdentityModel.Clients;

namespace Haipa.ClientRuntime.Configuration
{
    internal class PowershellEnvironment : DefaultEnvironment
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
    }
}