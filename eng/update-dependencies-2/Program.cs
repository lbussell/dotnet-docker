using System.CommandLine;
using update_dependencies_2;


var buildIdBinder = new BuildId.Binder()
{
    BuildIdOption = new Option<int>("--build-id")
};

var azdoAuthBinder = new AzdoAuthProvider.Binder()
{
    AzdoPatOption = new Option<string?>("--azdo-pat", "Azure DevOps Personal Access Token (PAT)")
};

var command = new RootCommand("Updates components of .NET container images");
command.AddOption(buildIdBinder.BuildIdOption);
command.AddOption(azdoAuthBinder.AzdoPatOption);
command.SetHandler(UpdateDependenciesAsync, buildIdBinder, azdoAuthBinder);

await command.InvokeAsync(args);


static async Task UpdateDependenciesAsync(BuildId buildId, AzdoAuthProvider authProvider)
{
    Console.WriteLine("BuildId: " + buildId.Id.ToString());

    var buildHelper = new AzdoBuildHelper(authProvider);
    var dotNetBuildProvider = new DotNetBuildProvider(buildHelper);

    DotNetReleaseInfo release = await dotNetBuildProvider.GetRelease(buildId.Id);

    Console.WriteLine("Release config:");
    Console.WriteLine(release);
}
