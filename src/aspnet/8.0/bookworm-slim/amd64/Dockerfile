ARG BASE_IMAGE

# Installer image
FROM amd64/buildpack-deps:bookworm-curl AS installer

# Copy ../aspnet.tar.gz to ./dotnet.tar.gz in the docker container
COPY dotnet-runtime-composite-8.0.100-dev-linux-x64.tar.gz dotnet.tar.gz

# Retrieve ASP.NET composite runtime
RUN aspnetcore_version=8.0.0-preview.4.23203.12 \
    # && curl -fSL --output dotnet.tar.gz https://dotnetbuilds.azureedge.net/public/aspnetcore/Runtime/$aspnetcore_version/aspnetcore-runtime-composite-$aspnetcore_version-linux-x64.tar.gz \
    # && dotnet_sha512='35c71b03a14be3782bff91add140259d7205708d01cd69dd2b1a88819812b1998ebe18f76ef669f18a19349299bec23655ffa5705ece9167b94a8c65f526a08d' \
    # && echo "$dotnet_sha512  dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p /dotnet \
    && tar -oxzf dotnet.tar.gz -C /dotnet \
    && rm dotnet.tar.gz


# ASP.NET composite runtime image
FROM $BASE_IMAGE

# .NET Runtime version
ENV DOTNET_VERSION=8.0.0-preview.4.23181.2
# ASP.NET Core version
ENV ASPNET_VERSION=8.0.0-preview.4.23203.12

COPY --from=installer ["/dotnet", "/usr/share/dotnet"]

RUN ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
