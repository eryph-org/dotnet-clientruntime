// Copyright (c) dbosoft GmbH and Haipa contributors. All rights reserved.
// Forked from https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/mgmtcommon/ClientRuntime/ClientRuntime
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Net.Http;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace Haipa.ClientRuntime
{
    /// <summary>
    /// Interface for all Microsoft Azure clients.
    /// </summary>
    public interface IHaipaClient
    {
        /// <summary>
        /// Gets Azure subscription credentials.
        /// </summary>
        ServiceClientCredentials Credentials { get; }

        /// <summary>
        /// Gets the HttpClient used for making HTTP requests.
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        /// Gets json serialization settings.
        /// </summary>
        JsonSerializerSettings SerializationSettings { get; }

        /// <summary>
        /// Gets json deserialization settings.
        /// </summary>
        JsonSerializerSettings DeserializationSettings { get; }

        /// <summary>
        /// When set to true a unique x-ms-client-request-id value
        /// is generated and included in each request. Default is true.
        /// </summary>
        bool? GenerateClientRequestId { get; set; }        
    }
}
