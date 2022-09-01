using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using Eryph.IdentityModel.Clients;
using Eryph.IdentityModel.Clients.Internal;
using Newtonsoft.Json.Linq;

namespace Eryph.ClientRuntime.Configuration
{
    public class LocalIdentityProviderInfo
    {
        protected readonly IEnvironment Environment;
        private readonly string _identityProviderName;

        public LocalIdentityProviderInfo(IEnvironment environment, string identityProviderName = "identity")
        {
            Environment = environment;
            _identityProviderName = identityProviderName;
        }

        public bool IsRunning => GetIsRunning();
        public IReadOnlyDictionary<string,Uri> Endpoints => GetEndpoints();
        protected virtual bool GetIsRunning()
        {

            var metadata = GetMetadata();

            metadata.TryGetValue("processName", out var processName);
            metadata.TryGetValue("processId", out var processId);

            if (string.IsNullOrWhiteSpace(processName?.ToString()) || processId== null)
                return false;

            return Environment.IsProcessRunning((string) processName, Convert.ToInt32(processId));
        }



        private IDictionary<string, object> GetMetadata()
        {
            var applicationDataPath =
                Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "eryph");

            var lockFilePath = Path.Combine(Path.Combine(applicationDataPath,
                $@"{_identityProviderName}{Path.DirectorySeparatorChar}.lock"));

            try
            {
                using (var reader = new StreamReader(Environment.FileSystem.OpenStream(lockFilePath)))
                {
                    var lockFileData = reader.ReadToEnd();
                    return JObject.Parse(lockFileData).ToObject<IDictionary<string, object>>();
                }
            }
            catch
            {
                return new Dictionary<string, object>();
            }

        }

        private IReadOnlyDictionary<string, Uri> GetEndpoints()
        {
            var result = new Dictionary<string, Uri>();
            if (!IsRunning) return result;

            var metadata = GetMetadata();

            if (!metadata.TryGetValue("endpoints", out var endpointsObject)) return result;

            var endpointsJObject = (JObject) endpointsObject;

            foreach (var kv in endpointsJObject)
            {
                result.Add(kv.Key, new Uri(kv.Value.ToString()));
            }

            return result;
        }

        public SecureString GetSystemClientPrivateKey()
        {
            var applicationDataPath =
                Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "eryph");

            var privateKeyPath = Path.Combine(applicationDataPath,
                $@"{_identityProviderName}{Path.DirectorySeparatorChar}private{Path.DirectorySeparatorChar}clients{Path.DirectorySeparatorChar}system-client.key");

            var endpoints = GetEndpoints();

            if(!endpoints.ContainsKey("identity"))
                throw new IOException("Could not find eryph-zero identity endpoint.");

            var entropy = Encoding.UTF8.GetBytes(endpoints["identity"].ToString());

            var privateKey =
                PrivateKey.ReadFile(privateKeyPath, Environment.FileSystem, PrivateKeyProtectionLevel.Machine, entropy);
            
            if (privateKey == null)
            {
                //second chance - read without encryption
                privateKey = PrivateKey.ReadFile(privateKeyPath, Environment.FileSystem);
                if (privateKey == null)
                    throw new IOException("could not read system-client private key");
            }

            return PrivateKey.ToSecureString(privateKey);

        }
    }
}