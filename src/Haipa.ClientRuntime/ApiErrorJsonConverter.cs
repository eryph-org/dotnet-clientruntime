// Copyright (c) dbosoft GmbH and Haipa contributors. All rights reserved.
// Forked from https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/mgmtcommon/ClientRuntime/ClientRuntime
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Haipa.ClientRuntime
{
    /// <summary>
    /// JsonConverter that provides custom deserialization for ApiError objects.
    /// </summary>
    public class ApiErrorJsonConverter : JsonConverter
    {
        private const string ErrorNode = "error";

        /// <summary>
        /// Returns true if the object being serialized is a CloudError.
        /// </summary>
        /// <param name="objectType">The type of the object to check.</param>
        /// <returns>True if the object being serialized is a CloudError. False otherwise.</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(ApiError) == objectType;
        }

        /// <summary>
        /// Deserializes an object from a JSON string and flattens out Error property.
        /// </summary>
        /// <param name="reader">The JSON reader.</param>
        /// <param name="objectType">The type of the object.</param>
        /// <param name="existingValue">The existing value.</param>
        /// <param name="serializer">The JSON serializer.</param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            if (objectType == null)
            {
                throw new ArgumentNullException(nameof(objectType));
            }
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            JObject jObject = JObject.Load(reader);
            JProperty errorObject = jObject.Properties().FirstOrDefault(p => 
                ErrorNode.Equals(p.Name, StringComparison.OrdinalIgnoreCase));
            if (errorObject != null)
            {
                jObject = errorObject.Value as JObject;
            }
            return jObject.ToObject<ApiError>(serializer.WithoutConverter(this));
        }

        /// <summary>
        /// Serializes an object into a JSON string adding Properties. 
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="value">The value to serialize.</param>
        /// <param name="serializer">The JSON serializer.</param>
        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}