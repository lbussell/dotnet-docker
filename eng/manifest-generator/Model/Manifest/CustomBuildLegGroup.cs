// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.DotNet.Docker.Model.Manifest;

public record CustomBuildLegGroup(string Name, CustomBuildLegDependencyType Type, string[] Dependencies);
