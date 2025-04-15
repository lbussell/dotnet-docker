// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.DarcLib;
using Microsoft.DotNet.DarcLib.Helpers;
using Microsoft.DotNet.DarcLib.Models;
using Microsoft.DotNet.DarcLib.Models.Darc;
using Microsoft.DotNet.ProductConstructionService.Client.Models;
using Microsoft.Extensions.Logging;

namespace DotNet.Docker;

public partial class FromChannelCommand : Command
{
    public FromChannelCommand() : base("from-channel", "Update dependencies from BAR channel")
    {
        Arguments.Add(
            new Argument<int>("channel")
            {
                Arity = ArgumentArity.ExactlyOne,
                Description = "The BAR channel to use as a source for the update"
            });
        Arguments.Add(
            new Argument<string>("repo")
            {
                Arity = ArgumentArity.ExactlyOne,
                Description = "The repository to get the latest build from (e.g. 'https://github.com/dotnet/sdk')"
            });
    }

    public partial class Handler(
        IBasicBarClient barClient,
        ILogger<Handler> logger,
        DependencyManagerFactory dependencyManagerFactory)
        : BaseCommandAction
    {
        private readonly IBasicBarClient _barClient = barClient;
        private readonly ILogger<Handler> _logger = logger;
        private readonly DependencyManagerFactory _dependencyManagerFactory = dependencyManagerFactory;

        public int Channel { get; init; }

        public string Repo { get; init; } = "";

        protected override async Task<int> RunAsync()
        {
            // Channel channel = await _barClient.GetChannelAsync(Channel);
            // var asset = await _barClient.GetAssetsAsync("productCommit-linux-x64.json");
            // IEnumerable<Asset> assets = await _barClient.GetAssetsAsync(buildId: latestBuild.Id);

            _logger.LogInformation($"Getting latest build for {Repo} from channel {Channel}");
            Build latestBuild = await _barClient.GetLatestBuildAsync(Repo, Channel);

            if (IsVmr(Repo))
            {
                var results = GetUpdatesFromVmrBuild(latestBuild);
            }

            if (!IsSdk(Repo))
            {
                throw new InvalidOperationException(
                    "Expected a build of the SDK repo, but got a build of " +
                    $"{latestBuild.AzureDevOpsRepository ?? latestBuild.GitHubRepository} instead.");
            }

            IEnumerable<DependencyDetail> dependencies = await GetRepoDependenciesAsync(Repo, latestBuild.Commit);

            var sdkCommit = latestBuild.Commit;

            var runtimeCommit =
                dependencies.FirstOrDefault(dependency => dependency.Name == "Microsoft.NETCore.App.Ref")?.Commit
                    ?? throw new InvalidOperationException("Could not find Microsoft.NETCore.App.Ref in dependencies.");

            var aspnetCommit =
                dependencies.FirstOrDefault(dependency => dependency.Name == "Microsoft.AspNetCore.App.Ref")?.Commit
                    ?? throw new InvalidOperationException("Could not find Microsoft.AspNetCore.App.Ref in dependencies.");

            Print(dependencies);

            return 0;
        }

        private IReadOnlyDictionary<string, string> GetUpdatesFromVmrBuild(Build vmrBuild)
        {
            var majorMinorVersion = new Version();
            var updates = new Dictionary<string, string>();

            return updates;
        }

        private async Task<IEnumerable<DependencyDetail>> GetRepoDependenciesAsync(
            string remoteRepoUri,
            string commitSha)
        {
            var dependencyFileManager = _dependencyManagerFactory.CreateDependencyFileManager(remoteRepoUri);

            VersionDetails versionDetails =
                await dependencyFileManager.ParseVersionDetailsXmlAsync(
                    remoteRepoUri,
                    commitSha,
                    includePinned: true);

            return versionDetails.Dependencies;
        }

        private static bool IsVmr(string Repo) =>
            Repo.Contains("github.com/dotnet/dotnet")
            || Repo.Contains("dev.azure.com/dnceng/internal/_git/dotnet-dotnet");

        private static bool IsSdk(string Repo) =>
            Repo.Contains("github.com/dotnet/sdk")
            || Repo.Contains("dev.azure.com/dnceng/internal/_git/dotnet-sdk");
    }
}
