using System.Runtime.Serialization;

namespace Haipa.ClientRuntime.Configuration
{
    public class HaipaClientConfiguration
    {
        [DataMember] public string Id { get; set; }

        [DataMember] public string Name { get; set; }

        [DataMember] public string Configuration { get; set; }

        [DataMember] public bool IsDefault { get; set; }

    }
}