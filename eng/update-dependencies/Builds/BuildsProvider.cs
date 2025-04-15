// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.DarcLib;
using Microsoft.DotNet.ProductConstructionService.Client.Models;

namespace DotNet.Docker.Builds;

public class BuildsProvider(IBasicBarClient barClient)
{
    private readonly IBasicBarClient _barClient = barClient;

    public async Task<IEnumerable<Build>> GetBuildsAsync(int[] buildIds)
    {
        var tasks = buildIds.Select(async buildId => await _barClient.GetBuildAsync(buildId));
        Build[] builds = await Task.WhenAll(tasks);
        return builds;
    }
}
