// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
//

using System.Collections.Generic;
using System.CommandLine;
using Microsoft.Extensions.Options;

namespace Dotnet.Docker
{
    public class SpecificCommandOptions : IOptions
    {
        public string GitHubProject { get; } = "dotnet-docker";
        public string GitHubUpstreamOwner { get; } = "dotnet";

        public required string InternalBaseUrl { get; init; }
        public required string InternalAccessToken { get; init; }
        public required bool ComputeChecksums { get; init; }
        public required string DockerfileVersion { get; init; }
        public required string Email { get; init; }
        public required string Password { get; init; }
        public string SourceBranch { get; init; } = "nightly";
        public required string TargetBranch { get; init; }
        public required string User { get; init; }
        public required string AzdoOrganization { get; init; }
        public required string AzdoProject { get; init; }
        public required string AzdoRepo { get; init; }
        public required IDictionary<string, string?> ProductVersions { get; init; } = new Dictionary<string, string?>();
        public required string[] Tools { get; init; }
        public required string VersionSourceName { get; init; }
        public required bool UseStableBranding { get; init; }
        public required string ChecksumsFile { get; init; }
        public required ReleaseState? ReleaseState { get; init; }

        public bool UpdateOnly => Email == null || Password == null || User == null || TargetBranch == null;
        public bool IsInternal => !string.IsNullOrEmpty(InternalBaseUrl);

        public static List<Argument> Arguments =>
        [
            new Argument<string>("dockerfile-version"),
        ];

        public static List<Option> Options => GetOptions();

        private static List<Option> GetOptions()
        {
            var toolOption = new Option<string[]>("--tool") { Description = "Tool to update." };
            toolOption.AcceptOnlyFromAmong(Docker.Tools.SupportedTools);

            List<Option> options =
            [
                new Option<string[]>("--product-version") { Description = "Product versions to update (<product-name>=<version>)" },
                toolOption,
                new Option<string>("--version-source-name") { Description = "The name of the source from which the version information was acquired." },
                new Option<string>("--email") { Description = "GitHub or AzDO email used to make PR (if not specified, a PR will not be created)" },
                new Option<string>("--password") { Description = "GitHub or AzDO password used to make PR (if not specified, a PR will not be created)" },
                new Option<string>("--user") { Description = "GitHub or AzDO user used to make PR (if not specified, a PR will not be created)" },
                new Option<bool>("--compute-shas") { Description = "Compute the checksum if a published checksum cannot be found" },
                new Option<bool>("--stable-branding") { Description = "Use stable branding version numbers to compute paths" },
                new Option<string>("--source-branch") { Description = "Branch where the Dockerfiles are hosted" },
                new Option<string>("--target-branch") { Description = "Target branch of the generated PR (defaults to value of source-branch)" },
                new Option<string>("--binary-sas") { Description = "SAS query string used to access binary files in blob storage" },
                new Option<string>("--checksum-sas") { Description = "SAS query string used to access checksum files in blob storage" },
                new Option<string>("--org") { Description = "Name of the AzDO organization" },
                new Option<string>("--project") { Description = "Name of the AzDO project" },
                new Option<string>("--repo") { Description = "Name of the AzDO repo" },
                new Option<string>("--checksums-file") { Description = "File containing a list of checksums for each product asset" },
                new Option<ReleaseState?>("--release-state") { Description = "The release state of the product assets" },
                new Option<string>("--internal-base-url") { Description = "Base Url for internal build artifacts" },
                new Option<string>("--internal-access-token") { Description = "PAT for accessing internal build artifacts" }
            ];

            return options;
        }

        // public SpecificCommandOptions(
        //     string dockerfileVersion,
        //     string[] productVersion,
        //     string[] tool,
        //     string versionSourceName,
        //     string email,
        //     string password,
        //     string user,
        //     bool computeShas,
        //     bool stableBranding,
        //     string binarySas,
        //     string checksumSas,
        //     string sourceBranch,
        //     string targetBranch,
        //     string org,
        //     string project,
        //     string repo,
        //     string checksumsFile,
        //     ReleaseState? releaseState,
        //     string internalBaseUrl,
        //     string internalAccessToken)
        // {
        //     DockerfileVersion = dockerfileVersion;
        //     ProductVersions = productVersion
        //         .Select(pair => pair.Split(new char[] { '=' }, 2))
        //         .ToDictionary(split => split[0].ToLower(), split => split.Skip(1).FirstOrDefault());
        //     Tools = tool;
        //     VersionSourceName = versionSourceName;
        //     Email = email;
        //     Password = password;
        //     User = user;
        //     ComputeChecksums = computeShas;
        //     ChecksumsFile = checksumsFile;
        //     UseStableBranding = stableBranding;
        //     SourceBranch = sourceBranch;
        //     InternalBaseUrl = internalBaseUrl;
        //     InternalAccessToken = internalAccessToken;

        //     // Default TargetBranch to SourceBranch if it's not explicitly provided
        //     TargetBranch = string.IsNullOrEmpty(targetBranch) ? sourceBranch : targetBranch;

        //     AzdoOrganization = org;
        //     AzdoProject = project;
        //     AzdoRepo = repo;

        //     // Special case for handling the shared dotnet product version variables.
        //     if (ProductVersions.ContainsKey("runtime"))
        //     {
        //         ProductVersions["dotnet"] = ProductVersions["runtime"];
        //     }
        //     else if (ProductVersions.ContainsKey("aspnet"))
        //     {
        //         ProductVersions["dotnet"] = ProductVersions["aspnet"];
        //     }

        //     ReleaseState = releaseState;
        // }
    }

    public enum ReleaseState
    {
        Prerelease,
        Release
    }
}
