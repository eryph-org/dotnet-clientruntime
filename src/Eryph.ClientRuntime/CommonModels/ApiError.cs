// Copyright (c) dbosoft GmbH and eryph contributors. All rights reserved.
// Forked from https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/mgmtcommon/ClientRuntime/ClientRuntime
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Eryph.ClientRuntime
{
    /// <summary>
    /// Provides additional information about an http error response
    /// </summary>
    public class ApiError : ErrorData
    {
        /// <summary>
        /// The inner error with exception details
        /// </summary>
        public InnerError InnerError { get; set; }

        public ErrorData[] Details { get; set; }
    }

    public class ErrorData
    {
        
        /// <summary>
        /// The error code parsed from the body of the http error response
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The error message parsed from the body of the http error response
        /// </summary>
        public string Message { get; set; }


        /// <summary>
        /// Gets or sets the target of the error.
        /// </summary>
        public string Target { get; set; }

    }

    public class InnerError
    {
        /// <summary>
        /// The exception type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The internal error message or exception dump.
        /// </summary>
        public string Message { get; set; }

        public string StackTrace { get; set; }

    }
}
