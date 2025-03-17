using System.CommandLine;
using System.CommandLine.Binding;
using System.Net.Http.Headers;
using System.Text;
using Azure.Identity;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace update_dependencies_2;

internal class AzdoAuthProvider(Func<string> getToken)
{
    private const string AzdoProject = "dnceng";

    private readonly Lazy<string> _token = new(getToken);

    public VssConnection GetVssConnection()
    {
        return new VssConnection(
            baseUrl: new Uri($"https://dev.azure.com/{AzdoProject}"),
            credentials: new VssBasicCredential(userName: string.Empty, password: _token.Value));
    }

    public AuthenticationHeaderValue GetAuthenticationHeader()
    {
        return new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", _token))));
    }

    private static string GetAzdCliToken()
    {
        const string Scope = "499b84ac-1321-427f-aa17-267ca6975798/.default";
        var cred = new AzureDeveloperCliCredential();
        string pat = cred.GetToken(new Azure.Core.TokenRequestContext(scopes: [Scope])).Token;
        return pat;
    }

    internal class Binder : BinderBase<AzdoAuthProvider>
    {
        public required Option<string?> AzdoPatOption { get; init; }

        protected override AzdoAuthProvider GetBoundValue(BindingContext context)
        {
            string? azdoPatOption = context.ParseResult.GetValueForOption(AzdoPatOption);
            Func<string> getToken = azdoPatOption switch
            {
                null => GetAzdCliToken,
                _ => () => azdoPatOption
            };

            return new AzdoAuthProvider(getToken);
        }
    }
}
