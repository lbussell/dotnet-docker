// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CommandLine;
using System.CommandLine.Hosting;
using DotNet.Docker;

var rootCommand = new RootCommand()
{
    new FromBuildsCommand().UseCommandHandler<FromBuildsCommand.Handler>(),
    new Command("specific")
    {
    },
};

var config = new CommandLineConfiguration(rootCommand).UseHost();

return await config.InvokeAsync(args);
