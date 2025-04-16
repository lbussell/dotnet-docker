// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.DotNet.DarcLib;
using Microsoft.DotNet.ProductConstructionService.Client.Models;
using Microsoft.Extensions.Logging;

namespace Dotnet.Docker;

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

internal partial class FromChannelCommand(
    IAssetUrlResolver assetUrlResolver,
    IBasicBarClient barClient,
    ILogger<FromChannelCommand> logger)
    : BaseCommand<FromChannelOptions>
{
    private readonly IAssetUrlResolver _assetResolver = assetUrlResolver;
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

        var updates = await GetUpdatesFromVmrBuildAsync(latestBuild);

        // var sdkCommit = latestBuild.Commit;

        // Channel channel = await _barClient.GetChannelAsync(Channel);
        // var asset = await _barClient.GetAssetsAsync("productCommit-linux-x64.json");

        // var runtimeCommit =
        //     dependencies.FirstOrDefault(dependency => dependency.Name == "Microsoft.NETCore.App.Ref")?.Commit
        //         ?? throw new InvalidOperationException("Could not find Microsoft.NETCore.App.Ref in dependencies.");

        // var aspnetCommit =
        //     dependencies.FirstOrDefault(dependency => dependency.Name == "Microsoft.AspNetCore.App.Ref")?.Commit
        //         ?? throw new InvalidOperationException("Could not find Microsoft.AspNetCore.App.Ref in dependencies.");

        // Print(dependencies);

        return 0;
    }

    private async Task<IReadOnlyDictionary<string, string>> GetUpdatesFromVmrBuildAsync(Build build)
    {
        var updates = new Dictionary<string, string>();

        IEnumerable<Asset> assets = await _barClient.GetAssetsAsync(buildId: build.Id);
        Asset productCommitAsset = assets.FirstOrDefault(a => ProductCommitInfos.AssetRegex.IsMatch(a.Name))
            ?? throw new InvalidOperationException($"Could not find product commit version in assets.");

        string productCommitVersionResponse = await _assetResolver.GetAssetContentsAsync(productCommitAsset);
        var productInfos = ProductCommitInfos.FromJson(productCommitVersionResponse);

        Version majorMinorVersion = ResolveMajorMinorVersion(productInfos.Sdk.Version);

        var updater = new DockerfileShaUpdater(
            "runtime",
            majorMinorVersion.ToString(),
            productInfos.Sdk.Version,
            "linux-musl",
            "x64",
            productInfos.Sdk.Commit,
            default!);

        /**
            "dotnet|10.0|product-version": "10.0.0-preview.3",
            "dotnet|10.0|fixed-tag": "$(dotnet|10.0|product-version)",
            "dotnet|10.0|minor-tag": "10.0-preview",
            "dotnet|10.0|base-url|main": "$(base-url|public|preview|main)",
            "dotnet|10.0|base-url|nightly": "$(base-url|public|preview|nightly)",

            "runtime|10.0|build-version": "10.0.0-preview.3.25171.5",
            "runtime|10.0|linux-musl|x64|sha": "...",
            "runtime|10.0|linux-musl|arm|sha": "...",
            "runtime|10.0|linux-musl|arm64|sha": "...",
            "runtime|10.0|linux|arm|sha": "...",
            "runtime|10.0|linux|x64|sha": "...",
            "runtime|10.0|linux|arm64|sha": "...",
            "runtime|10.0|win|x64|sha": "...",

            "aspnet|10.0|build-version": "10.0.0-preview.3.25172.1",
            "aspnet|10.0|linux-musl|x64|sha": "...",
            "aspnet|10.0|linux-musl|arm|sha": "...",
            "aspnet|10.0|linux-musl|arm64|sha": "...",
            "aspnet|10.0|linux|arm|sha": "...",
            "aspnet|10.0|linux|x64|sha": "...",
            "aspnet|10.0|linux|arm64|sha": "...",
            "aspnet|10.0|win|x64|sha": "...",

            "aspnet-composite|10.0|linux|x64|sha": "...",
            "aspnet-composite|10.0|linux|arm|sha": "...",
            "aspnet-composite|10.0|linux|arm64|sha": "...",
            "aspnet-composite|10.0|linux-musl|x64|sha": "...",
            "aspnet-composite|10.0|linux-musl|arm|sha": "...",
            "aspnet-composite|10.0|linux-musl|arm64|sha": "...",

            "sdk|10.0|build-version": "10.0.100-preview.3.25201.16",
            "sdk|10.0|product-version": "10.0.100-preview.3",
            "sdk|10.0|fixed-tag": "$(sdk|10.0|product-version)",
            "sdk|10.0|minor-tag": "$(dotnet|10.0|minor-tag)",
            "sdk|10.0|linux-musl|arm|sha": "...",
            "sdk|10.0|linux-musl|arm64|sha": "...",
            "sdk|10.0|linux-musl|x64|sha": "...",
            "sdk|10.0|linux|arm|sha": "...",
            "sdk|10.0|linux|arm64|sha": "...",
            "sdk|10.0|linux|x64|sha": "...",
            "sdk|10.0|win|x64|sha": "...",
        **/

        return updates;
    }

    private static Version ResolveMajorMinorVersion(string versionString)
    {
        var versionParts = versionString.Split('.');

        if (versionParts.Length < 2)
        {
            throw new InvalidOperationException($"Could not parse version '{versionString}'.");
        }

        return new Version(major: int.Parse(versionParts[0]), minor: int.Parse(versionParts[1]));
    }

    private static bool IsVmrBuild(Build build)
    {
        string repo = build.GitHubRepository ?? build.AzureDevOpsRepository;
        return repo.Contains("github.com/dotnet/dotnet")
            || repo.Contains("dev.azure.com/dnceng/internal/_git/dotnet-dotnet");
    }

    private partial record ProductCommitInfos(
        ProductCommitInfo Sdk,
        ProductCommitInfo Runtime,
        ProductCommitInfo Aspnet)
    {
        [GeneratedRegex("productCommit-linux-x64.json$")]
        public static partial Regex AssetRegex { get; }

        public static ProductCommitInfos FromJson(string json)
        {
            return JsonSerializer.Deserialize<ProductCommitInfos>(json, JsonOptions)
                ?? throw new InvalidOperationException($"Could not deserialize product commit versions.");
        }

        private static JsonSerializerOptions JsonOptions { get; } = new()
        {
            PropertyNameCaseInsensitive = true
        };
    };

    private record ProductCommitInfo(string Commit, string Version);
}
