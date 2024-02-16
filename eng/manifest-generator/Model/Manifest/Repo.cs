// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.DotNet.Docker.Model.Manifest;

using System.Text.Json.Serialization;

// public record Repo(string Id, Image[] Images, string McrTagsMetadataTemplate, string Name, Readme[] Readmes);

public record Repo
{
    [JsonPropertyOrder(0)]
    public required string Id { get; set; }

    [JsonPropertyOrder(1)]
    public required string Name { get; set; }

    [JsonPropertyOrder(2)]
    public required Readme[] Readmes { get; set; } = Array.Empty<Readme>();

    [JsonPropertyOrder(3)]
    public required string McrTagsMetadataTemplate { get; set; }

    [JsonPropertyOrder(4)]
    public required Image[] Images { get; set; }
}
