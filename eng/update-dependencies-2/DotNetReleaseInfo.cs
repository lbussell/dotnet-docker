namespace update_dependencies_2;

public record DotNetReleaseInfo
{
    public required int AzdoBuildId { get; init; }
    public required BuildInfo Sdk { get; init; }
    public required BuildInfo Runtime { get; init; }
    public required BuildInfo AspNetCore { get; init; }
}

public record BuildInfo(string BuildVersion, string ProductVersion);
