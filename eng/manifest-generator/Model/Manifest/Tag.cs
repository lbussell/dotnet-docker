// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;

namespace Microsoft.DotNet.Docker.Model.Manifest;

public record Tag
{
    public string? DocumentationGroup { get; set; } = null;

    public bool? IsLocal { get; set; } = null;

    public TagDocumentationType? DocType { get; set; } = null;

    public TagSyndication? Syndication { get; set; } = null;
}
