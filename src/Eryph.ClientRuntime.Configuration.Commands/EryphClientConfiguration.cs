using System.Runtime.Serialization;

namespace Eryph.ClientRuntime.Configuration
{
    public class EryphClientConfiguration
    {
        [DataMember] public string Id { get; set; }

        [DataMember] public string Name { get; set; }

        [DataMember] public string Configuration { get; set; }

        [DataMember] public bool IsDefault { get; set; }

    }
}