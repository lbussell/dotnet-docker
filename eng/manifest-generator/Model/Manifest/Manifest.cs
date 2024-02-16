// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.DotNet.Docker.Model.Manifest;

using System.Text.Json.Serialization;

public record Manifest
{
    [JsonPropertyOrder(0)]
    public required Readme Readme { get; set; }

    [JsonPropertyOrder(1)]
    public required string Registry { get; set; }

    [JsonPropertyOrder(2)]
    public required IDictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();

    [JsonPropertyOrder(3)]
    public required string[] Includes { get; set; }

    [JsonPropertyOrder(4)]
    public required Repo[] Repos { get; set; } = [];
}
