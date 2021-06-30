// Copyright (c) dbosoft GmbH and eryph contributors. All rights reserved.
// Forked from https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/mgmtcommon/ClientRuntime/ClientRuntime
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Eryph.ClientRuntime.OData
{
    /// <summary>
    /// Annotates OData methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ODataMethodAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets serialized name.
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        /// Initializes a new instance of ODataMethodAttribute with name.
        /// </summary>
        /// <param name="methodName">Serialized method name</param>
        public ODataMethodAttribute(string methodName)
        {
            this.MethodName = methodName;
        }
    }
}
