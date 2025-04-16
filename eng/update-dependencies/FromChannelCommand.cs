// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.DarcLib;
using Microsoft.DotNet.DarcLib.Models;
using Microsoft.DotNet.DarcLib.Models.Darc;
using Microsoft.DotNet.ProductConstructionService.Client.Models;
using Microsoft.Extensions.Logging;

namespace DotNet.Docker;

internal class FromChannelOptions : IOptions
{
    public required int Channel { get; init; }
    public required string Repo { get; init; }

    public static List<Argument> Arguments { get; } =
    [
        new Argument<int>("channel")
        {
            Arity = ArgumentArity.ExactlyOne,
            Description = "The BAR channel to use as a source for the update"
        },
        new Argument<string>("repo")
        {
            Arity = ArgumentArity.ExactlyOne,
            Description = "The repository to get the latest build from (e.g. 'https://github.com/dotnet/sdk')"
        },
    ];

    public static List<Option> Options { get; } = [];
}

internal class FromChannelCommand(
    IBasicBarClient barClient,
    ILogger<FromChannelCommand> logger)
    : BaseCommand<FromChannelOptions>
{
    private readonly IBasicBarClient _barClient = barClient;
    private readonly ILogger<FromChannelCommand> _logger = logger;

    public override async Task<int> ExecuteAsync(FromChannelOptions options)
    {
        _logger.LogInformation(
            "Getting latest build for {options.Repo} from channel {options.Channel}",
            options.Repo, options.Channel);
        Build latestBuild = await _barClient.GetLatestBuildAsync(options.Repo, options.Channel);

        string? channelName = latestBuild.Channels.FirstOrDefault(c => c.Id == options.Channel)?.Name;
        _logger.LogInformation(
            "Channel {options.Channel} is '{channel.Name}'",
            options.Channel, latestBuild.Channels.FirstOrDefault(c => c.Id == options.Channel)?.Name);
        _logger.LogInformation(
            "Got latest build {latestBuild.Id} with commit {options.Repo}@{latestBuild.Commit}",
            latestBuild.Id, latestBuild.AzureDevOpsRepository ?? latestBuild.GitHubRepository, latestBuild.Commit);

        if (!IsVmrBuild(latestBuild))
        {
            throw new InvalidOperationException(
                "Expected a build of the VMR, but got a build of " +
                $"{latestBuild.AzureDevOpsRepository ?? latestBuild.GitHubRepository} instead.");
        }

        var updates = GetUpdatesFromVmrBuild(latestBuild);

        // var sdkCommit = latestBuild.Commit;

        // Channel channel = await _barClient.GetChannelAsync(Channel);
        // var asset = await _barClient.GetAssetsAsync("productCommit-linux-x64.json");
        // IEnumerable<Asset> assets = await _barClient.GetAssetsAsync(buildId: latestBuild.Id);

        // var runtimeCommit =
        //     dependencies.FirstOrDefault(dependency => dependency.Name == "Microsoft.NETCore.App.Ref")?.Commit
        //         ?? throw new InvalidOperationException("Could not find Microsoft.NETCore.App.Ref in dependencies.");

        // var aspnetCommit =
        //     dependencies.FirstOrDefault(dependency => dependency.Name == "Microsoft.AspNetCore.App.Ref")?.Commit
        //         ?? throw new InvalidOperationException("Could not find Microsoft.AspNetCore.App.Ref in dependencies.");

        // Print(dependencies);

        return 0;
    }

    private IReadOnlyDictionary<string, string> GetUpdatesFromVmrBuild(Build vmrBuild)
    {
        var majorMinorVersion = new Version();
        var updates = new Dictionary<string, string>();

        return updates;
    }

    private static bool IsVmrBuild(Build build)
    {
        string repo = build.GitHubRepository ?? build.AzureDevOpsRepository;
        return repo.Contains("github.com/dotnet/dotnet")
            || repo.Contains("dev.azure.com/dnceng/internal/_git/dotnet-dotnet");
    }
}
