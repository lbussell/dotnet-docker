// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
//

namespace Microsoft.DotNet.Docker.Model.Manifest;

public record TagSyndication(string Repo, string[] DestinationTags);

// using System.ComponentModel;

// namespace Microsoft.DotNet.ImageBuilder.Model.Manifest
// {
//     [Description(
//         "A description of where a tag should be syndicated to."
//         )]
//     public class TagSyndication
//     {
//         [Description(
//             "Name of the repo to syndicate the tag to."
//         )]
//         public string Repo { get; set; }
//
//         [Description(
//             "List of destination tag names to syndicate the tag to."
//         )]
//         public string[] DestinationTags { get; set; }
//     }
// }
