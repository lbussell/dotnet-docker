// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Collections.Generic;

namespace Microsoft.DotNet.Docker.Tests;

/// <summary>
/// A Docker CLI wrapper that tracks built images and deletes them on disposal.
/// </summary>
public sealed class TrackingDockerCli(IDockerCli inner) : IDockerCli, IDisposable
{
    private readonly IDockerCli _inner = inner;
    private readonly List<string> _builtImages = [];
    private bool _disposed;

    public string Execute(string args, DockerCliRunOptions? options = null) =>
        _inner.Execute(args, options);

    public void Build(
        string tag = "",
        string dockerfile = "",
        string target = "",
        string contextDir = ".",
        bool pull = false,
        string platform = "",
        string output = "",
        params string[] buildArgs)
    {
        if (!string.IsNullOrWhiteSpace(tag))
        {
            _builtImages.Add(tag);
        }

        _inner.Build(tag, dockerfile, target, contextDir, pull, platform, output, buildArgs);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        foreach (string tag in _builtImages)
        {
            try
            {
                _inner.DeleteImage(tag);
            }
            catch
            {
                // We made our best effort to delete the images we built.
                // Ignore failures so as not to block test completion.
            }
        }

        _builtImages.Clear();
    }
}
