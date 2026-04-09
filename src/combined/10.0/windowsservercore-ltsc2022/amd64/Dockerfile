# escape=`

FROM mcr.microsoft.com/windows/servercore:ltsc2022-amd64

ENV `
    # Configure web servers to bind to port 8080 when present
    ASPNETCORE_HTTP_PORTS=8080 `
    # Enable detection of running in a container
    DOTNET_RUNNING_IN_CONTAINER=true `
    # .NET Runtime version
    DOTNET_VERSION=10.0.5

RUN setx /M PATH "%PATH%;C:\Program Files\dotnet"

# Retrieve .NET Runtime
RUN powershell -Command `
        $ErrorActionPreference = 'Stop'; `
        $ProgressPreference = 'SilentlyContinue'; `
        `
        $dotnet_version = '10.0.5'; `
        $dotnet_file = 'dotnet-runtime-' + $dotnet_version + '-win-x64.zip'; `
        $dotnet_sha512_file = $dotnet_file + '.sha512-bare'; `
        `
        Invoke-WebRequest -OutFile $dotnet_file https://builds.dotnet.microsoft.com/dotnet/Runtime/$dotnet_version/$dotnet_file; `
        Invoke-WebRequest -OutFile $dotnet_sha512_file https://builds.dotnet.microsoft.com/dotnet/Runtime/$dotnet_version/$dotnet_sha512_file; `
        `
        if ((Get-FileHash $dotnet_file -Algorithm sha512).Hash -ne (Get-Content $dotnet_sha512_file)) { `
            Write-Host 'CHECKSUM VERIFICATION FAILED!'; `
            exit 1; `
        }; `
        `
        mkdir 'C:\Program Files\dotnet'; `
        tar --gzip --extract --no-same-owner --file $dotnet_file --directory 'C:\Program Files\dotnet'; `
        Remove-Item -Force `
            $dotnet_file, `
            $dotnet_sha512_file

# ASP.NET Core version
ENV ASPNET_VERSION=10.0.5

# Install ASP.NET Core Runtime
RUN powershell -Command `
        $ErrorActionPreference = 'Stop'; `
        $ProgressPreference = 'SilentlyContinue'; `
        `
        $aspnetcore_version = '10.0.5'; `
        $aspnetcore_file = 'aspnetcore-runtime-' + $aspnetcore_version + '-win-x64.zip'; `
        $dotnet_sha512_file = $aspnetcore_file + '.sha512-bare'; `
        `
        Invoke-WebRequest -OutFile $aspnetcore_file https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/$aspnetcore_version/$aspnetcore_file; `
        Invoke-WebRequest -OutFile $dotnet_sha512_file https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/$aspnetcore_version/$dotnet_sha512_file; `
        `
        if ((Get-FileHash $aspnetcore_file -Algorithm sha512).Hash -ne (Get-Content $dotnet_sha512_file)) { `
            Write-Host 'CHECKSUM VERIFICATION FAILED!'; `
            exit 1; `
        }; `
        `
        tar --gzip --extract --no-same-owner --file $aspnetcore_file --directory 'C:\Program Files\dotnet' ./shared/Microsoft.AspNetCore.App; `
        Remove-Item -Force `
            $aspnetcore_file, `
            $dotnet_sha512_file
