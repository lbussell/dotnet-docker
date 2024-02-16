// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.DotNet.Docker.Model;

using System.Text.Json;
using System.Text.Json.Serialization;

public static class CustomJsonStringEnumConverter
{
    public class KebabCaseLower() : JsonStringEnumConverter(JsonNamingPolicy.KebabCaseLower, false);

    public class CamelCase() : JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false);
}
