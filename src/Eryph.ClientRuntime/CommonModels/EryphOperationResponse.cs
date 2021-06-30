// Copyright (c) dbosoft GmbH and eryph contributors. All rights reserved.
// Forked from https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/mgmtcommon/ClientRuntime/ClientRuntime
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Rest;

namespace Eryph.ClientRuntime
{
    /// <summary>
    /// A standard service response including request ID.
    /// </summary>
    public interface IEryphOperationResponse : IHttpOperationResponse
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
    public class EryphOperationResponse : HttpOperationResponse, IEryphOperationResponse
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
    public class EryphOperationResponse<T> : HttpOperationResponse<T>, IEryphOperationResponse
    {
        /// <summary>
        /// Gets or sets the value that uniquely identifies a request 
        /// made against the service.
        /// </summary>
        public string RequestId { get; set; }
    }

    public class EryphOperationHeaderResponse<THeader> : HttpOperationHeaderResponse<THeader>, IEryphOperationResponse
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
    public class EryphOperationResponse<TBody, THeader> : HttpOperationResponse<TBody, THeader>, IEryphOperationResponse
    {
        /// <summary>
        /// Gets or sets the value that uniquely identifies a request 
        /// made against the service.
        /// </summary>
        public string RequestId { get; set; }
    }
}