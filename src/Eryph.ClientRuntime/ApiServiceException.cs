// Copyright (c) dbosoft GmbH and eryph contributors. All rights reserved.
// Forked from https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/mgmtcommon/ClientRuntime/ClientRuntime
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.Rest;

namespace Eryph.ClientRuntime
{
    /// <summary>
    /// An exception generated from an http response returned from a eryph service
    /// </summary>
    public class ApiServiceException : RestException
    {
        /// <summary>
        /// Gets information about the associated HTTP request.
        /// </summary>
        public HttpRequestMessageWrapper Request { get; set; }

        /// <summary>
        /// Gets information about the associated HTTP response.
        /// </summary>
        public HttpResponseMessageWrapper Response { get; set; }

        /// <summary>
        /// Gets or sets the response object.
        /// </summary>
        public ApiError Body { get; set; }

        /// <summary>
        /// Gets or sets the value that uniquely identifies a request 
        /// made against the service.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Initializes a new instance of the EryphServiceException class.
        /// </summary>
        public ApiServiceException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the EryphServiceException class given exception message.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        public ApiServiceException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EryphServiceException class caused by another exception.
        /// </summary>
        /// <param name="message">A description of the error.</param>
        /// <param name="innerException">The exception which caused the current exception.</param>
        public ApiServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
