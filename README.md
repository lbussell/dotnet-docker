# Featured Repos

* [dotnet/sdk](https://hub.docker.com/_/microsoft-dotnet-sdk/): .NET SDK
* [dotnet/aspnet](https://hub.docker.com/_/microsoft-dotnet-aspnet/): ASP.NET Core Runtime
* [dotnet/runtime](https://hub.docker.com/_/microsoft-dotnet-runtime/): .NET Runtime
* [dotnet/runtime-deps](https://hub.docker.com/_/microsoft-dotnet-runtime-deps/): .NET Runtime Dependencies
* [dotnet/monitor](https://hub.docker.com/_/microsoft-dotnet-monitor/): .NET Monitor Tool
* [dotnet/samples](https://hub.docker.com/_/microsoft-dotnet-samples/): .NET Samples

# About

[.NET](https://docs.microsoft.com/dotnet/core/) is a general purpose development platform maintained by Microsoft and the .NET community on [GitHub](https://github.com/dotnet/core). It is cross-platform, supports Windows, macOS, and Linux, and can be used in device, cloud, and embedded/IoT scenarios.

.NET has several capabilities that make development productive, including automatic memory management, (runtime) generic types, reflection, [asynchronous constructs](https://learn.microsoft.com/dotnet/csharp/async), concurrency, and native interop. Millions of developers take advantage of these capabilities to efficiently build high-quality applications.

You can use C# or F# to write .NET apps.

- [C#](https://docs.microsoft.com/dotnet/csharp/) is powerful, type-safe, and object-oriented while retaining the expressiveness and elegance of C-style languages. Anyone familiar with C and similar languages will find it straightforward to write in C#.
- [F#](https://docs.microsoft.com/dotnet/fsharp/) is a cross-platform, open-source, functional programming language for .NET. It also includes object-oriented and imperative programming.

[.NET](https://github.com/dotnet/core) is open source (MIT and Apache 2 licenses) and was contributed to the [.NET Foundation](http://dotnetfoundation.org) by Microsoft in 2014. It can be freely adopted by individuals and companies, including for personal, academic or commercial purposes. Multiple companies use .NET as part of apps, tools, new platforms and hosting services.

You are invited to [contribute new features](https://github.com/dotnet/core/blob/master/CONTRIBUTING.md), fixes, or updates, large or small; we are always thrilled to receive pull requests, and do our best to process them as fast as we can.

> https://docs.microsoft.com/dotnet/core/

Watch [discussions](https://github.com/dotnet/dotnet-docker/discussions/categories/announcements) for Docker-related .NET announcements.

## New: Ubuntu Chiseled images

Ubuntu Chiseled .NET images are a type of "distroless" container image that contain only the minimal set of packages .NET needs, with everything else removed.
These images offer dramatically smaller deployment sizes and attack surface by including only the minimal set of packages required to run .NET applications.

Please see the [Ubuntu Chiseled + .NET](https://github.com/dotnet/dotnet-docker/blob/main/documentation/ubuntu-chiseled.md) documentation page for more info.

# Usage

The [.NET Docker samples](https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md) show various ways to use .NET and Docker together. See [Building Docker Images for .NET Applications](https://docs.microsoft.com/dotnet/core/docker/building-net-docker-images) to learn more.

## Container sample: Run a simple application

You can quickly run a container with a pre-built [.NET Docker image](https://hub.docker.com/_/microsoft-dotnet-samples/), based on the [.NET console sample](https://github.com/dotnet/dotnet-docker/blob/main/samples/dotnetapp/README.md).

Type the following command to run a sample console application:

```console
docker run --rm mcr.microsoft.com/dotnet/samples
```

## Container sample: Run a web application

You can quickly run a container with a pre-built [.NET Docker image](https://hub.docker.com/_/microsoft-dotnet-samples/), based on the [ASP.NET Core sample](https://github.com/dotnet/dotnet-docker/blob/main/samples/aspnetapp/README.md).

Type the following command to run a sample web application:

```console
docker run -it --rm -p 8000:8080 --name aspnetcore_sample mcr.microsoft.com/dotnet/samples:aspnetapp
```

After the application starts, navigate to `http://localhost:8000` in your web browser. You can also view the ASP.NET Core site running in the container from another machine with a local IP address such as `http://192.168.1.18:8000`.

> Note: ASP.NET Core apps (in official images) listen to [port 8080 by default](https://github.com/dotnet/dotnet-docker/blob/6da64f31944bb16ecde5495b6a53fc170fbe100d/src/runtime-deps/8.0/bookworm-slim/amd64/Dockerfile#L7), starting with .NET 8. The [`-p` argument](https://docs.docker.com/engine/reference/commandline/run/#publish) in these examples maps host port `8000` to container port `8080` (`host:container` mapping). The container will not be accessible without this mapping. ASP.NET Core can be [configured to listen on a different or additional port](https://learn.microsoft.com/aspnet/core/fundamentals/servers/kestrel/endpoints).

See [Hosting ASP.NET Core Images with Docker over HTTPS](https://github.com/dotnet/dotnet-docker/blob/main/samples/host-aspnetcore-https.md) to use HTTPS with this image.

## Tag Formatting

### .NET Versions

All .NET container images have both "fixed version" and "floating version" tags.
Floating version tags will always reference the latest version of a specific .NET major version, while fixed version tags will always only reference a specific patch version.
For all tags below, `<.NET Version>` can be substituted for either `<Major.Minor>` or `<Major.Minor.Patch>`, for example: `7.0` or `7.0.12`.

### Single-platform tags

These "fixed version" tags reference an image with a specific .NET version for a specific operating system and architecture.

- `<.NET Version>-<OS>-<Architecture>`
- `<.NET Version>-<OS>-<variant>-<Architecture>`
- `<.NET Version>-<OS>-<Architecture>`
- `<.NET Version>-<OS>-<variant>-<Architecture>`

### Multi-platform tags

These tags reference images for [multiple platforms](https://docs.docker.com/build/building/multi-platform/).

- `<.NET Version>`
    - The version-only floating tag refers to the latest Debian version available at the .NET Major Version's release.
- `<.NET Version>-<OS>`
- `<.NET Version>-<OS>-<variant>`

### Image Variants

By default, Ubuntu and Debian images for .NET 8 will have both `icu` and `tzdata` installed.
These images are intended to satisfy the most common use cases of .NET developers.

Our Alpine and Ubuntu Chiseled images are focused on size.
These images do not include `icu` or `tzdata`, meaning that these images only work with apps that are configured for [globalization-invariant mode](https://learn.microsoft.com/dotnet/core/runtime-config/globalization).
Apps that require globalization support can use the `extra` image variant of the [dotnet/runtime-deps](https://hub.docker.com/_/microsoft-dotnet-runtime-deps/) images. 

#### `extra`

The `extra` image variant is offered alongside our size-focused base images for self-contained or single file apps that depend on globalization functionality.
Extra images contain everything that the default images do, plus `icu` and `tzdata`.

#### `composite`

Compared to the default ASP.NET images, ASP.NET Composite images provide a smaller image size on disk as well as performance improvements for framework-dependent ASP.NET apps by performing some cross-assembly optimizations and between the .NET and ASP.NET runtimes.
However, this means that apps run on the ASP.NET Composite runtime cannot use handpicked custom versions of .NET or ASP.NET assemblies that are built into the image.

#### (Preview) `aot`

`aot` images provide an optimized deployment size for [native AOT](https://learn.microsoft.com/dotnet/core/deploying/native-aot/) compiled .NET apps.
Native AOT has the lowest size, startup time, and memory footprint of all .NET deployment models.
Please see ["Limiatations of Native AOT deployment"](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot#limitations-of-native-aot-deployment) to see if your app might be compatible.
`aot` image variants are only available for our size-focused `runtime-deps` images: Alpine and Ubuntu Chiseled.
They also require the use of the `aot` SDK image which include extra libraries needed for Native AOT compilation.

**Note:** `aot` images are only available as a preview in the [dotnet/nightly/sdk](https://hub.docker.com/_/microsoft-dotnet-nightly-sdk/) and [dotnet/nightly/runtime-deps](https://hub.docker.com/_/microsoft-dotnet-nightly-runtime-deps/) repos.
Native AOT compiled apps will function exactly the same on the existing `runtime-deps` (non-`aot`) images, but with a larger deployment size.
Please try out these new, smaller images and give us feedback!

# Related Repositories

.NET:

* [dotnet/nightly/sdk](https://hub.docker.com/_/microsoft-dotnet-nightly-sdk/): .NET SDK (Preview)
* [dotnet/nightly/aspnet](https://hub.docker.com/_/microsoft-dotnet-nightly-aspnet/): ASP.NET Core Runtime (Preview)
* [dotnet/nightly/runtime](https://hub.docker.com/_/microsoft-dotnet-nightly-runtime/): .NET Runtime (Preview)
* [dotnet/nightly/runtime-deps](https://hub.docker.com/_/microsoft-dotnet-nightly-runtime-deps/): .NET Runtime Dependencies (Preview)
* [dotnet/nightly/monitor](https://hub.docker.com/_/microsoft-dotnet-nightly-monitor/): .NET Monitor Tool (Preview)

.NET Framework:

* [dotnet/framework](https://hub.docker.com/_/microsoft-dotnet-framework/): .NET Framework, ASP.NET and WCF
* [dotnet/framework/samples](https://hub.docker.com/_/microsoft-dotnet-framework-samples/): .NET Framework, ASP.NET and WCF Samples

# Support

## Lifecycle

* [Microsoft Support for .NET](https://github.com/dotnet/core/blob/main/support.md)
* [Supported Container Platforms Policy](https://github.com/dotnet/dotnet-docker/blob/main/documentation/supported-platforms.md)
* [Supported Tags Policy](https://github.com/dotnet/dotnet-docker/blob/main/documentation/supported-tags.md)

## Image Update Policy

* We update the supported .NET images within 12 hours of any updates to their base images (e.g. debian:buster-slim, windows/nanoserver:ltsc2022, buildpack-deps:bionic-scm, etc.).
* We publish .NET images as part of releasing new versions of .NET including major/minor and servicing.

## Feedback

* [File an issue](https://github.com/dotnet/dotnet-docker/issues/new/choose)
* [Contact Microsoft Support](https://support.microsoft.com/contactus/)

# License

* Legal Notice: [Container License Information](https://aka.ms/mcr/osslegalnotice)
* [.NET license](https://github.com/dotnet/dotnet-docker/blob/main/LICENSE)
* [Discover licensing for Linux image contents](https://github.com/dotnet/dotnet-docker/blob/main/documentation/image-artifact-details.md)
* [Windows base image license](https://docs.microsoft.com/virtualization/windowscontainers/images-eula) (only applies to Windows containers)
* [Pricing and licensing for Windows Server 2019](https://www.microsoft.com/cloud-platform/windows-server-pricing)
