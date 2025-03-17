using update_dependencies_2.Model;

namespace update_dependencies_2;

internal class DotNetBuildProvider(AzdoBuildHelper azdoBuildHelper)
{
    private readonly AzdoBuildHelper _azdoBuildHelper = azdoBuildHelper;

    public async Task<DotNetReleaseInfo> GetRelease(int buildId)
    {
        string configText = await _azdoBuildHelper.DownloadTextArtifact(buildId, "drop", "config.json");
        var config = ReleaseConfig.FromJson(configText);

        return new DotNetReleaseInfo
        {
            AzdoBuildId = buildId,
            Sdk = new BuildInfo(
                // Todo: pick latest SDK version
                BuildVersion: config.Sdks[0],
                ProductVersion: config.Sdk_Builds[0]),
            Runtime = new BuildInfo(
                BuildVersion: config.Runtime,
                ProductVersion: config.Runtime_Build),
            AspNetCore = new BuildInfo(
                BuildVersion: config.Asp,
                ProductVersion: config.Asp_Build)
        };
    }
}
