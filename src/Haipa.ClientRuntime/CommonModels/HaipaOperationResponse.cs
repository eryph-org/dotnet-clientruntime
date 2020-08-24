// Copyright (c) dbosoft GmbH and Haipa contributors. All rights reserved.
// Forked from https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/mgmtcommon/ClientRuntime/ClientRuntime
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Rest;

namespace Haipa.ClientRuntime
{
    /// <summary>
    /// A standard service response including request ID.
    /// </summary>
    public interface IHaipaOperationResponse : IHttpOperationResponse
    {
        /// <summary>
        /// Gets or sets the value that uniquely identifies a request 
        /// made against the service.
        /// </summary>
        string RequestId { get; set; }
    }

    /// <summary>
    /// A standard service response including request ID.
    /// </summary>
    public class HaipaOperationResponse : HttpOperationResponse, IHaipaOperationResponse
    {
        /// <summary>
        /// Gets or sets the value that uniquely identifies a request 
        /// made against the service.
        /// </summary>
        public string RequestId { get; set; }
    }

    /// <summary>
    /// A standard service response including request ID.
    /// </summary>
    public class HaipaOperationResponse<T> : HttpOperationResponse<T>, IHaipaOperationResponse
    {
        /// <summary>
        /// Gets or sets the value that uniquely identifies a request 
        /// made against the service.
        /// </summary>
        public string RequestId { get; set; }
    }

    public class AzureOperationHeaderResponse<THeader> : HttpOperationHeaderResponse<THeader>, IHaipaOperationResponse
    {
        /// <summary>
        /// Gets or sets the value that uniquely identifies a request 
        /// made against the service.
        /// </summary>
        public string RequestId { get; set; }
    }

    /// <summary>
    /// A standard service response including request ID.
    /// </summary>
    public class HaipaOperationResponse<TBody, THeader> : HttpOperationResponse<TBody, THeader>, IHaipaOperationResponse
    {
        /// <summary>
        /// Gets or sets the value that uniquely identifies a request 
        /// made against the service.
        /// </summary>
        public string RequestId { get; set; }
    }
}