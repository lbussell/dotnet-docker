// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

namespace Microsoft.DotNet.Docker.Tests;

public interface IDockerCli
{
    string Execute(string args, DockerCliRunOptions? options = null);

    void Build(
        string tag = "",
        string dockerfile = "",
        string target = "",
        string contextDir = ".",
        bool pull = false,
        string platform = "",
        string output = "",
        params string[] buildArgs);
}
