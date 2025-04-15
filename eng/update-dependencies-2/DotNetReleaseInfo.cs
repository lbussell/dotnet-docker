// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.DotNet.Docker.UpdateDependencies;

public record DotNetReleaseInfo
{
    public required int AzdoBuildId { get; init; }
    public required BuildInfo Sdk { get; init; }
    public required BuildInfo Runtime { get; init; }
    public required BuildInfo AspNetCore { get; init; }
}

public record BuildInfo(string BuildVersion, string ProductVersion);
