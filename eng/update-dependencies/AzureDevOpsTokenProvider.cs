
using System.Threading.Tasks;
using Azure.Identity;
using Maestro.Common.AzureDevOpsTokens;

namespace Dotnet.Docker;

public class AzureDevOpsTokenProvider(string? token = null) : IAzureDevOpsTokenProvider
{
    private string? _token = token;

    public string GetTokenForAccount(string account) => GetToken();

    public Task<string> GetTokenForAccountAsync(string account) => Task.FromResult(GetToken());

    public string? GetTokenForRepository(string repoUri) => GetToken();

    public Task<string?> GetTokenForRepositoryAsync(string repoUri) => Task.FromResult(GetToken())!;

    private string GetToken()
    {
        if (_token is null)
        {
            const string Scope = "499b84ac-1321-427f-aa17-267ca6975798/.default";
            var credential = new AzureDeveloperCliCredential();
            string token = credential.GetToken(new Azure.Core.TokenRequestContext(scopes: [Scope])).Token;
            _token = token;
            return _token;
        }

        return _token;
    }
}
