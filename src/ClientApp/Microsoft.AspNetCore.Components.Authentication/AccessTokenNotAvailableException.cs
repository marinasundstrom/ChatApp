// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace Microsoft.AspNetCore.Components.Authentication;

public class AccessTokenNotAvailableException : Exception
{
    public AccessTokenNotAvailableException(string? message) : base(message)
    {
    }

    protected AccessTokenNotAvailableException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

