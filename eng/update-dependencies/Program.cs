// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CommandLine;
using System.CommandLine.Hosting;
using Microsoft.Extensions.Hosting;
using DotNet.Docker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.DotNet.DarcLib;
using Microsoft.DotNet.DarcLib.Helpers;
using Maestro.Common.AzureDevOpsTokens;

var rootCommand = new RootCommand()
{
    new FromBuildsCommand().UseCommandHandler<FromBuildsCommand.Handler>(),
    new FromChannelCommand().UseCommandHandler<FromChannelCommand.Handler>(),
    new Command("specific")
    {
    },
};

var config = new CommandLineConfiguration(rootCommand)
    .UseHost(host =>
        host.ConfigureServices(services =>
            {
                services.AddLogging(builder => builder.AddConsole());

                services.AddSingleton<IBasicBarClient>(_ => new BarApiClient(null, null, false));
                services.AddSingleton<IAzureDevOpsTokenProvider>(new DotNet.Docker.AzureDevOpsTokenProvider());
                services.AddSingleton<IProcessManager>(serviceProvider =>
                    new ProcessManager(serviceProvider.GetRequiredService<ILogger<ProcessManager>>(), "git"));
                services.AddSingleton<DependencyManagerFactory>();
            })
        .UseDefaultServiceProvider(configure =>
            configure.ValidateOnBuild = true));

return await config.InvokeAsync(args);
