// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#nullable enable

namespace Microsoft.DotNet.Docker.Tests;

public static class DockerCliExtensions
{
    public static void Copy(this IDockerCli docker, string src, string dest) =>
        docker.Execute($"cp {src} {dest}");

    public static void DeleteImage(this IDockerCli docker, string tag)
    {
        if (DockerHelper.ImageExists(tag))
        {
            docker.Execute($"image rm -f {tag}");
        }
    }

    public static void DeleteContainer(this IDockerCli docker, string container, bool captureLogs = false)
    {
        if (DockerHelper.ContainerExists(container))
        {
            if (captureLogs)
            {
                docker.Execute($"logs {container}", new DockerCliRunOptions(IgnoreErrors: true));
            }

            // If a container is already stopped, running `docker stop` again has no adverse effects.
            // This prevents some issues where containers could fail to be forcibly removed while they're running.
            // e.g. https://github.com/dotnet/dotnet-docker/issues/5127
            docker.StopContainer(container);

            docker.Execute($"container rm -f {container}");
        }
    }

    public static void StopContainer(this IDockerCli docker, string container)
    {
        if (DockerHelper.ContainerExists(container))
        {
            docker.Execute($"stop {container}", new DockerCliRunOptions(AutoRetry: true));
        }
    }

    public static string GetImageUser(this IDockerCli docker, string image) =>
        docker.Execute($"inspect -f \"{{{{ .Config.User }}}}\" {image}");

    public static IDictionary<string, string> GetEnvironmentVariables(this IDockerCli docker, string image)
    {
        string envVarsStr = docker.Execute($"inspect -f \"{{{{json .Config.Env }}}}\" {image}");
        JArray envVarsArray = (JArray)JsonConvert.DeserializeObject(envVarsStr)!;
        return envVarsArray
            .ToDictionary(
                item => item.ToString().Split('=')[0],
                item => item.ToString().Split('=')[1]);
    }

    public static string GetContainerAddress(this IDockerCli docker, string container)
    {
        string containerAddress = docker.Execute("inspect -f \"{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}\" " + container);
        if (string.IsNullOrWhiteSpace(containerAddress))
        {
            containerAddress = docker.Execute("inspect -f \"{{.NetworkSettings.Networks.nat.IPAddress }}\" " + container);
        }

        return containerAddress;
    }

    public static string GetContainerHostPort(this IDockerCli docker, string container, int containerPort = 80) =>
        docker.Execute(
            $"inspect -f \"{{{{(index (index .NetworkSettings.Ports \\\"{containerPort}/tcp\\\") 0).HostPort}}}}\" {container}");

    public static void Pull(this IDockerCli docker, string image) =>
        docker.Execute($"pull {image}", new DockerCliRunOptions(AutoRetry: true));

    /// <summary>
    /// Pulls an image from DockerHub, optionally redirecting it through a cache registry.
    /// </summary>
    /// <param name="docker">The Docker CLI instance.</param>
    /// <param name="image">
    /// The image to pull, in the format "repo:tag". Since the image is
    /// assumed to be from DockerHub, do not include a registry.
    /// </param>
    /// <returns>
    /// A tag for the image that was pulled. Use this value to refer to the
    /// image in subsequent operations. Do not use the original value of
    /// <paramref name="image"/>.
    /// </returns>
    public static string PullDockerHubImage(this IDockerCli docker, string image)
    {
        if (!string.IsNullOrEmpty(Config.CacheRegistry))
        {
            image = $"{Config.CacheRegistry}/{image}";
        }

        docker.Pull(image);
        return image;
    }

    public static string GetHistory(this IDockerCli docker, string image) =>
        docker.Execute($"history --no-trunc --format \"{{{{ .CreatedBy }}}}\" {image}");

    public static string Run(
        this IDockerCli docker,
        string image,
        string name,
        string? command = null,
        string? workdir = null,
        string? optionalRunArgs = null,
        bool detach = false,
        string? runAsUser = null,
        bool skipAutoCleanup = false,
        bool useMountedDockerSocket = false,
        bool silenceOutput = false,
        bool tty = true)
    {
        string cleanupArg = skipAutoCleanup ? string.Empty : " --rm";
        string detachArg = detach ? " -d" : string.Empty;
        string ttyArg = detach && tty ? " -t" : string.Empty;
        string userArg = runAsUser is not null ? $" -u {runAsUser}" : string.Empty;
        string workdirArg = workdir is null ? string.Empty : $" -w {workdir}";
        string mountedDockerSocketArg = useMountedDockerSocket ? " -v /var/run/docker.sock:/var/run/docker.sock" : string.Empty;
        return docker.Execute(
            $"run --name {name}{cleanupArg}{workdirArg}{userArg}{detachArg}{ttyArg}{mountedDockerSocketArg} {optionalRunArgs} {image} {command}",
            new DockerCliRunOptions(LogOutput: !silenceOutput));
    }

    /// <summary>
    /// Creates a file system volume that is backed by memory instead of disk.
    /// </summary>
    public static string CreateTmpfsVolume(this IDockerCli docker, string name, int? ownerUid = null)
    {
        // Create volume using the local driver (the default driver),
        // which accepts options similar to the 'mount' command.
        //
        // Additional options are specified to:
        // - make this volume an in-memory file system with a unique device name (type=tmpfs, device={guid}}).
        // - to set the owner of the root of the file system (o=uid=101).
        string optionalArgs = string.Empty;
        if (ownerUid.HasValue)
        {
            optionalArgs += $" --opt o=uid={ownerUid.Value}";
        }
        string device = Guid.NewGuid().ToString("D");
        return docker.Execute($"volume create --opt type=tmpfs --opt device={device}{optionalArgs} {name}");
    }

    public static string DeleteVolume(this IDockerCli docker, string name) =>
        docker.Execute($"volume remove {name}");
}
