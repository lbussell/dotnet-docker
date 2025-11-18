ARG REPO=mcr.microsoft.com/dotnet/runtime-deps
ARG RUNTIME_VERSION=10.0.0

# Installer image
FROM $REPO:$RUNTIME_VERSION-alpine3.22-amd64 AS installer
ARG RUNTIME_VERSION

# Retrieve .NET Runtime
RUN dotnet_file="dotnet-runtime-$RUNTIME_VERSION-linux-musl-x64.tar.gz" \
    && dotnet_url="https://builds.dotnet.microsoft.com/dotnet/Runtime/$RUNTIME_VERSION/$dotnet_file" \
    && wget $dotnet_url \
    && wget -qO- $dotnet_url.sha512 | sha512sum -c - \
    && mkdir --parents /dotnet \
    && tar --gzip --extract --no-same-owner --file $dotnet_file --directory /dotnet \
    && rm $dotnet_file

# .NET runtime image
FROM $REPO:$RUNTIME_VERSION-alpine3.22-amd64
ARG RUNTIME_VERSION

# .NET Runtime version
ENV DOTNET_VERSION=$RUNTIME_VERSION

COPY --from=installer ["/dotnet", "/usr/share/dotnet"]

RUN ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
