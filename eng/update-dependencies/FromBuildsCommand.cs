// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotNet.Docker.Builds;
using Microsoft.DotNet.ProductConstructionService.Client.Models;

namespace DotNet.Docker;

public partial class FromBuildsCommand : Command
{
    public FromBuildsCommand() : base("from-builds", "Update dependencies from BAR builds")
    {
        Arguments.Add(
            new Argument<int[]>("builds")
            {
                Arity = ArgumentArity.OneOrMore,
                Description = "One or more BAR build IDs to use as a source for the update."
            });
    }

    public partial class Handler(BuildsProvider buildsProvider) : BaseCommandAction
    {
        private readonly BuildsProvider _buildsProvider = buildsProvider;

        public int[] Builds { get; init; } = [];

        [GeneratedRegex(".*((linux(-musl)?)|win)-(arm|arm64|x64).*")]
        private static partial Regex AssetRegex { get; }

        protected override async Task<int> RunAsync()
        {
            Console.WriteLine($"Build IDs: [{string.Join(", ", Builds)}]");

            var builds = await _buildsProvider.GetBuildsAsync(Builds);

            // var buildLocations builds.Where

            // JsonSerializerOptions options = new() { WriteIndented = true };
            // var json = JsonSerializer.Serialize(builds, options);
            // Console.WriteLine(json);

            // foreach (var build in builds)
            // {
            //     foreach (var asset in build.Assets)
            //     {
            //         var name = asset.Name;
            //         if (AssetRegex.IsMatch(name))
            //         {
            //             Console.WriteLine($"Asset: {name}");
            //             if (asset.Locations is not null)
            //             {
            //                 foreach (var location in asset.Locations)
            //                 {
            //                     Console.WriteLine($"- Location: ({location.Type}) {location.Location}");
            //                 }
            //             }
            //         }
            //     }
            // }

            return 0;
        }
    }
}
