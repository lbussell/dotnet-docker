// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Docker;

public abstract class BaseCommandAction : AsynchronousCommandLineAction
{
    protected abstract Task<int> RunAsync();

    public override Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default)
    {
        return RunAsync();
    }

    #region Remove Me
    private static JsonSerializerOptions s_jsonOptions = new() { WriteIndented = true };
    protected static void Print<T>(IEnumerable<T> assets)
    {
        string json = JsonSerializer.Serialize(assets, s_jsonOptions);
        Console.WriteLine(json);
    }
    #endregion Remove Me
}
