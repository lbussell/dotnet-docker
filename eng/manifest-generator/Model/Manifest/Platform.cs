// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.DotNet.Docker.Model.Manifest;

using System.Text.Json.Serialization;

public record Platform
{
    [JsonPropertyOrder(0)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Architecture Architecture { get; set; } = Architecture.AMD64;

    [JsonPropertyOrder(1)]
    public IDictionary<string, string>? BuildArgs { get; set; }

    [JsonPropertyOrder(2)]
    public required string Dockerfile { get; set; }

    [JsonPropertyOrder(3)]
    public string? DockerfileTemplate { get; set; }

    [JsonPropertyOrder(4)]
    public required OS OS { get; set; }

    [JsonPropertyOrder(5)]
    public required string OsVersion { get; set; }

    [JsonPropertyOrder(6)]
    public required IDictionary<string, Tag> Tags { get; set; } = new Dictionary<string, Tag>();

    [JsonPropertyOrder(7)]
    public CustomBuildLegGroup[]? CustomBuildLegGroups { get; set; } = null;

    [JsonPropertyOrder(8)]
    public string? Variant { get; set; }
}
