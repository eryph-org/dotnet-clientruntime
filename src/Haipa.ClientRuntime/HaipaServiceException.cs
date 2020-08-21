// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Rest;

namespace Haipa.ClientRuntime
{
    /// <summary>
    /// An exception generated from an http response returned from a Haipa service
    /// </summary>
    public class HaipaServiceException : RestException
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
        public HaipaError Body { get; set; }

        /// <summary>
        /// Gets or sets the value that uniquely identifies a request 
        /// made against the service.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Initializes a new instance of the HaipaServiceException class.
        /// </summary>
        public HaipaServiceException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the HaipaServiceException class given exception message.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        public HaipaServiceException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the HaipaServiceException class caused by another exception.
        /// </summary>
        /// <param name="message">A description of the error.</param>
        /// <param name="innerException">The exception which caused the current exception.</param>
        public HaipaServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
