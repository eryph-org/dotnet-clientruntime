using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Eryph.ClientRuntime;

internal class ProblemDetails
{
    public string? Type { get; set; }
    
    public string? Title { get; set; }

    public int? Status { get; set; }

    public string? Detail { get; set; }

    public string? Instance { get; set; }

    public IDictionary<string, string[]>? Errors { get; set; }

    [JsonExtensionDataAttribute]
    public JsonObject? AdditionalProperties { get; set; }
}
