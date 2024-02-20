// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.DotNet.Docker.ManifestGenerator.Cli;

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.DotNet.Docker.Model.Manifest;

public class Program
{
    public static void Main(string[] args)
    {
        // RunTest();
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
