// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.DotNet.Docker.Model.Manifest;

using System.Text.Json.Serialization;
using Microsoft.DotNet.Docker.Model;

[JsonConverter(typeof(CustomJsonStringEnumConverter.CamelCase))]
public enum OS
{
    Linux,
    Windows,
}
