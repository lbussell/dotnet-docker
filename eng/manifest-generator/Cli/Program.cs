// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.DotNet.Docker.ManifestGenerator.Cli;

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Linq;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Microsoft.DotNet.Docker.Model.Manifest;

public class Program
{
    public static void Main(string[] args)
    {
        string manifestIn = "C:\\s\\dotnet-docker\\manifest.json";
        string manifestOut = "C:\\s\\dotnet-docker\\manifest.gen.json";

        JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        string input = File.ReadAllText(manifestIn);

        Manifest manifest = JsonSerializer.Deserialize<Manifest>(input, jsonOptions)
            ?? throw new Exception("Failed to deserialize manifest");

        manifest = Add(manifest, "noble", "jammy", "cbl-mariner2.0", "8.0");
        manifest = Replace(manifest, "noble", "jammy", "cbl-mariner2.0", "9.0");

        string output = JsonSerializer.Serialize(manifest, jsonOptions);
        File.WriteAllText(manifestOut, output);
    }

    public static void RunTest()
    {
        string manifestIn = "C:\\s\\dotnet-docker\\manifest.json";
        string manifestOut = "C:\\s\\dotnet-docker\\manifest.gen.json";
        string manifestTestOut = "C:\\s\\dotnet-docker\\manifest.test.json";

        JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        File.WriteAllText(manifestTestOut, TestManifest(jsonOptions));

        string input = File.ReadAllText(manifestIn);

        Manifest manifest = JsonSerializer.Deserialize<Manifest>(input, jsonOptions)
            ?? throw new Exception("Failed to deserialize manifest");

        string output = JsonSerializer.Serialize(manifest, jsonOptions);
        File.WriteAllText(manifestOut, output);
    }

    public static Manifest Add(
        Manifest manifest,
        string newOsVersion,
        string copyFromOsVersion,
        string insertBeforeOs,
        string dotnetVersion)
    {
        List<Repo> newRepos = [];
        foreach (var repo in manifest.Repos)
        {
            List<Image> newImages = [];
            foreach (Image image in repo.Images)
            {
                if (image.SharedTags != null && !image.SharedTags.Select(t => t.Key).First().Contains(dotnetVersion))
                {
                    continue;
                }

                string osVersion = image.Platforms[0].OsVersion;

                if (osVersion.Contains(copyFromOsVersion))
                {
                    List<Platform> newPlatforms = [];
                    foreach (Platform platform in image.Platforms)
                    {
                        // Replace the tag key with the new os version
                        Dictionary<string, Tag> newTags = [];
                        foreach (var (key, tag) in platform.Tags)
                        {
                            newTags.Add(key.Replace(copyFromOsVersion, newOsVersion), tag);
                        }

                        newPlatforms.Add(platform with
                            {
                                Dockerfile = platform.Dockerfile.Replace(copyFromOsVersion, newOsVersion),
                                OsVersion = platform.OsVersion.Replace(copyFromOsVersion, newOsVersion),
                                Tags = newTags,
                            });
                    }

                    Dictionary<string, Tag> newSharedTags = [];
                    if (image.SharedTags != null)
                    {
                        foreach (var (key, tag) in image.SharedTags)
                        {
                            newSharedTags.Add(key.Replace(copyFromOsVersion, newOsVersion), tag);
                        }
                    }

                    newImages.Add(image with
                    {
                        Platforms = [..newPlatforms],
                        SharedTags = newSharedTags,
                    });
                }
            }

            int index = 0;
            for (int i = 0; i < repo.Images.Length; i++)
            {
                Image image = repo.Images[i];

                if (image.SharedTags != null && !image.SharedTags.Select(t => t.Key).First().Contains(dotnetVersion))
                {
                    continue;
                }

                if (image.Platforms[0].OsVersion == insertBeforeOs)
                {
                    index = i;
                }
            }

            List<Image> before = repo.Images.Take(index).ToList();
            List<Image> after = repo.Images.Skip(index).ToList();

            newRepos = [..newRepos, repo with
            {
                // Images = [..repo.Images, ..newImages],
                Images = [..before, ..newImages, ..after],
            }];
        }

        return manifest with { Repos = [..newRepos] };
    }

    public static Manifest Replace(
        Manifest manifest,
        string newOsVersion,
        string copyFromOsVersion,
        string insertBeforeOs,
        string dotnetVersion)
    {
        for (var r = 0; r < manifest.Repos.Length; r += 1)
        {
            for (var i = 0; i < manifest.Repos[r].Images.Length; i += 1)
            {
                if (manifest.Repos[r].Images[i].SharedTags != null
                    && !manifest.Repos[r].Images[i].SharedTags!
                        .Select(t => t.Key).First().Contains(dotnetVersion))
                {
                    continue;
                }

                if (manifest.Repos[r].Images[i].Platforms[0].OsVersion.Contains(copyFromOsVersion))
                {
                    Dictionary<string, Tag> newSharedTags = [];
                    if (manifest.Repos[r].Images[i].SharedTags != null)
                    {
                        foreach (var (key, tag) in manifest.Repos[r].Images[i].SharedTags!)
                        {
                            newSharedTags.Add(key.Replace(copyFromOsVersion, newOsVersion), tag);
                        }
                    }

                    manifest.Repos[r].Images[i].SharedTags = newSharedTags;

                    for (var p = 0; p < manifest.Repos[r].Images[i].Platforms.Length; p += 1)
                    {
                        var platform = manifest.Repos[r].Images[i].Platforms[p];

                        manifest.Repos[r].Images[i].Platforms[p].OsVersion = newOsVersion;

                        // Replace the tag key with the new os version
                        Dictionary<string, Tag> newTags = [];
                        foreach (var (key, tag) in platform.Tags)
                        {
                            newTags.Add(key.Replace(copyFromOsVersion, newOsVersion), tag);
                        }

                        manifest.Repos[r].Images[i].Platforms[p] = platform with
                            {
                                Dockerfile = platform.Dockerfile.Replace(copyFromOsVersion, newOsVersion),
                                OsVersion = platform.OsVersion.Replace(copyFromOsVersion, newOsVersion),
                                Tags = newTags,
                            };
                    }
                }
            }
        }

        return manifest;
    }

    public static string TestManifest(JsonSerializerOptions jso)
    {
        Manifest m = new()
        {
            Includes = [],
            Readme = new Readme(
                Path: "readme.md", "bar"),
            Registry = "foo",
            Repos = [
                new Repo()
                {
                    Name = "myrepo",
                    Id = "myrepo",
                    Images = [
                        new Image()
                        {
                            Platforms = [
                                new Platform()
                                {
                                    BuildArgs = new Dictionary<string, string>() { },
                                    Dockerfile = "Dockerfile",
                                    DockerfileTemplate = "myTemplate",
                                    Tags = new Dictionary<string, Tag>() { },
                                    OS = OS.Linux,
                                    OsVersion = "bullseye",
                                    CustomBuildLegGroups = [],
                                    Variant = "blah",
                                }
                            ],

                            // SharedTags = new Dictionary<string, Tag>() { },
                            ProductVersion = "1.0.0",
                        }
                    ],
                    Readmes = [],
                    McrTagsMetadataTemplate = "myMcrTemplate",
                },
            ],
            Variables = new Dictionary<string, string>(),
        };

        return JsonSerializer.Serialize(m, jso);
    }
}
