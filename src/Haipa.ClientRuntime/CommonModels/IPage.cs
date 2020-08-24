// Copyright (c) dbosoft GmbH and Haipa contributors. All rights reserved.
// Forked from https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/mgmtcommon/ClientRuntime/ClientRuntime
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

 namespace Haipa.ClientRuntime
{
    /// <summary>
    /// Defines a page interface in Azure responses.
    /// </summary>
    /// <typeparam name="T">Type of the page content items</typeparam>
    public interface IPage<T> : IEnumerable<T>
    {
        /// <summary>
        /// Gets the link to the next page.
        /// </summary>
        string NextPageLink { get; }
    }
}
