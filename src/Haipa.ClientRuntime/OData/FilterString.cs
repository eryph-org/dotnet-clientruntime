// Copyright (c) dbosoft GmbH and Haipa contributors. All rights reserved.
// Forked from https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/mgmtcommon/ClientRuntime/ClientRuntime
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Haipa.ClientRuntime.OData
{
    /// <summary>
    /// Handles OData filter generation.
    /// </summary>
    public static class FilterString
    {
        /// <summary>
        /// Generates an OData filter from a specified Linq expression. Skips null parameters.
        /// </summary>
        /// <typeparam name="T">Filter type.</typeparam>
        /// <param name="filter">Entity to use for filter generation.</param>
        /// <returns></returns>
        public static string Generate<T>(Expression<Func<T, bool>> filter)
        {
            return Generate(filter, true);
        }

        /// <summary>
        /// Generates an OData filter from a specified Linq expression.
        /// </summary>
        /// <typeparam name="T">Filter type.</typeparam>
        /// <param name="filter">Entity to use for filter generation.</param>
        /// <param name="skipNullFilterParameters">Value indicating whether null values should be skipped.</param>
        /// <returns></returns>
        public static string Generate<T>(Expression<Func<T, bool>> filter, bool skipNullFilterParameters)
        {
            if (filter == null || !filter.Parameters.Any())
            {
                return string.Empty;
            }
            var visitor = new UrlExpressionVisitor(filter.Parameters.First(), skipNullFilterParameters);
            visitor.Visit(filter);
            return visitor.ToString();
        }
    }
}
