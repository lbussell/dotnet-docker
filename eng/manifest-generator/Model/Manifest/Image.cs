// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.DotNet.Docker.Model.Manifest;

using System.Text.Json.Serialization;

public record Image
{
    [JsonPropertyOrder(0)]
    public required string ProductVersion { get; set; }

    [JsonPropertyOrder(1)]
    public IDictionary<string, Tag>? SharedTags { get; set; } = null;

    [JsonPropertyOrder(2)]
    public required Platform[] Platforms { get; set; }
}
