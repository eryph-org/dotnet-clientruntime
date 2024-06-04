using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Azure;
using Azure.Core;

namespace Eryph.ClientRuntime;

public class EryphRequestFailedDetailsParser : RequestFailedDetailsParser
{
    public override bool TryParse(
        Response response,
        out ResponseError? error,
        out IDictionary<string, string>? data)
    {
        error = null;
        data = null;

        if (!response.Headers.TryGetValue("Content-Type", out var contentType))
            return false;

        if (!contentType.StartsWith("application/problem+json", StringComparison.OrdinalIgnoreCase))
            return false;

        if (!TryDeserializeProblemDetails(response.Content, out var problemDetails)
            || problemDetails is null)
            return false;

        data = CreateData(problemDetails);
        var message = CreateMessage(problemDetails);
        error = new ResponseError(null, message);

        return true;
    }

    private static IDictionary<string, string>? CreateData(ProblemDetails problemDetails)
    {
        if (problemDetails.AdditionalProperties is not { Count: > 0 })
            return null;

        if (!problemDetails.AdditionalProperties.TryGetPropertyValue("traceId", out var node)
            || node is not JsonValue jsonValue
            || !jsonValue.TryGetValue(out string traceId))
            return null;
        
        return new Dictionary<string, string>
        {
            ["traceId"] = traceId,
        };
    }

    private static string CreateMessage(ProblemDetails problemDetails)
    {
        var builder = new StringBuilder();
        var message = !string.IsNullOrWhiteSpace(problemDetails.Title)
            ? problemDetails.Title
            : "The request has failed";

        builder.AppendLine(message);

        if (!string.IsNullOrWhiteSpace(problemDetails.Detail))
            builder.AppendLine(problemDetails.Detail);

        if (problemDetails.Errors is not { Count: > 0 })
            return builder.ToString();
        
        builder.AppendLine("The request has the following errors:");
        foreach (var kp in problemDetails.Errors.OrderBy(kp => kp.Key))
        {
            builder.AppendLine($"- {kp.Key}");
            foreach (var value in kp.Value)
                builder.AppendLine($"  {value}");
        }

        return builder.ToString();
    }

    private static bool TryDeserializeProblemDetails(
        BinaryData content,
        out ProblemDetails? problemDetails)
    {
        try
        {
            problemDetails = JsonSerializer.Deserialize<ProblemDetails>(
                content,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                });
            return true;
        }
        catch (JsonException)
        {
            problemDetails = null;
            return false;
        }
    }
}
