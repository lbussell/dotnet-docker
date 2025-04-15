// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.TeamFoundation.Build.WebApi;

namespace Microsoft.DotNet.Docker.UpdateDependencies;

internal class AzdoBuildHelper
{
    const string ProjectName = "internal";

    private readonly AzdoAuthProvider _authProvider;

    private readonly Lazy<HttpClient> _httpClient;

    public AzdoBuildHelper(AzdoAuthProvider authProvider)
    {
        _authProvider = authProvider;
        _httpClient = new Lazy<HttpClient>(CreateHttpClient);
    }

    /// <summary>
    /// Downloads an Azure DevOps artifact as a string
    /// </summary>
    /// <param name="buildId">The ID of the build to download the artifact from (e.g. 2345678)</param>
    /// <param name="artifactName">The name of the artifact to download</param>
    /// <param name="artifactSubPath">The sub path of the specific file to download from the artifact</param>
    /// <returns></returns>
    public async Task<string> DownloadTextArtifact(int buildId, string artifactName, string artifactSubPath)
    {
        var buildsClient = _authProvider.GetVssConnection().GetClient<BuildHttpClient>();
        BuildArtifact artifact = await buildsClient.GetArtifactAsync(ProjectName, buildId, artifactName);

        string downloadUrl = artifact.Resource.DownloadUrl;

        // Instead of downloading the entire artifact as a zip file,modify the URL to download only a specific file
        downloadUrl = downloadUrl.Replace("/content?format=zip", $"/content?format=file&subPath=%2F{artifactSubPath}");

        using HttpResponseMessage response = await _httpClient.Value.GetAsync(downloadUrl);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    private HttpClient CreateHttpClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = _authProvider.GetAuthenticationHeader();
        return client;
    }
}
