﻿// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// ------------------------------------------------------------

namespace Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// This is used to force DocumentServiceLease implementations to be serialized throught <see cref="DocumentServiceLeaseConverter"/>
    /// </summary>
    internal sealed class DocumentServiceLeaseDisabledConverter : JsonConverter
    {
        public override bool CanRead => false;

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
