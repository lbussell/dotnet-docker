using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace update_dependencies_2;

/// <summary>
/// Represents the command line options for updating versions based on Azure DevOps build IDs
/// </summary>
/// <param name="Id">Azure DevOps build ID. Example: 2662038</param>
internal record BuildId(int Id)
{
    internal class Binder : BinderBase<BuildId>
    {
        public required Option<int> BuildIdOption { get; init; }

        protected override BuildId GetBoundValue(BindingContext context)
        {
            return new BuildId(context.ParseResult.GetValueForOption(BuildIdOption));
        }
    }
}
