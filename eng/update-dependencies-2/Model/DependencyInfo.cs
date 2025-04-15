// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.DotNet.Docker.UpdateDependencies.Model;

public record DependencyInfos
{
    public required DependencyInfo[] Dependencies { get; init; }
}

public record DependencyInfo
{
    public required int Channel { get; init; }
    public required string Repo { get; init; }
    public string[] Assets { get; init; } = [];
}
