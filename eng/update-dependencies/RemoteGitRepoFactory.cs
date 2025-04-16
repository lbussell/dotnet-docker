
using Maestro.Common.AzureDevOpsTokens;
using Microsoft.DotNet.DarcLib;
using Microsoft.DotNet.DarcLib.Helpers;
using Microsoft.Extensions.Logging;

namespace Dotnet.Docker;

public class DependencyManagerFactory(
    IAzureDevOpsTokenProvider azdoTokenProvider,
    ILoggerFactory loggerFactory,
    IProcessManager processManager)
{
    private readonly IAzureDevOpsTokenProvider _azdoTokenProvider = azdoTokenProvider;
    private readonly ILoggerFactory _loggerFactory = loggerFactory;
    private readonly IProcessManager _processManager = processManager;

    public IDependencyFileManager CreateDependencyFileManager(string repoUrl)
    {
        var dependencyFileManager = new DependencyFileManager(
            CreateRemoteGitRepo(repoUrl),
            new VersionDetailsParser(),
            _loggerFactory.CreateLogger<DependencyFileManager>());

        return dependencyFileManager;
    }

    private IRemoteGitRepo CreateRemoteGitRepo(string repoUri)
    {
        return repoUri switch
        {
            var repo when repo.Contains("github.com") =>
                new GitHubClient(
                    // Don't need auth to reach public GitHub repos
                    new RemoteTokenProvider(),
                    _processManager,
                    _loggerFactory.CreateLogger<GitHubClient>(),
                    cache: null),

            // Assume Azure DevOps otherwise
            _ => new AzureDevOpsClient(
                _azdoTokenProvider,
                _processManager,
                _loggerFactory.CreateLogger<AzureDevOpsClient>()),
        };
    }
}
