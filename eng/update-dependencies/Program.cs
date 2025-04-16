// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CommandLine;
using System.CommandLine.Hosting;
using Microsoft.Extensions.Hosting;
using DotNet.Docker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.DotNet.DarcLib;


var rootCommand = new RootCommand()
{
    FromChannelCommand.Create(
        name: "from-channel",
        description: "Update dependencies using the latest build from a channel"),
};

var config = new CommandLineConfiguration(rootCommand);

config.UseHost(
    _ => Host.CreateDefaultBuilder(),
    host =>
    {
        host.ConfigureServices(services =>
        {
            services.AddSingleton<IBasicBarClient>(_ => new BarApiClient(null, null, false));

            FromChannelCommand.Register<FromChannelCommand>(services);
        });
    });

return await config.InvokeAsync(args);
