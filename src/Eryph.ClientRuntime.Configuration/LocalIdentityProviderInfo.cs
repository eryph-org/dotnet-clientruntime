using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.Json;
using Eryph.IdentityModel.Clients;
using Eryph.IdentityModel.Clients.Internal;

namespace Eryph.ClientRuntime.Configuration;

public class LocalIdentityProviderInfo(
    IEnvironment environment,
    string identityProviderName = "identity")
{
    protected readonly IEnvironment Environment = environment;

    public bool IsRunning => GetIsRunning();

    public IReadOnlyDictionary<string,Uri> Endpoints => GetEndpoints();

    protected virtual bool GetIsRunning()
    {
        using var metadata = GetMetadata();
        return GetIsRunning(metadata);
    }

    private JsonDocument GetMetadata()
    {
        var applicationDataPath =
            Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "eryph");

        var lockFilePath = Path.Combine(applicationDataPath, identityProviderName, ".lock");

        try
        {
            using var reader = new StreamReader(Environment.FileSystem.OpenStream(lockFilePath));
            return JsonDocument.Parse(reader.ReadToEnd());
        }
        catch
        {
            return JsonDocument.Parse("{}");
        }
    }

    private bool GetIsRunning(JsonDocument metadata)
    {
        var root = metadata.RootElement;

        if (root.ValueKind is not JsonValueKind.Object
            || !root.TryGetProperty("processName", out var processNameValue)
            || processNameValue.ValueKind is not JsonValueKind.String
            || !root.TryGetProperty("processId", out var processIdValue)
            || !processIdValue.TryGetInt32(out var processId))
        {
            return false;
        }

        var processName = processNameValue.GetString();
        return !string.IsNullOrWhiteSpace(processName)
               && Environment.IsProcessRunning(processName, processId);
    }

    private IReadOnlyDictionary<string, Uri> GetEndpoints()
    {
        var result = new Dictionary<string, Uri>();
            
        using var metadata = GetMetadata();
        if (!GetIsRunning(metadata))
            return result;

        var root = metadata.RootElement;
        if (root.ValueKind is not JsonValueKind.Object
            || !root.TryGetProperty("endpoints", out var endpoints)
            || endpoints.ValueKind is not JsonValueKind.Object)
        {
            return result;
        }

        foreach (var endpoint in endpoints.EnumerateObject())
        {
            if (endpoint.Value.ValueKind is JsonValueKind.String
                && Uri.TryCreate(endpoint.Value.GetString(), UriKind.Absolute, out var uri))
            {
                result.Add(endpoint.Name, uri);
            }
        }

        return result;
    }

    public SecureString GetSystemClientPrivateKey()
    {
        var applicationDataPath = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData),
            "eryph");

        var privateKeyPath = Path.Combine(
            applicationDataPath,
            identityProviderName, 
            "private",
            "clients",
            "system-client.key");

        var endpoints = GetEndpoints();

        if (!endpoints.TryGetValue("identity", out var endpoint))
            throw new IOException("Could not find eryph-zero identity endpoint.");

        var entropy = Encoding.UTF8.GetBytes(endpoint.ToString());
        var privateKey = PrivateKey.ReadFile(privateKeyPath, Environment.FileSystem,
            PrivateKeyProtectionLevel.Machine, entropy);

        if (privateKey != null)
            return PrivateKey.ToSecureString(privateKey);

        // Try to read the private key again without encryption
        privateKey = PrivateKey.ReadFile(privateKeyPath, Environment.FileSystem);
        if (privateKey == null)
            throw new IOException("Could not read system-client private key.");

        return PrivateKey.ToSecureString(privateKey);
    }
}
