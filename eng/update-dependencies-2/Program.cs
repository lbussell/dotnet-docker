// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.DotNet.DarcLib;
using Microsoft.DotNet.ProductConstructionService.Client.Models;
using Microsoft.DotNet.Docker.UpdateDependencies.Model;
using static System.Console;
using System.CommandLine.Invocation;


var command = new RootCommand("Update dependencies");

var result = command.Parse(args);
var handler = CommandHandler.Create()


static async Task MessingAroundAsync()
{
    var monitorBuild = new DependencyInfo
    {
        Repo = "https://github.com/dotnet/dotnet-monitor",
        Channel = 548,
        Assets = [],
    };

    var client = new BarApiClient(null, null, false);

    var build = await GetBuildAsync(client, monitorBuild);
    WriteLine(build);

    var assets = await GetAssetsAsync(client, build.Id);
    foreach (var asset in assets)
    {
        var locations = "[" + string.Join(", ", asset.Locations.Select(l => l.Location)) + "]";
        WriteLine($"Asset '{asset.Name}'");
        // WriteLine($"- Locations {locations}");

        var urls = asset.Locations
            .Select(l => $"{l.Location}/{asset.Name}")
            .ToList();

        foreach (var url in urls)
        {
            WriteLine($"- {url}");
        }

        WriteLine();
    }


    static async Task<Build> GetBuildAsync(IBasicBarClient client, DependencyInfo dependencyInfo)
    {
        Build build = await client.GetLatestBuildAsync(dependencyInfo.Repo, dependencyInfo.Channel)
            ?? throw new Exception($"Build not found for {dependencyInfo.Repo} on channel {dependencyInfo.Channel}.");
        return build;
    }

    static async Task<IEnumerable<Asset>> GetAssetsAsync(IBasicBarClient client, int buildId)
    {
        var assets = await client.GetAssetsAsync(buildId: buildId);
        return assets;
    }
}
